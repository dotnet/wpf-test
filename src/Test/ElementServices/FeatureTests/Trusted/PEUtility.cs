// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Property Engine Trusted Utility
 *          This code is within trusted assembly so that it can assert 
 *          security permission
 * Contributors: 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/

using System;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;

namespace Avalon.Test.CoreUI.Trusted.Helper
{
    /// <summary>
    /// Property Engine Trusted Utility
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Assert necessary permission before displaying the window
        /// </summary>
        /// <param name="window">Window to show</param>
        /// <returns>Result</returns>
        [UIPermission(SecurityAction.Assert, Window = UIPermissionWindow.AllWindows)]
        [SecurityPermission(SecurityAction.Assert,  Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static bool? ShowWindowDialog(PEWindow window)
        {
            return window.ShowDialog();  //No need to call UIPermission.RevertAssert() as this stack frame is gone at return.
        }

        /// <summary>
        /// Help raise event
        /// </summary>
        [UIPermission(SecurityAction.Assert, Window = UIPermissionWindow.AllWindows)]
        public static void RaiseMouseEnterEventOnElement(FrameworkElement element)
        {
            MouseEventArgs args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
            args.RoutedEvent = Mouse.MouseEnterEvent;

            if (element != null)
            {
                element.RaiseEvent(args);
            }
        }
    }
}
