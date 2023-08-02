//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Xml;

using Microsoft.Test.Logging;

using Avalon.Test.ComponentModel.Utilities;

#endregion

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// AbstractUnitTest makes writing unit test cases more convenient
    /// </summary>
    /// <typeparam name="TestObjectType">the type of test object that derived classes really want</typeparam>
    public abstract class AssertUnitTestBase<TestObjectType> : IUnitTest
    {
        private object testObject;
        private XmlElement variation;

        /// <summary>
        /// define skeleton of IUnitTest
        /// it is implemented with IUnitTest.Perform() style to prevent overriding 
        /// and avoid name pollution in its derived classes 
        /// </summary>
        TestResult IUnitTest.Perform(object testElement, XmlElement variation)
        {
            testObject = testElement;
            this.variation = variation;
            return Assert.RunTest(Run) ? TestResult.Pass : TestResult.Fail;
        }

        /// <summary>
        /// the real test code here.
        /// derived classes should implement this method.
        /// it does not return anything. you should use Assert.Assert*() 
        /// or Assert.Fail() helper functions inside to indicate a failure. 
        /// </summary>
        /// <example>
        /// <code>
        /// protected override void Run(Button bt, XmlElement variation)
        /// {
        ///     Assert.AssertEqual("add didn't work", 5, 3 + 2);
        /// }
        /// </code>
        /// </example>
        /// <param name="testObject">the object to be tested</param>
        /// <param name="variation">xml elemtn for VARIATION</param>
        protected abstract void Run(TestObjectType testObject, XmlElement variation);

        private void Run()
        {
            Assert.AssertTrue("Incorrect test object type: Expected type=[" + typeof(TestObjectType).Name
                    + "]; Actual type=[" + testObject.GetType().Name + "]",
                    typeof(TestObjectType).IsAssignableFrom(testObject.GetType()));
            Run((TestObjectType)testObject, variation);
        }
    }

}
