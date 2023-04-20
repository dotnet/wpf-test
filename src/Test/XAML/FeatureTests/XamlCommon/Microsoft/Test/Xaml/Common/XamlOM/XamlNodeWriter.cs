// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    /// <summary>
    /// XamlNodeWriter - XamlWriter implementation that writes
    /// to a NodeList
    /// </summary>
    public class XamlNodeWriter : XamlWriter
    {
        /// <summary>
        /// schem acontext to use
        /// </summary>
        private XamlSchemaContext _schemaContext;

        /// <summary>
        /// Initializes a new instance of the XamlNodeWriter class
        /// </summary>
        /// <param name="schemaContext">Xaml schema context</param>
        public XamlNodeWriter(XamlSchemaContext schemaContext)
            : this(schemaContext, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XamlNodeWriter class
        /// </summary>
        /// <param name="schemaContext">schema context to use</param>
        /// <param name="innerWriter">inner writer to use</param>
        public XamlNodeWriter(XamlSchemaContext schemaContext, XamlWriter innerWriter)
        {
            this._schemaContext = schemaContext;
            this.Result = new NodeList(Guid.NewGuid().ToString());
            this.InnerWriter = innerWriter;
        }

        /// <summary>
        /// Gets or sets the Resulting NodeList
        /// </summary>
        public NodeList Result { get; set; }

        /// <summary>
        /// Gets the Xaml schema context
        /// </summary>
        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return this._schemaContext;
            }
        }

        /// <summary>
        /// Gets or sets the inner writer
        /// </summary>
        public XamlWriter InnerWriter { get; set; }

        /// <summary>
        /// Write Namespace out
        /// </summary>
        /// <param name="namespaceDeclaration">the namespace declaration</param>
        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            //// We don't validate the namespace node //
            //// this.Result.Add(new Namespace() { NS = namespaceDeclaration });
            if (InnerWriter != null)
            {
                InnerWriter.WriteNamespace(namespaceDeclaration);
            }
        }

        /// <summary>
        /// Write a start object
        /// </summary>
        /// <param name="type">xaml type of the object</param>
        public override void WriteStartObject(XamlType type)
        {
            this.Result.Add(new StartObject(type));
            if (InnerWriter != null)
            {
                InnerWriter.WriteStartObject(type);
            }
        }

        /// <summary>
        /// Write get object node
        /// </summary>
        public override void WriteGetObject()
        {
            this.Result.Add(new GetObject());
            if (InnerWriter != null)
            {
                InnerWriter.WriteGetObject();
            }
        }

        /// <summary>
        /// Write end object node
        /// </summary>
        public override void WriteEndObject()
        {
            this.Result.Add(new EndObject());
            if (InnerWriter != null)
            {
                InnerWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Write start member node
        /// </summary>
        /// <param name="property">member property</param>
        public override void WriteStartMember(XamlMember property)
        {
            this.Result.Add(new StartMember(property));
            if (InnerWriter != null)
            {
                InnerWriter.WriteStartMember(property);
            }
        }

        /// <summary>
        /// Write end member node
        /// </summary>
        public override void WriteEndMember()
        {
            this.Result.Add(new EndMember());
            if (InnerWriter != null)
            {
                InnerWriter.WriteEndMember();
            }
        }

        /// <summary>
        /// Write the value node
        /// </summary>
        /// <param name="value">value contents</param>
        public override void WriteValue(object value)
        {
            this.Result.Add(new ValueNode(value));
            if (InnerWriter != null)
            {
                InnerWriter.WriteValue(value);
            }
        }
    }
}
