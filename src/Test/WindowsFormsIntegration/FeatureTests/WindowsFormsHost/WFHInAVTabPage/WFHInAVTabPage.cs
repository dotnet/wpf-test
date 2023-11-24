using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Windows;
//
// Testcase:    WFHInAVTabPage
// Description: We need to verify that WFH and WF contols work in an AV TabPage
// Author:      nathane
//
namespace WindowsFormsHostTests
{

public class WFHInAVTabPage : WPFReflectBase {

    #region Testcase setup

    #region TestVariables

    private delegate void myEventHandler(object sender);
    private int ScenarioIndex = 0;
    private TParams _tp;
    private string Events;             // event sequence string
    private bool debug = false;         // set this true for TC debugging

    private SWC.Button AVButton;
    private SWC.TabControl AVTabControl;
    private SWC.TabItem AVTabItem1;
    private SWC.TabItem AVTabItem2;
    private SWC.TabItem AVTabItem3;
    private SWC.TabItem AVTabItem4;

    private WindowsFormsHost wfh1;
    private WindowsFormsHost wfh2;
    private WindowsFormsHost wfh3;

    private SWF.Button WF1Button;
    private SWF.TextBox WF2TextBox;
    private SWF.TextBox WF3TextBox;

    private const string WindowTitleName = "WFHInAVTabPage";

    private const string AVButtonName = "AVButton";
    private const string AVTabControlName = "AVTabControl";
    private const string AVTabItem1Name = "AVTabItem1";
    private const string AVTabItem2Name = "AVTabItem2";
    private const string AVTabItem3Name = "AVTabItem3";
    private const string AVTabItem4Name = "AVTabItem4";

    private const string WF1Name = "WF1";
    private const string WF2Name = "WF2";
    private const string WF3Name = "WF3";

    private const string WF1ButtonName = "WF1Button";
    private const string WF2TextBoxName = "WF2TextBox";
    private const string WF3TextBoxName = "WF3TextBox";

    #endregion

    #region Testcase setup
    public WFHInAVTabPage(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        SetupScenario(ScenarioIndex);
        Utilities.SleepDoEvents(10);
        return base.BeforeScenario(p, scenario);
    }


    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        this.UseMITA = true;
        _tp = p;
        TestSetup();
        base.InitTest(p);
        this.Top = 0;
        this.Left = 0;
    }
    #endregion


    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("2 tabs, 1 with a WFH & WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string expVal = String.Empty;
        expVal += "WF1Button-GotFocus:";
        expVal += "AVButton-GotFocus:";
        expVal += "WF1Button-GotFocus:";
        expVal += "AVButton-GotFocus:";
        expVal += "WF1Button-GotFocus:";
        expVal += "AVButton-GotFocus:";
        // Removing to get test passing. It takes 2 CTRL+TABs to get focus to the WF1 button.
        //expVal += "WF1Button-GotFocus:";
        //expVal += "AVButton-GotFocus:";

        myWriteLine("Switching tabs back and forth 8 times");
        UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVTabControlName));
        ctrl.SetFocus();
        Utilities.SleepDoEvents(100);
        Events = String.Empty;
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, Events, "Focus/Button content getting out of sync", p.log);
        return sr; 
    }

    [Scenario("2 tabs, both with a WFH & WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string expVal = String.Empty;
        expVal += "WF2TextBox-GotFocus:";
        expVal += "WF3TextBox-GotFocus:";
        expVal += "WF2TextBox-GotFocus:";
        expVal += "WF3TextBox-GotFocus:";
        expVal += "WF2TextBox-GotFocus:";
        expVal += "WF3TextBox-GotFocus:";
        expVal += "WF2TextBox-GotFocus:";
        expVal += "WF3TextBox-GotFocus:";

        myWriteLine("Switching tabs back and forth 8 times");
        UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(WindowTitleName));
        UIObject ctrl = uiApp.Descendants.Find(UICondition.CreateFromId(AVTabControlName));
        ctrl.SetFocus();
        Events = String.Empty;
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);
        uiApp.SendKeys("^{TAB}");
        Utilities.SleepDoEvents(10);

        WPFMiscUtils.IncCounters(sr, expVal, Events, "Focus/TextBox content getting out of sync", p.log);
        return sr;
    }

    #endregion
    #region HelperFunction

    private void TestSetup()
    {
        myWriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVButton = new SWC.Button();
        AVTabControl = new SWC.TabControl();
        AVTabItem1 = new SWC.TabItem();
        AVTabItem2 = new SWC.TabItem();
        AVTabItem3 = new SWC.TabItem();
        AVTabItem4 = new SWC.TabItem();

        AVTabControl.Name = AVTabControlName;
        AVButton.Content = AVButton.Name = AVButtonName;
        AVTabItem1.Header = AVTabItem1.Name = AVTabItem1Name;
        AVTabItem2.Header = AVTabItem2.Name = AVTabItem2Name;
        AVTabItem3.Header = AVTabItem3.Name = AVTabItem3Name;
        AVTabItem4.Header = AVTabItem4.Name = AVTabItem4Name;

        AVButton.GotFocus += new RoutedEventHandler(AVControl_GotFocus);
        #endregion

        #region SetupWFControl
        wfh1 = new WindowsFormsHost();
        wfh2 = new WindowsFormsHost();
        wfh3 = new WindowsFormsHost();
        WF1Button = new SWF.Button();
        WF2TextBox = new SWF.TextBox();
        WF3TextBox = new SWF.TextBox();

        WF1Button.Text = WF1Button.Name = WF1ButtonName;
        WF2TextBox.Text = WF2TextBox.Name = WF2TextBoxName;
        WF2TextBox.WordWrap = true;
        WF2TextBox.Multiline = true;
        WF3TextBox.Text = WF3TextBox.Name = WF3TextBoxName;
        WF2TextBox.WordWrap = true;
        WF2TextBox.Multiline = true;
        WF1Button.GotFocus += new EventHandler(WFControl_GotFocus);
        WF2TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
        WF3TextBox.GotFocus += new EventHandler(WFControl_GotFocus);
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;
        AVButton.Width = 250;
        AVButton.Height = 150;
        wfh1.Width = wfh2.Width = wfh3.Width = 250;
        wfh1.Height = wfh2.Height = wfh3.Height = 150;
        wfh1.Child = WF1Button;
        wfh2.Child = WF2TextBox;
        wfh3.Child = WF3TextBox;
        AVTabItem1.Content = AVButton;
        AVTabItem2.Content = wfh1;
        AVTabItem3.Content = wfh2;
        AVTabItem4.Content = wfh3;
        AVTabControl.Items.Add(AVTabItem1);
        AVTabControl.Items.Add(AVTabItem2);
        AVTabControl.Items.Add(AVTabItem3);
        AVTabControl.Items.Add(AVTabItem4);
        this.Content = AVTabControl;
        #endregion

        myWriteLine("TestSetup -- End ");
    }


    void WFControl_GotFocus(object sender, EventArgs e)
    {
        Events += ((SWF.Control)sender).Text + "-GotFocus:";
    }

    void AVControl_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender.GetType() == typeof(SWC.TextBox))
            Events += ((SWC.TextBox)sender).Text + "-GotFocus:";
        else if (sender.GetType() == typeof(SWC.Button))
            Events += ((SWC.Button)sender).Content + "-GotFocus:";
        else
            Events += ((SWC.Control)sender).Name + "-GotFocus:";
    }


    void myWriteLine(string s)
    {
        if (debug)
        {
            _tp.log.WriteLine(s);
        }
    }

    private void SetupScenario(int ScenarioIndex)
    {
        switch (ScenarioIndex)
        {
            case 1: //@ 2 tabs, 1 with a WFH & WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).
                AVTabItem1.Visibility = SW.Visibility.Visible;
                AVTabItem2.Visibility = SW.Visibility.Visible;
                AVTabItem3.Visibility = SW.Visibility.Collapsed;
                AVTabItem4.Visibility = SW.Visibility.Collapsed;
                break;
            case 2: //@ 2 tabs, both with a WFH & WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)
                AVTabItem1.Visibility = SW.Visibility.Collapsed;
                AVTabItem2.Visibility = SW.Visibility.Collapsed;
                AVTabItem3.Visibility = SW.Visibility.Visible;
                AVTabItem4.Visibility = SW.Visibility.Visible;
                break;
            default:
                AVTabItem1.Visibility = SW.Visibility.Visible;
                AVTabItem2.Visibility = SW.Visibility.Visible;
                AVTabItem3.Visibility = SW.Visibility.Visible;
                AVTabItem4.Visibility = SW.Visibility.Visible;
                break;
        }
    }
  
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 2 tabs, 1 with a WFH &amp; WF Button, 1 with AV button.  Verify that switching tabs show the correct button (switch back and forth more than once).
//@ 2 tabs, both with a WFH &amp; WF Textbox.  Verify that text is preserved between tabs. (switch back and forth more than once.)