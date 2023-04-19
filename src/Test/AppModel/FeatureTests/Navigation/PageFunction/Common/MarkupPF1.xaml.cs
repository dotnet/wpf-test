// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Controls;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class MarkupPF1 
    {
        private bool _varLoaded = false,_varSelfCheckPassed = false;
        
        public bool SelfTest() {
            // do check of self
            NavigationTests app = Application.Current as NavigationTests;
            
            // Obj check
            if (this.Content is DockPanel && ((Panel)this.Content).Name == "TstElem") {
                NavigationHelper.Output("Child of PF is correct");
            } else {
                NavigationHelper.Fail("Child of PF is incorrect");
                return false;
            }
            
            // journal check
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            if (!nw.CanGoForward && !nw.CanGoBack) {
                NavigationHelper.Output("Journal state is correct");
            } else {
                NavigationHelper.Fail("Journal state is incorrect");
            }
            
            if (!_varLoaded) {
                NavigationHelper.Fail("Loaded event has not fired on the markup pf");
                return false;
            }
            
            NavigationHelper.Pass("Self Test succeeded..");
            _varSelfCheckPassed = true;
            return _varSelfCheckPassed;
        }

        private void OnLinkClicked (object sender, RoutedEventArgs e) {
            NavigationHelper.Output("Link clicked");
        }
        
        private void Loaded_PF(object sender, EventArgs e) {
            NavigationTests app = Application.Current as NavigationTests;
            NavigationHelper.Output("Loaded the PageFunction");
            _varLoaded = true;
        }
        
    }
}
