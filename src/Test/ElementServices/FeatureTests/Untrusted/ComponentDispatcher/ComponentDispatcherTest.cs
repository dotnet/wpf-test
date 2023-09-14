// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;
using Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

using SWF = System.Windows.Forms;

namespace Avalon.Test.ComponentDispatcherTest
{
    /// <summary>
    /// ComponentDispatcher Model class
    /// </summary>
    /// 
    [Model(@"FeatureTests\ElementServices\ComponentDispatcher.xtc", 1, @"Interop\ComponentDispatcher", TestCaseSecurityLevel.FullTrust, "ComponentDispatcherModel", ExpandModelCases=true, Area="AppModel")]
    public class ComponentDispatcherModel : CoreModel
    {
        /// <summary>
        /// Create an instance of ComponentDispatcherModel.
        /// </summary>
        public ComponentDispatcherModel()
            : base()
        {
            Name = "ComponentDispatcherModel";
            Description = "ComponentDispatcher Model";


            // Add Action Handlers
            AddAction("Run", new ActionHandler(Run));
        }


        private bool Run(State endState, State inParams, State outParams)
        {
            ComDispatcherTest test = new ComDispatcherTest(inParams);

            return test.Run();
        }
    }


    internal class ComDispatcherTest
    {

        public ComDispatcherTest(IDictionary inParams)
        {
            _param = new ComDispatcherSetup(inParams);
        }

        public bool Run()
        {
            switch (_param.TopWindow)
            {
                case "Avalon":
                    AvalonDispatcher();
                    break;
                default:
                    throw new Exception("not supported yet");
            }

            
            return true;
        }

        // Avalon owns message pump
        private void AvalonDispatcher()
        {
            AvalonTopWindow win = new AvalonTopWindow();

            win.Show();

            Dispatcher MainDispatcher = win.Dispatcher;

            MainDispatcher.BeginInvoke(DispatcherPriority.Send,
                new DispatcherOperationCallback(AvalonTestThread), win);

            DispatcherHelper.DoEvents(5000);

        }

        object AvalonTestThread(object o)
        {
            bool bPass = true;

            AvalonTopWindow win = o as AvalonTopWindow;

            if (win == null)
            {
                throw new Exception("Top Level window is null");
            }
         
            // all public API
            bPass = TestPushAndPop(win);

            CoreLogger.LogStatus("Start MultiplePush Test");

            // delay for debugging before starting another test
            Thread.Sleep(300);

            // PushModal multiple times
            bPass &= TestMultiplePush(win);

            CoreLogger.LogStatus("Start MultiplePop Test");

            Thread.Sleep(300);

            // PopModal multiple times without Push
            bPass &= TestMultiplePop(win);

            if (!bPass)
                CoreLogger.LogTestResult(false, _param.TopWindow + " " + _param.Popup);

            return null;
        }

        bool TestPushAndPop(AvalonTopWindow win)
        {
            bool bPass = true;

            win.OpenChild(_param.Popup);

            ValidateTrace _trace = ValidateTrace.GetInstance();

            if (_trace[CompDispAPI.EnterThreadModal] != _trace[CompDispAPI.LeaveThreadModal])
            {
                bPass = false;
                CoreLogger.LogStatus("PushModal != PopModal", ConsoleColor.Red);
            }
            if (_trace[CompDispAPI.ThreadIdle] <= 0)
            {
                bPass = false;
                CoreLogger.LogStatus("ThreadIdle should be invoked");
            }
            if (_trace[CompDispAPI.ThreadFilterMsg] <= 0)
            {
                bPass = false;
                CoreLogger.LogStatus("ThreadFilterMsg should be invoked");
            }
            if (_trace[CompDispAPI.ThreadPreProcessMsg] <= 0)
            {
                bPass = false;
                CoreLogger.LogStatus("ThreadPreProcessMsg should be invoked");
            }

            _trace.Reset();

            return bPass;

        }

        bool TestMultiplePush(AvalonTopWindow win)
        {
            bool bPass = true;

            win.OpenChildMultiPush(_param.Popup);

            ValidateTrace _trace = ValidateTrace.GetInstance();

            if (_trace[CompDispAPI.EnterThreadModal] <= 0)
            {
                bPass = false;
                CoreLogger.LogStatus("PushModal failed", ConsoleColor.Red);
            }
            if (_trace[CompDispAPI.LeaveThreadModal] > 0)
            {
                bPass = false;
                CoreLogger.LogStatus("PopModal unexpected happened", ConsoleColor.Red);
            }


            return bPass;

        }

        bool TestMultiplePop(AvalonTopWindow win)
        {

            bool bPass = true;

            try
            {
                win.OpenChildMultiPop(_param.Popup);

                ValidateTrace _trace = ValidateTrace.GetInstance();

                if (_trace[CompDispAPI.LeaveThreadModal] < 0)
                {
                    bPass = false;
                    CoreLogger.LogStatus("PopModal failed", ConsoleColor.Red);
                }
            }
            catch (Exception e)
            {
                bPass = false;
                CoreLogger.LogTestResult(false, "unexpected exception: " + e.Message);
            }
            return bPass;
        }

        ComDispatcherSetup _param;
    }

    internal class ComDispatcherSetup : CoreModelState
    {
        public ComDispatcherSetup(IDictionary dict)
        {
            this.Dictionary = new Hashtable(dict);
        }

        public string TopWindow
        {
            get
            {
                return (string)Dictionary["TopLevel"];
            }
        }

        public string Popup
        {
            get
            {
                return (string)Dictionary["Popup"];
            }
        }

    }

  
}
