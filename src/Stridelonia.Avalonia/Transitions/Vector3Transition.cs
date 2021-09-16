using System;
using System.Reactive.Linq;
using Stride.Core.Mathematics;

namespace Avalonia.Animation
{
    public class Vector3Transition : Transition<Vector3>
    {
        public override IObservable<Vector3> DoTransition(IObservable<double> progress, Vector3 oldValue, Vector3 newValue)
        {
            var delta = newValue - oldValue;
            return progress
                .Select(p => (float)Easing.Ease(p) * delta + oldValue);
        }
    }
}
