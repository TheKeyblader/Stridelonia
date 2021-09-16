using System;
using System.Reactive.Linq;
using Stride.Core.Mathematics;

namespace Avalonia.Animation
{
    public class QuaternionTransition : Transition<Quaternion>
    {
        public override IObservable<Quaternion> DoTransition(IObservable<double> progress, Quaternion oldValue, Quaternion newValue)
        {
            return progress.Select(p => Quaternion.Lerp(oldValue, newValue, (float)Easing.Ease(p)));
        }
    }
}
