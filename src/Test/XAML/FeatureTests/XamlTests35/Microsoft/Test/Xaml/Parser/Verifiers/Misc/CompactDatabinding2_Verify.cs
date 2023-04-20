// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Misc
{
    /// <summary/>
    public static class CompactDatabinding2_Verify
    {
        /// <summary>
        /// Verify the element tree for CompactDatabinding2.xaml.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }

            DockPanel.GetDock(myPanel);

            if (!myPanel.LastChildFill)
            {
                GlobalLog.LogEvidence("LastChildFill should be true.");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                GlobalLog.LogEvidence("Should have only 1 child");
                result = false;
            }

            UIElement child = myChildren[0];

            TextBlock text = child as TextBlock;

            if (null == text)
            {
                GlobalLog.LogEvidence("Should Have Text");
                result = false;
            }

            if (0 != String.Compare(text.Text.Trim(), "Test", true))
            {
                GlobalLog.LogEvidence("Text.Text is >>" + text.Text.Trim() + "<<, should be Test");
                result = false;
            }

            //if (text.FontSize != 200)
            //{
            //  GlobalLog.LogEvidence("Text.FontSize should be 200").
            //  result = false;
            //}
            if (text.FontStyle != FontStyles.Italic)
            {
                GlobalLog.LogEvidence("Text.FontStyle is" + text.FontStyle + ", should be 2: Italic");
                result = false;
            }
            return result;
        }
    }
}
