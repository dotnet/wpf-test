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
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.IO;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Globalization;
using Microsoft.Test.Threading;

namespace ElementLayout.FeatureTests.Scenario
{
    //////////////////////////////////////////////////////////////////
    /// This is a regression tests for element layout.
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(3, "Regression", "RegressionTest1", Variables="Area=ElementLayout")]
    public class RegressionTest1 : CodeTest
    {
        public RegressionTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "LASTITEM.XAML";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        Grid _root;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            return _root;
        }

        public override void TestActions()
        {
            ListBox lbx = null;// = LogicalTreeHelper.FindLogicalNode(window, "lbx") as ListBox;

            while (lbx == null)
            {
                Helpers.Log("lbx is still null, checking again.");
                lbx = LogicalTreeHelper.FindLogicalNode(window, "lbx") as ListBox;
            }

            lbx.SelectionChanged += new SelectionChangedEventHandler(lbx_SelectionChanged);

            lbx.Focus();

            CommonFunctionality.FlushDispatcher();

            UserInput.KeyDown("End");
        }

        void lbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = ((ListBox)sender).SelectedItem as ListBoxItem;
            ValidateContent(lbi.Content.ToString());
        }

        void ValidateContent(string actual)
        {
            if (actual.ToLower() != "lastitem")
                _tempresult = false;
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(3, "Regression", "RegressionTest2", Variables="Area=ElementLayout")]
    public class RegressionTest2 : CodeTest
    {
        public RegressionTest2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "scrollviewer_style_duplicate_name.XAML";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            try
            {
                this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            }
            catch (Exception ex)
            {
                Helpers.Log(string.Format("Exception type: {0}", ex.GetType()));
                Helpers.Log(string.Format("Actual InnerException.Message: '{0}'", ex.InnerException.Message));

                if (System.Environment.Version.Major < 4)
                {
                    Helpers.Log(string.Format("Expected InnerException.Message: '{0}'", Exceptions.GetMessage("NameScopeDuplicateNamesNotAllowed", WpfBinaries.PresentationFramework)));
                    this.Result = Exceptions.CompareMessage(ex.InnerException.Message, "NameScopeDuplicateNamesNotAllowed", WpfBinaries.PresentationFramework);
                }
                else
                {                    
                    Helpers.Log("If this test is failing the bug should now be fixed and the test needs to be updated.");
                    // 


                    this.Result = !Exceptions.CompareMessage(ex.InnerException.Message, "NameScopeDuplicateNamesNotAllowed", WpfBinaries.PresentationFramework);
                }
            }

            f.Close();
        }
    }

    [Test(3, "Regression", "RegressionTest3", Variables="Area=ElementLayout")]
    public class RegressionTest3 : CodeTest
    {
        public RegressionTest3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Border _eRoot;
        Grid _fp;
        ScrollViewer _sv;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Border();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;
            _fp = new Grid();

            DockPanel dp = new DockPanel();
            dp.Margin = new System.Windows.Thickness(50);
            dp.LastChildFill = true;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Green));
            //DockPanel.SetDock(rect, Dock.Fill);
            dp.Children.Add(rect);
            _fp.Children.Add(_sv);
            _sv.Content = dp;
            _eRoot.Child = _fp;

            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.ScrollerVisibilities result = new ScrollTestCommon.ScrollerVisibilities(_sv);
            if ((_sv.ViewportWidth == 200) && (result.vertical == ScrollBarVisibility.Visible))
            {
                this.Result = true;
            }
            else
            {
                Helpers.Log("Margins are not been counted for ContentPresenter OR VerticalScrollBar not shown.");
                this.Result = false;
            }
        }
    }

    [Test(3, "Regression", "RegressionTest4", Variables="Area=ElementLayout")]
    public class RegressionTest4 : CodeTest
    {
        /*
         * System.Windows.CornerRadius: implement IEquatable T
         * Adding new case for Equals property on CornerRadius.
         */

        public RegressionTest4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        bool _tempResult = true;

        Grid _root;
        Border _childone;
        Border _childtwo;
        CornerRadius _crone;
        CornerRadius _crtwo;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Gray;
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            _root.ColumnDefinitions.Add(c1);
            _root.ColumnDefinitions.Add(c2);

            _childone = new Border();
            Grid.SetColumn(_childone, 0);
            _childone.Height = 401;
            _childone.Width = 401;
            _childone.Background = Brushes.Goldenrod;

            _root.Children.Add(_childone);

            _childtwo = new Border();
            Grid.SetColumn(_childtwo, 1);
            _childtwo.Height = 201;
            _childtwo.Width = 201;
            _childtwo.Background = Brushes.RoyalBlue;

            _root.Children.Add(_childtwo);

            return _root;
        }

        public override void TestActions()
        {
            _crone = new CornerRadius(10);
            _crtwo = new CornerRadius(10);

            CommonFunctionality.FlushDispatcher();

            if (!_crone.Equals(_crtwo) || !_crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius (no border) Compare Failed. [1]");
                Helpers.Log("CornerRadius's should be equal.");
                _tempResult = false;
            }

            _crone = new CornerRadius(101);
            _crtwo = new CornerRadius(201);

            CommonFunctionality.FlushDispatcher();

            if (_crone.Equals(_crtwo) || _crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius (no border) Compare Failed. [2]");
                Helpers.Log("CornerRadius's should not be equal.");
                _tempResult = false;
            }

            _crone = new CornerRadius(201);
            _crtwo = new CornerRadius(201);

            _childone.CornerRadius = _crone;
            _childtwo.CornerRadius = _crtwo;

            CommonFunctionality.FlushDispatcher();

            if (!_childone.CornerRadius.Equals(_childtwo.CornerRadius) || !_childtwo.CornerRadius.Equals(_childone.CornerRadius))
            {
                Helpers.Log("CornerRadius Compare Failed. [3]");
                Helpers.Log("CornerRadius's should be equal.");
                _tempResult = false;
            }

            _crone = new CornerRadius(401);
            _crtwo = new CornerRadius(201);

            _childone.CornerRadius = _crone;
            _childtwo.CornerRadius = _crtwo;

            CommonFunctionality.FlushDispatcher();

            if (_childone.CornerRadius.Equals(_childtwo.CornerRadius) || _childtwo.CornerRadius.Equals(_childone.CornerRadius))
            {
                Helpers.Log("CornerRadius Compare Failed. [4]");
                Helpers.Log("CornerRadius's should not be equal.");
                _tempResult = false;
            }
        }

        public override void TestVerify()
        {
            if (!_tempResult)
            {
                Helpers.Log("CornerRadius.Equals Test FAILED.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("CornerRadius.Equals Test PASSED.");
                this.Result = true;
            }
        }
    }

    [Test(3, "Regression", "RegressionTest5", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RegressionTest5 : CodeTest
    {
        public RegressionTest5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = this._winHeight;
            this.window.Width = this._winWidth;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        double _winHeight = 600;
        double _winWidth = 900;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            Viewbox vbx = new Viewbox();

            ListBox lbx = new ListBox();

            ListBoxItem item1 = new ListBoxItem();
            item1.Content = "aaaaaa";

            ListBoxItem item2 = new ListBoxItem();
            item2.Content = "aaaaaa";

            ListBoxItem item3 = new ListBoxItem();
            item3.Content = "aaaaaa";

            ListBoxItem item4 = new ListBoxItem();
            item4.Content = "aaaaaa";

            ListBoxItem item5 = new ListBoxItem();
            item5.Content = "aaaaaa";

            ListBoxItem item6 = new ListBoxItem();
            item6.Content = "aaaaaa";

            lbx.Items.Add(item1);
            lbx.Items.Add(item2);
            lbx.Items.Add(item3);
            lbx.Items.Add(item4);
            lbx.Items.Add(item5);
            lbx.Items.Add(item6);

            vbx.Child = lbx;

            root.Children.Add(vbx);

            return root;
        }

        public override void TestActions()
        {
            this.window.Height = 0;
            this.window.Width = 0;
            CommonFunctionality.FlushDispatcher();
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(3, "Regression", "RegressionTest6", Variables="Area=ElementLayout")]
    public class RegressionTest6 : CodeTest
    {
        public RegressionTest6()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Button _btn;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            Viewbox vbx = new Viewbox();
            vbx.Height = 300;
            vbx.Width = 300;

            _btn = new Button();
            _btn.Height = 100;
            _btn.Width = 100;
            _btn.Content = "I Button.";

            vbx.Child = _btn;

            root.Children.Add(vbx);

            return root;
        }

        System.Windows.Media.Animation.Storyboard _storyboard;

        public override void TestActions()
        {
            System.Windows.Media.Animation.DoubleAnimation wanim = new System.Windows.Media.Animation.DoubleAnimation();
            wanim.BeginTime = TimeSpan.FromSeconds(0);
            wanim.Duration = new Duration(TimeSpan.FromSeconds(5));
            wanim.To = 600;
            wanim.CurrentStateInvalidated += new EventHandler(wanimCurrentStateInvalidated);

            this._storyboard = new System.Windows.Media.Animation.Storyboard();
            this._storyboard.Name = "story";
            this._storyboard.Children.Add(wanim);

            PropertyPath path1 = new PropertyPath("(0)", new DependencyProperty[] { Button.WidthProperty });

            System.Windows.Media.Animation.Storyboard.SetTargetProperty(wanim, path1);

            this._storyboard.Begin(this._btn, System.Windows.Media.Animation.HandoffBehavior.SnapshotAndReplace, true);

        }

        bool _wanimfinished = false;
        void wanimCurrentStateInvalidated(object sender, EventArgs e)
        {
            Helpers.Log("State invalidated.");
            if (((System.Windows.Media.Animation.AnimationClock)sender).CurrentState == System.Windows.Media.Animation.ClockState.Active)
            {
                Helpers.Log("Animation Started...");
            }
            else
            {
                if (_btn.Width == 100)
                {
                    Helpers.Log("btn width still 100, no animation...");
                    _wanimfinished = false;
                    _resultset = true;
                    //EndTest(wanimfinished);
                }
                else
                {
                    Helpers.Log("btn inside viewbox animated via storyboard...");
                    _wanimfinished = true;
                    _resultset = true;
                    //EndTest(wanimfinished);
                }
            }
        }

        void CheckResult()
        {
            CommonFunctionality.FlushDispatcher();
            if (_resultset)
                _verifynow = true;
            else
                _verifynow = false;
        }

        bool _resultset = false;
        bool _verifynow = false;

        public override void TestVerify()
        {
            while (!_verifynow)
            {
                CommonFunctionality.FlushDispatcher();
                CheckResult();
            }

            if (!this._wanimfinished)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }

        }
    }

    [Test(3, "Regression", "RegressionTest7", Variables="Area=ElementLayout")]
    public class RegressionTest7 : CodeTest
    {
        /*
         * TransformDecorator: Changing properties on intermediate DockPanel can cause rotated text to lose rotation
         */

        public RegressionTest7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        DockPanel _dockPanel;
        TextBlock _textBlock;
        Decorator _td;

        double _preTestAnlge;

        public override FrameworkElement TestContent()
        {
            this._td = new Decorator();

            // Create a DockPanel under the TransformDecorator
            this._dockPanel = new DockPanel();
            this._dockPanel.LastChildFill = false;
            this._td.Child = this._dockPanel;
            this._dockPanel.Background = Brushes.CadetBlue;

            // Create a TextBlock to be rotated under the DockPanel
            this._textBlock = new TextBlock();
            this._textBlock.Background = Brushes.Orange;
            this._textBlock.Text = "Click Me!";
            this._textBlock.TextAlignment = TextAlignment.Center;
            this._textBlock.FontSize = 40;
            this._textBlock.LayoutTransform = new RotateTransform(-90.0);
            this._preTestAnlge = ((RotateTransform)this._textBlock.LayoutTransform).Angle;
            this._dockPanel.Children.Add(this._textBlock);

            return this._td;
        }

        public override void TestActions()
        {
            this._dockPanel.LastChildFill = true;
            Helpers.Log("DockPanel properties updated.");
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            double angle = ((RotateTransform)this._textBlock.LayoutTransform).Angle;

            if (this._dockPanel.LastChildFill == false)
            {
                Helpers.Log("DockPanel.LastChildFill does not equal true (no layout update happend).");
                this.Result = false;
            }
            else if (angle != _preTestAnlge)
            {
                Helpers.Log("Rotation was lost..");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Test Passed..");
                this.Result = true;
            }


        }

        void RotateHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //this.dockPanel.LastChildFill = true;
            //Helpers.Log("Left Click detected - DockPanel properties updated.");
        }
    }

    [Test(3, "Regression", "RegressionTest8", Variables="Area=ElementLayout")]
    public class RegressionTest8 : CodeTest
    {
        public RegressionTest8()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            string xamlfile = "RegressionTest9.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            try
            {
                this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            }
            catch (Exception ex)
            {
                Helpers.Log("Exception thrown loading xaml.");
                Helpers.Log(ex.Message);
                _tempresult = false;
            }

            f.Close();

        }

        public override FrameworkElement TestContent()
        {
            //need to load a xaml file;
            return null;
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                _loaded = CheckIfLoaded();
            }
            if (window.Content.GetType().Name != "Grid")
            {
                Helpers.Log("Grid was not window content.");
                _tempresult = false;
            }
            else
            {
                Grid wincontent = window.Content as Grid;

                if (wincontent.Children[0].GetType().Name != "ScrollViewer")
                {
                    Helpers.Log("Scrollviewer was not grid first child.");
                    _tempresult = false;
                }
                else
                {

                    ScrollViewer scroll = wincontent.Children[0] as ScrollViewer;

                    if (scroll.Content.GetType().Name != "StackPanel")
                    {
                        Helpers.Log("stackpanel was not scroller content");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("everything looks correct.");
                    }
                }
            }

        }

        bool _loaded = false;
        bool CheckIfLoaded()
        {
            if (((Grid)window.Content).IsLoaded)
            {

                return true;
            }
            else
            {
                Helpers.Log("not loaded.");
                return false;
            }
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
        }
    }

    [Test(3, "Regression", "RegressionTest10", Variables="Area=ElementLayout")]
    public class RegressionTest10 : CodeTest
    {

        public RegressionTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
            eRoot.ColumnDefinitions[0].MaxWidth = 150;
            eRoot.Background = Brushes.Beige;

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            Paragraph p = new Paragraph(new Run("With its fast and flexible playback features, Windows Media Player makes it easy to enjoy your favorite music and movies whenever you like, plus discover more with services offering premium entertainment."));
            tp.Document = new FlowDocument(p);


            //tp.Document.ContentStart.InsertTextInRun("With its fast and flexible playback features, Windows Media Player makes it easy to enjoy your favorite music and movies whenever you like, plus discover more with services offering premium entertainment.");
            eRoot.Children.Add(tp);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (Math.Abs(eRoot.ColumnDefinitions[0].ActualWidth - eRoot.ColumnDefinitions[0].MaxWidth) < 0.001) ? true : false;
            if (!this.Result) Helpers.Log("MaxWidth is not respected...");
        }
    }

    [Test(3, "Regression", "RegressionTest11", Variables="Area=ElementLayout")]
    public class RegressionTest11 : CodeTest
    {

        public RegressionTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gl_rt11 ={ new GridLength(5, GridUnitType.Star), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(3, 3, gl_rt11, gl_rt11);
            // PERCENT_REMOVED. please replace if necessary???
            Border b1 = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.Red), 100, 100);
            Border b2 = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.Blue), 100, 100);
            eRoot.Children.Add(b1);
            GridCommon.PlacingChild(eRoot, b2, 1, 1, true);

            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("\nGridWidth = " + eRoot.ActualWidth.ToString() + "\nTotal Column Width = " + GridCommon.TotalColumnWidth(eRoot).ToString());
            this.Result = (Math.Abs(eRoot.ActualWidth - GridCommon.TotalColumnWidth(eRoot)) < 0.001) ? true : false;
            if (!this.Result) Helpers.Log("Total Column width does not match with the grid width.");
        }
    }

    [Test(3, "Regression", "RegressionTest12", Variables="Area=ElementLayout")]
    public class RegressionTest12 : CodeTest
    {

        public RegressionTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gl_rt12 ={ new GridLength(100), new GridLength(2, GridUnitType.Star), new GridLength(100), new GridLength(1.5, GridUnitType.Star), new GridLength(3, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(5, 5, gl_rt12, gl_rt12);
            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("\n1.5* = " + eRoot.RowDefinitions[3].ActualHeight.ToString() + "\n3* = " + eRoot.RowDefinitions[4].ActualHeight.ToString());
            this.Result = (Math.Abs((eRoot.RowDefinitions[3].ActualHeight * 2) - eRoot.RowDefinitions[4].ActualHeight) < 0.001) ? true : false;
            if (!this.Result) Helpers.Log("\n* value is not calculated correctly.");
        }
    }

    [Test(3, "Regression", "RegressionTest13", Variables="Area=ElementLayout")]
    public class RegressionTest13 : CodeTest
    {

        public RegressionTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] g_rt13 = { new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(2, 2, g_rt13, g_rt13);
            Rectangle rect_rt13 = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Yellow));
            Grid.SetColumnSpan(rect_rt13, 2);
            Grid.SetRowSpan(rect_rt13, 2);
            eRoot.Children.Add(rect_rt13);
            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("\n1st Column Width: " + eRoot.ColumnDefinitions[0].ActualWidth.ToString());
            this.Result = (eRoot.ColumnDefinitions[0].ActualWidth == 0) ? true : false;
            if (!this.Result) Helpers.Log("\nColumn with Auto and ColumnSpan set is not calculated correctly.");
        }
    }

    [Test(3, "Regression", "RegressionTest14", Variables="Area=ElementLayout")]
    public class RegressionTest14 : CodeTest
    {

        public RegressionTest14()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] g_rt14 = { new GridLength(200, GridUnitType.Pixel), new GridLength(3, GridUnitType.Star), new GridLength(1, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(3, 3, g_rt14, g_rt14);
            Rectangle rect_rt14 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Yellow));
            GridCommon.PlacingChild(eRoot, rect_rt14, 2, 2, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            FrameworkElement gridChild = (FrameworkElement)eRoot.Children[0];
            Helpers.Log("\n* Width: " + eRoot.ColumnDefinitions[2].ActualWidth.ToString() + "\nRect Width: " + gridChild.ActualWidth.ToString());
            this.Result = (Math.Abs(eRoot.ColumnDefinitions[2].ActualWidth - gridChild.ActualWidth) < 0.001) ? true : false;
            if (!this.Result) Helpers.Log("Column Width and Child Width are not matched~");
        }
    }

    [Test(3, "Regression", "RegressionTest15", Variables="Area=ElementLayout")]
    public class RegressionTest15 : CodeTest
    {

        public RegressionTest15()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(1, 1);
            eRoot.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            Grid innerGrid = GridCommon.CreateGrid(1, 1);
            eRoot.Children.Add(innerGrid);
            TextBox tb = new TextBox();
            innerGrid.Children.Add(tb);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = true;
            Helpers.Log("No Crash occurred.");

        }
    }

    [Test(3, "Regression", "RegressionTest16", Variables="Area=ElementLayout")]
    public class RegressionTest16 : CodeTest
    {

        public RegressionTest16()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.ColumnDefinitions.Add(new ColumnDefinition());
            eRoot.ColumnDefinitions.Clear();
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (eRoot.ColumnDefinitions.Count == 0) ? true : false;
            if (!this.Result) Helpers.Log("ColumnDefinition is not cleared correctly.");
        }
    }

    [Test(3, "Regression", "RegressionTest17", Variables="Area=ElementLayout")]
    public class RegressionTest17 : CodeTest
    {

        public RegressionTest17()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            ScrollViewer sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            StackPanel sp = new StackPanel();
            byte color = 0;
            for (int i = 0; i < 10; i++)
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(150, 150, new SolidColorBrush(Color.FromRgb(color, color, 0)));
                sp.Children.Add(rect);
                color += 20;
            }
            sv.Content = sp;
            eRoot.Children.Add(sv);
            return eRoot;
        }
        public override void TestVerify()
        {
            ScrollViewer sv = eRoot.Children[0] as ScrollViewer;
            ScrollTestCommon.ScrollerVisibilities result = new ScrollTestCommon.ScrollerVisibilities(sv);
            this.Result = (result.vertical == ScrollBarVisibility.Visible) ? true : false;
            if (!this.Result) Helpers.Log("Vertical ScrollBar is not Visible...");
        }
    }

    [Test(3, "Regression", "RegressionTest18", Variables="Area=ElementLayout")]
    public class RegressionTest18 : CodeTest
    {
        public RegressionTest18()
        { }
        
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();
        }
        
        public Grid eRoot;
        public Image img;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
             img = new Image();
            
             img.Stretch = Stretch.Fill; 

            BitmapImage imgData = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));
            img.Source = imgData;
            eRoot.Children.Add(img);
            return eRoot;
        }
        
        public override void TestVerify()
        {
            if(!DoubleUtil.AreClose(eRoot.ColumnDefinitions[0].ActualWidth, img.ActualWidth))
            {
                this.Result = false;
                Helpers.Log("Grid Column is not sized to content.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Grid Column sized to content.");
            }
        }
    }

    [Test(3, "Regression", "RegressionTest19", Variables="Area=ElementLayout")]
    public class RegressionTest19 : CodeTest
    {

        public RegressionTest19()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            eRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = true;
            Helpers.Log("No Assertion occurred...");
        }
    }

    [Test(3, "Regression", "RegressionTest20", Variables="Area=ElementLayout")]
    public class RegressionTest20 : CodeTest
    {

        public RegressionTest20()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public string exceptionType = null;
        public override FrameworkElement TestContent()
        {
            try
            {
                GridLength gridLength = new GridLength(double.NaN);
            }
            catch (Exception e)
            {
                exceptionType = e.GetType().ToString();
            }
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (exceptionType == null) ? false : (exceptionType != "System.ArgumentException") ? false : true;
            if (!this.Result) Helpers.Log("Failed...System.ArgumentException NOT thrown..");
        }
    }

    [Test(3, "Regression", "RegressionTest21", Variables="Area=ElementLayout")]
    public class RegressionTest21 : CodeTest
    {
        public RegressionTest21()
        { }
     
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();
        }
        
        public Grid eRoot;
        
        public string expcetionType = null;
        
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            Rectangle rect = CommonFunctionality.CreateRectangle(10, 10, new SolidColorBrush(Colors.Red));
            eRoot.Children.Add(rect);
            try
            {
                Grid.SetColumn(rect, 5); //Column=5
            }
            catch (Exception e)
            {
                expcetionType = e.GetType().ToString();
            }
            return eRoot;
        }
        
        public override void TestVerify()
        {
            this.Result = (expcetionType != null) ? false : true;
            if (!this.Result) Helpers.Log("exception was trown~!");
        }
    }

    [Test(3, "Regression", "RegressionTest22", Variables="Area=ElementLayout")]
    public class RegressionTest22 : CodeTest
    {

        public RegressionTest22()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();
        }
        public Grid eRoot;
        public string exceptionType = null;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            try
            {
                eRoot.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                eRoot.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            }
            catch (Exception e)
            {
                exceptionType = e.GetType().ToString();
            }

            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (exceptionType == null) ? true : false;
            if (!this.Result) Helpers.Log("No exception expected, but " + exceptionType + " thrown..");
        }
    }

    [Test(3, "Regression", "RegressionTest23", Variables="Area=ElementLayout")]
    public class RegressionTest23 : CodeTest
    {

        public RegressionTest23()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            eRoot.Background = Brushes.Yellow;
            Grid grid_rt23 = GridCommon.CreateGrid(1, 1);
            grid_rt23.Background = Brushes.Orange;
            eRoot.Children.Add(grid_rt23);
            Rectangle rect_rt23 = CommonFunctionality.CreateRectangle(500, 500, new SolidColorBrush(Colors.Green));
            grid_rt23.Children.Add(rect_rt23);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (eRoot.ColumnDefinitions[0].ActualWidth == eRoot.ActualWidth / 2) ? true : false;
            if (!this.Result) Helpers.Log("\nColumn 0: " + eRoot.ColumnDefinitions[0].ActualWidth.ToString() + "\nColumn is not sized correctly.");
        }
    }

    [Test(3, "Regression", "RegressionTest24", Variables="Area=ElementLayout")]
    public class RegressionTest24 : CodeTest
    {

        public RegressionTest24()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            eRoot.Width = 500;
            Grid.SetIsSharedSizeScope(eRoot, true);
            Grid grid1 = GridCommon.CreateGrid(3, 1);
            grid1.Width = 400;
            grid1.Background = Brushes.Beige;
            grid1.ShowGridLines = true;
            grid1.ColumnDefinitions[0].Width = new GridLength(100);
            grid1.ColumnDefinitions[1].Width = new GridLength(200);
            grid1.ColumnDefinitions[0].SharedSizeGroup = "FirstColumn";
            grid1.ColumnDefinitions[1].SharedSizeGroup = "SecondColumn";

            TextBlock txt_rt24_0 = CommonFunctionality.CreateText("FirstColumn");
            TextBlock txt_rt24_1 = CommonFunctionality.CreateText("SecondColumn");
            GridCommon.PlacingChild(grid1, txt_rt24_0, 0, 0, true);
            GridCommon.PlacingChild(grid1, txt_rt24_1, 1, 0, true);

            Grid grid2 = GridCommon.CreateGrid(3, 1);
            grid2.Background = Brushes.Aqua;
            grid2.ShowGridLines = true;
            grid2.ColumnDefinitions[0].Width = new GridLength(300);
            grid2.ColumnDefinitions[0].SharedSizeGroup = "FirstColumn";
            grid2.ColumnDefinitions[1].SharedSizeGroup = "SecondColumn";
            System.Windows.Controls.TextBlock txt_rt24_2 = CommonFunctionality.CreateText("FirstColumn");
            System.Windows.Controls.TextBlock txt_rt24_3 = CommonFunctionality.CreateText("SecondColumn");
            GridCommon.PlacingChild(grid2, txt_rt24_2, 0, 0, true);
            GridCommon.PlacingChild(grid2, txt_rt24_3, 1, 0, true);

            GridCommon.PlacingChild(eRoot, grid1, 0, 0, true);
            GridCommon.PlacingChild(eRoot, grid2, 1, 1, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            Grid g1 = eRoot.Children[0] as Grid;
            Grid g2 = eRoot.Children[1] as Grid;

            this.Result = ((g1.ColumnDefinitions[0].ActualWidth == g2.ColumnDefinitions[0].ActualWidth) && (g1.ColumnDefinitions[1].ActualWidth == g2.ColumnDefinitions[1].ActualWidth)) ? true : false;
            if (!this.Result) Helpers.Log("Grid1 Column[0]:" + g1.ColumnDefinitions[0].ActualWidth.ToString()
                    + "Grid1 Column[1]:" + g1.ColumnDefinitions[1].ActualWidth.ToString()
                    + "Grid2 Column[0]:" + g1.ColumnDefinitions[0].ActualWidth.ToString()
                    + "Grid2 Column[1]:" + g1.ColumnDefinitions[1].ActualWidth.ToString());
        }
    }

    [Test(3, "Regression", "RegressionTest25", Variables="Area=ElementLayout")]
    public class RegressionTest25 : CodeTest
    {

        public RegressionTest25()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public string exceptionType = null;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Width = 500;
            Grid.SetIsSharedSizeScope(eRoot, true);
            Grid grid1 = GridCommon.CreateGrid(3, 1);
            eRoot.Children.Add(grid1);
            try
            {
                grid1.ColumnDefinitions[0].SharedSizeGroup = "?";
            }
            catch (Exception e)
            {
                exceptionType = e.GetType().ToString();
            }
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (exceptionType == null) ? false : true;
            if (!this.Result) Helpers.Log("Exception is not thrown ...");
        }
    }

    [Test(3, "Regression", "RegressionTest252", Variables="Area=ElementLayout")]
    public class RegressionTest252 : CodeTest
    {

        public RegressionTest252()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public string exceptionType = null;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Width = 500;
            Grid.SetIsSharedSizeScope(eRoot, true);
            Grid grid1 = GridCommon.CreateGrid(3, 1);
            eRoot.Children.Add(grid1);
            try
            {
                grid1.ColumnDefinitions[0].SharedSizeGroup = "Regression";
            }
            catch (Exception e)
            {
                exceptionType = e.GetType().ToString();
            }
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = (exceptionType != null) ? false : true;
            if (!this.Result) Helpers.Log("Exception was thrown ...");
        }
    }

    [Test(3, "Regression", "RegressionTest27", Variables="Area=ElementLayout")]
    public class RegressionTest27 : CodeTest
    {

        public RegressionTest27()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Width = 500;
            Grid.SetIsSharedSizeScope(eRoot, true);

            Grid grid1 = GridCommon.CreateGrid(3, 2);
            grid1.Width = 600;
            grid1.Background = Brushes.Beige;
            grid1.ShowGridLines = true;
            grid1.ColumnDefinitions[0].Width = new GridLength(300);
            grid1.ColumnDefinitions[1].Width = new GridLength(200);
            grid1.ColumnDefinitions[0].SharedSizeGroup = "FirstColumn";
            grid1.ColumnDefinitions[1].SharedSizeGroup = "SecondColumn";

            TextBlock txt_rt27_0 = CommonFunctionality.CreateText("FirstColumn");
            TextBlock txt_rt27_1 = CommonFunctionality.CreateText("SecondColumn");
            GridCommon.PlacingChild(grid1, txt_rt27_0, 0, 0, true);
            GridCommon.PlacingChild(grid1, txt_rt27_1, 1, 0, true);

            Grid grid2 = GridCommon.CreateGrid(3, 2);
            GridCommon.SettingSpan(grid2, 2, 1);
            grid2.Background = Brushes.Aqua;
            grid2.ShowGridLines = true;
            grid2.ColumnDefinitions[0].SharedSizeGroup = "FirstColumn";
            grid2.ColumnDefinitions[1].SharedSizeGroup = "SecondColumn";
            System.Windows.Controls.TextBlock txt_rt27_2 = CommonFunctionality.CreateText("FirstColumn");
            System.Windows.Controls.TextBlock txt_rt27_3 = CommonFunctionality.CreateText("SecondColumn");
            GridCommon.PlacingChild(grid2, txt_rt27_2, 0, 0, true);
            GridCommon.PlacingChild(grid2, txt_rt27_3, 1, 0, true);

            GridCommon.PlacingChild(grid1, grid2, 0, 1, true);
            eRoot.Children.Add(grid1);
            return eRoot;
        }
        public override void TestVerify()
        {
            Grid g1 = eRoot.Children[0] as Grid;
            Grid g2 = g1.Children[2] as Grid;

            this.Result = ((g1.ColumnDefinitions[0].ActualWidth == g2.ColumnDefinitions[0].ActualWidth) && (g1.ColumnDefinitions[1].ActualWidth == g2.ColumnDefinitions[1].ActualWidth)) ? true : false;
            if (!this.Result)
                Helpers.Log("Grid1 Column[0]:" + g1.ColumnDefinitions[0].ActualWidth.ToString()
                    + "Grid1 Column[1]:" + g1.ColumnDefinitions[1].ActualWidth.ToString()
                    + "Grid2 Column[0]:" + g1.ColumnDefinitions[0].ActualWidth.ToString()
                    + "Grid2 Column[1]:" + g1.ColumnDefinitions[1].ActualWidth.ToString());
        }
    }

    [Test(3, "Regression", "RegressionTest28", Variables="Area=ElementLayout")]
    public class RegressionTest28 : CodeTest
    {

        public RegressionTest28()
        {
        }
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Background = Brushes.Silver;
            StackPanel stack = new StackPanel();
            stack.Height = 200;
            stack.Width = 200;
            stack.Background = Brushes.Crimson;
            eRoot.Children.Add(stack);
            return eRoot;
        }

        bool _result = true;
        string _resultTxt = null;
        
        public override void TestActions()
        {
            try
            {
                if (eRoot.ColumnDefinitions.Count > 1) { }
            }
            catch (Exception ex)
            {
                _result = false;
                _resultTxt += "\n" + ex.Message + " when eRoot.ColumnDefinitions.Count > 1 used.";
            }

            try
            {
                if (eRoot.RowDefinitions.Count > 1) { }
            }
            catch (Exception ex)
            {
                _result = false;
                _resultTxt += "\n" + ex.Message + " when eRoot.RowDefinitions.Count > 1 used.";
            }
        }
        public override void TestVerify()
        {
            this.Result = _result;
            if (_result) Helpers.Log("Passed - No exception thrown...");
            else Helpers.Log("Fail - " + _resultTxt);
        }
    }

    [Test(3, "Regression", "RegressionTest29", Variables="Area=ElementLayout")]
    public class RegressionTest29 : CodeTest
    {
        public RegressionTest29()
        {
        }
      
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();

        }
        
        public Grid eRoot;
        
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Background = Brushes.Silver;
            return eRoot;
        }

        bool _result = true;
        string _resultTxt = null;
        
        public override void TestActions()
        {
            string xamlfile = "colsinrows.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            try
            {
                eRoot = (Grid)System.Windows.Markup.XamlReader.Load(f);
                _resultTxt += "Should not have loaded grid with cols in row def.";
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
            }

            f.Close();

            xamlfile = "rowsincols.xaml";
            f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            try
            {
                eRoot = (Grid)System.Windows.Markup.XamlReader.Load(f);
                _resultTxt += "Should not have loaded grid with rows in column def.";
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
            }

            f.Close();
        }
        
        public override void TestVerify()
        {
            this.Result = _result;
            if (_result) Helpers.Log("Passed - Correct exception thrown...");
            else Helpers.Log("Fail - " + _resultTxt);
        }
    }

    [Test(3, "Regression", "RegressionTest30", Disabled = true, Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RegressionTest30 : CodeTest
    {
        public RegressionTest30()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ToggleGridLines.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Grid _grid1;
        Button _btn;

        public override void TestActions()
        {
            _grid1 = LogicalTreeHelper.FindLogicalNode(this.window, "grid1") as Grid;
            _btn = LogicalTreeHelper.FindLogicalNode(this.window, "button1") as Button;

            if (_grid1 == null || _btn == null)
            {
                CommonFunctionality.FlushDispatcher();
                this.TestActions();
            }
            
            // removed click events.  not important to this test case.  just need to update grid lines.

            this.TestAction();
            DispatcherHelper.DoEvents();
        }

        private void TestAction()
        {
            this._grid1.ShowGridLines = !this._grid1.ShowGridLines;
            this._grid1.Children.Remove(this._btn);
            if (this._grid1.ShowGridLines)
                _tempresult = false;
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
                VScanCommon tool = new VScanCommon(this);
                if (!tool.CompareImage())
                {
                    this.Result = false;
                }
                else
                {
                    this.Result = true;
                }
            }
        }
    }

    [Test(3, "Regression", "RegressionTest31", Variables="Area=ElementLayout")]
    public class RegressionTest31 : CodeTest
    {
        public RegressionTest31()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.CornflowerBlue;

            _r1 = new RowDefinition();
            _r2 = new RowDefinition();

            _root.RowDefinitions.Add(_r1);
            _root.RowDefinitions.Add(_r2);


            _lbx = new ListBox();
            AddItems(_lbx);

            _root.Children.Add(_lbx);


            return _root;
        }

        Grid _root;
        RowDefinition _r1;
        RowDefinition _r2;
        ListBox _lbx;

        public override void TestActions()
        {
            _r1.Height = new GridLength(1, GridUnitType.Auto);
            _r2.Height = new GridLength(1, GridUnitType.Auto);
            CommonFunctionality.FlushDispatcher();

            ScrollBar s = findScrollbar(_lbx) as ScrollBar;

            if (isVisible(s))
                _tempresult = false;

            _r1.Height = new GridLength(1, GridUnitType.Star);
            _r2.Height = new GridLength(1, GridUnitType.Auto);
            CommonFunctionality.FlushDispatcher();

            s = findScrollbar(_lbx) as ScrollBar;

            if (!isVisible(s))
                _tempresult = false;
        }

        void AddItems(ListBox listbox)
        {
            for (int i = 0; i <= 50; i++)
            {
                Button b = new Button();
                b.Content = i.ToString();
                listbox.Items.Add(b);
            }
        }

        bool _tempresult = true;

        object findScrollbar(ListBox listbox)
        {
            return LayoutUtility.GetChildFromVisualTree((UIElement)listbox, typeof(ScrollBar));
        }

        bool isVisible(ScrollBar s)
        {
            if (s.IsVisible)
                return true;
            else
                return false;
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    //[Test(3, "Regression", "RegressionTest32", Variables="Area=ElementLayout")]
    //public class RegressionTest32 : CodeTest
    //{
    //    public RegressionTest32()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 600;
    //        this.window.Width = 800;
    //        this.window.Top = 0;
    //        this.window.Left = 0;

    //        FileStream fs = new FileStream("bad_transform.xaml", FileMode.Open);

    //        this.window.Content = XamlReader.Load(fs) as FrameworkElement;

    //        fs.Close();

    //        timer = new DispatcherTimer();
    //        timer.Interval = new TimeSpan(0, 0, 1);
    //        timer.Tick += new EventHandler(timer_Tick);
    //        timer.Start();
    //    }

    //    DispatcherTimer timer;
    //    Rectangle minutes = null;
    //    bool result = true;

    //    public override void TestActions()
    //    {
    //        minutes = LogicalTreeHelper.FindLogicalNode(window, "MinuteHand") as Rectangle;

    //        if (minutes == null)
    //        {
    //            result = false;
    //            Helpers.Log("minute hand was null");
    //        }
    //        else
    //        {
    //            Helpers.Log("found minute hand, continue test...");
    //            for (int i = 0; i < 10; i++)
    //            {
    //                    Point p = LayoutUtility.GetElementPosition(minutes, this.window);
    //                    range[counter] = p;
    //                    counter++;
    //            }
    //            timer.Stop();
    //            GetMaxMinValues(range);
    //            CommonFunctionality.FlushDispatcher();
    //            Test(null);
    //        }
    //    }

    //    object Test(object o)
    //    {


    //        return null;
    //    }

    //    double minX = 1000;
    //    double maxX = 0;
    //    double minY = 1000;
    //    double maxY = 0;

    //    void GetMaxMinValues(Point[] range)
    //    {
    //        foreach (Point p in range)
    //        {
    //            //add 3 pixel buffer.  range is not gathered at everypoint on ellipse, so there could be a slight range difference.
    //            if (p.X < minX) minX = p.X - 3;
    //            if (p.X > maxX) maxX = p.X + 3;
    //            if (p.Y < minY) minY = p.Y - 3;
    //            if (p.Y > maxY) maxY = p.Y + 3;
    //        }
    //    }

    //    Point[] range = new Point[10];

    //    int counter = 0;

    //    void timer_Tick(object sender, EventArgs e)
    //    {
    //        GlobalLog.LogEvidence("TICK");
    //    }

    //    public override void TestVerify()
    //    {
    //        window.Height = 100;
    //        CommonFunctionality.FlushDispatcher();

    //        Point p = LayoutUtility.GetElementPosition(minutes, window);
    //        //Helpers.Log(p.ToString());

    //        CommonFunctionality.FlushDispatcher();

    //        GlobalLog.LogEvidence(minX.ToString());
    //        GlobalLog.LogEvidence(maxX.ToString());
    //        GlobalLog.LogEvidence(p.X.ToString());

    //        if (DoubleUtil.LessThanOrClose(minX, p.X) && DoubleUtil.GreaterThanOrClose(maxX, p.X))
    //        {
    //            Helpers.Log("X is in range.  Transform is not corrupted.");
    //        }
    //        else
    //        {
    //            Helpers.Log("X is not in range.  Transform is corrupted.");
    //            result = false;
    //        }

    //        if (DoubleUtil.LessThanOrClose(minY, p.Y) && DoubleUtil.GreaterThanOrClose(maxY, p.Y))
    //        {
    //            Helpers.Log("Y is in range.  Transform is not corrupted.");
    //        }
    //        else
    //        {
    //            Helpers.Log("Y is not in range.  Transform is corrupted.");
    //            result = false;
    //        }
    //        this.Result = result;
        
    //    }
    //}




    //[Test(2, "Regression", "RegressionTest33",    
    //    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest33 : VisualScanTest
    //{
    //    public RegressionTest33()
    //        : base("RegressionTest33.xaml")
    //    { }
    //}

    //[Test(2, "Regression", "RegressionTest34",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest34 : VisualScanTest
    //{
    //    public RegressionTest34()
    //        : base("RegressionTest34.xaml")
    //    { }
    //}

    //[Test(2, "Regression", "RegressionTest35",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest35 : VisualScanTest
    //{
    //    public RegressionTest35()
    //        : base("RegressionTest35.xaml")
    //    { }
    //}
    //[Test(2, "Regression", "RegressionTest9",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest9 : VisualScanTest
    //{
    //    public RegressionTest9()
    //        : base("RegressionTest9.xaml")
    //    { }
    //}
    //[Test(2, "Regression", "RegressionTest36",
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest36 : VisualScanTest
    //{
    //    public RegressionTest36()
    //        : base("RegressionTest36.xaml")
    //    { }
    //}
    //[Test(2, "Regression", "RegressionTest37",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest37 : VisualScanTest
    //{
    //    public RegressionTest37()
    //        : base("RegressionTest37.xaml")
    //    { }
    //}
    //[Test(2, "Regression", "RegressionTest38",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest38 : VisualScanTest
    //{
    //    public RegressionTest38()
    //        : base("RegressionTest38.xaml")
    //    { }
    //}
    //[Test(2, "Regression", "RegressionTest39",    
    //Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    //public class RegressionTest39 : VisualScanTest
    //{
    //    public RegressionTest39()
    //        : base("RegressionTest39.xaml")
    //    { }
    //}
}
