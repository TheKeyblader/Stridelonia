using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Stride.Core.Mathematics;
using Stridelonia.Converters;

namespace Stridelonia
{
    public class WindowExtensions : AvaloniaObject
    {
        public static readonly AttachedProperty<uint> RenderGroupProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, uint>(
            "RenderGroup", 0, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<short> ZIndexProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, short>(
            "ZIndex", 0, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<bool> Is2DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, bool>(
            "Is2D", true, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<bool> HasInputProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, bool>(
            "HasInput", true, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<Vector3> Position3DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, Vector3>(
            "Position3D", default, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<Quaternion> Rotation3DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, Quaternion>(
            "Rotation3D", default, false, BindingMode.TwoWay);

        public static void SetRenderGroup(Window window, uint value) => window.SetValue(RenderGroupProperty, value);

        public static uint GetRenderGroup(Window window) => window.GetValue(RenderGroupProperty);

        public static void SetZIndex(Window window, short value) => window.SetValue(ZIndexProperty, value);

        public static short GetZIndex(Window window) => window.GetValue(ZIndexProperty);

        public static void SetIs2D(Window window, bool value) => window.SetValue(Is2DProperty, value);

        public static bool GetIs2D(Window window) => window.GetValue(Is2DProperty);

        public static void SetHasInput(Window window, bool value) => window.SetValue(HasInputProperty, value);

        public static bool GetHasInput(Window window) => window.GetValue(HasInputProperty);

        public static void SetPosition3D(Window window, Vector3 value) => window.SetValue(Position3DProperty, value);

        public static Vector3 GetPosition3D(Window window) => window.GetValue(Position3DProperty);

        public static void SetRotation3D(Window window, Quaternion value) => window.SetValue(Rotation3DProperty, value);

        public static Quaternion GetRotation3D(Window window) => window.GetValue(Rotation3DProperty);
    }
}
