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


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ListBoxItem Children
    /// </summary>
    [Serializable]
    public class UiaListBoxItemTest : UiaDistributedTestcase
    {
        [NonSerialized]
        private bool _buttonNotInListBoxItemWhenNotInView = false;

        public UiaListBoxItemTest()
        {
            AddStep(new UiaDistributedStep(VerifyButtonNotInListBoxItem), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(ScrollListBoxItemIntoView), UiaDistributedStepTarget.AutomationElement);
            AddStep(new UiaDistributedStep(VerifyButtonInListBoxItem), UiaDistributedStepTarget.AutomationElement);
        }

        private void VerifyButtonNotInListBoxItem(object target)
        {
            TestLog.Current.LogStatus("VerifyButtonNotInListBoxItem");
            AutomationElement element = (AutomationElement)target;
            // target is listboxitem
            // find button in listbox and expect not exception occur
            try
            {
                AutomationElement clientElement = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "button"));
                if (clientElement == null)
                {
                    TestLog.Current.LogEvidence("clientElement is null");
                    _buttonNotInListBoxItemWhenNotInView = true;
                }
                else
                {
                    TestLog.Current.LogEvidence("clientElement is not null");
                }
            }
            catch (Exception e)
            {
                TestLog.Current.LogEvidence("An Expected Exception has occured");
                TestLog.Current.LogEvidence(e.ToString());
                _buttonNotInListBoxItemWhenNotInView = true;
            }
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void ScrollListBoxItemIntoView(object target)
        {
            TestLog.Current.LogStatus("ScrollListBoxItemIntoView");
            AutomationElement element = (AutomationElement)target;
            // target is listboxitem
            ScrollItemPattern sip = element.GetCurrentPattern(ScrollItemPattern.Pattern) as ScrollItemPattern;
            sip.ScrollIntoView();
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 3));
        }

        private void VerifyButtonInListBoxItem(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            TestLog.Current.LogStatus("VerifyButtonInListBoxItem");
            AutomationElement element = (AutomationElement)target;

            if (_buttonNotInListBoxItemWhenNotInView)
            {
                // target is listboxitem
                // find button in listbox
                try
                {
                    AutomationElement clientElement = element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "button"));
                    if (clientElement == null)
                    {
                        TestLog.Current.LogEvidence("clientElement is null");
                    }
                    else
                    {
                        TestLog.Current.LogEvidence("clientElement is not null");
                    }
                }
                catch (Exception e)
                {
                    TestLog.Current.LogEvidence("An Expected Exception has occured");
                    TestLog.Current.LogEvidence(e.ToString());
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
