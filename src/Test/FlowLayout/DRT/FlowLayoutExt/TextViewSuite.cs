// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for TextView test suites.
//
//

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for TextView test suites.
    // ----------------------------------------------------------------------
    internal abstract class TextViewSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected TextViewSuite(string suiteName) : base(suiteName)
        {
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        static TextViewSuite()
        {
            _assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.
            _contentRoot = new Border();
            _contentRoot.Width = 800;
            _contentRoot.Height = 600;
            Border root = new Border();
            root.Background = Brushes.DarkGreen;
            root.Child = _contentRoot;
            DRT.Show(root);

            // Return the lists of tests to run against the tree
            return CreateTests();
        }

        // ------------------------------------------------------------------
        // Create collection of tests.
        // ------------------------------------------------------------------
        protected abstract DrtTest[] CreateTests();

        // ------------------------------------------------------------------
        // Verify GetTextPositionFromPoint.
        // ------------------------------------------------------------------
        protected void VerifyTextPositionFromPoint(PositionFromPointTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                TextPointer pos;
                int distance;
                PositionFromPointTestDesc test = tests[i];

                pos = tv.GetTextPositionFromPoint(test.Point, test.SnapToText);
                if (test.Distance < 0)
                {
                    DRT.Assert(pos == null, this.TestName + ": GetTextPositionFromPoint test # {0} failed. Expecting NULL TextPointer.", i);
                }
                else
                {
                    DRT.Assert(pos != null, this.TestName + ": GetTextPositionFromPoint test # {0} failed. Expecting valid TextPointer.", i);
                    distance = tc.Start.GetOffsetToPosition(pos);
                    DRT.Assert(distance == test.Distance, this.TestName + ": GetTextPositionFromPoint test # {0} failed. Expecting distance {1}, got {2} at position ({3},{4}).", i, test.Distance, distance, test.Point.X, test.Point.Y);
                    DRT.Assert(pos.LogicalDirection == test.LogicalDirection, this.TestName + ": GetTextPositionFromPoint test # {0} failed. Expecting logical direction {1}, got {2}.", i, test.LogicalDirection, pos.LogicalDirection);
                }
            }
        }

        // ------------------------------------------------------------------
        // Verify GetRectangleFromTextPosition.
        // ------------------------------------------------------------------
        protected void VerifyRectangleFromTextPosition(RectangleFromPositionTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                RectangleFromPositionTestDesc test = tests[i];

                TextPointer pos = TextContainer.CreateTextPointer(tc.Start, test.Distance, test.LogicalDirection);
                Rect rect = tv.GetRectangleFromTextPosition(pos);
                DRT.Assert(CompareRects(rect, test.Rect), this.TestName + ": GetRectangleFromTextPosition test # {0} failed. Expecting ({1}), got ({2}).", i, test.Rect, rect);
            }
        }

        // ------------------------------------------------------------------
        // Verify GetPositionAtNextLine.
        // ------------------------------------------------------------------
        protected void VerifyPositionAtNextLine(PositionAtNextLineTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                PositionAtNextLineTestDesc test = tests[i];

                TextPointer posOut;
                int distance;
                double newSuggestedX;
                int linesMoved;

                TextPointer posIn = TextContainer.CreateTextPointer(tc.Start, test.Distance, test.LogicalDirection);
                posOut = tv.GetPositionAtNextLine(posIn, test.SuggestedX, test.Count, out newSuggestedX, out linesMoved);

                DRT.Assert(posOut != null, this.TestName + ": GetPositionAtNextLine test # {0} failed. Expecting valid TextPosition.", i);
                distance = tc.Start.GetOffsetToPosition(posOut);
                DRT.Assert(distance == test.NewDistance, this.TestName + ": GetPositionAtNextLine test # {0} failed. Expecting distance {1}, got {2}.", i, test.NewDistance, distance);
                DRT.Assert(posOut.LogicalDirection == test.NewLogicalDirection, this.TestName + ": GetPositionAtNextLine test # {0} failed. Expecting logical direction {1}, got {2}.", i, test.NewLogicalDirection, posOut.LogicalDirection);
                DRT.Assert(CompareOffsets(newSuggestedX, test.NewSuggestedX), this.TestName + ": GetPositionAtNextLine test # {0} failed. Expecting newSuggestedX = {1}, got {2}.", i, test.NewSuggestedX, newSuggestedX);
                DRT.Assert(linesMoved == test.LinesMoved, this.TestName + ": GetPositionAtNextLine test # {0} failed. Expecting linesMoved = {1}, got {2}.", i, test.LinesMoved, linesMoved);
            }
        }

        // ------------------------------------------------------------------
        // Verify next caret positions
        // ------------------------------------------------------------------
        protected void VerifyNextCaretUnitPosition(NextCaretUnitPositionTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                NextCaretUnitPositionTestDesc test = tests[i];

                TextPointer originalCaretPosition;
                TextPointer actualCaretPosition;

                // Creat caret position
                originalCaretPosition = TextContainer.CreateTextPointer(tc.Start, test.PositionOffset, test.PositionDirection);

                // Get Next
                actualCaretPosition = (TextPointer)tv.GetNextCaretUnitPosition(originalCaretPosition, test.Direction);

                int offset = tc.Start.GetOffsetToPosition(actualCaretPosition);
                LogicalDirection direction = actualCaretPosition.LogicalDirection;

                // Compare against expected
                DRT.Assert(offset == test.NewOffset, "Failure in VerifyNextCaretUnitPosition test #{0}, expected position at offset {1}, got offset {2}", i, test.NewOffset, offset);
                DRT.Assert(direction == test.NewDirection, "Failure in VerifyNextCaretUnitPosition test #{0}, expected position with direction {1}, got direction {2}", i, test.NewDirection, direction);
            }
        }

        // ------------------------------------------------------------------
        // Verify backspace caret positions
        // ------------------------------------------------------------------
        protected void VerifyBackspaceCaretUnitPosition(BackspaceCaretUnitPositionTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                BackspaceCaretUnitPositionTestDesc test = tests[i];

                TextPointer originalCaretPosition;
                TextPointer actualCaretPosition;

                // Creat caret position
                originalCaretPosition = TextContainer.CreateTextPointer(tc.Start, test.PositionOffset, test.PositionDirection);

                // Get Next
                actualCaretPosition = (TextPointer)tv.GetBackspaceCaretUnitPosition(originalCaretPosition);

                int offset = tc.Start.GetOffsetToPosition(actualCaretPosition);
                LogicalDirection direction = actualCaretPosition.LogicalDirection;

                // Compare against expected
                DRT.Assert(offset == test.NewOffset, "Failure in VerifyBackspaceCaretUnitPosition test #{0}, expected position at offset {1}, got offset {2}", i, test.NewOffset, offset);
                DRT.Assert(direction == test.NewDirection, "Failure in VerifyBackspaceCaretUnitPosition test #{0}, expected position with direction {1}, got direction {2}", i, test.NewDirection, direction);
            }
        }

        // ------------------------------------------------------------------
        // Verify caret unit boundaries
        // ------------------------------------------------------------------
        protected void VerifyIsAtCaretUnitBoundary(IsAtCaretUnitBoundaryTestDesc[] tests, TextView tv)
        {
            TextContainer tc = tv.TextContainer;
            for (int i = 0; i < tests.Length; i++)
            {
                IsAtCaretUnitBoundaryTestDesc test = tests[i];

                TextPointer originalCaretPosition;

                // Creat caret position
                originalCaretPosition = TextContainer.CreateTextPointer(tc.Start, test.PositionOffset, test.PositionDirection);

                // Get Next
                bool isAtBoundary = (bool)tv.IsAtCaretUnitBoundary(originalCaretPosition);

                // Check that actual == expected
                DRT.Assert(isAtBoundary == test.IsAtBoundary, "Failure in VerifyIsAtCaretUnitBoundary test #{0}, expected return value {1}, got value {2}", i, test.IsAtBoundary, isAtBoundary);
            }
        }

        // ------------------------------------------------------------------
        // Rectangle comarison
        // ------------------------------------------------------------------
        protected bool CompareRects(Rect rect1, Rect rect2)
        {
            if (rect1 == Rect.Empty)
            {
                return (rect2 == Rect.Empty);
            }
            else if (rect2 == Rect.Empty)
            {
                return (rect1 == Rect.Empty);
            }
            else
            {
                if (Math.Abs(rect1.Left - rect2.Left) < 1 &&
                    Math.Abs(rect1.Top - rect2.Top) < 1 &&
                    Math.Abs(rect1.Width - rect2.Width) < 1 &&
                    Math.Abs(rect1.Height - rect2.Height) < 1)
                {
                    return true;
                }
            }
            return false;
        }

        // ------------------------------------------------------------------
        // Offset comarison
        // ------------------------------------------------------------------
        protected bool CompareOffsets(double offset1, double offset2)
        {
            if (double.IsNaN(offset1))
            {
                return double.IsNaN(offset2);
            }
            else if (double.IsNaN(offset2))
            {
                return double.IsNaN(offset1);
            }
            return (Math.Abs(offset1 - offset2) < 1);
        }

        protected struct PositionFromPointTestDesc
        {
            internal PositionFromPointTestDesc(Point point, bool snapToText, LogicalDirection logicalDirection, int distance)
            {
                this.Point = point;
                this.SnapToText = snapToText;
                this.LogicalDirection = logicalDirection;
                this.Distance = distance;
            }
            internal Point Point;
            internal bool SnapToText;
            internal LogicalDirection LogicalDirection;
            internal int Distance;
        }

        protected struct RectangleFromPositionTestDesc
        {
            internal RectangleFromPositionTestDesc(int distance, LogicalDirection logicalDirection, Rect rect)
            {
                this.Distance = distance;
                this.LogicalDirection = logicalDirection;
                this.Rect = rect;
            }
            internal int Distance;
            internal LogicalDirection LogicalDirection;
            internal Rect Rect;
        }

        protected struct PositionAtNextLineTestDesc
        {
            internal PositionAtNextLineTestDesc(int distance, LogicalDirection logicalDirection, double suggestedX, int count, int newDistance, LogicalDirection newLogicalDirection, double newSuggestedX, int linesMoved)
            {
                this.Distance = distance;
                this.LogicalDirection = logicalDirection;
                this.SuggestedX = suggestedX;
                this.Count = count;
                this.NewDistance = newDistance;
                this.NewLogicalDirection = newLogicalDirection;
                this.NewSuggestedX = newSuggestedX;
                this.LinesMoved = linesMoved;
            }
            internal int Distance;
            internal LogicalDirection LogicalDirection;
            internal double SuggestedX;
            internal int Count;
            internal int NewDistance;
            internal LogicalDirection NewLogicalDirection;
            internal double NewSuggestedX;
            internal int LinesMoved;
        }

        protected struct NextCaretUnitPositionTestDesc
        {
            internal NextCaretUnitPositionTestDesc(int positionOffset, LogicalDirection positionDirection, LogicalDirection direction, int newOffset, LogicalDirection newDirection)
            {
                this.PositionOffset = positionOffset;
                this.PositionDirection = positionDirection;
                this.Direction = direction;
                this.NewOffset = newOffset;
                this.NewDirection = newDirection;
            }
            internal int PositionOffset;
            internal LogicalDirection PositionDirection;
            internal LogicalDirection Direction;
            internal int NewOffset;
            internal LogicalDirection NewDirection;
        }

        protected struct BackspaceCaretUnitPositionTestDesc
        {
            internal BackspaceCaretUnitPositionTestDesc(int positionOffset, LogicalDirection positionDirection, int newOffset, LogicalDirection newDirection)
            {
                this.PositionOffset = positionOffset;
                this.PositionDirection = positionDirection;
                this.NewOffset = newOffset;
                this.NewDirection = newDirection;
            }
            internal int PositionOffset;
            internal LogicalDirection PositionDirection;
            internal int NewOffset;
            internal LogicalDirection NewDirection;
        }

        protected struct IsAtCaretUnitBoundaryTestDesc
        {
            internal IsAtCaretUnitBoundaryTestDesc(int positionOffset, LogicalDirection positionDirection, bool isAtBoundary)
            {
                this.PositionOffset = positionOffset;
                this.PositionDirection = positionDirection;
                this.IsAtBoundary = isAtBoundary;
            }
            internal int PositionOffset;
            internal LogicalDirection PositionDirection;
            internal bool IsAtBoundary;
        }

        // ------------------------------------------------------------------
        // Load from xaml file.
        // ------------------------------------------------------------------
        protected object LoadFromXaml(string xamlFile)
        {
            object content = null;
            System.IO.Stream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(DrtFilesDirectory + xamlFile);
                content = System.Windows.Markup.XamlReader.Load(stream);
            }
            finally
            {
                // done with the stream
                if (stream != null)
                {
                    stream.Close();
                }
            }
            DRT.Assert(content != null, "Failed to load xaml file '{0}'", this.DrtFilesDirectory + xamlFile);
            return content;
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        protected string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayoutExt\\"; }
        }

        // ------------------------------------------------------------------
        // Unique name of the test.
        // ------------------------------------------------------------------
        protected string TestName
        {
            get { return this.Name + _testName; }
        }

        // ------------------------------------------------------------------
        // The name of the current test.
        // ------------------------------------------------------------------
        protected string _testName;

        // ------------------------------------------------------------------
        // Placeholder for content.
        // ------------------------------------------------------------------
        protected Border _contentRoot;

        // ------------------------------------------------------------------
        // Framework assembly.
        // ------------------------------------------------------------------
        protected static System.Reflection.Assembly _assembly;
    }
}
