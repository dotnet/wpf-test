// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class StringPFMarkup {
                
        private bool _varSelfCheckPassed = false;
        private object _objchildreturn;
        // These are currently unused    
        //        private bool boolchildreturn;
        //        private string stringchildreturn;
        //        private int intchildreturn;
        
        public StringPFMarkup() 
        {
            //InitializeComponent();    
        }
        
        /*
         * Under normal circumstances, nCountLoaded will either be 0 or 1, 
         * even after coming back, or finishing children, etc. unless we save it
         * to the JournalData
         * However, we made this a number instead of a bool in case the loaded
         * event fires unexpected times (e.g. say if fires 3 or 4 times in a row)
         * 
         */
        private int _nCountLoaded = 0;
        private bool _isLoaded = false;
        DispatcherFrame _frame;

        public bool IsStrMkPFLoaded 
        {
            get 
            {
                return _isLoaded;
            }
            private set
            {
                _isLoaded = value;
            }
        }
        
        private int _nCountObjChildReturn    = 0,                    _nCountBoolChildReturn   = 0,                    _nCountStringChildReturn = 0,                    _nCountIntChildReturn    = 0;
        
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
            if (this.Content is DockPanel && ((Panel)this.Content).Name == "MarkupStrPF1Root") {
                NavigationHelper.Output("Child of PF is correct");
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
                NavigationHelper.Fail("Loaded event has not fired or fired incorrect number of times for the string markup pf");
                return false;
            }
            
            NavigationHelper.Output("Self Test succeeded..");
            _varSelfCheckPassed = true;
            return _varSelfCheckPassed;
        }
        
        public bool SCheck_ChildFinish_TC21_Void_45_456AE_44(int expObjChildReturn, int expBoolChildReturn, 
            int expStrChildReturn, int expIntChildReturn)
        {
            Application app = BasePFNavApp.MainApp;
            if (_nCountObjChildReturn == expObjChildReturn
                && _nCountBoolChildReturn == expBoolChildReturn
                && _nCountStringChildReturn == expStrChildReturn
                && _nCountIntChildReturn == expIntChildReturn)
            {
                NavigationHelper.Output("Return event handlers were invoked correct number of times");

                // if we expected no child object PF to return, then end this sub-test now
                if (expObjChildReturn == 0)
                {
                    NavigationHelper.Output("Child PF with no Return eventhandler.  No Return events expected");
                    return true;
                }

            } else {
                NavigationHelper.Fail("The Return event handlers fired incorrect number times");
                return false;
            }
            
            SolidColorBrush retobj = _objchildreturn as SolidColorBrush;
            // if we received an child object PF return event, then...
            if (_nCountObjChildReturn >= 1)
            {
                // [1] check the last object returned to see if it's null
                if (retobj == null)
                {
                    NavigationHelper.Fail("Object type incorrect in return event handler from markup object pagefunction");
                    return false;
                }

                // [2] if non-null, then check to see if it's our expected colour
                if (retobj.Color == Colors.Aqua)
                {
                    NavigationHelper.Output("Correctly returned an object from a child markup object pagefunction");
                    return true;
                }
                else
                {
                    NavigationHelper.Fail("Incorrectly returned an object from child markup object pagefunction");
                    return false;
                }
            }

            // if we haven't returned to the calling function yet, then we've reached an error
            NavigationHelper.Fail("Did not pass checks in SCheck_ChildFinish_TC21_Void_45_456AE_44");
            return false;
        }

        private void OnLinkClicked (object sender, RoutedEventArgs e) {
            //System.Windows.MessageBox.Show("Clicked");
            Application app = Application.Current;
            
            NavigationHelper.Output ("Clicked on PageFunction element");
            Control elt = sender as Control;
            switch (elt.Name) {
                case "LNKNext":
                    NavigationHelper.Output ("Navigating App to child pf and hooking return handler");
                    ObjectPFMarkup1 next = new ObjectPFMarkup1();
                    next.InitializeComponent();
                    NavigationHelper.Output("Attaching non-generic return handler");
                    next.Return += new ReturnEventHandler<object>(OnReturnObj);
                    ((NavigationWindow)app.MainWindow).Navigate(next);
                    break;
                case "LNKNextNoAttach":
                    NavigationHelper.Output ("Navigating App to child pf (not hooking return handler)");
                    ObjectPFMarkup1 next2 = new ObjectPFMarkup1();
                    next2.InitializeComponent();
                    ((NavigationWindow)app.MainWindow).Navigate(next2);
                    break;
                case "LNKDone":
                    OnReturn(new ReturnEventArgs<String>("TESTSTRING"));
                    break;
                default:
                    NavigationHelper.Output ("Not Navigating App");
                    break;
            }
            
        }
        
        public void OnReturnObj(object sender, ReturnEventArgs<Object> e) {
            Application app = Application.Current;                           
            NavigationHelper.Output("Return event handler for markup pf fired");            
            _nCountObjChildReturn++;

            _objchildreturn = e.Result;
            
            if (e.Result == null) {
                // returned null, is it a void pf?
                return;                
            }
            
            NavigationHelper.Output("Type of return value from markup object pf is: " + e.Result.GetType().ToString());
        }
        
        private void Loaded_PF(object sender, EventArgs e) {
            Application app = Application.Current;
            if (app == null) {
                NavigationHelper.Output("When checking Loaded, the app's type info was not in the assembly");
            } else {
                NavigationHelper.Output("Loaded the PageFunction; app type info was in the assembly");
            }
            _nCountLoaded++;
        }


        private void Loaded_PF2(object sender, EventArgs e)
        {
            IsStrMkPFLoaded = true;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
                {
                    if (_frame != null)
                    {
                        _frame.Continue = false;
                        NavigationHelper.Output("Finished waiting for loaded event : " + DateTime.Now.ToString());
                    }
                    return null;
                },
                null);
        }


        public void WaitForLoad()
        {
            if (IsStrMkPFLoaded)
            {
                NavigationHelper.Output("Already loaded");
                return;
            }
            else
            {
                NavigationHelper.Output("Waiting for PageFunction Load");
                _frame = new DispatcherFrame();
                Dispatcher.PushFrame(_frame);
            }
        }

    }
}
