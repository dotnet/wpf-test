// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Test.Win32;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Class containing common methods for automating interoperability.
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name="FullTrust")]
    public class Interop
    {
        /// <summary>
        /// Retrieve Win32 handle reference associated with a Window.
        /// </summary>
        /// <param name="w">Window</param>
        /// <returns>Reference to the window handle, or a null handle reference if window is invalid.</returns>
        public static HandleRef HandleRefFromWindow(Window w)
        {
            HandleRef href;

            if (w != null)
            {
                WindowInteropHelper helper = new WindowInteropHelper(w);
                IntPtr h = helper.Handle;

                href = new HandleRef(null, h);
            }
            else
            {
                href = NativeMethods.NullHandleRef;
            }

            return href;
        }

        /// <summary>
        /// Retrieve Win32 handle reference associated with a Window.
        /// </summary>
        /// <returns>Reference to the window handle, or a null handle reference if window is invalid.</returns>
        public static PresentationSource PresentationSourceFromVisual(Visual v)
        {
            PresentationSource source = PresentationSource.FromVisual(v);
            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetFocus(HandleRef handle)
        {
            NativeMethods.SetFocus(handle);
        }

        /// <summary>
        /// 
        /// </summary>
        public static HandleRef GetFocus()
        {
            return new HandleRef(null, NativeMethods.GetFocus());
        }
    }
}
