// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts Avalon sourcing operations.  The intention
 *          of abstracting the operations is to reduce the effect
 *          of future API changes in the product's core level.
 * 
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// This class generalizes the usage for common sourcing
    /// operations, such as creating an Avalon source in an hwnd.
    /// </summary>
    public class SourceHelper
    {
        /// <summary>
        /// </summary>
        public static HwndSource CreateHwndSource(int width, int height, int left, int top)
        {
            return CreateHwndSource(width, height, left, top, IntPtr.Zero, false, -1);
        }

        /// <summary>
        /// </summary>
        public static HwndSource CreateHwndSource(int width, int height, int left, int top, IntPtr parentHwnd)
        {
            return CreateHwndSource(width, height, left, top, parentHwnd, true, -1);
        }

        /// <summary>
        /// </summary>
        public static HwndSource CreateHwndSource(int width, int height, int left, int top, IntPtr parentHwnd, bool isChild)
        {
            return CreateHwndSource(width,height, left, top, parentHwnd, isChild, -1);            
        }

        /// <summary>
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public static HwndSource CreateHwndSource(int width, int height, int left, int top, IntPtr parentHwnd, bool isChild, int windowStyle)
        {
            HwndSourceParameters hwndParams = new HwndSourceParameters(
                "Avalon Test HwndSource",width,height);

            hwndParams.ParentWindow = parentHwnd;
            hwndParams.SetPosition(left,top);

            if (windowStyle == -1)
            {
                hwndParams.WindowStyle |= NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN;
            }
            else
            {
                hwndParams.WindowStyle = windowStyle;
            }

            if (isChild)
            {
                hwndParams.WindowStyle |= NativeConstants.WS_CHILD;
            }

            HwndSource source = new HwndSource (hwndParams);

            return source;
        }
    }
}
