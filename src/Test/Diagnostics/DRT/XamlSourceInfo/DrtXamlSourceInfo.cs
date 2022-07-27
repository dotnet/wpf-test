// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: DRT tests for xaml source info.
//

using System;

namespace DRT
{
    public class DrtXamlSourceInfo : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtXamlSourceInfo drt = new DrtXamlSourceInfo();
            return drt.Run(args);
        }

        private DrtXamlSourceInfo()
        {
            WindowTitle = "XamlSourceInfo DRT";
            DrtName = "DrtXamlSourceInfo";
            TeamContact = "Wpf";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                        new DrtXamlSourceInfoSuite(),
                        null};
        }
    }
}
