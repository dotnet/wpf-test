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
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using System.IO;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all Viewbox code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - StretchFillAllDirections
    /// - StretchNoneNoDirection
    /// - StretchUniformAllDirections
    /// - StretchUniformToFillAllDirections
    /// - MinHeightWidth
    /// - MaxHeightWidth
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.Viewbox", "ViewboxStretchFillAllDirections", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxStretchFillAllDirections : CodeTest
    {
        public ViewboxStretchFillAllDirections()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "ViewboxStretchFillAllDirections.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }
     
        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            VScanCommon vscan = new VScanCommon(this);
            if (!vscan.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Viewbox", "ViewboxStretchNoneNoDirection", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxStretchNoneNoDirection : CodeTest
    {
        public ViewboxStretchNoneNoDirection()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "ViewboxStretchNoneNoDirection.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon vscan = new VScanCommon(this);
            if (!vscan.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Viewbox", "ViewboxStretchUniformAllDirections", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxStretchUniformAllDirections : CodeTest
    {
        public ViewboxStretchUniformAllDirections()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "ViewboxStretchUniformAllDirections.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon vscan = new VScanCommon(this);
            if (!vscan.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Viewbox", "ViewboxStretchUniformToFillAllDirections", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxStretchUniformToFillAllDirections : CodeTest
    {
        public ViewboxStretchUniformToFillAllDirections()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "ViewboxStretchUniformToFillAllDirections.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon vscan = new VScanCommon(this);
            if (!vscan.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Viewbox", "ViewboxMinHeightWidth", Variables="Area=ElementLayout")]
    public class ViewboxMinHeightWidth : CodeTest
    {
        public ViewboxMinHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Border _viewboxcontent;

        double _minValue = 200;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _vb = new Viewbox();
            _vb.Height = 100;
            _vb.Width = 100;

            _viewboxcontent = new Border();
            _viewboxcontent.Background = Brushes.YellowGreen;
            _viewboxcontent.Height = 100;
            _viewboxcontent.Width = 100;

            _vb.Child = _viewboxcontent;

            _root.Children.Add(_vb);

            return _root;
        }

        public override void TestActions()
        {
            _vb.Height = 10;
            _vb.Width = 10;
            _vb.MinHeight = _minValue;
            _vb.MinWidth = _minValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size ViewboxSize = _vb.RenderSize;

            if (ViewboxSize.Height != _minValue || ViewboxSize.Width != _minValue)
            {
                Helpers.Log("Viewbox Min Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Viewbox Min Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Viewbox", "ViewboxMaxHeightWidth", Variables="Area=ElementLayout")]
    public class ViewboxMaxHeightWidth : CodeTest
    {
        public ViewboxMaxHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Border _viewboxcontent;

        double _maxValue = 350;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _vb = new Viewbox();
            _vb.Height = 100;
            _vb.Width = 100;

            _viewboxcontent = new Border();
            _viewboxcontent.Background = Brushes.YellowGreen;
            _viewboxcontent.Height = 100;
            _viewboxcontent.Width = 100;

            _vb.Child = _viewboxcontent;

            _root.Children.Add(_vb);

            return _root;
        }

        public override void TestActions()
        {
            _vb.Height = 1000;
            _vb.Width = 1000;
            _vb.MaxHeight = _maxValue;
            _vb.MaxWidth = _maxValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size ViewboxSize = _vb.RenderSize;

            if (ViewboxSize.Height != _maxValue || ViewboxSize.Width != _maxValue)
            {
                Helpers.Log("Viewbox Max Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Viewbox Max Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }
}
