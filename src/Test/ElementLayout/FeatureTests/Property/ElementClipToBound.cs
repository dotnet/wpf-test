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
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////////////
    /// This contains all code for all Element Clip To Bounds priority cases.
    /// 
    //////////////////////////////////////////////////////////////////////////

    [Test(1, "Property.Clipping", "ElementClipToBounds1", Variables="Area=ElementLayout")]
    public class ElementClipToBounds1 : CodeTest
    {
        public ElementClipToBounds1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Border";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds2", Variables="Area=ElementLayout")]
    public class ElementClipToBounds2 : CodeTest
    {
        public ElementClipToBounds2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Dock";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds3", Variables="Area=ElementLayout")]
    public class ElementClipToBounds3 : CodeTest
    {
        public ElementClipToBounds3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Canvas";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds4", Variables="Area=ElementLayout")]
    public class ElementClipToBounds4 : CodeTest
    {
        public ElementClipToBounds4()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Grid";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds5", Variables="Area=ElementLayout")]
    public class ElementClipToBounds5 : CodeTest
    {
        public ElementClipToBounds5()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Text";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds6", Variables="Area=ElementLayout")]
    public class ElementClipToBounds6 : CodeTest
    {
        public ElementClipToBounds6()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "FlowDoc";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds7", Variables="Area=ElementLayout")]
    public class ElementClipToBounds7 : CodeTest
    {
        public ElementClipToBounds7()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Panel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds8", Variables="Area=ElementLayout")]
    public class ElementClipToBounds8 : CodeTest
    {
        public ElementClipToBounds8()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "StackPanel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds9", Variables="Area=ElementLayout")]
    public class ElementClipToBounds9 : CodeTest
    {
        public ElementClipToBounds9()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Decorator";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds10", Variables="Area=ElementLayout")]
    public class ElementClipToBounds10 : CodeTest
    {
        public ElementClipToBounds10()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Viewbox";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds11", Variables="Area=ElementLayout")]
    public class ElementClipToBounds11 : CodeTest
    {
        public ElementClipToBounds11()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "Transform";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Property.Clipping", "ElementClipToBounds12", Variables="Area=ElementLayout")]
    public class ElementClipToBounds12 : CodeTest
    {
        public ElementClipToBounds12()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        static string s_elementItem = "WrapPanel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            _theRoot = new Grid();
            ((Grid)_theRoot).Background = Brushes.Lavender;
            _theParent = ElementClipHelper.AddTestContent(s_elementItem);
            ((Grid)_theRoot).Children.Add(_theParent);
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            if (s_elementItem == "Text")
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(120, 10));
            }
            else
            {
                inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(30, 120));
            }

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("Input element was Null");
                _tempresult = false;
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }
}
