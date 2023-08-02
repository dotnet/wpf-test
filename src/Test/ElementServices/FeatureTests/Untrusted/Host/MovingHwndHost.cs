// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>MovingHwndHost.cs</filename>
    ///</remarks>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    [TestDefaults]
    public class MovingHwndHost : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MovingHwndHost() :base(TestCaseType.HwndSourceSupport)
        {

        }

        /// <summary>
        /// Create a  HwndSource that host a Win32Button using HwndHost and Reparent Async. Later making a Click on the new parented
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create a HwndDispatcher and Context</li>
        ///     <li>Create a canvas and a HwndHost to host a Win32ButtonElement</li>
        ///     <li>When the button is painted we click on the Win32Button</li>
        ///     <li>When we receive the message, exit the dispatcher</li>
        ///  </ol>
	    ///     <filename>MovingHwndHost.cs</filename>
        /// </remarks>
        [TestAttribute(0, @"Hosting\Reparent\Simple", TestCaseSecurityLevel.FullTrust, "MovingHwndHost", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            _ev = new AutoResetEvent(false);

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

                ThreadingHelper.BeginInvokeOnSignalHandle(_ev, MainDispatcher, DispatcherPriority.SystemIdle, new DispatcherOperationCallback(reParent), null);
            
                Source.RootVisual = canvas;

                using (CoreLogger.AutoStatus("Dispatcher.Run"))
                {
                    Dispatcher.Run();
                }
            }            
            catch(Exception e)
            {
                CoreLogger.LogException(e);
            }
            CoreLogger.EndVariation();
        }

        void Fired(object t, EventArgs e)
        {
            s_count++;

            Win32ButtonElement button = t as Win32ButtonElement;
            button.ValidateHandle();

            if (_host.Width != 200  || _host.Height != 200 )
                  throw new Microsoft.Test.TestValidationException("Length are not expected");


            if (_host.Parent is StackPanel)
            {
                CoreLogger.LogStatus("Parent is stackpanel");
                ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle, new TimeSpan(0,0,1), new EventHandler(_mouseClick), null);

            }

            if (_host.Parent is Canvas)
            {
                CoreLogger.LogStatus("Parent is canvas");
                _ev.Set();
            }
        }


        object reParent(object o)
        {
            CoreLogger.LogStatus("Reparenting the HwndHost under a StackPanel.");

            Border b = new Border();
            b.Background = Brushes.Green;

            StackPanel sp = new StackPanel();

            b.Child = sp;

            Panel e = _host.Parent as Panel;

            e.Children.Remove(_host);

            sp.Children.Add(_host);

            Source.RootVisual = b;

            // Trigger fired again.
            ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle, new TimeSpan(0, 0, 2), new EventHandler(Fired), null);
            return null;
        }




        IntPtr mouseListener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeConstants.WM_COMMAND && lParam == _host.Win32Handle && (Int64)wParam == NativeConstants.BN_CLICKED)
            {
                if (!(_host.Parent is StackPanel))
                {
                    throw new Microsoft.Test.TestValidationException("The Parent is not correct");
                }
                using (CoreLogger.AutoStatus("Exit the dispatcher"))
                {
                     MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(_exitHandler), null);
                }
            }

            handled = false;

            return IntPtr.Zero;
        }


        
        void _mouseClick(object o, EventArgs args)
        {
            DispatcherTimer dt = (DispatcherTimer)o;
            dt.Stop();
            using (CoreLogger.AutoStatus("Click on the button"))
            {
                MouseHelper.Click((IntPtr)_host.Win32Handle);
            }
        }



        object _exitHandler(object o)
        {
            
            Source.Dispose();

            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }


        AutoResetEvent _ev = null;
        static int s_count  = 0;
        Win32ButtonElement _host = null;

    }
        
 }











