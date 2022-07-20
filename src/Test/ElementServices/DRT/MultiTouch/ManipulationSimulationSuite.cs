// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace DRT
{
    public class ManipulationSimulationSuite : DrtSuite<DrtMultiTouch>
    {
        public ManipulationSimulationSuite()
            : base("ManipulationSimulation")
        {
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            TestWindow = Drt.OpenTestWindow();

            if (!Drt.KeepAlive)
            {
                // Automated testing
                return new DrtTest[]
                {
                    new DrtTest(Start),
                    new DrtTest(ReportTranslate1Finger),
                    new DrtTest(ValidateTranslate1Finger),
                    //new DrtTest(ReportTranslate2Finger),
                    //new DrtTest(ValidateTranslate2Finger),
                    //new DrtTest(ReportRotate2Finger),
                    //new DrtTest(ValidateRotate2Finger),
                    //new DrtTest(ReportScale2Finger),
                    //new DrtTest(ValidateScale2Finger),
                    new DrtTest(Finish),
                };
            }
            else
            {
                // Manual testing
                return new DrtTest[] { };
            }
        }

        public override void ReleaseResources()
        {
            Drt.CloseTestWindow(TestWindow);
            TestWindow = null;
        }

        protected TestWindow TestWindow
        {
            get;
            private set;
        }

        private void Start()
        {
            // Prepare for tests
            _playback = new FramePlayback(DRT);
            _translate1Finger = FrameSequence.Load("Translate1Finger.xml");
            _translate2Finger = FrameSequence.Load("Translate2Finger.xml");
            _rotate2Finger = FrameSequence.Load("Rotate2Finger.xml");
            _scale2Finger = FrameSequence.Load("Scale2Finger.xml");

            TestWindow.TouchDown += OnTouchDown;
            TestWindow.TouchMove += OnTouchMove;
            TestWindow.TouchUp += OnTouchUp;
            TestWindow.ManipulationStarting += OnManipulationStarting;
            TestWindow.ManipulationStarted += OnManipulationStarted;
            TestWindow.ManipulationDelta += OnManipulationDelta;
            TestWindow.ManipulationInertiaStarting += OnManipulationInertiaStarting;
            TestWindow.ManipulationCompleted += OnManipulationCompleted;

            GeneralTransform transform = TestWindow.RectCanvas.TransformToAncestor(TestWindow);
            _origin = transform.Transform(new Point());
        }

        private void Finish()
        {
            // Cleanup
            TestWindow.TouchDown -= OnTouchDown;
            TestWindow.TouchMove -= OnTouchMove;
            TestWindow.TouchUp -= OnTouchUp;
            TestWindow.ManipulationStarting -= OnManipulationStarting;
            TestWindow.ManipulationStarted -= OnManipulationStarted;
            TestWindow.ManipulationDelta -= OnManipulationDelta;
            TestWindow.ManipulationInertiaStarting -= OnManipulationInertiaStarting;
            TestWindow.ManipulationCompleted -= OnManipulationCompleted;
        }

        private void PositionRect(ManipulationBorder element, Point position)
        {
            Matrix matrix = new Matrix();
            matrix.Translate(position.X, position.Y);
            element.SetMatrix(matrix);

        }

        /// <summary>
        ///     We need to drain some of the events after validation.
        ///     Otherwise, we might get some additional undesired manipulation if
        ///     an element moves while the touch device is still captured to it.
        /// </summary>
        private void AfterTest()
        {
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            Drt.WaitForCompleteRender();
            Drt.Pause(100);
        }

        private Point MakeGlobalPoint(double x, double y)
        {
            return new Point(_origin.X + x, _origin.Y + y);
        }

        private Point _origin;

        private void ReportTranslate1Finger()
        {
            PositionRect(TestWindow.Rect1, new Point(200, 200));
            _playback.Sequence = _translate1Finger;
            _playback.InitialOffset = MakeGlobalPoint(250, 250);
            _playback.Report();
        }

        private void ValidateTranslate1Finger()
        {
            ValidateFinalTransform(TestWindow.Rect1, new Matrix(1, 0, 0, 1, 215, 215));
            AfterTest();
        }

        private void ReportTranslate2Finger()
        {
            PositionRect(TestWindow.Rect1, new Point(200, 200));
            _playback.Sequence = _translate2Finger;
            _playback.InitialOffset = MakeGlobalPoint(210, 210);
            _playback.Report();
        }

        private void ValidateTranslate2Finger()
        {
            ValidateFinalTransform(TestWindow.Rect1, new Matrix(1, 0, 0, 1, 215, 215));
            AfterTest();
        }

        private void ReportRotate2Finger()
        {
            PositionRect(TestWindow.Rect1, new Point(200, 200));
            _playback.Sequence = _rotate2Finger;
            _playback.InitialOffset = MakeGlobalPoint(210, 210);
            _playback.Report();
        }

        private void ValidateRotate2Finger()
        {
            ValidateFinalTransform(TestWindow.Rect1, new Matrix(0.85467701998206, 0.146885065466157, -0.146885065466157, 0.85467701998206, 214.610402274205, 207.421895727589));
            AfterTest();
        }

        private void ReportScale2Finger()
        {
            PositionRect(TestWindow.Rect1, new Point(200, 200));
            _playback.Sequence = _scale2Finger;
            _playback.InitialOffset = MakeGlobalPoint(210, 210);
            _playback.Report();
        }

        private void ValidateScale2Finger()
        {
            ValidateFinalTransform(TestWindow.Rect1, new Matrix(1.56250021758876, 1.07774369634581E-16, -1.07774369634581E-16, 1.56250021758876, 171.874989120562, 171.874989120562));
            AfterTest();
        }

        private void ValidateFinalTransform(ManipulationBorder element, Matrix matrix)
        {
            MatrixTransform transform = element.CurrentMatrixTransform;
            Drt.Assert(transform != null, "Manipulation did not affect element.");

            Matrix actual = transform.Matrix;
            bool areClose = AreClose(actual.M11, matrix.M11) &&
                AreClose(actual.M12, matrix.M12) &&
                AreClose(actual.M21, matrix.M21) &&
                AreClose(actual.M22, matrix.M22) &&
                AreClose(actual.OffsetX, matrix.OffsetX) &&
                AreClose(actual.OffsetY, matrix.OffsetY);
            Drt.Assert(areClose, string.Format("Manipulation matrixes do not match. \n Expected Matrix: ({0}) \n Actual Matrix: ({1})", matrix, actual));
        }

        private void ValidatePosition(TouchEventArgs e)
        {
            Frame frame = _playback.CurrentFrame;
            FrameInput input = frame.GetFrameInput(e.TouchDevice.Id);

            Drt.Assert(input != null, String.Format("Received input from device {0} during frame {1}, which does not contain that device.", e.TouchDevice.Id, _playback.CurrentFrameIndex));

            TouchPoint point = e.GetTouchPoint(null);

            double expectedX = (input.X + _playback.InitialOffset.X);
            double expectedY = (input.Y + _playback.InitialOffset.Y);
            Drt.Assert(point.Position.X == expectedX, String.Format("Device {0} X={1} does not match Frame {2} X={3}.", e.TouchDevice.Id, point.Position.X, _playback.CurrentFrameIndex, expectedX));
            Drt.Assert(point.Position.Y == expectedY, String.Format("Device {0} Y={1} does not match Frame {2} Y={3}.", e.TouchDevice.Id, point.Position.Y, _playback.CurrentFrameIndex, expectedY));
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            ValidatePosition(e);
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            ValidatePosition(e);
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
        }

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
        }

        private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }


        private static bool AreClose(double value1, double value2)
        {
            // in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (2 * (|value1| + |value2|) + 10.0)) < DBL_EPSILON
            double eps = (2 * (Math.Abs(value1) + Math.Abs(value2)) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        private const double DBL_EPSILON = 0.005;

        private FramePlayback _playback;
        private FrameSequence _translate1Finger;
        private FrameSequence _translate2Finger;
        private FrameSequence _rotate2Finger;
        private FrameSequence _scale2Finger;
    }
}
