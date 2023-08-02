// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Modeling;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;


using SWF = System.Windows.Forms;

namespace Avalon.Test.ComponentDispatcherTest
{
    internal class AvalonTopWindow : Window
    {
        public AvalonTopWindow()
        {
            this.Background = Brushes.Aqua;
            this.Title = "Avalon Main Window";

            StackPanel sp = new StackPanel();
            this.Content = sp;

            _button = new Button();
            _button.Content = "Open child window";

            sp.Children.Add(_button);

            _pApp = new PopupApp();
        }

        private Button _button;

        public Button ButtonControl
        {
            get
            {
                return _button;
            }
        }

        public void OpenChild(string wndType)
        {
            _pApp.OpenDialog(wndType, this);

            CoreLogger.LogStatus("Open Child", ConsoleColor.Green);     
        }

        public void OpenChildMultiPush(string wndType)
        {
            _pApp.OpenDialogMultiPush(wndType, this);
            
            CoreLogger.LogStatus("Open Child", ConsoleColor.Green);     
            
        }

        public void OpenChildMultiPop(string wndType)
        {
            _pApp.OpenDialogMultiPop(wndType, this);

            CoreLogger.LogStatus("Open Child", ConsoleColor.Green); 
        }

        PopupApp _pApp;
                
    }

    // tested API
    enum CompDispAPI { EnterThreadModal, LeaveThreadModal, ThreadIdle, ThreadFilterMsg, ThreadPreProcessMsg };

    // register events and popup modal window
    internal class PopupApp
    {
        ChildPopWindow _child;

        ValidateTrace _trace;

        public PopupApp()
        {
            // register events
            System.Windows.Interop.ComponentDispatcher.ThreadIdle += OnIdle;
            System.Windows.Interop.ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(OnThreadFilterMessage);
            System.Windows.Interop.ComponentDispatcher.ThreadPreprocessMessage += new ThreadMessageEventHandler(OnThreadPreProcessMessage);
            System.Windows.Interop.ComponentDispatcher.EnterThreadModal += new EventHandler(OnGoDlgModal);
            System.Windows.Interop.ComponentDispatcher.LeaveThreadModal += new EventHandler(OnLeaveModal);

            _trace = ValidateTrace.GetInstance();    
        }

        public void OpenDialog(string wndType, Window parent)
        {
            System.Windows.Interop.ComponentDispatcher.PushModal();

            CreateWindowAndTimer(wndType, parent);

            if (_child == null)
                throw new Exception("not supported");

            _child.ShowModalWindow();

            System.Windows.Interop.ComponentDispatcher.PopModal();
        }

        public void OpenDialogMultiPush(string wndType, Window parent)
        {
            for(int i=0; i < 5; i++)
                System.Windows.Interop.ComponentDispatcher.PushModal();

            CreateWindowAndTimer(wndType, parent);

            if (_child == null)
                throw new Exception("not supported");

            _child.ShowModalWindow();

            System.Windows.Interop.ComponentDispatcher.PopModal();

        }

        public void OpenDialogMultiPop(string wndType, Window parent)
        {
            CreateWindowAndTimer(wndType, parent);

            if (_child == null)
                throw new Exception("not supported");

            _child.ShowModalWindow();

            for(int i=0; i < 10; i++)
                System.Windows.Interop.ComponentDispatcher.PopModal();
        }

        void CreateWindowAndTimer(string wndType, Window parent)
        {
            DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Interval = TimeSpan.FromMilliseconds(1200);

            _timer.Start();

            switch (wndType)
            {
                case "Avalon":
                    _child = new ChildWindow(parent);
                    break;
                case "WinForm":
                    _child = new ChildWinForm();
                    break;
                default:
                    _child = null;
                    break;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_child.IsWindowVisible)
            {
                _child.CloseModalWindow();
            }
        }

        private void OnGoDlgModal(object sender, EventArgs e)
        {
            //Console.WriteLine("On Dlg Go Modal");
            _trace.SetTrace(CompDispAPI.EnterThreadModal);
        }

        private void OnIdle(object sender, EventArgs e)
        {
            //Console.WriteLine("Application.Idle");
            _trace.SetTrace(CompDispAPI.ThreadIdle);
        }

        private void OnThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            //Console.WriteLine("OnThreadFilterMessage");
            if ((msg.message == NativeConstants.WM_KEYDOWN) && (msg.wParam.ToInt32() == NativeConstants.VK_TAB))
            {
                CoreLogger.LogStatus("Tab key", ConsoleColor.Gray);
                _trace.SetTrace(CompDispAPI.ThreadFilterMsg);
            }
        }

        private void OnThreadPreProcessMessage(ref MSG msg, ref bool handled)
        {
            if ((msg.message == NativeConstants.WM_KEYDOWN) && (msg.wParam.ToInt32() == NativeConstants.VK_TAB))
            {
                CoreLogger.LogStatus("Tab key", ConsoleColor.Magenta);
                _trace.SetTrace(CompDispAPI.ThreadPreProcessMsg);
            }
        }

        private void OnLeaveModal(object sender, EventArgs e)
        {
            //Console.WriteLine("OnLeaveModal");
            _trace.SetTrace(CompDispAPI.LeaveThreadModal);
        }
    }

    interface ChildPopWindow
    {
        void ShowModalWindow();

        void CloseModalWindow();

        bool IsWindowVisible { get; }
    }

    class ChildWindow : Window, ChildPopWindow
    {
        public ChildWindow(Window parent)
        {
            _onActivate = new EventHandler(OnActivate);

            this.Background = Brushes.Yellow;
            this.Owner = parent;
            this.Width = 300;
            this.Height = 200;
            this.Left = 300;
            this.Top = 300;
            this.ResizeMode = ResizeMode.NoResize;
            this.Title = "Dialog Box";
            this.Activated += _onActivate;

            Button b = new Button();
            b.Height = 100;
            b.Width = 100;
            b.Content = "Modal Avalon Window Button";
            this.Content = b;
        }

        private EventHandler _onActivate;

        private void OnActivate(object sender, EventArgs e)
        {
            this.Activated -= _onActivate;

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(DispatcherPriority.Input,
                (DispatcherOperationCallback)delegate(object o)
                {
                    KeyboardHelper.TypeKey(Key.Tab);

                    return null;

                }, null);
        }

        public void ShowModalWindow()
        {
            this.ShowDialog();
        }

        public void CloseModalWindow()
        {
            this.Close();
        }

        public bool IsWindowVisible
        {
            get
            {
                return (this.Visibility == Visibility.Visible);
            }
        }
    }


    // WinFORM
    internal class ChildWinForm : SWF.Form, ChildPopWindow
    {
        SWF.Button _b1 = null;
        SWF.Button _b2 = null;
        EventHandler _onActivate;

        public ChildWinForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _onActivate = new EventHandler(OnActivated);

            this.SuspendLayout();

            _b1 = new SWF.Button();
            this._b1.Location = new System.Drawing.Point(0, 0);
            this._b1.Name = "button1";
            this._b1.Size = new System.Drawing.Size(400, 30);
            this._b1.TabIndex = 1;
            this._b1.Text = "Outter WinForm. Mne Key: &O";

            this.Controls.Add(_b1);


            _b2 = new SWF.Button();
            this._b2.Location = new System.Drawing.Point(0, 30);
            this._b2.Size = new System.Drawing.Size(400, 300);
            this._b2.TabIndex = 2;

            this.Controls.Add(_b2);

            this.Size = new System.Drawing.Size(400, 400);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Activated += _onActivate;
        }

        delegate void SendInputDelegate();

        void SendInput()
        {
            //CoreLogger.LogStatus("Send Input", ConsoleColor.Magenta);
            KeyboardHelper.TypeKey(Key.Tab);
        }

        void OnActivated(object sender, EventArgs e)
        {
            this.Activated -= _onActivate;

            //CoreLogger.LogStatus("In Winform activate", ConsoleColor.Blue);
            BeginInvoke(new SendInputDelegate(SendInput));
        }

        public void ShowModalWindow()
        {
            this.ShowDialog();
        }

        public void CloseModalWindow()
        {
            this.Close();
        }

        public bool IsWindowVisible
        {
            get
            {
                return this.Visible;
            }
        }
    }

    

    class ValidateTrace
    {
        Dictionary<CompDispAPI, int> _traces;

        private ValidateTrace()
        {
            _traces = new Dictionary<CompDispAPI, int>();
        }

        public int this[CompDispAPI t]
        {
            get
            {
                if (_traces.ContainsKey(t))
                    return _traces[t];
                else
                    return -1;
            }
        }

        public void SetTrace(CompDispAPI t)
        {
            if (_traces.ContainsKey(t))
            {
                _traces[t]++;
            }
            else
            {
                _traces.Add(t, 1);
            }

        }

        static ValidateTrace s_inst;

        static public ValidateTrace GetInstance()
        {
            if (s_inst == null)
            {
                s_inst = new ValidateTrace();
            }
            return s_inst;
        }

        public void Reset()
        {
            _traces.Clear();
        }
    }

}
