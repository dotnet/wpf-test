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

    public partial class TwoDContent : Window
    {
        public TwoDContent()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("2D content is done rendering");
            XamlTestHelper.LogStatus("Time to shut down the Window");
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }
    }
}