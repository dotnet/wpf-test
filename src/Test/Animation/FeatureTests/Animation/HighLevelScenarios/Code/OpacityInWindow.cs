// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    /// Scenario:  verify animation when markup containing a Window element is loaded.


    [Test(2, "Animation.HighLevelScenarios.Regressions", "OpacityInWindowTest", SupportFiles=@"FeatureTests\Animation\OpacityInWindow.xaml")]
    public class OpacityInWindowTest : WindowTest
    {
        #region Test case members

        private DockPanel           _dockpanel1      = null;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          OpacityInWindowTest Constructor
        ******************************************************************************/
        public OpacityInWindowTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content and starts a DispatcherTimer used for controlling 
        /// verification of the Animation.
        /// </summary>
        /// <returns>Returns success</returns>
        TestResult CreateTree()
        {
            Window.Width        = 200d;
            Window.Height       = 150d;

            StreamReader sr = File.OpenText(@"OpacityInWindow.xaml");
            string sourceFileXAML = sr.ReadToEnd();
            sr.Close();

            // Convert a string to a stream, using UTF-8 encoding
            Encoding encoding = new UTF8Encoding(false, true);
            Stream stream = new System.IO.MemoryStream( encoding.GetBytes(sourceFileXAML) );

            Window window = (Window)XamlReader.Load(stream);
            DockPanel dock = (DockPanel)window.Content;
            window.Content = null;
            Window.Content = dock;

            _dockpanel1 = (DockPanel)LogicalTreeHelper.FindLogicalNode((DependencyObject)Window.Content,"dockpanel1");

            if (_dockpanel1 == null)
            {
                GlobalLog.LogEvidence("ERROR!!! CreateTree: DockPanel not found.");
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromSeconds(2);
            _aTimer.Start();
            
            GlobalLog.LogStatus("----DispatcherTimer Started----");
            
            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            WaitForSignal("AnimationDone");

            double actValue = (double)_dockpanel1.GetValue(UIElement.OpacityProperty);
            double expValue = 1d;

            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == expValue)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}
