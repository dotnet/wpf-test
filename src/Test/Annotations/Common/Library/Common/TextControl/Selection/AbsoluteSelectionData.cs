// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Defines a selection in absolute offsets.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
    public class AbsoluteSelectionData : ISelectionData
    {
        public AbsoluteSelectionData(int startOffset, int endOffset)
        {
            _mode = Mode.OffsetsOnly;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        public AbsoluteSelectionData(PagePosition startPosition, int endOffset)
        {
            _mode = Mode.SinglePosition;
            StartPosition = startPosition;
            EndOffset = endOffset;
        }

        public AbsoluteSelectionData(PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            _mode = Mode.PositionTuple;
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartOffset = startOffset;
            EndOffset = endOffset;
        }

        protected override TextRange DoSetSelection(SelectionModule selectionModule)
        {
            TextRange result;
            switch (_mode)
            {
                case Mode.OffsetsOnly:
                    result = selectionModule.SetSelection(StartOffset, EndOffset);
                    break;
                case Mode.SinglePosition:
                    result = selectionModule.SetSelection(StartPosition, EndOffset);
                    break;
                case Mode.PositionTuple:
                    result = selectionModule.SetSelection(StartPosition, StartOffset, EndPosition, EndOffset);
                    break;
                default:
                    throw new NotImplementedException(_mode.ToString());
            }
            return result;
        }

        private PagePosition StartPosition, EndPosition;
        private int StartOffset, EndOffset;
        private Mode _mode;

        enum Mode
        {
            OffsetsOnly,
            SinglePosition,
            PositionTuple
        }
    }
}
