using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Input;
using IMouseDevice = Stride.Input.IMouseDevice;
using InputManager = Stride.Input.InputManager;
using Matrix = Stride.Core.Mathematics.Matrix;
using MouseButton = Stride.Input.MouseButton;

namespace Stridelonia.Input
{
    internal class PickingSystem
    {
        private IEnumerable<WindowImpl> all;
        private readonly InputManager input;

        private WindowImpl focusedWindow;
        private WindowImpl hoveredWindow;
        private Vector2 lastMousePosition;
        private Ray cameraRay;

        public CameraComponent Camera { get; set; }

        public PickingSystem(IServiceRegistry registry)
        {
            input = registry.GetService<InputManager>();
            input.TextInput?.EnabledTextInput();
        }

        private ulong Timestamp => (ulong)(Environment.TickCount & int.MaxValue);

        public void Update()
        {
            all = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime)
                .Windows.Select(w => (WindowImpl)w.PlatformImpl);

            if (all.Count(w => w.IsVisible) == 0) return;

            var modifiers = GetRawInputModifiers();

            foreach (var _event in input.Events)
            {
                if (_event is PointerEvent pointerEvent)
                {
                    if (pointerEvent.EventType == PointerEventType.Pressed)
                    {
                        var newFocusedWindow = Get2DWindow(lastMousePosition) ?? Get3DWindow();

                        if (focusedWindow != newFocusedWindow)
                        {
                            if (focusedWindow != null)
                            {
                                var tosave = focusedWindow;
                                Dispatcher.UIThread.Post(() =>
                                {
                                    tosave.Deactivated?.Invoke();
                                    tosave.LostFocus?.Invoke();
                                }, DispatcherPriority.Input);
                            }

                            focusedWindow = newFocusedWindow;

                            if (focusedWindow != null)
                            {
                                var tosave = focusedWindow;
                                Dispatcher.UIThread.Post(() =>
                                {
                                    tosave.Activated?.Invoke();
                                }, DispatcherPriority.Input);
                            }
                        }
                    }

                    if (pointerEvent.Device is IMouseDevice)
                    {
                        if (pointerEvent.EventType == PointerEventType.Moved)
                        {
                            lastMousePosition = pointerEvent.AbsolutePosition;
                            CalculateRay(lastMousePosition);

                            var newHoveredWindow = Get2DWindow(lastMousePosition) ?? Get3DWindow();

                            if (hoveredWindow != newHoveredWindow)
                            {
                                if (hoveredWindow != null)
                                {
                                    var inputRoot = hoveredWindow.InputRoot;
                                    SendEvents(hoveredWindow, new RawPointerEventArgs(hoveredWindow.MouseDevice, Timestamp,
                                        inputRoot, RawPointerEventType.LeaveWindow, new Avalonia.Point(-1, -1), modifiers));
                                }

                                hoveredWindow = newHoveredWindow;
                            }

                            if (hoveredWindow != null)
                            {
                                var position = ScreenToWindowPoint(hoveredWindow, pointerEvent.AbsolutePosition);
                                var inputRoot = hoveredWindow.InputRoot;
                                SendEvents(hoveredWindow, new RawPointerEventArgs(hoveredWindow.MouseDevice, Timestamp,
                                    inputRoot, RawPointerEventType.Move, position.ToAvalonia(), modifiers));
                            }
                        }
                    }
                }
                else if (_event is MouseButtonEvent mouseEvent && focusedWindow != null)
                {
                    var position = ScreenToWindowPoint(focusedWindow, lastMousePosition);
                    SendEvents(focusedWindow, new RawPointerEventArgs(focusedWindow.MouseDevice, Timestamp, focusedWindow.InputRoot, ToAvalonia(mouseEvent.Button, mouseEvent.IsDown),
                        position.ToAvalonia(), modifiers));
                }
                else if (_event is KeyEvent keyEvent && focusedWindow != null && keyEvent.RepeatCount == 0)
                {
                    if (!strideToAvalonia.TryGetValue(keyEvent.Key, out Key key))
                        key = (Key)keyEvent.Key;
                    SendEvents(focusedWindow, new RawKeyEventArgs(KeyboardDevice.Instance, Timestamp, focusedWindow.InputRoot,
                        keyEvent.IsDown ? RawKeyEventType.KeyDown : RawKeyEventType.KeyUp, key, modifiers));
                }
                else if (_event is TextInputEvent textEvent && focusedWindow != null)
                {
                    SendEvents(focusedWindow, new RawTextInputEventArgs(KeyboardDevice.Instance, Timestamp, focusedWindow.InputRoot, textEvent.Text));
                }
            }
        }

        private void CalculateRay(Vector2 screenPoint)
        {
            var viewport = new Viewport(0, 0, 1280, 720);

            var unprojectedNear =
                viewport.Unproject(
                    new Vector3(screenPoint, 0.0f),
                    Camera.ProjectionMatrix,
                    Camera.ViewMatrix,
                    Matrix.Identity);

            var unprojectedFar =
                viewport.Unproject(
                    new Vector3(screenPoint, 1.0f),
                    Camera.ProjectionMatrix,
                    Camera.ViewMatrix,
                    Matrix.Identity);

            var vectorDir = unprojectedFar - unprojectedNear;
            vectorDir.Normalize();

            cameraRay = new Ray(unprojectedNear, vectorDir);
        }

        private WindowImpl Get2DWindow(Vector2 pos)
        {
            var windows = all.Where(w => w.IsVisible && w.Is2D && w.HasInput);
            foreach (var window in windows
                .OrderByDescending(w => w.IsTopmost)
                .ThenByDescending(w => w.ZIndex))
            {
                var position = window.Position.ToStride();
                var size = window.ClientSize.ToStride();
                var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
                if (rect.Contains(pos))
                    return window;
            }
            return null;
        }

        private WindowImpl Get3DWindow()
        {
            var windows = all.Where(w => w.IsVisible && !w.Is2D && w.HasInput);

            foreach (var window in windows
                .OrderByDescending(w => w.IsTopmost)
                .ThenByDescending(w => w.ZIndex))
            {
                var size = new Vector3((window.ClientSize / 100).ToStride(), 0);
                var matrix = window.WorldMatrix;

                if (CollisionHelper.RayIntersectsRectangle(ref cameraRay, ref matrix, ref size, 2, out _))
                    return window;
            }

            return null;
        }

        private Vector2 ScreenToWindowPoint(WindowImpl window, Vector2 screenPoint)
        {
            if (window.Is2D)
            {
                return screenPoint - window.Position.ToStride();
            }
            else
            {
                var size = new Vector3((window.ClientSize / 100).ToStride(), 0);
                var matrix = window.WorldMatrix;

                if (CollisionHelper.RayIntersectsRectangle(ref cameraRay, ref matrix, ref size, 2, out var intersectionPoint))
                {
                    matrix.Decompose(out _, out Matrix viewMatrix, out var translation);
                    viewMatrix.Transpose();
                    Vector3.TransformCoordinate(ref translation, ref viewMatrix, out translation);
                    viewMatrix.TranslationVector = -translation;

                    var projectionMatrix = Matrix.OrthoRH(size.X, size.Y, 0, 0);
                    Matrix.Multiply(ref viewMatrix, ref projectionMatrix, out var viewProjectMatrix);

                    Vector3.TransformCoordinate(ref intersectionPoint, ref viewProjectMatrix, out var clipPoint);
                    Vector3.TransformCoordinate(ref intersectionPoint, ref viewMatrix, out var viewSpace);

                    var windowPoint = new Vector3
                    {
                        X = (clipPoint.X + 1f) / 2f,
                        Y = (clipPoint.Y + 1f) / 2f,
                        Z = viewSpace.Z
                    };

                    return (windowPoint.XY() * window.ClientSize.ToStride());
                }
                else
                {
                    return new Vector2(-1, -1);
                }

            }
        }

        private void SendEvents(WindowImpl window, RawInputEventArgs args)
        {
            Dispatcher.UIThread.Post(() =>
            {
                window.Input?.Invoke(args);
            }, DispatcherPriority.Input);
        }

        private RawPointerEventType ToAvalonia(MouseButton type, bool isDown)
        {
            RawPointerEventType? rawType = null;
            switch (type)
            {
                case MouseButton.Left:
                    rawType = isDown ? RawPointerEventType.LeftButtonDown : RawPointerEventType.LeftButtonUp;
                    break;
                case MouseButton.Right:
                    rawType = isDown ? RawPointerEventType.RightButtonDown : RawPointerEventType.RightButtonUp;
                    break;
                case MouseButton.Middle:
                    rawType = isDown ? RawPointerEventType.MiddleButtonDown : RawPointerEventType.MiddleButtonUp;
                    break;
                case MouseButton.Extended1:
                    rawType = isDown ? RawPointerEventType.XButton1Down : RawPointerEventType.XButton1Up;
                    break;
                case MouseButton.Extended2:
                    rawType = isDown ? RawPointerEventType.XButton2Down : RawPointerEventType.XButton2Up;
                    break;
            }
            return rawType.Value;
        }

        private readonly Dictionary<Keys, Key> strideToAvalonia = new Dictionary<Keys, Key>
        {

        };

        private RawInputModifiers GetRawInputModifiers()
        {
            var modifiers = RawInputModifiers.None;

            if (input.IsMouseButtonDown(MouseButton.Left)) modifiers |= RawInputModifiers.LeftMouseButton;
            if (input.IsMouseButtonDown(MouseButton.Right)) modifiers |= RawInputModifiers.RightMouseButton;
            if (input.IsMouseButtonDown(MouseButton.Middle)) modifiers |= RawInputModifiers.MiddleMouseButton;
            if (input.IsMouseButtonDown(MouseButton.Extended1)) modifiers |= RawInputModifiers.XButton1MouseButton;
            if (input.IsMouseButtonDown(MouseButton.Extended2)) modifiers |= RawInputModifiers.XButton2MouseButton;

            if (input.IsKeyDown(Keys.LeftAlt) || input.IsKeyDown(Keys.RightAlt)) modifiers |= RawInputModifiers.Alt;
            if (input.IsKeyDown(Keys.LeftCtrl) || input.IsKeyDown(Keys.RightCtrl)) modifiers |= RawInputModifiers.Control;
            if (input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.RightShift)) modifiers |= RawInputModifiers.Shift;
            if (input.IsKeyDown(Keys.LeftWin) || input.IsKeyDown(Keys.RightWin)) modifiers |= RawInputModifiers.Meta;

            return modifiers;
        }
    }
}
