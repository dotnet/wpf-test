using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

//
// Testcase:    Enabled
// Description: Verify that the WF's Enabled Property and WPF's IsEnabled Property work properly
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class Enabled : WPFReflectBase
{
    #region Testcase setup
    public Enabled(string[] args) : base(args) { }

    // class vars
    private enum ContainerType { DockPanel, Grid, StackPanel, Canvas, WrapPanel };
    private DockPanel _dp;
    private Grid _grid;
    private StackPanel _stack;
    private Canvas _canvas;
    private WrapPanel _wrap;
    private enum TestType { Single, Complex, Container };

    private bool debug = false;         // set this true for TC debugging !!!

    // host controls
    WindowsFormsHost _wfh1;
    WindowsFormsHost _wfh2;
    WindowsFormsHost _wfh3;

    // WF controls
    private System.Windows.Forms.TextBox _tb1;
    private System.Windows.Forms.Button _wfbtn1;
    private System.Windows.Forms.DataGridView _dgv;
    private System.Windows.Forms.CheckBox _cbx;
    private System.Windows.Forms.UserControl _userctl;
    private System.Windows.Forms.Panel _panel;

    protected override void InitTest(TParams p)
    {
        // hacks to get window to show up !!!
        this.Topmost = true;
        this.Topmost = false;
        this.WindowState = WindowState.Maximized;
        this.WindowState = WindowState.Normal;

        this.Width = 500;
        this.Height = 500;

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        // need to call base.BeforeScenario before accessing "this"
        bool b = base.BeforeScenario(p, scenario);

        // debug !!!
        //if (scenario.Name != "Scenario3") { b = false; }

        return b;
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("WFH with single control.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // iterate through each container type
        foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
        {
            TestSetup(p, contType, TestType.Single);
            MyPause();
            DoSingleControlTest(p, sr, contType);
        }

        return sr;
    }

    [Scenario("WFH with complex control.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // iterate through each container type
        foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
        {
            TestSetup(p, contType, TestType.Complex);
            MyPause();
            DoComplexControlTest(p, sr, contType);
        }

        return sr;
    }

    [Scenario("WFH with container control.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // iterate through each container type
        foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
        {
            TestSetup(p, contType, TestType.Container);
            MyPause();
            DoContainerControlTest(p, sr, contType);
        }

        return sr;
    }

    [Scenario("WF control Enabled transitions (true -> true, true->false, false ->true)")]
    public ScenarioResult Scenario4(TParams p)
    {
        // Transition tests are performed within other Scenarios
        return new ScenarioResult(true, "Performed elsewhere", p.log);
    }

    [Scenario("Regress Windows OS Bug 1420270 - Exception when setting IsEnabled false")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // for this Scenario, let's just create a new window
        Window w = new Window();
        DockPanel dp = new DockPanel();

        // create Avalon button
        Button btn = new Button();
        btn.Content = "Push Me";
        //btn.Click += new RoutedEventHandler(btn_Click);
        dp.Children.Add(btn);

        // create first host control
        WindowsFormsHost wfh1 = new WindowsFormsHost();
        wfh1.Child = new System.Windows.Forms.Button();
        dp.Children.Add(wfh1);

        // create second host control
        WindowsFormsHost wfh2 = new WindowsFormsHost();
        wfh2.Child = new System.Windows.Forms.Button();
        dp.Children.Add(wfh2);

        // create Avalon button
        Button btn2 = new Button();
        btn2.Content = "Filler";
        dp.Children.Add(btn2);

        // show the window
        w.Content = dp;
        w.Show();
        MyPause();

        // do test
        // we should not get an exception here
        try
        {
            dp.IsEnabled = false;
            WPFMiscUtils.IncCounters(sr, p.log, true, "Did not get exception");
        }
        catch (Exception e)
        {
            // if we got here, we got an exception that we should not have gotten
            p.log.WriteLine("Got exception: '{0}'", e.ToString());
            p.log.LogKnownBug(BugDb.WindowsOSBugs, 1420270, "Got exception setting IsEnabled = false");
            WPFMiscUtils.IncCounters(sr, p.log, false, "Got unexpected exception");
        }

        // clean up
        w.Close();

        return sr;
    }

    #region Scenario Helper functions

    // Helper routine for Scenario1
    private void DoSingleControlTest(TParams p, ScenarioResult sr, ContainerType contType)
    {
        // get "control" bitmaps
        // have to set topmost so can get bitmap !!!
        this.Topmost = true;
        MyPause();

        // twiddle with window to get colors right !!!
        this.WindowState = WindowState.Maximized;
        this.WindowState = WindowState.Normal;

        // when enabled
        _tb1.Enabled = true;
        MyPause();
        System.Drawing.Bitmap bmpEnabled = Utilities.GetBitmapOfControl(_tb1, true);

        // when disabled
        _tb1.Enabled = false;
        MyPause();
        System.Drawing.Bitmap bmpDisabled = Utilities.GetBitmapOfControl(_tb1, true);

        // restore
        _tb1.Enabled = true;
        this.Topmost = false;
        if (debug)
        {
            bmpEnabled.Save("_Enabled.bmp");
            bmpDisabled.Save("_Disabled.bmp");
        }

        // iterate through settings
        // Container level
        foreach (bool contFlag in new bool[] { true, false })
        {
            if (debug) { p.log.WriteLine("contFlag = '{0}'", contFlag); }

            // WF Host control
            foreach (bool wfhFlag in new bool[] { true, false })
            {
                if (debug) { p.log.WriteLine("\twfhFlag = '{0}'", wfhFlag); }

                // WF control
                foreach (bool wfFlag in new bool[] { true, false })
                {
                    if (debug) { p.log.WriteLine("\t\twfFlag = '{0}'", wfFlag); }

                    // reset controls
                    if (true)
                    {
                        SetPanelIsEnabled(contType, true);
                        _wfh1.IsEnabled = true;
                        _tb1.Enabled = true;
                        _wfbtn1.Enabled = true;
                        _cbx.Enabled = true;
                        MyPause();
                    }

                    // set states to test values
                    if (debug) { p.log.WriteLine("setting state to  {0} {1} {2}", contFlag, wfhFlag, wfFlag); }
                    SetPanelIsEnabled(contType, contFlag);
                    _wfh1.IsEnabled = wfhFlag;
                    _tb1.Enabled = wfFlag;
                    _wfbtn1.Enabled = wfFlag;
                    _cbx.Enabled = wfFlag;

                    // not sure if need this !!!
                    MyPause();

                    // calc expected states
                    bool expContFlag = contFlag;
                    bool expHostFlag = expContFlag && wfhFlag;
                    bool expCtrlFlag = expHostFlag && wfFlag;

                    // log expected/actual state settings
                    if (debug)
                    {
                        p.log.WriteLine("expected state is {0} {1} {2}", expContFlag, expHostFlag, expCtrlFlag);
                        p.log.WriteLine("state is now      {0} {1} {2}", GetPanelIsEnabled(contType), _wfh1.IsEnabled, _tb1.Enabled);
                        string strState = (contFlag && wfhFlag && wfFlag ? "Enabled" : "Disabled");
                        p.log.WriteLine("WinForms Control should be '{0}'", strState);
                    }

                    // compare expected/actual
                    bool actIsEnabled = GetPanelIsEnabled(contType);
                    WPFMiscUtils.IncCounters(sr, expContFlag, actIsEnabled, "Container.IsEnabled not set properly", p.log);
                    WPFMiscUtils.IncCounters(sr, expHostFlag, _wfh1.IsEnabled, "WindowsFormsHost.IsEnabled not set properly", p.log);
                    WPFMiscUtils.IncCounters(sr, expCtrlFlag, _tb1.Enabled, "WinFormCtrl.Enabled not set properly", p.log);

                    MyPause();

                    // get bitmap of our control
                    // compare bitmap with that of what it is supposed to look like
                    if (true)
                    {
                        // have to set topmost so can get bitmap !!!
                        this.Topmost = true;
                        MyPause();

                        // get bitmap of our control
                        System.Drawing.Bitmap bmpTB = Utilities.GetBitmapOfControl(_tb1, true);
                        System.Drawing.Bitmap bmpBtn = Utilities.GetBitmapOfControl(_wfbtn1, true);
                        System.Drawing.Bitmap bmpCB = Utilities.GetBitmapOfControl(_cbx, true);
                        this.Topmost = false;
                        if (debug)
                        {
                            bmpTB.Save("_textbox.bmp");
                            bmpBtn.Save("_button.bmp");
                            bmpCB.Save("_combobox.bmp");
                        }

                        // decide which "control" bitmap our button is expected to look like
                        // if all parent controls are enabled, then control should be enabled
                        bool bMatch;
                        if (contFlag && wfhFlag && wfFlag)
                        {
                            if (debug) { p.log.WriteLine("Comparing with Enabled"); }
                            //!!!bMatch = BitmapsCloseEnough(bmpEnabled, bmpBtn);
                            bMatch = Utilities.BitmapsIdentical(bmpEnabled, bmpTB);
                        }
                        else
                        {
                            if (debug) { p.log.WriteLine("Comparing with Disabled"); }
                            //!!!bMatch = BitmapsCloseEnough(bmpDisabled, bmpBtn);
                            bMatch = Utilities.BitmapsIdentical(bmpDisabled, bmpTB);
                        }

                        WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                        if (!bMatch)
                        {
                            p.log.WriteLine("bMatch is {0}", bMatch);
                            //Utilities.ActiveFreeze("Bitmap doesn't match");
                        }
                    }

                    if (debug) { p.log.WriteLine(""); }
                }
            }
        }
    }

    // Helper routine for Scenario2
    private void DoComplexControlTest(TParams p, ScenarioResult sr, ContainerType contType)
    {
        // get "control" bitmaps
        // have to set topmost so can get bitmap !!!
        this.Topmost = true;
        MyPause();

        // twiddle with window to get colors right !!!
        this.WindowState = WindowState.Maximized;
        this.WindowState = WindowState.Normal;

        // when enabled
        _userctl.Enabled = true;
        MyPause();
        System.Drawing.Bitmap bmpEnabled = Utilities.GetBitmapOfControl(_userctl, true);

        // when disabled
        _userctl.Enabled = false;
        MyPause();
        System.Drawing.Bitmap bmpDisabled = Utilities.GetBitmapOfControl(_userctl, true);

        // restore
        _userctl.Enabled = true;
        this.Topmost = false;
        if (debug)
        {
            bmpEnabled.Save("_Enabled.bmp");
            bmpDisabled.Save("_Disabled.bmp");
        }

        // iterate through settings
        // Container level
        foreach (bool contFlag in new bool[] { true, false })
        {
            if (debug) { p.log.WriteLine("contFlag = '{0}'", contFlag); }

            // WF Host control
            foreach (bool wfhFlag in new bool[] { true, false })
            {
                if (debug) { p.log.WriteLine("\twfhFlag = '{0}'", wfhFlag); }

                // UserControl control
                foreach (bool usrFlag in new bool[] { true, false })
                {
                    if (debug) { p.log.WriteLine("\t\tusrFlag = '{0}'", usrFlag); }

                    // WF control
                    foreach (bool wfFlag in new bool[] { true, false })
                    {
                        if (debug) { p.log.WriteLine("\t\t\twfFlag = '{0}'", wfFlag); }

                        // reset controls
                        if (true)
                        {
                            SetPanelIsEnabled(contType, true);
                            _wfh1.IsEnabled = true;
                            _userctl.Enabled = true;
                            _dgv.Enabled = true;
                            _tb1.Enabled = true;
                            MyPause();
                        }

                        // set states to test values
                        if (debug)
                        {
                            p.log.WriteLine("setting state to  {0} {1} {2} {3}", 
                                contFlag, wfhFlag, usrFlag, wfFlag);
                        }
                        SetPanelIsEnabled(contType, contFlag);
                        _wfh1.IsEnabled = wfhFlag;
                        _userctl.Enabled = usrFlag;
                        _dgv.Enabled = wfFlag;
                        _tb1.Enabled = wfFlag;

                        // not sure if need this !!!
                        MyPause();

                        // calc expected states
                        bool expContFlag = contFlag;
                        bool expHostFlag = expContFlag && wfhFlag;
                        bool expUserFlag = expHostFlag && usrFlag;
                        bool expCtrlFlag = expUserFlag && wfFlag;

                        // log expected/actual state settings
                        if (debug)
                        {
                            p.log.WriteLine("expected state is {0} {1} {2} {3}", expContFlag, expHostFlag, expUserFlag, expCtrlFlag);
                            p.log.WriteLine("state is now      {0} {1} {2} {3}", GetPanelIsEnabled(contType), _wfh1.IsEnabled, _userctl.Enabled, _tb1.Enabled);
                            string strState = (contFlag && wfhFlag && wfFlag ? "Enabled" : "Disabled");
                            p.log.WriteLine("WinForms Control should be '{0}'", strState);
                        }

                        // compare expected/actual
                        bool actIsEnabled = GetPanelIsEnabled(contType);
                        WPFMiscUtils.IncCounters(sr, expContFlag, actIsEnabled, "Container.IsEnabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expHostFlag, _wfh1.IsEnabled, "WindowsFormsHost.IsEnabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expUserFlag, _userctl.Enabled, "UserControl.Enabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlFlag, _dgv.Enabled, "WinFormCtrl.Enabled not set properly - DGV", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlFlag, _tb1.Enabled, "WinFormCtrl.Enabled not set properly - TextBox", p.log);

                        MyPause();
                        _userctl.Refresh();

                        // get bitmap of our control
                        // compare bitmap with that of what it is supposed to look like
                        if (true)
                        {
                            // have to set topmost so can get bitmap !!!
                            this.Topmost = true;
                            MyPause();

                            // get bitmap of our control
                            System.Drawing.Bitmap bmpUser = Utilities.GetBitmapOfControl(_userctl, true);
                            this.Topmost = false;
                            if (debug)
                            {
                                bmpUser.Save("_userctrl.bmp");

                            }

                            // decide which "control" bitmap our button is expected to look like
                            // if all parent controls are enabled, then control should be enabled
                            bool bMatch;
                            if (contFlag && usrFlag && wfhFlag && wfFlag)
                            {
                                if (debug) { p.log.WriteLine("Comparing with Enabled"); }
                                //!!!bMatch = BitmapsCloseEnough(bmpEnabled, bmpBtn);
                                bMatch = Utilities.BitmapsIdentical(bmpEnabled, bmpUser);
                            }
                            else
                            {
                                if (debug) { p.log.WriteLine("Comparing with Disabled"); }
                                //!!!bMatch = BitmapsCloseEnough(bmpDisabled, bmpBtn);
                                bMatch = Utilities.BitmapsIdentical(bmpDisabled, bmpUser);
                            }

                            WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                            if (!bMatch)
                            {
                                p.log.WriteLine("bMatch is {0}", bMatch);
//                                Utilities.ActiveFreeze("Bitmap doesn't match");
                            }
                        }

                        if (debug) { p.log.WriteLine(""); }
                    }
                }
            }
        }
    }

    // Helper routine for Scenario3
    private void DoContainerControlTest(TParams p, ScenarioResult sr, ContainerType contType)
    {
        // get "control" bitmaps
        // have to set topmost so can get bitmap !!!
        this.Topmost = true;
        MyPause();

        // twiddle with window to get colors right !!!
        this.WindowState = WindowState.Maximized;
        this.WindowState = WindowState.Normal;

        // when enabled
        _panel.Enabled = true;
        MyPause();
        System.Drawing.Bitmap bmpEnabled = Utilities.GetBitmapOfControl(_panel, true);

        // when disabled
        _panel.Enabled = false;
        MyPause();
        System.Drawing.Bitmap bmpDisabled = Utilities.GetBitmapOfControl(_panel, true);

        // restore
        _panel.Enabled = true;
        this.Topmost = false;
        if (debug)
        {
            bmpEnabled.Save("_Enabled.bmp");
            bmpDisabled.Save("_Disabled.bmp");
        }

        // iterate through settings
        // Container level
        foreach (bool contFlag in new bool[] { true, false })
        {
            if (debug) { p.log.WriteLine("contFlag = '{0}'", contFlag); }

            // WF Host control
            foreach (bool wfhFlag in new bool[] { true, false })
            {
                if (debug) { p.log.WriteLine("\twfhFlag = '{0}'", wfhFlag); }

                // Panel control
                foreach (bool panFlag in new bool[] { true, false })
                {
                    if (debug) { p.log.WriteLine("\t\tpanFlag = '{0}'", panFlag); }

                    // WF control
                    foreach (bool wfFlag in new bool[] { true, false })
                    {
                        if (debug) { p.log.WriteLine("\t\t\twfFlag = '{0}'", wfFlag); }

                        // reset controls
                        if (true)
                        {
                            SetPanelIsEnabled(contType, true);
                            _wfh1.IsEnabled = true;
                            _panel.Enabled = true;
                            _tb1.Enabled = true;
                            _dgv.Enabled = true;
                            _wfbtn1.Enabled = true;
                            MyPause();
                        }

                        // set states to test values
                        if (debug)
                        {
                            p.log.WriteLine("setting state to  {0} {1} {2} {3}",
                                contFlag, wfhFlag, panFlag, wfFlag);
                        }
                        SetPanelIsEnabled(contType, contFlag);
                        _wfh1.IsEnabled = wfhFlag;
                        _panel.Enabled = panFlag;
                        _tb1.Enabled = wfFlag;
                        _dgv.Enabled = wfFlag;
                        _wfbtn1.Enabled = wfFlag;

                        // not sure if need this !!!
                        MyPause();

                        // calc expected states
                        bool expContFlag = contFlag;
                        bool expHostFlag = expContFlag && wfhFlag;
                        bool expPanelFlag = expHostFlag && panFlag;
                        bool expCtrlFlag = expPanelFlag && wfFlag;

                        // log expected/actual state settings
                        if (debug)
                        {
                            p.log.WriteLine("expected state is {0} {1} {2} {3}", expContFlag, expHostFlag, expPanelFlag, expCtrlFlag);
                            p.log.WriteLine("state is now      {0} {1} {2} {3}", GetPanelIsEnabled(contType), _wfh1.IsEnabled, _panel.Enabled, _tb1.Enabled);
                            string strState = (contFlag && wfhFlag && wfFlag ? "Enabled" : "Disabled");
                            p.log.WriteLine("WinForms Control should be '{0}'", strState);
                        }

                        // compare expected/actual
                        bool actIsEnabled = GetPanelIsEnabled(contType);
                        WPFMiscUtils.IncCounters(sr, expContFlag, actIsEnabled, "Container.IsEnabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expHostFlag, _wfh1.IsEnabled, "WindowsFormsHost.IsEnabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expPanelFlag, _panel.Enabled, "Panel.Enabled not set properly", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlFlag, _tb1.Enabled, "WinFormCtrl.Enabled not set properly - TextBox", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlFlag, _dgv.Enabled, "WinFormCtrl.Enabled not set properly - DGV", p.log);
                        WPFMiscUtils.IncCounters(sr, expCtrlFlag, _wfbtn1.Enabled, "WinFormCtrl.Enabled not set properly - Button", p.log);

                        MyPause();
                        _panel.Refresh();

                        // get bitmap of our control
                        // compare bitmap with that of what it is supposed to look like
                        if (true)
                        {
                            // have to set topmost so can get bitmap !!!
                            this.Topmost = true;
                            MyPause();

                            // get bitmap of our control
                            System.Drawing.Bitmap bmpPanel = Utilities.GetBitmapOfControl(_panel, true);
                            this.Topmost = false;
                            if (debug)
                            {
                                bmpPanel.Save("_panel.bmp");
                            }

                            // decide which "control" bitmap our button is expected to look like
                            // if all parent controls are enabled, then control should be enabled
                            bool bMatch;
                            if (contFlag && wfhFlag && panFlag && wfFlag)
                            {
                                if (debug) { p.log.WriteLine("Comparing with Enabled"); }
                                //!!!bMatch = BitmapsCloseEnough(bmpEnabled, bmpBtn);
                                bMatch = Utilities.BitmapsIdentical(bmpEnabled, bmpPanel);
                            }
                            else
                            {
                                if (debug) { p.log.WriteLine("Comparing with Disabled"); }
                                //!!!bMatch = BitmapsCloseEnough(bmpDisabled, bmpBtn);
                                bMatch = Utilities.BitmapsIdentical(bmpDisabled, bmpPanel);
                            }

                            WPFMiscUtils.IncCounters(sr, p.log, bMatch, "Control does not match bitmap image");
                            if (!bMatch)
                            {
                                p.log.WriteLine("bMatch is {0}", bMatch);
                                //Utilities.ActiveFreeze("Bitmap doesn't match");
                            }
                        }

                        if (debug) { p.log.WriteLine(""); }
                    }
                }
            }
        }
    }

    #endregion

    #region Helper functions

    // Helper function used to set the IsEnabled setting of container control we are using
    // (Note: for this purpose, I believe if-then-else looks cleaner than switch-case)
    private void SetPanelIsEnabled(ContainerType contType, bool value)
    {
        // create container
        if (contType == ContainerType.Canvas)
        {
            _canvas.IsEnabled = value;
        }
        else if (contType == ContainerType.DockPanel)
        {
            _dp.IsEnabled = value;
        }
        else if (contType == ContainerType.Grid)
        {
            _grid.IsEnabled = value;
        }
        else if (contType == ContainerType.StackPanel)
        {
            _stack.IsEnabled = value;
        }
        else if (contType == ContainerType.WrapPanel)
        {
            _wrap.IsEnabled = value;
        }
        else
        {
            throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
        }
    }

    // Helper function used to retrieve IsEnabled setting of container control we are using
    // (Note: for this purpose, I believe if-then-else looks cleaner than switch-case)
    private bool GetPanelIsEnabled(ContainerType contType)
    {
        // create container
        if (contType == ContainerType.Canvas)
        {
            return (_canvas.IsEnabled);
        }
        else if (contType == ContainerType.DockPanel)
        {
            return (_dp.IsEnabled);
        }
        else if (contType == ContainerType.Grid)
        {
            return (_grid.IsEnabled);
        }
        else if (contType == ContainerType.StackPanel)
        {
            return (_stack.IsEnabled);
        }
        else if (contType == ContainerType.WrapPanel)
        {
            return (_wrap.IsEnabled);
        }
        else
        {
            throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
        }
    }
    
    // Helper function to set up app for particular Scenario
    // Will create Avalon/WinForms controls based on desired Container Type and Test Type
    // Basically creates desired WFH control sandwiched between two Avalon buttons within specified container
    // contType - container type, the kind of high level Avalon container we are using
    // testType - test type, based on whats being tested in Scenario
    private void TestSetup(TParams p, ContainerType contType, TestType testType)
    {
        // update app title bar and log file
        string str = string.Format("Container type: {0} using {1} control", 
            contType.ToString(), testType.ToString());
        this.Title = str;
        p.log.WriteLine(str);

        // create WF host controls
        _wfh1 = new WindowsFormsHost();
        _wfh2 = new WindowsFormsHost();
        _wfh3 = new WindowsFormsHost();

        // WFH containing simple WF control - one w/TextBox; one w/Button; one w/ComboBox
        if (testType == TestType.Single)
        {
            // create WF controls
            _tb1 = new System.Windows.Forms.TextBox();
            _wfbtn1 = new System.Windows.Forms.Button();
            _cbx = new System.Windows.Forms.CheckBox();

            // throw some random text into controls
            _tb1.Text = p.ru.GetString(10);
            _wfbtn1.Text = "Push Me";
            _cbx.Text = "My Checkbox";

            // add WF controls to host controls
            _wfh1.Child = _tb1;
            _wfh2.Child = _wfbtn1;
            _wfh3.Child = _cbx;
        }

        // WFH containing WF usercontrol containing DataGridView and TextBox
        else if (testType == TestType.Complex)
        {
            // create WF usercontrol
            _userctl = new System.Windows.Forms.UserControl();
            _userctl.Dock = System.Windows.Forms.DockStyle.Fill;

            // add WF control to usercontrol
            _dgv = new System.Windows.Forms.DataGridView();
            _tb1 = new System.Windows.Forms.TextBox();
            _userctl.Controls.Add(_dgv);
            _userctl.Controls.Add(_tb1);

            // throw some random text into control
            _tb1.Text = p.ru.GetString(10);
            SetupDGV();

            // add WF usercontrol to host control
            System.Drawing.Size uSz = _userctl.Size;
            _wfh1.Child = _userctl;

            // !!! workaround - must manually change size of WFH after add UC
            // !!! when usercontrol added to wfh, it's size is set to 0,0 and cannot be changed
            _wfh1.Width = uSz.Width;
            _wfh1.Height = uSz.Height;
        }

        // WFH containing WF container (Panel) containing TextBox, Button, and DataGridView
        else if (testType == TestType.Container)
        {
            // create WF container with three controls
            _panel = new System.Windows.Forms.Panel();
            _panel.Dock = System.Windows.Forms.DockStyle.Fill;

            // add 3 WF controls to panel
            _tb1 = new System.Windows.Forms.TextBox();
            _dgv = new System.Windows.Forms.DataGridView();
            _wfbtn1 = new System.Windows.Forms.Button();

            //_tb1.Location = new System.Drawing.Point(10, 10);
            //_dgv.Location = new System.Drawing.Point(200, 10);
            //_wfbtn1.Location = new System.Drawing.Point(100, 100);
            _dgv.Location = new System.Drawing.Point(0, 0);
            _dgv.Height = 150;
            _tb1.Location = new System.Drawing.Point(50, 150);
            _wfbtn1.Location = new System.Drawing.Point(50, 180);
            _panel.Size = new System.Drawing.Size(_dgv.Size.Width, _dgv.Size.Height + 80);

            _panel.Controls.AddRange(new System.Windows.Forms.Control[] { _tb1, _wfbtn1, _dgv});

            // throw some random text into control
            _tb1.Text = p.ru.GetString(10);
            _wfbtn1.Text = p.ru.GetString(10);
            SetupDGV();

            // add WF panel to host control
            System.Drawing.Size uSz = _panel.Size;
            _wfh1.Child = _panel;

            // !!! workaround - must manually change size of WFH after add Panel
            // !!! when Panel added to wfh, it's size is set to 0,0 and cannot be changed
            _wfh1.Width = uSz.Width;
            _wfh1.Height = uSz.Height;
        }

        else
        {
            // new TestType?
            throw new ArgumentException("Unknown TestType '{0}'", testType.ToString());
        }

        // create appropriate panel and add hosts
        CreatePanel(contType);
    }

    /// <summary>
    /// Helper function used to create Avalon panel of proper type and add to window.
    /// Also adds our WFH controls to panel and sets up initial locations.
    /// </summary>
    /// <param name="contType"></param>
    private void CreatePanel(ContainerType contType)
    {
        if (contType == ContainerType.Canvas)
        {
            _canvas = new Canvas();
            this.Content = _canvas;

            // have to set explicit locations for Canvas
            Canvas.SetTop(_wfh1, 100);
            Canvas.SetLeft(_wfh1, 100);
            Canvas.SetLeft(_wfh2, 150);
            Canvas.SetLeft(_wfh3, 300);

            // add host controls to canvas
            _canvas.Children.Add(_wfh1);
            _canvas.Children.Add(_wfh2);
            _canvas.Children.Add(_wfh3);
        }
        else if (contType == ContainerType.DockPanel)
        {
            _dp = new DockPanel();
            this.Content = _dp;

            // add host controls to dock panel
            _dp.Children.Add(_wfh1);
            _dp.Children.Add(_wfh2);
            _dp.Children.Add(_wfh3);
        }
        else if (contType == ContainerType.Grid)
        {
            _grid = new Grid();
            this.Content = _grid;

            // have to define Columns/Rows for Grid
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(_wfh1, 0);
            Grid.SetColumn(_wfh2, 1);
            Grid.SetColumn(_wfh3, 2);

            // add host controls to grid
            _grid.Children.Add(_wfh1);
            _grid.Children.Add(_wfh2);
            _grid.Children.Add(_wfh3);
        }
        else if (contType == ContainerType.StackPanel)
        {
            _stack = new StackPanel();
            this.Content = _stack;

            // add host controls to stack panel
            _stack.Children.Add(_wfh1);
            _stack.Children.Add(_wfh2);
            _stack.Children.Add(_wfh3);
        }
        else if (contType == ContainerType.WrapPanel)
        {
            _wrap = new WrapPanel();
            this.Content = _wrap;

            // add host controls to wrap panel
            _wrap.Children.Add(_wfh1);
            _wrap.Children.Add(_wfh2);
            _wrap.Children.Add(_wfh3);
        }
        else
        {
            // unknown ContainerType?
            throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
        }
    }

    // Helper function to put stuff into DataGridView control
    private void SetupDGV()
    {
        // do stuff with DGV
        _dgv.Columns.Add("Col1", "Column 1");
        _dgv.Columns.Add("Col2", "Column 2");
        _dgv.Rows.Add(4);
        _dgv.Dock = System.Windows.Forms.DockStyle.Fill;
    }

    private static void MyPause()
    {
        for (int i = 0; i < 2; i++)
        {
            WPFReflectBase.DoEvents();
            System.Threading.Thread.Sleep(5);
        }
    }

    #endregion

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WFH with single control.
//@ WFH with complex control.
//@ WFH with container control.
//@ WF control Enabled transitions (true -&gt; true, true-&gt;false, false -&gt;true)
//@ Regress Windows OS Bug 1420270 - Exception when setting IsEnabled false
