using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Stride.Graphics;
using Point = Avalonia.Point;
using Size = Avalonia.Size;

namespace Stridelonia
{
    /// <summary>
    /// Main single TopLevel
    /// </summary>
    public class StrideTopLevel : ITopLevelImpl
    {
        public Size ClientSize { get; set; }

        public double RenderScaling => 1;

        public IEnumerable<object> Surfaces { get; }

        public Action<RawInputEventArgs> Input { get; set; }
        public Action<Rect> Paint { get; set; }
        public Action<Size> Resized { get; set; }
        public Action<double> ScalingChanged { get; set; }
        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }
        public Action Closed { get; set; }
        public Action LostFocus { get; set; }

        public IMouseDevice MouseDevice { get; }
        public IInputRoot InputRoot { get; private set; }

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.Transparent;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new AcrylicPlatformCompensationLevels();

        private readonly EmbeddableControlRoot _root;
        private readonly StrideExternalRenderTarget _renderTarget;

        public RenderInfo RenderInfo => _renderTarget.RenderInfo;
        public Control Content
        {
            get => (Control)_root.Content;
            set => _root.Content = value;
        }

        public StrideTopLevel(IGraphicsDeviceService service)
        {
            MouseDevice = new MouseDevice(new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true));

            var options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();

            _renderTarget = new StrideExternalRenderTarget(service);
            Surfaces = new object[] { _renderTarget };

            var graphicsDevice = service.GraphicsDevice;
            ClientSize = new Size(graphicsDevice.Presenter.BackBuffer.Width, graphicsDevice.Presenter.BackBuffer.Height);
            _renderTarget.ClientSize = ClientSize;

            _root = new EmbeddableControlRoot(this)
            {
                TransparencyLevelHint = WindowTransparencyLevel.Transparent,
                Background = new SolidColorBrush(Colors.Transparent),
            };
            _root.Prepare();
            _root.Renderer.DrawFps = options.DrawFps;
            _root.Renderer.Start();
        }

        public IPopupImpl CreatePopup()
        {
            return null;
        }

        public IRenderer CreateRenderer(IRenderRoot root)
        {
            var options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();
            var renderLoop = AvaloniaLocator.Current.GetService<IRenderLoop>();
            if (options.UseDeferredRendering)
                return new DeferredRenderer(root, renderLoop);
            else
                return new ImmediateRenderer(root);
        }

        public void Invalidate(Rect rect)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Paint?.Invoke(rect);
            }, DispatcherPriority.Render);
        }

        public Point PointToClient(PixelPoint point)
        {
            return point.ToPoint(1);
        }

        public PixelPoint PointToScreen(Point point)
        {
            return new PixelPoint((int)point.X, (int)point.Y);
        }

        public void SetCursor(IPlatformHandle cursor)
        {
            //NO-OP
        }

        public void SetInputRoot(IInputRoot inputRoot)
        {
            InputRoot = inputRoot;
        }

        public void SetTransparencyLevelHint(WindowTransparencyLevel transparencyLevel)
        {
            //NO-OP
        }

        public void Resize(Size newClientSize)
        {
            _renderTarget.DestroyRenderTarget();
            ClientSize = newClientSize;
            Dispatcher.UIThread.Post(() =>
            {
                Resized?.Invoke(newClientSize);

            });
        }

        public void Dispose()
        {
            _renderTarget.DestroyRenderTarget();
        }
    }
}
