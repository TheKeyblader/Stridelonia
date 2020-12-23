using Stride.Core.Mathematics;

namespace Avalonia.Animation.Animators
{
    public class Vector3Animator : Animator<Vector3>
    {
        public override Vector3 Interpolate(double progress, Vector3 oldValue, Vector3 newValue)
        {
            var delta = newValue - oldValue;
            return (float)progress * delta + oldValue;
        }
    }
}

