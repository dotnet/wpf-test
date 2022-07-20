// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Input;

namespace DRT
{
    //
    // Gestures still require full trust
    //
    [TestedSecurityLevelAttribute(SecurityLevel.Unrestricted)]
    public class GestureTest : DrtInkTestcase
    {

        private GestureRecognizer _gestureRecognizer;
        /// <summary>
        /// The method to run the test cases.
        /// </summary>
        public override void Run()
        {

            _gestureRecognizer = new GestureRecognizer();
            if (_gestureRecognizer.IsRecognizerAvailable)
            {
                TestSettingInvalidApplicationGestures();
                TestSettingValidApplicationGestures();
                TestRecognizeRightGesture(true);
                TestRecognizeRightGesture(false);
                TestRecognizeScratchoutGesture(true);
                TestRecognizeScratchoutGesture(false);
                TestRecognizeChevronDownGesture(true);
                TestRecognizeChevronDownGesture(false);
                TestMultiStrokeGesture();
                TestExceptions();
                TestDisposed();
            }

            // done
            Success = true;
        }


        private void TestSettingInvalidApplicationGestures()
        {
            bool caughtCorrectException = false;
            try
            {
                ApplicationGesture[] invalidGestures =
                    new ApplicationGesture[] { ApplicationGesture.AllGestures, ApplicationGesture.Right };
                _gestureRecognizer.SetEnabledGestures(invalidGestures);
            }
            catch (ArgumentException)
            {
                caughtCorrectException = true;
            }
            if (!caughtCorrectException)
            {
                throw new InvalidOperationException("Failed to detect an invalid ApplicaitonGesture[]");
            }
        }

        private void TestSettingValidApplicationGestures()
        {

            List<ApplicationGesture> validGestures1 =
                new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.Right, ApplicationGesture.Left });

            List<ApplicationGesture> validGestures2 =
                new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.AllGestures });

            //
            // validate setting and getting
            //
            _gestureRecognizer.SetEnabledGestures(validGestures1);
            List<ApplicationGesture> retrievedGestures = new List<ApplicationGesture>(_gestureRecognizer.GetEnabledGestures());
            CompareApplicationGestureArrays(retrievedGestures, validGestures1);

            _gestureRecognizer.SetEnabledGestures(validGestures2);
            retrievedGestures = new List<ApplicationGesture>(_gestureRecognizer.GetEnabledGestures());
            CompareApplicationGestureArrays(retrievedGestures, validGestures2);
        }

        private void CompareApplicationGestureArrays(List<ApplicationGesture> one, List<ApplicationGesture> two)
        {
            if (one.Count != two.Count)
            {
                throw new InvalidOperationException("Error comparing List<ApplicationGesture> counts");
            }
            for (int x = 0; x < one.Count; x++)
            {
                if (one[x] != two[x])
                {
                    throw new InvalidOperationException("Error comparing List<ApplicationGesture>'s");
                }
            }
        }

        private void TestRecognizeRightGesture(bool gestureEnabled)
        {
            List<ApplicationGesture> enabledGestures;
            if (gestureEnabled)
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.AllGestures });
            }
            else
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.Left });
            }

            //
            // validate setting and getting
            //
            _gestureRecognizer.SetEnabledGestures(enabledGestures);
            List<ApplicationGesture> retrievedGestures = new List<ApplicationGesture>(_gestureRecognizer.GetEnabledGestures());
            CompareApplicationGestureArrays(retrievedGestures, enabledGestures);
            StrokeCollection strokes = new StrokeCollection();
            StylusPoint[] points = new StylusPoint[]{   
                                            new StylusPoint(393f, 203.723333333333f),
                                            new StylusPoint(482f, 214.723333333333f),
                                            new StylusPoint(535f, 213.723333333333f),
                                            new StylusPoint(539f, 212.723333333333f),
                                            new StylusPoint(542f, 211.723333333333f),
                                            new StylusPoint(542f, 210.723333333333f)  
                                        };

            

            Stroke s = new Stroke(new StylusPointCollection(points));
            strokes.Add(s);
            //
            // perform recognition
            //
            ReadOnlyCollection<GestureRecognitionResult> results = _gestureRecognizer.Recognize(strokes);
            if (gestureEnabled)
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.Right)
                {
                    throw new InvalidOperationException("Failed to recognize a Right gesture");
                }
            }
            else
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.NoGesture)
                {
                    throw new InvalidOperationException("Incorrectly recognized a gesture!");
                }
            }

        }

        private void TestRecognizeScratchoutGesture(bool gestureEnabled)
        {
            List<ApplicationGesture> enabledGestures;
            if (gestureEnabled)
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.AllGestures });
            }
            else
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.Left });
            }

            //
            // validate setting and getting
            //
            _gestureRecognizer.SetEnabledGestures(enabledGestures);
            List<ApplicationGesture> retrievedGestures = new List<ApplicationGesture>(_gestureRecognizer.GetEnabledGestures());
            CompareApplicationGestureArrays(retrievedGestures, enabledGestures);
            StrokeCollection strokes = new StrokeCollection();
            StylusPoint[] points = new StylusPoint[]{   
                                            new StylusPoint(88f, 104.723333333333f),
                                            new StylusPoint(91f, 106.723333333333f),
                                            new StylusPoint(95f, 107.723333333333f),
                                            new StylusPoint(102f, 109.723333333333f),
                                            new StylusPoint(126f, 111.723333333333f),
                                            new StylusPoint(142f, 111.723333333333f),
                                            new StylusPoint(162f, 112.723333333333f),
                                            new StylusPoint(184f, 112.723333333333f),
                                            new StylusPoint(201f, 112.723333333333f),
                                            new StylusPoint(214f, 112.723333333333f),
                                            new StylusPoint(226f, 112.723333333333f),
                                            new StylusPoint(237f, 110.723333333333f),
                                            new StylusPoint(245f, 110.723333333333f),
                                            new StylusPoint(250f, 110.723333333333f),
                                            new StylusPoint(253f, 109.723333333333f),
                                            new StylusPoint(251f, 109.723333333333f),
                                            new StylusPoint(245f, 108.723333333333f),
                                            new StylusPoint(226f, 108.723333333333f),
                                            new StylusPoint(214f, 108.723333333333f),
                                            new StylusPoint(200f, 108.723333333333f),
                                            new StylusPoint(163f, 104.723333333333f),
                                            new StylusPoint(140f, 102.723333333333f),
                                            new StylusPoint(115f, 101.723333333333f),
                                            new StylusPoint(81f, 98.7233333333333f),
                                            new StylusPoint(71f, 98.7233333333333f),
                                            new StylusPoint(68f, 98.7233333333333f),
                                            new StylusPoint(72f, 98.7233333333333f),
                                            new StylusPoint(78f, 98.7233333333333f),
                                            new StylusPoint(92f, 98.7233333333333f),
                                            new StylusPoint(106f, 98.7233333333333f),
                                            new StylusPoint(125f, 99.7233333333333f),
                                            new StylusPoint(150f, 100.723333333333f),
                                            new StylusPoint(175f, 102.723333333333f),
                                            new StylusPoint(203f, 103.723333333333f),
                                            new StylusPoint(227f, 103.723333333333f),
                                            new StylusPoint(245f, 104.723333333333f),
                                            new StylusPoint(255f, 104.723333333333f),
                                            new StylusPoint(261f, 104.723333333333f),
                                            new StylusPoint(258f, 104.723333333333f),
                                            new StylusPoint(250f, 104.723333333333f),
                                            new StylusPoint(237f, 102.723333333333f),
                                            new StylusPoint(222f, 101.723333333333f),
                                            new StylusPoint(204f, 100.723333333333f),
                                            new StylusPoint(179f, 100.723333333333f),
                                            new StylusPoint(151f, 98.7233333333333f),
                                            new StylusPoint(121f, 96.7233333333333f),
                                            new StylusPoint(71f, 96.7233333333333f),
                                            new StylusPoint(59f, 97.7233333333333f),
                                            new StylusPoint(54f, 98.7233333333333f),
                                            new StylusPoint(62f, 99.7233333333333f),
                                            new StylusPoint(75f, 99.7233333333333f),
                                            new StylusPoint(94f, 97.7233333333333f),
                                            new StylusPoint(122f, 96.7233333333333f),
                                            new StylusPoint(149f, 94.7233333333333f),
                                            new StylusPoint(175f, 93.7233333333333f),
                                            new StylusPoint(197f, 91.7233333333333f),
                                            new StylusPoint(214f, 90.7233333333333f),
                                            new StylusPoint(224f, 90.7233333333333f),
                                            new StylusPoint(227f, 89.7233333333333f),
                                            new StylusPoint(224f, 89.7233333333333f),
                                            new StylusPoint(218f, 88.7233333333333f),
                                            new StylusPoint(209f, 88.7233333333333f),
                                            new StylusPoint(197f, 86.7233333333333f),
                                            new StylusPoint(185f, 86.7233333333333f),
                                            new StylusPoint(172f, 87.7233333333333f),
                                            new StylusPoint(162f, 87.7233333333333f),
                                            new StylusPoint(153f, 88.7233333333333f),
                                            new StylusPoint(146f, 88.7233333333333f),
                                            new StylusPoint(142f, 88.7233333333333f)
                                        };

            

            Stroke s = new Stroke(new StylusPointCollection(points));
            strokes.Add(s);
            //
            // perform recognition
            //
            ReadOnlyCollection<GestureRecognitionResult> results = _gestureRecognizer.Recognize(strokes);
            if (gestureEnabled)
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.ScratchOut)
                {
                    throw new InvalidOperationException("Failed to recognize a ScratchOut gesture");
                }
            }
            else
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.NoGesture)
                {
                    throw new InvalidOperationException("Incorrectly recognized a gesture!");
                }
            }
        }

        private void TestRecognizeChevronDownGesture(bool gestureEnabled)
        {
             List<ApplicationGesture> enabledGestures;
            if (gestureEnabled)
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.AllGestures });
            }
            else
            {
                enabledGestures = new List<ApplicationGesture>( new ApplicationGesture[] { ApplicationGesture.Left });
            }

            //
            // validate setting and getting
            //
            _gestureRecognizer.SetEnabledGestures(enabledGestures);
            List<ApplicationGesture> retrievedGestures = new List<ApplicationGesture>(_gestureRecognizer.GetEnabledGestures());
            CompareApplicationGestureArrays(retrievedGestures, enabledGestures);
            StrokeCollection strokes = new StrokeCollection();
            StylusPoint[] points = new StylusPoint[]{   
                                            new StylusPoint(214f, 180.723333333333f),
                                            new StylusPoint(214f, 182.723333333333f),
                                            new StylusPoint(217f, 190.723333333333f),
                                            new StylusPoint(219f, 195.723333333333f),
                                            new StylusPoint(221f, 200.723333333333f),
                                            new StylusPoint(226f, 212.723333333333f),
                                            new StylusPoint(227f, 216.723333333333f),
                                            new StylusPoint(229f, 221.723333333333f),
                                            new StylusPoint(231f, 224.723333333333f),
                                            new StylusPoint(233f, 230.723333333333f),
                                            new StylusPoint(235f, 237.723333333333f),
                                            new StylusPoint(238f, 243.723333333333f),
                                            new StylusPoint(241f, 250.723333333333f),
                                            new StylusPoint(244f, 256.723333333333f),
                                            new StylusPoint(246f, 260.723333333333f),
                                            new StylusPoint(249f, 264.723333333333f),
                                            new StylusPoint(250f, 266.723333333333f),
                                            new StylusPoint(252f, 264.723333333333f),
                                            new StylusPoint(254f, 256.723333333333f),
                                            new StylusPoint(257f, 249.723333333333f),
                                            new StylusPoint(259f, 241.723333333333f),
                                            new StylusPoint(261f, 234.723333333333f),
                                            new StylusPoint(263f, 227.723333333333f),
                                            new StylusPoint(265f, 218.723333333333f),
                                            new StylusPoint(266f, 210.723333333333f),
                                            new StylusPoint(269f, 203.723333333333f),
                                            new StylusPoint(271f, 197.723333333333f),
                                            new StylusPoint(272f, 194.723333333333f),
                                            new StylusPoint(273f, 192.723333333333f),
                                            new StylusPoint(274f, 191.723333333333f)
                                        };

            

            Stroke s = new Stroke(new StylusPointCollection(points));
            strokes.Add(s);
            //
            // perform recognition
            //
            ReadOnlyCollection<GestureRecognitionResult> results = _gestureRecognizer.Recognize(strokes);
            if (gestureEnabled)
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.ChevronDown)
                {
                    throw new InvalidOperationException("Failed to recognize a ChevronDown gesture");
                }
            }
            else
            {
                if (results.Count == 0 ||
                    results[0].ApplicationGesture != ApplicationGesture.NoGesture)
                {
                    throw new InvalidOperationException("Incorrectly recognized a gesture!");
                }
            }
        }

        private void TestMultiStrokeGesture()
        {
            StrokeCollection gestureStrokes = DrtHelpers.LoadInk("multiStrokeGesture.isf");
            StrokeCollection test = new StrokeCollection();
            test.Add(gestureStrokes[0]);
            test.Add(gestureStrokes[1]);
            ReadOnlyCollection<GestureRecognitionResult> results =
                _gestureRecognizer.Recognize(test);


            if (results.Count == 0 ||
                results[0].ApplicationGesture != ApplicationGesture.NoGesture)
            {
                throw new InvalidOperationException("Incorrectly thought we found a gesture");
            }
        }

        private void TestExceptions()
        {
            int exceptionCount = 0;
            try
            {
                _gestureRecognizer.Recognize(null);
            }
            catch (ArgumentNullException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.SetEnabledGestures(null);
            }
            catch (ArgumentNullException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.SetEnabledGestures(new ApplicationGesture[]{});
            }
            catch (ArgumentException)
            {
                exceptionCount++;
            }

            try
            {
                ApplicationGesture bogusGesture = (ApplicationGesture)77777;
                _gestureRecognizer.SetEnabledGestures(new ApplicationGesture[] {bogusGesture});
            }
            catch (ArgumentException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.SetEnabledGestures(new ApplicationGesture[] { ApplicationGesture.AllGestures, ApplicationGesture.ChevronDown });
            }
            catch (ArgumentException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.SetEnabledGestures(new ApplicationGesture[] { ApplicationGesture.ChevronDown, ApplicationGesture.ChevronDown });
            }
            catch (ArgumentException)
            {
                exceptionCount++;
            }

            if (exceptionCount != 6)
            {
                throw new InvalidOperationException("GestureRecognizer failed to throw correct exceptions");
            }
        }
        private void TestDisposed()
        {
            int exceptionCount = 0;
            _gestureRecognizer.Dispose();
            try
            {
                _gestureRecognizer.SetEnabledGestures(new ApplicationGesture[] { ApplicationGesture.AllGestures });
            }
            catch (ObjectDisposedException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.GetEnabledGestures();
            }
            catch (ObjectDisposedException)
            {
                exceptionCount++;
            }

            try
            {
                bool b = _gestureRecognizer.IsRecognizerAvailable;
            }
            catch (ObjectDisposedException)
            {
                exceptionCount++;
            }

            try
            {
                _gestureRecognizer.Recognize(new StrokeCollection());
            }
            catch (ObjectDisposedException)
            {
                exceptionCount++;
            }

            if (exceptionCount != 4)
            {
                throw new InvalidOperationException("GestureRecognizer failed to throw after being disposed");
            }
        }

    }
}
