using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.UI;
using Stride.UI.Controls;
using Stridelonia.Implementation;
using Matrix = Stride.Core.Mathematics.Matrix;
using Point = Avalonia.Point;
using Thickness = Avalonia.Thickness;

namespace Stridelonia
{
    internal class WindowImpl : IWindowImpl
    {
        #region Avalonia
        private WindowState _windowState;
        public WindowState WindowState { get => _windowState; set => SetWindowState(value); }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Action GotInputWhenDisabled { get; set; }
        public Func<bool> Closing { get; set; }

        public bool IsClientAreaExtendedToDecorations => false;

        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        public bool NeedsManagedDecorations => true;

        public Thickness ExtendedMargins => new(0);

        public Thickness OffScreenMargin => new(0);

        public double DesktopScaling => 1;

        public PixelPoint Position { get; set; }

        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }

        public IPlatformHandle Handle => new PlatformHandle(IntPtr.Zero, "No Handle");

        private Size _minSize;
        private Size _maxSize;
        public Size MaxAutoSizeHint => _maxSize;

        public IScreenImpl Screen { get; }

        public Size ClientSize { get; private set; }

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

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.Transparent;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new();
        #endregion

        public IInputRoot InputRoot { get; private set; }

        #region Internal Data
        public RenderGroup RenderGroup { get; set; }

        public bool IsVisible
        {
            get => RenderingElement.IsVisible;
            set => RenderingElement.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }

        public bool IsTopmost { get; private set; }
        public short ZIndex
        {
            get => (short)RenderingElement.GetPanelZIndex();
            set => RenderingElement.SetPanelZIndex(value);
        }
        public bool Is2D { get; set; } = true;
        public bool HasInput { get; set; } = true;
        public Matrix WorldMatrix => ContainerManager.GetMatrix(this);
        #endregion

        private readonly IAvaloniaRenderer renderTarget;
        public ImageElement RenderingElement { get; }

        public WindowImpl()
        {
            Screen = new ScreenImpl();
            MouseDevice = new MouseDevice(new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true));

            renderTarget = CreateRenderer();
            Surfaces = new object[] { renderTarget };

            RenderingElement = new ImageElement
            {
                Source = renderTarget.SpriteProvider
            };

            ContainerManager.AddWindow(this);
        }

        private IAvaloniaRenderer CreateRenderer()
        {
            return GraphicsDevice.Platform switch
            {
                _ => new GenericAvaloniaRenderer(),
            };
        }

        public void Activate()
        {

        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void CanResize(bool value)
        {

        }

        public IPopupImpl CreatePopup() => null;

        public IRenderer CreateRenderer(IRenderRoot root)
        {
            var options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();
            var loop = AvaloniaLocator.Current.GetService<IRenderLoop>();
            IRenderer renderer;
            if (options.UseDeferredRendering)
                renderer = new DeferredRenderer(root, loop);
            else
                renderer = new ImmediateRenderer(root);

            if (options.DrawFps) renderer.DrawFps = true;
            return renderer;
        }

        public void Dispose()
        {

        }

        public void Hide()
        {
            IsVisible = false;
            ulong timestamp = (ulong)(Environment.TickCount & int.MaxValue);
            Input?.Invoke(new RawPointerEventArgs(MouseDevice, timestamp, InputRoot,
                RawPointerEventType.LeaveWindow, new Point(-1, -1), RawInputModifiers.None));
        }

        public void Invalidate(Rect rect)
        {
            Dispatcher.UIThread.Post(() =>
            {
                Paint?.Invoke(rect);
            });
        }

        public void Move(PixelPoint point)
        {
            Position = point;
            RenderingElement.SetCanvasAbsolutePosition(new Vector3(point.ToStride(), 0));
            PositionChanged?.Invoke(point);
        }

        public Point PointToClient(PixelPoint point)
        {
            return point.ToPoint(1);
        }

        public PixelPoint PointToScreen(Point point)
        {
            return PixelPoint.FromPoint(point, 1);
        }

        public void Resize(Size clientSize)
        {
            ClientSize = clientSize;
            renderTarget.Size = new Size2((int)clientSize.Width, (int)clientSize.Height);
            Resized?.Invoke(clientSize);
        }

        public void SetCursor(IPlatformHandle cursor)
        {

        }

        public void SetEnabled(bool enable)
        {
            RenderingElement.IsEnabled = enable;
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            throw new NotImplementedException();
        }

        public void SetIcon(IWindowIconImpl icon)
        {

        }

        public void SetInputRoot(IInputRoot inputRoot) => InputRoot = inputRoot;

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            _minSize = minSize;
            _maxSize = maxSize;
        }

        public void SetParent(IWindowImpl parent)
        {

        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {

        }

        public void SetTitle(string title)
        {
            RenderingElement.Name = title;
        }

        public void SetTopmost(bool value)
        {
            IsTopmost = value;
        }

        public void SetTransparencyLevelHint(WindowTransparencyLevel transparencyLevel)
        {

        }

        public void Show()
        {
            IsVisible = true;
            ulong timestamp = (ulong)(Environment.TickCount & int.MaxValue);
            Input?.Invoke(new RawPointerEventArgs(MouseDevice, timestamp, InputRoot,
                RawPointerEventType.LeaveWindow, new Point(-1, -1), RawInputModifiers.None));
        }

        public void ShowTaskbarIcon(bool value)
        {

        }

        private void SetWindowState(WindowState state)
        {
            _windowState = state;
            WindowStateChanged?.Invoke(state);
            switch (state)
            {
                case WindowState.FullScreen:
                case WindowState.Maximized:
                    Resize(Screen.AllScreens[0].Bounds.Size.ToSize(1));
                    Position = new PixelPoint(0, 0);
                    PositionChanged?.Invoke(Position);
                    break;
            }
        }

        public void Show(bool activate)
        {
            IsVisible = true;
            ulong timestamp = (ulong)(Environment.TickCount & int.MaxValue);
            Input?.Invoke(new RawPointerEventArgs(MouseDevice, timestamp, InputRoot,
                RawPointerEventType.LeaveWindow, new Point(-1, -1), RawInputModifiers.None));
        }
    }
}
