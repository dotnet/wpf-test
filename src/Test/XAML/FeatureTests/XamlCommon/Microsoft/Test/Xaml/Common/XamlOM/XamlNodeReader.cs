// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xaml;

    /// <summary>
    /// XamlNodeReader - XamlReader implementation that
    /// takes a NodeList and reads the nodes.
    /// </summary>
    public class XamlNodeReader : XamlReader
    {
        /// <summary>
        /// current index
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// have you reached end of file
        /// </summary>
        private bool _isEof;

        /// <summary>
        /// xaml type information
        /// </summary>
        private XamlType _type;

        /// <summary>
        /// xaml member information
        /// </summary>
        private XamlMember _member;

        /// <summary>
        /// namespace information
        /// </summary>
        private NamespaceDeclaration _namespaceDecl;

        /// <summary>
        /// Current xaml node type
        /// </summary>
        private XamlNodeType _nodeType;

        /// <summary>
        /// current value (in value node)
        /// </summary>
        private object _value;

        /// <summary>
        /// xaml schema context
        /// </summary>
        private XamlSchemaContext _schemaContext;

        /// <summary>
        /// Initializes a new instance of the XamlNodeReader class.
        /// </summary>
        /// <param name="nodeList">input nodelist</param>
        /// <param name="context">xaml schema context</param>
        public XamlNodeReader(NodeList nodeList, XamlSchemaContext context)
        {
            this.NodeList = nodeList;
            this._schemaContext = context;
            this._currentIndex = 0;
        }

        /// <summary>
        /// Gets or sets the NodeList
        /// input NodeList to read from
        /// </summary>
        public NodeList NodeList { get; set; }

        /// <summary>
        /// Gets a value indicating whether eof is reached
        /// </summary>
        public override bool IsEof
        {
            get { return this._isEof; }
        }

        /// <summary>
        /// Gets the Member property
        /// </summary>
        public override XamlMember Member
        {
            get { return this._member; }
        }

        /// <summary>
        /// Gets the Namespace property
        /// </summary>
        public override NamespaceDeclaration Namespace
        {
            get { return this._namespaceDecl; }
        }

        /// <summary>
        /// Gets the NodeType property
        /// </summary>
        public override XamlNodeType NodeType
        {
            get { return this._nodeType; }
        }

        /// <summary>
        /// Gets the SchemaContext property
        /// </summary>
        public override XamlSchemaContext SchemaContext
        {
            get { return this._schemaContext; }
        }

        /// <summary>
        /// Gets the Type property
        /// </summary>
        public override XamlType Type
        {
            get { return this._type; }
        }

        /// <summary>
        /// Gets the Value property
        /// </summary>
        public override object Value
        {
            get { return this._value.ToString(); }
        }

        /// <summary>
        /// Read the next node
        /// </summary>
        /// <returns>true if read</returns>
        public override bool Read()
        {
            if (this.NodeList == null)
            {
                throw new ArgumentNullException("NodeList", "No input NodeList provided to the XamlNodeReader");
            }

            if (_currentIndex >= this.NodeList.Count)
            {
                this._isEof = true;
                return false;
            }
            else
            {
                Node node = (Node)this.NodeList[_currentIndex];
                this._nodeType = node.XamlNodeType;

                switch (node.XamlNodeType)
                {
                    case XamlNodeType.NamespaceDeclaration:
                        var ns = node as NamespaceNode;
                        this._namespaceDecl = ns.NS;
                        break;

                    case XamlNodeType.StartObject:
                        var startObject = node as StartObject;
                        this._type = startObject.XamlType;
                        break;

                    case XamlNodeType.GetObject:
                        var getObject = node as GetObject;
                        break;

                    case XamlNodeType.EndObject:
                        break;

                    case XamlNodeType.StartMember:
                        var startMember = node as StartMember;
                        this._member = startMember.XamlMember;
                        break;

                    case XamlNodeType.EndMember:
                        break;

                    case XamlNodeType.Value:
                        var value = node as ValueNode;
                        this._value = value.Value;
                        break;

                    default:
                        throw new DataTestException("Unkown node type" + node.GetType());
                }

                _currentIndex++;
                return true;
            }
        }
    }
}
