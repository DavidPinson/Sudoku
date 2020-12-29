using System.Collections.Generic;

namespace Sudoku.DP
{
  public class Cell
  {
    public int Index { get; set; }
    public int CurrentValue { get; set; }
    public List<int> PossibleValue { get; set; }

    public Cell(int index)
    {
      Index = index;
      CurrentValue = 0;
      PossibleValue = new List<int>();
    }
    public Cell(Cell cell)
    {
      Index = cell.Index;
      CurrentValue = cell.CurrentValue;
      PossibleValue = new List<int>(cell.PossibleValue);
    }
  }
}
