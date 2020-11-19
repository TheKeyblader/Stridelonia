using System;
using Avalonia.Input;
using Avalonia.Platform;

namespace Stridelonia.Implementation
{
    internal class CursorFactory : IStandardCursorFactory
    {
        public IPlatformHandle GetCursor(StandardCursorType cursorType)
            => new PlatformHandle(IntPtr.Zero, "ZeroCursor");
    }
}
