// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Xaml;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class DomReader : XamlReader
    {
        Stack<StackFrame> _stack = new Stack<StackFrame>();

        XamlSchemaContext _schemaContext;
        public DomReader(ObjectNode rootObjectNode, XamlSchemaContext schemaContext)
        {
            StackFrame currentFrame = new StackFrame();
            _stack.Push(currentFrame);
            currentFrame.ObjectOrValueNode = rootObjectNode;
            _schemaContext = schemaContext;
        }

        public override bool IsEof
        {
            get { return _nodeType != XamlNodeType.None; }
        }

        public override XamlMember Member
        {
            get
            {
                if (_nodeType == XamlNodeType.StartMember)
                    return _stack.Peek().PropertyNodes.Current.Member;
                else
                    return null;
            }
        }

        public override NamespaceDeclaration Namespace
        {
            get
            {
                NamespaceDeclaration nsNode = _stack.Peek().NamespaceNodes.Current;
                if (nsNode != null)
                    return nsNode;
                else
                    return null;
            }
        }

        public override XamlNodeType NodeType
        {
            get { return _nodeType; }
        }

        XamlNodeType _nodeType;
        public override bool Read()
        {
            StackFrame currentFrame;

            _nodeType = XamlNodeType.None;

            while (_nodeType == XamlNodeType.None)
            {
                if (_stack.Count != 0)
                    currentFrame = _stack.Peek();
                else
                    return false;
                if (currentFrame.ValueNodes == null)
                {
                    if (currentFrame.PropertyNodes == null)
                    {
                        ObjectNode objectNode = currentFrame.ObjectOrValueNode as ObjectNode;
                        ValueNode valueNode = currentFrame.ObjectOrValueNode as ValueNode;
                        if (objectNode != null)
                        {
                            if (currentFrame.NamespaceNodes == null)
                            {
                                currentFrame.NamespaceNodes = objectNode.NamespaceNodes.Values.GetEnumerator();
                            }
                            if (currentFrame.NamespaceNodes != null)
                            {
                                currentFrame.NamespaceNodes.MoveNext();
                                if (currentFrame.NamespaceNodes.Current != null)
                                {
                                    _nodeType = XamlNodeType.NamespaceDeclaration;
                                    return true;
                                }
                            }
                            if (objectNode.IsGetObject)
                                _nodeType = XamlNodeType.GetObject;
                            else
                                _nodeType = XamlNodeType.StartObject;
                            currentFrame.PropertyNodes = objectNode.MemberNodes.GetEnumerator();
                            return true;
                        }
                        else if (valueNode != null)
                        {
                            _stack.Pop();
                        }
                    }
                    else
                    {
                        bool hasProperty = currentFrame.PropertyNodes.MoveNext();
                        if (hasProperty)
                        {
                            _nodeType = XamlNodeType.StartMember;
                            currentFrame.ValueNodes = currentFrame.PropertyNodes.Current.ItemNodes.GetEnumerator();
                            return true;
                        }
                        else
                        {
                            _nodeType = XamlNodeType.EndObject;
                            _stack.Pop();
                            return true;
                        }
                    }
                }
                else
                {
                    bool hasValue = currentFrame.ValueNodes.MoveNext();
                    if (hasValue)
                    {
                        StackFrame newFrame = new StackFrame();
                        newFrame.ObjectOrValueNode = currentFrame.ValueNodes.Current;
                        ObjectNode objectNode = newFrame.ObjectOrValueNode as ObjectNode;
                        ValueNode valueNode = newFrame.ObjectOrValueNode as ValueNode;
                        if (objectNode != null)
                        {
                            if (currentFrame.NamespaceNodes == null)
                            {
                                currentFrame.NamespaceNodes = objectNode.NamespaceNodes.Values.GetEnumerator();
                            }
                            if (currentFrame.NamespaceNodes != null)
                            {
                                currentFrame.NamespaceNodes.MoveNext();
                                if (currentFrame.NamespaceNodes.Current != null)
                                {
                                    _nodeType = XamlNodeType.NamespaceDeclaration;
                                    return true;
                                }
                            }
                            if (objectNode.IsGetObject)
                                _nodeType = XamlNodeType.GetObject;
                            else
                                _nodeType = XamlNodeType.StartObject;

                            _stack.Push(newFrame);
                            newFrame.PropertyNodes = objectNode.MemberNodes.GetEnumerator();
                            return true;
                        }
                        else if (valueNode != null)
                        {
                            _nodeType = XamlNodeType.Value;
                            _stack.Push(newFrame);
                            return true;
                        }

                    }
                    else
                    {
                        if (currentFrame.PropertyNodes != null && currentFrame.PropertyNodes.Current != null
                            && currentFrame.ValueNodes != null)
                        {
                            _nodeType = XamlNodeType.EndMember;
                            currentFrame.ValueNodes = null;
                            return true;
                        }
                        bool hasProperty = currentFrame.PropertyNodes.MoveNext();
                        if (hasProperty)
                        {
                            _nodeType = XamlNodeType.StartMember;
                            currentFrame.ValueNodes = currentFrame.PropertyNodes.Current.ItemNodes.GetEnumerator();
                            return true;
                        }
                        else
                        {
                            _nodeType = XamlNodeType.EndObject;
                            _stack.Pop();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override XamlSchemaContext SchemaContext
        {
            get { return _schemaContext; }
        }

        public override XamlType Type
        {
            get
            {
                if (_nodeType == XamlNodeType.StartObject)
                    return ((ObjectNode)_stack.Peek().ObjectOrValueNode).Type;
                else
                    return null;
            }
        }

        public override object Value
        {
            get
            {
                if (_nodeType == XamlNodeType.Value)
                {
                    StackFrame currentFrame = _stack.Peek();
                    ValueNode value = currentFrame.ObjectOrValueNode as ValueNode;
                    return value.Value;
                }
                else
                    return null;
            }
        }

        public ObjectNode CurrentObjectNode
        {
            get
            {
                if (_stack.Count > 0)
                {
                    StackFrame currentFrame = _stack.Peek();
                    if (currentFrame != null)
                    {
                        return currentFrame.ObjectOrValueNode as ObjectNode;
                    }
                }
                return null;
            }
        }

        public MemberNode CurrentMemberNode
        {
            get
            {
                if (_stack.Count > 0)
                {
                    StackFrame currentFrame = _stack.Peek();
                    if (currentFrame != null && currentFrame.PropertyNodes != null && currentFrame.PropertyNodes.Current != null)
                    {
                        return currentFrame.PropertyNodes.Current as MemberNode;
                    }
                }
                return null;
            }
        }
    }

    class StackFrame
    {
        public IEnumerator<NamespaceDeclaration> NamespaceNodes { get; set; }
        public ItemNode ObjectOrValueNode { get; set; }
        public IEnumerator<MemberNode> PropertyNodes { get; set; }
        public IEnumerator<ItemNode> ValueNodes { get; set; }
    }


}
