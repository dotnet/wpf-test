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
using System.Windows.Controls;

//------------------------------------------------------------------
// Inject SD stamp into the assembly for each file
namespace List.Of.Sources
{
    public class StackPanelTest_xaml_cs
    {
        public static string version = "$Id: $ $Change: $";
    }
}

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class StackPanelTest : Window
    {

        public StackPanelTest()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "StackPanelTest1.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Change the size of the rectangles");
            Rect1.Width = 450;
            Rect1.Height = 550;
            Rect2.Width = 450;
            Rect2.Height = 550;
            Rect3.Width = 450;
            Rect3.Height = 550;
            return null;
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.Compare("StackPanelTest.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: the layout is correct");
            }
            else
            {
                XamlTestHelper.LogStatus("Fail: the new layout is not correct");
            }

            return null;
        }

    }
}