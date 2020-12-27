using System;
using System.Threading.Tasks;

namespace Sudoku.Interface
{
  public interface ISudokuEngine
  {
    Task<Tuple<bool, string>> SolveAsync(string puzzle);
  }
}