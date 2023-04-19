// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
//using AvalonTools.FrameworkUtils;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum Navigating_State
        {
            InitialNav, 
            Navigated
        }

        Navigating_State _navigating_curState = Navigating_State.InitialNav;

        void Navigating_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.ExpectedTestCount = 4;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"Navigating_Page2.xaml";
            
            Log.Current.CurrentVariation.LogMessage("Inside OnStartup()");
            this.StartupUri = new Uri(@"Navigating_Page1.xaml",UriKind.RelativeOrAbsolute);
        }

        public void Navigating_Navigating(object source, NavigatingCancelEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("Navigating event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void Navigating_LoadCompleted(object source, NavigationEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            Log.Current.CurrentVariation.LogMessage("uri is: " + e.Uri.ToString());
            Log.Current.CurrentVariation.LogMessage("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = this.Windows[0] as NavigationWindow;
            if (nw != null)
            {
                switch(_navigating_curState)
                {
                    case Navigating_State.InitialNav: 
                            _navigating_curState = Navigating_State.Navigated;
                            Log.Current.CurrentVariation.LogMessage("Calling Navigate on Navigating_Page2.xaml");
                            nw.Navigate(new Uri(@"Navigating_Page2.xaml",UriKind.RelativeOrAbsolute));
                            break;

                        case Navigating_State.Navigated: 
                            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                            this.Shutdown();
                            break;
                }
    
            }
        }
    }
}

