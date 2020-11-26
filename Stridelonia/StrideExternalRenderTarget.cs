using System;
using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.Direct2D1;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Stride.Graphics;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Resource = SharpDX.DXGI.Resource;

namespace Stridelonia
{
    public class StrideExternalRenderTarget : IExternalDirect2DRenderTargetSurface
    {
        private Mutex _mutex;
        private DeviceContext _renderTarget;
        private Bitmap1 _bitmap;

        private GraphicsDevice _device;
        private Texture2D renderedTexture;
        private Texture2D visibleTexture;
        private Texture _visibleStrideTexture;

        public Size ClientSize { get; set; }

        public Texture Texture => _visibleStrideTexture;

        public StrideExternalRenderTarget(GraphicsDevice device)
        {
            _mutex = new Mutex();
            _device = device;
        }

        public void BeforeDrawing()
        {
            _mutex.WaitOne();
        }

        public void AfterDrawing()
        {
            Direct2D1Platform.Direct3D11Device.ImmediateContext.CopyResource(renderedTexture, visibleTexture);
            Direct2D1Platform.Direct3D11Device.ImmediateContext.Flush();
            _mutex.ReleaseMutex();
        }

        public void DestroyRenderTarget()
        {
            _mutex.WaitOne();
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
                _bitmap.Dispose();
                visibleTexture?.Dispose();
                renderedTexture?.Dispose();
            }
            _mutex.ReleaseMutex();
        }


        public SharpDX.Direct2D1.RenderTarget GetOrCreateRenderTarget()
        {
            if (_renderTarget == null) _renderTarget = Create();
            return _renderTarget;
        }

        private DeviceContext Create()
        {
            if (ClientSize.Height == 0 || ClientSize.Width == 0) throw new InvalidOperationException("Can't create window or texture with size of 0");

            renderedTexture = new Texture2D(Direct2D1Platform.Direct3D11Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = (int)ClientSize.Height,
                Width = (int)ClientSize.Width,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });

            visibleTexture = new Texture2D(Direct2D1Platform.Direct3D11Device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = (int)ClientSize.Height,
                Width = (int)ClientSize.Width,
                OptionFlags = ResourceOptionFlags.Shared,
                SampleDescription = new SampleDescription(1, 0),
                MipLevels = 1,
                Usage = ResourceUsage.Default
            });

            var sharedHandle = visibleTexture.QueryInterface<Resource>().SharedHandle;
            var strideDevice = SharpDXInterop.GetNativeDevice(_device) as SharpDX.Direct3D11.Device;
            var strideTex = strideDevice.OpenSharedResource<Texture2D>(sharedHandle);
            _visibleStrideTexture = SharpDXInterop.CreateTextureFromNative(_device, strideTex, false);

            var surface = renderedTexture.QueryInterface<Surface>();
            _renderTarget = new DeviceContext(Direct2D1Platform.Direct2D1Device, DeviceContextOptions.EnableMultithreadedOptimizations)
            {
                DotsPerInch = new SharpDX.Size2F(96, 96)
            };

            _bitmap = new Bitmap1(_renderTarget, surface, new BitmapProperties1
            {
                BitmapOptions = BitmapOptions.CannotDraw | BitmapOptions.Target,
                DpiX = (int)ClientSize.Height,
                DpiY = (int)ClientSize.Height,
                PixelFormat = new PixelFormat
                {
                    AlphaMode = SharpDX.Direct2D1.AlphaMode.Premultiplied,
                    Format = Format.B8G8R8A8_UNorm
                }
            });

            _renderTarget.Target = _bitmap;
            return _renderTarget;
        }
    }
}
