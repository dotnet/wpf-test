// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshDependencyObjectTypeTest
{
    /******************************************************************************
    * CLASS:          TestDependencyObjectType
    ******************************************************************************/
    [Test(0, "PropertyEngine.DependencyObject", TestCaseSecurityLevel.FullTrust, "TestDependencyObjectType")]
    public class TestDependencyObjectType : AvalonTest
    {
        #region Private Data
        private MenuItem _menuItem = null;
        private DependencyObjectType _dType1,_dType2,_dType3,_dType4;
        private string _testName = "";
        private bool _testPassed = false;
        #endregion


        #region Constructor
 
        [Variation("TestFromSystemType")]
        [Variation("TestIsInstanceOfType")]
        [Variation("TestIsSubclassOf")]
        [Variation("TestGetHashCode")]

        /******************************************************************************
        * Function:          TestDependencyObjectType Constructor
        ******************************************************************************/
        public TestDependencyObjectType(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "TestFromSystemType":
                    TestFromSystemType();
                    break;
                case "TestIsInstanceOfType":
                    TestIsInstanceOfType();
                    break;
                case "TestIsSubclassOf":
                    TestIsSubclassOf();
                    break;
                case "TestGetHashCode":
                    TestGetHashCode();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          TestFromSystemType
        ******************************************************************************/
        ///<summary>
        ///API Testing for
        ///public static DependencyObjectType DependencyObjectType.FromSystemType(Type systemType);
        ///</summary>
        public void TestFromSystemType()
        {
            DependencyObjectType dType0, dType1, dType2, dType3, dType4;

            Utilities.PrintTitle("Test API DependnencyObjectType.FromSystemType(Type systemType)");

            //---------- 1 ---------- 
            Utilities.PrintStatus("(1) ArgumentNullException for Null Argument.");
            try
            {
                dType0 = DependencyObjectType.FromSystemType(null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            //---------- 2 ---------- 
            Utilities.PrintStatus("(2) ArgumentException for DTypeNotSupportedForSystemType.");
            try
            {
                dType0 = DependencyObjectType.FromSystemType(typeof(string));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            //---------- 3 ---------- 
            Utilities.PrintStatus("(3) Get DType for DependencyObject. Internally it may or may not stored.");
            dType0 = DependencyObjectType.FromSystemType(typeof(DependencyObject));
            Utilities.PrintDependencyObjectType(dType0);
            Utilities.Assert(dType0.SystemType == typeof(DependencyObject), "dType0.SystemType == typeof(DependencyObject)");

            //---------- 4 ---------- 
            Utilities.PrintStatus("(4) Get DType from type TestDoD3, which is derived from TestDoD2, from TestDoD1, from DependencyObject");
            dType1 = DependencyObjectType.FromSystemType(typeof(TestDoD3));
            Utilities.PrintDependencyObjectType(dType1);
            Utilities.Assert(dType1.SystemType == typeof(TestDoD3), "dType1.SystemType == typeof(TestDoD3)");

            //---------- 5 ---------- 
            Utilities.PrintStatus("(5) Again Get DType from type TestDoD3. This time it must be stored locally already");
            dType2 = DependencyObjectType.FromSystemType(typeof(TestDoD3));
            Utilities.PrintDependencyObjectType(dType2);
            Utilities.Assert(dType1 == dType2, "dType1 == dType2");

            //---------- 6 ---------- 
            Utilities.PrintStatus("(6) TestDoD2 already has its DType created");
            dType3 = DependencyObjectType.FromSystemType(typeof(TestDoD2));
            Utilities.PrintDependencyObjectType(dType3);
            Utilities.Assert(dType3 == dType2.BaseType, "dType3 == dType2.BaseType");

            //---------- 7 ---------- 
            Utilities.PrintStatus("(7) Imteresting to Note: DependencyObjectType for DependencyObject is always the first instance to create.");
            dType4 = DependencyObjectType.FromSystemType(typeof(DependencyObject));
            Utilities.PrintDependencyObjectType(dType4);
            Utilities.Assert(dType4.Id == 0, "dType4.Name == 0");

            //Any failures are captured by the Asserts above.
            _testPassed = true;
        }

        /******************************************************************************
        * Function:          TestIsInstanceOfType
        ******************************************************************************/
        ///<summary>
        ///API Testing
        ///public bool DependencyObjectType.IsInstanceOfType(DependencyObject d)
        ///</summary>
        public void TestIsInstanceOfType()
        {
            Utilities.PrintTitle("Test API DependnencyObjectType.IsInstanceOf(DependencyObject d). Avalon Types are Used");
            CreateMenuItem(null);

            //---------- 1 ---------- 
            Utilities.PrintStatus("(1) DependencyObjectType.IsInstanceOfType should return false for null argument ");
            Utilities.Assert(false == _dType3.IsInstanceOfType(null), "false == dType3.IsInstanceOfType(null)");

            //---------- 2 ---------- 
            Utilities.PrintStatus("(2) true since object is of the same DType");
            Utilities.Assert(_dType3.IsInstanceOfType(_menuItem), "dType3.IsInstanceOfType(menuItem)");

            //---------- 3 ---------- 
            Utilities.PrintStatus("(3) true since object is in the inheritance tree of DType");
            Utilities.Assert(_dType2.IsInstanceOfType(_menuItem), "dType2.IsInstanceOfType(menuItem)");
            Utilities.Assert(_dType1.IsInstanceOfType(_menuItem), "dType1.IsInstanceOfType(menuItem)");

            //---------- 4 ---------- 
            Utilities.PrintStatus("(4) false otherwise");
            Utilities.Assert(!_dType4.IsInstanceOfType(_menuItem), "!dType4.IsInstanceOfType(menuItem)");

            //Any failures are captured by the Asserts above.
            _testPassed = true;
        }



        /******************************************************************************
        * Function:          TestIsSubclassOf
        ******************************************************************************/
        ///<summary>
        ///API Testing
        /// public bool DependencyObjectType.IsSubclassOf(DependencyObjectType dependencyObjectType)
        ///</summary>
        public void TestIsSubclassOf()
        {
            DependencyObjectType dType1, dType2, dType3, dType4;

            dType1 = DependencyObjectType.FromSystemType(typeof(Visual));
            dType2 = DependencyObjectType.FromSystemType(typeof(FrameworkElement));
            dType3 = DependencyObjectType.FromSystemType(typeof(MenuItem));
            dType4 = DependencyObjectType.FromSystemType(typeof(Button));
            Utilities.PrintTitle("Test API DependnencyObjectType.IsSubclassOf(DependencyObjectType dType)");

            //---------- 1 ---------- 
            Utilities.PrintStatus("(1) gets True");
            Utilities.Assert(dType3.IsSubclassOf(dType2), "dType3.IsSubclassOf(dType2)");
            Utilities.Assert(dType4.IsSubclassOf(dType1), "dType4.IsSubclassOf(dType1)");

            //---------- 2 ----------
            Utilities.PrintStatus("(2) False when compared with itself");
            Utilities.Assert(!dType4.IsSubclassOf(dType4), "!dType4.IsSubclassOf(dType4)");

            //---------- 3 ---------
            Utilities.PrintStatus("(3) False in other case");
            Utilities.Assert(!dType4.IsSubclassOf(dType3), "!dType4.IsSubclassOf(dType3)");

            //---------- 4 ----------
            Utilities.PrintStatus("(4) False with null in argument");
            Utilities.Assert(!dType1.IsSubclassOf(null), "!dType1.IsSubclassOf(null)");

            //Any failures are captured by the Asserts above.
            _testPassed = true;
        }

        ///<summary>
        ///API Testing
        ///public int DependnencyObjectType.GetHashCode()
        ///</summary>
        public void TestGetHashCode()
        {
            DependencyObjectType dType1;

            //---------- 1 ----------
            dType1 = DependencyObjectType.FromSystemType(typeof(MenuItem));
            Utilities.PrintTitle("Test API DependnencyObjectType.GetHashCode(), internally it is its Name");
            Utilities.PrintDependencyObjectType(dType1);
            Utilities.Assert(dType1.Id == dType1.GetHashCode(), "dType1.Name == dType1.GetHashCode()");

            //Any failures are captured by the Asserts above.
            _testPassed = true;
        }
        #endregion


        #region Private Members        
        /******************************************************************************
        * Function:          CreateMenuItem
        ******************************************************************************/
        private object CreateMenuItem(object arj)
        {
            _dType1 = DependencyObjectType.FromSystemType(typeof(Visual));
            _dType2 = DependencyObjectType.FromSystemType(typeof(FrameworkElement));
            _dType3 = DependencyObjectType.FromSystemType(typeof(MenuItem));
            _dType4 = DependencyObjectType.FromSystemType(typeof(Button));

            _menuItem = new MenuItem();
            return null;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestDoD1
    ******************************************************************************/
    /// <summary>
    /// TestDoD1 is derived from DependencyObject
    /// </summary>
    public class TestDoD1 : DependencyObject
    {
        /// <summary>
        /// Constructor of TestDoD1
        /// </summary>
        /// <param name="tag">the tag</param>
        public TestDoD1(string tag)
        {
            this._tag = tag;
        }

        private string _tag;
    }


    /******************************************************************************
    * CLASS:          TestDoD2
    ******************************************************************************/
    /// <summary>
    /// TestDoD2 is derived from TestDoD1
    /// </summary>
    public class TestDoD2 : TestDoD1
    {
        /// <summary>
        /// TestDoD2 constructor
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <param name="secret">its secret</param>
        public TestDoD2(string tag, int secret)
            : base(tag)
        {
            this._secret = secret;
        }

        private int _secret;
    }


    /******************************************************************************
    * CLASS:          TestDoD3
    ******************************************************************************/
    /// <summary>
    /// TestDoD3 is derived from TestDoD2
    /// </summary>
    public class TestDoD3 : TestDoD2
    {
        /// <summary>
        /// TestDoD3 Constructor
        /// </summary>
        /// <param name="tag">The Tag</param>
        /// <param name="secret">its secret</param>
        /// <param name="state">its state</param>
        public TestDoD3(string tag, int secret, string state)
            : base(tag, secret)
        {
            this._state = state;
        }

        private string _state;
    }


    /******************************************************************************
    * CLASS:          TestClass
    ******************************************************************************/
    /// <summary>
    /// A test class (derived from object)
    /// </summary>
    public class TestClass
    {
        /// <summary>
        /// TestClass constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="salary">the salary</param>
        public TestClass(string name, decimal salary)
        {
            this.name = name;
            this.salary = salary;
        }

        /// <summary>
        /// name
        /// </summary>
        protected string name;

        /// <summary>
        /// the salary
        /// </summary>
        protected decimal salary;
    }


    /******************************************************************************
    * CLASS:          TestClassD1
    ******************************************************************************/
    /// <summary>
    /// TestClassD1 is derived from TestClass
    /// </summary>
    public class TestClassD1 : TestClass
    {
        /// <summary>
        /// Constructor of TestClassD1
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="salary">the salary</param>
        /// <param name="building">the building</param>
        public TestClassD1(string name, decimal salary, int building)
            : base(name, salary)
        {
            this.building = building;
        }

        /// <summary>
        /// The building
        /// </summary>
        protected int building;
    }


    /******************************************************************************
    * CLASS:          TestClassD2
    ******************************************************************************/
    /// <summary>
    /// TestClassD2 is derived from TestClassD1
    /// </summary>
    public class TestClassD2 : TestClassD1
    {
        /// <summary>
        /// Constructor of TestClassD2
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="salary">the salary</param>
        /// <param name="building">building</param>
        public TestClassD2(string name, decimal salary, int building)
            : base(name, salary, building)
        {
            bool shouldReview = false;

            if (this.salary > 1000)
            {
                shouldReview = true;
            }

            this.toReview = shouldReview;
        }

        /// <summary>
        /// to review or not
        /// </summary>
        protected bool toReview;
    }
}
