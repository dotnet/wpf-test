// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify parsing and serializing multiple cursors.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [TestCaseTitle(@"Verify parsing and serializing multiple cursors.")]
    [TestCasePriority("0")]
    public class MultipleCursorVerifiers
    {
        /// <summary>
        /// VerifyCursorValue
        /// </summary>
        /// <param name="uie">Root of tree in XAML.</param>
        public static void VerifyCursorValue(UIElement uie)
        {
            // IMPORTANT! Convention used here is that the end of the Name string must be:
            // 1. A Cursor enum value (for stock cursors)

            string[] stockCursorIDs = new string[] {
                // stock cursors
                "cursorAppStarting", 
                "cursorArrow", 
                "cursorArrowCD", 
                "cursorCross", 
                "cursorHand", 
                "cursorHelp", 
                "cursorIBeam", 
                "cursorNo", 
                "cursorNone",
                "cursorPen", 
                "cursorScrollAll",
                "cursorScrollE",
                "cursorScrollNE",
                "cursorScrollNS",
                "cursorScrollNW",
                "cursorScrollS",
                "cursorScrollAll",
                "cursorScrollSE", 
                "cursorScrollSW", 
                "cursorScrollW", 
                "cursorScrollWE", 
                "cursorSizeAll", 
                "cursorSizeNESW", 
                "cursorSizeNS",
                "cursorSizeNWSE", 
                "cursorSizeWE", 
                "cursorUpArrow", 
                "cursorWait",
            };

            TypeConverter ctc = TypeDescriptor.GetConverter(typeof(Cursor));

            CoreLogger.LogStatus("Checking stock cursors....");
            foreach (string elId in stockCursorIDs)
            {
                FrameworkElement el = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(uie, elId);

                Assert(el != null, "Expected element not found!");

                CoreLogger.LogStatus("el='" + el.ToString() + "', Name='" + elId + "'");

                // Construct cursor from our element
                Cursor c = el.Cursor;
                string sFromCursor = ctc.ConvertTo(c, typeof(string)) as string;

                Assert(((sFromCursor != "") || (sFromCursor != null)), "no real cursor!");

                // Compare cursor to what is specified in markup.
                Assert(elId.EndsWith(sFromCursor), "Cursor value doesn't match string value!");
            }

        }

        /// <summary>
        /// VerifyCustomCursorValue
        /// </summary>
        /// <param name="uie">Root of tree in XAML.</param>
        public static void VerifyCustomCursorValue(UIElement uie)
        {
            // IMPORTANT! Convention used here is that the end of the Name string must be:
            // 1. The filename of the Cursor property (for custom cursors)

            string[] customCursorIDs = new string[] {
                // custom cursors
                "cursorStarCur",
                "cursorAnitestAni",
                "cursorTestCursorColorDepth",
                "cursorTestCursorColorDepth4bpp",
                "cursorTestCursorColorDepth8bpp",
                "cursorTestCursorColorDepthNoMono",
                "cursorTestCursorColorDepthXP",
                "cursorTestCursorRes",
                "cursorTestCursorRes16x16",
                "cursorTestCursorRes32x32",
                "cursorTestCursorRes48x48",
                "cursorTestCursorRes64x64",
                "cursorTestCursorRes96x96",
                "cursorTestCursorResNo32x32",
            };


            TypeConverter ctc = TypeDescriptor.GetConverter(typeof(Cursor));

            CoreLogger.LogStatus("Checking custom cursors....");
            foreach (string elId in customCursorIDs)
            {
                FrameworkElement el = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(uie, elId);
                Assert(el != null, "Expected element not found!");

                CoreLogger.LogStatus("el='" + el.ToString() + "', Name='" + elId + "'");

                // Construct cursor from our element
                Cursor c = el.Cursor;
                string sFromCursor = ctc.ConvertTo(c, typeof(string)) as string;
                Assert(((sFromCursor != "") || (sFromCursor != null)), "no real cursor!");
            }
        }


        private static void Assert(bool condition, string exceptionMsg)
        {
            // Log intermediate result
            if (!condition)
            {
                // Intermediate result = FAIL
                string exceptionString = exceptionMsg;

                throw new Microsoft.Test.TestValidationException(exceptionMsg);
            }
        }

    }
}

