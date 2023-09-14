// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.CoreInput
{
    /******************************************************************************
    * CLASS:          CursorTypeConverterTest
    ******************************************************************************/
    [Test(1, "Input", TestCaseSecurityLevel.FullTrust, "CursorTypeConverterTest")]
    public class CursorTypeConverterTest : AvalonTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          CursorTypeConverterTest Constructor
        ******************************************************************************/
        public CursorTypeConverterTest()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// "Verify basic behavior of CursorTypeConverter."
        /// </summary>
        TestResult StartTest()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Cursor));

            //
            // Round trip values returned by GetStandardValues().
            //

            if (!converter.GetStandardValuesSupported(null)) throw new Microsoft.Test.TestValidationException("Failed");

            CoreLogger.LogStatus("Round-tripping cursors returned by GetStandardValues()...");

            foreach (object obj in converter.GetStandardValues(null))
            {
                Cursor cursor = (Cursor)obj;

                CoreLogger.LogStatus("\t" + cursor.ToString());

                if (!converter.CanConvertTo(null, typeof(string))) throw new Microsoft.Test.TestValidationException("Failed");

                string str = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, obj, typeof(string));

                if (!converter.CanConvertFrom(null, typeof(string))) throw new Microsoft.Test.TestValidationException("Failed");

                Cursor cursor2 = (Cursor)converter.ConvertFrom(null, CultureInfo.InvariantCulture, str);

                if (!cursor.Equals(cursor2)) throw new Microsoft.Test.TestValidationException("Failed");
            }

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
