// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>HwndHostClickWin32Button.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class HwndHostClickWin32Button : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndHostClickWin32Button() :base(TestCaseType.HwndSourceSupport)
        {

        }

        /// <summary>
        /// Create a  HwndSource that host a Win32Button using HwndHost and later click on the Win32Button and recive the message
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create a HwndDispatcher and Context</li>
        ///     <li>Create a canvas and a HwndHost to host a Win32ButtonElement</li>
        ///     <li>When the button is painted we click on the Win32Button</li>
        ///     <li>When we receive the message, exit the dispatcher</li>
        ///  </ol>
	    ///     <filename>HwndHostClickWin32Button.cs</filename>
        /// </remarks>
        [TestAttribute(0, @"Hosting\Click\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostClickWin32Button", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            try
            {

                Canvas canvas = new Canvas();

                using (CoreLogger.AutoStatus("Creating a Win32ButtonElement and adding to the Tree"))
                {
                    _host = new Win32ButtonElement();
                    _host.Width = 200;
                    _host.Height = 200;
                    canvas.Children.Add(_host);

                    _host.Painted += new EventHandler(Fired);

                    _host.ContainerWindowHook += new HwndSourceHook(mouseListener);
                }

                Source.RootVisual = canvas;


                using (CoreLogger.AutoStatus("Dispatcher.Run"))
                {
                    Dispatcher.Run();
                }
            }
            finally
            {
                CoreLogger.EndVariation();
            }            
            
        }

        void Fired(object t, EventArgs e)
        {

        
            if (!s_b)
            {
                s_b = true;

                Win32ButtonElement button = t as Win32ButtonElement;
                button.ValidateHandle();

                if (_host.Width != 200  || _host.Height != 200 )
                    throw new Microsoft.Test.TestValidationException("Length are not expected");


                ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle, new TimeSpan(0,0,2), new EventHandler(_mouseClick), null);

            }
        }



        IntPtr mouseListener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeConstants.WM_COMMAND && lParam == _host.Win32Handle && (Int64)wParam == NativeConstants.BN_CLICKED)
            {
                using (CoreLogger.AutoStatus("Exit the dispatcher"))
                {
                    MainDispatcher.InvokeShutdown();
                }
            }

            handled = false;

            return IntPtr.Zero;
        }


        
        void  _mouseClick(object o, EventArgs args)
        {
            DispatcherTimer dt = (DispatcherTimer)o;
            dt.Stop();            
            using (CoreLogger.AutoStatus("Click on the button"))
            {
                MouseHelper.Click((IntPtr)_host.Win32Handle);
            }


        }



        static bool s_b  = false;
        Win32ButtonElement _host = null;

    }
        
 }











