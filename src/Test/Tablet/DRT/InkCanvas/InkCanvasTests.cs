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
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Xml;

namespace DRT
{
    public sealed class InkCanvasTests : DrtTabletTestSuite, IEventsForwading
    {
        private InkCanvas           _inkCanvas;
        private Button              _button;
        private EventCallback       _changedCallback;
        private EventCallback       _changingCallback;
        private static string       s_buttonCaption = "DRT Test";
        private string              _xmlString;
        private double              _top = 350;
        private double              _right = 250;

        //
        // Test data
        //
        StrokeCollection _initialStrokes;

        #region Constructor

        public InkCanvasTests() : base("InkCanvasAPITests")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            // For example to create a tree via code:
            Visual root = CreateMyTree();

            _inkCanvas = new InkCanvas();
            _inkCanvas.Background = Brushes.White;

            _button = new Button();
            _button.Content = InkCanvasTests.s_buttonCaption;

            //((IAddChild)_inkCanvas).AddChild(button);
            _inkCanvas.Children.Add(_button);

            ((Border)(root)).Child = _inkCanvas;

            DRT.Show(root);

            _initialStrokes = DRT.LoadStrokeCollection(DRT.BaseDirectory + "drtinkcanvas_initialink.isf");
            DRT.Assert(_initialStrokes.Count > 0, "Couldn't load strokes from disk drtinkcanvas_initialink.isf");

            HookupEvents();

            ArrayList tests = new ArrayList();

            TestUnit unit;

            /////////////////////////////////////////////////////////////
            // Properties
            /////////////////////////////////////////////////////////////

            // InkCanvasEditingMode
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "EditingMode", InkCanvasEditingMode.Ink);
            tests.AddRange(unit.Tests);

            unit = new EnumTypePropertyTest(DRT, _inkCanvas, "EditingMode", typeof(InkCanvasEditingMode));
            tests.AddRange(unit.Tests);

            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            unit = new AccessPropertyTest(DRT, _inkCanvas, "EditingMode", InkCanvasEditingMode.EraseByPoint, this, false);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkCanvas, "EditingMode", 1000, typeof(ArgumentException));
            tests.AddRange(unit.Tests);

            // EditingModeInverted
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "EditingModeInverted", InkCanvasEditingMode.EraseByStroke);
            tests.AddRange(unit.Tests);

            unit = new EnumTypePropertyTest(DRT, _inkCanvas, "EditingModeInverted", typeof(InkCanvasEditingMode));
            tests.AddRange(unit.Tests);

            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            unit = new AccessPropertyTest(DRT, _inkCanvas, "EditingModeInverted", InkCanvasEditingMode.EraseByPoint, this, false);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkCanvas, "EditingModeInverted", 1000, typeof(ArgumentException));
            tests.AddRange(unit.Tests);

            // MoveEnabled
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "MoveEnabled", true);
            tests.AddRange(unit.Tests);

            unit = new BooleanTypePropertyTest(DRT, _inkCanvas, "MoveEnabled");
            tests.AddRange(unit.Tests);

            // ResizeEnabled
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "ResizeEnabled", true);
            tests.AddRange(unit.Tests);

            unit = new BooleanTypePropertyTest(DRT, _inkCanvas, "ResizeEnabled");
            tests.AddRange(unit.Tests);

            // UseCustomCursor
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "UseCustomCursor", false);
            tests.AddRange(unit.Tests);

            unit = new BooleanTypePropertyTest(DRT, _inkCanvas, "UseCustomCursor");
            tests.AddRange(unit.Tests);

            // Strokes
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "Strokes", new ValueCheckingCallback(CheckEmtryStroks));
            tests.AddRange(unit.Tests);

            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            unit = new AccessPropertyTest(DRT, _inkCanvas, "Strokes", _initialStrokes);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkCanvas, "Strokes", null, typeof(ArgumentException));
            tests.AddRange(unit.Tests);

            // DefaultDrawingAttributes
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "DefaultDrawingAttributes", new ValueCheckingCallback(CheckDefaultDefaultDrawingAttributes));
            tests.AddRange(unit.Tests);

            DrawingAttributes das = new DrawingAttributes();
            das.Height = 10f;
            das.Width = 10f;
            unit = new AccessPropertyTest(DRT, _inkCanvas, "DefaultDrawingAttributes", das);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkCanvas, "DefaultDrawingAttributes", null, typeof(ArgumentException));
            tests.AddRange(unit.Tests);

            // EraserShape
            unit = new PropertyDefaultValueTest(DRT, _inkCanvas, "EraserShape", new ValueCheckingCallback(CheckDefaultEraserShape));
            tests.AddRange(unit.Tests);

            StylusShape ss = new EllipseStylusShape(10f, 10f, 0f);
            unit = new AccessPropertyTest(DRT, _inkCanvas, "EraserShape", ss);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkCanvas, "EraserShape", null, typeof(ArgumentNullException));
            tests.AddRange(unit.Tests);

            // Children
            tests.Add(new DrtTest(TestChildrenProperty));

            // DPs
            tests.Add(new DrtTest(TestDPs));

            // Serialization Tests.
            tests.Add(new DrtTest(SerializeTest));
            tests.Add(new DrtTest(DeserializeTest));
            
            /////////////////////////////////////////////////////////////
            // Methods
            /////////////////////////////////////////////////////////////
            tests.Add(new DrtTest(TestClipboardMethods));

            //DynamicRenderer
            tests.Add(new DrtTest(TestDynamicRenderer));

            return (DrtTest[])tests.ToArray(typeof(DrtTest));
        }

        #endregion Override Methods

        #region IEventsForwading

        public void SetChangedCallback(EventCallback callback)
        {
            _changedCallback = callback;
        }

        public void SetChangingCallback(EventCallback callback)
        {
            _changingCallback = callback;
        }

        #endregion IEventsForwading

        #region Tests

        private void TestChildrenProperty()
        {
            DRT.Assert( 1 == _inkCanvas.Children.Count, "InkCanvas.Children.Count should be 1.");
            DRT.Assert( _inkCanvas.Children[0] is Button , "InkCanvas.Children[0] should be a button type.");
        }

        private void TestDPs()
        {
            DRT.ResumeAt(SetBackground);
        }

        private void SetBackground()
        {
            _inkCanvas.SetValue(InkCanvas.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
            DRT.ResumeAt(VerifyBackground);
        }

        private void VerifyBackground()
        {
            DRT.Assert( _inkCanvas.Background is SolidColorBrush && 
                ((SolidColorBrush)(_inkCanvas.Background)).Color == Colors.Yellow, "InkCanvas.Background should be a yellow SolidColorBrush.");
            DRT.ResumeAt(ClearBackground);
        }

        private void ClearBackground()
        {
            _inkCanvas.ClearValue(InkCanvas.BackgroundProperty);
            DRT.ResumeAt(VerifyClearBackground);
        }

        private void VerifyClearBackground()
        {
            PropertyMetadata metadata =
                InkCanvas.BackgroundProperty.GetMetadata(_inkCanvas);

            DRT.Assert(_inkCanvas.Background == SystemColors.WindowBrush, "SystemColors.WindowsBrush should be set as the background  in the InkCanvas' default style ");
            DRT.ResumeAt(SetTop);
        }
        
        private void SetTop()
        {
            _button.SetValue(InkCanvas.TopProperty, (double)100);
            DRT.ResumeAt(VerifyTop);
        }

        private void VerifyTop()
        {
            double val = (double)(_button.GetValue(InkCanvas.TopProperty));
            DRT.Assert( val == 100, "_button's InkCanvas.Top should equal to 100d.");
            DRT.ResumeAt(SetLeft);
        }

        private void SetLeft()
        {
            _button.SetValue(InkCanvas.LeftProperty, (double)100);
            DRT.ResumeAt(VerifyLeft);
        }

        private void VerifyLeft()
        {
            double val = (double)(_button.GetValue(InkCanvas.LeftProperty));
            DRT.Assert( val == 100, "_button's InkCanvas.Left should equal to 100d.");
            DRT.ResumeAt(SetBottom);
        }

        private void SetBottom()
        {
            _button.SetValue(InkCanvas.BottomProperty, (double)100);
            DRT.ResumeAt(VerifyBottom);
        }

        private void VerifyBottom()
        {
            double val = (double)(_button.GetValue(InkCanvas.BottomProperty));
            DRT.Assert( val == 100, "_button's InkCanvas.Bottom should equal to 100d.");
            DRT.ResumeAt(SetRight);
        }

        private void SetRight()
        {
            _button.SetValue(InkCanvas.RightProperty, (double)100);
            DRT.ResumeAt(VerifyRight);
        }

        private void VerifyRight()
        {
            double val = (double)(_button.GetValue(InkCanvas.RightProperty));
            DRT.Assert( val == 100, "_button's InkCanvas.Right should equal to 100d.");
            DRT.ResumeAt(ClearDefaultDrawingAttributes);
        }

        private void ClearDefaultDrawingAttributes()
        {
            _inkCanvas.ClearValue(InkCanvas.DefaultDrawingAttributesProperty);
            DRT.ResumeAt(VerifyClearDefaultDrawingAttributes);
        }

        private void VerifyClearDefaultDrawingAttributes()
        {
            PropertyMetadata metadata =
                InkCanvas.DefaultDrawingAttributesProperty.GetMetadata(_inkCanvas);

            DRT.Assert(_inkCanvas.DefaultDrawingAttributes == new DrawingAttributes(), "The DefaultDrawingAttributes should equal to the default DrawingAttributes.");
        }

        private void TestClipboardMethods()
        {
            try
            {
                IEnumerable<InkCanvasClipboardFormat> formats = _inkCanvas.PreferredPasteFormats;

                List<InkCanvasClipboardFormat> formatList = new List<InkCanvasClipboardFormat>();
                foreach(InkCanvasClipboardFormat format in formats)
                {
                    formatList.Add(format);
                }

                InkCanvasClipboardFormat[] aFormats = formatList.ToArray();
                DRT.Assert(aFormats.Length == 1 && aFormats[0] == InkCanvasClipboardFormat.InkSerializedFormat, "The default formats should only contain Isf format.");
                
                //
                // exercise the clipboard api's
                //
                DataObject dataObj;

                //
                // Test CanPaste with an unsupported format.
                //
                dataObj = new DataObject();

                // Copy a random into the clipboard.
                dataObj.SetData("Random Format", "Some Strings", true);

                // Put our data object into the clipboard.
                Clipboard.SetDataObject(dataObj, true);

                DRT.Assert(false == _inkCanvas.CanPaste(), "InkCanvas.CanPaste failed: The method should return false if there are the unsupport data in the clipboard.");


                //
                // Test CanPaste with ISF format.
                //
                StrokeCollection strokes = _inkCanvas.Strokes;

                dataObj = new DataObject();
                bool copiedData = false;


                if ( dataObj != null )
                {
                    // Copy ink data into the clipboard.
                    if ( strokes != null && strokes.Count != 0 )
                    {
                        // Save the data in the data object.
                        MemoryStream stream = new MemoryStream();
                        strokes.Save(stream);
                        stream.Position = 0;
                        dataObj.SetData(StrokeCollection.InkSerializedFormat, stream);
                        copiedData = true;
                    }
                }

                if ( dataObj != null && copiedData )
                {
                    // Put our data object into the clipboard.
                    Clipboard.SetDataObject(dataObj, true);
                }

                DRT.Assert(true == _inkCanvas.CanPaste(), "InkCanvas.CanPaste did not return true for ISF data");

                DRT.Assert(0 != _inkCanvas.Strokes.Count, "An empty StokeCollection has been detected before the Cut() being invoked.");
                //
                // Test Cut, Copy and Paste ... to be added here ...
                //


                // Cut()
                _inkCanvas.Select(_inkCanvas.Strokes);
                _inkCanvas.CutSelection();
                DRT.Assert(0 == _inkCanvas.Strokes.Count, "The Cut() has failed.");

                // Paste()
                _inkCanvas.Paste();
                DRT.Assert(0 != _inkCanvas.Strokes.Count, "The Paste() has failed.");

                // There is a bug in Paste(Point pt) now. After fixing the bug, add back the test case.
                // Paste(Point pt)

                // Copy()
                _inkCanvas.Select(_inkCanvas.Strokes);
                _inkCanvas.CopySelection();
                DRT.Assert(0 != _inkCanvas.Strokes.Count, "The Copy() has failed.");
                IDataObject idataObj = Clipboard.GetDataObject();
                DRT.Assert(idataObj.GetDataPresent(StrokeCollection.InkSerializedFormat) == true, "InkCanvas.Copy failed!");

                //
                // Test CanPaste with the unicode text format.
                //
                dataObj = new DataObject();
                // Copy a string into the clipboard.
                dataObj.SetData(DataFormats.UnicodeText, "Some Strings", true);
                // Put our data object into the clipboard.
                Clipboard.SetDataObject(dataObj, true);

                DRT.Assert(false == _inkCanvas.CanPaste(), "InkCanvas.CanPaste should return false before the Text format is enabled.");
                _inkCanvas.PreferredPasteFormats = new InkCanvasClipboardFormat[]{InkCanvasClipboardFormat.Text, InkCanvasClipboardFormat.InkSerializedFormat};

                DRT.Assert(true == _inkCanvas.CanPaste(), "InkCanvas.CanPaste should return true when the text format is enabled.");

                _inkCanvas.Paste();
                DRT.Assert( 2 == _inkCanvas.Children.Count, "InkCanvas.Paste() failed: The unicode text can't be pasted to InkCanvas.");
                DRT.Assert( _inkCanvas.Children[1] is TextBox, "InkCanvas.Paste() failed: A Textbox wasn't created after a unicode text has been pasted.");
                
            }
            catch ( ExternalException exp )
            {
                // OleGetClipboard failed
                if ( exp.ErrorCode == -2147221040 )
                {
                    DRT.ConsoleOut.WriteLine("\nOleGetClipboard Failed! Skip the clipboard tests.\n");

                }
                else
                {
                    throw;
                }
            }

        }

        private void TestDynamicRenderer()
        {
            DerivedInkCanvas inkCanvas = new DerivedInkCanvas();

            DynamicRenderer dr1 = new DynamicRenderer();
            DynamicRenderer dr2 = new DynamicRenderer();
            DynamicRenderer dr3 = new DynamicRenderer();
            DynamicRenderer drOriginal = inkCanvas.InkCanvasDynamicRenderer;

            //check our assumptions
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 1, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");

            //set to null
            inkCanvas.InkCanvasDynamicRenderer = null;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 0, "Incorrect InkCanvas plugin assumption!");

            //set to null again
            inkCanvas.InkCanvasDynamicRenderer = null;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 0, "Incorrect InkCanvas plugin assumption!");

            //set back to original
            inkCanvas.InkCanvasDynamicRenderer = drOriginal;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 1, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");

            //set to original again
            inkCanvas.InkCanvasDynamicRenderer = drOriginal;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 1, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");

            //order plugins
            inkCanvas.InkCanvasStylusPlugIns.Insert(0, dr1);
            inkCanvas.InkCanvasStylusPlugIns.Add(dr2);
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 3, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[1], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr2 == inkCanvas.InkCanvasStylusPlugIns[2], "Incorrect InkCanvas plugin assumption!");

            //replace plugin
            inkCanvas.InkCanvasDynamicRenderer = dr3;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 3, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr3 == inkCanvas.InkCanvasStylusPlugIns[1], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr2 == inkCanvas.InkCanvasStylusPlugIns[2], "Incorrect InkCanvas plugin assumption!");
            
            //replace plugin
            inkCanvas.InkCanvasDynamicRenderer = drOriginal;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 3, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[1], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr2 == inkCanvas.InkCanvasStylusPlugIns[2], "Incorrect InkCanvas plugin assumption!");

            //replace plugin
            inkCanvas.InkCanvasDynamicRenderer = dr2;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 2, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr2 == inkCanvas.InkCanvasStylusPlugIns[1], "Incorrect InkCanvas plugin assumption!");

            //set to null again
            inkCanvas.InkCanvasDynamicRenderer = null;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 1, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");

            //set back to original
            inkCanvas.InkCanvasDynamicRenderer = drOriginal;
            DRT.Assert(inkCanvas.InkCanvasStylusPlugIns.Count == 2, "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(dr1 == inkCanvas.InkCanvasStylusPlugIns[0], "Incorrect InkCanvas plugin assumption!");
            DRT.Assert(drOriginal == inkCanvas.InkCanvasStylusPlugIns[1], "Incorrect InkCanvas plugin assumption!");
        }

        private void SerializeTest()
        {
            InkCanvas.SetTop(_button, _top);
            InkCanvas.SetRight(_button, _right);

            _xmlString = XamlWriter.Save(_inkCanvas);
            DRT.Assert(_xmlString.Length != 0, "Fail to serialize InkCanvas!");
        }

        private void DeserializeTest()
        {
            InkCanvas newIC = XamlReader.Load(new XmlTextReader(new StringReader(_xmlString))) as InkCanvas;

            DRT.Assert(1 == newIC.Children.Count, "The deserialized InkCanvas.Children.Count should be 1.");
            DRT.Assert(newIC.Children[0] is Button, "The deserialized InkCanvas.Children[0] should be a button type.");

            DRT.Assert(newIC.Strokes.Count == _inkCanvas.Strokes.Count, "Fail to make a round trip for the InkCanvas.Strokes Property.");

            double top = InkCanvas.GetTop(newIC.Children[0]);
            double right = InkCanvas.GetRight(newIC.Children[0]);

            DRT.Assert(top == _top && right == _right, "Fail to make a round trip for the attached properties InkCanvas.TopProperty or InkCanvas.BottomProperty.");
        }

        #endregion Tests

        #region EventHandlers

        private void InkCanvas_EditingModeChanged(object target, RoutedEventArgs e)
        {
            object[] args = new object[] { target, e };
            ForwardChangedEvent((object)args);
        }

        #endregion EventHandlers

        #region Private Implementation

        private void HookupEvents()
        {
            // Note: EditingMode and EditingModeInverted share the same event handlers.
            _inkCanvas.EditingModeChanged += new RoutedEventHandler(InkCanvas_EditingModeChanged);

            _inkCanvas.EditingModeInvertedChanged += new RoutedEventHandler(InkCanvas_EditingModeChanged);

        }

        private Visual CreateMyTree()
        {
            return new Border();
        }

        private void ForwardChangedEvent(object arg)
        {
            if ( _changedCallback != null )
            {
                _changedCallback(arg);
            }
        }

        private bool ForwardChangingEvent(object arg)
        {
            bool ret = false;

            if ( _changingCallback != null )
            {
                ret = (bool)_changingCallback(arg);
            }

            return ret;
        }

        private void CheckEmtryStroks(DrtBase drt, object value, string propertyName)
        {
            StrokeCollection strokes = value as StrokeCollection;

            drt.Assert(strokes.Count == 0, string.Format("The stroke collection is not empty"));
        }

        private void CheckDefaultDefaultDrawingAttributes(DrtBase drt, object value, string propertyName)
        {
            DrawingAttributes das = value as DrawingAttributes;
            drt.Assert(das == new DrawingAttributes(), string.Format("The default value of {0} is incorrect!", propertyName));
        }

        private void CheckDefaultEraserShape(DrtBase drt, object value, string propertyName)
        {
            StylusShape ss = value as StylusShape;

            drt.Assert(ss.Width == 8f && ss.Height == 8f && ss.Rotation == 0,
                string.Format("The default value of {0} is incorrect!", propertyName));
        }

        #endregion Private Implementation
    }

    public class DerivedInkCanvas : InkCanvas
    {
        public DynamicRenderer InkCanvasDynamicRenderer
        {
            get { return this.DynamicRenderer; }
            set { this.DynamicRenderer = value; }
        }

        public StylusPlugInCollection InkCanvasStylusPlugIns
        {
            get { return this.StylusPlugIns; }
        }
    }
}
