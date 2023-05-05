// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Regression test
    /// v3 Compat: 4.0 parser doesn't set XmlAttributeProperties.XmlNamespaceMaps
    /// </summary>
    public class XmlNamespaceMapsTests
    {
        /// <summary>
        /// Use the XmlNamespaceMaps
        /// </summary>
        public static void RegressionIssue141()
        {
            const string Xaml =
            @"<Window xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
               xmlns:my='http://my'
               my:DemoControl.DemoProperty='1'
               Content='{Binding (my:DemoControl.DemoProperty), RelativeSource={RelativeSource Self}}' />";

            NamespaceMapEntry[] entries = new NamespaceMapEntry[] 
                                            {
                                                new NamespaceMapEntry
                                                {
                                                    XmlNamespace = "http://my",
                                                    ClrNamespace = "Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests",
                                                    AssemblyName = "XamlWpfTests40"
                                                }
                                            };

            XamlTypeMapper mapper = new XamlTypeMapper(new string[0], entries);

            ParserContext pc = new ParserContext { XamlTypeMapper = mapper };
            Window window = (Window)XamlReader.Parse(Xaml, pc);

            window.Show();
            object content = window.Content;
            window.Close();
            Assert.AreEqual(1, content);
        }

        /// <summary>
        /// Default XmlNamespaceMaps should not be null 
        /// </summary>
        public static void RegressionIssue141_1()
        {
            const string Xaml =
            @"<Button
                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' />";

            DependencyObject dob = (DependencyObject)XamlReader.Parse(Xaml);
            Hashtable mappings = dob.GetValue(XmlAttributeProperties.XmlNamespaceMapsProperty) as Hashtable;
            Assert.AreNotEqual(mappings, null);
        }
    }

    /// <summary>
    /// Demo control
    /// </summary>
    public class DemoControl : Control
    {
        /// <summary>
        /// Demo property
        /// </summary>
        public static readonly DependencyProperty DemoPropertyProperty =
            DependencyProperty.RegisterAttached(
                                                "DemoProperty",
                                                typeof(int),
                                                typeof(DemoControl));

        /// <summary>
        /// Gets the Demo dependency property
        /// </summary>
        /// <param name="obj">depenedncy object</param>
        /// <returns>property value</returns>
        public static int GetDemoProperty(DependencyObject obj)
        {
            return (int)obj.GetValue(DemoPropertyProperty);
        }

        /// <summary>
        /// Set the Demo dependency property
        /// </summary>
        /// <param name="obj">depenedncy object</param>
        /// <param name="value">property value</param>
        public static void SetDemoProperty(DependencyObject obj, int value)
        {
            obj.SetValue(DemoPropertyProperty, value);
        }
    }
}
