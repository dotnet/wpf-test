// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Runtime.InteropServices;
using MS.Win32;

namespace DRT
{
    /// <summary>
    /// This is a test suite for testing StickyNote.EXE scenarios
    /// 1. Moving the host window;
    /// 2. Resizing the host windows;
    /// </summary>
    public sealed class InkStickyNoteTests : DrtStickyNoteControlTestSuite
    {
        private enum InputAction
        {
            MouseMove = 0,
            MouseLeftDown = 1,
            MouseLeftUp = 2,
        }

        private class TestInput
        {
            private InputAction _action;
            private Point _position;

            public TestInput(InputAction action, Point position)
            {
                _action = action;
                _position = position;
            }

            public InputAction Action
            {
                get
                {
                    return _action;
                }
            }

            public Point Position
            {
                get
                {
                    return _position;
                }
            }
        }

        private InkCanvas _inkCanvas;
        private Queue<TestInput> _inputQueue;

        public InkStickyNoteTests()
            : base("InkStickyNoteTests", false)
        {
        }

        public override DrtTest[] PrepareTests()
        {
            _inputQueue = new Queue<TestInput>();

            DrtTest[] prepareStickyNoteControlTests = base.PrepareTests();

            List<DrtTest> tests = new List<DrtTest>();

            // Tests for creating StickyNoteControl via CAF api
            foreach ( DrtTest test in prepareStickyNoteControlTests )
            {
                tests.Add(test);
            }

            // Tests after the visual tree being set up.
            tests.Add(new DrtTest(IdentifyControls));
            tests.Add(new DrtTest(InkTest));
            tests.Add(new DrtTest(VerifyInkTest));
            tests.Add(new DrtTest(SelectTest));
            tests.Add(new DrtTest(VerifySelectTest));
            tests.Add(new DrtTest(EraseTest));
            tests.Add(new DrtTest(VerifyEraseTest));

            return tests.ToArray();
        }

        #region Tests

        private void SendInputs()
        {
            if ( _inputQueue.Count == 0 )
            {
                return;
            }

            TestInput input = _inputQueue.Dequeue();

            switch ( input.Action )
            {
                case InputAction.MouseMove:
                    DRT.MoveMouse(_inkCanvas, input.Position.X, input.Position.Y);
                    DRT.Trace(string.Format("Move: Position = {0}", input.Position));
                    break;
                case InputAction.MouseLeftDown:
                    DRT.MouseButtonDown();
                    DRT.Trace(string.Format("Down"));
                    break;
                case InputAction.MouseLeftUp:
                    DRT.MouseButtonUp(); ;
                    DRT.Trace(string.Format("Up"));
                    break;
            }
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void InkTest()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.15, 0.5)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(0.15, 0.5)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.23, 0.51)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.30, 0.48)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.36, 0.50)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.40, 0.49)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(0.40, 0.49)));
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyInkTest()
        {
            DRT.Assert(_inkCanvas.Strokes.Count != 0, "VerifyInkTest is failed!");
        }

        public void SelectTest()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            StickyNoteControl.InkCommand.Execute(InkCanvasEditingMode.Select, _inkCanvas);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.30, 0.48)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(0.30, 0.48)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(0.30, 0.48)));
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifySelectTest()
        {
            DRT.Assert(_inkCanvas.GetSelectedStrokes().Count != 0, "VerifySelectTest is failed!");
        }

        public void EraseTest()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            StickyNoteControl.InkCommand.Execute(InkCanvasEditingMode.EraseByStroke, _inkCanvas);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.29, 0.47)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(0.29, 0.47)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(0.31, 0.49)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(0.31, 0.49)));
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyEraseTest()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == 0, "VerifyEraseTest is failed!");
        }

        #endregion Tests

        #region Private Methods

        private void IdentifyControls()
        {
            _inkCanvas = (InkCanvas)( DRT.FindVisualByType(typeof(InkCanvas), StickyNote, false) );
        }

        #endregion Private Methods


    }
}
