using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Sudoku.Benchmark
{
  public class Program
  {
    public static void Main()
    {
      Summary summary = BenchmarkRunner.Run<SudokuBenchmark>();
    }
  }
}
