// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;

    public class CustomXamlMember :XamlMember
    {
        MethodInfo _getter;
        MethodInfo _setter;
        XamlSchemaContext _context;
        public CustomXamlMember(PropertyInfo info, XamlSchemaContext context)
            : base(info, context)
        {
            this._getter = info.GetGetMethod();
            this._setter = info.GetSetMethod();
            this._context = context;
        }

        public CustomXamlMember(EventInfo info, XamlSchemaContext context)
            : base(info, context)
        {
            this._getter = null;
            this._setter = info.GetAddMethod();
            this._context = context;
        }

        public CustomXamlMember(string attachableMemberName, MethodInfo getter, MethodInfo setter, XamlSchemaContext context)
            : base(attachableMemberName, getter, setter, context)
        {
            this._getter = getter;
            this._setter = setter;
            this._context = context;

        }

        protected override XamlMemberInvoker LookupInvoker()
        {
            
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return new CustomXamlMemberInvoker(_getter, _setter, this) { ErrorType = ((CustomSchemaContext)_context).ErrorType };
        }

        protected override ICustomAttributeProvider LookupCustomAttributeProvider()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return new CustomAttributeProvider(UnderlyingMember);
        }

        protected override IList<XamlMember> LookupDependsOn()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return base.LookupDependsOn();
        }

        protected override bool LookupIsAmbient()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return false;
        }

        protected override bool LookupIsEvent()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return _getter == null;
        }

        protected override XamlType LookupTargetType()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            if (_getter != null)
            {
                return new CustomXamlType(_getter.DeclaringType, _context, true, new CustomXamlTypeInvoker(_getter.DeclaringType));
            }

            if (_setter != null)
            {
                return new CustomXamlType(_setter.DeclaringType, _context, true, new CustomXamlTypeInvoker(_setter.DeclaringType));
            }

            throw new NotSupportedException("Unknown declaring type.");
        }

        protected override bool LookupIsReadOnly()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return _setter == null;
        }

        protected override bool LookupIsReadPublic()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return _getter != null;
        }

        protected override bool LookupIsUnknown()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return false;
        }

        protected override bool LookupIsWriteOnly()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return _getter == null;
        }

        protected override bool LookupIsWritePublic()
        {
            DetermineErrorAction(((CustomSchemaContext)_context).ErrorType);
            return _setter != null;
        }

        protected override XamlType LookupType()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            if (_getter != null)
            {
                return new CustomXamlType(_getter.DeclaringType, _context, true, new CustomXamlTypeInvoker(_getter.DeclaringType));
            }

            if (_setter != null)
            {
                return new CustomXamlType(_setter.DeclaringType, _context, true, new CustomXamlTypeInvoker(_setter.DeclaringType));
            }

            throw new NotSupportedException("Unknown declaring type.");
        }

        protected override MethodInfo LookupUnderlyingGetter()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return base.LookupUnderlyingGetter();
        }

        protected override MemberInfo LookupUnderlyingMember()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return base.LookupUnderlyingMember();
        }

        protected override MethodInfo LookupUnderlyingSetter()
        {
            if (DetermineErrorAction(((CustomSchemaContext)_context).ErrorType) == null)
            {
                return null;
            }

            return Invoker.UnderlyingSetter;
        }

        public static object DetermineErrorAction(ErrorType error)
        {
            switch (error)
            {
                case ErrorType.XamlMemberReturnsNull:
                    return null;
                case ErrorType.XamlMemberThrows:
                    throw new NotSupportedException("This is an expected exception thrown in a negative test case.");
                case ErrorType.NoError:
                default:
                    return new object();
            }
        }
        
    }
}
