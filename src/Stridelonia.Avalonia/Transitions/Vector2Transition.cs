using System;
using System.Reactive.Linq;
using Stride.Core.Mathematics;

namespace Avalonia.Animation
{
    public class Vector2Transition : Transition<Vector2>
    {
        public override IObservable<Vector2> DoTransition(IObservable<double> progress, Vector2 oldValue, Vector2 newValue)
        {
            var delta = newValue - oldValue;
            return progress
                .Select(p => (float)Easing.Ease(p) * delta + oldValue);
        }
    }
}
