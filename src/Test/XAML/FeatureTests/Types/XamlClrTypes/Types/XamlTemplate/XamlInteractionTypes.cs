// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Linq;
    using System.Reflection;
    using System.Xaml;

    public class InteractionClass<T> : XamlTemplateClass<T>
    {
        public InteractionClass() : base(null, null)
        {
        }

        public override T Evaluate()
        {
            throw new NotSupportedException();
        }

        public override T Evaluate(object context)
        {
            throw new NotSupportedException();
        }
    }

    public class InteractionConverterClass : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var iProvideValueTarget = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;
            PropertyInfo pi = iProvideValueTarget.TargetProperty as PropertyInfo;
            Type type = pi.PropertyType;

            var loaderFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));

            Type evaluationType = type.GetGenericArguments()[0];
            Type factoryType = typeof (InteractionClass<>).MakeGenericType(evaluationType);

            return Activator.CreateInstance(factoryType);
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            throw new NotSupportedException();
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point()
        {
        }
    }

    [System.Windows.Markup.XamlDeferLoad(typeof(PointXamlTemplateTypeConverterGood), typeof(Point))]
    public class PointWithXamlTemplate : Point
    {
        protected XamlNodeList nodes;
        protected IXamlObjectWriterFactory loader;

        public PointWithXamlTemplate()
        {
        }

        public PointWithXamlTemplate(IXamlObjectWriterFactory loader,
                                     XamlReader reader)
        {
            this.loader = loader;
            if (reader == null)
            {
                this.nodes = null;
            }
            else
            {
                this.nodes = new XamlNodeList(reader.SchemaContext);
                XamlServices.Transform(reader, nodes.Writer);
            }
        }

        public XamlNodeList Nodes
        {
            get
            {
                return nodes;
            }
        }

        public virtual PointWithXamlTemplate Evaluate()
        {
            return Evaluate(null);
        }

        public virtual PointWithXamlTemplate Evaluate(object context)
        {
            XamlObjectWriter writer = loader.GetXamlObjectWriter(null);
            XamlServices.Transform(nodes.GetReader(), writer);
            return (PointWithXamlTemplate) writer.Result;
        }
    }

    [System.Windows.Markup.XamlDeferLoad(typeof(PointInstanceDescriptorTypeConverterGood),typeof(Point))]
    public class PointWithInstanceDescriptor : Point
    {
    }

    public class PointInstanceDescriptorTypeConverterBad : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            return sourceType == typeof (InstanceDescriptor);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                                          Type destinationType)
        {
            return destinationType == typeof (InstanceDescriptor);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           System.Globalization.CultureInfo culture,
                                           object value)
        {
            throw new NotSupportedException();
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                                         System.Globalization.CultureInfo culture,
                                         object value,
                                         Type destinationType)
        {
            throw new NotSupportedException();
        }
    }

    public class PointInstanceDescriptorTypeConverterGood : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            return sourceType == typeof (InstanceDescriptor);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                                          Type destinationType)
        {
            return destinationType == typeof (InstanceDescriptor);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           System.Globalization.CultureInfo culture,
                                           object value)
        {
            throw new InvalidOperationException();
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                                         System.Globalization.CultureInfo culture,
                                         object value,
                                         Type destinationType)
        {
            throw new InvalidOperationException();
        }
    }

    public class PointXamlTemplateTypeConverterBad : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            throw new NotSupportedException();
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            throw new NotSupportedException();
        }
    }

    public class PointXamlTemplateTypeConverterGood : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var iProvideValueTarget = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;
            PropertyInfo pi = iProvideValueTarget.TargetProperty as PropertyInfo;
            Type type = pi.PropertyType;

            var loaderFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));

            return new PointWithXamlTemplate(loaderFactory, xamlReader);
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            PropertyInfo getNodes = value.GetType().GetProperty("Nodes");
            return ((PointWithXamlTemplate) value).Nodes.GetReader();
        }
    }
}
