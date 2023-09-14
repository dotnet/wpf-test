using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "AccessKeys", "AccessKeyBVT", Keywords = "Localization_Suite")]
    public class AccessKeyBVT : AccessKeyTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "label", true, System.Windows.Input.Key.L)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "button", true, System.Windows.Input.Key.B)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "repeatbutton", true, System.Windows.Input.Key.R)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "togglebutton", true, System.Windows.Input.Key.T)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "checkbox", true, System.Windows.Input.Key.C)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "radiobutton", true, System.Windows.Input.Key.A)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "tabitem", true, System.Windows.Input.Key.I)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "menuitem", true, System.Windows.Input.Key.F)]

        // FlowDirection.RightToLeft
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "label", true, System.Windows.Input.Key.L)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "button", true, System.Windows.Input.Key.B)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "repeatbutton", true, System.Windows.Input.Key.R)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "togglebutton", true, System.Windows.Input.Key.T)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "checkbox", true, System.Windows.Input.Key.C)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "radiobutton", true, System.Windows.Input.Key.A)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "tabitem", true, System.Windows.Input.Key.I)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "menuitem", true, System.Windows.Input.Key.F)]
        public AccessKeyBVT(string fileName, FlowDirection rootElementFlowDirection, string sourceName, bool isAccessKeyPressed, Key key)
            : base(fileName, rootElementFlowDirection, sourceName, isAccessKeyPressed, key)
        {
        }
    }
}


