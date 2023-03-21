// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Annotations.Storage;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    public class AXmlStreamStoreSuite : TestSuite
    {
        #region Protected Methods

        protected void VerifyRegisteringNamespacesFails(IDictionary<Uri, IList<Uri>> namespaces)
        {
            bool exception = false;
            try
            {
                new XmlStreamStore(new MemoryStream(), namespaces);
            }
            catch (ArgumentException e)
            {
                exception = true;
                printStatus("Expected Exception: " + e.Message);
            }
            Assert("Verify expected exception occurred.", exception);
        }

        protected void VerifyIgnoredNamespaces(XmlStreamStore store, Uri[] expectedNamespaces)
        {
            IList<Uri> ignoredNamespaces = store.IgnoredNamespaces;
            bool failed = (expectedNamespaces.Length != ignoredNamespaces.Count);
            if (!failed)
            {
                for (int i = 0; i < expectedNamespaces.Length; i++)
                {
                    if (!expectedNamespaces[i].Equals(ignoredNamespaces[i]))
                    {
                        failed = true;
                        break;
                    }
                }
            }
            if (failed)
            {
                printStatus("Expected IgnoredNamespaces:");
                foreach (Uri ns in expectedNamespaces)
                    printStatus("\t" + ns);
                printStatus("Actual IgnoredNamespaces:");
                foreach (Uri ns in ignoredNamespaces)
                    printStatus("\t" + ns);
                failTest("Actual IgnoredNamespaces differed from expected.");
            }

            AssertEquals("Verify number of loadable annotations.", 2, store.GetAnnotations().Count);
        }

        protected XmlStreamStore VerifyExceptionParsingStream(Stream stream)
        {
            XmlStreamStore store = null;
            bool exception = false;
            try
            {
                store = new XmlStreamStore(stream);
            }
            catch (Exception e)
            {
                printStatus(e.ToString());
                exception = true;
            }
            Assert("Verify expected exception occurred.", exception);
            return store;
        }

        protected IDictionary<Uri, IList<Uri>> CreateNamespaceDictionary(Uri key, Uri[] value)
        {
            IList<Uri> list = new List<Uri>();
            if (value == null)
                list = null;
            else
            {
                foreach (Uri entry in value)
                    list.Add(entry);
            }
            IDictionary<Uri, IList<Uri>> namespaces = new Dictionary<Uri, IList<Uri>>();
            namespaces.Add(key, list);
            return namespaces;
        }

        protected IDictionary<Uri, IList<Uri>> CreateNamespaceDictionary(Uri key)
        {
            IDictionary<Uri, IList<Uri>> namespaces = new Dictionary<Uri, IList<Uri>>();
            namespaces.Add(key, null);
            return namespaces;
        }

        protected IDictionary<Uri, IList<Uri>> CreateNamespaceDictionary(Uri [] keys, Uri[][] values)
        {
            IDictionary<Uri, IList<Uri>> namespaces = CreateNamespaceDictionary(keys[0], values[0]);
            for (int i = 1; i < keys.Length; i++)
            {
                IList<Uri> list = new List<Uri>();
                foreach (Uri entry in values[i])
                    list.Add(entry);
                namespaces.Add(keys[i], list);
            }
            return namespaces;
        }

        protected Stream CreateStreamFromTemplate(string namespaceDefinitions, string annotationAttributeUsage)
        {
            return CreateStreamFromTemplate(namespaceDefinitions, annotationAttributeUsage, "", "");
        }

        protected Stream CreateStreamFromTemplate(string namespaceDefinitions, string annotationAttributeUsage, string cargoAddition, string authorAddition)
        {
            Stream templateStream = Application.GetResourceStream(new Uri(TemplateStoreFilename, UriKind.Relative)).Stream;
            StreamReader reader = new StreamReader(templateStream);
            string file = reader.ReadToEnd();
            reader.Close();

            file = file.Replace(ExternalNamespaceTag, namespaceDefinitions);
            file = file.Replace(AnnotationNodeAttributeTag, annotationAttributeUsage);
            file = file.Replace(StickyNoteCargoTag, cargoAddition);
            file = file.Replace(AuthorTag, authorAddition);

            Stream finalStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(finalStream);
            writer.Write(file);
            writer.Flush();

            finalStream.Seek(0, SeekOrigin.Begin);
            return finalStream;
        }

        protected string AlternateAuthorXml(string[] namespaces, string[] authors, string fallbackAuthor)
        {
            string xml = string.Empty;
            AssertEquals("Verify input.", namespaces.Length, authors.Length);
            for (int i=0; i < namespaces.Length; i++) 
                xml += "<x:Choice Requires=\"" + namespaces[i] + "\"><anb:StringAuthor>" + authors[i] + "</anb:StringAuthor></x:Choice>";
            if (fallbackAuthor != null)
                xml += "<x:Fallback><anb:StringAuthor>" + fallbackAuthor + "</anb:StringAuthor></x:Fallback>";
            xml = "<x:AlternateContent>" + xml + "</x:AlternateContent>";
            return xml;
        }

        #endregion

        #region Fields

        protected Uri[] DefaultKnownNamespaces = new Uri[] {
                new Uri("http://schemas.microsoft.com/winfx/2006/xaml/presentation"),
                new Uri("http://schemas.microsoft.com/windows/annotations/2003/11/core"),
                new Uri("http://schemas.microsoft.com/windows/annotations/2003/11/base"),
            };

        protected Uri UnknownNamespace1 = new Uri("http://external.company.com/2005/");
        protected Uri UnknownNamespace2 = new Uri("http://schemas.microsoft.com/windows/annotations/2010/42/core");
        protected Uri UnknownNamespace3 = new Uri("http://foobar.bogus.com/1941/us-en");

        protected Uri CompatibilityReaderNamespace = new Uri("http://schemas.openxmlformats.org/markup-compatibility/2006");

        protected string TemplateStoreFilename = "AnnotationStoreTemplate.xml";

        protected string ExternalNamespaceTag = "EXTERNAL_NAMESPACE_DEFINITION";
        protected string AnnotationNodeAttributeTag = "ANNOTATION_NODE_ATTRIBUTES";
        protected string StickyNoteCargoTag = "STICKYNOTE_CARGO";
        protected string AuthorTag = "ANNOTATION_AUTHOR_TAG";

        #endregion
    }
}

