// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.NameScope
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\NameScope\NameScope.xaml
    /// </summary>
    public static class NameScopeBasic_Verify
    {
        /// <summary>
        /// This method checks that the name property of element set properly,
        /// it only checks witin a single NameScopes
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            Page page = rootElement as Page;
            TextBlock textBlock = page.FindName("textBlock1") as TextBlock;
            Label label = page.FindName("label1") as Label;
            Button button = page.FindName("button1") as Button;

            // verifying the same instance of object is accessed
            if (textBlock != null)
            {
                if (textBlock.Text != "bar")
                {
                    GlobalLog.LogEvidence("textBlock.Text != bar");
                    result = false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("textBlock == null");
                result = false;
            }

            // verifying the same instance of object is accessed
            if (label != null)
            {
                if (label.Content.ToString() != "Label")
                {
                    GlobalLog.LogEvidence("label.Content != Label");
                    result = false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("label == null");
                result = false;
            }

            // verifying the same instance of object is accessed
            if (button != null)
            {
                if (button.Content.ToString() != "Button")
                {
                    GlobalLog.LogEvidence("button.Content != Button");
                    result = false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("button == null");
                result = false;
            }

            return result;
        }
    }
}
