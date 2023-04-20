// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.Diagnostics;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class XamlDomWriter : XamlWriter, IXamlLineInfoConsumer
    {
        public XamlDomWriter()
        {
            _schemaContext = new XamlSchemaContext();
        }

        public XamlDomWriter(XamlSchemaContext schemaContext)
        {
            _schemaContext = schemaContext;
        }

        Stack<XamlDomNode> _writerStack = new Stack<XamlDomNode>();

        #region XamlWriter Members
        public XamlDomNode RootNode { get; set; }
        XamlSchemaContext _schemaContext;

        public override void WriteGetObject()
        {
            WriteObject(null, true);
        }

        public override void WriteStartObject(XamlType xamlType)
        {
            WriteObject(xamlType, false);
        }

        void WriteObject(XamlType xamlType, bool isGetObject)
        {
            XamlDomObject objectNode = (isGetObject ? new XamlDomObject() : new XamlDomObject(xamlType));
            objectNode.IsGetObject = isGetObject;
            objectNode.StartLinePosition = _linePosition;
            objectNode.StartLineNumber = _lineNumber;
            // If it's a GetObject or a Directive, we need to store the actual XamlSchemaContext
            if (objectNode.IsGetObject ||objectNode.Type.SchemaContext == XamlLanguage.Object.SchemaContext)
            {
                objectNode.SchemaContext = SchemaContext;
            }

            if (_namespaces != null)
            {
                foreach (XamlDomNamespace xdns in _namespaces)
                {
                    objectNode.Namespaces.Add(xdns);
                }

                _namespaces.Clear();
            }

            // If Root Node is null then this is the root node.
            // If Root Node is not null, then add this to the parent member.

            if (RootNode == null)
            {
                RootNode = objectNode;
            }
            else
            {
                XamlDomMember propertyNode = (XamlDomMember)_writerStack.Peek();
                propertyNode.Items.Add(objectNode);
                objectNode.Parent = propertyNode;
                if (isGetObject)
                {
                    objectNode.Type = propertyNode.Member.Type;
                }
            }
            _writerStack.Push(objectNode);
        }

        public override void WriteEndObject()
        {
            Debug.Assert(CurrentStackNode is XamlDomObject);
            CurrentStackNode.EndLineNumber = _lineNumber;
            CurrentStackNode.EndLinePosition = _linePosition;
            _writerStack.Pop();
        }

        public override void WriteStartMember(XamlMember property)
        {
            XamlDomMember propertyNode = new XamlDomMember(property);

            // Only need to set the SchemaContext if it's a XamlDirective
            if (property.IsDirective)
            {
                propertyNode.SchemaContext = SchemaContext;
            }

            if (RootNode != null)
            {

                XamlDomObject objectNode = (XamlDomObject)_writerStack.Peek();

                objectNode.MemberNodes.Add(propertyNode);
            }
            else
            {
                RootNode = propertyNode;
            }
            propertyNode.StartLineNumber = _lineNumber;
            propertyNode.StartLinePosition = _linePosition;

            _writerStack.Push(propertyNode);
        }

        public override void WriteEndMember()
        {
            Debug.Assert(CurrentStackNode is XamlDomMember);
            CurrentStackNode.EndLineNumber = _lineNumber;
            CurrentStackNode.EndLinePosition = _linePosition;
            _writerStack.Pop();
        }

        public override void WriteValue(object value)
        {
            XamlDomValue valueNode = new XamlDomValue();
            valueNode.Value = value;

            if (RootNode != null)
            {
                //text should always be inside of a property...
                XamlDomMember propertyNode = (XamlDomMember)_writerStack.Peek();
                propertyNode.Items.Add(valueNode);
            }
            else
            {
                RootNode = valueNode;
            }

            valueNode.StartLineNumber = _lineNumber;
            valueNode.StartLinePosition = _linePosition;
            valueNode.EndLineNumber = _lineNumber;
            valueNode.EndLinePosition = _linePosition;
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            if (_namespaces == null)
            {
                _namespaces = new List<XamlDomNamespace>();
            }
           
            XamlDomNamespace nsNode = new XamlDomNamespace(namespaceDeclaration);
            nsNode.StartLineNumber = _lineNumber;
            nsNode.StartLinePosition = _linePosition;
            nsNode.EndLineNumber = _lineNumber;
            nsNode.EndLinePosition = _linePosition;

            _namespaces.Add(nsNode);
        }

        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return _schemaContext;
            }
        }

        #endregion

        #region IXamlLineInfoConsumer Members

        void IXamlLineInfoConsumer.SetLineInfo(int lineNumber, int linePosition)
        {
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        bool IXamlLineInfoConsumer.ShouldProvideLineInfo
        {
            get { return true; }
        }

        #endregion

        private XamlDomNode CurrentStackNode
        {
            get
            {
                if (_writerStack.Count > 0)
                {
                    return _writerStack.Peek();
                }
                else
                {
                    return null;
                }
            }
        }

        private int _lineNumber;
        private int _linePosition;
        private List<XamlDomNamespace> _namespaces;

    }
}
