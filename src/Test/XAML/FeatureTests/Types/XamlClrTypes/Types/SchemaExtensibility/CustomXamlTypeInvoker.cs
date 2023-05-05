// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;

    public class CustomXamlTypeInvoker : XamlTypeInvoker
    {
        private Type _underlyingType;
        public CustomXamlTypeInvoker(Type underlyingType)
        {
            if (underlyingType == null)
            {
                throw new ArgumentNullException("underlyingType");
            }

            this._underlyingType = underlyingType;
            this.ErrorType = ErrorType.NoError;
        }

        public ErrorType ErrorType { get; set; }

        public override void AddToCollection(object instance, object item)
        {
            DetermineErrorAction(ErrorType);
            GetAddMethod(null).Invoke(instance, new object[] {item});
        }

        public override object CreateInstance(object[] arguments)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            
            return SupportedTypeInfo.CreateInstance(_underlyingType);
        }

        public override void AddToDictionary(object instance, object key, object item)
        {
            DetermineErrorAction(ErrorType);
            GetAddMethod(null).Invoke(instance, new object[] { key, item });
        }

        public override MethodInfo GetAddMethod(XamlType contentType)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return SupportedTypeInfo.LookupAddMethod(_underlyingType);
        }

        public override MethodInfo GetEnumeratorMethod()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return SupportedTypeInfo.LookupEnumeratorMethod(_underlyingType);
        }

        public override IEnumerator GetItems(object instance)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return (IEnumerator)GetEnumeratorMethod().Invoke(instance, new object[] { } );
        }

        public static object DetermineErrorAction(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.XamlTypeInvokerReturnsNull:
                    return null;
                case ErrorType.XamlTypeInvokerThrows:
                    throw new NotSupportedException("This is an expected exception thrown in a negative test case.");
                case ErrorType.NoError:
                default:
                    return new object();                    
            }
        }
    }
}
