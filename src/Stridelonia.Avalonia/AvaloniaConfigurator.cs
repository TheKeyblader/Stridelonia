using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stridelonia
{
    /// <summary>
    /// Define Avalonia configuration in Stride
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AvaloniaConfiguratorAttribute : Attribute
    {
        public AvaloniaConfiguratorAttribute(Type configuratorType)
        {
            ConfiguratorType = configuratorType;
        }

        public Type ConfiguratorType { get; private set; }
    }
}
