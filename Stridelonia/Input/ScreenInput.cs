using System;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Stride.Core;
using Stride.Games;
using Stride.Input;
using Stride.Core.Mathematics;
using MouseButton = Stride.Input.MouseButton;
using InputManager = Stride.Input.InputManager;
using System.Linq;
using Avalonia;
using IKeyboardDevice = Avalonia.Input.IKeyboardDevice;
using System.Collections.Generic;

namespace Stridelonia.Input
{
    public class ScreenInput : GameSystemBase
    {
        public StrideTopLevel TopLevel { get; set; }

        private Vector2 oldPosition;
        private InputManager input;
        private IKeyboardDevice keyboard;

        public ScreenInput(IServiceRegistry registry)
            : base(registry)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            input = Services.GetService<InputManager>();
            input.TextInput?.EnabledTextInput();
            Enabled = true;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TopLevel == null) return;

            ulong timestamp = (ulong)(Environment.TickCount & int.MaxValue);
            var modifiers = GetRawInputModifiers();

            #region Mouse
            if (oldPosition != input.AbsoluteMousePosition)
            {
                oldPosition = input.AbsoluteMousePosition;
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.Move, oldPosition.ToAvalonia(), modifiers));
                });
            }

            if (input.IsMouseButtonPressed(MouseButton.Left))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.LeftButtonDown, oldPosition.ToAvalonia(), modifiers));
                });
            }
            if (input.IsMouseButtonReleased(MouseButton.Left))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.LeftButtonUp, oldPosition.ToAvalonia(), modifiers));
                });
            }

            if (input.IsMouseButtonPressed(MouseButton.Right))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.RightButtonDown, oldPosition.ToAvalonia(), modifiers));
                });
            }
            if (input.IsMouseButtonReleased(MouseButton.Right))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.RightButtonUp, oldPosition.ToAvalonia(), modifiers));
                });
            }

            if (input.IsMouseButtonPressed(MouseButton.Middle))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.MiddleButtonDown, oldPosition.ToAvalonia(), modifiers));
                });
            }
            if (input.IsMouseButtonReleased(MouseButton.Middle))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.MiddleButtonDown, oldPosition.ToAvalonia(), modifiers));
                });
            }

            if (input.IsMouseButtonPressed(MouseButton.Extended1))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.XButton1Down, oldPosition.ToAvalonia(), modifiers));
                });
            }
            if (input.IsMouseButtonReleased(MouseButton.Extended1))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.XButton1Up, oldPosition.ToAvalonia(), modifiers));
                });
            }

            if (input.IsMouseButtonPressed(MouseButton.Extended2))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.XButton2Down, oldPosition.ToAvalonia(), modifiers));
                });
            }
            if (input.IsMouseButtonReleased(MouseButton.Extended2))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawPointerEventArgs(TopLevel.MouseDevice, timestamp, TopLevel.InputRoot,
                        RawPointerEventType.XButton2Up, oldPosition.ToAvalonia(), modifiers));
                });
            }
            #endregion

            foreach (var _event in input.PressedKeys)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (!strideToAvalonia.TryGetValue(_event, out Key key))
                        key = (Key)_event;

                    TopLevel.Input?.Invoke(new RawKeyEventArgs(KeyboardDevice.Instance, timestamp, TopLevel.InputRoot,
                        RawKeyEventType.KeyDown, key, modifiers));
                });
            }

            foreach (var _event in input.ReleasedKeys)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (!strideToAvalonia.TryGetValue(_event, out Key key))
                        key = (Key)_event;

                    TopLevel.Input?.Invoke(new RawKeyEventArgs(KeyboardDevice.Instance, timestamp, TopLevel.InputRoot,
                        RawKeyEventType.KeyUp, key, modifiers));
                });
            }

            foreach (var _event in input.Events.OfType<TextInputEvent>())
            {
                Dispatcher.UIThread.Post(() =>
                {
                    TopLevel.Input?.Invoke(new RawTextInputEventArgs(KeyboardDevice.Instance, timestamp, TopLevel.InputRoot, _event.Text));
                });
            }
        }

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

        private readonly Dictionary<Keys, Key> strideToAvalonia = new Dictionary<Keys, Key>
        {

        };

        private readonly char[] forbiddenChar = new char[]
        {

        };
    }
}
