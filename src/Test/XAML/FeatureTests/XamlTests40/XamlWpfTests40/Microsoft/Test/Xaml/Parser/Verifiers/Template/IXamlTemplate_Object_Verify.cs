// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Template
{
    /// <summary>
    /// Verification class for file: FeatureTests\XAML\Data\Parser\Template\IXamlTemplate_Object.xaml
    /// </summary>
    public static class IXamlTemplate_Object_Verify
    {
        /// <summary>
        /// Verifies that the System.Xaml templates can handle a custom object
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            CustomRoot root = (CustomRoot) rootElement;
            Custom_TemplateHost host = (Custom_TemplateHost) root.Content;

            Custom_Clr_StringID child = (Custom_Clr_StringID) host.Child;

            if (child.ID != "TemplateInnerObjectID")
            {
                GlobalLog.LogEvidence("Custom_TemplateHost.Child.ID did not equal \"TemplateInnerObjectID\", failing test.");
                result = false;
            }

            return result;
        }
    }
}
