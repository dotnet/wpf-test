// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.NameScope
{
    /// <summary>
    /// Verification class for Xaml File
    /// </summary>
    public static class NetFx2007NS_Verify
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
            Grid grid1 = ((StackPanel)rootElement).Children[0] as Grid;
            Grid grid2 = ((StackPanel)rootElement).Children[1] as Grid;
            Grid grid3 = ((StackPanel)rootElement).Children[2] as Grid;
            Grid grid4 = ((StackPanel)rootElement).Children[3] as Grid;

            if ((((grid1.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox1")
            {
                GlobalLog.LogEvidence("Did not find expected text: RichTextBox1");
                result = false;
            }

            if ((((grid2.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox2")
            {
                GlobalLog.LogEvidence("Did not find expected text: RichTextBox2");
                result = false;
            }

            if ((((grid3.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox3")
            {
                GlobalLog.LogEvidence("Did not find expected text: RichTextBox3");
                result = false;
            }

            if ((((grid4.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox4")
            {
                GlobalLog.LogEvidence("Did not find expected text: RichTextBox4");
                result = false;
            }

            return result;
        }
    }
}
