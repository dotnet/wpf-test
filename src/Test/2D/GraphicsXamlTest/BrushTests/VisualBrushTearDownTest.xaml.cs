// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class VisualBrushTearDownTest : Window
    {
        public VisualBrushTearDownTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This test updates the Visuals put inside a VisualBrush
        /// One directly and other through data binding and verifies the capture
        /// </summary>
        public void RunTest(object sender, System.EventArgs e)
        {

            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "CheckBoxRemoved.bmp");
            XamlTestHelper.AddStep(RemoveVisualBrushParent);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "CheckBoxRemoved.bmp");
            XamlTestHelper.AddStep(VerifyUpdatedScene);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object VerifyUpdatedScene(object arg)
        {
            if (XamlTestHelper.Compare("CheckBoxRemovedMaster.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: they render correctly");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: incorrectly rendered");
            }
            return null;

        }

        public object RemoveVisualBrushParent(object arg)
        {
            this.border.Child = null;
            return null;
        }

    }
}