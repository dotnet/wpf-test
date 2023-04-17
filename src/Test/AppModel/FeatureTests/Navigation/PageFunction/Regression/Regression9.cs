// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        void Regression9_Startup(object sender, StartupEventArgs e)
        {            
            NavigationHelper.CreateLog("Regression Test: Dev10 9");
            NavigationHelper.Output("Regression test for nested pagefunctions' access to parent window");
            NavigationHelper.Output("App Starting Up");
            NavigationWindow myWin = new NavigationWindow();
            PFRepro9 firstPage = new PFRepro9();
            firstPage.Name = "FirstInstance";
            myWin.Content = firstPage;
            Application.Current.MainWindow = myWin;
            myWin.Show();
            NavigationHelper.SetStage(TestStage.Run);
        }
    }

    /// <summary>
    /// Page function that re-invokes itself to ensure that nested pagefunctions can still access the window.
    /// </summary>
    public class PFRepro9 : PageFunction<String>
    {
        public bool shouldAutoReturn = false;
        private bool _runSecondPFReturn = false;
        private bool _haveRunLoadedHandler = false;
        private bool _hasAutoReturned = false;

        public PFRepro9()
        {
            Grid niceYellowGrid = new Grid();
            niceYellowGrid.Background = Brushes.Yellow;
            niceYellowGrid.Loaded += new RoutedEventHandler(Grid_Loaded);
            this.Content = niceYellowGrid;
        }        

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (shouldAutoReturn && !_hasAutoReturned)
            {
                _hasAutoReturned = true;
                NavigationHelper.Output("In second PF invocation, returning...");
                OnReturn(new ReturnEventArgs<string>("Returning from PageFunction"));
            }
            else if (this.Name == "FirstInstance" && !_haveRunLoadedHandler)
            {
                _haveRunLoadedHandler = true;
                NavigationHelper.Output("Setting up second Page Function... ");
                PFRepro9 secondPageFunction = new PFRepro9();
                secondPageFunction.shouldAutoReturn = true;
                secondPageFunction.Return += new ReturnEventHandler<string>(SecondPageFunction_Return);
                NavigationService.GetNavigationService(this).Navigate(secondPageFunction);
            }
        }

        void SecondPageFunction_Return(object sender, ReturnEventArgs<string> e)
        {
            if (!_runSecondPFReturn)
            {
                _runSecondPFReturn = true;
                Window myWindow = Window.GetWindow(this);

                if (myWindow == null)
                {
                    NavigationHelper.Fail("Error! Should have been able to get containing window from Window.GetWindow(this) in PF Return event handler but couldn't");
                }
                else
                {
                    NavigationHelper.PassTest("Success! Was able to get containing window from Window.GetWindow(this) in PF Return event handler");
                }
            }
        }
    }
}
