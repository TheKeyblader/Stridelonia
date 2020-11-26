using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;

namespace Stridelonia
{
    internal class AvaloniaEntityProcessor : EntityProcessor<AvaloniaComponent, AvaloniaEntityProcessor.WindowInfo>, IEntityComponentRenderProcessor
    {
        public VisibilityGroup VisibilityGroup { get; set; }

        public AvaloniaEntityProcessor()
            : base(typeof(TransformComponent))
        {

        }

        public override void Draw(RenderContext context)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                Work();
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(Work, DispatcherPriority.MaxValue).Wait();
            }
        }

        private void Work()
        {
            foreach (var avaloniaStateKeyPair in ComponentDatas)
            {
                var avaloniaComponent = avaloniaStateKeyPair.Key;
                var renderWindow = avaloniaStateKeyPair.Value.RenderWindow;

                renderWindow.Enabled = avaloniaComponent.Window.IsVisible;

                if (renderWindow.Enabled)
                {
                    renderWindow.RenderGroup = WindowExtensions.GetRenderGroup(avaloniaComponent.Window);

                    renderWindow.Is2D = WindowExtensions.GetIs2D(avaloniaComponent.Window);

                    renderWindow.Window = avaloniaComponent.Window;
                    renderWindow.WindowTexture = ((WindowImpl)avaloniaComponent.Window.PlatformImpl).Texture;

                    renderWindow.Topmost = avaloniaComponent.Window.Topmost;
                    renderWindow.ZIndex = WindowExtensions.GetZIndex(avaloniaComponent.Window);

                    var position = WindowExtensions.Get3DPosition(avaloniaComponent.Window);
                    if (position != null)
                    {
                        avaloniaComponent.Window.Position = new Avalonia.PixelPoint((int)position.Value.X, (int)position.Value.Y);
                        avaloniaComponent.Entity.Transform.Position = position.Value;
                    }
                    else if (WindowExtensions.GetIs2D(avaloniaComponent.Window))
                    {
                        avaloniaComponent.Entity.Transform.Position = new Vector3(avaloniaComponent.Window.Position.ToStride(), 0);
                    }

                    var rotation = WindowExtensions.Get3DRotation(avaloniaComponent.Window);
                    if (rotation != null)
                    {
                        avaloniaComponent.Entity.Transform.Rotation = rotation.Value;
                    }

                    avaloniaComponent.Entity.Transform.UpdateWorldMatrix();
                    renderWindow.WorldMatrix = avaloniaComponent.Entity.Transform.WorldMatrix;
                }

                var isActive = (renderWindow.WindowTexture != null) && renderWindow.Enabled;
                if (avaloniaStateKeyPair.Value.Active != isActive)
                {
                    avaloniaStateKeyPair.Value.Active = isActive;
                    if (isActive)
                        VisibilityGroup.RenderObjects.Add(renderWindow);
                    else
                        VisibilityGroup.RenderObjects.Remove(renderWindow);
                }
            }
        }

        protected override void OnEntityComponentRemoved(Entity entity, AvaloniaComponent component, WindowInfo data)
        {
            VisibilityGroup.RenderObjects.Remove(data.RenderWindow);
        }

        protected override WindowInfo GenerateComponentData(Entity entity, AvaloniaComponent component)
        {
            return new WindowInfo { RenderWindow = new RenderAvaloniaWindow { Source = component } };
        }

        protected override bool IsAssociatedDataValid(Entity entity, AvaloniaComponent component, WindowInfo associatedData)
        {
            return associatedData.RenderWindow.Source == component;
        }

        public class WindowInfo
        {
            public bool Active;
            public RenderAvaloniaWindow RenderWindow;
        }
    }
}
