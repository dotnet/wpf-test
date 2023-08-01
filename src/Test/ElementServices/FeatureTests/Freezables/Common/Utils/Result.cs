// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2007
 *
 *   Program:   Result class
 *   Author:
 *
 ************************************************************/

using System;
using System.Windows;
using System.Collections.Specialized;


namespace Microsoft.Test.ElementServices.Freezables.Utils
{
    /******************************************************************************
    * CLASS:          Result
    ******************************************************************************/
    public class Result
    {
        #region Result

        public Result()
        {
            failures = new StringCollection ();
            passed = true;
        }

        public StringCollection      failures;
        public bool                  passed;

        #endregion
    }
}
