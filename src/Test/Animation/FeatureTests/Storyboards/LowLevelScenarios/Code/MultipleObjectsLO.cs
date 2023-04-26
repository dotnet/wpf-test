// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Scalability Test *****************
*     Description:
*          Tests that Animation works with a large number of objects inside a Grid.
*     Pass Conditions:
*          The test case will Pass if GetCurrentValue returns the correct value for each Animation.
*     How verified:
*          The result of the comparisons between actual and expected values is passed to
*          TestResult.
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
    /// Scaling Test: Verify that a large number of ComboBox animations work
    /// </description>
    /// </summary>
    [Test(3, "Storyboards.LowLevelScenarios.Scaling", "MultipleObjectsLOTest")]
    public class MultipleObjectsLOTest : WindowTest
    {
        #region Test case members

        private string                          _animType           = null;
        private Int32                           _animTotal          = 0;
        private int                             _expectedCount      = 0;
        private Grid                            _body;
        private Storyboard                      _storyboard;
        private BeginStoryboard                 _beginStory;
        private PropertyPath                    _path;
        private int                             _begunCount          = 0;
        private int                             _endedCount          = 0;
        private int                             _COMBO_WIDTH         = 80;
        private int                             _COMBO_HEIGHT        = 30;
        private int                             _ANIM_DURATION       = 5000;
        private int                             _gridSize            = 0;
        private int                             _rowCount            = 0;
        private int                             _colCount            = 0;
        private int                             _top                 = 0;
        private int                             _left                = 0;
        private bool                            _pass3               = true;
        private double                          _expDoubleOpacity    = 0.1d;
        private double                          _expDoubleRotation   = 90d;
        private double                          _expDoubleSkew       = 25d;
        private double                          _expDoubleWidth      = 50d;
        private double                          _expDoubleOffset     = 0.90d;
        private double                          _doubleTolerance     = .0001;
        private Thickness                       _expThickness        = new Thickness(14, 14, 14, 14);
        private Color                           _expColor            = Colors.DarkViolet;
        private Point                           _expPoint            = new Point(310,320);
        private Rect                            _expRect             = new Rect(50,50,70,25);
        private bool                            _testPassed          = false;
        
        #endregion


        #region Constructor

        [Variation("Color", "2000")]
        [Variation("DoubleOffset", "4000")]
        [Variation("DoubleOpacity", "2000")]
        [Variation("DoubleRotation", "2000")]
        [Variation("DoubleSkew", "4000")]
        [Variation("DoubleWidth", "2000")]
        [Variation("Point", "2000")]
        [Variation("Rect", "2000")]
        [Variation("Thickness", "4000")]
        
        /******************************************************************************
        * Function:          MultipleObjectsLOTest Constructor
        ******************************************************************************/
        public MultipleObjectsLOTest(string testValue1, string testValue2)
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
            string[]    expList     = { "Color", "DoubleOffset", "DoubleOpacity", "DoubleRotation", "DoubleSkew", "DoubleWidth", "Point", "Rect", "Thickness" };

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

            NameScope.SetNameScope(Window, new NameScope());

            //------------------- DETERMINE SIZE OF GRID---------------------------
            _gridSize = (int)Math.Sqrt(_animTotal);

            _body = new Grid();
            _body.Background             = Brushes.DarkViolet;
            _body.ShowGridLines          = true;

            for (int i=0; i<_gridSize; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                colDef.Width = new System.Windows.GridLength(1, GridUnitType.Auto);
                _body.ColumnDefinitions.Add(colDef);

                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new System.Windows.GridLength(1, GridUnitType.Auto);
                _body.RowDefinitions.Add(rowDef);
            }

            //------------------- CREATE STORYBOARD-----------------------------
            GlobalLog.LogStatus("----Create Storyboard----");
            _storyboard = new Storyboard();
            _storyboard.Name             = "story";
            _storyboard.BeginTime        = TimeSpan.FromMilliseconds(0);
            _storyboard.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION + 2000));
            _storyboard.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidatedStory);

            //------------------- CREATE COMBOBOXES AND ANIMATIONS ----------------------
            //Create ComboBoxes and assign Animations to them.
            GlobalLog.LogStatus("----Creating animated ComboBoxes----");
            for (int i=0; i<_animTotal; i++)
            {
                 CreateComboBox(i, _animType);
            }
            GlobalLog.LogStatus("---All ComboBoxes Created---");

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

            _beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");
            _beginStory.Storyboard   = _storyboard;
            _beginStory.Storyboard.Begin(Window);
        }

        /******************************************************************************
        * Function:          CreateComboBox
        ******************************************************************************/
        /// <summary>
        /// CreateComboBox: Create the number of ComboBoxes requested and animate them.
        /// </summary>
        private void CreateComboBox(int j, string animType)
        {
            ComboBox combobox  = new ComboBox();
            combobox.Name                = "CB" + j.ToString();
            combobox.Width               = _COMBO_WIDTH;
            combobox.Height              = _COMBO_HEIGHT;
            combobox.Background          = Brushes.Lavender;
            Canvas.SetTop  (combobox, (double)_top);
            Canvas.SetLeft (combobox, (double)_left);

            ComboBoxItem CBI = new ComboBoxItem();
            CBI.Content = "Combo#" + j;
            combobox.Items.Add(CBI);
            CBI = new ComboBoxItem();
            CBI.Content = "Item1";
            combobox.Items.Add(CBI);
            CBI = new ComboBoxItem();
            CBI.Content = "Item2";
            combobox.Items.Add(CBI);
            combobox.SelectedIndex = 0;

            //Set the row and column positions of the ComboBox in the Grid.
            if ((j % _gridSize) == 0)
            {
                if (j > 0)
                {
                    _rowCount++;
                }
                _colCount = 0;
            }
            else
            {
                _colCount++;
            }
            Grid.SetRow(combobox, _rowCount);
            Grid.SetColumn(combobox, _colCount);

            Window.RegisterName(combobox.Name, combobox);

            _body.Children.Add(combobox);

            //Only animate every other ComboBox.
            if ((j % 2) == 0)
            {
                AssignAnimation(j, animType, ref combobox);
                _expectedCount++;  //Used only for verification.
            }
        }

        /******************************************************************************
        * Function:          AssignAnimation
        ******************************************************************************/
        /// <summary>
        /// AssignAnimation: Create and begin an Animation for each ComboBox requested.
        /// </summary>
          private void AssignAnimation(int animCount, string animationType, ref ComboBox combobox)
          {
               switch (animationType.ToLower(CultureInfo.InvariantCulture))
               {
                    case "color":
                         //SCENARIO 1: Color Animation
                         ColorAnimation animColor = new ColorAnimation();
                         animColor.From             = Colors.Lavender;
                         animColor.To               = _expColor;
                         animColor.BeginTime        = TimeSpan.FromMilliseconds(0);
                         animColor.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animColor.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);
                         animColor.FillBehavior     = FillBehavior.HoldEnd;

                         SolidColorBrush SCB1 = new SolidColorBrush();
                         SCB1.Color = Colors.Lavender;
                         combobox.Background = SCB1;

                         _storyboard.Children.Add(animColor);
                         _path = new PropertyPath("(0).(1)", ComboBox.BackgroundProperty, SolidColorBrush.ColorProperty);
                         Storyboard.SetTargetProperty(animColor, _path);
                         Storyboard.SetTargetName(animColor, combobox.Name);
                         break;
                    case "doubleopacity":
                         //SCENARIO 2: Double Animation --- OpacityAnimations
                         DoubleAnimation animDoubleOpacity = new DoubleAnimation();
                         animDoubleOpacity.From             = 1;
                         animDoubleOpacity.To               = _expDoubleOpacity;
                         animDoubleOpacity.BeginTime        = TimeSpan.FromMilliseconds(0);
                         animDoubleOpacity.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animDoubleOpacity.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);
                         animDoubleOpacity.FillBehavior     = FillBehavior.HoldEnd;

                         SolidColorBrush SCB2 = new SolidColorBrush();
                         SCB2.Color = Colors.Lavender;
                         combobox.Background = SCB2;

                         _storyboard.Children.Add(animDoubleOpacity);
                         _path = new PropertyPath("(0).(1)", ComboBox.BackgroundProperty, SolidColorBrush.OpacityProperty);
                         Storyboard.SetTargetProperty(animDoubleOpacity, _path);
                         Storyboard.SetTargetName(animDoubleOpacity, combobox.Name);
                         break;
                    case "doublewidth":
                         //SCENARIO 3: Double Animation (Width Property)
                         DoubleAnimation animDouble2 = new DoubleAnimation();
                         animDouble2.By                = _expDoubleWidth;
                         animDouble2.BeginTime         = TimeSpan.FromMilliseconds(0);
                         animDouble2.Duration          = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animDouble2.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);
                         animDouble2.FillBehavior      = FillBehavior.HoldEnd;

                         _storyboard.Children.Add(animDouble2);
                         _path = new PropertyPath("(0)", ComboBox.WidthProperty);
                         Storyboard.SetTargetProperty(animDouble2, _path);
                         Storyboard.SetTargetName(animDouble2, combobox.Name);
                         break;
                    case "rect":
                         //SCENARIO 4: Rect Animation
                         RectAnimation animRect = new RectAnimation();
                         animRect.To              = _expRect;
                         animRect.BeginTime       = TimeSpan.FromMilliseconds(0);
                         animRect.Duration        = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animRect.FillBehavior    = FillBehavior.HoldEnd;
                         animRect.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);

                         RectangleGeometry RG = new RectangleGeometry();
                         RG.Rect = new Rect(_left,_top,_COMBO_WIDTH-10,_COMBO_HEIGHT-5);

                         combobox.Clip = RG;

                         _storyboard.Children.Add(animRect);
                         _path = new PropertyPath("(0).(1)", ComboBox.ClipProperty, RectangleGeometry.RectProperty);
                         Storyboard.SetTargetProperty(animRect, _path);
                         Storyboard.SetTargetName(animRect, combobox.Name);
                         break;
                    case "point":
                         //SCENARIO 5: Point Animation
                         PointAnimation animPoint = new PointAnimation();
                         animPoint.To             = _expPoint;
                         animPoint.BeginTime      = TimeSpan.FromMilliseconds(0);
                         animPoint.Duration       = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animPoint.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);
                         animPoint.FillBehavior   = FillBehavior.HoldEnd;

                         LinearGradientBrush LGB = new LinearGradientBrush();
                         LGB.StartPoint = new Point(0.0, 0.0);
                         LGB.EndPoint = new Point(0.0, 1.0);
                         LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                         GradientStopCollection GSC = new GradientStopCollection();
                         GSC.Add(new GradientStop(Colors.Blue, 0.0));
                         GSC.Add(new GradientStop(Colors.LightBlue, 1.0));
                         LGB.GradientStops = GSC;

                         combobox.Background = LGB;

                         _storyboard.Children.Add(animPoint);
                         _path = new PropertyPath("(0).(1)", ComboBox.BackgroundProperty, LinearGradientBrush.EndPointProperty);
                         Storyboard.SetTargetProperty(animPoint, _path);
                         Storyboard.SetTargetName(animPoint, combobox.Name);
                         break;
                    case "doublerotation":
                         //SCENARIO 6: Double Animation --- RotateTransform.AngleProperty
                         int x = _left + (_COMBO_WIDTH  / 2);
                         int y = _top  + (_COMBO_HEIGHT / 2);

                         DoubleAnimation animDoubleRotation = new DoubleAnimation();
                         animDoubleRotation.FillBehavior     = FillBehavior.HoldEnd;
                         animDoubleRotation.BeginTime        = TimeSpan.FromMilliseconds(0);
                         animDoubleRotation.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animDoubleRotation.From             = 0d;
                         animDoubleRotation.To               = _expDoubleRotation;
                         animDoubleRotation.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

                         RotateTransform rotateTransform2 = new RotateTransform();
                         rotateTransform2.Angle   = 0.0f;
                         rotateTransform2.CenterX  = x;
                         rotateTransform2.CenterY  = y;

                         combobox.LayoutTransform = rotateTransform2;

                         _storyboard.Children.Add(animDoubleRotation);
                         _path     = new PropertyPath("(0).(1)", ComboBox.LayoutTransformProperty, RotateTransform.AngleProperty);
                         Storyboard.SetTargetProperty(animDoubleRotation, _path);
                         Storyboard.SetTargetName(animDoubleRotation, combobox.Name);
                         break;
                    case "doubleskew":
                         //SCENARIO 7: Double Animation --- SkewTransform.AngleYProperty
                         DoubleAnimation animDoubleSkew = new DoubleAnimation();
                         animDoubleSkew.FillBehavior        = FillBehavior.HoldEnd;
                         animDoubleSkew.BeginTime           = TimeSpan.FromMilliseconds(0);
                         animDoubleSkew.Duration            = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animDoubleSkew.From                = 0;
                         animDoubleSkew.To                  = _expDoubleSkew;
                         animDoubleSkew.CurrentStateInvalidated  += new EventHandler(OnCurrentStateInvalidated);

                         SkewTransform skewTransform = new SkewTransform();
                         skewTransform.AngleY   = 0.0f;

                         combobox.LayoutTransform = skewTransform;

                         _storyboard.Children.Add(animDoubleSkew);
                         _path = new PropertyPath("(0).(1)", ComboBox.LayoutTransformProperty, SkewTransform.AngleYProperty);
                         Storyboard.SetTargetProperty(animDoubleSkew, _path);
                         Storyboard.SetTargetName(animDoubleSkew, combobox.Name);
                         break;
                    case "thickness":
                         //SCENARIO 8: Thickness Animation --- ComboBox.MarginProperty
                         //First, must set base value for the ComboBox.
                         combobox.Margin                  = new Thickness(3, 3, 3, 3);

                         ThicknessAnimation animThickness = new ThicknessAnimation();
                         animThickness.FillBehavior    = FillBehavior.HoldEnd;
                         animThickness.BeginTime       = TimeSpan.FromMilliseconds(0);
                         animThickness.Duration        = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                         animThickness.By              = new Thickness(11, 11, 11, 11);
                         animThickness.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

                         _storyboard.Children.Add(animThickness);
                         _path = new PropertyPath("(0)", ComboBox.MarginProperty);
                         Storyboard.SetTargetProperty(animThickness, _path);
                         Storyboard.SetTargetName(animThickness, combobox.Name);
                         break;

                    case "doubleoffset":
                        //SCENARIO 9: Double Animation --- GradientStop.OffsetProperty

                        DoubleAnimation animOffset = new DoubleAnimation();
                        animOffset.FillBehavior       = FillBehavior.HoldEnd;
                        animOffset.BeginTime          = TimeSpan.FromMilliseconds(0);
                        animOffset.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION));
                        animOffset.From               = 0.2d;
                        animOffset.To                 = _expDoubleOffset;
                        animOffset.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

                        LGB = new LinearGradientBrush();
                        LGB.StartPoint = new Point(0.0, 0.0);
                        LGB.EndPoint   = new Point(1.0, 1.0);
                        LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;

                        GradientStop GS1 = new GradientStop(Colors.RoyalBlue, 0.0);
                        GradientStop GS2 = new GradientStop(Colors.SteelBlue, 0.2);
                        GradientStop GS3 = new GradientStop(Colors.LightBlue, 1.0);
                        GSC = new GradientStopCollection();
                        GSC.Add(GS1);
                        GSC.Add(GS2);
                        GSC.Add(GS3);

                        LGB.GradientStops = GSC;
                        combobox.Background = LGB;

                        _storyboard.Children.Add(animOffset);
                        _path = new PropertyPath("(0).(1)[1].(2)", ComboBox.BackgroundProperty,LinearGradientBrush.GradientStopsProperty, GradientStop.OffsetProperty);
                        Storyboard.SetTargetProperty(animOffset, _path);
                        Storyboard.SetTargetName(animOffset, combobox.Name);
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
               double baseValue = 0d;

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
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + actColor + "  --Expected: " + expColor);
                         break;
                    case "doubleopacity":
                         AnimationClock DAC = (AnimationClock)sender;
                         double actDoubleOpacity = (double)DAC.GetCurrentValue(baseValue, baseValue);
                         if (Math.Abs(actDoubleOpacity - _expDoubleOpacity) > _doubleTolerance)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (double)DAC.GetCurrentValue(baseValue) + "  --Expected: " + expDoubleOpacity);
                         break;
                    case "doublewidth":
                         AnimationClock DAC2 = (AnimationClock)sender;
                         if ((double)DAC2.GetCurrentValue(baseValue, baseValue) != _expDoubleWidth)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (double)DAC2.GetCurrentValue(baseValue) + "  --Expected: " + expDoubleWidth);
                         break;
                    case "rect":
                         AnimationClock RAC = (AnimationClock)sender;
                         if ((Rect)RAC.GetCurrentValue(new Rect(0,0,0,0), new Rect(0,0,0,0)) != _expRect)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (Rect)RAC.GetCurrentValue(new Rect(0,0,0,0)) + "  --Expected: " + expRect);
                         break;
                     case "point":
                         AnimationClock PAC2 = (AnimationClock)sender;
                         if ((Point)PAC2.GetCurrentValue(new Point(0,0), new Point(0,0)) != _expPoint)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (Point)PAC2.GetCurrentValue(new Point(0,0)) + "  --Expected: " + expPoint);
                         break;
                    case "doublerotation":
                         AnimationClock DAC3 = (AnimationClock)sender;
                         if ((double)DAC3.GetCurrentValue(baseValue, baseValue) != _expDoubleRotation)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (double)DAC3.GetCurrentValue(baseValue) + "  --Expected: " + expDoubleRotation);
                         break;
                    case "doubleskew":
                         AnimationClock DAC4 = (AnimationClock)sender;
                         if ((double)DAC4.GetCurrentValue(baseValue, baseValue) != _expDoubleSkew)
                         {
                             _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (double)DAC4.GetCurrentValue(baseValue) + "  --Expected: " + expDoubleSkew);
                         break;
                    case "thickness":
                         AnimationClock TAC = (AnimationClock)sender;
                         if ((Thickness)TAC.GetCurrentValue(new Thickness(3, 3, 3, 3), new Thickness(3, 3, 3, 3)) != _expThickness)
                         {
                             _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + (Thickness)TAC.GetCurrentValue(new Thickness(3, 3, 3, 3)) + "  --Expected: " + expThickness);
                        break;
                    case "doubleoffset":
                         AnimationClock DAC5 = (AnimationClock)sender;
                         double actDoubleOffset = (double)DAC5.GetCurrentValue(baseValue, baseValue);
                         if (Math.Abs(actDoubleOffset - _expDoubleOffset) > _doubleTolerance)
                         {
                              _pass3 = false;
                         }
                         //GlobalLog.LogStatus("GetCurrentValue----Actual: " + actDoubleOffset + "  --Expected: " + expDoubleOffset);
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
            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                GlobalLog.LogStatus("---StoryboardClock Begun---");
            }
            else
            {
                GlobalLog.LogStatus("---StoryboardClock Ended---");
                bool pass1 = (_endedCount == _expectedCount);
                bool pass2 = (_begunCount == _expectedCount);

                GlobalLog.LogEvidence("begunCount----Actual: " + _begunCount + "  --Expected: " + _expectedCount);
                GlobalLog.LogEvidence("endedCount----Actual: " + _endedCount + "  --Expected: " + _expectedCount);

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
            TestResult result = WaitForSignal("TestFinished", 360000);

            return result;
        }

        #endregion
    }
}
