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
        private DeviceContext _renderTarget;
        private Bitmap1 _bitmap;

        private IGraphicsDeviceService _service;
        private SharpDX.Direct3D11.Device _strideDevice;
        private Texture2D renderedTexture;
        private Texture2D visibleTexture;

        public RenderInfo RenderInfo { get; private set; }
        public Size ClientSize { get; set; }

        public static Mutex CriticalMutex { get; } = new Mutex();

        public StrideExternalRenderTarget(IGraphicsDeviceService service)
        {
            RenderInfo = new RenderInfo();
            _service = service;
            _strideDevice = SharpDXInterop.GetNativeDevice(_service.GraphicsDevice) as SharpDX.Direct3D11.Device;
        }

        public void BeforeDrawing()
        {
            
            CriticalMutex.WaitOne();
            Debug.WriteLine("Started calculting Avalonia texture");
        }

        public void AfterDrawing()
        {
            Debug.WriteLine("end calculting Avalonia texture");
            Debug.WriteLine("start coping Avalonia texture");
            Direct2D1Platform.Direct3D11Device.ImmediateContext.CopyResource(renderedTexture, visibleTexture);
            Direct2D1Platform.Direct3D11Device.ImmediateContext.Flush();
            Debug.WriteLine("end coping Avalonia texture");
            Debug.WriteLine("end calculting Avalonia texture");
            CriticalMutex.ReleaseMutex();
           
        }

        public void DestroyRenderTarget()
        {
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
                _bitmap.Dispose();
                visibleTexture?.Dispose();
                renderedTexture?.Dispose();
                RenderInfo.Texture.Dispose();
                RenderInfo.Texture = null;
            }
        }


        public SharpDX.Direct2D1.RenderTarget GetOrCreateRenderTarget()
        {
            if (_renderTarget == null)
                StrideDispatcher.StrideThread.InvokeAsync(() =>
                {
                    _renderTarget = Create();
                }).Wait();
            return _renderTarget;
        }

        private DeviceContext Create()
        {
            Debug.WriteLine("Creating Avalonia Render Target");
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
            var strideTex = _strideDevice.OpenSharedResource<Texture2D>(sharedHandle);
            RenderInfo.Texture = SharpDXInterop.CreateTextureFromNative(_service.GraphicsDevice, strideTex, false);

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

            Debug.WriteLine("Finish Creating Avalonia Render Target");
            return _renderTarget;
        }
    }
}
