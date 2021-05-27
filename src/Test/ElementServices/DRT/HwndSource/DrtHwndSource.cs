// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace DRT
{
    public sealed class HwndSourceDrt : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new HwndSourceDrt();
            return drt.Run(args);
        }

        private HwndSourceDrt()
        {
            DrtName = "DrtHwndSource";
            TeamContact = "Element Services";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                     new SizeToContentSuite(),
                     new WindowClassSuite(),
#if TESTBUILD_NET_ATLEAST_46
                     new PerPixelTransparencySuite(),
#endif
                     };
        }
    }
}
