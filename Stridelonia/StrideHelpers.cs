using Avalonia;
using Stride.Core.Mathematics;
using AvaloniaPoint = Avalonia.Point;

namespace Stridelonia
{
    public static class StrideHelpers
    {
        #region Vectors
        public static Vector2 ToUnity(this AvaloniaPoint point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }
        public static Vector2 ToUnity(this Size point)
        {
            return new Vector2((float)point.Width, (float)point.Height);
        }
        public static Vector2 ToUnity(this Vector vector)
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
    }
}
