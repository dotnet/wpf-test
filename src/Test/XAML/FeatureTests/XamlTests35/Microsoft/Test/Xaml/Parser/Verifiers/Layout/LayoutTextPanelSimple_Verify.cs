// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutTextPanelSimple_Verify
    {
        /// <summary>
        ///  Verifier for LayoutTextPanelSimple.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.LayoutTextPanelSimpleVerify()...");

            FlowDocumentScrollViewer fdsv    = rootElement as FlowDocumentScrollViewer;
            FlowDocument             myPanel = fdsv.Document;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be TextPanel");
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

            GlobalLog.LogStatus("Root Textpanel OK ...");

            //Verify the first element
            GlobalLog.LogStatus("Verifying first element ...");

            Paragraph e1 = (Paragraph) LogicalTreeHelper.FindLogicalNode(rootElement, "E1");

            if (null == e1)
            {
                GlobalLog.LogEvidence("null == e1");
                result = false;
            }
            return result;
        }
    }
}
