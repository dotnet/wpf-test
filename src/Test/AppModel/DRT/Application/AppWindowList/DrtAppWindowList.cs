// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DRT
{
/// <summary>
/// Verify that windows come and go from the application's window list as expected.
/// </summary>
    class DrtAppWindowList : Application
    {
        private Logger _logger = new Logger("DrtAppWindowList", "Microsoft", "Tests the Windows property of Application object");
        static bool _passed = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                this.TestConstruction();
                this.TestWindowNeverShown();
                this.TestWindowShown();

                _passed = true;
            }
            catch (ApplicationException ex)
            {
                _logger.Log("Test failed: " + ex.Message);
            }
            finally
            {
                if (this.Windows.Count != 0)
                {
                    // Shutdown might not work if there are messed up windows lying around
                    // so make sure this ends and returns the failure value.
                    _logger.Log("Failed.");
                    Environment.Exit(1);
                }
                else
                    this.Shutdown();
            }
        }

        /// <summary>
        /// Test the state of the window list on construction
        /// </summary>
        void TestConstruction()
        {
            if (this.Windows.Count != 0)
                throw new ApplicationException("Window list should be empty on construction");
        }

        /// <summary>
        /// Test the state of the window list when windows are created and closed without ever being shown.
        /// </summary>
        void TestWindowNeverShown()
        {
            bool incorrectOwnerUsageThrewException = false; 
            
            // Create and close a window

            Window window = TestWindow();

            if (this.Windows.Count != 1)
                throw new ApplicationException("(Not shown) Window list should contain window as soon as it has been constructed");

            window.Close();

            if (this.Windows.Count != 0)
                throw new ApplicationException("(Not shown) Window should be removed from list when it is closed, even if it has never been shown");


            // Create owner/owned windows, check for exception, close both one by one
            Window parent = TestWindow();
            Window child = TestWindow();

            try
            {
                child.Owner = parent;
            }
            catch (InvalidOperationException)
            {
                incorrectOwnerUsageThrewException = true;
            }

            if (incorrectOwnerUsageThrewException == false)
            {
                throw new ApplicationException("(Not shown) Setting Owner property to a never showns window did not throw InvalidOperationException");
            }
            
            child.Close();

            if (this.Windows.Count != 1)
                throw new ApplicationException(String.Format("(Not shown) Closing the child window should have decreased the count by one.  Expected count: 1; Acutal Count: {0}", this.Windows.Count));

            parent.Close();

            if (this.Windows.Count != 0)
                throw new ApplicationException(String.Format("(Not shown) Closing the parent window should have decreased the count by one.  Expected count: 0; Acutal Count: {0}", this.Windows.Count));
        }
     
        /// <summary>
        /// Test the state of the window list when windows are created, shown and closed.
        /// </summary>
        void TestWindowShown()
        {
            // Create, show and close a window

            Window window = TestWindow();

            window.Show();
            window.Close();

            if (this.Windows.Count != 0)
                throw new ApplicationException("Window that has been shown and then closed should be removed from list");

            // Create and show owner and owned windows, close owner

            Window parent = TestWindow();
            parent.Show();

            Window child = TestWindow();
            child.Owner = parent;
            child.Show();

            if (this.Windows.Count != 2)
                throw new ApplicationException("(Shown) Window list should have both parent and child windows");

            parent.Close();
            if (this.Windows.Count != 0)
                throw new ApplicationException("(Shown) Both parent and child window should be removed from the list when parent is closed");

            // Create and show owner and owned windows, close owned then owner

            parent = TestWindow();
            parent.Show();

            child = TestWindow();
            child.Owner = parent;
            child.Show();

            child.Close();
            if (this.Windows.Count != 1)
                throw new ApplicationException("(Shown) Closing the child window should not close the parent");

            parent.Close();
            if (this.Windows.Count != 0)
                throw new ApplicationException("(Shown) Closing the parent window should leave the list empty.");
        }

        /// <summary>
        /// This is just to make a small window so the display doesn't have to work as hard and the test is faster.
        /// </summary>
        /// <returns>A small window.</returns>
        Window TestWindow()
        {
            Window window = new Window();
            window.Width = 100;
            window.Height = 100;
            return window;
        }

        [STAThread]
        public static int Main()
        {
            DrtAppWindowList theApp = new DrtAppWindowList();
            theApp.Run();

            if (_passed)
            {
                theApp._logger.Log("Passed.");
                return 0;
            }
            else
            {
                theApp._logger.Log("Failed.");
                return 1;
            }
        }
    }
}
