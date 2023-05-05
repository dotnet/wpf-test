// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Reflection;

    class CustomAttributeProvider : ICustomAttributeProvider
    {
        private ICustomAttributeProvider _wrappedImplementation;

        public CustomAttributeProvider(ICustomAttributeProvider wrappedImplementation)
        {
            this._wrappedImplementation = wrappedImplementation;
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return _wrappedImplementation.GetCustomAttributes(inherit);
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _wrappedImplementation.GetCustomAttributes(attributeType, inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return _wrappedImplementation.IsDefined(attributeType, inherit);
        }
    }
}
