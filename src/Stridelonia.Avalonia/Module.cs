using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia.Animation;
using Avalonia.Animation.Animators;
using Stride.Core.Mathematics;
using Stridelonia.Converters;

namespace Stridelonia
{
    internal class Module
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            Animation.RegisterAnimator<Vector2Animator>(prop => typeof(Vector2).IsAssignableFrom(prop.PropertyType));
            Animation.RegisterAnimator<Vector3Animator>(prop => typeof(Vector3).IsAssignableFrom(prop.PropertyType));
            Animation.RegisterAnimator<Vector4Animator>(prop => typeof(Vector4).IsAssignableFrom(prop.PropertyType));
            Animation.RegisterAnimator<QuaternionAnimator>(prop => typeof(Quaternion).IsAssignableFrom(prop.PropertyType));

            TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(Vector2TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector4), new TypeConverterAttribute(typeof(Vector4TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Quaternion), new TypeConverterAttribute(typeof(QuaternionTypeConverter)));

            TypeDescriptor.AddAttributes(typeof(Vector2?), new TypeConverterAttribute(typeof(NullableVector2TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector3?), new TypeConverterAttribute(typeof(NullableVector3TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Vector4?), new TypeConverterAttribute(typeof(NullableVector4TypeConverter)));
            TypeDescriptor.AddAttributes(typeof(Quaternion?), new TypeConverterAttribute(typeof(NullableQuaternionTypeConverter)));
        }
    }
}
