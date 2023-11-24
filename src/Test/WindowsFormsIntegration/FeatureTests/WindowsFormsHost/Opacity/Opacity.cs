using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;


/// <Testcase>Opacity</Testcase>
/// <summary>
/// Description: When Opacity is set on the WFH, it's child should honor it
/// </summary>
/// <History>
/// pachan 3/20/2006 Created
/// </History>
namespace WindowsFormsHostTests
{

public class Opacity : WPFReflectBase {

    #region TestVariables

    private WindowsFormsHost wfh;
    private SWF.FlowLayoutPanel wf_FlowLayoutPanel;
    private SWF.Button wf_Button;

    private static string WFButtonName = "WFButton";
    private static string WFFlowLayoutPanelName = "WFFlowLayoutPanelText";
    private static string WFHostName = "WFH";
    #endregion

    #region Testcase setup
    public Opacity(string[] args) : base(args) { }

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
    [Scenario("WFH verify that default is 1")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "WFH verify that default is 1";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");

        //check if the WFH opacity property default is 1
        WPFMiscUtils.IncCounters(sr, (double)1, wfh.Opacity, "WFH Opacity default value not equal 1 -- value: " + wfh.Opacity.ToString(), p.log);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }

    [Scenario("verify opacity can be set to any value and no exception is thrown")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "verify opacity can be set to any value and no exception will be thrown";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");

        //check if the WFH opacity can be set to vary values
        TestOpacityValue(1, p, sr);
        TestOpacityValue(0, p, sr);
        TestOpacityValue(-1, p, sr);
        TestOpacityValue(0.1, p, sr);
        TestOpacityValue(1.1, p, sr);
        TestOpacityValue(Double.MaxValue, p, sr);
        TestOpacityValue(Double.MaxValue, p, sr);

        p.log.WriteLine(TCText + " - Test Run End");
        return sr;
    }



    #endregion

    #region TestFunction
    private void TestOpacityValue(double value, TParams p, ScenarioResult sr)
    {
        wfh.Opacity = value;
        WPFMiscUtils.IncCounters(sr, value, wfh.Opacity, "Failed to set WFH Opacity value to : " + wfh.Opacity.ToString(), p.log);
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
        wfh = new WindowsFormsHost();
        wf_Button = new Button();
        wf_FlowLayoutPanel = new FlowLayoutPanel();

        wf_Button.Name = WFButtonName;
        wf_FlowLayoutPanel.Name = WFFlowLayoutPanelName;
        wfh.Name = WFHostName;

        wf_Button.Text = WFButtonName;
        wf_FlowLayoutPanel.Controls.Add(wf_Button);

        wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        wfh.Child = wf_FlowLayoutPanel;

        this.Content = wfh;
        p.log.WriteLine(strTC + " - TestSetup -- End ");
    }
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WFH verify that default is 1
//@ verify opacity can be set to any value and no exception is thrown
