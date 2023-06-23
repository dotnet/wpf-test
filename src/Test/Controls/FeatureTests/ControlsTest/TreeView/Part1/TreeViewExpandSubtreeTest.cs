using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    public class TreeViewExpandSubtreeTest : XamlTest
    {
        public TreeViewExpandSubtreeTest(Dictionary<string, string> variation, string testInfo)
            : base(variation["XamlFileName"])
        {
            this.variation = variation;
            this.testInfo = testInfo;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        private Dictionary<string, string> variation;
        private string testInfo;
        private TreeView targetElement; 
        private TreeViewItem topLevelContainer;

        private TestResult Setup()
        {
            Status("Setup");
            LogComment(testInfo);
            string controlName = variation["ControlName"];

            targetElement = (TreeView)RootElement.FindName(controlName);
            if (targetElement == null)
            {
                throw new ArgumentNullException("Fail: the TargetElement " + controlName + " is null.");
            }

            System.Windows.Window.GetWindow(targetElement).FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), variation["FlowDirection"]);

            ItemsControlModifier modifier = new ItemsControlModifier(targetElement);
            modifier.Modify(Convert.ToBoolean(variation["IsVirtualizing"]), Convert.ToBoolean(variation["IsItemVirtualizing"]), (VirtualizationMode)Enum.Parse(typeof(VirtualizationMode), variation["VirtualizationMode"]));

            topLevelContainer = TreeViewHelper.GetContainer(targetElement, 0);
            if (topLevelContainer == null)
            {
                throw new ArgumentNullException("Fail: the TopLevelContainer is null.");
            }

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        protected TestResult RunTest()
        {
            switch (variation["InputType"])
            {
                case "Keyboard":
                    MouseClickFirstTreeViewItem();
                    using (ExpandSubtreeValidatorBase validator = new KeyboardExpandSubtreeValidator(topLevelContainer, "Item"))
                    {
                        validator.Run();
                    }
                    break;
                case "Code":
                    using (ExpandSubtreeValidatorBase validator = new ProgrammaticallyExpandSubtreeValidator(topLevelContainer, "Item"))
                    {
                        validator.Run();
                    }
                    break;
                default:
                    throw new NotSupportedException("Fail: unsupported input type " + variation["InputType"]);

            }

            return TestResult.Pass;
        }

        protected void MouseClickFirstTreeViewItem()
        {
            Collection<ContentPresenter> contentPresenters = VisualTreeHelper.GetVisualChildren<ContentPresenter>(topLevelContainer);

            ContentPresenter contentPresenter = null;

            // Get the first TreeViewItem ContentPresenter
            foreach(ContentPresenter _contentPresenter in contentPresenters)
            {
                contentPresenter = _contentPresenter;
                break;
            }

            if (contentPresenter == null)
            {
                throw new TestValidationException("Fail: the first TreeViewItem ContentPresenter is null.");
            }

            InputHelper.MouseMoveToElementCenter(contentPresenter);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
        }
    }
}


