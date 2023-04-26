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
    /// <area>Animation.HighLevelScenarios.Regressions</area>


    [Test(2, "Animation.HighLevelScenarios.Regressions", "ResetBaseValueTest")]
    public class ResetBaseValueTest : WindowTest
    {
        #region Test case members

        private string              _parm1               = "";
        private string              _parm2               = "";
        private Rectangle           _rectangle;
        private DoubleAnimation     _animation;
        private double              _originalBase        = 100d;
        private double              _newBase             =   0d;
        private double              _fromValue           = 100d;
        private double              _toValue             = 200d;
        private double              _byValue             = 200d;
        private bool                _animationCompleted  = false;
        private DispatcherTimer     _aTimer              = null;
        
        #endregion


        #region Constructor
        
        [Variation("BeginFirst", "FromTo", Priority=0)]
        [Variation("BeginLast",  "FromTo")]
        [Variation("NullFirst",  "FromTo")]
        [Variation("NullLast",   "FromTo")]
        [Variation("NullBegin",  "FromTo")]

        [Variation("BeginFirst", "FromBy")]
        [Variation("BeginLast",  "FromBy", Priority=1)]
        [Variation("NullFirst",  "FromBy")]
        [Variation("NullLast",   "FromBy")]
        [Variation("NullBegin",  "FromBy")]

        [Variation("BeginFirst", "By")]
        [Variation("BeginLast",  "By")]
        [Variation("NullFirst",  "By", Priority=1)]
        [Variation("NullLast",   "By")]
        [Variation("NullBegin",  "By")]

        [Variation("BeginFirst", "From")]
        [Variation("BeginLast",  "From")]
        [Variation("NullFirst",  "From")]
        [Variation("NullLast",   "From", Priority=1)]
        [Variation("NullBegin",  "From")]

        [Variation("BeginFirst", "To")]
        [Variation("BeginLast",  "To")]
        [Variation("NullFirst",  "To")]
        [Variation("NullLast",   "To")]
        [Variation("NullBegin",  "To", Priority=0)]

        /******************************************************************************
        * Function:          ResetBaseValueTest Constructor
        ******************************************************************************/
        public ResetBaseValueTest(string testValue1, string testValue2)
        {
            _parm1 = testValue1;
            _parm2 = testValue2;
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
            bool        arg1Found   = false;
            bool        arg2Found   = false;
            string      errMessage  = "";
            string[]    expList1     = { "BeginFirst", "BeginLast", "NullFirst", "NullLast", "NullBegin" };
            string[]    expList2     = { "FromTo", "FromBy", "By", "From", "To" };

            arg1Found = AnimationUtilities.CheckInputString(_parm1, expList1, ref errMessage);

            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(_parm2, expList2, ref errMessage);
                if (errMessage != "")
                {
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                }
            }

            if (arg1Found && arg2Found)
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
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            GlobalLog.LogStatus("---Testing: " + _parm1);

            Window.Width        = 550d;
            Window.Height       = 550d;
            Window.Left         = 0d;
            Window.Top          = 0d;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body = new Canvas();
            Window.Content = body;
            body.Background = Brushes.Cornsilk;

            Line line1 = new Line();
            body.Children.Add(line1);
            line1.X1                = 200d;
            line1.X2                = 200d;
            line1.Y1                = 0d;
            line1.Y2                = 400d;
            line1.Stroke            = Brushes.Red;
            line1.StrokeThickness   = 2d;

            Line line2 = new Line();
            body.Children.Add(line2);
            line2.X1                = 300d;
            line2.X2                = 300d;
            line2.Y1                = 0d;
            line2.Y2                = 400d;
            line2.Stroke            = Brushes.Blue;
            line2.StrokeThickness   = 2d;

            Line line3 = new Line();
            body.Children.Add(line3);
            line3.X1                = 500d;
            line3.X2                = 500d;
            line3.Y1                = 0d;
            line3.Y2                = 400d;
            line3.Stroke            = Brushes.Green;
            line3.StrokeThickness   = 2d;
            
            _rectangle = new Rectangle();
            body.Children.Add(_rectangle);
            _rectangle.Height            = 100;
            _rectangle.Fill              = Brushes.DodgerBlue;
            
            switch (_parm2)
            {
                case "FromTo":
                    _animation = new DoubleAnimation(_fromValue, _toValue, TimeSpan.FromSeconds(1.0));
                    break;
                case "FromBy":
                    _animation = new DoubleAnimation();
                    _animation.Duration  = new Duration(TimeSpan.FromSeconds(1.0));
                    _animation.From      = _fromValue;
                    _animation.By        = _byValue;
                    break;
                case "By":
                    _rectangle.Width = _originalBase;
                    _animation = new DoubleAnimation();
                    _animation.Duration  = new Duration(TimeSpan.FromSeconds(1.0));
                    _animation.By        = _byValue;
                    break;
                case "From":
                    _rectangle.Width = _toValue;
                    _animation = new DoubleAnimation();
                    _animation.Duration  = new Duration(TimeSpan.FromSeconds(1.0));
                    _animation.From      = _fromValue;  //Will mimic From/To animation.
                    break;
                case "To":
                    _rectangle.Width = _originalBase;
                    _animation = new DoubleAnimation();
                    _animation.Duration  = new Duration(TimeSpan.FromSeconds(1.0));
                    _animation.To        = _toValue;
                    break;
            }
            _animation.Completed += new EventHandler(OnCompleted);
            _rectangle.BeginAnimation(Rectangle.WidthProperty, _animation);
            
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
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
            _aTimer.Start();
            GlobalLog.LogStatus("---DispatcherTimer Started---");
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
            Signal("AnimationDone", TestResult.Pass);
        }

        /******************************************************************************
        * Function:          OnCompleted
        ******************************************************************************/
        /// <summary>
        /// Fires when the Completed event fires on the Animation.  Used to set a new
        /// base value and call BeginAnimation, but only the first time the event fires.
        /// </summary>
        /// <returns></returns>
        private void OnCompleted(object sender, EventArgs args)
        {
            if (_animationCompleted == false)
            {
                switch (_parm1)
                {
                    case "BeginFirst":
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, _animation);
                        _rectangle.Width = _newBase;
                        break;
                    case "BeginLast":
                        _rectangle.Width = _newBase;
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, _animation);
                        break;
                    case "NullFirst":
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, null);
                        _rectangle.Width = _newBase;
                        break;
                    case "NullLast":
                        _rectangle.Width = _newBase;
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, null);
                        break;
                    case "NullBegin":
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, null);
                        _rectangle.Width = _newBase;
                        _rectangle.BeginAnimation(Rectangle.WidthProperty, _animation);
                        break;
                }
                _animationCompleted = true;
            }
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
            WaitForSignal("AnimationDone");

            double actValue     = (double)_rectangle.GetValue(Rectangle.WidthProperty);
            double expValue     = 0d;

            switch (_parm1)
            {
                case "BeginFirst":
                    if (_parm2 == "FromTo" || _parm2 == "To")
                    {
                        expValue = _toValue;
                    }
                    else if (_parm2 == "FromBy")
                    {
                        expValue = _fromValue + _byValue;
                    }
                    else if (_parm2 == "From")
                    {
                        expValue = _newBase;
                    }
                    else if (_parm2 == "By")
                    {
                        //Handoff calculated using original base value.
                        expValue = _originalBase + _byValue + _byValue;
                    }
                    break;
                case "BeginLast":
                    if (_parm2 == "FromTo" || _parm2 == "To")
                    {
                        expValue = _toValue;
                    }
                    else if (_parm2 == "FromBy")
                    {
                        expValue = _fromValue + _byValue;
                    }
                    else if (_parm2 == "From")
                    {
                        expValue = _newBase;
                    }
                    else if (_parm2 == "By")
                    {
                        //Handoff calculated using new base value.
                        expValue = _newBase + _byValue + _byValue;
                    }
                    break;
                case "NullFirst":
                    expValue = _newBase;
                    break;
                case "NullLast":
                    expValue = _newBase;
                    break;
                case "NullBegin":
                    if (_parm2 == "FromTo" || _parm2 == "To")
                    {
                        expValue = _toValue;
                    }
                    else if (_parm2 == "FromBy")
                    {
                        expValue = _fromValue + _byValue;
                    }
                    else if (_parm2 == "From")
                    {
                        expValue = _newBase;
                    }
                    else if (_parm2 == "By")
                    {
                        expValue = _newBase + _byValue;
                    }
                    break;
            }
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);

            if (actValue == expValue)
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
