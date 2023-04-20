// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

#endregion

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This will load and run all code BVT's for Negative margins.
    /// 
    /// Possible Tests:
    /// 
    /// - NegativeMargins
    /// 
    //////////////////////////////////////////////////////////////////

    // [DISABLED_WHILE_PORTING] 
    [Test(0, "Property.NegativeMargin", "NegativeMargins", Variables="Area=ElementLayout", Disabled = true)]
    public class NegativeMargins : CodeTest
    {
        public NegativeMargins()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.WindowState = WindowState.Maximized;
            this.window.Content = this.TestContent();
        }

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

        Border _middle_man;
        Border _child;
        Border _content;
        Grid _root;

        public override void TestActions()
        {
            _content.Margin = new Thickness(-50, -50, -40, -40);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-20, -30, -40, -50);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-50, -40, -30, -20);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-50, -50, 50, 50);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(50, 50, -50, -50);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-100, -90, 100, 90);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();


            _content.Margin = new Thickness(-100, -40, 101, 41);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Height = double.NaN;
            _content.Width = double.NaN;

            _content.Margin = new Thickness(-300, 100, -300, 100);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();


            _content.Margin = new Thickness(-150, -50, -40, -140);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(20, 30, -140, -150);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-50, -40, 30, 20);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-150, -150, 150, 150);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(50, 50, -50, -50);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _content.Margin = new Thickness(-100, -90, 100, 90);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();


            _content.Margin = new Thickness(-100, -40, 101, 41);
            CommonFunctionality.FlushDispatcher();

            if (!Verify())
            {
                Helpers.Log("FAIL : " + _content.Margin);
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
        }

        bool Verify()
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
                    Helpers.Log("PASSED : " + _content.Margin + " ...");
                }
            }
            else
            {
                Helpers.Log("Content Margins are so large that is creates a height or width of 0.");
                assumed_union = new Rect(new Point(Math.Min(ChildRelativeToRoot.X, ContentRelativeToRoot.X), Math.Min(ChildRelativeToRoot.Y, ContentRelativeToRoot.Y)), new Size(Math.Max(content_rect.Width, child_rect.Width), Math.Max(content_rect.Height, child_rect.Height)));
                if (!DoubleUtil.AreClose(Union, assumed_union))
                {
                    return false;
                }
                else
                {
                    Helpers.Log("PASSED : " + _content.Margin + " ......");
                }
            }

            return true;
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
            this.window.WindowState = WindowState.Normal;
        }
    }
}
