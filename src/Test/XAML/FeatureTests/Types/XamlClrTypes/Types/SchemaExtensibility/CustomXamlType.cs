// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;

    public class CustomXamlType : XamlType
    {
        private Type _clrType;
        private XamlTypeInvoker _invoker;
        private bool _customXamlMember;

        public CustomXamlType(Type type, XamlSchemaContext context, bool customXamlMember, XamlTypeInvoker invoker)
            : base(type, context, invoker)
        {
            this._clrType = type;
            this._invoker = invoker;
            this._customXamlMember = customXamlMember;
        }

        public CustomXamlType(Type type, XamlSchemaContext context, bool customXamlMember)
            : this(type, context, customXamlMember, null)
        {
        }

        public CustomXamlType(CustomSchemaContext schemaContext)
            : base("Unknown", null, schemaContext)
        {
        }

        protected override XamlMember LookupAliasedProperty(XamlDirective directive)
        {
            DetermineErrorAction(ErrorType);
            return null;
        }

        protected override IEnumerable<XamlMember> LookupAllAttachableMembers()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            var attachableMembers = new HashSet<XamlMember>();
            foreach (string attachableMemberName in SupportedTypeInfo.GetAttachableMembers(_clrType))
            {
                attachableMembers.Add(
                    BuildXamlMember(attachableMemberName,
                                    _clrType.GetMethod("Get" + attachableMemberName),
                                    _clrType.GetMethod("Set" + attachableMemberName)));
            }

            return attachableMembers;
        }

        protected override IEnumerable<XamlMember> LookupAllMembers()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            var members = new HashSet<XamlMember>();
            foreach (MemberInfo memberInfo in SupportedTypeInfo.GetMembers(_clrType))
            {
                if (memberInfo is PropertyInfo)
                {
                    members.Add(BuildXamlMember((PropertyInfo)memberInfo));
                }
                else if (memberInfo is EventInfo)
                {
                    members.Add(BuildXamlMember((EventInfo)memberInfo));
                }
                else
                {
                    throw new NotSupportedException(memberInfo.GetType() + " is not a supported member type.");
                }
            }

            return members;
        }

        protected override IList<XamlType> LookupAllowedContentTypes()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return (from type in SupportedTypeInfo.GetContentTypes(_clrType)
                    select SchemaContext.GetXamlType(type)).ToList();
            
        }

        protected override XamlMember LookupAttachableMember(string name)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            if (!SupportedTypeInfo.GetAttachableMembers(_clrType).Contains(name))
            {
                throw new InvalidOperationException("Unable to find attachable property: " + name);
            }

            return BuildXamlMember(name, _clrType.GetMethod("Get" + name), _clrType.GetMethod("Set" + name));
        }

        protected override XamlCollectionKind LookupCollectionKind()
        {
            DetermineErrorAction(ErrorType);
            return SupportedTypeInfo.GetCollectionKind(_clrType);
        }

        protected override XamlType LookupBaseType()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return _clrType.BaseType == null ? 
                null :
                new CustomXamlType(_clrType.BaseType, SchemaContext, _customXamlMember, _invoker);
        }

        protected override bool LookupConstructionRequiresArguments()
        {
            DetermineErrorAction(ErrorType);
            return false;
        }

        protected override XamlMember LookupContentProperty()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            PropertyInfo contentProperty = SupportedTypeInfo.GetContentProperty(_clrType);
            return contentProperty == null ?
                null :
                BuildXamlMember(contentProperty);
        }

        protected override IList<XamlType> LookupContentWrappers()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return (from type in SupportedTypeInfo.GetContentWrappers(_clrType)
                    select SchemaContext.GetXamlType(type)).ToList();
        }

        protected override ICustomAttributeProvider LookupCustomAttributeProvider()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return new CustomAttributeProvider(_clrType);
        }

        protected override XamlValueConverter<XamlDeferringLoader> LookupDeferringLoader()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return base.LookupDeferringLoader();
        }

        protected override bool LookupIsConstructible()
        {
            DetermineErrorAction(ErrorType);
            return true;
        }

        protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck)
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            MemberInfo memberInfo = SupportedTypeInfo.GetMember(_clrType, name);
            if (memberInfo is PropertyInfo)
            {
                return BuildXamlMember((PropertyInfo)memberInfo);
            }
            else if (memberInfo is EventInfo)
            {
               return BuildXamlMember((EventInfo)memberInfo);
            }
            else
            {
                throw new NotSupportedException(memberInfo.GetType() + " is not a supported member type.");
            }
        }

        protected override Type LookupUnderlyingType()
        {
            if (DetermineErrorAction(ErrorType) == null)
            {
                return null;
            }

            return _clrType;
        }

        protected override bool LookupIsUnknown()
        {
            return false;
        }

        ErrorType ErrorType
        {
            get
            {
                return ((CustomSchemaContext)SchemaContext).ErrorType;
            }
        }

        XamlMember BuildXamlMember(PropertyInfo info)
        {
            return _customXamlMember ?
                new CustomXamlMember(info, SchemaContext) :
                new XamlMember(info, SchemaContext, new CustomXamlMemberInvoker(info.GetGetMethod(), info.GetSetMethod()) { ErrorType = this.ErrorType });
        }

        XamlMember BuildXamlMember(EventInfo info)
        {
            return _customXamlMember ?
                new CustomXamlMember(info, SchemaContext) :
                new XamlMember(info, SchemaContext, new CustomXamlMemberInvoker(null, info.GetAddMethod()) { ErrorType = this.ErrorType });
        }

        XamlMember BuildXamlMember(string attachablePropertyName, MethodInfo getter, MethodInfo setter)
        {
            return _customXamlMember ?
                new CustomXamlMember(attachablePropertyName, getter, setter, SchemaContext) :
                new XamlMember(attachablePropertyName, getter, setter, SchemaContext, new CustomXamlMemberInvoker(getter, setter) { ErrorType = this.ErrorType });
        }

        public static object DetermineErrorAction(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.XamlTypeReturnsNull:
                    return null;
                case ErrorType.XamlTypeThrows:
                    throw new NotSupportedException("This is an expected exception thrown in a negative test case.");
                case ErrorType.NoError:
                default:
                    return new object();
            }
        }
    }
}
