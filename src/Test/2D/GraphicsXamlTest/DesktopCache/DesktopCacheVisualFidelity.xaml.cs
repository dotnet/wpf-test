// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Same as the existing shape hit test, but this one has a cache on the topmost element.
    /// This test case requires version 4.0
    /// </summary>

    public partial class DesktopCacheVisualFidelity : Window
    {
        public DesktopCacheVisualFidelity()
        {
            InitializeComponent();
            DpiScalingHelper.ScaleWindowToFixedDpi(this, 96.0f, 96.0f);
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "DesktopCacheVisualFidelityCapture.bmp");
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object Verify(object arg)
        {
            if (XamlTestHelper.CompareWithoutDPI("DesktopCacheVisualFidelity.bmp"))
            {
                XamlTestHelper.LogStatus(" *** Good:  cached image was rendered correctly");
            }
            else
            {
                XamlTestHelper.LogFail(" *** Bad: cached image was rendered different than expected");
            }
            return null;
        }
    }
}
