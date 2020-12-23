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
using Avalonia.Threading;
using Stride.Engine;
using Stride.Games;
using Stride.Rendering;
using Stridelonia.Implementation;

namespace Avalonia
{
    public static class StrideApplicationExtensions
    {
        public static AppBuilder UseStride(this AppBuilder builder)
            => builder.UseWindowingSubsystem(() => Stridelonia.StridePlatform.Initialize(), "Stride");
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

            InitEvents();
        }

        private static void InitEvents()
        {
            WindowExtensions.RenderGroupProperty.Changed.Subscribe(e => ContainerManager.ChangeRenderGroup(((WindowImpl)((Window)e.Sender).PlatformImpl), (RenderGroup)e.NewValue.Value));
            WindowExtensions.ZIndexProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).ZIndex = e.NewValue.Value);
            WindowExtensions.Is2DProperty.Changed.Subscribe(e => ContainerManager.ChangeSpace(((WindowImpl)((Window)e.Sender).PlatformImpl), e.NewValue.Value));
            WindowExtensions.HasInputProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).HasInput = e.NewValue.Value);
            WindowExtensions.Position3DProperty.Changed.Subscribe(e => ContainerManager.ChangePosition(((WindowImpl)((Window)e.Sender).PlatformImpl), e.NewValue.Value));
            WindowExtensions.Rotation3DProperty.Changed.Subscribe(e => ContainerManager.ChangeRotation(((WindowImpl)((Window)e.Sender).PlatformImpl), e.NewValue.Value));
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            throw new NotSupportedException();
        }

        public IWindowImpl CreateWindow() => new WindowImpl();
    }
}
