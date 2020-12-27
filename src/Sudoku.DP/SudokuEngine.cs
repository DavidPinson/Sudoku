using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Interface;

namespace Sudoku.DP
{
  public class SimpleEngineDP : ISudokuEngine
  {
    public Task<Tuple<bool, string>> SolveAsync(string puzzle)
    {
      return Task.Run(() =>
      {
        Board board = new();
        Cell cell;
        for(int i = 0; i < 81; i++)
        {
          cell = new(i);
          cell.CurrentValue = puzzle[i] - 48;
          board.Cells.Add(cell);
        }

        bool solved = Solve(board);

        StringBuilder sb = new();
        board.Cells.ForEach(c =>
        {
          sb.Append((char)(c.CurrentValue + 48));
        });

        return new Tuple<bool, string>(solved, sb.ToString());
      });
    }

    private bool Solve(Board board)
    {
      bool knowedMovesTodo;
      do
      {
        foreach(Cell c in board.Cells)
        {
          board.GetPossibleCellValues(c);
        }

        // Console.WriteLine();
        // int indexTmp = 0;
        // for(int i = 0; i < 9; i++)
        // {
        //   for(int j = 0; j < 9; j++)
        //   {
        //     Console.Write($"{board.Cells[indexTmp].CurrentValue} ");
        //     indexTmp++;
        //   }
        //   Console.Write("\n");
        // }
        // Console.WriteLine();

        knowedMovesTodo = board.FindHiddenSingle();
      } while(knowedMovesTodo == true);

      // Est-ce que la grille est résolue?
      if(board.IsAllConstraintsRespected() == true)
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

            //Console.WriteLine($"Recurse cell index: {c.Index}, value tried: {val}");

            board.Push();
            bool success = Solve(board);

            //Console.WriteLine($"Back from Recurse cell index: {c.Index}, value tried: {val}, success: {success}");

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

      // Console.WriteLine();
      // int index = 0;
      // for(int i = 0; i < 9; i++)
      // {
      //   for(int j = 0; j < 9; j++)
      //   {
      //     Console.Write($"{board.Cells[index].CurrentValue} ");
      //     index++;
      //   }
      //   Console.Write("\n");
      // }
      // Console.WriteLine();

      return false;
    }
  }
}
