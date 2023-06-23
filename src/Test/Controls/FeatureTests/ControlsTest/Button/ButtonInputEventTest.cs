using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ButtonInputEventTest
    /// </summary>
    public class ButtonInputEventTest : XamlTest
    {
        public ButtonInputEventTest(Dictionary<string, string> variation, string testInfo)
            : base(variation["XamlFileName"])
        {
            this.variation = variation;
            this.testInfo = testInfo;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        private Dictionary<string, string> variation;
        private string testInfo;
        private ButtonBase source;

        private TestResult Setup()
        {
            Status("Setup");
            LogComment(testInfo);
            string controlName = variation["ControlName"];

            source = (ButtonBase)RootElement.FindName(controlName);
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source element " + controlName + " is null.");
            }

            System.Windows.Window.GetWindow(source).FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), variation["FlowDirection"]);

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            string eventName = variation["EventName"];
            bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);
            RoutedEventArgs routedEventArgs = null;

            switch (eventName)
            {
                case "Click":
                    routedEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
                    break;
                case "Checked":
                    routedEventArgs = new RoutedEventArgs(ToggleButton.CheckedEvent);
                    break;
                case "Unchecked":
                    routedEventArgs = new RoutedEventArgs(ToggleButton.UncheckedEvent);
                    break;
                case "Indeterminate":
                    routedEventArgs = new RoutedEventArgs(ToggleButton.IndeterminateEvent);
                    break;
                default:
                    throw new NotSupportedException("Fail: unknown event " + eventName);
            }
            
            using (ButtonValidator validator = ButtonValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate<RoutedEventArgs>(source, eventName, routedEventArgs, shouldEventFire, (Microsoft.Test.Input.Key)Enum.Parse(typeof(Microsoft.Test.Input.Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate<RoutedEventArgs>(source, eventName, routedEventArgs, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                        break;
                    default:
                        throw new NotSupportedException("Fail: unsupported input type " + variation["InputType"]);
                }
            }

            return TestResult.Pass;
        }
    }
}


