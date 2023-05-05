// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using System.Windows.Shapes;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This contains BVT's Clip Property.
    /// 
    /// Possible Tests:
    /// 
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(1, "Property.Clipping", "ElementClip1", Variables="Area=ElementLayout")]
    public class ElementClip1 : CodeTest
    {
        public ElementClip1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Border";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip2", Variables="Area=ElementLayout")]
    public class ElementClip2 : CodeTest
    {
        public ElementClip2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Dock";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip3", Variables="Area=ElementLayout")]
    public class ElementClip3 : CodeTest
    {
        public ElementClip3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Canvas";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip4", Variables="Area=ElementLayout")]
    public class ElementClip4 : CodeTest
    {
        public ElementClip4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Grid";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip5", Variables="Area=ElementLayout")]
    public class ElementClip5 : CodeTest
    {
        public ElementClip5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Text";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip6", Variables="Area=ElementLayout")]
    public class ElementClip6 : CodeTest
    {
        public ElementClip6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "FlowDoc";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip7", Variables="Area=ElementLayout")]
    public class ElementClip7 : CodeTest
    {
        public ElementClip7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Panel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip8", Variables="Area=ElementLayout")]
    public class ElementClip8 : CodeTest
    {
        public ElementClip8()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "StackPanel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip9", Variables="Area=ElementLayout")]
    public class ElementClip9 : CodeTest
    {
        public ElementClip9()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Decorator";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip10", Variables="Area=ElementLayout")]
    public class ElementClip10 : CodeTest
    {
        public ElementClip10()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Viewbox";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip11", Variables="Area=ElementLayout")]
    public class ElementClip11 : CodeTest
    {
        public ElementClip11()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "Transform";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }

    [Test(1, "Property.Clipping", "ElementClip12", Variables="Area=ElementLayout")]
    public class ElementClip12 : CodeTest
    {
        public ElementClip12()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        string _elementItem = "WrapPanel";

        FrameworkElement _theRoot;
        FrameworkElement _theParent;

        public override FrameworkElement TestContent()
        {
            Grid eRoot = new Grid();

            eRoot.Background = Brushes.Lavender;
            _theRoot = eRoot;

            _theParent = ElementClipHelper.AddTestContent(_elementItem);
            if (_theParent != null)
            {
                _theParent.Clip = new EllipseGeometry(new Point(50, 50), 50, 50);
                eRoot.Children.Add(_theParent);
            }
            return _theRoot;
        }

        public override void TestActions()
        {
            UIElement inputElement;

            inputElement = LayoutUtility.GetInputElement(_theRoot, new Point(3, 3));

            if (inputElement != null)
            {
                if (inputElement.GetType().Name != "Grid")
                {
                    Helpers.Log("ClipToBounds has not been applied");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Test Case Passed");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("input element was null.");
                this.Result = false;
            }

        }
    }
}
