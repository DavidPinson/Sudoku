using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.DP
{
  public class Board
  {
    private readonly Stack<List<int>> _stack = new();

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

    public Board()
    {
      Cells = new List<Cell>(81);
    }

    public static Task<bool> FindNakedSingleAsync(Cell cell)
    {
      return Task.Run(() =>
      {
        if(cell.CurrentValue == 0 && cell.PossibleValue.Count == 1)
        {
          cell.CurrentValue = cell.PossibleValue[0];
          return true;
        }
        return false;
      });
    }
    public Task<bool> FindHiddenSingleAsync()
    {
      // pour que cette méthode fonctione l'hypothèse selon laquelle toutes les "possibles values"
      // de toutes les cases sont trouvées est posée.

      return Task.Run(() =>
      {
        bool atLeastOneHiddenSingleHasBeenFound = false;
        Dictionary<int, List<int>> hiddenSingles = new();

        ConstraintCellPos.Houses.ForEach(l =>
        {
          hiddenSingles.Clear();
          l.ForEach(index =>
          {
            if(Cells[index].CurrentValue == 0)
            {
              Cells[index].PossibleValue.ForEach(val =>
              {
                if(hiddenSingles.ContainsKey(val) == false)
                {
                  hiddenSingles[val] = new List<int>();
                }

                hiddenSingles[val].Add(index);
              });
            }
          });

          Tuple<int, int> hiddenSingle = hiddenSingles
            .Where(kvp => kvp.Value.Count == 1)
            .Select(kvp => new Tuple<int, int>(kvp.Value[0], kvp.Key))
            .FirstOrDefault();

          if(hiddenSingle != default(Tuple<int, int>))
          {
            Cells[hiddenSingle.Item1].CurrentValue = hiddenSingle.Item2;
            Cells[hiddenSingle.Item1].PossibleValue.Clear();
            atLeastOneHiddenSingleHasBeenFound = true;

            ConstraintCellPos.Board[hiddenSingle.Item1].ForEach(i => Cells[i].PossibleValue.Remove(hiddenSingle.Item2));
          }
        });

        return atLeastOneHiddenSingleHasBeenFound;
      });
    }
    public Task<bool> IsAllConstraintsRespectedAsync(Cell cell)
    {
      return Task.Run(() =>
      {
        HashSet<int> usedValues = new();

        ConstraintCellPos.Board[cell.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));

        return usedValues.Add(cell.CurrentValue);
      });
    }
    public Task GetPossibleCellValuesAsync(Cell cell)
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

    public void Push()
    {
      List<int> values = new(81);
      Cells.ForEach(c => values.Add(c.CurrentValue));
      _stack.Push(values);
    }
    public void Pop()
    {
      List<int> values = _stack.Pop();
      for(int i = 0; i < 81; i++)
      {
        Cells[i].CurrentValue = values[i];
      }
    }

  }
}
