// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using System.Collections;


namespace Avalon.Test.CoreUI.SourceHook
{
    internal class SourceHookHelper
    {
        private Validation _validate = null;

        public SourceHookHelper()
        {
            _validate = Validation.GetInstance();
        }

        public IntPtr AllMessagesHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            IntPtr ret = IntPtr.Zero;

            _validate.SetTrackMessage("AllMessagesHook", msg);
  
            return IntPtr.Zero;
        }

        public IntPtr AllMessagesHook2(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            _validate.SetTrackMessage("AllMessagesHook2", msg);
 
            return IntPtr.Zero;
        }

        public IntPtr SimpleHook1(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                _validate.SetTrackMessage("SimpleHook1", msg);
            }

            return IntPtr.Zero;
        }

        public IntPtr SimpleHook2(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                _validate.SetTrackMessage("SimpleHook2", msg);
            }

            return IntPtr.Zero;
        }

        public IntPtr SimpleHook3(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                _validate.SetTrackMessage("SimpleHook3", msg);
                bHandled = true;
            }

            return IntPtr.Zero;
        }

        public IntPtr SimpleHook4(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                _validate.SetTrackMessage("SimpleHook4", msg);
            }

            return IntPtr.Zero;
        }

        public IntPtr SndMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                _validate.SetTrackMessage("SndMessageHook", msg);
                NativeMethods.SendMessage(new HandleRef(null, hwnd), NativeConstants.WM_KEYUP, wParam, lParam);
            }
            else if (msg == NativeConstants.WM_KEYUP)
            {
                _validate.SetTrackMessage("SndMessageHook", msg);
                bHandled = true;
            }

            return IntPtr.Zero;
        }

        public IntPtr DetachMsgHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                NativeMethods.SendMessage(new HandleRef(null, hwnd), 0x0000c257, wParam, lParam);
                _validate.SetTrackMessage("DetachMsgHook", msg);
            }
            else if (msg == 0x0000c257)
            {
                _validate.SetTrackMessage("DetachMsgHook", msg);
            }

            return IntPtr.Zero;
        }

        public IntPtr ExceptionHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            if (msg == NativeConstants.WM_KEYDOWN)
            {
                throw new Exception("ExceptionHook");
            }

            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Validation
    {
        struct Track
        {
            public int numOfCalls;
            public ArrayList msgList;
        }

        private Hashtable _hooks;

        private Stack _stack;

        /// <summary>
        /// 
        /// </summary>
        protected Validation()
        {
            _hooks = new Hashtable();
            _stack = new Stack();
        }


        private static Validation s_instance;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Validation GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new Validation();
            }
            return s_instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _hooks.Clear();
            _stack.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msgList"></param>
        public void SetTrack(string funName, ArrayList msgList)
        {
            _stack.Push(funName);
            if (_hooks.ContainsKey(funName))
            {
                Track tk = (Track)_hooks[funName];
                tk.numOfCalls++;
                tk.msgList = msgList;
                _hooks[funName] = tk;
            }
            else
            {
                Track tk = new Track();
                tk.numOfCalls = 1;
                tk.msgList = new ArrayList();
                tk.msgList = msgList;
                _hooks.Add(funName, tk);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msg"></param>
        public void SetTrackMessage(string funName, int msg)
        {
            _stack.Push(funName);
            if (_hooks.ContainsKey(funName))
            {
                Track tk = (Track)_hooks[funName];
                tk.numOfCalls++;
                tk.msgList.Add(msg);
                _hooks[funName] = tk;
            }
            else
            {
                Track tk = new Track();
                tk.numOfCalls = 1;
                tk.msgList = new ArrayList();
                tk.msgList.Add(msg);
                _hooks.Add(funName, tk);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public bool GetMessageList(string funName, ref ArrayList msgList)
        {
            if (_hooks.ContainsKey(funName))
            {
                msgList = ((Track)_hooks[funName]).msgList;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool FindMessage(string funName, int msg)
        {
            if (_hooks.ContainsKey(funName))
            {
                ArrayList al = ((Track)_hooks[funName]).msgList;
                if (al.Contains(msg))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="funName"></param>
        /// <returns></returns>
        public int GetCallTimes(string funName)
        {
            if (_hooks.ContainsKey(funName))
            {
                return ((Track)_hooks[funName]).numOfCalls;
            }
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="funNames"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool GetOrders(string[] funNames, int count)
        {
            if (_stack.Count == 0 || _stack.Count < count)
                return false;

            // get the lastest n calls.
            for (int i = count - 1; i >= 0; i--)
            {
                funNames[i] = (string)_stack.Pop();
            }

            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubSource : HwndSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public SubSource(HwndSourceParameters p)
            : base(p)
        { }

        /// <summary>
        /// 
        /// </summary>
        ~SubSource()
        {
            CoreLogger.LogStatus("SubSource finalizer");
        }
    }


}
