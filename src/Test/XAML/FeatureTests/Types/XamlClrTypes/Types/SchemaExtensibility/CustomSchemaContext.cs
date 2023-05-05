// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Xaml;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Types.ContentProperties;
    using Microsoft.Test.Xaml.Types.InstanceReference;
    
    public enum ErrorType
    {
        NoError = 0,
        SchemaContextReturnsNull,
        SchemaContextThrows,
        XamlTypeReturnsNull,
        XamlTypeThrows,
        XamlTypeInvokerReturnsNull,
        XamlTypeInvokerThrows,
        XamlMemberReturnsNull,
        XamlMemberThrows,
        XamlMemberInvokerReturnsNull,
        XamlMemberInvokerThrows
    }

    public class CustomSchemaContext : XamlSchemaContext
    {
        private bool _useCustomXamlTypeXamlMember;

        public CustomSchemaContext()
            : this(false)
        {
        }
        

        public CustomSchemaContext(bool useCustomXamlTypeXamlMember)
            : this(useCustomXamlTypeXamlMember, ErrorType.NoError)
        {
        }
        

        public CustomSchemaContext(bool useCustomXamlTypeXamlMember, ErrorType error)
        {
            this._useCustomXamlTypeXamlMember = useCustomXamlTypeXamlMember;
            this.ErrorType = error;
        }

        static readonly HashSet<Type> s_supportedTypes = new HashSet<Type>
        {
            typeof(ClassType2),
            typeof(ClassType2[]),
            typeof(ClassType1),
            typeof(GenericType1<DateTime, double>),
            typeof(GenericType1<,>),
            typeof(StringContentPropertyWClass),
            typeof(CollectionContainerType14),
            typeof(CollectionContainerType15),
            typeof(ItemType),
            typeof(AttachedPropertySource),
            typeof(ImplicitNamingBar)
        };

        static readonly HashSet<string> s_fxAssemblies = new HashSet<string>
        {
            "System",
            "mscorlib",
        };

        public ErrorType ErrorType { get; set; }
        public override XamlType GetXamlType(Type type)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            if (s_supportedTypes.Contains(type) ||
                (s_supportedTypes.Contains(type.DeclaringType) && type.ContainsGenericParameters))
            {
                return BuildXamlType(type);
            }

            if (s_fxAssemblies.Contains(type.Assembly.GetName().Name))
            {
                return BuildXamlType(type);
            }

            throw new NotSupportedException(type.FullName + " is not supported.");
        }

        XamlType BuildXamlType(Type type)
        {
            return _useCustomXamlTypeXamlMember ?
                new CustomXamlType(type, this, _useCustomXamlTypeXamlMember, new CustomXamlTypeInvoker(type) { ErrorType = this.ErrorType }) :
                new XamlType(type, this, new CustomXamlTypeInvoker(type) { ErrorType = this.ErrorType });
        }

        public static object DetermineErrorAction(ErrorType error)
        {
            switch(error)
            {                  
                case ErrorType.SchemaContextReturnsNull:
                    return null;
                case ErrorType.SchemaContextThrows:
                    throw new NotSupportedException("This is an expected exception thrown in a negative test case.");
                case ErrorType.NoError:
                default:
                    return new object();        
            }
        }
    }
}
