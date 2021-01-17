using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Threading;
using Stride.Core;
using Stride.Games;

namespace Stridelonia
{
    public class StrideDispatcher : GameSystemBase, IDispatcher
    {
        public static IDispatcher StrideThread { get; internal set; }

        [ThreadStatic]
        private static bool isStrideThread;

        private readonly ConcurrentQueue<Task> _taskqueue = new();

        public StrideDispatcher(IServiceRegistry registry) : base(registry)
        {
            if (StrideThread != null) throw new InvalidOperationException("One stride dispatcher allowed !");
            StrideThread = this;
            isStrideThread = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            Enabled = true;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_taskqueue.TryDequeue(out var task))
            {
                try
                {
                    task.RunSynchronously();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }
        }

        public bool CheckAccess() => isStrideThread;

        public void VerifyAccess()
        {
            if (!CheckAccess())
                throw new InvalidOperationException("Call from invalid thread");
        }

        public void Post(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (CheckAccess())
                action();
            else
                _taskqueue.Enqueue(new Task(action));
        }

        public Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var task = new Task(action);
            if (CheckAccess())
                task.RunSynchronously();
            else
                _taskqueue.Enqueue(task);
            return task;
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var task = new Task<TResult>(function);
            if (CheckAccess())
                task.RunSynchronously();
            else
                _taskqueue.Enqueue(task);
            return task;
        }

        public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var task = new Task<Task>(function);
            if (CheckAccess())
                task.RunSynchronously();
            else
                _taskqueue.Enqueue(task);
            return task.Unwrap();
        }

        public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var task = new Task<Task<TResult>>(function);
            if (CheckAccess())
                task.RunSynchronously();
            else
                _taskqueue.Enqueue(task);
            return task.Unwrap();
        }

    }
}
