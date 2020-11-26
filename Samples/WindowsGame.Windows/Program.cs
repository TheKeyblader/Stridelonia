using Avalonia;
using Avalonia.ReactiveUI;

namespace WindowsGame
{

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, Avalonia.Controls.ShutdownMode.OnMainWindowClose);
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UseWin32()
                .UseDirect2D1()
                .UseReactiveUI()
                .LogToTrace();
    }
}

