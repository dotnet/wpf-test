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
    // MultipleFwd4
    public partial class NavigationTests : Application
    {
        internal enum MultipleFwd4_CurrentTest
        {
            InitialNav,
            FirstNav,
            SecondNav,
            ThirdNav,
            FourthNav,
            WentBack
        }

        MultipleFwd4_CurrentTest _multipleFwd4Test = MultipleFwd4_CurrentTest.InitialNav;

        void MultipleFwd4_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("MultipleFwd4");
            Application.Current.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 13;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = ANCHOREDPAGE;

            NavigationHelper.SetStage(TestStage.Run);
            base.OnStartup(e);
        }

        void MultipleFwd4_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void MultipleFwd4_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_multipleFwd4Test)
                {
                    case MultipleFwd4_CurrentTest.InitialNav:
                        _multipleFwd4Test = MultipleFwd4_CurrentTest.FirstNav;
                        NavigationHelper.Output("Calling Navigate on " + FLOWDOCPAGE);
                        nw.Navigate(new Uri(FLOWDOCPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd4_CurrentTest.FirstNav:
                        _multipleFwd4Test = MultipleFwd4_CurrentTest.SecondNav;
                        nw.Navigate(new Uri(HLINKPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd4_CurrentTest.SecondNav:
                        _multipleFwd4Test = MultipleFwd4_CurrentTest.ThirdNav;
                        nw.Navigate(new Uri(ANCHOREDPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd4_CurrentTest.ThirdNav:
                        _multipleFwd4Test = MultipleFwd4_CurrentTest.FourthNav;
                        nw.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));
                        break;

                    case MultipleFwd4_CurrentTest.FourthNav:
                        _multipleFwd4Test = MultipleFwd4_CurrentTest.WentBack;
                        nw.GoBack();
                        break;

                    case MultipleFwd4_CurrentTest.WentBack:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
        }
    }
}
