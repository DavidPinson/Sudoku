using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Sudoku.Benchmark
{
  public class Program
  {
    // dotnet run -p .\Sudoku.Benchmark\Sudoku.Benchmark.csproj -c Release -f net5.0 --runtimes netcoreapp5.0 netcoreapp2.1 netcoreapp3.1 net461 
    public static void Main()
    {
      Summary summary = BenchmarkRunner.Run<SudokuBenchmark>();
    }
  }
}
