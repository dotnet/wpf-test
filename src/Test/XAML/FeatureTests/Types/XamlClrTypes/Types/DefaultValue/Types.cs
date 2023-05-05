// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Test.Xaml.Types.DefaultValues
{
    public class DefaultValueType
    {
        [DefaultValue(false)]
        public bool BooleanProperty { get; set; }

        [DefaultValue((byte) 20)]
        public Byte ByteProperty { get; set; }

        [DefaultValue('a')]
        public Char CharProperty { get; set; }

        [DefaultValue(100.5)]
        public Double DoubleProperty { get; set; }

        [DefaultValue(10)]
        public int IntProperty { get; set; }

        [DefaultValue((long) 102.4)]
        public long LongProperty { get; set; }

        [DefaultValue(null)]
        public MyObject MyObjectProperty { get; set; }

        [DefaultValue((short) 10)]
        public short ShortProperty { get; set; }

        [DefaultValue("Hello")]
        public string StringProperty { get; set; }

        [DefaultValue(typeof (ClassWithTypeConverter), "MyDefaultValue")]
        public ClassWithTypeConverter ClassWithTypeConverter { get; set; }
    }

    public class ClassWithDefaultList
    {
        [DefaultValue(null)]
        public List<string> Data { get; set; }
    }

    public class MyObject
    {
        public int foo { get; set; }
    }

    [TypeConverter(typeof (MyObjecttTypeConverter))]
    public class ClassWithTypeConverter
    {
        public string foo { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as ClassWithTypeConverter).foo.Equals(this.foo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MyObjecttTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new ClassWithTypeConverter()
                           {
                               foo = (value as string)
                           };
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ClassWithTypeConverter)
            {
                return ((ClassWithTypeConverter) value).foo;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }

    public class BoundaryDefaultValueType
    {
        [DefaultValue(true)]
        public bool BooleanPropertyTrue { get; set; }

        [DefaultValue(byte.MaxValue)]
        public Byte BytePropertyMax { get; set; }

        [DefaultValue(byte.MinValue)]
        public Byte BytePropertyMin { get; set; }

        [DefaultValue(double.MaxValue)]
        public Double DoublePropertyMax { get; set; }

        [DefaultValue(double.MinValue)]
        public Double DoublePropertyMin { get; set; }

        [DefaultValue(double.NaN)]
        public Double DoublePropertyNan { get; set; }

        [DefaultValue(double.NegativeInfinity)]
        public Double DoublePropertyNin { get; set; }

        [DefaultValue(double.PositiveInfinity)]
        public Double DoublePropertyPin { get; set; }

        [DefaultValue(double.Epsilon)]
        public Double DoublePropertyEps { get; set; }

        [DefaultValue(int.MaxValue)]
        public int IntPropertyMax { get; set; }

        [DefaultValue(int.MinValue)]
        public int IntPropertyMin { get; set; }

        [DefaultValue(long.MaxValue)]
        public long LongPropertyMax { get; set; }

        [DefaultValue(long.MinValue)]
        public long LongPropertyMin { get; set; }

        [DefaultValue(short.MaxValue)]
        public short ShortPropertyMax { get; set; }

        [DefaultValue(short.MinValue)]
        public short ShortPropertyMin { get; set; }

        [DefaultValue("")]
        public string StringPropertyEmpty { get; set; }

        [DefaultValue((string) null)]
        public string StringPropertyNull { get; set; }
    }

    public class BaseDefaultValue
    {
        [DefaultValue("some value")]
        public virtual string PropWithDefaultValue { get; set; }
    }

    public class DerivedDefaultValue : BaseDefaultValue
    {
        public override string PropWithDefaultValue { get; set; }
    }

    public class BaseDesignerSerializationVisibility
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string PropWithDesignerSerializationVisibility { get; set; }
    }

    public class DerivedDesignerSerializationVisibility : BaseDesignerSerializationVisibility
    {
        public override string PropWithDesignerSerializationVisibility { get; set; }
    }
}
