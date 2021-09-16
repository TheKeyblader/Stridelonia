using System.Collections.Generic;
using Avalonia;
using Avalonia.Platform;
using Stride.Games;
using Stride.Graphics;

namespace Stridelonia.Implementation
{
    internal class ScreenImpl : IScreenImpl
    {

        public ScreenImpl()
        {
            var game = AvaloniaLocator.Current.GetService<IGame>();
            ScreenCount = 1;
            var size = new PixelRect(0, 0, game.GraphicsDevice.Presenter.BackBuffer.Width, game.GraphicsDevice.Presenter.BackBuffer.Height);
            screens = new List<Screen>
            {
                new Screen(96,size,size,true)
            };
        }

        public int ScreenCount { get; }

        private readonly List<Screen> screens;
        public IReadOnlyList<Screen> AllScreens => screens;
    }
}
