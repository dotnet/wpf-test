// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define DRTBASE_EXT_ARGUMENT_SUPPORT
//
//
//
// Description: DRT for InkCanvas and InkEditor.
//

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DRT
{
    public sealed class DrtInk : DrtBase
    {
        static DrtBase drt = null;
        [STAThread]
        public static int Main(string[] args)
        {
            drt = new DrtInk();
            int result = drt.Run(args);
            drt.Dispose();
            return result;
        }

        public static DrtInk GetDrt()
        {
            return (DrtInk)drt;
        }

        System.Collections.Hashtable _options = new System.Collections.Hashtable();
        private DrtInk()
        {
            WindowTitle = "Tablet Ink DRT";
            Contact = "Microsoft";
            TeamContact = "WPF";
            DrtName = "DrtInk";

            DrtTestSuite[] suites = new DrtTestSuite[] { 
                                            new DrtInkTestSuite(),
                                            new DrtInkExceptionHardeningTests() };
            Suites = suites;
            foreach (DrtTestSuite suite in suites)
            {
                DrtInkTestSuite inkSuite = suite as DrtInkTestSuite;
                if (inkSuite != null)
                {
                    inkSuite.Options = _options;
                }
            }

        }
    }
}

