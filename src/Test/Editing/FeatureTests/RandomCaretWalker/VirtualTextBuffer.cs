// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  This file contains 4 classes:                                                       //
//  VirtualTextBufferChar: representation of a single character in the buffer           //
//  VirtualTextBufferPosition: representation of the position between each character    //
//  VirtualTextBuffer: An ArrayList of VirtualTextBufferChar                            //
//  VirtualTextEditor: logic to handle moving up / down / left / right and selection    //

namespace Test.Uis.TextEditing
{
    
    #region Namespace
    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Collections;
    using System.Windows;
    using System.Windows.Documents;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    #endregion

    // Relationship of the classes 
    // VirtualTextBufferChar index:       0 1 2 3 4 5 6 7 8
    // VirtualTextBuffer:                |T|h|i|s| |i|s| |a| 
    // VirtualTextBufferPosition index:  0 1 2 3 4 5 6 7 8 9
    
    /// <summary>
    /// Class representing a single character. The hashtable can be used 
    /// to support fomatted text
    /// </summary>
    internal class VirtualTextBufferChar : ICloneable
    {
        /// <summary>
        /// Create VirtualTextBufferChar given a character value
        /// </summary>
        /// <param name="value"></param>
        internal VirtualTextBufferChar(char value)
        {
            _value = value;
            _dependencyPropertyHash = new Hashtable();
        }

        /// <summary>
        /// Remember this method is different from the ctor
        /// because it copies the associated properties as well.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            VirtualTextBufferChar newChar = new VirtualTextBufferChar(_value);

            ICollection collection = _dependencyPropertyHash.Keys;

            IEnumerator enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {                
                newChar._dependencyPropertyHash.Add(enumerator.Current,
                    _dependencyPropertyHash[enumerator.Current]);
            }

            return newChar;
        }

        /// <summary>
        /// Apply dependency property on this VirtualTextBufferChar
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        internal void ApplyProperty(DependencyProperty property, object value)
        {
            if (!property.IsValidValue(value))
            {
                string message = String.Format("[0] can't be applied for property [1]",
                    value.ToString(),
                    property.ToString());

                throw new InvalidOperationException(message);
            }
            if (_dependencyPropertyHash.ContainsKey(property))
            {
                _dependencyPropertyHash[property] = value;
            }
            else
            {
                _dependencyPropertyHash.Add(property, value);
            }
        }

        /// <summary>
        /// Return the char value associated with VirtualTextBufferChar
        /// </summary>
        /// <value></value>
        internal char Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Dump the internal char value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "VirtualTextBufferChar [" + _value + "]";
        }

        private char _value;
        private Hashtable _dependencyPropertyHash;
    }

    /// <summary>
    /// Class maintains an ArrayList to hold VirtualTextBufferChar
    /// </summary>
    internal class VirtualTextBuffer
    {
        internal VirtualTextBuffer(string str)
        {
            _lineContent = new ArrayList();
            Fill(str);
        }

        internal VirtualTextBuffer() : this(String.Empty)
        {
        }

        /// <summary>
        /// Fill the list with content
        /// </summary>
        /// <param name="str"></param>
        internal void Fill(string str)
        {
            if (_lineContent == null)
            {
                _lineContent = new ArrayList();
            }

            for (int i = 0; i < str.Length; i++)
            {
                _lineContent.Add(new VirtualTextBufferChar(str[i]));
            }
        }

        /// <summary>
        /// Retrive index of the given VirtualTextBufferChar reference
        /// It returns -1 when there's no such VirtualTextBufferChar found
        /// </summary>
        /// <param name="bufferChar"></param>
        /// <returns></returns>
        internal int GetIndexOfChar(VirtualTextBufferChar bufferChar)
        {
            for (int i = 0; i < Length; i++)
            {
                if ((VirtualTextBufferChar)_lineContent[i] == bufferChar)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove VirtualTextBufferChar from the internal ArrayList
        /// </summary>
        /// <param name="startOffset">offset of where characters should be deleted</param>
        /// <param name="count">number of characters to be deleted</param>
        internal void RemoveRange(int startOffset, 
            int count)
        {
            if (startOffset < 0 || startOffset + count > Length)
            {
                throw new IndexOutOfRangeException(ExceptionTextHelper.OutOfBufferRange);
            }

            _lineContent.RemoveRange(startOffset, count);
        }

        /// <summary>
        /// Insert character at index. A VirtualTextBufferChar instance is created
        /// based on the supplied value char c, and inserted in the ArrayList at index
        /// </summary>
        /// <param name="index">index of the new character</param>
        /// <param name="c">value of the char value in the new VirtualTextBufferChar</param>
        internal void InsertCharAt(int index,
            char c)
        {
            VirtualTextBufferChar virtualChar = new VirtualTextBufferChar(c);

            InsertCharAt(index, virtualChar);
        }

        /// <summary>
        /// overload to InsertCharAt(int index, char c)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="virtualChar"></param>
        internal void InsertCharAt(int index,
            VirtualTextBufferChar virtualChar)
        {
            _lineContent.Insert(index, virtualChar);
        }

        /// <summary>
        /// Length of the buffer
        /// </summary>
        /// <value></value>
        internal int Length
        {
            get { return _lineContent.Count; }
        }

        /// <summary>
        /// Returns the VirtualTextBufferChar reference given the index
        /// so GetIndexOf(this[x]) == x should always be true
        /// </summary>
        /// <param name="index">Index of VirtualTextBufferChar</param>
        /// <returns>VirtualTextBufferChar reference</returns>
        internal VirtualTextBufferChar this[int index]
        {
            get 
            {
                return (VirtualTextBufferChar)_lineContent[index];
            }

        }

        /// <summary>
        /// Get raw string with start index and count
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal string GetString(int start,
            int count)
        {
            StringBuilder sb = new StringBuilder();

            if (start < 0 || start + count > Length)
            {
                throw new IndexOutOfRangeException(ExceptionTextHelper.OutOfBufferRange);
            }

            for (int i = start; i < start + count; i++)
            {
                sb.Append(this[i].Value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get raw string before / after start inclusive when direction is Backward / Forward
        /// respectively.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal string GetString(int start, 
            LogicalDirection direction)
        {
            return direction == LogicalDirection.Forward
                ? GetString(start, Length - start)
                : GetString(0, start + 1);
        }

        /// <summary>
        /// Find the occurance of searchStr starting at index at direction
        /// if the occurance is not found, the function returns -1, otherwise the 
        /// index of the character at the beginning of the pattern
        /// </summary>
        /// <param name="index"></param>
        /// <param name="searchStr"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal int FindOccurance(int index,
            string searchStr, 
            LogicalDirection direction)
        {
            if (direction == LogicalDirection.Forward)
            {
                for (int i = index; i < Length - searchStr.Length + 1; i++)
                {
                    if (GetString(i, searchStr.Length) == searchStr)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = index - searchStr.Length + 1; i >= 0; i--)
                {
                    if (GetString(i, searchStr.Length) == searchStr)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private ArrayList _lineContent;
    }

    /// <summary>
    /// VirtualTextBufferPosition represents a location between each of the character.
    /// Relationship of the classes 
    /// VirtualTextBufferChar index:       0 1 2 3 4 5 6 7 8
    /// VirtualTextBuffer:                |T|h|i|s| |i|s| |a| 
    /// VirtualTextBufferPosition index:  0 1 2 3 4 5 6 7 8 9
    /// 
    /// so the starting position of the buffer has an offset of 0.
    /// This class also exposes operations to move VirtualTextBufferPosition around.
    /// For example: One can call MoveBySymbolDistance to move the position to next 
    /// symbol position (which is NewLine-character-aware)
    /// 
    /// Caveats: I didn't implement any reference counting like the mechanism in the
    /// product code TextPointer, and I didn't keep a list of existing VirtualTextBufferPositions
    /// Removing charactes from the buffer will *NOT* adjust existing VirtualTextBufferPosition
    /// offset except thoses involved in the removal process. See RemoveCharacters for details
    /// </summary>
    internal class VirtualTextBufferPosition : ICloneable
    {
        private static string[] s_specialSymbols = {
            Environment.NewLine
        };

        /// <summary>
        /// store a reference to VirtualTextBuffer
        /// </summary>
        /// <param name="buffer"></param>
        internal VirtualTextBufferPosition(VirtualTextBuffer buffer)
        {
            _buffer = buffer;
            _offset = 0;
        }

        /// <summary>
        /// Retrieve text at backward / forward direction of the current position
        /// This is also useful in debugging
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal string GetString(LogicalDirection direction)
        {
            return GetString(direction, null);
        }

        /// <summary>
        /// an overload of GetString which can take a limit VirtualTextBufferPosition
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        internal string GetString(LogicalDirection direction,
            VirtualTextBufferPosition limit /*can be null */)
        {
            // make sure the _offset in this position is valid
            VerifyPosition();

            // if there's a limit VirtualTextBufferPosition supplied
            if (limit != null)
            {
                // make sure the relative position of limit and current position
                // consistent with direction supplied
                if ((direction == LogicalDirection.Backward && limit > this)
                    || (direction == LogicalDirection.Forward && limit < this))
                {
                    throw new InvalidOperationException(ExceptionTextHelper.PositionNotConsistentWithDirection);
                }

                // return the string by calling VirtualTextBuffer
                return direction == LogicalDirection.Forward
                    ? _buffer.GetString(_offset, limit.Offset - _offset)
                    : _buffer.GetString(limit.Offset, _offset - limit.Offset);
            }

            // if we are at the start or end of the buffer, return empty string right away
            if (IsStartOrEndOfText(direction))
            {
                return String.Empty;
            }

            // call VirtualTextBuffer.GetString to return the desired string
            return direction == LogicalDirection.Forward
                ? _buffer.GetString(_offset, direction)
                : _buffer.GetString(_offset - 1, direction);
        }

        /// <summary>
        /// Remove characters starting at the position pointed
        /// by this VirtualTextBufferPosition
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="requestedCount"></param>
        /// <returns></returns>
        internal int RemoveCharacters(LogicalDirection direction,
            int requestedCount)
        {
            VerifyPosition();
            // if we don't have that many characters to delete,
            // delete all the characters we have in that direction
            // and we reuse the parameter requestedCount here
            if (requestedCount > GetString(direction).Length)
            {
                requestedCount = GetString(direction).Length;
            }

            // if direction == forward we remove the characters right away,
            // otherwise we need to calculate the starting index
            if (direction == LogicalDirection.Forward)
            {
                _buffer.RemoveRange(_offset, requestedCount);
            }
            else
            {
                int startOffset = _offset - requestedCount + 1;
                _buffer.RemoveRange(startOffset, requestedCount);
                _offset = startOffset;
            }

            return requestedCount;
        }

        
        /// <summary>
        /// Remove character in-between two supplied VirtualTextBufferPositions
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        internal static int RemoveCharacters(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            position1.VerifyPosition();
            position2.VerifyPosition();

            VirtualTextBufferPosition startPosition;

            startPosition = position1 > position2 
                ? position2
                : position1;

            int numberOfChars = GetCharacterDistanceBetweenTwoPositions(position1, position2);

            // we need absolute value
            if (numberOfChars < 0)
            {
                numberOfChars *= -1;
            }

            Debug.Assert(startPosition.RemoveCharacters(LogicalDirection.Forward,
                numberOfChars) == numberOfChars);

            // adjust the two parameters
            position1 = (startPosition);
            position2 = (startPosition);

            return numberOfChars;
        }

        /// <summary>
        /// return raw content between two VirtualTextBufferPositions
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        internal static string GetStringBetweenTwoPositions(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            position1.VerifyPosition();
            position2.VerifyPosition();
            return position1 > position2
                ? position1.GetString(LogicalDirection.Backward, position2)
                : position1.GetString(LogicalDirection.Forward, position2);
        }

        /// <summary>
        /// Insert one char at current position
        /// </summary>
        /// <param name="c"></param>
        internal void InsertChar(char c)
        {
            VerifyPosition();
            _buffer.InsertCharAt(_offset, c);
        }


        /// <summary>
        /// Retrieve reference to VirtualTextBufferChar at direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal VirtualTextBufferChar GetCurrentChar(LogicalDirection direction)
        {
            VerifyPosition();
            if (IsStartOrEndOfText(direction))
            {
                return null;
            }

            return direction == LogicalDirection.Forward
                ? _buffer[_offset]
                : _buffer[_offset - 1];
        }

        /// <summary>
        /// Return next symbol offset at current position
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal int GetNextSymbolOffset(LogicalDirection direction)
        {
            VerifyPosition();

            if (IsStartOrEndOfText(direction))
            {
                return 0;
            }

            int startIndex = direction == LogicalDirection.Forward
                ? _offset
                : _offset - 1;

            int offset = 0;

            foreach(string symbol in s_specialSymbols)
            {
                offset = _buffer.FindOccurance(startIndex, 
                    symbol,
                    direction);

                // if we don't find anything for the current symbol, try the next one.
                if (offset < 0)
                {
                    continue;
                }

                if ((offset == startIndex && direction == LogicalDirection.Forward)
                    || (offset + symbol.Length - 1 == startIndex && direction == LogicalDirection.Backward))
                {
                    return direction == LogicalDirection.Forward
                        ? (offset - startIndex + symbol.Length)
                        // why -1?
                        // remember this is negative when direction == Backward
                        // -1 means we need absolute distance growing by 1
                        : (offset - startIndex - 1);
                }
            }

            return direction == LogicalDirection.Forward
                ? 1
                : -1;
        }

        /// <summary>
        /// This method moves this position for one symbol distance according to direction
        /// the return value is the number of *characters* that has been moved in this 
        /// operation (e.g. calling MoveToNextSymbol(Forward) if the next symbol is NewLine
        /// will return 2)
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal int MoveToNextSymbol(LogicalDirection direction)
        {
            VerifyPosition();
            int charMoved = GetNextSymbolOffset(direction);

            _offset += charMoved;

            return charMoved;
        }

        /// <summary>
        /// This method moves current position regardless of what hte next codepoint is.
        /// For example, it needs 2 MoveToNextCharacter calls to move over the NewLine character
        /// The return value is either -1 (move backward by 1), 0 (no move)or 1 (move forward
        /// by one character)
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal int MoveToNextCharacter(LogicalDirection direction)
        {
            VerifyPosition();

            int moved = 0;

            if (IsStartOrEndOfText(direction))
            {
                return moved;
            }

            moved = direction == LogicalDirection.Forward
                ? 1
                : -1;

            _offset += moved;
            return moved;
        }

        /// <summary>
        /// repeatedly move current position as specified by delta
        /// if delta is negative the position moves backward
        /// otherwise forward. The return value is the number
        /// of moves done.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        internal int MoveByCharacterDistance(int delta)
        {
            int i = 0;
            VerifyPosition();
            if (delta > 0)
            {
                for (i = 0; i < delta; i++)
                {
                    if (MoveToNextCharacter(LogicalDirection.Forward) == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (i = delta; i < 0; i++)
                {
                    if (MoveToNextCharacter(LogicalDirection.Backward) == 0)
                    {
                        break;
                    }
                }

            }
            return i;
        }

        /// <summary>
        /// Move to the number of SYMBOLS as specified by parameter delta
        /// </summary>
        /// <param name="delta">Number of symbols to move accross (usually this is equal to number of left / right keys)</param>
        /// <returns>number of CHARACTERS moved</returns>
        internal int MoveBySymbolDistance(int delta)
        {
            int i;
            int currentDisplacement = 0;
            int totalDisplacement = 0;
            VerifyPosition();
            if (delta > 0)
            {
                for (i = 0; i < delta; i++)
                {
                    currentDisplacement = MoveToNextSymbol(LogicalDirection.Forward);
                    totalDisplacement += currentDisplacement;
                    if (currentDisplacement == 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (i = delta; i < 0; i++)
                {
                    currentDisplacement = MoveToNextSymbol(LogicalDirection.Backward);
                    totalDisplacement += currentDisplacement;
                    if (currentDisplacement == 0)
                    {
                        break;
                    }
                }
            }
            return totalDisplacement;
        }

        /// <summary>
        /// Move current position to the location specified by position argument
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal int MoveToPosition(VirtualTextBufferPosition position)
        {
            position.VerifyPosition();
            int distance = _offset - position.Offset;
            Debug.Assert(_buffer == position._buffer);
            _offset = position.Offset;
            return distance;
        }

        internal int MoveToEndOfLine(ref bool isLastLine)
        {
            VerifyPosition();

            int distance = GetDistanceFromNewLine(LogicalDirection.Forward, ref isLastLine);

            Debug.Assert(distance >= 0);
            return MoveBySymbolDistance(distance);
        }

        internal string GetCurrentLine()
        {
            bool isFirstLine;
            bool isLastLine;

            isFirstLine = false;
            isLastLine = false;

            VirtualTextBufferPosition startOfLine = (VirtualTextBufferPosition)Clone();
            VirtualTextBufferPosition endOfLine = (VirtualTextBufferPosition)Clone();

            startOfLine.MoveToStartOfLine(ref isFirstLine);
            endOfLine.MoveToEndOfLine(ref isLastLine);

            Debug.Assert(startOfLine <= endOfLine);

            return GetStringBetweenTwoPositions(startOfLine, endOfLine);
        }

        internal int MoveByLine(int delta, bool respectRememberedX, int rememberedX)
        {
            int lineMoved;
            VirtualTextBufferPosition position;
            int offset;
            int temp;
            bool isAtFirstLine;
            bool isAtLastLine;

            lineMoved = 0;
            position = (VirtualTextBufferPosition)Clone();
            isAtFirstLine = false;
            isAtLastLine = false;

            if (delta < 0)
            {
                // moving up.
                for (int i = delta; i < 0; i++)
                {
                    offset = position.GetDistanceFromNewLine(LogicalDirection.Backward,
                        ref isAtFirstLine);

                    // bail out if we are at the first line, 
                    // else do the hardwork to adjust the position
                    if (isAtFirstLine)
                    {
                        break;
                    }
                    else
                    {
                        position.MoveToStartOfLine(ref isAtFirstLine);
                        temp = position.MoveBySymbolDistance(-1);

                        // there should exist the newline character
                        Debug.Assert(temp == Environment.NewLine.Length * -1);

                        // respect rememberedX if that's specified in the argument
                        offset = position.GetDistanceFromNewLine(LogicalDirection.Backward,
                            ref isAtFirstLine);

                        if (respectRememberedX
                            && offset > rememberedX)
                        {
                            // rememberedX - offset is non-positive
                            position.MoveBySymbolDistance(rememberedX - offset);
                        }
                        lineMoved++;
                    }
                }
            }
            else
            {
                // moving down
                for (int i = 0; i < delta; i++)
                {
                    offset = position.GetDistanceFromNewLine(LogicalDirection.Forward,
                        ref isAtLastLine);

                    if (isAtLastLine)
                    {
                        break;
                    }
                    else
                    {
                        position.MoveToEndOfLine(ref isAtLastLine);
                        temp = position.MoveBySymbolDistance(1);
                        Debug.Assert(temp == Environment.NewLine.Length);

                        // respect rememberedX if that's specified in the argument
                        offset = position.GetDistanceFromNewLine(LogicalDirection.Forward,
                            ref isAtLastLine);

                        if (respectRememberedX)
                        {
                            if (offset < rememberedX)
                            {
                                position.MoveBySymbolDistance(offset);
                            }
                            else
                            {
                                position.MoveBySymbolDistance(rememberedX);
                            }
                        }
                        lineMoved++;

                    }
                }
            }

            MoveToPosition(position);

            return lineMoved;
        }

        internal int MoveToStartOfLine(ref bool isFirstLine)
        {
            VerifyPosition();

            int distance = GetDistanceFromNewLine(LogicalDirection.Backward,
                ref isFirstLine);

            Debug.Assert(distance >= 0);

            return MoveBySymbolDistance(distance * -1);
        }

        /// <summary>
        ///  Returns the non-negative number of characters from the last / next new line character
        /// if there's no newline char found in that direction the position is at the first or last
        /// line of the text, in which case it will return the distance to the start / end
        /// of the buffer, and set isFirstOrLastLine to false
        /// for example:
        /// This is a test\r\nThis is the second line\r\nThis is the third line
        ///         |                |
        ///         |                |
        ///      position2        position1
        /// 
        /// position1.GetDistanceFromNewLine(LogicalDirection.Forward, ref hitsStartOrEndOfText) returns 16, with hitsStartOrEndOfText equals false
        /// position1.GetDistanceFromNewLine(LogicalDirection.Backward, ref hitsStartOrEndOfText returns 7, with hitsStartOrEndOfText equals false
        /// position2.GetDistanceFromNewLine(LogicalDirection.Forward, ref hitsStartOrEndOfText) returns 6, with hitsStartOrEndOfText equals false
        /// position2.GetDistanceFromNewLine(LogicalDirection.Backward, ref hitsStartOrEndOfText) returns 8, with hitsStartOrEndOfText equals true
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="hitsStartOrEndOfText"></param>
        /// <returns></returns>
        internal int GetDistanceFromNewLine(LogicalDirection direction,
            ref bool hitsStartOrEndOfText)
        {
            VerifyPosition();
            hitsStartOrEndOfText = false;

            if (IsStartOrEndOfText(direction))
            {
                hitsStartOrEndOfText = true;
                return 0;
            }

            int startIndex = direction == LogicalDirection.Forward
                ? _offset
                : _offset - 1;

            int offset = 0;

            offset = _buffer.FindOccurance(startIndex,
                Environment.NewLine,
                direction);

            // cannot find new line character
            // in that direction.
            if (offset < 0)
            {
                hitsStartOrEndOfText = true;
                return direction == LogicalDirection.Forward
                    ? _buffer.Length - _offset
                    : _offset;
            }

            offset = direction == LogicalDirection.Forward
                ? offset - _offset
                : _offset - offset - Environment.NewLine.Length;

            return offset;
        }
        
        /// <summary>
        /// Get number of characters between two VirtualTextBufferPositions
        /// if position1 is logically after position2 the return value is non-negative
        /// else otherwise
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        internal static int GetCharacterDistanceBetweenTwoPositions(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            return position1.Offset - position2.Offset;
        }

        /// <summary>
        /// Debug method to Assert when it detects problems with VirtualTextBufferPosition
        /// </summary>
        private void VerifyPosition()
        {
            Debug.Assert(_offset > -1 && _offset < _buffer.Length + 1);
        }

        /// <summary>
        /// Returns true if current position with specified direction is start / end of text buffer
        /// false otherwise
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool IsStartOrEndOfText(LogicalDirection direction)
        {
            return ((_offset == 0 && direction == LogicalDirection.Backward)
                || (_offset == _buffer.Length && direction == LogicalDirection.Forward));
        }

        /// <summary>
        /// Returns true if current position is logically before a NewLine sequence, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsAtEndOfLine()
        {
            return (GetString(LogicalDirection.Forward).StartsWith(Environment.NewLine)
                || _offset == _buffer.Length);
        }

        /// <summary>
        /// Returns true if current position is logically after a NewLine sequence, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsAtStartOfLine()
        {
            return (GetString(LogicalDirection.Backward).EndsWith(Environment.NewLine)
                || _offset == 0);
        }

        /// <summary>
        /// Return true if position1 is logically after position2 (i.e. position1 has 
        /// a larger offset)
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public static bool operator >(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            return position1.Offset > position2.Offset;
        }

        /// <summary>
        /// Overload
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public static bool operator >=(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            return position1.Offset >= position2.Offset;
        }

        /// <summary>
        /// Overload
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public static bool operator <(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            return position1.Offset < position2.Offset;
        }

        /// <summary>
        /// overload
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <returns></returns>
        public static bool operator <=(VirtualTextBufferPosition position1,
            VirtualTextBufferPosition position2)
        {
            return position1.Offset <= position2.Offset;
        }

        /// <summary>
        ///  return _offset
        /// </summary>
        /// <value></value>
        internal int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// Clone this position. The return value is a new instance of this position
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            VirtualTextBufferPosition newPosition = new VirtualTextBufferPosition(_buffer);
            newPosition._offset = _offset;
            return newPosition;
        }

        private int _offset;
        private VirtualTextBuffer _buffer;
    }

    /// <summary>
    /// Exception string helper class
    /// </summary>
    internal class ExceptionTextHelper
    {
        internal static string DifferentBuffer 
        {
            get
            {
                return "The two buffer reference doesn't appear to point to the same object";
            }
        }
        internal static string OutOfBufferRange
        {
            get
            {
                return "Index is out of the range of the buffer";
            }
        }
        internal static string PositionNotConsistentWithDirection
        {
            get
            {
                return "Limit position specified is not consistent with direction parameter specified";
            }
        }
    }

    /// <summary>
    /// The virtual editor. This class has all the logic to move caret / selection around.
    /// Selection is represented by two VirtualTextBufferPosition _selectionStart and _selectionEnd
    /// It has also maintained _rememberedX so that last X value can be restored on Up / Down 
    /// key navigation
    /// </summary>
    internal class VirtualTextEditor
    {
        /// <summary>
        /// singleton pattern
        /// </summary>
        /// <value></value>
        public static VirtualTextEditor Current
        {
            get { return s_editor; }
        }

        private static VirtualTextEditor s_editor = new VirtualTextEditor();
        private int _rememberedX;
        private VirtualTextBuffer _buffer;
        private VirtualTextBufferPosition _selectionStart;
        private VirtualTextBufferPosition _selectionEnd;
        private bool _insertMode = false;

        /// <summary>
        /// Fill internal VirtualTextBuffer
        /// </summary>
        /// <param name="str"></param>
        public void FillBuffer(string str)
        {
            _buffer.Fill(str);
        }

        /// <summary>
        /// private ctor
        /// </summary>
        private VirtualTextEditor()
        {
            _rememberedX = 0;
            _buffer = new VirtualTextBuffer();
            _selectionStart = new VirtualTextBufferPosition(_buffer);
            _selectionEnd = (VirtualTextBufferPosition)_selectionStart.Clone();
        }

        /// <summary>
        /// Return the character distance between _selectionStart and _selectionEnd
        /// </summary>
        /// <value></value>
        public int SelectionLength
        {
            get
            {
                int distance = VirtualTextBufferPosition.GetCharacterDistanceBetweenTwoPositions(_selectionStart,
                    _selectionEnd);

                return distance < 0 ? distance * -1 : distance;
            }
        }

        /// <summary>
        /// Get string before visual selection start. Visual selection start is defined
        /// as the selection edge where the one which has smaller offset
        /// </summary>
        /// <returns></returns>
        public string GetRawTextBeforeSelectionLogicalStart()
        {
            VirtualTextBufferPosition whichPosition = _selectionStart > _selectionEnd ? _selectionEnd : _selectionStart;

            return whichPosition.GetString(LogicalDirection.Backward);
        }

        /// <summary>
        /// Get string after visual selection End
        /// </summary>
        /// <returns></returns>
        public string GetRawTextAfterSelectionLogicalEnd()
        {
            VirtualTextBufferPosition whichPosition = _selectionStart < _selectionEnd ? _selectionEnd : _selectionStart;

            return whichPosition.GetString(LogicalDirection.Forward);
        }

        /// <summary>
        /// This property returns true when _selectionEnd has a larger offset than _selectionStart
        /// </summary>
        /// <value></value>
        public bool IsSelectionEndAfterStart
        {
            get { return _selectionEnd >= _selectionStart; }
        }

        /// <summary>
        /// Get raw text from internal VirtualTextBuffer
        /// </summary>
        /// <returns></returns>
        public string GetRawText()
        {
            return _buffer.GetString(0, LogicalDirection.Forward);
        }

        /// <summary>
        /// Returns true if text is selected (i.e. _selectionStart and _selectionEnd has different offset
        /// </summary>
        /// <value></value>
        public bool IsTextSelected
        {
            get { return SelectionLength > 0; }
        }

        /// <summary>
        /// Returns the string in the selection
        /// </summary>
        /// <value></value>
        public string SelectedText
        {
            get
            {
                return VirtualTextBufferPosition.GetStringBetweenTwoPositions(_selectionStart,
                    _selectionEnd);
            }
        }

        /// <summary>
        /// Return a VirtualTextBufferPosition which is adjusted for the end of line.
        /// For example in Shift-End operation the end of line marker (\r\n) is selected,
        /// but at that point _selectionEnd should be considered as the valid location in
        /// subsequent key operations since it is now at the start of next line. In this
        /// case we need to get the position that falls in the current line.
        /// We only consider if selectin is active and _selectionEnd is right after the
        /// new line marker. Key operations must call this method 
        /// to get the adjusted position when selection is active.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private VirtualTextBufferPosition GetAdjustedPositionForLine(VirtualTextBufferPosition position)
        {
            VirtualTextBufferPosition adjustedPosition;

            adjustedPosition = (VirtualTextBufferPosition)position.Clone();

            // if:
            // 1. selection is active
            // 2. position is right after a new line marker
            if (IsTextSelected 
                && position.GetString(LogicalDirection.Backward).EndsWith(Environment.NewLine))
            {
                adjustedPosition.MoveBySymbolDistance(-1);
            }

            return adjustedPosition;
        }
        
        /// <summary>
        /// Press left / right key
        /// if delta is negative it is a left key, else a right key.
        /// if isShiftKey true the key is Shift-Left / Shift-Right
        /// </summary>
        /// <param name="rightKey"></param>
        /// <param name="isShiftKey"></param>
        public void DoLeftRightKey(bool rightKey, bool isShiftKey)
        {
            int moved;
            bool isAtFirstLine;
            VirtualTextBufferPosition position;

            isAtFirstLine = false;

            // we extend selection on shift key pressed
            if (isShiftKey)
            {
                position = GetAdjustedPositionForLine(_selectionEnd);

                if (rightKey)
                {
                    moved = position.MoveBySymbolDistance(1);
                }
                else
                {
                    moved = position.MoveBySymbolDistance(-1);
                }

                _selectionEnd = (position);
            }
            else
            {
                // if text is selected and
                // 1. left key is pressed - selection collapses to the edge logically before
                // 2. right key is pressed - selection collapses to the edge logically after
                // else if selection is not active
                // simply move _selectionEnd 
                // need to sync up _selectionStart and _selectionEnd in all cases here.
                if (IsTextSelected)
                {
                    if (rightKey)
                    {
                        position = _selectionStart > _selectionEnd ? _selectionStart : _selectionEnd;
                    }
                    else
                    {
                        position = _selectionStart > _selectionEnd ? _selectionEnd : _selectionStart;
                    }

                    _selectionEnd = (position);
                }
                else
                {
                    if (rightKey)
                    {
                        moved = _selectionEnd.MoveBySymbolDistance(1);
                    }
                    else
                    {
                        moved = _selectionEnd.MoveBySymbolDistance(-1);
                    }
                }

                _selectionStart = (_selectionEnd);

            }

            // store _rememberedX for all cases.
            _rememberedX = _selectionEnd.GetDistanceFromNewLine(LogicalDirection.Backward,
                ref isAtFirstLine);
        }

        /// <summary>
        /// Press Up / Down key. delta > 0 is down else it is up.
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="isShiftKey"></param>
        /// <returns></returns>
        public void DoUpDownKey(int delta, bool isShiftKey)
        {
            int lineMoved;
            int moved;
            bool isAtFirstOrLastLine;
            VirtualTextBufferPosition position;

            isAtFirstOrLastLine = false;
            moved = 0;

            position = GetAdjustedPositionForLine(_selectionEnd);

            lineMoved = position.MoveByLine(delta, true /*respectRememberedX*/, _rememberedX);

            _selectionEnd.MoveToPosition(position);

            if (!isShiftKey)
            {
                _selectionStart = (_selectionEnd);
            }

            // if lineMoved == 0 and delta != 0 => we want to move but we can't
            // reason: we are at the first or last line
            // so it shift key is on we need to select all the way to the start / end
            // depending on the value of delta.
            else if (lineMoved == 0 && delta != 0 && isShiftKey)
            {
                moved = delta < 0
                    ? _selectionEnd.MoveToStartOfLine(ref isAtFirstOrLastLine)
                    : _selectionEnd.MoveToEndOfLine(ref isAtFirstOrLastLine);

                Debug.Assert(isAtFirstOrLastLine);
            }
        }

        public void DoDeleteBackspaceKey(bool backspaceKey, bool isShiftKey)
        {
            VirtualTextBufferPosition position;
            int moved;
            bool isAtFirstLine;
            int count; /* for debug purpose */

            isAtFirstLine = false;

            // if selection is active and shiftkey is pressed
            // regardless of whether it is backspace key or delete key
            // the behavior is the same: delete contents in selection
            // and collapse selection to the logical left edge of that
            if (IsTextSelected)
            {
                position = _selectionStart < _selectionEnd
                    ? _selectionStart
                    : _selectionEnd;

                count = SelectedText.Length;

                Debug.Assert(count == VirtualTextBufferPosition.RemoveCharacters(_selectionStart,
                    _selectionEnd));

                _selectionStart = (position);
                _selectionEnd = (position);
            }
            else if (!IsTextSelected)
            {
                // if backspace or shift-backspace
                if (backspaceKey)
                {
                    moved = _selectionEnd.MoveBySymbolDistance(-1);

                    Debug.Assert(moved <= 0);

                    // if moved == 0, it is at the start of the buffer, in
                    // this case a backspace is a no-op
                    if (moved < 0)
                    {
                        // remember moved is non-positive, but RemoveCharacters only 
                        // accept non-negative values
                        count = _selectionEnd.RemoveCharacters(LogicalDirection.Forward, moved * -1);

                        // count here is for debugging purpose, we are sure that we can move
                        // by "moved" number of characters, so we should be able to delete them
                        // if this fails there are two possibilies
                        // 1. MoveBySymbolDistance has a bug which moves the position to somewhere invalid,
                        //    so subsequent RemoveCharacters call fails since the characters are non-existing
                        // 2. RemovedCharacters has a bug: we can move that far but we can't delete those characters
                        // Well, what if this is changed to MT?
                        Debug.Assert(count == moved * -1);

                        // sync _selectionStart and _selectionEnd
                        _selectionStart = (_selectionEnd);

                        // store _rememberedX
                        _rememberedX = _selectionEnd.GetDistanceFromNewLine(LogicalDirection.Backward,
                            ref isAtFirstLine);
                    }
                }
                    // if delete key only (shift-delete is no-op)
                else if (!backspaceKey && !isShiftKey)
                {
                    position = (VirtualTextBufferPosition)_selectionEnd.Clone();

                    moved = position.MoveBySymbolDistance(1);

                    Debug.Assert(moved >= 0);

                    // moved > 0 means that we are not at the end of the text buffer
                    if (moved > 0)
                    {
                        count = VirtualTextBufferPosition.RemoveCharacters(_selectionEnd, position);

                        Debug.Assert(count == moved);

                        // sync _selectionStart and _selectionEnd
                        _selectionStart = (_selectionEnd);
                    }
                }
            }
        }

        public void DoHomeEndKey(bool homeKey, bool isShiftKey)
        {
            VirtualTextBufferPosition position;
            //VirtualTextBufferPosition tempPosition;
            bool isAtFirstLine;
            bool isAtLastLine;

            isAtFirstLine = false;
            isAtLastLine = false;
            // extend selection in this case
            // Shift-home covers all text from the start of the line to the current position
            // shift-end covers all text from current position to the end of the line, including
            // the preceeding newline character
            if (isShiftKey)
            {
                position = GetAdjustedPositionForLine(_selectionEnd);

                if (homeKey)
                {
                    position.MoveToStartOfLine(ref isAtFirstLine);
                    _selectionEnd = (position);
                }
                else
                {
                    position.MoveToEndOfLine(ref isAtLastLine);
                    _selectionEnd = (position);

                    // This is to cover the newline character.
                    _selectionEnd.MoveBySymbolDistance(1);
                }
            }

            // if selection is active, collapse selection in this manner:
            // 1. if it is home key, always collapse selection to the edge which is logically
            //    before
            // 2. if it is end key, always collapse selection to the edge which is logically
            //    after
            // when selection is active we need to act on position rather than _selectionEnd
            else
            {
                if (homeKey)
                {
                    position = _selectionEnd > _selectionStart
                        ? GetAdjustedPositionForLine(_selectionStart)
                        : GetAdjustedPositionForLine(_selectionEnd);

                }
                else
                {
                    position = _selectionStart > _selectionEnd
                        ? GetAdjustedPositionForLine(_selectionStart)
                        : GetAdjustedPositionForLine(_selectionEnd);
                }

                if (homeKey)
                {
                    position.MoveToStartOfLine(ref isAtFirstLine);
                }
                else
                {
                    position.MoveToEndOfLine(ref isAtLastLine);
                }

                // if tempPosition is operating on adjusted position,
                // we need to adjust _selectionEnd back.
                _selectionEnd = (position);

                // since this is not shift, sync up _selectionStart and _selectionEnd (selection collapse)
                _selectionStart = (_selectionEnd);
            }

            // store _rememberedX if it is *not* shift
            if (!isShiftKey)
            {
                _rememberedX = _selectionEnd.GetDistanceFromNewLine(LogicalDirection.Backward,
                    ref isAtFirstLine /*don't care */);
            }
        }

        /// <summary>
        /// Type each of the character as specified in str.
        /// </summary>
        /// <param name="str"></param>
        public void DoTypeString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                DoTypeChar(str[i]);
            }
        }

        /// <summary>
        /// Type the character c
        /// </summary>
        /// <param name="c"></param>
        public void DoTypeChar(char c)
        {
            bool isFirstOrLastLine = false;
            if (IsTextSelected)
            {
                // if we have selection, dismiss the selection and delete
                // those characters in the selection
                VirtualTextBufferPosition.RemoveCharacters(_selectionStart,
                    _selectionEnd);
            }
            else if (_insertMode)
            {
                // if it is in insert mode we delete the next character before we 
                // insert the new one.
                int nextSymbolOffset = _selectionEnd.GetNextSymbolOffset(LogicalDirection.Forward);

                Debug.Assert(nextSymbolOffset >= 0);

                string str = _selectionEnd.GetString(LogicalDirection.Forward);

                // we should not overwrite newline character on insert mode.  Windows OS 
                if (nextSymbolOffset != 0 && !str.StartsWith(Environment.NewLine))
                {
                    Debug.Assert(_selectionStart.RemoveCharacters(LogicalDirection.Forward,
                        nextSymbolOffset) == nextSymbolOffset);
                }
            }
            _selectionEnd.InsertChar(c);
            _selectionEnd.MoveByCharacterDistance(1);
            _selectionStart = (_selectionEnd);

            // cache the new _rememberedX
            _rememberedX = _selectionEnd.GetDistanceFromNewLine(LogicalDirection.Backward,
                ref isFirstOrLastLine);

            // GetDistanceFromNewLine always return non-negative number
            Debug.Assert(_rememberedX >= 0);
        }

        /// <summary>
        /// Type Enter Key
        /// </summary>
        /// <param name="isShiftKey"></param>
        public void DoTypeEnter(bool isShiftKey)
        {
            string str = Environment.NewLine;

            // we need to cache insert mode first
            // since insert mode has no effect
            // on Enter key
            // Note: This is not MT safe since _insertMode
            // is not protected but this is fine since 
            // we know that at any given time there
            // will only be one thread writing (actually reading as well)
            // _insertMode.
            bool cachedInsertMode = _insertMode;

            for (int i = 0; i < str.Length; i++)
            {
                // INSERT key has no effect on enter key
                _insertMode = false;
                DoTypeChar(str[i]);
            }
            _insertMode = cachedInsertMode;
        }

        // toggle the insert key.
        public void DoTypeInsert()
        {            
            _insertMode = _insertMode ? false : true;
        }
    }
}
