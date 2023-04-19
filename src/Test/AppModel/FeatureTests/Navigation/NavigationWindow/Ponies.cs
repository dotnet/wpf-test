// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Test.Logging;                       // TestLog, TestStage

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    //  Ponies
    public partial class NavigationTests : Application
    {
        internal enum Ponies_CurrentTest
        {
            InitialNav, 
            NavigatedToSecondPage,
            NavigatedToThirdPage,
            CalledGoBack,
            CalledGoForward,
            CalledGoForward2
        }

        Ponies_CurrentTest _ponies_curState = Ponies_CurrentTest.InitialNav;

        void Ponies_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("Ponies [RequestNavigateEventArgs]");
            Application.Current.StartupUri = new Uri("Ponies_Page1.xaml", UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 15;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"Ponies_Page3.xaml";

            NavigationHelper.Output("Registering application-level eventhandlers");
            Application.Current.Navigating += new NavigatingCancelEventHandler(Ponies_Navigating);
            Application.Current.Navigated += new NavigatedEventHandler(Ponies_Navigated);
            //Application.Current.LoadCompleted += new LoadCompletedEventHandler(Ponies_LoadCompleted);

            NavigationHelper.SetStage(TestStage.Run);
        }
        
        void Ponies_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
        }

        void Ponies_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void Ponies_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

			Hyperlink h;
			RequestNavigateEventArgs requestNavigateEventArgs;

            if (e.Uri == null)
            {
                NavigationHelper.Output("No uri. e.Content is = " + e.Content.ToString());
            }
            else 
            {
                NavigationHelper.Output("uri is: " + e.Uri.ToString());
                NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
            }

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_ponies_curState)
                {
                    case Ponies_CurrentTest.InitialNav:
                        _ponies_curState = Ponies_CurrentTest.NavigatedToSecondPage;
                        NavigationHelper.Output("Calling Navigate on Ponies_secondPage.xaml ");
                        nw.Navigate(new Uri("Ponies_secondPage.xaml", UriKind.RelativeOrAbsolute));
                        break;

                    case Ponies_CurrentTest.NavigatedToSecondPage:
                        _ponies_curState = Ponies_CurrentTest.NavigatedToThirdPage;
                        // get Hyperlink from MyOtherNewPage
                        h = LogicalTreeHelper.FindLogicalNode(nw, "hyperlink1") as Hyperlink;
                        // create RequestNavigateEventArgs here & execute them on my Hyperlink
                        requestNavigateEventArgs = new RequestNavigateEventArgs(h.NavigateUri, null);
                        requestNavigateEventArgs.Source=h;
                        h.RaiseEvent(requestNavigateEventArgs);
                        break;

                    case Ponies_CurrentTest.NavigatedToThirdPage:
                        _ponies_curState = Ponies_CurrentTest.CalledGoBack;
                        NavigationHelper.Output("Calling GoBack on Ponies_secondPage");
                        nw.GoBack();
                        NavigationHelper.Output("Just called GoBack");
                        break;

                    case Ponies_CurrentTest.CalledGoBack:
                        _ponies_curState = Ponies_CurrentTest.CalledGoForward;
                        // check RequestNavigateEventArgs
                        h = LogicalTreeHelper.FindLogicalNode(nw, "hyperlink1") as Hyperlink;
                        requestNavigateEventArgs = new RequestNavigateEventArgs(h.NavigateUri, null);
                        requestNavigateEventArgs.Source=h;
                        Ponies_CheckRNEventArgs(requestNavigateEventArgs);
                        NavigationHelper.Output("Calling GoForward on Ponies_page3.xaml");
                        nw.GoForward();
                        NavigationHelper.Output("Just called GoForward on Ponies_page3.xaml: ");
                        break;

                    case Ponies_CurrentTest.CalledGoForward: 
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }
    
            }
            else 
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        public void Ponies_CheckRNEventArgs(RequestNavigateEventArgs e)
        {
        	// Get Target string prop
        	if (e.Target != null)
        		NavigationHelper.Output("RequestNavigateEventArgs Target is: " + e.Target);
        	else 
        		NavigationHelper.Output("RequestNavigateEventArgs.Target is null");

        	// Get uri Uri prop
        	if (e.Uri != null)
        		NavigationHelper.Output("RequestNavigateEventArgs Uri is: " + e.Uri.ToString());
        	else 
        		NavigationHelper.Output("RequestNavigateEventArgs.Uri is null");
        }
    }
}

