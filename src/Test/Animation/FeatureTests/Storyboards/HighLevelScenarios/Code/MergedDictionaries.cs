// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.HighLevelScenarios</area>
    /// <priority>1</priority>
    /// <description>
    /// Verify that Storyboards can be found and started when specified in ResourceDictionaries
    /// that are merged.  Completion of the animations is verified.
    /// Note:
    ///    For details of the new feature being introduced in .Net3.5-SP1, refer to the test spec
    ///    located at:  Test\Animation\Specifications\Storyboard Parameterless Methods Test Spec.docx
    /// </description>
    /// </summary>
    [Test(1, "Storyboards.HighLevelScenarios", "MergedDictionariesTest")]
    public class MergedDictionariesTest : XamlTest
    {
        #region Test case members

        public static VisualVerifier    verifier;
        private string                  _inputString;
        private TextBox                 _textbox;
        private Storyboard              _resourceStoryboard1;
        private Storyboard              _resourceStoryboard2;
        private DispatcherTimer         _aTimer = null;
        
        #endregion


        #region Constructor

        [Variation("BeginWithParm")]
        [Variation("BeginWithNoParm")]

        /******************************************************************************
        * Function:          MergedDictionariesTest Constructor
        ******************************************************************************/
        public MergedDictionariesTest(string variation) : base(@"MergedDictionaries.xaml")
        {
            _inputString = variation;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(FindStoryboardsInResources);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Retrieves and merges the ResourceDictionaries.
        /// </summary>
        /// <returns>Returns success if the dictionaries were found</returns>
        private TestResult Initialize()
        {
            Window.Title = "Animation";
            Window.Left     = 0d;
            Window.Top      = 0d;
            Window.Topmost  = true;
            verifier = new VisualVerifier();

            _textbox = (TextBox)RootElement.FindName("TextBox1");

            if (_textbox == null)
            {
                GlobalLog.LogEvidence("TextBox1 was not found.");
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }

        /******************************************************************************
           * Function:          FindStoryboardsInResources
           ******************************************************************************/
        /// <summary>
        /// Retrieves the Storyboards that are specified in the merged ResourceDictionaries.
        /// <returns>Returns success if the Storyboard was found</returns>
        /// </summary>
        private TestResult FindStoryboardsInResources()
        {
            GlobalLog.LogStatus("---FindStoryboardInResource---");

            _resourceStoryboard1 = (Storyboard)RootElement.TryFindResource("Story1");
            _resourceStoryboard2 = (Storyboard)RootElement.TryFindResource("Story2");

            if (_resourceStoryboard1 == null)
            {
                GlobalLog.LogEvidence("ResourceDictionary #1 was not found.");
                return TestResult.Fail;
            }
            else if (_resourceStoryboard2 == null)
            {
                GlobalLog.LogEvidence("ResourceDictionary #2 was not found.");
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
        /// Begins the Storyboard and starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns></returns>
        TestResult StartTimer()
        {
            GlobalLog.LogStatus("---StartTimer---");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
            _aTimer.Start();

            if (_inputString == "BeginWithParm")
            {
                _resourceStoryboard1.Begin(_textbox, HandoffBehavior.SnapshotAndReplace, true);
                _resourceStoryboard2.Begin(_textbox, HandoffBehavior.SnapshotAndReplace, true);
            }
            else if (_inputString == "BeginWithNoParm")
            {
                _resourceStoryboard1.Begin();
                _resourceStoryboard2.Begin();
            }

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
            GlobalLog.LogStatus("---OnTick---");

            _aTimer.Stop();
            Signal("AnimationDone", TestResult.Pass);
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
            GlobalLog.LogStatus("---Verify---");

            WaitForSignal("AnimationDone");

            float tolerance = 0.10f;
            int x           = 100;
            int y           = 100;
            Color expColor  = Colors.Blue;

            verifier.InitRender(Window);

            Color actColor = verifier.getColorAtPoint(x,y);

            bool testPassed = AnimationUtilities.CompareColors(expColor, actColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x + "," + y + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor.ToString());
            
            if (testPassed)
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
