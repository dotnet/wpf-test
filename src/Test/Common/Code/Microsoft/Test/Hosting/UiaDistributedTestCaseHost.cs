// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Threading;
using System.Windows;

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// Setup UIAutomation test
    /// </summary>
    public class UiaDistributedTestcaseHost : MarshalByRefObject
    {
        #region Private Data

        private NavigationWindow testWindow = null;
        private Dispatcher dispatcher;
        private UiaDistributedTestcase currentTest;
        private object target;
        private delegate Exception MarshalExceptionCallback();

        internal const string RemoteSiteId = "UiaDistributedTestcaseHost";

        /// <summary>
        /// Xaml Browser Host Startup Uri
        /// </summary>
        public const string StartupUriId = "XamlBrowserHostStartupUri";

        #endregion

        #region Constructors

        /// <summary>
        /// get the window and current dispatcher
        /// </summary>
        /// <param name="window">Navigation window hosting the UI to test</param>
        public UiaDistributedTestcaseHost(NavigationWindow window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            //this class is a singleton
            //



            this.testWindow = window;
            dispatcher = Dispatcher.CurrentDispatcher;
            //

        }

        #endregion


        #region public API

        /// <summary>
        /// Gets the Application's Base Directory
        /// </summary>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public string GetApplicationBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Navigate to a xaml page
        /// </summary>
        /// <param name="xamlUri"></param>
        public void Navigate(Uri xamlUri)
        {
            //HACK: need to marshal the exception back to this thread because Invoke does not do it for us
            //need to invoke this on the dispatcher thread synchonously
            Exception exception = null;
            ManualResetEvent syncEvent = new ManualResetEvent(false);

            //handlers for navigation and unhandled exceptions
            DispatcherUnhandledExceptionEventHandler exceptionHandler = delegate(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                exception = e.Exception;
                e.Handled = true;
                syncEvent.Set();
            };
            NavigatedEventHandler navigatedHandler = delegate(object sender, NavigationEventArgs e)
            {
                syncEvent.Set();
            };

            dispatcher.UnhandledException += exceptionHandler;
            dispatcher.Invoke(DispatcherPriority.Background, (MarshalExceptionCallback)delegate()
            {
                testWindow.Navigated += navigatedHandler;

                try
                {
                    //when a navigation error happens the hosted app is shutdown because IE
                    //navigates to an html page with the error in it.  We attach to the dispatcher
                    //error event and prevent this navigation from occuring and marshal the exceptoin
                    //back to the remoting caller.
                    testWindow.Navigate(xamlUri);
                }
                catch (Exception err)
                {
                    exception = err;
                }
                return null;
            });

            //If Navigate did not throw an exception then it will happen async
            //we need to wait for the navigation event to be raised or the dispatcher to get an
            //unhandled exception caused by navigation
            if (exception == null)
                syncEvent.WaitOne();

            //detach the window navigation handler and the dispatcher unhandled exception handler
            dispatcher.Invoke(DispatcherPriority.Background, (MarshalExceptionCallback)delegate()
            {
                testWindow.Navigated -= navigatedHandler;
                return null;
            });
            dispatcher.UnhandledException -= exceptionHandler;

            if (exception != null)
                throw new Exception("An Unhandled Exception Occurred on the Avalon Thread", exception);
        }


        /// <summary>
        /// Attach Testcase
        /// </summary>
        /// <param name="testcase"></param>
        /// <param name="name"></param>
        public void AttachTestcase(UiaDistributedTestcase testcase, string name)
        {
            this.currentTest = testcase;

            //HACK: need to marshal the exception back to this thread because Invoke does not do it for us
            //need to invoke this on the dispatcher thread synchonously
            Exception e = (Exception)dispatcher.Invoke(DispatcherPriority.Background, (MarshalExceptionCallback)delegate()
            {
                try
                {
                    if (name != null)
                    {
                        if (testWindow.Content as FrameworkElement != null)
                        {
                            target = ((FrameworkElement)testWindow.Content).FindName(name);
                        }
                        else if (testWindow.Content as FrameworkContentElement != null)
                        {
                            target = ((FrameworkContentElement)testWindow.Content).FindName(name);
                        }
                        else
                        {
                            target = null;
                        }
                        if (target == null)
                        {
                            throw new ArgumentException("Could not find an element with the id " + name, "id");
                        }
                    }
                }
                catch (Exception err)
                {
                    return err;
                }
                return null;
            });

            if (e != null)
            {
                throw new Exception("An Unhandled Exception Occurred on the Avalon Thread", e);
            }

        }

        /// <summary>
        /// Validate Attached Verifier
        /// </summary>
        public void PerformStep(int stepIndex)
        {
            //HACK: need to marshal the exception back to this thread because Invoke does not do it for us
            //need to invoke this on the dispatcher thread synchonously
            Exception e = (Exception)dispatcher.Invoke(DispatcherPriority.Background, (MarshalExceptionCallback)delegate()
            {
                try
                {
                    currentTest.PerformStep(stepIndex, target);
                }
                catch (Exception err)
                {
                    return err;
                }
                return null;
            });

            if (e != null)
                throw new Exception("An Unhandled Exception Occurred on the Avalon Thread", e);
        }

        #endregion

    }
}
