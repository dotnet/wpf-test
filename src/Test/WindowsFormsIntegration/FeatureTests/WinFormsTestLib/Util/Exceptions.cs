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
