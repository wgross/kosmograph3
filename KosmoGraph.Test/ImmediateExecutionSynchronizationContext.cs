namespace KosmoGraph.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public sealed class ImmediateExecutionSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly object syncHandle = new object();
        
        public override void Send(SendOrPostCallback codeToRun, object state)
        {
            lock(syncHandle)
                codeToRun(state);
        }

        public override void Post(SendOrPostCallback codeToRun, object state)
        {
            lock(syncHandle)
             codeToRun(state);
        }

        
        public void Dispose()
        {
            return;
        }
    }
}
