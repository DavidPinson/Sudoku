using System;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Interface;

namespace Sudoku.BrutalSimplicity
{
  public class SudokuEngineBrutalSimplicity : ISudokuEngine
  {
    public Task<Tuple<bool, string>> SolveAsync(string puzzle)
    {
      return Task.Run(() =>
      {
        int[,] grid = new int[9, 9];

        int lineIndex = 0;
        int rowIndex = 0;
        for(int i = 0; i < 81; i++)
        {
          lineIndex = i / 9;
          rowIndex = i % 9;
          grid[lineIndex, rowIndex] = puzzle[i] - 48;
        }

        SudokuSolver solver = new(grid);
        bool solved = solver.Solve();
        StringBuilder sb = new();

        for(int i = 0; i < 9; i++)
        {
          for(int j = 0; j < 9; j++)
          {
            sb.Append((char)(solver.m_grid[i, j] + 48));
          }
        }

        return new Tuple<bool, string>(solved, sb.ToString());
      });
    }
  }
}
