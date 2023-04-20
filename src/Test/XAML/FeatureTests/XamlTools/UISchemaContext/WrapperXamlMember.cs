// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;

namespace Microsoft.Xaml.Tools
{
    internal class WrapperXamlMember : XamlMember
    {
        XamlMember _member;
        UISchemaContext _schema;

        public WrapperXamlMember(XamlMember member, UISchemaContext schemaContext)
            :base (member.Name, schemaContext.GetWrappedXamlType(member.DeclaringType), member.IsAttachable)
        {
            _schema = schemaContext;
            _member = member;
        }

        protected override System.Xaml.Schema.XamlValueConverter<XamlDeferringLoader> LookupDeferringLoader()
        {
            return _schema.GetWrappedDeferringLoader(_member.DeferringLoader);
        }

        protected override IList<XamlMember> LookupDependsOn()
        {
            return _schema.GetWrappedXamlMembers(_member.DependsOn);
        }

        protected override System.Xaml.Schema.XamlMemberInvoker LookupInvoker()
        {
            return _member.Invoker;
        }

        protected override bool LookupIsAmbient()
        {
            return _member.IsAmbient;
        }

        protected override bool LookupIsEvent()
        {
            return _member.IsEvent;
        }

        protected override bool LookupIsReadOnly()
        {
            return _member.IsReadOnly;
        }

        protected override bool LookupIsReadPublic()
        {
            return _member.IsReadPublic;
        }

        protected override bool LookupIsUnknown()
        {
            return _member.IsUnknown;
        }

        protected override bool LookupIsWriteOnly()
        {
            return _member.IsWriteOnly;
        }

        protected override bool LookupIsWritePublic()
        {
            return _member.IsWritePublic;
        }

        public override IList<string> GetXamlNamespaces()
        {
            return _member.GetXamlNamespaces();
        }

        protected override XamlType LookupTargetType()
        {
            return _schema.GetWrappedXamlType(_member.TargetType);
        }

        protected override XamlType LookupType()
        {
            return _schema.GetWrappedXamlType(_member.Type);
        }

        protected override System.Xaml.Schema.XamlValueConverter<System.ComponentModel.TypeConverter> LookupTypeConverter()
        {
            return _schema.GetWrappedTypeConverter(_member.TypeConverter);
        }

        protected override System.Reflection.MethodInfo LookupUnderlyingGetter()
        {
            return _member.Invoker.UnderlyingGetter;
        }

        protected override System.Reflection.MethodInfo LookupUnderlyingSetter()
        {
            return _member.Invoker.UnderlyingSetter;
        }

        protected override System.Reflection.MemberInfo LookupUnderlyingMember()
        {
            return _member.UnderlyingMember;
        }

        protected override System.Xaml.Schema.XamlValueConverter<System.Windows.Markup.ValueSerializer> LookupValueSerializer()
        {
            return _schema.GetWrappedValueSerializer(_member.ValueSerializer);
        }
    }
}
