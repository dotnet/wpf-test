using System;
using System.Xml;
using System.Windows;
using System.Collections;
using System.Windows.Controls;

using Avalon.Test.ComponentModel.Actions;

using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(ListBox))]
    public class CleanUpVirtualizedItemEventTest : IUnitTest
    {
        public TestResult Perform(object testElement, XmlElement variation)
        {
            bool bResult;

            TestLog.Current.LogEvidence("Verify CleanUpVirtualizedItemEvent");

            ListBox listBox = testElement as ListBox;

            handler = new CleanUpVirtualizedItemEventHandler(OnCleanupVirtualizedItem);
            VirtualizingStackPanel.AddCleanUpVirtualizedItemHandler(listBox,handler);

            for (int i = 0; i < 5; i++)
            {
                ControlPressPageDownAction controlPressPageDownAction = new ControlPressPageDownAction();
                controlPressPageDownAction.Do(listBox);
            }

            QueueHelper.WaitTillQueueItemsProcessed();
            bResult = isCleanUpVirtualizedItemEventFired;
            
            isCleanUpVirtualizedItemEventFired = false;
            VirtualizingStackPanel.RemoveCleanUpVirtualizedItemHandler(listBox,handler);
            ControlPressHomeAction controlPressHomeAction = new ControlPressHomeAction();
            controlPressHomeAction.Do(listBox);
            for (int i = 0; i < 5; i++)
            {
                ControlPressPageDownAction controlPressPageDownAction = new ControlPressPageDownAction();
                controlPressPageDownAction.Do(listBox);
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            bResult = !isCleanUpVirtualizedItemEventFired;

            if (isCleanUpVirtualizedItemEventFired)
                TestLog.Current.LogEvidence("FAIL - CleanUpVirtualizedItemEvent fired after removing event handler for CleanUpVirtualizedItemEvent");

            if (bResult)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }

        }

        public void OnCleanupVirtualizedItem(object sender, CleanUpVirtualizedItemEventArgs args)
        {
            isCleanUpVirtualizedItemEventFired = true;
            TestLog.Current.LogStatus("OnCleanupVirtualizedItem");
            TestLog.Current.LogStatus("Value: " + args.Value.GetType().ToString());
            TestLog.Current.LogStatus("UIElement: " + args.UIElement.GetType().ToString());
            TestLog.Current.LogStatus("Cancel: " + args.Cancel.GetType().ToString());
        }

        private bool isCleanUpVirtualizedItemEventFired = false;
        private CleanUpVirtualizedItemEventHandler handler;

    }
}


