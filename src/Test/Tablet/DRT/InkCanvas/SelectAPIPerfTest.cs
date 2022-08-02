// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Runtime.InteropServices;

namespace DRT
{
    public class SelectionAPIPerfTest : InkCanvasPerfTestBase
    {
        #region Constructor

        public SelectionAPIPerfTest()
            : base("SelectionAPIPerfTest")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            _inkCanvas = new InkCanvas();
            _inkCanvas.Strokes = DRT.LoadStrokeCollection(DRT.BaseDirectory + "largepage.isf");

            DRT.Show(_inkCanvas);

            return new DrtTest[] { 
                    new DrtTest(StartLog),
                    new DrtTest(SelectAll),
                    new DrtTest(EndLog),
                };

        }

        #endregion Override Methods

        #region Tests

        private void SelectAll()
        {
            UIElement[] elements = new UIElement[_inkCanvas.Children.Count];
            _inkCanvas.Children.CopyTo(elements, 0);
            _inkCanvas.Select(_inkCanvas.Strokes, elements);
        }

        #endregion Tests

    }
}
