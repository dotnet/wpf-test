using System;
using System.Diagnostics;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// What for:
    ///     It supports end user developer scenario (wpf projects) testing like code behind, resource look up.
    ///     
    /// How it works:
    ///     The UIAutomation client test launches WPF test app, and the client uses AutomationElement to get result from the WPF test app.
    /// 
    /// What we need:
    ///     The wpf test app MUST have a Button named ‘result’ and button.Content is set start with 'Pass'. The test app MUST set test result start 
    ///     with 'Fail' on the 'result' element when wpf test fails and UIAutomation client side gets it and reports it as a test result.
    /// </summary>
    public class ControlsIntegrationValidator : IDisposable
    {
        public ControlsIntegrationValidator(string executableName, string args)
        {
            process = OutOfProcTestHelper.StartProcess(executableName, args, ref windowElement);

            // Attach InvokedEvent to result element to listen when the test is done.
            resultElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, resultElementName));
            AutomationEventHandler OnInvokedEvent = null;
            OnInvokedEvent = delegate(object sender, AutomationEventArgs e)
            {
                Automation.RemoveAutomationEventHandler(InvokePatternIdentifiers.InvokedEvent, resultElement, OnInvokedEvent);
                ValidateResult();
            };
            Automation.AddAutomationEventHandler(InvokePatternIdentifiers.InvokedEvent, resultElement, TreeScope.Element, OnInvokedEvent);

            AutomationElement runTestElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, runTestElementName));

            // Run WPF app test.
            object pattern;
            InvokePattern invokePattern = null;
            if (runTestElement != null && runTestElement.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
            {
                invokePattern = pattern as InvokePattern;
                invokePattern.Invoke();
            }
        }

        /// <summary>
        /// It communicates with the wpf test application.
        /// 1, We need to use it to monitor the result element is set or not to know when the WPF test app is done with testing 
        ///    because client side doesn’t know when it is done(it can be less 1 sec or more than 5 seconds). We set IntegrationScenarioValidator.IsValidated 
        ///    to true when testing is done, and then the client of IntegrationScenarioValidator can use IsValidated that is true signal to exit the 
        ///    wait for the WPF test app to finish testing.
        /// 2, It gets the test result from the WPF test app.   
        /// </summary>
        private void ValidateResult()
        {
            isValidated = true;

            if (resultElement.Current.Name.StartsWith(expectedTestResult))
            {
                testResult = true;
            }

            actualTestResult = resultElement.Current.Name;
        }

        public bool IsValidated
        {
            get { return isValidated; }
        }

        public bool TestResult
        {
            get { return testResult; }
        }

        public string TestResultMessage
        {
            get { return actualTestResult; }
        }

        private Process process;
        private AutomationElement windowElement;
        private AutomationElement resultElement;
        private bool isDisposed = false;
        private bool isValidated = false;
        private bool testResult = false;
        private string expectedTestResult = "Pass";
        private string runTestElementName = "runTest";
        private string resultElementName = "result";
        private string actualTestResult = "Waiting test result...";

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (!process.HasExited)
                    {
                        process.CloseMainWindow();
                    }
                }
                isDisposed = true;
            }
        }

        public void Cleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ControlsIntegrationValidator()
        {
            Dispose(false);
        }

        #endregion
    }
}
