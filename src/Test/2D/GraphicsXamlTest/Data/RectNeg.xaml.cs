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

    public partial class RectNeg : Window
    {

        public RectNeg()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "RectNeg1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make the points negative");


            //MyRect.RectangleTop  = new Length(100);
            //MyRect.RectangleLeft = new Length(-50);
            System.Windows.Controls.Canvas.SetLeft(MyRect, -50);
            System.Windows.Controls.Canvas.SetTop(MyRect, 100);
            MyRect.Width = 250;
            MyRect.Height = 150;
            return null;
        }

        public object Verify(object arg)
        {

            double cLeft = System.Windows.Controls.Canvas.GetLeft(MyRect);

            if (XamlTestHelper.Compare("RectNeg.bmp") && cLeft == -50)
            {
                XamlTestHelper.LogStatus("Pass: the first Point is negative");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: the first point is not negative");
            }

            return null;
        }


    }
}