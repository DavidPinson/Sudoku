using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sudoku.ParallelDP
{
  public static class Extension
  {
    public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> func, int maxDegreeOfParallelism = 4)
    {
      async Task AwaitPartition(IEnumerator<T> partition)
      {
        using(partition)
        {
          while(partition.MoveNext())
          {
            await func(partition.Current).ConfigureAwait(false);
          }
        }
      }

      return Task.WhenAll(
        Partitioner
          .Create(source)
          .GetPartitions(maxDegreeOfParallelism)
          .AsParallel()
          .Select(p => AwaitPartition(p)));
    }

    public static async Task AsyncParallelForEach<T>(this IAsyncEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
    {
      ExecutionDataflowBlockOptions options = new()
      {
        MaxDegreeOfParallelism = maxDegreeOfParallelism
      };

      if(scheduler != null)
      {
        options.TaskScheduler = scheduler;
      }

      var block = new ActionBlock<T>(body, options);

      await foreach(var item in source)
        block.Post(item);

      block.Complete();
      await block.Completion;
    }
  }
}