// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Collections;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// This class has only one static method called Verify.
    /// For the given Xaml file, it gets the master error data from the 
    /// error data file provided, and verifies that the actual error thrown
    /// matches the master data.
    /// </summary>
    public class ErrorVerifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xamlfile"></param>
        /// <param name="errorDatafile"></param>
        /// <returns>True if the actual error matches expected error. 
        /// False otherwise 
        /// </returns>
        public static bool Verify(string xamlfile, string errorDatafile)
        {
            XamlLoadDataManager dataManager = new XamlLoadDataManager(errorDatafile);
            TestExecutor testExecutor = new XamlLoadTestExecutor();

            ErrComparer errComparer = new ReportingErrComparer();

            Hashtable expectedErrData = null;
            Hashtable actualErrData = null;

            if (null != (expectedErrData = dataManager.GetRecord(xamlfile)))
            {
                actualErrData = testExecutor.Run(expectedErrData);
                bool errorsSame = false;
                errComparer.Run(expectedErrData, actualErrData, null, ref errorsSame);
                return errorsSame;
            }
            else
            {
                return false;
            }
        }
    }
}
