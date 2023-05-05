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
    /// This contains all code for all Scale Transform priority cases.
    /// 
    //////////////////////////////////////////////////////////////////////////////

    [Test(2, "Transforms.Scale", "ScaleTransform1", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScaleTransform1 : CodeTest
    {
        public ScaleTransform1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "canvas_scale.xaml";
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

    [Test(2, "Transforms.Scale", "ScaleTransform2", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScaleTransform2 : CodeTest
    {
        public ScaleTransform2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "dock_scale.xaml";
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

    [Test(2, "Transforms.Scale", "ScaleTransform3", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScaleTransform3 : CodeTest
    {
        public ScaleTransform3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "Grid_scale.xaml";
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

    [Test(2, "Transforms.Scale", "ScaleTransform4", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScaleTransform4 : CodeTest
    {
        public ScaleTransform4()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "Table_scale.xaml";
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

    [Test(2, "Transforms.Scale", "ScaleTransform5", Disabled = true, Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScaleTransform5 : CodeTest
    {
        public ScaleTransform5()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "flowdoc_scale.xaml";
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
