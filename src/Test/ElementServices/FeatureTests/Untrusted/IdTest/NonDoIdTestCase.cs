// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows;

using Avalon.Test.CoreUI;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// A class to test IdScope. Actions:
    ///     Register an Id with and Non-DO Element.
    ///     Find the element with Id.
    ///     Unregister.
    ///     Find it again. 
    /// </summary>
    [Test(0, "IdTest", "NonDoIdTestCase")]
    public class NonDoIdTestCase : IdTestBaseCase
    {
        #region Constructor
        public NonDoIdTestCase()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(BasicAction);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResul.Pass</returns>
        TestResult Initialize()
        {
            CreateTree();

            return TestResult.Pass;
        }

        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult BasicAction()
        {
            GlobalLog.LogStatus("register and unregister Non-Do object.");

            object obj = new object();
            //register a Id with the button in _win
            RegisterName("element", obj, _win);

            //find the element from button
            object foundObj = (object)FindElementWithId(_win, "element");
            if (null == foundObj)
                throw new Microsoft.Test.TestValidationException("Cannot find object.");

            if (obj != foundObj)
                throw new Microsoft.Test.TestValidationException("object found is not correct");

            //unregiser id
            UnregisterName("element", _win);

            //Try to find again, should not find it.
            try
            {
                foundObj = (object)FindElementWithId(_win, "element");
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Should be no exception: " + e.Message);
            }

            if (null != foundObj)
            {
                throw new Microsoft.Test.TestValidationException("Should not have found object because it has been removed from Id Scope");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
