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
    /// <summary>
    /// BVT that tests that in a Partial Trust Xapp, a Frame navigating to
    /// a) loose xaml specified as part of application resource (now succeeds)
    /// b) any file on the local system (in this case c:\windows\explorer.exe)
    /// fails with a SecurityException
    /// </summary>
    public partial class NavigationTests : Application
    {
        enum FrameLocalFileAccess_CurrentTest
        {
            UnInit,
            Init,
            FrameNav1,
            FrameNav2,
            End
        };

        #region FrameLocalFileAccess globals
        FrameLocalFileAccess_CurrentTest _frameLocalFileAccessTest = FrameLocalFileAccess_CurrentTest.UnInit;
        NavigationWindow _frameLocalFileAccess_NavWin = null;
        #endregion

        void FrameLocalFileAccess_Startup(object sender, StartupEventArgs e)
        {
            _frameLocalFileAccessTest = FrameLocalFileAccess_CurrentTest.Init;
            NavigationHelper.CreateLog("FrameLocalFileAccess");

            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(FrameLocalFileAccess_DispatcherException);
            NavigationHelper.SetStage(TestStage.Run);

            StartupUri = new Uri("FrameLocalFileAccess_myFrame.xaml", UriKind.RelativeOrAbsolute);
        }


        void FrameLocalFileAccess_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_frameLocalFileAccessTest)
            {
                case FrameLocalFileAccess_CurrentTest.Init:
                {
                    if (_frameLocalFileAccess_NavWin == null)
                    {
                        _frameLocalFileAccess_NavWin = Application.Current.MainWindow as NavigationWindow;                    
                        _frameLocalFileAccess_NavWin.ContentRendered += new EventHandler(FrameLocalFileAccess_ContentRendered);
                    }
                    break;
                }
            }
        }

        void FrameLocalFileAccess_ContentRendered(object sender, EventArgs e)
        {
            FrameLocalFileAccessExecNextState();
        }

        void FrameLocalFileAccess_LoadCompleted(object sender, NavigationEventArgs e)
        {
            FrameLocalFileAccessExecNextState();
        }

        Frame _f = null;

        void FrameLocalFileAccessExecNextState()
        {
            switch (_frameLocalFileAccessTest)
            {
                case FrameLocalFileAccess_CurrentTest.Init:
                    _frameLocalFileAccessTest = FrameLocalFileAccess_CurrentTest.FrameNav1;
                    NavigationHelper.Output("Navigating to explorer.exe");
                    _f = LogicalTreeHelper.FindLogicalNode(_frameLocalFileAccess_NavWin.Content as DependencyObject, "myFrame") as Frame;
                    _f.LoadCompleted += new LoadCompletedEventHandler(FrameLocalFileAccess_LoadCompleted);
                    _frameLocalFileAccess_NavWin.ContentRendered -= new EventHandler(FrameLocalFileAccess_ContentRendered);
                    _f.Navigate(new Uri(@"file:///c:/windows/explorer.exe", UriKind.RelativeOrAbsolute));
                    break;

                case FrameLocalFileAccess_CurrentTest.FrameNav1:
                    _frameLocalFileAccessTest = FrameLocalFileAccess_CurrentTest.FrameNav2;
                    NavigationHelper.Output("Navigating to loose xaml included with app");
                    _f.Navigate(new Uri(FRAMEPAGE, UriKind.RelativeOrAbsolute));
                    break;

                case FrameLocalFileAccess_CurrentTest.FrameNav2:
                    _frameLocalFileAccessTest = FrameLocalFileAccess_CurrentTest.End;
                    NavigationHelper.Output("Current Source = " + _f.Source);

                    if (_f.Source.ToString().Equals(FRAMEPAGE))
                    {
                        // navigation succeeded - test pass
                        ++_frameLocalFileAccess_passed;
                    }
                    else
                    {
                        ++_frameLocalFileAccess_failed;
                    }

                    NavigationHelper.Output("FrameLocalFileAccess_passed = " + _frameLocalFileAccess_passed + " FrameLocalFileAccess_failed = " + _frameLocalFileAccess_failed);
                    if (_frameLocalFileAccess_passed == 2 && _frameLocalFileAccess_failed == 0)
                    {
                        NavigationHelper.Pass("Frame local file access test FrameLocalFileAccess_passed");
                    }
                    else
                    {
                        NavigationHelper.Fail("Frame local file access test FrameLocalFileAccess_failed");
                    }
                    break;
            }

        }

        int _frameLocalFileAccess_passed = 0;
        int _frameLocalFileAccess_failed = 0;

        void FrameLocalFileAccess_DispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            switch (_frameLocalFileAccessTest)
            {
                case FrameLocalFileAccess_CurrentTest.FrameNav1:
                    // Since this is a local file path, we'll receive a webException wrapping a FileIO SecurityException
                    if (e.Exception is System.Net.WebException)
                    {
                        // test FrameLocalFileAccess_passed						
                        ++_frameLocalFileAccess_passed;
                        e.Handled = true;
                        NavigationHelper.Output("WebException correctly thrown");
                        FrameLocalFileAccessExecNextState();
                    }
                    else
                    {
                        // test FrameLocalFileAccess_failed	
                        NavigationHelper.Output("WebException was expected but not thrown");
                        ++_frameLocalFileAccess_failed;
                    }
                    break;

                default:
                    NavigationHelper.Output("Should not have gotten to this point");
                    ++_frameLocalFileAccess_failed;
                    break;
            }
        }

    }
}
