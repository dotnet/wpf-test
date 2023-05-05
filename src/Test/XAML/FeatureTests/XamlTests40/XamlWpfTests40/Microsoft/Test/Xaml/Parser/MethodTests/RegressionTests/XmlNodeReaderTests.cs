// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;

[assembly:XmlnsDefinition("http://RegressionIssue143", "Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests.RegressionIssue143")]

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests.RegressionIssue143
{
    /// <summary>
    /// Regression tests
    /// </summary>
    public class XmlNodeReaderTests
    {
        /// <summary>
        /// Verify that a WPF xaml document that has object element syntax, passed in using XmlNodeReader, loads fine
        /// Uses XamlReader.Load
        /// </summary>
        public static void ObjectElementSyntaxTest()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement flowDocumentElement = doc.CreateElement(null, "FlowDocument", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            flowDocumentElement.SetAttribute("xml:space", "preserve");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(flowDocumentElement);
            object rootObject = System.Windows.Markup.XamlReader.Load(xmlNodeReader);

            if (rootObject.GetType() != typeof(FlowDocument))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected FlowDocument");
            }
        }

        /// <summary>
        /// Verify that a WPF xaml document that has property element syntax, passed in using XmlNodeReader, loads fine
        /// Uses XamlReader.Load
        /// </summary>
        public static void PropertyElementSyntaxTest()
        {
            string wpfns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XmlDocument doc = new XmlDocument();
            XmlElement canvasElement = doc.CreateElement(null, @"Canvas", wpfns);
            doc.AppendChild(canvasElement);
            XmlElement buttonElement = doc.CreateElement(null, @"Button", wpfns);
            canvasElement.AppendChild(buttonElement);
            XmlElement leftElement = doc.CreateElement(null, @"Canvas.Left", wpfns);
            buttonElement.AppendChild(leftElement);
            leftElement.AppendChild(doc.CreateTextNode("50"));

            XmlReader xmlNodeReader = new XmlNodeReader(doc);
            object rootObject = (Canvas)System.Windows.Markup.XamlReader.Load(xmlNodeReader);

            object childObject = (rootObject as Panel).Children[0];
            if (rootObject.GetType() != typeof(Canvas))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected Canvas");
            }
            else if (childObject.GetType() != typeof(Button))
            {
                throw new TestValidationException("childObject is of type " + childObject.GetType() + ". Expected Button");
            }
            else if ((double)(childObject as DependencyObject).GetValue(Canvas.LeftProperty) != 50)
            {
                throw new TestValidationException("Canvas.Left is " + (double)(childObject as DependencyObject).GetValue(Canvas.LeftProperty) + ". Expected 50");
            }
        }

        /// <summary>
        /// Test whether a non-WPF xaml with Poco object, passed in using XmlNodeReader, loads fine
        /// Uses XamlServices.Load
        /// </summary>
        public static void PocoTypeTest()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement pocoTypeElement = doc.CreateElement(null, "PocoType", "http://RegressionIssue143");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(pocoTypeElement);
            object rootObject = System.Xaml.XamlServices.Load(xmlNodeReader);

            if (rootObject.GetType() != typeof(PocoType))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected PocoType");
            }
        }

        /// <summary>
        /// Test whether a non-WPF xaml with generic object, passed in using XmlNodeReader, loads fine
        /// Uses XamlServices.Load
        /// </summary>
        public static void GenericTypeTest()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement genericTypeElement = doc.CreateElement(null, "GenericType", "http://RegressionIssue143");
            genericTypeElement.SetAttribute("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
            genericTypeElement.SetAttribute("x:TypeArguments", "x:String");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(genericTypeElement);
            object rootObject = System.Xaml.XamlServices.Load(xmlNodeReader);

            if (rootObject.GetType() != typeof(GenericType<string>))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected GenericType<string>");
            }
        }

        /// <summary>
        /// If xmlns is also specified along with namespaceURI(passed to CreateElement), namespaceURI should win and xmlns should be ignored
        /// Uses XamlServices.Load
        /// </summary>
        public static void XmlnsIgnoreTest()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement pocoTypeElement = doc.CreateElement(null, "PocoType", "http://RegressionIssue143");
            pocoTypeElement.SetAttribute("xmlns", "http://invalidnamespace");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(pocoTypeElement);
            object rootObject = System.Xaml.XamlServices.Load(xmlNodeReader);

            if (rootObject.GetType() != typeof(PocoType))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected PocoType");
            }
        }

        /// <summary>
        /// If xmlns:prefix is also specified along with namespaceURI(passed to CreateElement), namespaceURI should win and xmlns should be ignored
        /// Uses XamlServices.Load
        /// </summary>
        public static void XmlnsIgnoreWithPrefixTest()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement pocoTypeElement = doc.CreateElement("prefixA", "PocoType", "http://RegressionIssue143");
            pocoTypeElement.SetAttribute("xmlns:prefixA", "http://invalidnamespace");

            XmlNodeReader xmlNodeReader = new XmlNodeReader(pocoTypeElement);
            object rootObject = System.Xaml.XamlServices.Load(xmlNodeReader);

            if (rootObject.GetType() != typeof(PocoType))
            {
                throw new TestValidationException("rootObject is of type " + rootObject.GetType() + ". Expected PocoType");
            }
        }
    }

    /// <summary>
    /// Plain old CLR object
    /// </summary>
    public class PocoType
    {
    }

    /// <summary>
    /// Generic type
    /// </summary>
    /// <typeparam name="T">Type argument</typeparam>
    public class GenericType<T>
    {
    }
}
