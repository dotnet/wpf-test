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
    public partial class BadParameterTests : Window
    {
        public BadParameterTests()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            GeometryFactory();
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        private void GeometryFactory()
        {
            string geometry = XamlTestHelper.GetArgument("Geometry", XamlTestHelper.args);
            if (geometry == null)
            {
                throw new System.ApplicationException("BadParameterTests need Geometry argument");
            }

            switch (geometry)
            {
                case "Line":
                    XamlTestHelper.LogStatus("Create LineGeometry with all sort of bad parameters");
                    CreateLineGeometry();
                    break;
                case "Rectangle":
                    XamlTestHelper.LogStatus("Create RectangleGeometry with all sort of bad parameters");
                    CreateRectangleGeometry();
                    break;
                case "Ellipse":
                    XamlTestHelper.LogStatus("Create EllipseGeometry with all sort of bad parameters");
                    CreateEllipseGeometry();
                    break;
                case "Path":
                    XamlTestHelper.LogStatus("Create PathGeometry with all sort of bad parameters");
                    CreatePathGeometry();
                    break;
                default:
                    throw new System.ApplicationException("Unknown geometry type");
            }

        }

        private object CleanUpCanvas(object arg)
        {
            XamlTestHelper.LogStatus("Remove all children under Canvas, and prepare for the next test");

            //Remove all children under Canvas 
            //Prepare for the next test.
            rootElement.Children.Clear();
            return null;
        }

        private void AddGeometryToPath(object[] gArray)
        {
            XamlTestHelper.LogStatus("Create Path, and assign the generated geometry to the Path.Data");
            XamlTestHelper.LogStatus("Render the Paths");
            for (int i = 0; i < gArray.Length; i++)
            {
                Path p = new Path();
                p.Stroke = Brushes.Black;
                p.StrokeThickness = 5.0;
                p.Data = gArray[i] as Geometry;
                rootElement.Children.Add(p);
            }
        }

        private object VerifyBlank(object arg)
        {
            if (XamlTestHelper.Compare("Blank.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: the Geometry with extreme value test passed");
            }
            else
            {
                XamlTestHelper.LogFail("the Geometry with extreme value test failed!");
            }

            return null;
        }


    }

}