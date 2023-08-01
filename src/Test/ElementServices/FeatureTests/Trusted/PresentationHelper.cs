// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts core-level display/presentation operations.  
 *          Those include displaying the tree or reading or modifying 
 *          the presentation surface, i.e. hwnd or whatever. The 
 *          intention of abstracting the operations is to reduce the 
 *          effect of future API changes in the product's core level.
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;

using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstraction of core-level display/presentation operations
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public partial class PresentationHelper
    {
        /// <summary>
        /// Returns an hwnd reference from an element.
        /// </summary>
        public static HandleRef GetHwnd(Visual element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            PresentationSource source = PresentationSource.FromVisual((Visual)element);
            HandleRef hwnd;

            if (source == null)
            {
                return NativeMethods.NullHandleRef;
            }

            if (source is HwndSource)
            {
                HwndSource hwndSource = (HwndSource)source;

                hwnd = new HandleRef(null, hwndSource.Handle);
            }
            else
            {
                throw new InvalidOperationException("Cannot cast PresentationSource to HwndSource.");
            }

            return hwnd;
        }
        /// <summary>
        /// Returns an hwnd handle from an HwndSource.
        /// </summary>
        public static HandleRef GetHwnd(PresentationSource source)
        {
            if (source is HwndSource)
            {
                return new HandleRef(null, ((HwndSource)source).Handle);
            }
            else
            {
                return new HandleRef(null, IntPtr.Zero);
            }
        }
        /// <summary>
        /// Returns a PresentationSource from an element.
        /// </summary>
        public static PresentationSource FromElement(DependencyObject dobj)
        {
            if (dobj == null)
            {
                throw new ArgumentNullException("dobj");
            }

            PresentationSource source = null;
            DependencyObject parentNode = dobj;
            Visual visual = dobj as Visual;

            // Walk up ancestor chain until we find a Visual ancestor.
            while (parentNode != null && visual == null)
            {
                parentNode = LogicalTreeHelper.GetParent(parentNode);
                visual = parentNode as Visual;
            }

            if (visual != null)
            {
                source = PresentationSource.FromVisual(visual);
            }

            return source;
        }
        /// <summary>
        /// Returns the active PresentationSource from the primary MouseDevice.
        /// </summary>
        public static PresentationSource FromMouse()
        {
            return Mouse.PrimaryDevice.ActiveSource;
        }
        /// <summary>
        /// Returns the PresentationSource from InputEventArgs.
        /// </summary>
        public static PresentationSource FromKeyEventArgs(KeyEventArgs eventArgs)
        {
            return eventArgs.InputSource;
        }
    }
}

