// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Xaml.Utilities;
using X.Markup.L;

namespace Microsoft.Test.Xaml.XamlXmlWriterTests
{
    /// <summary>
    /// XamlXmlWriter XmlSupport Tests
    /// </summary>
    public static class XamlXmlWriter_XmlSupport
    {
        /// <summary>
        /// Xml LangSimple
        /// </summary>
        public static void XmlLangSimple()
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-16""?><String xml:lang=""en-US"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml"">Text</String>";

            StringWriter stringWriter = new StringWriter();
            XamlWriter xamlWriter = new XamlXmlWriter(stringWriter, new XamlSchemaContext());

            xamlWriter.WriteStartObject(XamlLanguage.String);
            xamlWriter.WriteStartMember(XamlLanguage.Lang);
            xamlWriter.WriteValue("en-US");
            xamlWriter.WriteEndMember();
            xamlWriter.WriteStartMember(XamlLanguage.Initialization);
            xamlWriter.WriteValue("Text");
            xamlWriter.WriteEndMember();
            xamlWriter.WriteEndObject();
            xamlWriter.Close();

            string generated = stringWriter.GetStringBuilder().ToString();
            Assert.AreEqual(generated, expected, "Expected [" + expected + "] \r\n Actual [" + generated + "]");
        }

        /// <summary>
        /// Xml As Prefix In ClrNamespace
        /// </summary>
        public static void XmlAsPrefixInClrNamespace()
        {
            string expected =
@"<p:ClassWithXmlNamespace xmlns=""http://randomNS"" xmlns:p=""clr-namespace:X.Markup.L;assembly=XamlClrTypes"">
  <p:ClassWithXmlNamespace.Text>
    <x:String xml:lang=""en-US"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">SAMPLESTRING</x:String>
  </p:ClassWithXmlNamespace.Text>
</p:ClassWithXmlNamespace>";

            XamlSchemaContext xsc = new XamlSchemaContext();
            var generated = new StringBuilder();
            var xmlSettings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            using (XamlXmlWriter writer = new XamlXmlWriter(XmlWriter.Create(new StringWriter(generated), xmlSettings), xsc))
            {
                writer.WriteNamespace(new NamespaceDeclaration("http://randomNS", String.Empty));
                XamlType a = xsc.GetXamlType(typeof(ClassWithXmlNamespace));
                writer.WriteStartObject(a);
                writer.WriteStartMember(a.GetMember("Text"));
                writer.WriteStartObject(XamlLanguage.String);
                writer.WriteStartMember(XamlLanguage.Lang);
                writer.WriteValue("en-US");
                writer.WriteEndMember();
                writer.WriteStartMember(XamlLanguage.Initialization);
                writer.WriteValue("SAMPLESTRING");
                writer.WriteEndMember();
                writer.WriteEndObject();
                writer.WriteEndMember();
                writer.WriteEndObject();
                writer.Close();
            }

            Assert.AreEqual(expected, generated.ToString(), "Expected [" + expected + "] \r\n Actual [" + generated.ToString() + "]");
        }

        /// <summary>
        /// Xml As Prefix In WriteNamespace
        /// </summary>
        public static void XmlAsPrefixInWriteNamespace()
        {
            StringWriter stringWriter = new StringWriter();
            XamlWriter xamlWriter = new XamlXmlWriter(stringWriter, new XamlSchemaContext());
            ExceptionHelper.ExpectException(
                delegate { xamlWriter.WriteNamespace(new NamespaceDeclaration("http://sampleNS", "xml")); },
                new ArgumentException(String.Empty, "namespaceDeclaration", null));
        }
    }
}
