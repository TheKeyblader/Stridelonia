using System.Diagnostics;
using SharpDX.Direct2D1;
using Stride.Engine;

namespace Stridelonia.Samples
{
    class Stridelonia_SamplesApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.GraphicsDeviceManager.DeviceCreationFlags = Stride.Graphics.DeviceCreationFlags.BgraSupport;
                game.Run();
            }
        }
    }
}
