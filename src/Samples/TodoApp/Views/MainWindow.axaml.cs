using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stridelonia;

namespace TodoApp.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            WindowExtensions.SetRenderGroup(this, 31);
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
