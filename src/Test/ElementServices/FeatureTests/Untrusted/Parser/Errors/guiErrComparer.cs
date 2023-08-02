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
    /// GUIErrComparer displays the expected and actual errors in a user interface
    /// and asks the user whether expected error data (master) should be changed
    /// to match the actual error data.
    /// </summary>
    public class GUIErrComparer : ErrComparer
    {
        /// <summary>
        /// Override of parent's method
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
                System.Windows.Forms.DialogResult dialogResult = new GUIErrComparerDialog(expectedErrData, actualErrData, filename).ShowDialog();
                return (System.Windows.Forms.DialogResult.Yes == dialogResult);
            }
            else
            {
                return false;
            }
        }
    }
}
