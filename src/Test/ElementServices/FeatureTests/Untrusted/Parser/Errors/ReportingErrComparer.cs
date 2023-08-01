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

using Avalon.Test.CoreUI.Trusted;
using System;
using System.Collections;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// An ErrComparer for use when user interaction is not possible, such as
    /// when running in an automated test run in a lab.
    /// It just logs whether actual error data matches expected error data. 
    /// </summary>
    public class ReportingErrComparer : ErrComparer
    {
        /// <summary>
        /// Override of parent's method.
        /// </summary>
        /// <param name="expectedErrData"></param>
        /// <param name="actualErrData"></param>
        /// <param name="otherArgs"></param>
        /// <param name="errorsSame"></param>
        /// <returns></returns>
        override public bool Run(Hashtable expectedErrData, Hashtable actualErrData, object otherArgs, ref bool errorsSame)
        {
            // Retrieve and then remove the "XamlFileName" entry from expectedErrData
            string filename = expectedErrData["XamlFileName"] as string;
            expectedErrData.Remove("XamlFileName");

            object differingKey;
            errorsSame = SameHashtables(expectedErrData, actualErrData, out differingKey);
            if (!errorsSame)
            {
                CoreLogger.LogStatus("Actual error differs from expected error for file " + filename);
                CoreLogger.LogStatus("Expected value of '" + differingKey + "' is '" + expectedErrData[differingKey] + "'");
                CoreLogger.LogStatus("Actual value of '" + differingKey + "' is '" + actualErrData[differingKey] + "'");
            }

            // Always return false, indicating that expected error data (master) should not 
            // be changed to match actual error data.
            // This is because this class works without user interaction, hence doesn't have
            // the authority to make decision about changing master.
            return false;
        }
    }
}
