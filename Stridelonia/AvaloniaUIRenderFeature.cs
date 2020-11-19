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
using System.Diagnostics;

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
        public override Type SupportedRenderObjectType => typeof(FakeRenderObject);

        private ThreadLocal<ThreadContext> threadContext;
        private Thread _avaloniaThread;
        private EventWaitHandle _init;
        private CancellationTokenSource _lifetimeTokenSource;

        private StrideTopLevel _topLevel;
        private ScreenInput input;

        private static AvaloniaUIRenderFeature instance;

        protected override void InitializeCore()
        {
            base.InitializeCore();

            if (instance != null) throw new InvalidOperationException("Can have only one AvaloniaUI Feature");
            instance = this;

            threadContext = new ThreadLocal<ThreadContext>(() => new ThreadContext(Context.GraphicsDevice), true);

            var gameSytems = RenderSystem.Services.GetSafeServiceAs<IGameSystemCollection>();
            var dispatcher = new StrideDispatcher(RenderSystem.Services);
            RenderSystem.Services.AddService(dispatcher);
            gameSytems.Add(dispatcher);

            input = new ScreenInput(RenderSystem.Services);
            RenderSystem.Services.AddService(input);
            gameSytems.Add(input);

            var options = GetOptions();
            AvaloniaLocator.CurrentMutable.BindToSelf(options);

            _lifetimeTokenSource = new CancellationTokenSource();
            _init = new EventWaitHandle(false, EventResetMode.ManualReset);
            _avaloniaThread = new Thread(AvaloniaThread);
            _avaloniaThread.Name = "Avalonia Thread";
            _avaloniaThread.Start(options);
            _init.WaitOne();
            _init.Dispose();

        }

        public override void Unload()
        {
            base.Unload();

            _lifetimeTokenSource.Cancel();
            _topLevel.Dispose();
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

            var lifetime = new StrideLifetime();
            builder.AfterSetup(_ =>
            {
                var service = RenderSystem.Services.GetSafeServiceAs<IGraphicsDeviceService>();
                _topLevel = new StrideTopLevel(service);
                lifetime.TopLevel = _topLevel;
                input.TopLevel = _topLevel;

            });
            builder.SetupWithLifetime(lifetime);

            _init.Set();

            builder.Instance.Run(_lifetimeTokenSource.Token);
        }

        public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage)
        {
            base.Draw(context, renderView, renderViewStage);

            if (_topLevel.RenderInfo.Texture == null) return;

            StrideExternalRenderTarget.CriticalMutex.WaitOne();
            Debug.WriteLine("Start Drawing AvaloniaSprite");
            var batch = threadContext.Value;
            batch.SpriteBatch.Begin(context.GraphicsContext, blendState: BlendStates.AlphaBlend, depthStencilState: DepthStencilStates.None, rasterizerState: RasterizerStates.CullNone);
            batch.SpriteBatch.Draw(_topLevel.RenderInfo.Texture, new Vector2(), Color.White);
            batch.SpriteBatch.End();
            Debug.WriteLine("End Drawing AvaloniaSprite");
            StrideExternalRenderTarget.CriticalMutex.ReleaseMutex();
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

        class StrideLifetime : ISingleViewApplicationLifetime
        {
            public Control MainView { get => TopLevel.Content; set => TopLevel.Content = value; }
            public StrideTopLevel TopLevel { get; set; }
        }
        class ThreadContext : IDisposable
        {
            public SpriteBatch SpriteBatch { get; }

            public ThreadContext(GraphicsDevice device)
            {
                SpriteBatch = new SpriteBatch(device);
            }

            public void Dispose()
            {
                SpriteBatch.Dispose();
            }
        }
        class FakeRenderObject : RenderObject { }
    }
}
