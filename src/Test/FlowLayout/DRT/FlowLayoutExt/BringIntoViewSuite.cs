// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for BringIntoView scenarios. 
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for pagination.
    // ----------------------------------------------------------------------
    internal sealed class BringIntoViewSuite : DrtTestSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal BringIntoViewSuite() : base("BringIntoView")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Return the list of individual tests (i.e. callback methods).
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            // Initialize the suite here. This includes loading the tree.

            _root = new Border();
            _root.Width = 800;
            _root.Height = 600;
            _root.Background = Brushes.White;
            DRT.Show(_root);

            // Set a standard Font not based on system metrics
            TextElement.SetFontFamily(_root, new FontFamily("Arial"));
            TextElement.SetFontSize(_root, 10.0 * 96.0 / 72.0);
            TextElement.SetFontStyle(_root, FontStyles.Normal);
            TextElement.SetFontWeight(_root, FontWeights.Normal);

            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(LoadTextBlock),     new DrtTest(ScrollTextBlock),       new DrtTest(VerifyTextBlock),
                new DrtTest(LoadTextFlow),      new DrtTest(ScrollTextFlow),        new DrtTest(VerifyTextFlow),
            };
        }

        // ------------------------------------------------------------------
        // LoadTextBlock
        // ------------------------------------------------------------------
        private void LoadTextBlock()
        {
            _testName = "TextBlock";
            // Load content from xaml file
            ScrollViewer sv = new ScrollViewer();
            sv.Content = LoadContentFromXaml();
            DRT.Assert(sv.Content is TextBlock, "{0}: Failed: Expecting TextBlock as root element.", this.TestName);
            _root.Child = sv;
            // Scroll to top-left of content
            sv.ScrollToHome();
        }

        // ------------------------------------------------------------------
        // ScrollTextBlock
        // ------------------------------------------------------------------
        private void ScrollTextBlock()
        {
            FrameworkContentElement contentElement = DRT.FindElementByID("H1") as FrameworkContentElement;
            DRT.Assert(contentElement != null, "{0}: Failed: Cannot find element: {1}.", this.TestName, "H1");
            contentElement.BringIntoView();
        }

        // ------------------------------------------------------------------
        // VerifyTextBlock
        // ------------------------------------------------------------------
        private void VerifyTextBlock()
        {
            Vector expectedOffset = new Vector(0,841);
            double offsetX = ((ScrollViewer)_root.Child).HorizontalOffset;
            double offsetY = ((ScrollViewer)_root.Child).VerticalOffset;
            DRT.Assert(CompareOffsets(offsetX, expectedOffset.X), "{0}: Failed: Expecting horizontal offset {1}, got {2}.", this.TestName, expectedOffset.X, offsetX);
            DRT.Assert(CompareOffsets(offsetY, expectedOffset.Y), "{0}: Failed: Expecting vertical offset {1}, got {2}.", this.TestName, expectedOffset.Y, offsetY);
        }

        // ------------------------------------------------------------------
        // LoadTextFlow
        // ------------------------------------------------------------------
        private void LoadTextFlow()
        {
            _testName = "TextFlow";
            // Load content from xaml file
            _root.Child = LoadContentFromXaml();
            DRT.Assert(_root.Child is FlowDocumentScrollViewer, "{0}: Failed: Expecting TextFlow as root element.", this.TestName);
        }

        // ------------------------------------------------------------------
        // ScrollTextFlow
        // ------------------------------------------------------------------
        private void ScrollTextFlow()
        {
            FrameworkContentElement contentElement = DRT.FindElementByID("H1") as FrameworkContentElement;
            DRT.Assert(contentElement != null, "{0}: Failed: Cannot find element: {1}.", this.TestName, "H1");
            contentElement.BringIntoView();
        }

        // ------------------------------------------------------------------
        // VerifyTextFlow
        // ------------------------------------------------------------------
        private void VerifyTextFlow()
        {
            ScrollViewer sv = (ScrollViewer)DRT.FindVisualByType(typeof(ScrollViewer), _root);
            Vector expectedOffset = new Vector(0,1096.0);
            double offsetX = sv.HorizontalOffset;
            double offsetY = sv.VerticalOffset;
            DRT.Assert(CompareOffsets(offsetX, expectedOffset.X), "{0}: Failed: Expecting horizontal offset {1}, got {2}.", this.TestName, expectedOffset.X, offsetX);
            DRT.Assert(CompareOffsets(offsetY, expectedOffset.Y), "{0}: Failed: Expecting vertical offset {1}, got {2}.", this.TestName, expectedOffset.Y, offsetY);
        }

        // ------------------------------------------------------------------
        // Load content from xaml file.
        // ------------------------------------------------------------------
        private UIElement LoadContentFromXaml()
        {
            UIElement content = null;
            System.IO.Stream stream = null;
            string fileName = this.DrtFilesDirectory + this.TestName + ".xaml";
            try
            {
                stream = System.IO.File.OpenRead(fileName);
                content = System.Windows.Markup.XamlReader.Load(stream) as UIElement;
            }
            finally
            {
                // done with the stream
                if (stream != null) { stream.Close(); }
            }
            DRT.Assert(content != null, "{0}: Failed to load xaml file '{1}'", this.TestName, fileName);
            return content;
        }

        // ------------------------------------------------------------------
        // Offset comarison
        // ------------------------------------------------------------------
        private bool CompareOffsets(double offset1, double offset2)
        {
            if (double.IsNaN(offset1))
            {
                return double.IsNaN(offset2);
            }
            else if (double.IsNaN(offset2))
            {
                return double.IsNaN(offset1);
            }
            return (Math.Abs(offset1 - offset2) < 1);
        }

        // ------------------------------------------------------------------
        // Location of all DRT related files.
        // ------------------------------------------------------------------
        private string DrtFilesDirectory
        {
            get { return DRT.BaseDirectory + "DrtFiles\\FlowLayoutExt\\"; }
        }

        // ------------------------------------------------------------------
        // Unique name of the test.
        // ------------------------------------------------------------------
        private string TestName
        {
            get { return this.Name + _testName; }
        }

        // ------------------------------------------------------------------
        // Private fields.
        // ------------------------------------------------------------------
        private string _testName;
        private Border _root;
    }
}
