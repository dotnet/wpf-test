// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a model of plain or rich text layout.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Models/TextLayoutModel.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Character index / bounding rectangle pair.
    /// </summary>
    public class TextLayoutUnit
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new TextLayoutUnit instance.
        /// </summary>
        /// <param name="index">Index of character bound.</param>
        /// <param name="rectangle">Rectangle bounding character.</param>
        public TextLayoutUnit(int index, Rect rectangle)
        {
            this._index = index;
            this._rectangle = rectangle;
        }

        #endregion Constructors.

        #region Public properties.

        /// <summary>
        /// Index of character or symbol in containing store.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>Bounding rectangle for character or symbol.</summary>
        public Rect Rectangle
        {
            get { return _rectangle; }
        }

        #endregion Public properties.

        #region Private fields.

        /// <summary>Index of character or symbol in containing store.</summary>
        private int _index;

        /// <summary>Bounding rectangle for character or symbol.</summary>
        private Rect _rectangle;

        #endregion Private fields.
    }

    /// <summary>Snapshot of layout information for text.</summary>
    public class TextLayoutModel
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new TextLayoutModel instance.
        /// </summary>
        /// <param name='wrapper'>Wrapper around object to capture.</param>
        public TextLayoutModel(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            this._wrapper = wrapper;
            this._units = new List<TextLayoutUnit>();
        }

        #endregion Constructors.

        #region Public methods.
        
        /// <summary>Captures all layout information.</summary>
        /// <remarks>
        /// As a special case, this will add a fake unit for empty documents.
        /// </remarks>
        public void CaptureLayoutInformation()
        {
            if (_wrapper.Start.GetOffsetToPosition(_wrapper.End) == 0)
            {
                Units.Add(new TextLayoutUnit(0, 
                    _wrapper.Start.GetCharacterRect(LogicalDirection.Forward)));
            }
            else
            {
                CaptureLayoutInformation(_wrapper.Start, _wrapper.End);
            }
        }

        /// <summary>Captures layout information between the given symbols.</summary>
        /// <param name='startIndex'>Index of first symbol to capture.</param>
        /// <param name='endIndex'>Index of last symbol to capture.</param>        
        public void CaptureLayoutInformation(int startIndex, int endIndex)
        {
            TextPointer startPointer;
            TextPointer endPointer;

            if (startIndex < 0 || startIndex > endIndex)
            {
                throw new ArgumentException("startIndex should be >= 0 and smaller than endIndex", "startIndex");
            }

            startPointer = _wrapper.Start;
            startPointer = startPointer.GetPositionAtOffset(startIndex);
            endPointer = startPointer;
            endPointer = endPointer.GetPositionAtOffset(endIndex - startIndex);

            CaptureLayoutInformation(startPointer, endPointer);
        }

        /// <summary>Captures layout information between the given pointers.</summary>
        /// <param name='startPointer'>Pointer before first symbol to capture.</param>
        /// <param name='endPointer'>Pointer after last symbol to capture.</param>        
        public void CaptureLayoutInformation(TextPointer startPointer, TextPointer endPointer)
        {
            TextPointer cursor; // Pointer that moves capturing information.
            int positionCount;  // Count of positions to move.
            int positionIndex;  // Index of position being recorded.

            cursor = startPointer;
            positionCount = cursor.GetOffsetToPosition(endPointer);
            positionIndex = cursor.DocumentStart.GetOffsetToPosition(startPointer);
            if (positionCount < 0)
            {
                throw new ArgumentException("startPointer should be before endPointer", "startPointer");
            }

            while (positionCount > 0)
            {
                Rect rect;  // Rectangle for forward-looking character.

                rect = cursor.GetCharacterRect(LogicalDirection.Forward);
                Units.Add(new TextLayoutUnit(positionIndex, rect));
                cursor = cursor.GetPositionAtOffset(1);
                positionIndex++;
                positionCount--;
            }
        }

        /// <summary>
        /// calculate the TextPointer that is at the insertion position from a pointer
        /// This method need to use the layout captured by CaptureLayoutInformation. When a
        /// layout is captured, we need the line information to compute the closest Pointer if snaptoText is true.
        /// </summary>
        /// <param name="point">point relative to the control</param>
        /// <param name="snapToText">true: return the closest TextPointer to the point. 
        /// false: return TextPointer if it in a bounding box, otherwise return null</param>
        /// <returns>TextPointer/null</returns>
        public TextPointer GetPositionFromPoint(Point point, bool snapToText)
        {
            TextPointer result, tempPointer;
            Rect textRect, tempRect;
            int lineCount, x, y;
            x =(int) point.X;
            y = (int) point.Y;

            result = this._wrapper.Start;

            if (!result.IsAtInsertionPosition)
                result = result.GetInsertionPosition(LogicalDirection.Forward);

            lineCount = LineCount;

            //Go through line by line
            for (int i = 0; i < lineCount; i++)
            {

                //go to the end of the line
                if (i == lineCount - 1)
                {
                    tempPointer = this._wrapper.End;
                }
                else
                {
                    tempPointer = result;
                    tempPointer = tempPointer.GetLineStartPosition(1);
                }

                tempPointer = tempPointer.GetInsertionPosition(LogicalDirection.Backward);

                //go through by each insertion pointer.
                do
                {
                    textRect = this._wrapper.GetElementRelativeCharacterRect(result, 0, LogicalDirection.Forward);

                    if (snapToText)
                    {
                        //The last line
                        if (i == lineCount - 1)
                        {
                            //be careful the empty lines should be considered.
                            if (x <= (textRect.Right + textRect.Left) / 2 || textRect.Right == textRect.Left)
                                return result;
                            else if (x > (textRect.Right + textRect.Left) / 2 && x <= textRect.Right)
                            {
                                result = result.GetNextInsertionPosition(LogicalDirection.Forward);
                                return result;
                            }
                        }
                        //other lines
                        else
                        {
                            if (y <= textRect.Bottom)
                            {
                                //be careful the empty lines should be considered. 
                                if (x <= (textRect.Right + textRect.Left) / 2 || textRect.Right == textRect.Left)
                                    return result;
                                else if (x > (textRect.Right + textRect.Left) / 2 && x <= textRect.Right)
                                {
                                    result = result.GetNextInsertionPosition(LogicalDirection.Forward);
                                    return result;
                                }
                            }
                        }
                    }
                    //if SnapToText is false, a TextPointer can only be returned if the point is in a bounding box.
                    else
                    {
                        if (y > textRect.Top && y <= textRect.Bottom)
                        {
                            //be careful the empty lines should be considered.
                            tempRect = this._wrapper.GetElementRelativeCharacterRect(this._wrapper.Start, 0, LogicalDirection.Forward);
                            //What is the value if a point is right on the enpty line? 
                            if (x > textRect.Left && x <= (textRect.Right + textRect.Left) / 2 || (textRect.Right == textRect.Left && textRect.Right < tempRect.Left))
                                return result;
                            else if (x > (textRect.Right + textRect.Left) / 2 && x <= textRect.Right)
                            {
                                result = result.GetNextInsertionPosition(LogicalDirection.Forward);
                                return result;
                            }
                        }
                    }
                    result = result.GetNextInsertionPosition(LogicalDirection.Forward);
                }
                while (result.CompareTo(tempPointer) != 0);
            }
            //return null if the SnapToText if point if out of the RichTextBox boundary.
            return null;
        }

        /// <summary>Returns a description of the layout contents.</summary>
        /// <returns>A multiline description of the layout contents.</returns>
        public string DescribeModel()
        {
            StringBuilder result;

            result = new StringBuilder(60 + 32 * Units.Count);
            result.AppendLine("Text Layout Model - unit count: " + Units.Count);
            foreach(TextLayoutUnit unit in Units)
            {
                result.AppendFormat(" Index {0,3:d} - Rectangle (L;T;R;B): " +
                    "{1,9:f4};{2,9:f4};{3,9:f4};{4,9:f4}\r\n", unit.Index, unit.Rectangle.Left,
                    unit.Rectangle.Top, unit.Rectangle.Right, unit.Rectangle.Bottom);
            }

            return result.ToString();
        }
        
        /// <summary>
        /// Gets the index of the layout unit that is hit-tested by the 
        /// specified point.
        /// </summary>
        /// <param name='point'>Point being hit-tested.</param>
        /// <param name='snapToText'>Whether testing snaps to closest text.</param>
        /// <returns>
        /// The index of the layout unit that is hit-tested by the 
        /// specified point, -1 if snapToText is false and the point
        /// is outside the layout model.
        /// </returns>
        public int GetUnitIndexFromPoint(Point point, bool snapToText)
        {
            int result;
            double closestHorizontalDistance;
            
            result = -1;
            closestHorizontalDistance = double.MaxValue;
            foreach(TextLayoutUnit unit in Units)
            {
                // If the point is directly on a character, use that.
                if (unit.Rectangle.Contains(point))
                {
                    result = unit.Index;
                    return result;
                }
                
                // If the point is on the line, look for the closest candidate.
                if (snapToText)
                {
                    if (unit.Rectangle.Top <= point.Y && unit.Rectangle.Bottom >= point.Y)
                    {
                        double distance;

                        distance = HorizontalDistance(point, unit.Rectangle);
                        if (distance < closestHorizontalDistance)
                        {
                            closestHorizontalDistance = distance;
                            result = unit.Index;
                        }
                    }
                }
            }

            return result;
        }
        
        #endregion Public methods.


        #region Public properties.
        
        /// <summary> return the line count of a Text Control</summary>
        int LineCount
        {
            get
            {
                // Get first insertion position of a document
                TextPointer lineStart = ((RichTextBox)(this._wrapper.Element)).Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);

                int count;
                lineStart.GetLineStartPosition(System.Int16.MaxValue, out count);

                return count;
            }
        }

        /// <summary>List of TextLayoutUnit objects.</summary>
        public List<TextLayoutUnit> Units
        {
            get { return _units; }
        }

        /// <summary>Wrapper around element encapsulated.</summary>
        public UIElementWrapper Wrapper
        {
            get { return this._wrapper; }
        }

        #endregion Public properties.


        #region Private methods.

        private double HorizontalDistance(Point point, Rect rect)
        {
            if (rect.Left > point.X)
            {
                return rect.Left - point.X;
            }
            else if (rect.Right < point.X)
            {
                return point.X - rect.Right;
            }
            else
            {
                return 0;
            }
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>List of TextLayoutUnit objects.</summary>
        private List<TextLayoutUnit> _units;

        /// <summary>Wrapper around element encapsulated.</summary>
        private UIElementWrapper _wrapper;

        #endregion Private fields.


        #region Unit tests.

        /*

        /// <summary>Main entry point for tests.</summary>
        [STAThread]
        static void Main()
        {
            TestTextLayoutUnit();

            TestConstructor();
            TestWrapper();
        }

        private static void Assert(bool condition, string conditionDescription)
        {
            if (!condition) throw new ApplicationException("Assertion failed: " + conditionDescription);
        }

        private static void TestTextLayoutUnit()
        {
            TextLayoutUnit unit;

            unit = new TextLayoutUnit(10, new Rect(10, 10, 20, 20));
            Verifier.Verify(unit.Index == 10, "Constructor sets Index.");
            Verifier.Verify(unit.Rectangle.Width == 20, "Constructor sets rectangle.");
        }

        private static void TestConstructor()
        {
            TextBox textbox;
            UIElementWrapper wrapper;

            try
            {
                new TextLayoutModel(null);
                Assert(false, "Exception not raised for null argument.");
            }
            catch (ArgumentNullException) { }

            textbox = new TextBox();
            wrapper = new UIElementWrapper(textbox);
            new TextLayoutModel(wrapper);
        }

        private static void TestWrapper()
        {
            TextBox textbox;
            UIElementWrapper wrapper;

            textbox = new TextBox();
            wrapper = new UIElementWrapper(textbox);
            Assert(new TextLayoutModel(wrapper).Wrapper == wrapper, "Wrapper matches constructor argument.");
        }
        
        */

        #endregion Unit tests.
    }
}
