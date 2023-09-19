// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SD = System.Drawing;

using MS.Internal.Mita.Foundation;


// Testcase:    SimpleXAMLScenario
// Description: We need to verify that WFH works when added to an AV Window via XAML
namespace WindowsFormsHostTests
{
    public class SimpleXAMLScenario : WPFReflectBase
    {
        #region Testcase setup
        public SimpleXAMLScenario(string[] args) : base(args) { }

        // class vars
        private Window _loadedWin;           // Avalon window build from Xaml file
        private SWF.Button _wfBtn;           // handles to WF controls on window
        private SWF.TextBox _wfTB;
        private SWF.MonthCalendar _wfCal;
        private bool _debug = true;
        private string _events;
        private string _fName = "MyScenario1.xaml";      // file on current directory

        protected override void InitTest(TParams p)
        {
            // hacks to get window to show up !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;

            // set up for Mita
            UseMITA = true;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario2") { return false; }

            this.Title = currentScenario.Name;
            this.Width = 200;
            this.Height = 100;
            _loadedWin = null;
            // run setup for particular scenario
            // must do all setup here because Scenarios are called on separate thread (Mita)
            SetupScenario(p, scenario.Name);
            MyPause();

            return b;
        }

        protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
        {
            // clean up - need to close window created in BeforeScenario so TC can exit
            // this is best done here (rather than in a Scenario) because of cross-thread issue
            if (_loadedWin != null)
            {
                _loadedWin.Close();
            }

            base.AfterScenario(p, scenario, result);
        }

        /// <summary>
        /// Function to set up window for a particular Scenario
        /// </summary>
        /// <param name="p"></param>
        /// <param name="scenario"></param>
        private void SetupScenario(TParams p, string scenario)
        {
            // Note: all Scenarios do the same stuff for setup

            // read XAML file, create window, setup handlers
            if (scenario.Equals("Scenario4") == false)   // Scenario4 will load the window in the Scenario 
            {
                _loadedWin = WindowFromXamlFile(_fName);
                if (_loadedWin != null)
                {
                    _loadedWin.Show();
                    SetUpWindow();
                }
            }
        }

        private void SetUpWindow()
        {
            // add button click handlers
            AddAvalonClickHandler(_loadedWin);
            AddWinformsClickHandler(_loadedWin);

            // now that window has been created, track down handles
            // to our Winforms controls so we can access them in Scenarios
            // (this all depends on names specified in Xaml file)
            StackPanel sp = (StackPanel)_loadedWin.FindName("sp");

            // get handles to controls, assign to globals (er, I mean "Class Variables")
            _wfBtn = (SWF.Button)_loadedWin.FindName("wfBtn1");
            _wfTB = (SWF.TextBox)_loadedWin.FindName("wfTB1");
            _wfCal = (SWF.MonthCalendar)_loadedWin.FindName("wfCal");

            // make sure we found the WF controls
            if (_wfBtn == null) { throw new ArgumentNullException("wfBtn"); }
            if (_wfTB == null) { throw new ArgumentNullException("wfTB"); }
            if (_wfCal == null) { throw new ArgumentNullException("wfCal"); }
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("With a WFH & WF Panel with a button, textbox and calendar added through Xaml, verify that the controls are displayed.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // verify "that the controls are displayed"

            // get bitmaps of Winforms controls
            // (have to use sneaky cross-thread-invoke process for this)
            p.log.WriteLine("Getting bitmaps of controls");
            SD.Bitmap bmpBtn = GetWinformBitmap(_wfBtn);
            SD.Bitmap bmpTB = GetWinformBitmap(_wfTB);
            SD.Bitmap bmpCal = GetWinformBitmap(_wfCal);
            if (_debug)
            {
                bmpBtn.Save("_button.bmp");
                bmpTB.Save("_textbox.bmp");
                bmpCal.Save("_calendar_actual.bmp");
            }

            // verify by looking for colors?
            p.log.WriteLine("Checking bitmaps for proper colors");

            // we set up Xaml file with specific colors
            // if these controls are visible, their bitmaps should contain these colors
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmpBtn, SD.Color.SkyBlue),
                "Cannot find expected color in Winforms Button");
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmpTB, SD.Color.YellowGreen),
                "Cannot find expected color in Winforms TextBox");
            WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmpCal, SD.Color.Orange),
                "Cannot find expected color in Winforms MonthCalendar");

            // It used to be the case that the MonthCalendar was the exact same "discreet"
            // size (178 x 155  pixels) in both WinForms and Avalon.  For this reason, it was possible
            // to directly compare two bitmaps and they would match perfectly.  Sometime between versions
            // 3.0.6201.0 and 3.0.6211.0 of WindowsFormsIntegration.dll this changed.  Now, it is possible
            // for a MonthCalendar displayed via Avalon to be 171 x 155 pixels, so this no longer works.

            return sr;
        }

        [Scenario("With a WFH & WF button, verify that it gets the click events.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // find WF button
            // Note: we are looking for a control on our loaded window, not the test window
            string winTitle = "Window for Scenario 1";
            UIObject uiAvBtn = FindUIObject(p, winTitle, "avBtn");
            UIObject uiWfBtn = FindUIObject(p, winTitle, "B1");

            // simulate a click on the Avalon button
            p.log.WriteLine("Simulating clicking on Avalon button");
            _events = "";
            uiAvBtn.Click();
            MyPause();
            p.log.WriteLine("Events string is '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, "avBtn:", _events, "Did not get expected event", p.log);

            // simulate a click on the WF button
            p.log.WriteLine("Simulating clicking on Winforms button");
            _events = "";
            uiWfBtn.Click();
            MyPause();
            p.log.WriteLine("Events string is '{0}'", _events);
            WPFMiscUtils.IncCounters(sr, "B1:", _events, "Did not get expected event", p.log);

            return sr;
        }

        [Scenario("With a WFH & WF textbox, verify that text can be inserted via the keyboard")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // find WF TextBox
            // Note: we are looking for a control on our loaded window, not the test window
            string winTitle = "Window for Scenario 1";
            UIObject uiWfBtn = FindUIObject(p, winTitle, "TB1");

            // get access to TextBox, set focus to it
            MS.Internal.Mita.Foundation.Controls.Edit edit1 = new MS.Internal.Mita.Foundation.Controls.Edit(uiWfBtn);
            edit1.SetFocus();

            // verify initial text
            p.log.WriteLine("TextBox contents are '{0}'", edit1.Value);
            WPFMiscUtils.IncCounters(sr, "WinForms TextBox", edit1.Value,
                "Initial Winforms TextBox text not correct", p.log);

            // type text
            Keyboard.Instance.SendKeys("Hello?");

            // get text from control
            p.log.WriteLine("TextBox contents are '{0}'", edit1.Value);
            WPFMiscUtils.IncCounters(sr, "Hello?", edit1.Value,
                "Winforms TextBox text not correct", p.log);

            return sr;
        }
        /*
        [Scenario("Add a WFH through XAML without adding xmlns information for it.")]
        public ScenarioResult Scenario4(TParams p)
        {
            private string fName2 = "MyScenario4.xaml";      // file on current directory
            enum FAIL_REASON { NoFail = 0, FileNotFound, ExpectedException, OtherException };
            private FAIL_REASON FailReason = FAIL_REASON.NoFail;
        
            // test done in SetupScenario function
            ScenarioResult sr = new ScenarioResult();

            // the following is the XAML file content for the Scenario4 test
            ////  <Window 
            ////  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            ////  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"  
            ////  xmlns:wf="clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms" 
            //>>>>>>> xaml will work if pass in >>>  xmlns:wfi="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            //>>>>>>> will fail without the proper namespace defined >>>  xmlns:wfi="wfi"
            ////  SizeToContent="Height"
            ////  Width="300" 
            ////  Title="Window for Scenario 4">
            ////  <StackPanel>
            ////    <wfi:WindowsFormsHost>
            ////    <wf:Label Text="test" />
            ////    </wfi:WindowsFormsHost>
            ////  </StackPanel>
            ////</Window>
            Window w = null;
            System.IO.FileStream fs = null;

            try
            {
                // open file
                fs = System.IO.File.Open(fName2, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                w = (Window)System.Windows.Markup.XamlReader.Load(fs);
            }
            catch (System.IO.FileNotFoundException e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                FailReason = FAIL_REASON.FileNotFound;
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got expected exception '{0}'", e.Message);
                FailReason = FAIL_REASON.ExpectedException;
            }
            catch (Exception e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                FailReason = FAIL_REASON.OtherException;
            }
            if (fs != null)
            {
                fs.Close();
            }
                    
            WPFMiscUtils.IncCounters(sr, FAIL_REASON.ExpectedException, FailReason, "not getting the expected exception", p.log);
            return sr;
        }
        */

        #region Helper code for getting bitmaps

        // is there a cleaner simpler way to do this? !!!
        private delegate SD.Bitmap TestDelegateGetBitmap(SWF.Control ctrl);

        private SD.Bitmap MethodGetBitmap(SWF.Control ctrl)
        {
            // get bitmap of specified control
            return Utilities.GetBitmapOfControl(ctrl, true);
        }

        private SD.Bitmap GetWinformBitmap(SWF.Control ctrl)
        {
            TestDelegateGetBitmap GetBitmap = new TestDelegateGetBitmap(MethodGetBitmap);
            SD.Bitmap bmp = (SD.Bitmap)ctrl.Invoke(GetBitmap, ctrl);
            return bmp;
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Helper function to locate a control in a window by name
        /// </summary>
        /// <param name="p"></param>
        /// <param name="windowTitle"></param>
        /// <param name="ctrlName"></param>
        private UIObject FindUIObject(TParams p, string windowTitle, string ctrlName)
        {
            p.log.WriteLine("FindUIObject: win='{0}' control = '{1}'", windowTitle, ctrlName);
            UIObject ctrl = null;

            try
            {
                // find window by name
                UIObject uiWin = UIObject.Root.Children.Find(UICondition.CreateFromName(windowTitle));
                p.log.WriteLine("  Found window: uiApp = '{0}'", uiWin);

                // find control by name
                ctrl = uiWin.Descendants.Find(UICondition.CreateFromId(ctrlName));
                p.log.WriteLine("  Found control: ctrl = '{0}'", ctrl);
            }
            catch (Exception e)
            {
                p.log.WriteLine("FindUIObject: Got exception '{0}'", e.Message);
            }

            return ctrl;
        }

        /// <summary>
        /// Helper function to pause to let events get processed (waits on both message pumps)
        /// </summary>
        private static void MyPause()
        {
            for (int i = 0; i < 2; i++)
            {
                Utilities.SleepDoEvents(10);
            }
        }

        /// <summary>
        /// Helper to add click handler to our Avalon button.  Searches for control by name.
        /// </summary>
        /// <param name="w2"></param>
        private void AddAvalonClickHandler(Window w2)
        {
            // find Avalon Button by name (specified in Xaml), add Click handler
            Button avBtn = (Button)w2.FindName("avBtn");
            avBtn.Click += new RoutedEventHandler(avBtn_Click);
        }

        void avBtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            _events += (b.Name + ":");
            //MessageBox.Show("Happy Day (Avalon)");
        }

        /// <summary>
        /// Helper to add click handler to our WinForms button.  Searches for WFH by name,
        /// then uses WinForms methods to follow path to button in panel in WFH.
        /// </summary>
        /// <param name="w2"></param>
        private void AddWinformsClickHandler(Window w2)
        {
            // find Winforms Button, add Click handler

            // find WFH in Window by name specified in Xaml
            WindowsFormsHost wfh = (WindowsFormsHost)w2.FindName("wfh1");

            // get to FLP as child of WFH
            SWF.FlowLayoutPanel wfFLP = (SWF.FlowLayoutPanel)wfh.Child;

            // find Button in FLP
            SWF.Control[] ctrls = wfFLP.Controls.Find("B1", true);
            SWF.Button wfBtn = (SWF.Button)ctrls[0];

            // add click handler to button
            wfBtn.Click += new EventHandler(wfBtn_Click);
        }

        void wfBtn_Click(object sender, EventArgs e)
        {
            SWF.Button b = (SWF.Button)sender;
            _events += (b.Name + ":");
            //MessageBox.Show("Happy Day (Winforms)");
        }

        /// <summary>
        /// Helper function to read Xaml code from file and create a Window
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Window WindowFromXamlFile(string filename)
        {
            // pseudocode !!!
            //FileStream fs = File.Open("myxaml.xaml");  // myxaml.xaml defines an Avalon button
            //System.Windows.Window w  = (System.Windows.Controls.Window)XamlReader.Load(fs);
            //w.show();

            // spruce this up, improve error checking, maybe make a library function? !!!

            Window w = null;
            System.IO.FileStream fs = null;

            // try to open file
            try
            {
                // open file
                fs = System.IO.File.Open(filename, System.IO.FileMode.Open);
            }
            catch (System.IO.FileNotFoundException e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                return null;
            }

            // try to parse file into Window
            try
            {
                // read file, parse into window
                w = (Window)System.Windows.Markup.XamlReader.Load(fs);
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                fs.Close();
                return null;
            }
            catch (Exception e)
            {
                scenarioParams.log.WriteLine("WindowFromXamlFile: got exception '{0}'", e.Message);
                fs.Close();
                return null;
            }

            if (fs != null)
            {
                fs.Close();
            }

            return w;
        }

        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ With a WFH &amp; WF Panel with a button, textbox and calendar added through Xaml, verify that the controls are displayed.
//@ With a WFH &amp; WF button, verify that it gets the click events.
//@ With a WFH &amp; WF textbox, verify that text can be inserted via the keyboard
//@ Add a WFH through XAML without adding xmlns information for it.
