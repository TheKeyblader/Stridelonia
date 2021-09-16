using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Data.Converters;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Stride.Core.Mathematics;

namespace WindowsGame.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IObservable<string> ScoreText { get; }

        [Reactive]
        public int Score { get; set; }

        public Vector3 StartPosition { get; } = new Vector3(0, 0, 0);

        public Vector3 EndPosition { get; } = new Vector3(0, 2, 0);

        public Quaternion StartRotation { get; } = Quaternion.Identity;

        public Quaternion EndRotation { get; }

        public MainWindowViewModel()
        {
            EndRotation = Quaternion.RotationY(360);

            ScoreText = this.WhenAnyValue(x => x.Score)
                .Select(x => "Score: " + x);

            UpScoreCommand = ReactiveCommand.Create(UpScore);
        }

        public ReactiveCommand<Unit, Unit> UpScoreCommand { get; }

        public void UpScore() => Score++;
    }
}
