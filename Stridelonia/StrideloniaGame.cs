using System;
using Avalonia;
using Avalonia.Skia;
using Stride.Engine;
using Stride.Graphics;
using Stridelonia.Generic;
using Stridelonia.Input;

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

            InitalizeRendering();

            GameSystems.Add(new PickingSystem(Services));

            AvaloniaManager.Initialize(this);
            AvaloniaManager.Run();
        }

        private void InitalizeRendering()
        {
            switch (GraphicsDevice.Platform)
            {
                default:
                    AvaloniaLocator.CurrentMutable.BindToSelf(new SkiaOptions { CustomGpuFactory = () => new GenericSkiaGpu() });
                    break;
            }
        }

        protected override void EndRun()
        {
            base.EndRun();
            AvaloniaManager.Stop();
        }
    }
}
