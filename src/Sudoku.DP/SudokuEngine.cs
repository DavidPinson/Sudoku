using System.Collections.Generic;
using System.Threading.Tasks;
using Sudoku.Interface;

namespace Sudoku.DP
{
  public class SimpleEngineDP : ISudokuEngine
  {
    public Task<bool> SolveAsync(string puzzle, out string solution)
    {
      throw new System.NotImplementedException();
    }

    private async Task<bool> Solve(Board board)
    {
      bool knowedMovesTodo;
      do
      {
        knowedMovesTodo = false;
        foreach(Cell c in board.Cells)
        {
          await board.GetPossibleCellValuesAsync(c).ConfigureAwait(false);
          knowedMovesTodo = knowedMovesTodo || await board.FindNakedSingleAsync(c).ConfigureAwait(false);
        }
      } while(knowedMovesTodo == true);

      // Est-ce que la grille est résolue?
      bool gridResolved = true;
      foreach(Cell c in board.Cells)
      {
        gridResolved = gridResolved && await board.IsAllConstraintsRespectedAsync(c).ConfigureAwait(false);
      }
      if(gridResolved == true)
      {
        return true; // solved!
      }

      foreach(Cell c in board.Cells)
      {
        if(c.CurrentValue == 0 && c.PossibleValue.Count > 0)
        {
          List<int> possibleValue = new List<int>(c.PossibleValue);
          foreach(int val in possibleValue)
          {
            c.CurrentValue = val;

            System.Console.WriteLine($"Recurse cell index: {c.Index}, value tried: {val}");

            board.Push();
            bool success = await Solve(board).ConfigureAwait(false);

            System.Console.WriteLine($"Back from Recurse cell index: {c.Index}, value tried: {val}, success: {success}");

            if(success == true)
            {
              return true;
            }
            else
            {
              board.Pop();
            }
          }
        }
      }

      System.Console.WriteLine();
      int index = 0;
      for(int i = 0; i < 9; i++)
      {
        for(int j = 0; j < 9; j++)
        {
          System.Console.Write($"{board.Cells[index].CurrentValue} ");
          index++;
        }
        System.Console.Write("\n");
      }
      System.Console.WriteLine();

      return false;
    }
  }
}
