// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Regression test
    /// </summary>
    public class JournalingTest
    {
        /// <summary>
        /// keep track of what step we are performing
        /// </summary>
        private static int s_step = 0;

        /// <summary>
        /// Journaling 
        /// causes the content to be saved when going
        /// back /forward.
        /// </summary>
        public static void RegressionIssue142()
        {
            Thread thread = new Thread(new ThreadStart(UiThread));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            // Passed if the navigation did not throw 
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Run the window in a separate thread
        /// </summary>
        public static void UiThread()
        {
            NavigationWindow navigationWindow = new NavigationWindow();
            Application app = new Application();
            navigationWindow.Show();

            navigationWindow.Navigate(new Page()
            {
                Title = "Sample Page1",
                Content = new DockPanel()
                {
                    Children =
                    {
                        new Label() { Content = "Hello" },
                    },
                },
            });
            navigationWindow.Navigated += new NavigatedEventHandler(Navigated);

            app.Run(navigationWindow);
        }

        /// <summary>
        /// On navigation complete do the next step.
        /// </summary>
        /// <param name="sender">the navigation window</param>
        /// <param name="e">event arguments</param>
        private static void Navigated(object sender, NavigationEventArgs e)
        {
            GlobalLog.LogStatus("Navigated..");
            NavigationWindow navigationWindow = sender as NavigationWindow;
            switch (++s_step)
            {
                case 1:
                    GlobalLog.LogStatus("Navigating to Page2");
                    navigationWindow.Navigate(new Page()
                    {
                        Title = "Sample Page2",
                        Content = new DockPanel()
                        {
                            Children =
                            {
                                new Label() { Content = "World" },
                            },
                        },
                    });
                    break;

                case 2:
                    GlobalLog.LogStatus("Navigating Back");
                    navigationWindow.GoBack();
                    break;

                case 3:
                    GlobalLog.LogStatus("Navigating Forward");
                    navigationWindow.GoForward();
                    break;

                case 4:
                    GlobalLog.LogStatus("Closing..");
                    navigationWindow.Close();
                    break;

                default:
                    throw new InvalidOperationException("Should not have reached here");
            }
        }
    }
}
