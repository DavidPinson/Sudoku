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

    // public static Task<bool> FindNakedSingleAsync(Cell cell)
    // {
    //   return Task.Run(() =>
    //   {
    //     if(cell.CurrentValue == 0 && cell.PossibleValue.Count == 1)
    //     {
    //       cell.CurrentValue = cell.PossibleValue[0];
    //       return true;
    //     }
    //     return false;
    //   });
    // }
    // public Task<bool> FindHiddenSingleAsync()
    // {
    //   // pour que cette méthode fonctione l'hypothèse selon laquelle toutes les "possibles values"
    //   // de toutes les cases sont trouvées est posée.

    //   return Task.Run(() =>
    //   {
    //     bool atLeastOneHiddenSingleHasBeenFound = false;
    //     Dictionary<int, List<int>> hiddenSingles = new();

    //     ConstraintCellPos.Houses.ForEach(l =>
    //     {
    //       hiddenSingles.Clear();
    //       l.ForEach(index =>
    //       {
    //         if(Cells[index].CurrentValue == 0)
    //         {
    //           Cells[index].PossibleValue.ForEach(val =>
    //           {
    //             if(hiddenSingles.ContainsKey(val) == false)
    //             {
    //               hiddenSingles[val] = new List<int>();
    //             }

    //             hiddenSingles[val].Add(index);
    //           });
    //         }
    //       });

    //       Tuple<int, int> hiddenSingle = hiddenSingles
    //         .Where(kvp => kvp.Value.Count == 1)
    //         .Select(kvp => new Tuple<int, int>(kvp.Value[0], kvp.Key))
    //         .FirstOrDefault();

    //       if(hiddenSingle != default(Tuple<int, int>))
    //       {
    //         Cells[hiddenSingle.Item1].CurrentValue = hiddenSingle.Item2;
    //         Cells[hiddenSingle.Item1].PossibleValue.Clear();
    //         atLeastOneHiddenSingleHasBeenFound = true;

    //         ConstraintCellPos.Board[hiddenSingle.Item1].ForEach(i => Cells[i].PossibleValue.Remove(hiddenSingle.Item2));
    //       }
    //     });

    //     return atLeastOneHiddenSingleHasBeenFound;
    //   });
    // }
    // public Task<bool> IsAllConstraintsRespectedAsync()
    // {
    //   return Task.Run(() =>
    //   {
    //     List<int> houseValues = new();

    //     foreach(List<int> l in ConstraintCellPos.Houses)
    //     {
    //       houseValues.Clear();
    //       l.ForEach(val => houseValues.Add(Cells[val].CurrentValue));

    //       if(ConstraintCellPos.AllPossibleValues.Except(houseValues).Any() == true)
    //       {
    //         return false;
    //       }
    //     }

    //     return true;
    //   });
    // }
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

    public async Task PushAsync()
    {
      List<Cell> values = new(81);

      Task AddCellToList(Cell cell)
      {
        return Task.Run(() =>
        {
          values.Add(new Cell(cell));
        });
      }

      await Cells.ParallelForEachAsync(AddCellToList, _degreeOfParallelism).ConfigureAwait(false);

      _stack.Push(values);
    }
    public async Task PopAsync()
    {
      List<Cell> values = _stack.Pop();

      Task AdjustCellInBoardCells(Cell cell)
      {
        return Task.Run(() =>
        {
          Cells[cell.Index].CurrentValue = cell.CurrentValue;
          Cells[cell.Index].PossibleValue = cell.PossibleValue;
        });
      }

      await values.ParallelForEachAsync(AdjustCellInBoardCells, _degreeOfParallelism).ConfigureAwait(false);
    }

  }
}
