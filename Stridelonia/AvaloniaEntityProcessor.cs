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
            Work();
        }

        private void Work()
        {
            foreach (var avaloniaStateKeyPair in ComponentDatas)
            {
                var avaloniaComponent = avaloniaStateKeyPair.Key;
                var platform = (WindowImpl)avaloniaComponent.Window.PlatformImpl;
                var renderWindow = avaloniaStateKeyPair.Value.RenderWindow;

                renderWindow.Enabled = platform.IsVisible;

                if (renderWindow.Enabled)
                {
                    renderWindow.RenderGroup = platform.RenderGroup;

                    renderWindow.Is2D = platform.Is2D;

                    renderWindow.Window = avaloniaComponent.Window;
                    renderWindow.WindowTexture = platform.Texture;

                    renderWindow.Topmost = platform.IsTopmost;
                    renderWindow.ZIndex = platform.ZIndex;

                    var position = platform.Position3D;
                    if (position != null)
                    {
                        avaloniaComponent.Entity.Transform.Position = position.Value;
                    }
                    else if (platform.Is2D)
                    {
                        avaloniaComponent.Entity.Transform.Position = new Vector3(platform.Position.ToStride(), 0);
                    }

                    var rotation = platform.Rotation3D;
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
