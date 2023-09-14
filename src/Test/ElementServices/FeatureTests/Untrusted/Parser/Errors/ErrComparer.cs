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
    /// ErrComparer compares expected error data and actual error data (both
    /// of these are in the form of Hashtables, i.e. key-value pairs) and returns 2 things:
    /// 1. Whether they are the same
    /// 2. Whether the expected error data (master) should be changed to match 
    /// the actual error data. It may ask the user for this decision.
    /// </summary>
    public abstract class ErrComparer
    {
        /// <summary>
        /// Main routine that performs the functions mentioned in the summary for this class.
        /// </summary>
        /// <param name="expectedErrData">Expected error data</param>
        /// <param name="actualErrData">Actual error data</param>
        /// <param name="otherArgs">Other misc. arguments that may be necessary</param>
        /// <param name="errorsSame">Reference parameter: returns whether actual and expected are same</param>
        /// <returns>Whether expected data should be changed to match actual data.</returns>
        public abstract bool Run(Hashtable expectedErrData, Hashtable actualErrData, object otherArgs, ref bool errorsSame);

        /// <summary>
        /// Compares 2 Hashtables, with the assumption that the keys and values are strings.
        /// 
        /// If the 2 hashtables are different, it returns the differing key as an out param.
        /// If they are same, the differing key is null.
        /// </summary>
        /// <param name="tbl1"></param>
        /// <param name="tbl2"></param>
        /// <param name="differingKey"></param>
        /// <returns></returns>
        protected static bool SameHashtables(Hashtable tbl1, Hashtable tbl2, out Object differingKey)
        {
            Hashtable table1 = tbl1.Clone() as Hashtable;
            Hashtable table2 = tbl2.Clone() as Hashtable;
            differingKey = null;

            foreach (string key in table1.Keys)
            {
                // Two tables have to contain the exact same keys
                if (!table2.Contains(key))
                {
                    differingKey = key;
                    return false;
                }
                else
                {
                    // Values for those keys need to be exactly same
                    if ((table1[key] as string) != (table2[key] as string))
                    {
                        differingKey = key;
                        return false;
                    }
                    table2.Remove(key);
                }
            }

            if (table2.Count > 0)
            {
                IEnumerator table2Keys = table2.Keys.GetEnumerator();
                table2Keys.MoveNext();
                differingKey = table2Keys.Current;
                return false;
            }

            // If nothing fails, return true
            return true;
        }
    }
}
