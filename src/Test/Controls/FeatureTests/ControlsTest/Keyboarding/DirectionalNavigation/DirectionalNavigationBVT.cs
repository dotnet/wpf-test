using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// It covers key below only because only these keys will cause directional navigation
    /// Down, Up, Right, Left
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "KeyboardNavigation", "DirectionalNavigationBVT")]
    public class DirectionalNavigationBVT : DirectionalNavigationTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "topButton", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button3", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "bottomButton", "button3", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button3", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button3", "button1", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button2", DirectionalNavigationKey.Right)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button2", "button1", DirectionalNavigationKey.Left)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button1", "button1", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "topButton", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button3", "button3", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "bottomButton", "button3", DirectionalNavigationKey.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "button3", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "textbox", DirectionalNavigationKey.Left)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button3", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button2", "button1", DirectionalNavigationKey.Right)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "topButton", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button3", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "bottomButton", "button3", DirectionalNavigationKey.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "topButton", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "bottomButton", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button3", "bottomButton", DirectionalNavigationKey.Down)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "topButton", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "bottomButton", "button3", DirectionalNavigationKey.Up)]

        // FlowDirection.RightToLeft
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "topButton", "button2", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button3", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "bottomButton", "textbox", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button3", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button3", "button1", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button2", DirectionalNavigationKey.Left)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button2", "button1", DirectionalNavigationKey.Right)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button1", "button1", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "topButton", "button2", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button3", "button3", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "bottomButton", "textbox", DirectionalNavigationKey.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "button3", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "textbox", DirectionalNavigationKey.Right)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button3", "button1", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button2", "button1", DirectionalNavigationKey.Left)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "topButton", "button2", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button3", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "bottomButton", "textbox", DirectionalNavigationKey.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "topButton", "bottomButton", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button1", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "bottomButton", "topButton", DirectionalNavigationKey.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button3", "bottomButton", DirectionalNavigationKey.Down)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "topButton", "button2", DirectionalNavigationKey.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "bottomButton", "textbox", DirectionalNavigationKey.Up)]
        public DirectionalNavigationBVT(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, DirectionalNavigationKey key)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode, from, to, (Key)Enum.Parse(typeof(Key), key.ToString(), true))
        {
        }
    }
}


