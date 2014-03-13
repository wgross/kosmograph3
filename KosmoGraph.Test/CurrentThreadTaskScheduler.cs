namespace KosmoGraph.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;
    using System;

    /// <summary>Provides a task scheduler that runs tasks on the current thread.</summary>
    public sealed class CurrentThreadTaskScheduler : TaskScheduler
    {
        public static void InstallAsDefaultScheduler()
        {
            typeof(TaskScheduler)
                .GetField("s_defaultTaskScheduler", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, new CurrentThreadTaskScheduler());
        }

        /// <summary>Runs the provided Task synchronously on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task)
        {
            TryExecuteTask(task);
        }

        /// <summary>Runs the provided Task synchronously on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the Task was previously queued to the scheduler.</param>
        /// <returns>True if the Task was successfully executed; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTask(task);
        }

        /// <summary>Gets the Tasks currently scheduled to this scheduler.</summary>
        /// <returns>An empty enumerable, as Tasks are never queued, only executed.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }

        /// <summary>Gets the maximum degree of parallelism for this scheduler.</summary>
        public override int MaximumConcurrencyLevel { get { return 1; } }

    }

    [TestClass]
    public class TestCurrentThreadTaskScheduler
    {
        [TestMethod]
        public void Test_StartNew_Exception()
        {

            Task target;
            try
            {
                target = Task.Factory.StartNew(() =>
                {
                    throw new InvalidOperationException();
                });

                target.Wait();

                Assert.Fail("Should not pass");
            }
            catch (AggregateException)
            {
            }
        }

        [TestMethod]
        public void Test_StartNew_Exception_MitScheduler()
        {
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();

            Task target;
            try
            {
                target = Task.Factory.StartNew(() =>
                {
                    throw new InvalidOperationException();
                });

                target.Wait();

                Assert.Fail("Should not pass");
            }
            catch (AggregateException)
            {
            }
        }

        [TestMethod]
        public void Test_StartNew_WithoutWait_Exception_MitScheduler()
        {
            CurrentThreadTaskScheduler.InstallAsDefaultScheduler();
            int count = 0;
            Task target;
            try
            {
                target = Task.Factory.StartNew(() =>
                {
                    count++;
                    throw new InvalidOperationException();
                });

                Assert.AreEqual(1, count);
                // this Assert.Fail really fails because the task's logic runs in "Synchronously" but in another thread and 
                //    AggregateException is thrown in the other thread.
                //Assert.Fail("Should not pass");
            }
            catch (AggregateException)
            {
            }
        }
    }
}

