// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted.Controls
{
    ///<summary>
    ///</summary>
    public interface IDisposedTest
    {
        ///<summary>
        ///</summary>
        bool IsControlDestroyed
        {
            get;
        }
    }

        /// <summary>
    /// </summary>
    public interface IMouseEvents
    {
        /// <summary>
        /// </summary>
        event EventHandler MouseMove;
    }

    
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>Win32ButtonElement.cs</filename>
    ///</remarks>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name="FullTrust")]    
    public class Win32ButtonCtrl : HwndHost, IMouseEvents, IDisposedTest
    {
        ///<summary>
        ///</summary>
        public const double ConstWidth = 100;

        ///<summary>
        ///</summary>        
        public const double ConstHeight = 100;

        ///<summary>
        ///</summary>
        public Win32ButtonCtrl()
        {
            this.MessageHook += new HwndSourceHook(_hook);
        }

        /// <summary>
        /// </summary>
        public new event EventHandler MouseMove;

        ///<summary>
        ///</summary>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {

            IntPtr p = NativeMethods.CreateWindowEx(0, "Button", "Push Me!", NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,0,0,(int)ConstWidth,(int)ConstHeight, (IntPtr)hwndParent, IntPtr.Zero, IntPtr.Zero, null);

            return new HandleRef(null,p);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (IsControlDestroyed)
            {
                throw new Microsoft.Test.TestValidationException("DestroyWindowCore should not be called twice.");
            }
            
            _isControlDestroyed = true;

            NativeMethods.DestroyWindow(new HandleRef (null, Handle));
        }

        ///<summary>
        ///</summary>

        public bool IsControlDestroyed
        {
            get
            {
                return _isControlDestroyed;
            }   
        }


		///<summary>
        ///</summary>
        IntPtr _hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (NativeConstants.WM_MOUSEMOVE == msg)
            {
                if (MouseMove != null)
                {
                    MouseMove(this, EventArgs.Empty);
                }
            }
            handled = false;


            return IntPtr.Zero;
        }


        bool _isControlDestroyed = false;

    }

}


