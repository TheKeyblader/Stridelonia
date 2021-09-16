using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Stridelonia;
using TodoApp.Views;

[assembly: AvaloniaConfigurator(typeof(TodoApp.Configurator))]

namespace TodoApp
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
                desktop.MainWindow = new MainWindow();
            }
            if (ApplicationLifetime is ISingleViewApplicationLifetime single)
            {
                single.MainView = new MainView();
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
                DrawFps = true,
            };
        }
    }
}
