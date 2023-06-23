using System;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarRegressionTest86Validator
    /// Call SetFocus method on Calendar AutomationElement to ensure InvalidOperationException doesn't occur
    /// </summary>
    public class CalendarRegressionTest86Validator : AutomationValidator
    {
        public CalendarRegressionTest86Validator(string args, string targetElementName)
            : base(args, targetElementName)
        {
        }

        public override void Run()
        {
            try
            {
                targetElement.SetFocus();
            }
            catch (InvalidOperationException e)
            {
                throw new TestValidationException("InvalidOperationException occurs: " +e.Message);
            }
        }
    }
}
