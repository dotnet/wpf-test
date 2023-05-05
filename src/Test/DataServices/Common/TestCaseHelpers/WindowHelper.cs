// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Data
{
    using System;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// This is a helper class for the window to front and idle api. It is used in VScanVerifier.
    /// </summary>
    // 
    internal class WindowHelper
    {

        private Window _win;
        private ManualResetEvent _onTopEvent;
        private ManualResetEvent _readyEvent;


        internal WindowHelper(Window win)
        {
            _win = win;
            if (win == null)
            {
                throw new NullReferenceException("Window can not be null");
            }
            _onTopEvent = new ManualResetEvent(false);
            _readyEvent = new ManualResetEvent(false);
        }

        internal void OnTop()
        {
            _win.Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(WindowToTop), DispatcherPriority.Send);
            _onTopEvent.WaitOne(300, false);
        }

        private object WindowToTop(object obj)
        {
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(_win);
            System.Runtime.InteropServices.HandleRef handleRef = new System.Runtime.InteropServices.HandleRef(null, helper.Handle);
            Microsoft.Test.Win32.NativeMethods.SetForegroundWindow(handleRef);
            _onTopEvent.Set();
            return null;
        }

        internal void WaitForWindowReady()
        {
			Sleep(200);
            _win.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DispatcherOperationCallback(WindowReady), null );
            Thread.Sleep(200);
            _readyEvent.WaitOne(300, false);
        }

        private object WindowReady(object obj)
        {
            _readyEvent.Set();
            return null;
        }

        internal void WindowToTopAndReady()
        {
            this.OnTop();
            this.WaitForWindowReady();
        }

        private void Sleep(int milliseconds)
        {

			DispatcherFrame frame = new DispatcherFrame();

			FrameTimer sleepTimer = new FrameTimer(frame, milliseconds, new DispatcherOperationCallback(ExitFrameOperation), DispatcherPriority.Send);
			sleepTimer.Start();

			//Pump the dispatcher
			Dispatcher.PushFrame(frame);

		}

        private static object ExitFrameOperation(object obj)
        {
            DispatcherFrame frame = obj as DispatcherFrame;
			frame.Continue = false;
			return null;
		}

        /// <summary>
        /// Get a reference to the Window of a Visual
        /// </summary>
        /// <param name="target">The Visual element that you want the ancestor window for</param>
        /// <returns>The reference to the first window or null if there is not a Window ancestor</returns>
        public static Window GetAncestorWindow(Visual target)
        {
            if (target is Window)
                return (target as Window);

            Visual parent = (Visual)VisualTreeHelper.GetParent(target);

            if (parent == null)
            {
                return null;
            }

            return GetAncestorWindow(parent);
        }

        /// <summary>
        /// Sets the specified window to the foreground, and 
        /// waits for the window to finish rendering
        /// </summary>
        /// <param name="win">Reference to the window</param>
        public static void WindowOnTopAndIdle(Window win)
        {
            WindowHelper helper = new WindowHelper(win);
            helper.WindowToTopAndReady();
        }
    }
	class FrameTimer : DispatcherTimer
	{
		DispatcherFrame _frame;
		DispatcherOperationCallback _callback;
		bool _isCompleted = false;

		public FrameTimer(DispatcherFrame frame, int milliseconds, DispatcherOperationCallback callback, DispatcherPriority priority) : base(priority)
		{
			this._frame = frame;
			this._callback = callback;
			Interval = TimeSpan.FromMilliseconds(milliseconds);
			Tick += new EventHandler(OnTick);
		}

		public DispatcherFrame Frame
		{
			get { return _frame; }
		}

		public bool IsCompleted
		{
			get { return _isCompleted; }
		}

		void OnTick(object sender, EventArgs args)
		{
			_isCompleted = true;
			Stop();
			_callback(_frame);
		}


	}

}
