using Stride.Core.Mathematics;

namespace Avalonia.Animation.Animators
{
    public class QuaternionAnimator : Animator<Quaternion>
    {
        public override Quaternion Interpolate(double progress, Quaternion oldValue, Quaternion newValue)
        {
            return Quaternion.Lerp(oldValue, newValue, (float)progress);
        }
    }
}
