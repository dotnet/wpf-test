using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Invalid test scenarios
    /// It covers some of following key types: these keys should not cause directionalnavigation to work
    /// Alphabetic, Alphanumeric, Modifiers, Navigation and typing mode, System commands
    /// Try not to use Alt key because Alt key lose from element focus. 
    /// </summary>
    [Test(2, "KeyboardNavigation", "LogicalNavigationTest")]
    public class LogicalNavigationTest : LogicalNavigationTestBase
    {
        // FlowDirection.LeftToRight
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Continue, "button1", "button1", System.Windows.Input.Key.LeftShift)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Contained, "button1", "button1", System.Windows.Input.Key.LeftShift)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Cycle, "button1", "button1", System.Windows.Input.Key.LeftShift)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Local, "button1", "button1", System.Windows.Input.Key.LeftShift)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.None, "button1", "button1", System.Windows.Input.Key.LeftShift)]

        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.A)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.D1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.F1)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.Add)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.Back)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.BrowserBack)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.Escape)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.Enter)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.PageDown)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.LeftCtrl)]
        [Variation("KeyboardNavigationBVT.xaml", FlowDirection.LeftToRight,  KeyboardNavigationMode.Once, "button1", "button1", System.Windows.Input.Key.LeftShift)]
        public LogicalNavigationTest(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, Key key)
            : base(fileName, rootElementFlowDirection, keyboardNavigationMode, from, to, key)
        {
        }
    }
}


