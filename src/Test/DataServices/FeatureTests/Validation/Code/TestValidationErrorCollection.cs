// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests all public methods and properties of ValidationErrorCollection.cs (to 
    /// exercise code, no real user scenarios)
    /// </description>
    /// </summary>
    [Test(3, "Validation", "TestValidationErrorCollection")]
    public class TestValidationErrorCollection : XamlTest
    {
        private ReadOnlyCollection<ValidationError> _errorCollection;
        private XmlDataProvider _dso;
        private TextBox _tbPrice1;
        private ValidationError _error1;
        private ValidationError _error2;
        private ValidationError _error3;

        public TestValidationErrorCollection()
            : base(@"TestValidationErrorCollection.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(CreateValidationErrorCollection);
            RunSteps += new TestStep(CreateValidationError);
            RunSteps += new TestStep(TestItemsGetter);
            RunSteps += new TestStep(TestIndexOf);
            RunSteps += new TestStep(TestContains);
            RunSteps += new TestStep(TestCopyTo);
            RunSteps += new TestStep(TestAdd);
            RunSteps += new TestStep(TestInsert);
            RunSteps += new TestStep(TestRemove);
            RunSteps += new TestStep(TestRemoveAt);
            RunSteps += new TestStep(TestCollectionGetters);
            RunSteps += new TestStep(TestIListGetters);
            RunSteps += new TestStep(TestItemsSetter);
            RunSteps += new TestStep(TestGetEnumerator);
            RunSteps += new TestStep(TestValidationError);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            _dso = RootElement.Resources["dso"] as XmlDataProvider;
            _tbPrice1 = (TextBox)Util.FindElement(RootElement, "tbPrice1");

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

            return TestResult.Pass;
        }

        private TestResult CreateValidationErrorCollection()
        {
            Status("CreateValidationErrorCollection");

            _tbPrice1.Text = "143"; // invalid - it must be between 0 and 10
            _tbPrice1.ToolTip = "43"; // invalid - it must be even

            WaitForPriority(DispatcherPriority.SystemIdle);

            BindingExpression bindingExpressionText = _tbPrice1.GetBindingExpression(TextBox.TextProperty);
            WaitForPriority(DispatcherPriority.SystemIdle);
            BindingExpression bindingExpressionToolTip = _tbPrice1.GetBindingExpression(TextBox.ToolTipProperty);
            WaitForPriority(DispatcherPriority.SystemIdle);
            bindingExpressionText.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            bindingExpressionToolTip.UpdateSource();

            WaitForPriority(DispatcherPriority.SystemIdle);
            _errorCollection = _tbPrice1.GetValue(Validation.ErrorsProperty) as ReadOnlyCollection<ValidationError>;

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (_errorCollection.Count == 0))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                _errorCollection = _tbPrice1.GetValue(Validation.ErrorsProperty) as ReadOnlyCollection<ValidationError>;
            }

            if (_errorCollection.Count != 2)
            {
                LogComment("Fail - There should be 2 errors in the collection, instead there are " + _errorCollection.Count);
                return TestResult.Fail;
            }

            _error1 = bindingExpressionText.ValidationError;
            if (_error1 == null)
            {
                LogComment("Fail - ValidationError associated with TextProperty is null");
                return TestResult.Fail;
            }
            _error2 = bindingExpressionToolTip.ValidationError;
            if (_error2 == null)
            {
                LogComment("Fail - ValidationError associated with ToolTipProperty is null");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult CreateValidationError()
        {
            Status("CreateValidationError");

            // Create a ValidationError that is not part of the collection - used to test
            // some methods of the collection
            BindingExpression bindingExpressionMaxLength = _tbPrice1.GetBindingExpression(TextBox.MaxLengthProperty);
            Binding bindingMaxLength = bindingExpressionMaxLength.ParentBinding;
            ValidationRule vrMaxLength = bindingMaxLength.ValidationRules[0] as ValidationRule;
            _error3 = new ValidationError(vrMaxLength, bindingExpressionMaxLength);
            if (_error3 == null)
            {
                LogComment("Fail - ValidationError associated with MaxLengthProperty is null");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #region TestItemsGetter
        private TestResult TestItemsGetter()
        {
            Status("TestItemsGetter");

            // verify count getter
            int count1 = _errorCollection.Count;
            if (count1 != 2)
            {
                LogComment("Fail - There should be 2 errors in ValidationErrorCollection, instead there are " + count1 + " (VEC)");
                return TestResult.Fail;
            }

            int count2 = ((IList)_errorCollection).Count;
            if (count2 != 2)
            {
                LogComment("Fail - There should be 2 errors in ValidationErrorCollection, instead there are " + count2 + " (IList)");
                return TestResult.Fail;
            }

            // verify item getter
            TestResult res1 = ItemsGetterIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = ItemsGetterVEC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ItemsGetterIList()
        {
            Status("ItemsGetterIList");

            ValidationError actualValidationError1 = ((IList)_errorCollection)[0] as ValidationError;
            if (actualValidationError1 != _error1)
            {
                LogComment("Fail - ValidationError in index 0 not as expected (IList)");
                return TestResult.Fail;
            }

            ValidationError actualValidationError2 = ((IList)_errorCollection)[1] as ValidationError;
            if (actualValidationError2 != _error2)
            {
                LogComment("Fail - ValidationError in index 1 not as expected (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult ItemsGetterVEC()
        {
            Status("ItemsGetterVEC");

            ValidationError actualValidationError1 = _errorCollection[0];
            if (actualValidationError1 != _error1)
            {
                LogComment("Fail - ValidationError in index 0 not as expected (VEC)");
                return TestResult.Fail;
            }

            ValidationError actualValidationError2 = _errorCollection[1];
            if (actualValidationError2 != _error2)
            {
                LogComment("Fail - ValidationError in index 1 not as expected (VEC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region TestIndexOf
        private TestResult TestIndexOf()
        {
            Status("TestIndexOf");

            // verify IndexOf
            TestResult res1 = TestIndexOfIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = TestIndexOfVEC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestIndexOfIList()
        {
            Status("TestIndexOfIList");

            int actualIndex1 = ((IList)_errorCollection).IndexOf(_error1);
            int expectedIndex1 = 0;
            if (expectedIndex1 != actualIndex1)
            {
                LogComment("Fail - Actual index of ve1:" + actualIndex1 + ", Expected:" + expectedIndex1 + " (IList)");
                return TestResult.Fail;
            }

            int actualIndex2 = ((IList)_errorCollection).IndexOf(_error2);
            int expectedIndex2 = 1;
            if (expectedIndex2 != actualIndex2)
            {
                LogComment("Fail - Actual index of ve2:" + actualIndex2 + ", Expected:" + expectedIndex2 + " (IList)");
                return TestResult.Fail;
            }

            int actualIndex3 = ((IList)_errorCollection).IndexOf(null);
            int expectedIndex3 = -1;
            if (expectedIndex3 != actualIndex3)
            {
                LogComment("Fail - Actual index of null:" + actualIndex3 + ", Expected:" + expectedIndex3 + " (IList)");
                return TestResult.Fail;
            }

            int actualIndex4 = ((IList)_errorCollection).IndexOf(_error3);
            int expectedIndex4 = -1;
            if (expectedIndex4 != actualIndex4)
            {
                LogComment("Fail - Actual index of ve3:" + actualIndex4 + ", Expected:" + expectedIndex4 + " (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestIndexOfVEC()
        {
            Status("TestIndexOfVEC");

            int actualIndex1 = _errorCollection.IndexOf(_error1);
            int expectedIndex1 = 0;
            if (expectedIndex1 != actualIndex1)
            {
                LogComment("Fail - Actual index of ve1:" + actualIndex1 + ", Expected:" + expectedIndex1 + " (VEC)");
                return TestResult.Fail;
            }

            int actualIndex2 = _errorCollection.IndexOf(_error2);
            int expectedIndex2 = 1;
            if (expectedIndex2 != actualIndex2)
            {
                LogComment("Fail - Actual index of ve2:" + actualIndex2 + ", Expected:" + expectedIndex2 + " (VEC)");
                return TestResult.Fail;
            }

            int actualIndex3 = _errorCollection.IndexOf(null);
            int expectedIndex3 = -1;
            if (expectedIndex3 != actualIndex3)
            {
                LogComment("Fail - Actual index of null:" + actualIndex3 + ", Expected:" + expectedIndex3 + " (VEC)");
                return TestResult.Fail;
            }

            int actualIndex4 = _errorCollection.IndexOf(_error3);
            int expectedIndex4 = -1;
            if (expectedIndex4 != actualIndex4)
            {
                LogComment("Fail - Actual index of ve3:" + actualIndex4 + ", Expected:" + expectedIndex4 + " (VEC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region TestContains
        private TestResult TestContains()
        {
            Status("TestContains");

            TestResult res1 = TestContainsIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = TestContainsVEC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestContainsIList()
        {
            Status("TestContainsIList");

            if (((IList)_errorCollection).Contains(_error1) == false)
            {
                LogComment("Fail - Contains method - vec does not contain ve1 (IList)");
                return TestResult.Fail;
            }
            if (((IList)_errorCollection).Contains(_error2) == false)
            {
                LogComment("Fail - Contains method - vec does not contain ve2 (IList)");
                return TestResult.Fail;
            }
            if (((IList)_errorCollection).Contains(_error3) == true)
            {
                LogComment("Fail - Contains method - vec contains ve3 (IList)");
                return TestResult.Fail;
            }
            if (((IList)_errorCollection).Contains(null) == true)
            {
                LogComment("Fail - Contains method - vec contains null (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestContainsVEC()
        {
            Status("TestContainsVEC");

            if (_errorCollection.Contains(_error1) == false)
            {
                LogComment("Fail - Contains method - vec does not contain ve1 (VEC)");
                return TestResult.Fail;
            }
            if (_errorCollection.Contains(_error2) == false)
            {
                LogComment("Fail - Contains method - vec does not contain ve2 (VEC)");
                return TestResult.Fail;
            }
            if (_errorCollection.Contains(_error3) == true)
            {
                LogComment("Fail - Contains method - vec contains ve3 (VEC)");
                return TestResult.Fail;
            }
            if (_errorCollection.Contains(null) == true)
            {
                LogComment("Fail - Contains method - vec contains null (VEC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region TestCopyTo
        private TestResult TestCopyTo()
        {
            Status("TestCopyTo");

            TestResult res1 = TestCopyToIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = TestCopyToVEC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestCopyToIList()
        {
            Status("TestCopyToIList");

            ValidationError[] errorArray = new ValidationError[10];

            ((IList)_errorCollection).CopyTo(errorArray, 1);

            if (errorArray[0] != null)
            {
                LogComment("Fail - errorArray[0] is not null, it should be (IList)");
                return TestResult.Fail;
            }
            if (errorArray[1] != _error1)
            {
                LogComment("Fail - errorArray[1] is not as expected (IList)");
                return TestResult.Fail;
            }
            if (errorArray[2] != _error2)
            {
                LogComment("Fail - errorArray[2] is not as expected (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestCopyToVEC()
        {
            Status("TestCopyToVEC");

            ValidationError[] errorArray = new ValidationError[10];

            _errorCollection.CopyTo(errorArray, 1);

            if (errorArray[0] != null)
            {
                LogComment("Fail - errorArray[0] is not null, it should be (VEC)");
                return TestResult.Fail;
            }
            if (errorArray[1] != _error1)
            {
                LogComment("Fail - errorArray[1] is not as expected (VEC)");
                return TestResult.Fail;
            }
            if (errorArray[2] != _error2)
            {
                LogComment("Fail - errorArray[2] is not as expected (VEC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        private TestResult TestAdd()
        {
            Status("TestAdd");

            // Add method only exists in IList. The one in System.Collections.Generic.IList<ValidationError> is internal
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            ((IList)_errorCollection).Add(_error3);

            return TestResult.Pass;
        }

        private TestResult TestInsert()
        {
            Status("TestInsert");

            // Insert in System.Collections.Generic.IList<ValidationError> is internal
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            ((IList)_errorCollection).Insert(0, _error3);

            return TestResult.Pass;
        }

        private TestResult TestRemove()
        {
            Status("TestRemove");

            // Remove in System.Collections.Generic.IList<ValidationError> is internal
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            ((IList)_errorCollection).RemoveAt(0);

            return TestResult.Pass;
        }

        private TestResult TestRemoveAt()
        {
            Status("TestRemoveAt");

            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            ((IList)_errorCollection).Remove(_error1);

            return TestResult.Pass;
        }

        private TestResult TestCollectionGetters()
        {
            Status("TestCollectionGetters");

            bool isSynchronized = ((ICollection)_errorCollection).IsSynchronized;
            if (isSynchronized != false)
            {
                LogComment("Fail - Collection should not be synchronized (thread safe) by default");
                return TestResult.Fail;
            }

            // SyncRoot is used to synchronize access to ICollection
            object obj = ((ICollection)_errorCollection).SyncRoot;
            if (obj == null)
            {
                LogComment("Fail - SyncRoot is null");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestIListGetters()
        {
            Status("TestIListGetters");

            bool isFixedSize = ((IList)_errorCollection).IsFixedSize;
            if (isFixedSize != true)
            {
                LogComment("Fail - IsFixedSize should be true");
                return TestResult.Fail;
            }

            bool isReadOnly = ((IList)_errorCollection).IsReadOnly;
            if (isReadOnly != true)
            {
                LogComment("Fail - IsReadOnly should be true");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestGetEnumerator()
        {
            Status("TestGetEnumerator");

            IEnumerator ie = ((IEnumerable)_errorCollection).GetEnumerator();
            if (ie == null)
            {
                LogComment("Fail - IEnumerator is null");
                return TestResult.Fail;
            }

            int i = 0;
            foreach (ValidationError ve in _errorCollection)
            {
                i++;
            }
            if (i != 2)
            {
                LogComment("Fail - There should be 2 ValidationErrors, instead there are " + i);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestItemsSetter()
        {
            Status("TestItemsSetter");

            // there is a setter only for IList, there is not setter for System.Collections.Generic.IList<ValidationError>
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            ((IList)_errorCollection)[1] = _error3;

            return TestResult.Pass;
        }

        public TestResult TestValidationError()
        {
            Status("TestValidationError");

            string newErrorContent = "This is the new error content";
            Exception newException = new Exception("New exception");
            RangeRule newRule = new RangeRule();

            _error3.ErrorContent = newErrorContent;
            _error3.Exception = newException;
            _error3.RuleInError = newRule;

            if ((string)_error3.ErrorContent != newErrorContent)
            {
                LogComment("Fail - Actual ValidationError's content:" + _error3.ErrorContent + " Expected:" + newErrorContent);
                return TestResult.Fail;
            }
            if (_error3.Exception != newException)
            {
                LogComment("Fail - Actual ValidationError's exception:" + _error3.Exception + " Expected:" + newException);
                return TestResult.Fail;
            }
            if (_error3.RuleInError != newRule)
            {
                LogComment("Fail - Actual ValidationError's rule:" + _error3.RuleInError + " Expected:" + newRule);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
