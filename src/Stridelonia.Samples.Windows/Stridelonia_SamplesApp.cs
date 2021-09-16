using System.Diagnostics;
using Stride.Engine;

namespace Stridelonia.Samples
{
    class Stridelonia_SamplesApp
    {
        static void Main(string[] args)
        {
            using (var game = new StrideloniaGame())
            {
                game.Run();
            }
        }
    }
}
