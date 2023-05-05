// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
using System;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// Simple driver class
    /// </summary>
    public abstract class Driver
    {

        /// <summary>
        /// Exception handler
        /// </summary>
        virtual protected void HandleTopLevelException( System.Exception ex )
        {
            // default is not to handle and re-throw
            throw ex;
        }
        /// <summary>
        /// Test Name
        /// </summary>
        virtual public string Name
        {
            // echo the class name as a default
            get{ return this.GetType().Name; }
        }

        /* Reporting interface */

        /// <summary>
        /// Log Method
        /// </summary>
        virtual protected void Log2( string message ) {}
        /// <summary>
        /// Reporting Method
        /// </summary>
        virtual protected void ReportResult( bool result, string comment )  {}
        /// <summary>
        /// Cleanup Method
        /// </summary>
        virtual protected void EndTest()  {}
        
        /// <summary>
        /// Internal exception class for driver issues.
        /// </summary>
        protected class InternalDriverException : System.Exception 
        {
            public InternalDriverException( string message ) : base( message ) {}
        }
    }


    /// <summary>
    /// Common Integration Test Driver implementations
    /// </summary>
    public class IntegrationDriver : Driver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IntegrationDriver() : base()
        {
            // Create reporting structure
            currentResults  = new CategoryFilter();
            currentResults.RegisterCategory(CommonConstants.TestPass);
            currentResults.RegisterCategory(CommonConstants.TestFail);
            currentResults.RegisterCategory(CommonConstants.TestIgnore);
            currentResults.RegisterCategory(CommonConstants.TestInternalFailure);
        }

        /// <summary>
        /// Custom exception handler, logs exception and moves to next step
        /// </summary>
        protected override void HandleTopLevelException( System.Exception ex )
        {
            // just log and continue
            Log2( String.Format( "### Exception Caught: {0} ", ex.ToString() ) );
        }

        /// <summary>
        /// Custom Logger
        /// </summary>
        protected override void Log2( string message )
        {
            LogHelper.LogTestMessage( String.Format( "[{0}]: {1}", this.Name, message ) );
        }
        /// <summary>
        /// Custom Reporter
        /// </summary>
        protected override void ReportResult( bool result, string comment )
        {
            // Print a nice formatted result
            Log2( "### Test breakdown:");
            Log2( "### ===============");
           // foreach( string str in currentResults.FormatCategories(" - ", "; ") ) { Log2(str); }
            LogHelper.LogTest( result, comment );
        }
        /// <summary>
        /// End test override
        /// </summary>
        protected override void EndTest()
        {
            // End the test
            base.EndTest();
            IntegrationUtilities.EndCurrentTest();
        }

        /// <summary>
        /// Category filter for sorting sub test results
        /// </summary>
        protected  CategoryFilter currentResults ;

    }

}




