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

    public partial class Window2 : Window
    {
	public Window2()
	{
        	InitializeComponent();
	}
        public void RunTest(object sender, System.EventArgs e)
        {
            //Console.WriteLine("ContentRendered:  Start test");
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            b1.Background = System.Windows.Media.Brushes.Red;

            XamlTestHelper.AddStep(test);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "NewCapture.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        public object test(object arg)
        {
            XamlTestHelper.LogStatus("Setting the background to white");
            b1.Background = System.Windows.Media.Brushes.White;
            return null;
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.Compare("firsttest.bmp"))
            {
                XamlTestHelper.LogStatus( "Yeah, the background of the button is white");
            }
            else
            {
                XamlTestHelper.LogFail( "Damn, it fails");
            }
            return null;
        }

    }
}