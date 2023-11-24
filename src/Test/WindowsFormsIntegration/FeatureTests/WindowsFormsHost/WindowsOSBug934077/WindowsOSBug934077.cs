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
using System.Drawing;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

using AxSHDocVw;


//
// Testcase:    WindowsOSBug934077
// Description: Ensure that a System.Windows.Forms.WebBrowser can be hosted in WFH.
// Author:      pachan
//
namespace WindowsFormsHostTests
{

public class WindowsOSBug934077 : WPFReflectBase 
{
    #region TestVariables
    private delegate void myEventHandler(object sender);
    private string Events = String.Empty;   // event sequence string
    private TParams _tp;

    private SWC.StackPanel AVStackPanel;
    private AxSHDocVw.AxWebBrowser axWebBrowser1;

    private WindowsFormsHost wfh;

    private const string WindowTitleName = "WindowsOSBug934077Test";

    private const string AVStackPanelName = "AVStackPanel";

    private const string WFName = "WFN";
    private string URL = "http://www.microsoft.com/about/";
    private string URL2 = "http://www.microsoft.com/careers/";
    private string URL3 = "http://support.microsoft.com/";
    #endregion

    #region Testcase setup
    public WindowsOSBug934077(string[] args) : base(args) { }


    protected override void InitTest(TParams p) 
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
        _tp = p;
        TestSetup();
        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Host a System.Windows.Forms.WebBrowser inside a WFH - verify that no exception is thrown.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string expVal = String.Empty;
        expVal += "BeforeNavigate2::";
        expVal += "NavigateComplete2::";
        expVal += "DocumentComplete::";
        expVal += "BeforeNavigate2::";
	expVal += "NavigateComplete2::";
        expVal += "DocumentComplete::";
        expVal += "BeforeNavigate2::";
        expVal += "NavigateComplete2::";
        expVal += "DocumentComplete::"; 

        while (axWebBrowser1.IsHandleCreated == false)
            System.Windows.Forms.Application.DoEvents();

        // Navigate to the first URL web site
        Utilities.SleepDoEvents(10);
        axWebBrowser1.Navigate(URL);
        while (axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            System.Windows.Forms.Application.DoEvents();

        // Navigate to the second URL web site
        Utilities.SleepDoEvents(10);
        axWebBrowser1.Navigate(URL2);
        while (axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            System.Windows.Forms.Application.DoEvents();

        // Navigate to the third URL web site
        Utilities.SleepDoEvents(10);
        axWebBrowser1.Navigate(URL3);
        while (axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            System.Windows.Forms.Application.DoEvents();

        WPFMiscUtils.IncCounters(sr, expVal, Events, "Not getting the proper Web Browser events", p.log);

        return sr;
    }
    #endregion

    private void TestSetup()
    {
        _tp.log.WriteLine("TestSetup -- Start ");

        #region SetupAVControl
        AVStackPanel = new SWC.StackPanel();
        #endregion

        #region SetupWFControl
        wfh = new WindowsFormsHost();
        axWebBrowser1 = new AxWebBrowser();
        wfh.Name = WFName;
        wfh.Width = 600;
        wfh.Height = 600;
        axWebBrowser1.BeforeNavigate2 += delegate { Events += "BeforeNavigate2::"; };
        axWebBrowser1.NavigateComplete2 += delegate { Events += "NavigateComplete2::"; };
        axWebBrowser1.DocumentComplete += delegate { Events += "DocumentComplete::"; };
        wfh.Child = axWebBrowser1;
        #endregion

        #region LayoutWindow
        this.Title = WindowTitleName;
        AVStackPanel.Children.Add(wfh);
        this.Content = AVStackPanel;
        #endregion

        _tp.log.WriteLine("TestSetup -- End ");
    }
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Host a System.Windows.Forms.WebBrowser inside a WFH - verify that no exception is thrown.

