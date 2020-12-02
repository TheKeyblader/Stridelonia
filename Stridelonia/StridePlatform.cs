using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Rendering;
using Stride.Engine;
using Stride.Games;
using Stridelonia.Implementation;

namespace Avalonia
{
    public static class StrideApplicationExtensions
    {
        public static AppBuilder UseStride(this AppBuilder builder)
            => builder.UseWindowingSubsystem(() => Stridelonia.StridePlatform.Initialize(), "Stride");
    }

    public class StridePlatformOptions
    {
        public bool UseMultiThreading { get; set; }
        public bool UseDeferredRendering { get; set; } = true;
        public bool DrawFps { get; set; }
        public Type ApplicationType { get; set; }
        public Action<AppBuilder> ConfigureApp { get; set; }
    }
}

namespace Stridelonia
{
    class StridePlatform : IPlatformSettings, IWindowingPlatform
    {
        public Size DoubleClickSize => new Size(4, 4);

        public TimeSpan DoubleClickTime => TimeSpan.FromSeconds(0.2);

        public static void Initialize()
        {
            var options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();

            if (options.UseMultiThreading)
                AvaloniaLocator.CurrentMutable.Bind<IPlatformThreadingInterface>().ToSingleton<StridePlatformThreadingInterface>();
            else
                AvaloniaLocator.CurrentMutable.Bind<IPlatformThreadingInterface>().ToSingleton<SingleThreadStridePlatformThreadingInterface>();

            AvaloniaLocator.CurrentMutable
                .Bind<IStandardCursorFactory>().ToSingleton<CursorFactory>()
                .Bind<IKeyboardDevice>().ToSingleton<KeyboardDevice>()
                .Bind<ISystemDialogImpl>().ToSingleton<SystemDialogImpl>()
                .Bind<IPlatformSettings>().ToSingleton<StridePlatform>()
                .Bind<IWindowingPlatform>().ToSingleton<StridePlatform>()
                .Bind<IPlatformIconLoader>().ToSingleton<PlatformIconLoader>()
                .Bind<PlatformHotkeyConfiguration>().ToSingleton<PlatformHotkeyConfiguration>()
                .Bind<IRenderLoop>().ToConstant(new RenderLoop())
                .Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60));

            WindowExtensions.Init();
        }

        static StridePlatform()
        {
            Window.WindowOpenedEvent.AddClassHandler(typeof(Window), OnWindowOpened);
        }

        private static void OnWindowOpened(object sender, RoutedEventArgs e)
        {

            var window = (Window)sender;
            if (!WindowExtensions.GetStrideInited(window))
            {
                var game = AvaloniaLocator.Current.GetService<IGame>();
                var scene = game.Services.GetService<SceneSystem>().SceneInstance.RootScene;

                var entity = new Entity();
                entity.Add(new AvaloniaComponent { Window = window });
                scene.Entities.Add(entity);

                WindowExtensions.SetStrideInited(window, true);
            }
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            throw new NotSupportedException();
        }

        public IWindowImpl CreateWindow() => new WindowImpl();
    }
}
