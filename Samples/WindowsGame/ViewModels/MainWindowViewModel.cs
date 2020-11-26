using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WindowsGame.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [ObservableAsProperty]
        public string ScoreText { get; }

        [Reactive]
        public int Score { get; set; }

        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.Score)
                .Select(x => "Score: " + x)
                .ToPropertyEx(this, x => x.ScoreText);

            UpScoreCommand = ReactiveCommand.Create(UpScore);
        }

        public ReactiveCommand<Unit, Unit> UpScoreCommand { get; }

        public void UpScore() => Score++;
    }
}
