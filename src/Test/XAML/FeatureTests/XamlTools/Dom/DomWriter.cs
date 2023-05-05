// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class DomWriter : XamlWriter
    {
        public DomWriter()
        {
            _schemaContext = new XamlSchemaContext();
        }

        public DomWriter(XamlSchemaContext schemaContext)
        {
            _schemaContext = schemaContext;
        }

        Stack<DomNode> _writerStack = new Stack<DomNode>();

        #region XamlWriter Members
        public DomNode RootNode = null;
        XamlSchemaContext _schemaContext;

        public override void WriteGetObject()
        {
            WriteObject(null, true);
        }

        public override void WriteStartObject(XamlType xamlType)
        {
            WriteObject(xamlType, false);
        }

        void WriteObject(XamlType xamlType, bool isRetrieved)
        {
            ObjectNode objectNode = new ObjectNode();
            objectNode.Type = xamlType;
            objectNode.IsGetObject = isRetrieved;

            if (RootNode != null)
            {
                MemberNode propertyNode = (MemberNode)_writerStack.Peek();
                propertyNode.ItemNodes.Add(objectNode);
                objectNode.ParentMemberNode = propertyNode;
            }
            else
            {
                RootNode = objectNode;
            }

            _writerStack.Push(objectNode);
        }

        public override void WriteEndObject()
        {
            _writerStack.Pop();
        }

        public override void WriteStartMember(XamlMember property)
        {
            MemberNode propertyNode = new MemberNode();
            propertyNode.Member = property;

            if (RootNode != null)
            {

                ObjectNode objectNode = (ObjectNode)_writerStack.Peek();

                objectNode.MemberNodes.Add(propertyNode);
            }
            else
            {
                RootNode = propertyNode;
            }

            _writerStack.Push(propertyNode);
        }

        public override void WriteEndMember()
        {
            _writerStack.Pop();
        }

        public override void WriteValue(object value)
        {
            ValueNode valueNode = new ValueNode();
            valueNode.Value = value;

            if (RootNode != null)
            {
                //text should always be inside of a property...
                MemberNode propertyNode = (MemberNode)_writerStack.Peek();
                propertyNode.ItemNodes.Add(valueNode);
            }
            else
            {
                RootNode = valueNode;
            }
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            bool createFrame = true;
            if (_writerStack.Count == 0)
            {
                createFrame = true;
            }
            else
            {
                ObjectNode lastObjectNode = _writerStack.Peek() as ObjectNode;
                if (lastObjectNode != null && lastObjectNode.Type != null)
                {
                    createFrame = true;
                }
            }

            if (createFrame)
            {
                ObjectNode newObjectNode = new ObjectNode();
                _writerStack.Push(newObjectNode);
            }

            ObjectNode objectNode = (ObjectNode)_writerStack.Peek();
            objectNode.NamespaceNodes.Add(namespaceDeclaration.Prefix, namespaceDeclaration);
        }

        public DomNode LastNodeWritten
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
        public ObjectNode LastObjectNodeWritten
        {
            get
            {
                return LastNodeWritten as ObjectNode;
            }
        }
        public MemberNode LastMemberNodeWritten
        {
            get
            {
                return LastNodeWritten as MemberNode;
            }
        }



        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return _schemaContext;
            }
        }

        #endregion
    }
}
