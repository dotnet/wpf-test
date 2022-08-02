// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: DRT for annotations framework. 
//
//

using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Resources;

using DRT;


// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually. 
// Note that these strings should all be lower case.
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_1.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_2.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_3.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_4.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_5.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_6.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_7.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_8.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_9.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_10.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_11.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_12.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_13.png")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/image_14.png")]

[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_1.ttf")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_2.ttf")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_3.ttf")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_4.ttf")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_5.ttf")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfixededit_files/font_6.ttf")]


namespace DrtAnnotations
{
    /// <summary>
    /// Top-level DRT class for annnotation drts.  This class
    /// sets up the test suites and runs them.
    /// </summary>
    public sealed class DrtAnnotationsDrt : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtAnnotationsDrt drt = new DrtAnnotationsDrt();
            return drt.Run(args);
        }

        public bool UseExistingStores
        {
            get
            {
                return _useExistingStores;
            }
        }
        
        public bool Interactive
        {
            get
            {
                return _interactive;
            }
        }
        private DrtAnnotationsDrt()
        {
            DrtName = "DrtAnnotations";
            WindowTitle = "Annotations DRT";
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[]{
                        new PageViewerTestSuite()
                        };
        }

        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "i":               // -i means interactive
                        _interactive = true;
                        KeepAlive = true;
                        Suites = new DrtTestSuite[] {
                            new PageViewerTestSuite()
                            };
                        break;

                    case "s":               // -s means use existing stores
                        _useExistingStores = true;
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }

            return false;
        }

        private bool _interactive = false;
        private bool _useExistingStores = false;
    }
}

