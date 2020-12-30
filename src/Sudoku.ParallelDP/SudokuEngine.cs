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

      bool solved = await Solve(board).ConfigureAwait(false);

      StringBuilder sb = new();
      board.Cells.ForEach(c =>
      {
        sb.Append((char)(c.CurrentValue + 48));
      });

      return new Tuple<bool, string>(solved, sb.ToString());
    }

    private async Task<bool> Solve(Board board)
    {
      bool knowedMovesTodo;
      do
      {
        do
        {
          knowedMovesTodo = false;
          foreach(Cell c in board.Cells)
          {
            await board.GetPossibleCellValuesAsync(c).ConfigureAwait(false);
            knowedMovesTodo = knowedMovesTodo || await Board.FindNakedSingleAsync(c).ConfigureAwait(false);
          }
        } while(knowedMovesTodo == true);

        Console.WriteLine();
        int indexTmp = 0;
        for(int i = 0; i < 9; i++)
        {
          for(int j = 0; j < 9; j++)
          {
            Console.Write($"{board.Cells[indexTmp].CurrentValue} ");
            indexTmp++;
          }
          Console.Write("\n");
        }
        Console.WriteLine();

        knowedMovesTodo = await board.FindHiddenSingleAsync().ConfigureAwait(false);
      } while(knowedMovesTodo == true);

      // Est-ce que la grille est résolue?
      if(await board.IsAllConstraintsRespectedAsync().ConfigureAwait(false) == true)
      {
        return true; // solved!
      }

      foreach(Cell c in board.Cells)
      {
        if(c.CurrentValue == 0 && c.PossibleValue.Count > 0)
        {
          List<int> possibleValue = new(c.PossibleValue);
          foreach(int val in possibleValue)
          {
            c.CurrentValue = val;

            Console.WriteLine($"Recurse cell index: {c.Index}, value tried: {val}");

            board.Push();
            bool success = await Solve(board).ConfigureAwait(false);

            Console.WriteLine($"Back from Recurse cell index: {c.Index}, value tried: {val}, success: {success}");

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

      Console.WriteLine();
      int index = 0;
      for(int i = 0; i < 9; i++)
      {
        for(int j = 0; j < 9; j++)
        {
          Console.Write($"{board.Cells[index].CurrentValue} ");
          index++;
        }
        Console.Write("\n");
      }
      Console.WriteLine();

      return false;
    }
  }
}
