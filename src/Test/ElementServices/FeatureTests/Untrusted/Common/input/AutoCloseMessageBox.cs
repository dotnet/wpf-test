// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Win32;
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Text;
using System.Timers;
using System.Windows;
using System.Runtime.InteropServices;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// An automatically closing message box.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This serves as a base class wrapper for multiple message box APIs.
    /// </para>
    /// <para>
    /// After a given timeout period expires, the message box is dismissed.
    /// </para>
    /// <para>
    /// The message box caption is adorned with a custom string for easy identification.
    /// </para>
    /// </remarks>
    public abstract class AutoCloseMessageBox
    {
        /// <summary>
        /// Construct an empty AutoCloseMessageBox.
        /// </summary>
        public AutoCloseMessageBox(): base()
        {
        }

        /// <summary>
        /// Show this message box.
        /// </summary>
        /// <returns>Message box result after the box is dismissed.</returns>
        public MessageBoxResult Show()
        {
            // NOTE: We use System.Timers.Timer so UI thread can't block it:
            // http://msdn.microsoft.com/msdnmag/issues/04/02/TimersinNET/default.aspx
            
            Timer t = new Timer(this.Timeout);

            t.AutoReset = false;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Enabled = true;

            return ShowCore(this.Text, CaptionAdornmentString + this.Caption);
        }

        /// <summary>
        /// Overrideable method to actually do the work to show the message box.
        /// </summary>
        /// <param name="text">Text to show in the message box.</param>
        /// <param name="caption">Caption for message box.</param>
        /// <returns>Message box result after the box is dismissed.</returns>
        protected abstract MessageBoxResult ShowCore(string text, string caption);

        /// <summary>
        /// Called after the message box timer has expired.
        /// </summary>
        /// <param name="sender">Object sending timeout.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            FindAndKillWindow();
        }

        /// <summary>
        /// Find and kill the message box.
        /// </summary>
        private void FindAndKillWindow()
        {
            // Get ready to find window
            IntPtr hParent = IntPtr.Zero;
            IntPtr hNext = IntPtr.Zero;
            String messageBoxClassName = "#32770";

            // Loop until window is found.
            do
            {
                hNext = NativeMethods.FindWindowEx(hParent, hNext, messageBoxClassName, null);

                if (!hNext.Equals(IntPtr.Zero))
                {
                    // We have a window! Get its caption.
                    StringBuilder sLimitedLengthWindowTitle = new StringBuilder(256);

                    NativeMethods.GetWindowText(hNext, sLimitedLengthWindowTitle, 256);

                    String windowCaption = sLimitedLengthWindowTitle.ToString();

                    if (windowCaption.Length > 0)
                    {
                        // We have a caption! It should have our text somewhere....
                        if (windowCaption.StartsWith(CaptionAdornmentString))
                        {
                            KillWindow(new HandleRef(null, hNext));
                        }
                    }
                }
            } 
			while (!hNext.Equals(IntPtr.Zero));
        }

        /// <summary>
        /// Safely kill a message box.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        protected virtual void KillWindow(HandleRef hwnd)
        {
            // 



            // Just send a message to kill the box.
            NativeMethods.SendMessage(hwnd, NativeConstants.WM_COMMAND, NativeConstants.IDCANCEL, 0);
            
            // Message box goes away.
        }

        /// <summary>
        /// How long the message box appears before being auto-closed.
        /// </summary>
        /// <value>Time span in milliseconds.</value>
        public int Timeout
        {
            get { return (int)_timeout.TotalMilliseconds; }
            set { _timeout = new TimeSpan(0, 0, 0, 0, value); }
        }
        private TimeSpan _timeout;

        /// <summary>
        /// Message box caption.
        /// </summary>
        /// <value>Title-bar string.</value>
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
        private string _caption;

        /// <summary>
        /// Message box text.
        /// </summary>
        /// <value>Main dialog string.</value>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        private string _text;

        private const string CaptionAdornmentString = @":AutoCloseMessageBox:";

    }
}
