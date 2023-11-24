using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Controls;
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Threading;
using System.Diagnostics;
using System.Reflection;


///
/// <Testcase>
/// BackgroundWorker
/// </Testcase>    
///
/// <summary>
/// Test the BackgroundWorker and it's events work as expected when called from an EH control. The tests should set Control.ControlAllowCrossThreadCalls = false.
/// </summary>
/// <history>
///  [sameerm]   3/17/2006   Created
///  [sameerm]   3/24/2006   Inc CR feedback
/// </history>


public class BackgroundWorker : ReflectBase {

    #region Testcase setup
    public BackgroundWorker(string[] args) : base(args) { }


    protected override void InitTest(TParams p) 
    {
        base.InitTest(p);
        //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        eh  = new ElementHost();
        b  = new System.Windows.Controls.Button();
        eh.Child = b;
        b.Content = "Hello";
        Controls.Add(eh);
        this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
         Controls.Clear();
         backgroundWorker1.Dispose();
         base.AfterScenario(p, scenario, result);
    }
    /*
    [Scenario("Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        this.Text = "Scenario1";
        this.backgroundWorker1.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                ChangeButtonProp();
                Utilities.SleepDoEvents(1);
            }
            catch (InvalidOperationException )
            {
                p.log.WriteLine("Got InvalidOperationException as expected.");
                sr.IncCounters(true);
            }
            catch (Exception ex)
            {
                p.log.WriteLine(ex.ToString());
                sr.IncCounters(false, "Calling ChangeButton Prop in DoWork method failed", p.log);
            }
        };

        this.backgroundWorker1.RunWorkerAsync();
        Utilities.SleepDoEvents(1);
        
        return sr; 
    }
    */
    [Scenario("Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event")]
    public ScenarioResult Scenario2(TParams p) 
    {
        
        ScenarioResult sr = new ScenarioResult();
        this.Text = "Scenario2";
        this.backgroundWorker1.WorkerReportsProgress = true;
        this.backgroundWorker1.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(100);
        };
        this.backgroundWorker1.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ChangeButtonProp();
                Utilities.SleepDoEvents(1);
                if (eh.Size == newSize && b.Content.ToString() == newText)
                    sr.IncCounters(true);
                else
                    sr.IncCounters(false);
           }
           catch (InvalidOperationException iopex)
           {
               p.log.WriteLine("Got InvalidOperationException RunWorkerCompleted method." + iopex.ToString());
               sr.IncCounters(false, "Got InvalidOperationException RunWorkerCompleted method.",p.log);
           }
           catch (Exception ex)
           {
                p.log.WriteLine(ex.ToString());
                sr.IncCounters(false, "Calling ChangeButton Prop in RunWorkerCompleted methoed failed", p.log);
           }
        };
        this.backgroundWorker1.RunWorkerAsync();

        Utilities.SleepDoEvents(1);
        return sr;
    }

    
    [Scenario("Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event")]
    public ScenarioResult Scenario3(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        this.Text = "Scenario3";
        backgroundWorker1.WorkerReportsProgress = true;
        this.backgroundWorker1.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Debug.WriteLine("Do nothing");
            backgroundWorker1.ReportProgress(100);
        };

        this.backgroundWorker1.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                ChangeButtonProp();
                Utilities.SleepDoEvents(1);
                if (eh.Size == newSize && b.Content.ToString() == newText)
                    sr.IncCounters(true);
                else
                    sr.IncCounters(false);
            }
            catch (InvalidOperationException iopex)
            {
                p.log.WriteLine("Got InvalidOperationException ProgressChange event." + iopex.ToString());
                sr.IncCounters(false, "Got InvalidOperationException ProgressChange event.", p.log);
            }
            catch (Exception ex)
            {
                p.log.WriteLine(ex.ToString());
                sr.IncCounters(false, "Calling ChangeButton Prop in ProgressChange Event failed.", p.log);
            }
        };
        this.backgroundWorker1.RunWorkerAsync();

        Utilities.SleepDoEvents(1);
        return sr;
    }
    
    #endregion 

    #region HELPERS
    private void ChangeButtonProp()
    {
        this.b.Background = SWM.Brushes.Goldenrod;
        this.b.FontFamily = new SWM.FontFamily("Times New Roman");
        b.Content = newText;
        this.eh.Size = newSize;

    }
    #endregion

    private ElementHost eh;  
    private System.Windows.Controls.Button b;  //= new System.Windows.Controls.Button();
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    Size newSize = new Size(100, 100);
    string newText = "Hello Winforms";    
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.
//@ Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event
//@ Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event