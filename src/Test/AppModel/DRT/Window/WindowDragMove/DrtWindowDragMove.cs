// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;

using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Automation;

namespace DRT
{
    public class Harness
    {
        [STAThread]
        public static int Main()
        {            
            WindowDragMoveApp.DisableProcessWindowsGhosting();
            WindowDragMoveApp theApp = new WindowDragMoveApp();
            try
            {                
                WindowDragMoveApp.BlockInput(1);
                theApp.Run();
            }
            catch (ApplicationException e)
            {                
                theApp.Logger.Log(e);
                return 1;
            }
            finally
            {
                WindowDragMoveApp.BlockInput(0);
            }            
            theApp.Logger.Log("Passed");
            return 0;
        }
    }

    /// <summary>
    //      Tests DragMove functionality
    /// </summary>        
    public class WindowDragMoveApp : Application
    {
        private static Logger _logger;

        private Window _win;
        
        private Border _root;
       
        private Point _oldPos;
        private Point _newPos;
        private Point _expectedPos;
        private Point _delta;
        private Point _correction;
        private static int TestCode = 0;
        
        public WindowDragMoveApp() : base()
        {
            _logger = new Logger("DrtWindowDragMove", "Microsoft", "Testing Window DragMove behavior");
        }

        internal Logger Logger
        {
            get
            {
                return _logger;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _win = new Window();
            _win.WindowStyle = WindowStyle.None;
            _win.Width = 500;
            _win.Height = 400;
            _win.Top = 0;
            _win.Left = 0;

            _root = new Border ();
            _root.Width = _win.Width;
            _root.Height = _win.Height;
            _root.Background = new SolidColorBrush(Colors.Red);
            _win.Content = _root;            
            _root.AddHandler (Mouse.MouseDownEvent, new MouseButtonEventHandler (OnMouseDown), false);            

            _win.Title = "Window DragMove test";            
            _win.Show();
            
            _oldPos = new Point(_win.Left, _win.Top);
            _delta = new Point(100, 100);
            _correction = new Point(10, 10);
            _expectedPos = new Point(_oldPos.X + _delta.X - _correction.X, _oldPos.Y + _delta.Y - _correction.Y);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(DragMoveTest),
                this);

            base.OnStartup(e);
        }

        /*
         * Test Case: DragMove
         */
        private object DragMoveTest(object obj)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(_root) as HwndSource;
            Debug.Assert(hwndSource != null, "PresentaionSource has to be HwndSource here");

            Point oldPosDeviceUnits = hwndSource.CompositionTarget.TransformToDevice.Transform(_oldPos);
            Point deltaDeviceUnits = hwndSource.CompositionTarget.TransformToDevice.Transform(_delta);
            Point correctionDeviceUnits = hwndSource.CompositionTarget.TransformToDevice.Transform(_correction);

            Input.MoveTo(new Point(oldPosDeviceUnits.X + correctionDeviceUnits.X, oldPosDeviceUnits.Y + correctionDeviceUnits.Y));
            Input.SendMouseInput(oldPosDeviceUnits.X + correctionDeviceUnits.X, oldPosDeviceUnits.Y + correctionDeviceUnits.Y, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.RightDown : SendMouseInputFlags.LeftDown);
            Input.MoveTo(new Point(oldPosDeviceUnits.X + deltaDeviceUnits.X, oldPosDeviceUnits.Y + deltaDeviceUnits.Y));
            Input.SendMouseInput(oldPosDeviceUnits.X + deltaDeviceUnits.X, oldPosDeviceUnits.Y + deltaDeviceUnits.Y, 0, SystemParameters.SwapButtons ? SendMouseInputFlags.RightUp : SendMouseInputFlags.LeftUp);
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(CloseWindow),
                this);
            return null;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _win.DragMove();
            e.Handled = true;
        }

        public static void SetError(string msg)
        {
            _logger.Log("FAILURE: " + msg);
            SetError();
        }

        public static void SetError()
        {
            TestCode = 1;
        }

        public static bool Failed
        {
            get { return TestCode != 0; }
        }

        private object CloseWindow(object obj)
        {
            _newPos = new Point((double)_win.GetValue(Window.LeftProperty), (double)_win.GetValue(Window.TopProperty));
            if ((_expectedPos.X != Math.Round(_newPos.X)) || _expectedPos.Y != Math.Round(_newPos.Y))
            {
                SetError("DragMove failed. The old position is " + _oldPos.ToString() + ". The new position is " + _newPos.ToString() +
                    ". The expected position is " +  _expectedPos.ToString());
                throw new ApplicationException("DragMove failed");
            }            
            _win.Close();
            return null;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int BlockInput(int fBlockIt);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();        
    }
}
