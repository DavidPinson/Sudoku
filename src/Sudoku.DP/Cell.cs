using System.Collections.Generic;
using System.Linq;
using Sudoku.Board.Interface;

namespace Sudoku.Board
{
  public class Cell : ICell
  {
    public int Index { get; set; }
    public bool IsInitialValue { get; set; }
    public int CurrentValue { get; set; }
    public List<int> PossibleValue { get; set; }

    public Cell(int index)
    {
      Index = index;
      IsInitialValue = false;
      CurrentValue = 0;
      PossibleValue = new List<int>();
    }
    //public Cell(Cell cell)
    //{
    //	Index = cell.Index;
    //	IsInitialValue = cell.IsInitialValue;
    //	CurrentValue = cell.CurrentValue;
    //	PossibleValue = new List<int>(cell.PossibleValue);
    //}
  }
}
