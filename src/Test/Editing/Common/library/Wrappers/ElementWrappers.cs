// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Abstraction to the UIElement types that support text operations.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Wrappers/ElementWrappers.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Collections;    
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Test.Uis.Utils;
    using Microsoft.Test.Imaging;
    using Test.Uis.TextEditing;

    #endregion Namespaces.

    /// <summary>
    /// Every UIElement tested in text editing scenarios should use
    /// this wrapper for portability.
    /// </summary>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public class UIElementWrapper
    {
        #region Constructors.

        /// <summary>
        /// Creates an UIElementWrapper associated with the specified UIElement.
        /// </summary>
        /// <param name="element">Element to associate the wrapper with.</param>
        public UIElementWrapper(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (!(element is TextBox
                || element is RichTextBox
                || IsFlowDocumentView(element)
                || element is FlowDocumentScrollViewer
                || element is TextBlock
                || element is FlowDocumentPageViewer
                || element is PasswordBox))
            {
                throw ANotSupportedException(element.GetType().Name);
            }

            this._element = element;
        }

        #endregion Constructors.

        #region Public methods.
        
        /// <summary>
        /// Determines if an element is a TextFlow
        /// </summary>
        static bool IsFlowDocumentView(object element)
        {
            return 0 == string.CompareOrdinal(element.GetType().FullName, "MS.Internal.Documents.FlowDocumentView");
        }

        #region Line layout information.

        /// <summary>
        /// Find the line number for a TextPointer
        /// </summary>
        public int LineNumberOfTextPointer(TextPointer textPointer)
        {
            TextPointer lineStart;
            TextPointer lastLineStart;
            int count;
            Rect rec1;
            Rect rec2;


            if (textPointer == null)
            {
                throw new ArgumentNullException("textPointer");
            }

            count = 1;
            lineStart = this.Start;
            if (!lineStart.IsAtInsertionPosition)
            {
                lineStart = lineStart.GetInsertionPosition(LogicalDirection.Forward);
            }

            lineStart = lineStart.GetPositionAtOffset(0, LogicalDirection.Forward);
            lastLineStart = lineStart;
            while ( (lineStart = lineStart.GetLineStartPosition(1)) != null && textPointer.CompareTo(lineStart) >= 0)
            {
                if (lineStart.CompareTo(lastLineStart) == 0)
                {
                    throw new Exception("TextPointer.GetLineStartPosition(1) " +
                        "returns the same pointer rather than null (at position " +
                        TextUtils.GetDistanceFromStart(lineStart) + ").");
                }
                lastLineStart = lineStart;
                count++;

                //if a position is at the end of a line, we need to adjust the line count by -1 
                //Note: the Logical direction on the TextPointer is note dependable here. 
                if (textPointer.CompareTo(lineStart) == 0)
                {
                    rec1 = this.GetElementRelativeCharacterRect(textPointer, 0, LogicalDirection.Forward);
                    rec2 = this.GetElementRelativeCharacterRect(lineStart, 0, LogicalDirection.Forward);
                    if (rec1.Top < rec2.Top)
                    {
                        count--;
                    }
                }
            }
         
            return count;
        }

        /// <summary>
        /// Find the the number of lines, whose height add up to a page size, from a pointer.
        /// </summary>
        /// <param name="startPointer"></param>
        /// <param name="MoveDirection"></param>
        /// <returns></returns>
        public int LinesInAPageFromTextPointer(TextPointer startPointer, bool MoveDirection)
        {
            int count = 0;
            double height = 0;
            double pageHeight;
            int direction;
            Rect rect;

            if (Element is PasswordBox)
            {
                // Password boxes have a single line.
                return 1;
            }

            if (!(Element is TextBoxBase))
            {
                throw ANotSupportedException("LinesInAPageFromTextPointer");
            }

            pageHeight = ((TextBoxBase)Element).ViewportHeight;
            direction = MoveDirection ? 1 : -1;
            TextPointer pointer = startPointer;

            if (!pointer.IsAtInsertionPosition)
            {
                pointer = pointer.GetInsertionPosition(LogicalDirection.Forward);
            }
            //pointer = pointer.GetPositionAtOffset(0, LogicalDirection.Forward);
            rect = this.GetElementRelativeCharacterRect(pointer, 0, LogicalDirection.Forward);
            height=rect.Bottom;

            while ( (pointer = pointer.GetLineStartPosition(direction)) != null)
            {
                rect = this.GetElementRelativeCharacterRect(pointer, 0, LogicalDirection.Forward);//this.GetGlobalCharacterRect(pointer);

                if ( Math.Abs(rect.Bottom - height) >= pageHeight)
                {
                    break;
                }

                count++;
            }
            return count;
        }

        /// <summary>Return the closest Block ancestor.</summary>
        /// <param name="pointer">Position from which to start searching for Block.</param>
        /// <returns>The closest Block ancestor.</returns>
        public Block GetBlockParentForTextPointer(TextPointer pointer)
        {
            DependencyObject blockElement;

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }
            if (!(Element is RichTextBox))
            {
                throw ANotSupportedException("GetBlockParentForTextPointer");
            }

            //we trust ElementWrapper to give use an none null value.
            //we will bottom up to fine a block and we must get one for RichTextBox.
            //If we not a null value at some point, we find a bug.
            blockElement = pointer.Parent;
            while (!(blockElement is Block))
            {
                if (blockElement is FlowDocument)
                {
                    // We might have been outside of all blocks (outside of
                    // the first Paragraph, for example). This is still
                    // a valid case.
                    return null;
                }
                blockElement = ((TextElement)blockElement).Parent;
            }
            return (Block)blockElement;
        }

        /// <summary>
        /// Gets the bounding box of a line relative to the
        /// top-left corner of the element.
        /// </summary>
        /// <param name='lineIndex'>Line index to click on.</param>
        /// <returns>
        /// The bounding rectangle. An exception is thrown if the line
        /// cannot be found.
        /// </returns>
        /// <remarks>Multiple paragraphs are currently not supported.</remarks>
        public Rect GetControlRelativeLineBounds(int lineIndex)
        {
            TextRange lineRange;    // TextRange encompassing contents.
            Rect lineBounds;        // Rectangle bounding line.

            if (lineIndex < 0)
            {
                throw new ArgumentOutOfRangeException("lineIndex",
                    "lineIndex argument should not be negative.");
            }

            lineRange = GetLineRange(lineIndex);
            lineBounds = GetTextRangeBounds(lineRange);
            return lineBounds;
        }

        /// <summary>
        /// Gets the TextRange that spans the specified line.
        /// </summary>
        /// <param name='lineIndex'>Line index.</param>
        /// <returns>The TextRange spanning the line contents.</returns>
        public TextRange GetLineRange(int lineIndex)
        {
            TextPointer startPointer;
            TextPointer endPointer;
            int startMoved;
            int endMoved;

            if (lineIndex < 0)
            {
                throw new ArgumentOutOfRangeException("lineIndex", "lineIndex argument should not be negative.");
            }

            if (!IsPointerAllowedOnThis)
            {
                throw ANotSupportedException("GetLineRange");
            }

            // pointer points to the start of the line and endPointer
            // points to the end of the current line / start of the next line
            // so TextRange(pointer, endPointer) will return current line range.
            startPointer = Start;
            endPointer = Start;

            // let's move startPointer to the line start of the target line
            // and endPointer to the line start of the target line + 1
            // and then set its direction to LogicalDirection.Backward

            //startMoved = startPointer.MoveToLineBoundary(lineIndex);
            //endMoved = endPointer.MoveToLineBoundary(lineIndex + 1);
            startPointer = startPointer.GetLineStartPosition(lineIndex, out startMoved);
            endPointer = endPointer.GetLineStartPosition(lineIndex + 1, out endMoved);

            endPointer = endPointer.GetPositionAtOffset(0, LogicalDirection.Backward);

            // we hit the last line.
            if (startMoved == endMoved)
            {
                endPointer = End;
            }

            return new TextRange(startPointer, endPointer);
        }


        #endregion Line layout information.

        #region Text manipulation.

        /// <summary>
        /// Retrieves text logically before and after TextSelection
        /// </summary>
        /// <param name="ld">LogicalDirection enum to retrieve text, either LogicalDirection.Forward or LogicalDirection.Backward</param>
        /// <returns>string on the backward / forward direction of the selection</returns>
        public string GetTextOutsideSelection(LogicalDirection ld)
        {
            return GetTextOutsideSelection(ld, false);
        }

        /// <summary>
        /// Retrieves text logically before and after TextSelection
        /// For PasswordBox this will return the string in clear text for password
        /// </summary>
        /// <param name="ld">LogicalDirection enum to retrieve text</param>
        /// <param name="preserveTagsInPlainTextFormat">true if the string returned preserves tags.</param>
        /// <returns>string on the backward / forward direction of the selection</returns>
        public string GetTextOutsideSelection(LogicalDirection ld, bool preserveTagsInPlainTextFormat)
        {
            string text;
            int startIndex;
            TextPointer positionalStartPointer;
            TextPointer positionalEndPointer;

            if (_element is TextBox)
            {
                startIndex = SelectionStart + SelectionLength;

                text = ld == LogicalDirection.Forward
                    ? Text.Substring(startIndex, Text.Length - startIndex)
                    : Text.Substring(0, SelectionStart);
            }
            else if (_element is RichTextBox)
            {
                positionalStartPointer = ((RichTextBox)_element).Document.ContentStart;
                positionalEndPointer = ((RichTextBox)_element).Document.ContentEnd;

                positionalStartPointer = SelectionInstance.Start;
                positionalEndPointer = SelectionInstance.End;

                if (SelectionInstance.Start.CompareTo(SelectionInstance.End) > 0)
                {
                    positionalStartPointer = SelectionInstance.End;
                    positionalEndPointer = SelectionInstance.Start;
                }

                if (preserveTagsInPlainTextFormat)
                {
                    text = ld == LogicalDirection.Forward
                        ? TextRangeDumper.Dump(positionalEndPointer, End, false /*convertTagToSymbol*/, true)
                        : TextRangeDumper.Dump(Start, positionalStartPointer, false /*convertTagToSymbol*/, true);
                }
                else
                {
                    text = ld == LogicalDirection.Forward
                        ? (new TextRange(positionalEndPointer, End)).Text
                        : (new TextRange(Start, positionalStartPointer)).Text;
                }
            }
            else if (_element is PasswordBox)
            {
                startIndex = SelectionStart + SelectionLength;

                text = ld == LogicalDirection.Forward
                    ? Text.Substring(startIndex, Text.Length - startIndex)
                    : Text.Substring(0, SelectionStart);
            }
            else
            {
                throw ANotSupportedException("GetTextOutsideSelection");
            }

            return text;
        }

        /// <summary>
        /// Get global (relative to desktop) character rect of the last character in the element
        /// wrapped in this wrapper.
        /// </summary>
        /// <returns>Rectangle of the last character</returns>
        public Rect GetGlobalCharacterRectOfLastCharacter()
        {
            Rect rect;

            if (IsPointerAllowedOnThis)
            {
                rect = GetGlobalCharacterRect(End, LogicalDirection.Backward);
            }
            else
            {
                // special case for now.
                if (_element is TextBox)
                {
                    rect = GetGlobalCharacterRect(Test.Uis.Utils.TextUtils.GetTextBoxEnd((TextBox)_element), LogicalDirection.Backward);
                }
                else
                {
                    throw ANotSupportedException("GetGlobalCharacterRectOfLastCharacter");
                }
            }

            return rect;
        }

        /// <summary>
        /// Gets the rectangle bounding the character forward of the
        /// position index-positions away from the document start,
        /// relative to the screen origin.
        /// </summary>
        /// <param name="index">Number of positions to move.</param>
        /// <returns>
        /// The bounding rectangle of the character. An exception is
        /// thrown if there is no character or the bounding box cannot
        /// be calculated.
        /// </returns>
        /// <example>The following code shows how to use this method.<code>...
        /// private void ClickOnChar(int index) {
        ///   Rect r = wrapper.GetGlobalCharacterRect(index);
        ///   MouseInput.MouseClick(r.Left + r.Width / 2, r.Top + r.Height / 2);
        /// }</code></example>
        public Rect GetGlobalCharacterRect(int index)
        {
            TextBox textbox;

            textbox = this._element as TextBox;
            if (textbox != null)
            {
                Rect rectCharacterInElement;
                Rect rectElement;
                float hFactor, vFactor; // Scaling factors for high DPI.

                rectCharacterInElement = textbox.GetRectFromCharacterIndex(index);
                rectElement = ElementUtils.GetScreenRelativeRect(_element);

                HighDpiScaleFactors(out hFactor, out vFactor);
                return new Rect(
                    rectElement.Location.X + rectCharacterInElement.Location.X * hFactor,
                    rectElement.Location.Y + rectCharacterInElement.Location.Y * vFactor,
                    rectCharacterInElement.Width * hFactor,
                    rectCharacterInElement.Height * vFactor);
            }
            else
            {
                return GetGlobalCharacterRect(index, LogicalDirection.Forward);
            }
        }

        /// <summary>
        /// Handle overload to retrieve LogicalDirection.Forward rect at TextPointer
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public Rect GetGlobalCharacterRect(TextPointer pointer)
        {
            return GetGlobalCharacterRect(pointer, LogicalDirection.Forward);
        }

        /// <summary>
        /// Retrieve LogicalDirection.Forward rect relative to the TextPointer at index offset
        /// </summary>
        /// <param name="pointer">TextPointer to retrieve rect</param>
        /// <param name="index">Offset to the TextPointer</param>
        /// <returns></returns>
        public Rect GetGlobalCharacterRect(TextPointer pointer, int index)
        {
            TextPointer newPointer;

            newPointer = pointer.GetPositionAtOffset(index);

            return GetGlobalCharacterRect(newPointer, LogicalDirection.Forward);
        }

        /// <summary>
        /// Gets the rectangle bounding the character forward of the
        /// specified TextPointer, relative to the screen origin.
        /// </summary>
        /// <param name="position">TextPointer to offset and get rectangle for.</param>
        /// <param name="direction">Direction from where the rectangle is retrieved.</param>
        /// <returns>
        /// The rectangle bounding the character, Rect.Empty if the
        /// information is not available.
        /// </returns>
        public Rect GetGlobalCharacterRect(TextPointer position, LogicalDirection direction)
        {
            Rect rectElement;           // Rectangle of element.
            Rect rectRelativeCharacter; // Rectangle of char relative to element.

            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            rectElement = ElementUtils.GetScreenRelativeRect(_element);
            rectRelativeCharacter = GetElementRelativeCharacterRectPrivate(position, direction);
            if (rectRelativeCharacter.IsEmpty)
            {
                string message;
                message = "The following TextPointer offset by index caret units " +
                    "results in an empty Rect, which cannot be used to calculate " +
                    "a bounding rectange." + Environment.NewLine +
                    Test.Uis.Loggers.TextTreeLogger.Describe(position);
                throw new InvalidOperationException(message);
            }
            //Adjust to Hight DPI
            float hFactor, vFactor;
            HighDpiScaleFactors(out hFactor, out vFactor);

            return new Rect(
                rectElement.Location.X + rectRelativeCharacter.Location.X * hFactor,
                rectElement.Location.Y + rectRelativeCharacter.Location.Y * vFactor,
                rectRelativeCharacter.Width * hFactor, rectRelativeCharacter.Height * vFactor);
        }

        /// <summary>
        /// Retrieve rect for character, index (displacement from TextPointer),
        /// and LogicalDirection relative to the start of document. LogicalDirection is always the
        /// relative positioning of characters in the backing store
        /// </summary>
        /// <param name="index">"symbol" index where the character rect is retrieved</param>
        /// <param name="direction">LogicalDirection of character to be retrieved relative to TextPointer</param>
        /// <returns></returns>
        public Rect GetGlobalCharacterRect(int index, LogicalDirection direction)
        {
            TextPointer pointer;

            if (_element is TextBox)
            {
                pointer = Test.Uis.Utils.TextUtils.GetTextBoxStart((TextBox)_element).GetPositionAtOffset(index);
            }
            else
            {
                if (_element is RichTextBox)
                {
                    pointer = ((RichTextBox)_element).Document.ContentStart;
                }
                else if(IsFlowDocumentView(_element)) {
                    pointer = (TextPointer)Test.Uis.Utils.ReflectionUtils.GetProperty(_element, "ContentStart");
                }
                else if (_element is FlowDocumentScrollViewer)
                {
                    pointer = ((FlowDocumentScrollViewer)_element).Document.ContentStart;
                }
                else if (_element is TextBlock)
                {
                    pointer = ((TextBlock)_element).ContentStart;
                }
                else
                {
                    throw ANotSupportedException("GetGlobalCharacterRect");
                }

                pointer = pointer.GetPositionAtOffset(index);
            }

            return GetGlobalCharacterRect(pointer, direction);
        }

        /// <summary>
        /// Retrieve rect for character, given TextPointer, index (displacement from TextPointer),
        /// and LogicalDirection relative to the TextPointer. LogicalDirection is always the
        /// relative positioning of characters in the backing store
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="index"></param>
        /// <param name="direction">LogicalDirection relative to TextPointer</param>
        /// <returns></returns>
        public Rect GetElementRelativeCharacterRect(TextPointer pointer, int index, LogicalDirection direction)
        {
            TextPointer textPointer;

            textPointer = pointer.GetPositionAtOffset(index);

            return GetElementRelativeCharacterRectPrivate(textPointer, direction);
        }

        /// <summary>
        /// Returns true if the symbol pointed to by the TextPointer in that direction
        /// is a real line break (carriage return / paragraph, linebreak or pagebreak tag)
        /// false otherwise
        /// </summary>
        /// <param name="textPointer">TextPointer</param>
        /// <param name="direction">LogicalDirection to look at</param>
        /// <returns>True if there is a real line break orelse False</returns>
        private bool IsRealLineBreak(TextPointer textPointer, LogicalDirection direction)
        {
            TextPointerContext textPointerContext;
            char[] c;
            int length;
            bool realLineBreak;

            realLineBreak = false;
            textPointerContext = textPointer.GetPointerContext(direction);

            if (textPointerContext == TextPointerContext.Text)
            {
                c = new char[2];
                length = textPointer.GetTextInRun(direction, c, 0, 2);
                if ((length == 2 && System.Environment.NewLine.ToCharArray() == c)
                    || (length == 1 && c[0] == '\n'))
                {
                    realLineBreak = true;
                }
            }
            else if (IsBreakNextToPointer(textPointer, direction))
            {
                realLineBreak = true;
            }
            return realLineBreak;
        }

        private bool IsBreakNextToPointer(TextPointer textPointer, LogicalDirection direction)
        {
            DependencyObject obj;
            bool result;
            TextPointerContext context;
            TextPointer pointerClone;
            TextPointer pointerClone2;
            TextPointerContext context2;

            result = false;
            pointerClone = textPointer;
            obj = pointerClone.Parent;

            while (!(obj is UIElement || result /* we found a Paragraph*/ ))
            {
                context = pointerClone.GetPointerContext(direction);

                // if we see Text, None, EmbeddedElement,
                // this is not a break next to this TextPointer
                // bail out now.
                if (context == TextPointerContext.Text
                    || context == TextPointerContext.None
                    || context == TextPointerContext.EmbeddedElement)
                {
                    break;
                }

                // <ElementStart>| or |<ElementEnd>
                if (((context == TextPointerContext.ElementStart && direction == LogicalDirection.Backward)
                    || (context == TextPointerContext.ElementEnd && direction == LogicalDirection.Forward)))
                {
                    if (obj is Paragraph)
                    {
                        pointerClone2 = pointerClone;

                        pointerClone2 = pointerClone2.GetNextContextPosition(direction);
                        context2 = pointerClone2.GetPointerContext(direction);

                        if ((context2 == TextPointerContext.ElementStart && direction == LogicalDirection.Forward)
                            || (context2 == TextPointerContext.ElementEnd && direction == LogicalDirection.Backward))
                        {
                            pointerClone2 = pointerClone2.GetNextContextPosition(direction);
                            if (pointerClone2.Parent is Paragraph)
                            {
                                result = true;
                            }
                        }
                    }
                }
                // |<ElemntStart> or <ElementEnd>|
                else if (((context == TextPointerContext.ElementStart && direction == LogicalDirection.Forward)
                    || (context == TextPointerContext.ElementEnd && direction == LogicalDirection.Backward)))
                {
                    if (obj is LineBreak )
                    {
                        result = true;
                    }
                }

                if (!result)
                {
                    pointerClone = pointerClone.GetNextContextPosition(direction);
                    obj = pointerClone.Parent;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the bounding rectangle for the character forward of
        /// 'index' positions away from the given position,
        /// relative to the wrapped element.
        /// </summary>
        /// <param name="pointer">
        /// TextPointer from which to count for character rectangle.
        /// </param>
        /// <param name="direction">LogicalDirection of the character relative to the TextPointer,
        /// LogicalDirection means the relative position of characters in backing store</param>
        /// <example>
        /// Get the 4th character in a plain text box
        /// <code>GetElementRelativeCharacterRect(this.TextPointerStart, 4);</code>
        /// </example>
        /// <returns>
        /// The bounding rectangle for the specified character. An
        /// exception is thrown if the character cannot be retrieved.
        /// </returns>
        private Rect GetElementRelativeCharacterRectPrivate(TextPointer pointer, LogicalDirection direction)
        {
            TextPointer textPointer;
            TextPointer currentLineTextPointer;
            TextPointer nextLineTextPointer;
            TextPointer nextTextPointer;
            Rect rect;
            Rect nextRect;
            Rect backwardRect;
            Rect forwardRect;

            textPointer = pointer;

            // need to normalize this
            if (!textPointer.IsAtInsertionPosition)
            {
                // if we are trying to get the rect for the TextPointer at LogicalDirection.Forward,
                // normalize the TextPointer to backwards
                if (direction == LogicalDirection.Forward)
                {
                    textPointer = textPointer.GetInsertionPosition(LogicalDirection.Backward);
                }
                else
                {
                    textPointer = textPointer.GetInsertionPosition(LogicalDirection.Forward);
                }
            }

            // Get back the empty width rectangle to that direction at this TextPointer location
            rect = textPointer.GetCharacterRect(direction);

            // Create another TextPointer for the next / previous position
            nextTextPointer = textPointer;

            // Move the nextTextPointer to one insertion distance in that direction
            // If MoveToNextInsertionPosition retruns true, we need to calculate the
            // next location and do the subraction of two rects to find out the width of the rect.
            // otherwise, we either reach the start / end of the document
            // (direction = Backward / direction = Forward respectively) we should return
            // the location of the rect right away.
            if ( (nextTextPointer = nextTextPointer.GetNextInsertionPosition(direction)) != null)
            {
                // if we need to get the bounding rect in forward direction for this TextPointer,
                // we move nextTextPointer to forward and retrieve the empty width rect in backward
                // direction for nextTextPointer
                // Conversely if we need to get the bounding rect in backward direction for this TextPointer,
                // we move nextTextPointer to backward and retrieve the empty width rect in forward
                // direction for nextTextPointer
                nextRect = nextTextPointer.GetCharacterRect(direction == LogicalDirection.Forward
                    ? LogicalDirection.Backward
                    : LogicalDirection.Forward);

                // split line or some other content
                if (!IsRealLineBreak(textPointer, direction))
                {
                    // The idea here is to make two more TextPointers
                    // the first one (currentLineTextPointer) moves to line start
                    // the second one is the one moved to next position, and this
                    // is also moved to line start, if at the end it turns out
                    // that they are pointing to different positions
                    // we have got a split line
                    currentLineTextPointer = textPointer;
                    currentLineTextPointer = currentLineTextPointer.GetLineStartPosition(0);

                    nextLineTextPointer = nextTextPointer;
                    nextLineTextPointer = nextLineTextPointer.GetLineStartPosition(0);

                    if (currentLineTextPointer.CompareTo(nextLineTextPointer) != 0)
                    {
                        backwardRect = textPointer.GetCharacterRect(LogicalDirection.Backward);
                        forwardRect = textPointer.GetCharacterRect(LogicalDirection.Forward);

                        // if we need to retrieve backward rect,
                        // we are getting the difference between
                        // rect and backwardRect
                        // otherwise, nextRect and forwardRect
                        if (direction == LogicalDirection.Backward)
                        {
                            if (rect.X > nextRect.X)
                            {
                                rect.X = nextRect.X;
                                rect.Y = nextRect.Y;
                                rect.Height = nextRect.Height;
                            }
                            else
                            {
                                /* rect is already equal to backwardRect. The below statements are only for clarity */
                                rect.X = backwardRect.X;
                                rect.Y = backwardRect.Y;
                                rect.Height = backwardRect.Height;
                            }
                            rect.Width = Math.Abs(backwardRect.X - nextRect.X);
                        }
                        else
                        {
                            if (rect.X > forwardRect.X)
                            {
                                /* rect is already equal to forwardRect. The below statements are only for clarity */
                                rect.X = forwardRect.X;
                                rect.Y = forwardRect.Y;
                                rect.Height = forwardRect.Height;
                            }
                            else
                            {
                                rect.X = nextRect.X;
                                rect.Y = nextRect.Y;
                                rect.Height = nextRect.Height;
                            }
                            rect.Width = Math.Abs(nextRect.X - forwardRect.X);
                        }
                    }
                    else
                    {
                        rect.Width = Math.Abs(rect.Left - nextRect.Left);

                        // if we are retriving the backward rect, we need to set the
                        // returing rect.X to be nextRect.X (which is before current TextPointer)
                        if (rect.X > nextRect.X)
                        {
                            rect.X = nextRect.X;
                            rect.Height = nextRect.Height;
                            rect.Y = nextRect.Y;
                        }
                    }
                }
            }

            return rect;
        }

        /// <summary>
        /// Clear the text in _element
        /// </summary>
        public void Clear()
        {
            if (_element is TextBox)
            {
                ((TextBox)_element).Clear();
            }
            else if (_element is RichTextBox)
            {
                ((RichTextBox)_element).Document = new FlowDocument(new Paragraph(new Run()));
            }
            else if (_element is PasswordBox)
            {
                ((PasswordBox)_element).Clear();
            }
            else if (IsFlowDocumentView(_element))
            {
                TextPointer contentStart = (TextPointer)Test.Uis.Utils.ReflectionUtils.GetProperty(_element, "ContentStart");
                TextPointer contentEnd = (TextPointer)Test.Uis.Utils.ReflectionUtils.GetProperty(_element, "ContentEnd");
                new TextRange(contentStart, contentEnd).Text = String.Empty;
            }
            else if (_element is FlowDocumentScrollViewer)
            {
                new TextRange(((FlowDocumentScrollViewer)_element).Document.ContentStart, ((FlowDocumentScrollViewer)_element).Document.ContentEnd).Text = String.Empty;
            }
            else if (_element is TextBlock)
            {
                new TextRange(((TextBlock)_element).ContentStart, ((TextBlock)_element).ContentEnd).Text = String.Empty;
            }
            else
            {
                throw ANotSupportedException(_element.GetType().Name);
            }
        }

        /// <summary>
        /// check to see if the caret is at the end of a line
        /// </summary>
        public bool IsCaretAtEndOfLine
        {
            get
            {
                TextPointer nextLine;

                //Check to see if selection is empty.
                if (SelectionInstance.Start.CompareTo(SelectionInstance.End) != 0)
                {
                    throw new Exception("Selection is not empty and you should have no Caret!");
                }
                //Return true if the caret is at the end of a Document.
                if (IsCaretAtEndOfDocument)
                {
                    return true;
                }
                //find the next insertion pointer that must be at the beginning of a line.
                nextLine = SelectionInstance.End.GetNextInsertionPosition(LogicalDirection.Forward);

                //IsAtLineStartPosition will skip formats.
                if (nextLine.IsAtLineStartPosition)
                {
                    return true;
                }
                else
                {
                    Rect rec1 = SelectionInstance.End.GetCharacterRect(LogicalDirection.Backward);
                    Rect rec2 = nextLine.GetCharacterRect(LogicalDirection.Forward);
                    return (rec1.Top < rec2.Top) ? true : false;
                }
            }
        }


        /// <summary>
        /// visual verification of rendered caret.
        /// This method my not support some fancy font and Italic.
        /// Not sure if this support international Language.
        /// </summary>
        /// <param name="CaretImage">Return the image that should contains the caret</param>
        /// <param name="isRightToLeft">If the character</param>
        /// <returns>true if rendered, false otherwise</returns>
        public bool IsCaretRendered(out System.Drawing.Bitmap CaretImage, bool isRightToLeft)
        {
            const double spacing = 1;   // Space before and after caret.

            System.Drawing.Bitmap elementBitmap;    // Colored image.
            System.Drawing.Bitmap bw;               // Black and white image.
            double caretWidth;                      // Expected width of caret.

            caretWidth = SystemParameters.CaretWidth;

            using (elementBitmap = BitmapCapture.CreateBitmapFromElement(this._element))
            using (bw = BitmapUtils.ColorToBlackWhite(elementBitmap))
            {
                CaretImage = null;
                TextPointer caretPointer;
                Rect rect; /* adjusted zero-width rect from rawRect for caret location */

                // selection is not empty, so no caret
                if (SelectionLength != 0)
                {
                    throw new InvalidOperationException("Selection is not empty.");
                }

                caretPointer = SelectionInstance.Start;

                // the caller knows that if the character next to the TextPointer
                // is RTL or LTR char or not.
                rect = GetElementRelativeCharacterRectPrivate(caretPointer,
                    isRightToLeft ? LogicalDirection.Backward : LogicalDirection.Forward);

                //scale to Dpi
                rect = BitmapUtils.AdjustBitmapSubAreaForDpi(bw, rect);

                // for caret finding purpose we just need a zero-width rect
                rect.Width = 0.0;

                if ((isRightToLeft && caretPointer.LogicalDirection == LogicalDirection.Forward)
                    || (!isRightToLeft && caretPointer.LogicalDirection == LogicalDirection.Backward))
                {
                    rect.X += rect.Width;
                }

                if (caretPointer.GetTextInRun(LogicalDirection.Forward) != string.Empty
                    && caretPointer.GetTextInRun(LogicalDirection.Backward) == string.Empty)
                {
                    rect.X = rect.X - spacing;

                }
                else if (caretPointer.GetTextInRun(LogicalDirection.Forward) == string.Empty
                    && caretPointer.GetTextInRun(LogicalDirection.Backward) != string.Empty)
                {
                    rect.X = rect.X - rect.Width - spacing;
                }
                else
                {
                    rect.X = rect.X - caretWidth - spacing;
                }
                rect.Width = caretWidth + spacing * 2;
                CaretImage = BitmapUtils.CreateSubBitmap(bw, rect);
                System.Drawing.Rectangle rectangle;
                return BitmapUtils.GetTextCaret(CaretImage, out rectangle);
            }
        }

        /// <summary>
        /// Find out if a TextPointer is inside a Table element
        /// </summary>
        public bool IsTextPointerInsideTextElement (TextPointer pointer, Type type)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            TextElement tag = pointer.Parent as TextElement;
            while (tag != null)
            {
                if(tag.GetType().Equals(type))
                {
                    return true;
                }
                tag = tag.Parent as TextElement;
            }
            return false;
        }


        /// <summary>
        /// Invokes the specified command on the target element.
        /// </summary>
        /// <param name='command'>Command to invoke.</param>
        /// <returns>true if the command was handled; false otherwise.</returns>
        public bool RaiseCommand(System.Windows.Input.RoutedCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (command.CanExecute(null, _element))
            {
                command.Execute(null, _element);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invokes the specified command on the target element.
        /// </summary>
        /// <param name='commandName'>Command to invoke.</param>
        /// <returns>true if the command was handled; false otherwise.</returns>
        /// <remarks>
        /// commandName is typically resolved as TextEditor command
        /// exposed as a [commandName]Command property.
        /// </remarks>
        public bool RaiseCommand(string commandName)
        {
            /*
            System.Windows.Input.RoutedCommand command;

            if (commandName == null)
            {
                throw new ArgumentNullException("commandName");
            }
            if (commandName.Length == 0)
            {
                throw new ArgumentException("commandName cannot be empty");
            }


            command = ReflectionUtils.GetStaticProperty(typeof(TextEditor),
                commandName + "Command") as System.Windows.Input.RoutedCommand;
            if (command == null)
            {
                throw new ArgumentException("Cannot find command in TextEditor for" +
                    "command [" + commandName + "]");
            }

            return RaiseCommand(command);
            */
            return true;
        }

        /// <summary>
        /// Uses mouse input emulation to select the specified range.
        /// </summary>
        /// <param name='direction'>option to specify whether it goes left to right or right to left.</param>
        /// <param name="range">TextRange to select text</param>
        public void SelectCharacterByMouse(SelectionDirection direction, TextRange range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            SelectCharacterByMouse(direction, range.Start, range.End);
        }

        /// <summary>
        /// Uses mouse input emulation to select the specified range.
        /// </summary>
        /// <param name='direction'>option to specify whether it goes left to right or right to left.</param>
        /// <param name='start'>Start position of selection.</param>
        /// <param name='end'>End position of selection.</param>
        public void SelectCharacterByMouse(SelectionDirection direction, TextPointer start, TextPointer end)
        {
            Rect startRect;     // Rect bounding start character.
            Rect endRect;       // Rect bounding end character.
            Point startPoint;   // Start point for mouse movement.
            Point endPoint;     // End point for mouse movement.

            // TextPointers used for swapping if necessary
            TextPointer startPointer;
            TextPointer endPointer;

            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            startPointer = start;
            endPointer = end;

            // we need to swap if
            // 1. direction == Left to right and end is before start
            // 2. direction == right to left and end is after start
            switch (direction)
            {
                case SelectionDirection.LeftToRight:
                    if (end.CompareTo(start) < 0)
                    {
                        startPointer = end;
                        endPointer = start;
                    }
                    break;
                case SelectionDirection.RightToLeft:
                    if (end.CompareTo(start) > 0)
                    {
                        startPointer = end;
                        endPointer = start;
                    }
                    break;
            }



            // Get the rectangle for the ietms.
            startRect = GetGlobalCharacterRect(startPointer, 0);
            if (startRect.IsEmpty)
            {
                throw new InvalidOperationException("Rectangle for start position is empty.");
            }

            endRect = GetGlobalCharacterRect(startPointer, 0);
            if (endRect.IsEmpty)
            {
                throw new InvalidOperationException("Rectangle for end position is empty.");
            }

            startPoint = new Point(startRect.Left, startRect.Top + startRect.Height / 2);
            endPoint = new Point(endRect.Right, endRect.Top + endRect.Height / 2);

            MouseInput.MouseDown(startPoint);

            QueueHelper queueHelper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);
            queueHelper.QueueDelegate(new MouseDragDelegate(MouseInput.MouseDrag), new object[] { startPoint, endPoint });
            queueHelper.QueueDelegate(new SimpleHandler(MouseInput.MouseUp));
        }

        /// <summary>
        /// Selects text in the wrapped instance.
        /// </summary>
        /// <param name="start">Index to the symbol.</param>
        /// <param name="length">number of symbols to select.</param>
        public void Select(int start, int length)
        {
            if (_element is TextBox)
            {
                ((TextBox)_element).Select(start, length);
            }
            else if (_element is PasswordBox)
            {
                ReflectionUtils.InvokeInstanceMethod(_element, "Select",
                    new object[] { start, length });
            }
            else if (_element is RichTextBox)
            {
                TextSelection selection;
                TextPointer startPointer;
                TextPointer endPointer;

                selection = this.SelectionInstance;
                startPointer = selection.Start.DocumentStart;
                startPointer = startPointer.GetPositionAtOffset(start);

                endPointer = startPointer.GetPositionAtOffset(length);
                if (endPointer == null)
                {
                    throw new ApplicationException("Selection indices are out of range");
                }

                selection.Select(startPointer, endPointer);
            }
            else
            {
                throw ANotSupportedException("Select");
            }
        }

        /// <summary>Selects all the element content.</summary>
        public void SelectAll()
        {
            if (_element is TextBoxBase)
            {
                ((TextBoxBase)_element).SelectAll();
            }
            else if (_element is PasswordBox)
            {
                ((PasswordBox)_element).SelectAll();
            }
            else
            {
                throw ANotSupportedException("SelectAll");
            }
        }

        /// <summary>
        /// Get plain text representation of the TextRange
        /// e.g. This is <Bold>Bold</Bold>text
        /// </summary>
        /// <param name="replaceTagsWithSymbol"></param>
        /// <returns></returns>
        public string GetPlainTextRepresentation(bool replaceTagsWithSymbol)
        {
            string text;
            text = String.Empty;

            if (IsPointerAllowedOnThis)
            {
                if (Start.CompareTo(End) > 0)
                {
                    text = TextRangeDumper.Dump(End, Start, replaceTagsWithSymbol, true /* normalize close tags */);
                }
                else
                {
                    text = TextRangeDumper.Dump(Start, End, replaceTagsWithSymbol, true /* normalize close tags */);
                }
            }
            else
            {
                text = Text;
            }

            return text;
        }

        /// <summary>
        /// Retrieve text under selection. Caller can specify if raw text is returned
        /// or if the text has preserved formatting tags.
        /// </summary>
        /// <param name="preserveFormattingTagsInPlainText">true if the returned text has all
        /// formatting tags preserved, false otherwise</param>
        /// <param name="normalizeCloseTags">true if the returned text has got balanced
        /// open and close tags, false otherwise. This argument is ignored if presrveFormattingTagsInPlainText
        /// is false</param>
        /// <returns></returns>
        public string GetSelectedText(bool preserveFormattingTagsInPlainText,
            bool normalizeCloseTags)
        {
            TextSelection textSelection;
            string text;

            if (IsPointerAllowedOnThis
                && IsSelectionAllowedOnThis
                && preserveFormattingTagsInPlainText)
            {
                textSelection = this.SelectionInstance;

                if (textSelection.Start.CompareTo(textSelection.End) < 0)
                {
                    text = TextRangeDumper.Dump(textSelection.Start,
                        textSelection.End,
                        false,
                        normalizeCloseTags);
                }
                else
                {
                    text = TextRangeDumper.Dump(textSelection.End,
                        textSelection.Start,
                        false,
                        normalizeCloseTags);
                }
            }
            else
            {
                if (_element is TextBox)
                {
                    text = ((TextBox)_element).SelectedText;
                }
                else if (_element is RichTextBox)
                {
                    text = ((RichTextBox)_element).Selection.Text;
                }
                else if (_element is PasswordBox)
                {
                    object passwordSelection;

                    passwordSelection = GetITextSelection();
                    return (string)ReflectionUtils.GetInterfaceProperty(
                        passwordSelection, "ITextRange", "Text");
                }
                else
                {
                    throw ANotSupportedException("SelectedText");
                }
            }

            return text;
        }

        /// <summary>
        /// Dump plain text representation from start to end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="convertTagsToSymbol"></param>
        /// <returns></returns>
        public static string GetPlainTextRepresentation(TextPointer start, TextPointer end, bool convertTagsToSymbol)
        {
            return TextRangeDumper.Dump(start, end, convertTagsToSymbol, true /*normalize close tags */);
        }

        /// <summary>
        /// Select characters with the mouse.
        /// </summary>
        /// <param name="direction">SelectionDirection, either LeftToRight or RightToLeft</param>
        /// <param name="startIndex">zero-based index of the starting character to be selected</param>
        /// <param name="cch">number of characters to be selected</param>
        public void SelectCharacterByMouse(SelectionDirection direction, int startIndex, int cch)
        {
            Rect rect = GetGlobalCharacterRect(startIndex);
            Rect rect1 = GetGlobalCharacterRect(startIndex + cch - 1);
            Point startPoint = new Point(MathUtils.GetSmallestPossibleIntegerValueWithinTheRect(rect), rect.Top + rect.Height / 2); ;

            Point endPoint = new Point(Math.Ceiling(rect1.Right), rect1.Top + rect1.Height / 2);

            switch (direction)
            {
                case SelectionDirection.RightToLeft:
                    Point swapPoint = startPoint;

                    startPoint = endPoint;
                    endPoint = swapPoint;
                    break;
            }
            MouseInput.MouseDown(startPoint);

            QueueHelper queueHelper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);
            queueHelper.QueueDelegate(new MouseDragDelegate(MouseInput.MouseDrag), new object[] { startPoint, endPoint });
            queueHelper.QueueDelegate(new SimpleHandler(MouseInput.MouseUp));
        }

        /// <summary>
        /// Gets the first character(which is a number) in the window
        /// </summary>
        /// <returns>the index of the line</returns>
        public int GetIndexOfFirstStringInWindow(double Ycoordinate, out double currYcoordinate)
        {
            currYcoordinate = 0; //initialise for default value
            if (_element is TextBox)
            {
                //use line indices for textbox because it corresponds with the numbering of the lines
                return ((TextBox)_element).GetFirstVisibleLineIndex();
            }
            else if (_element is RichTextBox)
            {
                double tempVal;
                TextPointer _tp = GetPointerToFirstStringInRTB(Ycoordinate, out tempVal);
                currYcoordinate = tempVal;
                return (this.LineNumberOfTextPointer(_tp) - 1); //to make it 0 based

            }
            return 0;
        }

        /// <summary>
        /// Gets Pointer to first string in window
        /// </summary>
        /// <param name="Ycoordinate">the vertical offset needed</param>
        /// <param name="currYcoordinate">the offset at which the string was found</param>
        /// <returns></returns>
        public TextPointer GetPointerToFirstStringInRTB(double Ycoordinate, out double currYcoordinate)
        {
            currYcoordinate = 0; //default value
            TextPointer _tp;
            string _tempBuffer = "";
            double x = 15.00;
            double y = Ycoordinate;
            y = (y > 0) ? y : 10;
            int _count = 0;

            do
            {
                //first time use default value of y (10) or prev value of y
                //previous value us necessary because RTB has padding at the top and bottom
                //of each line. Thus even when it is line down the line is not completely off
                //the screen. So it is necessary to get previous value of y so that u
                //dont hit the same line

                //next iteration increment y by 10
                if (_count != 0)
                {
                    y += 10.00;

                }
                _count++;
                //invalid case. cant be >60
                if (y > 60)
                {
                    return null;
                }

                //move in forward direction from the pointer
                System.Windows.Point _p = new System.Windows.Point(x, y);
                _tp = ((RichTextBox)_element).GetPositionFromPoint(_p, true);
                _tempBuffer = _tp.GetTextInRun(LogicalDirection.Forward);

            } while (_tempBuffer == "");
            //continue searching till u hit a character

            currYcoordinate = y;
            return _tp;
        }

        /// <summary> Finds first string in window </summary>
        public string GetFirstStringInWindow(double Ycoordinate, out double currYcoordinate)
        {
            currYcoordinate = 0; //default value
            if (_element is TextBox)
            {
                //Returns text of the visible line.... checks if scroll keeps focus on same line
                int _index = ((TextBox)_element).GetFirstVisibleLineIndex();
                return ((TextBox)_element).GetLineText(_index);
            }
            else if (_element is RichTextBox)
            {
                double _tempVal;
                string _tempBuffer = "";
                TextPointer _tp = GetPointerToFirstStringInRTB(Ycoordinate, out _tempVal);

                currYcoordinate = _tempVal;
                _tempBuffer = _tp.GetTextInRun(LogicalDirection.Forward);
                return (_tempBuffer);
            }
            return null;
        }

        /// <summary> Finds index of last line in window </summary>
        public int GetIndexOfLastStringInWindow(double Ycoordinate, out double currYcoordinate)
        {
            currYcoordinate = 0; //default value
            if (_element is TextBox)
            {
                return ((TextBox)_element).GetLastVisibleLineIndex();
            }
            else if (_element is RichTextBox)
            {
                double tempVal;
                TextPointer _tp = GetPointerToLastStringInRTB(Ycoordinate, out tempVal);
                currYcoordinate = tempVal;
                return (this.LineNumberOfTextPointer(_tp) - 1); //to make it 0 based

            }
            return 0;
        }

        /// <summary>
        /// Gets Pointer to last string in window
        /// </summary>
        /// <param name="Ycoordinate">the vertical offset needed</param>
        /// <param name="currYcoordinate">the offset at which the string was found</param>
        /// <returns></returns>
        public TextPointer GetPointerToLastStringInRTB(double Ycoordinate, out double currYcoordinate)
        {
            currYcoordinate = 0; //default value
            TextPointer _tp;
            string _tempBuffer = "";
            double x = 15.00;
            double y = Ycoordinate;
            y = (y > 0) ? y : (((TextBoxBase)_element).ViewportHeight - 10);// _element.Height;
            int _count = 0;
            do
            {
                if (_count != 0)
                {
                    y -= 10.00;

                }
                _count++;

                System.Windows.Point _p = new System.Windows.Point(x, y);
                _tp = ((RichTextBox)_element).GetPositionFromPoint(_p, true);
                _tempBuffer = _tp.GetTextInRun(LogicalDirection.Forward);

            } while (_tempBuffer == "");
            currYcoordinate = y;
            return _tp;
        }

        #endregion Text manipulation.

        #region Debugging utilities.

        /// <summary>Provides a string representation of the element.</summary>
        public override string ToString()
        {
            return "UIElementWrapper [element=" + Element.ToString() + ",element.Name=" + Element.GetValue(Control.NameProperty) + "]";
        }

        #endregion Debugging utilities.

        #region Caret support.

        /// <summary>
        /// Finds the adorner that represents the caret element under the
        /// given visual tree.
        /// </summary>
        /// <param name='node'>Root node in which to look for caret.</param>
        /// <returns>The caret element, possibly null.</returns>
        /// <remarks>
        /// If the Caret should always be present, use the CaretElement
        /// property, which throws an exception if the caret is not
        /// found.
        /// </remarks>
        private Adorner FindCaretElement(Visual node)
        {
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                Visual v = VisualTreeHelper.GetChild(node, i) as Visual;
                if (v != null)
                {
                    UIElement result;
                    if (v.GetType().Name == "CaretElement")
                    {
                        result = (UIElement) v;
                    }
                    else
                    {
                        result = FindCaretElement(v);
                    }
                    if (result != null)
                    {
                        return (Adorner)result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the caret on the control, and returns the
        /// new RenderSize value.
        /// </summary>
        /// <returns>The new RenderSize value.</returns>
        /// <remarks>
        /// If there is no caret available, an exception is thrown.
        /// </remarks>
        public Size GetUpdatedCaretRenderSize()
        {
            _element.UpdateLayout();

            return CaretElement.RenderSize;
        }

        /// <summary>
        /// Gets the position of the caret relative to the rendering
        /// control.
        /// </summary>
        /// <returns>
        /// The position of the caret relative to the rendering control
        /// (the adorned element), -1;-1 if no caret is available.
        /// </returns>
        public Point GetDocumentRelativeCaretPosition()
        {
            Adorner caret;
            double left, top;
            UIElement adornedElement;

            caret = FindCaretElement(this.Element);
            if (caret == null)
            {
                return new Point(-1, -1);
            }

            adornedElement = caret.AdornedElement;
            if (adornedElement == null)
            {
                throw new InvalidOperationException("There is a caret in the " +
                    Element + " control, but CaretElement.AdornedElement is null.");
            }
            
            //if (!(adornedElement is FlowDocumentScrollViewer || adornedElement is TextBlock))
            string adornedElementTypeName = adornedElement.GetType().FullName;
            if (!(IsFlowDocumentView(adornedElement) || adornedElement is FlowDocumentScrollViewer || adornedElement is TextBlock || adornedElementTypeName.Contains("TextBoxView")))
            {
                throw new InvalidOperationException("There is a caret in the " +
                    Element + " control, but CaretElement.AdornedElement is a " +
                    adornedElement.GetType().Name + " when TextFlow or " +
                    "TextBlock were expected.");
            }

            left = (double)ReflectionUtils.GetField(caret, "_left");
            top =(double) ReflectionUtils.GetField(caret, "_top");
            return new Point(left, top);
        }

        /// <summary>find the text that is before or after the caret</summary>
        /// <param name="direct"></param>
        /// <returns></returns>
        public string TextBeforeOrAfterCaret(LogicalDirection direct)
        {
            if (!this.SelectionInstance.IsEmpty)
            {
                throw new Exception("This Selection is not empty!!!");
            }
            TextPointer pointer = this.SelectionInstance.Start;
            return pointer.GetTextInRun(direct);
        }
        #endregion Caret support.

        #region Rich document support.

        /// <summary>Finds the first Table in the container.</summary>
        /// <returns>The first Table in the container, null if there are none.</returns>
        public Table FindTable()
        {
            return FindTable(Start, LogicalDirection.Forward);
        }

        /// <summary>Finds the first Table in the specified direction from a given position.</summary>
        /// <param name='position'>Position from which to start search.</param>
        /// <param name='direction'>Direction in which to search.</param>
        /// <returns>
        /// The first Table in the specified direction from a given position,
        /// null if there are none.
        /// </returns>
        public static Table FindTable(TextPointer position, LogicalDirection direction)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            while ((position.Parent != null) && position.Parent.GetType() != typeof(Table))
            {
                position = position.GetNextContextPosition(LogicalDirection.Forward);
                if (position == null)
                {
                    return null;
                }
            }

            return (Table) position.Parent;
        }

        /// <summary>Find the depth of a textpointer in a list.</summary>
        /// <param name="pointer"></param>
        /// <returns>0 mean not in the list. </returns>
        public int ListLevel(TextPointer pointer)
        {
            TextElement te = pointer.Parent as TextElement; ;
            int level = 0;
            do
            {
                if (te is System.Windows.Documents.List)
                    level++;
                te = te.Parent as TextElement;
            } while (te != null);
            return level;
        }

        #endregion Rich document support.

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// Gets the caret associated with the wrapped instance.
        /// </summary>
        public Adorner CaretElement
        {
            get
            {
                AdornerLayer layer;         // Layer hosting caret element.
                Adorner result;             // Resulting CaretElemet.

                // Use the control that render the text. For TextBox
                // and RichTextBox, this gets the inner adorner layer,
                // instead of the outer adorner layer around the control.
                layer = AdornerLayer.GetAdornerLayer(RenderingElement);
                if (layer == null)
                {
                    throw new InvalidOperationException(
                        "The rendering element has no adorner layer " +
                        "in which to find a caret.");
                }

                result = FindCaretElement(layer);
                if (result == null)
                {
                    throw new InvalidOperationException("No caret found.");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the UIElement associated with this wrapper instance.
        /// </summary>
        public UIElement Element
        {
            get { return this._element; }
        }

        /// <summary>
        /// Check to see if the caret is at the beginning of a ListItem.
        /// </summary>
        public bool IsCaretAtTheBeginningOfAListItem
        {
            get
            {
                return IsCaretAtListEdge(typeof(System.Windows.Documents.ListItem), LogicalDirection.Backward);
            }
        }

        /// <summary>
        /// Check to see if the caret is at the end of a list Items.
        /// </summary>
        public bool IsCaretAtTheEndOfAListItem
        {
            get
            {
                return IsCaretAtListEdge(typeof(System.Windows.Documents.ListItem), LogicalDirection.Forward);
            }
        }

        /// <summary>
        /// Check to see if the caret is at the Beginning of A List
        /// </summary>
        public bool IsCaretAtTheBeginningOfAList
        {
            get
            {
                return IsCaretAtListEdge(typeof(System.Windows.Documents.List), LogicalDirection.Backward);
            }
        }

        /// <summary>
        /// Check to see if the caret is at the End of a list.
        /// </summary>
        public bool IsCaretAtTheEndOfAList
        {
            get
            {
                return IsCaretAtListEdge(typeof(System.Windows.Documents.List), LogicalDirection.Forward);
            }
        }

        /// <summary>
        /// Check to see if the caret is before a Sub lists
        /// </summary>
        public bool IsCaretFollowedBySubList
        {
            get
            {
                TextPointer pointer = this.SelectionInstance.Start;

                //The pointer must be insde a list.
                if (this.ListLevel(pointer) > 0)
                {
                    //looking for the sub list tag.
                    while (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                    {
                        //we must have at least a  before the caret.
                        pointer = pointer.GetPositionAtOffset(1);
                        if (pointer != null && pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart &&
                            pointer.GetAdjacentElement(LogicalDirection.Forward) is System.Windows.Documents.List)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets whether the text editor is in Overtype (Ins) mode.
        /// </summary>
        public bool IsOvertypeModeEnabled
        {
            get
            {
                object textEditor;

                textEditor = TextEditor;
                if (textEditor == null)
                {
                    return false;
                }
                else
                {
                    return (bool)ReflectionUtils.GetField(TextEditor, "_overtypeMode");
                }
            }
        }

        /// <summary>
        /// Gets the UIElement that performs the actual rendering
        /// for the wrapped instance.
        /// </summary>
        public UIElement RenderingElement
        {
            get
            {
                Type elementType;

                elementType = Element.GetType();
                if (elementType == typeof(TextBlock) ||                    
                    elementType == typeof(FlowDocumentScrollViewer) ||
                    IsFlowDocumentView(elementType)
                )
                {
                    return Element;
                }
                else if (elementType == typeof(TextBox) ||
                         TypeName.IndexOf("RichTextBox") != -1 ||
                         TypeName.IndexOf("PasswordBox") != -1)
                {
                    return (UIElement)Test.Uis.Utils.ReflectionUtils.GetProperty(
                        this.Element, "RenderScope");
                }
                throw ANotSupportedException("RenderingElement");
            }
        }

        /// <summary>
        /// Gets the length of the selected length.
        /// </summary>
        public int SelectionLength
        {
            get
            {
                int selectionLength;

                // we can't do SelectedText.Length here
                // since SelectedText is not legal on PasswordBox.
                if (_element is TextBox)
                {
                    selectionLength = ((TextBox)_element).SelectionLength;
                }
                else if (_element is RichTextBox || _element is PasswordBox)
                {
                    selectionLength = SelectionInstance.Text.Length;
                }
                else
                {
                    throw ANotSupportedException("SelectionLength");
                }

                return selectionLength;
            }
        }

        /// <summary>
        /// Pointer to the moving edge of the wrapped element's selection.
        /// </summary>
        /// <remarks>May be null if SelectionInstance is null.</remarks>
        public TextPointer SelectionMovingPointer
        {
            get
            {
                if (_element is TextBoxBase)
                {
                    TextSelection selection;

                    selection = this.SelectionInstance;
                    if (selection == null)
                    {
                        return null;
                    }
                    else
                    {
                        return (TextPointer)
                            ReflectionUtils.GetProperty(selection, "MovingPosition");
                    }
                }
                else
                {
                    throw ANotSupportedException("SelectionMovingPointer");
                }
            }
        }

        /// <summary>Distance between start of container and moving pointer in selection.</summary>
        public int SelectionMovingPointerDistance
        {
            get
            {
                if (_element is TextBoxBase)
                {
                    return TextUtils.GetDistanceFromStart(SelectionMovingPointer);
                }
                else if (_element is PasswordBox)
                {
                    object selection;
                    object movingPosition;

                    selection = GetITextSelection();
                    if (selection == null)
                    {
                        return 0;
                    }
                    else
                    {
                        movingPosition = ReflectionUtils.GetInterfaceProperty(selection,
                            "ITextSelection", "MovingPosition");
                        return GetPasswordOffsetFromStart(movingPosition);
                    }
                }
                else
                {
                    throw ANotSupportedException("SelectionMovingPointerDistance");
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the starting point of text selected
        /// in the wrapped element.
        /// </summary>
        /// <value>The index of the starting position of text selected.</value>
        public int SelectionStart
        {
            get
            {
                int start;
                TextSelection textSelection;

                if (_element is TextBox)
                {
                    start = ((TextBox)_element).SelectionStart;
                }
                else if (_element is PasswordBox)
                {
                    object selection;
                    object selectionStart;

                    selection = ReflectionUtils.GetProperty(_element, "Selection");
                    selectionStart = ReflectionUtils.GetInterfaceProperty(selection, "ITextRange", "Start");
                    return GetPasswordOffsetFromStart(selectionStart);
                }
                else if (_element is RichTextBox)
                {
                    textSelection = SelectionInstance;

                    start = textSelection.Start.DocumentStart.GetOffsetToPosition(textSelection.Start);
                }
                else
                {
                    throw ANotSupportedException("SelectionStart");
                }

                return start;
            }
            set
            {
                if (_element is TextBox)
                {
                    ((TextBox)_element).SelectionStart = value;
                }
                else if (_element is PasswordBox)
                {
                    ReflectionUtils.InvokeInstanceMethod(_element,
                        "Select", new object[] { value, SelectionLength });
                }
                else if (_element is RichTextBox)
                {
                    TextPointer selectionStart;
                    TextPointer selectionEnd;
                    int selectionLength;

                    selectionLength = this.SelectionLength;
                    selectionStart = SelectionInstance.Start.DocumentStart;
                    selectionStart = selectionStart.GetPositionAtOffset(value);
                    selectionEnd = selectionStart;
                    selectionEnd = selectionEnd.GetPositionAtOffset(selectionLength);

                    SelectionInstance.Select(selectionStart, selectionEnd);
                }
                else
                {
                    throw ANotSupportedException("SelectionStart");
                }
            }
        }

        /// <summary>Text contents of the element.</summary>
        public string Text
        {
            set
            {
                if (_element is TextBox)
                {
                    ((TextBox)_element).Text = value;
                }
                else if (_element is RichTextBox)
                {
                    TextRange.Text = value;
                }
                else if (_element is PasswordBox)
                {
                    ((PasswordBox)_element).Password = value;
                }
                else if(IsFlowDocumentView(_element)) {
                    FlowDocument document = (FlowDocument)Test.Uis.Utils.ReflectionUtils.GetProperty(_element, "Document");
                    if(document == null) {
                        document = new FlowDocument();
                        Test.Uis.Utils.ReflectionUtils.SetProperty(this.Element, "Document", document);
                    }
                    new TextRange(document.ContentStart, document.ContentEnd).Text = value;
                }
                else if (_element is FlowDocumentScrollViewer)
                {
                    FlowDocumentScrollViewer tp = (FlowDocumentScrollViewer)_element;
                    new TextRange(tp.Document.ContentStart, tp.Document.ContentEnd).Text = value;
                }
                else if (_element is TextBlock)
                {
                    ((TextBlock)_element).Text = value;
                }
                else
                {
                    throw ANotSupportedException("Text");
                }
            }
            get
            {
                string text;

                text = String.Empty;
                if (_element is TextBox)
                {
                    text = ((TextBox)_element).Text;
                }
                else if (_element is RichTextBox)
                {
                    FlowDocument document = ((RichTextBox)_element).Document;
                    text = new TextRange(document.ContentStart, document.ContentEnd).Text;
                }
                else if (_element is PasswordBox)
                {
                    text = ((PasswordBox)_element).Password;
                }
                else if (_element is FlowDocumentScrollViewer)
                {
                    FlowDocumentScrollViewer tp = (FlowDocumentScrollViewer)_element;

                    text = (new TextRange(tp.Document.ContentStart, tp.Document.ContentEnd)).Text;
                }
                else if (_element is TextBlock)
                {
                    text = ((TextBlock)_element).Text;
                }

                if (text == null)
                {
                    throw ANotSupportedException("Text");
                }
                return text;
            }
        }

        /// <summary>Text that should be used to compute caret and word navigation.</summary>
        internal string TextForNavigation
        {
            get
            {
                if (this._element is TextBox)
                {
                    return ((TextBox)this._element).Text;
                }
                else if (this._element is PasswordBox)
                {
                    PasswordBox passwordBox = (PasswordBox)_element;
                    return new string(passwordBox.PasswordChar, passwordBox.Password.Length);
                }
                else
                {
                    throw ANotSupportedException("TextForNavigation");
                }
            }
        }

        /// <summary>
        /// Gets the TextEditor instance associated with the control,
        /// null if none is available.
        /// </summary>
        public object TextEditor
        {
            get
            {
                if (this._element is TextBoxBase)
                {
                    return ReflectionUtils.GetField(this._element, "_textEditor");
                }
                else if (this._element is PasswordBox)
                {
                    return ReflectionUtils.GetField(this._element, "_textEditor");
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public string TypeName
        {
            get { return _element.GetType().Name; }
        }


        #region TextBox OM wrappers

        /// <summary>
        /// Selection instance for the wrapped control, always non-null.
        /// </summary>
        /// <remarks>
        /// UIElement can be made TextEditing aware. TextBox and
        /// RichTextBox are by default are TextEditing aware, but you can
        /// also make, a button to be text editable if it supports the
        /// right interfaces.
        /// </remarks>
        public TextSelection SelectionInstance
        {
            get
            {
                TextSelection textSelection;

                textSelection = null;

                if (_element is TextBox)
                {
                    textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection((TextBox)_element);
                }

                if (_element is RichTextBox || _element is PasswordBox)
                {
                    textSelection=(TextSelection)Test.Uis.Utils.ReflectionUtils.GetProperty(_element, "Selection");
                }

                if (null == textSelection)
                {
                    throw ANotSupportedException("SelectionInstance");
                }

                return textSelection;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public TextRange TextRange
        {
            get
            {
                return new TextRange(Start, End);
            }
        }


        /// <summary>Start pointer of wrapped content.</summary>
        /// <remarks>This property is never null.</remarks>
        public TextPointer Start
        {
            get
            {
                TextPointer pointer;

                if (_element is FlowDocumentScrollViewer)
                {
                    pointer = ((FlowDocumentScrollViewer)_element).Document.ContentStart;
                }
                else if (_element is TextBlock)
                {
                    pointer = ((TextBlock)_element).ContentStart;
                }
                else if (_element is RichTextBox)
                {
                    pointer = ((RichTextBox)_element).Document.ContentStart;
                }
                else if (_element is FlowDocumentPageViewer)
                {
                    pointer = ((FlowDocument)((FlowDocumentPageViewer)_element).Document).ContentStart;
                }
                else if (_element is TextBox)
                {
                    object container;

                    container = TextContainer;
                    pointer = ReflectionUtils.GetProperty(container, "Start") as TextPointer;
                    if (pointer == null)
                    {
                        throw new Exception("Unable to get a non-null value for TextBox._textContainer.Start");
                    }
                }
                else
                {
                    throw ANotSupportedException("Start");
                }
                return pointer;
            }
        }

        /// <summary>End pointer of wrapped content.</summary>
        /// <remarks>This property is never null.</remarks>
        public TextPointer End
        {
            get
            {
                TextPointer pointer = null;

                if (_element is FlowDocumentScrollViewer)
                {
                    pointer = ((FlowDocumentScrollViewer)_element).Document.ContentEnd;
                }
                else if (_element is TextBlock)
                {
                    pointer = ((TextBlock)_element).ContentEnd;
                }
                else if (_element is RichTextBox)
                {
                    pointer = ((RichTextBox)_element).Document.ContentEnd;
                }
                else if (_element is TextBox)
                {
                    object container;

                    container = TextContainer;
                    pointer = ReflectionUtils.GetProperty(container, "End") as TextPointer;
                    if (pointer == null)
                    {
                        throw new Exception("Unable to get a non-null value for TextBox._textContainer.End");
                    }
                }
                else
                {
                    throw ANotSupportedException("End");
                }
                return pointer;
            }
        }

        /// <summary>
        /// check if the caret is at the beginning of the document
        /// </summary>
        public bool IsCaretAtBeginningOfDocument
        {
            get
            {
                TextPointer tp;
                tp = SelectionInstance.Start;
                while (tp.CompareTo(tp.DocumentStart) != 0)
                {
                    tp = tp.GetPositionAtOffset(-1);
                    if (tp.IsAtInsertionPosition)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// check if the caret is at the End of the document
        /// </summary>
        public bool IsCaretAtEndOfDocument
        {
            get
            {
                TextPointer tp;
                tp = SelectionInstance.Start;
                while (tp.CompareTo(tp.DocumentEnd) != 0)
                {
                    tp = tp.GetPositionAtOffset(1);
                    if (tp.IsAtInsertionPosition)
                        return false;
                }

                return true;
            }
        }

        /// <summary>Whether the edited element is a rich text element.</summary>
        public bool IsElementRichText
        {
            get
            {
                return (_element is RichTextBox ||
                        _element is FlowDocumentScrollViewer);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool IsSelectionAllowedOnThis
        {
            get
            {
                return (_element is TextBox || _element is RichTextBox || _element is PasswordBox);
            }
        }


        /// <summary>Whether the wrapped object supports a TOM API publicly.</summary>
        public bool IsPointerAllowedOnThis
        {
            get
            {
                return (_element is FlowDocumentScrollViewer || _element is TextBlock || _element is RichTextBox);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSelectionObjectAllowed
        {
            get { return IsSelectionAllowedOnThis && IsPointerAllowedOnThis; }
        }

        /// <summary>Whether text wraps or overflows the visible area.</summary>
        public bool Wrap
        {
            get
            {
                TextBox textBox;

                textBox = _element as TextBox;
                if (textBox != null)
                {
                    return (textBox.TextWrapping == TextWrapping.Wrap || textBox.TextWrapping == TextWrapping.WrapWithOverflow);
                }
                else
                {
                    return ((TextWrapping)_element.GetValue(TextBlock.TextWrappingProperty)) != TextWrapping.NoWrap;
                }
            }
            set
            {
                TextBox textBox;

                textBox = _element as TextBox;
                if (textBox != null)
                {
                    textBox.TextWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
                }
                else
                {
                    _element.SetValue(TextBlock.TextWrappingProperty,
                        value ? TextWrapping.Wrap: TextWrapping.NoWrap);
                }
            }
        }


        /// <summary>
        /// Gets or sets the contents of the encapsulated element in simple
        /// XAML format (without TextRange and start/end markers).
        /// </summary>
        public string XamlText
        {
            get
            {
                return XamlUtils.TextRange_GetXml(TextRange);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if ((_element is RichTextBox) || (_element is FlowDocumentScrollViewer) || (_element is TextBlock))
                {
                    TextRange.Text = String.Empty;
                    XamlUtils.SetXamlContent(TextRange, value);
                }
                else if (_element is TextBox)
                {
                    ((TextBox)_element).Text = value;
                }
                else
                {
                    throw new Exception("InvalidOperation on wrapper of element type: " + TypeName);
                }
            }
        }

        #endregion TextBox OM wrappers.

        #endregion Public properties.

        #region Private properties.

        /// <summary>TextContainer object; use only for TextBox.</summary>
        /// <remarks>This property is never null.</remarks>
        private object TextContainer
        {
            get
            {
                object result;

                result = ReflectionUtils.GetField(_element, "_textContainer");
                if (result == null)
                {
                    if (result == null)
                    {
                        throw new Exception("Unable to get a non-null value for _textContainer field.");
                    }
                }

                return result;
            }
        }

        #endregion Private properties.

        #region Private methods.

        /// <summary>
        /// Find the vertical and horizontal scale factors for different DPI setting.
        /// </summary>
        /// <param name="xFactor">float that returns the horizontal factor</param>
        /// <param name="yFactor">float that returns the vertical factor</param>
        /// <returns></returns>
         public static void HighDpiScaleFactors(out float xFactor, out float yFactor)
        {
            using (System.Drawing.Graphics gs = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                xFactor = gs.DpiX / 96;
                yFactor = gs.DpiY / 96;
            }
        }

        /// <summary>
        /// Creates a new NotSupportedException for the specified
        /// member.
        /// </summary>
        /// <example>The following sample shows how to use this method.<code>...
        /// public string PropertyName { get {
        ///   if (!(_element is ExpectedType))
        ///     throw ANotSupportedException("PropertyName");
        ///   ...
        /// } }</code></example>
        private Exception ANotSupportedException(string member)
        {
            return new NotSupportedException(member + " not supported for elements of type " + TypeName);
        }

        /// <summary>Gets the ITextSelection used on the current element.</summary>
        /// <returns>The ITextSelection used on the current element.</returns>
        private object GetITextSelection()
        {
            if (_element is PasswordBox)
            {
                return ReflectionUtils.GetProperty(_element, "Selection");
            }
            else
            {
                throw ANotSupportedException("GetITextSelection");
            }
        }

        /// <summary>
        /// Gets the offset of a PasswordBox position.
        /// </summary>
        /// <param name="position">Position to get offset for.</param>
        /// <returns>Offset from PasswordBox container start to the specified position.</returns>
        private int GetPasswordOffsetFromStart(object position)
        {
            object container;
            object containerStart;

            if (!(_element is PasswordBox))
            {
                throw new InvalidOperationException(
                    "Only PasswordBox is supported by GetPasswordOffsetFromStart.");
            }

            container = ReflectionUtils.GetField(_element, "_textContainer");
            containerStart = ReflectionUtils.GetProperty(container, "Start");

            return (int) ReflectionUtils.InvokeInterfaceMethod(
                containerStart, "ITextPointer", "GetOffsetToPosition", new object[] { position });
        }

        /// <summary>Gets the bounding rectangle for a text range.</summary>
        /// <param name='range'>Name to get bounds for.</param>
        /// <returns>Bounding rectangle for text range contents.</returns>
        private static Rect GetTextRangeBounds(TextRange range)
        {
            TextPointer rangeNavigator;   // Navigator moving across range.
            Rect result;                    // Results of operation.

            System.Diagnostics.Debug.Assert(range != null);

            // The range bounds are calculated by having a rectangle that
            // grows to encompass both sides of each character. This deals
            // with problematic BiDi cases.

            rangeNavigator = range.Start;
            result = rangeNavigator.GetCharacterRect(LogicalDirection.Backward);

            rangeNavigator = rangeNavigator.GetNextInsertionPosition(LogicalDirection.Forward);
            while (rangeNavigator.CompareTo(range.End) <= 0)
            {
                result.Union(rangeNavigator.GetCharacterRect(LogicalDirection.Backward));
                result.Union(rangeNavigator.GetCharacterRect(LogicalDirection.Forward));
                if ((rangeNavigator = rangeNavigator.GetNextInsertionPosition(LogicalDirection.Forward)) != null)
                {
                    break;
                }
            }

            return result;
        }
        /// <summary>
        /// find out if the caret is at a critical location of a list.
        /// </summary>
        /// <param name="listType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool IsCaretAtListEdge(Type listType, LogicalDirection direction)
        {
            if (!this.SelectionInstance.IsEmpty)
            {
                throw new Exception("Selection Is not empty and you don't have caret!!!");
            }
            TextPointer CaretLocation = this.SelectionInstance.Start;


            while (!(CaretLocation.GetPointerContext(direction) == TextPointerContext.EmbeddedElement
                   || CaretLocation.GetPointerContext(direction) == TextPointerContext.None
                   || CaretLocation.GetPointerContext(direction) == TextPointerContext.Text))
            {
                if (direction == LogicalDirection.Forward && CaretLocation.GetPointerContext(direction) == TextPointerContext.ElementStart)
                    break;
                if (direction == LogicalDirection.Backward && CaretLocation.GetPointerContext(direction) == TextPointerContext.ElementEnd)
                    break;
                if (CaretLocation.GetAdjacentElement(direction).GetType() == listType)
                {
                    return true;
                }
                CaretLocation = CaretLocation.GetPositionAtOffset((direction == LogicalDirection.Forward) ? 1 : -1);
            }
            return false;
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Reference to wrapped UIElement.</summary>
        private UIElement _element;

        #endregion Private fields.

        #region Nested types.

        /// <summary>
        /// SelectionDirection, used in SelectCharacterByMouse.
        /// </summary>
        public enum SelectionDirection
        {
            /// <summary>Selects from left to right.</summary>
            LeftToRight,
            /// <summary>Selects from right to left.</summary>
            RightToLeft
        }

        private delegate void MouseDragDelegate(Point startIndex, Point endIndex);

        #endregion Nested types.
    }
}