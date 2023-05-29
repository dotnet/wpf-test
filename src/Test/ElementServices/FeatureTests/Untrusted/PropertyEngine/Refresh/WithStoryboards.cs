// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithStoryboardsTest
{
    /******************************************************************************
    * CLASS:          WithStoryboards
    ******************************************************************************/
    [Test(0, "PropertyEngine.WithStoryboards", TestCaseSecurityLevel.FullTrust, "WithStoryboards")]
    public class WithStoryboards : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          WithStoryboards Constructor
        ******************************************************************************/
        public WithStoryboards()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            Utilities.StartRunAllTests("WithStoryboards");

            new WarmupNotCountAsTest().ShowDialog();

            Utilities.Assert(new Storyboards1_1().ShowDialog() == true, "Storyboards 1.1");
            Utilities.Assert(new Storyboards1_2().ShowDialog() == true, "Storyboards 1.2");
            Utilities.Assert(new Storyboards1_3().ShowDialog() == true, "Storyboards 1.3");
            Utilities.Assert(new Storyboards1_4().ShowDialog() == true, "Storyboards 1.4");

            Utilities.Assert(new Storyboards1_6().ShowDialog() == true, "Storyboards 1.6");
            Utilities.Assert(new Storyboards1_7().ShowDialog() == true, "Storyboards 1.7");
            Utilities.Assert(new Storyboards1_8().ShowDialog() == true, "Storyboards 1.8");
            Utilities.Assert(new Storyboards1_9().ShowDialog() == true, "Storyboards 1.9");
            Utilities.Assert(new Storyboards1_10().ShowDialog() == true, "Storyboards 1.10");

            Utilities.StopRunAllTests();

            return TestResult.Pass;
        }
        #endregion
    }

    /******************************************************************************
    * CLASS:          WarmupNotCountAsTest
    ******************************************************************************/
    /// <summary>
    /// Essentially 1.10. But do not validate it.
    /// </summary>
    internal class WarmupNotCountAsTest : StoryboardsModule1
    {
        Button _btn1;
        Button _btn2;
        protected override void TestSetupUI()
        {
            Utilities.PrintStatus("[[ Warm up | Preparation ]] ");
            StackPanel panel = GetStackPanelWithButton(2, false);
            SetMainWindowContent(this, panel);

            _btn1 = (Button)panel.Children[0];
            _btn2 = (Button)panel.Children[1];

            ParallelTimeline timeline1 = GetTimelineTargetBackground(3);
            ParallelTimeline timeline2 = GetTimelineTargetWidth(3);
            BeginTestStoryboard(timeline1, timeline2, _btn1, _btn2);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            if (pulseCount > 70)
            {
                SetResultOK();
                return true;
            }
            else
            {
                return base.PulseEvery100Ms(pulseCount);
            }
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_1
    ******************************************************************************/
    /// <summary>
    /// 1.1 Target Itself, One Window, One timelines, One Animation
    /// </summary>
    internal class Storyboards1_1 : StoryboardsModule1
    {
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.1 Target Itself, One Window, One timelines, One Animation");
            ParallelTimeline timeline = GetTimelineTargetBackground(1);
            SetMainWindowContent(this, null);
            BeginTestStoryboard(timeline, null, null, null);
        }

        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 1, this);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_2
    ******************************************************************************/
    /// <summary>
    /// 1.2 Target Itself, One Window, One timelines, Two Animations  
    /// </summary>
    internal class Storyboards1_2 : StoryboardsModule1
    {
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.2 Target Itself, One Window, One timelines, Two Animations");
            ParallelTimeline timeline = GetTimelineTargetBackground(2);
            SetMainWindowContent(this, null);
            BeginTestStoryboard(timeline, null, null, null);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 2, this);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_3
    ******************************************************************************/
    /// <summary>
    /// 1.3 Target Itself, One Window, One timelines, Three Animations 
    /// </summary>
    internal class Storyboards1_3 : StoryboardsModule1
    {
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("Target Itself, One Window, One timelines, Three Animations ");
            ParallelTimeline timeline = GetTimelineTargetBackground(3);
            SetMainWindowContent(this, null);
            BeginTestStoryboard(timeline, null, null, null);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 3, this);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_4
    ******************************************************************************/
    /// <summary>
    /// 1.4 Target Itself, One Window, Two timelines, Three Animations each
    /// </summary>
    internal class Storyboards1_4 : StoryboardsModule1
    {
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.4 Target Itself, One Window, Two timelines, Three Animations each");
            ParallelTimeline timeline1 = GetTimelineTargetBackground(3);
            ParallelTimeline timeline2 = GetTimelineTargetWidth(3);
            SetMainWindowContent(this, null);
            BeginTestStoryboard(timeline1, timeline2, null, null);
        }

        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 6, this);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_6
    ******************************************************************************/
    /// <summary>
    /// 1.6 Target child, One Button, One timelines, One Animation
    /// </summary>
    internal class Storyboards1_6 : StoryboardsModule1
    {
        Button _btn1;
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.6 Target child, One Button, One timelines, One Animation");
            StackPanel panel = GetStackPanelWithButton(1, false);
            SetMainWindowContent(this, panel);
            _btn1 = (Button)panel.Children[0];

            ParallelTimeline timeline = GetTimelineTargetBackground(1);
            BeginTestStoryboard(timeline, null, _btn1, null);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 1, _btn1);
            return base.PulseEvery100Ms(pulseCount);
        }
    }


    /******************************************************************************
    * CLASS:          Storyboards1_7
    ******************************************************************************/
    /// <summary>
    /// 1.7 Target child, One Button, One timelines, Two Animations  
    /// </summary>
    internal class Storyboards1_7 : StoryboardsModule1
    {
        Button _btn1;
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.7 Target child, One Button, One timelines, Two Animations ");
            StackPanel panel = GetStackPanelWithButton(1, false);
            SetMainWindowContent(this, panel);
            _btn1 = (Button)panel.Children[0];
            ParallelTimeline timeline = GetTimelineTargetBackground(2);
            BeginTestStoryboard(timeline, null, _btn1, null);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 2, _btn1);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_8
    ******************************************************************************/
    /// <summary>
    /// 1.8 Target child, One Button, One timelines, Three Animations 
    /// </summary>
    internal class Storyboards1_8 : StoryboardsModule1
    {
        Button _btn1;
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.8 Target child, One Button, One timelines, Three Animations ");
            StackPanel panel = GetStackPanelWithButton(1, false);
            SetMainWindowContent(this, panel);
            _btn1 = (Button)panel.Children[0];

            ParallelTimeline timeline = GetTimelineTargetBackground(3);
            BeginTestStoryboard(timeline, null, _btn1, null);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 3, _btn1);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_9
    ******************************************************************************/
    /// <summary>
    /// 1.9 Target child, One Button, Two timelines, Three Animations each 
    /// </summary>
    internal class Storyboards1_9 : StoryboardsModule1
    {
        Button _btn1;
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.9 Target child, One Button, Two timelines, Three Animations each ");
            StackPanel panel = GetStackPanelWithButton(1, false);
            SetMainWindowContent(this, panel);
            _btn1 = (Button)panel.Children[0];
            ParallelTimeline timeline1 = GetTimelineTargetBackground(3);
            ParallelTimeline timeline2 = GetTimelineTargetWidth(3);
            BeginTestStoryboard(timeline1, timeline2, _btn1, _btn1);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            CheckEvery100Ms(pulseCount, 6, _btn1);
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          Storyboards1_10
    ******************************************************************************/
    /// <summary>
    /// 1.10 Target child, Two Buttons, One timelines each, Three Animations each
    /// </summary>
    internal class Storyboards1_10 : StoryboardsModule1
    {
        Button _btn1;
        Button _btn2;
        protected override void TestSetupUI()
        {
            Utilities.PrintTitle("1.10 Target child, Two Buttons, One timelines each, Three Animations each");
            StackPanel panel = GetStackPanelWithButton(2, false);
            SetMainWindowContent(this, panel);
            _btn1 = (Button)panel.Children[0];
            _btn2 = (Button)panel.Children[1];
            ParallelTimeline timeline1 = GetTimelineTargetBackground(3);
            ParallelTimeline timeline2 = GetTimelineTargetWidth(3);
            BeginTestStoryboard(timeline1, timeline2, _btn1, _btn2);
        }
        protected override bool PulseEvery100Ms(int pulseCount)
        {
            if (pulseCount < 40)
            {
                CheckEvery100Ms(pulseCount, 6, _btn1);
            }
            else
            {
                CheckEvery100Ms(pulseCount, 6, _btn2);
            }
            return base.PulseEvery100Ms(pulseCount);
        }
    }

    /******************************************************************************
    * CLASS:          StoryboardsModule1
    ******************************************************************************/
    /// <summary>
    /// Used by tests 1.1 to 1.4, 1.6-1.10, 2.1-2.4
    /// </summary>
    internal class StoryboardsModule1 : StoryboardsWindow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pulseCount"></param>
        /// <param name="numOfSetters">1, 2, 3, or 6</param>
        /// <param name="testCtrl">Control to apply validation logic against</param>
        /// <returns></returns>
        protected bool CheckEvery100Ms(int pulseCount, int numOfSetters, Control testCtrl)
        {
            switch (pulseCount)
            {
                case 4:
                    WindowAssert(testCtrl.Background.ToString() == GrayString, "Background: Before");
                    if (numOfSetters == 6)
                    {
                        WindowAssert(testCtrl.Width == 500d, "Width: Before");
                    }
                    break;
                case 14:
                    WindowAssert(testCtrl.Background.ToString() == RedString, "Background: Red");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 20:
                    if (numOfSetters == 1)
                    {
                        WindowAssert(testCtrl.Background.ToString() == GrayString, "Background: After");
                    }
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 22:
                    if (numOfSetters == 1)
                    {
                        SetResultOK();
                    }
                    break;
                case 24:
                    WindowAssert(testCtrl.Background.ToString() == BlueString, "Background: Blue");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 30:
                    if (numOfSetters == 2)
                    {
                        WindowAssert(testCtrl.Background.ToString() == GrayString, "Background: After");
                    }
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 32:
                    if (numOfSetters == 2)
                    {
                        SetResultOK();
                    }
                    break;
                case 34:
                    WindowAssert(testCtrl.Background.ToString() == GreenString, "Background: Green");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 40:
                    if (numOfSetters == 3)
                    {
                        WindowAssert(testCtrl.Background.ToString() == GrayString, "Background: After");
                    }
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Background.ToString());
                    }
                    break;
                case 42:
                    if (numOfSetters == 3)
                    {
                        SetResultOK();
                    }
                    else
                    {
                        WindowAssert(testCtrl.Width == 50d, "Width: 50");
                        if (IsResultSetAbort)
                        {
                            Utilities.PrintStatus(testCtrl.Width.ToString());
                        }
                    }
                    break;
                case 52:
                    WindowAssert(testCtrl.Width == 150d, "Width: 150");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Width.ToString());
                    }
                    break;
                case 62:
                    WindowAssert(testCtrl.Width == 300d, "Width: 300");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Width.ToString());
                    }
                    break;
                case 70:
                    WindowAssert(testCtrl.Width == 500d, "Width: Before");
                    if (IsResultSetAbort)
                    {
                        Utilities.PrintStatus(testCtrl.Width.ToString());
                    }
                    break;
                case 72:
                    SetResultOK();
                    break;
            }
            return false;
        }
    }

    /******************************************************************************
    * CLASS:          StoryboardsWindow
    ******************************************************************************/
    /// <summary>
    /// It provides some commonly used service for StoryBoards tests.
    /// </summary>
    internal class StoryboardsWindow : PEApplication
    {
        protected void SetOnMainWindow(Window mainWindow, Timeline timeline1, Timeline timeline2)
        {
            mainWindow.Background = Brushes.Gray;
            mainWindow.Width = 500d;
            mainWindow.Left = 300;
            mainWindow.Top = 300;
        }

        protected void BeginTestStoryboard(Timeline timeline1, Timeline timeline2, Button btn1, Button btn2)
        {
            System.Diagnostics.Debug.Assert(timeline1 != null, "Debug BeginStoryboardOnButton Precondition");
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(timeline1);
            if (btn1 != null)
            {
                storyboard.Begin(btn1);
            }
            else
            {
                storyboard.Begin(this);
            }
            if (timeline2 != null)
            {
                storyboard = new Storyboard();
                storyboard.Children.Add(timeline2);
                if (btn2 != null)
                {
                    storyboard.Begin(btn2);
                }
                else
                {
                    storyboard.Begin(this);
                }
            }
        }

        protected void SetMainWindowContent(Window mainWindow, StackPanel panel)
        {
            if (panel != null)
            {
                mainWindow.Content = panel;
            }
            mainWindow.Background = Brushes.Gray;
            mainWindow.Top = 300;
            mainWindow.Left = 300;
            mainWindow.Width = 500d;
        }

        protected StackPanel GetStackPanelWithButton(int numOfButton, bool isUsedForStyleStoryboards)
        {
            System.Diagnostics.Debug.Assert(numOfButton >= 1 && numOfButton <= 5, "Support 1 or 2 buttons");

            StackPanel panel = new StackPanel();
            Button btn1 = new Button();
            panel.Children.Add(btn1);

            if (!isUsedForStyleStoryboards)
            {
                btn1.Content = "Test Button";
                btn1.Name = "Test_Button";
                btn1.Background = Brushes.Gray;
                btn1.Width = 500d;
            }

            if (numOfButton >= 2)
            {
                Button btn2 = new Button();
                panel.Children.Add(btn2);
                if (!isUsedForStyleStoryboards)
                {
                    btn2.Content = "Second Button";
                    btn2.Name = "Second_Button";
                    btn2.Background = Brushes.Gray;
                    btn2.Width = 500d;
                }
            }

            //For more than three, it has to be used with Style.Storyboards test
            for (int i = 3; i <= numOfButton; i++)
            {
                Button btnMore = new Button();
                panel.Children.Add(btnMore);
            }

            return panel;
        }

        private void GetTimelineTargetBackgroundHelper(ColorAnimation animation)
        {
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0).(1)", Button.BackgroundProperty, SolidColorBrush.ColorProperty));
        }

        protected ParallelTimeline GetTimelineTargetBackground(int numOfSetters)
        {
            System.Diagnostics.Debug.Assert(numOfSetters >= 1 && numOfSetters <= 3, "number of setters should be between 1 and 3, inclusive");
            ParallelTimeline timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromSeconds(1);

            ColorAnimation animationColorRed = new ColorAnimation(Colors.Red, Colors.Red, TimeSpan.FromSeconds(1));
            animationColorRed.BeginTime = TimeSpan.FromSeconds(0);
            GetTimelineTargetBackgroundHelper(animationColorRed);

            timeline.Children.Add(animationColorRed);

            if (numOfSetters >= 2)
            {
                ColorAnimation animationColorBlue = new ColorAnimation(Colors.Blue, Colors.Blue, TimeSpan.FromSeconds(1));
                animationColorBlue.BeginTime = TimeSpan.FromSeconds(1);
                GetTimelineTargetBackgroundHelper(animationColorBlue);

                timeline.Children.Add(animationColorBlue);
            }

            if (numOfSetters >= 3)
            {
                ColorAnimation animationColorGreen = new ColorAnimation(Colors.Green, Colors.Green, TimeSpan.FromSeconds(1));
                animationColorGreen.BeginTime = TimeSpan.FromSeconds(2);
                GetTimelineTargetBackgroundHelper(animationColorGreen);

                timeline.Children.Add(animationColorGreen);
            }
            return timeline;
        }

        private void GetTimelineTargetWidthHelper(DoubleAnimation animation)
        {
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTargetProperty(animation, new PropertyPath(Button.WidthProperty));
        }

        protected ParallelTimeline GetTimelineTargetWidth(int numOfSetters)
        {
            System.Diagnostics.Debug.Assert(numOfSetters >= 1 && numOfSetters <= 3, "number of setters should be between 1 and 3, inclusive");
            ParallelTimeline timeline = new ParallelTimeline();
            timeline.BeginTime = TimeSpan.FromSeconds(1);

            DoubleAnimation animation1 = new DoubleAnimation(50d, 50d, TimeSpan.FromSeconds(1));
            animation1.BeginTime = TimeSpan.FromSeconds(3);
            GetTimelineTargetWidthHelper(animation1);

            timeline.Children.Add(animation1);

            if (numOfSetters >= 2)
            {
                DoubleAnimation animation2 = new DoubleAnimation(150d, 150d, TimeSpan.FromSeconds(1));
                animation2.BeginTime = TimeSpan.FromSeconds(4);
                GetTimelineTargetWidthHelper(animation2);

                timeline.Children.Add(animation2);
            }

            if (numOfSetters >= 3)
            {
                DoubleAnimation animation3 = new DoubleAnimation(300d, 300d, TimeSpan.FromSeconds(1));
                animation3.BeginTime = TimeSpan.FromSeconds(5);
                GetTimelineTargetWidthHelper(animation3);

                timeline.Children.Add(animation3);
            }
            return timeline;
        }
    }
}
