using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Stride.Core.Mathematics;

namespace Stridelonia.Converters
{
    public class QuaternionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = (string)value;

            if (string.IsNullOrEmpty(s))
                return new Quaternion();

            s = s.Trim('[', ']', '(', ')');
            var components = s.Split(',').Select(s => float.Parse(s)).ToArray();
            if (components.Length > 3) throw new FormatException("Eurler Quaternion cant have at max 3 components");
            if (components.Length == 0) throw new FormatException("Eurler Quaternion must have at least one component");

            Quaternion rotation = new();
            rotation = Quaternion.RotationX(components[0]);
            if (components.Length > 1) rotation *= Quaternion.RotationY(components[1]);
            if (components.Length == 3) rotation *= Quaternion.RotationY(components[2]);
            return rotation;
        }
    }

    public class NullableQuaternionTypeConverter : NullableConverter
    {
        public NullableQuaternionTypeConverter() : base(typeof(Quaternion?))
        {
        }
    }
}
