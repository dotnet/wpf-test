// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using System.Windows.Media.Animation;

namespace DRT
{
    public sealed class TimingDrt : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new TimingDrt();
            return drt.Run(args);
        }

        private TimingDrt()
        {
            DrtName = "DrtTiming";
			TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                     new TimingSuite()
                     };
        }
    }
}
