// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Hosting;
using System.Windows;
using Microsoft.Test.UIAutomaion;
using System.Threading;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing FocusChangedEvent for Controls Below
    /// Button
    /// </summary>
    [Serializable]
    public class UiaFocusChangedEventTest : UiaDistributedTestcase
    {
        //private int eventCount = 0;
        [NonSerialized]
        private AutomationElement _element;
       // ManualResetEvent syncEvent = new ManualResetEvent(false);

        public UiaFocusChangedEventTest()
        {
            AddStep(new UiaDistributedStep(AttachHandler), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(SetUiElementFocus), UiaDistributedStepTarget.UiElement);
            AddStep(new UiaDistributedStep(SetAElementFocus), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(ChangeUiElementFocus), UiaDistributedStepTarget.UiElement);
            AddStep(new UiaDistributedStep(Validate), UiaDistributedStepTarget.AutomationElement);

            SharedState["EventCount"] = 0;
        }

        private void AttachHandler(object target)
        {
            TestLog.Current.LogStatus("AttachHandler");
            _element = (AutomationElement)target;
            Automation.AddAutomationFocusChangedEventHandler(new AutomationFocusChangedEventHandler(OnEvent));
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void SetUiElementFocus(object target)
        {
            //set focus to the element
            TestLog.Current.LogStatus("Setting focus to the element");
            UIElement element = (UIElement)target;
            element.Focus();
            ((Button)element).Content = "new Button";
            QueueHelper.WaitTillQueueItemsProcessed();
        }
        private void SetAElementFocus(object target)
        {
            //set focus to the element
            TestLog.Current.LogStatus("Setting focus to the element");
            AutomationElement element = (AutomationElement)target;
            element.SetFocus();
            QueueHelper.WaitTillQueueItemsProcessed();
        }


        private void ChangeUiElementFocus(object target)
        {
            //change focus on element
            TestLog.Current.LogStatus("changing focus on element");
            //UserInput.KeyPress("Tab");

            Button button = (Button)target;
            Canvas canvas = (Canvas)button.Parent;
            TextBox textBox = (TextBox)canvas.Children[0];
            textBox.Focus();
            string content = textBox.Text;
            TestLog.Current.LogStatus(content);
            QueueHelper.WaitTillQueueItemsProcessed();
            
            //Wait for events. 
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);

            if (SharedState["EventFired"] == null)
            {
                TestLog.Current.LogEvidence("EventFired is null");
                TestLog.Current.LogEvidence("EventCount is " + SharedState["EventCount"].ToString());
            }
        }

        private void OnEvent(object sender, AutomationFocusChangedEventArgs argument)
        {
            TestLog.Current.LogEvidence("*** The focuschanged event fired ***");
            if (((AutomationElement)sender).Current.AutomationId == "button")
            {
                TestLog.Current.LogEvidence("***Sender AutomationId is " + ((AutomationElement)sender).Current.AutomationId);
                SharedState["EventCount"] = ((int)SharedState["EventCount"]) + 1;
                //if (!Automation.Compare(element, (AutomationElement)sender))
                //{
                //    TestLog.Current.LogEvidence("The sender of the event was not the same as the automation element we attached the event to");
                //    TestLog.Current.Result = TestResult.Fail;
                //}
            }
            else
            {
                TestLog.Current.LogEvidence("***Sender AutomationId is " + ((AutomationElement)sender).Current.AutomationId);
            }
            SharedState["EventFired"] = true;
        }

        private void Validate(object target)
        {
            //Wait for the event to fire
            QueueHelper.WaitTillQueueItemsProcessed();
            //QueueHelper.WaitTillTimeout(new TimeSpan(0, 5, 0));

            if ((int)SharedState["EventCount"] == 1)
                TestLog.Current.Result = TestResult.Pass;
            else
            {
                TestLog.Current.LogEvidence("The focuschanged event was not raised the expected number of times");
                TestLog.Current.LogEvidence("Expected: 1");
                TestLog.Current.LogEvidence("Actual: " + SharedState["EventCount"]);
                TestLog.Current.Result = TestResult.Fail;
            }

            //remove the handler and reset the eventcount
            Automation.RemoveAutomationFocusChangedEventHandler(new AutomationFocusChangedEventHandler(OnEvent));
            SharedState["EventCount"] = 0;
        }
    }

}
