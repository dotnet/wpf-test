// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data to be used when manipulating text selection in tests.


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 4 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Common/Library/Input/TestKeyboardInputProvider.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Media; 
    using System.Windows.Threading;

    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Interesting values for a TextSelection dimension.
    /// </summary>
    public enum TextSelectionTestValue
    {
        /// <summary>Empty selection on empty text.</summary>
        EmptyOnEmptyText,
        /// <summary>Empty selection on populated text.</summary>
        EmptyOnPopulatedText,
        /// <summary>Selection spanning all text.</summary>
        SpanAllText,
        /// <summary>Selection spanning a fragment.</summary>
        SpanTextFragment,
        /// <summary>Selection spanning a delimited line.</summary>
        SpanDelimitedLine,
        /// <summary>Selection spanning a rendering line.</summary>
        SpanRenderedLine,
        /// <summary>Selection spanning a single newline.</summary>
        SpanNewline,
        /// <summary>Selection spanning multiple empty lines.</summary>
        SpanMultipleEmptyLines,
        /// <summary>Selection spanning multiple populated lines.</summary>
        SpanMultiplePopulatedLines,
        /// <summary>Empty selection after a delimited line.</summary>
        EmptySelectionAfterDelimitedLine,
        /// <summary>Empty selection after a wrapped line.</summary>
        EmptySelectionAfterWrapLine,
        /// <summary>Empty selection at end-of-line before a delimiter.</summary>
        EmptySelectionBeforeDelimitedLine,
        /// <summary>Empty selection at end-of-line before a wrapped line.</summary>
        EmptySelectionBeforeWrapLine,
        /// <summary>Empty selection at the start of the document.</summary>
        EmptyDocumentStart,
        /// <summary>Empty selection at the end of the document.</summary>
        EmptyDocumentEnd,
        /// <summary>Empty selection at the end of a visible viewport for scrolling.</summary>
        EmptyBeforeViewportEnd,
        /// <summary>Empty selection at the start of a visible viewport for scrolling.</summary>
        EmptyAfterViewportStart,
        /// <summary>Empty selection at the end of a table row.</summary>
        EmptyAtTableRowEnd,
    }

    /// <summary>
    /// Provides information about interesting text selection
    /// cases.
    /// </summary>
    public sealed class TextSelectionData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private TextSelectionData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Finds the TextSelectionData with the specified value.</summary>
        /// <param name='value'>Test value to look for.</param>
        /// <returns>
        /// The TextSelectionData instance for the specified value. If
        /// none is found, an exception is thrown.
        /// </returns>
        public static TextSelectionData GetForValue(TextSelectionTestValue value)
        {
            foreach(TextSelectionData result in Values)
            {
                if (result.TestValue == value)
                {
                    return result;
                }
            }
            throw new InvalidOperationException("Unable to find TextSelectionData for " + value);
        }

        /// <summary>
        /// Prepares the specified element to accept the selection.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to prepare.</param>
        public void PrepareForSelection(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            switch (TestValue)
            {
                case TextSelectionTestValue.EmptyOnEmptyText:
                    wrapper.Text = "";
                    break;
                case TextSelectionTestValue.EmptyOnPopulatedText:
                    wrapper.Text = "text content";
                    break;
                case TextSelectionTestValue.SpanTextFragment:
                    wrapper.Text = "123";
                    break;
                case TextSelectionTestValue.SpanRenderedLine:
                case TextSelectionTestValue.EmptySelectionAfterWrapLine:
                case TextSelectionTestValue.EmptySelectionBeforeWrapLine:
                    wrapper.Element.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
                    wrapper.Text = StringData.WrappingLine.Value;
                    break;
                case TextSelectionTestValue.SpanMultipleEmptyLines:
                    wrapper.Text = "\r\n\r\n\r\n";
                    break;
                case TextSelectionTestValue.SpanDelimitedLine:
                case TextSelectionTestValue.SpanNewline:
                case TextSelectionTestValue.SpanMultiplePopulatedLines:
                case TextSelectionTestValue.EmptySelectionAfterDelimitedLine:
                case TextSelectionTestValue.EmptySelectionBeforeDelimitedLine:
                    wrapper.Text = "abc\r\ndef";
                    break;
                case TextSelectionTestValue.EmptyBeforeViewportEnd:
                    wrapper.Element.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
                    wrapper.Element.SetValue(TextBox.FontSizeProperty, (double)72);
                    wrapper.Text = StringData.WrappingLine.Value;
                    GetForValue(TextSelectionTestValue.EmptyDocumentStart).Select(wrapper);
                    break;
                case TextSelectionTestValue.EmptyAfterViewportStart:
                    wrapper.Element.SetValue(TextBox.FontSizeProperty, (double)42);
                    wrapper.Text = TextUtils.RepeatString("sample line content\r\n", 64);
                    GetForValue(TextSelectionTestValue.EmptyDocumentEnd).Select(wrapper);
                    break;
                case TextSelectionTestValue.EmptyAtTableRowEnd:
                    if (wrapper.IsElementRichText)
                    {
                        TextPointer pointer;

                        pointer = wrapper.Start;
                        if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart &&
                            pointer.GetAdjacentElement(LogicalDirection.Forward) is FlowDocument)
                        {
                            FlowDocument fd = pointer.GetAdjacentElement(LogicalDirection.Forward) as FlowDocument;
                            fd.Blocks.Add(CreateTable(3, 3));
                            //pointer = pointer.GetPositionAtOffset(1);
                        }
                        
                        //pointerInsertTextElement(CreateTable(3, 3));
                    }
                    break;
            }
            // Nothing to do with the following values - they always work:
            // SpanAllText, EmptyDocumentStart, EmptyDocumentEnd
        }

        /// <summary>
        /// Sets the selection on the specified element.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to select.</param>
        /// <returns>
        /// true if the selection was made, false if something in the element
        /// would need to be changed by PrepareForSelection for the
        /// selection to be valid.
        /// </returns>
        public bool Select(UIElementWrapper wrapper)
        {
            TextPointer rangeStart;
            TextPointer rangeEnd;

            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            switch (TestValue)
            {
                case TextSelectionTestValue.EmptyOnEmptyText:
                    if (wrapper.Text.Length > 0)
                    {
                        return false;
                    }
                    wrapper.SelectionStart = 0;
                    return true;

                case TextSelectionTestValue.EmptyOnPopulatedText:
                    if (wrapper.Text.Length == 0)
                    {
                        return false;
                    }
                    wrapper.SelectionStart = 0;
                    return true;

                case TextSelectionTestValue.SpanAllText:
                    wrapper.SelectAll();
                    return true;

                case TextSelectionTestValue.SpanTextFragment:
                    if (wrapper.Text.Length <= 2)
                    {
                        return false;
                    }

                    if (wrapper.IsPointerAllowedOnThis)
                    {
                        rangeStart = wrapper.Start;
                        rangeEnd = wrapper.Start;
                        rangeStart = rangeStart.GetPositionAtOffset(1);
                        rangeEnd = rangeEnd.GetPositionAtOffset(2);

                        wrapper.SelectionInstance.Select(rangeStart, rangeEnd);
                    }
                    else
                    {
                        wrapper.Select(1, 2);
                    }
                    return true;

                case TextSelectionTestValue.SpanDelimitedLine:
                    if (wrapper.IsElementRichText)
                    {
                        return SpanDelimitedLineRich(wrapper);
                    }
                    else
                    {
                        return SpanDelimitedLinePlain(wrapper);
                    }

                case TextSelectionTestValue.SpanRenderedLine:
                    // Look for a line that does not end because of a \r\n
                    // sequence or because it's the end of the text.
                    TextRange lineRange;
                    int lineIndex;

                    if (!wrapper.IsPointerAllowedOnThis)
                    {
                        TextBox textbox;
                        int firstLineIndex;
                        int lastLineIndex;

                        // Line API only supported on TextBox or subclasses.
                        textbox = wrapper.Element as TextBox;
                        if (textbox == null)
                        {
                            return false;
                        }

                        // Look for a visible line that is not \r\n delimited.
                        firstLineIndex = textbox.GetFirstVisibleLineIndex();
                        lastLineIndex = textbox.GetLastVisibleLineIndex();
                        for (int i = firstLineIndex; i <= lastLineIndex; i++)
                        {
                            if (!textbox.GetLineText(i).EndsWith("\r\n"))
                            {
                                // If this is the last line, there is no
                                // wrapping line in this control.
                                int startIndex;
                                int length;

                                startIndex = textbox.GetCharacterIndexFromLineIndex(i);
                                length = textbox.GetLineLength(i);
                                if (startIndex + length == textbox.Text.Length)
                                {
                                    return false;
                                }
                                else
                                {
                                    textbox.Select(startIndex, length);
                                    return true;
                                }
                            }
                        }

                        // No visible lines found that meet the criteria.
                        return false;
                    }

                    lineIndex = 0;
                    lineRange = null;
                    while (lineIndex < 1024)
                    {
                        lineRange = wrapper.GetLineRange(lineIndex);
                        if (lineRange.Text.EndsWith("\r\n"))
                        {
                            lineIndex++;
                            continue;
                        }
                        if (lineRange.End.GetPointerContext(LogicalDirection.Forward) ==
                            TextPointerContext.None)
                        {
                            return false;
                        }
                        // We found a real wrapped line.
                        break;
                    }
                    if (lineRange == null)
                    {
                        return false;
                    }
                    wrapper.SelectionInstance.Select(lineRange.Start, lineRange.End);
                    return true;

                case TextSelectionTestValue.SpanNewline:
                    TextRange rangeWithDelimiter;

                    // Unlike SpanDelimitedLine, we only select the newline itself.
                    if (!wrapper.IsPointerAllowedOnThis)
                    {
                        return false;
                    }

                    rangeWithDelimiter = TextUtils.FindTextRangeWithText(
                        wrapper.Start, "\r\n");
                    if (rangeWithDelimiter == null)
                    {
                        return false;
                    }
                    wrapper.SelectionInstance.Select(
                        rangeWithDelimiter.Start, rangeWithDelimiter.End);
                    return true;

                case TextSelectionTestValue.SpanMultipleEmptyLines:
                    if (!wrapper.IsPointerAllowedOnThis)
                    {
                        return false;
                    }

                    rangeWithDelimiter = TextUtils.FindTextRangeWithText(
                        wrapper.Start, "\r\n\r\n");
                    if (rangeWithDelimiter == null)
                    {
                        return false;
                    }
                    wrapper.SelectionInstance.Select(
                        rangeWithDelimiter.Start, rangeWithDelimiter.End);
                    return true;

                case TextSelectionTestValue.SpanMultiplePopulatedLines:
                    TextPointer multipleLineStart;
                    TextPointer firstLineEnd;
                    TextPointer multipleLineEnd;

                    if (wrapper.Element is PasswordBox)
                    {
                        return false;
                    }

                    if (wrapper.Element is TextBox)
                    {
                        TextBox textbox;
                        int firstLineIndex;
                        int lastLineIndex;

                        // Line API only supported on TextBox or subclasses.
                        textbox = wrapper.Element as TextBox;
                        if (textbox == null)
                        {
                            return false;
                        }

                        // Look for visible lines that are not empty.
                        firstLineIndex = textbox.GetFirstVisibleLineIndex();
                        lastLineIndex = textbox.GetLastVisibleLineIndex();
                        for (int i = firstLineIndex; i < lastLineIndex; i++)
                        {
                            // Find two consecutive lines with more than \r\n.
                            if (textbox.GetLineText(i).Length > 2 &&
                                textbox.GetLineText(i + 1).Length > 2)
                            {
                                int startIndex; // Index of selection start.
                                int lastLine;   // Index of last line we select.
                                int endIndex;   // Index of selection end.

                                // Get as many lines as possible.
                                lastLine = i + 2;
                                while (lastLine < lastLineIndex &&
                                    textbox.GetLineText(lastLine).Length > 2)
                                {
                                    lastLine++;
                                }
                                lastLine--;

                                startIndex = textbox.GetCharacterIndexFromLineIndex(i);
                                endIndex = textbox.GetCharacterIndexFromLineIndex(lastLine);
                                endIndex += textbox.GetLineLength(lastLine);
                                textbox.Select(startIndex, endIndex - startIndex);
                                return true;
                            }
                        }

                        // No visible lines found that meet the criteria.
                        return false;
                    }

                    rangeWithDelimiter = TextUtils.FindTextRangeWithText(
                        wrapper.Start, "\r\n");
                    if (rangeWithDelimiter == null)
                    {
                        return false;
                    }
                    multipleLineStart = rangeWithDelimiter.Start;
                    firstLineEnd = rangeWithDelimiter.End;

                    rangeWithDelimiter = TextUtils.FindTextRangeWithText(
                        firstLineEnd, "\r\n");
                    if (rangeWithDelimiter == null)
                    {
                        if (firstLineEnd.CompareTo(wrapper.End) != 0)
                        {
                            multipleLineEnd = wrapper.End;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        multipleLineEnd = rangeWithDelimiter.End;
                    }
                    wrapper.SelectionInstance.Select(multipleLineStart, multipleLineEnd);
                    return true;
                case TextSelectionTestValue.EmptySelectionAfterDelimitedLine:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptySelectionAfterDelimitedLineRich(wrapper);
                    }
                    else
                    {
                        return EmptySelectionAfterDelimitedLinePlain(wrapper);
                    }
                case TextSelectionTestValue.EmptySelectionAfterWrapLine:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptySelectionAfterWrapLineRich(wrapper);
                    }
                    else
                    {
                        return EmptySelectionAfterWrapLinePlain(wrapper);
                    }
                case TextSelectionTestValue.EmptySelectionBeforeDelimitedLine:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptySelectionBeforeDelimitedLineRich(wrapper);
                    }
                    else
                    {
                        return EmptySelectionBeforeDelimitedLinePlain(wrapper);
                    }
                case TextSelectionTestValue.EmptySelectionBeforeWrapLine:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptySelectionBeforeWrapLineRich(wrapper);
                    }
                    else
                    {
                        return EmptySelectionBeforeWrapLinePlain(wrapper);
                    }
                case TextSelectionTestValue.EmptyDocumentStart:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptyDocumentStartRich(wrapper);
                    }
                    else
                    {
                        return EmptyDocumentStartPlain(wrapper);
                    }
                case TextSelectionTestValue.EmptyDocumentEnd:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptyDocumentEndRich(wrapper);
                    }
                    else
                    {
                        return EmptyDocumentEndPlain(wrapper);
                    }
                case TextSelectionTestValue.EmptyBeforeViewportEnd:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptyBeforeViewportEndRich(wrapper);
                    }
                    else
                    {
                        return EmptyBeforeViewportEndPlain(wrapper);
                    }
                case TextSelectionTestValue.EmptyAfterViewportStart:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptyAfterViewportStartRich(wrapper);
                    }
                    else
                    {
                        return EmptyAfterViewportStartPlain(wrapper);
                    }
                case TextSelectionTestValue.EmptyAtTableRowEnd:
                    if (wrapper.IsElementRichText)
                    {
                        return EmptyAtTableRowEndRich(wrapper);
                    }
                    break;
            }
            return false;
        }

        /// <summary>Returns a string representing the selection.</summary>
        /// <returns>A string representing the selection.</returns>
        public override string ToString()
        {
            return TestValue.ToString();
        }

        /// <summary>
        /// Verify the render of a selection
        /// </summary>
        /// <param name="wrapper">Element wrapper</param>
        /// <param name="highlightColorElement">Which part of the color to be verified for highlight</param>
        /// <param name="textColorElement">Which part of the color to be verified for highlighted Text</param>
        /// <param name="highlightMatchPercent">What percentage do you expected to be mached for Highlight</param>
        /// <param name="TextMatchPercent">What percentage do you expected to be mached for Highlighted Text</param>
        /// <returns></returns>
        public static void VerifySelectionRendering (Wrappers.UIElementWrapper wrapper, ColorElement highlightColorElement, ColorElement textColorElement,  int highlightMatchPercent, int TextMatchPercent)
        {
            if (null == wrapper)
            {
                throw new ArgumentNullException("wrapper");
            }
            if (wrapper.Element is TextBoxBase)
            {
                VerifyTextBoxSelectionRendering(wrapper, highlightColorElement, textColorElement, highlightMatchPercent, TextMatchPercent);
            }
            else
            {
                throw new Exception("We don't support password at this moment!");
            }
        }
     
        #endregion Public methods.


        #region Public properties.

        /// <summary>Value for text selection setting.</summary>
        public TextSelectionTestValue TestValue
        {
            get { return _testValue; }
        }

        /// <summary>Whether the selection is supported for rich text.</summary>
        public bool IsRichTextSupported
        {
            get { return _isRichTextSupported; }
        }

        /// <summary>Whether the selection is supported for plain text.</summary>
        public bool IsPlainTextSupported
        {
            get { return _isPlainTextSupported; }
        }

        /// <summary>Interesting values for testing text selection.</summary>
        public static TextSelectionData[] Values = new TextSelectionData[] {
            ForBoth(TextSelectionTestValue.EmptyOnEmptyText),
            ForBoth(TextSelectionTestValue.EmptyOnPopulatedText),
            ForBoth(TextSelectionTestValue.SpanAllText),
            ForBoth(TextSelectionTestValue.SpanTextFragment),
            ForBoth(TextSelectionTestValue.SpanDelimitedLine),
            ForBoth(TextSelectionTestValue.SpanRenderedLine),
            ForBoth(TextSelectionTestValue.SpanNewline),
            ForBoth(TextSelectionTestValue.SpanMultipleEmptyLines),
            ForBoth(TextSelectionTestValue.SpanMultiplePopulatedLines),
            ForBoth(TextSelectionTestValue.EmptySelectionAfterDelimitedLine),
            ForBoth(TextSelectionTestValue.EmptySelectionAfterWrapLine),
            ForBoth(TextSelectionTestValue.EmptySelectionBeforeDelimitedLine),
            ForBoth(TextSelectionTestValue.EmptySelectionBeforeWrapLine),
            ForBoth(TextSelectionTestValue.EmptyDocumentStart),
            ForBoth(TextSelectionTestValue.EmptyDocumentEnd),
            ForBoth(TextSelectionTestValue.EmptyBeforeViewportEnd),
            ForBoth(TextSelectionTestValue.EmptyAfterViewportStart),
        };

        #endregion Public properties.

        #region Internal methods.

        /// <summary>
        /// Checks whether the specified rendered line ends with a line
        /// delimiter.
        /// </summary>
        /// <param name='textbox'>Rendered TextBox to check.</param>
        /// <param name='lineIndex'>Index of line to check.</param>
        /// <returns>
        /// true if the specified rendered line ends with a
        /// line delimiter; false otherwise.
        /// </returns>
        internal static bool IsDelimitedLine(TextBox textbox, int lineIndex)
        {
            string lineText;

            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }

            lineText = textbox.GetLineText(lineIndex);
            return lineText.EndsWith("\r\n");
        }

        /// <summary>
        /// Checks whether the specified rendered line is the last one in the
        /// control.
        /// </summary>
        /// <param name='textbox'>Rendered TextBox to check.</param>
        /// <param name='lineIndex'>Index of line to check.</param>
        /// <returns>
        /// true if the specified rendered line is the last one in the control.
        /// </returns>
        internal static bool IsLastLine(TextBox textbox, int lineIndex)
        {
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }

            return lineIndex == textbox.LineCount - 1;
        }

        #endregion Internal methods.


        #region Private methods.

        /// <summary>
        /// Creates a table with the specified number of columns and rows.
        /// </summary>
        /// <param name="columnCount">Number of columns in table.</param>
        /// <param name="rowCount">Number of rows in table.</param>
        /// <returns>A new initialized Table instance.</returns>
        /// <remarks>
        /// All cells have an empty paragraph, and all rows are 
        /// added to a single row group.
        /// </remarks>
        private static Table CreateTable(int columnCount, int rowCount)
        {
            Table result;
            TableRowGroup rowGroup;

            rowGroup = new TableRowGroup();
            for (int i = 0; i < rowCount; i++)
            {
                TableRow row = new TableRow();
                rowGroup.Rows.Add(row);
                for (int j = 0; j < columnCount; j++)
                {
                    row.Cells.Add(new TableCell(new Paragraph()));
                }
            }

            result = new Table();
            result.RowGroups.Add(rowGroup);

            return result;
        }

        /// <summary>
        /// Creates a new TextSelectionTestValue for plain and
        /// rich text.
        /// </summary>
        private static TextSelectionData ForBoth(TextSelectionTestValue testValue)
        {
            TextSelectionData result;

            result = new TextSelectionData();
            result._testValue = testValue;
            result._isRichTextSupported = true;
            result._isPlainTextSupported = true;

            return result;
        }

        /// <summary>
        /// Creates a new TextSelectionTestValue for rich text.
        /// </summary>
        private static TextSelectionData ForRich(TextSelectionTestValue testValue)
        {
            TextSelectionData result;

            result = new TextSelectionData();
            result._testValue = testValue;
            result._isRichTextSupported = true;
            result._isPlainTextSupported = false;

            return result;
        }

        /// <summary>Place empty selection after the viewport start.</summary>
        private static bool EmptyAfterViewportStartPlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int firstLineIndex;
            int firstCharacterIndex;

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            // The viewport can scroll above the rendered content
            // when the first position of the first visible line
            // is not the start of the document.
            firstLineIndex = textbox.GetFirstVisibleLineIndex();
            firstCharacterIndex = textbox.GetCharacterIndexFromLineIndex(firstLineIndex);
            if (firstCharacterIndex > 0)
            {
                textbox.Select(firstCharacterIndex, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Place empty selection after the viewport start.</summary>
        private static bool EmptyAfterViewportStartRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Place empty selection at the end of a table row.</summary>
        private static bool EmptyAtTableRowEndRich(UIElementWrapper wrapper)
        {
            TextPointer cursor;

            cursor = wrapper.Start;
            do
            {
                if (cursor.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd &&
                    cursor.GetAdjacentElement(LogicalDirection.Backward) is TableRow)
                {
                    wrapper.SelectionInstance.Select(cursor, cursor);
                    return true;
                }
                cursor = cursor.GetPositionAtOffset(1);
            } while (cursor != null);
            return false;
        }

        /// <summary>Place empty selection before the viewport end.</summary>
        private static bool EmptyBeforeViewportEndPlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int lastLineIndex;
            int lastCharacterIndex;

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            // The viewport scroll beneath the rendered content
            // when the last position of the last visible line
            // is before the end of the text.
            lastLineIndex = textbox.GetLastVisibleLineIndex();
            lastCharacterIndex = textbox.GetCharacterIndexFromLineIndex(lastLineIndex) +
                textbox.GetLineLength(lastLineIndex);
            if (lastCharacterIndex < textbox.Text.Length)
            {
                textbox.Select(lastCharacterIndex, 0);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Place empty selection before the viewport end.</summary>
        private static bool EmptyBeforeViewportEndRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Place empty selection at document end.</summary>
        private static bool EmptyDocumentEndPlain(UIElementWrapper wrapper)
        {
            TextBox textBox;
            PasswordBox passwordBox;

            textBox = wrapper.Element as TextBox;
            if (textBox != null)
            {
                textBox.Select(textBox.Text.Length, 0);
                return true;
            }

            passwordBox = wrapper.Element as PasswordBox;
            if (passwordBox != null)
            {
                wrapper.Select(passwordBox.Password.Length, 0);
                return true;
            }

            return false;
        }

        /// <summary>Place empty selection at document end.</summary>
        private static bool EmptyDocumentEndRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Place empty selection at document start.</summary>
        private static bool EmptyDocumentStartPlain(UIElementWrapper wrapper)
        {
            TextBox textBox;
            PasswordBox passwordBox;

            textBox = wrapper.Element as TextBox;
            if (textBox != null)
            {
                textBox.Select(0, 0);
                return true;
            }

            passwordBox = wrapper.Element as PasswordBox;
            if (passwordBox != null)
            {
                wrapper.Select(0, 0);
                return true;
            }

            return false;
        }

        /// <summary>Place empty selection at document start.</summary>
        private static bool EmptyDocumentStartRich(UIElementWrapper wrapper)
        {
            wrapper.SelectionInstance.Select(wrapper.SelectionInstance.Start, wrapper.SelectionInstance.Start);
            return true;
        }

        /// <summary>Select an empty text after a delimiter is found.</summary>
        private static bool EmptySelectionAfterDelimitedLinePlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int newLineIndex;

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            newLineIndex = textbox.Text.IndexOf("\r\n");
            if (newLineIndex == -1)
            {
                return false;
            }
            textbox.Select(newLineIndex + 2, 0);

            return true;
        }

        /// <summary>Select an empty text after a delimiter is found.</summary>
        private static bool EmptySelectionAfterDelimitedLineRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Select an empty text after a wrapped line.</summary>
        private static bool EmptySelectionAfterWrapLinePlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int lineIndex;

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            lineIndex = FindFirstWrappingLine(textbox);
            if (lineIndex == -1)
            {
                return false;
            }
            else
            {
                int startIndex; // Index of selection start.

                startIndex = textbox.GetCharacterIndexFromLineIndex(lineIndex + 1);
                textbox.Select(startIndex, 0);
                return true;
            }
        }

        /// <summary>Select an empty text after a wrapped line.</summary>
        private static bool EmptySelectionAfterWrapLineRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Select an empty selection before a delimited line.</summary>
        private static bool EmptySelectionBeforeDelimitedLinePlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int newLineIndex;

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            newLineIndex = textbox.Text.IndexOf("\r\n");
            if (newLineIndex == -1)
            {
                return false;
            }
            textbox.Select(newLineIndex, 0);

            return true;
        }

        /// <summary>Select an empty selection before a delimited line.</summary>
        private static bool EmptySelectionBeforeDelimitedLineRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Select an empty selection before a wrapped line.</summary>
        private static bool EmptySelectionBeforeWrapLinePlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int lineIndex;
            int startIndex;

            // This can only be implemented by accessing the underlying
            // OM, as the selection direction is the only way to determine
            // whether the caret is at the end of a line that wraps or
            // at the begining of the following line.

            // Multiple plain lines make sense for TextBox, not PasswordBox.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            lineIndex = FindFirstWrappingLine(textbox);
            if (lineIndex == -1)
            {
                return false;
            }

            startIndex = textbox.GetCharacterIndexFromLineIndex(lineIndex);
            textbox.Select(startIndex + textbox.GetLineLength(lineIndex), 0);

            return true;
        }

        /// <summary>Select an empty selection before a wrapped line.</summary>
        private static bool EmptySelectionBeforeWrapLineRich(UIElementWrapper wrapper)
        {
            return false;
        }

        /// <summary>Finds the first rendered line that wraps.</summary>
        /// <param name='textbox'>TextBox to search.</param>
        /// <returns>
        /// The line index of the first rendered line that wraps;
        /// -1 if none is found.
        /// </returns>
        private static int FindFirstWrappingLine(TextBox textbox)
        {
            int firstLineIndex;
            int lastLineIndex;

            firstLineIndex = textbox.GetFirstVisibleLineIndex();
            lastLineIndex = textbox.GetLastVisibleLineIndex();
            for (int i = firstLineIndex; i <= lastLineIndex; i++)
            {
                if (!IsDelimitedLine(textbox, i) && !IsLastLine(textbox, i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>Select a line that ends with a delimiter (not including end-of-line).</summary>
        private static bool SpanDelimitedLinePlain(UIElementWrapper wrapper)
        {
            TextBox textbox;
            int firstLineIndex;
            int lastLineIndex;

            // Line API only supported on TextBox or subclasses.
            textbox = wrapper.Element as TextBox;
            if (textbox == null)
            {
                return false;
            }

            // Look for a visible line that is \r\n delimited.
            firstLineIndex = textbox.GetFirstVisibleLineIndex();
            lastLineIndex = textbox.GetLastVisibleLineIndex();
            for (int i = firstLineIndex; i <= lastLineIndex; i++)
            {
                if (textbox.GetLineText(i).EndsWith("\r\n"))
                {
                    textbox.Select(textbox.GetCharacterIndexFromLineIndex(i), textbox.GetLineLength(i));
                    return true;
                }
            }

            // No delimited lines are visible.
            return false;
        }

        /// <summary>Select a line that ends with a delimiter.</summary>
        private static bool SpanDelimitedLineRich(UIElementWrapper wrapper)
        {
            TextRange rangeWithDelimiter;

            rangeWithDelimiter = TextUtils.FindTextRangeWithText(wrapper.Start, "\r\n");
            if (rangeWithDelimiter == null)
            {
                return false;
            }
            wrapper.SelectionInstance.Select(wrapper.Start, rangeWithDelimiter.End);
            return true;
        }

        /// <summary>
        /// Verify TextSelection rendering in TextBox
        /// </summary>
        /// <param name="wrapper">Element Wrapper</param>
        /// <param name="hightlightColorelement">Which color element are you want to verify for Highlight</param>
        /// <param name="textColorElement">Which color element are you want to verify for Highlighted Text</param>
        /// <param name="hilightMatchPercent">Expected match percentage Highlight</param>
        /// <param name="textMatchPercent">Expected match percentage for Highlighted Text</param>
        /// <returns></returns>
        private static void VerifyTextBoxSelectionRendering(UIElementWrapper wrapper, ColorElement hightlightColorelement, ColorElement textColorElement, int hilightMatchPercent, int textMatchPercent)
        {
            TextPointer start;
            TextPointer end;
            Rect rect;
            int totalPixels = 0 ;
            int hilightMatchedPixels = 0;
            int textMatchedPixels = 0;
            System.Drawing.Bitmap selectedBitmap;
            System.Drawing.Bitmap controlBitmap;

            controlBitmap = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(wrapper.Element);
            start = wrapper.SelectionInstance.Start;
            end = wrapper.SelectionInstance.End;
            
            //When Start is the last insertion point at the end of a document, GetNextInsertionPosition() return null value. 
            //So we will jump out of the loop.
            while (start != null && start.GetOffsetToPosition(end) > 0)
            {
                rect = wrapper.GetElementRelativeCharacterRect(start, 0, LogicalDirection.Forward);
            
                //if rect.Width == 0, it is at the end of a line and we selected a new line. We pick 5 pixels only here.
                //If no paragraph in the document, we should not have anything to be selected.
                if (rect.Width == 0)
                    rect.Width = 5;
                
                //create the bitmap for selected area.
                selectedBitmap = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(controlBitmap, rect);
                
                //count the pixels for matched colors.
                hilightMatchedPixels += Microsoft.Test.Imaging.BitmapUtils.CountColoredPixels(selectedBitmap, SystemColors.HighlightColor, hightlightColorelement);
                textMatchedPixels += Microsoft.Test.Imaging.BitmapUtils.CountColoredPixels(selectedBitmap, SystemColors.HighlightTextColor, textColorElement);
                
                //calculate the total pixels.
                totalPixels += selectedBitmap.Size.Height * selectedBitmap.Size.Width;
               
                //Advance the start textpointer to next insertion point
                start = start.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            //When ther The selection is empty. Total Pixels is 0. There is no highlight and we return true(the default value).
            if (totalPixels != 0)
            {
                Verifier.Verify((hilightMatchedPixels * 100) / totalPixels >= hilightMatchPercent, 
                    "Highlight match percentage failed. Hilight matched Pixels[" + hilightMatchedPixels + "]. Total pixels[" + totalPixels + "]");

                Verifier.Verify ((textMatchedPixels * 100) / totalPixels >= textMatchPercent,
                    "Highlighted text match percentage failed. Highlight text matched Pixels[" + textMatchedPixels + "]. Total pixels[" + totalPixels + "]");
            }
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Value for text selection setting.</summary>
        private TextSelectionTestValue _testValue;

        /// <summary>Value for text selection setting.</summary>
        private bool _isRichTextSupported;

        /// <summary>Value for text selection setting.</summary>
        private bool _isPlainTextSupported;

        #endregion Private fields.
    }
}
