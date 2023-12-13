// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace WFCTestLib {
    public class WinFormsTestLibException : ApplicationException {
        public WinFormsTestLibException() : base() { }
        public WinFormsTestLibException(string s) : base(s) { }
        public WinFormsTestLibException(string s, Exception e) : base(s, e) { }
    }
}
