// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;



namespace Microsoft.Test.Layout
{

    public class TextContainerW : ReflectionHelper
    {
        static BindingFlags s_FULL_ACCESS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


        public static TextContainerW FromTextPointer(TextPointer fe)
        {
            Type textPointerType = typeof(TextPointer);
            PropertyInfo textContainerPropInfo = textPointerType.GetProperty("TextContainer", s_FULL_ACCESS);
            return new TextContainerW(textContainerPropInfo.GetValue(fe, null));
        }

        public TextContainerW(object textContainer)
            : base(textContainer)
        {
        }

        public TextPointer StartPosition
        {
            get { return GetProperty("Start", s_FULL_ACCESS) as TextPointer; }
        }

        public TextPointer EndPosition
        {
            get { return GetProperty("End", s_FULL_ACCESS) as TextPointer; }
        }
    }
}