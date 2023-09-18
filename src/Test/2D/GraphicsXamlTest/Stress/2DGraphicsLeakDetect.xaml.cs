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
    public partial class GraphicsStress : Window
    {
        public GraphicsStress()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.LogStatus("Get action!");

            string action = XamlTestHelper.GetArgument("Action", XamlTestHelper.args);
            if (string.IsNullOrEmpty(action))
            {
                throw new System.ApplicationException("GraphicsStress needs Action parameter");
            }

            switch (action.ToUpper())
            {
                case "RESIZE":
                    StressResize();
                    break;
                case "TABBROWSING":
                    TabBrowsingAction();
                    break;
                case "MOVE":
                    StressMove();
                    break;
                default:
                    throw new System.ApplicationException("unsupported action");
            }

            XamlTestHelper.AddStep(LogResult);
            XamlTestHelper.Run();
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