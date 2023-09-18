// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a model of state relevant for keyboard operations.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Models/KeyboardEditingState.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Threading;
    using System.Text;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Snapshot for a given element of interesting state for
    /// keyboard editing operations.
    /// </summary>
    public class KeyboardEditingState
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new KeyboardEditingState instance.
        /// </summary>
        /// <param name='wrapper'>Wrapper around object to capture.</param>
        internal KeyboardEditingState(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            // Capture state common to all controls.
            this.Wrapper = wrapper;

            this.CaretPosition = wrapper.GetDocumentRelativeCaretPosition();
            this.Container = (wrapper.IsElementRichText) ?
                new ArrayTextContainer(wrapper.Start) :
                new ArrayTextContainer(wrapper.Text);
            this.IsKeyboardFocused = wrapper.Element.IsFocused;
            this.IsOvertypeModeEnabled = wrapper.IsOvertypeModeEnabled;
            this.Text = wrapper.Text;
            this.SelectedText = wrapper.SelectionInstance.Text;

            this.SelectionMovingDistance = wrapper.SelectionMovingPointerDistance;

            this.IsPasswordBox = wrapper.Element is PasswordBox;

            if (IsPasswordBox)
            {
                this.AcceptsReturn = false;
                this.IsReadOnly = false;

                this.SelectionLength = wrapper.GetSelectedText(false, false).Length;
                this.SelectionStartDirection = LogicalDirection.Backward;
                this.SelectionStartDistance = wrapper.SelectionStart;
                this.SelectionEndDirection = LogicalDirection.Backward;
                this.SelectionEndDistance = wrapper.SelectionStart + wrapper.SelectionLength;
                this.SelectionMovingDirection = LogicalDirection.Backward;

                //Clear the Clipboard.
                Clipboard.Clear();
            }
            else
            {
                // TextBoxBase-specific properties.
                this.AcceptsReturn = (wrapper.Element is TextBoxBase) ?
                    ((TextBoxBase)wrapper.Element).AcceptsReturn : false;
                this.IsReadOnly = (wrapper.Element is TextBoxBase) ?
                    ((TextBoxBase)wrapper.Element).IsReadOnly : false;

                this.SelectionLength = wrapper.GetSelectedText(false, false).Length;
                this.SelectionStartDirection = wrapper.SelectionInstance.Start.LogicalDirection;
                this.SelectionStartDistance = TextUtils.GetDistanceFromStart(wrapper.SelectionInstance.Start);
                this.SelectionEndDirection = wrapper.SelectionInstance.End.LogicalDirection;
                this.SelectionEndDistance = TextUtils.GetDistanceFromStart(wrapper.SelectionInstance.End);
                this.SelectionMovingDirection = wrapper.SelectionMovingPointer.LogicalDirection;
                this.SelectionEnd = wrapper.SelectionInstance.End;
                this.SelectionStart = wrapper.SelectionInstance.Start;
                this.SelectionXAMLtext = XamlUtils.TextRange_GetXml(wrapper.SelectionInstance);

                this.HorizontalOffset = ((TextBoxBase)wrapper.Element).HorizontalOffset;
                this.lineIndexAtSelectionStart = wrapper.LineNumberOfTextPointer(wrapper.SelectionInstance.Start);

                //FontSize
                if (wrapper.SelectionInstance.GetPropertyValue(TextElement.FontSizeProperty) == DependencyProperty.UnsetValue)
                {
                    this.SelectionFontSize = double.NaN;
                }
                else
                {
                    this.SelectionFontSize = (double)wrapper.SelectionInstance.GetPropertyValue(TextElement.FontSizeProperty);
                }
                this.SelectionStartFontSize = (double)wrapper.SelectionInstance.Start.Parent.GetValue(TextElement.FontSizeProperty);

                this.SelectionStartFontWeight = (FontWeight)wrapper.SelectionInstance.Start.Parent.GetValue(TextElement.FontWeightProperty);
                this.SelectionStartFontStyle = (FontStyle)wrapper.SelectionInstance.Start.Parent.GetValue(TextElement.FontStyleProperty);
                this.SelectionStartTextDecoration = (TextDecorationCollection)wrapper.SelectionInstance.Start.Parent.GetValue(Inline.TextDecorationsProperty);

                this.VerticalOffset = ((TextBoxBase)wrapper.Element).VerticalOffset;
                this.SelectionEndtRelatedToControl = wrapper.GetElementRelativeCharacterRect(SelectionEnd, 0, LogicalDirection.Forward);
                this.SelectionStartRelatedToControl = wrapper.GetElementRelativeCharacterRect(SelectionStart, 0, LogicalDirection.Forward);
                this.XamlText = (wrapper.IsElementRichText) ? wrapper.XamlText : null;

                //Gets the content before the selection and after the selection
                GetInitialAndEndStrings();

                if (this.Wrapper.Element is RichTextBox)
                {
                    GetXamlForUnselectedParts();
                }

                //This should be called after the seleciton is captured.
                CalculateDeleteIndex();
                CalculateBackspaceIndex();
                CaptureMargins();
            }
        }



        #endregion Constructors.

        #region private

        /// <summary>
        /// this method calculate the start and end index for backspace key.
        /// </summary>
        private void CalculateBackspaceIndex()
        {
            TextPointer tp1;
            if (SelectionLength != 0)
            {
                tp1 = Wrapper.Start.GetPositionAtOffset(SelectionStartDistance);

                //Note Selection Start/End may not at a insertion location, We need to do a little adjustment.
                BackSpaceIndexStart = TextUtils.GetDistanceFromStart(tp1);

                tp1 = Wrapper.Start.GetPositionAtOffset(SelectionEndDistance);

                //Note Selection Start/End may not at a insertion location, We need to do a little adjustment.
                if (!tp1.IsAtInsertionPosition)
                {
                    tp1 = tp1.GetInsertionPosition(LogicalDirection.Forward);
                }
                BackSpaceIndexEnd = TextUtils.GetDistanceFromStart(tp1);
            }
            else
            {
                if (Wrapper.IsCaretAtBeginningOfDocument)
                {
                    BackSpaceIndexStart = BackSpaceIndexEnd = -1;
                }
                else
                {
                    tp1 = Wrapper.SelectionInstance.Start;
                    tp1 = tp1.GetNextInsertionPosition(LogicalDirection.Backward);

                    BackSpaceIndexStart = TextUtils.GetDistanceFromStart(tp1);

                    BackSpaceIndexEnd = SelectionStartDistance;
                }
            }
        }

        /// <summary>
        /// This method calculate the start/end index for delete key.
        /// </summary>
        private void CalculateDeleteIndex()
        {
            TextPointer tp1;
            if (SelectionLength != 0)
            {
                //find selection start index.
                tp1 = Wrapper.Start.GetPositionAtOffset(SelectionStartDistance);

               DeleteIndexStart = TextUtils.GetDistanceFromStart(tp1);

                //Note Selection end may not at a insertion point. We need to do a little adjustment.
               tp1 = Wrapper.Start.GetPositionAtOffset(SelectionEndDistance);
               if (!tp1.IsAtInsertionPosition)
               {
                   tp1 = tp1.GetInsertionPosition(LogicalDirection.Forward);
               }
               DeleteIndexEnd = TextUtils.GetDistanceFromStart(tp1);
            }
            else
            {
                if(Wrapper.IsCaretAtEndOfDocument)
                {
                    DeleteIndexEnd = DeleteIndexStart = -1;
                }
                else
                {
                    tp1 = Wrapper.SelectionInstance.Start;
                    tp1 = tp1.GetNextInsertionPosition(LogicalDirection.Forward);

                    DeleteIndexStart = SelectionStartDistance;
                    DeleteIndexEnd = TextUtils.GetDistanceFromStart(tp1);
                }
            }
        }

        /// <summary>Rembember the initial Margins.</summary>
        private void CaptureMargins()
        {
            int lines;
            TextPointer pointer;

            if(!(Wrapper.Element is RichTextBox))
            {
                return;
            }

            lines = Wrapper.LineNumberOfTextPointer(Wrapper.End);

            // Note ParagraphMargines[0] is not used.
            BlockMargines = new Thickness[lines + 1];

            pointer = Wrapper.Start;
            if (!pointer.IsAtInsertionPosition)
            {
                pointer = pointer.GetInsertionPosition(LogicalDirection.Forward);
            }

            for (int i = 1; i <= lines; i++)
            {
                Block block;

                block = Wrapper.GetBlockParentForTextPointer(pointer);
                if (block == null)
                {
                    continue;
                }

                BlockMargines[i] = block.Margin;

                if (i < lines)
                {
                    pointer = pointer.GetLineStartPosition(1);

                    if (!pointer.IsAtInsertionPosition)
                    {
                        pointer = pointer.GetInsertionPosition(LogicalDirection.Forward);
                    }
                }
            }
        }

        #endregion

        #region Internal fields.

        internal bool AcceptsReturn;
        internal ArrayTextContainer Container;
        internal UIElementWrapper Wrapper;
        internal string Text;
        internal bool IsReadOnly;
        internal string XamlText;
        internal bool IsKeyboardFocused;

        /// <summary>Whether the tested element is a PasswordBox.</summary>
        internal bool IsPasswordBox;

        /// <summary>Whether Overtype (Ins) is enabled on the control.</summary>
        internal bool IsOvertypeModeEnabled;

        /// <summary>Position of caret relative to text-rendering control (adorned element).</summary>
        internal Point CaretPosition;

        /// <summary>Backspace Start Index </summary>
        internal int BackSpaceIndexStart;

        /// <summary>Backspace end index </summary>
        internal int BackSpaceIndexEnd;

        /// <summary>Delete Action Start index </summary>
        internal int DeleteIndexStart;

        /// <summary>Delete action End index</summary>
        internal int DeleteIndexEnd;

        /// <summary>GetXamlForUnselectedParts</summary>
        internal void GetXamlForUnselectedParts()
        {
            TextRange _tRange = new TextRange(this.Wrapper.Start, this.SelectionStart);
            _XamlForPartBeforeSelection =XamlUtils.TextRange_GetXml(_tRange);
            _tRange = new TextRange(this.SelectionEnd, this.Wrapper.End);
            _XamlForPartAfterSelection =XamlUtils.TextRange_GetXml( _tRange);
        }

        /// <summary>GetInitialAndEndStrings</summary>
        internal void GetInitialAndEndStrings()
        {
            TextRange _tRange = new TextRange(this.Wrapper.Start, this.SelectionStart);
            this.PartBeforeSelection = _tRange.Text;
            _tRange = new TextRange(this.SelectionEnd, this.Wrapper.End);
            this.PartAfterSelection = _tRange.Text;
        }

        /// <summary>Initlal Horizontal Offset</summary>
        internal double HorizontalOffset;

        internal int lineIndexAtSelectionStart;

        /// <summary>Logical direction of Selection.Start.</summary>
        internal LogicalDirection SelectionStartDirection;

        internal int SelectionLength;

        /// <summary>Number of positions between document start and selection start.</summary>
        internal int SelectionStartDistance;

        /// <summary>Number of positions between document start and selection end.</summary>
        internal int SelectionEndDistance;

        /// <summary>Number of positions between document start and moving end.</summary>
        internal int SelectionMovingDistance;

        /// <summary>Logical direction of the moving end of the selection.</summary>
        internal LogicalDirection SelectionMovingDirection;

        /// <summary>Logical direction of Selection.End.</summary>
        internal LogicalDirection SelectionEndDirection;

        /// <summary>_partBeforeSelection.</summary>
        internal string PartBeforeSelection;

        /// <summary>_partAfterSelection.</summary>
        internal string PartAfterSelection;

        /// <summary>Initial selection Start</summary>
        internal TextPointer SelectionStart;

        /// <summary>Initlal Selection End</summary>
        internal TextPointer SelectionEnd;

        /// <summary>Location of the Selection Start related to the Control</summary>
        internal Rect SelectionStartRelatedToControl;

        /// <summary>Selected Text</summary>
        internal String SelectedText;
        /// <summary>Location of the selection end related to the Control</summary>
        internal Rect SelectionEndtRelatedToControl;

        /// <summary>Selection font size</summary>
        internal double SelectionFontSize;

        /// <summary>Fontsize at SelectionStart</summary>
        internal double SelectionStartFontSize;

        /// <summary>Font style at SelectionStart</summary>
        internal FontStyle SelectionStartFontStyle;

        /// <summary>Font weight at SelectionStart</summary>
        internal FontWeight SelectionStartFontWeight;

        /// <summary>TextDecorations at SelectionStart</summary>
        internal TextDecorationCollection SelectionStartTextDecoration;

        /// <summary>Xaml of selected text</summary>
        internal string SelectionXAMLtext;

        /// <summary>Initlal Vertical Offset</summary>
        internal double VerticalOffset;

        internal string _XamlForPartBeforeSelection;
        internal string _XamlForPartAfterSelection;

        internal Thickness[] BlockMargines;

        #endregion Internal fields.
    }
}
