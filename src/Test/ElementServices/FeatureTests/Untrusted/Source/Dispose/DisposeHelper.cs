// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using System.Windows.Media;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;


namespace Avalon.Test.CoreUI.DisposePattern
{
    /// <summary>
    /// library functions
    /// </summary>
    public class Helper
    {
        Validation _validation;

        /// <summary>
        /// construct an instance
        /// </summary>
        public Helper()
        {
            _validation = Validation.GetInstance();
            _source = CreateHwndSource();
        }

        /// <summary>
        /// test Dispose event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDispose(Object sender, EventArgs e)
        {
            _validation.SetTrack("DisposeEvent", 0);
        }

        /// <summary>
        /// Diposing from Dispose event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DiposeFromDisposeEvent(Object sender, EventArgs e)
        {
            _validation.SetTrack("DiposeFromDiposeEvent", 0);
            _source.Dispose();
        }

        /// <summary>
        /// Dispose before shutdown complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisposeShutdown(Object sender, EventArgs e)
        {
            _validation.SetTrack("Dispose_ShutdownStart", Dispatcher.CurrentDispatcher.HasShutdownStarted);
            _validation.SetTrack("Dispose_ShutdownFinished", Dispatcher.CurrentDispatcher.HasShutdownFinished);
            _validation.SetOrder("DisposeEvent");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnShutDownFinish(Object sender, EventArgs e)
        {
            _validation.SetOrder("OnDispatcherShutdownFinish");
        }

        /// <summary>
        /// SourceHook
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public IntPtr MsgListerner(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            _validation.SetTrack("MsgListerner", msg);
            return IntPtr.Zero;
        }

        /// <summary>
        /// an instance of HwndSource
        /// </summary>
        public HwndSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }



        private HwndSource CreateHwndSource()
        {
            HwndSourceParameters parameters = new HwndSourceParameters("DisposeTestHelper");

            HwndSource source = new HwndSource(parameters);

            return source;
        }

        private HwndSource _source = null;

    }

    /// <summary>
    /// collection trace information
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// 
        /// </summary>
        struct Track
        {
            public List<int> msgList;
            public int order;
            public bool hasFinished;
        }

        private Hashtable _report;

        private Validation()
        {
            _report = new Hashtable();
        }

        private static Validation s_instance = null;

        private int _order = 0;

        /// <summary>
        /// GetInstance
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
        /// Reset test states
        /// </summary>
        public void Reset()
        {
            _report.Clear();
        }

        /// <summary>
        /// trace messages
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msg"></param>
        public void SetTrack(string funName, int msg)
        {
            if (_report.ContainsKey(funName))
            {
                Track tk = (Track)_report[funName];
                tk.msgList.Add(msg);
                _report[funName] = tk;
            }
            else
            {
                Track tk = new Track();
                tk.msgList = new List<int>();
                tk.msgList.Add(msg);
                _report.Add(funName, tk);
            }
        }

        /// <summary>
        /// Get Messages List
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="msgList"></param>
        /// <returns></returns>
        public bool GetMessageList(string funName, ref List<int> msgList)
        {
            if (_report.ContainsKey(funName))
            {
                msgList = ((Track)_report[funName]).msgList;
                msgList.Sort();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Set calling order
        /// </summary>
        /// <param name="funName"></param>
        public void SetOrder(string funName)
        {
            _order++;
            if (_report.ContainsKey(funName))
            {
                Track tk = (Track)_report[funName];
                tk.order = _order;
                _report[funName] = tk;
            }
            else
            {
                Track tk = new Track();
                tk.order = _order;
                _report.Add(funName, tk);
            }
        }

        /// <summary>
        /// set calling order 
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="hasFinished"></param>
        public void SetTrack(string funName, bool hasFinished)
        {
            if (_report.ContainsKey(funName))
            {
                Track tk = (Track)_report[funName];
                tk.hasFinished = hasFinished;
                _report[funName] = tk;
            }
            else
            {
                Track tk = new Track();
                tk.hasFinished = hasFinished;
                _report.Add(funName, tk);
            }
        }

        /// <summary>
        /// find a event
        /// </summary>
        /// <param name="funName"></param>
        /// <returns></returns>
        public bool FindEvent(string funName)
        {
            if (_report.ContainsKey(funName))
                return true;
            else
                return false;
        }

        /// <summary>
        /// get the calling order of an event
        /// </summary>
        /// <param name="funName"></param>
        /// <returns></returns>
        public int GetEventOrder(string funName)
        {
            if (_report.ContainsKey(funName))
            {
                return ((Track)_report[funName]).order;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// get bool info of the event
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="hasFinished"></param>
        /// <returns></returns>
        public bool GetEventBool(string funName, ref bool hasFinished)
        {
            if (_report.ContainsKey(funName))
            {
                hasFinished = ((Track)_report[funName]).hasFinished;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
