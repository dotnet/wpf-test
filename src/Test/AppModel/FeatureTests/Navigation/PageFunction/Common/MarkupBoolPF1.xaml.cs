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
    public partial class BoolPFMarkup1 {
        
        
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
        private DateTime _creationtime;

        public BoolPFMarkup1() 
        {
        }
                
        private int _nCountObjChildReturn    = 0,                    _nCountBoolChildReturn   = 0,                    _nCountStringChildReturn = 0,                    _nCountIntChildReturn    = 0,                    _nCountStartCalled       = 0;
        
        /////private object objchildreturn = null;
        /////private bool boolchildreturn;
        private string _stringchildreturn = String.Empty;
        private int _intchildreturn = Int32.MinValue;
        
        protected override void Start() {
            Application app = BasePFNavApp.MainApp;
            NavigationHelper.Output("Start method override called on the PF: " + this.GetType().ToString());
            _nCountStartCalled++;
            base.Start();
        }
        
        public bool SelfTest(
                    bool exp_cangoback,
                    bool exp_cangofwd ,
                    int exp_CountLoaded,
                    int exp_startcount,
                    bool exp_rmvfromjnl
                    ) 
        {
            // do check of self
            Application app = BasePFNavApp.MainApp;
            // uiautomation check 
            
            // vscan check
            // deferred 
            
            // Obj check
            if (this.Content is Border && 
                ((FrameworkElement)this.Content).Name == "MarkupBoolPF1Root") {
                NavigationHelper.Output("Child of PF is correct");

            } else {
                NavigationHelper.Fail("Child of PF is incorrect");
                return false;
            }
            
            // journal check
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            if (nw.CanGoForward == exp_cangofwd && nw.CanGoBack == exp_cangoback) {
                NavigationHelper.Output("Journal state is correct");
            } else {
                NavigationHelper.Fail("Journal state is incorrect");
                //Debugger.Launch();
                return false;
            }
            
            if (_nCountLoaded != exp_CountLoaded) {
                NavigationHelper.Fail("Loaded event has not fired expected number of times for the obj markup PF");
                return false;
            }
            
            // this check is problematic , b/c Start is called at odd times
//            if (nCountStartCalled != exp_startcount) {
//                app.FW.LogTest(false,"Start method fired incorrect number of times");
//                return false;
//            }
            
            if (RemoveFromJournal != exp_rmvfromjnl) {
                NavigationHelper.Fail("RemoveFromJournal property of PF is not as expected");
                return false;
            } else {
                NavigationHelper.Output("Correct value for RemoveFromJournal");
            }
            
            NavigationHelper.Output("Self Test succeeded..");
            _varSelfCheckPassed = true;
            return _varSelfCheckPassed;
        }

        private void OnLinkClick (object sender, RoutedEventArgs e) {
            //System.Windows.MessageBox.Show("Button Clicked");
            Application app = Application.Current;
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            
            IntPFMarkup1 nextintpf;
            
            NavigationHelper.Output("Clicked on PageFunction element");
            Control elt = sender as Control;
            switch (elt.Name) {
                case "BtnDoneT":
                    OnReturn(new ReturnEventArgs<bool>(true));
                    break;
                case "BtnDoneF":
                    OnReturn(new ReturnEventArgs<bool>(false));
                    break;
                case "BtnNextChildInt":
                    // create another instance of the same type of pf
                    nextintpf = new IntPFMarkup1();
                    nextintpf.InitializeComponent();
                    nextintpf.Return += new ReturnEventHandler<Int32>(OnIntReturn);
                    nw.Navigate(nextintpf);
                    break;
                case "BtnNextChildIntNoAttach":
                    nextintpf = new IntPFMarkup1();
                    nextintpf.InitializeComponent();
                    nw.Navigate(nextintpf);
                    break;
                    
                default:
                    break;
            }
            
        }

        public void OnIntReturn(object sender, ReturnEventArgs<Int32> e) {
            Application app = BasePFNavApp.MainApp;
            NavigationHelper.Output("Return event handler from Markup Int PF invoked");            
            _nCountIntChildReturn++;
            _intchildreturn = e.Result;
            NavigationHelper.Output("Child Markup Int PF returned " + _intchildreturn);
        }
        
        
        private void Loaded_PF(object sender, EventArgs e) {
            Application app = Application.Current;
            if (app == null) {
                NavigationHelper.Output("When checking Loaded, the app's type info was not in the assembly");
            } else {
                NavigationHelper.Output("Loaded the PageFunction object: " + this.GetType().ToString());
            }
            _nCountLoaded++;
            _creationtime = DateTime.Now;
            BtnCreation.Content = "Creation: " + _creationtime.ToString();
        }
        
        
        #region Test Functions
        public bool SCheck_RmvJnl_TC21_IntChildFinish_45_456AE_46()
        {
            Application app = BasePFNavApp.MainApp;
            if (   _nCountObjChildReturn     == 0
                && _nCountBoolChildReturn    == 0
                && _nCountStringChildReturn  == 0
                && _nCountIntChildReturn     == 1) {
                NavigationHelper.Output("Return event handlers were invoked correct number of times");
            } else {
                NavigationHelper.Fail("The Return event handlers fired incorrect number times");
                return false;
            }
            
            if (_nCountStartCalled != 0) {
                // should be zero when child pf finishes
                NavigationHelper.Fail("Start fired when finishing child PF");
                return false;
            } else {
                NavigationHelper.Output("As expected, Start wasnt called when finishing child PF");
            }
            
            if (_intchildreturn != 15) {
                NavigationHelper.Fail("incorrect return value from child int PF (mk)");
                return false;
            }
            
            if (RemoveFromJournal != true) {
                NavigationHelper.Fail("incorrect value of RemoveFromJournal");
                return false;
            }
            
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            if (nw.CanGoForward) {
                NavigationHelper.Fail("incorrect value of CanGoForward when finishing a child with RemoveFromJournal");
                return false;
            } 

            // Since RemoveFromJournal is true by default, when a child PF returns to its parent
            // page, it is equivalent to invoking GoBack to get back to the parent page. If the 
            // parent page has no previous journal entries (like in this case), then CanGoBack should be false.
            
            if (nw.CanGoBack == true /*tweaked here*/) {
                NavigationHelper.Fail("incorrect value for CanGoBack when finishing a child with RemoveFromJournal");
                return false;
            }

            NavigationHelper.Output("Correct journal state");
            return true;      
        }
        #endregion
    }
}
