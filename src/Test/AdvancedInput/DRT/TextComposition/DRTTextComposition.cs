// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Automation;

namespace DRT
{
    public class DrtTextComposition : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtTextComposition();

            ((DrtTextComposition)drt).WindowSize = new Size(800,600);
            drt.Run(args);
            
            Console.WriteLine( "Passed" );
            return 0;
        }

        //
        // constructor
        //
        DrtTextComposition()
        {
            WindowTitle = "TextComposition Drt";
            TeamContact = "WPF";
            Contact= "Microsoft";
            DrtName= "DRTTextComposition";
            WindowSize = new Size(600, 200);

            WarningMismatchedForeground = WarningLevel.Error;
            
            Suites = new DrtTestSuite[] 
                        {   
                            new TextCompositionTestSuite(),
                            new AltNumpadTestSuite(),
                            new DeadCharTestSuite(),
                            null
                        };
        }

        // 

        
    }
}

