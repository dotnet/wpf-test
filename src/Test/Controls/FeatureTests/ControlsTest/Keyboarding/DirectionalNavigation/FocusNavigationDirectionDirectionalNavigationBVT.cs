using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Programmatically move focus
    /// Valid test scenarios
    /// It covers FocusNavigationDirection scenarios
    /// Down, Up, Right, Left
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "KeyboardNavigation", "FocusNavigationDirectionDirectionalNavigationBVT")]
    public class FocusNavigationDirectionDirectionalNavigationBVT : DirectionalNavigationTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "topButton", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button3", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "bottomButton", "button3", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button3", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button3", "button1", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button2", FocusNavigationDirection.Right)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button2", "button1", FocusNavigationDirection.Left)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button1", "button1", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "topButton", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button3", "button3", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "bottomButton", "button3", FocusNavigationDirection.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "button3", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "textbox", FocusNavigationDirection.Left)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button3", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button2", "button1", FocusNavigationDirection.Right)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "topButton", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button3", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "bottomButton", "button3", FocusNavigationDirection.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "topButton", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "bottomButton", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button3", "bottomButton", FocusNavigationDirection.Down)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "topButton", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "bottomButton", "button3", FocusNavigationDirection.Up)]

        // FlowDirection.RightToLeft
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "topButton", "button2", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button3", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "bottomButton", "textbox", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button3", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button3", "button1", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button2", FocusNavigationDirection.Left)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button2", "button1", FocusNavigationDirection.Right)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button1", "button1", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "topButton", "button2", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button3", "button3", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "bottomButton", "textbox", FocusNavigationDirection.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "button3", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "textbox", FocusNavigationDirection.Right)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button3", "button1", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button2", "button1", FocusNavigationDirection.Left)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "topButton", "button2", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button3", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "bottomButton", "textbox", FocusNavigationDirection.Up)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "topButton", "bottomButton", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button1", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "bottomButton", "topButton", FocusNavigationDirection.Up)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button3", "bottomButton", FocusNavigationDirection.Down)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "topButton", "button2", FocusNavigationDirection.Down)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "bottomButton", "textbox", FocusNavigationDirection.Up)]
        public FocusNavigationDirectionDirectionalNavigationBVT(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, FocusNavigationDirection focusNavigationDirection)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode, from, to, focusNavigationDirection)
        {
        }
    }
}


