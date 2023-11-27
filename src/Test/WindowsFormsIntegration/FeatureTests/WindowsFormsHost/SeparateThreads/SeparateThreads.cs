// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;
using System.Threading;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    SeparateThreads
// Description: Running WFH's on seperate AV Windows on seperate threads
//
namespace WindowsFormsHostTests
{
    public class SeparateThreads : WPFReflectBase {

        #region Testcase setup

        TParams _tp;
        SWC.Button _AVButton;
        SWC.StackPanel _AVStackPanel;

        // Second WinForms window
        SW.Window _AVWindow;
        WindowsFormsHost _wfh;
        SWF.Button _WFButton;

        private const string AVButtonName = "AVButton";
        private const string AVWindowName = "AVWindow";
        private const string WFButtonName = "WFButton";
        private const string AppName = "SeparateThreads";

        public SeparateThreads(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p) 
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            _tp = p;
            this.UseMITA = true;

            _AVStackPanel = new System.Windows.Controls.StackPanel();
            _AVButton = new SWC.Button();
            _AVButton.Content = _AVButton.Name = AVButtonName;
            _AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);
            _AVStackPanel.Children.Add(_AVButton);
            this.Content = _AVStackPanel;
            this.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            this.Title = AppName;
            this.Left = 0;
            this.Top = 200;
            base.InitTest(p);
        }

        void AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(AVDialogThread));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void AVDialogThread()
        {
            _AVWindow = new SW.Window();
            _AVWindow.Title = AVWindowName;
            _AVWindow.Left = 400;
            _AVWindow.Top = 200;
            _AVWindow.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;

            _wfh = new WindowsFormsHost();
            _WFButton = new System.Windows.Forms.Button();
            _WFButton.Text = _WFButton.Name = WFButtonName;
            _wfh.Child = _WFButton;
            _AVWindow.Content = _wfh;

            _WFButton.Click += new EventHandler(WFButton_Click);

            // start the AV Dialog Box on a separate thread
            _AVWindow.ShowDialog();
        }

        void WFButton_Click(object sender, EventArgs e)
        {
            _AVWindow.Close();
        }

        #endregion


        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Verifty that an Avalon App can spawn a new thread with a new Avalon Window that contains a WFH and WF control - does it blow up?")]
        public ScenarioResult Scenario1(TParams p) 
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                Utilities.SleepDoEvents(10);
                UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AppName));
                UIObject uiControl = uiApp.Descendants.Find(UICondition.CreateFromId(AVButtonName));
                // Click on the button to bring up the second Avalon window on a separate thread
                uiControl.Click(PointerButtons.Primary);
                Utilities.SleepDoEvents(30);

                UIObject uiSecondAV = UIObject.Root.Children.Find(UICondition.CreateFromName(AVWindowName));
                UIObject uiWFHBtn = uiSecondAV.Descendants.Find(UICondition.CreateFromId(WFButtonName));
                // Click on the WF Button to close the second Avalon window
                Utilities.SleepDoEvents(10);
                uiWFHBtn.Click(PointerButtons.Primary);
            }
            catch (Exception ee)
            {
                sr.IncCounters(false, ee.Message, p.log);
            }

            sr.IncCounters(true);
            return sr;
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verifty that an Avalon App can spawn a new thread with a new Avalon Window that contains a WFH and WF control - does it blow up?
