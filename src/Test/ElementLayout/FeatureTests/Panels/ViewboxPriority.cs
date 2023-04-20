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
using Microsoft.Test.Input;
using ElementLayout.TestLibrary;
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.RenderingVerification;
using System.IO;
using Microsoft.Test.Threading;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.Viewbox", "ViewboxResize", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResize : CodeTest
    {
        public ViewboxResize()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Border _viewboxcontent;

        double _pixelSize = 250;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _vb = new Viewbox();

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
            SetStretchProp("None");
            CommonFunctionality.FlushDispatcher();
            viewboxresize("Pixel");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            viewboxresize("Auto");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*********************************");

            SetStretchProp("Fill");
            CommonFunctionality.FlushDispatcher();
            viewboxresize("Pixel");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            viewboxresize("Auto");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*********************************");

            SetStretchProp("Uniform");
            CommonFunctionality.FlushDispatcher();
            viewboxresize("Pixel");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            viewboxresize("Auto");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*********************************");

            SetStretchProp("UniformToFill");
            CommonFunctionality.FlushDispatcher();
            viewboxresize("Pixel");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            viewboxresize("Auto");
            CommonFunctionality.FlushDispatcher();

            if (!Verifyviewboxresize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*********************************");
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void viewboxresize(string UnitType)
        {
            switch (UnitType)
            {
                case "Pixel":
                    Helpers.Log("Test : Pixel Size");
                    Helpers.Log("Stretch Value : " + _vb.Stretch.ToString());
                    _vb.Height = _pixelSize;
                    _vb.Width = _pixelSize;
                    CommonFunctionality.FlushDispatcher();
                    break;
                case "Auto":
                    Helpers.Log("Test : Auto Size");
                    Helpers.Log("Stretch Value : " + _vb.Stretch.ToString());
                    _vb.Height = double.NaN;
                    _vb.Width = double.NaN;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }

        bool Verifyviewboxresize()
        {
            CommonFunctionality.FlushDispatcher();

            Size ViewboxSize = _vb.RenderSize;
            Size RootSize = _root.RenderSize;
            Size VBContentSize = _viewboxcontent.RenderSize;

            string StretchProp = _vb.Stretch.ToString();

            if (!Double.IsNaN(_vb.Height))
            {
                if (StretchProp != "None")
                {
                    Helpers.Log("Pixel Verify");
                    if (ViewboxSize.Height != _pixelSize || ViewboxSize.Width != _pixelSize)
                    {
                        Helpers.Log("Viewbox Pixel Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Pixel Size is correct!");
                        return true;
                    }
                }
                else
                {
                    Helpers.Log("Pixel Size with Stretch = None Verify.");
                    if (ViewboxSize.Height != VBContentSize.Height || ViewboxSize.Width != VBContentSize.Width)
                    {
                        Helpers.Log("Viewbox Pixel Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Pixel Size is correct!");
                        return true;
                    }
                }
            }

            if (Double.IsNaN(_vb.Height))
            {
                if (StretchProp == "None")
                {
                    Helpers.Log("Auto Verify");
                    if (ViewboxSize.Height != VBContentSize.Height || ViewboxSize.Width != VBContentSize.Width)
                    {
                        Helpers.Log("Viewbox Auto Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Auto Size is correct!");
                        return true;
                    }
                }
                if (StretchProp == "Fill")
                {
                    Helpers.Log("Auto Verify");
                    if (ViewboxSize.Height != RootSize.Height || ViewboxSize.Width != RootSize.Width)
                    {
                        Helpers.Log("Viewbox Auto Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Auto Size is correct!");
                        return true;
                    }
                }
                if (StretchProp == "Uniform")
                {
                    Helpers.Log("Stretch = uniform");
                    if (ViewboxSize.Width != RootSize.Width || (ViewboxSize.Height / VBContentSize.Height) != (ViewboxSize.Width / VBContentSize.Width))
                    {
                        Helpers.Log("Viewbox Auto Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Auto Size is correct!");
                        return true;
                    }
                }
                if (StretchProp == "UniformToFill")
                {
                    Helpers.Log("Auto Verify");
                    if (ViewboxSize.Height != RootSize.Height || (ViewboxSize.Height / VBContentSize.Height) != (ViewboxSize.Width / VBContentSize.Width))
                    {
                        Helpers.Log("Viewbox Auto Size is not correct!");
                        return false;
                    }
                    else
                    {
                        Helpers.Log("Viewbox Auto Size is correct!");
                        return true;
                    }
                }
            }
            return true;
        }

        void SetStretchProp(string _Stretch)
        {
            switch (_Stretch)
            {
                case "None":
                    _vb.Stretch = Stretch.None;
                    break;
                case "Fill":
                    _vb.Stretch = Stretch.Fill;
                    break;
                case "Uniform":
                    _vb.Stretch = Stretch.Uniform;
                    break;
                case "UniformToFill":
                    _vb.Stretch = Stretch.UniformToFill;
                    break;
            }
        }

    }

    [Test(1, "Panels.Viewbox", "ViewboxResizeContent", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeContent : CodeTest
    {
        public ViewboxResizeContent()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Border _viewboxcontent;

        double _staticHeight = 250;
        double _staticWidth = 325;

        double _contentSmall = 10;
        double _contentMedium = 600;
        double _contentLarge = 2000;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _vb = new Viewbox();
            _vb.HorizontalAlignment = HorizontalAlignment.Center;
            _vb.VerticalAlignment = VerticalAlignment.Center;
            _vb.Height = _staticHeight;
            _vb.Width = _staticWidth;
            _vb.Stretch = Stretch.Fill;

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
            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentSmall);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentSmall");

            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentMedium);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentMedium");

            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentLarge);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentLarge");

            CommonFunctionality.FlushDispatcher();
            SetAutoSizeVB();
            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentSmall);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentSmall");

            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentMedium);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentMedium");

            CommonFunctionality.FlushDispatcher();
            viewboxresizecontent(_contentLarge);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxContentResize("ContentLarge");

            CommonFunctionality.FlushDispatcher();
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void viewboxresizecontent(double ContentSize)
        {
            _viewboxcontent.Height = ContentSize;
            _viewboxcontent.Width = ContentSize;
        }

        void SetAutoSizeVB()
        {
            _vb.Height = double.NaN;
            _vb.Width = double.NaN;
            _vb.Stretch = Stretch.None;
        }

        void VerifyViewboxContentResize(string ContentSizeValue)
        {
            Size ViewboxSize = _vb.RenderSize;
            Size ContentSize = _viewboxcontent.RenderSize;

            CommonFunctionality.FlushDispatcher();

            //absolute size on VB verification
            if (!Double.IsNaN(_vb.Height))
            {
                switch (ContentSizeValue)
                {
                    case "ContentSmall":
                        if (ViewboxSize.Height != _staticHeight || ViewboxSize.Width != _staticWidth || ContentSize.Height != _contentSmall || ContentSize.Width != _contentSmall)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;

                    case "ContentMedium":
                        if (ViewboxSize.Height != _staticHeight || ViewboxSize.Width != _staticWidth || ContentSize.Height != _contentMedium || ContentSize.Width != _contentMedium)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;

                    case "ContentLarge":
                        if (ViewboxSize.Height != _staticHeight || ViewboxSize.Width != _staticWidth || ContentSize.Height != _contentLarge || ContentSize.Width != _contentLarge)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;
                }
            }
            if (Double.IsNaN(_vb.Height))
            {
                switch (ContentSizeValue)
                {
                    case "ContentSmall":
                        if (ViewboxSize.Height != ContentSize.Height || ViewboxSize.Width != ContentSize.Width || ContentSize.Height != _contentSmall || ContentSize.Width != _contentSmall)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;

                    case "ContentMedium":
                        if (ViewboxSize.Height != ContentSize.Height || ViewboxSize.Width != ContentSize.Width || ContentSize.Height != _contentMedium || ContentSize.Width != _contentMedium)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;

                    case "ContentLarge":
                        if (ViewboxSize.Height != ContentSize.Height || ViewboxSize.Width != ContentSize.Width || ContentSize.Height != _contentLarge || ContentSize.Width != _contentLarge)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;

                    case "Percent":
                        if (ViewboxSize.Height != ContentSize.Height || ViewboxSize.Width != ContentSize.Width || ContentSize.Height != 0 || ContentSize.Width != 0)
                        {
                            Helpers.Log("Viewbox Size is not correct after child resize.");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Viewbox Size is correct.");
                        }
                        break;
                }
            }
        }
    }

    [Test(1, "Panels.Viewbox", "ViewboxClip", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxClip : CodeTest
    {
        public ViewboxClip()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ViewboxClip.xaml";
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
            Helpers.Log("No Test Actions ...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Viewbox", "ViewboxInsideEvent", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxInsideEvent : CodeTest
    {
        public ViewboxInsideEvent()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 650;
            this.window.Width = 650;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.HorizontalAlignment = HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;

            Border b = new Border();
            b.BorderThickness = new Thickness(2);
            b.BorderBrush = Brushes.Navy;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;
            _vb.Height = 75;
            _vb.Width = 250;
            _vb.StretchDirection = StretchDirection.UpOnly;

            _btn = new Button();
            _btn.Click += new RoutedEventHandler(btnClick);
            _btn.Content = "btn";

            _vb.Child = _btn;

            b.Child = _vb;

            _root.Children.Add(b);
            return _root;
        }

        public override void TestActions()
        {
            Helpers.ToggleWindowState(this.window);
            UserInput.MouseLeftClickCenter(_btn);
            CommonFunctionality.FlushDispatcher();
        }

        bool _tempresult = false;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                Helpers.Log("Result is false. Event was not handled inside Viewbox!!");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Result is true. Event handled inside Viewbox!!");
                this.Result = true;
            }
        }

        void btnClick(object sender, RoutedEventArgs e)
        {
            Helpers.Log("Button inside Viewbox handled click event");
            _tempresult = true;
        }
    }

    [Test(1, "Panels.Viewbox", "ViewboxMarginOnContent", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxMarginOnContent : CodeTest
    {
        public ViewboxMarginOnContent()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb0,_vb1,_vb2,_vb3;
        Grid _root;
        Border _viewboxcontent0,_viewboxcontent1,_viewboxcontent2,_viewboxcontent3;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            ColumnDefinition c0 = new ColumnDefinition();
            ColumnDefinition c1 = new ColumnDefinition();
            RowDefinition r0 = new RowDefinition();
            RowDefinition r1 = new RowDefinition();

            _root.ColumnDefinitions.Add(c0);
            _root.ColumnDefinitions.Add(c1);
            _root.RowDefinitions.Add(r0);
            _root.RowDefinitions.Add(r1);

            Border b0 = new Border();
            b0.Background = Brushes.CornflowerBlue;
            b0.HorizontalAlignment = HorizontalAlignment.Center;
            b0.VerticalAlignment = VerticalAlignment.Center;
            b0.BorderBrush = Brushes.Crimson;
            b0.BorderThickness = new Thickness(2);
            Grid.SetRow(b0, 0);
            Grid.SetColumn(b0, 0);

            _vb0 = new Viewbox();
            _vb0.Height = 300;
            _vb0.Width = 150;
            _vb0.Stretch = Stretch.Fill;

            _viewboxcontent0 = new Border();
            _viewboxcontent0.Background = Brushes.Crimson;
            _viewboxcontent0.BorderBrush = Brushes.Navy;
            _viewboxcontent0.BorderThickness = new Thickness(10);
            _viewboxcontent0.Height = 100;
            _viewboxcontent0.Width = 100;
            _viewboxcontent0.CornerRadius = new CornerRadius(50);
            _viewboxcontent0.Margin = new Thickness(25);

            _vb0.Child = _viewboxcontent0;

            b0.Child = _vb0;

            _root.Children.Add(b0);

            Border b1 = new Border();
            b1.Background = Brushes.CornflowerBlue;
            b1.HorizontalAlignment = HorizontalAlignment.Center;
            b1.VerticalAlignment = VerticalAlignment.Center;
            b1.BorderBrush = Brushes.Crimson;
            b1.BorderThickness = new Thickness(2);
            Grid.SetRow(b1, 0);
            Grid.SetColumn(b1, 1);

            _vb1 = new Viewbox();
            _vb1.Height = 300;
            _vb1.Width = 150;
            _vb1.Stretch = Stretch.Uniform;

            _viewboxcontent1 = new Border();
            _viewboxcontent1.Background = Brushes.Crimson;
            _viewboxcontent1.BorderBrush = Brushes.Navy;
            _viewboxcontent1.BorderThickness = new Thickness(10);
            _viewboxcontent1.Height = 100;
            _viewboxcontent1.Width = 100;
            _viewboxcontent1.CornerRadius = new CornerRadius(50);
            _viewboxcontent1.Margin = new Thickness(25);

            _vb1.Child = _viewboxcontent1;

            b1.Child = _vb1;

            _root.Children.Add(b1);

            Border b2 = new Border();
            b2.Background = Brushes.CornflowerBlue;
            b2.HorizontalAlignment = HorizontalAlignment.Center;
            b2.VerticalAlignment = VerticalAlignment.Center;
            b2.BorderBrush = Brushes.Crimson;
            b2.BorderThickness = new Thickness(2);
            Grid.SetRow(b2, 1);
            Grid.SetColumn(b2, 0);

            _vb2 = new Viewbox();
            _vb2.Height = 300;
            _vb2.Width = 150;
            _vb2.Stretch = Stretch.UniformToFill;

            _viewboxcontent2 = new Border();
            _viewboxcontent2.Background = Brushes.Crimson;
            _viewboxcontent2.BorderBrush = Brushes.Navy;
            _viewboxcontent2.BorderThickness = new Thickness(10);
            _viewboxcontent2.Height = 100;
            _viewboxcontent2.Width = 100;
            _viewboxcontent2.CornerRadius = new CornerRadius(50);
            _viewboxcontent2.Margin = new Thickness(25);

            _vb2.Child = _viewboxcontent2;

            b2.Child = _vb2;

            _root.Children.Add(b2);


            Border b3 = new Border();
            b3.Background = Brushes.CornflowerBlue;
            b3.HorizontalAlignment = HorizontalAlignment.Center;
            b3.VerticalAlignment = VerticalAlignment.Center;
            b3.BorderBrush = Brushes.Crimson;
            b3.BorderThickness = new Thickness(2);
            Grid.SetRow(b3, 1);
            Grid.SetColumn(b3, 1);

            _vb3 = new Viewbox();
            _vb3.Height = 300;
            _vb3.Width = 150;
            _vb3.Stretch = Stretch.None;

            _viewboxcontent3 = new Border();
            _viewboxcontent3.Background = Brushes.Crimson;
            _viewboxcontent3.BorderBrush = Brushes.Navy;
            _viewboxcontent3.BorderThickness = new Thickness(10);
            _viewboxcontent3.Height = 100;
            _viewboxcontent3.Width = 100;
            _viewboxcontent3.CornerRadius = new CornerRadius(50);
            _viewboxcontent3.Margin = new Thickness(25);

            _vb3.Child = _viewboxcontent3;

            b3.Child = _vb3;

            _root.Children.Add(b3);

            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Test Actions ...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Viewbox", "ViewboxPropertyAttributes", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxPropertyAttributes : CodeTest
    {
        public ViewboxPropertyAttributes()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Border _vbchild;

        Stretch[] _stretchProps = { Stretch.Fill, Stretch.None, Stretch.Uniform, Stretch.UniformToFill };
        StretchDirection[] _stretchDirectionProps = { StretchDirection.Both, StretchDirection.DownOnly, StretchDirection.UpOnly };

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            Border border = new Border();
            border.BorderBrush = Brushes.Navy;
            border.BorderThickness = new Thickness(2);

            _vb = new Viewbox();
            _vb.Height = 125;
            _vb.Width = 300;

            _vbchild = new Border();
            _vbchild.Height = 325;
            _vbchild.Width = 225;
            _vbchild.Background = Brushes.CornflowerBlue;

            _vb.Child = _vbchild;

            border.Child = _vb;

            _root.Children.Add(border);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(0, 0);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(0, 0);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(0, 1);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(0, 1);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(0, 2);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(0, 2);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(1, 0);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(1, 0);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(1, 1);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(1, 1);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(1, 2);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(1, 2);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(2, 0);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(2, 0);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(2, 1);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(2, 1);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(2, 2);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(2, 2);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(3, 0);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(3, 0);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(3, 1);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(3, 1);

            CommonFunctionality.FlushDispatcher();
            SetViewboxProps(3, 2);
            CommonFunctionality.FlushDispatcher();

            VerifyViewboxProps(3, 2);

            CommonFunctionality.FlushDispatcher();
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void SetViewboxProps(int Stretch, int StretchDirection)
        {
            Helpers.Log("Setting Viewbox Props.");

            _vb.SetValue(Viewbox.StretchProperty, _stretchProps[Stretch]);
            _vb.SetValue(Viewbox.StretchDirectionProperty, _stretchDirectionProps[StretchDirection]);
        }

        void VerifyViewboxProps(int Stretch, int StretchDirection)
        {
            Helpers.Log("Verifying Viewbox Props.");

            object ExpectedStretch = _stretchProps[Stretch];
            object ExpectedStretchDirection = _stretchDirectionProps[StretchDirection];

            object CurrentStretch = _vb.GetValue(Viewbox.StretchProperty);
            object CurrentStretchDirection = _vb.GetValue(Viewbox.StretchDirectionProperty);

            Helpers.Log("Expected Stretch	: " + ExpectedStretch.ToString());
            Helpers.Log("Expected Direction	: " + ExpectedStretchDirection.ToString());

            Helpers.Log("Current Stretch	: " + CurrentStretch.ToString());
            Helpers.Log("Current Direction	: " + CurrentStretchDirection.ToString());

            if (CurrentStretch.ToString() != ExpectedStretch.ToString() || CurrentStretchDirection.ToString() != ExpectedStretchDirection.ToString())
            {
                Helpers.Log("Viewbox Properties are not correct.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Viewbox Properties are correct. Test On!");
            }
        }
    }

    [Test(1, "Panels.Viewbox", "ViewboxVisibility", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxVisibility : CodeTest
    {
        public ViewboxVisibility()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ViewboxVisiblity.xaml";
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
            Helpers.Log("No Test Actions ...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    #region resize content

    [Test(3, "Panels.Viewbox", "ViewboxResizeBorder", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeBorder : CodeTest
    {
        public ViewboxResizeBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            Border b = new Border();
            b.Height = 275;
            b.Width = 275;
            b.Background = Brushes.LightGoldenrodYellow;
            b.BorderThickness = new Thickness(5);
            b.BorderBrush = Brushes.DarkOrange;
            b.Margin = new Thickness(10);

            Border b1 = new Border();
            b1.Background = Brushes.LightGreen;
            b1.BorderThickness = new Thickness(5);
            b1.BorderBrush = Brushes.DarkMagenta;
            b1.Margin = new Thickness(10);

            b.Child = b1;

            return b;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeCanvas", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeCanvas : CodeTest
    {
        public ViewboxResizeCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            Canvas can = new Canvas();
            can.Height = 275;
            can.Width = 275;
            can.Background = Brushes.DarkBlue;

            Border b1 = CommonFunctionality.CreateBorder(Brushes.CornflowerBlue, 150, 150);
            Border b2 = CommonFunctionality.CreateBorder(Brushes.Crimson, 150, 150);
            Border b3 = CommonFunctionality.CreateBorder(Brushes.Yellow, 150, 150);

            Canvas.SetBottom(b1, 25);
            Canvas.SetRight(b1, 25);

            Canvas.SetTop(b2, 50);
            Canvas.SetRight(b2, 50);

            Canvas.SetTop(b3, 25);
            Canvas.SetLeft(b3, 25);

            can.Children.Add(b1);
            can.Children.Add(b3);
            can.Children.Add(b2);

            return can;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeDockPanel", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeDockPanel : CodeTest
    {
        public ViewboxResizeDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            DockPanel dock = new DockPanel();
            dock.Height = 275;
            dock.Width = 275;
            dock.LastChildFill = true;

            DockPanel top = new DockPanel();
            top.Height = 50;
            top.Background = Brushes.DarkBlue;
            DockPanel.SetDock(top, Dock.Top);

            DockPanel bot = new DockPanel();
            bot.Height = 50;
            bot.Background = Brushes.DarkBlue;
            DockPanel.SetDock(bot, Dock.Bottom);

            DockPanel left = new DockPanel();
            left.Width = 50;
            left.Background = Brushes.Crimson;
            DockPanel.SetDock(left, Dock.Left);

            DockPanel right = new DockPanel();
            right.Width = 50;
            right.Background = Brushes.Crimson;
            DockPanel.SetDock(right, Dock.Right);

            DockPanel fill = new DockPanel();
            fill.Background = Brushes.Yellow;
            //DockPanel.SetDock(fill, Dock.Fill);

            dock.Children.Add(top);
            dock.Children.Add(bot);
            dock.Children.Add(left);
            dock.Children.Add(right);
            dock.Children.Add(fill);

            return dock;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeGrid", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeGrid : CodeTest
    {
        public ViewboxResizeGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            Grid grid = new Grid();
            grid.Height = 275;
            grid.Width = 275;
            grid.Background = Brushes.CornflowerBlue;
            grid.ShowGridLines = false;

            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();

            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            RowDefinition row3 = new RowDefinition();

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);

            Border b1 = CommonFunctionality.CreateBorder(Brushes.DarkBlue, 50, 50);
            Border b2 = CommonFunctionality.CreateBorder(Brushes.Yellow, 50, 50);

            Grid.SetColumn(b1, 0);
            Grid.SetRow(b1, 0);

            Grid.SetColumn(b2, 2);
            Grid.SetRow(b2, 2);

            GridCommon.SettingAlignment(b1, HorizontalAlignment.Center, VerticalAlignment.Center);

            GridCommon.SettingAlignment(b2, HorizontalAlignment.Center, VerticalAlignment.Center);

            grid.Children.Add(b1);
            grid.Children.Add(b2);

            return grid;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeImage", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeImage : CodeTest
    {
        public ViewboxResizeImage()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            BitmapImage id = new BitmapImage(new Uri(@"light.bmp", UriKind.RelativeOrAbsolute));

            Image img = new Image();
            img.Height = 275;
            img.Width = 275;
            img.Stretch = Stretch.UniformToFill;
            img.Source = id;

            return img;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeMixed", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeMixed : CodeTest
    {
        public ViewboxResizeMixed()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            BitmapImage id = new BitmapImage(new Uri(@"cloud.bmp", UriKind.RelativeOrAbsolute));

            ImageBrush ib = new ImageBrush(id);

            Canvas can = new Canvas();
            can.Height = 400;
            can.Width = 450;
            can.Background = ib;

            Ellipse e = new Ellipse();
            e.Fill = Brushes.Orange;
            e.Width = 200;
            e.Height = 200;
            Canvas.SetLeft(e, 100);
            Canvas.SetTop(e, 100);

            can.Children.Add(e);

            TextBlock foo = new TextBlock();
            foo.Foreground = Brushes.Red;
            foo.FontSize = 25;
            foo.FontFamily = new FontFamily("Tahoma");
            foo.Text = "foo foo foo foo foo";
            Canvas.SetTop(foo, 50);
            Canvas.SetLeft(foo, 50);

            can.Children.Add(foo);

            TextBlock bar = new TextBlock();
            bar.Foreground = Brushes.Yellow;
            bar.FontSize = 50;
            bar.FontFamily = new FontFamily("Tahoma");
            bar.Text = "bar bar bar bar bar";
            Canvas.SetTop(bar, 110);
            Canvas.SetLeft(bar, 25);

            can.Children.Add(bar);

            Run r = new Run("foobar foobar foobar");
            r.FontFamily = new FontFamily("Tahoma");
            Paragraph p = new Paragraph(r);
            p.FontFamily = new FontFamily("Tahoma");
            FlowDocumentScrollViewer foobar = new FlowDocumentScrollViewer();
            foobar.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            foobar.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            foobar.Document = new FlowDocument(p);
            foobar.Document.Foreground = Brushes.Red;
            foobar.Document.FontSize = 30;
            foobar.FontFamily = new FontFamily("Tahoma");

            //TextBlock child = new TextBlock();
            //child.Text = "foobar foobar foobar";
            //p.ContentEnd.InsertUIElement(child);

            //foobar.ContentStart.InsertText("foobar foobar foobar");
            Canvas.SetTop(foobar, 200);
            Canvas.SetLeft(foobar, 125);

            can.Children.Add(foobar);
            return can;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeTable", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeTable : CodeTest
    {
        public ViewboxResizeTable()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            FlowDocumentScrollViewer FlowDoc = new FlowDocumentScrollViewer();
            FlowDoc.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            FlowDoc.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            FlowDoc.Document = new FlowDocument();
            FlowDoc.FontFamily = new FontFamily("Tahoma");
            FlowDoc.FontSize = 25;
            FlowDoc.Height = 300;
            FlowDoc.Width = 325;

            Table Table = new Table();


            TableColumn col1 = new TableColumn();
            col1.Width = new GridLength(100);
            TableColumn col2 = new TableColumn();
            col2.Width = new GridLength(100);
            TableColumn col3 = new TableColumn();
            col3.Width = new GridLength(100);

            Table.Columns.Add(col1);
            Table.Columns.Add(col2);
            Table.Columns.Add(col3);

            TableRowGroup header = new TableRowGroup();
            TableRowGroup body = new TableRowGroup();
            TableRowGroup footer = new TableRowGroup();

            Table.RowGroups.Add(header);
            Table.RowGroups.Add(body);
            Table.RowGroups.Add(footer);

            TableRow headerrow = new TableRow();
            headerrow.Background = Brushes.LightGreen;
            TableCell hra = new TableCell(new Paragraph(new Run("A")));
            TableCell hrb = new TableCell(new Paragraph(new Run("B")));
            TableCell hrc = new TableCell(new Paragraph(new Run("C")));


            header.Rows.Add(headerrow);
            headerrow.Cells.Add(hra);
            headerrow.Cells.Add(hrb);
            headerrow.Cells.Add(hrc);

            TableRow footerrow = new TableRow();
            footerrow.Background = Brushes.LightGreen;
            TableCell fra = new TableCell(new Paragraph(new Run("A")));
            TableCell frb = new TableCell(new Paragraph(new Run("B")));
            TableCell frc = new TableCell(new Paragraph(new Run("C")));

            footer.Rows.Add(footerrow);
            footerrow.Cells.Add(fra);
            footerrow.Cells.Add(frb);
            footerrow.Cells.Add(frc);

            body.Rows.Add(CreateRow(1));
            body.Rows.Add(CreateRow(2));
            body.Rows.Add(CreateRow(3));
            body.Rows.Add(CreateRow(4));
            body.Rows.Add(CreateRow(5));
            body.Rows.Add(CreateRow(6));
            body.Rows.Add(CreateRow(7));
            body.Rows.Add(CreateRow(8));
            body.Rows.Add(CreateRow(9));
            body.Rows.Add(CreateRow(10));

            if (FlowDoc.Document.Blocks.Count == 0)
            {
                FlowDoc.Document.Blocks.Add(Table);
            }
            else
            {
                FlowDoc.Document.Blocks.InsertBefore(FlowDoc.Document.Blocks.FirstBlock, Table);
            }

            //(FlowDoc.TextRange.TextContainer).InsertElement(FlowDoc.TextRange.Start, FlowDoc.TextRange.Start, Table);

            return FlowDoc;
        }

        TableRow CreateRow(int RowNum)
        {
            TableRow row = new TableRow();
            row.Background = Brushes.LightPink;
            row.Cells.Add(CreateCell(RowNum));
            row.Cells.Add(CreateCell(0));
            row.Cells.Add(CreateCell(0));
            return row;
        }

        TableCell CreateCell(int i)
        {
            TableCell cell;
            if (i == 0)
            {
                cell = new TableCell(new Paragraph(new Run("")));
            }
            else
            {
                cell = new TableCell(new Paragraph(new Run(i.ToString())));
            }
            return cell;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeText", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeText : CodeTest
    {
        public ViewboxResizeText()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            TextBlock txt = new TextBlock();
            txt.Height = 275;
            txt.Width = 275;
            txt.Foreground = Brushes.Navy;
            txt.FontSize = 25;
            txt.FontFamily = new FontFamily("Tahoma");
            txt.Text = "Text Element";
            txt.FontStyle = FontStyles.Italic;
            txt.FontWeight = FontWeights.ExtraBold;

            return txt;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxResizeFlowDoc", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxResizeFlowDoc : CodeTest
    {
        public ViewboxResizeFlowDoc()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 677;
            this.window.Width = 608;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vb;
        Grid _root;
        Grid _vbParent;

        Size _startSize = new Size(300, 300);
        Size _smallSize = new Size(50, 50);
        Size _mediumSize = new Size(500, 500);
        Size _largeSize = new Size(1200, 1200);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;
            _root.Background = Brushes.LightGray;

            _vbParent = new Grid();
            _vbParent.Height = _startSize.Height;
            _vbParent.Width = _startSize.Width;
            _vbParent.Background = Brushes.CornflowerBlue;

            _vb = new Viewbox();
            _vb.Stretch = Stretch.Fill;

            _vb.Child = VBContent();

            _vbParent.Children.Add(_vb);

            _root.Children.Add(_vbParent);

            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            ResizeTest("Large");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Small");

            CommonFunctionality.FlushDispatcher();

            ResizeTest("Medium");

            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }

        FrameworkElement VBContent()
        {
            Run r = new Run("this is a FlowDoc with some text content.. this is a FlowDoc with some text content.. this is a FlowDoc with some text content.. this is a FlowDoc with some text content..");
            Paragraph p = new Paragraph(r);
            FlowDocumentScrollViewer txt = new FlowDocumentScrollViewer();
            txt.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            txt.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            txt.Document = new FlowDocument(p);
            txt.FontSize = 15;
            txt.FontFamily = new FontFamily("Tahoma");
            txt.Background = Brushes.Crimson;
            txt.Height = 325;
            txt.Width = 250;
            //txt.TextWrapping = TextWrapping.WrapWithOverflow;

            return txt;
        }

        void ResizeTest(string ResizeSize)
        {
            switch (ResizeSize)
            {
                case "Small":
                    Helpers.Log("Resizing Viewbox Parent to Small size");
                    _vbParent.Height = _smallSize.Height;
                    _vbParent.Width = _smallSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Medium":
                    Helpers.Log("Resizing Viewbox Parent to Medium size");
                    _vbParent.Height = _mediumSize.Height;
                    _vbParent.Width = _mediumSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Large":
                    Helpers.Log("Resizing Viewbox Parent to Large size");
                    _vbParent.Height = _largeSize.Height;
                    _vbParent.Width = _largeSize.Width;
                    CommonFunctionality.FlushDispatcher();
                    break;
            }
        }
    }

    #endregion

    #region stretch : uniform to fill

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillCanvas", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillCanvas : CodeTest
    {
        public ViewboxUniformToFillCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "CanvasContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillStackPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillStackPanel : CodeTest
    {
        public ViewboxUniformToFillStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "StackPanelContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillGrid", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillGrid : CodeTest
    {
        public ViewboxUniformToFillGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "GridContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillDockPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillDockPanel : CodeTest
    {
        public ViewboxUniformToFillDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;


            string xamlfile = "DockPanelContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillDecorator", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillDecorator : CodeTest
    {
        public ViewboxUniformToFillDecorator()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DecoratorContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillBorder", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillBorder : CodeTest
    {
        public ViewboxUniformToFillBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "BorderContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillViewbox", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillViewbox : CodeTest
    {
        public ViewboxUniformToFillViewbox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ViewboxContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillTransform", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillTransform : CodeTest
    {
        public ViewboxUniformToFillTransform()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "TransformContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformToFillScrollViewer", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformToFillScrollViewer : CodeTest
    {
        public ViewboxUniformToFillScrollViewer()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ScrollviewerContent.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            f.Close();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.UniformToFill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    #endregion

    #region stretch : fill

    [Test(2, "Panels.Viewbox", "ViewboxFillCanvas", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillCanvas : CodeTest
    {
        public ViewboxFillCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "CanvasContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillStackPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillStackPanel : CodeTest
    {
        public ViewboxFillStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "StackPanelContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillGrid", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillGrid : CodeTest
    {
        public ViewboxFillGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "GridContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillDockPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillDockPanel : CodeTest
    {
        public ViewboxFillDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DockPanelContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillDecorator", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillDecorator : CodeTest
    {
        public ViewboxFillDecorator()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DecoratorContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillBorder", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillBorder : CodeTest
    {
        public ViewboxFillBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 100;
            this.window.Left = 100;

            string xamlfile = "BorderContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            this.window.Focus();
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillViewbox", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillViewbox : CodeTest
    {
        public ViewboxFillViewbox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ViewboxContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillTransform", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillTransform : CodeTest
    {
        public ViewboxFillTransform()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "TransformContent.xaml";
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
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxFillScrollViewer", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxFillScrollViewer : CodeTest
    {
        public ViewboxFillScrollViewer()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ScrollviewerContent.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            f.Close();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Grid root = this.window.Content as Grid;
            Border rootFirstChild = null;
            if (root != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(root, i).GetType().Name == "Border")
                    {
                        rootFirstChild = VisualTreeHelper.GetChild(root, i) as Border;
                    }
                }
            }
            if (rootFirstChild != null)
            {
                Viewbox targetVB = rootFirstChild.Child as Viewbox;
                targetVB.Stretch = Stretch.Fill;
            }
            else
            {
                Helpers.Log("could not find target vb.");
            }
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    #endregion

    #region stretch : uniform

    [Test(2, "Panels.Viewbox", "ViewboxUniformCanvas", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformCanvas : CodeTest
    {
        public ViewboxUniformCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "CanvasContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformStackPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformStackPanel : CodeTest
    {
        public ViewboxUniformStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "StackPanelContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformGrid", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformGrid : CodeTest
    {
        public ViewboxUniformGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "GridContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformDockPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformDockPanel : CodeTest
    {
        public ViewboxUniformDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DockPanelContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformDecorator", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformDecorator : CodeTest
    {
        public ViewboxUniformDecorator()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DecoratorContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformBorder", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformBorder : CodeTest
    {
        public ViewboxUniformBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "DecoratorContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformViewbox", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformViewbox : CodeTest
    {
        public ViewboxUniformViewbox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ViewboxContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformTransform", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformTransform : CodeTest
    {
        public ViewboxUniformTransform()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "TransformContent.xaml";
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
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(2, "Panels.Viewbox", "ViewboxUniformScrollViewer", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxUniformScrollViewer : CodeTest
    {
        public ViewboxUniformScrollViewer()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "ScrollviewerContent.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            f.Close();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        public override void TestActions()
        {
            Helpers.Log("No Action Needed...");
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    #endregion

    #region content property change

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeRectangle", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeRectangle : CodeTest
    {
        public ViewboxContentPropChangeRectangle()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));

            _vbx.Child = _rect;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeButton", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeButton : CodeTest
    {


        public ViewboxContentPropChangeButton()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Viewbox _vbx;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);

            _vbx.Child = _btn;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeTextBox", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeTextBox : CodeTest
    {


        public ViewboxContentPropChangeTextBox()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Viewbox _vbx;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");

            _vbx.Child = _tbox;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeEllipse", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeEllipse : CodeTest
    {


        public ViewboxContentPropChangeEllipse()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Viewbox _vbx;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _vbx.Child = _elps;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeImage", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeImage : CodeTest
    {
        public ViewboxContentPropChangeImage()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _vbx.Child = _img;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeText", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeText : CodeTest
    {


        public ViewboxContentPropChangeText()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _vbx.Child = _txt;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeBorder", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeBorder : CodeTest
    {


        public ViewboxContentPropChangeBorder()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _vbx.Child = _b;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeLabel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeLabel : CodeTest
    {


        public ViewboxContentPropChangeLabel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing wrapPanel with Label~!";
            _vbx.Child = _lbl;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeListBox", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeListBox : CodeTest
    {


        public ViewboxContentPropChangeListBox()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);

            _vbx.Child = _lb;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeMenu", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeMenu : CodeTest
    {


        public ViewboxContentPropChangeMenu()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _vbx.Child = _menu;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeCanvas", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeCanvas : CodeTest
    {


        public ViewboxContentPropChangeCanvas()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);

            _vbx.Child = _canvas;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            Rectangle crect = CommonFunctionality.CreateRectangle(40, 40, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(crect, 30);
            Canvas.SetTop(crect, 30);
            _canvas.Children.Add(crect);
            _canvas.Width = _canvas.ActualWidth * 2;
            _canvas.Height = _canvas.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeDockPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeDockPanel : CodeTest
    {


        public ViewboxContentPropChangeDockPanel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _dockpanel = new DockPanel();
            _dockpanel.Background = new SolidColorBrush(Colors.SlateBlue);
            _dockpanel.LastChildFill = true;
            Rectangle rect0 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Red));
            DockPanel.SetDock(rect0, Dock.Top);
            _dockpanel.Children.Add(rect0);
            Rectangle rect1 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Blue));
            DockPanel.SetDock(rect1, Dock.Left);
            _dockpanel.Children.Add(rect1);
            Rectangle rect2 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Yellow));
            //DockPanel.SetDock(rect2, Dock.Fill);
            _dockpanel.Children.Add(rect2);

            _vbx.Child = _dockpanel;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeStackPanel", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeStackPanel : CodeTest
    {


        public ViewboxContentPropChangeStackPanel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;

            _vbx.Child = _s;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            Rectangle sChild1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Orange));
            Rectangle sChild2 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            Rectangle sChild3 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.DarkSeaGreen));
            _s.Children.Add(sChild1);
            _s.Children.Add(sChild2);
            _s.Children.Add(sChild3);
            _s.Width = 150;
        }

        public override void TestVerify()
        {
            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }

        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.Viewbox", "ViewboxContentPropChangeGrid", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxContentPropChangeGrid : CodeTest
    {


        public ViewboxContentPropChangeGrid()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Viewbox _vbx;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _vbx = new Viewbox();

            //content that will have a prop change.
            _g = GridCommon.CreateGrid(2, 2);
            _g.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle gRect0 = CommonFunctionality.CreateRectangle(10, 10, new SolidColorBrush(Colors.Red));
            GridCommon.PlacingChild(_g, gRect0, 0, 0);
            _g.Children.Add(gRect0);

            Rectangle gRect1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(_g, gRect1, 1, 0);
            _g.Children.Add(gRect1);

            Rectangle gRect2 = CommonFunctionality.CreateRectangle(30, 30, new SolidColorBrush(Colors.Yellow));
            GridCommon.PlacingChild(_g, gRect2, 0, 1);
            _g.Children.Add(gRect2);

            _vbx.Child = _g;

            _root.Children.Add(_vbx);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            _relayoutOccurred = false;
            _vbx.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated !!!");
                this.Result = true;
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    #endregion

    [Test(1, "Panels.Viewbox", "ViewboxVisualChildIndexException", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ViewboxVisualChildIndexException : CodeTest
    {
        public ViewboxVisualChildIndexException()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Viewbox _vbx;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            _vbx = new Viewbox();
            Border b = new Border();
            b.Background = Brushes.Orange;
            b.Height = 100;
            b.Width = 100;
            _vbx.Child = b;
            root.Children.Add(_vbx);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("Test: GetVisualChild at index > 0 should throw exception...");
            try
            {
                VisualTreeHelper.GetChild(_vbx, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Correct Exception Caught...");
                _tempresult = true;
            }
        }

        bool _tempresult = false;

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }
}
