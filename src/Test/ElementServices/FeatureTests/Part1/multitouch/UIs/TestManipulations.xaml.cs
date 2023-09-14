// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for TestManipulations.xaml
    ///     
    /// This class encapsulates the app logic for related Manipulation tests, most of the existing functionalities 
    /// are good for ad-hocing, so we keep them here for now. Once we have the verifier and the input simulation 
    /// hooked up, this will likely be changed.  
    /// 
    /// </summary>
    public partial class TestManipulations : Window
    {
        #region Fields

        private const int MaxWatchEvents = 30;
        private string _title = "Multi-Touch Manipulation Testing";
        private DispatcherTimer _timer;

        private MatrixTransform _nonRestrictedMatrix;

        public bool startingEventFired = false; 
        public bool startEventFired = false;
        public bool deltaEventFired = false;
        public bool completedEventFired = false;
        public bool inertiaStartingEventFired = false;
        public bool isInertial = false;
        public int startEventNumber = 0;
        public bool stop;
        public int tick;
        public bool hasPromo = false;

        public bool startingEventFiredWin = false;
        public bool startedEventFiredWin = false;         
        public bool deltaEventFiredWin = false;
        public bool completedEventFiredWin = false;
        public bool inertiaStartingEventFiredWin = false;
        public bool boundaryFeedbackEventFiredWin = false;
        public bool systemGeatureEventFiredWin = false; 

        #endregion

        #region Constructor 

        public TestManipulations(string wintitle)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(wintitle))
            {
                this.Title = _title;
            }
            else
            {
                this.Title = wintitle;
            }
        }

        #endregion 

        #region Public Methods

        /// <summary>
        /// on reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnReset(object sender, RoutedEventArgs e)
        {
            ResetAll(); 
        }

        /// <summary>
        /// reset all
        /// </summary>
        public void ResetAll()
        {
            // reset the transform for all
            ResetTransform();

            // reset all indicators
            ResetIndicators();
        }

        /// <summary>
        /// reset the render transform for all
        /// </summary>
        public void ResetTransform()
        {
            ManipulationRect.RenderTransform = null;
            TranslateRect.RenderTransform = null;
            RotateRect.RenderTransform = null;
            OrangeRect.RenderTransform = null;
            VioletRect.RenderTransform = null;
            BlueRect.RenderTransform = null;
            SalmonRect.RenderTransform = null;
        }

        /// <summary>
        /// reset the event fired indicators
        /// </summary>
        public void ResetIndicators()
        {
            startingEventFiredWin = false;
            startedEventFiredWin = false;
            deltaEventFiredWin = false;
            completedEventFiredWin = false;
            inertiaStartingEventFiredWin = false;
            boundaryFeedbackEventFiredWin = false;
            systemGeatureEventFiredWin = false; 

            startingEventFired = false;
            startEventFired = false;
            deltaEventFired = false;
            completedEventFired = false;
            inertiaStartingEventFired = false;

            isInertial = false;
            hasPromo = false;
        }

        #region top win's Events

        /// <summary>
        /// Stylus system gesture event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            systemGeatureEventFiredWin = true; 
            AddWatchEvent(String.Format("Gesture ({0}) - {1}", e.StylusDevice.TabletDevice.Type, e.SystemGesture));
        }

        private void OnStylusDownWin(object sender, StylusDownEventArgs e)
        {
            AddWatchEvent("StylusDown-Win");
        }

        private void OnStylusUpWin(object sender, StylusEventArgs e)
        {
            AddWatchEvent("StylusUp-Win");
        }

        private void OnTouchDownWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchDown-Win");
        }

        private void OnTouchMoveWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchMove-Win");
        }

        private void OnTouchUpWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchUp-Win");
        }

        private void OnMouseDownWin(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("MouseDown-Win");
        }

        private void OnMouseMoveWin(object sender, MouseEventArgs e)
        {
            AddWatchEvent("MouseMove-Win");
        }

        private void OnMouseUpWin(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("MouseUp-Win");
        }

        /// <summary>
        /// top win's starting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationStartingWin(object sender, ManipulationStartingEventArgs e)
        {
            AddWatchEvent("Win-Starting");
            e.ManipulationContainer = this;
            e.Mode = ManipulationModes.All;
            startingEventFiredWin = false;
        }

        public void OnManipulationBoundaryFeedbackWin(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            boundaryFeedbackEventFiredWin = true; 
            AddWatchEvent(String.Format("Win-Boundary ({0})", e.BoundaryFeedback));
        }

        #endregion


        #region border's events

        /// <summary>
        /// borders' starting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            AddWatchEvent(string.Format("Border-Starting: {0}", e.OriginalSource));
            e.ManipulationContainer = GetParent(sender);            

            // reset indicators
            ResetIndicators();
        }

        /// <summary>
        /// borders' starting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationStarting2(object sender, ManipulationStartingEventArgs e)
        {
            AddWatchEvent(string.Format("Border-Starting: {0}", e.OriginalSource));
            e.ManipulationContainer = GetParent(sender);
            e.Mode = ManipulationModes.All;
        }

        /// <summary>
        /// border started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            AddWatchEvent("Border-Started");
            _nonRestrictedMatrix = new MatrixTransform(EnsureMatrixTransform((UIElement)sender).Matrix);
            UpdateCenterPoint(e.ManipulationOrigin, (UIElement)sender);
        }

        /// <summary>
        /// border delta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var origin = e.ManipulationOrigin;
            var manipulation = e.DeltaManipulation;

            Vector pastEndVector;
            if (UpdateManipulationRectNew((UIElement)sender, origin, manipulation.Translation, manipulation.Rotation, manipulation.Scale, out pastEndVector))
            {
                e.ReportBoundaryFeedback(new ManipulationDelta(pastEndVector, 0.0, new Vector(1.0, 1.0), new Vector()));
            }

            if (e.CumulativeManipulation.Translation.X > 100d && !e.IsInertial)
            {
                //e.Complete();
                e.StartInertia();
            }
        }

        /// <summary>
        /// inertia starting 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            AddWatchEvent("Border-InertiaStarting");
            //todo: e.TranslationBehavior.DesiredDisplacement = 50d;
        }

        #endregion


        #region canvas' events

        public void OnManipulationStartingCanvas(object sender, ManipulationStartingEventArgs e)
        {
            AddWatchEvent("Canvas-Starting");
            startingEventFired = true; 

            e.ManipulationContainer = this;
            e.Mode = ManipulationModes.All;
        }

        /// <summary>
        /// canvas started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationStartedCanvas(object sender, ManipulationStartedEventArgs e)
        {
            AddWatchEvent("Canvas-Started");
            
            //set indicators
            startEventFired = true;
            startEventNumber++;

            //e.Handled = true; 
        }

        /// <summary>
        /// canvas delta 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationDeltaCanvas(object sender, ManipulationDeltaEventArgs e)
        {
            // indicator
            deltaEventFired = true;
            isInertial = e.IsInertial;

            var manipulation = e.CumulativeManipulation;
            AddWatchEvent(String.Format("Canvas-Delta {2}({0:F2},{1:F2})", manipulation.Translation.X, manipulation.Translation.Y, isInertial ? "- Inertia " : String.Empty));

            // start inertia if needed
            if (stop && isInertial)
            {
                stop = false;
                e.StartInertia();
            }
        }

        /// <summary>
        /// canvas inertia starting 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationInertiaStartingCanvas(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // write the trace and set the indicator
            AddWatchEvent("Canvas-InertiaStarting");

            inertiaStartingEventFired = true;
        }

        /// <summary>
        /// canvas completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnManipulationCompletedCanvas(object sender, ManipulationCompletedEventArgs e)
        {
            // set indicators
            completedEventFired = true;
            stop = true; 

            var manipulation = e.TotalManipulation; 
            AddWatchEvent(String.Format("Canvas-Completed ({0:F2},{1:F2})", manipulation.Translation.X, manipulation.Translation.Y));
        }

        #endregion 


        /// <summary>
        /// Initiate manual complete
        /// </summary>
        public void InitiateComplete(int tick)
        {
            this.tick = tick;
            StartTimer();
        }

        #endregion 

        #region Private methods

        /// <summary>
        /// ensure matrixtransform for a given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private MatrixTransform EnsureMatrixTransform(UIElement element)
        {
            var group = element.RenderTransform as TransformGroup;
            if (group == null)
            {
                group = new TransformGroup();
                var matrixTransform = new MatrixTransform();
                group.Children.Add(matrixTransform);
                element.RenderTransform = group;
                return matrixTransform;
            }
            else
            {
                return (MatrixTransform)group.Children[0];
            }
        }        

        /// <summary>
        /// Do the heavy lifting updates here
        /// </summary>
        /// <param name="element"></param>
        /// <param name="origin"></param>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="pastEndVector"></param>
        /// <returns></returns>
        private bool UpdateManipulationRectNew(UIElement element, 
            Point origin, Vector translation, double rotation, Vector scale, out Vector pastEndVector)
        {
            var matrixTransform = EnsureMatrixTransform(element);
            var newMatrix = matrixTransform.Matrix;
            var nonRestrictedMatrix = _nonRestrictedMatrix.Matrix;

            newMatrix.Translate(translation.X, translation.Y);
            nonRestrictedMatrix.Translate(translation.X, translation.Y);

            //var originalCenterPt = new Point(ActualWidth * 0.5, ActualWidth * 0.5);
            //var centerPt = newMatrix.Transform(originalCenterPt);
            newMatrix.RotateAt(rotation, origin.X, origin.Y);
            nonRestrictedMatrix.RotateAt(rotation, origin.X, origin.Y);

            //centerPt = newMatrix.Transform(originalCenterPt);
            newMatrix.ScaleAt(scale.X, scale.Y, origin.X, origin.Y);
            nonRestrictedMatrix.ScaleAt(scale.X, scale.Y, origin.X, origin.Y);

            _nonRestrictedMatrix.Matrix = nonRestrictedMatrix;

            pastEndVector = new Vector();

            bool pastEnd = false;
            double offsetX = 0.0;
            double offsetY = 0.0;
            UIElement parent = GetParent(this);
            if (parent != null)
            {
                Rect bounds = matrixTransform.TransformBounds(new Rect(new Point(), RenderSize));
                if (bounds.Left < 0.0)
                {
                    newMatrix.OffsetX -= bounds.Left;
                }
                else if (bounds.Right > parent.RenderSize.Width)
                {
                    newMatrix.OffsetX -= bounds.Right - parent.RenderSize.Width;
                }
                if (bounds.Top < 0.0)
                {
                    newMatrix.OffsetY -= bounds.Top;
                }
                else if (bounds.Bottom > parent.RenderSize.Height)
                {
                    newMatrix.OffsetY -= bounds.Bottom - parent.RenderSize.Height;
                }

                bounds = _nonRestrictedMatrix.TransformBounds(new Rect(new Point(), RenderSize));
                if (bounds.Left < 0.0)
                {
                    pastEnd = true;
                    offsetX = bounds.Left;
                }
                else if (bounds.Right > parent.RenderSize.Width)
                {
                    pastEnd = true;
                    offsetX = bounds.Right - parent.RenderSize.Width;
                }
                if (bounds.Top < 0.0)
                {
                    pastEnd = true;
                    offsetY = bounds.Top;
                }
                else if (bounds.Bottom > parent.RenderSize.Height)
                {
                    pastEnd = true;
                    offsetY = bounds.Bottom - parent.RenderSize.Height;
                }

                if (pastEnd)
                {
                    pastEndVector = new Vector(offsetX, offsetY);
                }
            }

            matrixTransform.Matrix = newMatrix;
            UpdateCenterPoint(origin, element);

            return pastEnd;
        }

        /// <summary>
        /// update the CenterPoint's margin based on a given point
        /// </summary>
        /// <param name="start"></param>
        private void UpdateCenterPoint(Point origin, UIElement element)
        {
            if (CenterPoint != null)
            {
                UIElement parent = GetParent(element);
                if (parent != null)
                {
                    GeneralTransform transform = parent.TransformToDescendant(element);
                    if (transform != null)
                    {
                        origin = transform.Transform(origin);
                    }
                }

                CenterPoint.Margin = new Thickness(origin.X - 4.0, origin.Y - 4.0, 0.0, 0.0);
            }
        }

        /// <summary>
        /// update the translateRect accordingly
        /// </summary>
        /// <param name="translation"></param>
        private void UpdateTranslateRect(Vector translation)
        {
            var newMatrix = new Matrix();
            newMatrix.Translate(translation.X, translation.Y);
            var matrixTransform = EnsureMatrixTransform(TranslateRect);
            matrixTransform.Matrix = newMatrix;
        }

        /// <summary>
        /// update the rotate rect accordingly
        /// </summary>
        /// <param name="rotation"></param>
        private void UpdateRotateRect(double rotation)
        {
            var newMatrix = new Matrix();
            newMatrix.RotateAt(rotation, 25.0, 25.0);
            var matrixTransform = EnsureMatrixTransform(RotateRect);
            matrixTransform.Matrix = newMatrix;
        }
        
        /// <summary>
        /// write the trace
        /// </summary>
        /// <param name="s"></param>
        private void AddWatchEvent(string s)
        {
            if (WatchList.Items.Count >= MaxWatchEvents)
            {
                WatchList.Items.RemoveAt(0);
            }

            WatchList.Items.Add(s);
        }

        /// <summary>
        /// find the visual parent
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static UIElement GetParent(object element)
        {
            return (UIElement)VisualTreeHelper.GetParent((UIElement)element);
        }

        /// <summary>
        /// start the local timer
        /// </summary>
        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += OnTimerTick;
            }

            _timer.Start();
        }

        /// <summary>
        /// stop the timer
        /// </summary>
        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// timer tick 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            tick--;

            if (tick == 0)
            {
                StopTimer();

                Manipulation.CompleteManipulation(ManipulationRect);
                Manipulation.CompleteManipulation(TranslateRect);
                Manipulation.CompleteManipulation(RotateRect);
                Manipulation.CompleteManipulation(OrangeRect);
                Manipulation.CompleteManipulation(VioletRect);
                Manipulation.CompleteManipulation(BlueRect);
                Manipulation.CompleteManipulation(SalmonRect);
                
                this.stop = true;
            }
        }

        /// <summary>
        /// Manual completed on UI thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCompleteClicked(object sender, RoutedEventArgs e)
        {
            tick = 10;
            UpdateCompleteButton();
            StartTimer();
        }

        /// <summary>
        /// update the button content 
        /// </summary>
        private void UpdateCompleteButton()
        {
            if (tick > 0)
            {
                CompleteButton.Content = String.Format("Complete ({0})", tick);
            }
            else
            {
                CompleteButton.Content = "Complete";
            }
        }

        private void LuckyButton_Click(object sender, RoutedEventArgs e)
        {
            LuckyButton.Content = "You clicked me!";
        }

        #endregion

        #region Commanding and Mouse promo

        private void OnOpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            AddWatchEvent(string.Format("OnOpenCommand executed from {0}", ((Button)e.OriginalSource).Content));
            hasPromo = true; 
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            AddWatchEvent(string.Format("OnClick executed from {0}", ((Button)sender).Content));
            hasPromo = true; 
        }

        private void OnButtonTouchDown(object sender, TouchEventArgs e)
        {
            Button button = (Button)sender;

            AddWatchEvent(string.Format("OnTouchDown executed from {0}", ((Button)sender).Content));
            e.TouchDevice.Capture((IInputElement)sender);
            e.Handled = true;
        }

        private void OnButtonTouchUp(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("OnTouchUp executed from {0}", ((Button)sender).Content));
            e.TouchDevice.Capture(null);
            e.Handled = true;
        }

        private void OnDisableManipulations(object sender, ManipulationStartingEventArgs e)
        {
            AddWatchEvent(string.Format("ManipulationStarting: " + e.Manipulators.Count()));

            e.Mode = ManipulationModes.None;
            e.Handled = true;
        }

        #endregion 
    }
}
