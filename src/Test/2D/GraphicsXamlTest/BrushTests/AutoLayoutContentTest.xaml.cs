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

    public partial class AutoLayoutContentTest : Window
    {

        public AutoLayoutContentTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This test takes the snapshot of the window rendered by the xaml and 
        /// 1. Sets AutoLayoutContent to False and compares the snapshot with a master
        /// 2. Sets AutoLayoutContent to True and compares the snapshot with a master
        /// </summary>
        public void RunTest(object sender, System.EventArgs e)
        {

            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "AutoLayoutContentFalseTest.bmp");
            XamlTestHelper.AddStep(VerifyAutoLayoutContentFalse);
            XamlTestHelper.AddStep(SetAutoLayoutContentTrueTest);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "AutoLayoutContentTrueTest.bmp");
            XamlTestHelper.AddStep(VerifyAutoLayoutContentTrue);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object VerifyAutoLayoutContentFalse(object arg)
        {
            if (XamlTestHelper.Compare("AutoLayoutContentFalseTestMaster.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: they render correctly");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: incorrectly rendered");
            }
            return null;

        }

        public object SetAutoLayoutContentTrueTest(object arg)
        {
            myVisualBrush.AutoLayoutContent = true;
            return null;
        }

        public object VerifyAutoLayoutContentTrue(object arg)
        {
            if (XamlTestHelper.Compare("AutoLayoutContentTrueTestMaster.bmp"))
            {
                XamlTestHelper.LogStatus("Pass: they render correctly");
            }
            else
            {
                XamlTestHelper.LogFail("Fail: incorrectly rendered");
            }
            return null;
        }

    }
}