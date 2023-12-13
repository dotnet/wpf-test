// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DRT
{
    using System; // Type
    using System.ComponentModel; // Win32Exception
    using System.Runtime.InteropServices;
    using System.Reflection;
    using System.Windows; // UIElement, etc.
    using System.Windows.Input;
    using System.Windows.Controls; // RichTextBox
    using System.Windows.Documents; // TextPointer
    using System.Windows.Media; // Brush, etc.
    using System.Windows.Markup;
    using System.Windows.Shapes; // Rectangle
    using System.Windows.Interop;

    using MS.Win32; // NativeMethods

    internal class RichTextBoxSuite : DrtTestSuite
    {
        // Constructor.
        internal RichTextBoxSuite() : base("RichTextBox")
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

            _reflectionCaretElement_Left = caretElementType.GetField("_left", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_Left == null)
            {
                throw new Exception("CaretElement._left field cannot be found");
            }

            _reflectionCaretElement_ShowCaret = caretElementType.GetField("_showCaret", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_ShowCaret == null)
            {
                throw new Exception("CaretElement._showCaret field cannot be found");
            }

            _reflectionCaretElement_BlinkAnimationClock = caretElementType.GetField("_blinkAnimationClock", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_BlinkAnimationClock == null)
            {
                throw new Exception("CaretElement._blinkAnimationClock field cannot be found");
            }

            _reflectionCaretElement_Top = caretElementType.GetField("_top", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_Top == null)
            {
                throw new Exception("CaretElement._top field cannot be found");
            }

            _reflectionCaretElement_SystemCaretWidth = caretElementType.GetField("_systemCaretWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_SystemCaretWidth == null)
            {
                throw new Exception("CaretElement._systemCaretWidth field cannot be found");
            }

            _reflectionCaretElement_InterimWidth = caretElementType.GetField("_interimWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_InterimWidth == null)
            {
                throw new Exception("CaretElement._interimWidth field cannot be found");
            }

            _reflectionCaretElement_Height = caretElementType.GetField("_height", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_Height == null)
            {
                throw new Exception("CaretElement._height field cannot be found");
            }

            _reflectionCaretElement_IsBlinkEnabled = caretElementType.GetField("_isBlinkEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_IsBlinkEnabled == null)
            {
                throw new Exception("CaretElement._isBlinkEnabled field cannot be found");
            }

            _reflectionCaretElement_CaretBrush = caretElementType.GetField("_caretBrush", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_CaretBrush == null)
            {
                throw new Exception("CaretElement._caretBrush field cannot be found");
            }

            _reflectionCaretElement_AdornerLayer = caretElementType.GetField("_adornerLayer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_AdornerLayer == null)
            {
                throw new Exception("CaretElement._adornerLayer field cannot be found");
            }

            _reflectionCaretElement_Italic = caretElementType.GetField("_italic", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_reflectionCaretElement_Italic == null)
            {
                throw new Exception("CaretElement._italic field cannot be found");
            }
        }

        private UIElement CreateTree()
        {
            Canvas canvas = new Canvas();
            _textbox = new RichTextBox();
            Canvas.SetLeft(_textbox, 5);
            Canvas.SetTop(_textbox, 5);
            _textbox.Width = 200;
            _textbox.Height = 200;

            // Set the font for consistency across themes
            _textbox.FontFamily = new FontFamily("Tahoma");
            _textbox.FontSize = 11.0;

            canvas.Children.Add(_textbox);

            _textBlock = new TextBlock();
            Canvas.SetLeft(_textBlock, 5);
            Canvas.SetTop(_textBlock, 210);
            _textBlock.Width = 200;
            _textBlock.Height = 25;
            canvas.Children.Add(_textBlock);

            return canvas;
        }

        // Initialize tests.
        public override DrtTest[] PrepareTests()
	    {
            DRT.Show(CreateTree());

            System.Collections.Generic.List<DrtTest> tests = new System.Collections.Generic.List<DrtTest>();

            //Tests common to all framework versions
            tests.AddRange(
                new DrtTest[]{
                    new DrtTest(ClickTextBox),
                    new DrtTest(TestAppendText),
                    new DrtTest(TestClear),
                    new DrtTest(TestIsReadOnly),
                    new DrtTest(TestIsReadOnlyVerifyTrue),
                    new DrtTest(TestIsReadOnlyVerifyFalse),
                    new DrtTest(TestProofingAssemblyNotLoaded),
                    new DrtTest(TextPointerNormalization),
                    new DrtTest(TestSelect),
                    new DrtTest(TestSelectedText),
                    new DrtTest(TestSelectionLength),
                    new DrtTest(TestSelectionStart),
                    new DrtTest(TestUndo1),
                    new DrtTest(TestUndo2),
                    new DrtTest(TestUndo3),
                    new DrtTest(TestScrollViewerMembers),
                    new DrtTest(TestScrollViewerMembersLaidOut),
                    new DrtTest(TestScrollViewerTyping),
                    new DrtTest(TestScrollViewerTypingCheckHorizontal),
                    new DrtTest(TestScrollViewerTypingCheckVertical),
                    new DrtTest(TestCopyCommand),
                    new DrtTest(TestCopyCommandCheck),
                    new DrtTest(TestCutCommand),
                    new DrtTest(TestCutCommandCheck),
                    new DrtTest(TestPasteCommand),
                    new DrtTest(TestPasteCommandCheck),
                    new DrtTest(TestLinesInTextBlock),
                    new DrtTest(VerifyLinesInTextBlock),
                    new DrtTest(TestText),
                    new DrtTest(TestHitTest),
                    new DrtTest(TestRangeFormatting),
                    new DrtTest(SelectWithMouse),
                    new DrtTest(CheckMouseSelectionFullText),
                    new DrtTest(CheckMouseSelectionPartialWord),
                    new DrtTest(CheckMouseSelectionFullWords),
                    new DrtTest(CheckMouseSelectionFullToPartialWord),
                    new DrtTest(CheckMouseSelectionWithinWord),
                    new DrtTest(CheckMouseSelectionPartialToFullText),
                    new DrtTest(CheckMouseSelectionFullToPartialText),
                    new DrtTest(CheckMouseSelectionPartialToPartialWord),
                    new DrtTest(ClearTextBoxAfterMouseSelection),
                    new DrtTest(TestTextMergeBetweenParagraphs),
                    new DrtTest(TestTextMergeBeforeListStart),
                    new DrtTest(TestTableSelection),
                    new DrtTest(SetSelectionPositionForVerticalCaretMovement),
                    new DrtTest(TestSelectionPositionForVerticalCaretMovement),
                    new DrtTest(TestCaretMovementOnHome),
                    new DrtTest(TestCaretMovementOnEnd),
                    new DrtTest(TestCaretMovementOnKeyUp),
                    new DrtTest(TestCaretMovementOnKeyDown),
                    new DrtTest(LoadMultiLineTextForVerticalCaretSelection),
                    new DrtTest(TestSelectionPositionMultiLineText),
                    new DrtTest(TestCaretMovementOnMultipleKeyDownAndKeyUp),
                    new DrtTest(TestKeyboardSelectionDownOneLine),
                    new DrtTest(TestKeyboardSelectionDownTwoLines),
                    new DrtTest(TestKeyboardSelectionDownThreeLines),
                    new DrtTest(TestKeyboardSelectionDownTextboxEnd),
                    new DrtTest(TestKeyboardSelectionUpOneLine),
                    new DrtTest(TestKeyboardSelectionUpTwoLines),
                    new DrtTest(TestKeyboardSelectionUpThreeLines),
                    new DrtTest(TestKeyboardSelectionUpFourLines),
                    new DrtTest(TestKeyboardSelectionUpTextboxStart),
                    new DrtTest(TestKeyboardSelectionDownToFirstPosition),
                    new DrtTest(LoadWrappingText),
                    new DrtTest(TestSelectionPositionWrappingText),
                    new DrtTest(TestCaretMovementOnKeyUpWrappingText),
                    new DrtTest(TestCaretMovementOnHomeWrappingText),
                    new DrtTest(TestStyleChange),
                    new DrtTest(TestDeleteTextInRun)
                }
            );

            //Speller not available in Arrowhead
            if(!DrtBase.IsClientSKUOnly)
            {
                tests.AddRange(
                    new DrtTest []
                    {
                        new DrtTest(TestSpellerAPI)
                    }
                );
            }

            return tests.ToArray();
        }

        /// <summary>
        /// Checks that the specified condition is true.
        /// </summary>
        /// <param name='condition'>
        /// Condition value.
        /// </param>
        /// <param name='description'>
        /// Description of evaluated condition
        /// </param>
        private void AssertCondition(bool condition, string description)
        {
            if (!condition)
            {
                string message = String.Format("Expected condition [{0}] does not hold", description);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Verifies that a given object has the expected value.
        /// </summary>
        /// <param name='actualValue'>
        /// Actual value.
        /// </param>
        /// <param name='expectedValue'>
        /// Expected value.
        /// </param>
        /// <param name='description'>
        /// An optional descriptive message.
        /// </param>
        private void AssertEqual(object actualValue, object expectedValue, string description)
        {
            bool condition = actualValue.Equals(expectedValue);
            if (!condition)
            {
                string message = String.Format("Expected [{0}] got [{1}] {2}", expectedValue, actualValue, description);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Checks that the second parameter passed is greater than the first.
        /// </summary>
        /// <param name='lesserValue'>
        /// Value expected to be smaller.
        /// </param>
        /// <param name='greaterValue'>
        /// Value expected to be larger.
        /// </param>
        /// <param name='description'>
        /// An descriptive message of the condition tested.
        /// </param>
        private void AssertSecondGreater(double lesserValue, double greaterValue, string description)
        {
            if (lesserValue >= greaterValue)
            {
                string message = String.Format("Expected condition [{0} < {1}] does not hold {2}", lesserValue, greaterValue, description);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Verifies that the SelectedText property has the expected value.
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckSelectedText(string expectedText)
        {
            AssertEqual(_textbox.Selection.Text, expectedText, "Verifying SelectedText");
        }

        /// <summary>
        /// Verifies that the previously SelectedText is same as clipbaord
        /// </summary>
        /// <param name='expectedText'>
        /// Expected value.
        /// </param>
        private void CheckClipboardText(string expectedText)
        {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject != null)
            {
                object data = dataObject.GetData(DataFormats.Text);
                if (data != null && data is string)
                {
                    AssertEqual(data as string, expectedText, "Verifying ClipboardText");
                }
                else
                {
                    AssertEqual(false, expectedText, "Verifying ClipboardText");
                }
            }
        }

        /// <summary>
        /// Verifies that the Text property has the expected value.
        /// </summary>
        /// <param name='expectedText'>
        /// Expected value.
        /// </param>
        private void CheckText(string expectedText)
        {
            TextRange fullRange = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            AssertEqual(fullRange.Text, expectedText, "Verifying Text");
        }

        /// <summary>
        /// Clicks on the TextBox control.
        /// </summary>
        private void ClickTextBox()
        {
            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            const int x = 150;
            const int y = 150;
            DrtInput.ClickScreenPoint(x, y);
        }

        /// <summary>
        /// Tests the TextBox.AppendText method.
        /// </summary>
        private void TestAppendText()
        {
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            range.Text = "";
            CheckText("");

            _textbox.AppendText("some text");
            CheckText("some text\r\n");

            _textbox.AppendText("more");
            CheckText("some textmore\r\n");

            _textbox.AppendText(" whitespace ");
            CheckText("some textmore whitespace \r\n");

            _textbox.AppendText("\nnew\nlines");
            CheckText("some textmore whitespace \r\nnew\r\nlines\r\n");  // note that \n will be treated as new Paragraph and returned back as \r\n
        }

        /// <summary>
        /// Tests the Clear method.
        /// </summary>
        private void TestClear()
        {
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            range.Text = "";
            CheckText("");

            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");
            range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            range.Text = "text";
            CheckText("text\r\n");

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2);

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");

            AssertEqual(_textbox.Document.ContentStart.GetOffsetToPosition(_textbox.Selection.Start), 2, "Selection start after Paragraph and Run");
            AssertEqual(_textbox.Selection.Start.GetOffsetToPosition(_textbox.Selection.End), 0, "Selection length is zero");
        }

        /// <summary>Tests the TextBox.IsReadOnly property.</summary>
        /// <remarks>
        /// Note that input emulation uses characters that can be mapped
        /// through any keyboard layout in this case.
        /// </remarks>
        private void TestIsReadOnly()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            _textbox.IsReadOnly = true;
            KeyboardType("  ");
        }

        private void TestIsReadOnlyVerifyTrue()
        {
            CheckText("\r\n");

            _textbox.IsReadOnly = false;
            KeyboardType("...");
        }

        private void TestIsReadOnlyVerifyFalse()
        {
            CheckText("...\r\n");
        }

        /// <summary>
        /// Verifies that the ProofingServices assembly has not been loaded
        /// at this point.
        /// </summary>
        private void TestProofingAssemblyNotLoaded()
        {
            const string match = "proof"; // assembly name may change - has happened before
            System.Reflection.Assembly[] assemblies;
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                string name = assemblies[i].GetName().Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                if (name.IndexOf(match) != -1)
                {
                    throw new Exception(
                        "Assembly [" + assemblies[i].FullName + "] loaded, " +
                        "possibly the proofing services assembly. This " +
                        "assembly should have been delayed.");
                }
            }
        }

        /// <summary>
        /// Tests TextPointer normalization
        /// </summary>
        private void TextPointerNormalization()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            //                                           0''''''''''0'''''0''''0000'''''0''''0011'''''1''''''1''''''''''''''''''1''''''''1'''''''''''''''''''1'''''''''''1'''''1'''''''''1''''''''''2''''''2'''''2'''''''''2''''''''''2''''2222'''''2'''''''''''3''''''''''3'''''''''3''''''''''3''''3333'''''3'''''''''''3''''''''''4''''''4''''''4''''''''''''''4'''''''''4''''''''''4''''''''''4''''4445'''''5'''''''''''5'''''''''''5''''''''''5''''''''''5''''5555'''''6'''''''''''6'''''''''''6''''''''''6'''''''''6''''''''''6''''''''''6''''6667'''''7'''''''''''7'''''''''''7''''''''''7''''''''''7''''7777'''''8'''''''''''8'''''''''''8''''''''''8'''''''''8''''''''''8''''''''''8''''8889'''''9'''''''''''9'''''''''''9''''''''''9''''''''''9''''9999'''''0'''''''''''0'''''''''''0''''''''''0'''''''''''''''0'''''''0
            //                                           0..........1.....2....3456.....7....8901.....2......3..................4........5...................6...........7.....8.........9..........0......1.....2.........3..........4....5678.....9...........0..........1.........2..........3....4567.....8...........9..........0......1......2..............3.........4..........5..........6....7890.....1...........2...........3..........4..........5....6789.....0...........1...........2..........3.........4..........5..........6....7890.....1...........2...........3..........4..........5....6789.....0...........1...........2..........3.........4..........5..........6....7890.....1...........2...........3..........4..........5....6789.....0...........1...........2..........3...............4.......5
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Span><Run>one</Run><Run>two</Run></Span><InlineUIContainer><Button/></InlineUIContainer></Paragraph><List><ListItem></ListItem></List><List><ListItem><Paragraph><Run>qqq</Run></Paragraph></ListItem><ListItem><Paragraph><Run>zzz</Run></Paragraph></ListItem></List><Table><TableRowGroup><TableRow><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow><TableRow><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow><TableRow><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow></TableRowGroup></Table>"));

            int[] positionNormalizedBackward = new int[]
                {                 3,         3,    3,   3,4,5,6, 6,   6,9,10,11,
                                                                               11,    11,                11,      16,                 16,         16,   19,       19,        19,    19,   25,       25,        25,  25,26,27,28,
                                                                                                                                                                                                                              28,        28,        28,        34,       34,  34,35,36,37,
                                                                                                                                                                                                                                                                                        37,        37,         37,   37,     47,           47,        47,        47,       47,  47,48,49,50,
                                                                                                                                                                                                                                                                                                                                                                                          50,        50,          50,        56,       56, 56,57,58,59,  59,      59,        62,        62,       67,         67,        67, 67,68,69,70,  70,     70,         70,        76,        76, 76,77,78,79,  79,     79,        82,        82,       87,        87,        87, 87,88,89,90,  90,     90,          90,       96,       96,  96,97,98,99, 99,      99,        102,        102,           102,    102 };
            int[] positionNormalizedForward = new int[]
                {                 3,         3,    3,   3,4,5,8, 8,   8,9,10,13,
                                                                               13,    13,                13,      16,                 16,         19,   19,       19,        19,    25,   25,       25,        25,  25,26,27,28,
                                                                                                                                                                                                                              28,        28,        34,        34,       34,  34,35,36,37,
                                                                                                                                                                                                                                                                                        37,        37,         37,   47,     47,           47,        47,        47,       47,  47,48,49,50,
                                                                                                                                                                                                                                                                                                                                                                                          50,        50,          56,        56,       56, 56,57,58,59,  59,      59,        62,        67,       67,         67,        67, 67,68,69,70,  70,     70,         76,        76,        76, 76,77,78,79,  79,     79,        82,        87,       87,        87,        87, 87,88,89,90,  90,     90,          96,       96,       96,  96,97,98,99, 99,      99,        102,        102,           102,    102 };
            TextPointer start = _textbox.Document.ContentStart;

            for (int offset = 0; offset < positionNormalizedBackward.Length; offset++)
            {
                TextPointer positionBackward = start.GetPositionAtOffset(offset).GetInsertionPosition(LogicalDirection.Backward);
                AssertEqual(/*actual:*/start.GetOffsetToPosition(positionBackward), /*expected:*/positionNormalizedBackward[offset], "Normalization Backward failed from offset " + offset);
                TextPointer positionForward = start.GetPositionAtOffset(offset).GetInsertionPosition(LogicalDirection.Forward);
                AssertEqual(/*actual:*/start.GetOffsetToPosition(positionForward), /*expected:*/positionNormalizedForward[offset], "Normalization forward failed from offset " + offset);
            }

            // Test potential insertion positions:
            //  1. Possible run positions
            //  2. table row end
            //  3. empty table cell, empty list item
            //  4. before the start of a first table in a collection of blocks
            //  5. inside BlockUIContainer

            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            //                    0''''''''''''''''0'''''''''0'''''''''''''''''0'''''''0''''''''''''''0'''''''''0'''''''''0''''''''''''0''''''''''0''''''''''1'''1111''''''1'''''''''''1'''''''''''1'''''''''1''''''''''1''''''''''2'''''2'''''''''''''''2'''''''''2''''''''''2'''''''''''2'''''''''2''''''''''2''''2233''''''3'''''''''''3'''''''''''3''''''''''3'''''''''''''''3'''''''3''''''''''3'''''''''''3''''''''''4''''4444''''4'''''''''''4''''''''''''4''''''''''4'''''''''''''''4'''''''5'''''''''''''''''5''''''''5''''''''''''''''''5''''''''''5''''''''''5
            //                    0................1.........2.................3.......4..............5.........6.........7............8..........9..........0...1234......5...........6...........7.........8..........9..........0.....1...............2.........3..........4...........5.........6..........7....8901......2...........3...........4..........5...............6.......7..........8...........9..........0....1234....5...........6............7..........8...............9.......0.................1........2..................3..........4..........5
            LoadXamlContent(DrtEditing.GetClipboardXaml("<BlockUIContainer><Button/></BlockUIContainer><Table><TableRowGroup><TableRow><TableCell></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow><TableRow><TableCell><Table><TableRowGroup><TableRow><TableCell></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow></TableRowGroup></Table></TableCell><TableCell><Paragraph><Run>one</Run></Paragraph></TableCell></TableRow></TableRowGroup></Table><BlockUIContainer><Button/></BlockUIContainer><Paragraph></Paragraph>"));

            positionNormalizedBackward = new int[]
                {                 1,               1,        2,                2,      7,             7,        7,        7,           7,         11,         11, 11,
                                                                                                                                                                 12,13,14, 14,         14,         17,       17,        20,        20,    24,            24,        24,       24,         24,       28,        28,28,29,30,31,31,         31,         34,        34,             34,     34,        34,         41,        41,41,42,43,44,44,       44,          47,        47,             47,     47,               51,      52,                52,       54,         54};

            positionNormalizedForward = new int[]
                {                 1,               1,        2,                7,      7,             7,        7,        7,           11,        11,         11, 11,
                                                                                                                                                                 12,13,14, 14,         14,         17,       20,        20,        20,    24,            24,        24,       24,         28,       28,        28,28,29,30,31,31,         31,         34,         34,            34,     34,        41,         41,        41,41,42,43,44,44,       44,          47,        47,             47,     51,               51,      52,                54,       54,         54};

            start = _textbox.Document.ContentStart;

            for (int offset = 0; offset < positionNormalizedBackward.Length; offset++)
            {
                TextPointer positionBackward = start.GetPositionAtOffset(offset).GetInsertionPosition(LogicalDirection.Backward);
                AssertEqual(/*actual:*/start.GetOffsetToPosition(positionBackward), /*expected:*/positionNormalizedBackward[offset], "Normalization Backward failed from offset " + offset);
                TextPointer positionForward = start.GetPositionAtOffset(offset).GetInsertionPosition(LogicalDirection.Forward);
                AssertEqual(/*actual:*/start.GetOffsetToPosition(positionForward), /*expected:*/positionNormalizedForward[offset], "Normalization forward failed from offset " + offset);
            }
        }

        /// <summary>
        /// Tests the TextBox.Select method.
        /// </summary>
        private void TestSelect()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");

            _textbox.Selection.Select(_textbox.Document.ContentStart, _textbox.Document.ContentStart);
            CheckSelectedText("");
            CheckText("\r\n");

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "Not selected text";
            CheckSelectedText("");

            _textbox.Selection.Select(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            _textbox.Selection.Text = "text";
            CheckSelectedText("text");

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+1); // 2 for Paragraph/Run start, the other for 't'

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            CheckSelectedText("t");

            TextPointer position2 = _textbox.Document.ContentStart.GetPositionAtOffset(+2+2); // 2 for Paragraph/Run start, two for 'te'

            _textbox.Selection.Select(position, position2);
            CheckSelectedText("e");

            position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+4); // 2 for Paragraph/Run start, four for 'text'

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            CheckSelectedText("text");
        }

        /// <summary>Tests the TextBox.SelectedText property.</summary>
        private void TestSelectedText()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");
            CheckSelectedText("");

            new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd).Text = "content";
            CheckText("content\r\n");
            CheckSelectedText("");

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+1); // 2 for Paragraph/Run start, the other for 'c'

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            CheckSelectedText("c");

            _textbox.Selection.Text = "more c";
            CheckSelectedText("more c");
            CheckText("more content\r\n");
        }

        /// <summary>Tests the TextBox.SelectionLength property.</summary>
        private void TestSelectionLength()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");
            AssertEqual(0, _textbox.Selection.Start.GetOffsetToPosition(_textbox.Selection.End), "SelectionLength");

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "some content";

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+4); // 2 for Paragraph/Run start, 4 - for 'some'

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            AssertEqual(_textbox.Selection.Start.GetOffsetToPosition(_textbox.Selection.End), 4, "SelectionLength");

            position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+2);
            TextPointer position2 = _textbox.Document.ContentStart.GetPositionAtOffset(+2+6);

            _textbox.Selection.Select(position, position2);
            AssertEqual(_textbox.Selection.Start.GetOffsetToPosition(_textbox.Selection.End), 6-2, "SelectionLength");

            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            AssertEqual(_textbox.Selection.Start.GetOffsetToPosition(_textbox.Selection.End), 0, "SelectionLength");
        }

        /// <summary>Tests the TextBox.SelectionStart property.</summary>
        private void TestSelectionStart()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            CheckText("\r\n");
            AssertEqual(_textbox.Document.ContentStart.GetOffsetToPosition(_textbox.Selection.Start), 2, "SelectionStart");

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "some content";

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+1+4);

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);
            AssertEqual(_textbox.Document.ContentStart.GetOffsetToPosition(_textbox.Selection.Start), +2, "SelectionStart"); // +2 - for Paragraph/Run tags

            position = _textbox.Document.ContentStart.GetPositionAtOffset(+2+2);
            TextPointer position2 = _textbox.Document.ContentStart.GetPositionAtOffset(+2+6);

            _textbox.Selection.Select(position, position2);
            AssertEqual(_textbox.Document.ContentStart.GetOffsetToPosition(_textbox.Selection.Start), 2 + 2, "SelectionStart");

            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
        }

        private void TestUndo1()
        {
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "first text";
        }

        private void TestUndo2()
        {
            _textbox.LockCurrentUndoUnit();
            _textbox.SelectAll();
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "second text";
        }

        private void TestUndo3()
        {
            _textbox.Undo();
            CheckText("first text\r\n");
            _textbox.Redo();
            CheckText("second text\r\n");
        }

        /// <summary>
        /// Tests the members that directly affect the scroll viewer:
        /// LineLeft, LineRight, PageLeft, PageRight,
        /// LineUp, LineDown, PageUp, PageDown,
        /// ScrollToHome, ScrollToEnd,
        /// ExtentWidth, ExtentHeight,
        /// ViewportWidth, ViewportHeight,
        /// HorizontalScrollBarVisibility, VerticalScrollBarVisibility,
        /// HorizontalOffset, VerticalOffset.
        /// </summary>
        private void TestScrollViewerMembers()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            _textbox.Document.PageWidth = 10000; // arbitrary large size
            CheckText("\r\n");

            new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd).Text = MultiPageText;
            _textbox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _textbox.AcceptsReturn = true;
            // collapse selection
            _textbox.Selection.Select(_textbox.Document.ContentStart, _textbox.Document.ContentStart);
        }

        /// <summary>
        /// Test scrolling after giving any objects to updated their layout.
        /// </summary>
        private void TestScrollViewerMembersLaidOut()
        {
            double extX = _textbox.ExtentWidth;  // horizontal extent
            double extY = _textbox.ExtentHeight;  // vertical extent
            double sizeX = _textbox.ViewportWidth; // horizontal size
            double sizeY = _textbox.ViewportHeight; // vertical size

            //AssertSecondGreater(extX, sizeX, "Horizontal layout size is greater than content extent");
            AssertSecondGreater(sizeY, extY, "Vertical content extent is greater than layout size");

            double posX = _textbox.HorizontalOffset; // horizontal position
            double posY = _textbox.VerticalOffset; // vertical position

            _textbox.LineRight();
            _textbox.UpdateLayout();
            AssertSecondGreater(posX, _textbox.HorizontalOffset, "TextBox.LineRight increases horizontal position.");

            _textbox.LineLeft();
            _textbox.UpdateLayout();
            AssertEqual(_textbox.HorizontalOffset, posX, "TextBox.LineLeft restores horizontal position.");

            _textbox.LineDown();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset, "TextBox.LineDown increases vertical position.");

            _textbox.LineUp();
            _textbox.UpdateLayout();
            AssertEqual(_textbox.VerticalOffset, posY, "TextBox.LineUp restores vertical position.");

            _textbox.PageRight();
            _textbox.UpdateLayout();
            AssertSecondGreater(posX, _textbox.HorizontalOffset, "TextBox.PageRight increases horizontal position.");

            _textbox.PageLeft();
            _textbox.UpdateLayout();
            AssertEqual(_textbox.HorizontalOffset, posX, "TextBox.PageLeft restores horizontal position.");

            _textbox.PageDown();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset, "TextBox.PageDown increases vertical position.");

            _textbox.PageUp();
            _textbox.UpdateLayout();
            AssertEqual(_textbox.VerticalOffset, posY, "TextBox.PageUp restores vertical position.");

            _textbox.ScrollToEnd();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset, "TextBox.ScrollToEnd increases vertical position.");

            _textbox.ScrollToHome();
            _textbox.UpdateLayout();
            AssertEqual(_textbox.VerticalOffset, posY, "TextBox.ScrollToHome restores vertical position.");

            // Clear for next stage.
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            _textbox.Document.PageWidth = 10000; // arbitrary large size
            CheckText("\r\n");
            _textbox.ScrollToHome();
        }

        /// <summary>
        /// Test scrolling while typing.
        /// </summary>
        private void TestScrollViewerTyping()
        {
            AssertEqual(_textbox.HorizontalOffset, (double)0, "TextBox.HorizontalOffset is reset to 0 horizontal position.");
            KeyboardType(LongText);
        }

        private void TestScrollViewerTypingCheckHorizontal()
        {
            AssertSecondGreater((double)0, _textbox.HorizontalOffset, "TextBox.HorizontalOffset moves to follow caret.");
            KeyboardType(
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}");
        }

        private void TestScrollViewerTypingCheckVertical()
        {
            AssertSecondGreater(0, _textbox.VerticalOffset, "TextBox.VerticalOffset moves to follow caret.");
        }

        /// <summary>Tests the TextBox.Copy property.</summary>
        private void TestCopyCommand()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));

            new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd).Text = "bigdeal this is a test";

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2 + "bindeal".Length); // +2 for Paragraph/Run start tag

            _textbox.Selection.Select(_textbox.Document.ContentStart, position);

            _textbox.Selection.Text = "bigdeal";
            CheckSelectedText("bigdeal");
            _textbox.Copy();
        }

        /// <summary> Test the TextBox Commands result </summary>
        private void TestCopyCommandCheck()
        {
            CheckClipboardText("bigdeal");
        }

        /// <summary>Tests the TextBox.Cut property.</summary>
        private void TestCutCommand()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            TextPointer start = _textbox.Document.ContentStart;

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "bigdeal this is a test";

            TextPointer position = start.GetPositionAtOffset(+2 + "bigdeal".Length); // +2 - for Paragraph/Run tags

            _textbox.Selection.Select(start, position);

            _textbox.Selection.Text = "bigdeal";
            CheckSelectedText("bigdeal");
            _textbox.Cut();
        }

        /// <summary> Test the TextBox Commands result </summary>
        private void TestCutCommandCheck()
        {
            CheckClipboardText("bigdeal");
        }

        /// <summary>Tests the TextBox.Paste property.</summary>
        private void TestPasteCommand()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            TextPointer start = _textbox.Document.ContentStart;

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = "what a good deal this is a test";

            TextPointer position = start.GetPositionAtOffset(+2 + "what".Length); // +2 - for Paragraph/Run tags

            _textbox.Selection.Select(start, position);

            _textbox.Selection.Text = "what";
            CheckSelectedText("what");
            _textbox.Paste();
        }

        /// <summary> Test the TextBox Commands result </summary>
        private void TestPasteCommandCheck()
        {
            CheckText("bigdeal a good deal this is a test\r\n");
        }

        private void TestLinesInTextBlock()
        {
            _textBlock.Inlines.Clear();
            _textBlock.Inlines.Add(new Run("text"));
            _textBlock.Inlines.Add(new LineBreak());
            _textBlock.Inlines.Add(new Bold(new Run("bold")));

            _textbox.Document.Blocks.Clear();
            Paragraph paragraph = new Paragraph(new Run("text"));
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add(new Bold(new Run("bold")));
            _textbox.Document.Blocks.Add(paragraph);
            _textbox.Document.Blocks.Add(new Paragraph(new Run("text")));
        }

        private void VerifyLinesInTextBlock()
        {
            //                                  <Run> t     e      x      t      </Run> <LineBreak></LineBreak> <Bold> <Run> b     o      l      d      </Run> </Bold>
            bool[] isAtLineStart = new bool[] { true, true, false, false, false, false, false,     false,       false, true, true, false, false, false, false, false, false };
            for (int i = 0; i < isAtLineStart.Length; i++)
            {
                TextPointer position = _textBlock.ContentStart.GetPositionAtOffset(i);
                AssertEqual(position.IsAtLineStartPosition, isAtLineStart[i], "IsAtLineStart test failed at index " + i + " for TextBlock");
            }

            //                           <Par>  <Run> t     e      x      t      </Run> <LineBreak></LineBreak> <Bold> <Run> b     o      l      d      </Run> </Bold> </Par> <Par>  <Run> t     e      x      t      </Run> </Par>
            isAtLineStart = new bool[] { false, true, true, false, false, false, false, false,     false,       false, true, true, false, false, false, false, false,  false, false, true, true, false, false, false, false, false, false   };
            for (int i = 0; i < isAtLineStart.Length; i++)
            {
                TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(i);
                AssertEqual(position.IsAtLineStartPosition, isAtLineStart[i], "IsAtLineStart test failed at index " + i + " for RichTextBox");
            }
        }

        /// <summary>Tests the TextBox.Text property.</summary>
        private void TestText()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));

            TextPointer start = _textbox.Document.ContentStart;
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            CheckText("\r\n");

            range.Text = "sample text";
            CheckText("sample text\r\n");

            TextPointer position = start.GetPositionAtOffset(+2 + "sam".Length);

            _textbox.Selection.Select(start, position);
            _textbox.Selection.Text = "SAM";
            CheckText("SAMple text\r\n");
        }

        // Tests the GetRectFromTextPosition and GetTextPositionFromPoint APIs.
        private void TestHitTest()
        {
#if false
            TextRange range = new TextRange(_textbox.Selection.Start.TextContainer.Start, _textbox.Selection.Start.TextContainer.End);

            range.Text = LongText;

            // Round trip the first char.

            _textbox.UpdateLayout();

            Rect rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.Document.ContentStartPosition, LogicalDirection.Forward));
            OrientedTextPosition position = _textbox.GetTextPositionFromPoint(new Point(rect.X + rect.Width / 4, rect.Y + rect.Height / 2), false);
            AssertEqual(position.TextPointer, _textbox.Document.ContentStartPosition, "Bad roundtrip from TestHitTest (1)");

            // Again, but this time snap from the left margin.

            rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.Document.ContentStartPosition, LogicalDirection.Forward));
            position = _textbox.GetTextPositionFromPoint(new Point(rect.X - rect.Width / 4, rect.Y + rect.Height / 2), true);
            // The assert below could fail if anyone shrinks the current margins on our TextBox...
            AssertEqual(position.TextPointer, _textbox.Document.ContentStartPosition, "Bad roundtrip from TestHitTest (2)");
#endif
        }

        /// <summary>
        /// Tests the TestCharacterFormatting function by introducing proeprties into a range, and then
        /// verifying that they apply over each character in the range
        /// </summary>
        private void TestRangeFormatting()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            // Enter a test value for range and verify it
            range.Text = "Test character formatting";
            CheckText("Test character formatting\r\n");

            TextPointer boldEndPosition = _textbox.Document.ContentStart.GetPositionAtOffset(+10);

            TextRange boldRange = new TextRange(_textbox.Document.ContentStart, boldEndPosition);
            boldRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            TestCharacterFormatting(boldRange, TextElement.FontWeightProperty, FontWeights.Bold);


            TextPointer italicStartPosition = _textbox.Document.ContentStart.GetPositionAtOffset(+6);

            TextRange italicRange = new TextRange(italicStartPosition, _textbox.Document.ContentEnd);
            italicRange.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            TestCharacterFormatting(italicRange, TextElement.FontStyleProperty, FontStyles.Italic);

            TextRange boldItalicRange = new TextRange(italicStartPosition, boldEndPosition);
            TestCharacterFormatting(boldItalicRange, TextElement.FontWeightProperty, FontWeights.Bold);
            TestCharacterFormatting(boldItalicRange, TextElement.FontStyleProperty, FontStyles.Italic);

            // Clear textbox again for other use
            range.Text = "";
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
        }

        // Series of tests that test mouse selection as the mouse moves around text

        /// <summary>
        /// Enters some text into the textbox and selects all of it with the mouse
        /// </summary>
        private void SelectWithMouse()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));

            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);

            range.Text = "Testing word selection with mouse";
            CheckText("Testing word selection with mouse\r\n");
            _textbox.Selection.Select(_textbox.Document.ContentStart, _textbox.Document.ContentStart);
            CheckSelectedText("");
            _textbox.UpdateLayout();

            Rect startRect = GetScreenCharacterRect(range.Start, LogicalDirection.Forward);
            Rect endRect = GetScreenCharacterRect(range.End, LogicalDirection.Backward);

            Point startPoint = new Point(startRect.X, startRect.Y);
            Point endPoint = new Point(endRect.X, endRect.Y);

            DrtInput.MouseMove((int)startPoint.X, (int)startPoint.Y);
            DrtInput.MouseDown();
            DrtInput.MouseMove((int)endPoint.X, (int)endPoint.Y);
        }

        /// <summary>
        /// Checks that all text has been selected by the previous function and moves the mouse
        /// back for a partial selection
        /// </summary>
        private void CheckMouseSelectionFullText()
        {
            CheckSelectedText("Testing word selection with mouse");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+4);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks partial selection from previous function and moves the mouse across the word boundary
        /// </summary>
        private void CheckMouseSelectionPartialWord()
        {
            CheckSelectedText("Test");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+10);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that mouse movement across the word boundary in previous selection results in full word selection
        /// being enabled. Them moves mouse back within the same word.
        /// </summary>
        private void CheckMouseSelectionFullWords()
        {
            CheckSelectedText("Testing word ");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+9);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that moving mouse backward into a word enables partial word selection. Then moves mouse forward within the
        /// the same word.
        /// </summary>
        private void CheckMouseSelectionFullToPartialWord()
        {
            CheckSelectedText("Testing w");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+11);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that once the mouse is moved within a word to enable partial word selection, moving the mouse
        /// forward within that word boundary also results in partial selection. Then moves mouse forward across word boundaries.
        /// </summary>
        private void CheckMouseSelectionWithinWord()
        {
            CheckSelectedText("Testing wor");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+30);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that mouse movement across the word boundaries in previous function results in selection of full text.
        /// Moves the mouse backwards into a word
        /// </summary>
        private void CheckMouseSelectionPartialToFullText()
        {
            CheckSelectedText("Testing word selection with mouse");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+25);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that moving mouse backward into a word in previous function enables partial word selection.
        /// Then moves mouse further backward into another word.
        /// </summary>
        private void CheckMouseSelectionFullToPartialText()
        {
            CheckSelectedText("Testing word selection wi");
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            TextPointer position = range.Start.GetPositionAtOffset(+17);

            Rect rect = GetScreenCharacterRect(position, LogicalDirection.Backward);
            Point point = new Point(rect.X, rect.Y);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Checks that mouse movement backword from partially selected word into another word  in previous function
        /// enables partial selection of the second word
        private void CheckMouseSelectionPartialToPartialWord()
        {
            CheckSelectedText("Testing word sele");
        }

        /// <summary>
        /// Releases mouse from selection tests and clears textbox.
        /// </summary>
        private void ClearTextBoxAfterMouseSelection()
        {
            DrtInput.MouseUp();
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            _textbox.UpdateLayout();
        }

        // Text merging tests

        /// <summary>
        /// Merges contents of two paragraphs
        /// <P>ppp[</P><P>]qqq</P> --> <P>pppqqq</P>
        /// </summary>
        private void TestTextMergeBetweenParagraphs()
        {
            // Prepare content for a test
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph>ppp</Paragraph><Paragraph>qqq</Paragraph>"));
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), DrtEditing.GetClipboardXaml("<Paragraph><Run>ppp</Run></Paragraph><Paragraph><Run>qqq</Run></Paragraph>"), "'ppp' and 'qqq' expected in different paragraphs");

            // Set up text range to merge
            TextRange rangeToMerge = CreateTextRange(range.Start, 3, 4);
            CheckRangeXaml(rangeToMerge, "<Paragraph><Run>ppp[</Run></Paragraph><Paragraph><Run>]qqq</Run></Paragraph>");

            // Do merge test
            rangeToMerge.Text = String.Empty;
            AssertEqual(rangeToMerge.Text, "", "Range must be empty after merging");
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), DrtEditing.GetClipboardXaml("<Paragraph><Run>pppqqq</Run></Paragraph>"), "'pppqqq' expected in single paragraph");
        }

        /// <summary>
        /// Merges paragraph in front of list with text in list
        /// </summary>
        private void TestTextMergeBeforeListStart()
        {
            // Prepare content for a test
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph>ppp</Paragraph><List><ListItem><Paragraph>qqq</Paragraph></ListItem><ListItem><Paragraph>zzz</Paragraph></ListItem></List>"));
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), DrtEditing.GetClipboardXaml("<Paragraph><Run>ppp</Run></Paragraph><List><ListItem><Paragraph><Run>qqq</Run></Paragraph></ListItem><ListItem><Paragraph><Run>zzz</Run></Paragraph></ListItem></List>"), "'qqq' expected as ListItem");

            // Set up text range to merge
            TextRange rangeToMerge = CreateTextRange(range.Start, 3, 6);
            CheckRangeXaml(rangeToMerge, "<Paragraph><Run>ppp[</Run></Paragraph><List><ListItem><Paragraph><Run>]qqq</Run></Paragraph></ListItem><ListItem><Paragraph><Run>zzz</Run></Paragraph></ListItem></List>");

            // Do merge test
            rangeToMerge.Text = String.Empty;
            AssertEqual(rangeToMerge.Text, "", "Range must be empty after merging");
            DrtEditing.AssertEqualXml(DrtEditing.GetTextRangeXml(range), DrtEditing.GetClipboardXaml("<Paragraph><Run>pppqqq</Run></Paragraph><List><ListItem><Paragraph><Run>zzz</Run></Paragraph></ListItem></List>"), "'pppqqq' expected in first paragraph, 'zzz' expected in list iotem");
        }

        private void TestTableSelection()
        {
            // Set up table and selection range in blank text box
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            Table table = CreateTable(4, 4);
            _textbox.Document.Blocks.Add(table);

            // Build table range
            _textbox.UpdateLayout(); // We need layout to be updated for column information available;
            
            TextRange range = new TextRange(table.RowGroups[0].Rows[1].Cells[2].ContentEnd, table.RowGroups[0].Rows[2].Cells[1].ContentStart);

            // Check that range selected is a rectangle
            AssertCondition(range.Start.CompareTo((table.RowGroups[0].Rows[1].Cells[1].Blocks.FirstBlock as Paragraph).Inlines.FirstInline.ContentStart) == 0, "Range expected to reposition around a rectangular selection");
            AssertCondition(range.End.CompareTo((table.RowGroups[0].Rows[2].Cells[3].Blocks.FirstBlock as Paragraph).Inlines.FirstInline.ContentStart) == 0, "Range expected to reposition around a rectangular selection");

            // Remove table
            _textbox.Document.Blocks.Remove(table);
        }

        // Tests for selection and cursor position after mouse movement or keyboard selection, home and end keys

        /// <summary>
        /// Loads text, formats paragraphs leaving one empty paragraph and clicks mouse to position caret on
        /// the empty paragraph
        /// </summary>
        private void SetSelectionPositionForVerticalCaretMovement()
        {
            // Load the text for the test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Run>Text</Run></Paragraph><Paragraph><Run></Run></Paragraph><Paragraph><Run>Text</Run></Paragraph>"));
            _textbox.UpdateLayout();

            // Find coordinates of empty paragraph
            Rect rect = GetScreenRectFromCharacterOffset(9, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y + rect.Height / 2);

            DrtInput.MouseMove((int)point.X, (int)point.Y);
            DrtInput.MouseDown();
            DrtInput.MouseUp();
        }

        /// <summary>
        /// Tests the selection start position (position of the caret) to see if it matches the position of
        /// mouse click in previous test. Also presses the HOME key to start next test
        /// </summary>
        private void TestSelectionPositionForVerticalCaretMovement()
        {
            // Obtain sleeciton start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Obtain point coordinates where mouse was clicked
            Rect rect = GetScreenRectFromCharacterOffset(9, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y);

            // Verify that cursor is at mouse click point
            AssertEqual(selectionStartPoint, point, "Selection expected to start at mouse click position!");

            // Press the home key
            KeyboardType("{HOME}");
        }

        /// <summary>
        /// Checks the caret position after the HOME key is pressed. On an empty paragraph, caret position
        /// should be unchanged. Also presses the END key for the following test
        /// </summary>
        private void TestCaretMovementOnHome()
        {
            // Verify that cursor does not move when we press HOME on the empty line in previous test
            // Obtain coordinates of selection start and home position
            Rect homeRect = GetScreenRectFromCharacterOffset(9, LogicalDirection.Forward);
            Point homePoint = new Point(homeRect.X, homeRect.Y);
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Verify that selection starts at home point, which is unchanged from previous test
            AssertEqual(selectionStartPoint, homePoint, "Selection not at expected position after pressing HOME key!");

            // Press the end key
            KeyboardType("{END}");
        }

        /// <summary>
        /// Checks the caret position after END is pressed in previous test. On empty paragraph caret position should
        /// be unchanged. Also presses UP key for next test.
        /// </summary>
        private void TestCaretMovementOnEnd()
        {
            // Verify that pressing END key on empty line also does not cause any change in selection position
            Rect endRect = GetScreenRectFromCharacterOffset(9, LogicalDirection.Forward);
            Point endPoint = new Point(endRect.X, endRect.Y);
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Verify that selection starts at endPoint, which is unchanged from previous test
            AssertEqual(selectionStartPoint, endPoint, "Selection not at expected position after pressing END key!");

            // Press the up key
            KeyboardType("{UP}");
        }

        /// <summary>
        /// Checks that selection start moves to the previous line when UP key is pressed in the previous test. Also
        /// presses the DOWN key twice to navigate to last line for next test
        /// </summary>
        private void TestCaretMovementOnKeyUp()
        {
            // Obtain coordinates of point on previous line where selection should be after pressing UP
            // And of actual selection coordinates
            Rect upRect = GetScreenRectFromCharacterOffset(2, LogicalDirection.Forward);
            Point upPoint = new Point(upRect.X, upRect.Y);
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Verify that selection starts at upPoint, which is on the previous line
            AssertEqual(selectionStartPoint, upPoint, "Selection not at expected position after pressing END key!");

            // Press the down key twice to go to the last line
            KeyboardType("{DOWN}");
            KeyboardType("{DOWN}");
        }

        /// <summary>
        /// Checks that selection start moves down two lines, to the last line, after DOWN key is pressed
        /// twice in previous test
        /// </summary>
        private void TestCaretMovementOnKeyDown()
        {
            // Obtain coordinates of point on third line where selection should be after pressing DOWN
            // And of actual selection coordinates
            Rect downRect = GetScreenRectFromCharacterOffset(14, LogicalDirection.Forward);
            Point downPoint = new Point(downRect.X, downRect.Y);
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Verify that selection starts at upPoint, which is on the previous line
            AssertEqual(selectionStartPoint, downPoint, "Selection not at expected position after pressing END key!");
        }

        /// <summary>
        /// Loads four-line text for testing selection from the jeyboard using SHIFT+UP and SHIFT+DOWN. Also
        /// positions the caret on the first line by clicking the mouse
        /// </summary>
        private void LoadMultiLineTextForVerticalCaretSelection()
        {
            // Load the text for the test
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Run>Long long long line</Run></Paragraph><Paragraph><Run>Long long line</Run></Paragraph><Paragraph><Run>Long line</Run></Paragraph><Paragraph><Run>Long</Run></Paragraph>"));
            _textbox.UpdateLayout();

            // Get coordinates of a point on the first line and click mouse to start selection
            Rect rect = GetScreenRectFromCharacterOffset(4, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y + rect.Height / 2);
            DrtInput.MouseMove((int)point.X, (int)point.Y);
            DrtInput.MouseDown();
            DrtInput.MouseUp();
        }

        /// <summary>
        /// Tests the caret position caused by mouse click in earlier tests. Presses the DOWN key multiple times,
        /// then the UP key an equal number of times for the next test
        /// </summary>
        private void TestSelectionPositionMultiLineText()
        {
            // Obtain sleeciton start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Obtain point coordinates where mouse was clicked
            Rect rect = GetScreenRectFromCharacterOffset(4, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y);

            // Verify that cursor is at mouse click point
            AssertEqual(selectionStartPoint, point, "Selection expected to start at mouse click position!");

            // Press the DOWN key repeatedly and then go back up
            KeyboardType("{DOWN}");
            KeyboardType("{DOWN}");
            KeyboardType("{DOWN}");
            KeyboardType("{UP}");
            KeyboardType("{UP}");
            KeyboardType("{UP}");
        }

        /// <summary>
        /// After multiple DOWN keys followed by equal number of UP movements in previous test, caret should return
        /// to its original position. This test checks the position and presses SHIFT+DOWN key to select one line
        /// </summary>
        private void TestCaretMovementOnMultipleKeyDownAndKeyUp()
        {
            // Obtain sleeciton start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Obtain point coordinates where mouse was clicked
            Rect rect = GetScreenRectFromCharacterOffset(2 + "Lo".Length, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y);

            // Verify that cursor is at mouse click point
            AssertEqual(selectionStartPoint, point, "Selection expected to start at mouse click position!");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after one line is selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownOneLine()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after two lines are selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownTwoLines()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Long long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after three lines are selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownThreeLines()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Long long line".Length + 4 + "Long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Checks that selecting a takes the selection end to the end of the _textbox. Also presses SHIFT+UP
        /// to start reversing selection
        /// </summary>
        private void TestKeyboardSelectionDownTextboxEnd()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            // Normalize _textbox.Document.ContentEnd position
            TextRange normalizedSelectionEnd = new TextRange(_textbox.Selection.End, _textbox.Selection.End);
            Rect selectionEndRect = GetScreenCharacterRect(normalizedSelectionEnd.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point. These should now be the end of the text box contents.
            // Normalize _textbox.Document.ContentEnd position
            TextRange normalizedContentEnd = new TextRange(_textbox.Document.ContentEnd, _textbox.Document.ContentEnd);
            Rect expectedSelectionEndRect = GetScreenCharacterRect(normalizedContentEnd.End, LogicalDirection.Backward);
            // Expected end point is one line below start
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after first SHIFT+UP, selection reverts to its earlier position when 3 lines were selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpOneLine()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position, which should be 51 from
            // earlier selection
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Long long line".Length + 4 + "Long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after second SHIFT+UP, selection reverts to its earlier position when 2 lines were selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpTwoLines()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Long long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after third SHIFT+UP, selection reverts to its earlier position when one line was selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpThreeLines()
        {
            // Obtain sleeciton end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point from it's character position
            Rect expectedSelectionEndRect = GetScreenRectFromCharacterOffset(2 + "Long long long line".Length + 4 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionEndPoint = new Point(expectedSelectionEndRect.X, expectedSelectionEndRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after fourth SHIFT+UP, selection is completely reversed and caret is now at its original
        /// position before the first SHIFT + DOWN. Presses SHIFT + UP to move selection to start of text
        /// </summary>
        private void TestKeyboardSelectionUpFourLines()
        {
            // Obtain selction end coordinates, since start coordinates stay the same
            Rect selectionEndRect = GetScreenCharacterRect(_textbox.Selection.End, LogicalDirection.Forward);
            Point selectionEndPoint = new Point(selectionEndRect.X, selectionEndRect.Y);

            // Obtain coordinates of expected point. For this case selection end should be exactly the same as the start
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            // Expected end point is one line below start
            Point expectedSelectionEndPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            AssertEqual(selectionEndPoint, expectedSelectionEndPoint, "Selection does not end at expected point");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after fifth SHIFT+UP, selection starts at beginning of text box content. Presses
        /// SHIFT+DOWN for next test
        /// </summary>
        private void TestKeyboardSelectionUpTextboxStart()
        {
            // For this case, the selection should be at the start of text box content
            // Obtain selction start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            Rect textboxStartRect = GetScreenRectFromCharacterOffset(2, LogicalDirection.Forward);
            Point textboxStartPoint = new Point(textboxStartRect.X, textboxStartRect.Y);

            // Verify that selection is now at start of text box content
            AssertEqual(selectionStartPoint, textboxStartPoint, "Selection start not equal to end");

            // Press SHIFT + DOWN keys to see if selection is remembered
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// After SHIFT+DOWN is pressed in previous test, selection should return to its original position.
        /// </summary>
        private void TestKeyboardSelectionDownToFirstPosition()
        {
            // Obtain selction start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            Rect expectedSelectionStartRect = GetScreenRectFromCharacterOffset(2 + "Lo".Length, LogicalDirection.Forward);
            Point expectedSelectionStartPoint = new Point(expectedSelectionStartRect.X, expectedSelectionStartRect.Y);

            // Verify that cursor is at mouse click point
            AssertEqual(selectionStartPoint, expectedSelectionStartPoint, "Selection does not start where expected");
        }

        /// <summary>
        /// Loads and formats wrapping text to test overlap in _textbox. Positions the caret on the last line of
        /// wrapping text
        /// </summary>
        private void LoadWrappingText()
        {
            LoadXamlContent(DrtEditing.GetClipboardXaml("<Paragraph><Run>Test case to create different overlapping styles. Thistexthasnospacesandisveryverylong</Run></Paragraph>"));
            _textbox.UpdateLayout();

            // Obtain coordinates of a point that will be somewhere close to the end of the third line
            // It must be close to the end because there should not be any text above it on the second line
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            Rect rect = GetScreenRectFromCharacterOffset(2 + range.Text.Length - "ryverylong".Length, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y + rect.Height / 2);

            // Click mouse on point on third line
            DrtInput.MouseMove((int)point.X, (int)point.Y);
            DrtInput.MouseDown();
            DrtInput.MouseUp();
        }

        /// <summary>
        /// Tests position of selection in wrapping text after mouse click. Also presses UP key for next test
        /// </summary>
        private void TestSelectionPositionWrappingText()
        {
            // Obtain selction start coordinates
            Rect selectionStartRect = GetScreenCharacterRect(_textbox.Selection.Start, LogicalDirection.Forward);
            Point selectionStartPoint = new Point(selectionStartRect.X, selectionStartRect.Y);

            // Verify them against point where mouse was clicked
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            Rect rect = GetScreenRectFromCharacterOffset(2 + range.Text.Length - "ryverylong".Length, LogicalDirection.Forward);
            Point point = new Point(rect.X, rect.Y);
            AssertEqual(selectionStartPoint, point, "Selection does not start where expected");

            // Go up one line
            KeyboardType("{UP}");
        }

        /// <summary>
        /// Checks that the caret is positioned at the end of the second line after key up from the third line
        /// </summary>
        private void TestCaretMovementOnKeyUpWrappingText()
        {
            Object caretElement = _reflectionCaretElement_Debug_CaretElement.GetValue(null, null);
            DRT.Assert(caretElement != null, "CaretElement not found");
            Point caretPositionInRenderScope = new Point((double)_reflectionCaretElement_Left.GetValue(caretElement), (double)_reflectionCaretElement_Top.GetValue(caretElement));
            Point caretPosition = GetScreenPointFromRenderScopePoint(caretPositionInRenderScope);

            // Cast to int
            caretPosition.X = (int)caretPosition.X;
            caretPosition.Y = (int)caretPosition.Y;

            // Obtain coordinates of last position in the second line. Caret position must be to the right of this
            Rect approximateCaretPositionRect = GetScreenRectFromCharacterOffset(2 + "Test case to create different overlapping styles. ".Length - 1, LogicalDirection.Backward);

            // We expect the Y coordinates to be the same, and the X coordinates of caret position should be greater than our approximation
            AssertEqual(caretPosition.Y, approximateCaretPositionRect.Y, "Caret's Y-coordinate not at expected position");
            AssertSecondGreater(approximateCaretPositionRect.X, caretPosition.X, "Caret's X-coordinate not at expected position");

            // Key down to third line, the press home key
            KeyboardType("{DOWN}");
            KeyboardType("{HOME}");
        }

        /// <summary>
        /// Checks that the caret is returned to its position before the KeyUp when KeyDown is pressed,
        /// and presses the HOME key for next test
        /// </summary>
        private void TestCaretMovementOnHomeWrappingText()
        {
            Object caretElement = _reflectionCaretElement_Debug_CaretElement.GetValue(null, null);
            DRT.Assert(caretElement != null, "CaretElement not found");
            double caretLeftOffset = (double)_reflectionCaretElement_Left.GetValue(caretElement);
            double caretTopOffset = (double)_reflectionCaretElement_Top.GetValue(caretElement);
            double caretHeight = (double)_reflectionCaretElement_Height.GetValue(caretElement);

            // Get screen point for caret element
            Point caretPositionInRenderScope = new Point((double)_reflectionCaretElement_Left.GetValue(caretElement), (double)_reflectionCaretElement_Top.GetValue(caretElement));
            Point caretPosition = GetScreenPointFromRenderScopePoint(caretPositionInRenderScope);

            // Cast to int
            caretPosition.X = (int)caretPosition.X;
            caretPosition.Y = (int)caretPosition.Y;

            // Obtain coordinates where we expect caret to be, at the start of the third line
            Rect expectedCaretPositionRect = GetScreenRectFromCharacterOffset(2 + "Test case to create different overlapping styles. ".Length, LogicalDirection.Forward);
            Point expectedCaretPosition = new Point(expectedCaretPositionRect.X, expectedCaretPositionRect.Y);

            AssertEqual(caretPosition, expectedCaretPosition, "Caret position not where expected");
        }

        // Tests that the TextBox's content is preserved if the style changes
        private void TestStyleChange()
        {
            // Start from empty copntent
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));

            // Set initial content
            _textbox.Document.Blocks.Clear();
            _textbox.Document.Blocks.Add(new Paragraph(new Run("Testing style change")));

            // Get a paragraph
            Paragraph paragraph = (Paragraph)_textbox.Document.Blocks.FirstBlock;

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(+2 + "Testing ".Length);
            TextPointer position2 = position.GetPositionAtOffset("style".Length);

            new TextRange(position, position2).ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            Button btn = new Button();
            btn.Width = 100;
            btn.Height = 30;
            paragraph.Inlines.Add(new InlineUIContainer(btn));
            object parent = btn.Parent;

            string initialContent = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd).Text;

            // Change the style.  Same as default TextBox style, but yellow background
            Style ds = new Style(typeof(RichTextBox));
            Brush b1 = Brushes.Yellow;
            b1.Freeze();
            Brush b2 = Brushes.Gray;
            b2.Freeze();
            ds.Setters.Add (new Setter(RichTextBox.BackgroundProperty, b1));
            ds.Setters.Add (new Setter(Border.BorderBrushProperty, b2));
            ds.Setters.Add (new Setter(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None));

            // default font (from system)
            ds.Setters.Add (new Setter(RichTextBox.ForegroundProperty, SystemColors.ControlTextBrush));
            ds.Setters.Add (new Setter(RichTextBox.FontFamilyProperty, SystemFonts.MessageFontFamily));
            ds.Setters.Add (new Setter(RichTextBox.FontSizeProperty, SystemFonts.MessageFontSize));
            ds.Setters.Add (new Setter(RichTextBox.FontStyleProperty, SystemFonts.MessageFontStyle));
            ds.Setters.Add (new Setter(RichTextBox.FontWeightProperty, SystemFonts.MessageFontWeight));
            ds.Setters.Add (new Setter(RichTextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Left));

            FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
            canvas.SetValue(Control.ForegroundProperty, new TemplateBindingExtension(RichTextBox.ForegroundProperty));
            canvas.SetValue(Control.FontSizeProperty, new TemplateBindingExtension(RichTextBox.FontSizeProperty));
            canvas.SetValue(Control.FontFamilyProperty, new TemplateBindingExtension(RichTextBox.FontFamilyProperty));
            canvas.SetValue(RichTextBox.WidthProperty, new TemplateBindingExtension(RichTextBox.WidthProperty));
            canvas.SetValue(RichTextBox.HeightProperty, new TemplateBindingExtension(RichTextBox.HeightProperty));
            canvas.SetValue(Canvas.StyleProperty, null);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Rectangle));
            border.SetValue(Shape.StrokeProperty, new TemplateBindingExtension(Border.BorderBrushProperty));
            border.SetValue(Shape.FillProperty, new TemplateBindingExtension(Border.BackgroundProperty));
            border.SetValue(Rectangle.RadiusXProperty, 4.0);
            border.SetValue(Rectangle.RadiusYProperty, 4.0);
            border.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory shadow = new FrameworkElementFactory(typeof(Rectangle));
            LinearGradientBrush b = new LinearGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x40, 0x00, 0x00, 0x00), 0.0));
            b.GradientStops.Add(new GradientStop(Colors.Transparent, 0.85));
            b.StartPoint = new Point(0, 0);
            b.EndPoint = new Point(0, 1);
            b.Freeze();
            shadow.SetValue(Shape.FillProperty, b);
            shadow.SetValue(Shape.StrokeProperty, Brushes.Transparent);
            shadow.SetValue(Rectangle.HeightProperty, 4d);
            shadow.SetValue(Rectangle.RadiusXProperty, 4.0);
            shadow.SetValue(Rectangle.RadiusYProperty, 4.0);
            shadow.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory shadowLeft = new FrameworkElementFactory(typeof(Rectangle));
            b = new LinearGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x40, 0x00, 0x00, 0x00), 0.0));
            b.GradientStops.Add(new GradientStop(Colors.Transparent, 0.85));
            b.StartPoint = new Point(0.0, 0.0);
            b.EndPoint = new Point(1.0, 0.0);
            b.Freeze();
            shadowLeft.SetValue(Shape.FillProperty, b);
            shadowLeft.SetValue(Shape.StrokeProperty, Brushes.Transparent);
            shadowLeft.SetValue(Rectangle.WidthProperty, 4d);
            shadowLeft.SetValue(Rectangle.RadiusXProperty, 4.0);
            shadowLeft.SetValue(Rectangle.RadiusYProperty, 4.0);
            shadowLeft.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory sv = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
            sv.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBindingExtension(ScrollViewer.VerticalScrollBarVisibilityProperty));

            canvas.AppendChild(border);
            canvas.AppendChild(shadow);
            canvas.AppendChild(shadowLeft);
            canvas.AppendChild(sv);
            ControlTemplate template = new ControlTemplate(typeof(RichTextBox));
            template.VisualTree = canvas;
            ds.Setters.Add(new Setter(Control.TemplateProperty, template));

            _textbox.Style = ds;
            _textbox.ApplyTemplate();
            AssertEqual(new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd).Text, initialContent, "Plain text not intact after style change.");
            position = _textbox.Document.ContentStart.GetPositionAtOffset(+2 + "Testing ".Length);
            AssertEqual(position.GetPointerContext(LogicalDirection.Forward), TextPointerContext.ElementEnd, "Bold not intact after style change.");
            position = position.GetPositionAtOffset(1);
            AssertEqual(position.GetPointerContext(LogicalDirection.Forward), TextPointerContext.ElementStart, "Bold not intact after style change.");
        }

        // Tests TextPointer.DeleteTextInRun() method
        private void TestDeleteTextInRun()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run("delete1textdelete2")));
            CheckText("delete1textdelete2\r\n");

            // Delete +ve
            TextPointer tp1 = _textbox.Document.ContentStart.GetPositionAtOffset(2); //2 for Paragraph/Run start
            int deletedChars = tp1.DeleteTextInRun("delete1".Length);
            AssertEqual(deletedChars, "delete1".Length, "Incorrect number of chars deleted");
            CheckText("textdelete2\r\n");

            // Delete -ve
            tp1 = _textbox.Document.ContentEnd.GetPositionAtOffset(-2);
            deletedChars = tp1.DeleteTextInRun(-"delete2".Length);
            AssertEqual(deletedChars, -"delete2".Length, "Incorrect number of chars deleted");
            CheckText("text\r\n");

            // Delete 0 inside non-empty run
            tp1 = _textbox.Document.ContentStart.GetPositionAtOffset(+2+1);
            deletedChars = tp1.DeleteTextInRun(0);
            AssertEqual(deletedChars, 0, "Incorrect number of chars deleted");
            CheckText("text\r\n");

            // Delete 0 inside empty run
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            tp1 = _textbox.Document.ContentStart.GetPositionAtOffset(+2);
            deletedChars = tp1.DeleteTextInRun(0);
            AssertEqual(deletedChars, 0, "Incorrect number of chars deleted");
            CheckText("\r\n");
        }

        private void TestSpellerAPI()
        {
            TestSpellerAPIWithDefaultDictionary();

            CleanupStaleDictionaries();
            TestSpellWithDictionaryFromPackUri();
            TestSpellWithDictionaryFromFile();
        }

        // Tests the speller OM.
        private void TestSpellerAPIWithDefaultDictionary()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run("This iz badz tezt.")));
            _textbox.SpellCheck.IsEnabled = true;

            // Do one run with a dirty document.
            TestSpellerAPIWorker();

            // Do a second run with an evaluated document (speller engine doesn't run).
            TestSpellerAPIWorker();

            _textbox.SpellCheck.IsEnabled = false;
        }

        // Tests the speller OM.
        private void TestSpellerAPIWorker()
        {
            TextRange errorRange;
            int count = 0;

            for (TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                 errorPosition != null;
                 errorPosition = _textbox.GetNextSpellingErrorPosition(errorRange.End, LogicalDirection.Forward))
            {
                SpellingError spellingError = _textbox.GetSpellingError(errorPosition);
                DRT.Assert(spellingError != null);
                errorRange = _textbox.GetSpellingErrorRange(errorPosition);

                if (count == 0)
                {
                    DRT.Assert(errorRange.Text == "iz");
                }
                else if (count == 1)
                {
                    DRT.Assert(errorRange.Text == "badz");
                }
                else
                {
                    DRT.Assert(errorRange.Text == "tezt");
                }

                count++;
            }

            DRT.Assert(count == 3);

            count = 0;

            for (TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentEnd, LogicalDirection.Backward);
                 errorPosition != null;
                 errorPosition = _textbox.GetNextSpellingErrorPosition(errorRange.Start, LogicalDirection.Backward))
            {
                SpellingError spellingError = _textbox.GetSpellingError(errorPosition);
                DRT.Assert(spellingError != null);
                errorRange = _textbox.GetSpellingErrorRange(errorPosition);

                if (count == 0)
                {
                    DRT.Assert(errorRange.Text == "tezt");
                }
                else if (count == 1)
                {
                    DRT.Assert(errorRange.Text == "badz");
                }
                else
                {
                    DRT.Assert(errorRange.Text == "iz");
                }

                count++;
            }

            DRT.Assert(count == 3);
        }

        /// <summary>
        /// Cleanup stale %temp%\*.dic and %temp%\*.tmp files
        /// Also cleanup keys under HKCU\Software\Microsoft\Spelling\Dictionaries
        /// The net effect would be to ensure that ISpellChecker COM server has no
        /// custom dictionaries registered with it.
        /// </summary>
        private void CleanupStaleDictionaries()
        {
            try
            {
                var tempPath = System.IO.Path.GetTempPath();

                var dictionaryFiles = System.IO.Directory.EnumerateFiles(tempPath, @"*.dic");

                foreach (var file in dictionaryFiles)
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch { }
                }

                var tmpFiles = System.IO.Directory.EnumerateFiles(tempPath, @"*.tmp");
                foreach (var file in tmpFiles)
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch { }
                }

                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Spelling\Dictionaries", writable: true);
                foreach (string valueName in key.GetValueNames())
                {
                    try
                    {
                        key.DeleteValue(valueName);
                    }
                    catch { }
                }
            }
            catch
            {
                // do nothing
            }
        }

        private void TestSpellWithDictionaryFromPackUri()
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run("CustDictAWordA CustDictAWordB")));

            // Wrap in try-finally and ensure that custom dictionaries are explicitly
            // released. Custom dictionaries on Win8.1 and above are persistently registered
            // to the user and remain registered until they are explicitly deregistered.
            // Before the start of the test, we try to cleanup all previously registered
            // custom dictionaries, but we need to explicilty release them so that
            // those registered in one test case  (and left stale) do not interfere
            // with the next test case
            try
            {
                // Check that word from custom dictionary is not recognized while dictionary isn't loaded.
                {
                    _textbox.SpellCheck.IsEnabled = true;
                    TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    SpellingError spellingError = _textbox.GetSpellingError(errorPosition);
                    DRT.Assert(spellingError != null);
                    TextRange errorRange = _textbox.GetSpellingErrorRange(errorPosition);
                    DRT.Assert(errorRange.Text == "CustDictAWordA");
                }

                // Now Load dictionary and make sure that same word is considered valid now.
                {
                    _textbox.SpellCheck.IsEnabled = true;
                    Uri[] uris = new Uri[] { new Uri("pack://application:,,,/CustomDict1.lex") };
                    _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);


                    TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);
                }

                // Set custom dictionary, then unset SpellCheck.IsEnabled
                // and make sure that the custom word becomes invalid.
                {
                    _textbox.SpellCheck.IsEnabled = true;
                    Uri[] uris = new Uri[] { new Uri("pack://application:,,,/CustomDict1.lex") };
                    _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);

                    TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);

                    _textbox.SpellCheck.IsEnabled = false;
                    System.Windows.Controls.SpellCheck.SetIsEnabled(_textbox, false);
                    errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);// no spell checking available, so no error should be reported.
                }

                // SpellCheck.IsEnabled = false, set custom dictionary, then SpellCheck.IsEnabled = true
                // and make sure that the custom word becomes VALID.
                {
                    _textbox.SpellCheck.IsEnabled = false;
                    Uri[] uris = new Uri[] { new Uri("pack://application:,,,/CustomDict1.lex") };
                    _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);

                    TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);// spell check is disabled so we shouldn't get any spelling errors.

                    // set IsEnabled = true and make sure the word became valid.
                    _textbox.SpellCheck.IsEnabled = true;
                    errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);
                }

                // Set dictionary, then remove it while SpellCheck.IsEnabled == true and make sure that word becomes INVALID.
                {
                    _textbox.SpellCheck.IsEnabled = true;
                    Uri[] uris = new Uri[] { new Uri("pack://application:,,,/CustomDict1.lex") };
                    _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);

                    TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition == null);// Dictionary is loaded so we should not get any error.

                    // clear all dictionaries
                    _textbox.SpellCheck.CustomDictionaries.Clear();

                    errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                    DRT.Assert(errorPosition != null);
                    SpellingError spellingError = _textbox.GetSpellingError(errorPosition);
                    DRT.Assert(spellingError != null);
                    TextRange errorRange = _textbox.GetSpellingErrorRange(errorPosition);
                    DRT.Assert(errorRange.Text == "CustDictAWordA");
                }
            }
            finally
            {
                // Make sure that all custom-dictionaries are released if there is a failure
                _textbox.SpellCheck.CustomDictionaries.Clear();
                _textbox.SpellCheck.IsEnabled = false;
            }
        }

        private void TestSpellWithDictionaryFromFile()
        {
            // load dictionary from a separate file specified by relative path
            _textbox.Document = new FlowDocument(new Paragraph(new Run("CustDictBWordA")));

            // Wrap in try-finally and ensure that custom dictionaries are explicitly
            // released. Custom dictionaries on Win8.1 and above are persistently registered
            // to the user and remain registered until they are explicitly deregistered.
            // Before the start of the test, we try to cleanup all previously registered
            // custom dictionaries, but we need to explicilty release them so that
            // those registered in one test case  (and left stale) do not interfere
            // with the next test case
            try
            {
                _textbox.SpellCheck.IsEnabled = true;

                // .Net BCL introduced a breaking change in 4.6.2 (see Regression_Bug267),
                // so do some gymnastics to work around it.
                const string FilePrefix = @"file:///";
                string codebase = Assembly.GetExecutingAssembly().Location;
                bool startsWithFile = codebase.StartsWith(FilePrefix);
                if (startsWithFile) codebase = codebase.Substring(FilePrefix.Length);
                string path = System.IO.Path.GetDirectoryName(codebase);
                if (startsWithFile) path = FilePrefix + path;
                path = path + @"\DrtFiles\Editing\CustomDict2.lex";

                Uri[] uris = new Uri[] { new Uri(path, UriKind.RelativeOrAbsolute) };
                _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);

                TextPointer errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                DRT.Assert(errorPosition == null);// content is correct

                // Related to Part1 Regression_Bug894. Replace the Document with a new one while SpellCheck.IsEnabled == true
                // and make sure that custom dictionary get loaded properly.
                _textbox.SpellCheck.IsEnabled = true;
                _textbox.Document = new FlowDocument(new Paragraph(new Run("CustDictBWordA")));
                _textbox.SpellCheck.CustomDictionaries.Add(uris[0]);

                errorPosition = _textbox.GetNextSpellingErrorPosition(_textbox.Document.ContentStart, LogicalDirection.Forward);
                DRT.Assert(errorPosition == null);// content is correct
            }
            finally
            {
                // Make sure that all custom-dictionaries are released if there is a failure
                _textbox.SpellCheck.CustomDictionaries.Clear();
                _textbox.SpellCheck.IsEnabled = false;
            }
        }

        // Helpers

        /// <summary>
        /// Loads textbox with given content
        /// </summary>
        /// <param name="s">
        /// String specifying text content to be loaded
        /// </param>
        private void LoadTextContent(string text)
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            range.Text = text;
            CheckText(text);
        }

        /// <summary>
        /// Loads textbox with given content
        /// </summary>
        /// <param name="s">
        /// String specifying text content to be loaded
        /// </param>
        private void LoadXamlContent(string s)
        {
            _textbox.Document = new FlowDocument(new Paragraph(new Run()));
            TextRange range = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            DrtEditing.SetTextRangeXml(range, s);
        }

        /// <summary>Emulates typing on the keyboard.</summary>
        /// <param name='text'>Text to type.</param>
        /// <remarks>
        /// Case is not respected - everything goes in lowercase.
        /// To get uppercase characters, add a "+" in front of the
        /// character. The original design had the "+" toggle the
        /// shift state, but by resetting it we make text string
        /// compatible with CLR's SendKeys.Send.
        /// <para />
        /// Eg, to type "Hello, WORLD!", pass "+hello, +W+O+R+L+D+1"
        /// <para />
        /// This method has not been globalized to keep it simple.
        /// Non-US keyboard may break this functionality.
        /// </remarks>
        private void KeyboardType(string text)
        {
            DrtInput.KeyboardType(text);
        }

        /// <summary>Text guaranteed to exceed one line.</summary>
        /// <remarks>
        /// Conceptually a string constant, but the compiler can have problems
        /// with the LongText and MultiPageText constants.
        /// </remarks>
        private string LongText
        {
            get
            {
                return
                "Text that is hopefully long enough to wrap " +
                "no matter what the default width of the window " +
                "is, and while we are taking variables into account, " +
                "why not mention that even the screen resolution or " +
                "the default font sizes might be enought to break " +
                "this unless it's long enough - and by now I hope " +
                "we all agree this is a reasonable length.";
            }
        }

        /// <summary>Text guaranteed to exceed one page.</summary>
        /// <remarks>
        /// Conceptually a string constant, but the compiler can have problems
        /// with the LongText and MultiPageText constants.
        /// </remarks>
        private string MultiPageText
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(490 * 8);
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(LongText);
                    sb.Append("\n\n\n");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates a text range within another text range or a textbox. Returns a TextRange with the specified length and
        /// offset from the anchor position
        /// </summary>
        /// <param name="anchor">
        /// TextPosition that is used to determine the offset of the text range from its parent. The text range positions are
        /// calculated with respect to this parameter
        /// </param>
        /// <param name="offset">
        /// Offset for the text range start with respect to the anchor position
        /// </param>
        /// <param name="length">
        /// Length (number of TextPositions) of the text range
        /// </param>
        private TextRange CreateTextRange(TextPointer anchor, int offset, int length)
        {
            TextPointer start = anchor.GetPositionAtOffset(+offset);

            TextPointer end = start.GetPositionAtOffset(+length);

            TextRange range = new TextRange(start, end);
            return range;
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
            source = PresentationSource.FromVisual(_textbox);
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
        private Point GetScreenPoint(Point point)
        {
            Point transformedPoint = new Point(point.X, point.Y);

            PresentationSource source;
            IWin32Window win32Window;
            GetVisualInfo(out source, out win32Window);
            CompositionTarget compositionTarget = source.CompositionTarget;

            GeneralTransform transform = _textbox.TransformToAncestor(compositionTarget.RootVisual);
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
            AssertCondition(renderScope.GetType() == DrtEditing.TextFlowType, "RenderScope for RichTextBox must be TextFlow");

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

            Point rectTopLeft = new Point(rect.Left, rect.Top);
            Point rectBottomRight = new Point(rect.Right, rect.Bottom);

            GeneralTransform transform = _textbox.TransformToAncestor(compositionTarget.RootVisual);
            transform.TryTransform(rectTopLeft, out rectTopLeft);
            transform.TryTransform(rectBottomRight, out rectBottomRight);

            // Recalculate startRect and endRect based on transformation to screen
            Rect transformedRect = TransformRootRectToScreenCoordinates(rectTopLeft, rectBottomRight, win32Window, source);

            return transformedRect;
        }

        /// <summary>
        /// Returns the the screen rect for the character at a specified offset
        /// </summary>
        /// <param name="offset">
        /// Integer offset from start of _textbox
        /// </param>
        /// <param name="direction">
        /// Logical direction to be used with the text pointer at that offset to get the rect
        /// </param>
        private Rect GetScreenRectFromCharacterOffset(int offset, LogicalDirection direction)
        {
            // Offset is always from start of text box, so it must be >= 0
            DRT.Assert(offset >= 0, "Offset >+ 0 expected");

            // Get the TextPointer corresponding to the offset
            TextPointer position = GetTextPointerFromCharacterOffset(offset);

            // Get screen character rect for this pointer using backward context, since we want the
            // position at that offset e.g. offset 4 means we want the 4th character
            Rect rect = GetScreenCharacterRect(position, direction);

            return rect;
        }

        /// <summary>
        /// Returns the the TextPointer for the character at a specified offset
        /// </summary>
        /// <param name="offset">
        /// Integer offset from start of _textbox
        /// </param>
        private TextPointer GetTextPointerFromCharacterOffset(int offset)
        {
            // Offset is always from start of text box, so it must be >= 0
            DRT.Assert(offset >= 0, "Offset >+ 0 expected");

            TextPointer position = _textbox.Document.ContentStart.GetPositionAtOffset(offset);

            return position;
        }

        /// <summary>
        /// Returns Rect representing rectangle for the TextPointer at specified offset in RenderScope element.
        /// NOTE:We do not need to transform this rect to screen here because this function is intended to provide
        /// offset of RenderScope (TextFlow) relative to parent (RichTextBox)
        /// </summary>
        /// <param name="offset">
        /// Distance of position from start of RenderScope
        /// </param>
        /// <param name="direction">
        /// Logical direction of pointer context
        /// </param>
        private Rect GetTextPointerRectInRenderScope(int offset, LogicalDirection direction)
        {
            // Obtain RenderScope element so that we can add it's offset to caret offset. Element must be
            // TextFlow for RichTextBox
            FrameworkElement renderScope = (FrameworkElement)_reflectionCaretElement_Debug_RenderScope.GetValue(null, null);
            AssertCondition(renderScope.GetType() == DrtEditing.TextFlowType, "RenderScope for RichTextBox must be TextFlow");

            // Obtain starting TextPointer of RenderScope and move to the required offset
            TextPointer renderScopePosition = DrtEditing.TextFlowContentStart(renderScope).GetPositionAtOffset(offset);

            return renderScopePosition.GetCharacterRect(direction);
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

            AssertCondition(rectBottom >= rectTop, "Verifying rectangle coordinates");
            AssertCondition(rectRight >= rectLeft, "Verifying Rectangle coordinates");

            int rectWidth = rectRight - rectLeft;
            int rectHeight = rectBottom - rectTop;

            rect = new Rect(rectLeft, rectTop, rectWidth, rectHeight);
            return rect;
        }

        /// <summary>
        /// Tests characters with a range for a specific value of a given DependencyProperty
        /// </summary>
        /// <param name="range">
        /// TextRange representing range containing characters to be tested
        /// </param>
        /// <param name="property">
        /// DependencyProperty representing property whose value is to be checked
        /// </param>
        /// <param name="propertyValue">
        /// object representing value to be checked for
        /// </param>
        private void TestCharacterFormatting(TextRange range, DependencyProperty property, object propertyValue)
        {
            TextPointer position = range.Start;

            while (position.CompareTo(range.End) < 0)
            {
                // Check formatting only for characters in the forward direction
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text || //
                    position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    // Check for the property value
                    AssertEqual(InheritedPropertyValueFound(position, property, propertyValue), true, "Property value not found");
                }
                position = position.GetPositionAtOffset(+1);
            }
        }

        /// <summary>
        /// Checks a character for a specified value of a given localDependencyProperty. Returns true if the value of the specified property
        /// possessed by the given character is the same as the specified value
        /// </summary>
        /// <param name="position">
        /// TextPointer whose context is the character to be checked
        /// </param>
        /// <param name="property">
        /// DependencyProperty to be checked for the specified value
        /// </param>
        /// <param name="propertyValue">
        /// object representing the value we want to check for
        /// </param>
        private bool LocalPropertyValueFound(TextPointer position, DependencyProperty property, object propertyValue)
        {
            AssertCondition(propertyValue != null, "propertyValue is null!");
            return propertyValue.Equals(position.Parent.ReadLocalValue(property));
        }

        /// <summary>
        /// Checks a character for a specified value of a given DependencyProperty that may be local or inherited.
        /// Returns true if the value of the specified property
        /// possessed by the given character is the same as the specified value
        /// </summary>
        /// <param name="position">
        /// TextPointer whose context is the character to be checked
        /// </param>
        /// <param name="property">
        /// DependencyProperty to be checked for the specified value
        /// </param>
        /// <param name="propertyValue">
        /// object representing the value we want to check for
        /// </param>
        private bool InheritedPropertyValueFound(TextPointer position, DependencyProperty property, object propertyValue)
        {
            AssertCondition(propertyValue != null, "propertyValue is null!");
            return propertyValue.Equals(position.Parent.GetValue(property));
        }

        /// <summary>
        /// Takes a range that is a subset of the text box and returns a Xaml string representing the text box Xaml if the given range
        /// were selected and demarcated with [ and ] symbols
        /// </summary>
        /// <param name="range">
        /// TextRange to be marked off
        /// </param>
        private string XamlWithRangeMarkers(TextRange range)
        {
            TextRange fullRange = new TextRange(_textbox.Document.ContentStart, _textbox.Document.ContentEnd);
            string fullRangeXaml;

            AssertCondition(range.Start.CompareTo(fullRange.Start) >= 0 && range.Start.CompareTo(fullRange.End) <= 0, "Subset of textbox expected");
            AssertCondition(range.End.CompareTo(fullRange.Start) >= 0 && range.End.CompareTo(fullRange.End) <= 0, "Subset of textbox expected");

            if (range.IsEmpty)
            {
                AssertCondition(range.Start.LogicalDirection == LogicalDirection.Backward, "Backward direction for range start expected");
                AssertCondition((object)range.Start == (object)range.End, "Empty range expected");

                TextPointer start = range.Start.GetPositionAtOffset(0, LogicalDirection.Forward);
                start.InsertTextInRun("[]");

                fullRangeXaml = DrtEditing.GetTextRangeXml(fullRange);

                new TextRange(range.Start, start).Text = "";
            }
            else
            {
                AssertCondition(range.Start.LogicalDirection == LogicalDirection.Backward, "Backward direction for range start expected");
                AssertCondition(range.End.LogicalDirection == LogicalDirection.Forward, "Forward direction for range end expected");

                TextPointer start = range.Start.GetPositionAtOffset(0, LogicalDirection.Forward);
                start.InsertTextInRun("[");

                TextPointer end = range.End.GetPositionAtOffset(0, LogicalDirection.Backward);
                end.InsertTextInRun("]");

                fullRangeXaml = DrtEditing.GetTextRangeXml(fullRange);

                new TextRange(range.Start, start).Text = "";
                new TextRange(end, range.End).Text = "";
            }

            return fullRangeXaml;
        }

        /// <summary>
        /// Checks that the Xaml value of a given text range contains the specified Xaml string. First it converts the
        /// xaml string into a complete Xaml range by calling the DrtEditing.GetClipboardXaml() function, then compares it with the
        /// Xml value of the range and asserts for equality
        /// </summary>
        /// <param name="range">
        /// TextRange whose Xaml we want to check
        /// </param>
        /// <param name="xaml">
        /// String representing Xaml expression to be checked for
        /// </param>
        private void CheckRangeXaml(TextRange range, string xaml)
        {
            DrtEditing.AssertEqualXml(XamlWithRangeMarkers(range), DrtEditing.GetClipboardXaml(xaml), "Range is in unexpected position");
        }

        /// <summary>
        /// Creates a table.
        /// </summary>
        /// <param name="insertionPosition">
        /// Position where table must be inserted.
        /// </param>
        /// <param name="rowCount">
        /// Number of rows generated in a table
        /// </param>
        /// <param name="columnCount">
        /// Number of columnns generated in each row
        /// </param>
        private Table CreateTable(int rowCount, int columnCount)
        {
            Table table = new Table();
            TableRowGroup body = new TableRowGroup();

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                TableRow row = new TableRow();

                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    TableCell cell = new TableCell(new Paragraph(new Run("c" + rowIndex + "." + columnIndex)));
                    cell.BorderThickness = new Thickness(1);
                    row.Cells.Add(cell);
                }
                body.Rows.Add(row);
            }
            table.RowGroups.Add(body);

            return table;
        }

        /// <summary>
        /// Verifies that the cursor/selection is at the point we expect. If not, throws exception
        /// </summary>
        /// <param name="point">
        /// Point of expected cursor position
        /// </param>
        private void VerifyCursorPosition(Point point)
        {
            // Obtain current position of cursor
            NativeMethods.POINT mousePoint = new NativeMethods.POINT(0, 0);
            UnsafeNativeMethods.GetCursorPos(mousePoint);
            Point cursorPoint = new Point(mousePoint.x, mousePoint.y);

            // Check that X and Y coordinates of cursor position are the same as the expected point
            AssertEqual(cursorPoint, point, "Cursor not at expected position");
        }

        private RichTextBox _textbox;
        private TextBlock _textBlock;
        private const int TS_E_NOLAYOUT = unchecked((int)0x80040206);
        private PropertyInfo _reflectionCaretElement_Debug_CaretElement;
        private PropertyInfo _reflectionCaretElement_Debug_RenderScope;
        private FieldInfo _reflectionCaretElement_Left;
        private FieldInfo _reflectionCaretElement_ShowCaret;
        private FieldInfo _reflectionCaretElement_BlinkAnimationClock;
        private FieldInfo _reflectionCaretElement_Top;
        private FieldInfo _reflectionCaretElement_SystemCaretWidth;
        private FieldInfo _reflectionCaretElement_InterimWidth;
        private FieldInfo _reflectionCaretElement_Height;
        private FieldInfo _reflectionCaretElement_IsBlinkEnabled;
        private FieldInfo _reflectionCaretElement_CaretBrush;
        private FieldInfo _reflectionCaretElement_AdornerLayer;
        private FieldInfo _reflectionCaretElement_Italic;
    }
}