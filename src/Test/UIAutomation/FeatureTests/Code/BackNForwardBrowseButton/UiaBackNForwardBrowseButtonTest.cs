// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Test.UIAutomaion;
using System.Windows.Input;
using Microsoft.Test.Loaders;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ScrollBar AutomationId
    /// </summary>
    [Serializable]
    public class UiaBackNForwardBrowseButtonTest : UiaDistributedTestcase
    {
        public UiaBackNForwardBrowseButtonTest()
        {
            TestLog.Current.Result = TestResult.Pass;
            int IEVersion = ApplicationDeploymentHelper.GetIEVersion();
            if (IEVersion == 6)
            {
                AddStep(new UiaDistributedStep(AssignNamesToBackAndForwardButtons), UiaDistributedStepTarget.UiElement);
                AddStep(new UiaDistributedStep(TestBackButton), UiaDistributedStepTarget.AutomationElement);
                AddStep(new UiaDistributedStep(TestForwardButton), UiaDistributedStepTarget.AutomationElement);
            }
            else
            {
                AddStep(new UiaDistributedStep(NoneIE6), UiaDistributedStepTarget.UiElement);
            }
        }

        private void NoneIE6(object target)
        {
            TestLog.Current.LogStatus("This test only runs on IE6.");
        }

        private void AssignNamesToBackAndForwardButtons(object target)
        {
            Visual window = ((NavigationWindow)Application.Current.MainWindow);
            Button forward = null;
            Button back = null;
            forward = FindButton(window, NavigationCommands.BrowseForward) as Button;
            if (forward != null)
            {
                forward.Name = "browseForward";
            }
            else
            {
                TestLog.Current.LogStatus("UIElement: Could not find forward Button.");
                TestLog.Current.Result = TestResult.Fail;
            }

            back = FindButton(window, NavigationCommands.BrowseBack) as Button;
            if (back != null)
            {
                back.Name = "browseBack";
            }
            else
            {
                TestLog.Current.LogStatus("UIElement: Could not find back Button.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        private Visual FindButton(Visual element, RoutedUICommand command)
        {
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                // check the visual
                if (visual is Button && ((Button)visual).Command == command)
                {
                    return visual;
                }
                Visual button = FindButton(visual, command);
                if (button != null)
                    return button;
            }
            return null;
        }

        private void TestBackButton(object target)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogStatus("TestBackButton");
            AutomationElement element = (AutomationElement)target;
            AutomationElement topWindowElement = TreeWalker.ControlViewWalker.GetParent(element);
            //get the AutomationElement
            AutomationElement clientElement = topWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "browseBack"));
            if (clientElement != null)
            {
                TestLog.Current.LogStatus("Found browseBack Button.");
                if (clientElement.Current.IsEnabled == true)
                {
                    TestLog.Current.LogStatus("The Back Button is Enabled");
                }
                else
                {
                    TestLog.Current.LogStatus("The Back Button is not Enabled");
                }
                InvokePattern ip = clientElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                ip.Invoke();

                //Wait for events. 
                Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
            }
            else
            {
                TestLog.Current.LogStatus("AutomationElement: Could not find back Button.");
                TestLog.Current.Result = TestResult.Fail;
            }

            clientElement = null;
            clientElement = topWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "defaultpage"));
            if (clientElement != null)
            {
                TestLog.Current.LogStatus("*** Back Button works.");
            }
            else
            {
                TestLog.Current.LogStatus("AutomationElement: Back Button Navigate failed.");
                TestLog.Current.Result = TestResult.Fail;
            }

        }

        private void TestForwardButton(object target)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogStatus("TestForwardButton");
            AutomationElement element = (AutomationElement)target;
            AutomationElement topWindowElement = TreeWalker.ControlViewWalker.GetParent(element);
            //get the AutomationElement
            AutomationElement clientElement = topWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "browseForward"));
            if (clientElement != null)
            {
                TestLog.Current.LogStatus("*** Found browseForward Button.");
                if (clientElement.Current.IsEnabled == true)
                {
                    TestLog.Current.LogStatus("The Forward Button is Enabled");
                }
                else
                {
                    TestLog.Current.LogStatus("The Forward Button is not Enabled");
                }
                InvokePattern ip = clientElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                ip.Invoke();
                //Wait for events. 
                Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);

            }
            else
            {
                TestLog.Current.LogStatus("AutomationElement: Could not find forward Button.");
                TestLog.Current.Result = TestResult.Fail;
            }

            clientElement = null;
            clientElement = topWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "AssignName"));
            if (clientElement != null)
            {
                TestLog.Current.LogStatus("*** Forward Button works.");
            }
            else
            {
                TestLog.Current.LogStatus("AutomationElement: Forward Button Navigate failed.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
