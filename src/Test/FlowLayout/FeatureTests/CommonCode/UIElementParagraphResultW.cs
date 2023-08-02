// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Test.Layout {
    public class UIElementParagraphResultW: ParagraphResultW {
        public UIElementParagraphResultW(object uIElementParagraphResult):
            base(uIElementParagraphResult, "MS.Internal.Documents.UIElementParagraphResult")
        {
        }
        
        public UIElement Element { 
            get { return (UIElement)GetProperty("Element"); } 
        }
    }   
}