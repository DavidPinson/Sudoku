using System.Threading.Tasks;

namespace Sudoku.Interface
{
  public interface ISudokuEngine
  {
      Task<bool> SolveAsync(string puzzle, out string solution);
  }
}