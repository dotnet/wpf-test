// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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


/// <Testcase>
/// BackgroundWorker
/// </Testcase>    
///
/// <summary>
/// Test the BackgroundWorker and it's events work as expected when called from an EH control. The tests should set Control.ControlAllowCrossThreadCalls = false.
/// </summary>
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
        _eh  = new ElementHost();
        _b  = new System.Windows.Controls.Button();
        _eh.Child = _b;
        _b.Content = "Hello";
        Controls.Add(_eh);
        this._backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
         Controls.Clear();
         _backgroundWorker1.Dispose();
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
        this._backgroundWorker1.WorkerReportsProgress = true;
        this._backgroundWorker1.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _backgroundWorker1.ReportProgress(100);
        };
        this._backgroundWorker1.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ChangeButtonProp();
                Utilities.SleepDoEvents(1);
                if (_eh.Size == _newSize && _b.Content.ToString() == _newText)
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
        this._backgroundWorker1.RunWorkerAsync();

        Utilities.SleepDoEvents(1);
        return sr;
    }

    
    [Scenario("Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event")]
    public ScenarioResult Scenario3(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        this.Text = "Scenario3";
        _backgroundWorker1.WorkerReportsProgress = true;
        this._backgroundWorker1.DoWork += delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Debug.WriteLine("Do nothing");
            _backgroundWorker1.ReportProgress(100);
        };

        this._backgroundWorker1.ProgressChanged += delegate(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                ChangeButtonProp();
                Utilities.SleepDoEvents(1);
                if (_eh.Size == _newSize && _b.Content.ToString() == _newText)
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
        this._backgroundWorker1.RunWorkerAsync();

        Utilities.SleepDoEvents(1);
        return sr;
    }
    
    #endregion 

    #region HELPERS
    private void ChangeButtonProp()
    {
        this._b.Background = SWM.Brushes.Goldenrod;
        this._b.FontFamily = new SWM.FontFamily("Times New Roman");
        _b.Content = _newText;
        this._eh.Size = _newSize;

    }
    #endregion

    private ElementHost _eh;  
    private System.Windows.Controls.Button _b;  //= new System.Windows.Controls.Button();
    private System.ComponentModel.BackgroundWorker _backgroundWorker1;
    Size _newSize = new Size(100, 100);
    string _newText = "Hello Winforms";    
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Start BW from WFH hosted WF Control Verify that we cannot change the WF control in the DoWork method.
//@ Start BW from WFH hosted WF Control Verify that BW Completed Events work and we can change the WF control in the event
//@ Start BW from WFH hosted WF Control Verify that BW Progress Changed Events work and we can change the WF control in the event
