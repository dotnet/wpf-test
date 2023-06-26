// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides helpers for text manipulation and text editing.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Text.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;

    using Test.Uis.Data;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Provides static utility methods.</summary>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class TextUtils
    {
        #region Public methods.

        #region Random text creation.

        /// <remarks>Creates a random word.</remarks>
        /// <param name="iLength">Length of word to create.</param>
        /// <returns>A random word, with no word-breaking characters.</returns>
        public static string CreateWord(int iLength)
        {
            string _sEnglishAlpha = "abcdefghijklmnopqrstuvwxyz";
            string sTemp = "";
            Random randNum = new Random();
            char[] charAlpha = _sEnglishAlpha.ToCharArray();

            for(int i = 0; i < iLength; i++)
            {
                sTemp += charAlpha[randNum.Next(0, charAlpha.GetUpperBound(0))];
            }
            return sTemp;
        }

        /// <remarks>Creates a random sentence.</remarks>
        /// <param name="iLength">Length of sentence to create.</param>
        /// <returns>A random sentence, with word-breaking characters and finishing
        /// with a punctuation mark.</returns>
        public static string CreateSentence(int iLength)
        {
            string _sEndPunctuation = "!?.";
            string sTemp = "";
            Random randNum = new Random();
            char[] charSentencePunctuation = _sEndPunctuation.ToCharArray();

            for(int i = 0; i < iLength; i++)
            {
                sTemp += CreateWord(randNum.Next(1, 10)) + " ";
            }
            sTemp += charSentencePunctuation[randNum.Next(0, charSentencePunctuation.GetUpperBound(0))];
            return sTemp;
        }

        /// <remarks>Creates a random paragraph.</remarks>
        /// <param name="iLength">Length of paragraph to create.</param>
        /// <returns>A random paragraph, based on random sentence generation.</returns>
        public static string CreateParagraph(int iLength)
        {
            string sTemp = "";
            Random randNum = new Random();
            for(int i = 0; i < iLength; i++)
            {
                sTemp += CreateSentence(randNum.Next(1, 15)) + " ";
            }
            return sTemp;
        }

        #endregion Random text creation.

        #region String utilities.

        /// <summary>
        /// Returns true if the specified text contains
        /// punctuation character; false if it's empty,
        /// null, has no punctuation, or punctuation is
        /// last or first character.
        /// </summary>
        public static bool ContainsPunctuation(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsPunctuation(text[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the specified text ends with
        /// a punctuation character; false if it's empty,
        /// null or has a non-punctuation last character.
        /// </summary>
        public static bool EndsWithPunctuation(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsPunctuation(text[text.Length - 1]);
        }

        /// <summary>
        /// Returns true if the specified text ends with
        /// whitespace character; false if it's empty,
        /// null or has no whitespace last character.
        /// </summary>
        public static bool EndsWithWhitespace(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsWhiteSpace(text[text.Length - 1]);
        }

        /// <summary>
        /// Returns number of whitespace characters.
        /// </summary>
        public static int GetWhitespaceCount(string text)
        {
            return TextUtils.CountOccurencies(text, " ");
        }

        /// <summary>
        /// Returns true if the specified text ends with
        /// a non-whitespace or non-puntuation character;
        /// false if it's empty, null or has
        /// a whitespace or punctuation end character.
        /// </summary>
        public static bool EndsWithText(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsLetterOrDigit(text[text.Length - 1]);
        }

        /// <summary>
        /// Returns true if textAfter is immediately after the specified position
        /// in the given string.
        /// </summary>
        /// <param name="value">Value to look in.</param>
        /// <param name="position">Position to look in value.</param>
        /// <param name="textAfter">Text to match.</param>
        /// <returns>true if textAfter starts at value[position], false otherwise.</returns>
        private static bool HasTextAfter(string value, int position, string textAfter)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (textAfter == null)
            {
                throw new ArgumentNullException("textAfter");
            }

            if (position >= value.Length)
            {
                return false;
            }
            else
            {
                return value.Substring(position, textAfter.Length) == textAfter;
            }
        }

        /// <summary>
        /// Returns true if textBefore is immediately before the specified position
        /// in the given string.
        /// </summary>
        /// <param name="value">Value to look in.</param>
        /// <param name="position">Position to look in value.</param>
        /// <param name="textBefore">Text to match.</param>
        /// <returns>true if textBefore ends befoer value[position], false otherwise.</returns>
        private static bool HasTextBefore(string value, int position, string textBefore)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (textBefore == null)
            {
                throw new ArgumentNullException("textBefore");
            }

            if (position - textBefore.Length < 0)
            {
                return false;
            }
            else
            {
                return value.Substring(position - textBefore.Length, textBefore.Length) == textBefore;
            }
        }

        /// <summary>
        /// Normalizes end-of-lines to a single linefeed character,
        /// to mimick XML parsing behavior.
        /// </summary>
        /// <param name='text'>Text to normalize.</param>
        /// <returns>The normalized text.</returns>
        public static string NormalizeEndOfLines(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Replace CR/LF pairs with newlines, then standalone
            // CR characters with newlines.
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        /// <summary>
        /// Removes the base and combining characters at the specified index
        /// of the specified string.
        /// </summary>
        /// <param name="text">The string from which to remove the text element.</param>
        /// <param name="index">The zero-based index at which the text element starts.</param>
        /// <returns>A string without the text element at the specified index of the specified string.</returns>
        /// <remarks>
        /// The .NET Framework defines a text element as a unit of text that
        /// is displayed as a single character; that is, a grapheme. A text
        /// element can be a base character, a surrogate pair, or a combining
        /// character sequence.
        /// </remarks>
        public static string RemoveCombinedCharacters(string text, int index)
        {
            string element;

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            element = System.Globalization.StringInfo.GetNextTextElement(text, index);
            System.Diagnostics.Debug.Assert(element != null);

            return text.Remove(index, element.Length);
        }

        /// <summary>
        /// Returns the specified string a number of times.
        /// </summary>
        /// <param name='val'>Value to repeat.</param>
        /// <param name='repeatCount'>Number of times to repeat val.</param>
        /// <returns>The val value, concatenated repeatCount times.</returns>
        public static string RepeatString(string val, int repeatCount)
        {
            StringBuilder builder;

            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException("repeatCount",
                    repeatCount, "Repeat count cannot be negative.");
            }

            if (repeatCount == 0)
            {
                return String.Empty;
            }
            else if (repeatCount == 1)
            {
                return val;
            }
            else
            {
                builder = new StringBuilder(val.Length * repeatCount);
                do
                {
                    builder.Append(val);
                    repeatCount--;
                } while (repeatCount > 0);

                return builder.ToString();
            }
        }

        /// <summary>
        /// Returns true if the specified text starts with
        /// whitespace character; false if it's empty,
        /// null or has no whitespace first character.
        /// </summary>
        public static bool StartsWithWhitespace(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsWhiteSpace(text[0]);
        }

        /// <summary>
        /// Returns true if the specified text starts with
        /// a punctuation character; false if it's empty,
        /// null or has a non-punctuation first character.
        /// </summary>
        public static bool StartsWithPunctuation(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsPunctuation(text[0]);
        }

        /// <summary>
        /// Returns true if the specified text starts with
        /// a non-whitespace character; false if it's empty,
        /// null or has a whitespace first character.
        /// </summary>
        public static bool StartsWithText(string text)
        {
            if (text == null || text.Length == 0)
            {
                return false;
            }
            return Char.IsLetterOrDigit(text[0]);
        }

        #endregion String utilities.

        #region TextOM utilities.

        /// <summary>
        /// Counts the number of positions in the container that the
        /// specified pointer is in.
        /// </summary>
        /// <param name='containerPointer'>Pointer inside container to count.</param>
        /// <returns>
        /// The number of positions int he container that the specified
        /// pointer is in.
        /// </returns>
        public static int CountContainerPositions(TextPointer containerPointer)
        {
            if (containerPointer == null)
            {
                throw new ArgumentNullException("containerPointer");
            }

            return containerPointer.DocumentStart.GetOffsetToPosition(containerPointer.DocumentEnd);
        }

        /// <summary>
        /// Finds the element with the specified ID in the given context.
        /// </summary>
        /// <param name='start'>TextPointer location for searching.</param>
        /// <param name='id'>ID value of element sought.</param>
        /// <returns>
        /// The first element with the specified id value, null if not found.
        /// </returns>
        /// <remarks>
        /// If id is a null value or an empty string, the first UIElement
        /// found will be returned.
        /// </remarks>
        public static UIElement FindElementInText(TextPointer start, string id)
        {
            TextPointer navigator;
            LogicalDirection direction;
            int textLength;
            FrameworkElement element;
            TextPointerContext textPointerContext;
            bool done;

            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (id == null)
            {
                id = String.Empty;
            }

            done = false;
            textLength = 0;
            element = null;
            direction = LogicalDirection.Forward;
            navigator = start;

            // so this is the algorithm of navigating the text contents:
            // first get PointerContext, if this is None, we don't need to go any further, set done to true and leave.
            // if it is EmbeddedObject, this embedded object is the one we want either id == String.Empty or element ID
            // matches the one we are looking for. if so, set done = true and leave, otherwise, move on and continue searching.
            while (!done)
            {
                textPointerContext = navigator.GetPointerContext(LogicalDirection.Forward);

                switch (textPointerContext)
                {
                    case TextPointerContext.None:
                        done = true;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        element = navigator.GetAdjacentElement(direction) as FrameworkElement;
                        if (element != null && (id == String.Empty || element.Name == id))
                        {
                            done = true;
                        }
                        else
                        {
                            // the element is not the one we are looking for
                            // move on and see if we can find any
                            navigator = navigator.GetPositionAtOffset(1);
                        }
                        break;
                    default:
                        // GetTextLength returns 0 when context = ElementStart / ElementEnd, and
                        // we need to set that to 1 to move across the position.
                        textLength = navigator.GetTextRunLength(LogicalDirection.Forward);
                        textLength = Math.Max(1, textLength);
                        navigator = navigator.GetPositionAtOffset(textLength);
                        break;

                }
            }
            return element;
        }

        /// <summary>
        /// Finds a TextRange encompassing the specified text.
        /// </summary>
        /// <param name='startPosition'>Position to start search from.</param>
        /// <param name='text'>Text to look for.</param>
        /// <returns>
        /// A TextRange encompassing the specified text, null if
        /// not found.
        /// </returns>
        public static TextRange FindTextRangeWithText(TextPointer startPosition,
            string text)
        {
            TextPointer cursor;       // Cursor moving through text container.
            LogicalDirection dir;       // Direction in which cursor moves.
            TextPointerContext symbolType;  // Type of symbol in container.
            TextPointer start;
            TextPointer end;

            if (startPosition == null)
            {
                throw new ArgumentNullException("position");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            dir = LogicalDirection.Forward;
            cursor = startPosition;
            while ((symbolType = cursor.GetPointerContext(dir)) != TextPointerContext.None)
            {
                if (symbolType == TextPointerContext.Text)
                {
                    string textInRun;   // Text in a given run.
                    int indexInRun;     // Position of text in textInRun.

                    textInRun = cursor.GetTextInRun(dir);
                    indexInRun = textInRun.IndexOf(text);
                    if (indexInRun != -1)
                    {
                        start = cursor;
                        end = cursor;

                        start = start.GetNextContextPosition(dir);

                        end = end.GetPositionAtOffset(dir == LogicalDirection.Forward
                            ? indexInRun + text.Length
                            : (indexInRun + text.Length) * -1);

                        return new TextRange(start, end);
                    }
                    else
                    {
                        cursor = cursor.GetPositionAtOffset(dir == LogicalDirection.Forward
                            ? textInRun.Length
                            : textInRun.Length * -1);
                    }
                }
                else
                {
                    cursor = cursor.GetPositionAtOffset(dir == LogicalDirection.Forward
                        ? 1
                        : -1);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the number of positions between the given
        /// position and the start of its container.
        /// </summary>
        /// <param name='position'>Position to count from.</param>
        /// <returns>
        /// The number of positions between the given position and
        /// the start of the container.
        /// </returns>
        public static int GetDistanceFromStart(TextPointer position)
        {
            //TextPointer navigator;          // Navigator to get to start.
            //int result;                     // Positions moved.

            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            /*
            result = 0;
            navigator = position;
            while (navigator.GetPointerContext(LogicalDirection.Backward) != TextPointerContext.None)
            {
                result -= navigator.MoveByOffset(-1024);
            }
            return result;
             */
            //return position.GetOffsetToPosition(position.DocumentStart);
            return position.DocumentStart.GetOffsetToPosition(position);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static TextPointer GetTextBoxStart(TextBox textBox)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            return new UIElementWrapper(textBox).Start;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static TextPointer GetTextBoxEnd(TextBox textBox)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            return new UIElementWrapper(textBox).End;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="textbox"></param>
        /// <returns></returns>
        public static TextSelection GetTextBoxSelection(TextBox textbox)
        {
            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
            {
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            }
            return TextSelectionInfo.GetValue(textbox, null) as TextSelection;
        }

        #endregion TextOM utilities.

        /// <summary>
        /// Converts every character in the given string to an ANSI string by
        /// replacing other characters with a C-style escape sequence.
        /// </summary>
        /// <param name='text'>Text to convert.</param>
        /// <returns>The specified text as C-style escape sequences.</returns>
        public static string ConvertToAnsi(string text)
        {
            // Format string: \ + x + hex w/ 4 digits minimum
            int numericChar;
            StringBuilder builder;

            builder = new StringBuilder(text.Length * 6);
            for (int i = 0; i < text.Length; i++)
            {
                numericChar = (int)text[i];
                builder.Append(String.Format("\\x{0:x4}", numericChar));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the given string to a single-line ANSI string by
        /// replacing other characters with a C-style escape sequence.
        /// </summary>
        /// <param name='text'>Text to convert.</param>
        /// <returns>
        /// The specified text as a single line, with C-style escape sequences.
        /// </returns>
        public static string ConvertToSingleLineAnsi(string text)
        {
            return EscapeCharacterConverter.ConverToSingleLineAnsi(text);
        }

        /// <summary>
        /// Find the freuqency of a string
        /// </summary>
        /// <param name="MainString"></param>
        /// <param name="SubString"></param>
        /// <returns></returns>
        public static int CountOccurencies(string MainString, string SubString)
        {
            return MainString.Contains(SubString) ? CountOccurencies(MainString.Substring(MainString.IndexOf(SubString) + SubString.Length), SubString) + 1 : 0;
        }

        /// <summary>Reverses a string.</summary>
        /// <param name="sInput">String to reverse.</param>
        /// <returns>The given string with the characters reversed.</returns>
        public static string GetReverseString(string sInput)
        {
            char[] cInputArray = sInput.ToCharArray();
            string sOutput = null;
            System.Array.Reverse(cInputArray);
            for(int i = 0; i < cInputArray.Length; i++)
            {
                sOutput += cInputArray[i].ToString();
            }
            return sOutput;
        }

        /// <summary>
        /// Indents a string value with embedded newline characters.
        /// </summary>
        /// <param name="lines">Lines to indent.</param>
        /// <param name="indentation">Indentation to prefix to each line.</param>
        /// <returns>The lines indented with the indentation value.</returns>
        public static string IndentLines(string lines, string indentation)
        {
            string result;

            if (lines == null || lines.Length == 0)
            {
                return lines;
            }

            // Indent the first line unless it already starts with a newline.
            if (lines[0] == '\n')
                result = "";
            else
                result = indentation;

            // Indent all newlines.
            result += lines.Replace("\n", "\n" + indentation);

            return result;
        }

        /// <remarks>Processes C-style escaped chars in a string.</remarks>
        /// <param name="text">String to process.</param>
        /// <returns>
        /// The text parameter with \n, \r and other such
        /// escape sequences replaced.
        /// </returns>
        public static string ProcessCStyleEscapedChars(string text)
        {
            if (text == null)
                return null;
            return EscapeCharacterConverter.Convert(text);
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// The U+FFFC OBJECT REPLACEMENT CHARACTER is used
        /// as an insertion point for object located within
        /// a stream of text.
        /// </summary>
        /// <remarks>
        /// All other information about the object is kept
        /// outside the character data stream. Internally it
        /// is a dummy character that acts as an anchor point
        /// for the object's formatting information.
        /// </remarks>
        public const char ObjectReplacementCharacter = '\xFFFC';

        #endregion Public properties.


        #region Private methods.

        private static TextPointer InternalGetTextBox(TextBox textBox, string containerPropertyName)
        {
            object textContainer;

            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            PropertyInfo TextContainerInfo = typeof(TextBoxBase).GetProperty("TextContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextContainerInfo == null)
            {
                throw new Exception("TextBoxBase.TextContainer property is not accessible");
            }
            textContainer = TextContainerInfo.GetValue(textBox, null);
            PropertyInfo property = textContainer.GetType().GetProperty(containerPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new Exception("TextContainer.Start property is not accessible");
            }
            return (TextPointer)property.GetValue(textContainer, null);
        }

        #endregion Private methods.
    }

    /// <summary>Provides methods to find caret and word boundaries.</summary>
    static class TextAnalyzer
    {
        /// <summary>
        /// Creates a list of valid caret positions in the specified string.
        /// </summary>
        /// <param name="text">String of text to analyze.</param>
        /// <param name="cultureInfo">Culture for the string.</param>
        /// <param name="flowDirection">FlowDirection for the string.</param>
        /// <param name="fontFamily">Font family used to render the string.</param>
        /// <returns>A list of characters before which there are valid caret positions.</returns>
        internal static List<int> ListValidCaretPositions(string text,
            CultureInfo cultureInfo, FlowDirection flowDirection,
            System.Windows.Media.FontFamily fontFamily)
        {
            // NOTE: cultureInfo, direction and fontFamily are currently unused.
            // NOTE: this does not take into account layout-specific effects, like hyphenation or tab leader.

            List<int> result;
            int index;

            result = new List<int>(text.Length + 1);
            index = 0;

            while (index < text.Length)
            {
                char c;

                c = text[index];

                // Consider \r\n first.
                if (c == '\n')
                {
                    if (index == 0 || text[index - 1] != '\r')
                    {
                        result.Add(index);
                    }
                    index++;
                    continue;
                }

                // Consider surrogate pairs.
                if (Char.IsLowSurrogate(c))
                {
                    if (index == 0 || !Char.IsHighSurrogate(text[index - 1]))
                    {
                        result.Add(index);
                    }
                    index++;
                    continue;
                }

                // Skip combining characters for scripts that don't break in them.
                if (IsModifier(c) && !BreaksInsideModifiers(TextScript.GetCharacterScript(c)))
                {
                    index++;
                    continue;
                }

                // Consider everything else a caret position candidate.
                result.Add(index);
                index++;
            }

            // Add the trailing text.
            result.Add(text.Length);

            return result;
        }

        /// <summary>
        /// Checks whether the specified (possibly null) script
        /// uses caret breaks inside character modifiers.
        /// </summary>
        private static bool BreaksInsideModifiers(TextScript script)
        {
            // Thaana breaks in Wordpad and Word, but not in Notepad.
            return script != null && (
                script.Name == "Arabic" ||
                script.Name == "Syriac" ||
                script.Name == "Thaana");
        }

        /// <summary>
        /// Checks whether the specified character is a modifier.
        /// </summary>
        private static bool IsModifier(Char c)
        {
            UnicodeCategory category;

            category = Char.GetUnicodeCategory(c);
            return category == UnicodeCategory.ModifierLetter ||
                category == UnicodeCategory.ModifierSymbol ||
                category == UnicodeCategory.NonSpacingMark;
        }
    }

    /// <summary>Provides a configurable character escaping object.</summary>
    /// <remarks>
    /// This class can be used to convert strings with c-style escaped
    /// characters to string.
    /// </remarks>
    sealed class EscapeCharacterConverter
    {
        #region Constructors.

        /// <summary>Hides the constructor.</summary>
        private EscapeCharacterConverter() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Converts the given string by converting C-style escape
        /// sequences to their corresponding characters.
        /// </summary>
        public static string Convert(string text)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                StringBuilder sb = new StringBuilder(text.Length + 4);
                int i = 0;
                while (i < text.Length)
                {
                    char c = text[i];
                    if (c == '\\')
                    {
                        // Process escape code.
                        int value;
                        i++;
                        c = text[i];
                        switch (c)
                        {
                            case '\'':
                                sb.Append('\'');    // single quote
                                break;
                            case '\"':
                                sb.Append('\"');    // double quote
                                break;
                            case '\\':
                                sb.Append('\\');    // backslash
                                break;
                            case '0':
                                sb.Append('\0');    // null
                                break;
                            case 'a':
                                sb.Append('\a');    // alert
                                break;
                            case 'b':
                                sb.Append('\b');    // backspace
                                break;
                            case 'f':
                                sb.Append('\f');    // form feed
                                break;
                            case 'n':
                                sb.Append('\n');    // new line
                                break;
                            case 'r':
                                sb.Append('\r');    // carriage return
                                break;
                            case 't':
                                sb.Append('\t');    // horizontal tab
                                break;
                            case 'v':
                                sb.Append('\v');    // vertical tab
                                break;
                            case 'x':              // hexadecimal escape sequence
                                i++;
                                value = ParseHex(text, ref i);
                                sb.Append(System.Convert.ToChar(value));
                                break;
                            case 'u':              // short unicode escape
                                i++;
                                value = ParseHex(text, ref i);
                                sb.Append(System.Convert.ToChar(value));
                                i--;
                                break;
                            case 'U':
                                throw new NotImplementedException();
                        }
                        i++;
                    }
                    else
                    {
                        sb.Append(c);
                        i++;
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Converts the given string to a single-line ANSI string by
        /// replacing other characters with a C-style escape sequence.
        /// </summary>
        public static string ConverToSingleLineAnsi(string text)
        {
            if (text == null) return null;
            if (text == String.Empty) return String.Empty;

            StringBuilder sb = new StringBuilder(text.Length * 4);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                //
                // Allow a few simple characters to pass through, escape
                // everything else.
                //
                if ((c >= 'A' && c <='Z') ||
                    (c >= 'a' && c <='z') ||
                    (c >= '0' && c <='9') ||
                    (c == ' ' || c == '.' || c == ',' || c == '-'))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(ConvertToVisibleAnsi(c));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string representation of the specified character that
        /// can render in any ANSI code page, without the user of control
        /// character.
        /// </summary>
        public static string ConvertToVisibleAnsi(char c)
        {
            // Allow a few simple characters to pass through, escape
            // everything else.
            if ((c >= 'A' && c <='Z') ||
                (c >= 'a' && c <='z') ||
                (c >= '0' && c <='9'))
            {
                return c.ToString();
            }
            else
            {
                // Format string: \ + x + hex w/ 4 digits minimum
                int numericChar = (int) c;
                return String.Format("\\x{0:x4}", numericChar);
            }
        }

        #endregion Public methods.

        #region Private methods.

        /// <summary>
        /// Parses up to four characters for a hex string, assuming the
        /// first is a valid character.
        /// </summary>
        private static int ParseHex(string text, ref int index)
        {
            StringBuilder sb = new StringBuilder(4);
            int charsProcessed = 0;
            while (charsProcessed < 4 && index < text.Length)
            {
                char c = text[index];
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F'))
                {
                    sb.Append(c);
                    charsProcessed++;
                    index++;
                }
                else
                {
                    // We've gone past into something that is not part of
                    // the hex string.
                    index--;
                    break;
                }
            }
            if (charsProcessed == 0)
            {
                throw new ArgumentException(
                    "Text has invalid hex escape code at index " + index);
            }
            return int.Parse(sb.ToString(),
                System.Globalization.NumberStyles.HexNumber);
        }

        #endregion Private methods.
    }
}
