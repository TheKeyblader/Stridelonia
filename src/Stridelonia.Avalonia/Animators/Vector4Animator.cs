using Stride.Core.Mathematics;

namespace Avalonia.Animation.Animators
{
    public class Vector4Animator : Animator<Vector4>
    {
        public override Vector4 Interpolate(double progress, Vector4 oldValue, Vector4 newValue)
        {
            var delta = newValue - oldValue;
            return (float)progress * delta + oldValue;
        }
    }
}

