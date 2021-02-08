using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Rendering;
using Stride.UI.Controls;
using Stride.UI.Panels;
using Matrix = Stride.Core.Mathematics.Matrix;

namespace Stridelonia
{
    internal static class ContainerManager
    {
        public static Entity AvaloniaEntity { get; }
        public static Entity Container3D { get; }
        public static Entity Container2D { get; }
        public static Dictionary<RenderGroup, UIComponent> ScreenPages { get; }
        public static Dictionary<WindowImpl, SpriteComponent> WorldPages { get; }

        static ContainerManager()
        {
            AvaloniaEntity = new("Avalonia");
            Container3D = new("3D");
            Container2D = new("2D");
            AvaloniaEntity.AddChild(Container3D);
            AvaloniaEntity.AddChild(Container2D);

            WorldPages = new();
            ScreenPages = new();

            var game = AvaloniaLocator.Current.GetService<IGame>();
            var scene = game.Services.GetService<SceneSystem>();
            scene.SceneInstance.RootScene.Entities.Add(AvaloniaEntity);
        }

        private static UIComponent Create2DUIPage(RenderGroup renderGroup)
        {
            var entity = new Entity("Avalonia Screen Page " + renderGroup);
            var uiPage = new UIComponent
            {
                RenderGroup = renderGroup,
                Page = new UIPage
                {
                    Name = "Screen Page " + renderGroup,
                    RootElement = new Canvas()
                }
            };

            var game = AvaloniaLocator.Current.GetService<IGame>();
            var width = game.GraphicsDevice.Presenter.Description.BackBufferWidth;
            var height = game.GraphicsDevice.Presenter.Description.BackBufferHeight;

            uiPage.Resolution = new Vector3(width, height, 1000);

            ScreenPages.Add(renderGroup, uiPage);
            entity.Add(uiPage);
            Container2D.AddChild(entity);
            return uiPage;
        }

        private static SpriteComponent Create3DUIPage(WindowImpl window)
        {
            var entity = new Entity("Avalonia Screen Page " + window.RenderGroup);
            var sprite = new SpriteComponent
            {
                RenderGroup = window.RenderGroup,
                SpriteProvider = window.RenderingElement.Source
            };
            WorldPages.Add(window, sprite);
            entity.Add(sprite);
            Container3D.AddChild(entity);
            return sprite;
        }

        private static UIComponent GetUIPage(RenderGroup renderGroup)
        {
            if (ScreenPages.TryGetValue(renderGroup, out var uiPage)) return uiPage;
            else return Create2DUIPage(renderGroup);
        }

        public static SpriteComponent GetSprite(WindowImpl window)
        {
            if (WorldPages.TryGetValue(window, out var sprite)) return sprite;
            else return Create3DUIPage(window);
        }

        public static void AddWindow(WindowImpl window)
        {
            if (window.Is2D)
            {
                var uiPage = GetUIPage(window.RenderGroup);
                var canvas = (Canvas)uiPage.Page.RootElement;
                canvas.Children.Add(window.RenderingElement);
            }
            else
            {
                _ = Create3DUIPage(window);
            }
        }

        public static void RemoveWindow(WindowImpl window)
        {
            if (window.Is2D)
            {
                var uiPage = GetUIPage(window.RenderGroup);
                var canvas = (Canvas)uiPage.Page.RootElement;
                canvas.Children.Remove(window.RenderingElement);
            }
            else
            {
                var uiPage = GetSprite(window);
                WorldPages.Remove(window);
                Container3D.RemoveChild(uiPage.Entity);
                uiPage.Entity.Dispose();
            }
        }

        public static void ChangeRenderGroup(WindowImpl window, RenderGroup newRenderGroup)
        {
            if (window.Is2D)
            {
                RemoveWindow(window);
                window.RenderGroup = newRenderGroup;
                AddWindow(window);
            }
            else
            {
                var sprite = GetSprite(window);
                window.RenderGroup = newRenderGroup;
                sprite.RenderGroup = newRenderGroup;
            }
        }

        public static void ChangeSpace(WindowImpl window, bool newIs2D)
        {
            if (window.Is2D == newIs2D) return;
            RemoveWindow(window);
            window.Is2D = newIs2D;
            AddWindow(window);
        }

        public static void ChangePosition(WindowImpl window, Vector3? newPosition)
        {
            var sprite = GetSprite(window);
            sprite.Entity.Transform.Position = newPosition ?? new Vector3();
        }

        public static void ChangeRotation(WindowImpl window, Quaternion? newRotation)
        {
            var sprite = GetSprite(window);
            sprite.Entity.Transform.Rotation = newRotation ?? new Quaternion();
        }

        public static Matrix GetMatrix(WindowImpl window)
        {
            if (window.Is2D) throw new InvalidOperationException();
            var sprite = GetSprite(window);
            return sprite.Entity.Transform.WorldMatrix;
        }
    }
}
