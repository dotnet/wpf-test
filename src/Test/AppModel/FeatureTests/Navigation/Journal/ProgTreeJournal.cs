// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // ProgTreeJournal
    public partial class NavigationTests : Application
    {
       internal enum ProgTreeJournal_State
        {
            InitialNav, 
            NavigatedToPage,
            NavigatedToXaml,
            CalledGoBack,
            CalledGoBack2,
            CalledGoForward,
            CalledGoForward2
        }

        ProgTreeJournal_State _progTreeJournalTest = ProgTreeJournal_State.InitialNav;

        void ProgTreeJournal_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("ProgTreeJournal");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 25;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = FLOWDOCPAGE;

            NavigationHelper.SetStage(TestStage.Run);
        }

        void ProgTreeJournal_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
            else if (e.NavigationMode == NavigationMode.Forward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void ProgTreeJournal_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void ProgTreeJournal_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            if (e.Uri == null)
            {
                NavigationHelper.Output("no uri here");
            }
            else 
            {
                NavigationHelper.Output("uri is: " + e.Uri.ToString());
                NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
            }

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_progTreeJournalTest)
                {
                    case ProgTreeJournal_State.InitialNav:
                        _progTreeJournalTest = ProgTreeJournal_State.NavigatedToPage;
                        NavigationHelper.Output("Calling Navigate on MyOtherNewPage ");
                        nw.Navigate(new MyOtherNewPage());
                        break;

                    case ProgTreeJournal_State.NavigatedToPage:
                        _progTreeJournalTest = ProgTreeJournal_State.NavigatedToXaml;
                        NavigationHelper.Output("Calling Navigate on " + FLOWDOCPAGE);
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case ProgTreeJournal_State.NavigatedToXaml:
                        _progTreeJournalTest = ProgTreeJournal_State.CalledGoBack;
                        NavigationHelper.Output("Calling GoBack on MyOtherNewPage canvas");
                        nw.GoBack();
                        NavigationHelper.Output("Just called GoBack");
                        break;

                    case ProgTreeJournal_State.CalledGoBack:
                        _progTreeJournalTest = ProgTreeJournal_State.CalledGoBack2;
                        NavigationHelper.Output("Calling GoBack on " + CONTROLSPAGE);
                        nw.GoBack();
                        NavigationHelper.Output("Just called GoBack on " + CONTROLSPAGE);
                        break;

                    case ProgTreeJournal_State.CalledGoBack2:
                        _progTreeJournalTest = ProgTreeJournal_State.CalledGoForward;
                        NavigationHelper.Output("Calling GoForward on MyOtherNewPage canvas");
                        nw.GoForward();
                        NavigationHelper.Output("Just called GoForward on MyOtherNewPage");
                        break;

                    case ProgTreeJournal_State.CalledGoForward:
                        _progTreeJournalTest = ProgTreeJournal_State.CalledGoForward2;
                        NavigationHelper.Output("Calling GoForward on " + FLOWDOCPAGE);
                        nw.GoForward();
                        NavigationHelper.Output("Just called GoForward on " + FLOWDOCPAGE);
                        break;

                    case ProgTreeJournal_State.CalledGoForward2:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }
    
            }
            else 
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }
    }

    public class MyOtherNewPage : Canvas
    {
        public MyOtherNewPage() : base()
        {
            Microsoft.Test.Logging.LogManager.LogMessageDangerously("Inside MyOtherNewPage constructor");
            this.Background = Brushes.DarkBlue;
            Button b = new Button();
            b.Content = "I am a button on MyOtherNewPage";
            this.Children.Add(b);

            Microsoft.Test.Logging.LogManager.LogMessageDangerously("Exiting MyOtherNewPage constructor");
        }
    }
}
