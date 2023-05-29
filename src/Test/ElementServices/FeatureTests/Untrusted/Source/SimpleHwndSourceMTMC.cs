// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Interop;
using sysInterop = System.Windows.Interop;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    /// Create thread with HwndSource and Dispatcher. Close Hwnd, Dispatcher and thread. Repeat once.
    /// </summary>
    /// <remarks>
    ///     <ol>Scenarios steps:
    ///         <li>Create new thread</li>
    ///         <li>Create Dispatcher</li>
    ///         <li>Create HwndSource</li>
    ///         <li>Start Dispatcher</li>
    ///         <li>Close hwnd, shutdown Dispatcher, end thread.</li>
    ///         <li>Repeat the above once</li>
    ///     </ol>
    ///     <Owner>Microsoft</Owner>
 
    ///     <Area>Source\CreateHwndSourceWithDispatcher</Area>
    ///     <location>.cs</location>
    ///</remarks>
    [Test(0, @"Source\MultiThreadMultiContext", TestCaseSecurityLevel.FullTrust, "MultThreadHwndSource", Area = "AppModel")]
    public class MultThreadHwndSource : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public MultThreadHwndSource() : base(TestCaseType.ContextSupport) { }
        
      /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {
            CoreLogger.BeginVariation();
            // Create program.
            CoreLogger.LogStatus("Starting first dispatcher.");
            Program p = new Program();

            // Sleep five seconds after window is created and shutdown dispatcher.
            Thread.Sleep(1000);
            MouseHelper.Click(p.hwnd.Handle);
            Thread.Sleep(1000);
            CoreLogger.LogStatus("Shutting down first dispatcher.");
            p.Shutdown();

            // Create another program.
            CoreLogger.LogStatus("Starting second dispatcher");
            Program p2 = new Program();

            Thread.Sleep(1000);
            MouseHelper.Click(p2.hwnd.Handle);
            Thread.Sleep(1000);
            CoreLogger.LogStatus("Shutting down second dispatcher.");
            p2.Shutdown();
            CoreLogger.EndVariation();
        }
    }

    internal class Program
    {

        public Program()
        {
            // Create a new thread for this instance of Program
            thread = new Thread(new ThreadStart(Run));
            thread.Name = "Test Thread " + thread.Name;
            thread.SetApartmentState(ApartmentState.STA);

            // Create and HwndSource on the new thread, return when creation is done and dispatcher is running.
            madeHwndSourceEvent = new AutoResetEvent(false);
            thread.Start();
            madeHwndSourceEvent.WaitOne();
        }
        
        private void Run()
        {
            // Creates new dispatcher for the current thread.
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            // Make an HwndSource.
            sysInterop.HwndSourceParameters hwndParams = new sysInterop.HwndSourceParameters();
            hwndParams.WindowName = "HwndSourceContext";
            hwndParams.PositionX = 0;
            hwndParams.PositionY = 0;
            hwndParams.Width = 400;
            hwndParams.Height = 400;
            hwndParams.WindowStyle = unchecked((int)0x90000000);    // what styles? overlapped and visible?
            hwndParams.ExtendedWindowStyle = unchecked((int)0x00000008);  // no idea 

            hwnd = new System.Windows.Interop.HwndSource(hwndParams);

            Button b = new Button();
            b.Click += clicked;

            hwnd.RootVisual = b;

            // Signal done making the HwndSource.
            madeHwndSourceEvent.Set();

            Dispatcher.Run();
        }

        public Thread thread;
        public AutoResetEvent madeHwndSourceEvent;
        private Dispatcher _dispatcher;
        private bool _gotClick = false;
        
        internal System.Windows.Interop.HwndSource hwnd = null;

        private void clicked(object s, RoutedEventArgs e)
        {
            _gotClick = true;
        }

        private object Exit(object o)
        {
            hwnd.Dispose();

            _dispatcher.InvokeShutdown();

            return o;
        }
        
 
        public void Shutdown()
        {
            if (!_gotClick)
            {
                throw new Microsoft.Test.TestValidationException("Did not recieve mouse click.");
            }

            // Invoke disposal of the HwndSource and shutdown of the dispatcher.
            _dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(Exit), null);

            // Wait for thread executing Run() to finish.
            // Dispatcher has shutdown because Dispatcher.Run() has returned.
            thread.Join();
        }

    }
 
}

