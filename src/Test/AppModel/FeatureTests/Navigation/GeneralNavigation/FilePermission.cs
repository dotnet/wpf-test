// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using System.Net;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT that tests that in a Partial Trust Xapp
    /// trying to get to a local file through webrequest
    /// fails with a WebException / SecurityException
    /// and navigation window navigating to 
    /// a loose xaml file specified as an absolute uri (in pack uri notation)
    /// succeeds
    /// </summary>
    public partial class NavigationTests : Application
    {
        NavigationWindow _filePermission_navWin = null;

        enum FilePermission_State
        {
            UnInit,
            Init,
            Nav1,
            End
        };

        FilePermission_State _filePermission_currState = FilePermission_State.UnInit;

        void FilePermission_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.SetStage(TestStage.Initialize);
            _filePermission_currState = FilePermission_State.Init;
            if (Log.Current == null)
                new TestLog("FilePermission");
            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherException_FilePermission_Local);
            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("FilePermission_Page1.xaml", UriKind.RelativeOrAbsolute);
        }


        void FilePermission_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_filePermission_currState)
            {
                case FilePermission_State.Init:
                    _filePermission_navWin = this.MainWindow as NavigationWindow;
                    _filePermission_navWin.ContentRendered += new EventHandler(FilePermission_ContentRendered_NavWin);
                    break;
            }
        }

        void FilePermission_ContentRendered_NavWin(object sender, EventArgs e)
        {
            FilePermission_ExecNextState();
        }

        void FilePermission_ExecNextState()
        {
            switch (_filePermission_currState)
            {
                case FilePermission_State.Init:
                    _filePermission_currState = FilePermission_State.Nav1;
                    NavigationHelper.Output("Navigating to explorer.exe");
                    WebRequest request = WebRequest.Create(@"c:\windows\explorer.exe");
                    try
                    {
                        WebResponse response = request.GetResponse();
                        // test failed
                        _filePermission_navWin.Navigate("len = " + response.ContentLength);
                        NavigationHelper.Fail("FilePermission fails");
                    }
                    catch (System.Net.WebException webEx)
                    {
                        // test passed
                        NavigationHelper.Output("Got webexception on trying to navigate to explorer.exe. Test passed\n" + webEx.ToString());
                        FilePermission_ExecNextState();
                    }
                    break;

                case FilePermission_State.Nav1:
                    _filePermission_currState = FilePermission_State.End;
                    NavigationHelper.Output("Navigating to pack://application:,,,/FilePermission_Loose.xaml");
                    _filePermission_navWin.Navigate(new Uri(@"pack://application:,,,/FilePermission_Loose.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case FilePermission_State.End:
                    NavigationHelper.Pass("FilePermission passes");
                    NavigationHelper.SetStage(TestStage.Cleanup);
                    Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
                    break;
            }
        }

        void OnDispatcherException_FilePermission_Local(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            switch (_filePermission_currState)
            {
                case FilePermission_State.End:
                    if (e.Exception is System.Security.SecurityException)
                    {
                        // test passed						
                        e.Handled = true;
                        FilePermission_ExecNextState();
                    }
                    else
                    {
                        NavigationHelper.Fail("FilePermission fails");
                    }
                    break;

                default:
                    NavigationHelper.Fail("FilePermission fails");
                    break;
            }
        }

    }
}
