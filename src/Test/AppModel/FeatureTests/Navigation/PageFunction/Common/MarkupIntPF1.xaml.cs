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
    public partial class IntPFMarkup1 {
        
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
        
        public IntPFMarkup1() 
        {
        }

        private int _nCountObjChildReturn    = 0,                    _nCountBoolChildReturn   = 0,                    _nCountStringChildReturn = 0,                    _nCountIntChildReturn    = 0,                    _nCountStartCalled       = 0;
        
        private string _stringchildreturn = String.Empty;
        private int _intchildreturn = Int32.MinValue;
        
        protected override void Start() {
            Application app = BasePFNavApp.MainApp;
            NavigationHelper.Output("Start method override called on the pagefunction: "+ this.GetType().ToString());
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
            if (this.Content is Border && ((FrameworkElement)this.Content).Name == "MarkupIntPF1Root") {
                NavigationHelper.Output("Child of PF is correct");

            } else {
                NavigationHelper.Fail("Child of PF is incorrect");
                return false;
            }
            
            // journal check
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            if (nw.CanGoForward == exp_cangofwd && nw.CanGoBack == exp_cangoback)
            {
                NavigationHelper.Output("Journal state is correct");
            }
            else
            {
                NavigationHelper.Fail("Journal state is incorrect");
                return false;
            }
            
            if (_nCountLoaded != exp_CountLoaded) {
                NavigationHelper.Fail("Loaded event has not fired expected number of times for the obj markup pf");
                return false;
            }
            
//            if (nCountStartCalled != exp_startcount) {
//                NavigationHelper.Fail("Start method fired incorrect number of times");
//                return false;
//            }
            
            if (RemoveFromJournal != exp_rmvfromjnl) {
                // currently this fails 
                NavigationHelper.Fail("Remove From Journal property of pagefunction is not as expected");
                return false;
            }
            
            NavigationHelper.Output("Self Test succeeded..");
            _varSelfCheckPassed = true;
            return _varSelfCheckPassed;
        }

        private void OnLinkClick (object sender, RoutedEventArgs e) {
            //System.Windows.MessageBox.Show("Button Clicked");
            Application app = Application.Current;
            NavigationWindow nw = app.MainWindow as NavigationWindow;
            
            IntPFMarkup1 next;
            
            NavigationHelper.Output("Clicked on PageFunction element");
            Control elt = sender as Control;
            switch (elt.Name) {
                case "BtnDone":
                    OnReturn(new ReturnEventArgs<Int32>(15));
                    break;
                case "BtnNextChild":
                    // create another instance of the same type of pf
                    next = new IntPFMarkup1();
                    next.InitializeComponent();
                    next.Return += new ReturnEventHandler<Int32>(OnIntReturn);
                    nw.Navigate(next);
                    break;
                case "BtnNextChildNoAttach":
                    next = new IntPFMarkup1();
                    next.InitializeComponent();
                    nw.Navigate(next);
                    break;
                default:
                    break;
            }
            
        }

        public void OnIntReturn(object sender, ReturnEventArgs<Int32> e) {
            Application app = BasePFNavApp.MainApp;
            NavigationHelper.Output("Return event handler from Markup Int PageFunction invoked");            
            _nCountIntChildReturn++;
            _intchildreturn = e.Result;
            NavigationHelper.Output("Child Markup Integer PF returned " + _intchildreturn);
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
        public bool SCheck_RmvJnl_TC21_Int_45_456AE_45()
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
            
            //if (nCountStart)
/*
            SolidColorBrush retobj = objchildreturn as SolidColorBrush;
            if ( retobj == null) {
                NavigationHelper.Fail("Object type incorrect in return event handler from markup object pagefunction");
                return false;
            }

            if (retobj.Color == Colors.Aqua) {
                NavigationHelper.Output("Correctly returned an object from a child markup object pagefunction");
                return true;    
            } else {
                NavigationHelper.Fail("Incorrectly returned an object from child markup object pagefunction");
                return false;
            }
*/                    
              return false;      
        }
        #endregion
    }
}
