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
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Transforms
{
    //////////////////////////////////////////////////////////////////////////////
    /// This contains all code for all RenderTransform priority cases.
    /// 
    //////////////////////////////////////////////////////////////////////////////

    [Test(2, "Transforms.Render", "RenderTransform1", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform1 : CodeTest
    {
        public RenderTransform1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };

        public override void TestActions()
        {
             //   case "FixedChildSquareParent":
                    ChangeProperty(300.0, 300.0, null, Stretch.None);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //   case "FixedChildSquareParent":
            _target.RenderTransform = new RotateTransform(10.0, 0, 0);

        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform2", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform2 : CodeTest
    {
        public RenderTransform2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FixedChildWideParent":
            ChangeProperty(300.0, 60.0, null, Stretch.None);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FixedChildWideParent":
            _target.RenderTransform = new SkewTransform(10.0, 0.0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform3", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform3 : CodeTest
    {
        public RenderTransform3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FixedChildTallParent":
            ChangeProperty(60.0, 300.0, null, Stretch.None);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FixedChildTallParent":
            _target.RenderTransform = new SkewTransform(0.0, 10.0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform4", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform4 : CodeTest
    {
        public RenderTransform4()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildSquareParentUniformMargin":
            ChangeProperty(300, 300, _margUniform, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildSquareParentUniformMargin":
            _target.RenderTransform = new ScaleTransform(0.5, 2.0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform5", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform5 : CodeTest
    {
        public RenderTransform5()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildWideParentUniformMargin":
            ChangeProperty(300, 60, _margUniform, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildWideParentUniformMargin":
            _target.RenderTransform = new RotateTransform(90.0, 0, 0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform6", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform6 : CodeTest
    {
        public RenderTransform6()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildTallParentUniformMargin":
            ChangeProperty(60, 300, _margUniform, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildTallParentUniformMargin":
            _target.RenderTransform = new RotateTransform(45.0, 0, 0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform7", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform7 : CodeTest
    {
        public RenderTransform7()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildSquareParentNonUniformMargin":
            ChangeProperty(300, 300, _margins, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildSquareParentNonUniformMargin":
            _target.RenderTransform = new RotateTransform(45.0, 120, 40);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform8", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform8 : CodeTest
    {
        public RenderTransform8()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildWideParentNonUniformMargin":
            ChangeProperty(300, 60, _margins, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildWideParentNonUniformMargin":
            _target.RenderTransform = new TranslateTransform(50.0, 25.0);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Render", "RenderTransform9", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class RenderTransform9 : CodeTest
    {
        public RenderTransform9()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Border _target;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.Lavender;

            _border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 0, 0);
            _border.Name = "MyElement";

            _target = new Border();
            _target.Height = 100;
            _target.Width = 100;

            _border.Child = _target;
            root.Children.Add(_border);

            return root;
        }

        double[] _margUniform = { 5.0 };
        double[] _margins = { 5.0, 4.0, 3.0, 2.0 };


        public override void TestActions()
        {
            //case "FillChildTallParentNonUniformMargin":
            ChangeProperty(60, 300, _margins, Stretch.Fill);
        }

        private void ChangeProperty(double borderWidth, double borderHeight, double[] marginSize, Stretch imageFillMode)
        {
            _border.Width = borderWidth;
            _border.Height = borderHeight;
            if (marginSize != null && marginSize.Length == 1)
            {
                _target.Margin = new Thickness(marginSize[0]);
            }
            else if (marginSize != null && marginSize.Length == 4)
            {
                _target.Margin = new Thickness(marginSize[0], marginSize[1], marginSize[2], marginSize[3]);
            }

            //case "FillChildTallParentNonUniformMargin":
            _target.RenderTransform = new RotateTransform(45.0, 140, 20);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }
}
