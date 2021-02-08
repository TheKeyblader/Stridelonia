using Avalonia;
using Avalonia.ReactiveUI;
using Split.UI;

namespace Split
{
    class SplitApp
    {
        static void Main(string[] args)
        {
            using var game = new Game();
            game.Run();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
