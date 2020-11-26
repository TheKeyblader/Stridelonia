using System.Diagnostics;
using Avalonia.Controls;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Graphics;
using Stride.Rendering;

namespace Stridelonia
{
    [DebuggerDisplay("Position: {Entity.Transform.Position}, Window: {Window.Title}")]
    [DefaultEntityComponentRenderer(typeof(AvaloniaEntityProcessor))]
    public class AvaloniaComponent : EntityComponent
    {
        public Window Window { get; internal set; }
    }
}
