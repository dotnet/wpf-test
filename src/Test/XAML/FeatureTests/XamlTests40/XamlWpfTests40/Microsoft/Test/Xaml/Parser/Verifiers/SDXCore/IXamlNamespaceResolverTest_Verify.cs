// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.SdxCore
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\SDXCore\IXamlNamespaceResolverTest.xaml
    /// </summary>
    public static class IXamlNamespaceResolverTest_Verify
    {
        /// <summary>
        /// Methof verifies the IXamlNamespaceResolver Serviceprovider returns the correct values
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            string rootNS = "http://XamlTestTypes";
            string ctaNS = "http://XamlTestTypes";
            string xNS = "http://schemas.microsoft.com/winfx/2006/xaml";

            CustomRootWithCollection root = (CustomRootWithCollection) rootElement;

            Custom_XamlNamespaceResolverTestObject element1 = (Custom_XamlNamespaceResolverTestObject) root.Content[0];
            Custom_XamlNamespaceResolverTestObject element2 = (Custom_XamlNamespaceResolverTestObject) root.Content[1];
            Custom_XamlNamespaceResolverTestObject element3 = (Custom_XamlNamespaceResolverTestObject) root.Content[2];

            if (element1.MEProp != rootNS || element1.TCProp != rootNS)
            {
                GlobalLog.LogEvidence("Element1 failed.  MEProp=" + element1.MEProp + " TCProp=" + element1.TCProp + "Expected: " + rootNS);
                result = false;
            }

            if (element2.MEProp != ctaNS || element2.TCProp != ctaNS)
            {
                GlobalLog.LogEvidence("Element2 failed.  MEProp=" + element2.MEProp + " TCProp=" + element2.TCProp + "Expected: " + ctaNS);
                result = false;
            }

            if (element3.MEProp != xNS || element3.TCProp != xNS)
            {
                GlobalLog.LogEvidence("Element3 failed.  MEProp=" + element3.MEProp + " TCProp=" + element3.TCProp + "Expected: " + xNS);
                result = false;
            }

            return result;
        }
    }
}
