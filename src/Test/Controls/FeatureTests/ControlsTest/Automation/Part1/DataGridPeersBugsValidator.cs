using System;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridPeersBugsValidator validating regression bugs
    /// </summary>
    public class DataGridPeersBugsValidator : AutomationValidator
    {
        public DataGridPeersBugsValidator(int bugNumber, string bugDescription, string args, string targetName)
            : base(args, targetName)
        {
            this.bugNumber = bugNumber;
            this.bugDescription = bugDescription;
        }

        private int bugNumber;
        private string bugDescription;

        public override void Run()
        {
            GlobalLog.LogStatus(bugDescription);
            switch (bugNumber)
            {
                case 1:
                    TestBug1();
                    break;
                case 2:
                    TestBug2();
                    break;
                default:
                    throw new TestValidationException("Fail: Unknown bug number.");
            }
        }

        // Ensure DataGridColumnHeader.Name shows name instead of index
        private void TestBug1()
        {
            AutomationElement target = targetElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridColumnHeader"));

            if (String.Compare(target.Current.Name, "ID") != 0)
            {
                throw new TestValidationException(String.Format("Fail: target.Current.Name is {0}.", target.Current.Name));
            }
        }

        // Ensure virtualized DataGridColumnHeader.ClassName throws ElementNotAvailableException because virtualized item is only for 
        // people to call VirtualizedItemPattern.Realize() to bring it into view.
        private void TestBug2()
        {
            bool isExceptonOccured = false;
            AutomationElement dataGridColumnHeadersPresenter = targetElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridColumnHeadersPresenter"));
            object pattern;

            if (dataGridColumnHeadersPresenter.TryGetCurrentPattern(ItemContainerPattern.Pattern, out pattern))
            {
                ItemContainerPattern icpPattern = pattern as ItemContainerPattern;
                AutomationElement datagridColHeader = null;

                // The fifth DataGridColumnHeader is virtualized.
                // Therefore, we use ItemContainerPattern.FindItemByProperty to find item one by one until the fifth one.
                for (int i = 0; i < 5; i++)
                {
                    datagridColHeader = icpPattern.FindItemByProperty(datagridColHeader, AutomationElementIdentifiers.ControlTypeProperty, ControlType.HeaderItem);
                    if (datagridColHeader == null)
                    {
                        throw new TestValidationException("Fail: DataGridColumHeader is null.");
                    }
                }

                try
                {
                    // Call virtualized DataGridColumnHeader.ClassName to cause ElementNotAvailableException
                    GlobalLog.LogEvidence(String.Format("datagridColHeader ClassName is {0}", datagridColHeader.Current.ClassName));
                }
                catch (ElementNotAvailableException e)
                {
                    GlobalLog.LogEvidence(String.Format("ElementNotAvailableException occurs: {0}", e.Message));
                    isExceptonOccured = true;
                }

                if (!isExceptonOccured)
                {
                    throw new TestValidationException("Fail: ElementNotAvailableException didn't occur.");
                }
            }
            else
            {
                throw new TestValidationException("Fail: Get ItemContainerPattern failed.");
            }
        }
    }
}
