using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Stride.Core.Mathematics;

namespace Stridelonia.Converters
{
    public class Vector3TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = (string)value;

            if (string.IsNullOrEmpty(s))
                return new Vector3();

            s = s.Trim('[', ']', '(', ')');
            var components = s.Split(',').Select(s => float.Parse(s)).ToArray();
            return new Vector3(components);
        }
    }

    public class NullableVector3TypeConverter : NullableConverter
    {
        public NullableVector3TypeConverter() : base(typeof(Vector3?))
        {
        }
    }
}
