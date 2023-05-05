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
using System.Windows.Documents;
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
    /// <area>Animation.HighLevelScenarios.Regressions</area>

    // [DISABLE WHILE PORTING]
    //[Test(2, "Animation.HighLevelScenarios.Regressions", "ClearAnimatedElementTest")]
    public class ClearAnimatedElementTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private string              _inputString         = "";
        private Canvas              _body                = null;
        private FlowDocumentReader  _flowDocReader1      = null;
        private Color               _bodyBackground      = Colors.CornflowerBlue;
        private Color               _rectBackground      = Colors.HotPink;
        private string              _windowTitle         = "Animation";
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor

        [Variation("Clear")]
        [Variation("ClearValue")]

        /******************************************************************************
        * Function:          ClearAnimatedElementTest Constructor
        ******************************************************************************/
        public ClearAnimatedElementTest(string testValue)
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
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 600d;
            Window.Height       = 400d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.Title        = _windowTitle; 
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            _body = new Canvas();
            _body.Background = new SolidColorBrush(_bodyBackground);

            _flowDocReader1 = new FlowDocumentReader();
            _flowDocReader1.Height        = 300d;
            _flowDocReader1.Width         = 400d;
            _flowDocReader1.Background    = Brushes.CornflowerBlue;
            _flowDocReader1.MinZoom       = 50;
            _flowDocReader1.MaxZoom       = 1000;
            _flowDocReader1.ZoomIncrement = 100d;
            _flowDocReader1.Zoom          = 100d;
            Canvas.SetTop  (_flowDocReader1, 0d);
            Canvas.SetLeft (_flowDocReader1, 0d);

            _body.Children.Add(_flowDocReader1);

            FlowDocument flowDocument1 = new FlowDocument();
            flowDocument1.Background    = Brushes.DarkViolet;

            _flowDocReader1.Document      = flowDocument1;

            Rectangle rectangle1 = new Rectangle();
            rectangle1.Height       = 100d;
            rectangle1.Width        = 100d;
            rectangle1.Fill         = new SolidColorBrush(_rectBackground);
            
            BlockUIContainer block1 = new BlockUIContainer(rectangle1);
            Figure figure1 = new Figure(block1);

            Paragraph paragraph1 = new Paragraph();
            paragraph1.Inlines.Add(figure1);
            flowDocument1.Blocks.Add(paragraph1);
            
            Window.Content = _body;

            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            StartAnimation();

            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = TimeSpan.FromMilliseconds(1500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
        }

        /******************************************************************************
        * Function:          StartAnimation
        ******************************************************************************/
        /// <summary>
        /// Create and begin the Animation.
        /// </summary>
        /// <returns></returns>
        private void StartAnimation()
        {
            DoubleAnimation animZoom = new DoubleAnimation();                                             
            animZoom.BeginTime         = TimeSpan.FromMilliseconds(0);
            animZoom.Duration          = new Duration(TimeSpan.FromMilliseconds(4000));
            animZoom.To                = 200d;
            
            _flowDocReader1.BeginAnimation(FlowDocumentReader.ZoomProperty, animZoom);
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
            
            if (_tickCount == 1)
            {
                if (_inputString == "Clear")
                {
                    _body.Children.Clear();
                }
                else if (_inputString == "ClearValue")
                {
                    _flowDocReader1.ClearValue(FlowDocumentReader.ZoomProperty);
                }
            }
            else if (_tickCount == 2)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();

                bool testPassed = CheckColor();
               
                if (testPassed)
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
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor()
        {
            bool passed         = true;
            int x1              = 200;
            int y1              = 200;
            
            //At each Tick, check the color at the specified point.
            Color actualColor = _verifier.getColorAtPoint(x1, y1);
            Color expectedColor;

            if (_inputString == "Clear")
            {
                expectedColor = _bodyBackground;
            }
            else
            {
                expectedColor = _rectBackground;
            }
            
            float tolerance  = 0.05f;

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
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }

        #endregion
    }
}
