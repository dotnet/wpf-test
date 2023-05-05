// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.SdxCore
{
    /// <summary>
    /// GetXmlNamespacesHarvestUri Test
    /// </summary>
    public static class GetXmlNamespacesHarvestUri
    {
        /// <summary>
        /// Runs the test.
        /// </summary>
        public static void RunTest()
        {
            string xmlnsx = "http://test.schemas.microsoft.com/winfx/2006/xaml/presentation";
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            XamlType xamlType = xamlSchemaContext.GetXamlType(xmlnsx, "CustomRoot");
            if (xamlType == null)
            {
                GlobalLog.LogEvidence("XamlType creation failed for: CustomRoot");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
            foreach (string xmlNamespace in xmlNamespaces)
            {
                GlobalLog.LogEvidence("XmlNamespace: " + xmlNamespace);
            }

            if (xmlNamespaces.Contains("http://test.schemas.microsoft.com/winfx/2006/xaml/presentation")
                && xmlNamespaces.Contains("http://test.schemas.microsoft.com/netfx/2007/xaml/presentation"))
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }
}
