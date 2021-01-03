using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.ParallelDP
{
  public class Board
  {
    private readonly int _degreeOfParallelism;
    private readonly Stack<List<Cell>> _stack = new();

    public List<Cell> Cells { get; private set; }
    public List<Cell> this[int index]
    {
      get
      {
        if(index <= 0 && index <= 8)
        {
          return Cells.Skip(index * 9).Take(9).ToList();
        }
        else
        {
          throw new IndexOutOfRangeException($"Index({index}) must be between 0 and 8 inclusive.");
        }
      }
    }

    public Board(int degreeOfParallelism = 8)
    {
      Cells = new List<Cell>(81);
      _degreeOfParallelism = degreeOfParallelism;
    }

    public async Task<bool> FindHiddenSingleAndPropagateAsync()
    {
      bool atLeastOneHiddenSingleHasBeenFound = false;

      Task<bool> FindHiddenSingleAndPropagateForHouseAsync(List<int> houseIndexes)
      {
        // pour que cette méthode fonctione l'hypothèse selon laquelle toutes les "possibles values"
        // de toutes les cases sont trouvées est posée.
        return Task.Run(() =>
        {
          Dictionary<int, List<int>> hiddenSingles = new();

          houseIndexes.ForEach(index =>
          {
            if(Cells[index].CurrentValue == 0)
            {
              foreach(KeyValuePair<int, byte> kvp in Cells[index].PossibleValue)
              {
                if(hiddenSingles.ContainsKey(kvp.Key) == false)
                {
                  hiddenSingles[kvp.Key] = new List<int>();
                }

                hiddenSingles[kvp.Key].Add(index);
              }
            }
          });

          Tuple<int, int> hiddenSingle = hiddenSingles
            .Where(kvp => kvp.Value.Count == 1)
            .Select(kvp => new Tuple<int, int>(kvp.Value[0], kvp.Key))
            .FirstOrDefault();

          if(hiddenSingle != default(Tuple<int, int>))
          {
            AssignCellValue(hiddenSingle.Item1, hiddenSingle.Item2);
            return true;
          }
          return false;
        });
      }

      List<Task<bool>> houses = new();
      ConstraintCellPos.Houses.ForEach(l => houses.Add(FindHiddenSingleAndPropagateForHouseAsync(l)));

      await Task.WhenAll(houses).ConfigureAwait(false);

      houses.ForEach(async (t) => atLeastOneHiddenSingleHasBeenFound = await t.ConfigureAwait(false));

      return atLeastOneHiddenSingleHasBeenFound;
    }

    public async Task GetPossibleCellValuesAsync()
    {
      Task GetPossibleCellValuesForCell(Cell cell)
      {
        return Task.Run(() =>
        {
          if(cell.CurrentValue == 0)
          {
            HashSet<int> usedValues = new();
            ConstraintCellPos.Board[cell.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));
            cell.PossibleValue.Clear();
            cell.PossibleValue.AddRange(ConstraintCellPos.AllPossibleValues.Except(usedValues));
          }
        });
      }

      await Cells.ParallelForEachAsync(GetPossibleCellValuesForCell, _degreeOfParallelism).ConfigureAwait(false);
    }
    public async Task FindNakedSingleAndPropagateAsync()
    {
      Task CheckNakedSingleAndPropagateAsync(Cell cell)
      {
        return Task.Run(() =>
        {
          if(cell.CurrentValue == 0 && cell.PossibleValue.Count == 1)
          {
            AssignCellValue(cell.Index, cell.PossibleValue.FirstOrDefault().Key);
          }
        });
      }

      await Cells.ParallelForEachAsync(CheckNakedSingleAndPropagateAsync, _degreeOfParallelism).ConfigureAwait(false);
    }
    private void AssignCellValue(int cellIndex, int cellValue)
    {
      if(cellValue != 0)
      {
        Cells[cellIndex].CurrentValue = cellValue;
        Cells[cellIndex].PossibleValue.Clear();
        ConstraintCellPos.Board[cellIndex].ForEach(i =>
        {
          if(Cells[i].PossibleValue.TryRemove(cellValue, out byte val) == true)
          {
            if(Cells[i].PossibleValue.Count == 1)
            {
              AssignCellValue(i, Cells[i].PossibleValue.FirstOrDefault().Key);
            }
          }
        });
      }
    }
    public async Task PushAsync()
    {
      List<Cell> values = new(81);

      Task AddCellToListAsync(Cell cell)
      {
        return Task.Run(() =>
        {
          values.Add(new Cell(cell));
        });
      }

      await Cells.ParallelForEachAsync(AddCellToListAsync, _degreeOfParallelism).ConfigureAwait(false);

      _stack.Push(values);
    }
    public async Task PopAsync()
    {
      List<Cell> values = _stack.Pop();

      Task AdjustCellInBoardCellsAsync(Cell cell)
      {
        return Task.Run(() =>
        {
          Cells[cell.Index].CurrentValue = cell.CurrentValue;
          Cells[cell.Index].PossibleValue = cell.PossibleValue;
        });
      }

      await values.ParallelForEachAsync(AdjustCellInBoardCellsAsync, _degreeOfParallelism).ConfigureAwait(false);
    }

  }
}
