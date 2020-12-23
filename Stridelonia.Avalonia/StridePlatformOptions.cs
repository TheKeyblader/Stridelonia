using System;

namespace Avalonia
{
    public class StridePlatformOptions
    {
        public bool UseMultiThreading { get; set; } = true;
        public bool UseDeferredRendering { get; set; } = true;
        public bool DrawFps { get; set; }
        public Type ApplicationType { get; set; }
        public Action<AppBuilder> ConfigureApp { get; set; }
    }
}