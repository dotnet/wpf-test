// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Base class for text find DRTs. 
//
//

using System;
using System.Windows;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Base class for text find DRTs.
    // ----------------------------------------------------------------------
    internal class DrtTextFindBase : DrtBase
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected DrtTextFindBase()
        {
            _dumpMode = DumpModeType.Drt;
        }

        // ------------------------------------------------------------------
        // Handle command-line arguments one-by-one.
        // ------------------------------------------------------------------
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            // Process arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:
            if (option)
            {
                switch (arg)
                {
                    case "diff":
                        _dumpMode = DumpModeType.Diff;
                        break;
                    case "dump":
                        _dumpMode = DumpModeType.Dump;
                        break;
                    case "view":
                        _dumpMode = DumpModeType.View;
                        break;
                    default: // Unknown option. Don't handle it.
                        return false;
                }
                return true;
            }
            return false;
        }

        // ------------------------------------------------------------------
        // Print a description of command line arguments.
        // ------------------------------------------------------------------
        protected override void PrintOptions()
        {
            Console.WriteLine("TextFindDrt specific options:");
            Console.WriteLine("  [-diff | -dump | -view]");
            Console.WriteLine("                    diff - shows differences in windiff, if test dump does not match master");
            Console.WriteLine("                    dump - re-writes master dump files");
            Console.WriteLine("                    view - delays each test by 0.5 second");
            base.PrintOptions();
        }

        // ------------------------------------------------------------------
        // Drt mode.
        // ------------------------------------------------------------------
        internal DumpModeType DumpMode { get { return _dumpMode; } }
        private DumpModeType _dumpMode;
        internal enum DumpModeType { Drt, Diff, Dump, View }
    }
}
