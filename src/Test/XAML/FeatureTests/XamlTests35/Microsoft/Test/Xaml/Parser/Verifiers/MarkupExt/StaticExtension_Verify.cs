// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class StaticExtension_Verify
    {
        /// <summary>
        /// StaticExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool   result  = true;
            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            Button button2 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button2");
            Button button3 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button3");
            Button button4 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button4");
            Button button5 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button5");


            // Verify Button0's cursor
            Cursor cursor = (Cursor) button0.GetValue(FrameworkElement.CursorProperty);
            if (cursor != Cursors.Pen)
            {
                result = false;
                GlobalLog.LogEvidence("cursor != Cursors.Pen");
            }


            // Verify Button1's cursor
            cursor = (Cursor) button1.GetValue(FrameworkElement.CursorProperty);
            if (cursor != Cursors.Pen)
            {
                result = false;
                GlobalLog.LogEvidence("cursor != Cursors.Pen(1)");
            }

            // Verify Button2 and Button3's content (using content tags and without)
            String content = (String) button2.GetValue(ContentControl.ContentProperty);
            if ("Hello world" != content)
            {
                result = false;
                GlobalLog.LogEvidence("Hello world != content");
            }
            content = (String) button4.GetValue(ContentControl.ContentProperty);
            if ("Hello world" != content)
            {
                result = false;
                GlobalLog.LogEvidence("Hello world != content(1)");
            }

            content = (String) button3.GetValue(ContentControl.ContentProperty);
            if ("My motto" != content)
            {
                result = false;
                GlobalLog.LogEvidence("My motto != content");
            }
            content = (String) button5.GetValue(ContentControl.ContentProperty);
            if ("My motto" != content)
            {
                result = false;
                GlobalLog.LogEvidence("My motto != content(1)");
            }


            // Verify ListBox0's SelectionMode
            ListBox       listbox0 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox0");
            SelectionMode mode     = (SelectionMode) listbox0.GetValue(ListBox.SelectionModeProperty);
            if (SelectionMode.Multiple != mode)
            {
                result = false;
                GlobalLog.LogEvidence("SelectionMode.Multiple != mode");
            }
            ListBox listbox1 = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox1");
            mode = (SelectionMode) listbox1.GetValue(ListBox.SelectionModeProperty);
            if (SelectionMode.Multiple != mode)
            {
                result = false;
                GlobalLog.LogEvidence("SelectionMode.Multiple != mode(1)");
            }
            return result;
        }
    }
}
