// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// MultiThread app
    /// BVT that creates and shows multiple Windows / NavigationWindows on multiple threads
    /// Verifies that only windows created on the Application main thread are added to the Application Windows collection
    /// Verifies that we can create, show, access / set properties such as Title, Content on Windows created on the same  thread
    /// Verifies that we can set content (navigate), goback, goforward, access cangoback / cangoforward, addbackentry and removebackentry on NavigationWindows created on the same thread
    /// </summary>
    public partial class NavigationTests : Application
    {
        private Hashtable _threads = new Hashtable();
        private int _multithreadedApp_maxThreads = 3;
        private int _multithreadedApp_maxWindowsPerThread = 3;
        private Hashtable _multithreadedApp_windows = new Hashtable();
        private int _multithreadedApp_TotalWindowsClosed = 0; // counter for the total number of windows closed

        private void MultithreadedApp_Startup(object sender, StartupEventArgs e)
        {
            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);

            // Create the main window
    	    Window w = new Window();
            w.Title = "Main Window";
            w.Background = Brushes.Green;
            w.Show();            
            
            // Create worker threads and windows
            for (int i = 0; i < _multithreadedApp_maxThreads; i++)
            {
                MultithreadedApp_CreateAndStartNewThread();
		        Thread.Sleep(5000);
            }

            // verify window properties and navigation on a different thread
            Thread t = new Thread(new ThreadStart(MultithreadedApp_VerifyTest));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        void MultithreadedApp_Exit(object sender, ExitEventArgs e)
        {
	        NavigationHelper.Output("Shutting down");
        }

        void MultithreadedApp_VerifyWindowTitle(Window window, String title)
        {
            NavigationHelper.Output("Window title = " + window.Title + " expectedTitle = " + title);
            if (window.Title.Equals(title))
            {
                NavigationHelper.Output("Window title matched");
            }
            else
            {
                NavigationHelper.Fail("Window title did not match");
	        }                      
        }

        void MultithreadedApp_VerifyTest()
        {
            // Verify that only one window exists on the Application WindowCollection
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object o)
            {
                NavigationHelper.Output("#Windows = " + Application.Current.Windows.Count
                    + " MainWindow title = " + Application.Current.MainWindow.Title);
                if (Application.Current.Windows.Count == 1
                    && Application.Current.MainWindow.Title.Equals("Main Window"))
                {
                    NavigationHelper.Output("Application Windows.Count and MainWindow title matched expected values");
                }
                else
                {
                    NavigationHelper.Fail("Application Windows.Count and MainWindow title did not match expected values");
                }
                return null;
            }, null);

            // loop through threads and windows and verify
            for (int i = 0; i < _multithreadedApp_maxThreads; i++)
            {
                for (int k = 0; k < _multithreadedApp_maxWindowsPerThread; k++)
                {
                    Thread thread = _threads["Thread" + i] as Thread;
                    Dispatcher d = Dispatcher.FromThread(thread);
                    int j = k;
                    Window window = _multithreadedApp_windows[thread.Name + " Window #" + k] as Window;
                    window.Closed += new EventHandler(WindowClosedHandler);
                    NavigationWindow navWin = null;

                    d.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
                    {
                        // activate window
                        lock (this)
                        {
                            NavigationHelper.Output("Activating window");
                            window.Activate();
                            NavigationHelper.Output("Window.IsActive = " + window.IsActive);
                            if (window.IsActive)
                            {
                                NavigationHelper.Output("Window was activated");
                            }
                            else
                            {
                                NavigationHelper.Fail("Window was not activated");
                            }
                        }

                        // verify title                    
                        MultithreadedApp_VerifyWindowTitle(window, thread.Name + " Window #" + j);

                        // set title
                        NavigationHelper.Output("Setting Window title to NewTitle");
                        window.Title = "NewTitle";

                        // verify title
                        MultithreadedApp_VerifyWindowTitle(window, "NewTitle");

                        if (window is NavigationWindow)
                        {
                            // verify navigation window content
                            navWin = window as NavigationWindow;
                            NavigationHelper.Output("NavigationWindow - verifying content and performing simple navigate operations");
                            NavigationHelper.Output("NavigationWindow content is " + navWin.Content);
                            NavigationHelper.Output("NavigationWindow title is " + navWin.Title);
                            Page p = navWin.Content as Page;
                            if (p != null)
                            {
                                NavigationHelper.Output("NavWin content was Page as expected");
                            }
                            else
                            {
                                NavigationHelper.Fail("NavWin content was not Page as expected");
                            }

                            // change the navigation window content to new page
                            d.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)
                                    delegate(object ob)
                            {
                                NavigationHelper.Output("Navigating to newly created Page 2");
                                Page newPage = new Page();
                                newPage.WindowTitle = "Page 2";
                                newPage.Title = "Page 2";
                                newPage.Content = "This is Page 2";
                                navWin.Content = newPage;
                                return null;
                            }, null);

                            // verify CanGoBack and go back
                            d.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)
                                    delegate(object ob)
                            {
                                MultithreadedApp_VerifyWindowTitle(navWin, "Page 2");
                                NavigationHelper.Output("CanGoBack, CanGoForward after navigation = " + navWin.CanGoBack
                                            + ", " + navWin.CanGoForward);
                                if (navWin.CanGoBack && !navWin.CanGoForward)
                                {
                                    NavigationHelper.Output("Back/Forward state as expected");
                                }
                                else
                                {
                                    NavigationHelper.Fail("Back/Forward state was not as expected");
                                }
                                navWin.GoBack();

                                return null;
                            }, null);

                            // verify CanGoForward and GoForward
                            d.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)
                                    delegate(object ob)
                            {
                                NavigationHelper.Output("Window title after going back = " + navWin.Title);
                                MultithreadedApp_VerifyWindowTitle(navWin, "Page 2");
                                NavigationHelper.Output("CanGoBack, CanGoForward after navigation = " + navWin.CanGoBack
                                    + ", " + navWin.CanGoForward);
                                if (!navWin.CanGoBack && navWin.CanGoForward)
                                {
                                    NavigationHelper.Output("Back/Forward state as expected");
                                }
                                else
                                {
                                    NavigationHelper.Fail("Back/Forward state was not as expected");
                                }
                                navWin.GoForward();
                                return null;
                            }, null);

                            // Add back journal entry and remove journal entry
                            d.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)
                                    delegate(object ob)
                            {
                                NavigationHelper.Output("Window title after going forward = " + navWin.Title);
                                MultithreadedApp_VerifyWindowTitle(navWin, "Page 2");
                                NavigationHelper.Output("Adding Back Entry");
                                DummyJournalEntry newEntry = new DummyJournalEntry("TestEntry");
                                navWin.AddBackEntry(newEntry);
                                NavigationHelper.Output("Removing Back Entry");
                                JournalEntry removedEntry = navWin.RemoveBackEntry();
                                NavigationHelper.Output("Removed entry = " + removedEntry.Name);
                                if (removedEntry.Name.Equals("TestEntry"))
                                {
                                    NavigationHelper.Output("Removed journal entry matched expected");
                                }
                                else
                                {
                                    NavigationHelper.Fail("Removed journal entry did not match expected");
                                }

                                navWin.Content = "Passed";
                                navWin.Close();

                                return null;
                            }, null);                            
                        }
                        else  // case window is a regular window
                        {
                            NavigationHelper.Output("Window - verifying content");
                            NavigationHelper.Output("Window content is " + window.Content);
                            ProgressBar pb = window.Content as ProgressBar;
                            if (pb != null)
                            {
                                NavigationHelper.Output("Window content was ProgressBar");
                            }
                            else
                            {
                                NavigationHelper.Fail("Window content was not ProgressBar");
                            }
                            window.Content = "Passed";
                            window.Close();
                        }
                        return null;
                    }, null);
                }
            }
        }

        // window close handler
        private void WindowClosedHandler(object sender, EventArgs args)
        {
            Window window = (Window) sender;
            NavigationHelper.Output("In WindowClosedHandler closing window " + window.Title);
            _multithreadedApp_TotalWindowsClosed++;

            // if all the windows are closed, shutdown the application
            if (_multithreadedApp_TotalWindowsClosed == _multithreadedApp_maxThreads * _multithreadedApp_maxWindowsPerThread)
            {
                Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.ApplicationIdle);
                NavigationHelper.Pass("Test Passed");
                NavigationHelper.SetStage(TestStage.Cleanup);
            }
        }

        // create worker threads
        private void MultithreadedApp_CreateAndStartNewThread()
        {
            Thread t = new Thread(new ThreadStart(MultithreadedApp_CreateNewWindows));
            t.Name = "Thread" + _threads.Count;
            _threads.Add(t.Name, t);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        // create windows
        private void MultithreadedApp_CreateNewWindows()
        {
            Window w = null;
            for (int i = 0; i < _multithreadedApp_maxWindowsPerThread; i++)
            {
                if (i % 2 == 0)
                {
                    w = new Window();
                    ProgressBar progressBar = new ProgressBar();
                    progressBar.Background = Brushes.LightCyan;
                    progressBar.Height = 20;
                    progressBar.Width = 200;
                    progressBar.IsIndeterminate = true;
                    progressBar.VerticalAlignment = VerticalAlignment.Center;
                    progressBar.HorizontalAlignment = HorizontalAlignment.Center;
                    w.Content = progressBar;
                }
                else
                {
                    w = new NavigationWindow();
                    Page p = new Page();
                    p.Title = "Page 1";
                    p.Background = Brushes.LightBlue;
                    w.Content = p;
                }
                w.Width = 200;
                w.Height = 200;
                w.Title = Thread.CurrentThread.Name + " Window #" + i;
                w.Show();
                NavigationHelper.Output("Created window " + w.Title);
                _multithreadedApp_windows.Add(w.Title, w);
            }
            Dispatcher.Run();
        }

        /// <remarks> This is not a real JournalEntry, just a state object attached to one created by
        /// the framework. 
        /// </remarks>
        [Serializable]
        public class DummyJournalEntry : System.Windows.Navigation.CustomContentState
        {
            string _displayName;

            public DummyJournalEntry(string name)
            {
                _displayName = name;
            }

            public override void Replay(NavigationService nav, NavigationMode navMode)
            {
                throw new NotImplementedException();
            }

            public override string JournalEntryName
            {
                get { return _displayName; }
            }
        };
    };
}
