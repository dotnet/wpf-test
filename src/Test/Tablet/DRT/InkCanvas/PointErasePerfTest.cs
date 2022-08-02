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
    public class PointErasePerfTest : InkCanvasPerfTestBase
    {
        #region Constructor

        public PointErasePerfTest()
            : base("PointErasePerfTest")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            string fullname = DRT.BaseDirectory + "SelectionMovingPerfTest.xaml";
            System.IO.Stream stream = File.OpenRead(fullname);
            Visual root = (Visual)XamlReader.Load(stream);

            InitTree(root);

            DRT.Show(root);

            return new DrtTest[] { 
                    new DrtTest(StartLog),
                    new DrtTest(PointErasing),
                    new DrtTest(EndLog),
                };

        }

        #endregion Override Methods

        #region Tests

        private void PointErasing()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            Point pt = new Point(50, 50);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));
            
            
            for ( int start = 0, current = start, end = 500, step = (end - start) / 250; 
                current < end; 
                current += step )
            {
                pt.Offset(step, step);
                _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            }

            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            pt = new Point(550, 50);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));


            for ( int start = 0, current = start, end = 500, step = ( end - start ) / 250;
                current < end;
                current += step )
            {
                pt.Offset(-step, step);
                _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            }

            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        #endregion Tests

        private void InitTree(DependencyObject root)
        {
            _inkCanvas = (InkCanvas)root;
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }
    }
}
