// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Inspect an Xpath
    /// </summary>
    internal class XPathInspector
    {
        /// <summary>
        /// Validate/Inspect xaml against a collection of xpath expressions
        /// </summary>
        /// <param name="xaml">xaml to inspect</param>
        /// <param name="xpathExpressions">collection of xpath expressions</param>
        /// <param name="prefixMaps">mappings of prefix's to namespaces</param>
        public static void Inspect(string xaml, ICollection<string> xpathExpressions, IDictionary<string, string> prefixMaps)
        {
            XPathDocument doc = new XPathDocument(new StringReader(xaml));
            XPathNavigator navigator = doc.CreateNavigator();
            foreach (string expression in xpathExpressions)
            {
                XPathNodeIterator iterator = null;

                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(navigator.NameTable);
                foreach (string name in prefixMaps.Keys)
                {
                    namespaceManager.AddNamespace(name, prefixMaps[name]);
                }

                iterator = navigator.Select(expression, namespaceManager);

                if (iterator == null || iterator.Count == 0)
                {
                    Tracer.LogTrace("The xpath:{0} does not match any nodes in the xaml:{1}", expression, xaml);
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, "The xpath '{0}' does not map to any node in the above xaml document.", expression));
                }
            }
        }
    }
}
