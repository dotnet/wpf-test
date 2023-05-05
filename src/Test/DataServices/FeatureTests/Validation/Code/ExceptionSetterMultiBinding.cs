// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This test throws an exception in the setter of a data source property
    /// and uses the UpdateSourceExceptionFilter callback to deal with that. Same as
    /// the ExceptionTransformer test but for MultiBind.
    /// </description>
    /// <relatedBugs>





    /// </relatedBugs>
    /// </summary>
    [Test(2, "Validation", "ExceptionSetterMultiBinding")]
    public class ExceptionSetterMultiBinding : XamlTest
    {
        private TextBox _tb;
        private Star _star;
        private ValidationNonStaticVerifier _nonStaticVerifier;
        private ValidationStaticVerifier _staticVerifier;
        private int _multiBindingReturnValidationErrorHandlerCount;
        private int _bindingReturnValidationErrorHandlerCount;
        private RangeRule _ruleInError0;
        private RangeRule _ruleInError1;
        private RangeRule _ruleInError2;
        private MultiBindingExpression _multiBindingExpression;
        private MultiBinding _multiBinding;
        private BindingExpression _bindingExpression0;
        private Binding _binding0;
        private BindingExpression _bindingExpression1;
        private Binding _binding1;
        private BindingExpression _bindingExpression2;
        private Binding _binding2;

        public ExceptionSetterMultiBinding()
            : base(@"ExceptionSetterMultiBinding.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            // Handler in Binding returns null
            RunSteps += new TestStep(BindingReturnNull);
            // Handler in Binding returns ValidationError
            RunSteps += new TestStep(BindingReturnValidationError);
            // Handler in Binding returns anything else - 
            RunSteps += new TestStep(BindingReturnAnythingElse);
            // No handler in Binding, handler in MultiBinding returns null
            RunSteps += new TestStep(MultiBindingReturnNull);
            // No handler in Binding, handler in MultiBinding returns ValidationError
            RunSteps += new TestStep(MultiBindingReturnValidationError);
            // No handler in Binding, handler in MultiBinding returns anything else - 
            RunSteps += new TestStep(MultiBindingReturnAnythingElse);
            // No handler in Binding, no handler in MultiBinding - 
            RunSteps += new TestStep(MultiBindingNoHandler);

            // Verify event handler gets called once for each binding - 
            RunSteps += new TestStep(VerifyMultiBindingReturnValidationErrorHandler);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _tb = Util.FindElement(RootElement, "tb") as TextBox;
            _star = RootElement.Resources["player1"] as Star;
            if (_tb == null)
            {
                LogComment("Fail - Unable to reference the TextBox tb");
                return TestResult.Fail;
            }
            if (_star == null)
            {
                LogComment("Fail - Unable to reference the data source Star");
                return TestResult.Fail;
            }

            _nonStaticVerifier = new ValidationNonStaticVerifier();
            _staticVerifier = new ValidationStaticVerifier(_tb);

            _multiBindingReturnValidationErrorHandlerCount = 0;
            _bindingReturnValidationErrorHandlerCount = 0;

            _ruleInError0 = new RangeRule();
            _ruleInError1 = new RangeRule();
            _ruleInError2 = new RangeRule();

            _multiBindingExpression = BindingOperations.GetMultiBindingExpression(_tb, TextBox.TextProperty);
            _multiBinding = _multiBindingExpression.ParentMultiBinding;
            _bindingExpression0 = (BindingExpression)_multiBindingExpression.BindingExpressions[0];
            _binding0 = _bindingExpression0.ParentBinding;
            _bindingExpression1 = (BindingExpression)_multiBindingExpression.BindingExpressions[1];
            _binding1 = _bindingExpression1.ParentBinding;
            _bindingExpression2 = (BindingExpression)_multiBindingExpression.BindingExpressions[2];
            _binding2 = _bindingExpression2.ParentBinding;

            return TestResult.Pass;
        }

        #region BindingReturnNull
        private TestResult BindingReturnNull()
        {
            Status("BindingReturnNull");

            _binding0.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnNullHandler);
            _binding1.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnNullHandler);
            _binding2.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnNullHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("BindingReturnNullHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnNull - element";
            if (!_staticVerifier.CheckHasError(false)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "ReturnNull - property";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, false)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "ReturnNull - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], false)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object BindingReturnNullHandler(object binding, Exception exception)
        {
            Status("BindingReturnNullHandler");
            Signal("BindingReturnNullHandler", TestResult.Pass);
            return null;
        }
        #endregion

        #region BindingReturnValidationError
        private TestResult BindingReturnValidationError()
        {
            Status("BindingReturnValidationError");

            _binding0.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnValidationErrorHandler);
            _binding1.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnValidationErrorHandler);
            _binding2.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnValidationErrorHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("BindingReturnValidationErrorHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnValidationError - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(3)) { return TestResult.Fail; }
            Exception expectedException0 = new Exception("Can not set FirstName");
            ValidationError expectedError0 = new ValidationError(_ruleInError0, _multiBindingExpression.BindingExpressions[0], expectedException0.Message, expectedException0);
            if (!_staticVerifier.CheckValidationError(0, expectedError0)) { return TestResult.Fail; }
            Exception expectedException1 = new Exception("Can not set LastName");
            ValidationError expectedError1 = new ValidationError(_ruleInError1, _multiBindingExpression.BindingExpressions[1], expectedException1.Message, expectedException1);
            if (!_staticVerifier.CheckValidationError(1, expectedError1)) { return TestResult.Fail; }
            Exception expectedException2 = new Exception("Can not set Age");
            ValidationError expectedError2 = new ValidationError(_ruleInError2, _multiBindingExpression.BindingExpressions[2], expectedException2.Message, expectedException2);
            if (!_staticVerifier.CheckValidationError(2, expectedError2)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "ReturnValidationError - property";
            // 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError0)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "ReturnValidationError - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[0], expectedError0)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[1], expectedError1)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], expectedError2)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object BindingReturnValidationErrorHandler(object binding, Exception exception)
        {
            Status("BindingReturnValidationErrorHandler");
            ValidationError error;
            if (_bindingReturnValidationErrorHandlerCount == 0)
            {
                // Passing a rule. Null cannot be passed anymore.
                error = new ValidationError(_ruleInError0, binding, exception.Message, exception);
            }
            else if (_bindingReturnValidationErrorHandlerCount == 1)
            {
                error = new ValidationError(_ruleInError1, binding, exception.Message, exception);
            }
            else
            {
                error = new ValidationError(_ruleInError2, binding, exception.Message, exception);
            }
            _bindingReturnValidationErrorHandlerCount++;
            Signal("BindingReturnValidationErrorHandler", TestResult.Pass);
            return error;
        }
        #endregion

        #region BindingReturnAnythingElse
        private TestResult BindingReturnAnythingElse()
        {
            Status("BindingReturnAnythingElse");

            _binding0.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnAnythingElseHandler);
            _binding1.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnAnythingElseHandler);
            _binding2.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(BindingReturnAnythingElseHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("BindingReturnAnythingElseHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "ReturnAnythingElse - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(3)) { return TestResult.Fail; }
            Exception expectedException0 = new Exception("Can not set FirstName");
            ValidationError expectedError0 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[0], 3, expectedException0);
            if (!_staticVerifier.CheckValidationError(0, expectedError0)) { return TestResult.Fail; }
            Exception expectedException1 = new Exception("Can not set LastName");
            ValidationError expectedError1 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[1], 3, expectedException1);
            if (!_staticVerifier.CheckValidationError(1, expectedError1)) { return TestResult.Fail; }
            Exception expectedException2 = new Exception("Can not set Age");
            ValidationError expectedError2 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[2], 3, expectedException2);
            if (!_staticVerifier.CheckValidationError(2, expectedError2)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "ReturnAnythingElse - property";
            // 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError0)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "ReturnAnythingElse - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[0], expectedError0)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[1], expectedError1)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], expectedError2)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object BindingReturnAnythingElseHandler(object binding, Exception exception)
        {
            Status("BindingReturnAnythingElseHandler");
            Signal("BindingReturnAnythingElseHandler", TestResult.Pass);
            return 3;
        }
        #endregion

        #region MultiBindingReturnNull
        private TestResult MultiBindingReturnNull()
        {
            Status("MultiBindingReturnNull");

            _multiBinding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(MultiBindingReturnNullHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("MultiBindingReturnNullHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "MultiBindingReturnNull - element";
            if (!_staticVerifier.CheckHasError(false)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(0)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MultiBindingReturnNull - property";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, false)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "MultiBindingReturnNull - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], false)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], false)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object MultiBindingReturnNullHandler(object binding, Exception exception)
        {
            Status("MultiBindingReturnNullHandler");
            Signal("MultiBindingReturnNullHandler", TestResult.Pass);
            return null;
        }
        #endregion

        #region MultiBindingReturnValidationError
        // No handler in Bind, handler in MultiBinding returns ValidationError
        private TestResult MultiBindingReturnValidationError()
        {
            Status("MultiBindingReturnValidationError");

            _multiBinding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(MultiBindingReturnValidationErrorHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("MultiBindingReturnValidationErrorHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "MultiBindingReturnValidationError - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(3)) { return TestResult.Fail; }
            Exception expectedException0 = new Exception("Can not set FirstName");
            ValidationError expectedError0 = new ValidationError(_ruleInError0, _multiBindingExpression.BindingExpressions[0], expectedException0.Message, expectedException0);
            if (!_staticVerifier.CheckValidationError(0, expectedError0)) { return TestResult.Fail; }
            Exception expectedException1 = new Exception("Can not set LastName");
            ValidationError expectedError1 = new ValidationError(_ruleInError1, _multiBindingExpression.BindingExpressions[1], expectedException1.Message, expectedException1);
            if (!_staticVerifier.CheckValidationError(1, expectedError1)) { return TestResult.Fail; }
            Exception expectedException2 = new Exception("Can not set Age");
            ValidationError expectedError2 = new ValidationError(_ruleInError2, _multiBindingExpression.BindingExpressions[2], expectedException2.Message, expectedException2);
            if (!_staticVerifier.CheckValidationError(2, expectedError2)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MultiBindingReturnValidationError - property";
            // 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError0)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "MultiBindingReturnValidationError - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[0], expectedError0)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[1], expectedError1)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], expectedError2)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object MultiBindingReturnValidationErrorHandler(object binding, Exception exception)
        {
            Status("MultiBindingReturnValidationErrorHandler");

            ValidationError error;
            if (_multiBindingReturnValidationErrorHandlerCount == 0)
            {
                // Passing a rule. Null cannot be passed anymore.
                error = new ValidationError(_ruleInError0, binding, exception.Message, exception);
            }
            else if (_multiBindingReturnValidationErrorHandlerCount == 1)
            {
                error = new ValidationError(_ruleInError1, binding, exception.Message, exception);
            }
            else
            {
                error = new ValidationError(_ruleInError2, binding, exception.Message, exception);
            }

            _multiBindingReturnValidationErrorHandlerCount++;
            Signal("MultiBindingReturnValidationErrorHandler", TestResult.Pass);
            return error;
        }
        #endregion

        #region MultiBindingReturnAnythingElse
        private TestResult MultiBindingReturnAnythingElse()
        {
            Status("MultiBindingReturnAnythingElse");

            _multiBinding.UpdateSourceExceptionFilter = new UpdateSourceExceptionFilterCallback(MultiBindingReturnAnythingElseHandler);

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            TestResult resultWaitForSignal = WaitForSignal("MultiBindingReturnAnythingElseHandler");
            if (resultWaitForSignal != TestResult.Pass) { return resultWaitForSignal; }

            // element level
            _staticVerifier.Step = "MultiBindingReturnAnythingElse - element";
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(3)) { return TestResult.Fail; }
            Exception expectedException0 = new Exception("Can not set FirstName");
            ValidationError expectedError0 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[0], 4, expectedException0);
            if (!_staticVerifier.CheckValidationError(0, expectedError0)) { return TestResult.Fail; }
            Exception expectedException1 = new Exception("Can not set LastName");
            ValidationError expectedError1 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[1], 4, expectedException1);
            if (!_staticVerifier.CheckValidationError(1, expectedError1)) { return TestResult.Fail; }
            Exception expectedException2 = new Exception("Can not set Age");
            ValidationError expectedError2 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[2], 4, expectedException2);
            if (!_staticVerifier.CheckValidationError(2, expectedError2)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MultiBindingReturnAnythingElse - property";
            // 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError0)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "MultiBindingReturnAnythingElse - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[0], expectedError0)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[1], expectedError1)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], expectedError2)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }

        object MultiBindingReturnAnythingElseHandler(object binding, Exception exception)
        {
            Status("MultiBindingReturnAnythingElseHandler");
            Signal("MultiBindingReturnAnythingElseHandler", TestResult.Pass);
            return 4;
        }
        #endregion

        #region MultiBindingNoHandler
        private TestResult MultiBindingNoHandler()
        {
            Status("MultiBindingNoHandler");

            _multiBinding.UpdateSourceExceptionFilter = null;

            // an exception happens when updating each of the binds because ThrowExceptionOnSetter = true
            _star.ThrowExceptionOnSetter = true;
            _tb.Text = "Beatriz Costa 26";

            _multiBindingExpression.UpdateSource();

            // element level
            _staticVerifier.Step = "MultiBindingNoHandler - element";
            // 
            if (!_staticVerifier.CheckHasError(true)) { return TestResult.Fail; }
            if (!_staticVerifier.CheckNumErrors(3)) { return TestResult.Fail; }
            Exception expectedException0 = new Exception("Can not set FirstName");
            ValidationError expectedError0 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[0], expectedException0.Message, expectedException0);
            if (!_staticVerifier.CheckValidationError(0, expectedError0)) { return TestResult.Fail; }
            Exception expectedException1 = new Exception("Can not set LastName");
            ValidationError expectedError1 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[1], expectedException1.Message, expectedException1);
            if (!_staticVerifier.CheckValidationError(1, expectedError1)) { return TestResult.Fail; }
            Exception expectedException2 = new Exception("Can not set Age");
            ValidationError expectedError2 = new ValidationError(new ExceptionValidationRule(), _multiBindingExpression.BindingExpressions[2], expectedException2.Message, expectedException2);
            if (!_staticVerifier.CheckValidationError(2, expectedError2)) { return TestResult.Fail; }

            // property level
            _nonStaticVerifier.Step = "MultiBindingNoHandler - property";
            // 
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression, true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression, expectedError0)) { return TestResult.Fail; }

            // internal binding level
            _nonStaticVerifier.Step = "MultiBindingNoHandler - binding";
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[0], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[1], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckHasError(_multiBindingExpression.BindingExpressions[2], true)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[0], expectedError0)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[1], expectedError1)) { return TestResult.Fail; }
            if (!_nonStaticVerifier.CheckValidationError(_multiBindingExpression.BindingExpressions[2], expectedError2)) { return TestResult.Fail; }

            ResetValidation();
            return TestResult.Pass;
        }
        #endregion


        // Verifies that the MultiBindingReturnValidationErrorHandler was called only once for each binding
        private TestResult VerifyMultiBindingReturnValidationErrorHandler()
        {
            Status("VerifyMultiBindingReturnValidationErrorHandler");
            // the handler should be called once for each binding - there's 3 bindings in the multibinding
            if (_multiBindingReturnValidationErrorHandlerCount != 3)
            {
                LogComment("Fail - Event handler was not called 3 times, it was called " + _multiBindingReturnValidationErrorHandlerCount + " times.");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private void ResetValidation()
        {
            _multiBinding.UpdateSourceExceptionFilter = null;
            _binding0.UpdateSourceExceptionFilter = null;
            _binding1.UpdateSourceExceptionFilter = null;
            _binding2.UpdateSourceExceptionFilter = null;

            _star.ThrowExceptionOnSetter = false;
            _tb.Text = "Beatriz Costa 26";
            _multiBindingExpression.UpdateSource();
        }

    }

}

