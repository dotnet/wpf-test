using System;
using System.Windows.Automation;
using System.Windows.Forms;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// CalendarRegressionTest3Validator
    /// Ensure no NullReferenceException occurs when walking tree with control's visibility is set to collapsed
    /// </summary>
    public class CalendarRegressionTest3Validator : AutomationValidator
    {
        public CalendarRegressionTest3Validator(string args, string targetElementName)
            : base(args, targetElementName)
        {
        }

        public override void Run()
        {
            try
            {
                WalkEnabledElements(windowElement);
            }
            catch (NullReferenceException e)
            {
                GlobalLog.LogStatus("### NullReferenceException occurs " + e.Message);
                throw new TestValidationException("NullReferenceException occurs: " + e.Message);
            }
        }

        private void WalkEnabledElements(AutomationElement rootElement)
        {
            Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            TreeWalker walker = new TreeWalker(new AndCondition(condition1, condition2));
            AutomationElement elementNode = walker.GetFirstChild(rootElement);

            while (elementNode != null)
            {
                WalkEnabledElements(elementNode);
                elementNode = walker.GetNextSibling(elementNode);
            }
        }
    }
}
