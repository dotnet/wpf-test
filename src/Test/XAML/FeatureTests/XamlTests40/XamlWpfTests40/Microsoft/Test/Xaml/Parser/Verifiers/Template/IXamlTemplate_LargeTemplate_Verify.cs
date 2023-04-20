// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Documents;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Template
{
    /// <summary>
    /// Verification class for file: FeatureTests\XAML\Data\Parser\Template\IXamlTemplate_LargeTemplate.xaml
    /// </summary>
    public static class IXamlTemplate_LargeTemplate_Verify
    {
        /// <summary>
        /// Verifies that the System.Xaml templates can handle a large template
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRoot root = (CustomRoot) rootElement;
            Custom_TemplateHost host = (Custom_TemplateHost) root.Content;

            FlowDocument flowDoc = (FlowDocument) host.Child;
            Paragraph childPara = flowDoc.FindName("TargetParagraph") as Paragraph;
            if (childPara == null)
            {
                GlobalLog.LogEvidence("The target paragraph was null");
                result = false;
            }

            return result;
        }
    }
}
