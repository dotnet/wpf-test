// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Threading;

    [Test(1, "INotifyDataErrorInfo", "BindingTestP1", SupportFiles = @"featuretests\dataservices\content\indei.binding.xaml")]
    public class BindingTestP1 : INotifyDataErrorInfoTest
    {
        public BindingTestP1() 
            : base("indei.binding.xaml", false, false)
        {
            this.RunSteps += ValidatesOnNotifyDataErrorsFalse;
            this.RunSteps += AddErrorConverterError;
            this.RunSteps += ClearBinding;
            this.RunSteps += ClearBindingDP;
            this.RunSteps += GetErrorsException;
            this.RunSteps += GetErrorsExceptionCritical;
            this.RunSteps += GetErrorsNull;
            this.RunSteps += UpdateBinding;
            this.RunSteps += SwitchDataContext;
            this.RunSteps += ErrorsChangedEventManagerAddRemove;
            this.RunSteps += ErrorsChangedEventManagerExceptions;
        }

        public override TestResult Initialize()
        {
            base.Initialize();

            _textboxA = FindDependecyObject<TextBox>(Window, "textboxA");
            _textboxB = FindDependecyObject<TextBox>(Window, "textboxB");
            _listboxA = FindDependecyObject<ListBox>(Window, "listboxA");
            _listboxB = FindDependecyObject<ListBox>(Window, "listboxB");

            Assert.IsNotNull(_textboxA, "Could not find textbox (A) target.");
            Assert.IsNotNull(_textboxB, "Could not find textbox (B) target.");
            Assert.IsNotNull(_listboxA, "Could not find listbox (A) target.");
            Assert.IsNotNull(_listboxB, "Could not find textbox (B) target.");

            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure errors are not notified when ValidatesOnNotifyDataErrors is false
        /// </summary>
        private TestResult ValidatesOnNotifyDataErrorsFalse()
        {
            DataStringErrors data = new DataStringErrors(false);
            data.StringProperty = "gotham";

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", false));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count.");

            _textboxA.Text = "#";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Error should not be norified.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify error thrown by ValueConverter is reflected in Validation.GetErrors
        /// </summary>
        private TestResult AddErrorConverterError()
        {
            object data = CreateDataObject(string.Empty, 2);
            SetBinding(_textboxB, TextBox.TextProperty, CreateBinding(data, "IntProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Initial error count.");

            // add error
            _textboxB.Text = "90000";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 1, _listboxB), "Fail: Adding 1 error.");

            // add converter error
            _textboxB.Text = "string";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 2, _listboxB), "Fail: Adding converter error.");

            // this will clear converter error, add two validation errors
            _textboxB.Text = "333333";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 2, _listboxB), "Fail: Clearing converter error, adding 2 error");

            // remove too large error, odd error remains
            _textboxB.Text = "1";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 1, _listboxB), "Fail: Clearing 1 error, 1 remaining.");

            // remove odd error, 0 errors
            _textboxB.Text = "10";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxB, 0, _listboxB), "Fail: Clearing all errors.");

            ResetBindings(_textboxB);

            return TestResult.Pass;
        }

        /// <summary>
        /// Bind to an object that contains no errors, then update property 
        /// with value to cause data.  Change data object that control is bound to
        /// an object with no errors and verify Validation.Errors returns correct errors.
        /// </summary>
        private TestResult ClearBinding()
        {
            object data = CreateDataObject("superman#", 0);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Initial error count.");

            BindingOperations.ClearAllBindings(_textboxA);

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Clearing bindings.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Bind to an object that contains no errors, then update property 
        /// with value to cause data.  Change data object that control is bound to
        /// an object with no errors and verify Validation.Errors returns correct errors.
        /// </summary>
        private TestResult ClearBindingDP()
        {
            object data = CreateDataObject("superman#", 0);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Initial error count.");

            _textboxA.ClearValue(TextBox.TextProperty);

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Clearing TextProperty.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Throw exception in Validation.GetErrors.  Verify error notification
        /// occurs, and exception is handled by data system.
        /// </summary>
        private TestResult GetErrorsException()
        {
            DataThrowGetErrors data = new DataThrowGetErrors(false, false);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            string teststring = "who is the green lantern?";

            _textboxA.Text = teststring;

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Exceptions thrown in GetErrors, count should still be 0.");
            Assert.IsTrue(data.StringProperty.Equals(teststring), "Data object's string property is not correct value");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Throw critical exception in Validation.GetErrors.  Verify exception is thrown
        /// </summary>
        private TestResult GetErrorsExceptionCritical()
        {
            string teststring = "where is krypton?";

            DataThrowGetErrors data = new DataThrowGetErrors(false, true);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            try
            {
                _textboxA.Text = teststring;
            }
            catch (System.Security.SecurityException)
            {
                Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Exceptions thrown in GetErrors, count should still be 0.");

                ResetBindings(_textboxA);

                return TestResult.Pass;
            }

            Log.LogStatus("Fail: Critical SecurityException should have been thrown.");
            return TestResult.Fail;
        }

        /// <summary>
        /// Return null from Validation.GetErrors.  Verify error notification occurs.
        /// </summary>
        private TestResult GetErrorsNull()
        {
            string teststring = "how fast is the flash?";
            
            DataNullErrors data = new DataNullErrors(false);
            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count.");

            _textboxA.Text = teststring;

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: GetErrors returns null, count should still be 0.");
            Assert.IsTrue(data.StringProperty.Equals(teststring), "Fail: Data object's string property is not correct value");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Update binding on DepenencyObject and verify that errors are removed
        /// </summary>
        private TestResult UpdateBinding()
        {
            object data1 = CreateDataObject("superman", 100);
            object data2 = CreateDataObject("batman", 888);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data1, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count.");

            _textboxA.Text += "#";

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 1, _listboxA), "Fail: Adding 1 error.");

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data2, "StringProperty", true));

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Error count is wrong after updating binding to new data object");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Switch DataContext of DepenencyObject and verify that errors are updated
        /// </summary>
        private TestResult SwitchDataContext() 
        {
            object data1 = CreateDataObject("batman", 0);
            object data2 = CreateDataObject("batman#lives#in#gotham", 0);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding("StringProperty", true));

            _textboxA.DataContext = data1;

            Wait();

            Assert.IsTrue(ValidateErrors(_textboxA, 0, _listboxA), "Fail: Initial error count.");

            _textboxA.DataContext = data2;

            DispatcherHelper.DoEvents(500);

            Assert.IsTrue(ValidateErrors(_textboxA, 2), "Fail: Error count is wrong after switching datacontext to new data object");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// Add/remove custom error handler to ErrorsChangedEventManager
        /// </summary>
        private TestResult ErrorsChangedEventManagerAddRemove()
        {
            bool errorsChanged = false;

            EventHandler<DataErrorsChangedEventArgs> customHandler =
                new EventHandler<DataErrorsChangedEventArgs>(delegate { errorsChanged = true; });

            DataStringErrors data = new DataStringErrors(false);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            // Add custom handler.
            ErrorsChangedEventManager.AddHandler(data, customHandler);

            Wait();

            _textboxA.Text = "its a bird, its a plane, no its superman";

            Wait();

            Assert.IsTrue(errorsChanged, "ErrorsChangedEventManager did not raise ErrorsChanged event to custom handler.");

            errorsChanged = false;

            // Remove custom handler
            ErrorsChangedEventManager.RemoveHandler(data, customHandler);

            Wait();

            _textboxA.Text += "#";

            Wait();

            Assert.IsFalse(errorsChanged, "ErrorsChangedEventManager should not have raised ErrorsChanged event to custom handler.");

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        /// <summary>
        /// ErrorsChangedEventManager exception tests
        /// </summary>
        private TestResult ErrorsChangedEventManagerExceptions()
        {
            EventHandler<DataErrorsChangedEventArgs> customHandler =
                new EventHandler<DataErrorsChangedEventArgs>(delegate { Log.LogStatus("Custom Error handler, we should never see this message"); });

            DataStringErrors data = new DataStringErrors(false);

            SetBinding(_textboxA, TextBox.TextProperty, CreateBinding(data, "StringProperty", true));

            // Exception: ErrorsChangedEventManager.AddHandler null data source
            try
            {
                ErrorsChangedEventManager.AddHandler(null, customHandler);
                Log.LogStatus("ErrorsChangedEventManager.AddHandler did not throw exception with null data source");
                return TestResult.Fail;
            }
            catch (ArgumentNullException)
            {
                Log.LogStatus("ErrorsChangedEventManager.AddHandler threw correct exception with null data source");
            }

            // Exception: ErrorsChangedEventManager.AddHandler null handler
            try
            {
                ErrorsChangedEventManager.AddHandler(data, null);
                Log.LogStatus("ErrorsChangedEventManager.AddHandler did not throw exception with null handler");
                return TestResult.Fail;
            }
            catch (ArgumentNullException)
            {
                Log.LogStatus("ErrorsChangedEventManager.AddHandler threw correct exception with null handler");
            }

            // Exception: ErrorsChangedEventManager.RemoveHandler null data source
            try
            {
                ErrorsChangedEventManager.RemoveHandler(null, customHandler);
                Log.LogStatus("ErrorsChangedEventManager.RemoveHandler did not throw exception with null data source");
                return TestResult.Fail;
            }
            catch (ArgumentNullException)
            {
                Log.LogStatus("ErrorsChangedEventManager.RemoveHandler threw correct exception with null data source");
            }

            // Exception: ErrorsChangedEventManager.RemoveHandler null handler
            try
            {
                ErrorsChangedEventManager.RemoveHandler(data, null);
                Log.LogStatus("ErrorsChangedEventManager.RemoveHandler did not throw exception with null handler");
                return TestResult.Fail;
            }
            catch (ArgumentNullException)
            {
                Log.LogStatus("ErrorsChangedEventManager.RemoveHandler threw correct exception with null handler");
            }

            ResetBindings(_textboxA);

            return TestResult.Pass;
        }

        private TextBox _textboxA = null;
        private TextBox _textboxB = null;
        private ListBox _listboxA = null;
        private ListBox _listboxB = null;
    }
}
