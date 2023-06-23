using System;
using System.Windows.Input;
using Microsoft.Test.Discovery;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Programmatically move focus
    /// Valid test scenarios
    /// It covers key below only because only these keys will cause logical navigation
    /// FocusNavigationDirection.Next, FocusNavigationDirection.Previous
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "KeyboardNavigation", "FocusNavigationDirectionLogicalNavigationBVT")]
    public class FocusNavigationDirectionLogicalNavigationBVT : LogicalNavigationTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button2", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button2", "button1", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        // FlowDirection.RightToLeft
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button2", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button2", "button1", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "bottomButton", "textbox", FocusNavigationDirection.Previous)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "button1", "topButton", FocusNavigationDirection.Previous)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "topButton", "button1", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "textbox", "bottomButton", FocusNavigationDirection.Next)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "bottomButton", "textbox", FocusNavigationDirection.Previous)]
        public FocusNavigationDirectionLogicalNavigationBVT(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, FocusNavigationDirection focusNavigationDirection)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode, from, to, focusNavigationDirection)
        {
        }
    }
}


