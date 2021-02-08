using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Stridelonia;
using WindowsGame.ViewModels;
using WindowsGame.Views;

[assembly: AvaloniaConfigurator(typeof(WindowsGame.Configurator))]

namespace WindowsGame
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class Configurator
    {
        public StridePlatformOptions Initialize()
        {
            return new StridePlatformOptions
            {
                ApplicationType = typeof(App),
                ConfigureApp = (builder) => builder.UseReactiveUI().LogToTrace(),
                UseMultiThreading = true,
                UseDeferredRendering = false,
                DrawFps = true
            };
        }
    }
}
