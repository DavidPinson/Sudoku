using System.Collections.Concurrent;

namespace Sudoku.ParallelDP
{
  public class Cell
  {
    private int _index;
    private readonly object _lockIndex = new();
    private int _currentValue;
    private readonly object _lockCurrentValue = new();

    public int Index
    {
      get
      {
        int tmp;
        lock(_lockIndex)
        {
          tmp = _index;
        }
        return tmp;
      }
      set
      {
        lock(_lockIndex)
        {
          _index = value;
        }
      }
    }
    public int CurrentValue
    {
      get
      {
        int tmp;
        lock(_lockCurrentValue)
        {
          tmp = _currentValue;
        }
        return tmp;
      }
      set
      {
        lock(_lockCurrentValue)
        {
          _currentValue = value;
        }
      }
    }

    public ConcurrentDictionary<int, byte> PossibleValue { get; set; }

    public Cell(int index)
    {
      Index = index;
      CurrentValue = 0;
      PossibleValue = new ConcurrentDictionary<int, byte>();
    }
    public Cell(Cell cell)
    {
      Index = cell.Index;
      CurrentValue = cell.CurrentValue;
      PossibleValue = new ConcurrentDictionary<int, byte>(cell.PossibleValue);
    }
  }
}
