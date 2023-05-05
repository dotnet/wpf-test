// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Scalability Test *****************
*     Description:
*          Tests that Animation works with a large number of objects inside a Canvas.
*     Pass Conditions:
*          The test case will Pass if GetCurrentValue returns the correct value for each Animation.
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:

* *******************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
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
    /// <area>Storyboards.LowLevelScenarios.Scaling</area>
    /// <priority>3</priority>
    /// <description>
    /// Scaling Test: Verify that a large number of TextBox animations work
    /// </description>
    /// </summary>
    [Test(3, "Storyboards.LowLevelScenarios.Scaling", "MultipleObjectsTest")]
    public class MultipleObjectsTest : WindowTest
    {
        #region Test case members

        private string                          _animType          = null;
        private Int32                           _animTotal         = 0;
        private Canvas                          _body;
        private Storyboard                      _storyboard;
        private PropertyPath                    _path;
        private int                             _begunCount        = 0;
        private int                             _endedCount        = 0;
        private int                             _TEXTBOX_WIDTH     = 66;
        private int                             _TEXTBOX_HEIGHT    = 22;
        private int                             _ANIM_DURATION     = 5000;
        private int                             _COLUMN_LENGTH     = 32;
        private int                             _top               = 0;
        private int                             _left              = 0;
        private bool                            _pass3             = true;
        private double                          _expDouble         = 0;
        private Color                           _expColor          = Colors.Green;
        private double                          _expDouble2        = 50d;
        private Point                           _expPoint1         = new Point(1,1);
        private Point                           _expPoint2         = new Point(300,300);
        private Rect                            _expRect           = new Rect(0,0,400,400);
        private bool                            _testPassed        = false;
        
        #endregion


        #region Constructor

        [Variation("Color", "1500")]
        [Variation("Double", "1500")]
        [Variation("Double2", "1500")]
        [Variation("Rect", "1500")]
        [Variation("GradientOrigin", "1500")]
        [Variation("Point", "1500")]
        
        /******************************************************************************
        * Function:          MultipleObjectsTest Constructor
        ******************************************************************************/
        public MultipleObjectsTest(string testValue1, string testValue2)
        {
            _animType    = testValue1;
            _animTotal   = Convert.ToInt32(testValue2);
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(Verify);
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
            bool        argFound    = false;
            string      errMessage  = "";
            string[]    expList     = { "Color", "Double", "Double2", "Rect", "GradientOrigin", "Point" };

            argFound = AnimationUtilities.CheckInputString(_animType, expList, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }

            if (argFound)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateTree()
        {
            Window.Width                = 900;
            Window.Height               = 800;
            Window.Title                = "Animation Scaling Test";
            Window.Left                 = 0;
            Window.Top                  = 0;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body = new Canvas();
            _body.Width               = 900;
            _body.Height              = 800;
            _body.Background          = Brushes.BurlyWood;

            //------------------- CREATE STORYBOARD-----------------------------
            GlobalLog.LogStatus("----Create Storyboard----");
            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.BeginTime       = TimeSpan.FromMilliseconds(0);
            _storyboard.Duration        = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION + 2000));
            _storyboard.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidatedStory);

            NameScope.SetNameScope(Window, new NameScope());

            //------------------- CREATE TEXTBOXES AND ANIMATIONS ----------------------
            //Create TextBoxes and assign Animations to them.
            for (int i=0; i<_animTotal; i++)
            {
                CreateTextBox(i, _animType);
            }
            GlobalLog.LogStatus("---All TextBoxes Created---");

            Window.Content = _body;

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
            GlobalLog.LogStatus("---OnContentRendered---Begin the Storyboard");

            _storyboard.Begin(Window, HandoffBehavior.SnapshotAndReplace, true);
        }

        /******************************************************************************
        * Function:          CreateTextBox
        ******************************************************************************/
        /// <summary>
        /// CreateTextBox: Create the number of TextBoxes requested and animate them.
        /// </summary>
        private void CreateTextBox(int j, string animType)
        {
            int width      = _TEXTBOX_WIDTH;
            int height     = _TEXTBOX_HEIGHT;

            if ((j % _COLUMN_LENGTH) == 0)
            {
                _left = ((j/_COLUMN_LENGTH) * width) + 1;
                _top = 0;
            }
            else
            {
                _top = (j % _COLUMN_LENGTH) * height + 1;
            }

            TextBox textbox  = new TextBox();
            textbox.Name                = "TB" + j.ToString();
            textbox.Width               = width;
            textbox.Height              = height;
            textbox.Background          = Brushes.Lavender;
            textbox.Text                = "TextBox" + j.ToString();
            Canvas.SetTop  (textbox, (double)_top);
            Canvas.SetLeft (textbox, (double)_left);
            _body.Children.Add(textbox);

            Window.RegisterName(textbox.Name, textbox);

            AssignAnimation(j, animType, ref textbox);

        }

        /******************************************************************************
        * Function:          AssignAnimation
        ******************************************************************************/
        /// <summary>
        /// AssignAnimation: Create and begin an Animation for each TextBox requested.
        /// </summary>
        private void AssignAnimation(int textboxCount, string animationType, ref TextBox TB)
        {
            //GlobalLog.LogStatus("---AssignAnimation #" + textboxCount + " ---");

            switch (animationType.ToLower(CultureInfo.InvariantCulture))
            {
                case "color":
                    //SCENARIO 1: Color Animation
                    ColorAnimation animColor = new ColorAnimation();
                    animColor.From              = Colors.Red;
                    animColor.To                = _expColor;
                    animColor.BeginTime         = TimeSpan.FromMilliseconds(0);
                    animColor.Duration          = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animColor.CurrentStateInvalidated            += new EventHandler(OnCurrentStateInvalidated);
                    animColor.FillBehavior      = FillBehavior.HoldEnd;

                    SolidColorBrush SCB1 = new SolidColorBrush();
                    SCB1.Color = Colors.Red;
                    TB.Background = SCB1;

                    _storyboard.Children.Add(animColor);
                    _path = new PropertyPath("(0).(1)", TextBox.BackgroundProperty, SolidColorBrush.ColorProperty);
                    Storyboard.SetTargetProperty(animColor, _path);
                    Storyboard.SetTargetName(animColor, TB.Name);
                    break;
                case "double":
                    //SCENARIO 2: Double Animation
                    DoubleAnimation animDouble = new DoubleAnimation();
                    animDouble.From             = 1;
                    animDouble.To               = _expDouble;
                    animDouble.BeginTime        = TimeSpan.FromMilliseconds(0);
                    animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animDouble.CurrentStateInvalidated           += new EventHandler(OnCurrentStateInvalidated);
                    animDouble.FillBehavior     = FillBehavior.HoldEnd;

                    SolidColorBrush SCB2 = new SolidColorBrush();
                    SCB2.Color = Colors.Blue;
                    TB.Background = SCB2;

                    _storyboard.Children.Add(animDouble);
                    _path = new PropertyPath("(0).(1)", TextBox.BackgroundProperty, SolidColorBrush.OpacityProperty);
                    Storyboard.SetTargetProperty(animDouble, _path);
                    Storyboard.SetTargetName(animDouble, TB.Name);
                    break;
                case "double2":
                    //SCENARIO 3: Double Animation (Left Property)
                    DoubleAnimation animDouble2 = new DoubleAnimation();
                    animDouble2.By               = _expDouble2;
                    animDouble2.BeginTime        = TimeSpan.FromMilliseconds(0);
                    animDouble2.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animDouble2.CurrentStateInvalidated           += new EventHandler(OnCurrentStateInvalidated);
                    animDouble2.FillBehavior     = FillBehavior.HoldEnd;

                    _path = new PropertyPath("(0)", Canvas.LeftProperty);
                    Storyboard.SetTargetProperty(animDouble2, _path);
                    _storyboard.Children.Add(animDouble2);
                    Storyboard.SetTargetName(animDouble2, TB.Name);
                    break;
                case "rect":
                    //SCENARIO 4: Rect Animation
                    RectAnimation animRect = new RectAnimation();
                    animRect.To                 = _expRect;
                    animRect.BeginTime          = TimeSpan.FromMilliseconds(0);
                    animRect.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animRect.FillBehavior       = FillBehavior.HoldEnd;
                    animRect.CurrentStateInvalidated             += new EventHandler(OnCurrentStateInvalidated);

                    RectangleGeometry RG = new RectangleGeometry();
                    RG.Rect = new Rect(_left,_top,_TEXTBOX_WIDTH-10,_TEXTBOX_HEIGHT-5);

                    TB.Clip = RG;

                    _storyboard.Children.Add(animRect);
                    _path  = new PropertyPath("(0).(1)", TextBox.ClipProperty, RectangleGeometry.RectProperty);
                    Storyboard.SetTargetProperty(animRect, _path);
                    Storyboard.SetTargetName(animRect, TB.Name);
                    break;
                case "gradientorigin":
                    //SCENARIO 5: Point Animation -- GradientOrigin
                    RadialGradientBrush RGB = new RadialGradientBrush();
                    RGB.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
                    RGB.GradientStops.Add(new GradientStop(Colors.LightGreen, 1.0));
                    RGB.Center          = new Point(0.5, 0.5);
                    RGB.RadiusX         = 1;
                    RGB.RadiusY         = 1;
                    RGB.GradientOrigin  = new Point(0.0, 0.0);

                    PointAnimation animPoint1 = new PointAnimation();
                    animPoint1.To               = _expPoint1;
                    animPoint1.BeginTime        = TimeSpan.FromMilliseconds(0);
                    animPoint1.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animPoint1.CurrentStateInvalidated           += new EventHandler(OnCurrentStateInvalidated);
                    animPoint1.FillBehavior     = FillBehavior.HoldEnd;

                    TB.Background = RGB;

                    _storyboard.Children.Add(animPoint1);
                    _path = new PropertyPath("(0).(1)", TextBox.BackgroundProperty, RadialGradientBrush.GradientOriginProperty);
                    Storyboard.SetTargetProperty(animPoint1, _path);
                    Storyboard.SetTargetName(animPoint1, TB.Name);
                    break;
                case "point":
                    //SCENARIO 6: Point Animation
                    LinearGradientBrush LGB = new LinearGradientBrush();
                    LGB.StartPoint = new Point(0.0, 0.0);
                    LGB.EndPoint = new Point(0.0, 1.0);
                    LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                    GradientStopCollection GSC = new GradientStopCollection();
                    GSC.Add(new GradientStop(Colors.Blue, 0.0));
                    GSC.Add(new GradientStop(Colors.LightBlue, 1.0));
                    LGB.GradientStops = GSC;

                    PointAnimation animPoint2 = new PointAnimation();
                    animPoint2.To               = _expPoint2;
                    animPoint2.BeginTime        = TimeSpan.FromMilliseconds(0);
                    animPoint2.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                    animPoint2.CurrentStateInvalidated           += new EventHandler(OnCurrentStateInvalidated);
                    animPoint2.FillBehavior     = FillBehavior.HoldEnd;

                    TB.Background = LGB;

                    _storyboard.Children.Add(animPoint2);
                    _path = new PropertyPath("(0).(1)", TextBox.BackgroundProperty, LinearGradientBrush.EndPointProperty);
                    Storyboard.SetTargetProperty(animPoint2, _path);
                    Storyboard.SetTargetName(animPoint2, TB.Name);
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! AssignAnimation: Incorrect argument.");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            //GlobalLog.LogStatus("---OnCurrentStateInvalidated--- CurrentState: " + ((Clock)sender).CurrentState);
            
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                _begunCount += 1;
            }
            else
            {
                switch (_animType.ToLower(CultureInfo.InvariantCulture))
                {
                    case "color":
                        AnimationClock CAC = (AnimationClock)sender;
                        Color actColor = (Color)CAC.GetCurrentValue(new Color(), new Color());
                        double expR = Convert.ToDouble(_expColor.R);
                        double expG = Convert.ToDouble(_expColor.G);
                        double expB = Convert.ToDouble(_expColor.B);

                        double actR = Convert.ToDouble(actColor.R);
                        double actG = Convert.ToDouble(actColor.G);
                        double actB = Convert.ToDouble(actColor.B);

                        bool bR = (actR == expR);
                        bool bG = (actG == expG);
                        bool bB = (actB == expB);

                        if (!bR || !bG || !bB)
                        {
                            _pass3 = false;
                        }
                        break;
                    case "double":
                        AnimationClock DAC = (AnimationClock)sender;
                        if ((double)DAC.GetCurrentValue(0d, 0d) != _expDouble)
                        {
                            _pass3 = false;
                        }
                        break;
                    case "double2":
                        AnimationClock DAC2 = (AnimationClock)sender;
                        if ((double)DAC2.GetCurrentValue(0d, 0d) != _expDouble2)
                        {
                            _pass3 = false;
                        }
                        break;
                    case "rect":
                        AnimationClock RAC = (AnimationClock)sender;
                        if ((Rect)RAC.GetCurrentValue(new Rect(0,0,0,0), new Rect(0,0,0,0)) != _expRect)
                        {
                            _pass3 = false;
                        }
                        break;
                    case "gradientorigin":
                        AnimationClock PAC1 = (AnimationClock)sender;
                        if ((Point)PAC1.GetCurrentValue(new Point(0,0), new Point(0,0)) != _expPoint1)
                        {
                            _pass3 = false;
                        }
                        break;
                    case "point":
                        AnimationClock PAC2 = (AnimationClock)sender;
                        if ((Point)PAC2.GetCurrentValue(new Point(0,0), new Point(0,0)) != _expPoint2)
                        {
                            _pass3 = false;
                        }
                        break;
                    default:
                        break;
                }

                _endedCount += 1;
            }
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidatedStory
           ******************************************************************************/
        private void OnCurrentStateInvalidatedStory(object sender, EventArgs e)
        {
            //GlobalLog.LogStatus("---OnCurrentStateInvalidatedStory--- CurrentState: " + ((Clock)sender).CurrentState);

            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                GlobalLog.LogStatus("---OnBegunClock---");
            }
            else
            {
                GlobalLog.LogStatus("---OnEndedClock---");

                bool pass1 = (_endedCount == _animTotal);
                bool pass2 = (_begunCount == _animTotal);

                GlobalLog.LogEvidence("begunCount----Actual: " + _begunCount + "  --Expected: " + _animTotal);
                GlobalLog.LogEvidence("endedCount----Actual: " + _endedCount + "  --Expected: " + _animTotal);
                GlobalLog.LogEvidence("GetCurrentValue correct-" + _pass3);

                _testPassed = (pass1 && pass2 && _pass3);

                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult object, indicating Pass or Fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
