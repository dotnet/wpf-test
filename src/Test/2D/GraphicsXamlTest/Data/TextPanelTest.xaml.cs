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

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class TextPanelTest : Window
    {

        public TextPanelTest()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "TextPanelTest1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make a bigger rectangle");

            Rect1.Width = 350;
            Rect1.Height = 250;
            return null;
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.Compare("TextPanelTest.bmp") && Rect1.Width == 350 && Rect1.Height == 250)
            {
                XamlTestHelper.LogStatus("Pass: the rectangle renders correctly");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: the rectangle does not render correctly");
            }
            return null;
        }


    }
}