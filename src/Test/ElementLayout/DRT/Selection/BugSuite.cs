// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Selection DRT's for TextDocumentView
//
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DRT
{
    internal sealed class BugSuite : DrtSelectionSuite
    {
        internal BugSuite()
            : base("Bugs", "Microsoft")
        {
        }

        protected override DrtTest[] CreateTests()
        {
            return new DrtTest[]
            {
                new DrtTest(Bug1_Load), 
                new DrtTest(Bug1_SelectHalf), 

                new DrtTest(Bug2_Load), 
                new DrtTest(Bug2_SelectAll), 
            };
        }

        internal void Bug1_Load()
        {
            LoadContentFromXaml("Bug1");
        }

        internal void Bug1_SelectHalf()
        {
            RichTextBox rtb = (RichTextBox)LogicalTreeHelper.FindLogicalNode(ContentRoot.Child, "IServiceProvider");
            base.CalcualteSelectionGeometry(rtb, 50, 100);
        }

        internal void Bug2_Load()
        {
            LoadContentFromXaml("Bug2");
        }

        internal void Bug2_SelectAll()
        {
            RichTextBox rtb = (RichTextBox)LogicalTreeHelper.FindLogicalNode(ContentRoot.Child, "IServiceProvider");
            base.CalcualteSelectionGeometry(rtb, 0, 100);
        }
    }
}
