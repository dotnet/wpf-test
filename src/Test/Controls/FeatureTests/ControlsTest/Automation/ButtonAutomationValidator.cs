using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ButtonAutomationValidator
    /// </summary>
    public class ButtonAutomationValidator : AutomationValidator
    {
        public ButtonAutomationValidator(string args, string targetElementName)
            : base(args, targetElementName)
        {
        }

        public override void Run()
        {
            object pattern;
            InvokePattern invokePattern = null;
            if (targetElement != null && targetElement.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                invokePattern = pattern as InvokePattern;
                invokePattern.Invoke();
            }

            AutomationElement resultElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "result"));
            
            if (resultElement == null)
            {
                throw new TestValidationException("Fail: resultElement is null.");
            }

            if (!resultElement.Current.Name.Contains("Pass"))
            {
                throw new TestValidationException("Fail: resultElement doesn't contain Pass.");
            }
        }
    }
}
