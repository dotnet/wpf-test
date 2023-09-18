// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for RichTextBox properties.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/RichTextBoxOM/RichTextBoxProperties.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    enum FlowDocOperations
    {
        SetToNull,
        SwitchDocs,
        SetProperties,
    }

    enum TabOperations
    {
        NoSelection,
        SelectIndividual,
        SelectAll,
    }

    enum BUICOperations
    {
        SelectAllDelete,
        BackspaceAfterBUIC,
        BackspacesFromNextPara,
        OvertypingBUICSelected,
    }

    /// <summary>
    /// Provides test cases for RichTextBox's Document property.
    /// </summary>
    [Test(0, "RichTextBox", "RichTextBoxProperties", MethodParameters = "/TestCaseType=RichTextBoxProperties")]
    [TestOwner("Microsoft"), TestTactics("683"), TestBugs("829,830,831,173,174,832,833,834,175")]
    public class RichTextBoxProperties : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _panel = new StackPanel();
            _button = new Button();
            _control = new RichTextBox();

            _panel.Children.Add(_button);
            _panel.Children.Add(_control);
            MainWindow.Content = _panel;
            QueueDelegate(VerifyDocument);
        }

        /// <summary>Verifies that the Document property works as expected.</summary>
        private void VerifyDocument()
        {
            FlowDocument document;
            FlowDocument otherDocument;
            RichTextBox otherControl;

            document = new FlowDocument(new Paragraph());
            otherDocument = new FlowDocument(new Paragraph());

            Verifier.Verify(null != (new RichTextBox()).Document,
                "Document CLR property is never null.", true);

            try
            {
                _control.Document = null;
                throw new ApplicationException("RichTextBox.Document accepts null documents.");
            }
            catch (ArgumentNullException)
            {
                Log("RichTextBox.Document rejects null documents as expected.");
            }

            _control.Document = document;
            otherControl = new RichTextBox();
            try
            {
                otherControl.Document = document;
                throw new ApplicationException("Documents can be shared directly.");
            }
            catch (ArgumentException) //Coverage for Regression_Bug173            
            {
                Log("Documents cannot be shared directly, as expected.");
            }

            _control.Document = otherDocument;
            Verifier.Verify(otherDocument == _control.Document,
                "SetValue can change from one document to another.", true);

            _control.Document = new FlowDocument(new Paragraph());
            QueueDelegate(CheckUpdate);
        }

        private void CheckUpdate()
        {
            Log("Check that the RichTextBox does not crash on re-rendering and re-taking focus...");
            MouseInput.MouseClick(_control);
            QueueDelegate(RemoveFocus);
        }

        private void RemoveFocus()
        {
            Log("Verifying that the document can be changed when the control is not focused.");
            //Coverage for Regression_Bug174
            MouseInput.MouseClick(_button);
            QueueDelegate(ChangeWithFocusMissing);
        }

        private void ChangeWithFocusMissing()
        {
            _control.Document = new FlowDocument(new Paragraph());
            MouseInput.MouseClick(_control);
            QueueDelegate(CheckFocusRetaken);
        }

        private void CheckFocusRetaken()
        {
            Verifier.Verify(_control.IsKeyboardFocused, "Focus regained by control.", true);
            QueueDelegate(CheckShouldSerializeDocument);
        }

        private void CheckShouldSerializeDocument()
        {
            RichTextBox rtb;
            Paragraph para;

            //Implicit document with non empty run
            rtb = new RichTextBox();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run("123")));
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Implicit document with non-empty Run", false);

            //Implicit document with empty run
            rtb = new RichTextBox();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run()));
            Verifier.Verify(rtb.ShouldSerializeDocument() == false,
                "Verifying ShouldSerializeDocument for Implicit document with empty Run", false);

            //Coverage for Regression_Bug175
            //Implicit document with empty first run but non-empty second run
            rtb = new RichTextBox();
            rtb.Document.Blocks.Clear();
            para = new Paragraph();
            para.Inlines.Add(new Run());
            para.Inlines.Add(new Run("123"));
            rtb.Document.Blocks.Add(para);
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Implicit document with empty first Run but non-empty second run", false);

            //Implicit document with empty paragraph
            rtb = new RichTextBox();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph());
            Verifier.Verify(rtb.ShouldSerializeDocument() == false,
                "Verifying ShouldSerializeDocument for Implicit document with empty paragraph", false);

            //Implicit document with multiple empty paragraph
            rtb = new RichTextBox();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph());
            rtb.Document.Blocks.Add(new Paragraph());
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Implicit document with multiple empty paragraph", false);

            //Explicit document with non empty run
            rtb = new RichTextBox();
            rtb.Document = new FlowDocument();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run("123")));
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Explicit document with non-empty Run", false);

            //Explicit document with empty run
            rtb = new RichTextBox();
            rtb.Document = new FlowDocument();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run()));
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Explicit document with empty Run", false);

            //Explicit document with empty first run but non-empty second run
            rtb = new RichTextBox();
            rtb.Document = new FlowDocument();
            rtb.Document.Blocks.Clear();
            para = new Paragraph();
            para.Inlines.Add(new Run());
            para.Inlines.Add(new Run("123"));
            rtb.Document.Blocks.Add(para);
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Explicit document with empty first Run but non-empty second run", false);

            //Explicit document with empty paragraph
            rtb = new RichTextBox();
            rtb.Document = new FlowDocument();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph());
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Explicit document with empty paragraph", false);

            //Explicit document with multiple empty paragraph
            rtb = new RichTextBox();
            rtb.Document = new FlowDocument();
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph());
            rtb.Document.Blocks.Add(new Paragraph());
            Verifier.Verify(rtb.ShouldSerializeDocument() == true,
                "Verifying ShouldSerializeDocument for Explicit document with multiple empty paragraph", false);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Control being tested.</summary>
        private RichTextBox _control;

        private StackPanel _panel;
        private Button _button;

        #endregion Private fields.
    }

    /// <summary>
    /// Test for CaretPosition property of RichTextBox   
    /// </summary>
    [Test(0, "Selection", "CaretPositionTest", MethodParameters = "/TestCaseType=CaretPositionTest")]
    [TestOwner("Microsoft"), TestTactics("684"), TestWorkItem(""), TestBugs("")]
    public class CaretPositionTest : CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            return new Dimension[] {
                new Dimension("TextSelection", TextSelectionData.Values),                
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _selectionData = (TextSelectionData)values["TextSelection"];
            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 500;
            _rtb.Width = 500;

            ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run("Sample Text"));
            _rtbWrapper = new UIElementWrapper(_rtb);

            TestElement = _rtb;

            if (!_invalidTestingDone)
            {
                DoInvalidTesting();
                DoSpecialCaseTesting();
                _invalidTestingDone = true;
            }

            QueueDelegate(BeforePrepare);
        }

        private void BeforePrepare()
        {
            _rtb.Focus();
            _selectionData.PrepareForSelection(_rtbWrapper);
            QueueDelegate(AfterPrepare);
        }

        private void AfterPrepare()
        {
            if (_selectionData.Select(_rtbWrapper))
            {
                Log("Adjusted selection for: " + _selectionData.TestValue);
                VerifyGet_CaretPosition();
                VerifySet_CaretPosition();
            }
            else
            {
                Log("Unable to select: " + _selectionData.TestValue);
                QueueDelegate(NextCombination);
            }
        }

        private void VerifyGet_CaretPosition()
        {
            _testCaretPosition = _rtb.CaretPosition;
            Verifier.Verify(_testCaretPosition.GetOffsetToPosition(_rtb.Selection.End) == 0,
                "Verifying that Get_CaretPosition returns the right end of the selection", true);
            QueueDelegate(NextCombination);
        }

        private void VerifySet_CaretPosition()
        {
            TextPointer _selectionStart, _selectionEnd, _tempPointer;

            _selectionStart = _rtb.Selection.Start;
            _selectionEnd = _rtb.Selection.End;

            _rtb.CaretPosition = _selectionStart;
            _tempPointer = _selectionStart;
            _tempPointer = _tempPointer.GetInsertionPosition(LogicalDirection.Forward);
            Verifier.Verify(_rtb.CaretPosition.GetOffsetToPosition(_tempPointer) == 0,
                "Verifying that setting CaretPosition to SelectionStart moves it correctly", true);
            Verifier.Verify(_rtb.Selection.IsEmpty, "Verifying that Selection is empty after setting CaretPosition", false);

            _rtb.Selection.Select(_selectionStart, _selectionEnd);
            _rtb.CaretPosition = _selectionEnd;
            _tempPointer = _selectionEnd;
            _tempPointer = _tempPointer.GetInsertionPosition(LogicalDirection.Backward);
            Verifier.Verify(_rtb.CaretPosition.GetOffsetToPosition(_tempPointer) == 0,
                "Verifying that setting CaretPosition to SelectionEnd moves it correctly", true);
            Verifier.Verify(_rtb.Selection.IsEmpty, "Verifying that Selection is empty after setting CaretPosition", false);

            _rtb.Selection.Select(_selectionStart, _selectionEnd);
            _rtb.CaretPosition = _rtb.Document.ContentStart; //Should get normalized
            _tempPointer = _rtb.Document.ContentStart;
            _tempPointer = _tempPointer.GetInsertionPosition(LogicalDirection.Forward);
            Verifier.Verify(_rtb.CaretPosition.GetOffsetToPosition(_tempPointer) == 0,
                "Verifying that setting CaretPosition to ContentStart moves it correctly", true);
            Verifier.Verify(_rtb.Selection.IsEmpty, "Verifying that Selection is empty after setting CaretPosition", false);

            _rtb.Selection.Select(_selectionStart, _selectionEnd);
            _rtb.CaretPosition = _rtb.Document.ContentEnd; //Should get normalized
            _tempPointer = _rtb.Document.ContentEnd;
            _tempPointer = _tempPointer.GetInsertionPosition(LogicalDirection.Backward);
            Verifier.Verify(_rtb.CaretPosition.GetOffsetToPosition(_tempPointer) == 0,
                "Verifying that setting CaretPosition to ContentEnd moves it correctly", true);
            Verifier.Verify(_rtb.Selection.IsEmpty, "Verifying that Selection is empty after setting CaretPosition", false);
        }

        private void DoInvalidTesting()
        {
            RichTextBox _otherRichTextBox;

            try
            {
                _rtb.CaretPosition = null;
                throw new ApplicationException("Able to assign null value to CaretPosition");
            }
            catch (ArgumentNullException e)
            {
                Log(e.Message);
                Log("ArgumentNullException thrown as expected when assigning null value to CaretPosition");
            }

            try
            {
                _otherRichTextBox = new RichTextBox();
                _rtb.CaretPosition = _otherRichTextBox.Document.ContentStart;
                throw new ApplicationException("Able to assign CaretPosition to a TextPointer in a different Container");
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
                Log("ArgumentException thrown as expected when trying to assign CaretPosition to a TextPointer in a different Container");
            }
        }

        private void DoSpecialCaseTesting()
        {
            RichTextBox _otherRichTextBox;
            TextPointer _otherTextPointer;
            int _normalizedCaretPositionOffset;

            _otherRichTextBox = new RichTextBox();
            ((Paragraph)_otherRichTextBox.Document.Blocks.FirstBlock).Inlines.Add(new Run("\r\n"));
            _otherTextPointer = ((Paragraph)_otherRichTextBox.Document.Blocks.FirstBlock).Inlines.FirstInline.ContentStart;
            _otherTextPointer = _otherTextPointer.GetPositionAtOffset(1); //will place it between \r and \n
            _otherRichTextBox.CaretPosition = _otherTextPointer;
            _normalizedCaretPositionOffset = _otherRichTextBox.Document.ContentStart.GetOffsetToPosition(_otherRichTextBox.CaretPosition);
            Log("Normalized CaretPosition's offset: " + _normalizedCaretPositionOffset);
            Verifier.Verify(_normalizedCaretPositionOffset == 2,
                "Verifying that caretPosition gets normalized if set to non-normalized positions", true);
        }

        #endregion Main flow

        #region Private fields

        RichTextBox _rtb;
        UIElementWrapper _rtbWrapper;
        private TextSelectionData _selectionData;
        TextPointer _testCaretPosition;
        bool _invalidTestingDone;

        #endregion Private fields
    }

    /// <summary>
    /// Test case to test cursor is settable in FlowDocument
    /// </summary>
    [Test(2, "RichTextBox", "CursorInFlowDocument", MethodParameters = "/TestCaseType=CursorInFlowDocument")]
    [TestOwner("Microsoft"), TestBugs("672"), TestTactics("682"), TestWorkItem("144")]
    public class CursorInFlowDocument : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _document = new FlowDocument();
            _rtb.Document = _document;

            _run1 = new Run("Run1 ");
            _run2 = new Run("Run2 ");

            _button = new Button();
            _button.Content = "Button";

            _paragraph1 = new Paragraph(_run1);
            _paragraph1.Inlines.Add(_button);
            _paragraph2 = new Paragraph(_run2);

            _document.Blocks.Add(_paragraph1);
            _document.Blocks.Add(_paragraph2);

            _wrapper = new UIElementWrapper(_rtb);

            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            TestElement = _panel;

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _rtb.Focus();

            if (!_isDefaultCursorSet)
            {
                Rect r1, r2;

                //Calculate mouse hover point to get the default cursor
                r1 = _wrapper.GetGlobalCharacterRect(_paragraph2.ContentStart,
                                LogicalDirection.Forward);
                r2 = _wrapper.GetGlobalCharacterRect(_paragraph2.ContentEnd,
                    LogicalDirection.Backward);
                _defaultHoverPoint = new Point((r1.X + r2.X) / 2, r1.Y + (r1.Height / 2));

                MouseInput.MouseMove(_defaultHoverPoint);

                QueueDelegate(GetDefaultCursor);
            }
            else
            {
                QueueDelegate(SetSelection);
            }
        }

        private void GetDefaultCursor()
        {
            _defaultCursor = Win32.SafeGetCursor();

            _rtb.Selection.Select(_paragraph2.ContentStart, _paragraph2.ContentEnd);

            //Adjust the mouse hover point by a small offset to get the default cursor with selection.
            Point adjustedHoverPoint = _defaultHoverPoint;
            adjustedHoverPoint.Offset(1d, 1d);

            MouseInput.MouseMove(adjustedHoverPoint);
            QueueDelegate(GetDefaultSelectionCursor);
        }

        private void GetDefaultSelectionCursor()
        {
            _defaultSelectionCursor = Win32.SafeGetCursor();
            _isDefaultCursorSet = true;
            QueueDelegate(SetSelection);
        }

        private void SetSelection()
        {
            Rect rect1, rect2;
            TextPointer startPointer, endPointer;
            startPointer = endPointer = null;
            Cursor crossCursor = Cursors.Cross;

            _rtb.Focus();

            //Set the cursor property on target element.
            switch (_targetElement)
            {
                case "Run":
                    _run1.Cursor = crossCursor;
                    startPointer = _run1.ContentStart.GetPositionAtOffset(0);
                    endPointer = _run1.ContentEnd.GetPositionAtOffset(0);
                    break;
                case "Paragraph":
                    _paragraph1.Cursor = crossCursor;
                    startPointer = _paragraph1.ContentStart.GetPositionAtOffset(0);
                    //endPointer = paragraph1.ContentEnd.GetPositionAtOffset(0);
                    //Need to get a mouse position on Paragraph (and not on button)
                    endPointer = _run1.ContentEnd.GetPositionAtOffset(0);
                    break;
                case "Button":
                    _button.Cursor = crossCursor;
                    startPointer = ((InlineUIContainer)_button.Parent).ContentStart.GetPositionAtOffset(0);
                    endPointer = ((InlineUIContainer)_button.Parent).ContentEnd.GetPositionAtOffset(0);
                    break;
                case "FlowDocument":
                    _document.Cursor = crossCursor;
                    startPointer = _paragraph2.ContentStart.GetPositionAtOffset(0);
                    endPointer = _paragraph2.ContentEnd.GetPositionAtOffset(0);
                    break;
            }

            //Set selection
            if (_withSelection)
            {
                _rtb.Selection.Select(startPointer, endPointer);
            }

            //Calculate mouse hover point
            rect1 = _wrapper.GetGlobalCharacterRect(startPointer,
                            LogicalDirection.Forward);
            rect2 = _wrapper.GetGlobalCharacterRect(endPointer,
                LogicalDirection.Forward);
            _hoverPoint = new Point((rect1.X + rect2.X) / 2, rect1.Y + (rect1.Height / 2));

            QueueDelegate(MoveMouse);
        }

        private void MoveMouse()
        {
            MouseInput.MouseMove(_hoverPoint);
            QueueDelegate(TestCursor);
        }

        private void TestCursor()
        {
            Win32.HCURSOR testCursor = Win32.SafeGetCursor();
            if (_targetElement != "Button")
            {
                if (_withSelection)
                {
                    Verifier.Verify(testCursor != _defaultSelectionCursor, "Verifying that testCursor[" + testCursor.ToString() +
                    "] != defaultSelectionCursor[" + _defaultSelectionCursor.ToString() + "]", true);
                }
                else
                {
                    Verifier.Verify(testCursor != _defaultCursor, "Verifying that testCursor[" + testCursor.ToString() +
                    "] != defaultCursor[" + _defaultCursor.ToString() + "]", true);
                }
            }
            else //Cursor is always default cursor when on top of Button/UIElements. Look at 
            {
                if (_withSelection)
                {
                    Verifier.Verify(testCursor == _defaultSelectionCursor, "Verifying that testCursor[" + testCursor.ToString() +
                    "] == defaultSelectionCursor[" + _defaultSelectionCursor.ToString() + "] when on top of Button", true);
                }
                else
                {
                    Verifier.Verify(testCursor == _defaultCursor, "Verifying that testCursor[" + testCursor.ToString() +
                    "] == defaultCursor[" + _defaultCursor.ToString() + "] when on top of Button", true);
                }
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields

        /// <summary>Specifies on which element the cursor property should be set</summary>
        string _targetElement = string.Empty;

        /// <summary>Specifies whether to test Cursor with Selection.</summary>
        bool _withSelection = false;

        StackPanel _panel;
        RichTextBox _rtb;
        UIElementWrapper _wrapper;
        FlowDocument _document;
        Run _run1,_run2;
        Paragraph _paragraph1,_paragraph2;
        Button _button;
        Point _hoverPoint,_defaultHoverPoint;
        Win32.HCURSOR _defaultCursor,_defaultSelectionCursor;
        bool _isDefaultCursorSet = false;

        #endregion Private fields
    }

    /// <summary>Tests  FlowDocument property in RTB</summary>
    [Test(0, "RichTextBox", "RichTextBoxFlowDocument", MethodParameters = "/TestCaseType=RichTextBoxFlowDocument")]
    [TestOwner("Microsoft"), TestTactics("685"), TestWorkItem("143"), TestBugs("673")]
    public class RichTextBoxFlowDocument : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox)
            {

                _rtb = _element as RichTextBox;
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 30;

                _otherRTB = new RichTextBox();
                _otherRTB.Document.Blocks.Clear();
                _panel = new StackPanel();
                _panel.Children.Add(_element);
                _panel.Children.Add(_otherRTB);
                TestElement = _panel;

                InitializeRTB();
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                NextCombination();
            }
        }

        private void ExecuteTrigger()
        {
            switch (_flowDocOperationsSwitch)
            {
                case FlowDocOperations.SetToNull:
                    try
                    {
                        _rtb.Document = null;
                        throw new ApplicationException("\r\n ArgumentNullException Expected when FlowDocument is set to Null~~~~~~\r\n");
                    }
                    catch (ArgumentNullException)
                    {
                        Log("\r\n~~~~~~~Exception thrown as Expected when FlowDocument is set to Null~~~~~~\r\n");
                    }
                    NextCombination();
                    break;

                case FlowDocOperations.SwitchDocs:
                    _rtb.Document = CreateFlowDocument(_initialString);
                    _rtbXaml = XamlWriter.Save(_rtb);
                    VerifyXamlContains(_rtbXaml, _initialString);
                    _rtb.Document = CreateFlowDocument(_switchString);
                    QueueDelegate(SwitchFlowDocuments);
                    break;

                case FlowDocOperations.SetProperties:
                    _rtb.Document.Background = Brushes.Brown;
                    _rtb.Document.Foreground = Brushes.Pink;
                    _rtb.Document.FontSize = 12;
                    _rtb.Document.FontWeight = FontWeights.Bold;
                    _rtb.Document.TextAlignment = TextAlignment.Center;
                    _rtb.Document.FlowDirection = FlowDirection.RightToLeft;
                    _rtb.SelectAll();
                    _rtb.Copy();

                    QueueDelegate(VerifyPropertiesInitially);

                    break;

                default:
                    break;
            }
        }

        private void SwitchFlowDocuments()
        {
            _rtbXaml = XamlWriter.Save(_rtb);
            VerifyXamlContains(_rtbXaml, _switchString);
            Verifier.Verify(_rtb.Selection.Text == "", "After switching Selection should be empty Expected [] Actual [" +
            _rtb.Selection.Text + "]", true);
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentStart);

            Verifier.Verify(_rtb.Document.ContentStart.GetOffsetToPosition(_rtb.CaretPosition) == 2, "Expected offset [2] Actual [" +
                _rtb.Document.ContentStart.GetOffsetToPosition(_rtb.CaretPosition).ToString() + "]", true);
            QueueDelegate(NextCombination);
        }

        private void VerifyPropertiesInitially()
        {
            string logString;

            _rtb.UpdateLayout();
            VerifyDocumentProps(Brushes.Pink, 12, FontWeights.Bold, TextAlignment.Center);

            Log("Pasting in otherRTB");
            _otherRTB.Paste();

            Log("Verify that the right properties are propagate after paste (to the paragraph)...");
            logString = "Actual [" + (FlowDirection)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FlowDirectionProperty) +
                "] Expected [" + FlowDirection.RightToLeft + "]";
            Verifier.Verify((FlowDirection)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FlowDirectionProperty) ==
                FlowDirection.RightToLeft, "Verifying that FlowDirection property gets copied&pasted. " + logString);

            logString = "Actual [" + (TextAlignment)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.TextAlignmentProperty) +
                "] Expected [" + TextAlignment.Center + "]";
            Verifier.Verify((TextAlignment)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.TextAlignmentProperty) ==
                TextAlignment.Center, "Verifying that TextAlignment property gets copied&pasted. " + logString);

            logString = "Actual [" + (FontWeight)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FontWeightProperty) +
                "] Expected [" + FontWeights.Bold + "]";
            Verifier.Verify((FontWeight)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FontWeightProperty) ==
                FontWeights.Bold, "Verifying that FontWeight property gets copied&pasted. " + logString);

            logString = "Actual [" + (double)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FontSizeProperty) +
                "] Expected [" + 12d + "]";
            Verifier.Verify((double)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.FontSizeProperty) ==
                12d, "Verifying that FontSize property gets copied&pasted. " + logString);

            logString = "Actual [" + ((SolidColorBrush)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.ForegroundProperty)).Color +
                "] Expected [" + Brushes.Pink.Color + "]";
            Verifier.Verify(((SolidColorBrush)_otherRTB.Document.Blocks.FirstBlock.GetValue(Block.ForegroundProperty)).Color ==
                Brushes.Pink.Color, "Verifying that Foreground property gets copied&pasted. " + logString);

            Log("Creating new FlowDocument");
            _rtb.Document = CreateFlowDocument(_switchString);
            QueueDelegate(VerifyPropertiesAfterSettingProperties);
        }

        private void VerifyPropertiesAfterSettingProperties()
        {
            VerifyDocumentProps(Brushes.Yellow, 16, FontWeights.Normal, TextAlignment.Right);
            QueueDelegate(NextCombination);
        }

        #region Helpers.

        private FlowDocument CreateFlowDocument(string data)
        {
            FlowDocument fd = new FlowDocument();
            fd.Blocks.Add(new Paragraph(new Run(data)));
            fd.Background = Brushes.Red;
            fd.Foreground = Brushes.Yellow;
            fd.FontSize = 16;
            fd.FontWeight = FontWeights.Normal;
            fd.TextAlignment = TextAlignment.Right;

            return fd;
        }

        private void InitializeRTB()
        {
            _controlWrapper.Text = _initialString;
        }

        private void VerifyXamlContains(string XamlString, string Token)
        {
            bool val = XamlString.Contains(Token);
            if (val == false)
            {
                Log(XamlString);
            }
            Verifier.Verify(val, "Xaml string Should contain [" +
                Token + "]\r\n ", true);
            int index = XamlString.IndexOf(_flowdocumentCloseTag);
            Verifier.Verify(index > -1, "Token FlowDocument shoudl exist in the XamlString Index Expected[>-1] Actual [" + index.ToString() + "]", true);
            index = XamlString.IndexOf(_flowdocumentCloseTag, index + 1);
            Verifier.Verify(index == -1, "Token FlowDocument should exist Only once in the XamlString Index Expected[==1] Actual [" + index.ToString() + "] \r\n Actual Xaml [" +
                XamlString + "]", false);

        }

        private void VerifyDocumentProps(SolidColorBrush foreground, double size,
            FontWeight Weight, TextAlignment alignment)
        {
            Verifier.Verify(_rtb.Selection.GetPropertyValue(TextElement.BackgroundProperty) == null, "Expected Background [null] Actual [" +
                _rtb.Selection.GetPropertyValue(TextElement.BackgroundProperty) + "]", true);
            Verifier.Verify(_rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty).ToString() == foreground.ToString(), "Expected Foreground [" +
                foreground.ToString() + "] Actual [" + _rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty).ToString() + "]", true);
            Verifier.Verify((double)_rtb.Selection.GetPropertyValue(TextElement.FontSizeProperty) == size, "Expected FontSize [" +
               size.ToString() + "] Actual [" + _rtb.Selection.GetPropertyValue(TextElement.FontSizeProperty).ToString() + "]", true);
            Verifier.Verify(_rtb.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() == Weight.ToString(), "Expected FontWeight [" +
               Weight.ToString() + "] Actual [" + _rtb.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() + "]", true);
            Verifier.Verify(_rtb.Selection.GetPropertyValue(Block.TextAlignmentProperty).ToString() == alignment.ToString(), "Expected TextAlignment [" +
                alignment.ToString() + "] Actual [" + _rtb.Selection.GetPropertyValue(Block.TextAlignmentProperty).ToString() + "]", true);
        }

        #endregion Helpers.

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;
        private RichTextBox _otherRTB = null;
        private StackPanel _panel = null;
        private FlowDocOperations _flowDocOperationsSwitch = 0;

        private string _initialString = "DATA";
        private string _switchString = "FLIPPED";
        private string _rtbXaml = "";
        private string _flowdocumentCloseTag = "FlowDocument></RichTextBox";

        #endregion Private Data.
    }

    /// <summary>Tests FlowDocument properties serialization during copy operations</summary>
    [Test(1, "RichTextBox", "FlowDocumentProperties", MethodParameters = "/TestCaseType=FlowDocumentProperties", Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestTactics("680"), TestWorkItem(""), TestBugs("673, 823"), TestLastUpdatedOn("May 21, 2006")]
    public class FlowDocumentProperties : ManagedCombinatorialTestCase
    {
        #region MainFlow

        /// <summary>Test case starts here</summary>
        protected override void DoRunCombination()
        {
            Log(_testProperty.Property.ToString() + ".IsInheritable: " + IsInheritableProp(_testProperty));
            Log(_testProperty.Property.ToString() + ".IsInlineFormattingProperty: " + _testProperty.IsInlineFormattingProperty);

            _run = new Run();
            _run.Text = "Sample Text";

            _para1 = new Paragraph();
            _para1.Inlines.Add(_run);

            _document = new FlowDocument();
            _document.Blocks.Add(_para1);

            _rtb = new RichTextBox();
            _rtb.Document = _document;

            //Default value for NumberSubstitution.CultureSource property is different for 
            //implicit FD and explicit FD case
            //Hence specifically set it to a default value which DependencyPropertyData expects
            _rtb.Document.SetValue(NumberSubstitution.CultureSourceProperty, NumberCultureSource.User);

            _otherRTB = new RichTextBox();

            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            _panel.Children.Add(_otherRTB);

            TestElement = _panel;

            QueueDelegate(DoCopyBeforeSettingProperty);
        }

        private void DoCopyBeforeSettingProperty()
        {
            if (_copyOnlyText)
            {
                _rtb.Selection.Select(_run.ContentStart, _run.ContentEnd);
            }
            else
            {
                _rtb.Selection.Select(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            }
            _rtb.Copy();
            _otherRTB.Document.Blocks.Clear();
            _otherRTB.Paste();

            QueueDelegate(VerifyPropertyBeforeSettingProperty);
        }

        private void VerifyPropertyBeforeSettingProperty()
        {
            Log("Verifying properties before setting test property on FD...");
            VerifyPropertiesSet(false, _copyOnlyText);

            //Set TestProperty on FlowDocument.
            _rtb.Document.SetValue(_testProperty.Property, _testProperty.TestValue);

            _rtb.Copy();
            _otherRTB.Document.Blocks.Clear();
            _otherRTB.Paste();

            QueueDelegate(VerifyPropertyAfterSettingProperty);
        }

        private void VerifyPropertyAfterSettingProperty()
        {
            Log("Verifying properties after setting test property on FD...");
            VerifyPropertiesSet(true, _copyOnlyText);

            QueueDelegate(NextCombination);
        }

        #endregion MainFlow

        #region Helpers

        private bool IsInheritableProp(DependencyPropertyData dpData)
        {
            if (dpData.IsInheritable)
            {
                return true;
            }
            PropertyMetadata propMetadata = dpData.Property.GetMetadata(typeof(FrameworkElement));
            if ((propMetadata is FrameworkPropertyMetadata) && (((FrameworkPropertyMetadata)propMetadata).Inherits))
            {
                return true;
            }

            propMetadata = dpData.Property.GetMetadata(typeof(FrameworkContentElement));
            if ((propMetadata is FrameworkPropertyMetadata) && (((FrameworkPropertyMetadata)propMetadata).Inherits))
            {
                return true;
            }

            return false;
        }

        private void VerifyPropertiesSet(bool checkWithTestValues, bool onRun)
        {
            Run otherRun;
            Paragraph otherParagraph;

            otherParagraph = (Paragraph)_otherRTB.Document.Blocks.FirstBlock;
            otherRun = (Run)otherParagraph.Inlines.FirstInline;

            //Verify TestValue when the property is Inheritable
            if ((checkWithTestValues) && (IsInheritableProp(_testProperty)))
            {
                if (onRun) //If only text is copied, look for test property value on Run
                {
                    if (_testProperty.IsInlineFormattingProperty)
                    {
                        Verifier.VerifyValue(_testProperty.Property.ToString(), _testProperty.TestValue, otherRun.GetValue(_testProperty.Property));
                    }
                    else
                    {
                        Verifier.VerifyValueDifferent(_testProperty.Property.ToString(), _testProperty.TestValue, otherRun.GetValue(_testProperty.Property));
                    }
                }
                else //if all contents are copied, look for test property value on paragraph
                {
                    if (!(_testProperty.Property.Name.Contains("IsHyphenationEnabled")))
                    {
                        Verifier.VerifyValue(_testProperty.Property.ToString(), _testProperty.TestValue, otherParagraph.GetValue(_testProperty.Property));
                    }
                }
            }
            else
            {
                //Case is Property is not inheritable or Test property value is not yet set.
                //Verify that you dont find the test property value on Run and Paragraph.

                Verifier.VerifyValueDifferent(_testProperty.Property.ToString(), _testProperty.TestValue, otherRun.GetValue(_testProperty.Property));
                Verifier.VerifyValueDifferent(_testProperty.Property.ToString(), _testProperty.TestValue, otherParagraph.GetValue(_testProperty.Property));
            }
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _rtb,_otherRTB;
        private StackPanel _panel;
        private FlowDocument _document;
        private Paragraph _para1;
        private Run _run;

        /// <summary>FlowDocument's DependencyPropertyData being tested</summary>
        private DependencyPropertyData _testProperty = null;

        /// <summary>Whether to copy only text or all contents</summary>
        private bool _copyOnlyText = false;

        #endregion Private fields
    }

    /// <summary>Tests  tabbing in RTB</summary>
    [Test(0, "RichTextBox", "RichTextBoxTabIndentation", MethodParameters = "/TestCaseType:RichTextBoxTabIndentation", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("679"), TestWorkItem("142"), TestBugs("5")]
    public class RichTextBoxTabIndentation : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox)
            {

                _rtb = _element as RichTextBox;
                _rtb.AcceptsTab = true;
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 30;
                TestElement = _element;
                InitializeRTB();
                _tabWidth = _tabCount = 0;
                _currentParaNumber = 0;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>Control programmer</summary>
        private void DoFocus()
        {
            MainWindow.Topmost = true;
            MouseInput.MouseClick(MainWindow);
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary>Control programmer</summary>
        private void ExecuteTrigger()
        {
            _rtb.Focus();
            _currentParaNumber = 1;
            switch (_tabOperationsSwitch)
            {
                case TabOperations.NoSelection:
                    TabbingOnPara();
                    break;

                case TabOperations.SelectAll:
                    _rtb.SelectAll();
                    TabbingWithSelectAll();
                    break;

                case TabOperations.SelectIndividual:
                    TabbingOnParaWithSelection();
                    break;

                default:
                    break;
            }
        }

        /// <summary>Tab on paras</summary>
        private void TabbingOnPara()
        {
            LogicForPressingTab();
            QueueDelegate(VerificationHelperForTab);
        }

        /// <summary>Verify margin and indent when pressing tab</summary>
        private void VerificationHelperForTab()
        {
            _tabCount++;
            TextPointer _tp = _rtb.CaretPosition;
            Paragraph _para = _tp.Paragraph;
            if (_tabCount == 1)
            {
                //First tab increases the indent not the margin
                VerifyParaIndent(_para);
            }
            else
            {
                //after tabbing once, then margin increases
                VerifyParaMargin(_para);
            }

            //if tab is not pressed 3 times yet then press tab
            if (_tabCount < 3)
            {
                QueueDelegate(TabbingOnPara);
            }
            else //if tab is pressed 3 times, then start pressing backspace
            {
                QueueDelegate(BackSpaceOnPara);

            }
        }

        /// <summary>Press Backspace</summary>
        private void BackSpaceOnPara()
        {
            KeyboardInput.TypeString("{BACKSPACE}");

            QueueDelegate(VerificationHelperForBackSpace);
        }

        /// <summary>Press Backspace</summary>
        private void VerificationHelperForBackSpace()
        {
            _tabCount--;
            TextPointer _tp = _rtb.CaretPosition;
            Paragraph _para = _tp.Paragraph;
            if (_tabCount >= 0)
            {
                //First tab removes the indent
                VerifyParaIndentIsZeroAndParaMarginReduces(_para);
            }
            if (_tabCount > 0)
            {
                //if backspaxce is not pressed 3 times yet press backspace
                QueueDelegate(BackSpaceOnPara);
            }
            else
            {
                _tabCount = 0;
                //There are 3 paras - so we need to iterate 3 times.
                if (_currentParaNumber < 3)
                {
                    _tabCount = 0;
                    _currentParaNumber++;
                    //If its the 3rd para then its a BUIC, so there is no indentation
                    if (_currentParaNumber == 3)
                    {
                        QueueDelegate(TabbingOnBUIC);
                    }
                    else
                    {
                        QueueDelegate(TabbingOnPara);
                    }
                }
            }
        }

        /// <summary>Press tab on a BUIC</summary>
        private void TabbingOnBUIC()
        {
            LogicForPressingTab();
            QueueDelegate(VerificationHelperForTabOnBUIC);
        }

        /// <summary>Verification of Pressing tab on a BUIC</summary>
        private void VerificationHelperForTabOnBUIC()
        {
            _tabCount++;
            TextPointer _tp = _rtb.CaretPosition;
            BlockUIContainer _buic = _tp.Parent as BlockUIContainer;
            VerifyBUICMargin(_buic);
            if (_tabCount < 3)
            {
                QueueDelegate(TabbingOnBUIC);
            }
            else
            {
                QueueDelegate(BackSpaceOnBUIC);
            }
        }

        /// <summary>Press Bckspace on a BUIC</summary>
        private void BackSpaceOnBUIC()
        {
            KeyboardInput.TypeString("{BACKSPACE}");
            QueueDelegate(VerificationHelperForBackSpaceOnBUIC);
        }

        /// <summary>Verification of backspace on a BUIC</summary>
        private void VerificationHelperForBackSpaceOnBUIC()
        {
            _tabCount--;
            TextPointer _tp = _rtb.CaretPosition;
            BlockUIContainer _buic = _tp.Parent as BlockUIContainer;
            VerifyBUICMargin(_buic);
            if (_tabCount > 0)
            {
                QueueDelegate(BackSpaceOnBUIC);
            }
            else
            {
                QueueDelegate(NextCombination);
            }

        }

        /// <summary>Press tab on a para which is selected</summary>
        private void TabbingOnParaWithSelection()
        {
            LogicForPressingTab();
            QueueDelegate(VerificationHelperForTabWithSelection);
        }

        /// <summary>VerificationHelperForTabWithSelection</summary>
        private void VerificationHelperForTabWithSelection()
        {
            _tabCount++;
            TextPointer _tp = _rtb.CaretPosition;
            TextElement t = _tp.Parent as TextElement;
            while (!(t is Block))
            {
                t = t.Parent as TextElement;
            }
            Block _block = t as Block;
            VerifyBlockMargin(_block);
            if (_tabCount < 3)
            {
                QueueDelegate(TabbingOnParaWithSelection);
            }
            else
            {
                QueueDelegate(ShiftTabOnParaWithSelection);
            }
        }

        /// <summary>ShiftTabOnParaWithSelection</summary>
        private void ShiftTabOnParaWithSelection()
        {
            KeyboardInput.TypeString("+{TAB}");
            QueueDelegate(VerificationHelperForShiftTabOnBlock);
        }

        /// <summary>VerificationHelperForShiftTabOnBlock</summary>
        private void VerificationHelperForShiftTabOnBlock()
        {
            _tabCount--;
            TextPointer _tp = _rtb.CaretPosition;
            TextElement t = _tp.Parent as TextElement;
            while (!(t is Block))
            {
                t = t.Parent as TextElement;
            }
            Block _block = t as Block;
            VerifyBlockMargin(_block);
            if (_tabCount > 0)
            {
                QueueDelegate(ShiftTabOnParaWithSelection);
            }
            else
            {
                _tabCount = 0;

                if (_currentParaNumber < 3)
                {
                    _tabCount = 0;
                    _currentParaNumber++;
                    QueueDelegate(TabbingOnParaWithSelection);
                }
                else
                {
                    QueueDelegate(NextCombination);
                }
            }
        }

        /// <summary>TabbingWithSelectAll</summary>
        private void TabbingWithSelectAll()
        {
            KeyboardInput.TypeString("{TAB}");
            QueueDelegate(VerificationHelperForTabWithSelectAll);
        }

        /// <summary>VerificationHelperForTabWithSelectAll</summary>
        private void VerificationHelperForTabWithSelectAll()
        {
            _tabCount++;

            HelperForSelectAllVerification();

            if (_tabCount < 3)
            {
                QueueDelegate(TabbingWithSelectAll);
            }
            else
            {
                QueueDelegate(ShiftTabWithSelectAll);
            }
        }

        /// <summary>ShiftTabWithSelectAll</summary>
        private void ShiftTabWithSelectAll()
        {
            KeyboardInput.TypeString("+{TAB}");
            QueueDelegate(VerificationHelperForShiftTabWithSelectAll);
        }

        /// <summary>VerificationHelperForShiftTabWithSelectAll</summary>
        private void VerificationHelperForShiftTabWithSelectAll()
        {
            _tabCount--;

            HelperForSelectAllVerification();

            if (_tabCount > 0)
            {
                QueueDelegate(ShiftTabWithSelectAll);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        #region Helpers.

        /// <summary>VerifyParaMargin</summary>
        private void VerifyParaMargin(Paragraph p)
        {
            double _expectedTabWidth = _tabWidth * (_tabCount - 1);
            double _actualTabWidth = (_currentParaNumber == 2) ? p.Margin.Right : p.Margin.Left;

            Verifier.Verify(_actualTabWidth == _expectedTabWidth, "Paragraph Margin Expected [" + _expectedTabWidth.ToString() +
                "] Actual [" + _actualTabWidth.ToString() + "]", true);
            Verifier.Verify(p.TextIndent == _tabWidth, "TextIndent should be equal to [" + _tabWidth.ToString() +
                   "] Actual [" + p.TextIndent.ToString() + "]", true);
        }

        /// <summary>VerifyParaIndent</summary>
        private void VerifyParaIndent(Paragraph p)
        {
            if (_tabWidth == 0)
            {
                Verifier.Verify(p.TextIndent > 0, "TextIndent should be greater than 0 Actual [" + p.TextIndent.ToString() + "]", true);
                _tabWidth = (_tabWidth == 0) ? p.TextIndent : _tabWidth;
            }
            else
            {
                Verifier.Verify(p.TextIndent == _tabWidth, "TextIndent should be equal to [" + _tabWidth.ToString() +
                    "] Actual [" + p.TextIndent.ToString() + "]", true);
            }
        }

        /// <summary>VerifyParaIndentIsZeroAndParaMarginReduces</summary>
        private void VerifyParaIndentIsZeroAndParaMarginReduces(Paragraph p)
        {
            Verifier.Verify(p.TextIndent == 0, "TextIndent should be == 0 on doing BackSpace Actual [" + p.TextIndent.ToString() + "]", true);

            double _expectedTabWidth = _tabWidth * (_tabCount);
            double _actualTabWidth = (_currentParaNumber == 2) ? p.Margin.Right : p.Margin.Left;
            Verifier.Verify(_actualTabWidth == _expectedTabWidth, "Paragraph Margin Expected [" + _expectedTabWidth.ToString() +
                "] Actual [" + _actualTabWidth.ToString() + "]", true);
        }

        /// <summary>VerifyParaMarginReduces</summary>
        private void VerifyParaMarginReduces(Paragraph p)
        {
            Verifier.Verify(p.TextIndent == 0, "TextIndent should be == 0 on doing BackSpace Actual [" + p.TextIndent.ToString() + "]", true);
            double _expectedTabWidth = _tabWidth * (_tabCount - 1);
            double _actualTabWidth = (_currentParaNumber == 2) ? p.Margin.Right : p.Margin.Left;

            Verifier.Verify(_actualTabWidth == _expectedTabWidth, "Paragraph Margin Expected [" + _expectedTabWidth.ToString() +
                "] Actual [" + _actualTabWidth.ToString() + "]", true);
        }

        /// <summary>VerifyBUICMargin</summary>
        private void VerifyBUICMargin(BlockUIContainer _buic)
        {
            double _expectedTabWidth = _tabWidth * (_tabCount);
            double _actualTabWidth = _buic.Margin.Left;
            Verifier.Verify(_actualTabWidth == _expectedTabWidth, "BUIC Margin Expected [" + _expectedTabWidth.ToString() +
                "] Actual [" + _actualTabWidth.ToString() + "]", true);
        }

        /// <summary>VerifyBlockMargin</summary>
        private void VerifyBlockMargin(Block _block)
        {
            double _expectedTabWidth = _tabWidth * (_tabCount);
            double _actualTabWidth = (_currentParaNumber == 2) ? _block.Margin.Right : _block.Margin.Left;

            if (_tabWidth == 0)
            {
                Verifier.Verify(_actualTabWidth > _expectedTabWidth, "Block Margin Expected >[" + _expectedTabWidth.ToString() +
                               "] Actual [" + _actualTabWidth.ToString() + "]", true);
                _tabWidth = _actualTabWidth;
            }
            else
            {
                Verifier.Verify(_actualTabWidth == _expectedTabWidth, "Block Margin Expected [" + _expectedTabWidth.ToString() +
                    "] Actual [" + _actualTabWidth.ToString() + "]", true);
            }
        }

        /// <summary>HelperForSelectAllVerification</summary>
        private void HelperForSelectAllVerification()
        {
            Block _block = _rtb.Document.Blocks.FirstBlock;
            VerifyBlockMargin(_block);
            TextElementCollection<Block> coll = _rtb.Document.Blocks;
            IEnumerator enumerator = coll.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            _block = enumerator.Current as Block;
            _currentParaNumber = 2;
            VerifyBlockMargin(_block);
            _block = _rtb.Document.Blocks.LastBlock;
            _currentParaNumber++;
            VerifyBlockMargin(_block);
        }

        /// <summary>SetCaretPosition</summary>
        private TextPointer SetCaretPosition(int _paraNumber)
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            TextPointer _start = tr.Start;
            int count = 2;
            while ((_paraNumber > 1) && (count <= _totalNumberOfParas))
            {
                _start = _start.GetNextInsertionPosition(LogicalDirection.Forward);
                _start = _start.GetNextInsertionPosition(LogicalDirection.Forward);
                if (count == _paraNumber) break;
                count++;
            }
            return _start;
        }

        /// <summary>SetSelection</summary>
        private void SetSelection(TextPointer _start, int _numberOfCharsSelected)
        {
            TextPointer _end = _start;
            if (_numberOfCharsSelected == 0)
            {
                return;
            }
            else
            {
                _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                _rtb.Selection.Select(_start, _end);
                return;
            }
        }

        /// <summary>InitializeRTB</summary>
        private void InitializeRTB()
        {
            _rtb.Document.Blocks.Clear();
            if (_tabOperationsSwitch == TabOperations.SelectAll)
            {
                _rtb.Document.Blocks.Add(new Paragraph());
            }
            else
            {
                _rtb.Document.Blocks.Add(new Paragraph(new Run(_firstParaData)));
            }
            Paragraph p = new Paragraph(new Run(_secParaData));
            p.FlowDirection = FlowDirection.RightToLeft;
            _rtb.Document.Blocks.Add(p);
            Thickness t = new Thickness();
            t.Top = t.Bottom = t.Right = 0;
            BlockUIContainer buic = new BlockUIContainer(new Button());
            buic.Margin = t;
            _rtb.Document.Blocks.Add(buic);
        }

        /// <summary>LogicForPressingTab</summary>
        private void LogicForPressingTab()
        {
            if (_tabCount == 0)
            {
                _rtb.CaretPosition = SetCaretPosition(_currentParaNumber);
                if (_tabOperationsSwitch == TabOperations.NoSelection)
                {
                    SetSelection(_rtb.CaretPosition, 0);
                }
                else
                {
                    SetSelection(_rtb.CaretPosition, 1);
                }
            }
            KeyboardInput.TypeString("{TAB}");
        }

        #endregion Helpers.

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;
        private TabOperations _tabOperationsSwitch = 0;

        private string _firstParaData = "A";
        private string _secParaData = "B";
        private int _totalNumberOfParas = 3;
        private int _tabCount = 0;
        private double _tabWidth = 0;
        private int _currentParaNumber = 0;

        #endregion Private Data.
    }

    /// <summary>
    /// Verifies RTB regressions bugs
    /// Regression_Bug176 - After RTB is rendered, setting Forground color does not change color of content until some operation is performed on the content
    /// Regression_Bug177 - Editing commands should apply FlowDirection property to any parent List(s) when FD is applied to paragraphs
    /// Regression_Bug178 - Infinite loop when applying Control+B to a link with Image
    /// </summary>
    [Test(0, "RichTextBox", "RichTextBoxRegressions", MethodParameters = "/TestCaseType:RichTextBoxRegressions")]
    [TestOwner("Microsoft"), TestTactics("29"), TestBugs("176,177,178"), TestWorkItem("141")]
    public class RichTextBoxRegressions : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _stackPanel = new StackPanel();
            MainWindow.Content = _stackPanel;

            QueueDelegate(ExecuteTrigger);
        }

        private void ExecuteTrigger()
        {
            if (_currentBugIndex < _bugNumbers.Length)
            {
                switch (_bugNumbers[_currentBugIndex])
                {
                    case 176:
                        InitializeRegression_Bug176();
                        QueueDelegate(CaptureInitialImageRegression_Bug176);
                        break;

                    case 177:
                        InitializeRegression_Bug177();
                        break;

                    case 178:
                        InitializeRegression_Bug178();
                        break;

                    default:
                        break;
                }
            }
            else
            {
                if (_success)
                {
                    Logger.Current.ReportSuccess();
                }
                else
                {
                    Logger.Current.ReportResult(false, _failedString);
                }

            }
        }

        #region Regression_Bug176.

        private void InitializeRegression_Bug176()
        {
            _control = new RichTextBox();
            ((TextBoxBase)_control).FontSize = 40;
            ((TextBoxBase)_control).FontWeight = FontWeights.Bold;
            _controlWrapper = new UIElementWrapper(_control);
            ((Paragraph)((RichTextBox)_control).Document.Blocks.FirstBlock).Inlines.Add(new Run("HELLO"));
            _controlWrapper = new UIElementWrapper(_control);
            _stackPanel.Children.Add(_control);
        }

        private void CaptureInitialImageRegression_Bug176()
        {
            _firstImage = BitmapCapture.CreateBitmapFromElement(_control);
            _firstImage = BitmapUtils.CreateBorderlessBitmap(_firstImage, 3);
            QueueDelegate(ChangeForeground);
        }

        private void ChangeForeground()
        {
            ((TextBoxBase)_control).Foreground = Brushes.Yellow;
            QueueDelegate(GetImagesAfterChangeInForeground);
        }

        private void GetImagesAfterChangeInForeground()
        {
            _secImage = BitmapCapture.CreateBitmapFromElement(_control);
            _secImage = BitmapUtils.CreateBorderlessBitmap(_secImage, 3);
            QueueDelegate(VerifyChangeInColor);
        }

        private void VerifyChangeInColor()
        {
            bool _equal = ComparisonOperationUtils.AreBitmapsEqual(_firstImage, _secImage, out _diffImage);
            if (_equal == true)
            {
                Logger.Current.LogImage(_firstImage, "_firstImage");
                Logger.Current.LogImage(_secImage, "_secImage");
                Logger.Current.LogImage(_diffImage, "_Differences");
                Logger.Current.ReportResult(false, "Regression_Bug176 REGRESSED", true);

                _success = false;
                _failedString = _bugNumbers[_currentBugIndex].ToString() + "FAILED\r\n";
            }
            Log("\r\n----------------" + _bugNumbers[_currentBugIndex].ToString() + " DONE --------------------------------\r\n");
            _currentBugIndex++;
            QueueDelegate(ExecuteTrigger);
        }

        #endregion Regression_Bug176.

        #region Regression_Bug177.

        private int _inputSwitch = 0;

        private void InitializeRegression_Bug177()
        {
            _stackPanel.Children.Clear();
            _control = new RichTextBox();
            _rtb = _control as RichTextBox;
            ((TextBoxBase)_control).FontSize = 40;
            ((TextBoxBase)_control).FontWeight = FontWeights.Bold;
            _controlWrapper = new UIElementWrapper(_control);
            List _list = new List();
            ListItem l1 = new ListItem();
            ListItem l2 = new ListItem();
            _list.ListItems.Add(l1);
            _list.ListItems.Add(l2);
            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(_list);
            _controlWrapper = new UIElementWrapper(_control);
            _stackPanel.Children.Add(_control);
            MouseInput.MouseClick(_rtb);

            QueueDelegate(CaptureInitialImageRegression_Bug177);
        }

        private void CaptureInitialImageRegression_Bug177()
        {
            _rtb.Focus();
            _firstImage = BitmapCapture.CreateBitmapFromElement(_control);
            _firstImage = BitmapUtils.CreateBorderlessBitmap(_firstImage, 3);
            if (_inputSwitch == 1)
            {
                KeyboardInput.TypeString("^{Home}^A");
            }
            QueueDelegate(ChangeFlowDirection);
        }

        private void ChangeFlowDirection()
        {
            _rtb.Focus();
            if (_inputSwitch == 0)
            {
                _rtb.FlowDirection = FlowDirection.RightToLeft;
                QueueDelegate(GetImageAfterChangeInFlowDirection);
            }
            else
            {
                KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.ControlShiftRight);
                data[0].PerformAction(_controlWrapper, null);

                QueueDelegate(RemoveFocus);
            }
        }

        private void RemoveFocus()
        {
            KeyboardInput.TypeString("{UP}");
            _rtb.IsReadOnly = true;
            QueueDelegate(GetImageAfterChangeInFlowDirection);
        }

        private void GetImageAfterChangeInFlowDirection()
        {
            _rtb.IsReadOnly = true;
            _secImage = BitmapCapture.CreateBitmapFromElement(_control);
            _secImage = BitmapUtils.CreateBorderlessBitmap(_secImage, 3);
            QueueDelegate(VerifyChangeInFlowDirection);
        }

        private void VerifyChangeInFlowDirection()
        {
            bool _equal = ComparisonOperationUtils.AreBitmapsEqual(_firstImage, _secImage, out _diffImage);
            if (_equal == true)
            {
                Logger.Current.LogImage(_firstImage, "_firstImage");
                Logger.Current.LogImage(_secImage, "_secImage");
                if (_diffImage != null)
                    Logger.Current.LogImage(_diffImage, "_Differences");
                Logger.Current.ReportResult(false, "Regression_Bug177 REGRESSED", true);

                _success = false;
                _failedString = _bugNumbers[_currentBugIndex].ToString() + "FAILED\r\n";
            }
            if ((_inputSwitch == 0) && (KeyboardInput.IsBidiInputLanguageInstalled()))
            {
                _inputSwitch++;
                QueueDelegate(InitializeRegression_Bug177);
            }
            else
            {
                Log("\r\n----------------" + _bugNumbers[_currentBugIndex].ToString() + " DONE --------------------------------\r\n");

                _currentBugIndex++;
                QueueDelegate(ExecuteTrigger);
            }
        }

        #endregion Regression_Bug177.        

        #region Regression_Bug178

        private void InitializeRegression_Bug178()
        {
            _stackPanel.Children.Clear();

            Image linkImage = new Image();
            linkImage.Height = linkImage.Width = 50;
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new FileStream("colors.png", FileMode.Open);
            bitmapImage.EndInit();
            linkImage.Source = bitmapImage;

            Hyperlink link = new Hyperlink();
            link.NavigateUri = new Uri("http://www.live.com");
            link.Inlines.Add(new InlineUIContainer(linkImage));

            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run("This is some text "));
            para.Inlines.Add(link);

            _rtb = new RichTextBox();
            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(para);

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _rtb.Focus();
            QueueDelegate(DoSelectAll);
        }

        private void DoSelectAll()
        {
            _rtb.SelectAll();
            QueueDelegate(DoBold);
        }

        private void DoBold()
        {
            EditingCommands.ToggleBold.Execute(null, _rtb);
            QueueDelegate(VerifyBold);
        }

        private void VerifyBold()
        {
            TextPointer tp = _rtb.Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
            Verifier.Verify((FontWeight)tp.Parent.GetValue(TextElement.FontWeightProperty) == FontWeights.Bold,
                "Verifying that bold is applied on a selection with Hyperlink which has a InlineUIContainer", true);

            Log("\r\n----------------" + _bugNumbers[_currentBugIndex].ToString() + " DONE --------------------------------\r\n");
            _currentBugIndex++;
            QueueDelegate(ExecuteTrigger);
        }

        #endregion

        #endregion Main flow.

        #region Private fields.

        private UIElement _control;
        private RichTextBox _rtb;
        UIElementWrapper _controlWrapper = null;
        private System.Drawing.Bitmap _firstImage = null;
        private System.Drawing.Bitmap _secImage = null;
        private System.Drawing.Bitmap _diffImage = null;
        private StackPanel _stackPanel;
        private int _currentBugIndex = 0;
        private bool _success = true;
        private string _failedString = "";

        private int[] _bugNumbers ={ 176, 177, 178 };

        #endregion Private fields.
    }

    /// <summary>Tests  keyboard navigation on BUIC</summary>
    [Test(0, "RichEditing", "RichTextBoxBUICNavigation", MethodParameters = "/TestCaseType:RichTextBoxBUICNavigation")]
    [TestOwner("Microsoft"), TestTactics("681"), TestWorkItem("140")]
    public class RichTextBoxBUICNavigation : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox || _element is RichTextBoxSubClass)
            {

                _rtb = _element as RichTextBox;
                _rtb.AcceptsTab = true;
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 30;
                TestElement = _element;
                _actionCount = 0;
                InitializeRTB();
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        /// <summary>program controller</summary>
        private void ExecuteTrigger()
        {
            _element.Focus();
            if (_actionCount == _keyActions.Length)
            {
                QueueDelegate(NextCombination);
            }
            else
            {
                QueueDelegate(PressKeyboardAction);
            }
        }

        /// <summary>keyboard action</summary>
        private void PressKeyboardAction()
        {
            Log("\r\n********************Keyboard action:" + _keyActions[_actionCount] + "********************\r\n");
            KeyboardInput.TypeString(_keyActions[_actionCount]);
            QueueDelegate(CheckCaretPosition);
        }

        /// <summary>check caret is correctly placed</summary>
        private void CheckCaretPosition()
        {
            _expectedCaretPosition = GetExpectedCaretPosition();
            VerifyCaretPositionInCorrectPara();
            _actionCount++;
            QueueDelegate(ExecuteTrigger);
        }

        #region helpers.

        /// <summary>VerifyCaretPositionInCorrectPara</summary>
        private void VerifyCaretPositionInCorrectPara()
        {
            TextRange tr = new TextRange(_rtb.CaretPosition, _rtb.CaretPosition);
            int offsetVal = _rtb.Document.ContentStart.GetOffsetToPosition(tr.Start);
            int expectedOffsetVal = _rtb.Document.ContentStart.GetOffsetToPosition(_expectedCaretPosition);

            Verifier.Verify(expectedOffsetVal == offsetVal, "Offset Expected [" + expectedOffsetVal.ToString() +
                "] Actual [" + offsetVal.ToString() + "]", true);
        }

        /// <summary>GetExpectedCaretPosition</summary>
        private TextPointer GetExpectedCaretPosition()
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentStart);
            TextPointer tp = tr.Start;
            int paraNumber = _paraNumber[_actionCount];

            for (int i = 0; i < (paraNumber - 1); i++)
            {
                tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            if (_caretPosition[_actionCount] == 1)
            {
                tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            return tp;
        }

        /// <summary>InitializeRTB</summary>
        private void InitializeRTB()
        {
            _rtb.Document.Blocks.Clear();

            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            TextPointer tp = tr.Start;

            Image simpleImage = new Image();
            simpleImage.Height = 200;
            simpleImage.Margin = new Thickness(5);

            System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"pack://siteoforigin:,,,/test.png");
            bi.EndInit();
            simpleImage.Source = bi;

            BlockUIContainer buicImage = new BlockUIContainer(simpleImage);
            _rtb.Document.Blocks.Add(buicImage);
            BlockUIContainer buic1 = new BlockUIContainer(new Button());
            _rtb.Document.Blocks.Add(buic1);
            BlockUIContainer buic2 = new BlockUIContainer(new Button());
            _rtb.Document.Blocks.Add(buic2);
            BlockUIContainer buic3 = new BlockUIContainer(new Button());
            _rtb.Document.Blocks.Add(buic3);
            _rtb.Height = 200;
        }

        #endregion helpers.

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;

        private string[] _keyActions ={ "^{HOME}", "^{END}", "{PGUP}", "{PGDN}", "{HOME}", "{PGUP}", "{END}", "{DOWN}", "{LEFT}", "{UP}", "{RIGHT}" };
        //this is 1 based array
        private int[] _paraNumber = { 1, 4, 1, 4, 4, 1, 1, 2, 2, 1, 1 };
        //0 means caret is on the leftmost side 
        //1 means caret is on the right most side
        private int[] _caretPosition = { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1 };
        private int _actionCount = 0;
        private TextPointer _expectedCaretPosition;

        #endregion Private Data.
    }

    /// <summary>Tests  delete backspace operations on BUIC</summary>
    [Test(2, "RichEditing", "RichTextBoxBUICBackSpaceDelete", MethodParameters = "/TestCaseType:RichTextBoxBUICBackSpaceDelete")]
    [TestOwner("Microsoft"), TestTactics("678"), TestWorkItem("139"), TestBugs("824, 825")]
    public class RichTextBoxBUICBackSpaceDelete : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox || _element is RichTextBoxSubClass)
            {

                _rtb = _element as RichTextBox;
                _rtb.AcceptsTab = true;
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 30;
                TestElement = _element;
                InitializeRTB();
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private void ExecuteTrigger()
        {
            _rtb.Focus();
            switch (_buicOperationsSwitch)
            {
                case BUICOperations.SelectAllDelete:
                    _rtb.SelectAll();
                    KeyboardInput.TypeString("{DELETE}");
                    _expectedString = "";
                    QueueDelegate(VerifyEmptyContents);
                    break;

                case BUICOperations.BackspaceAfterBUIC:
                    KeyboardInput.TypeString("^{HOME}{RIGHT}{BACKSPACE}");
                    _expectedString = "A\r\n";
                    QueueDelegate(VerifyEmptyContents);
                    break;

                case BUICOperations.BackspacesFromNextPara:
                    KeyboardInput.TypeString("^{HOME}{RIGHT 2}{BACKSPACE}{BACKSPACE}");
                    _expectedString = "A\r\n";
                    QueueDelegate(VerifyEmptyContents);
                    break;

                case BUICOperations.OvertypingBUICSelected:
                    KeyboardInput.TypeString("^{HOME}+{RIGHT}A");
                    _expectedString = "AA\r\n";
                    QueueDelegate(VerifyEmptyContents);
                    break;

                default:
                    break;
            }
        }

        private void VerifyEmptyContents()
        {
            string str = _expectedString;
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            Verifier.Verify(tr.Text == str, "Expected Contents[" + str + "]  Actual [" + tr.Text + "]", true);
            QueueDelegate(NextCombination);
        }

        #region Helpers.

        /// <summary>InitializeRTB</summary>
        private void InitializeRTB()
        {
            _rtb.Document.Blocks.Clear();
            BlockUIContainer buic = new BlockUIContainer(new Button());
            _rtb.Document.Blocks.Add(buic);
            _rtb.Document.Blocks.Add(new Paragraph(new Run("A")));
        }

        #endregion Helpers.

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;
        private BUICOperations _buicOperationsSwitch = 0;
        string _expectedString = "";

        #endregion Private Data.
    }

    /// <summary>Tests  tabs gets correctly rendered</summary>
    [Test(0, "RichEditing", "TestTabsAreCorrectlyRendered", MethodParameters = "/TestCaseType:TestTabsAreCorrectlyRendered")]
    [TestOwner("Microsoft"), TestTactics("674"), TestWorkItem("138"), TestBugs("826, 827"), TestLastUpdatedOn("May 2, 2006")]
    public class TestTabsAreCorrectlyRendered : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(_element);
                ((TextBoxBase)_element).FontSize = 30;
                ((TextBoxBase)_element).AcceptsTab = true;
                ((TextBoxBase)_element).FlowDirection = _controlFlowDirection;
                _element.Height = _element.Width = 400;
                TestElement = _element;

                DockPanel dp = MainWindow.Content as DockPanel;
                dp.Children.Clear();
                _button = new Button();
                _button.Content = "Clicked to remove focus from Control for Image Capture";
                dp.Children.Add(_element);
                dp.Children.Add(_button);
                _controlWrapper.Text = "DATA";
                QueueDelegate(CaptureImage);
            }
        }

        private void CaptureImage()
        {
            _button.Focus();
            _initial = BitmapCapture.CreateBitmapFromElement(_element);
            _element.Focus();
            QueueDelegate(TypeInOneTabs);
        }

        private void TypeInOneTabs()
        {
            KeyboardInput.TypeString("{HOME}{TAB}");
            QueueDelegate(CaptureIntermediateImage);
        }

        private void CaptureIntermediateImage()
        {
            _button.Focus();
            _final = BitmapCapture.CreateBitmapFromElement(_element);
            QueueDelegate(VerifyFirstTabLocation);
        }

        private void VerifyFirstTabLocation()
        {
            ComparisonCriteria _criteria = new ComparisonCriteria();
            _criteria.MaxColorDistance = 0.01f;
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_initial, _final, _criteria, false) == true)
            {
                Log("Images are same - Should be different after first tab - Images are _initial/_final");
                Logger.Current.LogImage(_initial, "_initial");
                Logger.Current.LogImage(_final, "_final");
                Logger.Current.ReportResult(false, "FAILED COMBINATION", false);
            }
            else
            {
                _initial = _final;
                _element.Focus();
                KeyboardInput.TypeString("{HOME}{TAB}");
                QueueDelegate(CaptureFinalImage);
            }
        }

        private void CaptureFinalImage()
        {
            _button.Focus();
            _final = BitmapCapture.CreateBitmapFromElement(_element);
            QueueDelegate(VerifyTabLocation);
        }

        private void VerifyTabLocation()
        {
            ComparisonCriteria _criteria = new ComparisonCriteria();
            _criteria.MaxColorDistance = 0.01f;

            VerifyMarginAndIndents();
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_initial, _final, _criteria, false) == true)
            {
                Log("Images are same - Should be different after second tab - Images are _initial/_final");
                Logger.Current.LogImage(_initial, "_initial");
                Logger.Current.LogImage(_final, "_final");

                Logger.Current.ReportResult(false, "FAILED COMBINATION", false);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }


        #region helper.

        /// <summary>VerifyMarginAndIndents</summary>
        private void VerifyMarginAndIndents()
        {
            double _textIndent = 20.00;
            double _margin = (_element is TextBox) ? (0) : (_textIndent);

            double _actualMargin;
            if (_element is TextBox)
            {
                _actualMargin = (_controlFlowDirection == FlowDirection.RightToLeft) ? ((TextBoxBase)_element).Margin.Right : ((TextBoxBase)_element).Margin.Left;
                Verifier.Verify(_margin == _actualMargin, "Margins should be equal Expected [" + _margin.ToString() + "] Actual [" +
                    _actualMargin.ToString() + "]", true);
            }
            else
            {
                double _actualIndent = ((Paragraph)((RichTextBox)_element).Document.Blocks.FirstBlock).TextIndent;
                _actualMargin = ((Paragraph)((RichTextBox)_element).Document.Blocks.FirstBlock).Margin.Left;
                Verifier.Verify(_textIndent == _actualIndent, "Indents should be equal Expected [" + _textIndent.ToString() + "] Actual [" +
                    _actualIndent.ToString() + "]", true);
                Verifier.Verify(_margin == _actualMargin, "Margins should be equal Expected [" + _margin.ToString() + "] Actual [" +
                     _actualMargin.ToString() + "]", true);
            }
        }

        #endregion helper.

        #region data.

        private UIElementWrapper _controlWrapper = null;
        private TextEditableType _editableType = null;
        private FrameworkElement _element = null;
        private FlowDirection _controlFlowDirection = FlowDirection.LeftToRight;

        private System.Drawing.Bitmap _initial, _final;
        private Button _button;

        #endregion data.
    }

    /// <summary>Tests  HyphenationEnabled</summary>
    [Test(1, "RichEditing", "TestHyphenationEnabled", MethodParameters = "/TestCaseType:TestHyphenationEnabled ",Versions="3.0SP1,3.0SP2") ]
    [TestOwner("Microsoft"), TestTactics("675"), TestBugs("826, 827"), TestLastUpdatedOn("May 8, 2006")]
    public class TestHyphenationEnabled : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            ((TextBoxBase)_element).FontSize = 30;
            _element.Height = _element.Width = 400;
            Paragraph p = new Paragraph();
            p.Inlines.Add(new Run(_str));
            ((RichTextBox)_element).Document.IsOptimalParagraphEnabled = _isOptimalParagraphEnabled;
            if (_setPropertyOnControl)
            {
                ((RichTextBox)_element).Document.IsHyphenationEnabled = _isHyphenationEnabled;
            }
            else
            {
                p.IsHyphenationEnabled = _isHyphenationEnabled;
            }

            ((RichTextBox)_element).Document.Blocks.Clear();
            ((RichTextBox)_element).Document.Blocks.Add(p);
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _element.Focus();
            QueueDelegate(TypeAtBeginningOfDoc);
        }

        private void TypeAtBeginningOfDoc()
        {
            KeyboardInput.TypeString("^{HOME}ABC");
            QueueDelegate(new SimpleHandler(VerifyTypingAtBeginning));
        }

        private void VerifyTypingAtBeginning()
        {
            Verifier.Verify(_controlWrapper.Text.IndexOf("ABC") == 0, "string is typed at beginning of document Index Found [" +
                _controlWrapper.Text.IndexOf("ABC").ToString() + "] Expected[0]", true);
            KeyboardInput.TypeString("{END}{LEFT 5}DEF");
            QueueDelegate(VerifyTypingInTheMiddle);
        }

        private void VerifyTypingInTheMiddle()
        {
            Verifier.Verify((_controlWrapper.Text.IndexOf("DEF") != 0) && (_controlWrapper.Text.IndexOf("DEF") != _controlWrapper.Text.Length), "string is typed in the middle of document Index Found [" +
               _controlWrapper.Text.IndexOf("DEF").ToString() + "] Expected != [0] OR [" + _controlWrapper.Text.Length.ToString() + "]", true);
            QueueDelegate(NextCombination);
        }

        #region data.

        private UIElementWrapper _controlWrapper = null;
        private TextEditableType _editableType = null;
        private FrameworkElement _element = null;
        private bool _setPropertyOnControl = false;
        private bool _isHyphenationEnabled = false;
        private bool _isOptimalParagraphEnabled = false;

        private string _str = "The move, officially announced Thursday after weeks of speculation, shows Microsoft is coming up with inventive ways to compete with rival Google on the advertising front, said Microsoft Sterling, principal analyst for Sterling Market Intelligence";

        #endregion data.
    }   
}
