// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace Microsoft.Test.Layout {
    public class TableParagraphResultW: ParagraphResultW {
        public TableParagraphResultW(object tableParagraphResult):
            base(tableParagraphResult, "MS.Internal.Documents.TableParagraphResult")
        {
        }
        
        public LineResultListW Lines { 
            get { 
                IEnumerable lines = (IEnumerable)GetProperty("Lines");
                return new LineResultListW(lines); 
            } 
        }
        
        public ReadOnlyCollection<UIElement>  Floaters { 
            get { return (ReadOnlyCollection<UIElement> )GetProperty("Floaters"); } 
        }
        
        public ReadOnlyCollection<UIElement>  Figures { 
            get { return (ReadOnlyCollection<UIElement> )GetProperty("Figures"); } 
        }
    }   
}