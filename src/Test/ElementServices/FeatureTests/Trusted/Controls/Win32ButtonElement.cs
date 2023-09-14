// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
//using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Threading;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted.Controls
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>Win32ButtonElement.cs</filename>
    ///</remarks>
    public class Win32ButtonElement : HwndHost
    {

        ///<summary>
        ///</summary>
        public Win32ButtonElement()
        {
            this.MessageHook += new HwndSourceHook(_hook);
        }

        ///<summary>
        ///</summary>
        public IntPtr Win32Handle
        {
            get
            {
                return _controlHandle ;
            }
        }

        ///<summary>
        ///</summary>
        public IntPtr ContainerWindowHandle
        {
            get
            {
                return _mainWindow;
            }
        }


        ///<summary>
        ///</summary>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            mainWindow = new HwndWrapper(0,NativeConstants.WS_CHILD |NativeConstants.WS_VISIBLE,0,0,0,100,100,"ContainerWindow",(IntPtr)hwndParent,null);

            _mainWindow = mainWindow.Handle;
        
            IntPtr p = NativeMethods.CreateWindowEx(0, "Button", "Push Me!", NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,0,0,100,100, mainWindow.Handle, IntPtr.Zero, IntPtr.Zero, null);

            _controlHandle = p;
            return new HandleRef(null,_mainWindow);
        }

        ///<summary>
        ///</summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }


        HwndWrapper mainWindow;
		///<summary>
        ///</summary>
        IntPtr _hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {


            handled = false;

            if (hwnd == _mainWindow)
            {
                if (ContainerWindowHook != null)
                    ContainerWindowHook(hwnd,msg,wParam,lParam,ref handled);
            }
            else if (hwnd == _controlHandle)
            {
                if (ControlWindowHook != null)
                    ControlWindowHook(hwnd,msg,wParam,lParam,ref handled);
            }

            return IntPtr.Zero;
        }

        ///<summary>
        ///</summary>
        public void ValidateHandle()
        {
            if (ContainerWindowHandle != ((IWin32Window)this).Handle)
                throw new Microsoft.Test.TestValidationException("The handle is not correct");
                
        }

        ///<summary>
        ///</summary>
        public event HwndSourceHook ContainerWindowHook;


        ///<summary>
        ///</summary>
        public event HwndSourceHook ControlWindowHook;

        ///<summary>
        ///</summary>
        public event EventHandler Painted;

        ///<summary>
        ///</summary>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _arrangeBounds = arrangeBounds;
            return base.ArrangeOverride(arrangeBounds);
        }

        ///<summary>
        ///</summary>
         protected override Size MeasureOverride(Size constraint)
         {
            _measure = base.MeasureOverride(constraint);

            return _measure;
         }

        ///<summary>
        ///</summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            NativeMethods.MoveWindow(new HandleRef(null,_controlHandle), (int)0, (int)0, (int)_measure.Width, (int)_measure.Height, true);

            if (Painted != null)
               Painted(this,new EventArgs());

        }

        IntPtr _controlHandle = IntPtr.Zero;
        IntPtr _mainWindow = IntPtr.Zero;
        Size _arrangeBounds;
        Size _measure;

    }
  
}










