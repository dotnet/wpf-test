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
    /// DefaultButtonTestBase
    /// </summary>
    public abstract class DefaultButtonTestBase : XamlTest
    {
        public DefaultButtonTestBase(string fileName, FlowDirection rootElementFlowDirection, DefaultButtonMode defaultButtonMode, string buttonName, string focusElementName, bool expectedIsClicked, Key key)
            : base(fileName)
        {
            this.rootElementFlowDirection = rootElementFlowDirection;
            this.defaultButtonMode = defaultButtonMode;
            this.buttonName = buttonName;
            this.focusElementName = focusElementName;
            this.expectedIsClicked = expectedIsClicked;
            this.key = key;
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        DefaultButtonMode defaultButtonMode;
        FlowDirection rootElementFlowDirection;
        string buttonName;
        string focusElementName;
        bool expectedIsClicked;
        Key key;
        Button button;
        FrameworkElement focusElement;

        private TestResult Setup()
        {
            Status("Setup");

            button = (Button)RootElement.FindName(buttonName);
            if (button == null)
            {
                throw new NullReferenceException("Fail: the button is null.");
            }
            System.Windows.Window.GetWindow(button).FlowDirection = rootElementFlowDirection;

            focusElement = (FrameworkElement)RootElement.FindName(focusElementName);
            if (focusElement == null)
            {
                throw new NullReferenceException("Fail: the focusElement is null.");
            }

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            button = null;
            focusElement = null;
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            using (DefaultButtonValidator validator = DefaultButtonValidator.GetValidator)
            {
                validator.Validate(defaultButtonMode, button, focusElement, expectedIsClicked, key);
            }

            return TestResult.Pass;
        }
    }
}


