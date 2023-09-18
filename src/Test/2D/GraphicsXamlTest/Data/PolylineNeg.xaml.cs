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

//------------------------------------------------------------------
// Inject SD stamp into the assembly for each file
namespace List.Of.Sources
{
    public class PolylineNeg_xaml_cs
    {
        public static string version = "$Id: $ $Change: $";
    }
}

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class PolylineNeg : Window
    {
    public PolylineNeg()
    {
        InitializeComponent();
    }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "PolylineNeg1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

         public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make the points negative");


        Polyline p = MyPolyline;
            PointCollection pc = new PointCollection();
            pc.Add(new Point(-50, 120));
            pc.Add(new Point(175, 130));
            pc.Add(new Point(200, 200));
            pc.Add(new Point(230, 140));

            p.Points = pc;
            return null;
        }

         public object Verify(object arg)
    {

            if (XamlTestHelper.Compare("PolylineNeg.bmp") && MyPolyline.Points[0].X == -50)
            {
                XamlTestHelper.LogStatus ("Pass: the first Point is negative");
            }
            else
            {
                XamlTestHelper.LogFail ("Fail: the first point is not negative");
            }


            return null;
        }


    }
}