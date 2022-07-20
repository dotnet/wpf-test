// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Definition of a selection relative to a Figure.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
    /// <summary>
    /// Definition of a selection relative to a Figure or Floater.
    /// </summary>
    public class AnchoredBlockSelectionData : ISelectionData
    {
        public AnchoredBlockSelectionData(Type blockType, string name, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            _blockType = blockType;
            _name = name;
            _startPosition = startPosition;
            _startOffset = startOffset;
            _endPosition = endPosition;
            _endOffset = endOffset;
        }

        protected override TextRange DoSetSelection(SelectionModule selectionModule)
        {
            return selectionModule.AnchoredBlocks.Select(_blockType, _name, _startPosition, _startOffset, _endPosition, _endOffset);
        }

        Type _blockType;
        string _name;
        PagePosition _startPosition, _endPosition;
        int _startOffset, _endOffset;
    }
}
