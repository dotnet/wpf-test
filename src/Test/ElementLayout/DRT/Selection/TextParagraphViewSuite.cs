// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Selection DRT's for TextParagraphView
//
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DRT
{
    internal sealed class TextParagraphViewSuite : DrtSelectionSuite
    {
        internal TextParagraphViewSuite()
            : base("TextParagraphView", "Microsoft")
        {
        }

        protected override DrtTest[] CreateTests()
        {
            return new DrtTest[]
            {
                new DrtTest(TextParagraphView01_Load), 
                new DrtTest(TextParagraphView01_Select01), 
                new DrtTest(TextParagraphView01_Select02), 
                new DrtTest(TextParagraphView01_Select03), 
                new DrtTest(TextParagraphView01_Select04), 
                new DrtTest(TextParagraphView01_Select05), 

                new DrtTest(TextParagraphView02_Load), 
                new DrtTest(TextParagraphView01_Select01), 
                new DrtTest(TextParagraphView01_Select02), 
                new DrtTest(TextParagraphView01_Select03), 
                new DrtTest(TextParagraphView01_Select04), 
                new DrtTest(TextParagraphView01_Select05), 

                new DrtTest(TextParagraphView03_Load), 
                new DrtTest(TextParagraphView01_Select01), 
                new DrtTest(TextParagraphView01_Select02), 
                new DrtTest(TextParagraphView01_Select03), 
                new DrtTest(TextParagraphView01_Select04), 
                new DrtTest(TextParagraphView01_Select05), 
            };
        }

        internal void TextParagraphView01_Load()
        {
            LoadContentFromXaml("TextParagraphView01");
        }

        internal void TextParagraphView02_Load()
        {
            LoadContentFromXaml("TextParagraphView02");
        }

        internal void TextParagraphView03_Load()
        {
            LoadContentFromXaml("TextParagraphView03");
        }

        internal void TextParagraphView01_Select01()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 0, 1);
        }

        internal void TextParagraphView01_Select02()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 20, 21);
        }

        internal void TextParagraphView01_Select03()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 10, 60);
        }

        internal void TextParagraphView01_Select04()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 1, 100);
        }

        internal void TextParagraphView01_Select05()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 92, 98);
        }
    }
}
