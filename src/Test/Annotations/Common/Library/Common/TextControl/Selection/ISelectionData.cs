// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
    /// <summary>
    /// Interface for a class that represents a text selection.
    /// </summary>
    public abstract class ISelectionData
    {
        /// <summary>
        /// Call SetSelection on the selectionModule passing selection data this this object represents.
        /// </summary>
        public string SetSelection(SelectionModule selectionModule)
        {
            return DoSetSelection(selectionModule).Text;
        }

        public string GetSelection(SelectionModule selectionModule)
        {
            return GetSelectionAsTextRange(selectionModule).Text;
        }

        public TextRange GetSelectionAsTextRange(SelectionModule selectionModule)
        {
            selectionModule.WriteThrough = false;
            TextRange range = DoSetSelection(selectionModule);
            selectionModule.WriteThrough = true;
            return range;
        }

        protected abstract TextRange DoSetSelection(SelectionModule selectionModule);
    }
}
