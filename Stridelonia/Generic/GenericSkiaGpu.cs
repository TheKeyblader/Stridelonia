using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Skia;

namespace Stridelonia.Generic
{
    internal class GenericSkiaGpu : ISkiaGpu
    {
        public ISkiaGpuRenderTarget TryCreateRenderTarget(IEnumerable<object> surfaces)
        {
            foreach (var surface in surfaces)
            {
                if (surface is GenericAvaloniaRenderer generic)
                    return new GenericSkiaRenderer(generic);
            }

            return null;
        }

        public ISkiaSurface TryCreateSurface(PixelSize size, ISkiaGpuRenderSession session)
        {
            return new GenericSkiaSurface(size);
        }
    }
}
