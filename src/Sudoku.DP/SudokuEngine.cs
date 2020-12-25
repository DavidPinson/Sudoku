using Sudoku.Board;
using Sudoku.Board.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sudoku.Engine
{
  public class SimpleEngine : IEngine
  {
    public async Task<bool> Solve(IBoard board)
    {
      // trouver tout les moves logique "sans guess"
      bool knowedMovesTodo;
      do
      {
        knowedMovesTodo = false;
        foreach (ICell c in board.Cells)
        {
          await board.GetPossibleCellValuesAsync(c).ConfigureAwait(false);
          knowedMovesTodo = knowedMovesTodo || await board.ComputeKnownMovesAsync(c).ConfigureAwait(false);
        }
      } while (knowedMovesTodo == true);

      // Est-ce que la grille est résolue?
      bool gridResolved = true;
      foreach (ICell c in board.Cells)
      {
        gridResolved = gridResolved && await board.IsAllConstraintsRespectedAsync(c).ConfigureAwait(false);
      }
      if (gridResolved == true)
      {
        return true; // solved!
      }

      foreach (ICell c in board.Cells)
      {
        if (c.CurrentValue == 0 && c.PossibleValue.Count > 0)
        {
          List<int> possibleValue = new List<int>(c.PossibleValue);
          foreach (int val in possibleValue)
          {
            c.CurrentValue = val;

            System.Console.WriteLine($"Recurse cell index: {c.Index}, value tried: {val}");

            ((Board.Board)board).Push();
            bool success = await Solve(board).ConfigureAwait(false);

            System.Console.WriteLine($"Back from Recurse cell index: {c.Index}, value tried: {val}, success: {success}");

            if (success == true)
            {
              return true;
            }
            else
            {
              ((Board.Board)board).Pop();
            }
          }
        }
      }

      System.Console.WriteLine();
      int index = 0;
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
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
