using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.DP
{
  public class Board
  {
    private Stack<List<int>> _stack = new Stack<List<int>>();

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

    public Task<bool> FindNakedSingleAsync(Cell cell)
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
    public Task<bool> FindHiddenSingleAsync(Cell cell)
    {
      return Task.Run(() =>
      {
        return false;
      });
    }
    public Task<bool> IsAllConstraintsRespectedAsync(Cell cell)
    {
      return Task.Run(() =>
      {
        HashSet<int> usedValues = new HashSet<int>();

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
          HashSet<int> usedValues = new HashSet<int>();
          ConstraintCellPos.Board[cell.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));
          cell.PossibleValue.Clear();
          cell.PossibleValue.AddRange(ConstraintCellPos.AllPossibleValues.Except(usedValues));
        }
      });
    }

    public void Push()
    {
      List<int> values = new List<int>(81);
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
