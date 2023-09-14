// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Base class helper for all the test case that target only
    /// WindowsBase.dll
    /// This class should not contain any references to any other dll
    /// on Avalon. Only WindowsBase.dll
    /// </summary>
    abstract public class TestCaseCommon : AvalonTest
    {

        /// <summary>
        /// Static constructor that creates the Array 
        /// </summary>
        static TestCaseCommon()
        {
            ExceptionList = new List<Exception>();
        }


        /// <summary>
        ///  
        /// </summary>
        protected virtual void FinalReportFailure()
        {   
            // Walk throw the exception list to find out if there is any
            // exception that happen in there. 
            if (ExceptionList != null && ExceptionList.Count > 0)
            {
                // If we found an exception we write to the logger
                // you that information is store on the log file
                for (int i=0;i<ExceptionList.Count;i++)
                {
                    Exception e = ExceptionList[i];
                    CoreLogger.LogStatus("Exception #" + i.ToString());
                    CoreLogger.LogStatus(e.Message.ToString());
                    if (e.StackTrace != null)
                        CoreLogger.LogStatus(e.StackTrace.ToString());
                }


                this.TestCaseFailed = true;

                // To make if easy for debugging, we throw the first exception
                // that we found on the exception list
                throw ExceptionList[0];
            }

            if (IsTestCaseFail)
                CoreLogger.LogStatus("Test case fail but there is no more information.");

        }


        /// <summary>
        /// This is very useful if you the meaning for Fail is different as
        /// the one that is by default
        /// </summary>
        protected virtual bool IsTestCaseFail
        {
            get
            {
                return TestCaseFailed;
            }               
        }

        /// <summary>
        /// Helper holder to mark if the test case fail or passed
        /// </summary>
        protected bool TestCaseFailed = false;

        
        /// <summary>
        /// Provides a Reference to the main Dispatcher, as a helper.
        /// </summary>
        protected Dispatcher MainDispatcher;

        /// <summary>
        /// Provides a place holder for any Exception that my occur and
        /// you want to store it. This is very useful on multiple
        /// thread test cases.
        /// </summary>
        static protected List<Exception> ExceptionList = null;

    }

}
