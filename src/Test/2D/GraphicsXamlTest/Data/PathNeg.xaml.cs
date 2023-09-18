// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define DEBUG  //Workaround - This allows Debug.Write to work.

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

//------------------------------------------------------------------
// Inject SD stamp into the assembly for each file
namespace List.Of.Sources
{
    public class PathNeg_xaml_cs
    {
        public static string version = "$Id: $ $Change: $";
    }
}

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class PathNeg : Window
    {

        public PathNeg()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "PathNeg1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Make the points negative");


            Path p = MyPath;
            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(100, 130);
            pf.Segments.Add(new LineSegment(new Point(-50, 70), true));
            pf.Segments.Add(new ArcSegment(new Point(200, 200), new Size(100, 100), 35, false, SweepDirection.Counterclockwise, true));
            pg.Figures.Add(pf);
            p.Data = pg;

            return null;
        }

        public object Verify(object arg)
        {

            if (XamlTestHelper.Compare("PathNeg.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: path in negative zone");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: path is not negative zone");
            }


            return null;
        }


    }
}



