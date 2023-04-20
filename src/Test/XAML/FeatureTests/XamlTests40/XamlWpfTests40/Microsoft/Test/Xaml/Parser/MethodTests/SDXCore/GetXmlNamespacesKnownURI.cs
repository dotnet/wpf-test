// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.SdxCore
{
    /// <summary>
    /// GetXmlNamespacesKnownUri Test
    /// </summary>
    public static class GetXmlNamespacesKnownUri
    {
        /// <summary>
        /// Runs the test.
        /// </summary>
        public static void RunTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xmlnsx = "http://schemas.microsoft.com/winfx/2006/xaml";
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            XamlType xamlType = xamlSchemaContext.GetXamlType(xmlnsx, "Array");
            if (xamlType == null)
            {
                GlobalLog.LogEvidence("XamlType creation failed for: Array");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
            foreach (string xmlNamespace in xmlNamespaces)
            {
                if (xmlNamespace == xmlnsx)
                {
                    GlobalLog.LogEvidence("Found XmlNamespace: " + xmlNamespace);
                    TestLog.Current.Result = TestResult.Pass;
                    return;
                }
            }

            TestLog.Current.Result = TestResult.Fail;
        }
    }
}
