// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// A class to test IdScope. Actions:
    ///     Register an Id with and Element.
    ///     Find the element with Id.
    ///     Unregister.
    ///     Find it again. 
    /// </summary>
    [Test(1, "IdTest", "ElementIdTestCase2")]
    public class ElementIdTestCase2 : IdTestBaseCase
    {
        #region
        private string _testName = null;
        #endregion


        #region Constructor
        [Variation("RegisterFrameworkTemplate")]

        public ElementIdTestCase2(string arg)
        {
            _testName = arg;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
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

            // Verify on FrameworkElement.
            VerifyNameRegistration(_fe, _fe);

            // Verify on FrameworkContentElement.
            VerifyNameRegistration(_fce, _fce);

            return TestResult.Pass;
        }

        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "RegisterFrameworkTemplate":
                    RegisterFrameworkTemplate();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Verify basic name registration on FrameworkTemplate.
        /// </summary>
        public void RegisterFrameworkTemplate()
        {
            CreateTree();

            // Verify on FrameworkElement.
            VerifyNameRegistration(_fe, _controlTemplate);

            // Verify on FrameworkContentElement.
            VerifyNameRegistration(_fce, _controlTemplate);
        }
        #endregion


        #region Private Methods
        private void VerifyNameRegistration(object elementToRegister, object whereToRegister)
        {
            GlobalLog.LogStatus("Verifying name registeration of " + elementToRegister.GetType().Name + " on " + whereToRegister.GetType().Name + ".");

            //register a Id with the button in _win
            RegisterName("element", elementToRegister, whereToRegister);

            //find the element from button
            object found = FindElementWithId(whereToRegister, "element");
            if (null == found)
                throw new Microsoft.Test.TestValidationException("Cannot find element.");

            if (elementToRegister != found)
                throw new Microsoft.Test.TestValidationException("Element found is not correct.");

            //unregiser id
            UnregisterName("element", whereToRegister);

            //Try to find again, should not find it.
            try
            {
                found = FindElementWithId(whereToRegister, "element");
            }
            catch (Exception e)
            {
                throw new Microsoft.Test.TestValidationException("Should be no exception.", e);
            }

            if (found != null)
                throw new Microsoft.Test.TestValidationException("Found element by name after unregistering it.");

            //unregister again - should cause exception
            Exception ex = null;
            try { _controlTemplate.UnregisterName("foo"); }
            catch (Exception e) { ex = e; }

            if (ex == null)
                throw new Microsoft.Test.TestValidationException("UnregisterName with bogus key did not cause exception.");
        }
        #endregion
    }
}
