// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test ***************************************************
*     Purpose:
*       To verify that Animations behave appropriately when invoked via Navigation-related
*       events. Two parameters (passed to the test case) are varied:
*           ANIMATION TYPE: the way the animation is carried out
*               (1) BeginAnimation
*               (2) AnimationClock
*               (3) Storyboard.Begin
*               (4) BeginStoryboard
*               (5) Storyboard in an EventTrigger
*               (6) Animation defined in Markup using Resources
*           EVENT USED: the event used to invoke the Animation
*               (1) Application Navigated
*               (2) Application Navigating
*               (3) NavigationWindow Navigated
*               (4) NavigationWindow Navigating
*               (5) Element Unloaded
*               (6) NavigationWindow Closing
*               (7) NavigationWindow Closed
*
*     Pass Conditions:
*       In most cases, a test case passes if CurrentStateInvalidated fires when the Animation starts.
*       However, verification for the Closing/Closed tests consists only to check that no exception
*       is thrown; otherwise, they automatically pass.
*
*     How verified:
*       CurrentStateInvalidated is checked.
*
*     Requirements / Restrictions:
*
*     Running the Test:
*           From the command line, pass a string specifying two values
*           (1) the type of Animation to be carried out, e.g., AnimationClock
*           (2) the event to be tested, e.g., Navigated
*           
*     Framework:          An Avalon executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationCommon.dll
*     Support Files:               
********************************************************************************************/
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Markup;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Navigating</area>
    /// <description>
    /// Verification that Animations behave appropriately when invoked via Navigation-related events
    /// </description>
    /// <


    [Test(3, "Animation.HighLevelScenarios.Navigating", "NavigatingTest", SupportFiles=@"FeatureTests\Animation\Navigating1.xaml,FeatureTests\Animation\Navigating2.xaml")]

    class NavigatingTest : AvalonTest
    {
        #region Test case members
        
        private NavigationWindow    _navWin;
        public static  NavigatingB  navigatingB;
        private string              _animType        = "";
        public static string        winEvent        = "";
        public static bool          navigated       = false;
        public  bool                testPassed      = false;
        
        #endregion


        #region Constructor

        [Variation("BeginAnimation", "Navigated")]
        [Variation("BeginAnimation", "Navigating")]
        [Variation("BeginAnimation", "AppNavigated", Priority=1)]
        [Variation("BeginAnimation", "AppNavigating", Priority=0)]
        [Variation("BeginAnimation", "Unloaded")]
        [Variation("BeginAnimation", "Closing")]
        [Variation("BeginAnimation", "Closed")]

        [Variation("AnimationClock", "Navigated")]
        [Variation("AnimationClock", "Navigating")]
        [Variation("AnimationClock", "AppNavigated")]
        [Variation("AnimationClock", "AppNavigating")]
        [Variation("AnimationClock", "Unloaded")]
        [Variation("AnimationClock", "Closing")]
        [Variation("AnimationClock", "Closed")]

        [Variation("StoryboardBegin", "Navigated")]
        [Variation("StoryboardBegin", "Navigating")]
        [Variation("StoryboardBegin", "AppNavigated")]
        [Variation("StoryboardBegin", "AppNavigating", Priority=0)]
        [Variation("StoryboardBegin", "Unloaded")]
        [Variation("StoryboardBegin", "Closing")]
        [Variation("StoryboardBegin", "Closed")]

        [Variation("BeginStoryboard", "Navigated")]
        [Variation("BeginStoryboard", "Navigating")]
        [Variation("BeginStoryboard", "AppNavigated")]
        [Variation("BeginStoryboard", "AppNavigating", Priority=0)]
        [Variation("BeginStoryboard", "Unloaded")]
        [Variation("BeginStoryboard", "Closing")]
        [Variation("BeginStoryboard", "Closed")]

        [Variation("PropertyTrigger", "Navigated")]
        [Variation("PropertyTrigger", "Navigating")]
        [Variation("PropertyTrigger", "AppNavigated")]
        [Variation("PropertyTrigger", "AppNavigating")]
        [Variation("PropertyTrigger", "Unloaded")]
        [Variation("PropertyTrigger", "Closing")]
        [Variation("PropertyTrigger", "Closed")]

        [Variation("AnimationInMarkup", "Navigated")]
        [Variation("AnimationInMarkup", "Navigating")]
        [Variation("AnimationInMarkup", "AppNavigated", Priority=0)]
        [Variation("AnimationInMarkup", "AppNavigating")]
        [Variation("AnimationInMarkup", "Unloaded")]
        [Variation("AnimationInMarkup", "Closing")]
        [Variation("AnimationInMarkup", "Closed")]

        /******************************************************************************
        * Function:          NavigatingTest Constructor
        ******************************************************************************/
        public NavigatingTest(string testValue1, string testValue2)
        {
            _animType = testValue1;
            winEvent = testValue2;
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(Navigate1);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CheckInputParameters
        ******************************************************************************/
        /// <summary>
        /// CheckInputParameters: checks for a valid input string.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult CheckInputParameters()
        {
            bool        arg1Found   = false;
            bool        arg2Found   = false;
            string      errMessage  = "";
            string[]    expList1    = { "BeginAnimation", "AnimationClock", "StoryboardBegin", "BeginStoryboard", "PropertyTrigger", "AnimationInMarkup" };
            string[]    expList2    = { "Navigated", "Navigating", "AppNavigated", "AppNavigating", "Unloaded", "Closing", "Closed" };

            arg1Found = AnimationUtilities.CheckInputString(_animType, expList1, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(winEvent, expList2, ref errMessage);
                if (errMessage != "")
                {
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                }
            }

            if (arg1Found && arg2Found)
            {
                navigatingB = new NavigatingB();
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          Navigate1
        ******************************************************************************/
        /// <summary>
        /// Navigate1:  Navigate to the first .xaml page.
        /// </summary>
        /// <returns></returns>
        TestResult Navigate1()
        {
            GlobalLog.LogStatus("---Navigating--");

            if (_animType != "" && winEvent != "")
            {
                _navWin = new NavigationWindow();
                _navWin.Left                 = 0d;
                _navWin.Top                  = 0d;
                _navWin.Height               = 300d;
                _navWin.Width                = 300d;
                _navWin.WindowStyle          = WindowStyle.None;
                _navWin.Title                = "Animation";
                _navWin.Show();
                _navWin.ContentRendered += new EventHandler(OnContentRendered);

                AppNav appNav = new AppNav();
                appNav.SetApplicationEvents();

                //navWin.Navigate(new Uri(@"Navigating1.xaml", UriKind.RelativeOrAbsolute));
                _navWin.Content = (Page)XamlReader.Load(File.OpenRead("Navigating1.xaml"));

                return TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("ERROR!! Input parameter(s) missing. \n");
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// StartTest:  Begin the test.
        /// </summary>
        /// <returns></returns>
        TestResult StartTest()
        {
            GlobalLog.LogStatus("------StartTest------");

            WaitForSignal("PageLoaded");

            if (!navigated)
            {
                navigated = true;
                testPassed = navigatingB.ContinueTest(_animType, winEvent, _navWin);

                GlobalLog.LogStatus("---Verifying---");
                if (testPassed)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the .xaml page renders.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("------OnContentRendered------");
            Signal("PageLoaded", TestResult.Pass);
        }

        #endregion
    }

    class AppNav : Application
    {
        //Constructor.
        public AppNav() { }

        /******************************************************************************
           * Function:          SetApplicationEvents
           ******************************************************************************/
        public void  SetApplicationEvents()
        {
            Application app = Application.Current;
            app.Navigating += new NavigatingCancelEventHandler(OnAppNavigating);
            app.Navigated  += new NavigatedEventHandler(OnAppNavigated);
        }
        /******************************************************************************
           * Function:          OnAppNavigating
           ******************************************************************************/
        private void OnAppNavigating(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnAppNavigating---");
            if (NavigatingTest.winEvent == "AppNavigating" && NavigatingTest.navigated)
            {
                NavigatingTest.navigatingB.StartAnimation();
            }
        }

        /******************************************************************************
           * Function:          OnAppNavigated
           ******************************************************************************/
        private void OnAppNavigated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnAppNavigated--");
            if (NavigatingTest.winEvent == "AppNavigated" && NavigatingTest.navigated)
            {
                NavigatingTest.navigatingB.StartAnimation();
            }
        }

    }
}
