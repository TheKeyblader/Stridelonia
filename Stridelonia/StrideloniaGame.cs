using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Engine;

namespace Stridelonia
{
    public class StrideloniaGame : Game
    {
        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDeviceManager.DeviceCreationFlags |= Stride.Graphics.DeviceCreationFlags.BgraSupport;
        }

        protected override void BeginRun()
        {
            base.BeginRun();
            AvaloniaManager.Initialize(this);
            SharpDX.Configuration.EnableReleaseOnFinalizer = false;
            AvaloniaManager.Run();
        }

        protected override void EndRun()
        {
            base.EndRun();
            AvaloniaManager.Stop();
        }
    }
}
