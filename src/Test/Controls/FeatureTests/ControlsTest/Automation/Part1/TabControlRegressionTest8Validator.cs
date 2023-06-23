using System;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TabControlRegressionTest8Validator
    /// Ensure TabItem contents are visible to automation, even after changing IsEnabled
    /// </summary>
    public class TabControlRegressionTest8Validator : AutomationValidator
    {
        AutomationElement welcomeElement, launchElement, checkBoxElement;

        public TabControlRegressionTest8Validator(string args, string targetElementName)
            : base(args, targetElementName)
        {
            TestResult = TestResult.Pass;
        }

        public override void Run()
        {
            welcomeElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "rbWelcome"));
            launchElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "rbLaunch"));
            checkBoxElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "checkBox"));

            VerifyTabControl();
            VerifyTreeView();
        }

        // DDVSO RegressionTest8 complains that the content of a TabItem is no longer
        // visible to automation after changing TabItem.IsEnabled (even changing
        // it back to 'true')
        void VerifyTabControl()
        {
            // find the TabControl, its first TabItem, and the button therein
            AutomationElement tabElement = windowElement.FindFirst(TreeScope.Descendants,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab));
            AutomationElementCollection tabItems = tabElement.FindAll(TreeScope.Descendants,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
            AutomationElement tabItem1 = tabItems[0];

            AutomationElementCollection buttons = tabItem1.FindAll(TreeScope.Descendants,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            if (buttons == null || buttons.Count == 0)
            {
                GlobalLog.LogStatus("Cannot find button within tabItem1 - initial load.");
                TestResult = TestResult.Fail;
            }

            // set IsEnabled=false, give it time to settle, then look for the button again
            ToggleElement(checkBoxElement);
            System.Threading.Thread.Sleep(1000);
            buttons = tabItem1.FindAll(TreeScope.Descendants,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            if (buttons == null || buttons.Count == 0)
            {
                GlobalLog.LogStatus("Cannot find button within tabItem1 - after IsEnabled=false.");
                TestResult = TestResult.Fail;
            }

            // set IsEnabled=true, give it time to settle, then look for the button again
            ToggleElement(checkBoxElement);
            System.Threading.Thread.Sleep(1000);
            buttons = tabItem1.FindAll(TreeScope.Descendants,
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            if (buttons == null || buttons.Count == 0)
            {
                GlobalLog.LogStatus("Cannot find button within tabItem1 - after IsEnabled=true.");
                TestResult = TestResult.Fail;
            }
        }

        // an early fix caused crashes in WebMatrix and other apps.
        // Test a comparable scenario to guard against regressions:
        //  1. TreeView with enough content to require virtualization
        //  2. Automation listening for both structure-change and property-change events
        //  3. Change IsEnabled in the TreeView.  This schedules a call to UpdateSubtree on a 2nd-level TVItem's peer.
        //  4. Before the call occurs, remove the TVItem from the tree (by changing TV.ItemSource or comparable operation)
        //  5. The bad fix changes parentage of the 2nd-level peers, leading to null-reference
        //      exception when 2nd-level UpdateSubtree calls GetNameCore.
        void VerifyTreeView()
        {
            // register for structure change and property change events
            Automation.AddStructureChangedEventHandler(windowElement, TreeScope.Descendants,
                    new StructureChangedEventHandler(OnStructureChanged));
            Automation.AddAutomationPropertyChangedEventHandler(windowElement, TreeScope.Descendants,
                    new AutomationPropertyChangedEventHandler(OnPropertyChanged),
                    AutomationElement.IsPasswordProperty);

            // move to Launch page
            SelectElement(launchElement);

            // find the TreeView
            AutomationElement treeViewElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "treeView"));

            // expand some of its nodes
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement tviElement = walker.GetFirstChild(treeViewElement);
            for (int i=0; i<3; ++i)
            {
                ExpandElement(tviElement);
                tviElement = walker.GetNextSibling(tviElement);
            }

            // disable the TreeView
            ToggleElement(checkBoxElement);

            // return to the Welcome page.   With the older fix, this crashes
            SelectElement(welcomeElement);

            // give the previous command time to crash
            System.Threading.Thread.Sleep(1000);
        }

        void OnStructureChanged(object sender, StructureChangedEventArgs e)
        {
            // no need to do anything - we just need elements to think someone is listening
        }

        void OnPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            // no need to do anything - we just need elements to think someone is listening
        }

        void SelectElement(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
            {
                SelectionItemPattern siPattern = pattern as SelectionItemPattern;
                siPattern.Select();
            }
        }

        void ExpandElement(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out pattern))
            {
                ExpandCollapsePattern ecPattern = pattern as ExpandCollapsePattern;
                ecPattern.Expand();
            }
        }

        void ToggleElement(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(TogglePattern.Pattern, out pattern))
            {
                TogglePattern togglePattern = pattern as TogglePattern;
                togglePattern.Toggle();
            }
        }
    }
}
