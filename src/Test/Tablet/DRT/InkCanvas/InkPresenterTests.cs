// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Runtime.InteropServices;

namespace DRT
{
    public sealed class InkPresenterTests : DrtTabletTestSuite
    {
        private InkPresenter        _inkPresenter;

        //
        // Test data
        //
        StrokeCollection _initialStrokes;

        #region Constructor

        public InkPresenterTests()
            : base("InkPresenterTests")
        {
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            // For example to create a tree via code:
            Visual root = CreateMyTree();
            
            _inkPresenter = new InkPresenter();
            ((Border)root).Child = _inkPresenter;

            DRT.Show(root);

            _initialStrokes = DRT.LoadStrokeCollection(DRT.BaseDirectory + "drtinkcanvas_initialink.isf");
            DRT.Assert(_initialStrokes.Count > 0, "Couldn't load strokes from disk drtinkcanvas_initialink.isf");

            ArrayList tests = new ArrayList();

            TestUnit unit;

            /////////////////////////////////////////////////////////////
            // Properties
            /////////////////////////////////////////////////////////////

            // Strokes  
            unit = new PropertyDefaultValueTest(DRT, _inkPresenter, "Strokes", new ValueCheckingCallback(CheckEmtryStroks));
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkPresenter, "Strokes", _initialStrokes);
            tests.AddRange(unit.Tests);

            unit = new AccessPropertyTest(DRT, _inkPresenter, "Strokes", null, typeof(ArgumentException));
            tests.AddRange(unit.Tests);

            /////////////////////////////////////////////////////////////
            // Methods
            /////////////////////////////////////////////////////////////

            return (DrtTest[])tests.ToArray(typeof(DrtTest));
        }

        #endregion Override Methods

        #region Private Implementation

        private Visual CreateMyTree()
        {
            Border rootBorder = new Border();
            rootBorder.Background = Brushes.LightGray;

            return rootBorder;
        }

        private void CheckEmtryStroks(DrtBase drt, object value, string propertyName)
        {
            StrokeCollection strokes = value as StrokeCollection;

            drt.Assert(strokes.Count == 0, string.Format("The stroke collection is not empty"));
        }

        #endregion Private Implementation
    }
}
