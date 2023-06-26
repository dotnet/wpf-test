// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace XamlPadEdit
{
    public sealed class ParseResult
    {
        public object Root;
        public string ErrorMessage;
        public int ErrorLineNumber;
        public int ErrorPosition;

        public ParseResult()
        {
            Root = null;
            ErrorMessage = null;
            ErrorLineNumber = 0;
            ErrorPosition = 0;
        }
    }
}