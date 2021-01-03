using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Interface;

namespace Sudoku.ParallelDP
{
  public class SimpleEngineParallelDP : ISudokuEngine
  {
    public async Task<Tuple<bool, string>> SolveAsync(string puzzle)
    {
      Board board = new();
      Cell cell;
      for(int i = 0; i < 81; i++)
      {
        cell = new(i);
        cell.CurrentValue = puzzle[i] - 48;
        board.Cells.Add(cell);
      }

      await board.GetPossibleCellValuesAsync().ConfigureAwait(false);
      bool solved = await Solve(board).ConfigureAwait(false);

      StringBuilder sb = new();
      board.Cells.ForEach(c =>
      {
        sb.Append((char)(c.CurrentValue + 48));
      });

      return new Tuple<bool, string>(solved, sb.ToString());
    }

    private static async Task<bool> Solve(Board board)
    {
      await board.FindNakedSingleAndPropagateAsync().ConfigureAwait(false);
      while(board.FindHiddenSingleAndPropagate() == true) ;

      if(board.IsStillSolvable() == false)
      {
        // pas besoin de faire d'autre récursion pour rien
        return false;
      }

      // Est-ce que la grille est résolu?
      if(board.IsAllConstraintsRespected() == true)
      {
        return true; // solved!
      }

      List<int> possibleValues;
      foreach(Cell c in board.Cells)
      {
        if(c.CurrentValue == 0 && c.PossibleValue.Count > 0)
        {
          possibleValues = new List<int>(c.PossibleValue);
          foreach(int val in possibleValues)
          {
            board.Push();
            c.PossibleValue.Clear();
            c.PossibleValue.Add(val);
            bool success = Solve(board);
            if(success == true)
            {
              return true; // solved!
            }
            else
            {
              board.Pop();
            }
          }
          return false;
        }
      }
      return false;
    }

  }
}
