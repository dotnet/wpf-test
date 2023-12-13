// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Data;
using System.Xml;
using System.Configuration;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class TestPolyline : Window
    {
        public TestPolyline()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "TestPolyline1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Remove the Polyline points");
            MyPolyline.Points = null;
            return null;
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.Compare("TestPolyline.bmp") && MyPolyline.Points == null)
            {
                XamlTestHelper.LogStatus("Polyline contains no points");
            }
            else
            {
                XamlTestHelper.LogFail("Polyline contains some points");
            }

            return null;
        }


    }
}