// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
namespace WindowsFormsHostTests
{
    public class BackgroundWorker : WPFReflectBase
    {
    #region TestVariables

        private WindowsFormsHost _wfh;
        private SWF.FlowLayoutPanel _wf_FlowLayoutPanel;
        private SWF.Button _wf_btnStart;

        private System.ComponentModel.BackgroundWorker _bgW;

        private static string s_windowTitleName = "BackgroundWorkerTest";
        private static string s_WFStartButtonName = "WFStartButton";
        private static string s_WFFlowLayoutPanelName = "WFFlowLayoutPanelText";
        private static string s_WFHostName = "WFH";

        private const int numberToCompute = 35;
        private const long answer = 14930352;

        private bool _bExpectedExcp = false;
        private bool _bProgressChangedFailed = false;
        private string _strErr = String.Empty;
        private string _strErr2 = String.Empty;

        private int _percentComplete = 0;
        private int _numberReportProgressCall = 0;
        private int _progressChangedBeingCalled = 0;
        private int _highestPercentageReached = 0;
        private int _computedAnswer = 0;

        #endregion

        #region Testcase setup
        public BackgroundWorker(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p) 
        {
            _bgW = new System.ComponentModel.BackgroundWorker();
            _bgW.WorkerSupportsCancellation = true;
            _bgW.WorkerReportsProgress = true;
            _bgW.DoWork += new System.ComponentModel.DoWorkEventHandler(bgW_DoWork);
            _bgW.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bgW_RunWorkerCompleted);
            _bgW.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bgW_ProgressChanged);

            this.Title = s_windowTitleName;
            _wfh = new WindowsFormsHost();
            _wf_FlowLayoutPanel = new FlowLayoutPanel();
            _wf_btnStart = new Button();

            _wfh.Name = s_WFHostName;
            _wf_FlowLayoutPanel.Name = s_WFFlowLayoutPanelName;
            _wf_btnStart.Name = s_WFStartButtonName;
            _wf_btnStart.Text = s_WFStartButtonName;

            _wf_FlowLayoutPanel.Controls.Add(_wf_btnStart);
            _wf_btnStart.Click += new EventHandler(wf_btnStart_Click);

            // this will force the application to check for illegal cross thread calls
            Control.CheckForIllegalCrossThreadCalls = true;

            _wf_FlowLayoutPanel.FlowDirection = SWF.FlowDirection.TopDown;
            _wfh.Child = _wf_FlowLayoutPanel;

            this.Content = _wfh;
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
            _wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

            // Wait for the BackgroundWorker to finish
            while (_bgW.IsBusy)
            {
                WPFReflectBase.DoEvents();
            }

            //check if we are getting the proper exception and correct result
            WPFMiscUtils.IncCounters(sr, true, _bExpectedExcp, "Did not get an exception on setting control value inside the DoWork Event ", p.log);
            WPFMiscUtils.IncCounters(sr, true, _wf_btnStart.Visible, "Control value being set to true", p.log);

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
            _wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

            // Wait for the BackgroundWorker to finish the download.
            while (_bgW.IsBusy)
            {
                WPFReflectBase.DoEvents();
            }

            //check if we are getting the proper exception and correct result
            WPFMiscUtils.IncCounters(sr, String.Empty, _strErr, "Error on changing WF Control property on the ProgressChanged Event", p.log);
            WPFMiscUtils.IncCounters(sr, true, _bProgressChangedFailed, "Incorrect Progress Changed Percentage", p.log);
            WPFMiscUtils.IncCounters(sr, System.Drawing.Color.HotPink, _wf_btnStart.BackColor, "Control property value not being changed", p.log);
            WPFMiscUtils.IncCounters(sr, _progressChangedBeingCalled, _numberReportProgressCall, "Not getting the same number of ProgressChanged Calls", p.log);

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
            _wf_btnStart.Invoke(new EventHandler(wf_btnStart_Click));

            // Wait for the BackgroundWorker to finish 
            while (_bgW.IsBusy)
            {
                WPFReflectBase.DoEvents();
            }

            //check if we are getting the proper exception and correct result
            WPFMiscUtils.IncCounters(sr, String.Empty, _strErr2, "Error on changing WF Control property on the RunWorkerCompleted Event", p.log);
            WPFMiscUtils.IncCounters(sr, System.Drawing.Color.Aqua, _wf_btnStart.ForeColor, "Control property value not being changed", p.log);
            WPFMiscUtils.IncCounters(sr, (long)answer, (long)_computedAnswer, "Computed Answer is incorrect", p.log);

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

            _bExpectedExcp = false;
            _strErr = String.Empty;
            _strErr2 = String.Empty;
            _bProgressChangedFailed = false;
            _progressChangedBeingCalled = 0;
            _numberReportProgressCall = 0;
            _highestPercentageReached = 0;
            _computedAnswer = 0;
            _wf_btnStart.BackColor = System.Drawing.Color.White;

            p.log.WriteLine(strTC + " - TestSetup -- End ");
        }
        #endregion

        #region TestFunction

        void wf_btnStart_Click(object sender, EventArgs e)
        {
            _bgW.RunWorkerAsync(numberToCompute); 
        }

        void bgW_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                // make sure we can change the WF control property 
                _wf_btnStart.BackColor = System.Drawing.Color.HotPink;
            }
            catch (Exception excp)
            {
                _strErr = excp.Message;
            }
            finally
            {
                _progressChangedBeingCalled++;
                if (e.ProgressPercentage != _percentComplete)
                {
                    _bProgressChangedFailed = true;
                }
            }
        }

        void bgW_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                // make sure we can change the WF control property 
                _wf_btnStart.ForeColor = System.Drawing.Color.Aqua;
            }
            catch (Exception excp)
            {
                _strErr2 = excp.Message;
            }
            finally
            {
                _computedAnswer = Convert.ToInt32(e.Result);
            }
        }

        void bgW_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                // force it to fail
                _wf_btnStart.Visible = false;
                //System.Diagnostics.Debugger.Break();          
            }
            
            catch (System.InvalidOperationException)
            {
                //Make sure we are getting an exception that we cannot change the control value
                _bExpectedExcp = true;
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
                _percentComplete =
                    (int)((float)n / (float)numberToCompute * 100);
                if (_percentComplete > _highestPercentageReached)
                {
                    _highestPercentageReached = _percentComplete;
                    worker.ReportProgress(_percentComplete);
                    _numberReportProgressCall++;
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