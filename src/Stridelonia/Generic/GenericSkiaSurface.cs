using System;
using Avalonia;
using Avalonia.Skia;
using SkiaSharp;

namespace Stridelonia.Generic
{
    public class GenericSkiaSurface : ISkiaSurface
    {
        public SKSurface Surface { get; private set; }

        public bool CanBlit => false;

        public GenericSkiaSurface(PixelSize size)
        {
            Surface = SKSurface.Create(new SKImageInfo(size.Width, size.Height));
        }

        public void Blit(SKCanvas canvas) => throw new NotSupportedException();

        public void Dispose()
        {
            Surface.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
