using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// It covers key below only because only these keys work on DefaultButton
    /// Enter, Escape
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "DefaultButton", "DefaultButtonBVT", Keywords = "Localization_Suite")]
    public class DefaultButtonBVT : DefaultButtonTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button3", "textbox", true, DefaultButtonKey.Enter, Disabled=true)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button3", "textbox", false, DefaultButtonKey.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button1", "button2", false, DefaultButtonKey.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsDefault, "button1", "button2", false, DefaultButtonKey.Escape)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button3", "textbox", false, DefaultButtonKey.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button3", "textbox", true, DefaultButtonKey.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button1", "button2", false, DefaultButtonKey.Enter)]
        //[Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, DefaultButtonMode.IsCancel, "button1", "button2", false, DefaultButtonKey.Escape)]

        // FlowDirection.RightToLeft 
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsDefault, "button3", "textbox", true, DefaultButtonKey.Enter, Disabled=true)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsDefault, "button3", "textbox", false, DefaultButtonKey.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsDefault, "button1", "button2", false, DefaultButtonKey.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsDefault, "button1", "button2", false, DefaultButtonKey.Escape)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsCancel, "button3", "textbox", false, DefaultButtonKey.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsCancel, "button3", "textbox", true, DefaultButtonKey.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsCancel, "button1", "button2", false, DefaultButtonKey.Enter)]
        //[Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, DefaultButtonMode.IsCancel, "button1", "button2", false, DefaultButtonKey.Escape)]
        public DefaultButtonBVT(string fileName, FlowDirection rootElementFlowDirection, DefaultButtonMode defaultButtonMode, string buttonName, string focusElementName, bool expectedIsClicked, DefaultButtonKey defaultButtonKey)
            : base(fileName, rootElementFlowDirection, defaultButtonMode, buttonName, focusElementName, expectedIsClicked, (Key)Enum.Parse(typeof(Key), defaultButtonKey.ToString(), true))
        {
        }
    }
}


