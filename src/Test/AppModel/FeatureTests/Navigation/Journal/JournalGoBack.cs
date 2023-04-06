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
    // JournalGoBackApp
    public partial class NavigationTests : Application
    {
        internal enum JournalGoBack_CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack
        }

        JournalGoBack_CurrentTest _journalGoBackTest = JournalGoBack_CurrentTest.InitialNav;

        void JournalGoBack_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("JournalGoBack");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 7;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = CONTROLSPAGE;

            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }

        public void JournalGoBack_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigateionMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        public void JournalGoBack_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_journalGoBackTest)
                {
                    case JournalGoBack_CurrentTest.InitialNav:
                        _journalGoBackTest = JournalGoBack_CurrentTest.Navigated;
                        NavigationHelper.Output("Calling Navigate on " + FLOWDOCPAGE);
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case JournalGoBack_CurrentTest.Navigated:
                        _journalGoBackTest = JournalGoBack_CurrentTest.CalledGoBack;
                        nw.GoBack();
                        break;

                    case JournalGoBack_CurrentTest.CalledGoBack:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
        }

    }
}
