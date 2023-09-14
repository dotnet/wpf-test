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
    /// RepeatButtonDelayIntervalTest
    /// </summary>
    public class RepeatButtonDelayIntervalTest : XamlTest
    {
        public RepeatButtonDelayIntervalTest(Dictionary<string, string> variation, string testInfo)
            : base(variation["XamlFileName"])
        {
            this.variation = variation;
            this.testInfo = testInfo;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        private Dictionary<string, string> variation;
        private string testInfo;
        private RepeatButton source;

        private TestResult Setup()
        {
            Status("Setup");
            LogComment(testInfo);
            string controlName = variation["ControlName"];

            source = (RepeatButton)RootElement.FindName(controlName);
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the sourceElement is null.");
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
            bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);

            using (RepeatButtonValidator validator = RepeatButtonValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate(source, shouldEventFire, (Key)Enum.Parse(typeof(Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate(source, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                        break;
                    default:
                        throw new NotSupportedException("Fail: unsupported input type " + variation["InputType"]);
                }
            }

            return TestResult.Pass;
        }
    }
}


