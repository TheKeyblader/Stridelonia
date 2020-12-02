using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Stride.Core.Mathematics;
using Stride.Rendering;

namespace Stridelonia
{
    public class WindowExtensions : AvaloniaObject
    {
        public static readonly AttachedProperty<RenderGroup> RenderGroupProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, RenderGroup>(
            "RenderGroup", RenderGroup.Group0, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<int> ZIndexProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, int>(
            "ZIndex", 0, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<bool> Is2DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, bool>(
            "Is2D", true, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<bool> HasInputProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, bool>(
            "HasInput", true, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<Vector3?> Position3DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, Vector3?>(
            "3DPosition", default, false, BindingMode.TwoWay);

        public static readonly AttachedProperty<Quaternion?> Rotation3DProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, Quaternion?>(
            "3DRotation", default, false, BindingMode.TwoWay);

        internal static void Init()
        {
            RenderGroupProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).RenderGroup = e.NewValue.Value);
            ZIndexProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).ZIndex = e.NewValue.Value);
            Is2DProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).Is2D = e.NewValue.Value);
            HasInputProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).HasInput = e.NewValue.Value);
            Position3DProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).Position3D = e.NewValue.Value);
            Rotation3DProperty.Changed.Subscribe(e => ((WindowImpl)((Window)e.Sender).PlatformImpl).Rotation3D = e.NewValue.Value);
        }

        public static void SetRenderGroup(Window window, RenderGroup value) => window.SetValue(RenderGroupProperty, value);

        public static RenderGroup GetRenderGroup(Window window) => window.GetValue(RenderGroupProperty);

        public static void SetZIndex(Window window, int value) => window.SetValue(ZIndexProperty, value);

        public static int GetZIndex(Window window) => window.GetValue(ZIndexProperty);

        public static void SetIs2D(Window window, bool value) => window.SetValue(Is2DProperty, value);

        public static bool GetIs2D(Window window) => window.GetValue(Is2DProperty);

        public static void SetHasInput(Window window, bool value) => window.SetValue(HasInputProperty, value);

        public static bool GetHasInput(Window window) => window.GetValue(HasInputProperty);

        public static void Set3DPosition(Window window, Vector3? value) => window.SetValue(Position3DProperty, value);

        public static Vector3? Get3DPosition(Window window) => window.GetValue(Position3DProperty);

        public static void Set3DRotation(Window window, Quaternion? value) => window.SetValue(Rotation3DProperty, value);

        public static Quaternion? Get3DRotation(Window window) => window.GetValue(Rotation3DProperty);



        internal static AttachedProperty<bool> StrideInitedProperty = AvaloniaProperty.RegisterAttached<WindowExtensions, Window, bool>(
            "StrideInited", false);

        internal static void SetStrideInited(Window window, bool value) => window.SetValue(StrideInitedProperty, value);

        internal static bool GetStrideInited(Window window) => window.GetValue(StrideInitedProperty);
    }
}
