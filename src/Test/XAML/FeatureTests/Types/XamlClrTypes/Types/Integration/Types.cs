// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Markup;
    using System.Xaml;
    
    public class Container
    {
        public object Content { get; set; }

        public object SecondContent { get; set; }
    }

    [RuntimeNameProperty("Name")]
    [TypeConverter(typeof(CustomMEConverter))]
    public class CustomMEClass
    {
        public CustomMEClass()
        {
        }

        public string Name { get; set; }

        public object Content { get; set; }

        public object SecondContent { get; set; }
    }

    public class CustomMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            CustomMEClass CustomMEClass = value as CustomMEClass;

            if (null == CustomMEClass)
            {
                throw new ArgumentException();
            }

            CustomME extension = new CustomME();
            extension.Content = CustomMEClass.Content;
            extension.SecondContent = CustomMEClass.SecondContent;

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    [RuntimeNameProperty("Name")]
    public class CustomME : MarkupExtension
    {
        private CustomMEClass _customMEClass;

        public string Name { get; set; }

        public CustomME()
        {
        }

        public object Content { get; set; }

        public object SecondContent { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_customMEClass == null)
            {
                _customMEClass = new CustomMEClass();
                _customMEClass.Content = this.Content;
                _customMEClass.SecondContent = this.SecondContent;
            }

            return _customMEClass;
        }

        public static CustomME Create()
        {
            return new CustomME() { Content = new TimeSpan(1000) };
        }
    }


    [TypeConverter(typeof(CustomMEWithTCConverter))]
    public class CustomMEWithTCClass
    {
        public CustomMEWithTCClass()
        {
        }

        public object Content { get; set; }

        public object SecondContent { get; set; }
    }

    public class CustomMEWithTCConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            CustomMEWithTCClass CustomMEClass = value as CustomMEWithTCClass;

            if (null == CustomMEClass)
            {
                throw new ArgumentException();
            }

            CustomMEWithTC extension = new CustomMEWithTC();
            extension.Content = CustomMEClass.Content;
            extension.SecondContent = CustomMEClass.SecondContent;

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    [TypeConverter(typeof(CustomMEWithTCTypeConverter))]
    public class CustomMEWithTC : MarkupExtension
    {
        private CustomMEWithTCClass _customMEClass;

        public CustomMEWithTC()
        {
        }

        public object Content { get; set; }

        public object SecondContent { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_customMEClass == null)
            {
                _customMEClass = new CustomMEWithTCClass();
                _customMEClass.Content = this.Content;
                _customMEClass.SecondContent = this.SecondContent;
            }

            return _customMEClass;
        }
    }

    public class CustomMEWithTCTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            string[] values = (value as string).Split('*');
            CustomMEWithTC obj = new CustomMEWithTC()
            {
                Content = values[0],
                SecondContent = values[1],
            };
            return obj;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType().IsAssignableFrom(typeof(NestedBase)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            CustomMEWithTC obj = (CustomMEWithTC)value;
            return obj.Content.ToString() + "*" + obj.SecondContent.ToString();
        }
    }

    [TypeConverter(typeof(CustomMEXargsConverter))]
    public class CustomMEXargsClass
    {
        public CustomMEXargsClass()
        {
        }

        public object Content { get; set; }
    }

    public class CustomMEXargsConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            CustomMEXargsClass customMEXargsClass = value as CustomMEXargsClass;

            if (null == customMEXargsClass)
            {
                throw new ArgumentException();
            }

            CustomMEWithXArgs extension = new CustomMEWithXArgs(customMEXargsClass.Content);

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class CustomMEWithXArgs : MarkupExtension
    {
        private CustomMEClass _customMEClass;

        public CustomMEWithXArgs(object content)
        {
            this.Content = content;
        }

        [ConstructorArgument("content")]
        public object Content { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_customMEClass == null)
            {
                _customMEClass = new CustomMEClass();
                _customMEClass.Content = this.Content;
            }

            return _customMEClass;
        }
    }

    public class TimeSpanContainer
    {
        public TimeSpanContainer()
        {
            this.Span = new TimeSpan(1000);
        }

        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan Span { get; set; }
    }

    public class TimeSpanTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return TimeSpan.Parse((value as string).Trim('$'));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType().IsAssignableFrom(typeof(NestedBase)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return "$" + ((TimeSpan)value).ToString() + "$";
        }
    }

    [TypeConverter(typeof(CustomTypeConverter))]
    public class CustomType
    {
        public String Content { get; set; }
    }

    public class CustomTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return new CustomType() { Content = (value as string).Trim('$') };
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType().IsAssignableFrom(typeof(NestedBase)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return "$" + ((CustomType)value).Content.ToString() + "$";
        }
    }

    public class TCAndXArgsContainer
    {
        public TCAndXArgsContainer(TimeSpan timeSpan)
        {
            this.Span = timeSpan;
        }

        [ConstructorArgument("timeSpan")]
        [TypeConverter(typeof(TimeSpanTypeConverter))]
        public TimeSpan Span { get; set; }
    }

    public class MEAndXArgsContainer
    {
        public MEAndXArgsContainer(CustomMEClass mEProperty)
        {
            this.MEProperty = mEProperty;
        }

        [ConstructorArgument("mEProperty")]
        public CustomMEClass MEProperty { get; set; }
    }

    public class ArgumentContainer
    {
        public ArgumentContainer(object content)
        {
            this.Content = content;
        }

        [ConstructorArgument("content")]
        public object Content { get; set; }

        public static ArgumentContainer Create()
        {
            return new ArgumentContainer(20);
        }
    }

    public class Factory
    {
        public static CustomMEXargsClass CreateTypeConvertedObject()
        {
            return new CustomMEXargsClass() { Content = "Hello" };
        }

        public static List<string> CreateStringList()
        {
            return new List<string>
            {
                "Hello",
                "Xaml"
            };
        }

        public static object CreateReferencedObject(object input)
        {
            return new Container()
            {
                Content = input,
                SecondContent = input
            };
        }

        public static string CreateString()
        {
            return "Hello World";
        }
    }

    public class TemplateContainer
    {
        public Custom_Template Template { get; set; }
    }

    [XamlDeferLoad(typeof(Custom_TemplateConverter), typeof(object))]
    public class Custom_Template
    {
        private readonly XamlNodeList _xamlNodeList;

        public Custom_Template(System.Xaml.XamlReader xamlReader, IXamlObjectWriterFactory factory)
        {
            XamlObjectWriterFactory = factory;
            _xamlNodeList = new XamlNodeList(xamlReader.SchemaContext);
            XamlServices.Transform(xamlReader, _xamlNodeList.Writer);
        }

        public IXamlObjectWriterFactory XamlObjectWriterFactory { get; private set; }

        public System.Xaml.XamlReader GetXamlReader()
        {
            return _xamlNodeList.GetReader();
        }

        public object LoadTemplate(XamlObjectWriterSettings settings)
        {
            XamlObjectWriter xamlWriter = XamlObjectWriterFactory.GetXamlObjectWriter(settings);
            System.Xaml.XamlReader xamlReader = GetXamlReader();
            XamlServices.Transform(xamlReader, xamlWriter);
            return xamlWriter.Result;
        }
    }

    public class Custom_TemplateConverter : XamlDeferringLoader
    {
        public override object Load(System.Xaml.XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService(typeof(IXamlObjectWriterFactory)) as IXamlObjectWriterFactory;
            if (factory == null)
            {
                throw new InvalidOperationException("Missing Service Provider Service 'IXamlObjectWriterFactory'");
            }

            var testTemplate = new Custom_Template(xamlReader, factory);
            return testTemplate;
        }

        public override System.Xaml.XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            var template = value as Custom_Template;
            return template.GetXamlReader();
        }
    }

    public class StringContainer
    {
        public string Content { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }
    
    /// <summary>
    /// Generic markup extension
    /// </summary>
    /// <typeparam name="X">type X</typeparam>
    /// <typeparam name="Y">type Y</typeparam>
    public class GenericMarkupExtension<X, Y> : MarkupExtension
    {
        public GenericMarkupExtension()
        {
            _x = new List<X>();
            _y = new List<Y>();

        }

        IList<X> _x;
        IList<Y> _y;

        public IList<X> PropX { get { return _x; } }

        public IList<Y> PropY { get { return _y; } }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return "[" + PropX.ToString() + "," + PropY.ToString() + "]";
        }
    }

    public class EventHolder
    {
        public event EventHandler SomeEvent;

        public void RaiseSomeEvent()
        {
            SomeEvent(this, new EventArgs());
        }
    }

    public class EventHolderExtension : MarkupExtension
    {
        public event EventHandler SomeEvent;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            EventHolder holder = new EventHolder();
            holder.SomeEvent += this.SomeEvent;
            return holder;
        }
    }

    public class WrapperWithEventHanlder
    {
        public object Data { get; set; }
        private bool _fired = false;
        public bool Fired { get { return _fired; } }

        public void SomeEventHandler(object sender, EventArgs arg)
        {
            _fired = true;
        }
    }
}
