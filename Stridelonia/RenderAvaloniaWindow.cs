using Stride.Graphics;
using Stride.Rendering;
using Stride.Core.Mathematics;
using Avalonia.Controls;

namespace Stridelonia
{
    public class RenderAvaloniaWindow : RenderObject
    {
        public Matrix WorldMatrix;
        public int ZIndex;
        public bool Topmost;
        public bool Is2D;
        public Texture WindowTexture;
        public Window Window;
    }
}
