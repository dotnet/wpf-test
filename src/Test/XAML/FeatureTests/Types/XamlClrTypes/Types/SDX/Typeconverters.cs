// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.SDX
{
    /// <summary>
    /// NamespacePrefixValidation class
    /// </summary>
    public class NamespacePrefixValidation
    {
        /// <summary>
        /// Gets or sets the bar container.
        /// </summary>
        /// <value>The bar container.</value>
        public BarContainer BarContainer { get; set; }

        /// <summary>
        /// Gets or sets the foo container.
        /// </summary>
        /// <value>The foo container.</value>
        public FooContainer FooContainer { get; set; }

        /// <summary>
        /// Gets or sets the foo container template.
        /// </summary>
        /// <value>The foo container template.</value>
        [System.Windows.Markup.XamlDeferLoad(typeof(FooContainerTemplateConverter), typeof(FooContainer))]
        public Func<FooContainer> FooContainerTemplate { get; set; }
    }

    /// <summary>
    /// FooContainer class
    /// </summary>
    public class FooContainer
    {
        /// <summary>
        /// Gets or sets the foo.
        /// </summary>
        /// <value>The foo value.</value>
        [TypeConverter(typeof(NamespacePrefixValidatingConverter))]
        public int Foo { get; set; }
    }

    /// <summary>
    /// BarContainer class
    /// </summary>
    public class BarContainer
    {
        /// <summary>
        /// Gets or sets the bar.
        /// </summary>
        /// <value>The bar value.</value>
        public int Bar { get; set; }
    }

    /// <summary>
    /// FooContainerTemplateConverter class
    /// </summary>
    public class FooContainerTemplateConverter : XamlDeferringLoader
    {
        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="context">The context.</param>
        /// <returns>object value</returns>
        public override object Load(XamlReader reader, IServiceProvider context)
        {
            IXamlObjectWriterFactory objectWriterFactory = (IXamlObjectWriterFactory)context.GetService(typeof(IXamlObjectWriterFactory));
            FooContainerFactory fooContainerFactory = new FooContainerFactory(objectWriterFactory, reader);
            return new Func<FooContainer>(() => fooContainerFactory.GetFooContainer());
        }

        /// <summary>
        /// Saves the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value</returns>
        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// FooContainerFactory class
    /// </summary>
    public class FooContainerFactory
    {
        /// <summary>
        /// node List value
        /// </summary>
        private XamlNodeList _nodeList;

        /// <summary>
        /// objectWriter Factory
        /// </summary>
        private IXamlObjectWriterFactory _objectWriterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FooContainerFactory"/> class.
        /// </summary>
        /// <param name="objectWriterFactory">The object writer factory.</param>
        /// <param name="xamlReader">The xaml reader.</param>
        public FooContainerFactory(IXamlObjectWriterFactory objectWriterFactory, XamlReader xamlReader)
        {
            this._nodeList = new XamlNodeList(xamlReader.SchemaContext);
            XamlServices.Transform(xamlReader, _nodeList.Writer);

            this._objectWriterFactory = objectWriterFactory;
        }

        /// <summary>
        /// Gets the foo container.
        /// </summary>
        /// <returns>object value</returns>
        public FooContainer GetFooContainer()
        {
            XamlObjectWriter objectWriter = this._objectWriterFactory.GetXamlObjectWriter(new XamlObjectWriterSettings());
            XamlServices.Transform(_nodeList.GetReader(), objectWriter);
            return (FooContainer)objectWriter.Result;
        }
    }

    /// <summary>
    /// NamespacePrefixValidatingConverter class
    /// </summary>
    public class NamespacePrefixValidatingConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            IXamlNamespaceResolver namespaceResolver = (IXamlNamespaceResolver)context.GetService(typeof(IXamlNamespaceResolver));

            int expectedNamespaceCount = int.Parse(value.ToString());
            int count = 0;
            foreach (NamespaceDeclaration namespaceDeclaration in namespaceResolver.GetNamespacePrefixes())
            {
                count++;
            }

            if (count != expectedNamespaceCount)
            {
                throw new Exception("Count of items returned from IXamlNamespaceResolver.GetNamespacePrefixes should be " + value);
            }

            return expectedNamespaceCount;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Classes to ensure that we still call type converters, even if they don't properly report that they CanConvertFrom.
    /// </summary>
    public class ConverterTestElement
    {
        /// <summary>
        /// Gets or sets the string.
        /// </summary>
        /// <value>The string.</value>
        [TypeConverter(typeof(ConverterWhichSaysItCannotConvertFromStringButReallyCan))]
        public string String { get; set; }
    }

    /// <summary>
    /// ConverterWhichSaysItCannotConvertFromStringButReallyCan class
    /// </summary>
    public class ConverterWhichSaysItCannotConvertFromStringButReallyCan : TypeConverter
    {
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string strValue = value as string;
            return strValue + " has been converted.";
        }
    }

    /// <summary>
    /// GenericTypeConverter class
    /// </summary>
    /// <typeparam name="T">Type passed</typeparam>
    public class GenericTypeConverter<T> : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return Activator.CreateInstance(typeof(T), value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// ClassWithGenericConverter class
    /// </summary>
    [TypeConverter(typeof(GenericTypeConverter<ClassWithGenericConverter>))]
    public class ClassWithGenericConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassWithGenericConverter"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ClassWithGenericConverter(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
