//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;

namespace WpfControlToolkit
{

    internal static class DatePickerHelper
    {
        /// <summary>
        /// True, if node is derived from reference
        /// </summary>
        internal static bool IsDescendant(Visual reference, Visual node)
        {
            bool success = false;

            Visual curr = node;

            while (curr != null)
            {
                if (curr == reference)
                {
                    success = true;
                    break;
                }

                // Try to jump up logical links if possible.
                FrameworkElement logicalCurrent = curr as FrameworkElement;
                Visual logicalCurrentVisualParent = null;
                // Check for logical parent and make sure it's a Visual
                if (logicalCurrent != null)
                {
                    logicalCurrentVisualParent = logicalCurrent.Parent as Visual;
                }

                if (logicalCurrentVisualParent != null)
                {
                    curr = logicalCurrentVisualParent;
                }
                else
                {
                    // Logical link isn't there; use child link
                    curr = VisualTreeHelper.GetParent(curr) as Visual;
                }
            }

            return success;
        }
    }

}
