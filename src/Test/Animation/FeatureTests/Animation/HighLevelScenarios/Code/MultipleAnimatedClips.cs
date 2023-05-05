// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
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


    [Test(2, "Animation.HighLevelScenarios.Regressions", "MultipleAnimatedClipsTest")]

    class MultipleAnimatedClipsTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private DispatcherTimer     _aTimer              = null;
        private string              _STYLEKEY            = "StyleKey";
        private ResourceDictionary  _rootDictionary      = null;

        #endregion


        #region Constructor


        /******************************************************************************
        * Function:          MultipleAnimatedClipsTest Constructor
        ******************************************************************************/
        public MultipleAnimatedClipsTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize:  Add Window properties and content.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            Window.Left             = 0;
            Window.Top              = 0;
            Window.Height           = 600;
            Window.Width            = 400;
            Window.Title            = "Animation"; 
            Window.WindowStyle      = WindowStyle.None;

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _rootDictionary = new ResourceDictionary();

            NameScope.SetNameScope(Window, new NameScope());

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Background = Brushes.MintCream;

            Button button1 = new Button();
            body.Children.Add(button1);
            button1.Name        = "button1";
            button1.Height      = 150d;
            button1.Width       = 150d;
            button1.Background  = Brushes.Green;
            button1.Content     = "Button1";
            button1.FontSize    = 36d;
            Canvas.SetLeft(button1, 0d);
            Canvas.SetTop(button1,  0d);

            Button button2 = new Button();
            body.Children.Add(button2);
            button2.Name        = "button2";
            button2.Height      = 150d;
            button2.Width       = 150d;
            button2.Background  = Brushes.Blue;
            button2.Content     = "Button2";
            button2.FontSize    = 36d;
            Canvas.SetLeft(button2, 0d);
            Canvas.SetTop(button2,  150d);

            Button button3 = new Button();
            body.Children.Add(button3);
            button3.Name        = "button3";
            button3.Height      = 150d;
            button3.Width       = 150d;
            button3.Background  = Brushes.Yellow;
            button3.Content     = "Button3";
            button3.FontSize    = 36d;
            Canvas.SetLeft(button3, 0d);
            Canvas.SetTop(button3,  300d);

            EllipseGeometry EG5 = new EllipseGeometry();
            EG5.RadiusX               = 75d;
            EG5.RadiusY               = 75d;
            EG5.Center                = new Point(75,75);

            button1.Clip = EG5;
            button2.Clip = EG5;
            button3.Clip = EG5;

            Window.RegisterName(button1.Name, button1);
            Window.RegisterName(button2.Name, button2);
            Window.RegisterName(button3.Name, button3);
            Window.RegisterName("CLIP", EG5); //Provide the Animatable a Name.

            PointAnimation animPoint = new PointAnimation();                                             
            animPoint.BeginTime          = TimeSpan.FromSeconds(0);
            animPoint.Duration           = new Duration(TimeSpan.FromSeconds(2));
            animPoint.To                 = new Point(400,400);

            Storyboard storyboard = new Storyboard();
            storyboard.Name = "story";
            storyboard.Children.Add(animPoint);

            PropertyPath path  = new PropertyPath("(0).(1)", new DependencyProperty[] { Button.ClipProperty, EllipseGeometry.CenterProperty });
            Storyboard.SetTargetProperty(animPoint, path);

            BeginStoryboard beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "BeginStory");
            beginStory.Storyboard = storyboard;
            EventTrigger eventTrigger = AnimationUtilities.CreateEventTrigger(FrameworkElement.LoadedEvent, beginStory);

//            button2.Triggers.Add(eventTrigger); This works!

            Style style = new Style();
            style.TargetType = typeof(Button);
            style.Triggers.Add(eventTrigger);

            _rootDictionary.Add(_STYLEKEY, style);
            ((FrameworkElement)Window).Resources = _rootDictionary;

            button2.Style = (Style)Window.Resources[_STYLEKEY]; //HACK-HACK: why does this work?

            return TestResult.Pass;
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
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
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
            _aTimer.Stop();

            bool testPassed = true;
            Color expColor = Colors.MintCream;  //The color of the page's background.

            testPassed = CheckColor(75,  75, expColor) && testPassed;  //Check Button 1.
            testPassed = CheckColor(75, 225, expColor) && testPassed;  //Check Button 2.
            testPassed = CheckColor(75, 375, expColor) && testPassed;  //Check Button 3.
            
            if (testPassed)
            {
                Signal("AnimationDone", TestResult.Pass);
            }
            else
            {
                Signal("AnimationDone", TestResult.Fail);
            }
        }

        /******************************************************************************
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor(int x1, int y1, Color expectedColor)
        {
            bool passed         = true;
            float tolerance     = 0.001f;
            
            //At each Tick, check the color at the specified point.
            Color actualColor = _verifier.getColorAtPoint(x1, y1);

            passed = AnimationUtilities.CompareColors(expectedColor, actualColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actualColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expectedColor.ToString());

            return passed;
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
            return WaitForSignal("AnimationDone");
        }

        #endregion
    }
}
