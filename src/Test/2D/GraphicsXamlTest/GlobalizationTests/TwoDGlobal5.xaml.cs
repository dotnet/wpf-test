// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//This is a list of commonly used namespaces for an application class.
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
    /// Test contains: Polygon, LinearGradientbrush and RotateTransform.
    /// Try to verify all those 2D elements can be accessed in a satallite assembly.
    /// </summary>
    public partial class TwoDGlobal5Test : Window
    {

        public TwoDGlobal5Test()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "TwoDGlobal5.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object Verify(object arg)
        {
            if ( XamlTestHelper.Compare("TwoDGlobal5Master.bmp") )
            {
                XamlTestHelper.LogStatus("Pass: they render correctly");
            }
            else
            {
                XamlTestHelper.LogStatus("Fail: incorrectly rendered");
            }

            return null;
        }

    }
}