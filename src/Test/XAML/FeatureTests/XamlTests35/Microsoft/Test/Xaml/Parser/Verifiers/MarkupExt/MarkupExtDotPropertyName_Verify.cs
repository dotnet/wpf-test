// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary>
    /// Verifier class for Xaml file:
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\MarkupExt\MarkupExtDotPropertyName.xaml
    /// </summary>
    public sealed class MarkupExtDotPropertyName_Verify
    {
        /// <summary>
        /// Verifies the specified root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;

            StackPanel root = rootElement as StackPanel;
            TextBox textBox = root.Children[0] as TextBox;
            Binding bind = BindingOperations.GetBinding(textBox, TextBox.TextProperty);

            if (bind.NotifyOnTargetUpdated != true)
            {
                GlobalLog.LogEvidence("NotifyOnTargetUpdated was false");
                result = false;
            }

            if (bind.NotifyOnSourceUpdated != false)
            {
                GlobalLog.LogEvidence("NotifyOnTargetUpdated was true");
                result = false;
            }

            if (textBox.Text != root.Orientation.ToString())
            {
                GlobalLog.LogEvidence("TextBox.Text was incorrect");
                GlobalLog.LogEvidence("Expected: " + root.Orientation.ToString());
                GlobalLog.LogEvidence("Found: " + textBox.Text);
                result = false;
            }

            return result;
        }
    }
}
