// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test ***************************************************
*     Purpose:
*       To verify Animation-related methods, properties, and events when a ContentElement
*       is animated. The parameter (passed to the test case) specifies one of the following:
*           ANIMATION TYPE: the way the animation is carried out
*               (1)  BeginAnimation
*               (2)  BeginAnimation with HandoffBehavior
*               (3)  AnimationClock
*               (4)  AnimationClock with immediate Pause and Resume
*               (5)  AnimationClock with immediate Seek
*               (6)  AnimationClock with immediate Stop
*               (7)  AnimationClock with immediate SkipToFill
*               (8)  AnimationClock with immediate Remove
*               (9)  AnimationClock with HandoffBehavior
*               (10) AnimationClock with HandoffBehavior and immediate Pause
*               (11) AnimationClock with HandoffBehavior and Stop at end
*               (12) AnimationClock with HandoffBehavior and Remove at end
*               (13) BeginStoryboard(storyboard)
*               (14) BeginStoryboard(storyboard, HandoffBehavior)
*               (15) BeginStoryboard(storyboard, HandoffBehavior, isControllable)
*
*     Pass Conditions:
*       The test case passes if (a) APIs return the correct values, and (b) events fire appropriately.
*
*     How verified:
*       APIs are checked after the Animation finishes.
*
*     Framework:          An Avalon executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:               
********************************************************************************************/
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Navigation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.ContentElement</area>
    /// <priority>2</priority>
    /// <description>
    /// Verification of animation of a ContentElement.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.ContentElement", "ContentElementTest")]

    class ContentElementTest : XamlTest
    {
        #region Test case members
        private string                       _inputString = "";
        #endregion


        #region Constructor

        [Variation("BeginAnimation", Priority=0)]
        [Variation("BeginAnimationHandoff", Priority=0)]
        [Variation("AnimationClock")]
        [Variation("AnimationClockPauseResume")]
        [Variation("AnimationClockSeek", Priority=0)]
        [Variation("AnimationClockSeekAligned")]
        [Variation("AnimationClockStop")]
        [Variation("AnimationClockSkipToFill")]
        [Variation("AnimationClockRemove")]
        [Variation("AnimationClockHandoff")]
        [Variation("AnimationClockHandoffPause")]
        [Variation("AnimationClockHandoffRemoveAtEnd")]
        [Variation("AnimationClockHandoffStopAtEnd")]
        [Variation("BeginStoryboard")]
        [Variation("BeginStoryboardHandoff")]
        [Variation("BeginStoryboardHandoffControllable")]

        /******************************************************************************
        * Function:          ContentElementTest Constructor
        ******************************************************************************/
        public ContentElementTest(string testValue): base(@"ContentElement.xaml")
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Animate);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initializing---");
            
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Starting the Animation here.
        /// </summary>
        /// <returns></returns>
        TestResult Animate()
        {
            GlobalLog.LogStatus("---Animate---");
            
            Window.Height   = 300d;
            Window.Width    = 300d;

            ContentElementB elementB = new ContentElementB();
            bool testPassed = elementB.StartTest( _inputString, Window );
                
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
