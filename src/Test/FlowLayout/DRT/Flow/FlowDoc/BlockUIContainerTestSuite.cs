// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test suite for BlockUIContainer.
//

using System.Windows;               
using System.Windows.Controls;  // FlowDocumentScrollViewer
using System.Windows.Documents;  // FlowDocument
using System.Windows.Media;  // Brushes


namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for BlockUIContainer.
    // ----------------------------------------------------------------------
    internal sealed class BlockUIContainerTestSuite: LayoutDumpTestSuite
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal BlockUIContainerTestSuite() : base("BlockUIContainer")
        {
             this.Contact = "Microsoft";
        }

        /// <summary>
        /// Create tests
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(CreateEmptyContainer),          new DrtTest(DumpCreate),
                new DrtTest(LoadContent),                   new DrtTest(DumpAppend),
                new DrtTest(ResizeUIElements),              new DrtTest(DumpAppend),
                new DrtTest(ChangeFlowDirection),                 new DrtTest(DumpFinalizeAndVerify)
            };
        }

        /// <summary>
        /// Create empty BlockUIContainer
        /// </summary>
        private void CreateEmptyContainer()
        {
            _fdsv = new FlowDocumentScrollViewer();
            _fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _fdsv.Document = new FlowDocument();
            _fdsv.Document.TextAlignment = TextAlignment.Left;
            _fdsv.Document.PagePadding = new Thickness(0);
            this.Root.Background = Brushes.Red;

            BlockUIContainer buic = new BlockUIContainer();
            _fdsv.Document.Blocks.Add(buic);
            this.Root.Child = _fdsv;
        }

        /// <summary>
        /// Load content from XAML.
        /// </summary>
        private void LoadContent()
        {
            _fdsv = LoadFromXaml("BlockUIContainer.xaml") as FlowDocumentScrollViewer;
            DRT.Assert(_fdsv != null, "Cannot load BlockUIContainer.");
            this.Root.Child = _fdsv;
        }

        private void ResizeUIElements()
        {
            Border border1 = DRT.FindElementByID("border1", _fdsv) as Border;
            border1.Height = 400;
            border1.Width = 250;

            Border border2 = DRT.FindElementByID("border2", _fdsv) as Border;
            border1.Height = 50;
            border1.Width = 50;
        }

        private void ChangeFlowDirection()
        {
            BlockUIContainer buic1 = DRT.FindElementByID("buic1", _fdsv) as BlockUIContainer;
            buic1.FlowDirection = FlowDirection.RightToLeft;

            BlockUIContainer buic2 = DRT.FindElementByID("buic2", _fdsv) as BlockUIContainer;
            buic2.FlowDirection = FlowDirection.RightToLeft;

            BlockUIContainer buic6 = DRT.FindElementByID("buic6", _fdsv) as BlockUIContainer;
            buic6.FlowDirection = FlowDirection.RightToLeft;

            Figure figure1 = DRT.FindElementByID("figure1", _fdsv) as Figure;
            figure1.FlowDirection = FlowDirection.RightToLeft;

            Floater floater1 = DRT.FindElementByID("floater1", _fdsv) as Floater;
            floater1.FlowDirection = FlowDirection.RightToLeft;
            BlockUIContainer buic5 = DRT.FindElementByID("buic5", _fdsv) as BlockUIContainer;
            buic5.FlowDirection = FlowDirection.LeftToRight;
        }

        private FlowDocumentScrollViewer _fdsv;
    }
}
