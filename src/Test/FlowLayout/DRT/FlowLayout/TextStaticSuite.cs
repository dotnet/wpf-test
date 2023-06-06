// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for Text with static content. 
//
//

using System;
using System.Windows;
using System.Windows.Controls;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for Text with static content.
    // ----------------------------------------------------------------------
    internal sealed class TextStaticSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TextStaticSuite() : base("Text")
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
                new DrtTest(LoadSimple),            new DrtTest(VerifyLayout),
                new DrtTest(LoadSizing),            new DrtTest(VerifyLayout),
                new DrtTest(LoadTrimming),          new DrtTest(VerifyLayout),
                new DrtTest(LoadRlt),               new DrtTest(VerifyLayout),
                new DrtTest(LoadLineHeight),        new DrtTest(VerifyLayout),
            };
        }

        // ------------------------------------------------------------------
        // Load TextSimple.xaml
        // ------------------------------------------------------------------
        private void LoadSimple()
        {
            _testName = "Simple";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextSizing.xaml
        // ------------------------------------------------------------------
        private void LoadSizing()
        {
            _testName = "Sizing";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextTrimming.xaml
        // ------------------------------------------------------------------
        private void LoadTrimming()
        {
            _testName = "Trimming";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextRlt.xaml
        // ------------------------------------------------------------------
        private void LoadRlt()
        {
            _testName = "Rtl";
            LoadContentFromXaml();
        }

        // ------------------------------------------------------------------
        // Load TextLineHeight.xaml
        // ------------------------------------------------------------------
        private void LoadLineHeight()
        {
            _testName = "LineHeight";
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
