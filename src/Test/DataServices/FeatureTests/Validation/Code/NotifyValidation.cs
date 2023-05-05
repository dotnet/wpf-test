// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests the NotifyOnValidationError BindFlag. Makes sure the event handler added
    /// is called when the element becomes valid and when it becomes invalid.
    /// It also tests binding to count 
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(1, "Validation", "NotifyValidation")]
    public class NotifyValidation : XamlTest
    {
        private XmlDataProvider _dso;
        private TextBox _tbPrice1;
        private TextBlock _txtSource;
        private TextBlock _txtErrors;
        private ValidationError _errorIsClearedTrue;
        private ValidationError _errorIsClearedFalse;
        private ValidationStaticVerifier _staticVerifier;
        private ValidationNonStaticVerifier _nonStaticVerifier;
        private BindingExpression _bindingExpression;

        public NotifyValidation()
            : base(@"NotifyValidation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Notify);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _dso = RootElement.Resources["dso"] as XmlDataProvider;
            _tbPrice1 = (TextBox)Util.FindElement(RootElement, "tbPrice1");
            _txtSource = (TextBlock)Util.FindElement(RootElement, "txtSource");
            _txtErrors = (TextBlock)Util.FindElement(RootElement, "txtErrors");

            if (_dso == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }

            if (_tbPrice1 == null)
            {
                LogComment("Unable to reference tbPrice1 element.");
                return TestResult.Fail;
            }

            if (_txtSource == null)
            {
                LogComment("Unable to reference txtSource element.");
                return TestResult.Fail;
            }

            _staticVerifier = new ValidationStaticVerifier(_tbPrice1);
            _nonStaticVerifier = new ValidationNonStaticVerifier();

            return TestResult.Pass;
        }

        private TestResult Notify()
        {
            Status("Notify");
            _bindingExpression = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            Validation.AddErrorHandler(_tbPrice1, new EventHandler<ValidationErrorEventArgs>(OnValidationErrorIsClearedFalse));

            // set text box to an invalid value
            _tbPrice1.Text = "12";

            WaitForPriority(DispatcherPriority.SystemIdle);
            _bindingExpression.UpdateSource();

            WaitForPriority(DispatcherPriority.SystemIdle);

            // OnValidationError is now called and ValidationErrorEventAction.Added
            // Use a huge timeout so the test definitely will fail if the signal isnt seen.
            TestResult resultWaitForSignal1 = WaitForSignal("NotifyIsClearedFalse", TimeSpan.FromHours(1).Milliseconds);
            if (resultWaitForSignal1 != TestResult.Pass) { return resultWaitForSignal1; }

            if (_txtErrors.Text != "1")
            {
                LogComment("Fail - Bound error count should be 1, instead it is " + _txtErrors.Text);
                return TestResult.Fail;
            }

            Validation.RemoveErrorHandler(_tbPrice1, new EventHandler<ValidationErrorEventArgs>(OnValidationErrorIsClearedFalse));
            _tbPrice1.AddHandler(Validation.ErrorEvent, (EventHandler<ValidationErrorEventArgs>)new EventHandler<ValidationErrorEventArgs>(OnValidationErrorIsClearedTrue));

            // set text box to a valid value
            _tbPrice1.Text = "3";

            WaitForPriority(DispatcherPriority.SystemIdle);

            _bindingExpression.UpdateSource();

            WaitForPriority(DispatcherPriority.SystemIdle);

            // OnValidationError is now called and ValidationErrorEventAction.Removed
            TestResult resultWaitForSignal2 = WaitForSignal("NotifyIsClearedTrue", TimeSpan.FromHours(1).Milliseconds);
            if (resultWaitForSignal2 != TestResult.Pass) { return resultWaitForSignal2; }

            if (_txtErrors.Text != "0")
            {
                LogComment("Fail - Bound error count should be 0, instead it is " + _txtErrors.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // args.Action shows whether the text box became valid (Removed) or invalid (Added)
        // called when text box becomes invalid
        public void OnValidationErrorIsClearedFalse(Object sender, ValidationErrorEventArgs args)
        {
            Status("OnValidationErrorIsClearedFalse");
            // get the Validation error and Action from args
            _errorIsClearedFalse = args.Error;

            // element level
            _staticVerifier.Step = "OnValidationErrorIsClearedFalse - element";
            if (!_staticVerifier.CheckNumErrors(1))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
             
            if (!_staticVerifier.CheckHasError(true))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            if (!_staticVerifier.CheckValidationError(0, _errorIsClearedFalse))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            // property level - TextProperty
            _nonStaticVerifier.Step = "OnValidationErrorIsClearedFalse - property";
             
            if (!_nonStaticVerifier.CheckHasError(_bindingExpression, true))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }
            if (!_nonStaticVerifier.CheckValidationError(_bindingExpression, _errorIsClearedFalse))
            {
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }

            ValidationErrorEventAction action = args.Action;
            if (action == ValidationErrorEventAction.Removed)
            {
                LogComment("Fail - ValidationErrorEventArgs.Action == Removed, it should be == Added");
                Signal("NotifyIsClearedFalse", TestResult.Fail);
                return;
            }

            Signal("NotifyIsClearedFalse", TestResult.Pass);
            return;
        }

        // called when text box becomes valid
        public void OnValidationErrorIsClearedTrue(Object sender, ValidationErrorEventArgs args)
        {
            Status("OnValidationErrorIsClearedTrue");
            // get the Validation error that was cleared and Action from args
            _errorIsClearedTrue = args.Error;

            if (!CompareValidationErrors(_errorIsClearedFalse, _errorIsClearedTrue))
            {
                Signal("NotifyIsClearedTrue", TestResult.Fail);
                return;
            }

            // element level
            _staticVerifier.Step = "OnValidationErrorIsClearedTrue - element";
            if (!_staticVerifier.CheckNumErrors(0))
            {
                Signal("NotifyIsClearedTrue", TestResult.Fail);
                return;
            }
             
            if (!_staticVerifier.CheckHasError(false))
            {
                Signal("NotifyIsClearedTrue", TestResult.Fail);
                return;
            }
            // property level - TextProperty
            _nonStaticVerifier.Step = "OnValidationErrorIsClearedTrue - property";
            if (!_nonStaticVerifier.CheckHasError(_bindingExpression, false))
            {
                Signal("NotifyIsClearedTrue", TestResult.Fail);
                return;
            }

            ValidationErrorEventAction action = args.Action;
            if (action == ValidationErrorEventAction.Removed)
            {
                Signal("NotifyIsClearedTrue", TestResult.Pass);
                return;
            }
            else
            {
                LogComment("Fail - action == ValidationErrorEventAction.Added, expected == .Removed");
                Signal("NotifyIsClearedTrue", TestResult.Fail);
                return;
            }
        }

        private bool CompareValidationErrors(ValidationError v1, ValidationError v2)
        {
            Status("CompareValidationErrors");

            if (v1.BindingInError != v2.BindingInError)
            {
                LogComment("Fail - v1.BindingInError and v2.BindingInError are not the same");
                return false;
            }
            if (v1.ErrorContent != v2.ErrorContent)
            {
                LogComment("Fail - v1.ErrorContent and v2.ErrorContent are not the same");
                return false;
            }
            if (v1.Exception != v2.Exception)
            {
                LogComment("Fail - v1.Exception and v2.Exception are not the same");
                return false;
            }
            if (v1.RuleInError != v2.RuleInError)
            {
                LogComment("Fail - v1.RuleInError and v2.RuleInError are not the same");
                return false;
            }

            LogComment("CompareValidationErrors was successful");
            return true;
        }
    }
}

