// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Interop;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using System.Collections.Generic;

namespace Avalon.Test.CoreUI.Win32
{
    /// <summary>
    /// </summary>
    public class Win32GenericMessagePump
    {
        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        public static int Count = 0;

        /// <summary>
        /// 
        /// </summary>
        public static Win32GenericMessagePumpObj Current
        {
            get
            {
                return new Win32GenericMessagePumpObj();
            }
        }

        /// <summary>
        /// </summary>
        public static void Run()
        {
            MSG msg = new MSG();
            try
            {
                Count++;
                while (NativeMethods.GetMessage(ref msg, IntPtr.Zero, 0, 0) != 0)
                {
                    NativeMethods.TranslateMessage(ref msg);
                    NativeMethods.DispatchMessage(ref msg);
                    msg = new MSG();
                }
            }
            finally
            {
                Count--;
            }
        }

        /// <summary>
        /// </summary>
        public static void Stop()
        {
            NativeMethods.PostQuitMessage(0);
        }            
    }

    /// <summary>
    /// 
    /// </summary>
    public class Win32GenericMessagePumpObj
    {
        /// <summary>
        /// 
        /// </summary>
        internal Win32GenericMessagePumpObj()
        {
            _threadId = NativeMethods.GetCurrentThreadId();
        }

        /// <summary>
        /// </summary>
        public void Stop()
        {
            if (_threadId != 0)
            {
                NativeMethods.PostThreadMessage(_threadId, NativeConstants.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
            }
        }            
        
        /// <summary>
        /// 
        /// </summary>
        int _threadId = 0;
    }
}

