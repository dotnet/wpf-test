using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Invalid test scenarios
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(2, "AccessKeys", "AccessKeyButtonBaseEventTest")]
    public class AccessKeyButtonBaseEventTest : AccessKeyTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "button", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "repeatbutton", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "togglebutton", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "checkbox", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.LeftToRight, "radiobutton", "Click", false, System.Windows.Input.Key.Z)]

        // FlowDirection.RightToLeft
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "button", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "repeatbutton", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "togglebutton", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "checkbox", "Click", false, System.Windows.Input.Key.Z)]
        [Variation("AccessKeysBVT.xaml", FlowDirection.RightToLeft, "radiobutton", "Click", false, System.Windows.Input.Key.Z)]
        public AccessKeyButtonBaseEventTest(string fileName, FlowDirection rootElementFlowDirection, string sourceName, string eventName, bool isEventFired, Key key)
            : base(fileName, rootElementFlowDirection, sourceName, eventName, isEventFired, key)
        {
        }
    }
}


