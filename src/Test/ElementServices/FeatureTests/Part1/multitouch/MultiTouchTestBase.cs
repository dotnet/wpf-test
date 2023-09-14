// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Base class for MultiTouch testing     
    /// </summary>
    public abstract class MultiTouchTestBase : AvalonTest
    {
        #region Constructors
        
        protected MultiTouchTestBase() 
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            s_mtTestBase = this;
        }

        #endregion
        
        #region Public and Protected Properties
        
        /// <summary>
        /// The root element.
        /// </summary>
        public FrameworkElement RootElement
        {
            get { return MainWindow.RootVisual as FrameworkElement; }
        }

        /// <summary>
        /// Get the main window.  First access creates the window. 
        /// </summary>
        /// <value></value>
        public HwndSource MainWindow
        {
            get
            {
                if (s_source == null)
                {
                    HwndSourceParameters param = new HwndSourceParameters(WindowTitle);
                    param.SetPosition((int)WindowPosition.X, (int)WindowPosition.Y);

                    if (WindowSize != Size.Empty)
                    {
                        param.SetSize((int)WindowSize.Width, (int)WindowSize.Height);
                    }

                    s_source = new HwndSource(param);

                    InitializeMainWindow();
                }

                return s_source;
            }
            set
            {
                s_source = value;
            }
        }

        public Dispatcher Dispatcher 
        { 
            get 
            { 
                return _dispatcher; 
            } 
        }
        
        /// <summary>
        /// Set the Window title
        /// </summary>
        protected virtual string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; }
        }

        /// <summary>
        /// Set the window size. 
        /// </summary>
        protected Size WindowSize
        {
            get { return _windowSize; }
            set { _windowSize = value; }
        }

        /// <summary>
        /// Set the window position.  
        /// </summary>
        protected Point WindowPosition
        {
            get { return _windowPosition; }
            set { _windowPosition = value; }
        }

        /// <summary>
        /// Set the Window state
        /// </summary>
        protected WindowState WindowState
        {
            get { return _windowState; }
            set { _windowState = value; }
        }

        #endregion
        
        #region Public and Protected Methods
        
        /// <summary>
        /// This is a temp solution to check only supported OS for the infra and lab run config can't support 
        /// OS + Version filtering yet.  So we make pass the MT tests on downlevel OSs to reduce the false negative 
        /// caused by the limitation of the infra / lab run config filtering.  
        /// </summary>
        /// <returns></returns>
        public TestResult PreInitialize()
        {
            TestResult result = TestResult.Pass;

            if (!MultiTouchVerifier.IsSupportedOS() || !(new TouchVerifier(null).IsSimulationAvailable))
            {
                if (!MultiTouchVerifier.IsSupportedOS())
                {
                    GlobalLog.LogStatus("Unsupported OS");
                }
                else
                {
                    GlobalLog.LogStatus("Multitouch Simulator Driver not enabled, contact lab.");
                }
                TestLog log = null;
                bool isLocalLog = false;

                if (TestLog.Current == null)
                {
                    GlobalLog.LogStatus("Not Current TestLog");

                    log = new TestLog(this.GetType().Name);
                    isLocalLog = true;
                }
                else
                {
                    log = TestLog.Current;
                }

                log.Result = TestResult.Ignore;
                result = log.Result;

                if (isLocalLog)
                {
                    log.Close();
                }
            }
            return result;
        }
        
        /// <summary>
        /// Dispose the test window
        /// </summary>
        public void Dispose()
        {
            if (s_source != null)
            {
                s_source.Dispose();
                s_source = null;
            }
        }
        
        /// <summary>
        /// Open the test window
        /// </summary>
        protected abstract void OpenTestWindow();

        /// <summary>
        /// close the test window
        /// </summary>
        protected abstract void CloseTestWindow();
        
        /// <summary>
        /// Do the initilization 
        /// </summary>
        protected void InitializeMainWindow()
        {
            s_source.AddHook(new HwndSourceHook(ApplicationFilterMessage));

            //SetTopMost(source.Handle, true); // 
        }

        /// <summary>
        /// Do the uninitialization 
        /// </summary>
        protected void UninitializeMainWindow()
        {
             s_source.RemoveHook(new HwndSourceHook(ApplicationFilterMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool DetectGesture()
        {
            //
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool DetectManipulation()
        {
            //
            return true; 
        }

        /// <summary>
        /// Beta 2
        /// </summary>
        /// <returns></returns>
        protected bool DetectTouch()
        {
            //

            return true;
        }
        
        /// <summary>
        /// Event which occurs when the main window is deactivated.
        /// </summary>
        protected virtual void OnDeactivated()
        {
        }
        
        /// <summary>
        /// Make sure the element is not null
        /// </summary>
        /// <param name="element">the element being eval</param>
        protected void AssertNotNull(object element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(string.Format("The param {0} should not be null", element)); 
            }
        }

        /// <summary>
        /// Make sure the element is not null
        /// </summary>
        /// <param name="element">the element being eval</param>
        protected void AssertNotNull(object element, string msg)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element", msg);
            }
        }

        /// <summary>
        /// Get the center point of the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual Point GetCenterPoint(UIElement element)
        {
            AssertNotNull(element);
            Point point = new Point(5, 5);

            if (PresentationSource.FromVisual(element) == null)
            {
                WaitFor(500);
            }

            FrameworkElement fElement = element as FrameworkElement; 
            if (fElement != null)
            {
                // for controls
                double height = fElement.ActualHeight;
                double width = fElement.ActualWidth;
                point = new Point(width / 2, height / 2);

                return fElement.PointToScreen(point);
            }

            return point; 
        }

        /// <summary>
        /// get the screen point
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected virtual Point GetScreenPoint(UIElement element)
        {
            AssertNotNull(element);

            if (PresentationSource.FromVisual(element) != null)
            {
                return element.PointToScreen(new Point(5, 5));
            }
            else
            {
                return new Point(5, 5);
            }
        }
        
        /// <summary>
        /// Helper function to block till all queue items are processed, also introduce time
        /// lag between subsequent test runs
        /// 
        /// NOTE - due to te instability of the MultiTouch driver and some issue with the event routing, 
        /// we don't always get the expected ManipulationCompleted or TouchUp event, so use this more generic 
        /// method, instead of some specific waitfor
        /// </summary>
        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// For prev .NET 4.0
        /// </summary>
        private void SetTouch()
        {
            if (MainWindow != null && MainWindow.Handle != IntPtr.Zero)
            {
                MultiTouchNativeMethods.SetPropMT(new HandleRef(this, MainWindow.Handle), "MicrosoftTabletPenServiceProperty", new HandleRef(null, new IntPtr(0x01000000)));
            }
        }

        private static IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // quit the app if the source win is closed
            if (msg == MultiTouchNativeMethods.WM_CLOSE)
            {
                s_mtTestBase.Dispatcher.BeginInvoke(
                    DispatcherPriority.ApplicationIdle,
                    new DispatcherOperationCallback(Quit),
                    null
                    );

                handled = true;
            }

            if (msg == MultiTouchNativeMethods.WM_ACTIVATE && wParam == IntPtr.Zero)
            {
                s_mtTestBase.OnDeactivated();
            }

            return IntPtr.Zero;
        }

        private static object Quit(object arg)
        {
            s_mtTestBase.Dispatcher.InvokeShutdown();
            s_mtTestBase.Dispose();
            return null;
        }
        
        #endregion

        #region Fields

        private static HwndSource s_source = null;
        //private static Visual rootElement;
        private static MultiTouchTestBase s_mtTestBase;

        private string _windowTitle = "MultiTouch Testing";
        private Size _windowSize = new Size(800, 600);
        private Point _windowPosition = new Point(50, 50);
        private WindowState _windowState;
        private Dispatcher _dispatcher;

        #endregion

    }
}
