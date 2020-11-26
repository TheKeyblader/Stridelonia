using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stridelonia;

namespace WindowsGame.Views
{
    public class MainWindow : Window
    {
        public static TextBlock TextBlock;


        public MainWindow()
        {
            WindowExtensions.SetRenderGroup(this, Stride.Rendering.RenderGroup.Group31);
            WindowExtensions.SetIs2D(this, false);
            WindowExtensions.Set3DPosition(this, new Stride.Core.Mathematics.Vector3());
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            TextBlock = this.FindControl<TextBlock>("Test");
            this.Activated += MainWindow_Activated;
            this.Deactivated += MainWindow_Deactivated;
            this.PointerLeave += MainWindow_PointerLeave;
            this.PointerEnter += MainWindow_PointerEnter;
            this.GotFocus += MainWindow_GotFocus;
            this.LostFocus += MainWindow_LostFocus;
        }

        private void MainWindow_LostFocus(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Debug.WriteLine("Window lost focus");
        }

        private void MainWindow_GotFocus(object sender, Avalonia.Input.GotFocusEventArgs e)
        {
            Debug.WriteLine("Window got focus");
        }

        private void MainWindow_PointerEnter(object sender, Avalonia.Input.PointerEventArgs e)
        {
            Debug.WriteLine("Window Pointer Enter");
        }

        private void MainWindow_PointerLeave(object sender, Avalonia.Input.PointerEventArgs e)
        {
            Debug.WriteLine("Window Pointer Leave");
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("Window is Desactivated");
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            Debug.WriteLine("Window is Activated");
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
