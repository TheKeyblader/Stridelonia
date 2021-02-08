using Avalonia;
using Avalonia.ReactiveUI;
using Merged.UI;

namespace Merged
{
    class MergedApp
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
