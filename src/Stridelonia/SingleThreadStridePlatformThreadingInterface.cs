using System;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Stridelonia
{
    internal class SingleThreadStridePlatformThreadingInterface : IPlatformThreadingInterface
    {
        public bool CurrentThreadIsLoopThread => _thread == Thread.CurrentThread;

        public event Action<DispatcherPriority?> Signaled;

        private readonly Thread _thread;
        private bool _signaled;

        public SingleThreadStridePlatformThreadingInterface()
        {
            _thread = Thread.CurrentThread;
        }

        public void RunLoop(CancellationToken cancellationToken)
        {

        }

        public void Signal(DispatcherPriority priority)
        {
            lock (this)
            {
                if (_signaled)
                    return;
                _signaled = true;
            }

            StrideDispatcher.StrideThread.Post(() =>
            {
                lock (this)
                    _signaled = false;
                Signaled?.Invoke(null);
            });
        }

        public IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick)
        {
            var cancelled = false;
            var enqueued = false;
            var l = new object();
            var timer = new Timer(_ =>
            {
                lock (l)
                {
                    if (cancelled || enqueued)
                        return;
                    enqueued = true;
                    Dispatcher.UIThread.Post(() =>
                    {
                        lock (l)
                        {
                            enqueued = false;
                            if (cancelled)
                                return;
                            tick();
                        }
                    }, priority);
                }
            }, null, interval, interval);
            return Disposable.Create(() =>
            {
                lock (l)
                {
                    timer.Dispose();
                    cancelled = true;
                }
            });
        }
    }
}
