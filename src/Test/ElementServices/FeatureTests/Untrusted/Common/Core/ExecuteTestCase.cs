// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.IO;

using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Windows.Forms; // For Screen class
using System.Drawing;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Parser;



namespace Avalon.Test.CoreUI
{
    /// <summary>
    ///     
    ///</summary>
    /// <remarks>
    ///         <ol>Scenarios steps:
    ///         <li>The main thread creates 5 thread that it will execute these:</li>
    ///         </ol>
    /// </remarks>
    public class ExecuteFileTestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public ExecuteFileTestCase() {}
        

        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        public void ExecuteFile()
        {
            int timeOut;
            string exeFile = "";
            ExeTestCaseInfo testCase;
            string paramsStr = "";

            // Start Data Setup 
			testCase = (ExeTestCaseInfo)TestCaseInfo.GetCurrentInfo();

			TimeSpan timeOutSpan = new TimeSpan(0, 0, CoreTests.Timeout);
            timeOut = (int)timeOutSpan.TotalMilliseconds;

            exeFile = testCase.Exe;
            paramsStr = testCase.Params;
            // End Data Setup

            if (!File.Exists(exeFile))
            {
                throw new Microsoft.Test.TestSetupException("The file " + exeFile + " was not found");
            }

            CoreLogger.LogStatus("File " + exeFile + " found");

            CoreTestsSingleRunServices.RunTestCaseProcess(exeFile,timeOut, paramsStr, true);

            CoreLogger.LogStatus(exeFile + " exited");

    }


    }


}

