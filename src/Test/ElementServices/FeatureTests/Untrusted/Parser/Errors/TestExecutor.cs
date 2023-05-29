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
    /// TestExecutor executes the test, catches errors and returns error data in the form
    /// of a Hashtable (key-value pairs).
    /// </summary>
    public abstract class TestExecutor
    {
        /// <summary>
        /// 
        /// </summary>
        protected TestExecutor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedErrData"></param>
        /// <returns></returns>
        public abstract Hashtable Run(Hashtable expectedErrData);
    }
}
