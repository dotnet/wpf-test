// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel; // Win32Exception
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Reflection;
using MS.Win32; // NativeMethods


namespace DRT
{
    // Regression tests for exception continuability.
    //
    // This suite verifies that all of editing's public events are consistent.
    // If a listener throws a recoverable exception, and then the application
    // swallows the exception, the editing code should remain in a valid state.
    internal class ExceptionHardeningSuite : DrtTestSuite
    {
        #region Constructors

        // Static ctor.
        static ExceptionHardeningSuite()
        {
            // Reflect on the interal TextBoxBase.UndoAction property so we can verify it later.
            s_pendingUndoActionPropertyInfo = typeof(TextBoxBase).GetProperty("PendingUndoAction", BindingFlags.NonPublic | BindingFlags.Instance);
            if (s_pendingUndoActionPropertyInfo == null)
            {
                throw new Exception("Unable to reflect on TextBoxBase.UndoAction!");
            }
        }

        // Ctor.
        internal ExceptionHardeningSuite() : base("ExceptionHardening")
        {
            // Check availability of internal methods from TextEditor class
            Type caretElementType = typeof(FrameworkElement).Assembly.GetType("System.Windows.Documents.CaretElement", /*ingoreCase:*/false);

            _reflectionCaretElement_Debug_CaretElement = caretElementType.GetProperty("Debug_CaretElement", BindingFlags.NonPublic | BindingFlags.Static);
            if (_reflectionCaretElement_Debug_CaretElement == null)
            {
                throw new Exception("CaretElement.Debug_CaretElement method cannot be found");
            }

            _reflectionCaretElement_Debug_RenderScope = caretElementType.GetProperty("Debug_RenderScope", BindingFlags.NonPublic | BindingFlags.Static);
            if (_reflectionCaretElement_Debug_RenderScope == null)
            {
                throw new Exception("CaretElement.Debug_RenderScope property cannot be found");
            }
        }

        #endregion Constructors

        #region Public Methods

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            // Set up an exception handler we'll use to eat
            // ReliabilityAssertExceptions thrown by the tests.
            // This simulates an application ignoring the exceptions.
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherException);

            _richTextBox = new RichTextBox();

            _textBox = new TextBox();

            // Cache a TextRange instance just so we can stress it across exceptions.
            _range = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);

            _stackPanel = new StackPanel();
            _stackPanel.Children.Add(_richTextBox);
            _stackPanel.Children.Add(_textBox);

            DRT.Show(_stackPanel);
            DRT.ShowRoot();

            // Return the lists of tests to run against the tree.
            return new DrtTest[] {
                new DrtTest(ClickRichTextBox),
                // TextBoxBase.TextChanged, TextContainer.Changed
                new DrtTest(AddTextChangedEvent),
                new DrtTest(UpdateRichTextBoxText),
                new DrtTest(UpdateRichTextBoxText),
                new DrtTest(VerifyRichTextBoxChangeCount),
                // TextBoxBase.SelectionChanged
                new DrtTest(UpdateSelection),
                new DrtTest(UpdateSelection),
                new DrtTest(VerifyRichTextBoxSelectionChangeCount),
                // TextRange.Changed
                new DrtTest(UpdateTextRange),
                new DrtTest(UpdateTextRange),
                new DrtTest(VerifyTextRangeChangeCount),
                // TextBox.TextChanged
                new DrtTest(UpdateTextBoxText),
                new DrtTest(UpdateTextBoxText),
                new DrtTest(VerifyTextBoxChangeCount),
                new DrtTest(RemoveTextChangedEvent),
                // Test Copy/Paste events
                new DrtTest(AddCopyPasteEvent),
                new DrtTest(DoCopyingEventTest),
                new DrtTest(DoCopyingEventTest),
                new DrtTest(DoCopyingEventTest),
                new DrtTest(DoCopyingEventTest),
                new DrtTest(VerifyCopyingEventCount),
                new DrtTest(DoPastingEventTest),
                new DrtTest(DoPastingEventTest),
                new DrtTest(VerifyPastingEventCount),
                new DrtTest(RemoveCopyPasteEvent),
                // Test DragDrop events
                new DrtTest(AddDragDropEvent),
                new DrtTest(SetDragDropPosition),
                // 1 DragDrop for PreviewQueryContinue 
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 2 DragDrop for QueryContinue 
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 3 DragDrop for PreviewDragEnter 
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 4 DragDrop for DragEnter
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 5 DragDrop for PreivewGiveFeedback
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 6 DragDrop for GiveFeedback
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 7 DragDrop for PreivewDragOver
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 8 DragDrop for DragOver
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 9 DragDrop for PreviewDragLeave
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 10 DragDrop for DragLeave
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 11 DragDrop for PreviewDrop
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 12 DragDrop for Drop
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // 13 DragDrop all events
                new DrtTest(DoDragDropSelectAll),
                new DrtTest(DoDragDropStartTest),
                // DragDrop verification
                new DrtTest(VerifyDragDropEventCount),
                new DrtTest(RemoveDragDropEvent),
                // Cleanup
                new DrtTest(Uninitialize), // Must be last.
            };
        }

        #endregion Public Methods

        #region Private Methods

        private void ClickRichTextBox()
        {
            // Click the mouse on RichTextBox.
            Rect startRect = GetScreenCharacterRect(_range.Start, LogicalDirection.Forward);
            Rect endRect = GetScreenCharacterRect(_range.End, LogicalDirection.Backward);

            DrtInput.MouseMove((int)startRect.X + 10, (int)startRect.Y + 10);
            DrtInput.MouseDown();
            DrtInput.MouseUp();
        }

        // Add TextBoxBase.TextChanged and TextContainer.Changed event handler.
        private void AddTextChangedEvent()
        {
            _richTextBox.TextChanged += new TextChangedEventHandler(OnRichTextBoxChanged);
            _range.Changed += new EventHandler(OnTextRangeChanged);

            _richTextBox.SelectionChanged += new RoutedEventHandler(OnRichTextBoxSelectionChanged);
            _textBox.TextChanged += new TextChangedEventHandler(OnTextBoxChanged);
        }

        // Modifies the content of a RichTextBox to trigger a TextChanged event.
        // Our event listener will throw a ReliabilityAssertException inside
        // the call.
        //
        // We call this method twice to ensure that the RichTextBox and TextRange
        // are still useable after the first call.
        private void UpdateRichTextBoxText()
        {
            // Stress the state a little, looking for any corruption after the first
            // exception is thrown.
            DRT.Assert(_range.Text == (new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd)).Text);

            // TextBoxBase.PendingUndoAction should only ever not equal
            // UndoAction.None during the TextChanged event.
            UndoAction pendingUndoAction = (UndoAction)s_pendingUndoActionPropertyInfo.GetValue(_richTextBox, null);
            DRT.Assert(pendingUndoAction == UndoAction.None);

            _range.Text = "foo\r\n";
        }

        // Verifies we receive the expected number of callbacks to our
        // RichTextBox.TextChanged listener.
        private void VerifyRichTextBoxChangeCount()
        {
            DRT.Assert(_onRichTextBoxChangedCallbackCount == 2, "Unexpected number of callbacks to OnRichTextBoxChanged!");
        }

        // Modifies the selection of a RichTextBox to trigger a SelectionChanged event.
        // Our event listener will throw a ReliabilityAssertException inside
        // the call.
        //
        // We call this method twice to ensure that the RichTextBox 
        // is still useable after the first call.
        private void UpdateSelection()
        {
            if (_richTextBox.Selection.Start.CompareTo(_richTextBox.Document.ContentStart) != 0)
            {
                _richTextBox.Selection.Select(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentStart);
            }
            else
            {
                _richTextBox.Selection.Select(_richTextBox.Document.ContentEnd, _richTextBox.Document.ContentEnd);
            }
        }
        
        // Verifies we receive the expected number of callbacks to our
        // RichTextBox.TextChanged listener.  
        private void VerifyRichTextBoxSelectionChangeCount()
        {
            DRT.Assert(_onRichTextBoxSelectionChangedCallbackCount == 2, "Unexpected number of callbacks (" + _onRichTextBoxSelectionChangedCallbackCount + ") to OnRichTextBoxSelectionChanged!");
        }

        // Shifts the position of a TextRange to trigger a Changed event.
        // Our event listener will throw a ReliabilityAssertException inside
        // the call.
        //
        // We call this method twice to ensure that the RichTextBox and TextRange
        // are still useable after the first call.
        private void UpdateTextRange()
        {
            DRT.Assert(!(new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd)).IsEmpty);

            if (_range.IsEmpty)
            {
                _range.Select(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            }
            else
            {
                _range.Select(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentStart);
            }
        }

        // Verifies we receive the expected number of callbacks to our
        // TextRange.Changed listener.
        private void VerifyTextRangeChangeCount()
        {
            DRT.Assert(_onTextRangeChangedCallbackCount == 2, "Unexpected number of callbacks to VerifyTextRangeChangeCount!");
        }

        // Modifies the content of a TextBox to trigger a TextChanged event.
        // Our event listener will throw a ReliabilityAssertException inside
        // the call.
        //
        // We call this method twice to ensure that the TextBox 
        // is still useable after the first call.
        private void UpdateTextBoxText()
        {
            _textBox.Text += "foo";
        }

        // Verifies we receive the expected number of callbacks to our
        // TextBox.TextChanged listener.
        private void VerifyTextBoxChangeCount()
        {
            DRT.Assert(_onTextBoxChangedCallbackCount == 2, "Unexpected number of callbacks (" + _onTextBoxChangedCallbackCount + ")to OnTextBoxChanged!");
        }

        // Remove TextBoxBase.TextChanged and TextContainer.Changed events handler.
        private void RemoveTextChangedEvent()
        {
            _richTextBox.TextChanged -= new TextChangedEventHandler(OnRichTextBoxChanged);
            _range.Changed -= new EventHandler(OnTextRangeChanged);

            _richTextBox.SelectionChanged -= new RoutedEventHandler(OnRichTextBoxSelectionChanged);
            _textBox.TextChanged -= new TextChangedEventHandler(OnTextBoxChanged);
        }

        // Add Copy/Paste events handler for hardening test of Copy/Paste events.
        private void AddCopyPasteEvent()
        {
            DataObject.AddCopyingHandler(_richTextBox, new DataObjectCopyingEventHandler(OnCopying));
            DataObject.AddPastingHandler(_richTextBox, new DataObjectPastingEventHandler(OnPasting));
            DataObject.AddSettingDataHandler(_richTextBox, new DataObjectSettingDataEventHandler(OnSettingData));

        }

        // Do copy operation to trigger Copy events.
        // Our copy event listener will throw a ReliabilityAssertException inside
        // the call.
        private void DoCopyingEventTest()
        {
            _range.Text = "This is a Copy events hardening testing.\r\n";

            // Select all text and objects of the source text box.
            _richTextBox.SelectAll();

            // Copy all text and embedded objects from the source text box.
            DrtInput.KeyboardType("^C");
        }

        // Verifies we receive the expected number of callbacks to our
        // DataObjectCopying listener.
        private void VerifyCopyingEventCount()
        {
            DRT.Assert(_onDataObjectSettingDataCallbackCount == 10, "Unexpected setting data number of callbacks to VerifyCopyingEventCount!");
            DRT.Assert(_onDataObjectCopyingCallbackCount == 2, "Unexpected copying number of callbacks to VerifyCopyingEventCount!");
        }

        // Do paste operation to trigger Paste events.
        // Our paste event listener will throw a ReliabilityAssertException inside
        // the call.
        private void DoPastingEventTest()
        {
            // Set the focus on the source text box.
            _richTextBox.Focus();

            DrtInput.KeyboardType("^V");
        }

        // Verifies we receive the expected number of callbacks to our
        // DataObjectPasting listener.
        private void VerifyPastingEventCount()
        {
            DRT.Assert(_onDataObjectSettingDataCallbackCount == 10, "Unexpected setting data number of callbacks to VerifyPastingingEventCount!");
            DRT.Assert(_onDataObjectPastingCallbackCount == 2, "Unexpected pasting number of callbacks to VerifyPastingingEventCount!");
        }

        // Remove Copy/Paste events handler that is used for hardening test of Copy/Paste events.
        private void RemoveCopyPasteEvent()
        {
            DataObject.RemoveCopyingHandler(_richTextBox, new DataObjectCopyingEventHandler(OnCopying));
            DataObject.RemovePastingHandler(_richTextBox, new DataObjectPastingEventHandler(OnPasting));
            DataObject.RemoveSettingDataHandler(_richTextBox, new DataObjectSettingDataEventHandler(OnSettingData));
        }

        // Add DragDrop Tunnel and Bubble events handler to test DragDrop events hardening.
        private void AddDragDropEvent()
        {
            _richTextBox.AddHandler(DragDrop.PreviewQueryContinueDragEvent, new QueryContinueDragEventHandler(OnPreviewQueryContinueDrag), true);
            _richTextBox.AddHandler(DragDrop.PreviewGiveFeedbackEvent, new GiveFeedbackEventHandler(OnPreviewGiveFeedback), true);
            _richTextBox.AddHandler(DragDrop.PreviewDragEnterEvent, new DragEventHandler(OnPreviewDragEnter), true);
            _richTextBox.AddHandler(DragDrop.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragOver), true);
            _richTextBox.AddHandler(DragDrop.PreviewDragLeaveEvent, new DragEventHandler(OnPreviewDragLeave), true);
            _richTextBox.AddHandler(DragDrop.PreviewDropEvent, new DragEventHandler(OnPreviewDrop), true);
            _richTextBox.AddHandler(DragDrop.QueryContinueDragEvent, new QueryContinueDragEventHandler(OnQueryContinueDrag), true);
            _richTextBox.AddHandler(DragDrop.GiveFeedbackEvent, new GiveFeedbackEventHandler(OnGiveFeedback), true);
            _richTextBox.AddHandler(DragDrop.DragEnterEvent, new DragEventHandler(OnDragEnter), true);
            _richTextBox.AddHandler(DragDrop.DragOverEvent, new DragEventHandler(OnDragOver), true);
            _richTextBox.AddHandler(DragDrop.DragLeaveEvent, new DragEventHandler(OnDragLeave), true);
            _richTextBox.AddHandler(DragDrop.DropEvent, new DragEventHandler(OnDrop), true);

            _range.Text = "This is DragDrop events hardening testing:";
        }

        // Set the drag start, dragging and dropping position.
        private void SetDragDropPosition()
        {
            _startRect = GetScreenCharacterRect(_range.Start, LogicalDirection.Forward);
            _endRect = GetScreenCharacterRect(_range.End, LogicalDirection.Backward);
        }

        // Select the text for the DragDrop operation.
        private void DoDragDropSelectAll()
        {
            _dragDropCount++;

            DrtInput.KeyboardType("{END}");
            DrtInput.KeyboardType(_dragDropCount.ToString());

            // Select all the text.
            DrtInput.KeyboardType("^A");
        }

        // Start the dragging the objec to go into DragDrop operation.
        private void DoDragDropStartTest()
        {
            // Start DragDrop operation
            DrtInput.MouseMove((int)((_startRect.X + _endRect.X) / 2), (int)(_endRect.Y + _endRect.Height / 2));
            DrtInput.MouseDown();
            DrtInput.MouseMove((int)_endRect.X, (int)_endRect.Y);

            // Move the mouse to the out of the current selection range
            DrtInput.MouseMove((int)(_endRect.X + 100), (int)(_endRect.Y + 100));

            // Drop the object to the target point.
            DrtInput.MouseMove((int)((_startRect.X + (_endRect.X - _startRect.X) / 2) + 5), (int)(_endRect.Y + (_endRect.Height / 2)));
            DrtInput.MouseUp();

            // Clearly finish of DragDrop operation.
            DrtInput.MouseDown();
            DrtInput.MouseUp();

            DrtInput.KeyboardType("{HOME}");
        }

        // Verify DragDrop events counts for hardening.
        private void VerifyDragDropEventCount()
        {
            DRT.Assert(_onPreviewQueryContinueDragCallbackCount > 1, "Unexpected PreviewQueryContinueDrag count of callbacks to VerifyDragDropEventCount! PreviewQueryContinue={0} DragDropCount={1}", _onPreviewQueryContinueDragCallbackCount, _dragDropCount);
            DRT.Assert(_onPreviewGiveFeedbackCallbackCount > 1, "Unexpected PreviewGiveFeedback count of callbacks to VerifyDragDropEventCount! PreviewGiveFeedback={0} DragDropCount={1} ", _onPreviewGiveFeedbackCallbackCount, _dragDropCount);
            DRT.Assert(_onPreviewDragEnterCallbackCount > 1, "Unexpected PreviewDragEnter count of callbacks to VerifyDragDropEventCount! PreviewDragEnter={0} DragDropCount={1} ", _onPreviewDragEnterCallbackCount, _dragDropCount);
            // Block checking of DragOver and DragLeave since we couldn't simulate mouse move
            // during OLE DragDrop operation. Actually there is no chance to dispatch our next test steps
            // while OLE DragDrop holding the message handling.
            DRT.Assert(_onPreviewDropCallbackCount > 1, "Unexpected PreviewDrop count of callbacks to VerifyDragDropEventCount! PreviewDrop={0} DragDropCount={1} ", _onPreviewDropCallbackCount, _dragDropCount);
            DRT.Assert(_onQueryContinueDragCallbackCount > 1, "Unexpected QueryContinueDrag count of callbacks to VerifyDragDropEventCount! QueryContinueDrag={0} DragDropCount={1} ", _onQueryContinueDragCallbackCount, _dragDropCount);
            DRT.Assert(_onGiveFeedbackCallbackCount > 1, "Unexpected GiveFeedback count of callbacks to VerifyDragDropEventCount! GiveFeedback={0} DragDropCount={1} ", _onGiveFeedbackCallbackCount, _dragDropCount);
            DRT.Assert(_onDragEnterCallbackCount > 1, "Unexpected DragEnter count of callbacks to VerifyDragDropEventCount! DragEnter={0} DragDropCount={1} ", _onDragEnterCallbackCount, _dragDropCount);
            DRT.Assert(_onDropCallbackCount > 1, "Unexpected Drop count of callbacks to VerifyDragDropEventCount! Drop={0} DragDropCount={1} ", _onDropCallbackCount, _dragDropCount);
        }

        // Remove all DragDrop events handler.
        private void RemoveDragDropEvent()
        {
            _richTextBox.RemoveHandler(DragDrop.PreviewQueryContinueDragEvent, new QueryContinueDragEventHandler(OnPreviewQueryContinueDrag));
            _richTextBox.RemoveHandler(DragDrop.PreviewGiveFeedbackEvent, new GiveFeedbackEventHandler(OnPreviewGiveFeedback));
            _richTextBox.RemoveHandler(DragDrop.PreviewDragEnterEvent, new DragEventHandler(OnPreviewDragEnter));
            _richTextBox.RemoveHandler(DragDrop.PreviewDragOverEvent, new DragEventHandler(OnPreviewDragOver));
            _richTextBox.RemoveHandler(DragDrop.PreviewDragLeaveEvent, new DragEventHandler(OnPreviewDragLeave));
            _richTextBox.RemoveHandler(DragDrop.PreviewDropEvent, new DragEventHandler(OnPreviewDrop));
            _richTextBox.RemoveHandler(DragDrop.QueryContinueDragEvent, new QueryContinueDragEventHandler(OnQueryContinueDrag));
            _richTextBox.RemoveHandler(DragDrop.GiveFeedbackEvent, new GiveFeedbackEventHandler(OnGiveFeedback));
            _richTextBox.RemoveHandler(DragDrop.DragEnterEvent, new DragEventHandler(OnDragEnter));
            _richTextBox.RemoveHandler(DragDrop.DragOverEvent, new DragEventHandler(OnDragOver));
            _richTextBox.RemoveHandler(DragDrop.DragLeaveEvent, new DragEventHandler(OnDragLeave));
            _richTextBox.RemoveHandler(DragDrop.DropEvent, new DragEventHandler(OnDrop));
        }

        // Test cleanup.  Removes the Dispatcher exception handler.
        private void Uninitialize()
        {
            Dispatcher.CurrentDispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(OnDispatcherException);
        }

        // RichTextBox.OnTextChanged event listener.
        // Throws a ReliabilityAssertException.
        private void OnRichTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            _onRichTextBoxChangedCallbackCount++;
            throw new ReliabilityAssertException("OnRichTextBoxChanged");
        }

        // RichTextBox.OnTextChanged event listener.
        // Throws a ReliabilityAssertException.
        private void OnRichTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            _onRichTextBoxSelectionChangedCallbackCount++;
            throw new ReliabilityAssertException("OnRichTextBoxSelectionChanged");
        }

        // TextRange.Changed event listener.
        // Throws a ReliabilityAssertException.
        private void OnTextRangeChanged(object sender, EventArgs e)
        {
            _onTextRangeChangedCallbackCount++;
            throw new ReliabilityAssertException("OnTextRangeChanged");
        }

        // TextBox.OnTextChanged event listener.
        // Throws a ReliabilityAssertException.
        private void OnTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            _onTextBoxChangedCallbackCount++;
            throw new ReliabilityAssertException("OnTextBoxChanged");
        }

        // DataObjectCopying event listener.
        // Throws a ReliabilityAssertException.
        private void OnCopying(object sender, EventArgs e)
        {
            _onDataObjectCopyingCallbackCount++;
            throw new ReliabilityAssertException("OnCopying");
        }

        // DataPasting event listener.
        // Throws a ReliabilityAssertException.
        private void OnPasting(object sender, EventArgs e)
        {
            _onDataObjectPastingCallbackCount++;
            throw new ReliabilityAssertException("OnPasting");
        }

        // DataObjectSettingData event listener.
        // Throws a ReliabilityAssertException.
        private void OnSettingData(object sender, EventArgs e)
        {
            _onDataObjectSettingDataCallbackCount++;
            if (_onDataObjectSettingDataCallbackCount < 3)
            {
                throw new ReliabilityAssertException("OnSettingData");
            }
        }

        // PreviewQueryContinueDrag event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewQueryContinueDrag(object sender, EventArgs e)
        {
            _onPreviewQueryContinueDragCallbackCount++;
            if (_onPreviewQueryContinueDragCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewQueryContinueDrag");
            }
        }

        // PreviewGiveFeedback event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewGiveFeedback(object sender, EventArgs e)
        {
            _onPreviewGiveFeedbackCallbackCount++;
            if (_onPreviewGiveFeedbackCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewGiveFeedback");
            }
        }

        // PreviewDragEnter event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewDragEnter(object sender, EventArgs e)
        {
            _onPreviewDragEnterCallbackCount++;
            if (_onPreviewDragEnterCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewDragEnter");
            }
        }

        // PreviewDragOver event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewDragOver(object sender, EventArgs e)
        {
            _onPreviewDragOverCallbackCount++;
            if (_onPreviewDragOverCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewDragOver");
            }
        }

        // PreviewDragLeave event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewDragLeave(object sender, EventArgs e)
        {
            _onPreviewDragLeaveCallbackCount++;
            if (_onPreviewDragLeaveCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewDragLeave");
            }
        }

        // PreviewDrop event listener.
        // Throws a ReliabilityAssertException.
        private void OnPreviewDrop(object sender, EventArgs e)
        {
            _onPreviewDropCallbackCount++;
            if (_onPreviewDropCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnPreviewDrop");
            }
        }

        // QueryContinueDrag event listener.
        // Throws a ReliabilityAssertException.
        private void OnQueryContinueDrag(object sender, EventArgs e)
        {
            _onQueryContinueDragCallbackCount++;
            if (_onQueryContinueDragCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnQueryContinueDrag");
            }
        }

        // GiveFeedback event listener.
        // Throws a ReliabilityAssertException.
        private void OnGiveFeedback(object sender, EventArgs e)
        {
            _onGiveFeedbackCallbackCount++;
            if (_onGiveFeedbackCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnGiveFeedback");
            }
        }

        // DragEnter event listener.
        // Throws a ReliabilityAssertException.
        private void OnDragEnter(object sender, EventArgs e)
        {
            _onDragEnterCallbackCount++;
            if (_onDragEnterCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnDragEnter");
            }
        }

        // DragOver event listener.
        // Throws a ReliabilityAssertException.
        private void OnDragOver(object sender, EventArgs e)
        {
            _onDragOverCallbackCount++;
            if (_onDragOverCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnDragOver");
            }
        }

        // ragLeave event listener.
        // Throws a ReliabilityAssertException.
        private void OnDragLeave(object sender, EventArgs e)
        {
            _onDragLeaveCallbackCount++;
            if (_onDragLeaveCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnDragLeave");
            }
        }

        // Drop event listener.
        // Throws a ReliabilityAssertException.
        private void OnDrop(object sender, EventArgs e)
        {
            _onDropCallbackCount++;
            if (_onDropCallbackCount < 2)
            {
                throw new ReliabilityAssertException("OnDrop");
            }
        }

        // DispatcherException used to eat ReliabilityAssertExceptions thrown
        // by the tests.
        // This simulates an application ignoring the exceptions.
        private void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ReliabilityAssertException ||
                e.Exception.InnerException is ReliabilityAssertException)
            {
                // Swallow the ReliabilityAssertException -- Dispatcher will move
                // on to next queue item.
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets visual information necessary for calculating screen coordinates
        /// </summary>
        /// <param name="source">
        /// Presentation source 
        /// </param>
        /// <param name="win32Window">
        /// IWin32Window with respect to which coordinates are calculated
        /// </param>
        private void GetVisualInfo(out PresentationSource source, out IWin32Window win32Window)
        {
            // Get an hwnd, CompositionTarget, and TextView.
            source = PresentationSource.FromVisual(_richTextBox);
            win32Window = source as IWin32Window;

            if (win32Window == null)
            {
                throw new COMException("TS_E_NOLAYOUT", TS_E_NOLAYOUT);
            }
        }

        /// <summary>
        /// Gets coordinates of a point on the actual screen for mouse application. Returns a point with the new coordinates
        /// </summary>
        /// <param name="point">
        /// Point to be transformed to screen coordinates
        /// </param>
        private Point GetScreenPointFromRenderScopePoint(Point point)
        {
            Point transformedPoint = new Point(point.X, point.Y);

            PresentationSource source;
            IWin32Window win32Window;
            GetVisualInfo(out source, out win32Window);
            CompositionTarget compositionTarget = source.CompositionTarget;
            FrameworkElement renderScope = (FrameworkElement)_reflectionCaretElement_Debug_RenderScope.GetValue(null, null);

            GeneralTransform transform = renderScope.TransformToAncestor(compositionTarget.RootVisual);
            transform.TryTransform(transformedPoint, out transformedPoint);

            transformedPoint = compositionTarget.TransformToDevice.Transform(transformedPoint);
            NativeMethods.POINT clientPoint = new NativeMethods.POINT();
            if (UnsafeNativeMethods.ClientToScreen(new HandleRef(null, win32Window.Handle), /* ref by interop */ clientPoint) == 0)
            {
                throw new Win32Exception();
            }

            transformedPoint.X += clientPoint.x;
            transformedPoint.Y += clientPoint.y;

            return transformedPoint;
        }

        /// <summary>
        /// Gets screen coordinates for the Rect of a TextPointer element for moue application. Returns a Rect with the transformed coordinates
        /// </summary>
        /// <param name="position"></param>
        /// TextPointer whose Rect screen coordinates are to be calculated
        /// <param name="direction">
        /// LogicalDirection in which pointer Rect is to be determined
        /// </param>
        private Rect GetScreenCharacterRect(TextPointer position, LogicalDirection direction)
        {
            PresentationSource source;
            IWin32Window win32Window;
            GetVisualInfo(out source, out win32Window);
            CompositionTarget compositionTarget = source.CompositionTarget;

            Rect rect = position.GetCharacterRect(direction);            

            GeneralTransform transform = _richTextBox.TransformToAncestor(compositionTarget.RootVisual);
            rect = transform.TransformBounds(rect);    

            Point rectTopLeft = new Point(rect.Left, rect.Top);
            Point rectBottomRight = new Point(rect.Right, rect.Bottom);
            
            // Recalculate startRect and endRect based on transformation to screen
            Rect transformedRect = TransformRootRectToScreenCoordinates(rectTopLeft, rectBottomRight, win32Window, source);

            return transformedRect;
        }

        /// <summary>
        /// Helper that takes rectangle coordinates that have been transformed from text box to window, and transforms them
        /// from window to screen. Returns Rect with transformed coordinates
        /// </summary>
        /// <param name="topLeft">
        /// Top left point of Rect
        /// </param>
        /// <param name="bottomRight">
        /// Bottom right point of Rect
        /// </param>
        /// <param name="win32Window">
        /// Win32Window in which Rect lies
        /// </param>
        /// <param name="source">
        /// PresentationSource to be used in transformation
        /// </param>
        /// <returns></returns>
        private Rect TransformRootRectToScreenCoordinates(Point topLeft, Point bottomRight, IWin32Window win32Window, PresentationSource source)
        {
            Rect rect = new Rect();

            // Transform to device units.
            CompositionTarget compositionTarget = source.CompositionTarget;
            topLeft = compositionTarget.TransformToDevice.Transform(topLeft);
            bottomRight = compositionTarget.TransformToDevice.Transform(bottomRight);

            // Transform to screen coords.
            NativeMethods.POINT clientPoint = new NativeMethods.POINT();
            if (UnsafeNativeMethods.ClientToScreen(new HandleRef(null, win32Window.Handle), /* ref by interop */ clientPoint) == 0)
            {
                throw new Win32Exception();
            }

            int rectTop = (int)(clientPoint.y + topLeft.Y);
            int rectLeft = (int)(clientPoint.x + topLeft.X);
            int rectBottom = (int)(clientPoint.y + bottomRight.Y);
            int rectRight = (int)(clientPoint.x + bottomRight.X);

            DRT.Assert(rectBottom >= rectTop, "Verifying rectangle coordinates");
            DRT.Assert(rectRight >= rectLeft, "Verifying Rectangle coordinates");

            int rectWidth = rectRight - rectLeft;
            int rectHeight = rectBottom - rectTop;

            rect = new Rect(rectLeft, rectTop, rectWidth, rectHeight);
            return rect;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetEnvironmentVariable(string variable, string value);

        #endregion Private Methods

        #region Private Fields

        // Test controls.
        private RichTextBox _richTextBox;
        private StackPanel _stackPanel;
        private TextBox _textBox;

        // Cached TextRange used across tests.
        private TextRange _range;

        // Count of RichTextBox.OnTextChanged callbacks received by our listener.
        private int _onRichTextBoxChangedCallbackCount;

        // Count of RichTextBox.OnSelectionChanged callbacks received by our listener.
        private int _onRichTextBoxSelectionChangedCallbackCount;

        // Count of TextRange.OnChanged callbacks received by our listener.
        private int _onTextRangeChangedCallbackCount;

        // Count of TextBox.OnTextChanged callbacks received by our listener.
        private int _onTextBoxChangedCallbackCount;

        // Count of DataObject.DataObjectCopying callbacks received by our listener.
        private int _onDataObjectCopyingCallbackCount;

        // Count of DataObject.DataObjectPasting callbacks received by our listener.
        private int _onDataObjectPastingCallbackCount;

        // Count of DataObject.DataObjectSettingData callbacks received by our listener.
        private int _onDataObjectSettingDataCallbackCount;

        // Count of DragDrop.PreviewQueryContinueDrag callbacks received by our listener.
        private int _onPreviewQueryContinueDragCallbackCount;

        // Count of DragDrop.PreviewGiveFeedback callbacks received by our listener.
        private int _onPreviewGiveFeedbackCallbackCount;

        // Count of DataObject.PreviewDragEnter callbacks received by our listener.
        private int _onPreviewDragEnterCallbackCount;

        // Count of DragDrop.reviewDragOver callbacks received by our listener.
        private int _onPreviewDragOverCallbackCount;

        // Count of DragDrop.PreviewDragLeave callbacks received by our listener.
        private int _onPreviewDragLeaveCallbackCount;

        // Count of DragDrop.PreviewDrop callbacks received by our listener.
        private int _onPreviewDropCallbackCount;

        // Count of DragDrop.QueryContinueDrag callbacks received by our listener.
        private int _onQueryContinueDragCallbackCount;

        // Count of DragDrop.GiveFeedback callbacks received by our listener.
        private int _onGiveFeedbackCallbackCount;

        // Count of DragDrop.DragEnter callbacks received by our listener.
        private int _onDragEnterCallbackCount;

        // Count of DragDrop.DragOver callbacks received by our listener.
        private int _onDragOverCallbackCount;

        // Count of DragDrop.DragLeave callbacks received by our listener.
        private int _onDragLeaveCallbackCount;

        // Count of DragDrop.Drop callbacks received by our listener.
        private int _onDropCallbackCount;

        // Start/End rect of the text selection for DragDrop operation.
        private Rect _startRect;
        private Rect _endRect;

        // Count of DragDrop operation.
        private int _dragDropCount;

        // PropertyInfo for the internal TextBoxBase.PendingUndoAction property.
        private static PropertyInfo s_pendingUndoActionPropertyInfo;
        private const int TS_E_NOLAYOUT = unchecked((int)0x80040206);
        private PropertyInfo _reflectionCaretElement_Debug_CaretElement;
        private PropertyInfo _reflectionCaretElement_Debug_RenderScope;

        #endregion Private Fields
    }
}