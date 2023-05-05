// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;

namespace Microsoft.Xaml.Tools.XamlDom
{
    [DebuggerDisplay("<{Type.Name}>")]
    [System.Windows.Markup.ContentProperty("MemberNodes")]
    public class ObjectNode : ItemNode, IXamlTypeResolver
    {
        public override bool IsSealed
        {
            get { return _isSealed; }
        }

        public override void Seal()
        {
            _isSealed = true;
            if (_propertyNodes != null)
            {
                _propertyNodes.Seal();
            }
            if (_namespaceNodes != null)
            {
                _namespaceNodes.Seal();
            }
        }

        [DefaultValue(null)]
        public XamlType Type
        {
            get { return _type; }
            set { CheckSealed(); _type = value; }
        }

        [DefaultValue(false)]
        public bool IsGetObject
        {
            get { return _isGetObject; }
            set { CheckSealed(); _isGetObject = value; }
        }
        
        [DefaultValue(null)]
        public string XmlLang
        {
            get { return _xmlLang; }
            set { CheckSealed(); _xmlLang = value; }
        }

        public string LookupNamespaceByPrefix(string prefix)
        {
            NamespaceDeclaration namespaceDeclaration;
            if (_namespaceNodes != null && _namespaceNodes.TryGetValue(prefix, out namespaceDeclaration))
            {
                return namespaceDeclaration.Namespace;
            }
            else
            {
                if (this.ParentMemberNode != null)
                {
                    return this.ParentMemberNode.LookupNamespaceByPrefix(prefix);
                }
                else
                {
                    return null;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDictionary<string,NamespaceDeclaration> NamespaceNodes
        {
            get
            {
                if (_namespaceNodes == null)
                {
                    _namespaceNodes = new SealableDictionary<string, NamespaceDeclaration>();
                    if (IsSealed)
                    {
                        _namespaceNodes.Seal();
                    }
                }
                return _namespaceNodes;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NodeCollection<MemberNode> MemberNodes
        {
            get
            {
                if (_propertyNodes == null)
                {
                    _propertyNodes = new NodeCollection<MemberNode>(this);
                    if (IsSealed)
                    {
                        _propertyNodes.Seal();
                    }
                }
                return _propertyNodes;
            }
        }

        public MemberNode GetMemberNode(XamlMember member)
        {
            if (_propertyNodes != null)
            {
                foreach (MemberNode memberNode in _propertyNodes)
                {
                    if (memberNode.Member == member)
                    {
                        return memberNode;
                    }
                }
            }

            return null;
        }

        public Type Resolve(string qualifiedTypeName)
        {
            int colon = qualifiedTypeName.IndexOf(':');
            string prefix = "";
            if (colon > -1)
            {
                prefix = qualifiedTypeName.Substring(0, colon);
            }
            string xmlNs = this.LookupNamespaceByPrefix(prefix);
            if (xmlNs == null)
            {
                return null;
            }
            string typeName = qualifiedTypeName.Substring(colon + 1);

            if (this.Type != null)
            {
                XamlType referencedXamlType = this.Type.SchemaContext.GetXamlType(new XamlTypeName(xmlNs, typeName));

                return (referencedXamlType == null || referencedXamlType.UnderlyingType == null)
                    ? null : referencedXamlType.UnderlyingType;
            }
            else
            {
                return null;
            }
        }

        private void CheckSealed()
        {
            if (IsSealed)
            {
                throw new NotSupportedException("The ObjectNode is sealed.");
            }
        }


        private SealableDictionary<string, NamespaceDeclaration> _namespaceNodes;
        private bool _isSealed;
        private XamlType _type;
        private bool _isGetObject;
        private NodeCollection<MemberNode> _propertyNodes;
        private string _xmlLang;
    }
}

