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
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;
using System.Threading;

//
// Testcase:    AltTab
// Description: Verify that Alt+TAB works and that Focus is good
// Author:      a-rickyt
//
namespace WindowsFormsHostTests
{

public class AltTab : WPFReflectBase
{
    #region Testcase setup
    
    Edit _edit1;
    Edit _edit2;
    SWC.StackPanel stackPanel;
    WindowsFormsHost winformsHost1;
    SWF.Button wfButton1;
    SWF.TextBox wfTextBox1;
    SWF.FlowLayoutPanel panel;
    System.Threading.Thread NewThread;

    public AltTab(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {
        this.Title = "AltTab";
        this.UseMITA = true;

        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        panel = new SWF.FlowLayoutPanel();

        wfButton1 = new SWF.Button();
        wfButton1.Name = "wfButton1";
        wfButton1.Text = "WinForm Button";
        panel.Controls.Add(wfButton1);

        wfTextBox1 = new SWF.TextBox();
        wfTextBox1.Name = "wfTextBox1";
        wfTextBox1.Text = "WinForm TextBox";
        panel.Controls.Add(wfTextBox1);

        winformsHost1 = new WindowsFormsHost();
        winformsHost1.Child = panel;
        stackPanel = new SWC.StackPanel();
        stackPanel.Children.Add(winformsHost1);
        this.Content = stackPanel;

        if (scenario.Name == "Scenario2")
        {
            NewThread = new System.Threading.Thread(new ThreadStart(ShowModalForm.ShowIt));
            NewThread.SetApartmentState(ApartmentState.STA);
            NewThread.Start();
            Utilities.SleepDoEvents(10);
        }

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        if (scenario.Name == "Scenario2")
        {
            NewThread.Abort();
        }

        base.AfterScenario(p, scenario, result);
    }

    #endregion


    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Verify focus preserved after Alt+TAB.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "AltTab", "wfButton1", "wfTextBox1"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(2);

        //Check WinForm Button
        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(5);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus, initial state. HasKeyboardFocus=" + 
            _edit1.HasKeyboardFocus);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);
        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);
        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        //WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
        //    "Failed at wfButton1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" + 
        //    _edit1.HasKeyboardFocus);

        //Check WinForm TextBox
        _edit2.Click(PointerButtons.Primary); //click on WinForm TextBox
        Utilities.SleepDoEvents(5);

	//Resolution: 1648389 Duplicate of Won't Fix Bug
        //WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus, 
        //    "Failed at wfTextBox1.HasKeyboardFocus, initial state. HasKeyboardFocus=" + 
        //    _edit2.HasKeyboardFocus);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);
        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        //WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
        //    "Failed at wfTextBox1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" + 
        //    _edit2.HasKeyboardFocus);

        //Resolution: Duplicate of Won't Fix Bug
        if (sr.FailCount > 0)
        {
            p.log.LogKnownBug(BugDb.WindowsOSBugs, 1648389, 
                "Focus is lost when returning from a non-modal window");
        }

        return sr; 
    }

    [Scenario("Verify Focus preserved after Alt+TAB from an AV Modal Dialog.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        if (!GetEditControls(p, "ModalWinForm", "wfButton", "wfTextBox"))
        {
            return new ScenarioResult(false);
        }
        Utilities.SleepDoEvents(2);

        //Check WinForm Button
        _edit1.Click(PointerButtons.Primary); //click on WinForm button
        Utilities.SleepDoEvents(5);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus, initial state. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        WPFMiscUtils.IncCounters(sr, p.log, _edit1.HasKeyboardFocus,
            "Failed at wfButton1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" +
            _edit1.HasKeyboardFocus);

        //Check WinForm TextBox
        _edit2.Click(PointerButtons.Primary); //click on WinForm TextBox
        Utilities.SleepDoEvents(5);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at wfTextBox1.HasKeyboardFocus, initial state. HasKeyboardFocus=" +
            _edit2.HasKeyboardFocus);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        Keyboard.Instance.SendKeys("%{TAB}");
        Utilities.SleepDoEvents(5);

        WPFMiscUtils.IncCounters(sr, p.log, _edit2.HasKeyboardFocus,
            "Failed at wfTextBox1.HasKeyboardFocus, after Alt-Tab. HasKeyboardFocus=" +
            _edit2.HasKeyboardFocus);

        if (sr.FailCount > 0)
        {
            p.log.LogKnownBug(BugDb.WindowsOSBugs, 1562663,
                "Keyboard focus is lost when returning from a modal dialog");
        }

        return sr; 
    }

    [Scenario("WindowsOS Bug 1527202.")]
    public ScenarioResult Scenario3(TParams p)
    {
        p.log.WriteLine("WindowsOS Bug 1527202 is checked in Scenario 1");
        return new ScenarioResult(true);
    }

    #endregion

    #region Helper Functions 

    bool GetEditControls(TParams p, string window1, string control1, string control2)
    {
        try
        {
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
            BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control1));
            BreadthFirstDescendantsNavigator bfTB2 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId(control2));
            _edit1 = new Edit(bfTB1[0]);
            _edit2 = new Edit(bfTB2[0]);
            return true;
        }
        catch (Exception ex)
        {
            p.log.WriteLine("Failed to get Mita wrapper controls. " + ex.ToString());
            return false;
        }
    }
  
    #endregion

    class ShowModalForm
    {
        public static SW.Window avWindow = new SW.Window();
        static SWF.FlowLayoutPanel pane = new SWF.FlowLayoutPanel();

        public static void ShowIt()
        {
            WindowsFormsHost host = new WindowsFormsHost();
            SWF.Button wfButton = new SWF.Button();
            SWF.TextBox wfTextbox = new SWF.TextBox();

            wfButton.Name = "wfButton";
            wfButton.Text = "WinForm Button";
            pane.Controls.Add(wfButton);

            wfTextbox.Name = "wfTextBox";
            wfTextbox.Text = "WinForm TextBox";
            pane.Controls.Add(wfTextbox);

            host.Child = pane;

            avWindow.Title = "ModalWinForm";
            avWindow.Content = host;
            avWindow.ShowDialog();
        }
    }
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify focus preserved after Alt+TAB
//@ Verify Focus preserved after Alt+TAB from an AV Modal Dialog
//@ WindowsOS Bug 1527202
