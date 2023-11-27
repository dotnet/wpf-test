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



/// <Testcase>WindowsOSBug1538023</Testcase>
/// <summary>
/// Description: Ensure that setting IsVisible to false does not cause a handle recreate.
/// </summary>
namespace WindowsFormsHostTests
{
    public class WindowsOSBug1538023 : WPFReflectBase
    {
        #region TestVariables
        private WindowsFormsHost _wfh;
        #endregion

        #region Testcase setup
        public WindowsOSBug1538023(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Ensure that setting IsVisible to false does not cause a handle recreate")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                SWF.Button b = new SWF.Button();
                b.Text = "Hello";
                b.Size = new System.Drawing.Size(30, 100);
                _wfh = new WindowsFormsHost();
                _wfh.Child = b;
                
                this.AddChild(_wfh);
                Utilities.SleepDoEvents(1, 500);
                IntPtr handle1 = _wfh.Handle;
                
                _wfh.Visibility = System.Windows.Visibility.Hidden;
                Utilities.SleepDoEvents(1, 500);
                IntPtr handle2 = _wfh.Handle;

                Utilities.SleepDoEvents(1, 500);
                if (!handle1.Equals(handle2))
                {
                    p.log.WriteLine("New Handle Created on IsVisibleChange");
                    WPFMiscUtils.IncCounters(sr,  p.log, false, "New Handle Created on IsVisibleChange");
                    return sr;
                }
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.ToString(), p.log);
            }
            WPFMiscUtils.IncCounters(sr,p.log, true, "No new Handle Created.");
            return sr;
        }
        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Ensure that setting IsVisible to false does not cause a handle recreate 