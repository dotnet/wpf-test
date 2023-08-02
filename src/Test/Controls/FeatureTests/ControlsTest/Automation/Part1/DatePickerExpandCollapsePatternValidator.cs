using System;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DatePickerExpandCollapsePatternValidator
    /// </summary>
    public class DatePickerExpandCollapsePatternValidator : AutomationValidator
    {
        public DatePickerExpandCollapsePatternValidator(string args, string targetName)
            : base(args, targetName)
        {
        }

        public override void Run()
        {
            object pattern;

            if (targetElement.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
            {
                ExpandCollapsePattern expandCollapsePattern = pattern as ExpandCollapsePattern;

                // Test Expand
                expandCollapsePattern.Expand();

                AutomationElement resultElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "result"));

                // Validate the Calendar is opened
                if (!resultElement.Current.Name.Contains("Opened"))
                {
                    throw new TestValidationException("Fail: resultElement doesn't contain Opened.");
                }

                // Test Collapse
                expandCollapsePattern.Collapse();

                resultElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "result"));

                // Validate the Calendar is closed
                if (!resultElement.Current.Name.Contains("Closed"))
                {
                    throw new TestValidationException("Fail: resultElement doesn't contain Closed.");
                }
            }
            else
            {
                throw new TestValidationException("Fail: TryGetCurrentPattern fail.");
            }
        }
    }
}
