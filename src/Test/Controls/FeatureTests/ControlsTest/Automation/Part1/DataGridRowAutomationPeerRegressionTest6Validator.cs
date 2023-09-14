using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridRowAutomationPeerRegressionTest6Validator
    /// </summary>
    public class DataGridRowAutomationPeerRegressionTest6Validator : AutomationValidator
    {
        public DataGridRowAutomationPeerRegressionTest6Validator(string args, string targetName)
            : base(args, targetName)
        {
        }

        public override void Run()
        {
            // Ensure AutomationElement could find Button after retemplated.
            if (targetElement == null)
            {
                throw new TestValidationException(String.Format("Fail: could not find targetElement {0}.", targetElement.Current.AutomationId));
            }
        }
    }
}
