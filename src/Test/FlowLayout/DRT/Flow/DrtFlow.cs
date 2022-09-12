// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Flow DRT.
//

using System;                       // string
using System.Windows;               // Size

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually.
// Another option would be to use pack://siteoforigin:,,,/ site of origin syntax at each file
// reference.
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/customstyles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/flowdocumenttoc.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/flowdocumentplain.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/flowdocumentcomplex.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/flowdocumentevents.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flow/blockuicontainer.xaml")]

namespace DRT
{
    /// <summary>
    /// Flow DRT.
    /// </summary>
    internal class DrtFlow : DrtFlowBase
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtBase drt = new DrtFlow();
            return drt.Run(args);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private DrtFlow()
        {
            this.WindowSize = new Size(850, 690);
            this.WindowTitle = "Flow DRT";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "DrtFlow";
            this.LoudAsserts = false;   // Display all failures that happen in the end of the test.

            this.Suites = TestSuites;
        }

        /// <summary>
        /// Test suites for this DRT.
        /// </summary>
        internal override DrtTestSuite[] TestSuites
        {
            get
            {
                return new DrtTestSuite[] {
                    // ScrollBar width
                    new ScrollBarWidthTestSuite(),
                    // FlowDocument
                    // new FlowDocUIHostingTestSuite(),
                    // Viewers
                    new PageViewerTestSuite(),
                    new PageViewerNavTestSuite(),
                    new ScrollViewerTestSuite(),
                    new ScrollViewerNavTestSuite(),
                    new ReaderViewerTestSuite(),
                    new ReaderViewerNavTestSuite(),
                    new ViewerBringIntoViewTestSuite(),
                    new ViewerEventRoutingTestSuite(),
                    new ViewerFlowDirectionTestSuite(),
                    new CustomViewerTestSuite(),
                    // UIAutomation
                    new FlowDocTextPatternTestSuite(),
                    new DocumentPeerTestSuite(),
                    new ReaderPeerTestSuite(),
                    new BlockUIContainerTestSuite(),
                };
            }
        }

        /// <summary>
        /// Location of all DRT related files.
        /// </summary>
        internal override string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\Flow\\"; }
        }

        /// <summary>
        /// SD location of DRT files.
        /// </summary>
        internal override string DrtSDDirectory
        {
            get { return "windows\\Wcp\\DevTest\\Drts\\Flow\\"; }
        }
    }

    internal class ScrollBarWidthTestSuite : DrtTestSuite
    {
        internal ScrollBarWidthTestSuite() : base("ScrollBarWidth")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] {
                new DrtTest(TestScrollBarWdith),
            };
        }

        private void TestScrollBarWdith()
        {
            int width = (int)SystemParameters.VerticalScrollBarWidth;

            if (width != 17)
            {
                string s = String.Format("The Vertical Scrollbar Width on this system is {0}, but the master\n" +
                    "files assume 17.  Many of the tests in this DRT will fail.\n" +
                    "Please change the scrollbar width to 17 and try again." +
                    "(The steps for changing scrollbar width depend on your OS.)", width);
                DRT.Assert(width == 17, s);
            }
        }
    }
}
