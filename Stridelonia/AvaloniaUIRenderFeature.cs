using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Stride.Core;
using Stride.Games;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Core.Mathematics;
using Stridelonia.Input;
using System.Collections.Generic;
using Avalonia.Threading;
using Stride.Core.Extensions;

namespace Stridelonia
{
    /// <summary>
    /// Define Avalonia configuration in Stride
    /// <code>
    /// 
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AvaloniaConfiguratorAttribute : Attribute
    {
        public AvaloniaConfiguratorAttribute(Type configuratorType)
        {
            ConfiguratorType = configuratorType;
        }

        public Type ConfiguratorType { get; private set; }
    }

    public class AvaloniaUIRenderFeature : RootRenderFeature
    {
        public override Type SupportedRenderObjectType => typeof(RenderAvaloniaWindow);

        public StridePlatformOptions Options { get; }

        private Thread _avaloniaThread;
        private EventWaitHandle _init;

        private SpriteBatch batch;
        private Sprite3DBatch batch3d;

        public AvaloniaUIRenderFeature() : base()
        {
            Options = AvaloniaLocator.Current.GetService<StridePlatformOptions>() ?? GetOptions();
            AvaloniaLocator.CurrentMutable.BindToSelf(Options);
        }

        protected override void InitializeCore()
        {
            base.InitializeCore();

            AvaloniaLocator.CurrentMutable.BindToSelf(RenderSystem.Services.GetService<IGame>());

            var dispatcher = RenderSystem.Services.GetService<StrideDispatcher>();
            if (dispatcher == null)
            {
                var gameSytems = RenderSystem.Services.GetSafeServiceAs<IGameSystemCollection>();
                dispatcher = new StrideDispatcher(RenderSystem.Services);
                RenderSystem.Services.AddService(dispatcher);
                gameSytems.Add(dispatcher);
            }

            batch = new SpriteBatch(Context.GraphicsDevice);
            batch3d = new Sprite3DBatch(Context.GraphicsDevice);

            if (Application.Current == null) StartAvalonia();

            var picking = RenderSystem.Services.GetService<PickingSystem>();
            if (picking == null)
            {
                var gameSytems = RenderSystem.Services.GetSafeServiceAs<IGameSystemCollection>();
                picking = new PickingSystem(RenderSystem.Services);
                RenderSystem.Services.AddService(picking);
                gameSytems.Add(picking);
            }
        }

        public override void Unload()
        {
            base.Unload();

            if (Application.Current.ApplicationLifetime is IControlledApplicationLifetime controlledLifetime)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    controlledLifetime.Shutdown();
                });
            }
        }

        public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage, int startIndex, int endIndex)
        {
            base.Draw(context, renderView, renderViewStage, startIndex, endIndex);

            var windows = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                .Windows.Select(w => (WindowImpl)w.PlatformImpl);
            var sortedWindows = windows.Where(w => w.IsVisible)
                .OrderByDescending(w => w.IsTopmost).ThenByDescending(w => w.ZIndex);

            batch.Begin(context.GraphicsContext, SpriteSortMode.BackToFront, blendState: BlendStates.AlphaBlend, depthStencilState: DepthStencilStates.None, rasterizerState: RasterizerStates.CullNone);
            batch3d.Begin(context.GraphicsContext, renderView.ViewProjection, SpriteSortMode.BackToFront, BlendStates.AlphaBlend, null, DepthStencilStates.None, RasterizerStates.CullNone);
            for (var index = startIndex; index < endIndex; index++)
            {
                var renderNodeReference = renderViewStage.SortedRenderNodes[index].RenderNode;
                var renderNode = GetRenderNode(renderNodeReference);

                var renderWindow = (RenderAvaloniaWindow)renderNode.RenderObject;

                var depth = sortedWindows.Reverse().IndexOf(w => w == renderWindow.Window.PlatformImpl);

                if (renderWindow.Is2D)
                {
                    batch.Draw(renderWindow.WindowTexture, renderWindow.WorldMatrix.TranslationVector.XY(), Color.White, 0, new Vector2(0, 0),
                        layerDepth: depth);
                }
                else
                {
                    var sourceRect = new RectangleF(0, 0, renderWindow.WindowTexture.Width, renderWindow.WindowTexture.Height);
                    var size = new Vector2(sourceRect.Width, sourceRect.Height) / 100;
                    var color = Color4.White;
                    batch3d.Draw(renderWindow.WindowTexture, ref renderWindow.WorldMatrix, ref sourceRect, ref size, ref color, depth: depth);
                }
            }
            batch3d.End();
            batch.End();
        }

        private void StartAvalonia()
        {
            if (Options.UseMultiThreading)
            {
                _init = new EventWaitHandle(false, EventResetMode.ManualReset);
                _avaloniaThread = new Thread(AvaloniaThread);
                _avaloniaThread.Name = "Avalonia Thread";
                _avaloniaThread.Start(Options);
                _init.WaitOne();
                _init.Dispose();
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
                lifetime.Start(Environment.GetCommandLineArgs());
            }
        }

        private void AvaloniaThread(object parameter)
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

            _init.Set();

            lifetime.Start(Environment.GetCommandLineArgs());
        }

        private StridePlatformOptions GetOptions()
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
    }
}
