
namespace KosmoGraph.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static void EndWith(this Task thisTask, Action succeeded, Func<Exception,bool> failed = null, Action cancelled = null)
        {
            thisTask.ContinueWith(t =>
            {
                if (t.IsCompleted && succeeded != null)
                    succeeded();
                else if (t.IsFaulted && failed != null)
                    t.Exception.Flatten().Handle(failed);
                else if (t.IsCanceled && cancelled != null)
                    cancelled();
            });
        }

        public static void EndWith<T>(this Task<T> thisTask, Action<T> succeeded, Func<Exception,bool> failed = null, Action cancelled = null)
        {
            thisTask.ContinueWith(t =>
            {
                if (t.IsCompleted && succeeded != null)
                    succeeded(t.Result);
                else if (t.IsFaulted && failed != null)
                    t.Exception.Flatten().Handle(failed);
                else if (t.IsCanceled && cancelled != null)
                    cancelled();
            });
        }
    }
}
