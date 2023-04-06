// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // FrameNavigated (NavigationFrameNaved)
    public partial class NavigationTests : Application
    {
        internal enum FrameNavigated_CurrentTest
        {
            InitialNav,
            Navigated
        }

        FrameNavigated_CurrentTest _frameNavigatedTest = FrameNavigated_CurrentTest.InitialNav;

        void FrameNavigated_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("FrameNavigated");

            NavigationHelper.ExpectedTestCount = 4;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = FLOWDOCPAGE;

            NavigationHelper.Output("Inside OnStartup()");

            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }


        void FrameNavigated_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void FrameNavigated_LoadCompleted(object source, NavigationEventArgs e)
        {
            Frame f;
            FrameworkElement el;

            NavigationHelper.Output("LoadCompleted event fired ON APPLICATION");
            NavigationHelper.ActualTestCount++;
            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                el = nw.Content as FrameworkElement;
                if (el != null)
                {
                    f = LogicalTreeHelper.FindLogicalNode(el, "testFrame") as Frame;
                    if (f != null)
                    {
                        switch (_frameNavigatedTest)
                        {
                            case FrameNavigated_CurrentTest.InitialNav:
                                if ((e.IsNavigationInitiator) && (NavigationHelper.getFileName(e.Uri) == "a.xaml"))
                                {
                                    f.Navigated += new NavigatedEventHandler(FrameNavigated_Navigated);

                                    _frameNavigatedTest = FrameNavigated_CurrentTest.Navigated;
                                    NavigationHelper.Output("Calling Navigate on "+ FLOWDOCPAGE);
                                    f.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                                }
                                break;

                            case FrameNavigated_CurrentTest.Navigated:
                                NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                                break;
                        }
                    }
                }
            }
        }
    }
}
