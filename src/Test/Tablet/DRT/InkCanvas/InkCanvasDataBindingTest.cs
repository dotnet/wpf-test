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
    public class InkCanvasDataBindingTests : InkCanvasEditingTests
    {
        private   ListBox             _lbEditingMode;
        private   ListBox             _lbDrawingAttributes;
        private   ListBox             _lbStrokeCollections;
        private   TextBlock           _tbCount;
        private   int                 _count = 0;

        #region Constructor

        public InkCanvasDataBindingTests()
            : base("InkCanvasDataBindingTests")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            string fullname = DRT.BaseDirectory + "DrtInkCanvasDataBinding.xaml";
            System.IO.Stream stream = File.OpenRead(fullname);
            Visual root = (Visual)XamlReader.Load(stream);

            InitTree(root);

            DRT.Show(root);

            return new DrtTest[] { 
                    new DrtTest(VerifyEditingModeListBoxSelectedItem),
                    new DrtTest(DrawInk),
                    new DrtTest(VerifyInk),
                    new DrtTest(ChangeToSelectMode),
                    new DrtTest(TapOnStroke),
                    new DrtTest(VerifySelection),
                    new DrtTest(ChangeToEraseByStrokeMode),
                    new DrtTest(TapOnStroke),
                    new DrtTest(VerifyErase),
                    new DrtTest(ChangeInkCanvas_EditingModeToInk),
                    new DrtTest(VerifylbEditingMode_SelectedItem),
                    new DrtTest(ChangeToDrawingAttributes0),
                    new DrtTest(DrawInk),
                    new DrtTest(VerifyDrawingAttributes0),
                    new DrtTest(ChangeToDrawingAttributes1),
                    new DrtTest(DrawInk),
                    new DrtTest(VerifyDrawingAttributes1),
                    new DrtTest(ModifyDrawingAttributes1),
                    new DrtTest(DrawInk),
                    new DrtTest(VerifyModifiedDrawingAttributes1),
                    new DrtTest(ChangeToStrokeCollection1),
                    new DrtTest(VerifyStrokeCollectionChange),
                    new DrtTest(ChangeToStrokeCollection3),
                    new DrtTest(VerifyStrokeCollectionChange),
                };

        }

        #endregion Override Methods

        #region Tests

        private void VerifyEditingModeListBoxSelectedItem()
        {
            DRT.Assert(_inkCanvas.EditingMode == (InkCanvasEditingMode)(_lbEditingMode.SelectedItem), "The selected item in lbEditingMode shoud equal to inkCanvas.EditingMode");
        }

        private void VerifyInk()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == 1, "No Ink collected!");
        }

        private void ChangeToSelectMode()
        {
            _lbEditingMode.SelectedIndex = 1;
        }

        private void VerifySelection()
        {
            DRT.AssertEqual(1, _inkCanvas.GetSelectedStrokes().Count, "Failed to select a stroke!");
        }

        private void ChangeToEraseByStrokeMode()
        {
            _lbEditingMode.SelectedIndex = 2;
        }

        private void VerifyErase()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == 0, "Failed to erase a stroke!");
        }

        private void ChangeInkCanvas_EditingModeToInk()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void VerifylbEditingMode_SelectedItem()
        {
            DRT.Assert(InkCanvasEditingMode.Ink == (InkCanvasEditingMode)( _lbEditingMode.SelectedItem ), "The selected item in lbEditingMode shoud be InkCanvasEditingMode.Ink");
        }

        private void ChangeToDrawingAttributes0()
        {
            _lbDrawingAttributes.SelectedIndex = 0;
        }

        private void VerifyDrawingAttributes0()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == _count + 1, "Failed to draw a stroke!");
            DRT.Assert(_inkCanvas.Strokes[_count].DrawingAttributes == ((DrawingAttributes)_lbDrawingAttributes.Items[0]), "Wrong DrawingAttributes on the new stroke. It shoud be lbDrawingAttributes.Items[0]");
            _count = _inkCanvas.Strokes.Count;
        }

        private void ChangeToDrawingAttributes1()
        {
            _lbDrawingAttributes.SelectedIndex = 1;
        }

        private void VerifyDrawingAttributes1()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == _count + 1, "Failed to draw a stroke!");
            DRT.Assert(_inkCanvas.Strokes[_count].DrawingAttributes == ((DrawingAttributes)_lbDrawingAttributes.Items[1]), "Wrong DrawingAttributes on the new stroke. It shoud be lbDrawingAttributes.Items[1]");
            _count = _inkCanvas.Strokes.Count;
        }

        private void ModifyDrawingAttributes1()
        {
            ((DrawingAttributes)_lbDrawingAttributes.Items[1]).Color = Colors.Red;
        }

        private void VerifyModifiedDrawingAttributes1()
        {
            DRT.Assert(_inkCanvas.Strokes.Count == _count + 1, "Failed to draw a stroke!");
            DRT.Assert(_inkCanvas.Strokes[_count].DrawingAttributes.Color == Colors.Red, 
                "Wrong Color of the DrawingAttributes on the new stroke. It shoud be Red");
            _count = _inkCanvas.Strokes.Count;
        }

        private void DrawInk()
        {
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(200, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(200, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(220, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(230, 155)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(240, 145)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(250, 150)));

            DRT.ResumeAt(new DrtTest(SendInputs));

        }

        private void TapOnStroke()
        {
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(220, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(220, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(220, 150)));

            DRT.ResumeAt(new DrtTest(SendInputs));

        }

        private void ChangeToStrokeCollection1()
        {
            _lbStrokeCollections.SelectedIndex = 1;
        }

        private void VerifyStrokeCollectionChange()
        {
            string isfData = (string)(_lbStrokeCollections.SelectedItem);
            StrokeCollection strokes = new StrokeCollection(new MemoryStream(Convert.FromBase64String(isfData)));

            DRT.Assert(strokes.Count == Convert.ToInt32(_tbCount.Text), "Incorrect StrokeCollection count in the bound tbTextBlock");
        }

        private void ChangeToStrokeCollection3()
        {
            _lbStrokeCollections.SelectedIndex = 3;
        }

        #endregion Tests

        private void InitTree(DependencyObject root)
        {
            _inkCanvas = DRT.FindElementByID("inkCanvas", root) as InkCanvas;
            _lbEditingMode = DRT.FindElementByID("lbEditingMode", root) as ListBox;
            _lbDrawingAttributes = DRT.FindElementByID("lbDrawingAttributes", root) as ListBox;
            _lbStrokeCollections = DRT.FindElementByID("lbStrokeCollections", root) as ListBox;
            _tbCount = DRT.FindElementByID("tbCount", root) as TextBlock;

            DRT.Assert(_inkCanvas != null);
            DRT.Assert(_lbEditingMode != null);
            DRT.Assert(_lbDrawingAttributes != null);
            DRT.Assert(_lbStrokeCollections != null);
            DRT.Assert(_tbCount != null);

            DRT.Assert(_lbEditingMode.Items.Count == 3, "Wrong data in lbEditingMode");
            DRT.Assert((InkCanvasEditingMode)(_lbEditingMode.Items[0]) == InkCanvasEditingMode.Ink, "Wrong data in lbEditingMode");
            DRT.Assert((InkCanvasEditingMode)(_lbEditingMode.Items[1]) == InkCanvasEditingMode.Select, "Wrong data in lbEditingMode");
            DRT.Assert((InkCanvasEditingMode)(_lbEditingMode.Items[2]) == InkCanvasEditingMode.EraseByStroke, "Wrong data in lbEditingMode");

            DRT.Assert(_lbDrawingAttributes.Items.Count == 2, "Wrong data in lbDrawingAttributes");
            DRT.Assert(_lbStrokeCollections.Items.Count == 4, "Wrong data in lbStrokeCollections");

        }

    }
}
