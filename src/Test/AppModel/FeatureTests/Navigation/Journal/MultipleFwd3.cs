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
    // MultipleFwd3
    public partial class NavigationTests : Application
    {
        internal enum MultipleFwd3_CurrentTest
        {
            InitialNav,
            FirstNav,
            SecondNav,
            ThirdNav,
            WentBack
        }

        MultipleFwd3_CurrentTest _multipleFwd3Test = MultipleFwd3_CurrentTest.InitialNav;

        void MultipleFwd3_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("MultipleFwd3");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 11;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = FLOWDOCPAGE;

            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }

        void MultipleFwd3_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void MultipleFwd3_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_multipleFwd3Test)
                {
                    case MultipleFwd3_CurrentTest.InitialNav:
                        _multipleFwd3Test = MultipleFwd3_CurrentTest.FirstNav;
                        NavigationHelper.Output("Calling Navigate on " + ANCHOREDPAGE);
                        nw.Navigate(new Uri(ANCHOREDPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd3_CurrentTest.FirstNav:
                        _multipleFwd3Test = MultipleFwd3_CurrentTest.SecondNav;
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd3_CurrentTest.SecondNav:
                        _multipleFwd3Test = MultipleFwd3_CurrentTest.ThirdNav;
                        nw.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd3_CurrentTest.ThirdNav:
                        _multipleFwd3Test = MultipleFwd3_CurrentTest.WentBack;
                        nw.GoBack();
                        break;

                    case MultipleFwd3_CurrentTest.WentBack:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
        }
    }
}
