// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DRT
{
    class DrtAppThreadingModel: Application
    {
        private static DrtAppThreadingModel _app;

        private Logger              _logger = new Logger("DrtAppThreadingModel", "Microsoft", "Tests Application functionality for multi-threaded scenarios");

        private Window              _win1;
        private NavigationWindow    _win2;
        private string              _propertyKey = "ID";
        private string              _propertyValue = "123";
        private string              _resourceKey = "resourceDey";
        private object              _resourceValue = new Object();
        private string              _windowTextResourceKey = "windowText";
        private string              _windowTextResourceValue = "Text from Resource";
        
        private bool                _propertiesTestPassed;
        private bool                _findResourceTestPassed;
        private bool                _currentTestPassed;
        private bool                _runTestPassed;
        private bool                _shutdownTestPassed;
        private bool                _shutdownmodeTestPassed;
        private bool                _windowsTestPassed;
        private bool                _mainwindowTestPassed;
        private bool                _win2ResourceInvalidationPassed;
        private bool                _win1ResourceInvalidationPassed;
            
        private bool                _result = true;

        // This DRTs does the following tests:
        //
        // 1) Verify that APIs that are specific to Application thread throw an exception
        //    when called from a worker thread.  We do this test before calling 
        //    Application.Run since we also want to verify run.
        //
        // 2) Add a properties to the Properties property on App thread and verify reading it from 
        //    worker thread.
        //
        // 3) Add a resource on App thread and verify Finding (FindResource) it from worker thread.
        //
        // 4) Read Current from App and worker thread
        //
        // 5) Set up resource references for Window property on two windows created on different
        //    threads.  Then, update resources from worker thread and verify that resources are 
        //    invalidated on both threads/windows.  Resources on the thread adding the resource
        //    invalidated synchronously while worker thread resources are invalidated asynchronously.
        //
        [STAThread]
        public static int Main()
        {
            _app = new DrtAppThreadingModel();
            if (!TestAppThreadOnlyAPIs())
            {
                _app._logger.Log("Failed.");
                return 1;
            }
            else
            {
                _app.Run();
                if (_app._result)
                {
                    _app._logger.Log("Passed.");
                    return 0;
                }
                else
                {
                    _app._logger.Log("Failed.");
                    return 1;
                }
            }
        }

        private static bool TestAppThreadOnlyAPIs()
        {
            Thread t2 = new Thread(new ThreadStart(TestAppThreadOnlyAPIsImpl));
            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();

            // of t2 terminated
            if (t2.Join(TimeSpan.FromMilliseconds(30000)) == true)
            {
                return VerifyAppThreadOnlyAPIs();
            }
            else
            {
                // t2 hung
                // REPORT error
                _app._logger.Log("Thread 2 hung inside TestAppThreadOnlyAPIs");
                t2.Abort();
                return false;
            }
        }

        private static void TestAppThreadOnlyAPIsImpl()
        {
            try
            {
                _app.Run();
            }
            catch (InvalidOperationException)
            {
                _app._runTestPassed = true;
            }

            try
            {
                _app.Shutdown();
            }
            catch (InvalidOperationException)
            {
                _app._shutdownTestPassed = true;
            }

            try
            {
                _app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch (InvalidOperationException)
            {
                _app._shutdownmodeTestPassed = true;
            }

            try
            {
                WindowCollection wc = _app.Windows;
            }
            catch (InvalidOperationException)
            {
                _app._windowsTestPassed = true;
            }

            try
            {
                Window w = _app.MainWindow;
            }
            catch (InvalidOperationException)
            {
                _app._mainwindowTestPassed = true;
            }
        }

        private static bool VerifyAppThreadOnlyAPIs()
        {          
            bool success = true;
            if (_app._runTestPassed == false)
            {
                _app._logger.Log("App.Run() did not throw InvalidOperationException when called from non App thread");
                success = false;
            }

            if (_app._shutdownTestPassed == false)
            {
                _app._logger.Log("App.Shutdown() did not throw InvalidOperationException when called from non App thread");
                success = false;
            }

            if (_app._shutdownmodeTestPassed == false)
            {
                _app._logger.Log("App.ShutdownMode did not throw InvalidOperationException when called from non App thread");
                success = false;
            }

            if(_app._windowsTestPassed == false)
            {
                _app._logger.Log("App.Windows did not throw InvalidOperationException when called from non App thread");
                success = false;
            }

            if(_app._mainwindowTestPassed == false)
            {
                _app._logger.Log("App.MainWindow did not throw InvalidOperationException when called from non App thread");
                success = false;
            }
            return success;
        }
               
        private bool TestPropertiesFindResourceAndCurrentFromWorkerThread()
        {
            Thread t2 = new Thread(new ThreadStart(TestPropertiesFindResourceAndCurrentFromWorkerThreadImpl));
            t2.SetApartmentState(ApartmentState.STA);
            t2.Name = "Worker Thread";
            t2.Start();

            // of t2 terminated
            if (t2.Join(TimeSpan.FromMilliseconds(30000)) == true)
            {
                return VerifyPropertiesFindResourceAndCurrentFromWorkerThread();
            }
            else
            {
                // t2 hung
                // REPORT error
                _logger.Log("Worker thread hung inside TestPropertiesFindResourceAndCurrentFromWorkerThread");
                t2.Abort();
                return false;
            }
        }

        private void TestPropertiesFindResourceAndCurrentFromWorkerThreadImpl()
        {
            if (Properties[_propertyKey].Equals(_propertyValue) == true)
            {
                _propertiesTestPassed = true;
            }

            if (FindResource(_resourceKey) == _resourceValue)
            {
                _findResourceTestPassed = true;
            }
            
            if (this == Current)
            {
                _currentTestPassed = true;
            }
        }
        
        private bool VerifyPropertiesFindResourceAndCurrentFromWorkerThread()
        {
            bool success = true;
            if (_propertiesTestPassed == false)
            {
                _app._logger.Log("NavApp.Properties Test failed in VerifyPropertiesFindResourceAndCurrentFromWorkerThread");
                success = false;
            }

            if (_findResourceTestPassed == false)
            {
                _app._logger.Log("NavApp.FindResource() Test failed in VerifyPropertiesFindResourceAndCurrentFromWorkerThread");
                success = false;                
            }
            
            if(_currentTestPassed == false)
            {
                _app._logger.Log("App.Current test failed in VerifyPropertiesFindResourceAndCurrentFromWorkerThread");
                success = false;
            }
            return success;
        }

        
        // now test stuff that is supposed to work from any thread
        protected override void OnStartup(StartupEventArgs args)
        {
            Properties[_propertyKey] = _propertyValue;
            Resources = new ResourceDictionary();
            Resources.Add(_resourceKey, _resourceValue);
            Resources.Add(_windowTextResourceKey, _windowTextResourceValue);

            if (TestPropertiesFindResourceAndCurrentFromWorkerThread() == false)
            {
                _result = false;
                Shutdown();
            }
            else
            {
                // continue testing for resources
                _win1 = new Window();
                _win1.Width = 300;
                _win1.Height = 300;
                _win1.Title = "Window on App Thread";
                _win1.SetResourceReference(Window.TitleProperty, _windowTextResourceKey); 
                _win1.Show();
                this.Exit += new ExitEventHandler(VerifyResourceInvalidationTestOnAppExit);
                
                ShowWindowOnWorkerThread(); 
            }
            base.OnStartup(args);
        }

        private void VerifyResourceInvalidationTestOnAppExit(Object sender, ExitEventArgs args)
        {
            if (_win1ResourceInvalidationPassed == false)
            {
                _logger.Log("Resource not invalidated on app thread");
                _result = false;
            }

            if (_win2ResourceInvalidationPassed == false)
            {
                _logger.Log("Resource not invalidated on worker thread");
                _result = false;
            }            
        }
        
        private void ShowWindowOnWorkerThread()
        {
            Thread windowThread = new Thread(new ThreadStart(ShowWindow));
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Name = "Worker UI Thread";

            //This will eventually call ShowDialog on the window
            windowThread.Start();
        }

        private void ShowWindow()
        {
            _win2 = new NavigationWindow();
            _win2.Width = 300;
            _win2.Height = 300;
            _win2.Title = "Window on another Thread";
            _win2.Show();
            _win2.Closed += new EventHandler(Win2ClosedHandler);

            _win2.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(StartResourceInvalidationTest),
                    null);
                        
            Dispatcher.Run();
        }

        private void Win2ClosedHandler(object sender, EventArgs args)
        {
            _win2.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }

        // this is called on worker thread
        private object StartResourceInvalidationTest(object obj)
        {            
            _win2.SetResourceReference(Window.TitleProperty, _windowTextResourceKey);

            if (_win2.Title.Equals(_windowTextResourceValue) == true)
            {
                _win2ResourceInvalidationPassed = true;
            }
            _win2.Close();

            _win1.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyResourceInvalidationTestOnAppThread),
                null);
            return null;
        }

        // this is called on App thread
        private object VerifyResourceInvalidationTestOnAppThread(object obj)
        {
            if (_win1.Title.Equals(_windowTextResourceValue) == true)
            {
                _win1ResourceInvalidationPassed = true;
            }
            _win1.Close();
            return null;
        }
    }
}
