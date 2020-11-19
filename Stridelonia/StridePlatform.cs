using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
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
            AvaloniaLocator.CurrentMutable
                .Bind<IStandardCursorFactory>().ToSingleton<CursorFactory>()
                .Bind<IKeyboardDevice>().ToSingleton<KeyboardDevice>()
                .Bind<ISystemDialogImpl>().ToSingleton<SystemDialogImpl>()
                .Bind<IPlatformSettings>().ToSingleton<StridePlatform>()
                .Bind<IWindowingPlatform>().ToSingleton<StridePlatform>()
                .Bind<IPlatformIconLoader>().ToSingleton<PlatformIconLoader>()
                .Bind<IPlatformThreadingInterface>().ToSingleton<StridePlatformThreadingInterface>()
                .Bind<PlatformHotkeyConfiguration>().ToSingleton<PlatformHotkeyConfiguration>()
                .Bind<IRenderLoop>().ToConstant(new RenderLoop())
                .Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //Do Clipboard
            }
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            throw new NotSupportedException();
        }

        public IWindowImpl CreateWindow()
        {
            throw new NotSupportedException();
        }
    }
}
