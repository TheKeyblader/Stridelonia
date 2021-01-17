using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Skia;
using SkiaSharp;
using Stride.Core.Mathematics;
using Stride.Engine.Design;
using Stride.Games;
using Stride.Graphics;

namespace Stridelonia.Generic
{
    internal class GenericSkiaRenderer : ISkiaGpuRenderTarget
    {
        private readonly GenericAvaloniaRenderer _data;
        private readonly StridePlatformOptions _options;

        private Size2 _currentSize;
        private SKSurface _surface;

        public GenericSkiaRenderer(GenericAvaloniaRenderer generic)
        {
            _data = generic;
            _options = AvaloniaLocator.Current.GetService<StridePlatformOptions>();
        }

        public bool IsCorrupted => false;

        class Session : ISkiaGpuRenderSession
        {
            public GRContext GrContext { get; }

            public SKSurface SkSurface { get; }

            public double ScaleFactor => 1;

            public GRSurfaceOrigin SurfaceOrigin => GRSurfaceOrigin.TopLeft;

            private readonly IAvaloniaRenderer _renderData;
            private readonly bool _waitCopy;

            public Session(IAvaloniaRenderer renderData, SKSurface skiaSurface, bool waitCopy)
            {
                _renderData = renderData;
                _waitCopy = waitCopy;
                SkSurface = skiaSurface;
            }

            public void Dispose()
            {
                var pixels = SkSurface.PeekPixels();
                SetTexture(pixels.GetPixels(), pixels.RowBytes * pixels.Height);
            }

            private void SetTexture(IntPtr pixels, int size)
            {
                if (_renderData.Texture == null) return;
                var copyTask = StrideDispatcher.StrideThread.InvokeAsync(() =>
                {
                    var game = AvaloniaLocator.Current.GetService<IGame>();
                    var device = game.GraphicsDevice;
                    var context = game.GraphicsContext;

                    _renderData.Texture.SetData(context.CommandList, new DataPointer(pixels, size));
                });
                if (_waitCopy) copyTask.Wait();
            }
        }

        public ISkiaGpuRenderSession BeginRenderingSession()
        {
            if (_currentSize != _data.Size || _surface == null)
            {
                _data.Texture?.Dispose();
                _surface?.Dispose();
                _surface = null;
                _data.Texture = null;

                var game = AvaloniaLocator.Current.GetService<IGame>();
                var gameSettings = game.Services.GetService<IGameSettingsService>().Settings;
                var device = game.GraphicsDevice;

                StrideDispatcher.StrideThread.InvokeAsync(() =>
                {
                    _data.Texture = Texture.New2D(device, _data.Size.Width, _data.Size.Height, PixelFormat.B8G8R8A8_UNorm);
                }).Wait(1000);
                if (gameSettings.Configurations.Get<RenderingSettings>().ColorSpace == ColorSpace.Linear)
                    _surface = SKSurface.Create(new SKImageInfo(_data.Size.Width, _data.Size.Height, SKColorType.Bgra8888, SKAlphaType.Premul, SKColorSpace.CreateSrgbLinear()));
                else
                    _surface = SKSurface.Create(new SKImageInfo(_data.Size.Width, _data.Size.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
                _currentSize = _data.Size;
            }
            return new Session(_data, _surface, _options.WaitCopyTexture);
        }

        public void Dispose()
        {
            _data.Texture?.Dispose();
            _surface?.Dispose();
        }
    }
}
