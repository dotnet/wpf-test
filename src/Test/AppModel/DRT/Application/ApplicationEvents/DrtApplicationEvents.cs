// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Reflection;

using System.Windows.Threading;
using System.Windows;
using System.Windows.Interop;

namespace DRT
{
    public class DrtApplication
    {
        [STAThread]
        public static int Main()
        {
            DrtApplication theTest = new DrtApplication();
            DrtApplication.DisableProcessWindowsGhosting();
            
            try
            {
                theTest.Run(); 
            }
            catch (ApplicationException e)
            {
                theTest.Output(e);
                return 1;
            }   

            if ( ! _fStartupCalled ) 
            {
                theTest.Output( "Error: - DrtApplication. Listener to Application.Startup not called");
                return 1;
            }
            else if ( ! _fExitCalled ) 
            {
                theTest.Output( "Error: - DrtApplication. Listener to Application.Exit not called");
                return 1;
            }
            else if ( ! _fActivatedCalled )
            {
                theTest.Output( "Error: - DrtApplication. Listener to Application.Activated not called");
                return 1;
            }
            else if ( ! _fDeactivatedCalled )
            {
                theTest.Output( "Error: - DrtApplication. Listener to Application.Deactivated not called");
                return 1;
            }
            else if ( ! _fSessionEndingCalled )
            {
                theTest.Output( "Error: - DrtApplication. Listener to Application.SessionEnding not called");
                return 1;
            }
            else if ( ! _fReasonLogoff ) 
            {
                theTest.Output( "Error: - DrtApplication. Application.SessionEnding -- ReasonSessionEnding did not equal to ReasonSessionEnding.Logoff");
                return 1;
            }
            else if ( ! _fReasonShutdown )
            {
                theTest.Output( "Error: - DrtApplication. Application.SessionEnding -- ReasonSessionEnding did not equal to ReasonSessionEnding.Shutdown");
                return 1;
            }
            
            theTest.Output( "Passed" );
            return 0;
        }

        private Logger      _logger = new Logger("DrtApplicationEvents", "Microsoft", "Testing Application object events");
        private static bool _fStartupCalled ; 
        private static bool _fExitCalled ; 
        private static bool _fActivatedCalled ;
        private static bool _fDeactivatedCalled ;
        private static bool _fSessionEndingCalled ;
        private static bool _fReasonShutdown ;
        private static bool _fReasonLogoff ;
     
        private void Run() 
        {
            //
            // Basic AppObject test. 
            //
            // 1 - Create an Application Object. 
            // 2 - Attach Listeners to StartingUp, ShuttingDown, Activated, Deactivated, 
            //     and SessionEnding Events
            // 2a- Creates window before app creation
            // 3 - Call Application.Run(window).
            // 4 - Create a window and show it inside the StartingUp eventhandler.
            //     Also send WM_QUERYENDSESSION msg twice to simulate logoff and shutdown on machine
            // 5 - Ensure that StartingUp and ShuttingDown events are called, and in the right order
            //     Validate that passed in arguments are obtained on the starting up event. 
            //     Validate that Application.Current is not null. 
            //     Validate that setting ExitCode in ShuttingDown works. 
            //     Ensure that Activated and Deactivated events are called.
            //     Enusre that the SessionEndingEventArgs has the correct ReasonSessionEnding enum value
            //     Cancel SessionEnding from SessionEnding event handler the first time it's called
            //     to check that cancellation works
            //     Do not cancel SessionEnding from SessionEnding event handler the second time it's called
            //     to check that the App shut's down automatically

            Window w = new Window();
            Application theApplication = new Application();

            theApplication.Startup += new StartupEventHandler( StartupHandler ); 
            theApplication.Exit += new ExitEventHandler( ExitHandler );
            theApplication.Activated += new EventHandler( OnActivated ) ;
            theApplication.Deactivated += new EventHandler( OnDeactivated ) ;
            theApplication.SessionEnding += new SessionEndingCancelEventHandler( OnSessionEnding ) ;

            VerifyCookies();

            int theCode = theApplication.Run(w);
            if ( theCode != -999 )
            {
                throw new ApplicationException( "Error: - DrtApplication. Did not get expected result passed from ShuttingDownEventArgs.ApplicationExitCode to Application.Run");
            }
        }

        private void VerifyCookies()
        {
            DateTime dt = DateTime.UtcNow;
            dt = dt.AddYears(10);

            string HelloWorld = "Hello=World";
            Uri uri = new Uri("http://www.asdf123456.com");
            Application.SetCookie(uri,  HelloWorld + "; expires=" + dt.ToString("r"));
            string s = Application.GetCookie(uri);
            if (s != HelloWorld)
            {
                throw new ApplicationException("Error: - DrtApplication. Set/Get cookie API did not work.");
            }

            string HelloWorldLong = "Hello=WorldSupercalifragilisticexpealidocious";
            Application.SetCookie(uri,  HelloWorldLong + "; expires=" + dt.ToString("r"));
            s = Application.GetCookie(uri);
            if (s != HelloWorldLong)
            {
                throw new ApplicationException("Error: - DrtApplication. Set/Get long cookie API did not work.");
            }
        }
        
        private void StartupHandler(Object sender, StartupEventArgs e)
        {
            Output("Startup event handler called");
            if (DrtApplication._fExitCalled)
            {
                throw new ApplicationException("DrtApplication. Application events called in wrong order ( Exit called before Startup)");
            }

            if ((e.Args.Length != 2) || (e.Args[0] != "Hello") || (e.Args[1] != "World"))
            {
                throw new ApplicationException("DrtApplication. Did not get expected arguments passed to StartupEventArgs.Args ");
            }
               
            DrtApplication._fStartupCalled = true;
            Application  theApplication = Application.Current;
            if (theApplication == null)
            {
                throw new ApplicationException("DrtApplication. Application.Current was unexpectedly null in Startup event");
            }
                
            Application.Current.MainWindow.Width = 200;
            Application.Current.MainWindow.Height = 200;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(SendMessage),
                null);
        }

        private object SendMessage(object obj)
        {
            // get the _parkingHwnd field from Application
            Type appType = Application.Current.GetType();
            FieldInfo fieldInfo = appType.GetField("_parkingHwnd", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            object parkingHwnd = fieldInfo.GetValue(Application.Current);

            Assembly pfAssembly = typeof(System.Windows.DependencyProperty).Assembly; // WindowsBase.dll
            Output(String.Format("AssemblyName: {0}", pfAssembly.ToString()));

            // Get HwndWrapper internal type, Handle method on it and 
            // retrieve the hwnd from _parkingHwnd
            Type hwndWrapperType = pfAssembly.GetType("MS.Win32.HwndWrapper", false, true);
            Output(String.Format("ClassName: {0}", hwndWrapperType.ToString()));

            MethodInfo handleInfo = hwndWrapperType.GetMethod("get_Handle");
            IntPtr hwnd = (IntPtr)handleInfo.Invoke(parkingHwnd, new object[0]);
            Output(String.Format("hwnd = {0}", hwnd));
            
            SendMessage(new HandleRef(null, hwnd),
                0x11, //  <--- NativeMethods.WM_QUERYENDSESSION
                new IntPtr(), 
                new IntPtr(unchecked((int)0x80003400))); //<--- ENDSESSION_LOGOFF

            SendMessage(new HandleRef(null, hwnd),
                0x11, //  <--- NativeMethods.WM_QUERYENDSESSION
                new IntPtr(), 
                new IntPtr(0)); // <--- SHUTDOWN

            // We don't need to call Application.Shutdown here b/c when the SessionEnding event is called the
            // second time, the event handler does not cancel it which results in automatic shutting down of
            // app
            return null;
        }

        private void ExitHandler (Object sender, ExitEventArgs e)
        {
            Output("Exit event handler called");
            DrtApplication._fExitCalled = true; 
            e.ApplicationExitCode = -999; 
        }
        private void OnActivated(Object sender, EventArgs e)
        {
            Output("Activated event handler called");
            DrtApplication._fActivatedCalled = true;
        }

        private void OnDeactivated(Object sender, EventArgs e)
        {
            Output("Deactivated event handler called");
            DrtApplication._fDeactivatedCalled = true; 
        }
        
        private  void OnSessionEnding(Object sender, SessionEndingCancelEventArgs e)
        {
            Output("SessionEnding event handler called");
            if ( DrtApplication._fSessionEndingCalled == false )
            {
                if ( e.ReasonSessionEnding == ReasonSessionEnding.Logoff )
                {
                    DrtApplication._fReasonLogoff = true;
                }
                e.Cancel = true;
            }
            else
            {
                if ( e.ReasonSessionEnding == ReasonSessionEnding.Shutdown )
                {
                    DrtApplication._fReasonShutdown = true;
                }
            }
            DrtApplication._fSessionEndingCalled = true;
        }

        private void Output(object obj)
        {
            _logger.Log(obj.ToString());
        }
        
        [DllImport("User32.dll", CharSet=CharSet.Auto)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();        
    }
}
