// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  ClearForwardStack tests that the forward stack is emptied
//                when we navigate to a second page, go back to the initial page
//                and then navigate to a new third page.  This test verifies
//                that we receive the correct number of events during the
//                navigations, that we end up at the correct location and that
//                the forward stack has the correct contents when we nav fwd/back.
//

using System;
using System.Collections;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;       // TestLog, TestStage

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // ClearForwardStack
    public partial class NavigationTests : Application
    {
        internal enum ClearForwardStack_CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack,
            NavigateNew
        }

        #region ClearForwardStack globals
        ClearForwardStack_CurrentTest _curState = ClearForwardStack_CurrentTest.InitialNav;
        #endregion

        void ClearForwardStack_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("ClearForwardStack");

            NavigationHelper.ExpectedTestCount = 15;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = IMAGEPAGE;

            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.Output("Registering application-level eventhandlers.");
            Application.Current.Navigating += new NavigatingCancelEventHandler(ClearForwardStack_Navigating);
            Application.Current.Navigated += new NavigatedEventHandler(ClearForwardStack_Navigated);
            //Application.Current.LoadCompleted += new LoadCompletedEventHandler(ClearForwardStack_LoadCompleted);

            NavigationHelper.SetStage(TestStage.Run);
        }


        void ClearForwardStack_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Forward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void ClearForwardStack_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void ClearForwardStack_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_curState)
                {
                    case ClearForwardStack_CurrentTest.InitialNav:
                        _curState = ClearForwardStack_CurrentTest.Navigated;
                        NavigationHelper.Output("Calling Navigate on " + FLOWDOCPAGE);
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case ClearForwardStack_CurrentTest.Navigated:
                        _curState = ClearForwardStack_CurrentTest.CalledGoBack;
                        NavigationHelper.Output("Calling GoBack from NavigationWindow");
                        nw.GoBack();
                        break;

                    case ClearForwardStack_CurrentTest.CalledGoBack:
                        _curState = ClearForwardStack_CurrentTest.NavigateNew;
                        if (VerifyJournalEntries(nw, 1))
                        {
                            NavigationHelper.Output("Verified 1 entry in the Forward Stack");
                            NavigationHelper.ActualTestCount++;
                        }
                        else
                        {
                            NavigationHelper.Fail("Wrong number of entries in the Forward Stack");
                        }
                        NavigationHelper.Output("Calling Navigate on " + IMAGEPAGE);
                        nw.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case ClearForwardStack_CurrentTest.NavigateNew:
                        if (VerifyJournalEntries(nw, 0))
                        {
                            NavigationHelper.Output("Verified no entries in the Forward Stack");
                            NavigationHelper.ActualTestCount++;
                        }
                        else
                        {
                            NavigationHelper.Fail("Wrong number of entries in the Forward Stack");
                        }
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }
            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        private bool VerifyJournalEntries(NavigationWindow nw, int expectedEntries)
        {
            int counter = 0;

            if (nw != null)
            {
                IEnumerator journalEnum = ((IEnumerable)nw.GetValue(NavigationWindow.ForwardStackProperty)).GetEnumerator();

                while (journalEnum.MoveNext())
                    counter++;

                NavigationHelper.Output("EXPECTED: ForwardStack has " + expectedEntries + " entries");
                NavigationHelper.Output("ACTUAL: ForwardStack has " + counter + " entries");

                if (counter == expectedEntries)
                    return true;
                else
                    return false;
            }
            else
            {
                NavigationHelper.Output("NavigationWindow nw passed in is NULL");
                return false;
            }
        }
    }
}

