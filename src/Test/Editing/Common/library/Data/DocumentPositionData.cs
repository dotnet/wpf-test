// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about interesting document positions.

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
    using System.Windows.Threading;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Interesting values for a document position.
    /// </summary>
    public enum DocumentPosition
    {
        /// <summary>The first caret position in the document.</summary>
        StartOfDocument,
        /// <summary>The last caret position in the document.</summary>
        EndOfDocument,
        /// <summary>The first caret position on a \r\n-delimited line or Paragraph.</summary>
        StartOfDelimitedLine,
        /// <summary>The last caret position on a \r\n-delimited line or Paragraph.</summary>
        EndOfDelimitedLine,
        /// <summary>A position between StartOfDelimitedLine and EndOfDelimitedLine.</summary>
        InsideDelimitedLine,
        /// <summary>A caret position at the start of a word.</summary>
        StartOfWord,
        /// <summary>A caret position at the end of a word.</summary>
        EndOfWord,
        /// <summary>A caret position within a word.</summary>
        InsideWord,
        /// <summary>A caret position at the start of a whitespace run.</summary>
        StartOfWhitespace,
        /// <summary>A caret position within a whitespace run.</summary>
        InsideWhitespace,
        /// <summary>A caret position at the end of a whitespace run.</summary>
        EndOfWhitespace,
        /// <summary>The first caret position on a visually-wrapped line.</summary>
        StartOfWrappedLine,
        /// <summary>The last caret position on a visually-wrapped line.</summary>
        EndOfWrappedLine,        
        /// <summary>The first caret position in the Table cell</summary>
        StartOfCell,
        /// <summary>A caret position at the end of a Table cell</summary>
        EndOfCell,
        /// <summary>A caret position at the middle cell of table</summary>
        CrossCell,
    }

    /// <summary>
    /// Provides information about interesting document positions.
    /// </summary>
    public sealed class DocumentPositionData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private DocumentPositionData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Finds the DocumentPositionData with the specified value.</summary>
        /// <param name='value'>Test value to look for.</param>
        /// <returns>
        /// The DocumentPositionData instance for the specified value. If
        /// none is found, an exception is thrown.
        /// </returns>
        public static DocumentPositionData GetForValue(DocumentPosition value)
        {
            foreach (DocumentPositionData result in Values)
            {
                if (result.DocumentPosition == value)
                {
                    return result;
                }
            }
            throw new InvalidOperationException("Unable to find DocumentPositionData for " + value);
        }

        /// <summary>Finds a caret position for this position after the specified pointer.</summary>
        /// <param name="pointer">Pointer at which to start search.</param>
        /// <returns>A caret position for this position after the given element, null if none are available.</returns>
        public TextPointer FindAfter(TextPointer pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }
            return Find(pointer, LogicalDirection.Forward, false);
        }

        /// <summary>Finds a caret position for this position in the wrapped element.</summary>
        /// <param name="wrapper">Wrapped element to find position in.</param>
        /// <returns>a caret position for this position in the wrapped element, null if none are available.</returns>
        public TextPointer FindAny(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            return Find(wrapper.Start, LogicalDirection.Forward, true);
        }

        /// <summary>Finds a caret position for this position before the specified pointer.</summary>
        /// <param name="pointer">Pointer at which to start search.</param>
        /// <returns>A caret position for this position before the given element, null if none are available.</returns>
        public TextPointer FindBefore(TextPointer pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }
            return Find(pointer, LogicalDirection.Backward, false);
        }

        /// <summary>Returns a string representing the selection.</summary>
        /// <returns>A string representing the selection.</returns>
        public override string ToString()
        {
            return DocumentPosition.ToString();
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Value for the document position.</summary>
        public DocumentPosition DocumentPosition
        {
            get { return _documentPosition; }
        }

        /// <summary>Whether the position is supported for plain text.</summary>
        public bool IsPlainTextSupported
        {
            get { return _isPlainTextSupported; }
        }

        /// <summary>Interesting values for testing document positions.</summary>
        public static DocumentPositionData[] Values = new DocumentPositionData[] {
            ForBoth(DocumentPosition.StartOfDocument),
            ForBoth(DocumentPosition.EndOfDocument),
            ForBoth(DocumentPosition.StartOfDelimitedLine),
            ForBoth(DocumentPosition.EndOfDelimitedLine),
            ForBoth(DocumentPosition.InsideDelimitedLine),
            ForBoth(DocumentPosition.StartOfWord),
            ForBoth(DocumentPosition.EndOfWord),
            ForBoth(DocumentPosition.InsideWord),
            ForBoth(DocumentPosition.StartOfWhitespace),
            ForBoth(DocumentPosition.InsideWhitespace),
            ForBoth(DocumentPosition.EndOfWhitespace),
            ForBoth(DocumentPosition.StartOfWrappedLine),
            ForBoth(DocumentPosition.EndOfWrappedLine),
            ForBoth(DocumentPosition.StartOfCell),
            ForBoth(DocumentPosition.EndOfCell),
            ForBoth(DocumentPosition.CrossCell),
        };

        #endregion Public properties.


        #region Internal methods.

        #endregion Internal methods.


        #region Private methods.

        /// <summary>
        /// Creates a non-frozen text pointer based on pointer, possibly offset
        /// in the given direction.
        /// </summary>
        private TextPointer CreateRelativeCursor(TextPointer pointer, LogicalDirection direction, bool stayInPlace)
        {
            TextPointer result;
                
            result = pointer;
            if (!stayInPlace)
            {
                // Return if the position is already valid
                if(result.GetPointerContext(direction) == TextPointerContext.None)
                    return result;

                result = result.GetPositionAtOffset((direction == LogicalDirection.Forward) ? 1 : -1);
            }
            return result;
        }

        private TextPointer Find(TextPointer pointer, LogicalDirection direction, bool evaluatePointer)
        {
            int directionUnit;
            bool isPlainText;

            System.Diagnostics.Debug.Assert(pointer != null);

            directionUnit = (direction == LogicalDirection.Forward) ? 1 : -1;
            isPlainText = pointer.DocumentStart.Parent is TextBox;

            switch (DocumentPosition)
            {
                case DocumentPosition.EndOfDelimitedLine:
                    return FindEndOfDelimitedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.EndOfDocument:
                    return FindEndOfDocument(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.EndOfWhitespace:
                    return FindEndOfWhitespace(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.EndOfWord:
                    return FindEndOfWord(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.EndOfWrappedLine:
                    return FindEndOfWrappedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.InsideDelimitedLine:
                    return FindInsideDelimitedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.InsideWhitespace:
                    return FindInsideWhitespace(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.InsideWord:
                    return FindInsideWord(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfDelimitedLine:
                    return FindStartOfDelimitedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfDocument:
                    return FindStartOfDocument(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfWhitespace:
                    return FindStartOfWhitespace(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfWord:
                    return FindStartOfWord(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfWrappedLine:
                    return FindStartOfWrappedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.StartOfCell:
                    return FindStartOfCell(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.EndOfCell:
                    return FindEndOfCell(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                case DocumentPosition.CrossCell:
                    return FindCrossCell(pointer, direction, evaluatePointer, directionUnit, isPlainText);

                default:
                    return null;
            }
        }

        /// <summary>The last caret position on a \r\n-delimited line or Paragraph.</summary>
        private TextPointer FindEndOfDelimitedLine(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;

            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);
            if (isPlainText)
            {
                // For plain text, we look for a position preceding \r\n or empty
                while (!(cursor.GetTextInRun(LogicalDirection.Forward).StartsWith("\r\n") ||
                    cursor.GetTextInRun(LogicalDirection.Forward) == ""))
                {
                    if ( (cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                    {
                        break;
                    }
                }
                return cursor;
            }
            else
            {
                // For rich text, we look for a position after Run, Paragraph or LineBreak
                bool isAtEnd;
                do
                {
                    // Look for the Paragraph end tag
                    isAtEnd = cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                        cursor.GetAdjacentElement(LogicalDirection.Forward) is Paragraph;

                    // Look for LineBreak end tag (LineBreak has open and end)
                    isAtEnd = isAtEnd ||
                        (cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                        cursor.GetAdjacentElement(LogicalDirection.Forward) is LineBreak);                    
                    
                    // Stop moving if we found the position we look for or if we can't move any further.
                    if (isAtEnd)
                    {
                        if(cursor.GetAdjacentElement(LogicalDirection.Forward) is LineBreak)
                            cursor = cursor.GetPositionAtOffset(-2);
                        else
                            cursor = cursor.GetPositionAtOffset(-1);
                        return cursor;
                    }
                    else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                    {
                        break;
                    }
                } while (true);

                return null;
            }
        }

        /// <summary>The last caret position in the document.</summary>
        private TextPointer FindEndOfDocument(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;

            // This is not a relative thing, so just look for the real end of the document
            // and figure out whether the pointer is after that.
            if (isPlainText)
            {
                cursor = pointer.DocumentEnd;
            }
            else
            {
                cursor = FindParagraph(pointer.DocumentEnd, LogicalDirection.Backward);
                if (cursor != null)
                {
                    cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);
                }
            }
            if (cursor == null) return null;
            if (evaluatePointer && cursor.CompareTo(pointer) == 0) return cursor;
            return (IsPointerInDirection(cursor, direction, pointer)) ? cursor : null;
        }

        /// <summary>A caret position at the end of a whitespace run.</summary>
        private TextPointer FindEndOfWhitespace(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            bool isAtEnd;
            TextPointer cursor;
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);

            // Walk caret forward. look forward for non-whitespace and look backward for a white space
            do
            {
                isAtEnd = cursor.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None &&
                    !cursor.GetTextInRun(LogicalDirection.Forward).StartsWith(" ") && 
                    cursor.GetTextInRun(LogicalDirection.Backward).EndsWith(" ");
                if(isAtEnd)
                {
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                {
                    break;
                }
            } while (true);
            return null;
        }

        /// <summary>A caret position at the end of a word.</summary>
        private TextPointer FindEndOfWord(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            bool isAtEnd;
            TextPointer cursor;
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);

            // Walk forward until hitting a whitespace or \r\n (paragraph or linebreak)
            // For paragraph and linebreak case make sure there is no precedding whitespace.
            do
            {
                // TextPointerContext.None is the adjacent to the beginning or end of content
                isAtEnd = cursor.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None &&
                    cursor.GetTextInRun(LogicalDirection.Forward).StartsWith(" ");

                if (isPlainText)
                {
                    // if text doesn't have a whitespace but end of line (\r\n)
                    isAtEnd = isAtEnd ||
                        (cursor.GetTextInRun(LogicalDirection.Forward).StartsWith("\r\n") &&
                        !cursor.GetTextInRun(LogicalDirection.Backward).EndsWith(" "));

                    // or there is no text at the end
                    isAtEnd = isAtEnd || cursor.GetTextInRun(LogicalDirection.Forward) == "";
                }
                else
                {
                    // if text doesn't have a whitespace but paragraph
                    isAtEnd = isAtEnd ||
                        (cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                        cursor.GetAdjacentElement(LogicalDirection.Forward) is Paragraph &&
                        !cursor.GetTextInRun(LogicalDirection.Backward).EndsWith(" "));

                    // if text doesn't have a whitespace but linebreak;
                    isAtEnd = isAtEnd ||
                        (cursor.GetAdjacentElement(LogicalDirection.Forward) is LineBreak &&
                        !cursor.GetTextInRun(LogicalDirection.Backward).EndsWith(" "));
                }

                if (isAtEnd)
                {
                    if (cursor.GetAdjacentElement(LogicalDirection.Forward) is LineBreak)
                        cursor = cursor.GetPositionAtOffset(-2);
                    else
                        cursor = cursor.GetPositionAtOffset(-1);
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                        break;

            } while (true);
            return null;
        }

        /// <summary>The last caret position on a visually-wrapped line.</summary>
        private TextPointer FindEndOfWrappedLine(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;
            Rect beforeRect;
            Rect afterRect;
            bool found;

            // We consider the pointer at the end of a wrapped line if the right of the character
            // looking forward is less than the right of the character looking backward when we
            // are in the middle of a text / embedded object run.
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);

            // Move cursor until it IsAtInsertionPosition
            if (!cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetNextInsertionPosition(direction);
            }

            if (cursor == null) return null;

            // Set cursor direction to Backward
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);
            do
            {
                // We stop looking for candidate positions when we no longer have a valid layout.
                if (!pointer.HasValidLayout)
                {
                    break;
                }

                beforeRect = cursor.GetCharacterRect(LogicalDirection.Backward);
                afterRect = cursor.GetCharacterRect(LogicalDirection.Forward);
                found = afterRect.Top > beforeRect.Top && IsWithinInlineRun(cursor);

                // found true only if wrapped line is not a \r\n
                // For a TextPointer between \r and \n, you get different Rect.Top's. Below conditions 
                // handle that case also.
                found = found &&                    
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\r")) &&
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\n")) &&
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\r\n"));

                if (found)
                {
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                    break;

            } while (true);

            return null;
        }

        /// <summary>A position between StartOfDelimitedLine and EndOfDelimitedLine.</summary>
        private TextPointer FindInsideDelimitedLine(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer start;
            TextPointer end;

            // Find start of delimited line
            start = FindStartOfDelimitedLine(pointer, direction, evaluatePointer, directionUnit, isPlainText);
            if (start == null) return null;

            // Find end of delimited line
            end = FindEndOfDelimitedLine(start, LogicalDirection.Forward, evaluatePointer, directionUnit, isPlainText);
                        
            // If start and end are the same position, return end edge
            // If distance from start to end is 1, return end edge
            if (start.GetOffsetToPosition(end) == 0 || start.GetOffsetToPosition(end) == 1)
                return end;
            
            // Then find the middle point of start and end
            start = start.GetPositionAtOffset(start.GetOffsetToPosition(end) / 2);

            return start;
        }

        /// <summary>A caret position within a whitespace run.</summary>
        private TextPointer FindInsideWhitespace(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            bool found;
            TextPointer cursor;
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);

            // Move cursor until it IsAtInsertionPosition
            if (!cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetNextInsertionPosition(direction);
            }

            if (cursor == null) return null;

            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Backward);
            // Walk forward until we are in the middle of a whitespace.
            do
            {
                found = cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text &&
                    cursor.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text;

                found = found &&
                    StartsWithWhitespace(cursor.GetTextInRun(LogicalDirection.Forward));

                found = found &&
                    EndsWithWhitespace(cursor.GetTextInRun(LogicalDirection.Backward));

                if (found)
                {
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                {
                    break;
                }

            } while (cursor != null);
            return cursor;
        }

        /// <summary>A caret position within a word.</summary>
        private TextPointer FindInsideWord (TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer start;
            TextPointer end;
            // Find start of a word
            start = FindStartOfWord(pointer, direction, evaluatePointer, directionUnit, isPlainText);

            if (start == null) return null;

            // Find end of a word
            end = FindEndOfWord(start, LogicalDirection.Forward, evaluatePointer, directionUnit, isPlainText);

            // If start and end are the same position, return end edge
            // If distance from start to end is 1, return end edge
            if (start.GetOffsetToPosition(end) == 0 || start.GetOffsetToPosition(end) == 1)
                return end;

            // Then find the middle point of start and end
            start = start.GetPositionAtOffset(start.GetOffsetToPosition(end) / 2);

            return start;
        }

        /// <summary>The first caret position on a \r\n-delimited line or Paragraph.</summary>
        private TextPointer FindStartOfDelimitedLine(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;

            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);
            if (!cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetNextInsertionPosition(direction);
            }
            if (cursor == null) return null;
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Forward);

            if (isPlainText)
            {
                // For plain text, we look for a position with preceding \r\n.
                while (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\r\n"))
                {
                    if ( (cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                    {
                        // If we are moving backward, the start of the document is a 'hard' delimiter.
                        return (direction == LogicalDirection.Backward) ? cursor : null;
                    }
                }
                return cursor;
            }
            else
            {
                // For rich text, we look for a position with preceding Paragraph or LineBreak
                bool isAtStart;
                do
                {
                    // Look for the Paragraph start tag first
                    isAtStart = cursor.GetPointerContext(direction) == TextPointerContext.ElementStart &&
                        cursor.GetAdjacentElement(direction) is Paragraph;

                    // Look for a preceding LineBreak element
                    // the cursor should be after the LineBreak
                    isAtStart = isAtStart ||
                        (cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                        cursor.GetAdjacentElement(LogicalDirection.Forward) is LineBreak);

                    // Stop moving if we found the position we look for or if we can't move any further.
                    if (isAtStart)
                    {
                        if (cursor.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cursor = cursor.GetPositionAtOffset(1);
                        else
                            cursor = cursor.GetPositionAtOffset(2);

                        return cursor;
                    }
                    else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                    {
                        break;
                    }
                } while (cursor != null);

                return cursor;
            }
        }

        /// <summary>The first caret position in the document.</summary>
        private TextPointer FindStartOfDocument(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;

            // This is not a relative thing, so just look for the real start of the document
            // and figure out whether the pointer is before that.
            if (isPlainText)
            {
                cursor = pointer.DocumentStart;
            }
            else
            {
                cursor = FindParagraph(pointer.DocumentStart, LogicalDirection.Forward);
                if (cursor != null)
                {
                    // Set direction Forward
                    cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Forward);
                }
            }

            if (cursor == null) return null;
            if (evaluatePointer && cursor.CompareTo(pointer) == 0) return cursor;
            return (IsPointerInDirection(cursor, direction, pointer)) ?
                cursor : null;
        }

        /// <summary>A caret position at the start of a whitespace run.</summary>
        private TextPointer FindStartOfWhitespace(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            bool isAtStart;
            TextPointer cursor;
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);

            if (!cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetNextInsertionPosition(direction);
            }

            if (cursor == null) return null;
            
            // Walk caret forward and look forward for a whitespace
            // whitespace could be beginging of document
            // whitespace could be after embededobject
            do
            {
                isAtStart = cursor.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None &&
                    cursor.GetTextInRun(LogicalDirection.Forward).StartsWith(" ");

                if (isAtStart)
                {
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                {
                    break;
                }

            } while (cursor != null);
            return cursor;
        }

        /// <summary>A caret position at the start of a word.</summary>
        private TextPointer FindStartOfWord(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            bool isAtStart;
            TextPointer cursor;
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);

            // Move cursor until it IsAtInsertionPosition
            if (!cursor.IsAtInsertionPosition)
            {
                if (cursor.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                    cursor.GetAdjacentElement(direction) is Paragraph)
                {
                    direction = LogicalDirection.Backward;
                }

                cursor = cursor.GetNextInsertionPosition(direction);
            }
            
            do
            {
                // Walk caret and look backward for a white space and look forward for non-empty text or non-whitespace
                isAtStart = cursor.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None &&
                    cursor.GetTextInRun(LogicalDirection.Backward).EndsWith(" ");

                isAtStart = isAtStart && cursor.GetTextInRun(LogicalDirection.Forward) != "";
                isAtStart = isAtStart && !cursor.GetTextInRun(LogicalDirection.Forward).StartsWith(" ");

                if (isAtStart)
                {
                    // Point direction forward
                    cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Forward);
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                {
                    break;
                }

            } while (cursor != null);
            return cursor;
        }

        /// <summary>The first caret position on a visually-wrapped line.</summary>
        private TextPointer FindStartOfWrappedLine(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor;
            Rect beforeRect;
            Rect afterRect;
            bool found;

            // We consider the pointer the start of a wrapped line if the Top of the character
            // looking forward is greater than the top of the character looking backward when we
            // are in the middle of a text / embedded object run.
            cursor = CreateRelativeCursor(pointer, direction, evaluatePointer);
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Forward);
            do
            {
                // We stop looking for candidate positions when we no longer have a valid layout.
                if (!pointer.HasValidLayout)
                {
                    break;
                }

                beforeRect = cursor.GetCharacterRect(LogicalDirection.Backward);
                afterRect = cursor.GetCharacterRect(LogicalDirection.Forward);
                found = afterRect.Top> beforeRect.Top && IsWithinInlineRun(cursor);

                // found true only if wrapped line is not a \r\n
                // For a TextPointer between \r and \n, you get different Rect.Top's. Below conditions 
                // handle that case also.
                found = found &&                
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\r")) &&
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\n")) &&
                    (!cursor.GetTextInRun(LogicalDirection.Backward).EndsWith("\r\n"));

                if (found)
                {
                    return cursor;
                }
                else if ((cursor = cursor.GetPositionAtOffset(directionUnit)) == null)
                {
                    break;
                }
            } while (true);

            return null;
        }

        /// <summary>The first caret position in the Table cell</summary>
        private TextPointer FindStartOfCell(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor = pointer;
            while (cursor != null &&
                !(cursor.GetPointerContext(direction) == TextPointerContext.ElementStart &&
                  cursor.GetAdjacentElement(direction) is TableCell))
            {
                cursor = cursor.GetNextContextPosition(direction);
            }
            if (null != cursor && !cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetInsertionPosition(LogicalDirection.Forward);   
            }
           
            return cursor;
        }

        /// <summary>A caret position at the end of a Table cell</summary>
        private TextPointer FindEndOfCell(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor = pointer;
            while (cursor != null &&
                !(cursor.GetPointerContext(direction) == TextPointerContext.ElementEnd &&
                  cursor.GetAdjacentElement(direction) is TableCell))
            {
                cursor = cursor.GetNextContextPosition(direction);
            }
            if (null != cursor && !cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetInsertionPosition(LogicalDirection.Backward);
            }

            return cursor;
        }

        /// <summary>A caret position at the middle cell of table</summary>
        private TextPointer FindCrossCell(TextPointer pointer, LogicalDirection direction,
            bool evaluatePointer, int directionUnit, bool isPlainText)
        {
            TextPointer cursor = pointer;
            while (cursor != null &&
                !(cursor.GetPointerContext(direction) == TextPointerContext.ElementEnd &&
                  cursor.GetAdjacentElement(direction) is TableCell))
            {
                cursor = cursor.GetNextContextPosition(direction);
            }

            // Move to next TableCell
            while (cursor != null &&
                !(cursor.GetPointerContext(direction) == TextPointerContext.ElementStart &&
                  cursor.GetAdjacentElement(direction) is TableCell))
            {
                cursor = cursor.GetNextContextPosition(direction);
            }

            if (null != cursor && !cursor.IsAtInsertionPosition)
            {
                cursor = cursor.GetInsertionPosition(LogicalDirection.Forward);
            }

            return cursor;
        }
        
        private static bool StartsWithWhitespace(string text)
        {
            if (text.Length == 0)
            {
                return false;
            }

            return text[0] == ' ' || text[0] == '\t';
        }

        private static bool EndsWithWhitespace(string text)
        {
            if (text.Length == 0)
            {
                return false;
            }

            return text[text.Length - 1] == ' ' || text[text.Length - 1] == '\t';
        }

        private static TextPointer FindParagraph(TextPointer pointer, LogicalDirection direction)
        {
            TextPointer cursor;

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            cursor = pointer;
                        
            while (cursor.Parent.GetType() != typeof(Paragraph))
            {
                if ((cursor = cursor.GetNextContextPosition(direction)) == null)
                {
                    return null;
                }
            }

            cursor = cursor.GetNextContextPosition(direction);

            if (cursor.Parent.GetType() != typeof(Run))
            {
                // Move back if direction is Forward
                // Move forward if direcion is Backward
                if (direction == LogicalDirection.Forward)
                    direction = LogicalDirection.Backward;
                else
                    direction = LogicalDirection.Forward;

                cursor = cursor.GetNextContextPosition(direction);
            }

            return cursor;
        }

        /// <summary>
        /// Creates a new DocumentPositionData for plain and
        /// rich text documents.
        /// </summary>
        private static DocumentPositionData ForBoth(DocumentPosition position)
        {
            DocumentPositionData result;

            result = new DocumentPositionData();
            result._documentPosition = position;
            result._isPlainTextSupported = true;

            return result;
        }

        /// <summary>
        /// Checks whether the specified pointer is in the given direction relative
        /// from a given pointer.
        /// </summary>
        /// <param name="pointer">Pointer being evaluated.</param>
        /// <param name="direction">Direction that pointer is expected to be relative from the other pointer.</param>
        /// <param name="relativeFrom">Pointer relative from which the first pointer should be.</param>
        /// <returns>true if pointer can be found relative to relativeFrom in the specified direction, false otherwise.</returns>
        private static bool IsPointerInDirection(TextPointer pointer, LogicalDirection direction,
            TextPointer relativeFrom)
        {
            int comparison;

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }
            if (relativeFrom == null)
            {
                throw new ArgumentNullException("relativeFrom");
            }

            comparison = relativeFrom.CompareTo(pointer);
            return (comparison < 0 && direction == LogicalDirection.Forward) ||
                (comparison > 0 && direction == LogicalDirection.Backward);
        }

        /// <summary>
        /// Checks whether the specified pointer is within a run of inline items.
        /// </summary>
        /// <param name="pointer">Pointer to evaluate.</param>
        /// <returns>true if the pointer is within text or embedded objects; false otherwise.</returns>
        private bool IsWithinInlineRun(TextPointer pointer)
        {
            bool result;

            result =
                pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text ||
                pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement;
            result = result &&
                (pointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text ||
                pointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.EmbeddedElement);

            return result;
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>Value for the document position.</summary>
        private DocumentPosition _documentPosition;

        /// <summary>Whether plain text supports this position.</summary>
        private bool _isPlainTextSupported;

        #endregion Private fields.
    }
}