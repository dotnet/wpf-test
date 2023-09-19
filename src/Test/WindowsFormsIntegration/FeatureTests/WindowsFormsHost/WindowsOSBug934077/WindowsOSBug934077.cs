// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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


// Testcase:    WindowsOSBug934077
// Description: Ensure that a System.Windows.Forms.WebBrowser can be hosted in WFH.
namespace WindowsFormsHostTests
{
    public class WindowsOSBug934077 : WPFReflectBase 
    {
        #region TestVariables
        private delegate void myEventHandler(object sender);
        private string _events = String.Empty;   // event sequence string
        private TParams _tp;

        private SWC.StackPanel _AVStackPanel;
        private AxSHDocVw.AxWebBrowser _axWebBrowser1;

        private WindowsFormsHost _wfh;

        private const string WindowTitleName = "WindowsOSBug934077Test";

        private const string AVStackPanelName = "AVStackPanel";

        private const string WFName = "WFN";
        private string _URL = "http://www.microsoft.com/about/";
        private string _URL2 = "http://www.microsoft.com/careers/";
        private string _URL3 = "http://support.microsoft.com/";
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

            while (_axWebBrowser1.IsHandleCreated == false)
                System.Windows.Forms.Application.DoEvents();

            // Navigate to the first URL web site
            Utilities.SleepDoEvents(10);
            _axWebBrowser1.Navigate(_URL);
            while (_axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                System.Windows.Forms.Application.DoEvents();

            // Navigate to the second URL web site
            Utilities.SleepDoEvents(10);
            _axWebBrowser1.Navigate(_URL2);
            while (_axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                System.Windows.Forms.Application.DoEvents();

            // Navigate to the third URL web site
            Utilities.SleepDoEvents(10);
            _axWebBrowser1.Navigate(_URL3);
            while (_axWebBrowser1.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                System.Windows.Forms.Application.DoEvents();

            WPFMiscUtils.IncCounters(sr, expVal, _events, "Not getting the proper Web Browser events", p.log);

            return sr;
        }
        #endregion

        private void TestSetup()
        {
            _tp.log.WriteLine("TestSetup -- Start ");

            #region SetupAVControl
            _AVStackPanel = new SWC.StackPanel();
            #endregion

            #region SetupWFControl
            _wfh = new WindowsFormsHost();
            _axWebBrowser1 = new AxWebBrowser();
            _wfh.Name = WFName;
            _wfh.Width = 600;
            _wfh.Height = 600;
            _axWebBrowser1.BeforeNavigate2 += delegate { _events += "BeforeNavigate2::"; };
            _axWebBrowser1.NavigateComplete2 += delegate { _events += "NavigateComplete2::"; };
            _axWebBrowser1.DocumentComplete += delegate { _events += "DocumentComplete::"; };
            _wfh.Child = _axWebBrowser1;
            #endregion

            #region LayoutWindow
            this.Title = WindowTitleName;
            _AVStackPanel.Children.Add(_wfh);
            this.Content = _AVStackPanel;
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
