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
    internal sealed class TextDocumentViewSuite : DrtSelectionSuite
    {
        internal TextDocumentViewSuite()
            : base("TextDocumentView", "Microsot")
        {
        }

        protected override DrtTest[] CreateTests()
        {
            return new DrtTest[]
            {
                new DrtTest(TextDocumentView01_Load), 
                new DrtTest(TextDocumentView01_Select01), 
                new DrtTest(TextDocumentView01_Select02), 
                new DrtTest(TextDocumentView01_Select03), 
                new DrtTest(TextDocumentView01_Select04), 
                new DrtTest(TextDocumentView01_Select05), 

                new DrtTest(TextDocumentView02_Load), 
                new DrtTest(TextDocumentView01_Select01), 
                new DrtTest(TextDocumentView01_Select02), 
                new DrtTest(TextDocumentView01_Select03), 
                new DrtTest(TextDocumentView01_Select04), 
                new DrtTest(TextDocumentView01_Select05), 

                new DrtTest(TextDocumentView03_Load), 
                new DrtTest(TextDocumentView01_Select01), 
                new DrtTest(TextDocumentView01_Select02), 
                new DrtTest(TextDocumentView01_Select03), 
                new DrtTest(TextDocumentView01_Select04), 
                new DrtTest(TextDocumentView01_Select05), 
            };
        }

        internal void TextDocumentView01_Load()
        {
            LoadContentFromXaml("TextDocumentView01");
        }

        internal void TextDocumentView02_Load()
        {
            LoadContentFromXaml("TextDocumentView02");
        }

        internal void TextDocumentView03_Load()
        {
            LoadContentFromXaml("TextDocumentView03");
        }

        internal void TextDocumentView01_Select01()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 1, 1.5);
        }

        internal void TextDocumentView01_Select02()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 20, 21);
        }

        internal void TextDocumentView01_Select03()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 10, 60);
        }

        internal void TextDocumentView01_Select04()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 1, 100);
        }

        internal void TextDocumentView01_Select05()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 92, 98);
        }
    }
}
