// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xaml;
using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xaml.Tools.XamlDom
{
    [DebuggerDisplay("{Member.Name}")]
    [System.Windows.Markup.ContentProperty("ItemNodes")]
    public class XamlDomMember : XamlDomNode
    {
        public XamlDomMember()
        {
        }

        public XamlDomMember(XamlMember xamlMember)
        {
            _member = xamlMember;
            if (xamlMember != null)
            {
                _schemaContext = xamlMember.Type.SchemaContext;
            }
        }

        public XamlDomMember(XamlMember xamlMember, params XamlDomItem[] items)
            : this(xamlMember)
        {
            foreach (XamlDomItem item in items)
            {
                Items.Add(item);
            }
            Resolve();
        }

        public XamlDomMember(XamlMember xamlMember, object value)
            : this(xamlMember)
        {
            Item = new XamlDomValue(value);
            Resolve();
        }

        public XamlDomMember(string instanceMember, params XamlDomItem[] items)
            : this((Type)null, instanceMember, items)
        {
            if (instanceMember.Contains("."))
            {
                throw new NotSupportedException("Attached members are not support in this constructor. Please call the constructor passing in the type and attached member name");
            }
        }

        public XamlDomMember(Type declaringType, string attachedMember, params XamlDomItem[] items)
        {
            foreach (XamlDomItem itemNode in items)
            {
                Items.Add(itemNode);
            }

            _unresolvedDeclaringType = declaringType;
            _unresolvedMemberName = attachedMember;
        }

        // XamlDomObject.GetMemberNode() needs to take a XamlType and string for supporting unknown attached members.
        // This constructor is here to match that.
        public XamlDomMember(XamlType declaringType, string attachedMember, params XamlDomItem[] items)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }
            if (attachedMember == null)
            {
                throw new ArgumentNullException("attachedMember");
            }

            foreach (XamlDomItem itemNode in items)
            {
                Items.Add(itemNode);
            }

            XamlMember member = declaringType.GetAttachableMember(attachedMember);
            if (member == null)
            {
                throw new NotImplementedException("Unknown members are not implemented");
            }
            _member = member;
            Resolve();            
        }

        public XamlDomMember(string instanceMember, object value)
            : this(null, instanceMember, value)
        {
            if (instanceMember.Contains("."))
            {
                throw new NotSupportedException("Attached members are not support in this constructor. Please call the constructor passing in the type and attached member name");
            }
        }

        public XamlDomMember(Type declaringType, string attachedMember, object value)
        {
            if (attachedMember == null)
            {
                throw new ArgumentNullException("attachedMember");
            }
            _unresolvedDeclaringType = declaringType;
            _unresolvedMemberName = attachedMember;

            Item = new XamlDomValue(value);
        }

        public override void Seal()
        {
            base.Seal();
            if (_items != null)
            {
                _items.Seal();
            }
        }

        [DefaultValue(null)]
        public virtual XamlMember Member
        {
            get { return _member; }
            set { CheckSealed(); _member = value; _schemaContext = _member.Type.SchemaContext; Resolve(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual XamlDomObject Parent
        {
            get { return _parent; }
            set
            {
                CheckSealed();
                _parent = value;
            }
        }

        internal void Resolve()
        {
            if (SchemaContext == null && Parent != null && Parent.SchemaContext != null)
            {
                _schemaContext = Parent.SchemaContext;
            }
            if (Member == null && _unresolvedMemberName != null)
            {
                if (_unresolvedDeclaringType != null)
                {
                    _member = SchemaContext.GetXamlType(_unresolvedDeclaringType).GetAttachableMember(_unresolvedMemberName);
                }
                else
                {
                    _member = Parent.Type.GetMember(_unresolvedMemberName);
                }
                _unresolvedMemberName = null;
                _unresolvedDeclaringType = null;
            }

            foreach (XamlDomItem itemNode in Items)
            {
                XamlDomObject objNode = itemNode as XamlDomObject;
                if (objNode != null)
                {
                    objNode.Resolve();
                }
            }
        }

        public virtual XamlDomItem Item
        {
            get
            {
                if (Items.Count > 1)
                {
                    throw new NotSupportedException("Member has more than one member");
                }
                if (Items.Count == 0)
                {
                    return null;
                }                   
                return Items[0];
            }
            set
            {
                Items.Clear();
                Items.Add(value);
            }

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public virtual IList<XamlDomItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new XamlNodeCollection<XamlDomItem>(this);
                    if (IsSealed)
                    {
                        _items.Seal();
                    }
                }
                return _items;
            }
        }

        public virtual XamlSchemaContext SchemaContext
        {
            get { return _schemaContext; }
            set 
            {
                CheckSealed();
                if (Member != null && !Member.IsDirective && Member.Type.SchemaContext != value)
                {
                    throw new InvalidOperationException("SchemaContext must match the XamlMember's SchemaContext");
                }
                _schemaContext = value;
            }
        }

        internal string LookupNamespaceByPrefix(string prefix)
        {
            return this.Parent.GetNamespace(prefix);
        }

        private XamlMember _member;
        private XamlNodeCollection<XamlDomItem> _items;
        private XamlDomObject _parent;
        private XamlSchemaContext _schemaContext;

        private string _unresolvedMemberName;
        private Type _unresolvedDeclaringType;
    }
}
