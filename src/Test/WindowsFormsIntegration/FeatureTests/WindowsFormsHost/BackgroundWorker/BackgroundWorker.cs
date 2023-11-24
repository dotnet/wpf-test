using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Threading;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;



/// <Testcase>BackgroundWorker</Testcase>
/// <summary>
/// Description: Test the BackgroundWorker and it's events work correctly when called from WF ctr
/// </summary>
/// <History>
/// pachan 3/20/2006 Created
/// </History>

namespace WindowsFormsHostTests
{

public class BackgroundWorker : WPFReflectBase
{
   #region TestVariables

    private WindowsFormsHost wfh;
    private SWF.FlowLayoutPanel wf_FlowLayoutPanel;
    private SWF.Button wf_btnStart;

    private System.ComponentModel.BackgroundWorker bgW;

    private static string WindowTitleName = "BackgroundWorkerTest";
    private static string WFStartButtonName = "WFStartButton";
    private static string WFFlowLayoutPanelName = "WFFlowLayoutPanelText";
    private static string WFHostName = "WFH";

    private const int numberToCompute = 35;
    private const long answer = 14930352;

    private bool bExpectedExcp = false;
    private bool bProgressChangedFailed = false;
    private string strErr = String.Empty;
    private string strErr2 = String.Empty;

    private int percentComplete = 0;
    private int numberReportProgressCall = 0;
    private int progressChangedBeingCalled = 0;
    private int highestPercentageReached = 0;
    private int computedAnswer = 0;

    #endregion

    #region Testcase setup
    public BackgroundWorker(string[] args) : base(args) { }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        return base.BeforeScenario(p, scenario);
    }

    protected override void InitTest(TParams p) 
    {
        bgW = new System.ComponentModel.BackgroundWorker();
        bgW.WorkerSupportsCancellation = true;
        bgW.WorkerReportsProgress = true;
        bgW.DoWork += new System.ComponentModel.DoWorkEventHandler(bgW_DoWork);
        bgW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgW_RunWorkerCompleted);
        bgW.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bgW_ProgressChanged);

        this.Title = WindowTitleName;
        wfh = new WindowsFormsHost();
        wf_FlowLayoutPanel = new FlowLayoutPanel();
        wf_btnStart = new Button();

        wfh.Name = WFHostName;
        wf_FlowLayoutPanel.Name = WFFlowLayoutPanelName;
        wf_btnStart.Name = WFStartButtonName;
        wf_btnStart.Text = WFStartButtonName;

        wf_FlowLayoutPanel.Controls.Add(wf_btnStart);
        wf_btnStart.Click += new EventHandler(wf_btnStart_Click);

        // this will force the application to check for illegal cross thread calls
        Control.CheckForIllegalCrossThreadCalls = true;

        wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
        wfh.Child = wf_FlowLayoutPanel;

        this.Content = wfh;
        base.InitTest(p);
            
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");

        // change the wf_btnStart Visible property to false on the DoWork event
        // click button wf_btnStart to start
        wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

        // Wait for the BackgroundWorker to finish
        while (bgW.IsBusy)
        {
            WPFReflectBase.DoEvents();
        }

        //check if we are getting the proper exception and correct result
        WPFMiscUtils.IncCounters(sr, true, bExpectedExcp, "Did not get an exception on setting control value inside the DoWork Event ", p.log);
        WPFMiscUtils.IncCounters(sr, true, wf_btnStart.Visible, "Control value being set to true", p.log);

        return sr;
    }

    [Scenario("Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");


        // change the wf_btnStart BackColor to HotPink on the ProgressChanged Event
        // click button wf_btnStart to start
        wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

        // Wait for the BackgroundWorker to finish the download.
        while (bgW.IsBusy)
        {
            WPFReflectBase.DoEvents();
        }

        //check if we are getting the proper exception and correct result
        WPFMiscUtils.IncCounters(sr, String.Empty, strErr, "Error on changing WF Control property on the ProgressChanged Event", p.log);
        WPFMiscUtils.IncCounters(sr, true, bProgressChangedFailed, "Incorrect Progress Changed Percentage", p.log);
        WPFMiscUtils.IncCounters(sr, System.Drawing.Color.HotPink, wf_btnStart.BackColor, "Control property value not being changed", p.log);
        WPFMiscUtils.IncCounters(sr, progressChangedBeingCalled, numberReportProgressCall, "Not getting the same number of ProgressChanged Calls", p.log);

        //KeepRunningTests = false;
        return sr;
    }

    [Scenario("Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event")]
    public ScenarioResult Scenario3(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        string TCText = "Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event";
        TestSetup(p, TCText);
        MyPause();

        p.log.WriteLine(TCText + " - Test Run Start");

        // change the wf_btnStart BackColor to HotPink on the RunWorkerCompleted Event
        // click button wf_btnStart to start
        wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

        // Wait for the BackgroundWorker to finish 
        while (bgW.IsBusy)
        {
            WPFReflectBase.DoEvents();
        }

        //check if we are getting the proper exception and correct result
        WPFMiscUtils.IncCounters(sr, String.Empty, strErr2, "Error on changing WF Control property on the RunWorkerCompleted Event", p.log);
        WPFMiscUtils.IncCounters(sr, System.Drawing.Color.Aqua, wf_btnStart.ForeColor, "Control property value not being changed", p.log);
        WPFMiscUtils.IncCounters(sr, (long)answer, (long)computedAnswer, "Computed Answer is incorrect", p.log);

        return sr;
    }

    #endregion



    #region HelperFunction
    // Helper function to set up app for particular Scenario
    private static void MyPause()
    {
        WPFReflectBase.DoEvents();
        System.Threading.Thread.Sleep(200);
    }

    private void TestSetup(TParams p, string strTC)
    {
        p.log.WriteLine(strTC + " - TestSetup -- Start ");

        bExpectedExcp = false;
        strErr = String.Empty;
        strErr2 = String.Empty;
        bProgressChangedFailed = false;
        progressChangedBeingCalled = 0;
        numberReportProgressCall = 0;
        highestPercentageReached = 0;
        computedAnswer = 0;
        wf_btnStart.BackColor = System.Drawing.Color.White;

        p.log.WriteLine(strTC + " - TestSetup -- End ");
    }
    #endregion


    #region TestFunction

    void wf_btnStart_Click(object sender, EventArgs e)
    {
        bgW.RunWorkerAsync(numberToCompute); 
    }

    void bgW_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {
        try
        {
            // make sure we can change the WF control property 
            wf_btnStart.BackColor = System.Drawing.Color.HotPink;
        }
        catch (Exception excp)
        {
            strErr = excp.Message;
        }
        finally
        {
            progressChangedBeingCalled++;
            if (e.ProgressPercentage != percentComplete)
            {
                bProgressChangedFailed = true;
            }
        }
    }

    void bgW_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        try
        {
            // make sure we can change the WF control property 
            wf_btnStart.ForeColor = System.Drawing.Color.Aqua;
        }
        catch (Exception excp)
        {
            strErr2 = excp.Message;
        }
        finally
        {
            computedAnswer = Convert.ToInt32(e.Result);
        }
    }

    void bgW_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        try
        {
            // force it to fail
            wf_btnStart.Visible = false;
            //System.Diagnostics.Debugger.Break();          
        }
        
        catch (System.InvalidOperationException)
        {
            //Make sure we are getting an exception that we cannot change the control value
            bExpectedExcp = true;
        }
        finally
        {
            // Get the BackgroundWorker that raised this event.
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = ComputeFibonacci((int)e.Argument, worker, e);

        }
    }

    // This is the method that does the actual work. For this
    // example, it computes a Fibonacci number and
    // reports progress as it does its work.
    private long ComputeFibonacci(int n, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
    {
        long result = 0;

        // Abort the operation if the user has canceled.
        // Note that a call to CancelAsync may have set 
        // CancellationPending to true just after the
        // last invocation of this method exits, so this 
        // code will not have the opportunity to set the 
        // DoWorkEventArgs.Cancel flag to true. This means
        // that RunWorkerCompletedEventArgs.Cancelled will
        // not be set to true in your RunWorkerCompleted
        // event handler. This is a race condition.

        if (worker.CancellationPending)
        {
            e.Cancel = true;
        }
        else
        {
            if (n < 2)
            {
                result = 1;
            }
            else
            {
                result = ComputeFibonacci(n - 1, worker, e) +
                         ComputeFibonacci(n - 2, worker, e);
            }

            // Report progress as a percentage of the total task.
            percentComplete =
                (int)((float)n / (float)numberToCompute * 100);
            if (percentComplete > highestPercentageReached)
            {
                highestPercentageReached = percentComplete;
                worker.ReportProgress(percentComplete);
                numberReportProgressCall++;
            }
        }

        return result;
    }

    #endregion

}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.

//@ Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event

//@ Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event