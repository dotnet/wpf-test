// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------------
//
//
// Description: Test Framework for Document Reading Platform 
// 
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Globalization;

namespace DRT
{
    /// <summary>
    /// A simple hashtable of data for test cases.
    /// </summary>
    public class TestContext
    {
        #region Public Methods
        /// <summary>
        /// Return the existing value or null for a given name.
        /// 
        /// Case Insensitive
        /// </summary>
        /// <param name="name">Parameter name (key).</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>True (existing code needes this)</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        public bool GetValue(string name, out object value)
        {
            name = name.ToLower(CultureInfo.CurrentCulture);
            if (ht.Contains(name))
            {
                value = ht[name];
                return true;
            }
            value = null;
            return true;
        }

        /// <summary>
        /// Set the value for a given name.
        /// 
        /// Case Insensitive
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue(string name, object value)
        {
            ht[name.ToLower(CultureInfo.CurrentCulture)] = value;
        }
        #endregion

        #region Private Fields
        private Hashtable ht = new Hashtable();
        #endregion
    }
}
