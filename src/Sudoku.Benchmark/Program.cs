using System;

// https://www.sudopedia.org/wiki/Naked_Triple
// https://www.sudopedia.org/wiki/Naked_Pair
// https://www.sudopedia.org/wiki/Hidden_Single
// https://www.sudopedia.org/wiki/Naked_Single
// https://www.sudopedia.org/wiki/Locked_Candidates
// http://www.angusj.com/sudoku/hints.php
// https://www.e-sudoku.fr/grille-de-sudoku.php

namespace Sudoku.Benchmark
{
    class Program
    {
    static async Task Main(string[] args)
    {
      IEngine engine = new SimpleEngine();
      IBoard board = new Board.Board();
      IBoard solutinBoard = new Board.Board();
      ICell cell;

      try
      {
        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string directory = Path.GetDirectoryName(path);
        string dataDirectory = Path.Combine(directory, "data");

        string line;
        int index = 0;
        using (FileStream fs = new FileStream(Path.Combine(dataDirectory, "1.txt"), FileMode.Open, FileAccess.Read, FileShare.Read))
        using (StreamReader sr = new StreamReader(fs))
        {
          for (int i = 0; i < 9; i++)
          {
            line = await sr.ReadLineAsync().ConfigureAwait(false);

            for (int j = 0; j < 9; j++)
            {
              cell = new Cell(index);
              cell.CurrentValue = line[j] - 48;
              board.Cells.Add(cell);
              index++;
            }
          }

          // ligne vide
          line = await sr.ReadLineAsync().ConfigureAwait(false);

          index = 0;
          for (int i = 0; i < 9; i++)
          {
            line = await sr.ReadLineAsync().ConfigureAwait(false);

            for (int j = 0; j < 9; j++)
            {
              cell = new Cell(index);
              cell.CurrentValue = line[j] - 48;
              solutinBoard.Cells.Add(cell);
              index++;
            }
          }
        }

        bool resolved = await engine.Solve(board).ConfigureAwait(false);

        System.Console.WriteLine($"Result: {resolved}");
      }
      catch (Exception ex)
      {
        int boulette = 0;
      }
    }
    }
}
