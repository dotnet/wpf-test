// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Windows;
using Microsoft.Test.UIAutomaion;
using System.Threading;
using Microsoft.Test.Input;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing FocusChangedEvent for Controls Below
    /// Button
    /// </summary>
    [Serializable]
    public class UiaMenuOpenedClosedEventTest : UiaDistributedTestcase
    {
        public UiaMenuOpenedClosedEventTest()
        {
            AddStep(new UiaDistributedStep(AttachMenuOpenedEventHandler), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(OpenMenu), UiaDistributedStepTarget.UiElement);
            AddStep(new UiaDistributedStep(ValidateMenuOpenedEvent), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(AttachMenuClosedEventHandler), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(CloseMenu), UiaDistributedStepTarget.UiElement);
            AddStep(new UiaDistributedStep(ValidateMenuClosedEvent), UiaDistributedStepTarget.AutomationElement);

            SharedState["EventCount"] = 0;
        }

        private void AttachMenuOpenedEventHandler(object target)
        {
            TestLog.Current.LogEvidence("*** Attach MenuOpenedEvent Handler ***");
            Automation.AddAutomationEventHandler(AutomationElementIdentifiers.MenuOpenedEvent, AutomationElement.RootElement, TreeScope.Subtree, new AutomationEventHandler(OnMenuOpenedEvent));
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void AttachMenuClosedEventHandler(object target)
        {
            TestLog.Current.LogEvidence("*** Attach MenuClosedEvent Handler ***");
            Automation.AddAutomationEventHandler(AutomationElementIdentifiers.MenuClosedEvent, AutomationElement.RootElement, TreeScope.Subtree, new AutomationEventHandler(OnMenuClosedEvent));
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void OpenMenu(object target)
        {
            TestLog.Current.LogStatus("*** Open Menu ***");
            UserInput.KeyPress("LeftAlt");
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress("Down");
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));

            Thread.Sleep(500);

            if (SharedState["MenuOpenedEventFired"] == null)
            {
                TestLog.Current.LogEvidence("MenuOpenedEventFired is null");
                TestLog.Current.LogEvidence("EventCount is " + SharedState["EventCount"].ToString());
            }
        }

        private void CloseMenu(object target)
        {
            TestLog.Current.LogStatus("*** Close Menu ***");
            UserInput.KeyPress("Escape");
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));
            UserInput.KeyPress("Escape");
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));

            Thread.Sleep(500);

            if (SharedState["MenuClosedEventFired"] == null)
            {
                TestLog.Current.LogEvidence("MenuClosedEventFired is null");
                TestLog.Current.LogEvidence("EventCount is " + SharedState["EventCount"].ToString());
            }
        }

        void OnMenuOpenedEvent(object obj, AutomationEventArgs ev)
        {
            TestLog.Current.LogEvidence("*** The MenuOpenedEvent fired ***");
            SharedState["EventCount"] = ((int)SharedState["EventCount"]) + 1;
            SharedState["MenuOpenedEventFired"] = true;
        }

        void OnMenuClosedEvent(object obj, AutomationEventArgs ev)
        {
            TestLog.Current.LogEvidence("*** The MenuClosedEvent fired ***");
            SharedState["EventCount"] = ((int)SharedState["EventCount"]) + 1;
            SharedState["MenuClosedEventFired"] = true;
        }

        private void ValidateMenuOpenedEvent(object target)
        {
            //Wait for the event to fire
            QueueHelper.WaitTillQueueItemsProcessed();

            if ((int)SharedState["EventCount"] == 1)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The MenuOpened event was not raised the expected number of times");
                TestLog.Current.LogEvidence("Expected: 1");
                TestLog.Current.LogEvidence("Actual: " + SharedState["EventCount"]);
                TestLog.Current.Result = TestResult.Fail;
            }

            //remove the handler and reset the eventcount
            SharedState["EventCount"] = 0;
        }
        private void ValidateMenuClosedEvent(object target)
        {
            //Wait for the event to fire
            QueueHelper.WaitTillQueueItemsProcessed();

            if ((int)SharedState["EventCount"] == 1)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The MenuClosed event was not raised the expected number of times");
                TestLog.Current.LogEvidence("Expected: 1");
                TestLog.Current.LogEvidence("Actual: " + SharedState["EventCount"]);
                TestLog.Current.Result = TestResult.Fail;
            }

            //remove the handler and reset the eventcount
            Automation.RemoveAutomationEventHandler(AutomationElementIdentifiers.MenuClosedEvent, AutomationElement.RootElement, new AutomationEventHandler(OnMenuClosedEvent));
            SharedState["EventCount"] = 0;
        }
    }

}
