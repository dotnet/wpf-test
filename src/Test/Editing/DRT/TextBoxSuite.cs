// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define TEST_SPELLER_API

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.ComponentModel; // Win32Exception
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Reflection;
using MS.Win32; // NativeMethods

namespace DRT
{
    internal class TextBoxSuite : DrtTestSuite
    {
        // Constructor.
        internal TextBoxSuite() : base("TextBox")
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
            _textbox = new TextBox();
            Canvas.SetLeft(_textbox, 5);
            Canvas.SetTop(_textbox, 5);
            _textbox.Width = 200;
            _textbox.Height = 200;
            
            // Set the font for consistency across themes
            _textbox.FontFamily = new FontFamily("Tahoma");
            _textbox.FontSize = 11.0;

            canvas.Children.Add(_textbox);

            return canvas;
        }

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {            
            DRT.Show(CreateTree());

            DRT.ShowRoot();
            
            System.Collections.Generic.List<DrtTest> tests = new System.Collections.Generic.List<DrtTest>();
            
            //Tests common to all framework versions
            tests.AddRange(                
                new DrtTest[]{
                    new DrtTest(ClickTextBox),
                    new DrtTest(TestAppendText),
                    new DrtTest(TestCharacterCasing),
                    new DrtTest(TestCharacterCasingLower),
                    new DrtTest(TestCharacterCasingUpper),
                    new DrtTest(TestCharacterCasingNormal),
                    new DrtTest(TestClear),
                    new DrtTest(TestIsReadOnly),
                    new DrtTest(TestIsReadOnlyVerifyTrue),
                    new DrtTest(TestIsReadOnlyVerifyFalse),
                    new DrtTest(TestMaxLengthSurrogate),
                    new DrtTest(TestMaxLengthHighSurrogate),
                    new DrtTest(TestMaxLengthLowSurrogate),
                    new DrtTest(TestMaxLength),
                    new DrtTest(TestMaxLengthLimited),
                    new DrtTest(TestMaxLengthUnlimited),
                    new DrtTest(TestProofingAssemblyNotLoaded),
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
                    new DrtTest(TestText),
                    new DrtTest(TestWrap),
                    new DrtTest(TestWrapVerifyTrue),
                    new DrtTest(TestWrapVerifyFalse),
                    new DrtTest(TestHitTest),
                    new DrtTest(PrepareTestLineAPI1),
                    new DrtTest(PrepareTestLineAPI2),
                    new DrtTest(TestLineAPI),
                    new DrtTest(TestScrollToLine1),
                    new DrtTest(TestScrollToLine2),
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
                    new DrtTest(TestProgrammaticUndo)
                }
            );
            
            //Speller not available in Arrowhead
            if(!DrtBase.IsClientSKUOnly)
            {
                tests.AddRange(
                    new DrtTest [] 
                    {
                        new DrtTest(TestSpellerAPIWarmup),
                        new DrtTest(TestSpellerAPI)
                    }
                );
            }
            
            // Return the lists of tests to run against the tree
            return tests.ToArray();
        }

        /// <summary>Clicks on the TextBox control.</summary>
        private void ClickTextBox()
        {
            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            const int x = 150;
            const int y = 150;
            DrtInput.ClickScreenPoint(x, y);
        }

        /// <summary>Tests the TextBox.AppendText method.</summary>
        private void TestAppendText()
        {
            _textbox.Text = "";
            CheckText("");

            _textbox.AppendText("some text");
            CheckText("some text");

            _textbox.AppendText("more");
            CheckText("some textmore");

            _textbox.AppendText(" whitespace ");
            CheckText("some textmore whitespace ");

            _textbox.AppendText("\nnew\nlines");
            CheckText("some textmore whitespace \nnew\nlines");
        }

        /// <summary>Tests the CharacterCasing property.</summary>
        /// <remarks>
        /// Note that the type/check sequence of the methods must
        /// be honored.
        /// </remarks>
        private void TestCharacterCasing()
        {
            _textbox.Text = "";
            _textbox.CharacterCasing = CharacterCasing.Lower;
            KeyboardType("ab +cd. 1+1");
        }

        private void TestCharacterCasingLower()
        {
            CheckText("ab cd. 1!");

            _textbox.Text = "";
            _textbox.CharacterCasing = CharacterCasing.Upper;
            KeyboardType("ab +cd. 1+1");
        }

        private void TestCharacterCasingUpper()
        {
            CheckText("AB CD. 1!");

            _textbox.Text = "";
            _textbox.CharacterCasing = CharacterCasing.Normal;
            KeyboardType("ab +cd. 1+1");
        }

        private void TestCharacterCasingNormal()
        {
            CheckText("ab Cd. 1!");
            _textbox.Text = "";
            KeyboardType("+text and numbers 123+x45");
        }

        /// <summary>Tests the Clear method.</summary>
        private void TestClear()
        {
            _textbox.Text = "";
            CheckText("");

            _textbox.Clear();
            CheckText("");

            _textbox.Clear();
            CheckText("");

            _textbox.Text = "text";
            CheckText("text");

            _textbox.Select(0, 2);
            _textbox.Clear();
            CheckText("");
            AssertEqual(_textbox.SelectionStart, 0,
                "Selection start is zero");
            AssertEqual(_textbox.SelectionLength, 0,
                "Selection length is zero");
        }

        /// <summary>Tests the TextBox.IsReadOnly property.</summary>
        /// <remarks>
        /// Note that input emulation uses characters that can be mapped
        /// through any keyboard layout in this case.
        /// </remarks>
        private void TestIsReadOnly()
        {
            _textbox.Clear();
            _textbox.IsReadOnly = true;
            KeyboardType("  ");
        }

        private void TestIsReadOnlyVerifyTrue()
        {
            CheckText("");

            _textbox.IsReadOnly = false;
            KeyboardType("...");
        }

        private void TestIsReadOnlyVerifyFalse()
        {
            CheckText("...");
        }

        /// <summary>
        /// Tests inserting surrogate char with the TextBox.MaxLength property.
        /// Regression_Bug268-WPF textbox exception when input emotion icon if "MaxLength" is set
        /// Regression_Bug269-Incorrect truncated text when input string contains emotion icon and exceeds the "MaxLength" of WPF textbox
        /// Regression_Bug270-A rectangle is shown when input emotion icon to a WPF textbox with 1 remainig space to reach its MaxLength
        /// </summary>
        private void TestMaxLengthSurrogate()
        {
            _textbox.Clear();
            _textbox.MaxLength = 4;
            Clipboard.SetData(DataFormats.UnicodeText, "\uD83D\uDE03abc");
            _textbox.Paste();
        }

        private void TestMaxLengthHighSurrogate()
        {
            CheckText("\ud83d\ude03ab");

            _textbox.Text = "abc";

            // high surrogate char
            KeyboardType("\ud83d");
        }

        private void TestMaxLengthLowSurrogate()
        {
            CheckText("abc");

            // low surrogate char
            KeyboardType("\ude03");
        }

        /// <summary>Tests the TextBox.MaxLength property.</summary>
        private void TestMaxLength()
        {
            CheckText("abc");

            _textbox.Clear();
            _textbox.MaxLength = 2;
            KeyboardType("abc");
        }

        private void TestMaxLengthLimited()
        {
            CheckText("ab");

            _textbox.MaxLength = 0;
            KeyboardType("abc");
        }

        private void TestMaxLengthUnlimited()
        {
            CheckText("ababc");
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
                string name = assemblies[i].GetName().Name
                    .ToLower(System.Globalization.CultureInfo.InvariantCulture);
                if (name.IndexOf(match) != -1)
                {
                    throw new Exception(
                        "Assembly [" + assemblies[i].FullName + "] loaded, " +
                        "possibly the proofing services assembly. This " +
                        "assembly should have been delayed.");
                }
            }
        }

        /// <summary>Tests the TextBox.Select method.</summary>
        private void TestSelect()
        {
            _textbox.Clear();
            CheckText("");

            _textbox.Select(0, 0);
            CheckSelectedText("");
            CheckText("");

            _textbox.Text = "text";
            CheckSelectedText("");

            _textbox.Select(0, 1);
            CheckSelectedText("t");

            _textbox.Select(1, 1);
            CheckSelectedText("e");

            _textbox.Select(0, 4);
            CheckSelectedText("text");
        }

        /// <summary>Tests the TextBox.SelectedText property.</summary>
        private void TestSelectedText()
        {
            _textbox.Clear();
            CheckText("");
            CheckSelectedText("");

            _textbox.Text = "content";
            CheckText("content");
            CheckSelectedText("");

            _textbox.Select(0, 1);
            CheckSelectedText("c");

            _textbox.SelectedText = "more c";
            CheckSelectedText("more c");
            CheckText("more content");
        }

        /// <summary>Tests the TextBox.SelectionLength property.</summary>
        private void TestSelectionLength()
        {
            _textbox.Clear();
            CheckText("");
            AssertEqual(_textbox.SelectionLength, 0, "SelectionLength");

            _textbox.Text = "some content";
            _textbox.Select(0, 4);
            AssertEqual(_textbox.SelectionLength, 4, "SelectionLength");

            _textbox.Select(2, 4);
            AssertEqual(_textbox.SelectionLength, 4, "SelectionLength");

            _textbox.Clear();
            AssertEqual(_textbox.SelectionLength, 0, "SelectionLength");
        }

        /// <summary>Tests the TextBox.SelectionStart property.</summary>
        private void TestSelectionStart()
        {
            _textbox.Clear();
            CheckText("");
            AssertEqual(_textbox.SelectionStart, 0, "SelectionStart");

            _textbox.Text = "some content";
            _textbox.Select(0, 4);
            AssertEqual(_textbox.SelectionStart, 0, "SelectionStart");

            _textbox.Select(2, 4);
            AssertEqual(_textbox.SelectionStart, 2, "SelectionStart");

            _textbox.Select(_textbox.Text.Length - 1, 0);
            AssertEqual(_textbox.SelectionStart, _textbox.Text.Length - 1, "SelectionStart");
            _textbox.Clear();
            AssertEqual(_textbox.SelectionStart, 0, "SelectionStart");
        }

        private void TestUndo1()
        {
            _textbox.Text = "first text";
        }

        private void TestUndo2()
        {
            _textbox.LockCurrentUndoUnit();
            _textbox.SelectAll();
            _textbox.Text = "second text";
        }

        private void TestUndo3()
        {
            _textbox.Undo();
            CheckText("first text");
            _textbox.Redo();
            CheckText("second text");
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
            _textbox.Clear();
            CheckText("");
            _textbox.Text = MultiPageText;
            _textbox.TextWrapping = TextWrapping.NoWrap;
            _textbox.AcceptsReturn = true;
        }

        /// <summary>
        /// Test scrolling after giving any objects to updated their layout.
        /// </summary>
        private void TestScrollViewerMembersLaidOut()
        {
            double extX = _textbox.ExtentWidth; // horizontal extent
            double extY = _textbox.ExtentHeight; // vertical extent
            double sizeX = _textbox.ViewportWidth; // horizontal size
            double sizeY = _textbox.ViewportHeight; // vertical size

            AssertSecondGreater(sizeX, extX,
                "Horizontal content extent is greater than layout size");
            AssertSecondGreater(sizeY, extY,
                "Vertical content extent is greater than layout size");

            double posX = _textbox.HorizontalOffset; // horizontal position
            double posY = _textbox.VerticalOffset; // vertical position

            _textbox.LineRight();
            _textbox.UpdateLayout();
            AssertSecondGreater(posX, _textbox.HorizontalOffset,
                "TextBox.LineRight increases horizontal position.");

            _textbox.LineLeft();
            _textbox.UpdateLayout();
            AssertEqual(posX, _textbox.HorizontalOffset,
                "TextBox.LineLeft restores horizontal position.");

            _textbox.LineDown();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset,
                "TextBox.LineDown increases vertical position.");

            _textbox.LineUp();
            _textbox.UpdateLayout();
            AssertEqual(posY, _textbox.VerticalOffset,
                "TextBox.LineUp restores vertical position.");

            _textbox.PageRight();
            _textbox.UpdateLayout();
            AssertSecondGreater(posX, _textbox.HorizontalOffset,
                "TextBox.PageRight increases horizontal position.");

            _textbox.PageLeft();
            _textbox.UpdateLayout();
            AssertEqual(posX, _textbox.HorizontalOffset,
                "TextBox.PageLeft restores horizontal position.");

            _textbox.PageDown();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset,
                "TextBox.PageDown increases vertical position.");

            _textbox.PageUp();
            _textbox.UpdateLayout();
            AssertEqual(posY, _textbox.VerticalOffset,
                "TextBox.PageUp restores vertical position.");

            _textbox.ScrollToEnd();
            _textbox.UpdateLayout();
            AssertSecondGreater(posY, _textbox.VerticalOffset,
                "TextBox.ScrollToEnd increases vertical position.");

            _textbox.ScrollToHome();
            _textbox.UpdateLayout();
            AssertEqual(posY, _textbox.VerticalOffset,
                "TextBox.ScrollToHome restores vertical position.");

            // Clear for next stage.
            _textbox.ScrollToHome();  // TextFlow no longer autosizes to text, so its size will
                                      // not change when text is removed, so horizontal scrollbar
                                      // must be explicitly returned to 0 as needed by next test.
           _textbox.Clear();
            CheckText("");
        }

        /// <summary>
        /// Test scrolling while typing.
        /// </summary>
        private void TestScrollViewerTyping()
        {
            AssertEqual((double)0, _textbox.HorizontalOffset,
                "TextBox.HorizontalOffset is reset to 0 horizontal position.");

            KeyboardType(LongText);
        }

        private void TestScrollViewerTypingCheckHorizontal()
        {
            AssertSecondGreater((double)0, _textbox.HorizontalOffset,
                "TextBox.HorizontalOffset moves to follow caret.");
            KeyboardType(
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}" +
                "{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}{ENTER}");
        }

        private void TestScrollViewerTypingCheckVertical()
        {
            AssertSecondGreater(0, _textbox.VerticalOffset,
                "TextBox.VerticalOffset moves to follow caret.");
        }

        /// <summary>Tests the TextBox.Copy property.</summary>
        private void TestCopyCommand()
        {
            _textbox.Clear();
            _textbox.Text = "bigdeal this is a test";
            _textbox.Select(0, 7);

            _textbox.SelectedText = "bigdeal";
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
            _textbox.Clear();
            _textbox.Text = "bigdeal this is a test";
            _textbox.Select(0, 7);

            _textbox.SelectedText = "bigdeal";
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
            _textbox.Clear();
            _textbox.Text = "what a good deal this is a test";
            _textbox.Select(0, 4);

            _textbox.SelectedText = "what";
            CheckSelectedText("what");
            _textbox.Paste();
        }

        /// <summary> Test the TextBox Commands result </summary>
        private void TestPasteCommandCheck()
        {
            CheckText("bigdeal a good deal this is a test");
        }

        /// <summary>Tests the TextBox.Text property.</summary>
        private void TestText()
        {
            _textbox.Clear();
            CheckText("");
            _textbox.Text = "sample text";
            CheckText("sample text");

            _textbox.Select(0, 3);
            CheckText("sample text");

            _textbox.TextWrapping = TextWrapping.Wrap;
            CheckText("sample text");
        }

        /// <summary>Tests the TextBox.</summary>
        private void TestWrap()
        {
            _textbox.Clear();
            CheckText("");
            _textbox.TextWrapping = TextWrapping.Wrap;
            _textbox.Text = LongText;
        }

        private void TestWrapVerifyTrue()
        {
            AssertEqual(_textbox.TextWrapping, TextWrapping.Wrap, "Wrapping is Wrap");

            _textbox.TextWrapping = TextWrapping.NoWrap;
        }

        private void TestWrapVerifyFalse()
        {
            AssertEqual(_textbox.TextWrapping, TextWrapping.NoWrap, "Wrapping is NoWrap");
        }

        // Tests the GetRectFromTextPosition and GetTextPositionFromPoint APIs.
        private void TestHitTest()
        {
            // TextBox.GetRectFromTextPosition is interal...need to restore
            // this test when a cp based hittest method is ready.
            #if DISABLED_BY_TOM_BREAKING_CHANGE

            Rect rect;
            OrientedTextPosition position;

            _textbox.Text = LongText;

            // Round trip the first char.

            _textbox.UpdateLayout();

            rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.StartPosition, LogicalDirection.Forward));
            position = _textbox.GetTextPositionFromPoint(new Point(rect.X + rect.Width / 4, rect.Y + rect.Height / 2), false);
            AssertEqual(position.TextPosition, _textbox.StartPosition, "Bad roundtrip from TestHitTest (1)");

            // Again, but this time snap from the left margin.

            rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.StartPosition, LogicalDirection.Forward));
            position = _textbox.GetTextPositionFromPoint(new Point(rect.X - rect.Width / 4, rect.Y + rect.Height / 2), true);
            // The assert below could fail if anyone shrinks the current margins on our TextBox...
            AssertEqual(position.TextPosition, _textbox.StartPosition, "Bad roundtrip from TestHitTest (2)");
            #endif // DISABLED_BY_TOM_BREAKING_CHANGE
        }

        private void PrepareTestLineAPI1()
        {
            _textbox.TextWrapping = TextWrapping.NoWrap;
            _textbox.Text = "Line 0\nLine 1\nLine 2\nThis line has 28 characters\nLine 4\nLine 5\nLine 6\nLine 7\nLine 8\nLine 9\nLine 10\nLine 11\nLine 12\nLine 13\nLine 14\nLine 15\nLine 16\nLine 17";
        }

        private void PrepareTestLineAPI2()
        {
            // Make sure that content scrolled so that the very first line is visible
            _textbox.SelectionStart = 0;
            _textbox.SelectionLength = 0;
            _textbox.ScrollToLine(0);
        }

        private void TestLineAPI()
        {
            int charIndex = _textbox.GetCharacterIndexFromLineIndex(0);
            AssertEqual(charIndex, 0, "GetCharacterIndexFromLineIndex(0)");

            charIndex = _textbox.GetCharacterIndexFromLineIndex(2);
            AssertEqual(charIndex, 14, "GetCharacterIndexFromLineIndex(2)");

            int lineIndex = _textbox.GetLineIndexFromCharacterIndex(charIndex);
            AssertEqual(lineIndex, 2, "GetLineIndexFromCharacterIndex()");

            // Check absolute line indexes
            AssertEqual(_textbox.GetLineLength(3), 28, "GetLineLength(3)");
            AssertEqual(_textbox.GetLineText(2), "Line 2\n", "GetLineText(2)");
            AssertEqual(_textbox.LineCount, 18, "LineCount");

            // Check initial visible lines. We assume that caret is at the beginning of a text
            AssertEqual(_textbox.GetFirstVisibleLineIndex(), 0, "First visible line must be 0-th");
            AssertEqual(_textbox.GetLastVisibleLineIndex(), 14, "Last visible line must be 14-th");

            // Now make visible two more lines from the bottom
            _textbox.ScrollToLine(16);
        }

        private void TestScrollToLine1()
        {
            // After scrolling to line 16 we must see lines 3-17
            AssertEqual(_textbox.GetFirstVisibleLineIndex(), 3, "GetFirstVisibleLineIndex()");
            AssertEqual(_textbox.GetLastVisibleLineIndex(), 17, "GetLastVisibleLineIndex()");

            _textbox.ScrollToLine(1);
        }

        private void TestScrollToLine2()
        {
            AssertEqual(_textbox.GetFirstVisibleLineIndex(), 1, "GetFirstVisibleLineIndex()");
            AssertEqual(_textbox.GetLastVisibleLineIndex(), 15, "GetLastVisibleLineIndex()");
        }

        /// <summary>
        /// Loads text and clicks mouse to position cursor
        /// </summary>
        private void SetSelectionPositionForVerticalCaretMovement()
        {
            // Load the text for the test
            _textbox.Clear();
            _textbox.Text = "Text\r\n\r\nText";
            _textbox.UpdateLayout();

            // Find coordinates of empty paragraph
            Rect rect = GetScreenRectFromCharacterOffset(6, LogicalDirection.Forward);
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
            // Verify that selection starts 6 places away from _textbox Start, which is where we clicked the mouse
            AssertEqual(_textbox.SelectionStart, 6, "Selection start expected value is 6");

            // Press the home key
            KeyboardType("{HOME}");
        }

        /// <summary>
        /// Checks the caret position after the HOME key is pressed. On an empty paragraph, caret position
        /// should be unchanged. Also presses the END key for the following test
        /// </summary>
        private void TestCaretMovementOnHome()
        {
            // Verify that selection start is unchanged
            AssertEqual(_textbox.SelectionStart, 6, "Selection start expected value is 6");

            // Press the end key
            KeyboardType("{END}");
        }

        /// <summary>
        /// Checks the caret position after END is pressed in previous test. On empty paragraph caret position should
        /// be unchanged. Also presses UP key for next test.
        /// </summary>
        private void TestCaretMovementOnEnd()
        {
            // Verify that selection start is unchanged
            AssertEqual(_textbox.SelectionStart, 6, "Selection start expected value is 6");

            // Press the up key
            KeyboardType("{UP}");
        }

        /// <summary>
        /// Checks that selection start moves to the previous line when UP key is pressed in the previous test. Also
        /// presses the DOWN key twice to navigate to last line for next test
        /// </summary>
        private void TestCaretMovementOnKeyUp()
        {
            // Verify that selection start moves to beginning of top line
            AssertEqual(_textbox.SelectionStart, 0, "Selection start expected value is 0");

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
            // Verify that selection start moves to beginning of last line, after the \r\n sequences
            AssertEqual(_textbox.SelectionStart, 8, "Selection start expected value is 8");

            // Clear _textbox for next set of tests
            _textbox.Clear();
        }

        /// <summary>
        /// Loads four-line text for testing selection from the jeyboard using SHIFT+UP and SHIFT+DOWN. Also
        /// positions the caret on the first line by clicking the mouse
        /// </summary>
        private void LoadMultiLineTextForVerticalCaretSelection()
        {
            // Load the text for the test
            _textbox.Text = "Long long long line\r\nLong long line\r\nLong line\r\nLong";
            _textbox.UpdateLayout();

            // Get coordinates of a point on the first line and click mouse to start selection
            Rect rect = GetScreenRectFromCharacterOffset(2, LogicalDirection.Forward);
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
            // Verify that cursor is at mouse click point
            AssertEqual(_textbox.SelectionStart, 2, "Selection start expected value is 2");

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
            // Check that seleciton start index is not changed from its position before keyboard navigation
            AssertEqual(_textbox.SelectionStart, 2, "Selection start expected value is 2");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after one line is selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownOneLine()
        {
            // Check that seleciton ends exactly one line below its start - it starts and index 2 and has length 21
            // for this text, both Text[3] and Text[24] are "g"
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 23, "Selection end expected value is 23");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after two lines are selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownTwoLines()
        {
            // Check that seleciton ends exactly two lines below its start 
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 39, "Selection end expected value is 39");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Tests selection end position after three lines are selected in previous test. Presses SHIFT+DOWN to select
        /// another line
        /// </summary>
        private void TestKeyboardSelectionDownThreeLines()
        {
            // Check that seleciton ends exactly two lines below its start 
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 50, "Selection end expected value is 50");

            // Press SHIFT + DOWN keys to alter selection
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// Checks that selecting a takes the selection end to the end of the _textbox. Also presses SHIFT+UP
        /// to start reversing selection
        /// </summary>
        private void TestKeyboardSelectionDownTextboxEnd()
        {
            // Verify that selection ends at textbox end
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, _textbox.Text.Length, "Selection expected to end at textbox end");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after first SHIFT+UP, selection reverts to its earlier position when 3 lines were selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpOneLine()
        {
            // Verify that selection ends at its previous value before the last SHIFT+DOWN
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 50, "Selection end expected value is 50");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after second SHIFT+UP, selection reverts to its earlier position when 2 lines were selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpTwoLines()
        {
            // Verify that selection end goes up one line
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 39, "Selection end expected value is 39");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after third SHIFT+UP, selection reverts to its earlier position when one line was selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpThreeLines()
        {
            // Verify that selection end goes up one line
            AssertEqual(_textbox.SelectionStart + _textbox.SelectionLength, 23, "Selection end expected value is 23");

            // Press SHIFT + UP keys to reverse movement
            KeyboardType("+{UP}");
        }

        /// <summary>
        /// Checks that after third SHIFT+UP, selection reverts to its earlier position when one line was selected.
        /// Presses SHIFT+UP to go up one more line
        /// </summary>
        private void TestKeyboardSelectionUpFourLines()
        {
            // Verify that selection start returns to its original position where mouse was first clicked and
            // that length is 0
            AssertEqual(_textbox.SelectionStart, 2, "Selection start expected value is 2");
            AssertEqual(_textbox.SelectionLength, 0, "Selection length expected value is 0");

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
            // Verify that selection starts on text box start
            AssertEqual(_textbox.SelectionStart, 0, "Selection start expected value is 0");

            // Press SHIFT + DOWN keys to see if selection is remembered
            KeyboardType("+{DOWN}");
        }

        /// <summary>
        /// After SHIFT+DOWN is pressed in previous test, selection should return to its original position.
        /// </summary>
        private void TestKeyboardSelectionDownToFirstPosition()
        {
            // Verify that selection start is remembered, i.e. it now returns to its former position before the
            // last SHIFT+UP
            // Verify that selection starts on text box start
            AssertEqual(_textbox.SelectionStart, 2, "Selection start expected value is 0");
            AssertEqual(_textbox.SelectionLength, 0, "Selection length expected value is 0");

            // Clear the textbox and release the keyboard
            KeyboardType("+");
            KeyboardType("{UP}");
            _textbox.Clear();
        }

        /// <summary>
        /// Loads and formats wrapping text to test overlap in _textbox. Positions the caret on the last line of
        /// wrapping text
        /// </summary>
        private void LoadWrappingText()
        {
            _textbox.Text = "Test case to create different overlapping styles. Thistexthasnospacesandisveryverylong";
            _textbox.TextWrapping = TextWrapping.Wrap;
            _textbox.UpdateLayout();

            // Obtain coordinates of a point that will be somewhere close to the end of the third line
            // It must be close to the end because there should not be any text above it on the second line
            Rect rect = GetScreenRectFromCharacterOffset(_textbox.Text.Length - 4, LogicalDirection.Forward);
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
            // Check that selection is where we expect
            AssertEqual(_textbox.SelectionStart, _textbox.Text.Length - 4, "Selection does not start at expected position");

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
            Rect approximateCaretPositionRect = GetScreenRectFromCharacterOffset(49, LogicalDirection.Forward);

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

            // Get coordinates of caret position in RenderScope and transform to screen
            Point caretPositionInRenderScope = new Point((double)_reflectionCaretElement_Left.GetValue(caretElement), (double)_reflectionCaretElement_Top.GetValue(caretElement));
            Point caretPosition = GetScreenPointFromRenderScopePoint(caretPositionInRenderScope);

            // Cast to int 
            caretPosition.X = (int)caretPosition.X;
            caretPosition.Y = (int)caretPosition.Y;

            // Obtain coordinates where we expect caret to be, at the start of the third line
            Rect expectedCaretPositionRect = GetScreenRectFromCharacterOffset(0, LogicalDirection.Forward);
            Point expectedCaretPosition = new Point(expectedCaretPositionRect.X, expectedCaretPositionRect.Y + 2 * ((int)caretHeight));

            AssertEqual(caretPosition, expectedCaretPosition, "Caret positoin not where expected");
            _textbox.TextWrapping = TextWrapping.NoWrap;
        }


        // Tests that the TextBox's content is preserved if the style changes
        private void TestStyleChange()
        {
            String s = "Testing style change";
            _textbox.Text = s;

            // Update the current text since the extra space is added by adding Button embedded object.
            s = _textbox.Text;

            // Change the style.  Same as default TextBox style, but yellow background
            Style ds = new Style(typeof(TextBox));
            Brush b1 = Brushes.Yellow;
            b1.Freeze();
            Brush b2 = Brushes.Gray;
            b2.Freeze();
            ds.Setters.Add (new Setter(TextBox.BackgroundProperty, b1));
            ds.Setters.Add (new Setter(Border.BorderBrushProperty, b2));
            ds.Setters.Add (new Setter(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None));

            // default font (from system)
            ds.Setters.Add (new Setter(TextBox.ForegroundProperty, SystemColors.ControlTextBrush));
            ds.Setters.Add (new Setter(TextBox.FontFamilyProperty, SystemFonts.MessageFontFamily));
            ds.Setters.Add (new Setter(TextBox.FontSizeProperty, SystemFonts.MessageFontSize));
            ds.Setters.Add (new Setter(TextBox.FontStyleProperty, SystemFonts.MessageFontStyle));
            ds.Setters.Add (new Setter(TextBox.FontWeightProperty, SystemFonts.MessageFontWeight));
            ds.Setters.Add (new Setter(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Left));

            FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
            canvas.SetValue(Control.ForegroundProperty, new TemplateBindingExtension(TextBox.ForegroundProperty));
            canvas.SetValue(Control.FontSizeProperty, new TemplateBindingExtension(TextBox.FontSizeProperty));
            canvas.SetValue(Control.FontFamilyProperty, new TemplateBindingExtension(TextBox.FontFamilyProperty));
            canvas.SetValue(TextBox.WidthProperty, new TemplateBindingExtension(TextBox.WidthProperty));
            canvas.SetValue(TextBox.HeightProperty, new TemplateBindingExtension(TextBox.HeightProperty));
            canvas.SetValue(Canvas.StyleProperty, null);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Rectangle));
            border.SetValue(Shape.StrokeProperty, new TemplateBindingExtension(Border.BorderBrushProperty));
            border.SetValue(Shape.FillProperty, new TemplateBindingExtension(Border.BackgroundProperty));
            border.SetValue(Rectangle.RadiusXProperty, 4.0d);
            border.SetValue(Rectangle.RadiusYProperty, 4.0d);
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
            shadow.SetValue(Rectangle.RadiusXProperty, 4.0d);
            shadow.SetValue(Rectangle.RadiusYProperty, 4.0d);
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
            sv.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, new TemplateBindingExtension(ScrollViewer.HorizontalScrollBarVisibilityProperty));
            sv.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBindingExtension(ScrollViewer.VerticalScrollBarVisibilityProperty));

            canvas.AppendChild(border);
            canvas.AppendChild(shadow);
            canvas.AppendChild(shadowLeft);
            canvas.AppendChild(sv);
            ControlTemplate template = new ControlTemplate(typeof(TextBox));
            template.VisualTree = canvas;
            ds.Setters.Add(new Setter(Control.TemplateProperty, template));

            _textbox.Style = ds;
            _textbox.ApplyTemplate();

            // Verify that content is intact
            AssertEqual(_textbox.Text, s, "Plain text not intact after style change.");
        }

        // Tests undoing programmatic changes to a TextBox.
        private void TestProgrammaticUndo()
        {
            _textbox.Text = String.Empty;

            _textbox.IsUndoEnabled = false;
            DRT.Assert(!_textbox.CanUndo, "ClearUndo left non-empty undo stack!");

            _textbox.Text = "unrecorded content";
            _textbox.Text = String.Empty;
            DRT.Assert(!_textbox.CanUndo, "Programmatic change in undo stack with IsUndoEnabled == false!");

            _textbox.IsUndoEnabled = true;

            _textbox.Text = "programmatic content";
            DRT.Assert(_textbox.CanUndo, "Programmatic change not in undo stack!");

            _textbox.Undo();
            DRT.Assert(_textbox.Text == String.Empty, "Failed to undo programmatic content!");
        }

        // Tests the speller OM.
        private void TestSpellerAPIWarmup()
        {
#if TEST_SPELLER_API
            _textbox.Text = "This iz badz tezt.";
            _textbox.IsSpellCheckEnabled = true;

            // Wait for speller to run.
#endif
        }

        // Tests the speller OM.
        private void TestSpellerAPI()
        {
#if TEST_SPELLER_API
            int errorLength;
            int count = 0;

            for (int errorStart = _textbox.GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward);
                 errorStart != -1;
                 errorStart = _textbox.GetNextSpellingErrorCharacterIndex(errorStart + errorLength, LogicalDirection.Forward))
            {
                SpellingError spellingError = _textbox.GetSpellingError(errorStart);
                DRT.Assert(spellingError != null);
                errorLength = _textbox.GetSpellingErrorLength(errorStart);

                if (count == 0)
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "iz");
                }
                else if (count == 1)
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "badz");
                }
                else
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "tezt");
                }

                count++;
            }

            DRT.Assert(count == 3);

            count = 0;

            for (int errorStart = _textbox.GetNextSpellingErrorCharacterIndex(_textbox.Text.Length, LogicalDirection.Backward);
                 errorStart != -1;
                 errorStart = _textbox.GetNextSpellingErrorCharacterIndex(errorStart, LogicalDirection.Backward))
            {
                SpellingError spellingError = _textbox.GetSpellingError(errorStart);
                DRT.Assert(spellingError != null);
                errorLength = _textbox.GetSpellingErrorLength(errorStart);

                if (count == 0)
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "tezt");
                }
                else if (count == 1)
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "badz");
                }
                else
                {
                    DRT.Assert(_textbox.Text.Substring(errorStart, errorLength) == "iz");
                }

                count++;
            }

            DRT.Assert(count == 3);
            _textbox.IsSpellCheckEnabled = false;
#endif
        }


        /// <summary>
        /// Checks that the specified condition is true.
        /// </summary>
        /// <param name='value'>Condition value.</param>
        /// <param name='description'>Description of evaluated condition</param>
        private void AssertCondition(bool value, string description)
        {
            if (!value)
            {
                string s = String.Format(
                    "Expected condition [{0}] does not hold", description);
                throw new Exception(s);
            }
        }

        /// <summary>
        /// Verifies that a given object has the expected value.
        /// </summary>
        /// <param name='value'>Actual value.</param>
        /// <param name='expected'>Expected value.</param>
        /// <param name='message'>An optional descriptive message.</param>
        private void AssertEqual(object value, object expected,
            string message)
        {
            bool condition = value.Equals(expected);
            string description = String.Format(
                "Expected [{0}] got [{1}] {2}", expected, value, message);
            if (!condition)
            {
                throw new Exception(description);
            }
        }

        /// <summary>
        /// Checks that the second parameter passed is greater than the first.
        /// </summary>
        /// <param name='lesserValue'>Value expected to be smaller.</param>
        /// <param name='greaterValue'>Value expected to be larger.</param>
        /// <param name='message'>An descriptive message of the condition tested.</param>
        private void AssertSecondGreater(double lesserValue, double greaterValue,
            string message)
        {
            if (lesserValue >= greaterValue)
            {
                string s = String.Format(
                    "Expected condition [{0} < {1}] does not hold {2}",
                    lesserValue, greaterValue, message);
                throw new Exception(s);
            }
        }

        /// <summary>
        /// Verifies that the SelectedText property has the expected value.
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckSelectedText(string expectedText)
        {
            AssertEqual(_textbox.SelectedText, expectedText,
            "Verifying SelectedText");
        }

        /// <summary>
        /// Verifies that the previously SelectedText is same as clipbaord
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckClipboardText(string expectedText)
        {
            IDataObject dataObj = Clipboard.GetDataObject();
            if (dataObj != null)
            {
                object data = dataObj.GetData(DataFormats.Text);
                if (data != null && data is string)
                {
                    AssertEqual(data as string, expectedText, "Verifying ClipboardText");
                }
                else
                    AssertEqual(false, expectedText, "Verifying ClipboardText");
            }
        }

        /// <summary>
        /// Verifies that the Text property has the expected value.
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckText(string expectedText)
        {
            AssertEqual(_textbox.Text, expectedText, "Verifying Text");
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
            // System.Windows.Forms.SendKeys.SendWait(text);
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

            // Obtain starting TextPointer of RenderScope and move to the required offset
            TextPointer renderScopePosition;
            if (renderScope is TextBlock)
            {
                renderScopePosition = (renderScope as TextBlock).ContentStart;
            }
            else
            {
                renderScopePosition = DrtEditing.TextFlowContentStart(renderScope);
            }
            renderScopePosition = renderScopePosition.GetPositionAtOffset(offset);

            return renderScopePosition.GetCharacterRect(direction);
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

            object textContainer;
            PropertyInfo TextContainerInfo = typeof(TextBoxBase).GetProperty("TextContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextContainerInfo == null)
            {
                throw new Exception("TextBoxBase.TextContainer property is not accessible");
            }
            textContainer = TextContainerInfo.GetValue(_textbox, null);
            PropertyInfo property = textContainer.GetType().GetProperty("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new Exception("TextContainer.Start property is not accessible");
            }
            TextPointer position = (TextPointer)property.GetValue(textContainer, null);

            position = position.GetPositionAtOffset(offset);

            return position;
        }
        
        private TextBox _textbox;
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

    internal class TestElement : FrameworkElement
    {
        public TestElement()
        {
        }

        public void Add(TestElement e)
        {
            AddLogicalChild(e);
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                return null;
            }
        }
    }
}