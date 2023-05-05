// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This is for Negative Margin test.
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(1, "Property.NegativeMargin", "NegativeMarginRandom", Variables="Area=ElementLayout")]
    public class NegativeMarginRandom : CodeTest
    {
        public NegativeMarginRandom()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Border _middle_man;
        Border _child;
        Border _content;
        Grid _root;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.CornflowerBlue;


            _middle_man = new Border();
            _middle_man.Height = 200;
            _middle_man.Width = 200;
            _middle_man.HorizontalAlignment = HorizontalAlignment.Center;
            _middle_man.VerticalAlignment = VerticalAlignment.Center;

            _child = new Border();
            _child.Name = "child";
            _child.Height = 200;
            _child.Width = 200;
            _child.Background = Brushes.Yellow;

            _content = new Border();
            _content.Name = "content";
            _content.Background = Brushes.Gray;
            _content.Height = 200;
            _content.Width = 200;

            _child.Child = _content;

            _middle_man.Child = _child;

            _root.Children.Add(_middle_man);


            return _root;
        }

        public override void TestActions()
        {
            for (int i = 0; i < 150; i++)
            {
                _content.Margin = RandomThickness();
                CommonFunctionality.FlushDispatcher();
                if (!RandomMarginVerify())
                {
                    Helpers.Log("FAIL : " + _content.Margin);
                    _tempresult = false;
                }
            }
        }

        bool RandomMarginVerify()
        {
            Point ChildRelativeToRoot = LayoutUtility.GetElementPosition(_child, _root);
            Point ContentRelativeToRoot = LayoutUtility.GetElementPosition(_content, _root);

            Rect child_rect = new Rect(new Point(ChildRelativeToRoot.X, ChildRelativeToRoot.Y), new Size(_child.ActualWidth, _child.ActualHeight));
            Rect content_rect = new Rect(new Point(ContentRelativeToRoot.X, ContentRelativeToRoot.Y), new Size(_content.ActualWidth, _content.ActualHeight));

            Rect Union = Rect.Union(child_rect, content_rect);
            Rect midman = VisualTreeHelper.GetDescendantBounds(_middle_man);

            Rect assumed_union = new Rect(new Point(Math.Min(ChildRelativeToRoot.X, ContentRelativeToRoot.X), Math.Min(ChildRelativeToRoot.Y, ContentRelativeToRoot.Y)), new Size(midman.Width, midman.Height));

            if (_content.ActualHeight > 0 && _content.ActualWidth > 0)
            {
                if (!DoubleUtil.AreClose(Union, assumed_union))
                {
                    return false;
                }
                else
                {
                    Helpers.Log("PASSED : " + _content.Margin + ".");
                }
            }
            else
            {
                assumed_union = new Rect(new Point(Math.Min(ChildRelativeToRoot.X, ContentRelativeToRoot.X), Math.Min(ChildRelativeToRoot.Y, ContentRelativeToRoot.Y)), new Size(Math.Max(content_rect.Width, child_rect.Width), Math.Max(content_rect.Height, child_rect.Height)));
                if (!DoubleUtil.AreClose(Union, assumed_union))
                {
                    return false;
                }
                else
                {
                    Helpers.Log("PASSED : " + _content.Margin + ".");
                }
            }

            return true;
        }

        int _seed = 0;
        Thickness RandomThickness()
        {
            Thickness randomT = new Thickness();
            Random random = new Random(_seed);

            randomT.Bottom = random.Next(-1000, 0);
            randomT.Left = random.Next(-1000, 0);
            randomT.Right = random.Next(-1000, 0);
            randomT.Top = random.Next(-1000, 0);

            _seed++;
            
            return randomT;
        }
        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
            if (!this.Result)
            {
                Helpers.Log("Random Negative Margin Test Failed.");
            }
            else
            {
                Helpers.Log("Random Negative Margin Test Passed.");
            }
        }

    }

    [Test(2, "Property.NegativeMargin", "NegMarginAndLayoutTran", Variables="Area=ElementLayout")]
    public class NegMarginAndLayoutTran : PropertyDumpTest
    {
        public NegMarginAndLayoutTran() { this.DumpTest("NegMarginAndLayoutTran.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginAndRenderTran", Variables="Area=ElementLayout")]
    public class NegMarginAndRenderTran : PropertyDumpTest
    {
        public NegMarginAndRenderTran() { this.DumpTest("NegMarginAndRenderTran.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginBorder", Variables="Area=ElementLayout")]
    public class NegMarginBorder : PropertyDumpTest
    {
        public NegMarginBorder() { this.DumpTest("NegMarginBorder.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginBorderContent", Variables="Area=ElementLayout")]
    public class NegMarginBorderContent : PropertyDumpTest
    {
        public NegMarginBorderContent() { this.DumpTest("NegMarginBorderContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginCanvas", Variables="Area=ElementLayout")]
    public class NegMarginCanvas : PropertyDumpTest
    {
        public NegMarginCanvas() { this.DumpTest("NegMarginCanvas.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginCanvasContent", Variables="Area=ElementLayout")]
    public class NegMarginCanvasContent : PropertyDumpTest
    {
        public NegMarginCanvasContent() { this.DumpTest("NegMarginCanvasContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginDecorator", Variables="Area=ElementLayout")]
    public class NegMarginDecorator : PropertyDumpTest
    {
        public NegMarginDecorator() { this.DumpTest("NegMarginDecorator.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginDecoratorContent", Variables="Area=ElementLayout")]
    public class NegMarginDecoratorContent : PropertyDumpTest
    {
        public NegMarginDecoratorContent() { this.DumpTest("NegMarginDecoratorContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginDock", Variables="Area=ElementLayout")]
    public class NegMarginDock : PropertyDumpTest
    {
        public NegMarginDock() { this.DumpTest("NegMarginDock.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginDockContent", Variables="Area=ElementLayout")]
    public class NegMarginDockContent : PropertyDumpTest
    {
        public NegMarginDockContent() { this.DumpTest("NegMarginDockContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginGrid", Variables="Area=ElementLayout")]
    public class NegMarginGrid : PropertyDumpTest
    {
        public NegMarginGrid() { this.DumpTest("NegMarginGrid.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginGridContent", Variables="Area=ElementLayout")]
    public class NegMarginGridContent : PropertyDumpTest
    {
        public NegMarginGridContent() { this.DumpTest("NegMarginGridContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginHorizStack", Variables="Area=ElementLayout")]
    public class NegMarginHorizStack : PropertyDumpTest
    {
        public NegMarginHorizStack() { this.DumpTest("NegMarginHorizStack.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginHorizStackContent", Variables="Area=ElementLayout")]
    public class NegMarginHorizStackContent : PropertyDumpTest
    {
        public NegMarginHorizStackContent() { this.DumpTest("NegMarginHorizStackContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginHorizWrap", Variables="Area=ElementLayout")]
    public class NegMarginHorizWrap : PropertyDumpTest
    {
        public NegMarginHorizWrap() { this.DumpTest("NegMarginHorizWrap.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginHorizWrapContent", Variables="Area=ElementLayout")]
    public class NegMarginHorizWrapContent : PropertyDumpTest
    {
        public NegMarginHorizWrapContent() { this.DumpTest("NegMarginHorizWrapContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginScroll", Variables="Area=ElementLayout")]
    public class NegMarginScroll : PropertyDumpTest
    {
        public NegMarginScroll() { this.DumpTest("NegMarginScroll.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginScrollContent", Variables="Area=ElementLayout")]
    public class NegMarginScrollContent : PropertyDumpTest
    {
        public NegMarginScrollContent() { this.DumpTest("NegMarginScrollContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginVertStack", Variables="Area=ElementLayout")]
    public class NegMarginVertStack : PropertyDumpTest
    {
        public NegMarginVertStack() { this.DumpTest("NegMarginVertStack.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginVertStackContent", Variables="Area=ElementLayout")]
    public class NegMarginVertStackContent : PropertyDumpTest
    {
        public NegMarginVertStackContent() { this.DumpTest("NegMarginVertStackContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginVertWrap", Variables="Area=ElementLayout")]
    public class NegMarginVertWrap : PropertyDumpTest
    {
        public NegMarginVertWrap() { this.DumpTest("NegMarginVertWrap.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginVertWrapContent", Variables="Area=ElementLayout")]
    public class NegMarginVertWrapContent : PropertyDumpTest
    {
        public NegMarginVertWrapContent() { this.DumpTest("NegMarginVertWrapContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginViewbox", Variables="Area=ElementLayout")]
    public class NegMarginViewbox : PropertyDumpTest
    {
        public NegMarginViewbox() { this.DumpTest("NegMarginViewbox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(2, "Property.NegativeMargin", "NegMarginViewboxContent", Variables="Area=ElementLayout")]
    public class NegMarginViewboxContent : PropertyDumpTest
    {
        public NegMarginViewboxContent() { this.DumpTest("NegMarginViewboxContent.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }
}
