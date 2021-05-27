// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// UIAutomation share test state
    /// </summary>
    public class UiaTestState : MarshalByRefObject
    {
        private Dictionary<object, object> propBag = new Dictionary<object, object>();

        /// <summary>
        /// UIAutomation share state property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[object name]
        {
            get
            {
                //we want it to return null if not set
                object value = null;
                if (propBag.TryGetValue(name, out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            set 
            { 
                propBag[name] = value; 
            }
        }
    }
}
