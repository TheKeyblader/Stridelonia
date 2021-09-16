using System;
using System.Reactive.Linq;
using Stride.Core.Mathematics;

namespace Avalonia.Animation
{
    public class Vector4Transition : Transition<Vector4>
    {
        public override IObservable<Vector4> DoTransition(IObservable<double> progress, Vector4 oldValue, Vector4 newValue)
        {
            var delta = newValue - oldValue;
            return progress
                .Select(p => (float)Easing.Ease(p) * delta + oldValue);
        }
    }
}
