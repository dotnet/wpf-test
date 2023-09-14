// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Base class for layout DRTs. 
//
//

using System;
using System.Windows;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Base class for layout DRTs.
    // ----------------------------------------------------------------------
    internal class DrtLayoutBase : DrtBase
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        protected DrtLayoutBase()
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
                    case "dump":
                        string mode = args[++k];
                        switch (mode)
                        {
                            case "diff":
                                _dumpMode = DumpModeType.Diff;
                                break;
                            case "update":
                                _dumpMode = DumpModeType.Update;
                                break;
                            default:
                                _dumpMode = DumpModeType.Drt;
                                break;
                        }
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
            Console.WriteLine("Options:");
            Console.WriteLine("  -dump [drt | diff | update]");
            Console.WriteLine("                    drt - assert, if master and test dumps dont match");
            Console.WriteLine("                    diff - show differences in windiff, if master and test dumps dont match");
            Console.WriteLine("                    update - update master dump files");
            base.PrintOptions();
        }

        // ------------------------------------------------------------------
        // Drt mode.
        // ------------------------------------------------------------------
        internal DumpModeType DumpMode { get { return _dumpMode; } }
        private DumpModeType _dumpMode;
        internal enum DumpModeType { Drt, Diff, Update }
    }
}
