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
    /// <priority>2</priority>
    /// <description>
    /// Verify adding a animation defined in code to a Storyboard defined in Markup Resources.
    /// The animated DP is an Animatable (SolidColorBrush.Opacity). 
    /// Variation 1: The animated DP in Markup is Width; the animated dp in Code is Height.
    /// Variation 2: The animated DP in Markup is Width; the animated dp in Code is Opacity
    ///     on a SolidColorBrush.
    /// Variation 3: The animated DP in Markup is Color on a SolidColorBrush; the animated
    ///     dp in Code is Opacity on a SolidColorBrush.
    /// Variation 4: The animated DP in Markup is Color on a SolidColorBrush; the animated
    ///     dp in Code is Height.
    /// Variation 5: like Variation 3, but SetTargetName is applied to the animated element
    ///     itself (TextBlock). (The first three variations apply the animation to the animated
    ///     element's parent (Canvas)).
    /// </description>
    /// </summary>
    [Test(2, "Storyboards.HighLevelScenarios", "AddAnimationToStoryboardTest")]
    public class AddAnimationToStoryboardTest : XamlTest
    {
        #region Test case members

        public static VisualVerifier    verifier;
        private string                  _inputString;
        private SolidColorBrush         _textblockSCB;
        private FrameworkElement        _parentElement;
        private Storyboard              _resourceStoryboard;
        private DispatcherTimer         _aTimer              = null;
        
        #endregion


        #region Constructor

        [Variation("Var1", Priority=0)]
        // [DISABLE WHILE PORTING]
        // [Variation("Var2")]
        // [Variation("Var3")]
        [Variation("Var4")]
        // [DISABLE WHILE PORTING]
        // [Variation("Var5", Priority=1)]
        // [Variation("Var6")]

        /******************************************************************************
        * Function:          AddAnimationToStoryboardTest Constructor
        ******************************************************************************/
        public AddAnimationToStoryboardTest(string variation) : base(@"AddAnimationToStoryboard.xaml")
        {
            _inputString = variation;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(FindStoryboardInResource);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
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
        TestResult Initialize()
        {
            Window.Title = "Animation";
            verifier = new VisualVerifier();

            _textblockSCB = (SolidColorBrush)RootElement.FindName("textblockSCB");

            if (_textblockSCB == null)
            {
                GlobalLog.LogEvidence("A SolidColorBrush was not found.");
                return TestResult.Fail;
            }
            else
            {
                if (_inputString == "Var5")
                {
                    //Test 5: the "parent element" is the container of the animated element.
                    _parentElement = (FrameworkElement)RootElement.FindName("canvas1");
                }
                else if (_inputString == "Var1" || _inputString == "Var2" ||_inputString == "Var3" ||_inputString == "Var4" ||_inputString == "Var6")
                {
                    //Test 1-4,6: the "parent element" is the animated element.
                    _parentElement = (FrameworkElement)RootElement.FindName("textblock1");
                }
                else
                {
                    GlobalLog.LogEvidence("The 2nd parameter (" + _inputString + ") is incorrect.\nIt must be one of the following: Var1, Var2, Var3, Var4, Var5, or Var6.");
                    return TestResult.Fail;
                }

                if (_parentElement == null)
                {
                    GlobalLog.LogEvidence("The parent element was not found.");
                    return TestResult.Fail;
                }
                else
                {
                    GlobalLog.LogStatus("The parent element was found.");
                    return TestResult.Pass;
                }
            }
        }

        /******************************************************************************
           * Function:          FindStoryboardInResource
           ******************************************************************************/
        /// <summary>
        /// Retrieve the Storyboard that is specified in the Resources section in Markup.
        /// Then add a new Animation to the Storyboard, and start it.
        /// <returns>Returns success if the Storyboard was found</returns>
        /// </summary>
        private TestResult FindStoryboardInResource()
        {
            GlobalLog.LogStatus("---FindStoryboardInResource---");

            switch (_inputString)
            {
                case "Var1":
                    //Verify Width animated in Markup and Height animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey1");
                    AddHeightAnimation();
                    break;
                case "Var2":
                    //Verify Width animated in Markup and Opacity animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey1");
                    AddOpacityAnimation();
                    break;
                case "Var3":
                    //Verify Color animated in Markup and Opacity animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey2");
                    AddOpacityAnimation();
                    break;
                case "Var4":
                    //Verify Color animated in Markup and Height animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey2");
                    AddHeightAnimation();
                    break;
                case "Var5":
                    //Verify Color animated in Markup and Opacity animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey2");
                    AddOpacityAnimation();
                    break;
                case "Var6":
                    //Verify Width animated in Markup and again animated in Code.
                    _resourceStoryboard = (Storyboard)RootElement.FindResource("StoryKey1");
                    AddWidthAnimation();
                    break;
            }

            //NOTE: the parentElement used to start the animation may not be the one that is
            //actually animating.  It may be its 'containing element'.
            _resourceStoryboard.Begin(_parentElement);  //START THE ANIMATION.

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
        * Function:          AddHeightAnimation
        ******************************************************************************/
        /// <summary>
        /// Adds an additional DoubleAnimation for Height to the Storyboard from Markup.
        /// (for Height).
        /// </summary>
        /// <returns></returns>
        private void AddHeightAnimation()
        {
            GlobalLog.LogStatus("---AddHeightAnimation---");

            _textblockSCB.Opacity = 1;  //Overriding the value set in Markup, to verify the Height animation.

            DoubleAnimation animHeight = new DoubleAnimation();
            animHeight.BeginTime    = TimeSpan.FromMilliseconds(0);
            animHeight.Duration     = new Duration(TimeSpan.FromMilliseconds(1000));
            animHeight.To           = 350d;

            PropertyPath pathHeight = new PropertyPath("(0)", new DependencyProperty[] {FrameworkElement.HeightProperty});
            Storyboard.SetTargetProperty(animHeight, pathHeight);
            Storyboard.SetTargetName(_resourceStoryboard, _parentElement.Name);

            _resourceStoryboard.Children.Add(animHeight);
        }

        /******************************************************************************
        * Function:          AddWidthAnimation
        ******************************************************************************/
        /// <summary>
        /// Adds an additional DoubleAnimation for Width to the Storyboard from Markup.
        /// </summary>
        /// <returns></returns>
        private void AddWidthAnimation()
        {
            GlobalLog.LogStatus("---AddWidthAnimation---");

            _textblockSCB.Opacity = 1;  //Overriding the value set in Markup, to verify the Width animation.

            DoubleAnimation animWidth = new DoubleAnimation();
            animWidth.BeginTime    = TimeSpan.FromMilliseconds(0);
            animWidth.Duration     = new Duration(TimeSpan.FromMilliseconds(1000));
            animWidth.To           = 0d;

            PropertyPath pathWidth = new PropertyPath("(0)", new DependencyProperty[] {FrameworkElement.WidthProperty});
            Storyboard.SetTargetProperty(animWidth, pathWidth);
            Storyboard.SetTargetName(_resourceStoryboard, _parentElement.Name);

            _resourceStoryboard.Children.Add(animWidth);
        }

        /******************************************************************************
        * Function:          AddOpacityAnimation
        ******************************************************************************/
        /// <summary>
        /// Adds an additional DoubleAnimation for Opacity to the Storyboard from Markup.
        /// (for Opacity).
        /// </summary>
        /// <returns></returns>
        private void AddOpacityAnimation()
        {
            GlobalLog.LogStatus("---AddOpacityAnimation---");

            DoubleAnimation animOpacity = new DoubleAnimation();
            animOpacity.BeginTime   = TimeSpan.FromMilliseconds(0);
            animOpacity.Duration    = new Duration(TimeSpan.FromMilliseconds(1000));
            animOpacity.To          = 1d;

            PropertyPath pathOpacity = new PropertyPath("(0)", new DependencyProperty[] {SolidColorBrush.OpacityProperty} );
            Storyboard.SetTargetProperty(animOpacity, pathOpacity);
            Storyboard.SetTargetName(_resourceStoryboard, "textblockSCB");

            _resourceStoryboard.Children.Add(animOpacity);
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
            GlobalLog.LogStatus("---StartTimer---");

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,2000);
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
            int x           = 0;
            int y           = 0;
            Color expColor  = Colors.Black;

            switch (_inputString)
            {
                case "Var1":    //Verify Width and Height.
                    x = 300;
                    y = 300;
                    expColor = Colors.MediumVioletRed;
                    break;
                case "Var2":    //Verify Width and Opacity.
                    x = 300;
                    y = 75;
                    expColor = Colors.MediumVioletRed;
                    break;
                case "Var3":    //Verify Color and Opacity.
                    x = 75;
                    y = 75;
                    expColor = Colors.White;
                    break;
                case "Var4":    //Verify Color and Height.
                    x = 75;
                    y = 300;
                    expColor = Colors.White;
                    break;
                case "Var5":    //Verify Color and Opacity.
                    x = 75;
                    y = 75;
                    expColor = Colors.White;
                    break;
                case "Var6":    //Verify Width and Width again.
                    x = 50;
                    y = 50;
                    expColor = Colors.MediumSlateBlue;
                    break;
            }

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
