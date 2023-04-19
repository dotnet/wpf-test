// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class ObjectPFMarkup1 {
        
        private bool _varSelfCheckPassed = false;
        
        /*
         * Under normal circumstances, nCountLoaded will either be 0 or 1, 
         * even after coming back, or finishing children, etc. unless we save it
         * to the JournalData
         * However, we made this a number instead of a bool in case the loaded
         * event fires unexpected times (e.g. say if fires 3 or 4 times in a row)
         * 
         */
        private int _nCountLoaded = 0;
        

        
        public bool SelfTest(
                    bool exp_cangoback,
                    bool exp_cangofwd ,
                    int exp_CountLoaded    
                    ) 
        {
            // do check of self
            Application app = BasePFNavApp.MainApp;
            // uiautomation check 
            
            // vscan check
            // deferred 
            
            // Obj check
            if (this.Content is DockPanel && ((FrameworkElement)this.Content).Name == "MarkupObjPF1Root") {
                NavigationHelper.Output ("Child of PF is correct");

            } else {
                NavigationHelper.Fail("Child of PF is incorrect");
                return false;
            }
            
            // journal check
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            if (nw.CanGoForward == exp_cangofwd && nw.CanGoBack == exp_cangoback) {
                NavigationHelper.Output ("Journal state is correct");
            } else {
                NavigationHelper.Fail("Journal state is incorrect");
                return false;
            }
            
            if (_nCountLoaded != exp_CountLoaded) {
                NavigationHelper.Fail("Loaded event has not fired expected number of times for the obj markup pf");
                return false;
            }
            
            NavigationHelper.Output("Self Test succeeded..");
            _varSelfCheckPassed = true;
            return _varSelfCheckPassed;
        }

        private void OnLinkClick (object sender, RoutedEventArgs e) {
            Application app = Application.Current;
            NavigationHelper.Output ("Clicked on PageFunction element");
            Control elt = sender as Control;
            switch (elt.Name) {
                case "BtnDone":
                    OnReturn(new ReturnEventArgs<object>(new SolidColorBrush(Colors.Aqua)));
                    break;
                default:
                    break;
            }
            
        }

        
        private void Loaded_PF(object sender, EventArgs e) {
            Application app = Application.Current;
            if (app == null) {
                NavigationHelper.Output("When checking Loaded, the app's type info was not in the assembly");
            } else {
                NavigationHelper.Output("Loaded the PageFunction object");
            }
            _nCountLoaded++;
        }

    }
}
