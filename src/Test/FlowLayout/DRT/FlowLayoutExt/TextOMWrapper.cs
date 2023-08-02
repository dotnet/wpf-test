// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Wrapper for TextOM internal classes. 
//
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // ITextView wrapper.
    // ----------------------------------------------------------------------
    internal sealed class TextView
    {
        // ------------------------------------------------------------------
        //
        // Constructors
        //
        // ------------------------------------------------------------------

        #region Constructors

        // ------------------------------------------------------------------
        // Constructor
        //
        //      textView - actual instance of ITextView
        // ------------------------------------------------------------------
        internal TextView(object textView)
        {
            _textView = textView;
        }

        #endregion Constructors

        // ------------------------------------------------------------------
        //
        // ITextView
        //
        // ------------------------------------------------------------------

        #region ITextView

        // ------------------------------------------------------------------
        // ITextView.TextSegments wrapper
        // ------------------------------------------------------------------
        internal ReadOnlyCollection<object> TextSegments
        {
            get
            {
                ReadOnlyCollection<object> textSegments;
                textSegments = (ReadOnlyCollection<object>)s_textViewType.InvokeMember(
                    "TextSegments",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    null, _textView, null);
                return textSegments;
            }
        }

        // ------------------------------------------------------------------
        // ITextView.TextContainer wrapper
        // ------------------------------------------------------------------
        internal TextContainer TextContainer
        {
            get
            {
                object textContainer = s_textViewType.InvokeMember(
                    "TextContainer",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    null, _textView, null);
                return new TextContainer(textContainer);
            }
        }

        // ------------------------------------------------------------------
        // ITextView.GetTextPositionFromPoint wrapper
        // ------------------------------------------------------------------
        internal TextPointer GetTextPositionFromPoint(Point point, bool snapToText)
        {
            object[] args = new object[] { point, snapToText };
            TextPointer pos = (TextPointer)s_textViewType.InvokeMember(
                "GetTextPositionFromPoint",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args);
            return pos;
        }

        // ------------------------------------------------------------------
        // ITextView.GetRectangleFromTextPosition wrapper
        // ------------------------------------------------------------------
        internal Rect GetRectangleFromTextPosition(TextPointer pos)
        {
            object[] args = new object[] { pos };
            Rect rect = (Rect)s_textViewType.InvokeMember(
                "GetRectangleFromTextPosition",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args);
            return rect;
        }

        // ------------------------------------------------------------------
        // TextView.GetPositionAtNextLine wrapper
        // ------------------------------------------------------------------
        internal TextPointer GetPositionAtNextLine(TextPointer posIn, double suggestedX, int count, out double newSuggestedX, out int linesMoved)
        {
            object objNewSuggestedX = null;
            object objLinesMovedObj = null;
            object[] args = new object[] { posIn, suggestedX, count, objNewSuggestedX, objLinesMovedObj };
            TextPointer posOut = (TextPointer)s_textViewType.InvokeMember(
                "GetPositionAtNextLine",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args, s_pmPositionAtNextLine, null, null);
            newSuggestedX = (double)args[3];
            linesMoved = (int)args[4];
            return posOut;
        }

        // ------------------------------------------------------------------
        // TextView.GetNextCaretUnitPosition wrapper
        // ------------------------------------------------------------------
        internal TextPointer GetNextCaretUnitPosition(TextPointer position, LogicalDirection direction)
        {
            object[] args = new object[] { position, direction };
            TextPointer nextPosition = (TextPointer)s_textViewType.InvokeMember(
                "GetNextCaretUnitPosition",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args);
            return nextPosition;
        }

        // ------------------------------------------------------------------
        // TextView.GetBackspaceCaretUnitPosition wrapper
        // ------------------------------------------------------------------
        internal TextPointer GetBackspaceCaretUnitPosition(TextPointer position)
        {
            object[] args = new object[] { position };
            TextPointer backspacePosition = (TextPointer)s_textViewType.InvokeMember(
                "GetBackspaceCaretUnitPosition",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args);
            return backspacePosition;
        }

        // ------------------------------------------------------------------
        // TextView.IsAtCaretUnitBoundary wrapper
        // ------------------------------------------------------------------
        internal bool IsAtCaretUnitBoundary(TextPointer position)
        {
            object[] args = new object[] { position };
            bool isAtCaretUnitBoundary = (bool)s_textViewType.InvokeMember(
                "IsAtCaretUnitBoundary",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, _textView, args);
            return isAtCaretUnitBoundary;
        }

        #endregion ITextView

        // ------------------------------------------------------------------
        //
        // MultiPageTextView
        //
        // ------------------------------------------------------------------

        #region MultiPageTextView

        // ------------------------------------------------------------------
        // MultiPageTextView.ActiveDocumentPageViews
        // ------------------------------------------------------------------
        internal void SetActiveDocumentPageViews(ReadOnlyCollection<DocumentPageView> pageViews)
        {
            object[] args = new object[] { pageViews };
            s_multiPageTextViewType.InvokeMember(
                "ActiveDocumentPageViews",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, _textView, args);
        }

        #endregion MultiPageTextView

        // ------------------------------------------------------------------
        //
        // Internal
        //
        // ------------------------------------------------------------------

        #region Internal

        // ------------------------------------------------------------------
        // Initialzie
        // ------------------------------------------------------------------
        internal static void Initialize(DrtBase drt)
        {
            s_drt = drt;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            DRT.Assert(assembly != null, "Failed to load PresentationFramework assembly.");
            s_textViewType = assembly.GetType("System.Windows.Documents.ITextView");
            DRT.Assert(s_textViewType != null, "Failed to get type for ITextView.");
            s_multiPageTextViewType = assembly.GetType("MS.Internal.Documents.MultiPageTextView");
            DRT.Assert(s_multiPageTextViewType != null, "Failed to get type for MultiPageTextView.");

            s_pmPositionAtNextLine = new ParameterModifier[] { new ParameterModifier(5) };
            s_pmPositionAtNextLine[0][0] = false;
            s_pmPositionAtNextLine[0][1] = false;
            s_pmPositionAtNextLine[0][2] = false;
            s_pmPositionAtNextLine[0][3] = true;
            s_pmPositionAtNextLine[0][4] = true;
        }

        // ------------------------------------------------------------------
        // Actual instance of ITextView
        // ------------------------------------------------------------------
        internal object Instance { get { return _textView; } }

        // ------------------------------------------------------------------
        // Type of ITextView
        // ------------------------------------------------------------------
        internal static Type Type { get { return s_textViewType; } }

        // ------------------------------------------------------------------
        // DrtBase instance
        // ------------------------------------------------------------------
        internal static DrtBase DRT { get { return s_drt; } }

        #endregion Internal

        // ------------------------------------------------------------------
        //
        // Private Data
        //
        // ------------------------------------------------------------------

        #region Private Fields

        // ------------------------------------------------------------------
        // Private Fields
        // ------------------------------------------------------------------
        private object _textView;
        private static Type s_textViewType;
        private static Type s_multiPageTextViewType;
        private static ParameterModifier[] s_pmPositionAtNextLine;
        private static DrtBase s_drt;

        #endregion Private Fields
    }

    // ----------------------------------------------------------------------
    // ITextContainer wrapper.
    // ----------------------------------------------------------------------
    internal sealed class TextContainer
    {
        // ------------------------------------------------------------------
        //
        // Constructors
        //
        // ------------------------------------------------------------------

        #region Constructors

        // ------------------------------------------------------------------
        // Constructor
        //
        //      textContainer - actual instance of ITextContainer
        // ------------------------------------------------------------------
        internal TextContainer(object textContainer)
        {
            _textContainer = textContainer;
        }

        #endregion Constructors

        // ------------------------------------------------------------------
        //
        // ITextContainer
        //
        // ------------------------------------------------------------------

        #region ITextContainer

        // ------------------------------------------------------------------
        // ITextContainer.Start wrapper
        // ------------------------------------------------------------------
        internal TextPointer Start
        {
            get
            {
                TextPointer textPointer = (TextPointer)s_textContainerType.InvokeMember(
                    "Start",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    null, _textContainer, null);
                return textPointer;
            }
        }

        // ------------------------------------------------------------------
        // ITextContainer.Start wrapper
        // ------------------------------------------------------------------
        internal TextPointer End
        {
            get
            {
                TextPointer textPointer = (TextPointer)s_textContainerType.InvokeMember(
                    "End",
                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    null, _textContainer, null);
                return textPointer;
            }
        }

        #endregion ITextContainer

        // ------------------------------------------------------------------
        //
        // Internal
        //
        // ------------------------------------------------------------------

        #region Internal

        // ------------------------------------------------------------------
        // Constructor
        // ------------------------------------------------------------------
        internal static void Initialize(DrtBase drt)
        {
            s_drt = drt;

            s_assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            DRT.Assert(s_assembly != null, "Failed to load PresentationFramework assembly.");
            s_textContainerType = s_assembly.GetType("System.Windows.Documents.ITextContainer");
            DRT.Assert(s_textContainerType != null, "Failed to get type for ITextContainer.");
        }

        // ------------------------------------------------------------------
        // Get TextContainer instance from TextPointer
        // ------------------------------------------------------------------
        internal static TextContainer FromTextPointer(TextPointer textPointer)
        {
            object textContainer = typeof(TextPointer).InvokeMember(
                "TextContainer",
                BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                null, textPointer, null);
            DRT.Assert(textContainer != null, "Failed to get TextContainer from TextPointer.");
            return new TextContainer(textContainer);
        }

        // ------------------------------------------------------------------
        // Get TextContainer instance from TextPointer
        // ------------------------------------------------------------------
        internal static TextPointer CreateTextPointer(TextPointer textPointer, int distance, LogicalDirection logicalDirection)
        {
            TextPointer pos = (TextPointer)s_assembly.CreateInstance("System.Windows.Documents.TextPointer", false,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance, null,
                new object[] { textPointer, distance, logicalDirection }, System.Globalization.CultureInfo.InvariantCulture, null);
            DRT.Assert(pos != null, "Failed to create TextPointer.");
            return pos;
        }

        internal static TextPointer CreateTextPointer(TextPointer textPointer, int distance)
        {
            TextPointer pos = (TextPointer)s_assembly.CreateInstance("System.Windows.Documents.TextPointer", false,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance, null,
                new object[] { textPointer, distance}, System.Globalization.CultureInfo.InvariantCulture, null);
            DRT.Assert(pos != null, "Failed to create TextPointer.");
            return pos;
        }

        // ------------------------------------------------------------------
        // Actual instance of ITextContainer
        // ------------------------------------------------------------------
        internal object Instance { get { return _textContainer; } }

        // ------------------------------------------------------------------
        // Type of ITextView
        // ------------------------------------------------------------------
        internal static Type Type { get { return s_textContainerType; } }

        // ------------------------------------------------------------------
        // DrtBase instance
        // ------------------------------------------------------------------
        internal static DrtBase DRT { get { return s_drt; } }

        #endregion Internal

        // ------------------------------------------------------------------
        //
        // Private Data
        //
        // ------------------------------------------------------------------

        #region Private Fields

        // ------------------------------------------------------------------
        // Private Fields
        // ------------------------------------------------------------------
        private object _textContainer;
        private static Assembly s_assembly;
        private static Type s_textContainerType;
        private static DrtBase s_drt;

        #endregion Private Fields
    }
}
