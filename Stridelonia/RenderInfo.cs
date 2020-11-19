using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DXGI;
using Stride.Graphics;

namespace Stridelonia
{
    public class RenderInfo
    {
        public Texture Texture { get; set; }
        public KeyedMutex Mutex { get; set; }
    }
}
