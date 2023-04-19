// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    // GoBackUriToPage (JournalGoBackUriToPageApp)
    public partial class NavigationTests : Application
    {
        internal enum GoBackUriToPage_CurrentTest
        {
            InitialNav, 
            Navigated,
            CalledGoBack
        }

        GoBackUriToPage_CurrentTest _goBackUriToPageTest = GoBackUriToPage_CurrentTest.InitialNav;

        void GoBackUriToPage_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("GoBackUriToPage");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 7;
            NavigationHelper.ActualTestCount = 0;

            NavigationHelper.ExpectedFileName = CONTROLSPAGE;
            
            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }
        
        void GoBackUriToPage_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void GoBackUriToPage_LoadCompleted(object source, NavigationEventArgs e)
        {
            //bool wentBack;

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
                switch(_goBackUriToPageTest)
                {
                    case GoBackUriToPage_CurrentTest.InitialNav : 
                            _goBackUriToPageTest = GoBackUriToPage_CurrentTest.Navigated;
                            NavigationHelper.Output("Calling Navigate on MyOtherNewPage canvas object");
                            nw.Navigate(new MyOtherNewPage());
                            break;
    
                    case GoBackUriToPage_CurrentTest.Navigated : 
                            _goBackUriToPageTest = GoBackUriToPage_CurrentTest.CalledGoBack;
                            NavigationHelper.Output("Calling GoBack on MyOtherNewPage canvas object");
                            // wentBack = nw.GoBack();
                            nw.GoBack();
                            // NavigationHelper.Output("wentBack is " + wentBack);
                            NavigationHelper.Output("Called GoBack (does not return bool - it will throw an exception if it breaks...)");
                            break;
    
                    case GoBackUriToPage_CurrentTest.CalledGoBack : 
                            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                            break;
                }
    
            }
        }
    }    
}
