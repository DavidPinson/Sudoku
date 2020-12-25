using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sudoku.DP;
using Sudoku.Interface;

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
    private static readonly List<Tuple<string, string>> _puzzles = new List<Tuple<string, string>>(2)
    {
      new Tuple<string, string>("698500012070020090000018607006092003002000700700480200209160000060040080410003926",
        "698537412173624895524918637856792143942351768731486259289165374367249581415873926"),
      new Tuple<string, string>("026780000000003460000056000090000200730090058002000040000940000087200000000061370",
        "")
    };

    static async Task Main(string[] args)
    {
      try
      {
        await TestSudokuEngineDPAsync().ConfigureAwait(false);
      }
      catch(Exception ex)
      {
        Console.WriteLine($"Error, msg: {ex.Message}, stacktrace: {ex.StackTrace}");
      }
    }

    static async Task TestSudokuEngineDPAsync()
    {
      ISudokuEngine sudokuEngineDP = new SimpleEngineDP();

      foreach(Tuple<string, string> t in _puzzles)
      {
        if(await sudokuEngineDP.SolveAsync(t.Item1, out string solution).ConfigureAwait(false) == true)
        {
          if(t.Item2.Equals(solution) == true)
          {
            Console.WriteLine($"puzzle solved");
          }
        }
      }
    }

  }
}
