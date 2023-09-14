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
    /// LogicalNavigationTestBase
    /// </summary>
    public abstract class LogicalNavigationTestBase : XamlTest
    {
        public LogicalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, Key key)
            : base(fileName)
        {
            this.key = key;
            Initialize(rootElementFlowDirection, keyboardNavigationMode, from, to);
        }

        public LogicalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, FocusNavigationDirection focusNavigationDirection)
            : base(fileName)
        {
            this.focusNavigationDirection = focusNavigationDirection;
            isFocusNavigationDirection = true;
            Initialize(rootElementFlowDirection, keyboardNavigationMode, from, to);
        }

        public LogicalNavigationTestBase(string fileName, FlowDirection rootElementFlowDirection, KeyboardNavigationMode keyboardNavigationMode, string from, string to, LogicalNavigationKey key)
            : base(fileName)
        {
            logicalNavigationKey = key;
            islogicalNavigationKey = true;
            Initialize(rootElementFlowDirection, keyboardNavigationMode, from, to);
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

        bool islogicalNavigationKey = false;
        bool isFocusNavigationDirection = false;
        KeyboardNavigationMode keyboardNavigationMode;
        FlowDirection rootElementFlowDirection;
        string fromName;
        string toName;
        FocusNavigationDirection focusNavigationDirection;
        LogicalNavigationKey logicalNavigationKey;
        Key key;
        DependencyObject fromElement;
        DependencyObject toElement;

        private TestResult Setup()
        {
            Status("Setup");

            Panel panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new NullReferenceException("Fail: the panel is null.");
            }
            System.Windows.Window.GetWindow(panel).FlowDirection = rootElementFlowDirection;
            KeyboardNavigation.SetDirectionalNavigation(panel, keyboardNavigationMode);

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

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
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
                if (islogicalNavigationKey)
                {
                    validator.LogicalNavigate(fromElement, toElement, logicalNavigationKey);
                }
                else if (isFocusNavigationDirection)
                {
                    validator.FocusNavigationDirectionNavigate(fromElement, toElement, focusNavigationDirection);
                }
                else
                {
                    validator.LogicalNavigate(fromElement, toElement, key);
                }
            }

            return TestResult.Pass;
        }
    }
}


