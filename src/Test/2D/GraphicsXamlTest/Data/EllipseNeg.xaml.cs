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

    public partial class EllipseNeg : Window
    {

        public EllipseNeg()
        {
            InitializeComponent();
        }
        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "EllipseNeg1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make Ellipse negative");
            MyEllipse.SetValue(System.Windows.Controls.Canvas.LeftProperty, -108d);
            MyEllipse.SetValue(System.Windows.Controls.Canvas.TopProperty, 72d);
            MyEllipse.Width = 156;
            MyEllipse.Height = 156;

            return null;
        }

        public object Verify(object arg)
        {
            double left = (double)MyEllipse.GetValue(System.Windows.Controls.Canvas.LeftProperty);
            if (XamlTestHelper.Compare("EllipseNeg.bmp") && left == -108d)
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
