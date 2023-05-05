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
    internal class WrapperXamlType : XamlType
    {
        private XamlType _xamlType;
        private UISchemaContext _schemaContext;

        public WrapperXamlType(XamlType xamlType, UISchemaContext schemaContext)
            : base(xamlType.UnderlyingType, schemaContext)
        {
            _xamlType = xamlType;
            _schemaContext = schemaContext;
        }

        protected override XamlMember LookupAliasedProperty(XamlDirective directive)
        {
            return _schemaContext.GetWrappedXamlMember(_xamlType.GetAliasedProperty(directive));
        }

        public override IList<string> GetXamlNamespaces()
        {
            return _xamlType.GetXamlNamespaces();
        }

        protected override IEnumerable<XamlMember> LookupAllAttachableMembers()
        {
            return _schemaContext.GetWrappedXamlMembers(_xamlType.GetAllAttachableMembers());
        }

        protected override IEnumerable<XamlMember> LookupAllMembers()
        {
            return _schemaContext.GetWrappedXamlMembers(_xamlType.GetAllMembers());
        }

        protected override IList<XamlType> LookupAllowedContentTypes()
        {
            return _schemaContext.GetWrappedXamlTypes(_xamlType.AllowedContentTypes);
        }

        protected override XamlMember LookupAttachableMember(string name)
        {
            return _schemaContext.GetWrappedXamlMember(_xamlType.GetAttachableMember(name));
        }

        protected override XamlType LookupBaseType()
        {
            return _schemaContext.GetWrappedXamlType(_xamlType.BaseType);
        }

        protected override System.Xaml.Schema.XamlCollectionKind LookupCollectionKind()
        {
            if (_xamlType.IsArray)
            {
                return System.Xaml.Schema.XamlCollectionKind.Array;
            }
            else if (_xamlType.IsDictionary)
            {
                return System.Xaml.Schema.XamlCollectionKind.Dictionary;
            }
            else if (_xamlType.IsCollection)
            {
                return System.Xaml.Schema.XamlCollectionKind.Collection;
            }
            else
            {
                return System.Xaml.Schema.XamlCollectionKind.None;
            }
        }

        protected override bool LookupConstructionRequiresArguments()
        {
            return _xamlType.ConstructionRequiresArguments;
        }

        protected override XamlMember LookupContentProperty()
        {
            return _schemaContext.GetWrappedXamlMember(_xamlType.ContentProperty);
        }

        protected override IList<XamlType> LookupContentWrappers()
        {
            return _schemaContext.GetWrappedXamlTypes(_xamlType.ContentWrappers);
        }

        protected override System.Xaml.Schema.XamlValueConverter<XamlDeferringLoader> LookupDeferringLoader()
        {
            return _schemaContext.GetWrappedDeferringLoader(_xamlType.DeferringLoader);
        }

        protected override bool LookupIsAmbient()
        {
            return _xamlType.IsAmbient;
        }

        protected override bool LookupIsConstructible()
        {
            return _xamlType.IsConstructible;
        }

        protected override bool LookupIsMarkupExtension()
        {
            return _xamlType.IsMarkupExtension;
        }

        protected override bool LookupIsNameScope()
        {
            return _xamlType.IsNameScope;
        }

        protected override bool LookupIsNullable()
        {
            return _xamlType.IsNullable;
        }

        protected override bool LookupIsPublic()
        {
            return _xamlType.IsPublic;
        }

        protected override bool LookupIsUnknown()
        {
            return _xamlType.IsUnknown;
        }

        protected override bool LookupIsWhitespaceSignificantCollection()
        {
            return _xamlType.IsWhitespaceSignificantCollection;
        }

        protected override bool LookupIsXData()
        {
            return _xamlType.IsXData;
        }

        protected override XamlType LookupItemType()
        {
            return _schemaContext.GetWrappedXamlType(_xamlType.ItemType);
        }

        protected override XamlType LookupKeyType()
        {
            return _schemaContext.GetWrappedXamlType(_xamlType.KeyType);
        }

        protected override XamlType LookupMarkupExtensionReturnType()
        {
            return _schemaContext.GetWrappedXamlType(_xamlType.MarkupExtensionReturnType);
        }

        protected override XamlMember LookupMember(string name, bool skipReadOnlyCheck)
        {
            // skipReadOnlyCheck is only used by XamlType.GetAliasedProperty for the DictionaryKeyProperty
            // In XAML, properties need to be settable but for the DKPA case, we only need to retrieve the property so 
            // read only is okay.  If we're looking for the DKPA, we'll allow this but throw otherwise.
            if (skipReadOnlyCheck)
            {
                XamlMember member = _xamlType.GetAliasedProperty(XamlLanguage.Key);
                if (member != null && member.Name == name)
                {
                    return _schemaContext.GetWrappedXamlMember(_xamlType.GetAliasedProperty(XamlLanguage.Key));
                }
                else
                {
                    throw new NotSupportedException("skipReadOnlyCheck is only support on the DictionaryKeyProperty");
                }
            }

            return _schemaContext.GetWrappedXamlMember(_xamlType.GetMember(name));
        }

        protected override IList<XamlType> LookupPositionalParameters(int parameterCount)
        {
            return _schemaContext.GetWrappedXamlTypes(_xamlType.GetPositionalParameters(parameterCount));
        }

        protected override EventHandler<System.Windows.Markup.XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler()
        {
            throw new NotImplementedException();
        }

        protected override EventHandler<System.Windows.Markup.XamlSetTypeConverterEventArgs> LookupSetTypeConverterHandler()
        {
            throw new NotImplementedException();
        }

        protected override bool LookupTrimSurroundingWhitespace()
        {
            return _xamlType.TrimSurroundingWhitespace;
        }

        protected override Type LookupUnderlyingType()
        {
            return _xamlType.UnderlyingType;
        }

        protected override bool LookupUsableDuringInitialization()
        {
            return _xamlType.IsUsableDuringInitialization;
        }

        protected override System.Xaml.Schema.XamlValueConverter<System.ComponentModel.TypeConverter> LookupTypeConverter()
        {
            return _schemaContext.GetWrappedTypeConverter(_xamlType.TypeConverter);
        }

        protected override System.Xaml.Schema.XamlValueConverter<System.Windows.Markup.ValueSerializer> LookupValueSerializer()
        {
            return _schemaContext.GetWrappedValueSerializer(_xamlType.ValueSerializer);
        }

        protected override System.Xaml.Schema.XamlTypeInvoker LookupInvoker()
        {
            return _xamlType.Invoker;
        }
    }
}
