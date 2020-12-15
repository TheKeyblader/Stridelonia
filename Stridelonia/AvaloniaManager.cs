using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Avalonia;
using Stride.Core.Diagnostics;
using Stride.Games;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Stridelonia
{
    internal static class AvaloniaManager
    {
        private static readonly EventWaitHandle _initedEvent;
        private static readonly EventWaitHandle _runEvent;

        private static Thread _avaloniaThread;
        private static bool _isInitialize;

        public static StridePlatformOptions Options { get; }

        static AvaloniaManager()
        {
            _initedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
            _runEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

            Options = AvaloniaLocator.Current.GetService<StridePlatformOptions>() ?? GetOptions();
            AvaloniaLocator.CurrentMutable.BindToSelf(Options);
        }

        public static void Initialize(IGame game)
        {
            if (_isInitialize) return;

            if (game.GetType().Name.Contains("Editor", StringComparison.OrdinalIgnoreCase))
            {
                var logger = GlobalLogger.GetLogger("Stridelonia");
                logger.Info("Stridelonia is disabled in GameStudio");
                return;
            }
            if (Options == null) return;

            AvaloniaLocator.CurrentMutable.BindToSelf(game);

            var dispatcher = game.Services.GetService<StrideDispatcher>();
            if (dispatcher == null)
            {
                dispatcher = new StrideDispatcher(game.Services);
                game.Services.AddService(dispatcher);
                game.GameSystems.Add(dispatcher);
            }

            if (Application.Current == null) StartAvalonia();

            _isInitialize = true;
        }

        public static void Run()
        {
            if (Options.UseMultiThreading)
                _runEvent.Set();
            else
            {
                var lifetime = (ClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
                lifetime.Start(Environment.GetCommandLineArgs());
            }
        }

        public static void Stop()
        {
            var lifetime = (ClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            Dispatcher.UIThread.Post(() =>
            {
                lifetime.Shutdown();
            });
        }

        private static void StartAvalonia()
        {
            if (Options.ConfigureApp == null || Options.ApplicationType == null)
            {
                var logger = GlobalLogger.GetLogger("Stridelonia");
                logger.Debug("No Application in StridePlatformOptions");
                return;
            }

            if (Options.UseMultiThreading)
            {
                _avaloniaThread = new Thread(AvaloniaThread)
                {
                    Name = "Avalonia Thread"
                };
                _avaloniaThread.Start(Options);
                _initedEvent.WaitOne();
                _initedEvent.Dispose();
            }
            else
            {
                var builderType = typeof(AppBuilderBase<>).MakeGenericType(typeof(AppBuilder));
                var configureMethod = builderType.GetMethod(nameof(AppBuilder.Configure), BindingFlags.Public | BindingFlags.Static, null, new Type[0], null);
                var builder = (AppBuilder)configureMethod.MakeGenericMethod(Options.ApplicationType).Invoke(null, new object[0]);

                builder
                    .UseStride()
                    .UseDirect2D1();
                Options.ConfigureApp(builder);

                var lifetime = new ClassicDesktopStyleApplicationLifetime
                {
                    Args = Environment.GetCommandLineArgs(),
                    ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
                builder.SetupWithLifetime(lifetime);
            }
        }

        private static void AvaloniaThread(object parameter)
        {
            var options = (StridePlatformOptions)parameter;

            var builderType = typeof(AppBuilderBase<>).MakeGenericType(typeof(AppBuilder));
            var configureMethod = builderType.GetMethod(nameof(AppBuilder.Configure), BindingFlags.Public | BindingFlags.Static, null, new Type[0], null);
            var builder = (AppBuilder)configureMethod.MakeGenericMethod(options.ApplicationType).Invoke(null, new object[0]);

            builder
                .UseStride()
                .UseDirect2D1();
            options.ConfigureApp(builder);

            var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                Args = Environment.GetCommandLineArgs(),
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            builder.SetupWithLifetime(lifetime);

            _initedEvent.Set();
            _runEvent.WaitOne();

            lifetime.Start(Environment.GetCommandLineArgs());
        }

        private static StridePlatformOptions GetOptions()
        {
            try
            {
                var configuratorConstructor =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetCustomAttributes<AvaloniaConfiguratorAttribute>())
                        .Single().ConfiguratorType.GetTypeInfo().DeclaredConstructors
                        .Where(c => c.GetParameters().Length == 0 && !c.IsStatic).Single();

                var configuratorMethod = configuratorConstructor.DeclaringType.GetTypeInfo().DeclaredMethods
                    .Single(m => m.GetParameters().Length == 0 && m.ReturnType == typeof(StridePlatformOptions));

                var instance = configuratorConstructor.Invoke(Array.Empty<object>());
                return (StridePlatformOptions)configuratorMethod.Invoke(instance, Array.Empty<object>());
            }
            catch (InvalidOperationException)
            {
                var logger = GlobalLogger.GetLogger("Stridelonia");
                logger.Debug("No Application configurator found");
                return null;
            }
        }
    }
}
