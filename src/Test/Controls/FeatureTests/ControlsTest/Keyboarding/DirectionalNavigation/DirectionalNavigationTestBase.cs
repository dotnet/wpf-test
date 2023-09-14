using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DirectionalNavigationTestBase
    /// </summary>
    public abstract class DirectionalNavigationTestBase : XamlTest
    {
        public DirectionalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, FocusNavigationDirection focusNavigationDirection)
            : base(fileName)
        {
            this.focusNavigationDirection = focusNavigationDirection;
            isFocusNavigationDirection = true;
            Initialize(rootElementFlowDirection, keyboardNavigationMode, from, to);
        }

        public DirectionalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, Key key)
            : base(fileName)
        {
            this.key = key;
            Initialize(rootElementFlowDirection, keyboardNavigationMode, from, to);
        }

        public DirectionalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode)
            : base(fileName)
        {
            Initialize(rootElementFlowDirection, keyboardNavigationMode);
        }

        private void Initialize(FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to)
        {
            this.rootElementFlowDirection = rootElementFlowDirection;
            this.keyboardNavigationMode = keyboardNavigationMode;
            fromName = from;
            toName = to;
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private void Initialize(FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode)
        {
            this.rootElementFlowDirection = rootElementFlowDirection;
            this.keyboardNavigationMode = keyboardNavigationMode;
            InitializeSteps += new TestStep(SetupBatchStep);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunBatch);
        }

        bool isFocusNavigationDirection = false;
        KeyboardNavigationMode keyboardNavigationMode;
        FlowDirection rootElementFlowDirection;
        string fromName;
        string toName;
        FocusNavigationDirection focusNavigationDirection;
        Key key;
        DependencyObject fromElement;
        DependencyObject toElement;

        private TestResult SetupBatchStep()
        {
            Status("Setup");
            SetupBatch();
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private void SetupBatch()
        {
            Panel panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new NullReferenceException("Fail: the panel is null.");
            }
            System.Windows.Window.GetWindow(panel).FlowDirection = rootElementFlowDirection;
            KeyboardNavigation.SetDirectionalNavigation(panel, keyboardNavigationMode);

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        private TestResult Setup()
        {
            Status("Setup");

            SetupBatch();

            fromElement = (DependencyObject)RootElement.FindName(fromName);
            if (fromElement == null)
            {
                throw new NullReferenceException("Fail: the fromElement is null.");
            }

            toElement = (DependencyObject)RootElement.FindName(toName);
            if (toElement == null)
            {
                throw new NullReferenceException("Fail: the toElement is null.");
            }

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            fromElement = null;
            toElement = null;
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            using (KeyboardNavigationValidator validator = KeyboardNavigationValidator.GetValidator)
            {
                if (isFocusNavigationDirection)
                {
                    validator.FocusNavigationDirectionNavigate(fromElement, toElement, focusNavigationDirection);
                }
                else
                {
                    validator.DirectionalNavigate(fromElement, toElement, key);
                }
            }

            return TestResult.Pass;
        }

        private TestResult RunBatch()
        {
            TestResult result = TestResult.Pass;
            using (KeyboardNavigationValidator validator = KeyboardNavigationValidator.GetValidator)
            {
                result = RunBatchCore(validator);
            }

            return result;
        }

        public virtual TestResult RunBatchCore(KeyboardNavigationValidator validator)
        {
            return TestResult.Pass;
        }
    }
}


