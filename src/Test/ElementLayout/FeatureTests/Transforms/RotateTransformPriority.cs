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
using Microsoft.Test.RenderingVerification;
using System.IO;

namespace ElementLayout.FeatureTests.Transforms
{
    //////////////////////////////////////////////////////////////////////////////
    /// This contains all code for all Rotoate Transform priority cases.
    /// 
    //////////////////////////////////////////////////////////////////////////////

    [Test(2, "Transforms.Rotate", "RotateTransform1", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RotateTransform1 : CodeTest
    {
        public RotateTransform1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "canvas_rotate.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Rotate", "RotateTransform2", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RotateTransform2 : CodeTest
    {
        public RotateTransform2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "dock_rotate.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Rotate", "RotateTransform3", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RotateTransform3 : CodeTest
    {
        public RotateTransform3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "Grid_rotate.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Rotate", "RotateTransform4", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RotateTransform4 : CodeTest
    {
        public RotateTransform4()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "Table_rotate.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }

    [Test(2, "Transforms.Rotate", "RotateTransform5", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class RotateTransform5 : CodeTest
    {
        public RotateTransform5()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "flowdoc_rotate.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestVerify()
        {
            VScanCommon verify = new VScanCommon(this);
            this.Result = verify.CompareImage();
        }
    }
}
