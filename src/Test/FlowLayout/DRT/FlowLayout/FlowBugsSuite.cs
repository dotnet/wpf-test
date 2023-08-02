// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Test suite for misc Flow FT bugs. 
//

using System;
using System.Windows;
using System.Windows.Documents;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Test suite for misc Flow FT bugs.
    // ----------------------------------------------------------------------
    internal sealed class FlowBugsSuite : LayoutSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal FlowBugsSuite() : base("FlowBugs")
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
                new DrtTest(LoadMirroring),         new DrtTest(VerifyLayoutCreateAndFinalize),
            };
        }

        // ------------------------------------------------------------------
        // Load FlowBugsMirroring.xaml
        // ------------------------------------------------------------------
        private void LoadMirroring()
        {
            _testName = "Mirroring";
            LoadContentFromXaml();
        }
    }
}
