// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Template
{
    /// <summary>
    /// Verification class for file: FeatureTests\XAML\Data\Parser\Template\IXamlTemplate_BVT.xaml
    /// </summary>
    public static class IXamlTemplate_BVT_Verify
    {
        /// <summary>
        /// Verifies that the System.Xaml template system can handle a string
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRoot root = (CustomRoot) rootElement;
            Custom_TemplateHost host = (Custom_TemplateHost) root.Content;

            string child = (string) host.Child;

            if (child != "TestTemplateString")
            {
                GlobalLog.LogEvidence("Custom_TemplateHost.Child did not equal \"TestTemplateString\", failing test.");
                result = false;
            }

            return result;
        }
    }
}
