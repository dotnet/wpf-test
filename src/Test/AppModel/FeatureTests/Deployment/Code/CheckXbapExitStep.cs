// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.CrossProcess;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomSteps
{
    /// <summary>
    /// Loader Step that can be used to activate an Avalon application type
    /// </summary>

	public class CheckXbapExitStep : Microsoft.Test.Loaders.LoaderStep
	{
        /// <summary>
        /// Checks the property bag for a URI written by ActivationStep.
        /// My Test Browser apps write a cookie to this URI, which can then be used to verify that
        /// the ExitEventHandler ran.
        /// </summary>
        /// <returns>true</returns>
        public override bool DoStep() 
		{
            string UriToCheck = DictionaryStore.Current["ActivationStepUri"];

            if (UriToCheck == null)
            {
                GlobalLog.LogEvidence("Property bag value \"ActivationStepUri\" was not defined!");
                return false;
            }

            GlobalLog.LogEvidence("  *****  *****  *****");
			GlobalLog.LogEvidence("Checking for presence of Xbap Exit cookie with URL <" + UriToCheck + ">" );
            
            if (System.Windows.Application.GetCookie(new Uri(UriToCheck)).Contains("App ShutDown was successful"))
            {
                GlobalLog.LogEvidence("Success! App's shutdown cookie wrote successfully.");
                TestLog.Current.Result = TestResult.Pass;                
            }
            else
            {
                GlobalLog.LogEvidence("Error Reading App Shutdown cookie: Got " + System.Windows.Application.GetCookie(new Uri(UriToCheck)));
                GlobalLog.LogEvidence("(Expected \"App ShutDown was successful\"" );
                TestLog.Current.Result = TestResult.Fail;
            }
            GlobalLog.LogEvidence("  *****  *****  *****");
            
            return true;
        }

    }


}
