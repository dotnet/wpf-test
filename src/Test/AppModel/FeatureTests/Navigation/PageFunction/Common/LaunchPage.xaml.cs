// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NonPFLaunchPage
    {
        string _retval;

        private void LaunchPF(object sender, RoutedEventArgs e)
        {
            Application app = BasePFNavApp.MainApp;
            // This child PageFunction object has RemoveFromJournal set to true by default
            StringPFMarkup pfres = new StringPFMarkup();
            pfres.InitializeComponent();
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;

            NavigationHelper.Output("Navigating from page to pagefunction");

            pfres.RemoveFromJournal = false;
            pfres.Return += new ReturnEventHandler<String>(OnStrReturn);
            nw.Navigate(pfres);
        }

        public void OnStrReturn(object sender, ReturnEventArgs<String> args)
        {
            Application app = BasePFNavApp.MainApp;
            _retval = args.Result;
            NavigationHelper.Output("The pagefunction navigated from the page returned: " + _retval);
        }

        public bool SelfCheck_1()
        {
            Application app = BasePFNavApp.MainApp;

            if (this.Name != "RootLaunch")
            {
                NavigationHelper.Fail("Incorrect Name of root element of the page");
                return false;
            }
            else
            {
                NavigationHelper.Output("Correct content specified as root element of the page");
            }

            if (this._retval != "TESTSTRING")
            {
                NavigationHelper.Fail("Incorrect return value from pagefunction launched from a page");
                return false;
            }
            else
            {
                NavigationHelper.Output("Child pagefunction launched from this page returned correct value");
            }

            NavigationWindow nw = app.MainWindow as NavigationWindow;

            if (!nw.CanGoBack)
            {
                NavigationHelper.Fail("If RemoveFromJournal=false, the pagefunction launched from the page should be in the window's back stack, but it isn't");
                return false;
            }

            if (nw.CanGoForward)
            {
                NavigationHelper.Fail("CanGoForward: false", "ACTUAL: CanGoForward: true");
                return false;
            }

            NavigationHelper.Output("App's journal state is correct after finishing the pagefunction launched from the page (rmvfromjournal is default)");
            return true;
        }
    }
}
