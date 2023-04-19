// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Deployment.Application;
using System.Deployment;
using System.Windows.Interop;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Microsoft.Test.WPF.AppModel.Deployment
{
    public partial class BasicInternetXbap
    {
        public static string activationUri;
        public static RoutedCommand CustomRoutedCommand1 = new RoutedCommand();
        public static RoutedCommand CustomRoutedCommand2 = new RoutedCommand();

        #region InputGesture Handlers

        public void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            mainPage.WindowTitle = "Got Save Command...";
            e.Handled = true;
        }

        public void ExecutedCustomCommand1(object sender, ExecutedRoutedEventArgs e)
        {
            inputGestureIndicator.Text = "Alt-V Captured from Binding!";
            e.Handled = true;
        }

        public void ExecutedCustomCommand2(object sender, ExecutedRoutedEventArgs e)
        {
            inputGestureIndicator.Text = "Ctrl-F Captured!";
            e.Handled = true;
        }

        //CanExecuteRoutedEventHandler that always lets the command execute
        public void CanExecuteCustomCommand(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region ExitEvent Handler

        void WriteExitCookie(object sender, ExitEventArgs e)
        {
            try
            {
                // Sleep just a bit to simulate an app that does a significant amount of stuff in its Exit handler
                Thread.Sleep(1000);
                string myDeploymentDir = BasicInternetXbap.activationUri;

                DateTime dt = DateTime.UtcNow;
                dt = dt.AddYears(1);
                string theCookie = "App ShutDown was successful; expires=" + dt.ToString("r");
                System.Windows.Application.SetCookie(new System.Uri(myDeploymentDir), theCookie);
            }
            catch (System.InvalidOperationException)
            {
                // Do nothing... this happens when the app is in a frame since Cookies are not allowed then.
            }
        }

        #endregion

        #region Isolated Storage Methods

        bool ReadIsoStorage()
        {
            string lastDateTime = "";
            
            using (IsolatedStorageFile isoLastState = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamReader stateReader = new StreamReader(new IsolatedStorageFileStream("ExpressAppState.txt", FileMode.OpenOrCreate, isoLastState)))
                {
                    //Open the isolated storage from the previous instance and read the data
                    if (stateReader == null)
                    {
                        lastDateTime = "Failed to Read Stored Data.";
                    }
                    else
                    {
                        if ((lastDateTime = stateReader.ReadToEnd()) == "")
                        {
                            isoIndicator.Text = "Wrote to Isolated Storage";
                            return false;
                        }
                        else if (lastDateTime.StartsWith("Express App Isolated Storage Succeeded"))
                        {
                            isoIndicator.Text = "Passed: Read Isolated Storage";
                        }
                        else
                        {
                            isoIndicator.Text = "Failed: Read Isolated Storage";
                        }
                    }
                    stateReader.Close();
                    
                }
            }
            return true;
        }

        void WriteIsoStorage()
        {
            using (IsolatedStorageFile isoLastState = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (StreamWriter stateWriter = new StreamWriter(new IsolatedStorageFileStream("ExpressAppState.txt", FileMode.OpenOrCreate, FileAccess.Write, isoLastState)))
                {
                    stateWriter.WriteLine("Express App Isolated Storage Succeeded");
                    stateWriter.WriteLine("The Current time is " + DateTime.Now.ToString());
                    stateWriter.WriteLine("**************************************************************");
                    stateWriter.Flush();
                    stateWriter.Close();
                }
            }
        }

        #endregion

        #region Test Stuff

        void ResizeRootBrowserWindowTest(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Width = 1699;
            Application.Current.MainWindow.Height = 1299;
            Application.Current.MainWindow.Width = 1700;
            Application.Current.MainWindow.Height = 1300;
        }

        void NavigationUserAgentStringTest(object sender, EventArgs e)
        {
            Application.Current.Navigating += new NavigatingCancelEventHandler(Application_Navigating);

            string absoluteUri = BrowserInteropHelper.Source.AbsoluteUri;
            // This file isnt even AT this URI, I'm just doing this to get the WebRequest, and its UA string.
            absoluteUri = absoluteUri.Remove(absoluteUri.LastIndexOf("/")) + "/Deploy_Markup1.xaml";

            ((NavigationWindow)Application.Current.MainWindow).Navigate(new Uri(absoluteUri, UriKind.Absolute));
        }

        private void Application_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.WebRequest.GetType() == typeof(HttpWebRequest))
            {
                Application.Current.MainWindow.Title = ((HttpWebRequest)e.WebRequest).UserAgent;
                userAgentStringTxt.Text = ((HttpWebRequest)e.WebRequest).UserAgent;
                e.Cancel = true;
            }
        }

        void DebugSOOTest(object sender, EventArgs e)
        {
            SOODebugFrame.Source = new Uri("pack://siteoforigin:,,,/Deploy_Markup2.xaml");
        }

        void CancelAllNavigations(object sender, EventArgs e)
        {
            NavigationWindow nw = (NavigationWindow)base.Parent;
            nw.Navigating += new NavigatingCancelEventHandler(this.CancelNavigation);
        }

        void CancelNavigation(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = true;
        }

        void CheckURLParams(object sender, EventArgs e)
        {
            CommandBinding cb = new CommandBinding(ApplicationCommands.Save, this.SaveCmdExecuted);
            Application.Current.MainWindow.CommandBindings.Add(cb);

            try
            {
                string actUri = System.Windows.Interop.BrowserInteropHelper.Source.ToString();

                if (actUri.IndexOf("?") >= 0)
                {
                    urlParamBtn.Text = actUri.Substring(actUri.IndexOf("?") + 1);
                    actUriTxt.Text = actUri.Substring(0, actUri.IndexOf("?"));
                }
                else
                {
                    actUriTxt.Text = actUri;
                }
                activationUri = actUri;

            }
            catch (Exception excep)
            {
                // Do nothing... for when there are no URL params.
                urlParamBtn.Text = excep.Message;
            }

            Application.Current.Exit += new ExitEventHandler(this.WriteExitCookie);

            try
            {
                appConfigIndicator.Text = Properties.Settings.Default.aUserProperty + Properties.Settings.Default.anAppProperty;
                if (!ReadIsoStorage())
                {
                    WriteIsoStorage();
                }
            }
            catch (Exception excep)
            {
                // Do nothing... for when there are no URL params.
                isoIndicator.Text = "ERROR with Iso Storage or App.config" + excep.Message;
            }


        }

        void InfiniteLoop(object sender, EventArgs e)
        {
            loopIndicator.Text = "Looping forever...";
            while (true != false)
            {
                Thread.Sleep(150);
            }
        }

        void testSecurity(object sender, EventArgs e)
        {
            // Try doing things that invoke the levels.  Start with local, then try intranet, then internet.
            mainPage.WindowTitle = "Testing Security Zone ...";

            // Local case: 
            try
            {
                new System.Security.Permissions.SecurityPermission(System.Security.Permissions.PermissionState.Unrestricted).Demand();
                stopLight.Fill = new RadialGradientBrush(Colors.LightGreen, Colors.Green);
                permResult.Text = "Full Trust Granted";
                return;
            }
            catch
            {
                try
                {
                    // Demands the complete set of Intranet Zone permissions as of 1/26/05
                    new System.Security.Permissions.EnvironmentPermission(EnvironmentPermissionAccess.Read, "USERNAME").Demand();
                    new System.Security.Permissions.FileDialogPermission(PermissionState.Unrestricted).Demand();

                    IsolatedStorageFilePermission isfp = new System.Security.Permissions.IsolatedStorageFilePermission(PermissionState.None);
                    isfp.UsageAllowed = IsolatedStorageContainment.AssemblyIsolationByUser;
                    isfp.UserQuota = 9223372036854775807;
                    isfp.Demand();

//todo: May need to delete this entirely.  For now, we'll just block the warning.
#pragma warning disable 0618
                    new System.Security.Permissions.ReflectionPermission(ReflectionPermissionFlag.ReflectionEmit).Demand();
#pragma warning restore 0618

                    new System.Security.Permissions.SecurityPermission(SecurityPermissionFlag.Assertion | SecurityPermissionFlag.Execution | SecurityPermissionFlag.BindingRedirects).Demand();
                    new System.Security.Permissions.UIPermission(PermissionState.Unrestricted).Demand();
                    new System.Net.DnsPermission(PermissionState.Unrestricted).Demand();
                    new System.Drawing.Printing.PrintingPermission(System.Drawing.Printing.PrintingPermissionLevel.DefaultPrinting).Demand();

                    stopLight.Fill = new RadialGradientBrush(Colors.LightGreen, Colors.Green);
                    permResult.Text = "Partial Trust (Intranet) Granted";
                }
                catch
                {
                    try
                    {
                        // Demands the complete set of Internet Zone permissions as of 1/26/05
                        new System.Security.Permissions.FileDialogPermission(FileDialogPermissionAccess.Open).Demand();

                        IsolatedStorageFilePermission isfp2 = new System.Security.Permissions.IsolatedStorageFilePermission(PermissionState.None);
                        isfp2.UsageAllowed = IsolatedStorageContainment.DomainIsolationByUser;
                        isfp2.UserQuota = 10240;
                        isfp2.Demand();

                        new System.Security.Permissions.SecurityPermission(SecurityPermissionFlag.Execution).Demand();
                        new System.Security.Permissions.UIPermission(UIPermissionWindow.SafeTopLevelWindows, UIPermissionClipboard.OwnClipboard).Demand();

                        stopLight.Fill = new RadialGradientBrush(Colors.LightGreen, Colors.Green);
                        permResult.Text = "Partial Trust (Internet) Granted";
                    }
                    catch
                    {
                        stopLight.Fill = new RadialGradientBrush(Colors.Red, Colors.DarkRed);
                        permResult.Text = "No Trust Granted";
                    }
                }
            }
        }
        #endregion
    }
}
