using System;
using System.Windows.Input;
using Microsoft.Test.Discovery;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Valid test scenarios
    /// It covers key below only because only these keys will cause logical navigation
    /// Tab, ShiftTab, CtrlTab, CtrlShiftTab
    /// For Globalization and Localization test coverage, we test both LeftToRight and RightToLeft FlowDirection
    /// </summary>
    [Test(0, "KeyboardNavigation", "LogicalNavigationBVT")]
    public class LogicalNavigationBVT : LogicalNavigationTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button2", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button2", "button1", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "button2", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button2", "button1", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Continue, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Contained, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Local, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.None, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight, KeyboardNavigationMode.Once, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        // FlowDirection.RightToLeft
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button2", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button2", "button1", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "button2", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button2", "button1", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Continue, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Contained, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Cycle, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Local, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.None, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "button1", "topButton", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "topButton", "button1", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "textbox", "bottomButton", LogicalNavigationKey.Tab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "bottomButton", "textbox", LogicalNavigationKey.ShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "button1", "topButton", LogicalNavigationKey.CtrlShiftTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "topButton", "button1", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "textbox", "bottomButton", LogicalNavigationKey.CtrlTab)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.RightToLeft, KeyboardNavigationMode.Once, "bottomButton", "textbox", LogicalNavigationKey.CtrlShiftTab)]

        public LogicalNavigationBVT(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, LogicalNavigationKey key)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode, from, to, key)
        {
        }
    }
}


