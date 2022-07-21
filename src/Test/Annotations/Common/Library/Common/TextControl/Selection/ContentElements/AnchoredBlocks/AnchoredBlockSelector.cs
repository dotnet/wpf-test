// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module for creating selection inside AnchoredBlocks in FlowDocuments.
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    public class AnchoredBlockSelector : FlowElementSelector
    {
        #region Constructor

        public AnchoredBlockSelector(SelectionModule selectionModule)
            : base(selectionModule)
        {
            // nothing.
        }

        #endregion

        #region Public Methods

        public TextRange Select(Type controlType, string name, PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset) 
        {
            ElementPosition figurePosition = FindElementWithName(controlType, name);
            return Select(figurePosition, startPosition, startOffset, endPosition, endOffset);            
        }

        #endregion
    }
}	
