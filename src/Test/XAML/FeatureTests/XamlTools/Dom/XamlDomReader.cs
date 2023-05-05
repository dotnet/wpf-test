// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xaml;
using System;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class XamlDomReader : XamlReader, IXamlLineInfo
    {
        IEnumerator<XamlNode> _nodes;
        XamlSchemaContext _schemaContext;
        private bool _doNotReorder;

        public XamlDomReader(XamlDomNode domNode, XamlSchemaContext schemaContext)
            : this(domNode, schemaContext, null)
        {
        }

        public XamlDomReader(XamlDomNode domNode, XamlSchemaContext schemaContext, XamlDomReaderSettings settings)
        {
            if (schemaContext == null)
            {
                throw new ArgumentNullException("schemaContext");
            }
            if (domNode == null)
            {
                throw new ArgumentNullException("domNode");
            }

            _schemaContext = schemaContext;
            if (settings != null)
            {
                _doNotReorder = settings.DoNotReorderMembers;
            }
            _nodes = WalkDom(domNode).GetEnumerator();
        }

        public override bool IsEof
        {
            get { return NodeType != XamlNodeType.None; }
        }

        public override XamlMember Member
        {
            get
            {
                if (NodeType == XamlNodeType.StartMember)
                    return _nodes.Current.Member;
                else
                    return null;
            }
        }

        public override NamespaceDeclaration Namespace
        {
            get
            {
                NamespaceDeclaration nsNode = _nodes.Current.Namespace;
                if (nsNode != null)
                    return nsNode;
                else
                    return null;
            }
        }

        public override XamlNodeType NodeType
        {
            get { return _nodes.Current.NodeType; }
        }

        public override XamlSchemaContext SchemaContext
        {
            get { return _schemaContext; }
        }

        public override XamlType Type
        {
            get
            {
                if (NodeType == XamlNodeType.StartObject)
                    return _nodes.Current.Type;
                else
                    return null;
            }
        }

        public override object Value
        {
            get
            {
                if (NodeType == XamlNodeType.Value)
                {
                    return _nodes.Current.Value;
                }
                else
                    return null;
            }
        }

        public override bool Read()
        {
            return _nodes.MoveNext();
        }

        #region IXamlLineInfo Members

        bool IXamlLineInfo.HasLineInfo
        {
            get { return true; }
        }

        int IXamlLineInfo.LineNumber
        {
            get
            {
                return _nodes.Current.LineNumber;
            }
        }

        int IXamlLineInfo.LinePosition
        {
            get
            {
                return _nodes.Current.LinePosition;         
            }
        }

        #endregion

        private IEnumerable<XamlNode> WalkDom(XamlDomNode domNode)
        {
            XamlDomObject objectNode = domNode as XamlDomObject;
            if (objectNode != null)
            {
                foreach (var node in ReadObjectNode(objectNode))
                {
                    yield return node;
                }
            }
            else
            {
                XamlDomMember memberNode = domNode as XamlDomMember;
                if (memberNode != null)
                {
                    foreach (var node in ReadMemberNode(memberNode))
                    {
                        yield return node;
                    }
                }
                else
                {
                    foreach (var node in ReadValueNode(domNode as XamlDomValue))
                    {
                        yield return node;
                    }
                }
            }
        }

        private IEnumerable<XamlNode> ReadValueNode(XamlDomValue XamlDomValue)
        {
            yield return XamlNode.GetValue(XamlDomValue);
        }

        private IEnumerable<XamlNode> ReadMemberNode(XamlDomMember memberNode)
        {
            if (memberNode.Items != null && memberNode.Items.Count > 0)
            {
                yield return XamlNode.GetStartMember(memberNode);
                foreach (XamlDomItem itemNode in memberNode.Items)
                {
                    XamlDomObject objectNode = itemNode as XamlDomObject;
                    IEnumerable<XamlNode> enumerable;
                    if (objectNode != null)
                    {
                        enumerable = ReadObjectNode(objectNode);
                    }
                    else
                    {
                        enumerable = ReadValueNode(itemNode as XamlDomValue);
                    }

                    foreach (XamlNode node in enumerable)
                    {
                        yield return node;
                    }
                }
                yield return XamlNode.GetEndMember(memberNode);
            }
        }

        private IEnumerable<XamlNode> ReadObjectNode(XamlDomObject objectNode)
        {
            foreach (XamlDomNamespace nsNode in objectNode.Namespaces)
            {
                yield return XamlNode.GetNamespaceDeclaration(nsNode);
            }

            yield return XamlNode.GetStartObject(objectNode);

            // We want to write out simple things that could be attributes out first if setting is set
            // We write out single values and things that are MEs
            if (!_doNotReorder)
            {
                foreach (var node in WritePossibleAttributes(objectNode))
                {
                    yield return node;
                }

                foreach (var node in WriteElementMembers(objectNode))
                {
                    yield return node;
                }
            }
            else
            {
                foreach (XamlDomMember memberNode in objectNode.MemberNodes)
                {
                    foreach (var node in ReadMemberNode(memberNode))
                    {
                        yield return node;
                    }
                }
            }

            yield return XamlNode.GetEndObject(objectNode);
        }

        private IEnumerable<XamlNode> WriteElementMembers(XamlDomObject objectNode)
        {
            foreach (XamlDomMember memberNode in objectNode.MemberNodes)
            {
                if (IsAttribute(memberNode))
                {
                    continue;
                }

                foreach (var node in ReadMemberNode(memberNode))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<XamlNode> WritePossibleAttributes(XamlDomObject objectNode)
        {
            foreach (XamlDomMember memberNode in objectNode.MemberNodes)
            {
                if (IsAttribute(memberNode))
                {
                    foreach (var node in ReadMemberNode(memberNode))
                    {
                        yield return node;
                    }
                }
            }
        }

        private bool IsAttribute(XamlDomMember memberNode)
        {
            if (memberNode.Items.Count == 1)
            {
                if (memberNode.Item is XamlDomValue)
                {
                    return true;
                }
                else
                {
                    XamlType objectType = (memberNode.Item as XamlDomObject).Type;
                    if (objectType != null && objectType.IsMarkupExtension)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private class XamlNode
        {
            private static XamlNode s_xamlNode = new XamlNode();

            public XamlType Type = null;
            public XamlMember Member = null;
            public NamespaceDeclaration Namespace = null;
            public XamlNodeType NodeType = XamlNodeType.None;
            public object Value = null;
            public int LineNumber = 0;
            public int LinePosition = 0;

            public void Clear()
            {
                Type = null;
                Member = null;
                Namespace = null;
                NodeType = XamlNodeType.None;
                Value = null;
                LineNumber = 0;
                LinePosition = 0;
            }

            public static XamlNode GetNamespaceDeclaration(XamlDomNamespace nsNode)
            {
                s_xamlNode.Clear();
                s_xamlNode.Namespace = nsNode.NamespaceDeclaration;
                s_xamlNode.NodeType = XamlNodeType.NamespaceDeclaration;
                s_xamlNode.LineNumber = nsNode.StartLineNumber;
                s_xamlNode.LinePosition = nsNode.StartLinePosition;
                return s_xamlNode;
            }

            public static XamlNode GetStartObject(XamlDomObject objectNode)
            {
                s_xamlNode.Clear();
                if (objectNode.IsGetObject)
                {
                    s_xamlNode.NodeType = XamlNodeType.GetObject;
                }
                else
                {
                    s_xamlNode.NodeType = XamlNodeType.StartObject;
                    s_xamlNode.Type = objectNode.Type;
                }
                s_xamlNode.LineNumber = objectNode.StartLineNumber;
                s_xamlNode.LinePosition = objectNode.StartLinePosition;
                return s_xamlNode;
            }

            internal static XamlNode GetEndObject(XamlDomObject objectNode)
            {
                s_xamlNode.Clear();
                s_xamlNode.NodeType = XamlNodeType.EndObject;
                s_xamlNode.LineNumber = objectNode.EndLineNumber;
                s_xamlNode.LinePosition = objectNode.EndLinePosition;
                return s_xamlNode;
            }

            internal static XamlNode GetStartMember(XamlDomMember memberNode)
            {
                s_xamlNode.Clear();
                s_xamlNode.NodeType = XamlNodeType.StartMember;
                s_xamlNode.Member = memberNode.Member;
                s_xamlNode.LineNumber = memberNode.StartLineNumber;
                s_xamlNode.LinePosition = memberNode.StartLinePosition;
                return s_xamlNode;
            }


            internal static XamlNode GetEndMember(XamlDomMember memberNode)
            {
                s_xamlNode.Clear();
                s_xamlNode.NodeType = XamlNodeType.EndMember;
                s_xamlNode.LineNumber = memberNode.EndLineNumber;
                s_xamlNode.LinePosition = memberNode.EndLinePosition;
                return s_xamlNode;
            }

            internal static XamlNode GetValue(XamlDomValue XamlDomValue)
            {
                s_xamlNode.Clear();
                s_xamlNode.NodeType = XamlNodeType.Value;
                s_xamlNode.Value = XamlDomValue.Value;
                s_xamlNode.LineNumber = XamlDomValue.StartLineNumber;
                s_xamlNode.LinePosition = XamlDomValue.StartLinePosition;
                return s_xamlNode;
            }
        }
    }
}
