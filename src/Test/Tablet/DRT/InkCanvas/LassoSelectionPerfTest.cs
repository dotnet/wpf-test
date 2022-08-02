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
    public class LassoSelectionPerfTest : InkCanvasPerfTestBase
    {
        #region Constructor

        public LassoSelectionPerfTest()
            : base("LassoSelectionPerfTest")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            _inkCanvas = new InkCanvas();
            _inkCanvas.Strokes = DRT.LoadStrokeCollection(DRT.BaseDirectory + "largepage.isf");
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;

            DRT.Show(_inkCanvas);

            return new DrtTest[] { 
                    new DrtTest(StartLog),
                    new DrtTest(LassoSelect),
                    new DrtTest(EndLog),
                };

        }

        #endregion Override Methods

        #region Tests

        private void LassoSelect()
        {
            StrokeCollection lasso;

            lasso = DRT.LoadStrokeCollection(DRT.BaseDirectory + "lasso.isf");

            DRT.MouseButtonUp();            // just in case someone left it down

            int count =lasso.Count;
            for ( int i = 0; i < count; i++ )
            {
                Stroke stroke = lasso[i];
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
