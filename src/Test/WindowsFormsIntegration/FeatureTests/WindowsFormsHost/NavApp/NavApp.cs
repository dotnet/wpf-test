using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;
using System.Threading;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;
using SWN = System.Windows.Navigation;
using SWT = System.Windows.Threading;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
//
// Testcase:    NavApp
// Description: Verify that dialog keys (i.e. Enter, ESC) work for Modal and non Modal WF
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class NavApp : WPFReflectBase
{
    #region TestVariables

    private delegate void myEventHandler(object sender);

    private int ScenarioIndex = 0;
    private bool debug = false;         // set this true for TC debugging
    private TParams _tp;
    private string Events = String.Empty;
    private UIObject uiApp;
    private bool bCanGoBack = false;
    private bool bCanGoForward = false;
    private string CurrentPage = String.Empty;
    private enum DispatchFunction { GetCurrentPage, CanGoBack, CanGoForward };


    private SWN.NavigationWindow AVNavigationWindow;
    private SWC.Page AVPage1;
    private SWC.Page AVPage2;
    private SWC.StackPanel Page1AVStackPanel;
    private SWC.StackPanel Page2AVStackPanel;
    private SWC.Button Page1AVButton;
    private SWC.Button Page2AVButton;
    private SWC.TextBox Page1AVTextBox;
    private SWC.TextBox Page2AVTextBox;

    private WindowsFormsHost Page1wfh1;
    private WindowsFormsHost Page1wfh2;
    private WindowsFormsHost Page2wfh1;
    private WindowsFormsHost Page2wfh2;
    private SWF.Button Page1WF1Button;
    private SWF.TextBox Page1WF2TextBox;
    private SWF.Button Page2WF1Button;
    private SWF.TextBox Page2WF2TextBox;

    private const string WindowTitleName = "NavApp";
    private const string AVNavigationWindowName = "AVNavigationWindow";
    private const string AVPage1Name = "AVPage1";
    private const string AVPage2Name = "AVPage2";
    private const string Page1AVStackPanelName = "Page1AVStackPanel";
    private const string Page2AVStackPanelName = "Page2AVStackPanel";
    private const string Page1AVButtonName = "Page1AVButton";
    private const string Page2AVButtonName = "Page2AVButton";
    private const string Page1AVTextBoxName = "Page1AVTextBox";
    private const string Page2AVTextBoxName = "Page2AVTextBox";

    private const string Page1wfh1Name = "Page1wfh1";
    private const string Page1wfh2Name = "Page1wfh2";
    private const string Page2wfh1Name = "Page2wfh1";
    private const string Page2wfh2Name = "Page2wfh2";

    private const string Page1WF1ButtonName = "Page1WF1Button";
    private const string Page1WF2TextBoxName = "Page1WF2TextBox";

    private const string Page2WF1ButtonName = "Page2WF1Button";
    private const string Page2WF2TextBoxName = "Page2WF2TextBox";

    private const string TBValidationMsg = "TextBox Validation is being called";

    #endregion

    #region Testcase setup
    public NavApp(string[] args) : base(args) { }


    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        ScenarioIndex = Convert.ToInt32(scenario.Name.Substring(8));
        ScenarioSetup(ScenarioIndex);
	Utilities.SleepDoEvents(10);
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        AVNavigationWindow.Close();
        base.AfterScenario(p, scenario, result);
    }
    protected override void InitTest(TParams p)
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.Visibility = System.Windows.Visibility.Hidden;
        _tp = p;
        this.UseMITA = true;
        base.InitTest(p);
    }
    #endregion


    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Navigate to and from a page that has a WFH + WF Control and verify that the control works")]
    public ScenarioResult Scenario1(TParams p) 
    {
        String expVal = String.Empty;
        Events = String.Empty;
        ScenarioResult sr = new ScenarioResult();
	Utilities.SleepDoEvents(10);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
        UIObject ctrlForward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
        UIObject ctrlBackward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
        UIObject ctrlAVBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
        UIObject ctrlWFBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF1ButtonName));
	Utilities.SleepDoEvents(10);
        myWriteLine("both Back and Forward arrows should be disabled initially");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == false, "Back button is enabled");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled==false, "Forward button is enabled");

        myWriteLine("click on Page1AVButton to bring up the second page");
        ctrlAVBtn1.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(20);

        WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == true, "Back button does not get enabled");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled==false, "Forward button does not get disabled");

        UIObject ctrlWFBtn2 = uiApp.Descendants.Find(UICondition.CreateFromId(Page2WF1ButtonName));
        UIObject ctrlAVBtn2 = uiApp.Descendants.Find(UICondition.CreateFromId(Page2AVButtonName));

        myWriteLine("click forward and backward and click on WF button 5 times");
        for (int i = 0; i < 5; i++)
        {
            myWriteLine("Backward");
            ctrlBackward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            expVal += AVPage1Name + "::";
            Events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
            Utilities.SleepDoEvents(5);

            myWriteLine("Click on the WF button - should be on Page 1"); // should be on Page1 at all time
            expVal += Page1WF1ButtonName + "::";
            if (ctrlWFBtn1.IsOffscreen == false)
            {
                ctrlWFBtn1.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
            if (ctrlWFBtn2.IsOffscreen == false)
            {
                ctrlWFBtn2.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
            
            myWriteLine("Forward");
            ctrlForward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            expVal += AVPage2Name + "::";
            Events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
            Utilities.SleepDoEvents(5);

            myWriteLine("Click on the WF button - should be on Page 2"); // should be on Page2 at all time
            expVal += Page2WF1ButtonName + "::"; 

            if (ctrlWFBtn1.IsOffscreen == false)
            {
                ctrlWFBtn1.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
            if (ctrlWFBtn2.IsOffscreen == false)
            {
                ctrlWFBtn2.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }

        }
        //p.log.LogKnownBug(BugDb.WindowsOSBugs, 1559116, "Navigation Arrows are disabled after focus change from AV control to WF control and then click on the Left/Right Navigation arrow");
        //p.log.LogKnownBug(BugDb.WindowsOSBugs, 1559303, "Navigation arrows are not enabled when the navigation is initiated from a WFH control while the focus is on a AV control");
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Page displayed not in the right order", p.log);
        return sr;
    }

    [Scenario("Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.")]
    public ScenarioResult Scenario2(TParams p) 
    {
        String expVal = String.Empty;
        Events = String.Empty;
        ScenarioResult sr = new ScenarioResult();
	Utilities.SleepDoEvents(5);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
        UIObject ctrlForward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
        UIObject ctrlBackward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
        UIObject ctrlAVBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
        UIObject ctrlWFtb = uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF2TextBoxName));
        MitaControl.Edit editWFtb= new MitaControl.Edit(ctrlWFtb);

        myWriteLine("click on Page1AVButton to bring up the second page");
        ctrlAVBtn1.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);

        myWriteLine("Go Back to Page1");
        ctrlBackward.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);

        editWFtb.SetValue(Page1WF2TextBoxName + Page1WF2TextBoxName);
        Utilities.SleepDoEvents(5);

        myWriteLine("Go to Page 2 the back to Page1");
        ctrlForward.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);
        ctrlBackward.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);

        myWriteLine("Check on the Text in the WF TextBox");
        WPFMiscUtils.IncCounters(sr, Page1WF2TextBoxName + Page1WF2TextBoxName, editWFtb.Value, "WF TextBox content is not being preserved", p.log);
        return sr;
    }

    [Scenario("WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).")]
    public ScenarioResult Scenario3(TParams p) 
    {
        String expVal = String.Empty;
        Events = String.Empty;
        ScenarioResult sr = new ScenarioResult();
	Utilities.SleepDoEvents(5);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
        UIObject ctrlForward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
        UIObject ctrlBackward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
        UIObject ctrlAVBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
        UIObject ctrlWFtb = uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF2TextBoxName));
        MitaControl.Edit editWFtb = new MitaControl.Edit(ctrlWFtb);

        ctrlWFtb.SendKeys(Page1WF2TextBoxName + Page1WF2TextBoxName);
        Utilities.SleepDoEvents(5);
        ctrlWFtb.Click(PointerButtons.Primary);

        myWriteLine("click on Page1AVButton to bring up the second page");
        ctrlAVBtn1.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);

        myWriteLine("Go Back to Page1");
        ctrlBackward.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);
        
        myWriteLine("Check on the Text in the WF TextBox");
        WPFMiscUtils.IncCounters(sr, Page1WF2TextBoxName + Page1WF2TextBoxName, editWFtb.Value, "WF TextBox content is not being preserved", p.log);

        myWriteLine("Make sure TB Validation being called once");
        WPFMiscUtils.IncCounters(sr, TBValidationMsg, Events, "WF TextBox Validating event not being called", p.log);

        return sr;
    }

    [Scenario("Navigate to another page form a WF control in a WFH.")]
    public ScenarioResult Scenario4(TParams p) 
    {
        String expVal = String.Empty;
        Events = String.Empty;
        ScenarioResult sr = new ScenarioResult();
	Utilities.SleepDoEvents(5);
        uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AVNavigationWindowName));
        UIObject ctrlForward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseForward"));
        UIObject ctrlBackward = uiApp.Descendants.Find(UICondition.CreateFromId("BrowseBack"));
        UIObject ctrlAVBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1AVButtonName));
        UIObject ctrlWFBtn1 = uiApp.Descendants.Find(UICondition.CreateFromId(Page1WF1ButtonName));

        myWriteLine("both Back and Forward arrows should be disabled initially");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == false, "Back button is enabled");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled == false, "Forward button is enabled");

        myWriteLine("click on Page1AVButton to bring up the second page");
        ctrlWFBtn1.Click(PointerButtons.Primary);
        Utilities.SleepDoEvents(5);
        expVal += AVPage2Name + "::";
        Events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
        Utilities.SleepDoEvents(5);
        WPFMiscUtils.IncCounters(sr, p.log, ctrlBackward.IsEnabled == true, "Back button does not get enabled");
        WPFMiscUtils.IncCounters(sr, p.log, ctrlForward.IsEnabled == false, "Forward button does not get disabled");

        UIObject ctrlWFBtn2 = uiApp.Descendants.Find(UICondition.CreateFromId(Page2WF1ButtonName));
        UIObject ctrlAVBtn2 = uiApp.Descendants.Find(UICondition.CreateFromId(Page2AVButtonName));

        myWriteLine("click forward and backward and click on AV button 5 times");
        for (int i = 0; i < 5; i++)
        {
            myWriteLine("Backward");
            ctrlBackward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            expVal += AVPage1Name + "::";
            Events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
            Utilities.SleepDoEvents(5);

            myWriteLine("Click on the AV button - should be on Page 1"); // should be on Page1 at all time
            expVal += Page1AVButtonName + "::";
            if (ctrlAVBtn1.IsOffscreen == false)
            {
                ctrlAVBtn1.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
            if (ctrlAVBtn2.IsOffscreen == false)
            {
                ctrlAVBtn2.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }

            myWriteLine("Forward");
            ctrlForward.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(5);
            expVal += AVPage2Name + "::";
            Events += (String)myDispatcher(DispatchFunction.GetCurrentPage) + "::";
            Utilities.SleepDoEvents(5);

            myWriteLine("Click on the AV button - should be on Page 2"); // should be on Page2 at all time
            expVal += Page2AVButtonName + "::";

            if (ctrlAVBtn1.IsOffscreen == false)
            {
                ctrlAVBtn1.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
            if (ctrlAVBtn2.IsOffscreen == false)
            {
                ctrlAVBtn2.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(5);
            }
        }
        //p.log.LogKnownBug(BugDb.WindowsOSBugs, 1559116, "Navigation Arrows are disabled after focus change from AV control to WF control and then click on the Left/Right Navigation arrow");
        //p.log.LogKnownBug(BugDb.WindowsOSBugs, 1559303, "Navigation arrows are not enabled when the navigation is initiated from a WFH control while the focus is on a AV control");
        WPFMiscUtils.IncCounters(sr, expVal, Events, "Page displayed not in the right order", p.log);
        return sr;
    }

    #endregion


    #region HelperFunction

    private void TestSetup()
    {
        myWriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVNavigationWindow = new System.Windows.Navigation.NavigationWindow();
        AVPage1 = new System.Windows.Controls.Page();
        AVPage2 = new System.Windows.Controls.Page();
        Page1AVStackPanel = new System.Windows.Controls.StackPanel();
        Page2AVStackPanel = new System.Windows.Controls.StackPanel();
        Page1AVButton = new System.Windows.Controls.Button();
        Page2AVButton = new System.Windows.Controls.Button();
        Page1AVTextBox = new System.Windows.Controls.TextBox();
        Page2AVTextBox = new System.Windows.Controls.TextBox();

        AVNavigationWindow.Title = AVNavigationWindow.Name = AVNavigationWindowName;
        AVPage1.Name = AVPage1.Title = AVPage1Name;
        AVPage2.Name = AVPage2.Title = AVPage2Name;
        Page1AVStackPanel.Name = Page1AVStackPanelName;
        Page2AVStackPanel.Name = Page2AVStackPanelName;
        Page1AVButton.Content = Page1AVButton.Name = Page1AVButtonName;
        Page2AVButton.Content = Page2AVButton.Name = Page2AVButtonName;
        Page1AVTextBox.Text = Page1AVTextBox.Name = Page1AVTextBoxName;
        Page2AVTextBox.Text = Page2AVTextBox.Name = Page2AVTextBoxName;
        Page1AVTextBox.Width = Page2AVTextBox.Width = 300;
        Page1AVTextBox.Height = Page2AVTextBox.Height = 150;
        #endregion

        #region SetupWFControl
        Page1wfh1 = new WindowsFormsHost();
        Page1wfh2 = new WindowsFormsHost();
        Page2wfh1 = new WindowsFormsHost();
        Page2wfh2 = new WindowsFormsHost();

        Page1WF1Button = new System.Windows.Forms.Button();
        Page2WF1Button = new System.Windows.Forms.Button();
        Page1WF2TextBox = new System.Windows.Forms.TextBox();
        Page2WF2TextBox = new System.Windows.Forms.TextBox();

        Page1wfh1.Name = Page1wfh1Name;
        Page1wfh2.Name = Page1wfh2Name;
        Page2wfh1.Name = Page2wfh1Name;
        Page2wfh2.Name = Page2wfh2Name;
        Page1WF1Button.Text = Page1WF1Button.Name = Page1WF1ButtonName;
        Page2WF1Button.Text = Page2WF1Button.Name = Page2WF1ButtonName;
        Page1WF2TextBox.Text = Page1WF2TextBox.Name = Page1WF2TextBoxName;
        Page2WF2TextBox.Text = Page2WF2TextBox.Name = Page2WF2TextBoxName;

        Page1wfh1.Child = Page1WF1Button;
        Page2wfh1.Child = Page2WF1Button;
        Page1wfh2.Child = Page1WF2TextBox;
        Page2wfh2.Child = Page2WF2TextBox;

        Page1wfh1.Width = Page2wfh1.Width = Page1wfh2.Width = Page2wfh2.Width = 300;
        Page1wfh1.Height = Page2wfh1.Height = Page1wfh2.Height = Page2wfh2.Height = 200;
        #endregion

        #region LayoutWindow
        Page1AVStackPanel.Children.Add(Page1wfh1);
        Page1AVStackPanel.Children.Add(Page1wfh2);
        Page1AVStackPanel.Children.Add(Page1AVTextBox);
        Page1AVStackPanel.Children.Add(Page1AVButton);

        Page2AVStackPanel.Children.Add(Page2wfh1);
        Page2AVStackPanel.Children.Add(Page2wfh2);
        Page2AVStackPanel.Children.Add(Page2AVTextBox);
        Page2AVStackPanel.Children.Add(Page2AVButton);

        AVPage1.Content = Page1AVStackPanel;
        AVPage2.Content = Page2AVStackPanel;

        AVNavigationWindow.Content = AVPage1;
        AVNavigationWindow.ShowsNavigationUI = true;
        AVNavigationWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        AVNavigationWindow.Show();
        #endregion

        myWriteLine("TestSetup -- End ");
    }

    #region EventHandler
    void WFButton_Click(object sender, EventArgs e)
    {
        Events += ((SWF.Button)sender).Name + "::";
    }

    void Page1WF1Button_Click(object sender, EventArgs e)
    {
        AVNavigationWindow.Content = AVPage2;
    }

    void AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Events += ((SWC.Button)sender).Name + "::";
    }

    void Page1AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        AVNavigationWindow.Content = AVPage2;
    }

    void Page1WF2TextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Events += TBValidationMsg;
        this.Title = Events;
    }
    #endregion

    void CanGoBack(object sender)
    {
        bCanGoBack = ((SWN.NavigationWindow)sender).CanGoBack;
    }

    void CanGoForward(object sender)
    {
        bCanGoForward = ((SWN.NavigationWindow)sender).CanGoForward;
    }

    void GetCurrentPage(object sender)
    {
        SWC.Page pg = ((SWN.NavigationWindow)sender).Content as SWC.Page;
        CurrentPage = pg.Name.ToString();
    }

    object myDispatcher(DispatchFunction df)
    {
        object retObj = null;
        switch (df)
        {
            case DispatchFunction.GetCurrentPage:
                AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(GetCurrentPage), AVNavigationWindow);
                retObj = CurrentPage;
                break;
            case DispatchFunction.CanGoBack:
                AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(CanGoBack), AVNavigationWindow);
                retObj = bCanGoBack;
                break;
            case DispatchFunction.CanGoForward:
                AVNavigationWindow.Dispatcher.Invoke(SWT.DispatcherPriority.Normal, new myEventHandler(CanGoForward), AVNavigationWindow);
                retObj = bCanGoForward;
                break;
        }
        return retObj;
    }

    void myWriteLine(string s)
    {
        if (debug)
        {
            myWriteLine(s);
        }
    }

    private void ScenarioSetup(int ScenarioIndex)
    {
        switch (ScenarioIndex)
        {
            case 1: //@ Navigate to and from a page that has a WFH + WF Control and verify that the control works
                TestSetup();
                Page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page1AVButton.Visibility = System.Windows.Visibility.Visible;
                Page2AVButton.Visibility = System.Windows.Visibility.Visible;
                Page1wfh1.Visibility = System.Windows.Visibility.Visible;
                Page2wfh1.Visibility = System.Windows.Visibility.Visible;
                Page1wfh2.Visibility = System.Windows.Visibility.Collapsed;
                Page2wfh2.Visibility = System.Windows.Visibility.Collapsed;

                Page1WF1Button.Click += new EventHandler(WFButton_Click);
                Page2WF1Button.Click += new EventHandler(WFButton_Click);
                Page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                Page2AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                break;
            case 2: //@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.
                TestSetup();
                Page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page1wfh1.Visibility = System.Windows.Visibility.Collapsed;
                Page2wfh1.Visibility = System.Windows.Visibility.Collapsed;
                Page1wfh2.Visibility = System.Windows.Visibility.Visible;
                Page2wfh2.Visibility = System.Windows.Visibility.Visible;
                Page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                break;
            case 3: //@ WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).
                TestSetup();
                Page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page1wfh1.Visibility = System.Windows.Visibility.Collapsed;
                Page2wfh1.Visibility = System.Windows.Visibility.Collapsed;
                Page1wfh2.Visibility = System.Windows.Visibility.Visible;
                Page2wfh2.Visibility = System.Windows.Visibility.Visible;
                Page1AVButton.Click += new System.Windows.RoutedEventHandler(Page1AVButton_Click);
                Page1WF2TextBox.Validating += new System.ComponentModel.CancelEventHandler(Page1WF2TextBox_Validating);
                break;
            case 4: //@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.
                TestSetup();
                Page1AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page2AVTextBox.Visibility = System.Windows.Visibility.Collapsed;
                Page1AVButton.Visibility = System.Windows.Visibility.Visible;
                Page2AVButton.Visibility = System.Windows.Visibility.Visible;
                Page1wfh1.Visibility = System.Windows.Visibility.Visible;
                Page2wfh1.Visibility = System.Windows.Visibility.Visible;
                Page1wfh2.Visibility = System.Windows.Visibility.Collapsed;
                Page2wfh2.Visibility = System.Windows.Visibility.Collapsed;

                Page1WF1Button.Click += new EventHandler(Page1WF1Button_Click); // click to show the second page
                Page2WF1Button.Click += new EventHandler(WFButton_Click); // click to show the second page
                Page1AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                Page2AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
                break;
            default:
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
//@ Navigate to and from a page that has a WFH + WF Control and verify that the control works

//@ Verify that Text in a WF Textbox on Page1 remians when navigating to page 2 and back.

//@ WF Validation on a WF Text box in an AV Page.  Navigate to another page and verify Validation doesn't break the app (we don't expect validation to work here).

//@ Navigate to another page form a WF control in a WFH.