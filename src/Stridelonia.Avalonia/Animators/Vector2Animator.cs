using Stride.Core.Mathematics;

namespace Avalonia.Animation.Animators
{
    public class Vector2Animator : Animator<Vector2>
    {
        public override Vector2 Interpolate(double progress, Vector2 oldValue, Vector2 newValue)
        {
            var delta = newValue - oldValue;
            return (float)progress * delta + oldValue;
        }
    }
}
