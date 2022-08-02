// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Runtime.InteropServices;

namespace DRT
{
    public class InkCollectionPerfTest : InkCanvasPerfTestBase
    {
        private StrokeCollection _strokes;
        #region Constructor

        public InkCollectionPerfTest()
            : base("InkCollectionPerfTest")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            _inkCanvas = new InkCanvas();
            _strokes = DRT.LoadStrokeCollection(DRT.BaseDirectory + "inkcollection.isf");

            DRT.Show(_inkCanvas);

            return new DrtTest[] { 
                    new DrtTest(StartLog),
                    new DrtTest(DrawStrokes),
                    new DrtTest(EndLog),
                };

        }

        #endregion Override Methods

        #region Tests

        private void DrawStrokes()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            int count = Math.Min(100, _strokes.Count);
            for ( int i = 0; i < count; i++ )
            {
                Stroke stroke = _strokes[i];
                StylusPointCollection stylusPoints = stroke.StylusPoints;
                
                Point pt = stylusPoints[0].ToPoint();
                _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
                _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));

                int pointsCount = stylusPoints.Count;
                for ( int j = 1; j < pointsCount; j++ )
                {
                    pt = stylusPoints[j].ToPoint();
                    _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
                }

                _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));
            }

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        #endregion Tests

    }
}
