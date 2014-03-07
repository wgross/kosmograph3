
namespace KosmoGraph.Persistence.MongoDb.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        //public static void EndWith(this Task thisTask, Action succeeded, Func<Exception,bool> handleExceptions = null, Action cancelled = null)
        //{
        //    thisTask.ContinueWith(t => (succeeded ?? delegate { })(), TaskContinuationOptions.OnlyOnRanToCompletion);
        //    thisTask.ContinueWith(t => (cancelled ?? delegate { })(), TaskContinuationOptions.OnlyOnCanceled);
        //    thisTask.ContinueWith(t => 
        //    {
        //        if (handleExceptions != null)
        //            t.Exception.Flatten().Handle(handleExceptions);
        //        t.Wait(); // throw
        //    }, TaskContinuationOptions.OnlyOnFaulted);
            
        //}

        //public static void EndWith<T>(this Task<T> thisTask, Action<T> succeeded, Action<IEnumerable<Exception>> failed = null, Action cancelled = null)
        //{
        //    thisTask.ContinueWith(t => (succeeded ?? delegate { })(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        //    thisTask.ContinueWith(t => (failed ?? delegate { })(t.Exception.Flatten().InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
        //    thisTask.ContinueWith(t => (cancelled ?? delegate { })(), TaskContinuationOptions.OnlyOnCanceled);
        //}

        //public static void EndWith(this Task thisTask, Action succeeded, Action<IEnumerable<Exception>> failed = null, Action cancelled = null)
        //{
        //    thisTask.ContinueWith(t => (succeeded ?? delegate { })(), TaskContinuationOptions.OnlyOnRanToCompletion);
        //    thisTask.ContinueWith(t => (failed ?? delegate { })(t.Exception.Flatten().InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
        //    thisTask.ContinueWith(t => (cancelled ?? delegate { })(), TaskContinuationOptions.OnlyOnCanceled);
        //}

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
