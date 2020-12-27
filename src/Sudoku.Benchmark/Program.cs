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
    private static readonly List<Tuple<string, string>> _puzzles = new()
    {
      new Tuple<string, string>("698500012070020090000018607006092003002000700700480200209160000060040080410003926",
        "698537412173624895524918637856792143942351768731486259289165374367249581415873926"),
      new Tuple<string, string>("026780000000003460000056000090000200730090058002000040000940000087200000000061370",
        "426789513578123469913456782891574236734692158652318947365947821187235694249861375")
    };

    static async Task Main()
    {
      try
      {
        ISudokuEngine engine = new SimpleEngineDP();
        await TestSudokuEngineAsync(engine, "SimpleEngineDP").ConfigureAwait(false);
      }
      catch(Exception ex)
      {
        Console.WriteLine($"Error, msg: {ex.Message}, stacktrace: {ex.StackTrace}");
      }
    }

    static async Task TestSudokuEngineAsync(ISudokuEngine engine, string name)
    {
      Console.WriteLine();
      Console.WriteLine($"----------------------------------------------");
      Console.WriteLine($"{name}");
      Console.WriteLine($"----------------------------------------------");
      Console.WriteLine();

      Tuple<bool, string> solvedReturn;
      foreach(Tuple<string, string> t in _puzzles)
      {
        solvedReturn = await engine.SolveAsync(t.Item1).ConfigureAwait(false);
        if(solvedReturn.Item1 == true)
        {
          if(t.Item2.Equals(solvedReturn.Item2) == true)
          {
            Console.WriteLine($"SOLVED");
          }
          else
          {
            Console.WriteLine($"Problem: solver think it solved the puzzle but it's wrong");
          }
        }
        else
        {
          Console.WriteLine($"Solver did not solved the puzzle");
        }
      }
    }

  }
}
