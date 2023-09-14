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
    /// A class to test IdScope: Check illegal parameter for methods: Register, Unregister 
    /// </summary>
    [Test(1, "IdTest.Boundary", "ElementIdTestCase1")]
    public class ElementIdTestCase1 : IdTestBaseCase
    {
        #region
        private string _testName = null;
        #endregion


        #region Constructor
        [Variation("RegisterNull")]
        [Variation("UnRegisterNull")]
        [Variation("UnRegisterNotExist")]
        [Variation("RegisterEmptyString")]
        [Variation("UnRegisterEmptyString")]
        [Variation("RegisterTwice")]

        /******************************************************************************
        * Function:          ElementIdTestCase1 Constructor
        ******************************************************************************/
        public ElementIdTestCase1(string arg)
        {
            _testName = arg;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResul.Pass</returns>
        TestResult Initialize()
        {
            CreateTree();

            return TestResult.Pass;
        }

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
                case "RegisterNull":
                    RegisterNull();
                    break;
                case "UnRegisterNull":
                    UnRegisterNull();
                    break;
                case "UnRegisterNotExist":
                    UnRegisterNotExist();
                    break;
                case "RegisterEmptyString":
                    RegisterEmptyString();
                    break;
                case "UnRegisterEmptyString":
                    UnRegisterEmptyString();
                    break;
                case "RegisterTwice":
                    RegisterTwice();
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
        /// Feed Register method with null Id.
        /// </summary>
        public void RegisterNull()
        {
            GlobalLog.LogStatus("Register null as an Id.");
            bool catchIt = false;

            try
            {
                RegisterName(null, _canvas, _win);
            }
            catch (ArgumentNullException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }

            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentNullException.");
            }
        }

        /// <summary>
        /// Unregister an null Id.
        /// </summary>
        public void UnRegisterNull()
        {
            GlobalLog.LogStatus("UnRegister null as an Id.");
            bool catchIt = false;

            try
            {
                UnregisterName(null, _win);
            }
            catch (ArgumentNullException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }

            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentNullException.");
            }
        }

        /// <summary>
        /// Unregister an Id not existed.
        /// </summary>
        public void UnRegisterNotExist()
        {
            GlobalLog.LogStatus("UnRegister an Id not exist.");
            bool catchIt = false;

            try
            {
                UnregisterName("NotExitedId", _win);
            }
            catch (ArgumentException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }

            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentNullException.");
            }
        }

        /// <summary>
        /// Register an Id which is an empty string.
        /// </summary>
        public void RegisterEmptyString()
        {
            GlobalLog.LogStatus("Register Empty String as an Id.");
            bool catchIt = false;

            try
            {
                RegisterName("", _canvas, _win);
            }
            catch (ArgumentException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }

            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentException.");
            }
        }

        /// <summary>
        /// UnRegister Id which is an empty string.
        /// </summary>
        public void UnRegisterEmptyString()
        {
            GlobalLog.LogStatus("UnRegister Empty String as an Id.");
            bool catchIt = false;

            try
            {
                UnregisterName("", _win);
            }
            catch (ArgumentException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }

            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentException.");
            }
        }

        /// <summary>
        /// Register duplication Id.
        /// </summary>
        public void RegisterTwice()
        {
            GlobalLog.LogStatus("Register duplicate Id.");
            bool catchIt = false;

            RegisterName("element", _fe, _win);
            try
            {
                RegisterName("element", _canvas, _win);
            }
            catch (ArgumentException e)
            {
                catchIt = true;
                GlobalLog.LogStatus("Catch exception: " + e.Message);
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Wrong type of exception, type: " + e.GetType().ToString() + " Message: " + e.Message);
            }
            finally
            {
                UnregisterName("element", _win);
            }
            if (!catchIt)
            {
                throw new Microsoft.Test.TestValidationException("Should have throw an exception of type ArgumentException.");
            }
        }
        #endregion
    }
}
