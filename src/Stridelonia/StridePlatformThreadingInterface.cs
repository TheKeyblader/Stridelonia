﻿using System;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Stridelonia
{
    class StridePlatformThreadingInterface : IPlatformThreadingInterface
    {
        public StridePlatformThreadingInterface()
        {
            _thread = Thread.CurrentThread;
            _event = new AutoResetEvent(false);
            _lock = new object();
        }

        private readonly AutoResetEvent _event;
        private readonly Thread _thread;
        private readonly object _lock;
        private DispatcherPriority? _signaledPriority;

        public void RunLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DispatcherPriority? signaled = null;
                lock (_lock)
                {
                    signaled = _signaledPriority;
                    _signaledPriority = null;
                }
                if (signaled.HasValue)
                    Signaled?.Invoke(signaled);
                WaitHandle.WaitAny(new[] { cancellationToken.WaitHandle, _event });
            }
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
        public void Signal(DispatcherPriority priority)
        {
            lock (_lock)
            {
                if (_signaledPriority == null || _signaledPriority.Value > priority)
                {
                    _signaledPriority = priority;
                }
                _event.Set();
            }
        }

        public bool CurrentThreadIsLoopThread => _thread == Thread.CurrentThread;
        public event Action<DispatcherPriority?> Signaled;
    }
}
