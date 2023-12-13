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

    public partial class VisualBrushUpdateTest: Window
    {
        public VisualBrushUpdateTest()
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
            XamlTestHelper.AddStep(SetButtonsChecked);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot,"CheckBoxChecked.bmp");
            XamlTestHelper.AddStep(VerifyUpdatedVisualBrush);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object VerifyUpdatedVisualBrush(object arg)
        {
            if ( XamlTestHelper.Compare("CheckBoxCheckedMaster.bmp") )
            {
                XamlTestHelper.LogStatus ("Pass: they render correctly");
            }
            else
            {
                XamlTestHelper.LogFail ("Fail: incorrectly rendered");
            }
            return null;

        }

        public object SetButtonsChecked(object arg)
        {
            checkbox.IsChecked = true;
            radio.IsChecked = true;
            return null;
        }

    }
}