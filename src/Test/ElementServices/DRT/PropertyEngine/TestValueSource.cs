// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT test suite for GetValueSource helper
//

using System;
using System.Collections;

using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DRT
{
    public sealed class TestValueSource : DrtTestSuite
    {
        public TestValueSource () : base("ValueSource")
        {
            Contact = "Microsoft";
        }

        #region Setup

        public override DrtTest[] PrepareTests()
        {
            // load the markup
            DRT.LoadXamlFile(@"DrtFiles\PropertyEngine\TestValueSource.xaml");

            return new DrtTest[]
            {
                new DrtTest(VerifyValueSource),
            };
        }

        void CheckValueSource(DependencyObject d, DependencyProperty dp, string name,
                                BaseValueSource valueSource,
                                bool isExpression,
                                bool isAnimated,
                                bool isCoerced)
        {
            ValueSource info = DependencyPropertyHelper.GetValueSource(d, dp);

            DRT.AssertEqual(valueSource, info.BaseValueSource,
                            "Wrong BaseValueSource for {0}.{1}", name, dp.Name);
            DRT.AssertEqual(isExpression, info.IsExpression,
                            "Wrong IsExpression for {0}.{1}", name, dp.Name);
            DRT.AssertEqual(isAnimated, info.IsAnimated,
                            "Wrong IsAnimated for {0}.{1}", name, dp.Name);
            DRT.AssertEqual(isCoerced, info.IsCoerced,
                            "Wrong IsCoerced for {0}.{1}", name, dp.Name);
        }

        #endregion Setup

        #region Tests for ValueSource

        // loaded event should fire on the tree in a VisualBrush
        void VerifyValueSource()
        {
            Button button;
            TextBlock textblock;
            ScrollBar scrollbar;

            button = (Button)DRT.FindVisualByID("Button1");
            CheckValueSource(button, Button.DataContextProperty, button.Name,
                BaseValueSource.Default, false, false, false);
            CheckValueSource(button, Button.ContentProperty, button.Name,
                BaseValueSource.Local, false, false, false);
            CheckValueSource(button, Button.BackgroundProperty, button.Name,
                BaseValueSource.DefaultStyle, false, false, false);
            CheckValueSource(button, Button.StyleProperty, button.Name,
                BaseValueSource.ImplicitStyleReference, false, false, false);
            CheckValueSource(button, Button.MarginProperty, button.Name,
                BaseValueSource.Style, false, false, false);

            button = (Button)DRT.FindVisualByID("Button2");
            CheckValueSource(button, Button.ForegroundProperty, button.Name,
                BaseValueSource.Style, false, false, false);
            CheckValueSource(button, Button.BackgroundProperty, button.Name,
                BaseValueSource.StyleTrigger, false, false, false);

            button = (Button)DRT.FindVisualByID("Button3");
            CheckValueSource(button, Button.DataContextProperty, button.Name,
                BaseValueSource.Inherited, false, false, false);

            button = (Button)DRT.FindVisualByID("Button4");
            textblock = (TextBlock)DRT.FindVisualByID("PlainTextBlock", button);
            CheckValueSource(textblock, TextBlock.TextProperty, textblock.Name,
                BaseValueSource.ParentTemplate, true, false, false);
            CheckValueSource(textblock, TextBlock.ForegroundProperty, textblock.Name,
                BaseValueSource.ParentTemplate, false, false, false);
            CheckValueSource(textblock, TextBlock.BackgroundProperty, textblock.Name,
                BaseValueSource.ParentTemplateTrigger, false, false, false);

            scrollbar = (ScrollBar)DRT.FindVisualByID("ScrollBar1");
            CheckValueSource(scrollbar, ScrollBar.MinWidthProperty, scrollbar.Name,
                BaseValueSource.DefaultStyleTrigger, false, false, false);
        }

        #endregion Tests for ValueSource

        #region Fields

        #endregion Fields
    }
}
