// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Diag XamlReader
    /// </summary>
    public class DiagXamlReader : XamlReader
    {
        /// <summary> diagxaml path</summary>
        private readonly string _path;

        /// <summary> Xaml NodeList</summary>
        private XamlNodeList _nodeList;

        /// <summary> Xaml Reader </summary>
        private XamlReader _reader;

        /// <summary>Diag XamlParser </summary>
        private DiagXamlParser _parser = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagXamlReader"/> class.
        /// </summary>
        /// <param name="path">The path value.</param>
        public DiagXamlReader(string path)
        {
            this._path = path;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is EOF.
        /// </summary>
        /// <value><c>true</c> if this instance is EOF; otherwise, <c>false</c>.</value>
        public override bool IsEof
        {
            get
            {
                return _reader.IsEof;
            }
        }

        /// <summary>
        /// Gets the schema context.
        /// </summary>
        /// <value>The schema context.</value>
        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return _parser.SchemaContext;
            }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public override NamespaceDeclaration Namespace
        {
            get
            {
                return _reader.Namespace;
            }
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public override XamlNodeType NodeType
        {
            get
            {
                return _reader.NodeType;
            }
        }

        /// <summary>
        /// Gets the member.
        /// </summary>
        /// <value>The member.</value>
        public override XamlMember Member
        {
            get
            {
                return _reader.Member;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value .</value>
        public override object Value
        {
            get
            {
                return _reader.Value;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type value.</value>
        public override XamlType Type
        {
            get
            {
                return _reader.Type;
            }
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>bool value</returns>
        public override bool Read()
        {
            if (_nodeList == null)
            {
                _nodeList = new XamlNodeList(new XamlSchemaContext());
                _parser = new DiagXamlParser(_nodeList.Writer, _path);
                _parser.Parse();
                _reader = _nodeList.GetReader();
            }

            return _reader.Read();
        }
    }
}
