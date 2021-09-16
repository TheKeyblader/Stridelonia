using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Core.Mathematics;
using Stridelonia;

namespace WindowsGame.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            var random = new Random();
            foreach (var i in Enumerable.Range(0, 10))
                new ButtonWindow(random)
                {
                    Title = i.ToString(),
                    DataContext = DataContext
                }.Show();
        }
    }
}
