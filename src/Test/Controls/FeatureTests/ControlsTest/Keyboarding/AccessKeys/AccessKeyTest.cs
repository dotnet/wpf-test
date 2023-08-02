using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Invalid test scenarios
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(2, "AccessKeys", "AccessKeyInvalidTest")]
    public class AccessKeyInvalidTest : AccessKeyTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "label", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "button", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "repeatbutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "togglebutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "checkbox", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "radiobutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "tabitem", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "menuitem", false, System.Windows.Input.Key.Z)]

        // FlowDirection.RightToLeft
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "label", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "button", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "repeatbutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "togglebutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "checkbox", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "radiobutton", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "tabitem", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "menuitem", false, System.Windows.Input.Key.Z)]
        public AccessKeyInvalidTest(string fileName, FlowDirection rootElementFlowDirection, string sourceName, bool isAccessKeyPressed, Key key)
            : base(fileName, rootElementFlowDirection, sourceName, isAccessKeyPressed, key)
        {
        }
    }
}


