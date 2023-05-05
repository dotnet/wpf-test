// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Markup;
    using System.Xaml;

    public class SimpleMEClassContainer
    {
        public SimpleMEClass SimpleMEClass { get; set; }
    }

    [TypeConverter(typeof (SimpleMEConverter))]
    public class SimpleMEClass
    {
        private string _streetAddress;
        private int _aptNo;

        public SimpleMEClass()
        {
        }

        public string Address
        {
            get
            {
                return this._streetAddress;
            }
            set
            {
                this._streetAddress = value;
            }
        }

        public int AptNo
        {
            get
            {
                return this._aptNo;
            }
            set
            {
                this._aptNo = value;
            }
        }
    }

    public class SimpleMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof (MarkupExtension))
            {
                throw new ArgumentException("destinationType");
            }

            SimpleMEClass simpleMEClass = value as SimpleMEClass;

            if (null == simpleMEClass)
            {
                throw new ArgumentException("simpleMEClass");
            }

            SimpleME extension = new SimpleME();
            extension.Address = simpleMEClass.Address;
            extension.AptNo = simpleMEClass.AptNo;

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class SimpleME : MarkupExtension
    {
        private string _address;
        private int _apt;

        private SimpleMEClass _simpleMEClass;

        public SimpleME()
        {
        }

        public SimpleME(string foo)
        {
            this._address = foo;
        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }
        public int AptNo
        {
            get
            {
                return this._apt;
            }
            set
            {
                this._apt = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEClass == null)
            {
                _simpleMEClass = new SimpleMEClass();
                _simpleMEClass.Address = this._address;
                _simpleMEClass.AptNo = this._apt;
            }

            return _simpleMEClass;
        }
    }
}
