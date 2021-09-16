using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Stride.Core.Mathematics;

namespace Stridelonia.Converters
{
    public class Vector4TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = (string)value;

            if (string.IsNullOrEmpty(s))
                return new Vector4();

            s = s.Trim('[', ']', '(', ')');
            var components = s.Split(',').Select(s => float.Parse(s)).ToArray();
            return new Vector4(components);
        }
    }

    public class NullableVector4TypeConverter : NullableConverter
    {
        public NullableVector4TypeConverter() : base(typeof(Vector4?))
        {
        }
    }
}
