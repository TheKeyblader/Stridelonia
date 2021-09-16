using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Stride.Core.Mathematics;

namespace Stridelonia.Converters
{
    public class Vector2TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = (string)value;

            if (string.IsNullOrEmpty(s))
                return new Vector2();

            s = s.Trim('[', ']', '(', ')');
            var components = s.Split(',').Select(s => float.Parse(s)).ToArray();
            return new Vector2(components);
        }
    }

    public class NullableVector2TypeConverter : NullableConverter
    {
        public NullableVector2TypeConverter() : base(typeof(Vector2?))
        {
        }
    }
}
