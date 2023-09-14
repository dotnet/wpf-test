using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Invalid scenarios
    /// It covers some of following key types: these keys won't on Default Button.
    /// Alphabetic, Alphanumeric
    /// </summary>
    [Test(2, "DefaultButton", "DefaultButtonTest")]
    public class DefaultButtonTest : DefaultButtonTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button3", "textbox", false, System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button3", "textbox", false, System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button1", "button2", false, System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button1", "button2", false, System.Windows.Input.Key.A)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button3", "textbox", false, System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button3", "textbox", false, System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button1", "button2", false, System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button1", "button2", false, System.Windows.Input.Key.A)]
        public DefaultButtonTest(string fileName, FlowDirection rootElementFlowDirection, DefaultButtonMode defaultButtonMode, string buttonName, string focusElementName, bool expectedIsClicked, Key key)
            : base(fileName, rootElementFlowDirection,defaultButtonMode, buttonName, focusElementName, expectedIsClicked, key)
        {
        }
    }
}


