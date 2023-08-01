// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          MouseActionConverterCultureNullApp
    ******************************************************************************/
    // 






    [Test(0, "Commanding.TypeConverter", TestCaseSecurityLevel.FullTrust, "MouseActionConverterCultureNullApp")]
    public class MouseActionConverterCultureNullApp : TestApp
    {
        #region Private Data
        private TypeConverter _mouseActionConverter;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          MouseActionConverterCultureNullApp Constructor
        ******************************************************************************/
        public MouseActionConverterCultureNullApp() :base()
        {
            GlobalLog.LogStatus("In MouseActionConverterCultureNullApp constructor");
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            GlobalLog.LogStatus("Running app...");
            this.RunTestApp();
            GlobalLog.LogStatus("App run!");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            GlobalLog.LogStatus("Getting Converter...");
            
            _mouseActionConverter = TypeDescriptor.GetConverter(typeof(MouseAction));
            Debug.Assert(_mouseActionConverter is MouseActionConverter);
            Debug.Assert(_mouseActionConverter.CanConvertFrom(typeof(String)));

            return null;
        }

        /******************************************************************************
        * Function:          DoValidate
        ******************************************************************************/
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.BeginVariation("MouseActionConverterCultureNullApp");
            GlobalLog.LogStatus("Validating...");

            // Goal: Verify that CultureInfo parameter is ignored by 'MouseActionConverter.ConvertFrom' method,
            //       and that Invariant Culture is always used for internal comparisons.
            // Strategy: Do round-trip conversion

            MouseAction[] actions = (MouseAction[])Enum.GetValues(typeof(MouseAction));

            // Try the required test for null as well as every available culture in case some would
            // be problematic if not ignored by MouseActionConverter. The latter may be overkill since
            // none of the current MouseAction strings cause any String manipulation problem for any 
            // currently available CultureInfo value.
            //
            CultureInfo[] cultures = new CultureInfo[1+ CultureInfo.GetCultures(CultureTypes.AllCultures).GetLength(0)];
            cultures[0] = null;
            CultureInfo.GetCultures(CultureTypes.AllCultures).CopyTo(cultures, 1);
            TestPassed = true;
            try
            {
                foreach (CultureInfo culture in cultures)
                {
                    foreach (MouseAction action in actions)
                    {
                        String actionName = _mouseActionConverter.ConvertToInvariantString(action);
                        if (action != (MouseAction)_mouseActionConverter.ConvertFrom(null, culture, actionName.ToLowerInvariant()))
                        {
                            // log error and exit
                            GlobalLog.LogStatus("MouseActionConverter.ConvertFrom must ignore CultureInfo argument");
                            TestPassed = false;
                            return null; //jump to finally block
                        }
                    }
                }
            }
            catch(ArgumentNullException e)
            {
                // null not accepted as ITypeDescriptorContext or as CultureInfo argument
                GlobalLog.LogStatus("MouseActionConverter.ConvertFrom(...) must accept null ITypeDescriptorContext and/or CultureInfo arguments. " + e.ToString());
                TestPassed = false;                
            }
            finally
            {
                CultureInfo.CurrentCulture.ClearCachedData();
                GlobalLog.LogStatus("Setting log result to " + TestPassed.ToString());
            }

            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();
            return null;
        }
        #endregion
    }
}

