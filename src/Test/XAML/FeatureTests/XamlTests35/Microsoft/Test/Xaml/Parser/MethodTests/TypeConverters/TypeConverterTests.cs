// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Serialization;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.TypeConverters
{
    /// <summary>
    /// Verifies basic behavior of various TypeConverters.
    /// </summary>
    public class TypeConverterTests
    {
        /// <summary>
        /// Verifies basic behavior of TemplateKeyConverter.
        /// </summary>
        public void VerifyTemplateKeyConverter()
        {
            DataTemplateKey templateKey = new DataTemplateKey(typeof(DependencyObject));
            TemplateKeyConverter converter = new TemplateKeyConverter();

            GlobalLog.LogStatus("Verify cannot convert to or from string...");
            if (converter.CanConvertTo(null, typeof(string)) || converter.CanConvertFrom(null, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            GlobalLog.LogStatus("Verify ConvertTo and ConvertFrom throw...");
            Exception ex = null;
            try { converter.ConvertTo(null, CultureInfo.InvariantCulture, templateKey, typeof(string)); }
            catch (Exception e) { ex = e; }

            if (ex == null) throw new Microsoft.Test.TestValidationException("FAILED");

            ex = null;
            try { converter.ConvertFrom(null, CultureInfo.InvariantCulture, ""); }
            catch (Exception e) { ex = e; }

            if (ex == null) throw new Microsoft.Test.TestValidationException("FAILED");

        }
        /// <summary>
        /// Verifies basic behavior of RoutedEventConverter.
        /// </summary>
        public void VerifyRoutedEventConverter()
        {
            RoutedEventConverter converter = new RoutedEventConverter();

            FrameworkElement frameworkElement = new FrameworkElement();
            InternalObject internalObj = InternalObject.CreateInstance(frameworkElement.GetType(), "System.Windows.Markup.TypeConvertContext", new object[] { _CreateParserContext() });
            
            ITypeDescriptorContext context = (ITypeDescriptorContext)internalObj.Target;

            GlobalLog.LogStatus("Verify cannot convert to string...");
            if (converter.CanConvertTo(null, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            GlobalLog.LogStatus("Verify ConvertTo throws...");
            Exception ex = null;
            try { converter.ConvertTo(null, CultureInfo.InvariantCulture, Mouse.MouseUpEvent, typeof(string)); }
            catch (Exception e) { ex = e; }

            if (ex == null) throw new Microsoft.Test.TestValidationException("FAILED");

            GlobalLog.LogStatus("Verify can convert from string...");
            if (!converter.CanConvertFrom(context, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            object obj = converter.ConvertFrom(context, CultureInfo.InvariantCulture, "UIElement.MouseUp");

            if (!(obj is RoutedEvent)) throw new Microsoft.Test.TestValidationException("FAILED");

            if (obj != Mouse.MouseUpEvent) throw new Microsoft.Test.TestValidationException("FAILED");

            obj = converter.ConvertFrom(context, CultureInfo.InvariantCulture, "Mouse.MouseUp");

            if (!(obj is RoutedEvent)) throw new Microsoft.Test.TestValidationException("FAILED");

            if (obj != Mouse.MouseUpEvent) throw new Microsoft.Test.TestValidationException("FAILED");
        }
        /// <summary>
        /// Verifies basic behavior of DependencyPropertyConverter.
        /// </summary>
        public void VerifyDependencyPropertyConverter()
        {
            DependencyPropertyConverter converter = new DependencyPropertyConverter();

            FrameworkElement frameworkElement = new FrameworkElement();
            InternalObject internalObj = InternalObject.CreateInstance(frameworkElement.GetType(), "System.Windows.Markup.TypeConvertContext", new object[] { _CreateParserContext() });

            ITypeDescriptorContext context = (ITypeDescriptorContext)internalObj.Target;

            GlobalLog.LogStatus("Verify cannot convert to string...");
            if (converter.CanConvertTo(null, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            GlobalLog.LogStatus("Verify ConvertTo throws...");
            Exception ex = null;
            try { converter.ConvertTo(null, CultureInfo.InvariantCulture, Mouse.MouseUpEvent, typeof(string)); }
            catch (Exception e) { ex = e; }

            if (ex == null) throw new Microsoft.Test.TestValidationException("FAILED");

            GlobalLog.LogStatus("Verify can convert from string...");
            if (!converter.CanConvertFrom(context, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            object obj = converter.ConvertFrom(context, CultureInfo.InvariantCulture, "UIElement.IsEnabled");

            if (!(obj is DependencyProperty)) throw new Microsoft.Test.TestValidationException("FAILED");

            if (obj != UIElement.IsEnabledProperty) throw new Microsoft.Test.TestValidationException("FAILED");
        }
        /// <summary>
        /// Verifies basic behavior of InputScopeNameConverter.
        /// </summary>
        public void VerifyInputScopeNameConverter()
        {
            InputScopeNameConverter converter = new InputScopeNameConverter();

            Array values = Enum.GetValues(typeof(InputScopeNameValue));

            foreach (InputScopeNameValue value in values)
            {

                InputScopeName scopeName = new InputScopeName(value);
                string nameString = Enum.GetName(typeof(InputScopeNameValue), value);

                GlobalLog.LogStatus("Verifying InputScopeNameValue." + nameString + "...");

                ITypeDescriptorContext context = new InputScopeNameConverter_TypeDescriptorContext(scopeName);

                // Verify can convert to string.
                if (!converter.CanConvertTo(context, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

                object obj = converter.ConvertTo(context, CultureInfo.InvariantCulture, scopeName, typeof(string));

                if (!(obj is string)) throw new Microsoft.Test.TestValidationException("FAILED");

                if ((string)obj != nameString) throw new Microsoft.Test.TestValidationException("FAILED");

                // Verify can convert from string.
                if (!converter.CanConvertFrom(context, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

                obj = converter.ConvertFrom(context, CultureInfo.InvariantCulture, (string)obj);

                if (!(obj is InputScopeName)) throw new Microsoft.Test.TestValidationException("FAILED");

                if (((InputScopeName)obj).NameValue != scopeName.NameValue) throw new Microsoft.Test.TestValidationException("FAILED");
            }
        }

        private ParserContext _CreateParserContext()
        {
            // Create the XmlParserContext.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            nsmgr.AddNamespace("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            ParserContext parserContext = new ParserContext(context);
            parserContext.XamlTypeMapper = new XamlTypeMapper(new string[] { "PresentationFramework", "PresentationCore" });

            return parserContext;
        }
        // Dummy ITypeDescriptorContext to use for testing InputScopeNameConverter.
        private class InputScopeNameConverter_TypeDescriptorContext : ITypeDescriptorContext
        {
            public InputScopeNameConverter_TypeDescriptorContext(InputScopeName name)
            {
                _name = name;
            }

            #region ITypeDescriptorContext Members

            System.ComponentModel.IContainer System.ComponentModel.ITypeDescriptorContext.Container
            {
                get { return null; }
            }

            // Returns a value of a property - to be detected for convertability to string in a type converter
            object System.ComponentModel.ITypeDescriptorContext.Instance
            {
                get
                {
                    return _name;
                }
            }

            void System.ComponentModel.ITypeDescriptorContext.OnComponentChanged()
            {
            }

            bool System.ComponentModel.ITypeDescriptorContext.OnComponentChanging()
            {
                return false;
            }

            System.ComponentModel.PropertyDescriptor System.ComponentModel.ITypeDescriptorContext.PropertyDescriptor
            {
                get { return null; }
            }

            #endregion

            #region IServiceProvider Members

            object IServiceProvider.GetService(Type serviceType)
            {
                return null;
            }

            #endregion

            private readonly InputScopeName _name = null;
        }

    }
}
