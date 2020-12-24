using Stride.Engine;

namespace Stridelonia
{
    public class StrideloniaGame : Game
    {
        protected override void BeginRun()
        {
            base.BeginRun();
            StrideloniaApplication.Start(this);
        }

        protected override void EndRun()
        {
            base.EndRun();
            StrideloniaApplication.Stop();
        }
    }
}
