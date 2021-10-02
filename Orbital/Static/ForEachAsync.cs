using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Rodin.Static
{
    static class EnumerableExtentions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> source,
           int dop,
           Func<T, Task> action)
        {
            // Arguments validation omitted
            var block = new ActionBlock<T>(action,
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = dop });
            try
            {
                foreach (var item in source) block.Post(item);
                block.Complete();
            }
            catch (Exception ex) { ((IDataflowBlock)block).Fault(ex); }
            return block.Completion;
        }
    }
}
