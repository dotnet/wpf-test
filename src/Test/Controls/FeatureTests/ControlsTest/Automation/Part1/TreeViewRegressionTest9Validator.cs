using System;
using System.Collections.Generic;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// TreeViewRegressionTest9Validator
    /// Ensure TabItem contents are visible to automation, even after changing IsEnabled
    /// </summary>
    public class TreeViewRegressionTest9Validator : AutomationValidator
    {
        AutomationElement btnLoop, btnReset, cbDisable, cbTopDown, btnGo,
            cb1stRemove, cb1stAdd, cb2ndRemove, cb2ndAdd, errorList;
        ToggleState _lastToggleState;
        bool _seenToggleFromOn;
        string _summary;
        List<string> _errors = new List<string>();

        public TreeViewRegressionTest9Validator(string args, string targetElementName)
            : base(args, targetElementName)
        {
            TestResult = TestResult.Pass;
        }

        public string Summary { get { return _summary; } }
        public List<string> Errors { get { return _errors; } }

        public override void Run()
        {
            btnLoop = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnLoop"));
            btnReset = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnReset"));
            cbDisable = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cbDisable"));
            cbTopDown = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cbTopDown"));
            btnGo = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnGo"));
            cb1stRemove = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cb1stRemove"));
            cb1stAdd = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cb1stAdd"));
            cb2ndRemove = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cb2ndRemove"));
            cb2ndAdd = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "cb2ndAdd"));
            errorList = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "errorList"));

            // register for structure change and property change events
            Automation.AddStructureChangedEventHandler(windowElement, TreeScope.Descendants,
                    new StructureChangedEventHandler(OnStructureChanged));
            Automation.AddAutomationPropertyChangedEventHandler(windowElement, TreeScope.Descendants,
                    new AutomationPropertyChangedEventHandler(OnPropertyChanged),
                    AutomationElement.IsPasswordProperty);
            Automation.AddAutomationPropertyChangedEventHandler(cbDisable, TreeScope.Element,
                    new AutomationPropertyChangedEventHandler(OnDisableCheckboxChanged),
                    TogglePattern.ToggleStateProperty);


            VerifyTreeView();
        }

        // DDVSO RegressionTest9 complains about ElementNotAvailable exceptions while
        // manipulating a TreeView.  It turned out the problem arose when
        // automation was active and the app did the following steps (in one
        // button click-handler, so dispatcher and automation didn't have a
        // chance to react singly):
        //  1. Start with a top-level collection with one item A, with
        //      two sub-items B and C, and a fully-expanded TreeView.
        //  2. Disable the TreeView
        //  3. Clear A's subitem collection
        //  4. Add a new subitem to A's collection
        //  5. Clear the main collection
        //  6. Re-insert A into the main collection
        // Here we test variations on this, doing steps 3-6 in various different
        // orders.  There should be no exceptions.
        void VerifyTreeView()
        {
            // the loop ends when we see the Disable checkbox going from On to Off
            _lastToggleState = ToggleState.Off;
            _seenToggleFromOn = false;

            // click the button that loops through all variations
            ClickButton(btnLoop);

            // wait for the page to finish the loop
            while (!_seenToggleFromOn)
            {
                System.Threading.Thread.Sleep(1000);
            }

            // the error list contains one item for each error, plus a summary item
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement item = walker.GetLastChild(errorList);
            _summary = GetText(item);

            for (item = walker.GetPreviousSibling(item); item != null; item = walker.GetPreviousSibling(item))
            {
                _errors.Insert(0, GetText(item));
            }

            if (_errors.Count > 0)
            {
                TestResult = TestResult.Fail;
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

        void OnDisableCheckboxChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            // when the "Disable" checkbox changes from checked to unchecked, the
            // loop is done
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
    }
}

