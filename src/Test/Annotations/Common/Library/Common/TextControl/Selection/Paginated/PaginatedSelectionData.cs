// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Selection definition specific to paginated content.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
	public abstract class PaginatedSelectionData : ISelectionData
	{
		protected override TextRange DoSetSelection(SelectionModule selectionModule)
		{
            ADocumentViewerBaseSelector selector = selectionModule as ADocumentViewerBaseSelector;
            if (selector == null)
                throw new ArgumentException("PaginatedSelectionData objects can only be used on paginated content selectors.");
            return DoSetSelection(selector);
		}

        abstract protected TextRange DoSetSelection(ADocumentViewerBaseSelector selector);
    }
}
