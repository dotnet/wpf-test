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
using System.Windows.Media.Media3D;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.HighLevelScenarios.Regressions</area>

    [Test(2, "Storyboards.HighLevelScenarios.Regressions", "RemoveAnimatedViewport3DTest")]
    public class RemoveAnimatedViewport3DTest : XamlTest
    {

        #region Test case members

        public static VisualVerifier    verifier;
        private Page                    _rootElement;
        private Viewport3D              _animatedElement;
        private Storyboard              _resourceStoryboard;
        private DispatcherTimer         _aTimer              = null;
        private int                     _tickCount           = 0;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          RemoveAnimatedViewport3DTest Constructor
        ******************************************************************************/
        public RemoveAnimatedViewport3DTest() : base(@"RemoveAnimatedViewport3D.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(FindStoryboardInResource);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult Initialize()
        {
            Window.Title    = "Animation";
            Window.Left     = 0d;
            Window.Top      = 0d;

            verifier = new VisualVerifier();
            verifier.InitRender(Window);

            _rootElement     = (Page)AnimationUtilities.FindElement(RootElement, "ThePage");
            _animatedElement = (Viewport3D)AnimationUtilities.FindElement(RootElement, "TheViewport");
            
            if (_rootElement == null && _animatedElement == null)
            {
                GlobalLog.LogEvidence("The root or animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The root and animated elements were found.");
                return TestResult.Pass;
            }
        }

        /******************************************************************************
           * Function:          FindStoryboardInResource
           ******************************************************************************/
        /// <summary>
        /// Retrieve the Storyboard that is specified in the Resources section in Markup.
        /// <returns>Returns success if the Storyboard was found</returns>
        /// </summary>
        private TestResult FindStoryboardInResource()
        {
            _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey");

            if (_resourceStoryboard == null)
            {
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
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns></returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
            _aTimer.Start();

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
            _tickCount++;
            GlobalLog.LogStatus("-----Tick #" + _tickCount);

            if (_tickCount == 1)
            {
                _resourceStoryboard.Begin(_rootElement);
                _resourceStoryboard.Pause(_rootElement);
                _resourceStoryboard.Seek(_rootElement, new TimeSpan(0, 0, 0, 2), TimeSeekOrigin.BeginTime);
            }
            else if (_tickCount == 2)
            {
                ModelVisual3D modelVisual3D = (ModelVisual3D)_animatedElement.Children[0];
                Model3DGroup model3DGroup = (Model3DGroup)modelVisual3D.Content.Clone();
                model3DGroup.Children.RemoveAt(2);
                modelVisual3D.Content = model3DGroup;
                _animatedElement.Children[0] = new ModelVisual3D();  //Must clear out the old one.
                _animatedElement.Children[0] = modelVisual3D;
            }
            else
            {
                _aTimer.Stop();
                Signal("AnimationDone", TestResult.Pass);
            }
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

            float tolerance = 0.10f;

            int x1           = 250;
            int y1           = 125;
            int x2           = 300;
            int y2           = 275;

            Color actColor1 = verifier.getColorAtPoint(x1,y1);
            Color expColor1 = Colors.Blue;
            Color actColor2 = verifier.getColorAtPoint(x2,y2);
            Color expColor2 = Colors.Blue;

            bool b1 = AnimationUtilities.CompareColors(expColor1, actColor1, tolerance);
            bool b2 = AnimationUtilities.CompareColors(expColor2, actColor2, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor1.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor1.ToString());
            GlobalLog.LogEvidence("---------- Result at (" + x2 + "," + y2 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actColor2.ToString());
            GlobalLog.LogEvidence(" Expected : " + expColor2.ToString());
            GlobalLog.LogEvidence("---------------------------------------------------");
            GlobalLog.LogEvidence("--FINAL RESULTS: " + b1 + " / " + b2);
            GlobalLog.LogEvidence("---------------------------------------------------");
            
            if (b1 && b2)
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
