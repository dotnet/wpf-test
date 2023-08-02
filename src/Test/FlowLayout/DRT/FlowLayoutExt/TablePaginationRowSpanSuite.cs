// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Table loaded from XAML
//
//

using System;
using System.Threading;

using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;

namespace DRT
{
    // 
    // Table DRT's.
    // 
    internal sealed class TablePaginationRowSpanSuite : FlowLayoutExtSuite
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        internal TablePaginationRowSpanSuite() : base("TablePaginationRowSpan")
        {
            this.Contact = "Microsoft";
        }

        /// <summary>
        /// Creates array of callbacks
        /// </summary>
        /// <returns>Array of callbacks</returns>
        protected override DrtTest[] CreateTests()
        {
            return new DrtTest[]
            {
                new DrtTest(Root),
                new DrtTest(VerifyLayoutCreateAndFinalize),
            };
        }

        /// <summary>
        /// Creates DRT's tree
        /// </summary>
        /// <returns>Root of the tree</returns>
        internal void Root()
        {
            IDocumentPaginatorSource content = LoadFromXaml("TablePaginationRowSpan.xaml") as IDocumentPaginatorSource;
            SplitPage root = new SplitPage();
            root.Content = content;

            _contentRoot.Child = root;
        }
    }
}
