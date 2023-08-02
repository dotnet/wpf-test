using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ItemReferenceValidator
    /// Ensure that ItemAutomationPeer updates its Item property correctly
    /// </summary>
    public class ItemReferenceValidator : AutomationValidator
    {
        AutomationElement btnReplace, btnGC, listbox;
        string _summary;

        public ItemReferenceValidator(string args, string targetElementName)
            : base(args, targetElementName)
        {
        }

        public string Summary { get { return _summary; } }

        public override void Run()
        {
            btnReplace = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnReplace"));
            btnGC = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnGC"));
            listbox = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "listbox"));

            // register for structure change and property change events
            Automation.AddStructureChangedEventHandler(windowElement, TreeScope.Descendants,
                    new StructureChangedEventHandler(OnStructureChanged));
            Automation.AddAutomationPropertyChangedEventHandler(windowElement, TreeScope.Descendants,
                    new AutomationPropertyChangedEventHandler(OnPropertyChanged),
                    AutomationElement.IsPasswordProperty);

            VerifyItemReference();
        }

        // When an ItemCollection is changed or refreshed, the ItemAutomationPeers
        // are re-used for items that still appear in the new collection.
        // Regression Test : In this sceenario the peer doesn't update its Item reference
        // in the case when the new item is a different object from the old item,
        // but compares equal in the sense of Object.Equals().
        // We test this by replacing an item with a different-but-equal item,
        // then running an aggressive GC that will collect the old item.  If the
        // peer holds the old (weak) reference, it will now be null.
        void VerifyItemReference()
        {
            // click the button that replaces the item
            ClickButton(btnReplace);

            // click the button that runs the aggressive GC
            ClickButton(btnGC);

            // find the name of the first item in the listbox
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement item = walker.GetFirstChild(listbox);
            string name = GetText(item);

            // the name should not be null/empty
            if (String.IsNullOrWhiteSpace(name))
            {
                TestResult = TestResult.Fail;
                _summary = "First item failed to update its reference";
            }
            else
            {
                TestResult = TestResult.Pass;
                _summary = String.Format("First item updated its reference to '{0}'", name);
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

