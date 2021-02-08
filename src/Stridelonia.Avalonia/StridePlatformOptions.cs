using System;

namespace Avalonia
{
    /// <summary>
    /// Stridelonia Options
    /// </summary>
    public class StridePlatformOptions
    {
        /// <summary>
        /// Set to true if UI is blinking
        /// </summary>
        public bool WaitCopyTexture { get; set; }

        /// <summary>
        /// If true Avalonia will run in this own thread, default to true
        /// </summary>
        public bool UseMultiThreading { get; set; } = true;

        /// <summary>
        /// Activate Deffered Rendering on Avalonia, default to true
        /// </summary>
        public bool UseDeferredRendering { get; set; } = true;

        /// <summary>
        /// If true, draw fps on Avalonia Windows
        /// </summary>
        public bool DrawFps { get; set; }

        /// <summary>
        /// Avalonia Application Type (Required)
        /// </summary>
        public Type ApplicationType { get; set; }

        /// <summary>
        /// Allow to configure Avalonia Application
        /// </summary>
        public Action<AppBuilder> ConfigureApp { get; set; }
    }
}