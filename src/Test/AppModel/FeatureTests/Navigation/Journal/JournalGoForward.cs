// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // JournalGoForwardApp
    public partial class NavigationTests : Application
    {
        internal enum JournalGoForward_CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack,
            CalledGoForward
        }

        JournalGoForward_CurrentTest _journalGoFwdTest = JournalGoForward_CurrentTest.InitialNav;

        void JournalGoForward_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("JournalGoForward");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 9;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = FLOWDOCPAGE;

            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }


        public void JournalGoForward_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Forward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        public void JournalGoForward_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = this.Windows[0] as NavigationWindow;
            if (nw != null)
            {
                switch (_journalGoFwdTest)
                {
                    case JournalGoForward_CurrentTest.InitialNav:
                        _journalGoFwdTest = JournalGoForward_CurrentTest.Navigated;
                        NavigationHelper.Output("Calling Navigate on " + FLOWDOCPAGE);
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case JournalGoForward_CurrentTest.Navigated:
                        _journalGoFwdTest = JournalGoForward_CurrentTest.CalledGoBack;
                        nw.GoBack();
                        break;

                    case JournalGoForward_CurrentTest.CalledGoBack:
                        _journalGoFwdTest = JournalGoForward_CurrentTest.CalledGoForward;
                        nw.GoForward();
                        break;

                    case JournalGoForward_CurrentTest.CalledGoForward:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
        }
    }
}
