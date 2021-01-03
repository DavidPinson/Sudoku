using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sudoku.ParallelDP
{
  // https://scatteredcode.net/parallel-foreach-async-in-c/
  // https://github.com/ops-ai/experiments

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

      ActionBlock<T> block = new(body, options);

      await foreach(T item in source)
      {
        block.Post(item);
      }

      block.Complete();
      await block.Completion;
    }

    public static void TryAddOrUpdate<T, U>(this ConcurrentDictionary<T, U> cd, T key, U val)
    {
      U valTmp = default(U);
      if(cd.TryGetValue(key, out valTmp) == true)
      {
        cd.TryUpdate(key, val, valTmp);
      }
      else
      {
        cd.TryAdd(key, val);
      }
    }
  }
}