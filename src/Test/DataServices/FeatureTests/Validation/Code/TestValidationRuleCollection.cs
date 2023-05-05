// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Globalization;
using System.Security;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests all public methods and properties of ValidationRuleCollection.cs (to 
    /// exercise code, no real user scenarios)
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(3, "Validation", "TestValidationRuleCollection")]
    public class TestValidationRuleCollection : WindowTest
    {
        private IList<ValidationRule> _ruleCollectionVRC; // used with the IList<IValidationRule> methods
        private IList<ValidationRule> _ruleCollectionIList; // used with the IList methods
        private RangeRule _rangeRule1;
        private RangeRule _rangeRule2;
        private EvenRule _evenRule1;
        private EvenRule _evenRule2;
        private Binding _bindingTestVRC;
        private Binding _bindingTestIList;

        public TestValidationRuleCollection()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AddElementsToCollection);
            RunSteps += new TestStep(FindElementsInCollection);
            RunSteps += new TestStep(DeleteElementsFromCollection);
            RunSteps += new TestStep(CopyToArray);
            RunSteps += new TestStep(TestCollectionGetters);
            RunSteps += new TestStep(TestIListGetters);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _rangeRule1 = new RangeRule();
            _rangeRule1.Min = 0;
            _rangeRule1.Max = 10;
            _rangeRule1.ErrorContent = "Value must be between " + _rangeRule1.Min + " and " + _rangeRule1.Max;

            _rangeRule2 = new RangeRule();
            _rangeRule2.Min = 2;
            _rangeRule2.Max = 16;
            _rangeRule1.ErrorContent = "Value must be between " + _rangeRule2.Min + " and " + _rangeRule2.Max;

            _evenRule1 = new EvenRule();
            _evenRule1.ErrorContent = "Value must be even - er1";

            _evenRule2 = new EvenRule();
            _evenRule2.ErrorContent = "Value must be even - er2";

            _bindingTestVRC = new Binding();
            _bindingTestIList = new Binding();
            _ruleCollectionVRC = _bindingTestVRC.ValidationRules;
            _ruleCollectionIList = _bindingTestIList.ValidationRules;

            return TestResult.Pass;
        }

        #region AddElementsToCollection
        private TestResult AddElementsToCollection()
        {
            Status("AddElementsToCollection");

            if (AddElementsToCollectionIList() != TestResult.Pass) { return TestResult.Fail; }
            if (AddElementsToCollectionVRC() != TestResult.Pass) { return TestResult.Fail; }
            if (AddNullIList() != TestResult.Pass) { return TestResult.Fail; }
            if (AddNotValidationRuleIList() != TestResult.Pass) { return TestResult.Fail; }
            if (InsertNullIList() != TestResult.Pass) { return TestResult.Fail; }
            if (InsertNotValidationRuleIList() != TestResult.Pass) { return TestResult.Fail; }
            if (ReplaceWithNullIList() != TestResult.Pass) { return TestResult.Fail; }
            if (ReplaceWithNotValidationRuleIList() != TestResult.Pass) { return TestResult.Fail; }
            if (InsertOutOfRangeIList() != TestResult.Pass) { return TestResult.Fail; }
            if (AddNullVRC() != TestResult.Pass) { return TestResult.Fail; }
            if (InsertNullVRC() != TestResult.Pass) { return TestResult.Fail; }
            if (ReplaceWithNullVRC() != TestResult.Pass) { return TestResult.Fail; }
            if (InsertOutOfRangeVRC() != TestResult.Pass) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        private TestResult AddElementsToCollectionIList()
        {
            Status("AddElementsToCollectionIList");

            // add an element with the Add(object obj) method and verify it was added
            ((IList)_ruleCollectionIList).Add(_rangeRule1);
            TestResult resultVerifyElement1 = VerifyElementIList(1, 0, _rangeRule1, "add rr1 (IList)");
            if (resultVerifyElement1 != TestResult.Pass) { return TestResult.Fail; }

            // replace an element using this[int index]
            ((IList)_ruleCollectionIList)[0] = _evenRule1;
            TestResult resultVerifyElement2 = VerifyElementIList(1, 0, _evenRule1, "replace with er1 (IList)");
            if (resultVerifyElement2 != TestResult.Pass) { return TestResult.Fail; }

            // insert an element using the Insert(...) method
            ((IList)_ruleCollectionIList).Insert(0, _rangeRule1); // order should be rr1, er1
            TestResult resultVerifyElement3 = VerifyElementIList(2, 0, _rangeRule1, "insert rr1, verify rr1 (IList)");
            if (resultVerifyElement3 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement4 = VerifyElementIList(2, 1, _evenRule1, "insert rr1, verify er1 (IList)");
            if (resultVerifyElement4 != TestResult.Pass) { return TestResult.Fail; }

            // add a few more elements 
            ((IList)_ruleCollectionIList).Add(_evenRule2);
            ((IList)_ruleCollectionIList).Add(_rangeRule2);
            // order should be rr1, er1, er2, rr2

            return TestResult.Pass;
        }

        private TestResult AddElementsToCollectionVRC()
        {
            Status("AddElementsToCollectionVRC");

            // add an element with the Add(ValidationRule validationRule) method and verify it was added
            _ruleCollectionVRC.Add(_rangeRule1);
            TestResult resultVerifyElement1 = VerifyElementVRC(1, 0, _rangeRule1, "add rr1 (VRC)");
            if (resultVerifyElement1 != TestResult.Pass) { return TestResult.Fail; }

            // replace an element using this[int index]
            _ruleCollectionVRC[0] = _evenRule1;
            TestResult resultVerifyElement2 = VerifyElementVRC(1, 0, _evenRule1, "replace with er1 (VRC)");
            if (resultVerifyElement2 != TestResult.Pass) { return TestResult.Fail; }

            // insert an element using the Insert(...) method
            _ruleCollectionVRC.Insert(0, _rangeRule1); // order should be rr1, er1
            TestResult resultVerifyElement3 = VerifyElementVRC(2, 0, _rangeRule1, "insert rr1, verify rr1 (VRC)");
            if (resultVerifyElement3 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement4 = VerifyElementVRC(2, 1, _evenRule1, "insert rr1, verify er1 (VRC)");
            if (resultVerifyElement4 != TestResult.Pass) { return TestResult.Fail; }

            // add a few more elements
            _ruleCollectionVRC.Add(_evenRule2);
            _ruleCollectionVRC.Add(_rangeRule2);
            // order should be rr1, er1, er2, rr2

            return TestResult.Pass;
        }

        // add null, verify it throws - Add
        private TestResult AddNullIList()
        {
            Status("AddNull");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            ((IList)_ruleCollectionIList).Add(null);
            return TestResult.Pass;
        }

        // add something that is not an ValidationRule - Add
        private TestResult AddNotValidationRuleIList()
        {
            Status("AddNotValidationRule");
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            ((IList)_ruleCollectionIList).Add(0);
            return TestResult.Pass;
        }

        // insert null, verify it throws - Insert
        private TestResult InsertNullIList()
        {
            Status("InsertNull");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            ((IList)_ruleCollectionIList).Insert(0, null);
            return TestResult.Pass;
        }

        // insert something that is not an ValidationRule - Insert
        private TestResult InsertNotValidationRuleIList()
        {
            Status("InsertNotValidationRule");
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            ((IList)_ruleCollectionIList).Insert(0, 0);
            return TestResult.Pass;
        }

        // replace a value with null - Assign
        private TestResult ReplaceWithNullIList()
        {
            Status("ReplaceWithNull");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            ((IList)_ruleCollectionIList)[0] = null;
            return TestResult.Pass;
        }

        // replace a value with something that is not an ValidationRule - Assign
        private TestResult ReplaceWithNotValidationRuleIList()
        {
            Status("ReplaceWithNotValidationRule");
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            ((IList)_ruleCollectionIList)[0] = 1;
            return TestResult.Pass;
        }

        // insert something out of range
        private TestResult InsertOutOfRangeIList()
        {
            Status("InsertOutOfRange");
            SetExpectedErrorTypeInStep(typeof(ArgumentOutOfRangeException));
            ((IList)_ruleCollectionIList).Insert(-1, _rangeRule1);
            return TestResult.Pass;
        }

        // add null, verify it throws
        private TestResult AddNullVRC()
        {
            Status("AddNullVRC");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            _ruleCollectionVRC.Add(null);
            return TestResult.Pass;
        }

        // insert null, verify it throws
        private TestResult InsertNullVRC()
        {
            Status("InsertNullVRC");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            _ruleCollectionVRC.Insert(0, null);
            return TestResult.Pass;
        }

        // assign null to a value, verify it throws
        private TestResult ReplaceWithNullVRC()
        {
            Status("ReplaceWithNullVRC");
            SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
            _ruleCollectionVRC[0] = null;
            return TestResult.Pass;
        }

        // insert something out of range
        private TestResult InsertOutOfRangeVRC()
        {
            Status("InsertOutOfRangeVRC");
            SetExpectedErrorTypeInStep(typeof(ArgumentOutOfRangeException));
            _ruleCollectionVRC.Insert(-1, _rangeRule1);
            return TestResult.Pass;
        }
        #endregion

        #region FindElementsInCollection
        private TestResult FindElementsInCollection()
        {
            Status("FindElementsInCollection");

            TestResult res1 = FindElementsInCollectionIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = FindElementsInCollectionVRC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult FindElementsInCollectionIList()
        {
            Status("FindElementsInCollectionIList");

            // test Contains(objec obj)
            if (!((IList)_ruleCollectionIList).Contains(_rangeRule1))
            {
                LogComment("Fail - Contains() method - vrc does not contain rr1 (it should) (Ilist)");
                return TestResult.Fail;
            }
            if (!((IList)_ruleCollectionIList).Contains(_evenRule1))
            {
                LogComment("Fail - Contains() method - vrc does not contain er1 (it should) (IList)");
                return TestResult.Fail;
            }
            if (!((IList)_ruleCollectionIList).Contains(_rangeRule2))
            {
                LogComment("Fail - Contains() method - vrc does not contain rr2 (it should) (IList)");
                return TestResult.Fail;
            }
            // test IndexOf(object obj)
            int actualRr1 = ((IList)_ruleCollectionIList).IndexOf(_rangeRule1);
            int expectedRr1 = 0;
            if (actualRr1 != expectedRr1)
            {
                LogComment("Fail - IndexOf() method - actual index of rr1:" + actualRr1 + ", Expected:" + expectedRr1 + " (IList)");
                return TestResult.Fail;
            }
            int actualEr2 = ((IList)_ruleCollectionIList).IndexOf(_evenRule2);
            int expectedEr2 = 2;
            if (actualEr2 != expectedEr2)
            {
                LogComment("Fail - IndexOf() method - actual index of er2:" + actualEr2 + ", Expected:" + expectedEr2 + " (IList)");
                return TestResult.Fail;
            }
            int actualRr2 = ((IList)_ruleCollectionIList).IndexOf(_rangeRule2);
            int expectedRr2 = 3;
            if (actualRr2 != expectedRr2)
            {
                LogComment("Fail - IndexOf() method - actual index of rr2:" + actualRr2 + ", Expected:" + expectedRr2 + " (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult FindElementsInCollectionVRC()
        {
            Status("FindElementsInCollectionVRC");

            // test Contains(ValidationRule validationRule)
            if (!_ruleCollectionVRC.Contains(_rangeRule1))
            {
                LogComment("Fail - Contains() method - vrc does not contain rr1 (it should) (VRC)");
                return TestResult.Fail;
            }
            if (!_ruleCollectionVRC.Contains(_evenRule1))
            {
                LogComment("Fail - Contains() method - vrc does not contain er1 (it should) (VRC)");
                return TestResult.Fail;
            }
            if (!_ruleCollectionVRC.Contains(_rangeRule2))
            {
                LogComment("Fail - Contains() method - vrc does not contain rr2 (it should) (VRC)");
                return TestResult.Fail;
            }
            // test IndexOf(ValidationRule validationRule)
            int actualRr1 = _ruleCollectionVRC.IndexOf(_rangeRule1);
            int expectedRr1 = 0;
            if (actualRr1 != expectedRr1)
            {
                LogComment("Fail - IndexOf() method - actual index of rr1:" + actualRr1 + ", Expected:" + expectedRr1 + " (VRC)");
                return TestResult.Fail;
            }
            int actualEr2 = _ruleCollectionVRC.IndexOf(_evenRule2);
            int expectedEr2 = 2;
            if (actualEr2 != expectedEr2)
            {
                LogComment("Fail - IndexOf() method - actual index of er2:" + actualEr2 + ", Expected:" + expectedEr2 + " (VRC)");
                return TestResult.Fail;
            }
            int actualRr2 = _ruleCollectionVRC.IndexOf(_rangeRule2);
            int expectedRr2 = 3;
            if (actualRr2 != expectedRr2)
            {
                LogComment("Fail - IndexOf() method - actual index of rr2:" + actualRr2 + ", Expected:" + expectedRr2 + " (VRC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region DeleteElementsFromCollection
        private TestResult DeleteElementsFromCollection()
        {
            Status("DeleteElementsFromCollection");

            TestResult res1 = DeleteElementsFromCollectionIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = DeleteElementsFromCollectionVRC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult DeleteElementsFromCollectionIList()
        {
            Status("DeleteElementsFromCollectionIList");

            // test Remove(object obj)
            ((IList)_ruleCollectionIList).Remove(_evenRule2); // order should be rr1, er1, rr2
            TestResult resultVerifyElement1 = VerifyElementIList(3, 2, _rangeRule2, "remove er2, checking rr2 (IList)");
            if (resultVerifyElement1 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement2 = VerifyElementIList(3, 1, _evenRule1, "remove er2, checking er1 (IList)");
            if (resultVerifyElement2 != TestResult.Pass) { return TestResult.Fail; }

            // test RemoveAt(int index)
            ((IList)_ruleCollectionIList).RemoveAt(1); // order should be rr1, rr2
            TestResult resultVerifyElement3 = VerifyElementIList(2, 1, _rangeRule2, "remove at index 1, checking rr2 (IList)");
            if (resultVerifyElement3 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement4 = VerifyElementIList(2, 0, _rangeRule1, "remove at index 1, checking rr1 (IList)");
            if (resultVerifyElement4 != TestResult.Pass) { return TestResult.Fail; }

            // test Clear()
            ((IList)_ruleCollectionIList).Clear();
            if (((IList)_ruleCollectionIList).Count != 0)
            {
                LogComment("Fail - collection should have count 0, instead it has count " + ((IList)_ruleCollectionIList).Count + " (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult DeleteElementsFromCollectionVRC()
        {
            Status("DeleteElementsFromCollectionVRC");

            // test Remove(ValidationRule validationRule)
            _ruleCollectionVRC.Remove(_evenRule2); // order should be rr1, er1, rr2
            TestResult resultVerifyElement1 = VerifyElementVRC(3, 2, _rangeRule2, "remove er2, checking rr2 (VRC)");
            if (resultVerifyElement1 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement2 = VerifyElementVRC(3, 1, _evenRule1, "remove er2, checking er1 (VRC)");
            if (resultVerifyElement2 != TestResult.Pass) { return TestResult.Fail; }

            // test RemoveAt(int index)
            _ruleCollectionVRC.RemoveAt(1); // order should be rr1, rr2
            TestResult resultVerifyElement3 = VerifyElementVRC(2, 1, _rangeRule2, "remove at index 1, checking rr2 (VRC)");
            if (resultVerifyElement3 != TestResult.Pass) { return TestResult.Fail; }
            TestResult resultVerifyElement4 = VerifyElementVRC(2, 0, _rangeRule1, "remove at index 1, checking rr1 (VRC)");
            if (resultVerifyElement4 != TestResult.Pass) { return TestResult.Fail; }

            // test Clear()
            _ruleCollectionVRC.Clear();
            if (_ruleCollectionVRC.Count != 0)
            {
                LogComment("Fail - collection should have count 0, instead it has count " + _ruleCollectionVRC.Count + "  (VRC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region CopyToArray
        private TestResult CopyToArray()
        {
            Status("CopyToArray");

            TestResult res1 = CopyToArrayIList();
            if (res1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            TestResult res2 = CopyToArrayVRC();
            if (res2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult CopyToArrayIList()
        {
            Status("CopyToArrayIList");

            ((IList)_ruleCollectionIList).Add(_rangeRule1);
            ((IList)_ruleCollectionIList).Add(_rangeRule2);
            ((IList)_ruleCollectionIList).Add(_evenRule1);
            ((IList)_ruleCollectionIList).Add(_evenRule2);

            ValidationRule[] arrayRules = new ValidationRule[5];

            ((IList)_ruleCollectionIList).CopyTo(arrayRules, 1);
            
            if (arrayRules[0] != null)
            {
                LogComment("Fail - arrayRules[0] not as expected (IList)");
                return TestResult.Fail;
            }
            if (arrayRules[1] != _rangeRule1)
            {
                LogComment("Fail - arrayRules[1] not as expected (IList)");
                return TestResult.Fail;
            }
            if (arrayRules[2] != _rangeRule2)
            {
                LogComment("Fail - arrayRules[2] not as expected (IList)");
                return TestResult.Fail;
            }
            if (arrayRules[3] != _evenRule1)
            {
                LogComment("Fail - arrayRules[3] not as expected (IList)");
                return TestResult.Fail;
            }
            if (arrayRules[4] != _evenRule2)
            {
                LogComment("Fail - arrayRules[4] not as expected (IList)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult CopyToArrayVRC()
        {
            Status("CopyToArrayVRC");

            _ruleCollectionVRC.Add(_rangeRule1);
            _ruleCollectionVRC.Add(_rangeRule2);
            _ruleCollectionVRC.Add(_evenRule1);
            _ruleCollectionVRC.Add(_evenRule2);

            ValidationRule[] arrayRules = new ValidationRule[5];

            _ruleCollectionVRC.CopyTo(arrayRules, 1);
            
            if (arrayRules[0] != null)
            {
                LogComment("Fail - arrayRules[0] not as expected (VRC)");
                return TestResult.Fail;
            }
            if (arrayRules[1] != _rangeRule1)
            {
                LogComment("Fail - arrayRules[1] not as expected (VRC)");
                return TestResult.Fail;
            }
            if (arrayRules[2] != _rangeRule2)
            {
                LogComment("Fail - arrayRules[2] not as expected (VRC)");
                return TestResult.Fail;
            }
            if (arrayRules[3] != _evenRule1)
            {
                LogComment("Fail - arrayRules[3] not as expected (VRC)");
                return TestResult.Fail;
            }
            if (arrayRules[4] != _evenRule2)
            {
                LogComment("Fail - arrayRules[4] not as expected (VRC)");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        private TestResult TestCollectionGetters()
        {
            Status("TestCollectionGetters");

            bool isSynchronized = ((ICollection)_ruleCollectionVRC).IsSynchronized;
            if (isSynchronized != false)
            {
                LogComment("Fail - Collection should not be synchronized (thread safe) by default");
                return TestResult.Fail;
            }

            // SyncRoot is used to synchronize access to ICollection
            object obj = ((ICollection)_ruleCollectionVRC).SyncRoot;
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

            bool isFixedSize = ((IList)_ruleCollectionVRC).IsFixedSize;
            if (isFixedSize != false)
            {
                LogComment("Fail - IsFixedSize should be false");
                return TestResult.Fail;
            }

            bool isReadOnly = ((IList)_ruleCollectionVRC).IsReadOnly;
            if (isReadOnly != false)
            {
                LogComment("Fail - IsReadOnly should be false");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #region AuxMethods
        private TestResult VerifyElementVRC(int expectedCount, int index, ValidationRule expectedRule, string step)
        {
            Status("VerifyElementVRC - " + step);

            if (_ruleCollectionVRC.Count != expectedCount)
            {
                LogComment("Fail - actual number of elements in list:" + _ruleCollectionVRC.Count + ", expected:" + expectedCount + " - " + step);
                return TestResult.Fail;
            }
            ValidationRule rule = _ruleCollectionVRC[index] as ValidationRule;
            if (rule == null)
            {
                LogComment("Fail - could not cast expectedRule in index " + index + " to ValidationRule - " + step);
                return TestResult.Fail;
            }
            if (rule != expectedRule)
            {
                LogComment("Fail - expectedRule is not as expected - " + step);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult VerifyElementIList(int expectedCount, int index, ValidationRule expectedRule, string step)
        {
            Status("VerifyElementIList");

            if (((IList)_ruleCollectionIList).Count != expectedCount)
            {
                LogComment("Fail - actual number of elements in list:" + ((IList)_ruleCollectionIList).Count + ", expected:" + expectedCount + " - " + step);
                return TestResult.Fail;
            }
            ValidationRule rule = ((IList)_ruleCollectionIList)[index] as ValidationRule;
            if (rule == null)
            {
                LogComment("Fail - could not cast expectedRule in index " + index + " to ValidationRule - " + step);
                return TestResult.Fail;
            }
            if (rule != expectedRule)
            {
                LogComment("Fail - expectedRule is not as expected - " + step);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion
    }
}
