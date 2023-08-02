using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ItemLeakValidator
    /// Ensure that data items don't leak due to automation
    /// </summary>
    public class ItemLeakValidator : AutomationValidator
    {
        AutomationElement btnTest, cbRunning, cbFailures, lbTestLog;
        ToggleState _lastToggleState;
        bool _seenToggleFromOn;
        string _summary;
        List<string> _errors = new List<string>();

        public ItemLeakValidator(string args, string targetElementName)
            : base(args, targetElementName)
        {
            TestResult = TestResult.Pass;
            _summary = "No leaks detected";
        }

        public string Summary { get { return _summary; } }
        public List<string> Errors { get { return _errors; } }

        public override void Run()
        {
            btnTest = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnTest"));
            cbRunning = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cbRunning"));
            cbFailures = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cbFailures"));
            lbTestLog = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "lbTestLog"));

            // register for structure change and property change events
            Automation.AddStructureChangedEventHandler(windowElement, TreeScope.Descendants,
                    new StructureChangedEventHandler(OnStructureChanged));
            Automation.AddAutomationPropertyChangedEventHandler(windowElement, TreeScope.Descendants,
                    new AutomationPropertyChangedEventHandler(OnPropertyChanged),
                    AutomationElement.IsPasswordProperty);

            Automation.AddAutomationPropertyChangedEventHandler(cbRunning, TreeScope.Element,
                    new AutomationPropertyChangedEventHandler(OnRunningChanged),
                    TogglePattern.ToggleStateProperty);


            VerifyItemLeak();
        }

        // Regression Test : In this scenario, memory leaks happens when removing items from
        // the underlying data model, when automation is active.  UIAutomation
        // keeps references to the COM proxies long after the items have left the
        // model, and the proxies keep references to the peers and eventually to
        // the items themselves.
        // We test this by running a "FullTest" in the associated page.  The page
        // has several different ItemsControls, and the full test removes items
        // from the collections for each of them, then checks whether a full GC
        // collects the removed items.  When the test is complete, we check the
        // results it announced.
        void VerifyItemLeak()
        {
            // the loop ends when we see the Running checkbox going from On to Off
            _lastToggleState = ToggleState.Off;
            _seenToggleFromOn = false;

            // click the button that runs the full test
            ClickButton(btnTest);

            // wait for the page to finish the test
            while (!_seenToggleFromOn)
            {
                System.Threading.Thread.Sleep(1000);
            }

            if (IsElementToggledOn(cbFailures))
            {
                TestResult = TestResult.Fail;

                StringBuilder sb = new StringBuilder();
                TreeWalker walker = TreeWalker.ContentViewWalker;
                AutomationElement item = walker.GetFirstChild(lbTestLog);
                while (item != null)
                {
                    sb.AppendLine(GetText(item));
                    item = walker.GetNextSibling(item);
                }
                _summary = sb.ToString();
            }
        }

        void OnStructureChanged(object sender, StructureChangedEventArgs e)
        {
            // no need to do anything - we just need elements to think someone is listening
        }

        void OnPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            // no need to do anything - we just need elements to think someone is listening
        }

        void OnRunningChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            // when the "Running" checkbox changes from checked to unchecked, the
            // test is done
            if (!_seenToggleFromOn)
            {
                ToggleState newToggleState = (ToggleState)e.NewValue;
                if (_lastToggleState == ToggleState.On && newToggleState != ToggleState.On)
                {
                    _seenToggleFromOn = true;
                }

                _lastToggleState = newToggleState;
            }
        }

        void ClickButton(AutomationElement element)
        {
            object pattern;
            if (element.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                InvokePattern invokePattern = pattern as InvokePattern;
                invokePattern.Invoke();
            }
        }

        string GetText(AutomationElement element)
        {
            return element.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
        }

        private bool IsElementToggledOn(AutomationElement element)
        {
            Object objPattern;
            TogglePattern togPattern;
            if (true == element.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
            {
                togPattern = objPattern as TogglePattern;
                return togPattern.Current.ToggleState == ToggleState.On;
            }
            return false;
        }
    }
}

