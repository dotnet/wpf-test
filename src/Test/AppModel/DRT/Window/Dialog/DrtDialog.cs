// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using System.Windows.Automation;


namespace DRT
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            DrtDialog.DisableProcessWindowsGhosting();

            DrtDialog myApp = new DrtDialog();
            try
            {
                DrtDialog.BlockInput(true);
                myApp.Run();
            }
            finally
            {
                DrtDialog.BlockInput(false);
            }

            if ( myApp.Result == 0 )
            {
                myApp.Logger.Log("Passed");
            }
            else
            {
                myApp.Logger.Log("Dialog Window failed. Drt owner: WPF");
            }

            return myApp.Result;
        }
    }

    //  Create owner Window
    //  Create Dialog
    //  Try to mouse click on owner window and check whether dialog
    //  window is still the ForegroundWindow
    //  Test AcessKey Esc. It should close the dialog when there is a cancel button.
    public class DrtDialog : Application
    {
        int             _retVal;
        bool            _cancelButtonClicked;
        DispatcherTimer _timer;
        Window          _window0;
        Window          _window;
        Window          _dlg;
        EventHandler    _onWindowActivated;
        EventHandler    _onDialogActivated;
        int             _enterModalCount;
        int             _leaveModalCount;

        Logger _logger;

        internal Logger Logger
        {
            get
            {
                return _logger;
            }
        }

        internal int Result
        {
            get
            {
                return _retVal;
            }
        }
        public DrtDialog() :base()
        {
            _logger = new Logger("DrtDialog", "Microsoft", "Testing Dialog behavior");

            _onWindowActivated = new EventHandler(OnWindowActivated);
            _onDialogActivated = new EventHandler(OnDialogActivated);

            ComponentDispatcher.EnterThreadModal += new EventHandler(OnEnterThreadModal);
            ComponentDispatcher.LeaveThreadModal += new EventHandler(OnLeaveThreadModal);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Log("Creating owner Window...");

            // The first window. At (0, 0)
            _window0 = new Window();
            _window0.Background = Brushes.Aqua;
            _window0.Left = 0;
            _window0.Top = 0;
            _window0.Width = 500;
            _window0.Height = 400;
            _window0.Title = "The first Window";
            _window0.Show();

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(OpenSecondWindow),
                null);
        }

        private object OpenSecondWindow(object obj)
        {
            // The second and owner window. At (100, 100)
            _window = new Window();
            _window.Background = Brushes.Aqua;
            _window.Left = 100;
            _window.Top = 100;
            _window.Width = 500;
            _window.Height = 400;
            _window.Title = "The owner Window";
            _window.Activated += _onWindowActivated;
            _window.Closed += new EventHandler(OnWindowClosed);

            Button button1 = new Button();
            button1.Height = 100;
            button1.Width = 100;
            button1.Content = "Open a Modal Dialog";

            _window.Content = button1;
            _window.Show();
            return null;
        }

        private void OnWindowActivated ( object sender, EventArgs e )
        {
            _window.Activated -= _onWindowActivated;
            // This doesn't guarantee that the window will come
            // to the top of the z-order.
            _window.Activate();
            OpenDialog();
        }

        public void OpenDialog()
        {
            // The dialog. At (300, 300)
            _dlg = new Window();
            _dlg.Background = Brushes.Aqua;
            _dlg.Owner = _window;
            _dlg.Width = 300;
            _dlg.Height = 200;
            _dlg.Left = 300;
            _dlg.Top = 300;
            //_dlg.WindowBorderStyle = WindowBorderStyle.FixedDialog;
            _dlg.ResizeMode = ResizeMode.NoResize;
            _dlg.Title = "Dialog Box";
            _dlg.Activated += _onDialogActivated;

            //add Cancel button to dialog. It responses to Esc.
            Button button1 = new Button();
            button1.Height = 100;
            button1.Width = 100;
            button1.Content = "Cancel";
            button1.IsCancel = true;
            button1.Click += new RoutedEventHandler(OnButtonClick);

            _logger.Log("Showing dialog...");

            _dlg.Content = button1;

            // Start timeout timer for 120 secs (2 mins)
            // In case the drt fails, when timeout, the dialog and all windows will
            // be closed.
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Interval = TimeSpan.FromMilliseconds(120000);

            _timer.Start();

            _dlg.ShowDialog(); // this call blocks

            // Close windows and stop timer
            _window.Close();
            _window0.Close();
            _timer.Stop();

            // verify that ComponentDispatcher callbacks were called
            if (_leaveModalCount != 1)
            {
                _logger.Log("LeaveThreadModal callback was not called.");
                _retVal = 1;
            }

        }

        private void OnDialogActivated ( object sender, EventArgs e )
        {
            _dlg.Activated -= _onDialogActivated;
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(DialogVerifyCallback),
                null);
        }

        private object DialogVerifyCallback(object obj)
        {
            IntPtr fgHandle;
            WindowInteropHelper wihFirstWin = new WindowInteropHelper(_window0);
            WindowInteropHelper wihSecondWin = new WindowInteropHelper(_window);

            _logger.Log("Verifying Dialog behavior.");

            // verify that ComponentDispatcher callbacks were called
            if (_enterModalCount != 1)
            {
                _logger.Log("EnterThreadModal callback was not called.");
                _retVal = 1;
            }

            // Automation stuff...
            // Try clicking on the first window i.e _window0
            Input.MoveToAndClick(new Point(50, 50));
            fgHandle = GetForegroundWindow();
            if (fgHandle == wihFirstWin.Handle)
            {
                _logger.Log("Focus moved to the first window from dialog...error!!");
                _logger.Log("Dialog is thread Modal. The first window should be and remain disabled when dialog shows up.");
                _retVal = 1;
            }

            // Try clicking on the owner window i.e _window
            Input.MoveToAndClick(new Point(200, 200));
            fgHandle = GetForegroundWindow();
            if (fgHandle == wihSecondWin.Handle)
            {
                _logger.Log("Focus moved to owner window from dialog...error!!");
                _logger.Log("Dialog is thread Modal. The owner window should be and remain disabled when dialog shows up.");
                _retVal = 1;
            }

            PressEscape();

            return null;
        }

        //verify the Cancel button was clicked and DialogResult is Cancel
        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_cancelButtonClicked == false)
            {
                _logger.Log("Test failed. Cancel button was not clicked.");
                _logger.Log("Esacpe was pressed but Cacnel button event handler hasnot been called.");
                _logger.Log("Pressing Escape should be the same as clicking on the Cancel button; and Button event handler should be called before Window closes.");

                _retVal = 1;
            }

            if (_dlg.DialogResult != false)
            {
                _logger.Log("DialogResult should be false");
                _logger.Log("When Dialog close is triggered via Cancel button, DialogResult should be false.");

                _retVal = 1;
            }
        }

        // Timeout. Close all Windows
        private void OnTick(object sender, EventArgs e)
        {
            if (_dlg.Visibility == Visibility.Visible)
            {
                _logger.Log("Dialog was not closed -- timed out");
                _logger.Log("We press Escape and Dialog should be closed when Cacnel button is presented.");
                _logger.Log("We wait for 2 mins and if you get here it means Dialog has failed to close, we abort the test");

                _dlg.DialogResult = null;
                _retVal = 1;
            }

            //close all windows
            _window.Close();
            _window0.Close();
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _cancelButtonClicked = true;
        }

        private void PressEscape()
        {
            Input.SendKeyboardInput(Key.Escape, true);
            Input.SendKeyboardInput(Key.Escape, false);
        }

        private void OnEnterThreadModal(object sender, EventArgs e)
        {
            ++ _enterModalCount;
        }

        private void OnLeaveThreadModal(object sender, EventArgs e)
        {
            ++ _leaveModalCount;
        }


        [DllImport( "user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int BlockInput(bool fBlockIt);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();

    }

}

