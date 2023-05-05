// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xaml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.NonParserMethodTests.RegressionIssue7
{
    /// <summary>
    /// Regression test

    public class RegressionIssue7_XOWSecurity
    {
        /// <summary>
        /// Verifies that delegates passed to XamlObjectWriter are not visible to type converters and markup extensions
        /// </summary>
        public void Run()
        {
            string xaml = @"<Root xmlns=""clr-namespace:Microsoft.Test.Xaml.NonParserMethodTests.RegressionIssue7;assembly=XamlWpfTests40"" TCProperty=""abcd"" MEProperty=""{TypeAExtension efgh}"" />";

            XamlXmlReader xamlReader = new XamlXmlReader(new StringReader(xaml));

            XamlObjectWriterSettings xowSettings = new XamlObjectWriterSettings();
            xowSettings.AfterBeginInitHandler = new EventHandler<XamlObjectEventArgs>(AfterBeginInitCallback);
            xowSettings.AfterEndInitHandler = new EventHandler<XamlObjectEventArgs>(AfterEndInitCallback);
            xowSettings.AfterPropertiesHandler = new EventHandler<XamlObjectEventArgs>(AfterPropertiesCallback);
            xowSettings.BeforePropertiesHandler = new EventHandler<XamlObjectEventArgs>(BeforePropertiesCallback);
            xowSettings.XamlSetValueHandler = new EventHandler<System.Windows.Markup.XamlSetValueEventArgs>(XamlSetValueCallback);

            XamlObjectWriter xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, xowSettings);

            try
            {
                XamlServices.Transform(xamlReader, xamlWriter);
            }
            catch (XamlObjectWriterException exception)
            {
                // Asserts in TypeAConverter and TypeAExtension throw TestValidationException if they fail
                if (exception.InnerException != null && exception.InnerException.GetType() == typeof(TestValidationException))
                {
                    throw new TestValidationException("Delegates passed to XamlObjectWriter are visible to type converters and markup extensions. This is a security bug RegressionIssue7. ", exception.InnerException);
                }
                else
                {
                    throw new TestValidationException("Unexpected exception occurred.\n", exception);
                }
            }
            catch (Exception exception)
            {
                throw new TestValidationException("Unexpected exception occurred.\n", exception);
            }

            Root root = (Root)xamlWriter.Result;

            if (root.TCProperty.StringProperty != "abcd")
            {
                throw new TestValidationException("Unexpected value of root.TCProperty.StringProperty. Expected: abcd. Actual: " + root.TCProperty.StringProperty);
            }

            if (root.MEProperty.StringProperty != "efgh")
            {
                throw new TestValidationException("Unexpected value of root.MEProperty.StringProperty. Expected: abcd. Actual: " + root.MEProperty.StringProperty);
            }

            GlobalLog.LogEvidence("Delegates passed to XamlObjectWriter are not visible to type converters and markup extensions (as expected).");
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Dummy callback method to set XamlObjectWriterSettings.AfterBeginInitHandler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void AfterBeginInitCallback(object sender, XamlObjectEventArgs e)
        {
        }

        /// <summary>
        /// Dummy callback method to set XamlObjectWriterSettings.AfterEndInitHandler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void AfterEndInitCallback(object sender, XamlObjectEventArgs e)
        {
        }

        /// <summary>
        /// Dummy callback method to set XamlObjectWriterSettings.AfterPropertiesHandler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void AfterPropertiesCallback(object sender, XamlObjectEventArgs e)
        {
        }

        /// <summary>
        /// Dummy callback method to set XamlObjectWriterSettings.BeforePropertiesHandler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void BeforePropertiesCallback(object sender, XamlObjectEventArgs e)
        {
        }

        /// <summary>
        /// Dummy callback method to set XamlObjectWriterSettings.XamlSetValueHandler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void XamlSetValueCallback(object sender, System.Windows.Markup.XamlSetValueEventArgs e)
        {
        }
    }

    /// <summary>
    /// Root of the test xaml
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Gets or sets This is a property
        /// </summary>
        public TypeA TCProperty { get; set; }

        /// <summary>
        /// Gets or sets This is a property
        /// </summary>
        public TypeA MEProperty { get; set; }
    }

    /// <summary>
    /// Test type used by the test
    /// </summary>
    [TypeConverter(typeof(TypeAConverter))]
    public class TypeA
    {
        /// <summary>
        /// Gets or sets String property
        /// </summary>
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// Type converter for TypeA
    /// </summary>
    public class TypeAConverter : TypeConverter
    {
        /// <summary>
        /// Can convert from method
        /// </summary>
        /// <param name="context">This is a context</param>
        /// <param name="sourceType">This is a sourceType</param>
        /// <returns>Returns a bool</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Convert from method
        /// </summary>
        /// <param name="context">This is a context</param>
        /// <param name="culture">This is a culture</param>
        /// <param name="value">This is a value</param>
        /// <returns>Returns an object</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            IXamlObjectWriterFactory xowFactory = (IXamlObjectWriterFactory)context.GetService(typeof(IXamlObjectWriterFactory));
            XamlObjectWriterSettings xowSettings = xowFactory.GetParentSettings();

            Assert.IsNull(xowSettings.AfterBeginInitHandler, "xowSettings.AfterBeginInitHandler is not null");
            Assert.IsNull(xowSettings.AfterEndInitHandler, "xowSettings.AfterEndInitHandler is not null");
            Assert.IsNull(xowSettings.AfterPropertiesHandler, "xowSettings.AfterPropertiesHandler is not null");
            Assert.IsNull(xowSettings.BeforePropertiesHandler, "xowSettings.BeforePropertiesHandler is not null");
            Assert.IsNull(xowSettings.XamlSetValueHandler, "xowSettings.XamlSetValueHandler is not null");

            if (value.GetType() == typeof(string))
            {
                return new TypeA() { StringProperty = (string)value };
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Markup extension that returns TypeA
    /// </summary>
    public class TypeAExtension : System.Windows.Markup.MarkupExtension
    {
        /// <summary>
        /// Input field used by this ME
        /// </summary>
        private string _input;

        /// <summary>
        /// Initializes a new instance of the TypeAExtension class
        /// </summary>
        /// <param name="arg">Input argument to be used</param>
        public TypeAExtension(string arg)
        {
            _input = arg;
        }

        /// <summary>
        /// Provide value method
        /// </summary>
        /// <param name="serviceProvider">This is a serviceProvider</param>
        /// <returns>Returns an object</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlObjectWriterFactory xowFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));
            XamlObjectWriterSettings xowSettings = xowFactory.GetParentSettings();

            Assert.IsNull(xowSettings.AfterBeginInitHandler, "xowSettings.AfterBeginInitHandler is not null");
            Assert.IsNull(xowSettings.AfterEndInitHandler, "xowSettings.AfterEndInitHandler is not null");
            Assert.IsNull(xowSettings.AfterPropertiesHandler, "xowSettings.AfterPropertiesHandler is not null");
            Assert.IsNull(xowSettings.BeforePropertiesHandler, "xowSettings.BeforePropertiesHandler is not null");
            Assert.IsNull(xowSettings.XamlSetValueHandler, "xowSettings.XamlSetValueHandler is not null");

            return new TypeA() { StringProperty = (string)_input };
        }
    }
}
