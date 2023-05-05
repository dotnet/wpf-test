// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutTextPanel_Verify
    {
        /// <summary>
        ///  Verifier for LayoutTextPanel.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            FlowDocumentScrollViewer fdsv    = rootElement as FlowDocumentScrollViewer;
            FlowDocument             myPanel = fdsv.Document;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be FlowDocumentScrollViewer");
                result = false;
            }

            GlobalLog.LogStatus("Verifying root element ...");
            if ((int)(myPanel.Typography.Capitals) != (int)(FontCapitals.Unicase))
            {
                GlobalLog.LogEvidence("(int)(myPanel.Typography.Capitals) != (int)(FontCapitals.Unicase)");
                result = false;
            }
            if (fdsv.FontFamily.Source != "Palatino Linotype")
            {
                GlobalLog.LogEvidence("fdsv.FontFamily.Source != Palatino Linotype");
                result = false;
            }
            if (fdsv.FontSize != 17)
            {
                GlobalLog.LogEvidence("fdsv.FontSize != 17");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(fdsv.Foreground)).Color, Colors.Gray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(fdsv.Foreground)).Color, Colors.Gray)");
                result = false;
            }
            if (myPanel.LineHeight != 25)
            {
                GlobalLog.LogEvidence("myPanel.LineHeight != 25");
                result = false;
            }
            if (!myPanel.Typography.StandardLigatures)
            {
                GlobalLog.LogEvidence("!myPanel.Typography.StandardLigatures");
                result = false;
            }
            //if((int)myPanel.TextTrimming, (int)TextTrimming.CharacterEllipsis);

            GlobalLog.LogStatus("Root Textpanel OK ...");


            //Verify the first element
            GlobalLog.LogStatus("Verifying first element ...");

            Paragraph e1 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E1");

            if (null == e1)
            {
                GlobalLog.LogEvidence("null == e1");
                result = false;
            }
            //ife1.TextDecorations.Equals(System.Windows.TextDecorations.Underline), true).
            //if((int)e1.TextWrapping, (int)TextWrapping.NoWrap);
            if ((new TextRange(e1.ContentStart, e1.ContentEnd)).Text != "WONDERWALL")
            {
                GlobalLog.LogEvidence("(new TextRange(e1.ContentStart, e1.ContentEnd)).Text != WONDERWALL");
                result = false;
            }
            //Verify the second element
            GlobalLog.LogStatus("Verifying the second element ...");

            Paragraph e2 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E2");

            if (null == e2)
            {
                GlobalLog.LogEvidence("null == e2");
                result = false;
            }

            //VerifyElement.VerifyDouble(e2.TextIndent, 25).
            if ((new TextRange(e2.ContentStart, e2.ContentEnd)).Text != "-by oasis")
            {
                GlobalLog.LogEvidence("(new TextRange(e2.ContentStart, e2.ContentEnd)).Text != -by oasis");
                result = false;
            }

            Paragraph e3 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E3");
            Paragraph e4 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E4");
            Section   e5 = (Section) LogicalTreeHelper.FindLogicalNode(rootElement, "E5");
            Inline    e6 = (Inline) LogicalTreeHelper.FindLogicalNode(rootElement, "E6");
            Paragraph e7 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E7");

            if (null == e3 || null == e4 || null == e5 || null == e6 || null == e7)
            {
                GlobalLog.LogEvidence("Some text component missed");
                result = false;
            }
            if ((int) e6.Typography.Variants != (int) FontVariants.Superscript)
            {
                GlobalLog.LogEvidence("(int)e6.Typography.Variants != (int)FontVariants.Superscript");
                result = false;
            }
            return result;
        }
    }
}
