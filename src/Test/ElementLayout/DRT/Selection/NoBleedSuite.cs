// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Selection DRT's for Figure, Floater and Table to prevent bleeding 
//		regressions.
//
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DRT
{
    internal sealed class NoBleedSuite : DrtSelectionSuite
    {
        internal NoBleedSuite()
            : base("NoBleed", "Microsoft")
        {
        }

        protected override DrtTest[] CreateTests()
        {
            return new DrtTest[]
            {
                new DrtTest(Load), 
                new DrtTest(SelectFigure), 
                new DrtTest(SelectFloater), 
                new DrtTest(SelectTable), 
            };
        }

        internal void Load()
        {
            LoadContentFromXaml("NoBleed");
        }

        internal void SelectFigure()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 49, 55);
        }

        internal void SelectFloater()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 64, 70);
        }

        internal void SelectTable()
        {
            base.CalcualteSelectionGeometry(ContentRoot.Child as IServiceProvider, 84, 90);
        }
    }
}
