// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;

    public class CtorArgsWStringTypeConverter
    {
        public CtorArgsWStringTypeConverter(MemberWStringTypeConverter member)
        {
            this.Member = member;
        }

        [ConstructorArgument("member")]
        public MemberWStringTypeConverter Member { get; set; }
    }

    public class CtorArgsWMarkupExtensionTypeConverter
    {
        public CtorArgsWMarkupExtensionTypeConverter(MemberWMarkupExtensionTypeConverter member)
        {
            this.Member = member;
        }

        [ConstructorArgument("member")]
        public MemberWMarkupExtensionTypeConverter Member { get; set; }
    }

    public class CtorArgsWInstanceDescriptorTypeConverter
    {
        public CtorArgsWInstanceDescriptorTypeConverter(MemberWInstanceDescriptorTypeConverter member)
        {
            this.Member = member;
        }

        [ConstructorArgument("member")]
        public MemberWInstanceDescriptorTypeConverter Member { get; set; }
    }

    //public class CtorArgsWXamlTemplateTypeConverter
    //{
    //    public CtorArgsWXamlTemplateTypeConverter(MemberWXamlTemplateTypeConverter member)
    //    {
    //        this.Member = member;
    //    }

    //    [ConstructorArgument("member")]
    //    public MemberWXamlTemplateTypeConverter Member { get; set; }
    //}

    [TypeConverter(typeof (CtorArgConverter<string>))]
    public class MemberWStringTypeConverter
    {
        public int A { get; set; }
        public int B { get; set; }
    }

    [TypeConverter(typeof (CtorArgConverter<MarkupExtension>))]
    public class MemberWMarkupExtensionTypeConverter
    {
        public int A { get; set; }
        public int B { get; set; }
    }

    [TypeConverter(typeof (CtorArgConverter<InstanceDescriptor>))]
    public class MemberWInstanceDescriptorTypeConverter
    {
        internal MemberWInstanceDescriptorTypeConverter() { }
        public int A { get; set; }
        public int B { get; set; }

        public static MemberWInstanceDescriptorTypeConverter Create(int a, int b)
        {
            return new MemberWInstanceDescriptorTypeConverter()
            {
                A = a,
                B = b
            };
        }
    }

    //[XamlTemplate(typeof(CtorArgConverter<System.Runtime.Xaml.XamlReader>),typeof(object))]
    //public class MemberWXamlTemplateTypeConverter
    //{
    //    public int A { get; set; }
    //    public int B { get; set; }
    //}

    public class CtorArgConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (T);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (T);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new DataTestException("ConvertFrom called.");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new DataTestException("ConvertTo called.");
        }
    }
}
