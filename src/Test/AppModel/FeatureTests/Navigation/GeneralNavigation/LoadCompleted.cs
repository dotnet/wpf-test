// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : System.Windows.Application //Application
    //public partial class App : 
    {
        internal enum LoadCompleted_State
        {
            InitialNav, 
            Navigated
        }

        LoadCompleted_State _loadCompleted_curState = LoadCompleted_State.InitialNav;

        void LoadCompleted_Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
                new TestLog("LoadCompleted Test");

            NavigationHelper.ExpectedTestCount = 2;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"LoadCompleted_Page2.xaml";
            
            Log.Current.CurrentVariation.LogMessage("Inside OnStartup()");
            this.StartupUri = new Uri(@"LoadCompleted_Page1.xaml", UriKind.RelativeOrAbsolute);

            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            //NavigationHelper.frmwk.Stage = AutomationFramework.STAGE_RUN;
            //base.OnStartup(e);
        }


        public void LoadCompleted_LoadCompleted(object source, NavigationEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            Log.Current.CurrentVariation.LogMessage("uri is: " + e.Uri.ToString());
            Log.Current.CurrentVariation.LogMessage("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = this.Windows[0] as NavigationWindow;
            if (nw != null)
            {
                switch (_loadCompleted_curState)
                {
                    case LoadCompleted_State.InitialNav:
                        _loadCompleted_curState = LoadCompleted_State.Navigated;
                            Log.Current.CurrentVariation.LogMessage("Calling Navigate on secondPage.xaml");
                            nw.Navigate(new Uri(@"LoadCompleted_Page2.xaml", UriKind.RelativeOrAbsolute));
                            break;

                        case LoadCompleted_State.Navigated: 
                            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                            this.Shutdown();
                            break;
                }
    
            }
        }
    }
}

