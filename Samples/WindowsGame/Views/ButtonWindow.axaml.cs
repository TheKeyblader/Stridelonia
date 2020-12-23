using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Stridelonia;
using WindowsGame.ViewModels;

namespace WindowsGame.Views
{
    public class ButtonWindow : Window
    {

        const int MaxSize = 450;
        const int MinSize = 100;
        readonly Random Random;

        public ButtonWindow() : this(new Random())
        {

        }

        public ButtonWindow(Random random)
        {
            Random = random;
            WindowExtensions.SetRenderGroup(this, 31);
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            SetRandomSizePos();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            WindowExtensions.SetZIndex(this, short.Parse(Title));
            var btn = this.FindControl<Button>("CloseBtn");
            btn.Content = "Click Me " + Title + " !";
        }

        private void SetRandomSizePos()
        {
            Width = Random.Next(MinSize, MaxSize);
            Height = Random.Next(MinSize, MaxSize);

            Position = new PixelPoint(Random.Next(Screens.Primary.WorkingArea.Width - 100), Random.Next(Screens.Primary.WorkingArea.Height - 100));
        }

        public void CloseButton(object sender, RoutedEventArgs args)
        {
            Hide();
            ((MainWindowViewModel)(DataContext)).UpScore();
            Task.Run(async () =>
            {
                await Task.Delay(1500);

                Dispatcher.UIThread.Post(() =>
                {
                    SetRandomSizePos();
                    Show();
                });
            });
        }
    }
}
