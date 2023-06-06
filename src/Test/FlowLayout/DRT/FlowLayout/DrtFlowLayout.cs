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

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually. 
// Another option would be to use pack://siteoforigin:,,,/ site of origin syntax at each file 
// reference.
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/flowbugsmirroring.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/flowdocumentpageicontenthosthostedelements.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/flowdocumentpageicontenthostonchilddesiredsizechanged.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/flowdocumentpageicontenthostrectangles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/flowdocumentsample1.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/hyperlinkpage1.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/hyperlinkpage2.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/hyperlinkpage3.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/masterflowbugsmirroring.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/masterpaginationbasic.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertablebasic.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextdynamicchanges.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextlineheight.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextmisc.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpaneldynamicchanges.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelfloatlinesizing.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelincrementalcomplex.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelincrementalsimple.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanellineheight.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanellists.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelmargincollapsing.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelmbp.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelmisc.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelparagraphs.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelrtl.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelrtlcomplex.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelsimple.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextpanelsizing.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextrtl.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextsimple.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertextsizing.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/mastertexttrimming.xml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textblockrendertrailingspaceshittest.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textblockrendertrailingspacesrectangles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textflowrendertrailingspaceshittest.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textflowrendertrailingspacesrectangles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/texticontenthosthostedelements.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/texticontenthostrectangles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textlineheight.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textmisc.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelfloatlinesizing.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelicontenthosthostedelements.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelicontenthostrectangles.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanellineheight.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanellists.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelmargincollapsing.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelmbp.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelmisc.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelparagraphs.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelrtl.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelrtlcomplex.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelsimple.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textpanelsizing.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textrtl.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textsimple.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/textsizing.xaml")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/flowlayout/texttrimming.xaml")]


namespace DRT
{
    // ----------------------------------------------------------------------
    // Layout DRT for content presentation layouts.
    // ----------------------------------------------------------------------
    internal class DrtFlowLayout : DrtLayoutBase
    {
        // ------------------------------------------------------------------
        // Application entry point.
        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtBase drt = new DrtFlowLayout();
            return drt.Run(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtFlowLayout()
        {
            //this.AssertsAsExceptions = false;
            this.WindowSize = new Size(850, 650);
            this.WindowTitle = "Flow Layout DRT";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "DrtFlowLayout";
            this.LoudAsserts = false;   // Display all failures that happen in the end of the test.

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type typeBackgroundFormatInfo = assembly.GetType("MS.Internal.PtsHost.BackgroundFormatInfo");
            System.Reflection.FieldInfo fieldInfo = typeBackgroundFormatInfo.GetField("_isBackgroundFormatEnabled", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            fieldInfo.SetValue(null, false);

            this.Suites = new DrtTestSuite[] {
                new DrtHyperlinkTestSuite(),
                new TextStaticSuite(),
                new TextDynamicChangesSuite(),
                new TextPanelStaticSuite(),
                new TextPanelDynamicChangesSuite(),
                //new TextPanelIncrementalSimpleSuite(),
                //new TextPanelIncrementalComplexSuite(),
                new TableBasicSuite(),
                new PaginationBasicSuite(),
                new PaginationApiSuite(),
                new TextIContentHostSuite(),
                new TextPanelIContentHostSuite(),
                new FlowDocumentPageIContentHostSuite(),
                new FlowBugsSuite(),
                //new ContentElementEventsSuite(), moved to DrtFlow
                new TextBlockRenderTrailingSpacesSuite(),
                new TextFlowRenderTrailingSpacesSuite(),
                new TextElementsSuite(),
                new InlineFlowDirectionSuite(),
            };
        }
    }
}
