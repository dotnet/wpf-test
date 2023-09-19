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
using System.Windows.Controls;

namespace Microsoft.Test.Graphics
{
    public partial class AnimatedSpace : Window
    {
        public AnimatedSpace()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.LogStatus("Animation is running!");
            //Wait 20 second for all the animations to finish
            XamlTestHelper.AddStep(PlaceHolder, null, 25000);
            XamlTestHelper.AddStep(LogResult);
            XamlTestHelper.Run();
        }

        public object PlaceHolder(object arg)
        {
            XamlTestHelper.LogStatus("Animation continues to run!");
            return null;
        }
        public object LogResult(object arg)
        {
            XamlTestHelper.LogStatus("Pass!");
            XamlTestHelper.LogStatus( "Test Passed");
            XamlTestHelper.Quit(null);
            return null;
        }
    }
}