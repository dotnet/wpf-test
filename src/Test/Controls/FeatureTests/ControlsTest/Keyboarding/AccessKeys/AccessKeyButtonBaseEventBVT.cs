using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "AccessKeys", "AccessKeyButtonBaseEventBVT", Keywords = "Localization_Suite")]
    public class AccessKeyButtonBaseEventBVT : AccessKeyTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "button", "Click", true, System.Windows.Input.Key.B)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "repeatbutton", "Click", true, System.Windows.Input.Key.R)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "togglebutton", "Click", true, System.Windows.Input.Key.T)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "checkbox", "Click", true, System.Windows.Input.Key.C)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "radiobutton", "Click", true, System.Windows.Input.Key.A)]

        // FlowDirection.RightToLeft
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "button", "Click", true, System.Windows.Input.Key.B)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "repeatbutton", "Click", true, System.Windows.Input.Key.R)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "togglebutton", "Click", true, System.Windows.Input.Key.T)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "checkbox", "Click", true, System.Windows.Input.Key.C)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "radiobutton", "Click", true, System.Windows.Input.Key.A)]
        public AccessKeyButtonBaseEventBVT(string fileName, FlowDirection rootElementFlowDirection, string sourceName, string eventName, bool isEventFired, Key key)
            : base(fileName, rootElementFlowDirection, sourceName, eventName, isEventFired, key)
        {
        }
    }
}


