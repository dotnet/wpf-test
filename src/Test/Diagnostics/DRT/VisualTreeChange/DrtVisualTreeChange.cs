// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT tests for visual tree change notification.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace DRT
{
    public class DrtVisualTreeChange : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtVisualTreeChange drt = new DrtVisualTreeChange();
            return drt.Run(args);
        }

        private DrtVisualTreeChange()
        {
            DrtName = "DrtVisualTreeChange";
            //snasuasua;
            WindowTitle = "VisualTreeChange DRT";
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                        new DrtVisualTreeChangeSuite(),
                        null};
        }
    }
}
