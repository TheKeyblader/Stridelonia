using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;

namespace Stridelonia
{
    public interface IAvaloniaRenderer
    {
        Texture Texture { get; set; }

        ISpriteProvider SpriteProvider { get; }

        Size2 Size { get; set; }
    }

    public class GenericAvaloniaRenderer : IAvaloniaRenderer
    {
        public Texture Texture
        {
            get => ((SpriteFromTexture)SpriteProvider).Texture;
            set => ((SpriteFromTexture)SpriteProvider).Texture = value;
        }

        public Size2 Size { get; set; }

        public ISpriteProvider SpriteProvider { get; }

        public GenericAvaloniaRenderer()
        {
            SpriteProvider = new SpriteFromTexture
            {
                IsTransparent = true,
            };
        }
    }
}
