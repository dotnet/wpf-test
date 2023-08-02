// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using Microsoft.Win32;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.SourceHook
{
    /// <summary>
    /// 
    /// </summary>
    public class HwndSubclassHelper
    {
        private int _gotMessage = 0;

        /// <summary>
        /// 
        /// </summary>
        public int GotMessage
        {
            get
            {
                return _gotMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _gotMessage = 0;
            if (_gotMessages != null)
            {
                _gotMessages.Clear();
            }
        }

        private ArrayList _gotMessages = null;

        /// <summary>
        /// 
        /// </summary>
        public ArrayList GotMessages
        {
            get
            {
                return _gotMessages;
            }
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        private IntPtr SimpleHook1(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                ++_gotMessage;
            }

            return IntPtr.Zero;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        private IntPtr AllMessagesHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            _gotMessages.Add(msg);

            return IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        public HwndSubclassHelper(IntPtr hwnd)
        {
            System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
            set.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows));
            set.Assert();
            _subclass = new HwndSubclass(new HwndWrapperHook(SimpleHook1));

            _subclass.Attach(hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        public HwndSubclassHelper()
        {
            _gotMessages = new ArrayList();


            _subclass = new HwndSubclass(new HwndWrapperHook(AllMessagesHook));
        }

        /// <summary>
        /// 
        /// </summary>
        public IntPtr Attach(IntPtr hwnd)
        {
            System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
            set.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows));
            set.Assert();

            return _subclass.Attach(hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Detach(bool force)
        {
            return _subclass.Detach(force);
        }

        private HwndSubclass _subclass = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreateWindowHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static LayeredHost CreateLayeredHost()
        {
            LayeredHost lh = new LayeredHost();

            return lh;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static object[] CreateLayerWindows()
        {
            HwndSourceParameters p = new HwndSourceParameters("layeredSource");
            HwndSource source = new HwndSource(p);

            StackPanel sp = new StackPanel();
            source.RootVisual = sp;

            //LayeredHost lh = CreateWindowHelper.CreateLayeredHost();
            LayeredHost lh = new LayeredHost();
            sp.Children.Add(lh);

            return new object[] { source, lh };
        }

    }

    /// <summary>
    /// HostedHwnd
    /// </summary>
    public class LayeredHost : HwndHost
    {
        private IntPtr _hwnd = IntPtr.Zero;

        /// <summary>
        /// Create an instance of LayeredHost
        /// </summary>
        public LayeredHost()
        {

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <returns></returns>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _hwnd = NativeMethods.CreateWindowEx(0, "static", "STATIC",
                                NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE, 0, 0, 100, 100,
                                hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, null);

            NativeMethods.ShowWindow(new HandleRef(this, _hwnd), NativeConstants.SW_SHOW);

            return new HandleRef(null, _hwnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }
    }


}
