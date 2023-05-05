// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class LiteralAndNullExtension_Verify
    {
        /// <summary>
        /// LiteralAndNullExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool   result    = true;
            String errorMesg = "LiteralAndNullExtension verification failed";
            Button button0   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            Button button2   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button2");
            Button button3   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button3");
            Button button4   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button4");
            Button button5   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button5");

            // Verify the Content property for Buttons 0 thru 4
            String content = (String) button0.GetValue(ContentControl.ContentProperty);
            if (" {Foo}" != content)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button0.");
            }

            content = (String) button1.GetValue(ContentControl.ContentProperty);
            if ("}" != content)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button1.");
            }

            content = (String) button2.GetValue(ContentControl.ContentProperty);
            if ("{Foo}" != content)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button2.");
            }

            content = (String) button3.GetValue(ContentControl.ContentProperty);
            if ("{" != content)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button3.");
            }

            content = (String) button4.GetValue(ContentControl.ContentProperty);
            if ("{{}}" != content)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button4.");
            }

            // Verify that Button5's background is set to Null
            SolidColorBrush background = (SolidColorBrush) button5.GetValue(Control.BackgroundProperty);
            if (null != background)
            {
                result = false;
                GlobalLog.LogEvidence(errorMesg + " for Button5.");
            }
            return result;
        }
    }
}
