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

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class LineNeg : Window
    {

        public LineNeg()
        {
            InitializeComponent();
        }
        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "LineNeg1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make the line negative");
            MyLine.X1 = -50;
            MyLine.Y1 = 10;
            MyLine.X2 = 250;
            MyLine.Y2 = 300;
            return null;
        }

        public object Verify(object arg)
        {

            if (XamlTestHelper.Compare("LineNeg.bmp") && MyLine.X1 == -50)
            {
                XamlTestHelper.LogStatus("Yeah, the first point is negative");
            }
            else
            {
                XamlTestHelper.LogFail("No the first point is not negative= fail");
            }

            return null;
        }


    }
}