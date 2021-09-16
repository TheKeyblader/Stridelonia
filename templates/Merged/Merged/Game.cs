using Avalonia;
using Avalonia.ReactiveUI;
using Merged.UI;
using Stridelonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merged
{
    public class Game : StrideloniaGame
    {
        public Game() : base()
        {
            AvaloniaLocator.CurrentMutable.BindToSelf(new StridePlatformOptions
            {
                ApplicationType = typeof(App),
                ConfigureApp = builder => builder.LogToTrace().UseReactiveUI(),
                WaitCopyTexture = true
            });
        }
    }
}
