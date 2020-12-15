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
using Stride.Core.Diagnostics;

namespace Stridelonia
{
    /// <summary>
    /// Define Avalonia configuration in Stride
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

        private SpriteBatch batch;
        private Sprite3DBatch batch3d;
        private PickingSystem picking;

        protected override void InitializeCore()
        {
            base.InitializeCore();

            if (Application.Current == null) return;

            batch = new SpriteBatch(Context.GraphicsDevice);
            batch3d = new Sprite3DBatch(Context.GraphicsDevice);

            picking = RenderSystem.Services.GetService<PickingSystem>();
            if (picking == null)
            {
                picking = new PickingSystem(RenderSystem.Services);
                RenderSystem.Services.AddService(picking);
            }
        }

        public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage, int startIndex, int endIndex)
        {
            base.Draw(context, renderView, renderViewStage, startIndex, endIndex);

            if (Application.Current == null) return;

            var cameraComponent = context.RenderContext.Tags.Get(CameraComponentRendererExtensions.Current);
            if (cameraComponent != null)
            {
                picking.Camera = cameraComponent;
                picking.Update();
            }

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
    }
}
