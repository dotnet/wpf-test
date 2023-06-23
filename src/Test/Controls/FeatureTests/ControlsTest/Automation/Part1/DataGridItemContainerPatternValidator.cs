using System;
using System.Windows.Automation;
using Microsoft.Test.Logging;
using System.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGridItemContainerPatternValidator
    /// </summary>
    public class DataGridItemContainerPatternValidator : AutomationValidator
    {
        public DataGridItemContainerPatternValidator(string args, string targetName, bool hasSelectedItem)
            : base(args, targetName)
        {
            this.hasSelectedItem = hasSelectedItem;

            // Attach InvokedEvent to ready element to listen when the test is done.
            AutomationElement readyElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "ready"));
            AutomationEventHandler OnInvokedEvent = null;
            OnInvokedEvent = delegate(object sender, AutomationEventArgs e)
            {
                Automation.RemoveAutomationEventHandler(InvokePatternIdentifiers.InvokedEvent, readyElement, OnInvokedEvent);
                RunTest();
            };
            Automation.AddAutomationEventHandler(InvokePatternIdentifiers.InvokedEvent, readyElement, TreeScope.Element, OnInvokedEvent);
        }

        private bool hasSelectedItem;
        private ManualResetEvent testingCompleted = new ManualResetEvent(false);

        private void SetupAutomationIdForEachCell()
        {
            object pattern;

            // Setup AutomationId for each cell.
            AutomationElement button = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "button"));

            InvokePattern invokePattern = null;
            if (button != null && button.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                invokePattern = pattern as InvokePattern;
                invokePattern.Invoke();
            }
        }

        private void TestItemContainerPatternForNonVirtualizedItem(string cellId, bool shouldOffscreen, object pattern)
        {
            string rowIndex = cellId.Split(',')[1];
            string[] cellIdCoordinates = cellId.Split(',');

            // We have to hard code here due to the limitation of setting automation Id on datagrid cells.
            // We test it on Vista to find out after BringIntoView to scroll down and ScrollToHome to scroll up, the top
            // item container y coordinate is 24.
            cellId = string.Format("{0},{1}", cellIdCoordinates[0], Convert.ToInt32(cellIdCoordinates[1]) + 24);

            ItemContainerPattern icpPattern = pattern as ItemContainerPattern;

            AutomationElement datagridRow = null;
            datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.AutomationIdProperty, rowIndex);
            if (datagridRow == null)
            {
                throw new TestValidationException("Fail: DataGridRow is null.");
            }

            object cellPattern = null;

            if (datagridRow.TryGetCurrentPattern(ItemContainerPattern.Pattern, out cellPattern))
            {
                ItemContainerPattern cellIcpPattern = cellPattern as ItemContainerPattern;

                if (!hasSelectedItem)
                {
                    AutomationElement selectedItem = null;
                    selectedItem = cellIcpPattern.FindItemByProperty(selectedItem, SelectionItemPatternIdentifiers.IsSelectedProperty, true);

                    if (hasSelectedItem)
                    {
                        if (selectedItem == null)
                        {
                            throw new TestValidationException("Fail: selectedRow is null on a selected rows datagrid.");
                        }
                    }
                    else
                    {
                        if (selectedItem != null)
                        {
                            throw new TestValidationException("Fail: selectedRow is not null on a no selected row datagrid.");
                        }
                    }
                }

                AutomationElement datagridItem = null;
                datagridItem = cellIcpPattern.FindItemByProperty(datagridItem, AutomationElementIdentifiers.AutomationIdProperty, cellId);

                if (datagridItem == null)
                {
                    throw new TestValidationException("Fail: DataGridCell is null.");
                }

                if (datagridItem.Current.AutomationId != cellId)
                {
                    throw new TestValidationException("Fail: DataGridCell AutomationId is not " + cellId);
                }

                bool isItemContainerPatternAvailable = (bool)datagridItem.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty);
                if (isItemContainerPatternAvailable != false)
                {
                    throw new TestValidationException("Fail: DataGridCell isItemContainerPatternAvailable is not false");
                }

                bool isVirtualizedItemPatternAvailable = (bool)datagridItem.GetCurrentPropertyValue(AutomationElement.IsVirtualizedItemPatternAvailableProperty);
                if (isVirtualizedItemPatternAvailable != false)
                {
                    throw new TestValidationException("Fail: DataGridCell isVirtualizedItemPatternAvailable is not false");
                }

                bool isOffscreen = (bool)datagridItem.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);

                if (isOffscreen != shouldOffscreen)
                {
                    throw new TestValidationException("Fail: DataGridCell isOffscreen is not " + shouldOffscreen);
                }

                // Ensure we can scroll non-virtualized that is outside of view into view.
                if (datagridItem.TryGetCurrentPattern(ScrollItemPattern.Pattern, out pattern))
                {
                    ScrollItemPattern viPattern = pattern as ScrollItemPattern;
                    if (viPattern == null)
                    {
                        throw new TestValidationException("Fail: ScrollItemPattern is null.");
                    }

                    viPattern.ScrollIntoView();
                }

                isOffscreen = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
                if (isOffscreen == true)
                {
                    throw new TestValidationException("Fail: DataGridRow isOffscreen is true.");
                }
            }
            else
            {
                throw new TestValidationException("Fail: DataGridRow.TryGetCurrentPattern fail.");
            }
        }

        private void TestItemContainerPatternForNonVirtualizedRow(string rowId, bool shouldItemContainerPatternAvailable, bool shouldOffscreen, object objPattern)
        {
            ItemContainerPattern icpPattern = objPattern as ItemContainerPattern;

            AutomationElement selectedRow = null;
            selectedRow = icpPattern.FindItemByProperty(selectedRow, SelectionItemPatternIdentifiers.IsSelectedProperty, true);
            if (hasSelectedItem)
            {
                if (selectedRow == null)
                {
                    throw new TestValidationException("Fail: selectedRow is null on a selected rows datagrid.");
                }
            }
            else
            {
                if (selectedRow != null)
                {
                    throw new TestValidationException("Fail: selectedRow is not null on a no selected row datagrid.");
                }
            }

            AutomationElement datagridRow = null;
            datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.AutomationIdProperty, rowId);
            if (datagridRow == null)
            {
                throw new TestValidationException("Fail: DataGridRow is null.");
            }

            if (datagridRow.Current.AutomationId != rowId)
            {
                throw new TestValidationException("Fail: DataGridRow AutomationId is not " + rowId);
            }

            bool isItemContainerPatternAvailable = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty);
            if (isItemContainerPatternAvailable != shouldItemContainerPatternAvailable)
            {
                throw new TestValidationException("Fail: DataGridRow isItemContainerPatternAvailable is not " + shouldItemContainerPatternAvailable);
            }

            bool isVirtualizedItemPatternAvailable = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsVirtualizedItemPatternAvailableProperty);
            if (isVirtualizedItemPatternAvailable != false)
            {
                throw new TestValidationException("Fail: DataGridRow isVirtualizedItemPatternAvailable is not false");
            }

            bool isOffscreen = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
            if (isOffscreen != shouldOffscreen)
            {
                throw new TestValidationException("Fail: DataGridRow isOffscreen is not " + shouldOffscreen);
            }
        }

        private void TestItemContainerPatternForVirtualizedRow(string rowId, bool shouldItemContainerPatternAvailable, bool shouldOffscreen, object objPattern)
        {
            ItemContainerPattern icpPattern = objPattern as ItemContainerPattern;
            AutomationElement datagridRow = null;

            for (int i = 0; i < Convert.ToInt32(rowId); i++)
            {
                datagridRow = icpPattern.FindItemByProperty(datagridRow, AutomationElementIdentifiers.ControlTypeProperty, ControlType.DataItem);
                if (datagridRow == null)
                {
                    throw new TestValidationException("Fail: DataGridRow is null.");
                }
            }

            bool isItemContainerPatternAvailable = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty);
            if (isItemContainerPatternAvailable != shouldItemContainerPatternAvailable)
            {
                throw new TestValidationException("Fail: DataGridRow isItemContainerPatternAvailable is not " + shouldItemContainerPatternAvailable);
            }

            bool isVirtualizedItemPatternAvailable = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsVirtualizedItemPatternAvailableProperty);
            if (isVirtualizedItemPatternAvailable != true)
            {
                throw new TestValidationException("Fail: DataGridRow isVirtualizedItemPatternAvailable is not true");
            }

            // Calling datagridRow.Current.AutomationId and datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty) should throw exception because 
            // only VirtualizedItemPattern.Realize method, AutomationElement.IsItemContainerPatternAvailableProperty and AutomationElement.IsVirtualizedItemPatternAvailableProperty 
            // work.
            // We should test the expected exception is throwed on code below.
            ValidateElementNotAvailableException(datagridRow,
            delegate()
            {
                string automationId = datagridRow.Current.AutomationId;
            });

            ValidateElementNotAvailableException(datagridRow,
            delegate()
            {
                bool isOffscreen = (bool)datagridRow.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
            });
        }

        private void TestItemContainerPatternForVirtualizedColumnHeader(string colId, object objPattern)
        {
            ItemContainerPattern icpPattern = objPattern as ItemContainerPattern;
            AutomationElement datagridColHeader = null;

            for (int i = 0; i < Convert.ToInt32(colId); i++)
            {
                datagridColHeader = icpPattern.FindItemByProperty(datagridColHeader, AutomationElementIdentifiers.ControlTypeProperty, ControlType.HeaderItem);
                if (datagridColHeader == null)
                {
                    throw new TestValidationException("Fail: DataGridColumHeader is null.");
                }
            }

            if (datagridColHeader.TryGetCurrentPattern(VirtualizedItemPattern.Pattern, out objPattern))
            {
                VirtualizedItemPattern viPattern = objPattern as VirtualizedItemPattern;
                viPattern.Realize();
            }

            bool isOffscreen = (bool)datagridColHeader.GetCurrentPropertyValue(AutomationElement.IsOffscreenProperty);
            if (isOffscreen == true)
            {
                throw new TestValidationException("Fail: DataGridColHeader isOffscreen is true.");
            }
        }

        private void TestItemContainerPatternColumHeader(AutomationElement target)
        {
            //This is very ad-hoc and not a well thought out test.
            AutomationElementCollection collection = target.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridColumnHeadersPresenter"));
            object pattern;

            GlobalLog.LogEvidence("test10");
            foreach (AutomationElement elem in collection)
            {
                if (elem.TryGetCurrentPattern(ItemContainerPattern.Pattern, out pattern))
                {
                    TestItemContainerPatternForVirtualizedColumnHeader("5", pattern);
                }
                else
                {
                    throw new TestValidationException("Fail: DataGrid.TryGetCurrentPattern fail on DataGridColumnHeadersPresenter.");
                }
            }
        }

        private void ValidateElementNotAvailableException(AutomationElement datagridRow, Action action)
        {
            string elementNotAvailableExceptionMessage = "Element does not exist or it is virtualized; use VirtualizedItem Pattern if it is supported";
            bool isExpectedExceptionOccured = false;

            try
            {
                action();
            }
            catch (ElementNotAvailableException e)
            {
                isExpectedExceptionOccured = true;
                if (e.Message.Split('.')[0] != elementNotAvailableExceptionMessage)
                {
                    throw new TestValidationException("Fail: exception message does not equal " + elementNotAvailableExceptionMessage);
                }
            }

            if (!isExpectedExceptionOccured)
            {
                throw new TestValidationException("Fail: ElementNotAvailableException did not occur.");
            }
        }

        public override void Run()
        {
            SetupAutomationIdForEachCell();

            // Wait until the testing is done.
            // Instead of spinning in a sleep-less loop 
            // (Totally chokes out other threads in the process) use a ManualResetEvent.
            testingCompleted.WaitOne();
        }

        private void RunTest()
        {
            object pattern;

            if (targetElement.TryGetCurrentPattern(ItemContainerPattern.Pattern, out pattern))
            {
                // Test it on win7
                // Rows 0 to 7 is in viewport. Estimate 15 non virtualized items.
                // Validate row 0, 7 that are in viewport and 9 that is not in viewport.
                // Test top and bottom within viewport non virtualized items.
                TestItemContainerPatternForNonVirtualizedRow("0", true, false, pattern);
                GlobalLog.LogEvidence("Test1");
                TestItemContainerPatternForNonVirtualizedItem("2,0", false, pattern);
                GlobalLog.LogEvidence("Test2");
                TestItemContainerPatternForNonVirtualizedRow("7", true, false, pattern);
                GlobalLog.LogEvidence("Test3");
                TestItemContainerPatternForNonVirtualizedItem("2,7", false, pattern);
                GlobalLog.LogEvidence("Test4");

                // Test a non virtualized item is outside of viewport.
                TestItemContainerPatternForNonVirtualizedRow("9", true, true, pattern);
                GlobalLog.LogEvidence("Test5");

                // Ensure cell is outside of x and y-coordinate.
                TestItemContainerPatternForNonVirtualizedItem("4,9", true, pattern);
                GlobalLog.LogEvidence("Test6");

                // Test top, middle, bottm outside of viewport virtualized items.
                // DataGrid cell won't work on virtualized items because we are unable to set automation Id
                // on virtualized items. Hence, we don't have TestItemContainerPatternForVirtualizedItem tests.
                TestItemContainerPatternForVirtualizedRow("20", true, true, pattern);
                GlobalLog.LogEvidence("Test7");
                TestItemContainerPatternForVirtualizedRow("30", true, true, pattern);
                GlobalLog.LogEvidence("Test8");
                TestItemContainerPatternForVirtualizedRow("40", true, true, pattern);
                GlobalLog.LogEvidence("Test9");
            }
            else
            {
                throw new TestValidationException("Fail: DataGrid.TryGetCurrentPattern fail.");
            }            
            TestItemContainerPatternColumHeader(targetElement);
            testingCompleted.Set();
        }
    }
}
