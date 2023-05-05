// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Reflection;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This test throws an exception in the setter of the property
    /// and uses the UpdateSourceExceptionFilter callback to deal with that.
    /// </description>
    /// <relatedBugs>



    /// </relatedBugs>
    /// </summary>
    [Test(0, "Validation", "ExceptionSetter")]
    public class ExceptionSetter : XamlTest
    {
        private TextBox _textBox;
        private Player _player;
        private ValidationError _error;
        private ValidationNonStaticVerifier _nonStaticVerifier;
        private ValidationStaticVerifier _staticVerifier;

        public ExceptionSetter()
            : base(@"ExceptionSetter.xaml")
        {
            InitializeSteps += new TestStep(Setup);

            // ValidationError is added to the ValidationErrorCollection of element
            RunSteps += new TestStep(ReturnValidationError);
            // for coverage
            RunSteps += new TestStep(VerifyUpdateSourceExceptionFilter);
            // Nothing happens - exception not thrown, ValidationError not added to ValidationErrorCollection
            RunSteps += new TestStep(ReturnNull);
            // ValidationError is created and added to collection. 
            // Exception: the original exception 
            // ErrorContent: the object returned
            // RuleInError: ExceptionValidationRule (internal)
            RunSteps += new TestStep(ReturnAnythingElse);
            // ValidationError is created and added to the ValidationErrorCollection of element
            // Exception: the exception thrown
            // ErrorContent: exception.Message
            // RuleInError: ExceptionValidationRule (internal)
            RunSteps += new TestStep(NoHandler);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _player = RootElement.Resources["player1"] as Player;
            _textBox = (TextBox)Util.FindElement(RootElement, "textBox");

            if (_player == null)
            {
                LogComment("Unable to reference the Player data source.");
                return TestResult.Fail;
            }

            if (_textBox == null)
            {
                LogComment("Unable to reference textBox element.");
                return TestResult.Fail;
            }

            _nonStaticVerifier = new ValidationNonStaticVerifier();
            _staticVerifier = new ValidationStaticVerifier(_textBox);

            return TestResult.Pass;
        }

        #region ReturnValidationError
        // check the situation where the exception handler returns a ValidationError
        private TestResult ReturnValidationError()
        {
            Status("ReturnValidationError");

            BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            binding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(ReturnValidationErrorHandler);

            // this will cause an exception because the data source is of type int
            _textBox.Text = "hello";

            bindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("ReturnValidationErrorHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnValidationError - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckValidationError(0, _error)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "ReturnValidationError - property";
            if (!_nonStaticVerifier.CheckHasError(bindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(bindingExpression, _error)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object ReturnValidationErrorHandler(object binding, Exception exception)
        {
            Status("ReturnValidationErrorHandler");
            BindingExpression priceBinding = _textBox.GetBindingExpression(TextBox.TextProperty);
            if (priceBinding != binding)
            {
                LogComment("Fail - BindingExpression received as a parameter is not as expected");
                Signal("ReturnValidationErrorHandler", TestResult.Fail);
            }
            Binding bind = ((BindingExpression)binding).ParentBinding;
            if (bind == null)
            {
                LogComment("Fail - Binding is null");
                Signal("ReturnValidationErrorHandler", TestResult.Fail);
            }

            if (exception.GetType() != typeof(FormatException))
            {
                LogComment("Fail - Exception's type not as expected");
                Signal("ReturnValidationErrorHandler", TestResult.Fail);
            }

            // Passing a rule. Null cannot be passed anymore.
            _error = new ValidationError(new RangeRule(), binding, exception.Message, exception);
            Signal("ReturnValidationErrorHandler", TestResult.Pass);
            return _error;
        }
        #endregion

        // provides coverage for the getter of binding.UpdateSourceExceptionFilter
        private TestResult VerifyUpdateSourceExceptionFilter()
        {
            Status("VerifyUpdateSourceExceptionFilter");

            BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            UpdateSourceExceptionFilterCallback callback = binding.UpdateSourceExceptionFilter;

            if (callback.Method.Name != "ReturnValidationErrorHandler")
            {
                LogComment("Fail - Unexpected method name. Expected:ReturnValidationErrorHandler. Actual:" +
                    callback.Method.Name);
                return TestResult.Fail;
            }

            if (callback.Target != this)
            {
                LogComment("Fail - callback.Target not as expected.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #region ReturnNull
        private TestResult ReturnNull()
        {
            Status("ReturnNull");

            BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            binding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(ReturnNullHandler);

            // this will cause an exception because the data source is of type int
            _textBox.Text = "hello";

            bindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("ReturnNullHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnNull - element";
#if TESTBUILD_CLR40
            if (!staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }
#endif
            // This is expected behavior.  
#if TESTBUILD_CLR20
            if (staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; } // Should be 0 errors for pre-Dev10
#endif
            // property level
            _nonStaticVerifier.Step = "ReturnNull - property";
#if TESTBUILD_CLR40
            if (!nonStaticVerifier.CheckHasError(bindingExpression, true)) { return TestResult.Fail; }
#endif
#if TESTBUILD_CLR20
            if (nonStaticVerifier.CheckHasError(bindingExpression, true)) { return TestResult.Fail; }  // No errors for 3.X
#endif
            ResetValidation();
            return TestResult.Pass;
        }

        object ReturnNullHandler(object binding, Exception exception)
        {
            Status("ReturnNullHandler");
            Signal("ReturnNullHandler", TestResult.Pass);
            return null;
        }
        #endregion

        #region ReturnAnythingElse
        private TestResult ReturnAnythingElse()
        {
            Status("ReturnAnythingElse");

            BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            binding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(ReturnAnythingElse);

            // this will cause an exception because the data source is of type int
            _textBox.Text = "hello";

            bindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("ReturnAnythingElse");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnAnythingElse - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
            FormatException originalException = new FormatException("Input string was not in a correct format.");
            ValidationError errorExpected = new ValidationError(new ExceptionValidationRule(), bindingExpression, "any object", originalException);
            if (!_staticVerifier.CheckValidationError(0, errorExpected)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "ReturnAnythingElse - property";
            if (!_nonStaticVerifier.CheckHasError(bindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(bindingExpression, errorExpected)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object ReturnAnythingElse(object binding, Exception exception)
        {
            Status("ReturnAnythingElse");
            Signal("ReturnAnythingElse", TestResult.Pass);
            return "any object";
        }

        #endregion

        #region NoHandler

        private TestResult NoHandler()
        {
            Status("NoHandler");

            BindingExpression bindingExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
            Binding binding = bindingExpression.ParentBinding;
            binding.UpdateSourceExceptionFilter = null;

            // this will cause an exception because the data source is of type int
            _textBox.Text = "hello";

            bindingExpression.UpdateSource();

            // element level
            _staticVerifier.Step = "NoHandler - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(1)) { return TestResult.Fail; }
            // Notice how the error content gets set to the exception's message. 
            FormatException exception = new FormatException("Input string was not in a correct format.");
            ValidationError errorExpected = new ValidationError(new ExceptionValidationRule(), bindingExpression, exception.Message, exception);
            if (!_staticVerifier.CheckValidationError(0, errorExpected)) { return TestResult.Fail; }
            // property level
            _nonStaticVerifier.Step = "NoHandler - property";
            if (!_nonStaticVerifier.CheckHasError(bindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(bindingExpression, errorExpected)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        #endregion

        #region AuxMethods
        private void ResetValidation()
        {
            BindingExpression binding = _textBox.GetBindingExpression(TextBox.TextProperty);
            _textBox.Text = "3";
            binding.UpdateSource();
        }
        #endregion
    }
}



