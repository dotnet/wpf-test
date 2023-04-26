// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  Changed Integration Test *****************
*    Description: verify that the CurrentTimeInvalidated event fires during an Animation.
*                 Some of the events are set on the Animation template, others directly on the Clock
*                 that was created.
*    Major Actions:
*          (a) Via OnStartup, create Elements, including a Canvas element,
*              including a subclassed Canvas called Canvv that will be animated.
*          (b) Create the following:
*             -1- an Animation; associate it with the Canvas.
*             -2- a separate Timeline is be used to control verification.
*          (c) This test case will not verify actual rendering, only event firing.
*    Pass Conditions:
*          The test passes if the events fire correctly.
*     How verified:
*          The result of the comparisons between actual and expected values is passed to TestResult().
*     Framework:          A CLR executable is created.
*     Area:               MIL Animation & Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:        
**********************************************************/
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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.PropertyMethodEvent.Events</area>
    /// <priority>2</priority>
    /// <description>
    /// Scenario Test: Verify Animation via the MouseWheel event
    /// </description>
    /// </summary>
    [Test(2, "Animation.PropertyMethodEvent.Events", "CurrentTimeInvalidatedTest")]
    public class CurrentTimeInvalidatedTest : WindowTest
    {
        #region Test case members

        private string                  _inputString     = "";
        private DispatcherTimer         _aTimer          = null;
        private Canvas                  _body            = null;
        private Canvv                   _CV              = null;
        private AnimationClock          _CAclock         = null;
        private AnimationClock          _DAclock         = null;
        private AnimationClock          _DA2clock        = null;
        private AnimationClock          _PAclock         = null;
        private AnimationClock          _BAclock         = null;
        private AnimationClock          _TAclock         = null;
        private AnimationClock          _I32clock        = null;

        private TimeSpan                _DISPATCHER_TICK_INTERVAL    = TimeSpan.FromSeconds(2);
        private int                     _EXPECTED_TOTAL              = 325;
        private int                     _dispatcherTickCount         = 0;
        private Decorator               _TD                          = null;
        private RotateTransform         _rotateTransform             = null;
        private LineGeometry            _lineGeometry                = null;
        private Path                    _path                        = null;
        private int                     _eventFired                  = 0;
        private string                  _windowTitle                 = "CurrentTimeInvalidated";
        private bool                    _testPassed                  = false;
        
        #endregion


        #region Constructor

        [Variation("Color")]
        [Variation("Double")]
        [Variation("Double2")]
        [Variation("Point", Priority=1)]
        // [DISABLE WHILE PORTING]
     //    [Variation("Bool")]
     //    [Variation("Int32")]
        [Variation("Thickness")]
        
        /******************************************************************************
        * Function:          CurrentTimeInvalidatedTest Constructor
        ******************************************************************************/
        public CurrentTimeInvalidatedTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateTree()
        {
            Window.Visibility          = Visibility.Visible;   
            Window.Title               = _windowTitle;
            Window.Left                = 0;
            Window.Top                 = 0;
            Window.Height              = 300;
            Window.Width               = 300;
            Window.WindowStyle         = WindowStyle.None;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = (_body);
            _body.Width               = 300;
            _body.Height              = 300;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);               
            Border borderbody        = new Border();
            borderbody.Width         = 300d;
            borderbody.Height        = 300d;
            borderbody.Background    = new SolidColorBrush(Colors.LawnGreen);
            _body.Children.Add(borderbody);

            _CV  = new Canvv();
            _body.Children.Add(_CV);
            _CV.Width          = 10d;
            _CV.Height         = 30d;
            _CV.Background     = new SolidColorBrush(Colors.Magenta);
            Canvas.SetTop  (_CV, 200d);
            Canvas.SetLeft (_CV, 20d);               

            GlobalLog.LogStatus("---- Window created ----");

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
               GlobalLog.LogStatus("---- OnContentRendered ----");

               //Start a separate Timer to invoke Timing Methods.
               _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
               _aTimer.Tick += new EventHandler(OnInvokeMethod);
               _aTimer.Interval = _DISPATCHER_TICK_INTERVAL;
               _aTimer.Start();
               GlobalLog.LogStatus("--- DispatcherTimer Started ---");
                        
               switch (_inputString.ToLower(CultureInfo.InvariantCulture))
               {
                    case "color":
                         ColorAnimation CA = new ColorAnimation();
                         CA.From     = Colors.Magenta;
                         CA.To       = Colors.Navy;
                         CA.BeginTime = null;
                         CA.Duration = new Duration(TimeSpan.FromMilliseconds(9000));
                         CA.FillBehavior = FillBehavior.HoldEnd;
                         
                         SolidColorBrush SCB = new SolidColorBrush();
                         SCB.Color = Colors.Magenta;
                         
                         _CAclock = CA.CreateClock();
                         SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _CAclock);
                         _CAclock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                         
                         _CV.Background = SCB;                         
                         break;                                   
                    
                    case "double":
                         DoubleAnimation DA = new DoubleAnimation();                                             
                         DA.FillBehavior     = FillBehavior.HoldEnd;
                         DA.BeginTime        = null;
                         DA.Duration         = new Duration(TimeSpan.FromMilliseconds(9000));
                         DA.From             = 0;
                         DA.To               = 90;
                         DA.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);

                         _rotateTransform = new RotateTransform();
                         _rotateTransform.Angle   = 0.0f;
                         _rotateTransform.CenterX  = 25;
                         _rotateTransform.CenterY  = 25;
                          
                         _DAclock = DA.CreateClock();
                         _rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, _DAclock);

                         _TD = new Decorator();
                         Canvas.SetTop(_TD, 150d);
                         Canvas.SetLeft(_TD, 150d);               
                         _TD.LayoutTransform     = _rotateTransform;
                         
                         _body.Children.Add(_TD);
                         _body.Children.Remove(_CV);
                         _TD.Child = _CV;
                         break;
                    
                    case "double2":
                         //----------------------------------------------------------------------
                         DoubleAnimation LA = new DoubleAnimation();
                         LA.From             = 10d;
                         LA.To               = 110d;
                         LA.BeginTime        = null;
                         LA.Duration         = new Duration(TimeSpan.FromMilliseconds(9000));
                         LA.FillBehavior     = FillBehavior.HoldEnd;
                         
                         _DA2clock = LA.CreateClock();
                         _CV.ApplyAnimationClock(Canvas.WidthProperty, _DA2clock);
                         _DA2clock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);

                         break;                                   
                    
                    case "point":
                         PointAnimation PA = new PointAnimation();
                         PA.From             = new Point(60,60);
                         PA.To               = new Point(120,120);
                         PA.BeginTime        = null;
                         PA.Duration         = new Duration(TimeSpan.FromMilliseconds(9000));
                         PA.FillBehavior     = FillBehavior.HoldEnd;
                         PA.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                         
                         _lineGeometry = new LineGeometry();
                         _lineGeometry.StartPoint = new Point(60,60);
                         _lineGeometry.EndPoint = new Point(200,200);

                         _path = new Path();
                         _path.Data = _lineGeometry;
                         _path.StrokeThickness = 2;
                         _path.Stroke = Brushes.Black;
                         
                         _PAclock = PA.CreateClock();
                         _lineGeometry.ApplyAnimationClock(LineGeometry.StartPointProperty, _PAclock);
                         
                         _CV.Children.Add(_path);
                         break;
                         
                    case "bool":

                         _EXPECTED_TOTAL = 180;
                         BooleanAnimationUsingKeyFrames animBool = new BooleanAnimationUsingKeyFrames();

                         BooleanKeyFrameCollection BKFC = new BooleanKeyFrameCollection();
                         BKFC.Add(new DiscreteBooleanKeyFrame(false,KeyTime.FromPercent(0f)));
                         BKFC.Add(new DiscreteBooleanKeyFrame(true,KeyTime.FromPercent(0.5f)));
                         BKFC.Add(new DiscreteBooleanKeyFrame(false, KeyTime.FromPercent(1.0f)));
                         animBool.KeyFrames  = BKFC;
                         animBool.BeginTime  = null;
                         animBool.Duration   = new Duration(TimeSpan.FromMilliseconds(9000));
                         animBool.FillBehavior = FillBehavior.HoldEnd;

                         _BAclock = animBool.CreateClock();
                         _CV.ApplyAnimationClock(Canvas.FocusableProperty, _BAclock);
                         _BAclock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                         break;
                         
                    case "int32":

                         _EXPECTED_TOTAL = 180;
                         Int32Animation animInt32 = new Int32Animation();
                         animInt32.From                  = 5;
                         animInt32.To                    = 0;
                         animInt32.BeginTime      = null;
                         animInt32.Duration       = new Duration(TimeSpan.FromMilliseconds(9000));
                         animInt32.FillBehavior   = FillBehavior.HoldEnd;
                         animInt32.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                         
                         _I32clock = animInt32.CreateClock();
                         _CV.ApplyAnimationClock(Control.TabIndexProperty, _I32clock);
                         break;  
                         
                    case "thickness":
                         ThicknessAnimation animThickness = new ThicknessAnimation();

                         animThickness.By              = new Thickness(9, 9, 9, 9);
                         animThickness.BeginTime       = null;
                         animThickness.Duration        = new Duration(TimeSpan.FromMilliseconds(9000));
                         animThickness.FillBehavior    = FillBehavior.HoldEnd;

                         _CV.Margin = new Thickness(1, 1, 1, 1);
                         
                         _TAclock = animThickness.CreateClock();
                         _CV.ApplyAnimationClock(Canvas.MarginProperty, _TAclock);
                         _TAclock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
                         break;
     
                    default:
                         GlobalLog.LogEvidence("ERROR!!! OnContentRendered: Incorrect argument.");
                         Signal("TestFinished", TestResult.Fail);
                         break;
               }
          }

          /******************************************************************************
               * Function:          OnInvokeMethod
               ******************************************************************************/
          /// <summary>
          /// OnInvokeMethod: Called every 3 seconds by the Timeline TL2, in order to space out the
          /// timing methods invoked on the AnimationClocks.  The 'dispatcherTickCount' is used to
          /// control the sequence of methods to be called: Begin, Pause, Resume, Seek, and Stop.
          /// It is incremented by 1 every time this routine is called.
          /// </summary>
          private void OnInvokeMethod(object sender, EventArgs e)
          {
               GlobalLog.LogStatus("---- InvokeMethod---- " + _dispatcherTickCount.ToString());
               
               _dispatcherTickCount++;
               
               switch(_dispatcherTickCount)
               {
                    case 1:
                         switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                         {
                              case "color":
                                   _CAclock.Controller.Begin();
                                   break;
                              case "double":
                                   _DAclock.Controller.Begin();
                                   break;
                              case "double2":
                                   _DA2clock.Controller.Begin();
                                   break;
                              case "point":
                                   _PAclock.Controller.Begin();
                                   break;
                              case "bool":
                                   _BAclock.Controller.Begin();
                                   break;
                              case "thickness":
                                   _TAclock.Controller.Begin();
                                   break;
                              case "int32":
                                   _I32clock.Controller.Begin();
                                   break;
                              default:
                                   GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected failure to match argument -- 1.");
                                   Signal("TestFinished", TestResult.Fail);
                                   break;
                         }
                         break;
                    case 2:
                         switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                         {
                              case "color":
                                   _CAclock.Controller.Pause();
                                   break;
                              case "double":
                                   _DAclock.Controller.Pause();
                                   break;
                              case "double2":
                                   _DA2clock.Controller.Pause();
                                   break;
                              case "point":
                                   _PAclock.Controller.Pause();
                                   break;
                              case "bool":
                                   _BAclock.Controller.Pause();
                                   break;
                              case "int32":
                                   _I32clock.Controller.Pause();
                                   break;
                              case "thickness":
                                   _TAclock.Controller.Pause();
                                   break;
                              default:
                                   GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected failure to match argument -- 2.");
                                   Signal("TestFinished", TestResult.Fail);
                                   break;
                         }
                         break;
                    case 3:
                         switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                         {
                              case "color":
                                   _CAclock.Controller.Resume();
                                   break;
                              case "double":
                                   _DAclock.Controller.Resume();
                                   break;
                              case "double2":
                                   _DA2clock.Controller.Resume();
                                   break;
                              case "point":
                                   _PAclock.Controller.Resume();
                                   break;
                              case "bool":
                                   _BAclock.Controller.Resume();
                                   break;
                              case "int32":
                                   _I32clock.Controller.Resume();
                                   break;
                              case "thickness":
                                   _TAclock.Controller.Resume();
                                   break;
                              default:
                                   GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected failure to match argument -- 3.");
                                   Signal("TestFinished", TestResult.Fail);
                                   break;
                         }
                         break;
                    case 4:
                         switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                         {
                              case "color":
                                   _CAclock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "double":
                                   _DAclock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "double2":
                                   _DA2clock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "point":
                                   _PAclock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "bool":
                                   _BAclock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "int32":
                                   _I32clock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              case "thickness":
                                   _TAclock.Controller.Seek(TimeSpan.FromMilliseconds(2000),TimeSeekOrigin.BeginTime);
                                   break;
                              default:
                                   GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected failure to match argument -- 4.");
                                   Signal("TestFinished", TestResult.Fail);
                                   break;
                         }
                         break;
                    case 5:
                         switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                         {
                              case "color":
                                   _CAclock.Controller.Stop();
                                   break;
                              case "double":
                                   _DAclock.Controller.Stop();
                                   break;
                              case "double2":
                                   _DA2clock.Controller.Stop();
                                   break;
                              case "point":
                                   _PAclock.Controller.Stop();
                                   break;
                              case "bool":
                                   _BAclock.Controller.Stop();
                                   break;
                              case "int32":
                                   _I32clock.Controller.Stop();
                                   break;
                              case "thickness":
                                   _TAclock.Controller.Stop();
                                   break;
                              default:
                                   GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected failure to match argument -- 5.");
                                   Signal("TestFinished", TestResult.Fail);
                                   break;
                         }
                         break;
                    case 6:
                         _aTimer.Stop();  //Stop ticking and verify.
                         CheckResults();
                         break;
                    default:
                         GlobalLog.LogEvidence("ERROR!!! OnInvokeMethod: Unexpected value in switch statement.");
                         Signal("TestFinished", TestResult.Fail);
                         break;                    
               }
          }

        /******************************************************************************
           * Function:          OnCurrentTimeInvalidated
           ******************************************************************************/
        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            //GlobalLog.LogStatus("---- OnCurrentTimeInvalidated----");
            _eventFired++;
        }

        /******************************************************************************
           * Function:          CheckResults
           ******************************************************************************/
        public void CheckResults()
        {
            GlobalLog.LogEvidence("----------FINAL RESULTS----------");
            GlobalLog.LogEvidence("----------Expected: > " + _EXPECTED_TOTAL);
            GlobalLog.LogEvidence("----------Actual:     " + _eventFired.ToString());

            _testPassed =  (_eventFired > _EXPECTED_TOTAL);

            if (_testPassed)
            {
                Signal("TestFinished", TestResult.Pass);
            }
            else
            {
                Signal("TestFinished", TestResult.Fail);
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
     
     public class Canvv : Canvas
     {
          public Canvv() : base()
          {
          }
     }     
}
