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

    public partial class GridPanelTest : Window
    {

        public GridPanelTest()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "GridPanelTest1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Change the size of the rectangle");
            Rect1.Width = 350;
            Rect1.Height = 450;
            return null;
        }

        public object Verify(object arg)
        {

            if (XamlTestHelper.Compare("GridpanelTest.bmp") && Rect1.Width == 350 && Rect1.Height == 450)
            {
                XamlTestHelper.LogStatus("Pass: the layout is correct");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: the new layout is not correct");
            }

            return null;
        }

    }
}
