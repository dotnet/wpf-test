// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the TextEditor class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Editing/TextEditorTests.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;    

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Threading;
    using Microsoft.Test.Input;

    #endregion Namespaces.

    /// <summary>
    /// Verifies a number of specific TextEditor fixes.
    /// </summary>
    /// <remarks>
    /// Regression_Bug59:
    /// Verifies that the TextEditor can handle mouse down events
    /// on an unpositioned selection.
    /// 
    /// Regression_Bug60:
    /// Verifies that the TextEditor will delete newlines with a single
    /// Delete keystroke.
    ///
    /// Regression_Bug61:
    /// Verifies that Bold, Italic and Underline commands toggle.
    ///
    /// Regression_Bug62:
    /// Verifies that hit-testing still works, even after having the
    /// TextEditor delete an element. This is done by splitting
    /// a Paragraph.
    ///
    /// Regression_Bug63:
    /// Verifies that multiple lines are rendered in a TextBlock control
    /// with CR\LF (tested with Regression_Bug60).
    ///
    /// Regression_Bug64:
    /// Verifies that embedded objects can be selected with a mouse click.
    ///
    /// Regression_Bug65:
    /// Verifies that inline formatting is not lost on empty paragraphs.
    ///
    /// Regression_Bug66:
    /// Verifies that the cursor is always an arrow when it's over
    /// selected text.
    ///
    /// Regression_Bug67:
    /// Verifies that multiple Enter keys can be pressed in succession.
    /// This is mostly a dupe of Regression_Bug240.
    ///
    /// Regression_Bug68:
    /// Verifies that CaretPosition after hitting HOME Key when we have a LineBreak Element
    /// </remarks>
    [Test(1, "Editor", "TextEditorRegressions", MethodParameters = "/TestCaseType=TextEditorRegressions", Timeout = 500)]
    [TestOwner("Microsoft"), TestTactics("299"), TestBugs("59,60,65,64,66,240,63,67,68,69,478")]
    public class TextEditorRegressions: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _testedTypeIndex = 0;
            QueueDelegate(DoIteration);
        }

        #region Regression_Bug59.

        private void DoIteration()
        {
            _editableType = TextEditableType.Values[_testedTypeIndex];
            Log("\r\n\r\nTesting type " + _testedTypeIndex + ": " +
                _editableType.Type + "\r\n");

            Log("Creating and laying out an element of type: " + _editableType.Type);
            _testedElement = _editableType.CreateInstance();
            _testedWrapper = new UIElementWrapper(_testedElement);

            Log("Verifying that Regression_Bug59 does not repro...");
            MainWindow.Content = _testedElement;
            MainWindow.ApplyTemplate();
            _testedElement.UpdateLayout();
            _testedElement.ApplyTemplate();

            Log("Setting focus to element to position the selection...");
            _testedElement.Focus();

            Log("Clicking element...");
            MouseInput.MouseClick(_testedElement);

            Log("Queuing successful test case for after click handling...");
            QueueDelegate(AfterInitialClick);
        }

        private void AfterInitialClick()
        {
            Log("Regression_Bug59 verified.");
            if (_editableType.IsPassword)
            {
                _testedTypeIndex++;
                QueueDelegate(DoIteration);
            }
            else
            {
                QueueDelegate(TestRegression_Bug60);
            }
        }

        #endregion Regression_Bug59.

        #region Regression_Bug60.

        private void TestRegression_Bug60()
        {
            Log("Verifying Regression_Bug60.");
            _testedElement.SetValue(KeyboardNavigation.AcceptsReturnProperty, true);

            Log("Typing string with Enter...");
            MouseInput.MouseClick(_testedElement);
            KeyboardInput.TypeString("^a{DEL}abc{ENTER}d{UP}{END}");

            QueueDelegate(TestRegression_Bug60CheckBreak);
        }

        private void TestRegression_Bug60CheckBreak()
        {
            string text;
            int paragraphIndex;

            if (_editableType.SupportsParagraphs)
            {
                text = _testedWrapper.XamlText;
                Log("XAML Text: [" + text + "]");

                paragraphIndex = text.IndexOf("<Paragraph");
                Verifier.Verify(paragraphIndex >= 0, "First paragraph found in XAML.", true);
                paragraphIndex = text.IndexOf("<Paragraph", paragraphIndex + 1);
                Verifier.Verify(paragraphIndex >= 0, "Second paragraph found in XAML.", true);
            }
            else
            {
                text = _testedWrapper.Text;
                Log("Text: [" + text + "]");
                Verifier.Verify(text.IndexOf('\r') >= 0, "CR found in text.", true);
                Verifier.Verify(text.IndexOf('\n') >= 0, "NL found in text.", true);
            }

            Log("Pressing Delete character to remove newline...");
            KeyboardInput.TypeString("{DELETE}");

            QueueDelegate(TestRegression_Bug60CheckBreakDeleted);
        }

        private void TestRegression_Bug60CheckBreakDeleted()
        {
            string text;
            int paragraphIndex;

            if (_editableType.SupportsParagraphs)
            {
                text = _testedWrapper.XamlText;
                Log("XAML Text: [" + text + "]");

                paragraphIndex = text.IndexOf("<Paragraph");
                Verifier.Verify(paragraphIndex >= 0, "First paragraph found in XAML.", true);
                paragraphIndex = text.IndexOf("<Paragraph", paragraphIndex + 1);
                Verifier.Verify(paragraphIndex < 0, "Second paragraph not found in XAML.", true);
            }
            else
            {
                text = _testedWrapper.Text;
                Log("Text: [" + text + "]");
            }

            Log("Regression_Bug60 verified.");
            QueueDelegate(TestRegression_Bug240);
        }

        #endregion Regression_Bug60.

        #region Regression_Bug62.

        private void TestRegression_Bug240()
        {
            Log("Verifying that Regression_Bug240 does not repro...");
            _testedWrapper.Text = "";
            if (_editableType.SupportsParagraphs)
            {
                _testedWrapper.XamlText = "<Paragraph>Welcome to text editing.</Paragraph>";
            }
            else
            {
                _testedWrapper.Text = "Welcome to text editing.";
            }
            ActionItemWrapper.MouseElementRelative(_testedElement, "click left 20 8");
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(TestRegression_Bug240SecondSplit);
        }

        private void TestRegression_Bug240SecondSplit()
        {
            ActionItemWrapper.MouseElementRelative(_testedElement, "click left 30 8");
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(TestRegression_Bug61Format);
        }

        #endregion Regression_Bug62.

        #region Regression_Bug61.

        private void TestRegression_Bug61Format()
        {
            Log("Verifying that Regression_Bug61 does not repro...");
            if (!_editableType.SupportsParagraphs)
            {
                Log("Element does not support rich text. Skipping to next test.");
                QueueDelegate(TestRegression_Bug64SetupContent);
                return;
            }

            _toggleFormatIndex = 0;
            QueueDelegate(TestRegression_Bug61IterateFormat);
        }

        private void TestRegression_Bug61IterateFormat()
        {
            KeyboardInput.TypeString("^a{DEL}some toggled text{LEFT 5}+{LEFT 7}");
            QueueDelegate(TestRegression_Bug61GetInitialValue);
        }

        private void TestRegression_Bug61GetInitialValue()
        {
            DependencyProperty property;
            ToggleFormat format;

            format = ToggleFormat.Formats[_toggleFormatIndex];
            property = format.ChangedProperty;
            TextPointer tempPointer = _testedWrapper.SelectionInstance.Start;
            tempPointer = tempPointer.GetPositionAtOffset(1);
            _initialToggleFormat = tempPointer.Parent.GetValue(property);
            Log("Initial formatting property: " + _initialToggleFormat);

            KeyboardInput.TypeString(format.KeyboardInput);

            QueueDelegate(TestRegression_Bug61CheckChangedValue);
        }

        private void TestRegression_Bug61CheckChangedValue()
        {
            DependencyProperty property;
            ToggleFormat format;
            object changedValue;

            format = ToggleFormat.Formats[_toggleFormatIndex];
            property = format.ChangedProperty;
            TextPointer tempPointer = _testedWrapper.SelectionInstance.Start;
            tempPointer = tempPointer.GetPositionAtOffset(1);
            changedValue = tempPointer.Parent.GetValue(property);
            Log("Changed formatting property value: " + changedValue);
            Verifier.VerifyValueDifferent("changed value", _initialToggleFormat, changedValue);

            KeyboardInput.TypeString(format.KeyboardInput);

            QueueDelegate(TestRegression_Bug61CheckRestoredValue);
        }

        private void TestRegression_Bug61CheckRestoredValue()
        {
            DependencyProperty property;
            ToggleFormat format;
            object restoredValue;

            format = ToggleFormat.Formats[_toggleFormatIndex];
            property = format.ChangedProperty;
            TextPointer tempPointer = _testedWrapper.SelectionInstance.Start;
            tempPointer = tempPointer.GetPositionAtOffset(1);
            restoredValue = tempPointer.Parent.GetValue(property);
            Log("Restored formatting property value: " + restoredValue);
            Verifier.VerifyValue("restored value", _initialToggleFormat, restoredValue);

            // Iterate tot he next format to try toggling.
            _toggleFormatIndex++;
            if (_toggleFormatIndex == ToggleFormat.Formats.Length)
            {
                QueueDelegate(TestRegression_Bug64SetupContent);
            }
            else
            {
                QueueDelegate(TestRegression_Bug61IterateFormat);
            }
        }

        #endregion Regression_Bug61.

        #region Regression_Bug64.

        private TextBlock _embeddedObject;

        private void TestRegression_Bug64SetupContent()
        {
            if (_testedWrapper.Element is TextBox)
            {
                //This bug repro is only for RichTextBox
                QueueDelegate(TestRegression_Bug65SetupBox);
                return;
            }

            RichTextBox control;
            Paragraph paragraph;

            Log("Verifying Regression_Bug64...");

            Log("Adding an embedded object to text content...");
            _embeddedObject = new System.Windows.Controls.TextBlock();
            _embeddedObject.FontWeight = FontWeights.Bold;
            _embeddedObject.Text = "hello, world";

            control = (RichTextBox)_testedWrapper.Element;
            control.Document.Blocks.Clear();

            paragraph = new Paragraph();
            control.Document.Blocks.Add(paragraph);
            paragraph.Inlines.Add("plain");
            paragraph.Inlines.Add(_embeddedObject);
            paragraph.Inlines.Add("text");

            QueueDelegate(TestRegression_Bug64ClickEmbeddedObject);
        }

        private void TestRegression_Bug64ClickEmbeddedObject()
        {
            Log("Clicking on embedded object...");
            MouseInput.MouseClick(_embeddedObject);

            QueueDelegate(TestRegression_Bug64CheckSelection);
        }

        private void TestRegression_Bug64CheckSelection()
        {
            object positionObject;  // Object retrieved from position.
            TextPointer position;  // Position to get object from.

            Log("Verifying that selection encompasses embedded object...");
            position = _testedWrapper.SelectionInstance.Start;
            //need to move past the end Run tag which is before IUIC
            position = position.GetPositionAtOffset(1);
            //need to move past the start IUIC tag which is before embeddedObject
            position = position.GetPositionAtOffset(1);
            positionObject = position.GetAdjacentElement(LogicalDirection.Forward);

            Log("Object obtained from start position is: " + positionObject);
            Verifier.Verify(positionObject == _embeddedObject,
                "Position object matches inserted object.", true);

            position = _testedWrapper.SelectionInstance.End;
            //need to move past the start Run tag which is after IUIC
            position = position.GetPositionAtOffset(-1);
            //need to move past the end IUIC tag which is before embeddedObject
            position = position.GetPositionAtOffset(-1);
            positionObject = position.GetAdjacentElement(LogicalDirection.Backward);

            Log("Object obtained from end position is: " + positionObject);
            Verifier.Verify(positionObject == _embeddedObject,
                "Position object matches inserted object.", true);

            QueueDelegate(TestRegression_Bug65SetupBox);
        }

        #endregion Regression_Bug64.

        #region Regression_Bug65.

        private void TestRegression_Bug65SetupBox()
        {
            Log("Verifying Regression_Bug65...");
            _testedWrapper.Text = "";
            if (!_editableType.SupportsParagraphs)
            {
                Log("Cannot verify for the current editable type - paragraph support required.");
                QueueDelegate(TestRegression_Bug66Setup);
                return;
            }
            ActionItemWrapper.MouseElementRelative(_testedElement, "click left 20 8");

            // The second line sets up a Bold element encompassing all of its
            // text. Then the content is deleted, and the caret moved elsewhere
            // to remove springload formatting. TextBlock is re-entered, and we expect
            // that this text be bold.
            KeyboardInput.TypeString("first line{ENTER}text+{LEFT 4}" + 
                EditingCommandData.ToggleBold.KeyboardShortcut + "{END}{BS 4}{UP}{DOWN}text");
            QueueDelegate(TestRegression_Bug65CheckFormatting);
        }

        private void TestRegression_Bug65CheckFormatting()
        {
            object boldValue;
            Log("Verifying that second line is still bold...");
            TextPointer tempPointer = _testedWrapper.End.GetPositionAtOffset(-3);
            boldValue = tempPointer.Parent.GetValue(System.Windows.Controls.TextBlock.FontWeightProperty);
            Log("Bold value on second line: " + boldValue);
            Verifier.Verify((FontWeight)boldValue == FontWeights.Bold,
                "Font weight is bold on second line, as expected.", true);

            QueueDelegate(TestRegression_Bug66Setup);
        }

        #endregion Regression_Bug65.

        #region Regression_Bug66.

        /// <summary>Cursor displayed when mouse is over button.</summary>
        private Win32.HCURSOR _buttonCursor;

        /// <summary>Cursor displayed when mouse is over selected text.</summary>
        private Win32.HCURSOR _selectedTextCursor;

        /// <summary>Cursor displayed when mouse is over unselected text.</summary>
        private Win32.HCURSOR _unselectedTextCursor;

        /// <summary>Button used to take focus away from TextBox.</summary>
        private Button _testRegression_Bug66Button;

        private void TestRegression_Bug66Setup()
        {
            //FlowPanel topPanel;
            //DockPanel topPanel; //Changed to not use FlowPanel - Work around for Regression_Bug69

            Log("Verifying that Regression_Bug66 does not repro...");

            Log("Adding a Button to the main window in a panel...");
            MainWindow.Content = null;
            _testRegression_Bug66Button = new Button();
            _topPanel = new StackPanel();
            //topPanel = new DockPanel(); //Changed to not use FlowPanel - Work around for Regression_Bug69
            _testRegression_Bug66Button.Content = "Click to change focus from TextBox";
            _topPanel.Children.Add(_testedElement);
            _topPanel.Children.Add(_testRegression_Bug66Button);
            MainWindow.Content = _topPanel;

            _testedWrapper.Text = "text";
            _testedElement.SetValue(
                System.Windows.Controls.TextBlock.FontSizeProperty, 32.0);

            QueueDelegate(TestRegression_Bug66MouseOverText);
        }

        private void TestRegression_Bug66MouseOverText()
        {
            Log("Placing mouse over selected text...");
            MouseInput.MouseClick(_testedElement);
            ActionItemWrapper.MouseElementRelative(_testedElement, "move 20 10");

            QueueDelegate(TestRegression_Bug66MouseOverSelectedText);
        }

        private void TestRegression_Bug66MouseOverSelectedText()
        {
            Log("Retrieving selected text cursor...");
            _unselectedTextCursor = Win32.SafeGetCursor();            

            KeyboardInput.TypeString("^a");

            QueueDelegate(TestRegression_Bug66MoveOverSelectedText);
        }

        private void TestRegression_Bug66MoveOverSelectedText()
        {
            Log("Placing mouse over selected text...");
            ActionItemWrapper.MouseElementRelative(_testedElement, "move 25 15");

            QueueDelegate(TestRegression_Bug66MouseOverButton);
        }

        private void TestRegression_Bug66MouseOverButton()
        {
            Log("Retrieving selected text cursor...");
            _selectedTextCursor = Win32.SafeGetCursor();

            Log("Clicking on button...");
            MouseInput.MouseClick(_testRegression_Bug66Button);

            QueueDelegate(TestRegression_Bug66MouseBackToSelection);
        }

        private void TestRegression_Bug66MouseBackToSelection()
        {
            Log("Retrieving button cursor...");
            _buttonCursor = Win32.SafeGetCursor();

            Log("Button cursor:        " + _buttonCursor);
            Log("Selected text cursor: " + _selectedTextCursor);
            Log("Regular text cursor:  " + _unselectedTextCursor);

            Verifier.Verify(_unselectedTextCursor != _buttonCursor,
                "Cursor changed for selection.", true);
            Verifier.Verify(_selectedTextCursor == _buttonCursor,
                "Selected text and button shared cursors.", true);

            ActionItemWrapper.MouseElementRelative(_testedElement, "move 20 10");

            QueueDelegate(TestRegression_Bug66MouseBackToSelectionCheck);
        }

        private void TestRegression_Bug66MouseBackToSelectionCheck()
        {
            Win32.HCURSOR selectionCursorNonFocused;

            Log("Retrieving selected text cursor...");
            selectionCursorNonFocused = Win32.SafeGetCursor();

            Verifier.Verify(_selectedTextCursor != selectionCursorNonFocused,
                "Cursor is not selectionCursor when not in focus", true);

            QueueDelegate(TestRegression_Bug67);
        }

        #endregion Regression_Bug66.

        #region Regression_Bug67.

        private void TestRegression_Bug67()
        {
            Log("Verifying Regression_Bug67 does not repro...");
            MouseInput.MouseClick(_testedElement);
            KeyboardInput.TypeString("^a{DEL}First line.{ENTER}{UP}{HOME}{RIGHT 3}{ENTER}{ENTER}");

            QueueDelegate(TestRegression_Bug67Additional);
        }

        private void TestRegression_Bug67Additional()
        {
            Log("Verifying that more Enter keypresses can be handled after layout...");
            KeyboardInput.TypeString("^{HOME}{RIGHT}{ENTER}");

            QueueDelegate(TestRegression_Bug67AdditionalAfterLayout);
        }

        private void TestRegression_Bug67AdditionalAfterLayout()
        {
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(TestRegression_Bug67Finished);
        }

        private void TestRegression_Bug67Finished()
        {
            Log("Regression_Bug67 not reproed.");

            _topPanel.Children.Remove(_testedElement);
            QueueDelegate(TestRegression_Bug68);
        }

        #endregion Regression_Bug67.

        #region Regression_Bug68 - siri

        private void TestRegression_Bug68()
        {
            if (_testedWrapper.Element is TextBox)
            {
                //This repro is only for RichTextBox
                QueueDelegate(FinishedIteration);
                return;
            }

            Log("Verifying Regression_Bug68.");
            MainWindow.Content = null;
            MainWindow.Content = _testedElement;
            _testedWrapper.Text = "";
            _testedElement.SetValue(KeyboardNavigation.AcceptsReturnProperty, true);
            _testedElement.SetValue(FrameworkElement.FocusableProperty, true);

            Paragraph para = new Paragraph();
            para.Inlines.Add("This is a test");
            para.Inlines.Add(new LineBreak());
            para.Inlines.Add("This is a test");
            ((RichTextBox)_testedWrapper.Element).Document.Blocks.Add(para);       

            QueueDelegate(DoMouseClick);
        }

        private void DoMouseClick()
        {
            Log("Typing Cntrl-End...");
            MouseInput.MouseClick(_testedElement);
            KeyboardInput.TypeString("^{END}");

            QueueDelegate(HitHome);
        }

        private void HitHome()
        {
            KeyboardInput.TypeString("{HOME}");
            QueueDelegate(VerifyRegression_Bug68);
        }

        private void VerifyRegression_Bug68()
        {
            TextSelection textSelection;
            TextPointer tp;
            string leftString, rightString;
            leftString = _testedWrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            Log("String to the left: [" + leftString + "]");
            rightString = _testedWrapper.GetTextOutsideSelection(LogicalDirection.Forward);
            Log("String to the right: [" + rightString + "]");

            textSelection = _testedWrapper.SelectionInstance;
            tp = textSelection.Start;

            Verifier.Verify(tp.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart,
                "TextPointer of the caret should be after a TextSymbol of type ElementStart", true);
            Verifier.Verify(tp.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text,
                "TextPointer of the caret should be before a TextSymbol of type Character", true);
            tp = tp.GetPositionAtOffset(-1);
            Verifier.Verify(tp.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd,
                "TextPointer of the caret should be after a TextSymbol of type ElementEnd", true);
            Verifier.Verify(tp.GetAdjacentElement(LogicalDirection.Backward) is LineBreak,
                "LineBreak element should be in Backward LogicalDirection to the caret position", true);
            tp = tp.GetPositionAtOffset(-1);
            Verifier.Verify(tp.Parent.GetType() == typeof(LineBreak),
                "Verifying that pointer is now inside LineBreak", true);

            Log("Regression_Bug68 has not repro-ed");
            QueueDelegate(FinishedIteration);
        }

        #endregion Regression_Bug68

        private void FinishedIteration()
        {
            Log("Type tested.");
            _testedTypeIndex++;

            if (_testedTypeIndex == TextEditableType.Values.Length)
            {
                Logger.Current.QueueSuccess();
            }
            else
            {
                QueueDelegate(DoIteration);
            }
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Object being edited.</summary>
        private FrameworkElement _testedElement;

        /// <summary>Wrapper for object being edited.</summary>
        private UIElementWrapper _testedWrapper;

        /// <summary>Description of type being tested.</summary>
        private TextEditableType _editableType;

        /// <summary>Index of type being tested.</summary>
        private int _testedTypeIndex;

        private StackPanel _topPanel;

        private int _toggleFormatIndex;

        private object _initialToggleFormat;

        #endregion Private fields.

        #region Inner types.

        /// <summary>
        /// Holds information about formatting properties that can be toggled.
        /// </summary>
        class ToggleFormat
        {
            internal ToggleFormat(string keyboardInput,
                DependencyProperty changedProperty)
            {
                this.KeyboardInput = keyboardInput;
                this.ChangedProperty = changedProperty;
            }

            internal readonly string KeyboardInput;
            internal readonly DependencyProperty ChangedProperty;
            internal static ToggleFormat[] Formats = new ToggleFormat[] {
                new ToggleFormat(EditingCommandData.ToggleBold.KeyboardShortcut, TextBlock.FontWeightProperty),
                new ToggleFormat(EditingCommandData.ToggleItalic.KeyboardShortcut, TextBlock.FontStyleProperty),
                new ToggleFormat(EditingCommandData.ToggleUnderline.KeyboardShortcut, TextBlock.TextDecorationsProperty),
            };
        }

        #endregion Inner types.
    }

    /// <summary>
    /// Verifies that the TextEditor will handle the mouse in a way
    /// that allows horizontal scrolling.
    /// </summary>
    [Test(2, "Editor", "TextEditorRepro479", MethodParameters = "/TestCaseType=TextEditorRepro479")]
    [TestOwner("Microsoft"), TestTactics("300"), TestBugs("479,480")]
    public class TextEditorRepro479: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupCombinations();
            QueueDelegate(TestCombination);
        }

        private void TestCombination()
        {
            if (!ReadNextCombination())
            {
                Logger.Current.ReportSuccess();
                return;
            }

            Log("Creating and laying out control...");
            _box = _editableType.CreateInstance() as TextBoxBase;
           
            if (_box == null)
            {
                Log("This test case only supports TextBoxBase instances.");
                QueueDelegate(TestCombination);
                return;
            }
            _box.FontSize = 96.0;
            if (_box is TextBox)
            {
                ((TextBox)_box).TextWrapping = TextWrapping.NoWrap;
                Log("Wrap: " + ((TextBox)_box).TextWrapping);
            }            
            MainWindow.Content = _box;
            MainWindow.Width = 200;

            Log("Moving the window to the right...");
            MainWindow.Left = 100;
            _wrapper = new UIElementWrapper(_box);
            QueueDelegate(MoveToEnd);
        }

        private void MoveToEnd()
        {
            Log("Moving caret to the end of the control...");
            MouseInput.MouseClick(_box);
            
            _wrapper.Text = "very very long content that is left-to-right " +
                "and is almost guaranteed to scroll with a big font size";
            KeyboardInput.TypeString("^{END}{LEFT 1}");
            QueueDelegate(CheckAtEnd);
        }

        private void CheckAtEnd()
        {
            DispatcherHelper.DoEvents();
            Log("Verifying that the control has scrolled if its a TextBox...");
            _scrollOffsetAtEnd = _box.HorizontalOffset;
            Log("Scroll offset at end: " + _scrollOffsetAtEnd);
            Verifier.Verify(!double.IsNaN(_scrollOffsetAtEnd), "Offset is a real number.", true);

            Rect charRect;
            if (_box is RichTextBox)
            {
                Verifier.Verify(_scrollOffsetAtEnd == 0, "Offset is = zero for a RTB.", true);
                charRect = _wrapper.GetGlobalCharacterRect(((RichTextBox)_box).CaretPosition);
            }
            else
            {
                Verifier.Verify(_scrollOffsetAtEnd > 0, "Offset is greater than zero.", true);
                charRect = _wrapper.GetGlobalCharacterRect(((TextBox)_box).CaretIndex);
            }

            Log("Performing mouse gestures to scroll...");
            _box.Focus();
            MouseInput.MouseDragInOtherThread(new Point(charRect.TopLeft.X + 2, charRect.TopLeft.Y + charRect.Height/2),
                new Point(0, charRect.TopLeft.Y + charRect.Height / 2), true, new TimeSpan(0, 0, 2), new SimpleHandler(CheckAtStart), _box.Dispatcher);
        }

        private void CheckAtStart()
        {
            double scrollOffsetAtStart;

            Log("Checking scrolling position after 4 seconds...");
            scrollOffsetAtStart = _box.HorizontalOffset;
            Log("Scroll offset at start: " + scrollOffsetAtStart);
            if (_box is RichTextBox)
            {
                Verifier.Verify(scrollOffsetAtStart == _scrollOffsetAtEnd,
                    "RTB doesnt scroll.", true);
            }
            else
            {
                Verifier.Verify(scrollOffsetAtStart < _scrollOffsetAtEnd,
                    "TextBox has scrolled to the left.", true);
            }
            QueueDelegate(TestCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        private bool ReadNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (_engine.Next(values))
            {
                _editableType = (TextEditableType) values["TextEditableType"];

                Log("Editable type: " + _editableType.Type.Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupCombinations()
        {
            Dimension[] dimensions = new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),
            };
            _engine = CombinatorialEngine.FromDimensions(dimensions);
        }

        #endregion Helper methods.

        #region Private fields.

        private CombinatorialEngine _engine;
        private TextEditableType _editableType;

        private TextBoxBase _box;
        private double _scrollOffsetAtEnd;
        private UIElementWrapper _wrapper;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the TextEditor doesnt ignore FrameworkElement.IsEnabledProperty
    /// </summary>
    [Test(0, "Editor", "TextEditorRepro481", MethodParameters = "/TestCaseType=TextEditorRepro481")]
    [TestOwner("Microsoft"), TestTactics("301"), TestBugs("481")]
    public class TextEditorRepro481: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _mainPanel = new DockPanel();

            _textbox = new TextBox();
            _textbox.IsEnabled = true;
            _textbox.Height = 30;

            _button = new Button();
            _button.Content = "button!";
            _button.Click += new RoutedEventHandler(button_Click);

            _mainPanel.Children.Add(_textbox);
            _mainPanel.Children.Add(_button);

            DockPanel.SetDock(_textbox, Dock.Top);
            MainWindow.Content = _mainPanel;
            QueueDelegate(DoInputAction);
        }

        private void DoInputAction()
        {
            MouseInput.MouseClick(_button);
        }

        private void button_Click(object sender, RoutedEventArgs args)
        {
            _mainPanel.IsEnabled = false;
            QueueDelegate(TypeInTextBox);
        }

        private void TypeInTextBox()
        {
            Log("Trying to type on textbox");
            MouseInput.MouseClick(_textbox);
            KeyboardInput.TypeString("This is a test");
            Log("Typing finished: " + _textbox.IsEnabled.ToString());
            QueueDelegate(VerifyTextEditorIsEnabled);
        }

        private void VerifyTextEditorIsEnabled()
        {
            Verifier.Verify(_textbox.Text == "", "TextBox should be empty", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private TextBox _textbox;
        private DockPanel _mainPanel;
        private Button _button;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that TextBlock and TextFlow elements can be set up in read-only
    /// mode.
    /// </summary>
    /// <ImplementedCustomerScenario CustomerName='Encarta' ScenarioName='RichReadOnly' />
    [TestOwner("Microsoft"), TestTactics("302"), TestBugs("420,482"), TestSample("TextEditor")]
    public class TextEditorReadOnly: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            QueueDelegate(DoIteration);
        }

        /// <summary>Runs the test case for a specific textEditableTypeIndex.</summary>
        private void DoIteration()
        {
            TextEditableType textEditableType;
            string content;
            string xaml;

            System.Diagnostics.Debug.Assert(_typeIndex <= TextEditableType.Values.Length);
            if (_typeIndex == TextEditableType.Values.Length)
            {
                Logger.Current.ReportSuccess();
                return;
            }

            textEditableType = TextEditableType.Values[_typeIndex];

            if (textEditableType.SupportsParagraphs)
            {
                content = RichTextContent;
            }
            else
            {
                content = PlainTextContent;
            }

            xaml = "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><" +
                textEditableType.XamlName + " Name='TestedElement' ";
            if (!textEditableType.IsAggregate)
            {
                xaml += NonAggregateProperties;
            }
            xaml += "IsReadOnly='True'>" + content +
                "</" + textEditableType.XamlName + "></DockPanel>";

            ActionItemWrapper.SetMainXaml(xaml);
            QueueDelegate(PerformInput);
        }

        private void PerformInput()
        {
            Log("Getting element to be tested...");
            _testedElement = (FrameworkElement) ActionItemWrapper.FindElement("TestedElement");
            if (_testedElement == null)
            {
                throw new Exception("Cannot find element 'TestedElement'");
            }
            _testedWrapper = new UIElementWrapper(_testedElement);

            Log("Clicking on element...");
            MouseInput.MouseClick(_testedElement);
            KeyboardInput.TypeString("   {LEFT}{LEFT}+{END}^b^x^z^v");

            QueueDelegate(CheckInput);
        }

        private void CheckInput()
        {
            TextSelection selection;

            Log("Verifying that selection is available after clicking...");
                selection = _testedWrapper.SelectionInstance;
            Verifier.Verify(selection != null, "Selection is available.", true);

            Verifier.Verify(_testedElement.IsKeyboardFocusWithin == true,
                "Tested element has focus.", true);

            Log("Current text:  [" + _testedWrapper.Text + "]");
            Log("Expected text: [" + PlainTextContent + "]");
            Verifier.Verify(_testedWrapper.Text == PlainTextContent,
                "Content has not been modified.", true);

            Verifier.Verify(selection.Text.Length > 0,
                "Selection keystrokes were processed.", true);

            Log("Disabling read-only mode and typing '1'...");
            ReflectionUtils.SetProperty(_testedElement, "IsReadOnly", false);
            KeyboardInput.TypeString("{HOME}1");
            QueueDelegate(CheckTyped);
        }

        private void CheckTyped()
        {
            Log("Checking that '1' was inserted...");
            Log("Current text:  [" + _testedWrapper.Text + "]");
            Verifier.Verify(_testedWrapper.Text.IndexOf("1") != -1,
                "Digit '1' found.", true);

            Log("Re-enabling read-only mode and trying to undo...");
            ReflectionUtils.SetProperty(_testedElement, "IsReadOnly", true);
            //testedElement.SetValue(TextEditor.IsReadOnlyProperty, true);
            KeyboardInput.TypeString("^z");

            QueueDelegate(CheckUndoInReadOnly);
        }

        private void CheckUndoInReadOnly()
        {
            Log("Checking that '1' was not removed...");
            Log("Current text:  [" + _testedWrapper.Text + "]");
            Verifier.Verify(_testedWrapper.Text.IndexOf("1") != -1,
                "Digit '1' found.", true);

            FinishedIteration();
        }

        private void FinishedIteration()
        {
            _typeIndex++;
            QueueDelegate(DoIteration);
        }

        #endregion Main flow.

        #region Private fields.

        private FrameworkElement _testedElement;
        private UIElementWrapper _testedWrapper;
        private const string NonAggregateProperties =
            " TextEditor.IsEnabled='True' Focusable='True' ";
        private const string PlainTextContent = "This is some sample content.";
        private const string RichTextContent =
            "<Paragraph>This is some <Bold>sample</Bold> content.</Paragraph>";
        private int _typeIndex;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that keyboard editing works with all types
    /// of selection.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-0: EditingTest.exe /TestCaseType=TextEditorKeyboardEditing /Pri=0 (~56 seconds)
    /// Pri-1: EditingTest.exe /TestCaseType=TextEditorKeyboardEditing /Pri=1 (~7min, 14seconds)
    /// Pri-3: EditingTest.exe /TestCaseType=TextEditorKeyboardEditing /Pri=2 (~27min)
    /// </remarks>
    [Test(0, "Editor", "TextEditorKeyboardEditing", MethodParameters="/TestCaseType=TextEditorKeyboardEditing /Pri=0", Timeout=240)]    
    [TestOwner("Microsoft"), TestTactics("316,317,318"), TestBugs("28,483")]
    public class TextEditorKeyboardEditing: ManagedCombinatorialTestCase
    {
        #region Combinations support.

        /// <summary>Reads values for a specific combination.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            if (result)
            {
                // Ignore cases where AcceptsReturn isn't supported anyway.
                bool supportsAcceptsReturn;
                supportsAcceptsReturn = typeof(TextBoxBase).IsAssignableFrom(_editableType.Type);
                result = _acceptsReturn == false || supportsAcceptsReturn;
            }

            return result;
        }

        #endregion Combinations support.

        #region Main flow.

        /// <summary>Tests a specific combination.</summary>
        protected override void DoRunCombination()
        {
            FrameworkElement element;   // Element using in combination.
            TextBoxBase textbox;        // Element, typed as TextBoxBase.

            element = _editableType.CreateInstance();
            if (element is RichTextBox)
            {
                element.Width = 250;                
            }
            _wrapper = new UIElementWrapper(element);

            // Set the AcceptsReturn value, if applicable.
            textbox = element as TextBoxBase;
            if (textbox != null)
            {
                textbox.AcceptsReturn = _acceptsReturn;
            }

            _wrapper.Text = _stringData.Value;

            SetupTestElement(element, BeforePrepare);
        }

        /// <summary>
        /// Sets up the test element in the visual tree and makes it ready for
        /// editing.
        /// </summary>
        /// <param name="element">Element to set up.</param>
        /// <param name="queueCallback">Callback to queue after completion.</param>
        protected virtual void SetupTestElement(FrameworkElement element,
            SimpleHandler queueCallback)
        {
            TestElement = element;
            QueueDelegate(queueCallback);
        }

        private void BeforePrepare()
        {
            _wrapper.Element.Focus();
            _textSelection.PrepareForSelection(_wrapper);
            QueueDelegate(AfterPrepare);
        }

        private void AfterPrepare()
        {
            _wrapper.Element.Focus();
            if (_textSelection.Select(_wrapper))
            {
                Log("Adjusted selection for: " + _textSelection.TestValue);
                QueueDelegate(AfterSelection);
            }
            else
            {
                Log("Unable to select: " + _textSelection.TestValue);
                QueueDelegate(NextCombination);
            }
        }

        private void AfterSelection()
        {
            Log("Performing editing for action: " + KeyboardEditing.TestValue);
            _editingState = KeyboardEditing.CaptureBeforeEditing(_wrapper);
            KeyboardEditing.PerformAction(_wrapper, AfterEditing, _preferCommands);
        }

        private void AfterEditing()
        {
            Log("Verifying state after editing...");
            KeyboardEditing.VerifyEditing(_editingState);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Protected fields.

        /// <summary>The keyboard editing action to perform.</summary>
        protected KeyboardEditingData KeyboardEditing;

        #endregion Protected fields.

        #region Private fields.

        /// <summary>Editing state during verification.</summary>
        private KeyboardEditingState _editingState;

        /// <summary>Wrapper around instance being edited.</summary>
        private UIElementWrapper _wrapper;

        /// <summary>Whether AcceptsReturn should be set on the control.</summary>
        private bool _acceptsReturn =false;

        /// <summary>The selection to make before performing an action.</summary>
        private TextSelectionData _textSelection=null;

        /// <summary>The type of control to test.</summary>
        private TextEditableType _editableType=null;

        /// <summary>The string data to use as sample text.</summary>
        private StringData _stringData=null;

        /// <summary>Whether to use commands instead of (slower) keyboard input when possible.</summary>
        private bool _preferCommands = false;

        #endregion Private fields.
    }


    /// <summary>
    /// Verifies that keyboard editing works regardless of the container
    /// for the edited control.
    /// The following execution modes are expected:
    /// EditingTest.exe /TestCaseType:EditingContainerTest /Test=SingleLine_EditingValues /InputMonitorEnabled:False
    /// EditingTest.exe /TestCaseType:EditingContainerTest /Test=Test=MultipleLine_EditingValues /InputMonitorEnabled:False
    /// EditingTest.exe /TestCaseType:EditingContainerTest /Test=SingleLine_NavigationValues
    /// EditingTest.exe /TestCaseType:EditingContainerTest /Test=MultipleLine_NavigationValues
    /// </summary>
    [Test(3, "Editor", "EditingContainerTest1_TB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_EditingValues_TB /InputMonitorEnabled:False", Timeout = 800, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest1_RTB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_EditingValues_RTB /InputMonitorEnabled:False", Timeout = 800, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest1_PB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_EditingValues_PB /InputMonitorEnabled:False", Timeout = 800, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest2_TB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_NavigationValues_TB", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest2_RTB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_NavigationValues_RTB", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest2_PB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=SingleLine_NavigationValues_PB", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest3_TB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=MultipleLine_EditingValues_TB /InputMonitorEnabled:False", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest3_RTB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=MultipleLine_EditingValues_RTB /InputMonitorEnabled:False", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest4_TB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=MultipleLine_NavigationValues_TB", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "EditingContainerTest4_RTB", MethodParameters = "/TestCaseType:EditingContainerTest /Test=MultipleLine_NavigationValues_RTB", Timeout = 600, Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("319,320,321,322"), TestWorkItem("26"),
     TestBugs("484,485,486,487")]
    public class EditingContainerTest: TextEditorKeyboardEditing
    {
        #region Main flow.

        /// <summary>Reads this combination and evaluates for processing.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            result = result && !
                (this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageDown ||
                 this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageDownControl ||
                 this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageDownShift ||
                 this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageUp ||
                 this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageUpControl ||
                 this.KeyboardEditing.TestValue == KeyboardEditingTestValue.PageUpShift);

            return result;
        }

        /// <summary>
        /// Sets up the test element in the visual tree and makes it ready for
        /// editing.
        /// </summary>
        /// <param name="element">Element to set up.</param>
        /// <param name="queueCallback">Callback to queue after completion.</param>
        protected override void SetupTestElement(FrameworkElement element,
            SimpleHandler queueCallback)
        {
            _container = Container.CreateElement();
            TestElement = new Canvas();

            EditingContainer.PrepareForEditing(TestElement, _container,
                element, queueCallback);
        }

        #endregion Main flow.

        #region Private fields.

        private EditingContainer Container = null;

        private FrameworkElement _container;

        #endregion Private fields.
    }

    class EditingContainer
    {
        #region Constructors.

        private EditingContainer(Type containerType)
        {
            this._containerType = containerType;
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Creates an element instance of the container type represented
        /// by this object.
        /// </summary>
        /// <returns>A new FrameworkElement that can contain editable controls.</returns>
        public FrameworkElement CreateElement()
        {
            return (FrameworkElement)Activator.CreateInstance(ContainerType);
        }

        /// <summary>
        /// Prepares the specified container to show the editable child,
        /// and embeds everything in the given top panel.
        /// </summary>
        public static void PrepareForEditing(FrameworkElement topPanel,
            FrameworkElement editingContainer, FrameworkElement child,
            SimpleHandler queueCallback)
        {
            bool queueBeforeExit;
            FrameworkElement elementForTopPanel;

            if (topPanel == null)
            {
                throw new ArgumentNullException("topPanel");
            }
            if (editingContainer == null)
            {
                throw new ArgumentNullException("editingContainer");
            }
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            elementForTopPanel = editingContainer;
            queueBeforeExit = true;
            if (editingContainer is Panel)
            {
                Panel panel = (Panel) editingContainer;

                panel.Children.Add(child);
            }
            else if (editingContainer is ListBox)
            {
                ListBox listBox = (ListBox)editingContainer;
                listBox.Items.Add(child);
            }
            else if (editingContainer is ContentControl)
            {
                ContentControl contentControl = (ContentControl)editingContainer;
                contentControl.Content = child;
            }
            else if (editingContainer is ContextMenu)
            {
                ContextMenu contextMenu = (ContextMenu)editingContainer;
                contextMenu.Items.Add(child);

                System.Windows.Shapes.Rectangle rectangle;
                rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Height = 100d;
                rectangle.Width = 100d;
                rectangle.Fill = Brushes.Green;
                rectangle.ContextMenu = contextMenu;

                elementForTopPanel = rectangle;
                queueBeforeExit = false;
                QueueHelper.Current.QueueDelegate(delegate() {
                    MouseInput.RightMouseClick(rectangle);
                    QueueHelper.Current.QueueDelegate(queueCallback);
                });
            }

            if (elementForTopPanel != null)
            {
                if (topPanel is ContentControl)
                {
                    ((ContentControl)topPanel).Content = elementForTopPanel;
                }
                else if (topPanel is Panel)
                {
                    ((Panel)topPanel).Children.Add(elementForTopPanel);
                }
                ((FrameworkElement)editingContainer).Tag = RemoveTag;
            }
            if (queueBeforeExit)
            {
                QueueHelper.Current.QueueDelegate(queueCallback);
            }
        }

        /// <summary>Provides a string representation of this object.</summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return ContainerType.Name;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Type of container for editable controls.</summary>
        public Type ContainerType
        {
            get { return this._containerType; }
        }

        /// <summary>Available editing containers.</summary>
        public static EditingContainer[] Values
        {
            get
            {
                if (s_values == null)
                {
                    s_values = new EditingContainer[] {
                        new EditingContainer(typeof(StackPanel)),
                        new EditingContainer(typeof(DockPanel)),
                        new EditingContainer(typeof(Canvas)),
                        new EditingContainer(typeof(Grid)),
                        new EditingContainer(typeof(WrapPanel)),
                        new EditingContainer(typeof(Button)),
                        new EditingContainer(typeof(ContextMenu)),
                        new EditingContainer(typeof(ListBox)),
                        new EditingContainer(typeof(ScrollViewer)),
                    };
                }
                return s_values;
            }
        }

        /// <summary>
        /// Tag used to indicate which children should be removed from
        /// the top parent after a call to PrepareForEditing.
        /// </summary>
        public const string RemoveTag = "RemoveTag";

        #endregion Public properties.

        #region Private fields.

        private Type _containerType;
        private static EditingContainer[] s_values;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that keyboard editing works with all types of strings.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=TextScriptEditing (~2m)
    /// Pri-3: EditingTest.exe /TestCaseType=TextScriptEditing /ExhaustiveSelections=True
    /// </remarks>
    [TestOwner("Microsoft"), TestTactics("123"), TestBugs("488"),
     TestArgument("ExhaustiveSelections", "Whether exhaustive selections should be tested.")]
    public class TextScriptEditing: CustomCombinatorialTestCase
    {
        #region Combinations support.

        /// <summary>Gets dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            object[] selectionData;
            KeyboardEditingData[] editingData;
            StringData[] stringData;

            selectionData =
                (ConfigurationSettings.Current.GetArgumentAsBool("ExhaustiveSelections"))?
                TextSelectionData.Values :
                new object[] { TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentStart),
                               TextSelectionData.GetForValue(TextSelectionTestValue.EmptyDocumentEnd)};
            editingData = KeyboardEditingData.Values;
            stringData = StringData.Values;

            return new Dimension[] {
                new Dimension("KeyboardEditing", editingData),
                new Dimension("TextSelection", selectionData),
                new Dimension("StringData", stringData),
                };
        }

        /// <summary>Reads values for a specific combination.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _selectionData = (TextSelectionData)values["TextSelection"];
            _editingData = (KeyboardEditingData)values["KeyboardEditing"];
            _stringData = (StringData)values["StringData"];

            return true;
        }

        #endregion Combinations support.

        #region Main flow.

        /// <summary>Tests a specific combination.</summary>
        protected override void DoRunCombination()
        {
            // Use an element of the appropriate type.
            if (_control == null)
            {
                _control = new TextBox();
                _wrapper = new UIElementWrapper(_control);
                TestElement = _control;
            }
            _control.Text = _stringData.Value;
            QueueDelegate(BeforePrepare);
        }

        private void BeforePrepare()
        {
            _control.Focus();
            _selectionData.PrepareForSelection(_wrapper);
            QueueDelegate(AfterPrepare);
        }

        private void AfterPrepare()
        {
            if (_selectionData.Select(_wrapper))
            {
                Log("Adjusted selection for: " + _selectionData.TestValue);
                QueueDelegate(AfterSelection);
            }
            else
            {
                Log("Unable to select: " + _selectionData.TestValue);
                QueueDelegate(NextCombination);
            }
        }

        private void AfterSelection()
        {
            Log("Performing editing for action: " + _editingData.TestValue);
            _editingState = _editingData.CaptureBeforeEditing(_wrapper);
            _editingData.PerformAction(_wrapper, AfterEditing);
        }

        private void AfterEditing()
        {
            Log("Verifying state after editing...");
            _editingData.VerifyEditing(_editingState);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Editing state during verification.</summary>
        private KeyboardEditingState _editingState;

        /// <summary>Control being edited.</summary>
        private TextBox _control;

        /// <summary>Wrapper around control being edited.</summary>
        private UIElementWrapper _wrapper;

        private TextSelectionData _selectionData;
        private KeyboardEditingData _editingData;
        private StringData _stringData;

        #endregion Private fields.
    }

    /// <summary>Test for events which TextEditor listens</summary>
    [Test(0, "Editor", "TextEditorEventTesting", MethodParameters="/TestCaseType:TextEditorEventTesting", Timeout=200)]
    [TestOwner("Microsoft"), TestTactics("323"), TestBugs("489"), TestWorkItem("27")]
    public class TextEditorEventTesting : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            // Prepare the control to be tested.
            _editingControl = (Control)_editableType.CreateInstance();

            _wrapper = new UIElementWrapper(_editingControl);

            _panel = new StackPanel();
            _button = new Button();
            _panel.Children.Add(_button);
            _panel.Children.Add(_editingControl);
            TestElement = _panel;

            QueueDelegate(AfterRender);
        }

        private void AfterRender()
        {
            _editingControl.Focus();

            Log("Adding events to the EditingControl");
            AttachAllEvents();

            QueueDelegate(RemoveEditingControlFromTree);
        }

        private void RemoveEditingControlFromTree()
        {
            //Removing the EditingControl from the tree after events are attached.
            //No crashes or exceptions expected.
            Log("Removing EditingControl from Tree");
            _panel.Children.Remove(_editingControl);

            //Change few properties which query layout after EditingControl is removed from Tree.
            //No crashes or exception expected
            FlipFlowDirectionProperty();
            _wrapper.Clear();

            QueueDelegate(ReAddEditingControl);
        }

        private void ReAddEditingControl()
        {
            Log("ReAdding EditingControl to the Tree");
            _panel.Children.Add(_editingControl);
            QueueDelegate(TestKeyDownEvent);
        }

        private void TestKeyDownEvent()
        {
            DetachAllEvents();

            //Revert the change (in Property value) done in method RemoveEditingControlFromTree
            FlipFlowDirectionProperty();

            _editingControl.Focus();

            //Add handlers for KeyDown events
            _editingControl.AddHandler(Control.KeyDownEvent, new KeyEventHandler(_editingControl_KeyDown), true);
            //NOTE: if add handler using _editingControl.KeyDown, events for Enter/Delete down trigger because
            //they are marked handled before we get it.
            _editingControl.PreviewKeyDown += new KeyEventHandler(_editingControl_PreviewKeyDown);

            _keyDownEventCount = 0;
            QueueDelegate(DoKeyboardInput);
        }

        private void DoKeyboardInput()
        {
            _expectedKey = Key.A;
            KeyboardInput.TypeString("a");

            QueueDelegate(FocusAway);
        }

        private void FocusAway()
        {
            Verifier.Verify(_keyDownEventCount == 2, "Verifying the Event Count", false);

            Log("Changing the focus to Button");
            _button.Focus();
            QueueDelegate(FocusBack);
        }

        private void FocusBack()
        {
            Log("Changing the focus back to EditingControl");
            _editingControl.Focus();

            QueueDelegate(DoKeyboardInputAfterRefocus);
        }

        private void DoKeyboardInputAfterRefocus()
        {
            Log("Verifying KeyDown event after EditingControl was focus away and back");

            _expectedKey = Key.Back;
            KeyboardInput.TypeString("{BACKSPACE}");

            QueueDelegate(DoKeyUpEventOnlyTesting);
        }

        private void DoKeyUpEventOnlyTesting()
        {
            Verifier.Verify(_keyDownEventCount == 4, "Verifying Event Count after focusing back", false);

            //Remove handlers for KeyDown events
            _editingControl.RemoveHandler(Control.KeyDownEvent, new KeyEventHandler(_editingControl_KeyDown));
            _editingControl.PreviewKeyDown -= new KeyEventHandler(_editingControl_PreviewKeyDown);

            //Clear the contents
            _wrapper.Clear();

            Log("Verifying when KeyUp event is raised programmatically without corresponding KeyDown event");

            //Programmatically raise KeyUp event. TextEditor only listens to KeyDown events and
            //hence this should be like a no-op.
            KeyEventArgs keyUpEvent = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice,
                InputManager.Current.PrimaryKeyboardDevice.ActiveSource, 0, Key.A);
            keyUpEvent.RoutedEvent = Control.KeyUpEvent;
            _editingControl.RaiseEvent(keyUpEvent);

            Verifier.Verify(!_wrapper.Text.Contains("a"), "Verifying that KeyUp event didnt go through", false);

            //Move the focus on the Button
            _button.Focus();

            //Place the Mouse over the EditingControl
            MouseInput.MouseMove(_editingControl);

            QueueDelegate(DoMouseUpEventOnlyTesting);
        }

        private void DoMouseUpEventOnlyTesting()
        {
            Log("Verifying when MouseUp event is raised programmatically without corresponding MouseDown event");

            //Programmatically raise MouseUp event. TextEditor listens to MouseDown events and
            //hence this should be like a no-op.
            MouseButtonEventArgs mouseUpEvent = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice,
                0, MouseButton.Left);
            mouseUpEvent.RoutedEvent = System.Windows.Input.Mouse.PreviewMouseUpEvent;
            _editingControl.RaiseEvent(mouseUpEvent);

            QueueDelegate(DoLostKeyboardFocusOnlyTesting);
        }

        private void DoLostKeyboardFocusOnlyTesting()
        {
            //verify that button is still focused and didnt change due to the MouseUp event.
            Verifier.Verify(_button.IsKeyboardFocused, "Verify that focus didnt change due to MouseUp event", false);

            Log("Verifying when LostKeyboardFocus event is raised programmatically without EditingControl being focused");

            //Programmatically raise LostKeyboardFocus event when TextBox is not focused.
            KeyboardFocusChangedEventArgs focusChangedEvent = new KeyboardFocusChangedEventArgs(InputManager.Current.PrimaryKeyboardDevice,
                0, _editingControl, _button);
            focusChangedEvent.RoutedEvent = Control.LostKeyboardFocusEvent;
            _editingControl.RaiseEvent(focusChangedEvent);

            Verifier.Verify(_button.IsKeyboardFocused, "Verify that focus didnt change due to LostKeyboardFocus event on EditingControl", false);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow

        #region Helper functions

        private void FlipFlowDirectionProperty()
        {
            _editingControl.FlowDirection = (_editingControl.FlowDirection == FlowDirection.LeftToRight) ?
                FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void AttachAllEvents()
        {
            _editingControl.TextInput += new TextCompositionEventHandler(_editingControl_TextInput);
            _editingControl.PreviewTextInput += new TextCompositionEventHandler(_editingControl_PreviewTextInput);
            _editingControl.KeyDown += new KeyEventHandler(_editingControl_KeyDown);
            _editingControl.PreviewKeyDown += new KeyEventHandler(_editingControl_PreviewKeyDown);
            _editingControl.MouseEnter += new MouseEventHandler(_editingControl_MouseEnter);
            _editingControl.PreviewDragLeave += new DragEventHandler(_editingControl_PreviewDragLeave);
            _editingControl.PreviewDragEnter += new DragEventHandler(_editingControl_PreviewDragEnter);
            _editingControl.DragLeave += new DragEventHandler(_editingControl_DragLeave);
            _editingControl.DragEnter += new DragEventHandler(_editingControl_DragEnter);

            if (_editingControl is TextBoxBase)
            {
                ((TextBoxBase)_editingControl).SelectionChanged += new RoutedEventHandler(TextBoxBase_SelectionChanged);
            }
        }

        private void DetachAllEvents()
        {
            _editingControl.TextInput -= new TextCompositionEventHandler(_editingControl_TextInput);
            _editingControl.PreviewTextInput -= new TextCompositionEventHandler(_editingControl_PreviewTextInput);
            _editingControl.KeyDown -= new KeyEventHandler(_editingControl_KeyDown);
            _editingControl.PreviewKeyDown -= new KeyEventHandler(_editingControl_PreviewKeyDown);
            _editingControl.MouseEnter -= new MouseEventHandler(_editingControl_MouseEnter);
            _editingControl.PreviewDragLeave -= new DragEventHandler(_editingControl_PreviewDragLeave);
            _editingControl.PreviewDragEnter -= new DragEventHandler(_editingControl_PreviewDragEnter);
            _editingControl.DragLeave -= new DragEventHandler(_editingControl_DragLeave);
            _editingControl.DragEnter -= new DragEventHandler(_editingControl_DragEnter);

            if (_editingControl is TextBoxBase)
            {
                ((TextBoxBase)_editingControl).SelectionChanged -= new RoutedEventHandler(TextBoxBase_SelectionChanged);
            }
        }

        #endregion Helper functions

        #region Event handlers

        void _editingControl_DragEnter(object sender, DragEventArgs e)
        {
            Log("DragEnter event fired");
        }

        void _editingControl_DragLeave(object sender, DragEventArgs e)
        {
            Log("DragLeave event fired");
        }

        void _editingControl_KeyDown(object sender, KeyEventArgs e)
        {
            Log("KeyDown event fired: " + e.Key.ToString());

            Verifier.Verify(e.Key == _expectedKey, "Verifying that right Key is passed to the KeyDown event", false);
            _keyDownEventCount++;
        }

        void _editingControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Log("MouseEnter event fired");
        }

        void _editingControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            Log("PreviewDragEnter event fired");
        }

        void _editingControl_PreviewDragLeave(object sender, DragEventArgs e)
        {
            Log("PreviewDragLeave event fired");
        }

        void _editingControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Log("PreviewKeyDown event fired: " + e.Key.ToString());

            Verifier.Verify(e.Key == _expectedKey, "Verifying that right Key is passed to the PreviewKeyDown event", false);
            _keyDownEventCount++;
        }

        void _editingControl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Log("PreviewTextInput event is fired");
        }

        void _editingControl_TextInput(object sender, TextCompositionEventArgs e)
        {
            Log("TextInput event is fired");
        }

        void TextBoxBase_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Log("TextBoxBase.SelectionChanged event fired");
        }

        #endregion Event handlers

        #region Private fields

        /// <summary>EditableType of control tested.</summary>
        private TextEditableType _editableType =null;

        /// <summary>Editing control being tested</summary>
        private Control _editingControl;
        private UIElementWrapper _wrapper;
        private StackPanel _panel;
        private int _keyDownEventCount;
        private Button _button;
        private Key _expectedKey;

        #endregion Private fields
    }

    /// <summary>Tests synchronous input ness in TextBox</summary>
    [Test(2, "Editor", "TextEditorSynchronousInput", MethodParameters="/TestCaseType:TextEditorSynchronousInput")]
    [TestOwner("Microsoft"), TestTactics("324"), TestBugs("490"), TestLastUpdatedOn("June 8, 2006")]
    public class TextEditorSynchronousInput : CustomTestCase
    {
        #region Private Members

        private CustomTB _customTB;
        private string _typeString = "a";
        private static bool s_result = false;

        #endregion Private Members

        #region Private types

        private class CustomTB : TextBox
        {
            /// <summary>Initializes a new CustomTB instance.</summary>
            public CustomTB() : base()
            {
            }

            protected override void OnTextInput(System.Windows.Input.TextCompositionEventArgs e)
            {
                base.OnTextInput(e);
                if (this.Text != "a")
                {
                    TextEditorSynchronousInput.s_result = false;
                }
                else
                {
                    TextEditorSynchronousInput.s_result = true;
                }
            }
        }
        
        #endregion Private types

        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _customTB = new CustomTB();            
            MainWindow.Content = _customTB;

            QueueDelegate(DoFocus);
        }        

        private void DoFocus()
        {
            _customTB.Focus();
            QueueDelegate(DoType);
        }

        private void DoType()
        {
            KeyboardInput.TypeString(_typeString);
            QueueDelegate(VerifySynchronousInput);
        }

        private void VerifySynchronousInput()
        {
            Verifier.Verify(s_result, "Verifying synchronous input on TextBox", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow
    }
}
