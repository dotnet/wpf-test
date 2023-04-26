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
    /// <area>Storyboards.PropertyMethodEvent.Regressions</area>

    [Test(2, "Storyboards.PropertyMethodEvent.Regressions", "ReadOnlyPropertyTest")]
    public class ReadOnlyPropertyTest : XamlTest
    {

        #region Test case members

        private Button              _animatedElement;
        private Storyboard          _resourceStoryboard;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          ReadOnlyPropertyTest Constructor
        ******************************************************************************/
        public ReadOnlyPropertyTest() : base(@"ReadOnlyProperty.xaml")
        {
            InitializeSteps += new TestStep(GetElement);
            RunSteps += new TestStep(FindStoryboardInResource);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(StartStoryboard);
        }

        #endregion


        #region Test Steps

        
        /******************************************************************************
        * Function:          GetElement
        ******************************************************************************/
        /// <summary>
        /// Retrieves the animated element from the markup.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult GetElement()
        {
            _animatedElement = (Button)AnimationUtilities.FindElement(RootElement, "button1");
            
            if (_animatedElement == null)
            {
                GlobalLog.LogEvidence("The animated element was not found.");
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("The animated element was found.");
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
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
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
            _aTimer.Stop();
            Signal("AnimationDone", TestResult.Pass);
        }
        
        /******************************************************************************
        * Function:          StartStoryboard
        ******************************************************************************/
        /// <summary>
        /// Starts the Storyboard, which should throw an exception.
        /// </summary>
        /// <returns></returns>
        private TestResult StartStoryboard()
        {
            SetExpectedErrorTypeInStep(typeof(System.InvalidOperationException), "Outer");                
            _animatedElement.BeginStoryboard(_resourceStoryboard);     //This throws the exception.
            
            WaitForSignal("AnimationDone");

            return TestResult.Pass;
        }

        #endregion
    }
}
