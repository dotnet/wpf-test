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

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // HlinkLocalFileAccess
    /// <summary>
    /// BVT that tests that in a Partial Trust Xapp, a Hyperlink 
    /// raising a RequestNavigateEvent with NavigateUri pointing to
    /// a) loose xaml specified as part of application resource succeeds
    /// b) any file on the local system (in this case c:\windows\explorer.exe)
    /// fails with a SecurityException
    /// </summary>
    public partial class NavigationTests : Application
    {
        internal enum HlinkLocalFileAccess_CurrentTest
        {
            UnInit, 
            Init, 
            HlinkNav1,
            HlinkNav2, 
            End
        }

        #region HlinkLocalFileAccess globals
        HlinkLocalFileAccess_CurrentTest _hlinkLocalFileAccessTest = HlinkLocalFileAccess_CurrentTest.UnInit;
        NavigationWindow _hlinkLocalFileAccess_NavWin = null;
        #endregion

        void HlinkLocalFileAccess_Startup(object sender, StartupEventArgs e)
        {
            _hlinkLocalFileAccessTest = HlinkLocalFileAccess_CurrentTest.Init;
            NavigationHelper.CreateLog("HyperlinkLocalFileAccess");

            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HlinkLocalFileAccess_DispatcherException);
            NavigationHelper.SetStage(TestStage.Run);

            StartupUri = new Uri("HyperlinkLocalFileAccess_hyperlink.xaml", UriKind.RelativeOrAbsolute);

        }

        void HlinkLocalFileAccess_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_hlinkLocalFileAccessTest)
            {
                case HlinkLocalFileAccess_CurrentTest.Init:
                    _hlinkLocalFileAccess_NavWin = Application.Current.MainWindow as NavigationWindow;
                    _hlinkLocalFileAccess_NavWin.ContentRendered += new EventHandler(HlinkLocalFileAccess_ContentRendered);
                    break;
            }
        }

        void HlinkLocalFileAccess_ContentRendered(object sender, EventArgs e)
        {
            ExecNextState();
        }

        void ExecNextState()
        {
            RequestNavigateEventArgs rne;
            switch (_hlinkLocalFileAccessTest)
            {
                case HlinkLocalFileAccess_CurrentTest.Init:
                    _hlinkLocalFileAccessTest = HlinkLocalFileAccess_CurrentTest.HlinkNav1;
                    NavigationHelper.Output("Hyperlink to loose xaml included with app");
                    Hyperlink looseXaml = LogicalTreeHelper.FindLogicalNode(_hlinkLocalFileAccess_NavWin.Content as DependencyObject, "looseXaml") as Hyperlink;
                    rne = new RequestNavigateEventArgs(looseXaml.NavigateUri, null);
                    rne.Source = looseXaml;
                    looseXaml.RaiseEvent(rne);
                    break;

                case HlinkLocalFileAccess_CurrentTest.HlinkNav1:
                    _hlinkLocalFileAccessTest = HlinkLocalFileAccess_CurrentTest.HlinkNav2;
                    NavigationHelper.Output("Hyperlink to explorer.exe");
                    Hyperlink localExe = LogicalTreeHelper.FindLogicalNode(_hlinkLocalFileAccess_NavWin.Content as DependencyObject, "localExe") as Hyperlink;
                    rne = new RequestNavigateEventArgs(localExe.NavigateUri, null);
                    rne.Source = localExe;
                    localExe.RaiseEvent(rne);
                    break;

                case HlinkLocalFileAccess_CurrentTest.HlinkNav2:
                    _hlinkLocalFileAccessTest = HlinkLocalFileAccess_CurrentTest.End;
                    NavigationHelper.Output("passed = " + _passed + " failed = " + _failed);
                    if (_passed == 1 && _failed == 0)
                    {
                        NavigationHelper.Pass("Hyperlink local file access test passed");
                    }
                    else
                    {
                        NavigationHelper.Fail("Hyperlink local file access test failed");
                    }
                    break;

            }

        }

        int _passed = 0;
        int _failed = 0;

        void HlinkLocalFileAccess_DispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            switch (_hlinkLocalFileAccessTest)
            {
                case HlinkLocalFileAccess_CurrentTest.HlinkNav2:
                    // Since this is a local file path, we'll receive a webException wrapping a FileIO SecurityException
                    if (e.Exception is System.Net.WebException)
                    {
                        // test passed
                        ++_passed;
                        e.Handled = true;
                        NavigationHelper.Output("WebException correctly thrown");
                        ExecNextState();
                    }
                    else
                    {
                        // test failed
                        NavigationHelper.Output("WebException was expected but not thrown");
                        ++_failed;
                    }
                    break;
                default:
                    NavigationHelper.Output("Should not have gotten to this point");
                    ++_failed;
                    break;
            }
        }

    }
}
