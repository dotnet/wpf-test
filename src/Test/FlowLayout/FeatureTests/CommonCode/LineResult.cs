// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Reflection;

namespace Microsoft.Test.Layout
{
    public class LineW : ReflectionHelper
    {
        public LineW(object line)
            :
            base(line, "MS.Internal.Text.Line")
        {
        }

        public TextLine GetLine()
        {
            return (TextLine)GetField("_line");
        }

        public double Width
        {
            get { return (double)GetProperty("Width"); }
        }
    }
    
    public class LineMetricsW : ReflectionHelper
    {
        public LineMetricsW(object lineprop)
            :
            base(lineprop, "MS.Internal.Text.LineMetrics")
        {
        }

        public TextLineBreak TextLineBreak
        {
            get { return (TextLineBreak)GetField("_textLineBreak", BindingFlags.Instance | BindingFlags.NonPublic); }
        }

        public Int32 Length
        {
            get { return (Int32)GetProperty("Length", BindingFlags.Instance | BindingFlags.NonPublic); }
            
        }

        public double Height
        {
            get { return (double)GetField("_height", BindingFlags.Instance | BindingFlags.NonPublic); }
        }
    }

    public class TextBlockCacheW : ReflectionHelper
    {
        public TextBlockCacheW(object obj)
            :
            base(obj, "System.Windows.Controls.TextBlockCache")
        {
        }

        public TextRunCache TextRunCache
        {
            get { return (TextRunCache)GetField("_textRunCache", BindingFlags.Instance | BindingFlags.Public); }
        }

        public object LineProperties
        {
            get { return GetField("_lineProperties", BindingFlags.Instance | BindingFlags.Public); }
        }
    }

    public class LinePropertiesW : ReflectionHelper
    {
        public LinePropertiesW(object obj)
            :
            base(obj, "MS.Internal.Text.LineProperties")
        {
        }       
    }
}
