// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class DockPanelTest : Window
    {

        public DockPanelTest()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "DockPanelTest1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        
        {
            XamlTestHelper.LogStatus("Change the dockings of the rectangles");
            RectRed.SetValue(DockPanel.DockProperty, Dock.Left);
            RectPurple.SetValue(DockPanel.DockProperty, Dock.Right);
            RectBlue.SetValue(DockPanel.DockProperty, Dock.Bottom);
            return null;
        }

        public object Verify(object arg)
        {
            //Dock BlueDock = System.Windows.Controls.DockPanel.GetDock(RectBlue);

            if ( XamlTestHelper.Compare("DockpanelTest.bmp") )
            {
                XamlTestHelper.LogStatus ("Pass: the new dock is correct");
            }
            else
            {
                XamlTestHelper.LogFail ("Fail: the new dock is not correct");
            }

            return null;
        }

    }
}