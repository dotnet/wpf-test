// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Layout DRT for content presentation layouts.
//
//

using MS.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Layout DRT for content presentation layouts.
    // ----------------------------------------------------------------------
    internal class DrtSelection : DrtSelectionBase
    {
        // ------------------------------------------------------------------
        // Application entry point.
        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            int retVal; 
            DrtSelection drt = new DrtSelection();
            try
            {
                retVal = drt.Run(args);
            }
            finally
            {
                // Final cleanup allows DrtSelection to restore system wide settings it may have changed.
                drt.FinalCleanup();
            }

            return retVal;
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtSelection()
        {
            //this.AssertsAsExceptions = false;
            this.WindowSize = new Size(850, 650);
            this.WindowTitle = "Selection DRT";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "DrtSelection";

            // This changes system parameters.  Note this means parallel running with another DRT would be bad.
            // Some system metrics change based on OS/theme.  Get old settings.
            _ncm = new NativeMethods.NONCLIENTMETRICS();
            if (!UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, _ncm.cbSize, _ncm, 0))
            {
                _ncm = null;
                throw new Exception("Failed to get NONCLIENTMETRICS.");
            }

            // Change settings as necessary, but save old settings.
            int iScrollWidth = _ncm.iScrollWidth;
            int iScrollHeight = _ncm.iScrollHeight;
            if ((iScrollWidth != _scrollDimension) || (iScrollHeight != _scrollDimension))
            {
                _ncm.iScrollWidth = _ncm.iScrollHeight = _scrollDimension;
                if (!UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_SETNONCLIENTMETRICS, _ncm.cbSize, _ncm, 0))
                {
                    _ncm = null;
                    throw new Exception("Failed to set NONCLIENTMETRICS to test values.");
                }

                _ncm.iScrollWidth = iScrollWidth;
                _ncm.iScrollHeight = iScrollHeight;
            }
            else
            {
                _ncm = null; // No change needed
            }

            this.Suites = new DrtTestSuite[] {
                new TextParagraphViewSuite(),
                new TextDocumentViewSuite(),
/*
                new DocumentPageTextViewSuite(),
                new MultiPageTextViewSuite(),
 */
                new BugSuite(),
                new NoBleedSuite(),
            };
        }

        private void FinalCleanup()
        {
            if (    _ncm != null
                &&  !UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_SETNONCLIENTMETRICS, _ncm.cbSize, _ncm, 0))
            {
                _ncm = null;
                throw new Exception("Failed to restore NONCLIENTMETRICS to pretest values.");
            }
        }

        private NativeMethods.NONCLIENTMETRICS _ncm;
        private const int _scrollDimension = 17;
    }
}
