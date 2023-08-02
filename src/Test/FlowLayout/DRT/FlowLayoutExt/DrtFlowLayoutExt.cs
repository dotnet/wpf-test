// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Layout DRT for content presentation layouts.
//
//

using System;
using System.Windows;
using System.Windows.Media;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Layout DRT for content presentation layouts.
    // ----------------------------------------------------------------------
    internal class DrtFlowLayoutExt : DrtLayoutBase
    {
        // ------------------------------------------------------------------
        // Application entry point.
        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtBase drt = new DrtFlowLayoutExt();
            return drt.Run(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtFlowLayoutExt()
        {
            TextContainer.Initialize(this);
            TextView.Initialize(this);

            this.WindowSize = new Size(850, 650);
            this.WindowTitle = "Flow Layout DRT";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "DrtFlowLayoutExt";
            this.LoudAsserts = false;   // Display all failures that happen in the end of the test.

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type typeBackgroundFormatInfo = assembly.GetType("MS.Internal.PtsHost.BackgroundFormatInfo");
            System.Reflection.FieldInfo fieldInfo = typeBackgroundFormatInfo.GetField("_isBackgroundFormatEnabled", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            fieldInfo.SetValue(null, false);

            this.Suites = new DrtTestSuite[] {
                new TextPanelStaticSuite(),
                //new TextPanelColumnsSuite(),
                new TextPanelFiguresSuite(),
                new TableBugsSuite(),
                new TableBugs2Suite(),
                new TableCellAlignmentSuite(),
                new TableColumnsSuite(),
                new TableDynamicSuite(),
                new TableNestingSuite(),
                new TableOMSuite(),
                new TablePaginationBasicSuite(),
                new TablePaginationRowSpanSuite(),
                new TableSpansSuite(),
                new TableStyleInheritanceSuite(),
                new TableTasksSuite(),
                new TableXamlSuite(),
                //new TextViewMultiPageSuite(),
                new TextViewTextBlockSuite(),
            };
        }
    }
}
