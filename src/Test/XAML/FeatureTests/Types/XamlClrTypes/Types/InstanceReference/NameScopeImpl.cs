// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types.InstanceReference
{
    public class NameScopeImpl : INameScope
    {
        private readonly Dictionary<string, Object> _scopedNames = new Dictionary<string, object>();

        #region INameScope Members

        public object FindName(string name)
        {
            // return ScopedNames[name];
            object value;
            _scopedNames.TryGetValue(name, out value);
            return value;
        }

        public void RegisterName(string name, object scopedElement)
        {
            _scopedNames.Add(name, scopedElement);
        }

        public void UnregisterName(string name)
        {
            _scopedNames.Remove(name);
        }

        #endregion
    }
}
