// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;  // Validation
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <area>UIBinding.Validation</area>

    /// <description>
    /// Used to test 
    /// 1. Validation.HasErrorProperty / Validation.GetHasError(...) 
    /// 2. The count of ValidationErrors
    /// 3. Validation.ValidationErrorsProperty / Validation.GetValidationErrors(...)  
    /// </description>
    /// <spec>
    /// <name>Validation</name>
    /// </spec>
    /// </summary>
    public class ValidationStaticVerifier
    {
        #region Properties
        private FrameworkElement _element;

        public FrameworkElement Element
        {
            get { return _element; }
            set { _element = value; }
        }

        private string _step;

        public string Step
        {
            get { return _step; }
            set { _step = value; }
        }
        #endregion

        public ValidationStaticVerifier(FrameworkElement element)
        {
            Element = element;
        }

        public bool CheckHasError(bool expectedHasError)
        {
            GlobalLog.LogStatus("CheckHasError");

            bool hasError1 = (bool)_element.GetValue(Validation.HasErrorProperty);
            bool hasError2 = (bool)Validation.GetHasError(_element);

            if (hasError1 != hasError2)
            {
                GlobalLog.LogStatus("Fail - element.GetValue(Validation.HasErrorProperty):" + hasError1 +
                    " Validation.GetHasError(dObject):" + hasError2 + " - they should be the same - " +
                    Step);
                return false;
            }

            if (hasError1 != expectedHasError)
            {
                GlobalLog.LogStatus("Fail - Expected HasError at element level:" + expectedHasError + " Actual:" +
                    hasError1 + " - " + Step);
                return false;
            }

            return true;
        }

        public bool CheckNumErrors(int expectedCount)
        {
            GlobalLog.LogStatus("CheckNumErrors");
            
            // If there's a dispatcher, wait until everything possible has processed.
            if (Dispatcher.CurrentDispatcher != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                            DispatcherPriority.SystemIdle, (DispatcherOperationCallback)delegate(object unused)
                {
                    System.Threading.Thread.Sleep(50);
                    return null;
                },
                null);
            }
            System.Collections.Generic.IList<ValidationError> vec1 = _element.GetValue(Validation.ErrorsProperty) as System.Collections.Generic.IList<ValidationError>;
            if (vec1 == null)
            {
                GlobalLog.LogStatus("Fail - vec1 is null - " + Step);
                return false;
            }
            int count1 = vec1.Count;
            System.Collections.Generic.IList<ValidationError> vec2 = Validation.GetErrors(_element) as System.Collections.Generic.IList<ValidationError>;
            if (vec2 == null)
            {
                GlobalLog.LogStatus("Fail - vec2 is null - " + Step);
                return false;
            }
            int count2 = vec2.Count;

            if (count1 != count2)
            {
                GlobalLog.LogStatus("Fail - count1:" + count1 + " count2:" + count2 +
                    " - they should be the same - " + Step);
                return false;
            }
            if (count1 != expectedCount)
            {
                GlobalLog.LogStatus("Fail - Expected count:" + expectedCount + " Actual:" + count1 +
                    " - " + Step);
                return false;
            }

            return true;
        }

        public bool CheckValidationError(int veIndex, ValidationError expectedVe)
        {
            GlobalLog.LogStatus("CheckValidationError");

            System.Collections.Generic.IList<ValidationError> vec = _element.GetValue(Validation.ErrorsProperty) as System.Collections.Generic.IList<ValidationError>;
            ValidationError actualVe = vec[veIndex] as ValidationError;

            // validate BindingInError
            if (actualVe.BindingInError != expectedVe.BindingInError)
            {
                GlobalLog.LogStatus("Fail - ValidationError's BindingInError not as expected - " + Step);
                return false;
            }

            // don't validate ErrorContent - causes tests to fail in Japanese

            // validate exception - don't compare references because if the exception is thrown in the
            // setter, I have no way to get to it to pass it to this method - it gets wrapped by the 
            // ValidationError before I can get to it
            // Do not test for Exception message, this causes issues when running in a localized OS.
            if (actualVe.Exception != null && expectedVe.Exception != null)
            {
                if ((actualVe.Exception).GetType() != (expectedVe.Exception).GetType())
                {
                    GlobalLog.LogStatus("Fail - ValidationError's Exception does not have expected type - " + Step);
                    return false;
                }
            }
            else
            {
                if (actualVe.Exception != null)
                {
                    GlobalLog.LogStatus("Fail - Actual Exception is null and expected isn't - " + Step);
                    return false;
                }
                if (expectedVe.Exception != null)
                {
                    GlobalLog.LogStatus("Fail - Expected Exception is null and expected isn't - " + Step);
                    return false;
                }
            }

            // validate rule in error
            if (actualVe.RuleInError.GetType() != expectedVe.RuleInError.GetType())
            {
                GlobalLog.LogStatus("Fail - ValidationError's RuleInError not as expected - " + Step);
                return false;
            }

            return true;
        }
    }
}

