// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Data;
using System.Xml;
using System.Configuration;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class TestPolygon : Window
    {
        public TestPolygon()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "TestPolygon1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Remove the Polygon points");
            MyPolygon.Points = null;
            return null;
        }

        public object Verify(object arg)
        {

            if (XamlTestHelper.Compare("TestPolygon.bmp") && MyPolygon.Points == null)
            {
                XamlTestHelper.LogStatus( "Yeah, The points are null");
                XamlTestHelper.LogStatus( "Test Passed");
            }
            else
            {
                XamlTestHelper.LogFail( "Damn, The points are not null = fail");
                XamlTestHelper.LogFail( "Test failed");
            }

            return null;
        }


    }
}