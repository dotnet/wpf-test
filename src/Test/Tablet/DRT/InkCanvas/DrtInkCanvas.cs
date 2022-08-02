// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT for InkCanvas and InkEditor.
//

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRT
{
    public sealed class DrtInkCanvas : DrtTabletBase
    {
        private static DrtInkCanvas thisDrt;
        public bool SaveInk = false;

        [STAThread]
        public static int Main(string[] args)
        {
            thisDrt = new DrtInkCanvas();
            int result = thisDrt.Run(args);
            thisDrt.Dispose();
            return result;
        }
        
        private DrtInkCanvas() : base()
        {
            WindowTitle = "Tablet InkCanvas DRT";
            Contact     = "Microsoft";
            TeamContact = "WPF";
            DrtName     = "DrtInkCanvas";
            BaseDirectory = @".\DrtFiles\InkCanvas\";

            int nCount = System.Windows.Input.Tablet.TabletDevices.Count;

            Suites = new DrtTestSuite[]{
                        new InkCanvasTests(),
                        new InkCanvasEditingTests(),
                        new InkPresenterTests(),
                        new InkCanvasMidStrokeTests(),
                        new InkCanvasDataBindingTests(),
                        new InkCanvasExceptionHardeningTests(),
                        new InkCollectionPerfTest(),
                        new SelectionAPIPerfTest(),
                        new SelectionMovingPerfTest(),
                        new LassoSelectionPerfTest(),
                        new PointErasePerfTest(),
                        new StrokeErasePerfTest(),
                    };

            foreach ( DrtTestSuite suite in Suites )
            {
                // Disable the perf tests by default
                if ( suite is InkCanvasPerfTestBase )
                {
                    DisableSuite(suite.GetType().Name);
                }
            }
        }

        // Override this in derived classes to handle command-line arguments one-by-one.
        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if ( base.HandleCommandLineArgument(arg, option, args, ref k) )
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
                    case "saveink":         // simple boolean option:   -foo
                        SaveInk = true;
                        break;
                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }

            return false;
        }

        internal static IList<InkCanvasPerfTestBase> PerfSuites
        {
            get
            {
                List<InkCanvasPerfTestBase> perfSuites = new List<InkCanvasPerfTestBase>();
                if ( thisDrt != null )
                {
                    foreach ( DrtTestSuite suite in thisDrt.Suites )
                    {
                        // Disable the perf tests by default
                        if ( suite is InkCanvasPerfTestBase )
                        {
                            perfSuites.Add((InkCanvasPerfTestBase)suite);
                        }
                    }
                }

                return new ReadOnlyCollection<InkCanvasPerfTestBase>(perfSuites);
            }
        }

    }
}
