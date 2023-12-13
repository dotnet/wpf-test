// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Threading;

//
// Testcase:    Async
// Description: Test Async W/O Background Worker
//
namespace WindowsFormsHostTests
{
    public delegate bool TestDelegate();
    public delegate bool TestDelegateWithParams(string[] args, int count);

    public class Async : WPFReflectBase 
    {
        #region Testcase setup

        private Button _avB;
        private WindowsFormsHost _wfh;
        private System.Windows.Forms.ListBox _wfLB;
        private AutoResetEvent _are = new AutoResetEvent(false);

        public Async(string[] args)
            : base(args)
        {
            _avB = new Button();
            _wfh = new WindowsFormsHost();
            _wfLB = new System.Windows.Forms.ListBox();
            _wfLB.Items.Add("One");
            _wfLB.Items.Add("Two");
            _wfLB.Items.Add("Three");

            _wfh.Child = _wfLB;

            _avB.Content = _wfh;
            this.Content = _avB;
        }

        protected override void InitTest(TParams p)
        {
            UseMITA = true;
            base.InitTest(p);
        }

        public bool TestDelegate()
        {
            _are.Set();
            return true;
        }

        public bool TestDelegateWithParams(string[] args, int count)
        {
            _are.Set();
            return (args.Length == count);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Control.Invoke")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            bool isOk = false;
            TestDelegate td = new TestDelegate(this.TestDelegate);
            TestDelegateWithParams tdwp = new TestDelegateWithParams(this.TestDelegateWithParams);

            isOk = (bool)_wfLB.Invoke(td);

            sr.IncCounters(isOk, "Invoke did not execute", p.log);

            string[] args = new string[] { "one", "two", "three" };
            isOk = (bool)_wfLB.Invoke(tdwp, new object[] { args, args.Length });

            sr.IncCounters(isOk, "Invoke with parameters did not execute", p.log);

            return sr;
        }

        [Scenario("Control.BeginInvoke")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            bool isOk = false;
            TestDelegate td = new TestDelegate(this.TestDelegate);
            TestDelegateWithParams tdwp = new TestDelegateWithParams(this.TestDelegateWithParams);

            IAsyncResult ar = _wfLB.BeginInvoke(td);

            _are.WaitOne(2000, false);
            isOk = (bool)_wfLB.EndInvoke(ar);

            sr.IncCounters(isOk, "BeginInvoke did not execute in the time out specified", p.log);

            string[] args = new string[] { "one", "two", "three" };

            ar = _wfLB.BeginInvoke(tdwp, new object[] { args, args.Length });
            _are.WaitOne(2000, false);
            isOk = (bool)_wfLB.EndInvoke(ar);

            sr.IncCounters(isOk, "BeginInvoke with Parameters did not execute in the time out specified", p.log);
            return sr;
        }

        [Scenario("Control.InvokeRequired")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //Because the UseMITA flag is true, The scenarios will run on a thread other than the UI thread.
            //We can simply check to make sure that InvokeRequired is set.
            sr.IncCounters(_wfLB.InvokeRequired, "InvokeRequired not set", p.log);

            return sr;
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Control.Invoke

//@ Control.BeginInvoke

//@ Control.InvokeRequired