// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Shapes;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Layout.PropertyDump;
using System.Globalization;

namespace ElementLayout.FeatureTests.Transforms
{
    [Test(2, "Transforms.Layout", "BasicTransformTest01", TestParameters = "test=ViewBox:Scale:1.5:1.5:Rotate:90:null", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest02", TestParameters = "test=Border:Rotate:30:null:Scale:1.5:1.5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest03", TestParameters = "test=DockPanel:Skew:10:20:Rotate:90:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest04", TestParameters = "test=Button:Rotate:45:null:Scale:10:10:Skew:10:10", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest05", TestParameters = "test=Grid:Rotate:25:null:Scale:1.5:1.5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest06", TestParameters = "test=Button:Rotate:45:null:Skew:10:20", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest07", TestParameters = "test=Grid:Skew:10:20:Rotate:30:null:Scale:1.5:1.5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest08", TestParameters = "test=Image:Scale:2:1:Rotate:45:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest09", TestParameters = "test=Label:Scale:1:2:Skew:30:30", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest10", TestParameters = "test=TextBox:Rotate:45:null:Scale:2:2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest11", TestParameters = "test=Image:Skew:20:10:Scale:2.5:2.5:Rotate:30:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest12", TestParameters = "test=DockPanel:Scale:1.5:1.5:Skew:10:10:Rotate:45:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest13", TestParameters = "test=RichTextBox:Rotate:180:null:Skew:15:15", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest14", TestParameters = "test=TabControl:Skew:10:20:Rotate:45:null", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest15", TestParameters = "test=HyperLink:Rotate:40:null:Skew:20:20", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest16", TestParameters = "test=ListBox:Skew:10:20:Rotate:270:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest17", TestParameters = "test=Menu:Skew:20:10:Scale:1.5:2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest18", TestParameters = "test=ScrollViewer:Scale:2:1:Rotate:360:null", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest19", TestParameters = "test=StackPanel:Scale:1:2:Skew:10:10", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest20", TestParameters = "test=Border:Rotate:90:null:Skew:15:30:Scale:2:2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest21", TestParameters = "test=Canvas:Scale:2:2:Rotate:45:null:Skew:15:15", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest22", TestParameters = "test=Panel:Rotate:90:null:Scale:1:1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest23", TestParameters = "test=ToolBar:Rotate:90:null:Skew:10:10", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest24", TestParameters = "test=Canvas:Scale:2:1:Rotate:45:null", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest25", TestParameters = "test=DocumentViewer:Skew:20:10:Scale:2:1.5", Disabled = true, Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest26", TestParameters = "test=ComboBox:Scale:1:2:Skew:15:15", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "BasicTransformTest27", TestParameters = "test=TextBlock:Skew:20:10:Scale:1.5:1.5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BasicTransformTest : CodeTest
    {
        private Grid _parent;
        private FrameworkElement _target;

        // Test Paramaters
        // [0] = Target Element for test
        // [1] = Transform 1 type
        // [2] = Transform 1 value 1
        // [3] = Transform 1 value 2
        // [4] = Transform 2 type
        // [5] = Transform 2 value 1
        // [6] = Transform 2 value 2
        // [4] = Transform 3 type
        // [5] = Transform 3 value 1
        // [6] = Transform 3 value 2
        string[] _testParams = null;

        public BasicTransformTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            _testParams = ParseParams();
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            _parent = GridCommon.CreateGrid(1, 1);

            //Adding target Element
            _target = LayoutTransformHelpers.AddingChild(_testParams[0]);

            //Applying LayoutTransform
            TransformGroup group = new TransformGroup();
            TransformCollection collection = new TransformCollection();

            for (int i = 1; i < _testParams.Length; i += 3)
            {
                // Now call Double.Parse with CultureInfo.CreateSpecificCulture("en-US") instead of CultureInfo.CurrentUICulture
                // because how different languages interperet '.'
                // example in german 1.5 gets converted to 15
                switch (_testParams[i])
                {
                    case "Rotate":
                        collection.Add(new RotateTransform(Double.Parse(_testParams[i + 1], CultureInfo.CreateSpecificCulture("en-US"))));
                        break;
                    case "Scale":
                        collection.Add(new ScaleTransform(Double.Parse(_testParams[i + 1], CultureInfo.CreateSpecificCulture("en-US")), Double.Parse(_testParams[i + 2], CultureInfo.CreateSpecificCulture("en-US"))));
                        break;
                    case "Skew":
                        collection.Add(new SkewTransform(Double.Parse(_testParams[i + 1], CultureInfo.CreateSpecificCulture("en-US")), Double.Parse(_testParams[i + 2], CultureInfo.CreateSpecificCulture("en-US"))));
                        break;
                    default:
                        break;
                }
            }

            group.Children = collection;
            _target.LayoutTransform = group;
            _parent.Children.Add(_target);

            return _parent;
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private string[] ParseParams()
        {
            string allParams = DriverState.DriverParameters["test"].ToString();
            return allParams.Split(':');
        }
    }

    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment01", TestParameters = "test=VABRotate4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment02", TestParameters = "test=HABRotate2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment03", TestParameters = "test=HABRotate1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment04", TestParameters = "test=HARotate2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment05", TestParameters = "test=HABSkew2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment06", TestParameters = "test=VASkew5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment07", TestParameters = "test=VABRotate2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment08", TestParameters = "test=VABRotate3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment09", TestParameters = "test=VABScale4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment10", TestParameters = "test=VASkew4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment11", TestParameters = "test=VARotate3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment12", TestParameters = "test=VASkew3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment13", TestParameters = "test=HASkew3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment14", TestParameters = "test=HABScale1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment15", TestParameters = "test=VABScale3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment16", TestParameters = "test=HARotate1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment17", TestParameters = "test=VASkew2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment18", TestParameters = "test=HARotate3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment19", TestParameters = "test=HARotate4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment20", TestParameters = "test=VABScale2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment21", TestParameters = "test=HABScale3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment22", TestParameters = "test=HABScale4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment23", TestParameters = "test=VABScale1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment24", TestParameters = "test=VABScale5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment25", TestParameters = "test=HABSkew1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment26", TestParameters = "test=HAScale1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment27", TestParameters = "test=VARotate1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment28", TestParameters = "test=VARotate2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment29", TestParameters = "test=HABRotate4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment30", TestParameters = "test=VARotate4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment31", TestParameters = "test=VARotate1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment32", TestParameters = "test=HABSkew4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment33", TestParameters = "test=HAScale2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment34", TestParameters = "test=HABRotate3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment35", TestParameters = "test=VABSkew3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment36", TestParameters = "test=VABRotate1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment37", TestParameters = "test=HABSkew3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment38", TestParameters = "test=VABSkew1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment39", TestParameters = "test=VABSkew2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment40", TestParameters = "test=VABSkew4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment41", TestParameters = "test=VABSkew5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment42", TestParameters = "test=HAScale4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment43", TestParameters = "test=VAScale1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment44", TestParameters = "test=VAScale3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment45", TestParameters = "test=VAScale4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment46", TestParameters = "test=HAScale3", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment47", TestParameters = "test=HASkew1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment48", TestParameters = "test=HASkew2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment49", TestParameters = "test=VAScale2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment50", TestParameters = "test=HASkew4", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment51", TestParameters = "test=VASkew1", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment52", TestParameters = "test=VAScale5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment53", TestParameters = "test=VABRotate5", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignment54", TestParameters = "test=HABScale2", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class LayoutTransformWithAlignment : CodeTest
    {
        public LayoutTransformWithAlignment()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            FrameworkElement content = LayoutTransformHelpers.AlignmentTestContent(DriverState.DriverParameters["test"].ToString());
            return content;
        }

        public override void TestVerify()
        {
            ((FrameworkElement)this.window.Content).Height = 564;
            ((FrameworkElement)this.window.Content).Width = 784;
            CommonFunctionality.FlushDispatcher();
            VScanCommon vscan = new VScanCommon(this.window, this, DriverState.TestName);
            this.Result = vscan.CompareImage();
        }
    }
   
    [Test(2, "Transforms.Layout", "LayoutTransformWithMargin",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class LayoutTransformWithMargin : VisualScanTest
    {
        public LayoutTransformWithMargin()
            : base("LayoutTransformWithMargin.xaml")
        { }
    }

    [Test(2, "Transforms.Layout", "LayoutTransformWithPadding",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class LayoutTransformWithPadding : VisualScanTest
    {
        public LayoutTransformWithPadding()
            : base("LayoutTransformWithPadding.xaml")
        { }
    }

    [Test(2, "Transforms.Layout", "RotatedButton", Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class RotatedButton : VisualScanTest
    {
        public RotatedButton()
            : base("RotatedButton.xaml")
        { }
    }

    [Test(2, "Transforms.Layout", "ScaledButton", Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class ScaledButton : VisualScanTest
    {
        public ScaledButton()
            : base("ScaledButton.xaml", Helpers.LoadStyle("genericcontrols.xaml"))
        { }
    }

    [Test(2, "Transforms.Layout", "VerticalMenuBar", Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class VerticalMenuBar : VisualScanTest
    {
        public VerticalMenuBar() 
            : base("VerticalMenuBar.xaml", Helpers.LoadStyle("genericcontrols.xaml"))
        { }
    }

    [Test(2, "Transforms.Layout", "VerticalToolBar", Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class VerticalToolBar : VisualScanTest
    {
        public VerticalToolBar()
            : base("VerticalToolBar.xaml", Helpers.LoadStyle("genericcontrols.xaml"))
        { }
    }

    [Test(2, "Transforms.Layout", "LayoutTransformWithAlignments",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class LayoutTransformWithAlignments : VisualScanTest
    {
        public LayoutTransformWithAlignments()
            : base("LayoutTransformWithAlignments.xaml")
        { }
    }
}
