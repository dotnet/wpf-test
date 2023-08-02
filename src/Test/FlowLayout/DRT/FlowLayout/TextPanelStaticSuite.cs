// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for TextFlow with static content. 
//
//

using System;
using System.Windows;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for TextFlow with static content.
    // ----------------------------------------------------------------------
    internal sealed class TextPanelStaticSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextPanelStaticSuite() : base("TextPanel")
        {
            this.Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(LoadLineHeight),        new DrtTest(VerifyLayout),
                new DrtTest(LoadLists),             new DrtTest(VerifyLayout),
                new DrtTest(LoadMarginCollapsing),  new DrtTest(VerifyLayout),
                new DrtTest(LoadMBP),               new DrtTest(VerifyLayout),
                new DrtTest(LoadMisc),              new DrtTest(VerifyLayout),
                new DrtTest(LoadParagraphs),        new DrtTest(VerifyLayout),
                new DrtTest(LoadRtl),               new DrtTest(VerifyLayout),
                new DrtTest(LoadSimple),            new DrtTest(VerifyLayout),
                new DrtTest(LoadSizing),            new DrtTest(VerifyLayout),
                new DrtTest(LoadFloatLineSizing),   new DrtTest(VerifyLayout),
            };
        }

        // ------------------------------------------------------------------
        // Load TextPanelLineHeight.xaml
        // ------------------------------------------------------------------
        private void LoadLineHeight()
        {
            _testName = "LineHeight";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelLists.xaml
        // ------------------------------------------------------------------
        private void LoadLists()
        {
            _testName = "Lists";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelMarginCollapsing.xaml
        // ------------------------------------------------------------------
        private void LoadMarginCollapsing()
        {
            _testName = "MarginCollapsing";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelMBP.xaml
        // ------------------------------------------------------------------
        private void LoadMBP()
        {
            _testName = "MBP";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelMisc.xaml
        // ------------------------------------------------------------------
        private void LoadMisc()
        {
            _testName = "Misc";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelParagraphs.xaml
        // ------------------------------------------------------------------
        private void LoadParagraphs()
        {
            _testName = "Paragraphs";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelRlt.xaml
        // ------------------------------------------------------------------
        private void LoadRtl()
        {
            _testName = "Rtl";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelSimple.xaml
        // ------------------------------------------------------------------
        private void LoadSimple()
        {
            _testName = "Simple";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelSizing.xaml
        // ------------------------------------------------------------------
        private void LoadSizing()
        {
            _testName = "Sizing";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextPanelFloatLineSizing.xaml
        // ------------------------------------------------------------------
        private void LoadFloatLineSizing()
        {
            _testName = "FloatLineSizing";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Verify content.
        // ------------------------------------------------------------------
        private void VerifyLayout()
        {
            DRT.Assert(_testName != string.Empty, "{0}: Test name is not initialized.", this.TestName);
            DumpLayoutTree(false, true);
            _testName = string.Empty;
        }
    }
}
