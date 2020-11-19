using System.Diagnostics;
using SharpDX.Direct2D1;
using Stride.Engine;

namespace Stridelonia.Samples
{
    class CustomGame : Game
    {
        protected override void EndDraw(bool present)
        {
            StrideExternalRenderTarget.CriticalMutex.WaitOne();
            Debug.WriteLine("start rendering");
            base.EndDraw(present);
            Debug.WriteLine("end rendering");
            StrideExternalRenderTarget.CriticalMutex.ReleaseMutex();
        }
    }

    class Stridelonia_SamplesApp
    {
        static void Main(string[] args)
        {
            using (var game = new CustomGame())
            {
                game.GraphicsDeviceManager.DeviceCreationFlags = Stride.Graphics.DeviceCreationFlags.BgraSupport;
                game.Run();
            }
        }
    }
}
