using Avalonia;
using Stride.Core.Mathematics;
using AvaloniaPoint = Avalonia.Point;

namespace Stridelonia
{
    internal static class StrideHelpers
    {
        #region Vectors
        public static Vector2 ToStride(this AvaloniaPoint point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }
        public static Vector2 ToStride(this PixelPoint pixelPoint)
        {
            return new Vector2(pixelPoint.X, pixelPoint.Y);
        }
        public static Vector2 ToStride(this Size point)
        {
            return new Vector2((float)point.Width, (float)point.Height);
        }
        public static Vector2 ToStride(this Vector vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }
        public static AvaloniaPoint ToAvalonia(this Vector2 vector)
        {
            return new AvaloniaPoint(vector.X, vector.Y);
        }
        public static Size ToAvalonia(this Size2 vector)
        {
            return new Size(vector.Width, vector.Height);
        }
        #endregion

        public static PixelRect ToAvalonia(this Rectangle rect)
        {
            return new PixelRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
