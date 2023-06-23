using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using SW=System.Windows;
using SWC=System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
//
// Testcase:    Cursor
// Description: Cursor propigation
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class Cursor : WPFReflectBase
{

    #region TestVariables

    private bool debug = false;         // set this true for TC debugging

    private SWC.ScrollViewer av_ScrollViewer;
    private SWC.StackPanel av_StackPanel;

    private SWF.Button wf_Button;
    private SWF.FlowLayoutPanel wf_FlowLayoutPanel;
    private SWF.TextBox wf_TextBox;
    private SWF.RichTextBox wf_RichTextBox;
    private SWF.ListBox wf_ListBox;
    private SWF.ComboBox wf_ComboBox;
    private SWF.CheckBox wf_CheckBox;
    private SWF.DataGridView wf_DataGridView;
    private SWF.UserControl wf_UserControl;
    private SWF.RadioButton wf_RadioButton;
    private SWF.DateTimePicker wf_DateTimePicker;
    private SWF.MonthCalendar wf_MonthCalendar;
    private SWF.PictureBox wf_PictureBox;
    private SWF.ProgressBar wf_ProgressBar;
    private SWF.VScrollBar wf_VScrollBar;
    private SWF.HScrollBar wf_HScrollBar;
    private SWF.TrackBar wf_TrackBar;
    private SWF.TreeView wf_TreeView;
    private SWF.ToolStrip wf_ToolStrip;
    private SWF.Label wf_Label;

    private WindowsFormsHost wfh;
    private static string WFButtonName = "WFButton";
    private static string WFFlowLayoutPanelName = "WFFlowLayoutPanelText";
    private static string WFTextBoxName = "WFTextBox";
    private static string WFRichTextBoxName = "WFRichTextBox";
    private static string WFListBoxName = "WFListBox";
    private static string WFComboBoxName = "WFComboBox";
    private static string WFCheckBoxName = "WFCheckBox";
    private static string WFDataGridViewName = "WFDataGridView";
    private static string WFUserControlName = "WFUserControl";
    private static string WFRadioButtonName = "WFRadioButton";
    private static string WFDateTimePickerName = "WFDateTimePicker";
    private static string WFMonthCalendarName = "WFMonthCalendar";
    private static string WFPictureBoxName = "WFPictureBox";
    private static string WFProgressBarName = "WFProgressBar";
    private static string WFVScrollBarName = "WFVScrollBar";
    private static string WFHScrollBarName = "WFHScrollBar";
    private static string WFTrackBarName = "WFTrackBar";
    private static string WFTreeViewName = "WFTreeView";
    private static string WFToolStripName = "WFToolStrip";
    private static string WFLabelName = "WFLabel";
    private static string WFHostName = "WFH";

    #endregion

    #region Testcase setup
    public Cursor(string[] args) : base(args) { }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        // hacks to get window to show up !!!
        this.Topmost = true;
        this.Topmost = false;
        this.WindowState = SW.WindowState.Maximized;
        this.WindowState = SW.WindowState.Normal;
        this.Width = 500;
        this.Height = 500; 

        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    
    [Scenario("Set WFH parent")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH parent";    
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");

        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, false);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, false);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, false);
        // Default Cursor Mapping
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, false);

        // Known bug !!!
        // when this bug is fixed, we can complete this Scenario !!!
        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1566844, "changes on the Cursor value on the WFH parent does not get propergated down to the WF control");

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH parent twice")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH parent twice";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");
        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, false);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, false);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, false);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, false);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, false);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, false);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, false);
        // Default Cursor Mapping
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, false);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, false);
        // Known bug !!!
        // when this bug is fixed, we can complete this Scenario !!!
        p.log.LogKnownBug(BugDb.WindowsOSBugs, 1566844, "changes on the Cursor value on the WFH parent does not get propergated down to the WF control");

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH child and make sure WFH parent doesn't change")]
    public ScenarioResult Scenario3(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH child and make sure WFH parent doesn't change";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");
        SetChildCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, false);
        SetChildCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, false);
        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH child then WFH parent and make sure WFH child doesn't change to WFH parent.")]
    public ScenarioResult Scenario4(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH child then WFH parent and make sure WFH child doesn't change to WFH parent.";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, false);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, false);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH parent then WFH child")]
    public ScenarioResult Scenario5(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH parent then WFH child";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, false);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, false);
        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH ")]
    public ScenarioResult Scenario6(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH ";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, true);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, true);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, true);
        // Default Cursor Mapping
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, true);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH twice")]
    public ScenarioResult Scenario7(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH twice";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Arrow, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, true);
        SetParentCursorTest(sr, p, SWI.Cursors.AppStarting, SWF.Cursors.AppStarting, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Cross, SWF.Cursors.Cross, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Hand, SWF.Cursors.Hand, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Help, SWF.Cursors.Help, true);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, true);
        SetParentCursorTest(sr, p, SWI.Cursors.IBeam, SWF.Cursors.IBeam, true);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, true);
        SetParentCursorTest(sr, p, SWI.Cursors.No, SWF.Cursors.No, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollAll, SWF.Cursors.NoMove2D, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollE, SWF.Cursors.PanEast, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNE, SWF.Cursors.PanNE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollN, SWF.Cursors.PanNorth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNS, SWF.Cursors.NoMoveVert, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollNW, SWF.Cursors.PanNW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollS, SWF.Cursors.PanSouth, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSE, SWF.Cursors.PanSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollSW, SWF.Cursors.PanSW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollW, SWF.Cursors.PanWest, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ScrollWE, SWF.Cursors.NoMoveHoriz, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeAll, SWF.Cursors.SizeAll, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNESW, SWF.Cursors.SizeNESW, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNS, SWF.Cursors.SizeNS, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeNWSE, SWF.Cursors.SizeNWSE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.SizeWE, SWF.Cursors.SizeWE, true);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.UpArrow, SWF.Cursors.UpArrow, true);
        // Default Cursor Mapping
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.None, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.Pen, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, true);
        SetParentCursorTest(sr, p, SWI.Cursors.ArrowCD, SWF.Cursors.Arrow, true);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH child and make sure WFH doesn't change")]
    public ScenarioResult Scenario8(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH child and make sure WFH doesn't change";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetChildCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, true);
        SetChildCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, true);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH child then WFH and make sure child doesn't change to WFH")]
    public ScenarioResult Scenario9(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH child then WFH and make sure child doesn't change to WFH";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, true);
        SetChildThenParentCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, true);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("Set WFH then child")]
    public ScenarioResult Scenario10(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Set WFH then child";
        TestSetup(p, TCText);
        MyPause();
        p.log.WriteLine(TCText + " - Test Run Start");

        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Arrow, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.AppStarting, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Cross, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Hand, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.Help, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.IBeam, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.No, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMove2D, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanEast, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNE, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNorth, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMoveVert, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanNW, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSouth, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSE, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanSW, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.PanWest, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.NoMoveHoriz, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeAll, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNESW, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNS, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeNWSE, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.SizeWE, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.UpArrow, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.HSplit, SWI.Cursors.Cross, true);
        SetParentThenChildCursorTest(sr, p, SWF.Cursors.VSplit, SWI.Cursors.Cross, true); 
        
        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    #endregion

    #region TestFunction

    /// <summary>
    /// Set the cursor on the WF controls and make sure the cursor on the parent does not get changed
    /// </summary>
    /// <param name="sr">ScenarioResult</param>
    /// <param name="p">TParams</param>
    /// <param name="wf_Cursor">WF control set to cursor</param>
    /// <param name="av_Cursor">parent set to cursor</param>
    /// <param name="bWFH">bWFH=true ==> parent=WFH, else parent=AVWindow</param>
    private void SetChildCursorTest(ScenarioResult sr, TParams p, SWF.Cursor wf_Cursor, SWI.Cursor av_Cursor, bool bWFH)
    {
        if (debug)
            p.log.WriteLine("Set WF Child to " + wf_Cursor.ToString());

        // initialize the parent cursor 
        if (bWFH)
            wfh.Cursor = av_Cursor;
        else
            this.Cursor = av_Cursor;

        foreach (SWF.Control WFControl in wf_FlowLayoutPanel.Controls)
        {
            //set the WF Child control cursor to
            WFControl.Cursor = wf_Cursor;

            //check if the WFH parent cursor stay the same
            WPFMiscUtils.IncCounters(sr, (bWFH ? wfh.Cursor : this.Cursor), 
                av_Cursor, "Parent Cursor changed to " + (bWFH ? wfh.Cursor.ToString() : this.Cursor.ToString()) + " improperly", p.log);
        }
    }

    /// <summary>
    /// Set the cursor on the Child WF controls then the Parent Cursor
    /// make sure the child cursor does not get changed by the Parent
    /// </summary>
    /// <param name="sr">ScenarioResult</param>
    /// <param name="p">TParams</param>
    /// <param name="wf_Cursor">WF control set to cursor</param>
    /// <param name="av_Cursor">parent set to cursor</param>
    /// <param name="bWFH">bWFH=true ==> parent=WFH, else parent=AVWindow</param>
    private void SetChildThenParentCursorTest(ScenarioResult sr, TParams p, SWF.Cursor wf_Cursor, SWI.Cursor av_Cursor, bool bWFH)
    {
        if (debug)
            p.log.WriteLine("Set WF Child to " + wf_Cursor.ToString() + " then WFH parent to " + av_Cursor.ToString());

        foreach (SWF.Control WFControl in wf_FlowLayoutPanel.Controls)
        {
            //set the WF Child control cursor to
            WFControl.Cursor = wf_Cursor;
            if (bWFH)
                wfh.Cursor = av_Cursor;
            else
                this.Cursor = av_Cursor;

            //check if the WFH parent cursor stay the same
            WPFMiscUtils.IncCounters(sr, wf_Cursor, WFControl.Cursor, "WFH Child cursor changed to " + WFControl.Cursor.ToString() + " improperly", p.log);
        }
    }

    /// <summary>
    /// Set the cursor on the Parent then to the Child WF controls
    /// make sure the parent cursor does not get changed by the child
    /// </summary>
    /// <param name="sr">ScenarioResult</param>
    /// <param name="p">TParams</param>
    /// <param name="wf_Cursor">WF control set to cursor</param>
    /// <param name="av_Cursor">parent set to cursor</param>
    /// <param name="bWFH">bWFH=true ==> parent=WFH, else parent=AVWindow</param>
    private void SetParentThenChildCursorTest(ScenarioResult sr, TParams p, SWF.Cursor wf_Cursor, SWI.Cursor av_Cursor, bool bWFH)
    {
        if (debug)
            p.log.WriteLine("Set WF Child to " + wf_Cursor.ToString() + " then WFH parent to " + av_Cursor.ToString());

        foreach (SWF.Control WFControl in wf_FlowLayoutPanel.Controls)
        {
            if (bWFH)
                wfh.Cursor = av_Cursor;
            else
                this.Cursor = av_Cursor;

            //set the WF Child control cursor to
            WFControl.Cursor = wf_Cursor;

            //check if the WFH parent cursor stay the same
            WPFMiscUtils.IncCounters(sr, wf_Cursor, WFControl.Cursor, "WFH Child cursor changed to " + WFControl.Cursor.ToString() + " improperly", p.log);
        }
    }
    /// <summary>
    /// Set the cursor on the parent and make sure the child WF control inherent the same cursor 
    /// exception: TextBox and RichTextBox will stay as IBeam Cursor
    /// </summary>
    /// <param name="sr">ScenarioResult</param>
    /// <param name="p">TParams</param>
    /// <param name="wf_Cursor">WF control set to cursor</param>
    /// <param name="av_Cursor">parent set to cursor</param>
    /// <param name="bWFH">bWFH=true ==> parent=WFH, else parent=AVWindow</param>
    private void SetParentCursorTest(ScenarioResult sr, TParams p, SWI.Cursor av_Cursor, SWF.Cursor wf_Cursor, bool bWFH)
    {
        if (debug)
            p.log.WriteLine("Set WF Control Parent Cursor to " + av_Cursor.ToString());

        if (bWFH)
            wfh.Cursor = av_Cursor;
        else
            this.Cursor  = av_Cursor;
        foreach (SWF.Control WFControl in wf_FlowLayoutPanel.Controls)
        {
            //Cursor for TextBox and RichTextBox need to stay as IBeam
            if (wf_TextBox == WFControl || wf_RichTextBox == WFControl)
                WPFMiscUtils.IncCounters(sr, SWF.Cursors.IBeam, WFControl.Cursor, "Control " + WFControl.GetType().ToString() + " - Cursor on Control not set properly", p.log);
            else
                WPFMiscUtils.IncCounters(sr, wf_Cursor, WFControl.Cursor, "Control " + WFControl.GetType().ToString() + " - Cursor on Control not set properly", p.log);
        }
    }

    #endregion


    #region HelperFunction
    private static void MyPause()
    {
        WPFReflectBase.DoEvents();
        System.Threading.Thread.Sleep(200);
    }

    // Helper function to set up app for particular Scenario
    private void TestSetup(TParams p, string strTC)
    {
        p.log.WriteLine(strTC + " - TestSetup -- Start ");

        this.Title = strTC;
        av_ScrollViewer = new SWC.ScrollViewer();
        av_StackPanel = new SWC.StackPanel();
        av_ScrollViewer.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
        av_ScrollViewer.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto;
        wfh = new WindowsFormsHost();
        wf_Button = new Button();
        wf_FlowLayoutPanel = new FlowLayoutPanel();
        wf_TextBox = new TextBox();
        wf_RichTextBox = new RichTextBox();
        wf_ListBox = new ListBox();
        wf_ComboBox = new ComboBox();
        wf_CheckBox = new CheckBox();
        wf_DataGridView = new DataGridView();
        wf_UserControl = new UserControl();
        wf_RadioButton = new RadioButton();
        wf_DateTimePicker = new DateTimePicker();
        wf_MonthCalendar = new MonthCalendar();
        wf_PictureBox = new PictureBox();
        wf_ProgressBar = new ProgressBar();
        wf_VScrollBar = new VScrollBar();
        wf_HScrollBar = new HScrollBar();
        wf_TrackBar = new TrackBar();
        wf_TreeView = new TreeView();
        wf_ToolStrip = new ToolStrip();
        wf_Label = new Label();

        wf_Button.Name = WFButtonName;
        wf_FlowLayoutPanel.Name = WFFlowLayoutPanelName;
        wf_TextBox.Name = WFTextBoxName;
        wf_RichTextBox.Name = WFRichTextBoxName;
        wf_ListBox.Name = WFListBoxName;
        wf_ComboBox.Name = WFComboBoxName;
        wf_CheckBox.Name = WFCheckBoxName;
        wf_DataGridView.Name = WFDataGridViewName;
        wf_UserControl.Name = WFUserControlName;
        wf_RadioButton.Name = WFRadioButtonName;
        wf_DateTimePicker.Name = WFDateTimePickerName;
        wf_MonthCalendar.Name = WFMonthCalendarName;
        wf_PictureBox.Name = WFPictureBoxName;
        wf_ProgressBar.Name = WFProgressBarName;
        wf_VScrollBar.Name = WFVScrollBarName;
        wf_HScrollBar.Name = WFHScrollBarName;
        wf_TrackBar.Name = WFTrackBarName;
        wf_TreeView.Name = WFTreeViewName;
        wf_ToolStrip.Name = WFToolStripName;
        wf_Label.Name = WFLabelName;
        wfh.Name = WFHostName;

        wf_Button.Text = WFButtonName;
        wf_TextBox.Text = WFTextBoxName;
        wf_RichTextBox.Text = WFRichTextBoxName;
        wf_CheckBox.Text = WFCheckBoxName;
        wf_RadioButton.Text = WFRadioButtonName;
        wf_Label.Text = WFLabelName;
        wf_VScrollBar.Minimum = 0;
        wf_VScrollBar.Minimum = 100;
        wf_HScrollBar.Minimum = 0;
        wf_HScrollBar.Maximum = 100;
        wf_ListBox.Items.Add("List Item 1");
        wf_ListBox.Items.Add("List Item 2");
        wf_ListBox.Items.Add("List Item 3");
        wf_ComboBox.Items.Add("Combo Box Item 1");
        wf_ComboBox.Items.Add("Combo Box Item 2");
        wf_ComboBox.Items.Add("Combo Box Item 3");
        wf_DataGridView.Columns.Add("Col1", "Column 1");
        wf_DataGridView.Columns.Add("Col2", "Column 2");
        wf_DataGridView.Rows.Add(4);
        wf_DataGridView.Dock = SWF.DockStyle.Fill;
        wf_UserControl.Dock = SWF.DockStyle.Fill;

        wf_FlowLayoutPanel.Controls.Add(wf_Button);
        wf_FlowLayoutPanel.Controls.Add(wf_TextBox);
        wf_FlowLayoutPanel.Controls.Add(wf_RichTextBox);
        wf_FlowLayoutPanel.Controls.Add(wf_ListBox);
        wf_FlowLayoutPanel.Controls.Add(wf_ComboBox);
        wf_FlowLayoutPanel.Controls.Add(wf_CheckBox);
        wf_FlowLayoutPanel.Controls.Add(wf_DataGridView);
        wf_FlowLayoutPanel.Controls.Add(wf_UserControl);
        wf_FlowLayoutPanel.Controls.Add(wf_RadioButton);
        wf_FlowLayoutPanel.Controls.Add(wf_DateTimePicker);
        wf_FlowLayoutPanel.Controls.Add(wf_MonthCalendar);
        wf_FlowLayoutPanel.Controls.Add(wf_PictureBox);
        wf_FlowLayoutPanel.Controls.Add(wf_ProgressBar);
        wf_FlowLayoutPanel.Controls.Add(wf_VScrollBar);
        wf_FlowLayoutPanel.Controls.Add(wf_HScrollBar);
        wf_FlowLayoutPanel.Controls.Add(wf_TrackBar);
        wf_FlowLayoutPanel.Controls.Add(wf_TreeView);
        wf_FlowLayoutPanel.Controls.Add(wf_ToolStrip);
        wf_FlowLayoutPanel.Controls.Add(wf_Label);

        wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        wfh.Child = wf_FlowLayoutPanel;

        av_StackPanel.Children.Add(wfh);
        av_ScrollViewer.Content = av_StackPanel;
        this.Content = av_ScrollViewer;
        p.log.WriteLine(strTC + " - TestSetup -- End ");
    }
    #endregion
}
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Set WFH parent

//@ Set WFH parent twice

//@ Set WFH child and make sure WFH parent doesn't change

//@ Set WFH child then WFH parent and make sure WFH child doesn't change to WFH parent.

//@ Set WFH parent then WFH child

//@ Set WFH 

//@ Set WFH twice

//@ Set WFH child and make sure WFH doesn't change

//@ Set WFH child then WFH and make sure child doesn't change to WFH

//@ Set WFH then child