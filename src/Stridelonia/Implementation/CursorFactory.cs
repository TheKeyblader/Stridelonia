﻿using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;

namespace Stridelonia.Implementation
{
    internal class CursorFactory : ICursorFactory
    {
        public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot) => CursorImpl.ZeroCursor;

        public ICursorImpl GetCursor(StandardCursorType cursorType) => CursorImpl.ZeroCursor;

        private sealed class CursorImpl : ICursorImpl
        {
            public static CursorImpl ZeroCursor { get; } = new CursorImpl();

            private CursorImpl() { }

            public void Dispose() { }
        }
    }
}
