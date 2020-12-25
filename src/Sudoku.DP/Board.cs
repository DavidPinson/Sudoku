using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.DP
{
  public class Board
  {
    private Stack<List<Tuple<int, List<int>>>> _stack = new Stack<List<Tuple<int, List<int>>>>();

    public List<ICell> Cells { get; private set; }
    public List<ICell> this[int index]
    {
      get
      {
        if (index <= 0 && index <= 8)
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
      Cells = new List<ICell>(81);
    }

    public Task<bool> ComputeKnownMovesAsync(ICell cell)
    {
      return Task.Run(() =>
      {
        if (cell.CurrentValue == 0 && cell.PossibleValue.Count == 1)
        {
          cell.CurrentValue = cell.PossibleValue[0];
          return true;
        }
        return false;
      });
    }
    public Task<bool> IsAllConstraintsRespectedAsync(ICell cell)
    {
      return Task.Run(() =>
      {
        HashSet<int> usedValues = new HashSet<int>();

        ConstraintCellPos.Board[cell.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));

        return usedValues.Add(cell.CurrentValue);
      });
    }
    public Task GetPossibleCellValuesAsync(ICell cell)
    {
      return Task.Run(() =>
      {
        if (cell.CurrentValue == 0)
        {
          HashSet<int> usedValues = new HashSet<int>();
          ConstraintCellPos.Board[cell.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));
          cell.PossibleValue.Clear();
          cell.PossibleValue.AddRange(ConstraintCellPos.AllPossibleValues.Except(usedValues));
        }
      });
    }

    public void Push()
    {
      List<Tuple<int, List<int>>> values = new List<Tuple<int, List<int>>>(81);
      Cells.ForEach(c => values.Add(new Tuple<int, List<int>>(c.CurrentValue, new List<int>(c.PossibleValue))));
      _stack.Push(values);
    }
    public void Pop()
    {
      List<Tuple<int, List<int>>> values = _stack.Pop();
      for (int i = 0; i < 81; i++)
      {
        Cells[i].CurrentValue = values[i].Item1;
        Cells[i].PossibleValue = values[i].Item2;
      }
    }

  }
}
