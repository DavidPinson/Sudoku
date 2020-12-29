using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku.DP
{
  public class Board
  {
    private readonly Stack<List<Cell>> _stack = new();

    public List<Cell> Cells { get; private set; }
    public List<Cell> this[int index]
    {
      get
      {
        if(index >= 0 && index <= 8)
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

    public bool FindHiddenSingleAndPropagate()
    {
      // pour que cette méthode fonctione l'hypothèse selon laquelle toutes les "possibles values"
      // de toutes les cases sont trouvées est posée.

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
          AssignCellValue(hiddenSingle.Item1, hiddenSingle.Item2);
          atLeastOneHiddenSingleHasBeenFound = true;
        }
      });

      return atLeastOneHiddenSingleHasBeenFound;
    }
    public bool IsAllConstraintsRespected()
    {
      List<int> houseValues = new();

      if(Cells.Any(c => c.CurrentValue == 0) == true)
      {
        return false;
      }

      foreach(List<int> l in ConstraintCellPos.Houses)
      {
        houseValues.Clear();
        l.ForEach(val => houseValues.Add(Cells[val].CurrentValue));

        if(ConstraintCellPos.AllPossibleValues.Except(houseValues).Any() == true)
        {
          return false;
        }
      }

      _stack.Clear();
      return true;
    }
    public bool IsStillSolvable()
    {
      HashSet<int> possibleValues = new();
      foreach(List<int> l in ConstraintCellPos.Houses)
      {
        possibleValues.Clear();
        l.ForEach(index =>
        {
          if(Cells[index].CurrentValue != 0)
          {
            possibleValues.Add(Cells[index].CurrentValue);
          }
          else
          {
            Cells[index].PossibleValue.ForEach(pv => possibleValues.Add(pv));
          }
        });

        if(ConstraintCellPos.AllPossibleValues.Except(possibleValues).Any() == true)
        {
          return false;
        }
      }
      return true;
    }
    public void GetPossibleCellValues()
    {
      foreach(Cell c in Cells)
      {
        if(c.CurrentValue == 0)
        {
          HashSet<int> usedValues = new();
          ConstraintCellPos.Board[c.Index].ForEach(i => usedValues.Add(Cells[i].CurrentValue));
          c.PossibleValue.Clear();
          c.PossibleValue.AddRange(ConstraintCellPos.AllPossibleValues.Except(usedValues));
        }
      }
    }
    public void FindNakedSingleAndPropagate()
    {
      foreach(Cell c in Cells)
      {
        if(c.CurrentValue == 0 && c.PossibleValue.Count == 1)
        {
          AssignCellValue(c.Index, c.PossibleValue[0]);
        }
      }
    }
    private void AssignCellValue(int cellIndex, int cellValue)
    {
      Cells[cellIndex].CurrentValue = cellValue;
      Cells[cellIndex].PossibleValue.Clear();
      ConstraintCellPos.Board[cellIndex].ForEach(i =>
      {
        if(Cells[i].PossibleValue.Remove(cellValue) == true)
        {
          if(Cells[i].PossibleValue.Count == 1)
          {
            AssignCellValue(i, Cells[i].PossibleValue[0]);
          }
        }
      });
    }

    public void Push()
    {
      List<Cell> values = new(81);
      Cells.ForEach(c => values.Add(new Cell(c)));
      _stack.Push(values);
    }
    public void Pop()
    {
      List<Cell> values = _stack.Pop();
      values.ForEach(c =>
      {
        Cells[c.Index].CurrentValue = c.CurrentValue;
        Cells[c.Index].PossibleValue = c.PossibleValue;
      });
    }

    public void Print(TextWriter tw)
    {
      int space = Cells.Max(c => c.PossibleValue.Count);
      space = space == 0 ? 1 : space;
      string maxDashString = new('-', (space + 1) * 3);

      tw.WriteLine();
      for(int r = 0; r < 9; r++)
      {
        if((r % 3) == 0 && r != 0)
        {
          tw.Write(maxDashString);
          tw.Write("+");
          tw.Write(maxDashString);
          tw.Write("-+");
          tw.Write(maxDashString);
          tw.Write("-");
          tw.WriteLine();
        }
        for(int c = 0; c < 9; c++)
        {
          if((c % 3) == 0 && c != 0)
          {
            tw.Write("| ");
          }

          if(this[r][c].CurrentValue == 0)
          {
            this[r][c].PossibleValue.ForEach(v =>
            {
              tw.Write(v);
            });

            tw.Write(new string(' ', space - this[r][c].PossibleValue.Count));
            tw.Write(" ");
          }
          else
          {
            tw.Write(this[r][c].CurrentValue);
            if(space > 1)
              tw.Write(new string(' ', space - 1));
            tw.Write(" ");
          }
        }
        tw.WriteLine();
      }
    }

  }
}
