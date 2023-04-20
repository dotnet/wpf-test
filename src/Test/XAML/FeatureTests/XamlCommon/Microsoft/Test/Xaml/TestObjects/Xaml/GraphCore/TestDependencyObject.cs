// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Test dependency object
    /// </summary>
    [Serializable]
    public class TestDependencyObject : ITestDependencyObject
    {
        /// <summary>
        /// Storage for the properties
        /// </summary>
        private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        /// <summary>
        /// Gets the Properties dictionary
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get
            {
                return _storage;
            }
        }

        /// <summary>
        /// Set a property value
        /// </summary>
        /// <param name="p">name of the property</param>
        /// <param name="value">value of the property</param>
        public virtual void SetValue(XName p, object value)
        {
            if (_storage.Keys.Contains(p.LocalName))
            {
                _storage[p.LocalName] = value;
            }
            else
            {
                _storage.Add(p.LocalName, value);
            }
        }

        /// <summary>
        /// Get a property value
        /// </summary>
        /// <param name="p">name of the property</param>
        /// <returns>value of the property</returns>
        public virtual object GetValue(XName p)
        {
            if (_storage.Keys.Contains(p.LocalName))
            {
                return _storage[p.LocalName];
            }
            else
            {
                return null;
            }
        }
    }
}
