// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml.Schema;
using System.Reflection;

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    public class CustomXamlMemberInvoker : XamlMemberInvoker
    {
        public CustomXamlMemberInvoker(MethodInfo getter, MethodInfo setter)
        {
            _underlyingGetter = getter;
            _underlyingSetter = setter;
        }

        public CustomXamlMemberInvoker(MethodInfo getter, MethodInfo setter, System.Xaml.XamlMember member)
            : base(member)
        {
            _underlyingGetter = getter;
            _underlyingSetter = setter;
        }

        public ErrorType ErrorType { get; set; }

        public override object GetValue(object instance)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return _underlyingGetter.Invoke(instance, new object[] { });
        }

        public override void SetValue(object instance, object value)
        {
            DetermineErrorAction(ErrorType);

            if (_underlyingSetter.IsStatic)
            {
                _underlyingSetter.Invoke(null, new object[] { instance, value });
            }
            else
            {
                _underlyingSetter.Invoke(instance, new object[] { value });
            }
        }

        private MethodInfo _underlyingGetter;
        private MethodInfo _underlyingSetter;

        public static object DetermineErrorAction(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.XamlMemberInvokerReturnsNull:
                    return null;
                case ErrorType.XamlMemberInvokerThrows:
                    throw new NotSupportedException("This is an expected exception thrown in a negative test case.");
                case ErrorType.NoError:
                default:
                    return new object();
            }
        }
    }
}
