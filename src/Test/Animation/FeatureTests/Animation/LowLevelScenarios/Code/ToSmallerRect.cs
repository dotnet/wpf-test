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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Regressions</area>


    [Test(2, "Animation.LowLevelScenarios.Regressions", "ToSmallerRectTest", SupportFiles=@"FeatureTests\Animation\avalon.png")]
    public class ToSmallerRectTest : WindowTest
    {
        #region Test case members

        private ImageDrawing                    _image1;
        private RectAnimation                   _animRect;
        private Rect                            _fromValue       = new Rect(0, 150, 25, 25);
        private Rect                            _toValue         = new Rect(0, 50, 10, 10);
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          ToSmallerRectTest Constructor
        ******************************************************************************/
        public ToSmallerRectTest()
        {
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
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
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MediumSeaGreen;
            body.Height     = 300d;
            body.Width      = 300d;

            DrawingGroup imageDrawings = new DrawingGroup();

            // Create a 100 by 100 image with an upper-left point of (75,75). 
            ImageDrawing bigImage = new ImageDrawing();
            bigImage.Rect = new Rect(75, 75, 100, 100);
            bigImage.ImageSource = new BitmapImage(new Uri(@"avalon.png", UriKind.Relative));
            
            imageDrawings.Children.Add(bigImage);
            
            // Create a 25 by 25 image with an upper-left point of (0,150). 
            _image1 = new ImageDrawing();
            _image1.Rect = _fromValue;
            _image1.ImageSource = new BitmapImage(new Uri(@"avalon.png", UriKind.Relative));
            imageDrawings.Children.Add(_image1);

            // Create a 25 by 25 image with an upper-left point of (150,0). 
            ImageDrawing image2 = new ImageDrawing();
            image2.Rect = new Rect(150, 0, 25, 25);
            image2.ImageSource = new BitmapImage(new Uri(@"avalon.png", UriKind.Relative));
            imageDrawings.Children.Add(image2);

            //
            // Use a DrawingImage and an Image control to
            // display the drawings.
            //
            DrawingImage drawingImageSource = new DrawingImage(imageDrawings);
            
            Image imageControl = new Image();
            imageControl.Stretch = Stretch.None;
            imageControl.Source = drawingImageSource;

            // Create a border to contain the Image control.
            Border imageBorder = new Border();
            imageBorder.BorderBrush = Brushes.Gray;
            imageBorder.BorderThickness = new Thickness(2);
            imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
            imageBorder.VerticalAlignment = VerticalAlignment.Top;
            imageBorder.Margin = new Thickness(20);
            imageBorder.Child = imageControl;

            body.Children.Add(imageBorder);
            imageBorder.SetValue(Canvas.LeftProperty, 50d);
            imageBorder.SetValue(Canvas.TopProperty, 50d);

            Window.Content = body;

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
            _aTimer.Interval = new TimeSpan(0,0,0,0,1500);
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
            _tickCount++;
            
            if (_tickCount == 1)
            {
                _animRect = new RectAnimation();
                _animRect.BeginTime      = TimeSpan.FromMilliseconds(0);
                _animRect.Duration       = new Duration(TimeSpan.FromMilliseconds(1000));
                _animRect.From           = _fromValue;
                _animRect.To             = _toValue;
                _image1.BeginAnimation(ImageDrawing.RectProperty, _animRect);
            }
            else
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
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

            Rect actValue = (Rect)_image1.GetValue(ImageDrawing.RectProperty);

            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value:       " + _toValue);
            GlobalLog.LogEvidence("Actual Value:         " + actValue);
            
            if (actValue == _toValue)
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
