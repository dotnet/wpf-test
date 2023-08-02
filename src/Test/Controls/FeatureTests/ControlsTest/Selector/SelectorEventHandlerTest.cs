using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// SelectorEventHandlerTest
    /// </summary>
    public class SelectorEventHandlerTest : SelectorEventTestBase
    {
        public SelectorEventHandlerTest(Dictionary<string, string> variation, string testInfo)
            : base(variation, testInfo)
        {
        }

        public override TestResult RunTest()
        {
            using (SelectorValidator validator = SelectorValidator.GetValidator)
            {
                int fromIndex = Convert.ToInt32(variation["FromIndex"]);
                int attachEventIndex = Convert.ToInt32(variation["AttachEventIndex"]);
                string eventName = variation["EventName"];
                bool shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);

                switch (variation["InputType"])
                {
                    case "Keyboard":
                        validator.Validate(source, fromIndex, attachEventIndex, eventName, shouldEventFire, (Key)Enum.Parse(typeof(Key), variation["Key"]));
                        break;
                    case "Mouse":
                        validator.Validate(source, fromIndex, Convert.ToInt32(variation["ToIndex"]), attachEventIndex, eventName, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                        break;
                }
            }

            return TestResult.Pass;
        }
    }
}


