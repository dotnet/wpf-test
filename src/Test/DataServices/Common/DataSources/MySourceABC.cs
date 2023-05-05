// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

// This source is good to test binding to subproperties with dot notation. It allows the following path:
// Path = A.B.C

namespace Microsoft.Test.DataServices
{
    public class MySourceA
    {
        private MySourceB _a;

        public MySourceB A
        {
            get { return _a; }
            set { _a = value; }
        }

        public MySourceA()
        {
            this._a = new MySourceB(new MySourceC(3));
        }
    }

    public class MySourceB
    {
        private MySourceC _b;

        public MySourceC B
        {
            get { return _b; }
            set { _b = value; }
        }

        public MySourceB(MySourceC srcC)
        {
            this._b = srcC;
        }
    }

    public class MySourceC
    {
        private int _c;

        public int C
        {
            get { return _c; }
            set { _c = value; }
        }

        public MySourceC(int c)
        {
            this._c = c;
        }
    }
}
