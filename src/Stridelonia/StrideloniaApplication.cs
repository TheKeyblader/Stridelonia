using System;
using Avalonia;
using Stride.Games;
using Stride.Graphics;
using Stridelonia.Generic;
using Stridelonia.Input;

namespace Stridelonia
{
    public static class StrideloniaApplication
    {
        public static void Start(IGame game)
        {
            InitalizeRendering();

            game.GameSystems.Add(new PickingSystem(game.Services));

            AvaloniaManager.Initialize(game);
            AvaloniaManager.Run();
        }

        public static void Stop() => AvaloniaManager.Stop();

        private static void InitalizeRendering()
        {
            switch (GraphicsDevice.Platform)
            {
                default:
                    AvaloniaLocator.CurrentMutable.BindToSelf(new SkiaOptions { CustomGpuFactory = () => new GenericSkiaGpu() });
                    break;
            }
        }
    }
}
