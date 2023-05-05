// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xaml;
using System;

namespace Microsoft.Xaml.Tools.XamlDom
{
    [DebuggerDisplay("{Member.Name}")]
    [System.Windows.Markup.ContentProperty("ItemNodes")]
    public class MemberNode : DomNode
    {
        public override bool IsSealed
        {
            get { return _isSealed; }
        }

        public override void Seal()
        {
            _isSealed = true;
            if (_itemNodes != null)
            {
                _itemNodes.Seal();
            }
            if (_namespaceNodes != null)
            {
                _namespaceNodes.Seal();
            }
        }

        [DefaultValue(null)]
        public XamlMember Member
        {
            get { return _member; }
            set { CheckSealed(); _member = value; }
        }
        [DefaultValue(null)]
        public string Prefix
        {
            get { return _prefix; }
            set { CheckSealed(); _prefix = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectNode ParentObjectNode
        {
            get { return _parentObjectNode; }
            set { CheckSealed(); _parentObjectNode = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public NodeCollection<ItemNode> ItemNodes
        {
            get
            {
                if (_itemNodes == null)
                {
                    _itemNodes = new NodeCollection<ItemNode>(this);
                    if (IsSealed)
                    {
                        _itemNodes.Seal();
                    }
                }
                return _itemNodes;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDictionary<string, NamespaceDeclaration> NamespaceNodes
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
        
        internal string LookupNamespaceByPrefix(string prefix)
        {
            NamespaceDeclaration namespaceDeclaration;
            if (_namespaceNodes != null && _namespaceNodes.TryGetValue(prefix, out namespaceDeclaration))
            {
                return namespaceDeclaration.Namespace;
            }
            else
            {
                return this.ParentObjectNode.LookupNamespaceByPrefix(prefix);
            }
        }

        private void CheckSealed()
        {
            if (IsSealed)
            {
                throw new NotSupportedException("The MemberNode is sealed.");
            }
        }

        private XamlMember _member;
        private NodeCollection<ItemNode> _itemNodes;
        private ObjectNode _parentObjectNode;
        private SealableDictionary<string, NamespaceDeclaration> _namespaceNodes;
        private string _prefix;
        private bool _isSealed;
    }
}
