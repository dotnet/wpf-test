// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xaml;
    using System.Xml;

    /// <summary>
    /// Writer that writes out a NodeList to Xml (using xmlwriter)
    /// - The nodes can have test metadata properties that hint on
    /// how the NodeListXmlWriter should behave. For eg. WriteAsAttribute
    /// will force the writer to write that node as an attribute.
    /// </summary>
    public class NodeListXmlWriter
    {
        /// <summary>
        /// WriteAsAttributeProperty tag
        /// </summary>
        public const string WriteAsAttributeProperty = "WriteAsAttribute";

        /// <summary>
        /// XamlNamespaceProperty tag
        /// </summary>
        public const string XamlNamespaceProperty = "XamlNamespace";

        /// <summary>
        /// SkipWritingTagProperty tag
        /// </summary>
        public const string SkipWritingTagProperty = "SkipWritingTag";

        /// <summary>
        /// DoNotExpectTagProperty tag
        /// </summary>
        public const string DoNotExpectTagProperty = "DoNotExpectTag";

        /// <summary>
        ///  inner xml writer
        /// </summary>
        private XmlWriter _innerXmlWriter;

        /// <summary>
        /// string writer
        /// </summary>
        private StringWriter _stringWriter;

        /// <summary>
        /// stack to keep track of end objects to skip
        /// </summary>
        private Stack<bool> _skipEndObject = new Stack<bool>();

        /// <summary>
        /// stack to keep track of end members to skip
        /// </summary>
        private Stack<bool> _skipEndMember = new Stack<bool>();

        /// <summary>
        /// stack to keep track of members written as xml attribute
        /// </summary>
        private Stack<bool> _memberAsAttribute = new Stack<bool>();

        /// <summary>
        /// namespaces that are pending write
        /// </summary>
        private List<NamespaceNode> _pendingNamespaces = new List<NamespaceNode>();

        /// <summary>
        /// Initializes a new instance of the NodeListXmlWriter class.
        /// </summary>
        public NodeListXmlWriter()
        {
            _stringWriter = new StringWriter();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            this._innerXmlWriter = XmlWriter.Create(_stringWriter, xmlWriterSettings);
            this.ExpectedNodeList = new NodeList();
        }

        /// <summary>
        /// Gets or sets the ExpectedNodeList property
        /// While writing the nodes, create a Node list
        /// that should be expected when reading the Xaml using
        /// XamlXmlReader.
        /// </summary>
        public NodeList ExpectedNodeList { get; set; }

        /// <summary>
        /// Gets the Result property
        /// The resulting Xml created as string
        /// </summary>
        public string Result
        {
            get
            {
                if (this._stringWriter != null)
                {
                    this._innerXmlWriter.Flush();
                    return _stringWriter.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Write as attribute property
        /// </summary>
        /// <param name="props">property collection</param>
        /// <returns>true if need to write as attribute</returns>
        public static bool WriteAsAttribute(PropertyBag props)
        {
            return props.GetBoolProperty(WriteAsAttributeProperty);
        }

        /// <summary>
        /// Get xaml namespace from properties
        /// </summary>
        /// <param name="props">property collection</param>
        /// <returns>string xaml namespace value</returns>
        public static string GetXamlNs(PropertyBag props)
        {
            XNamespace value = (XNamespace)props[XamlNamespaceProperty];
            return value == null ? (string)null : value.NamespaceName;
        }

        /// <summary>
        /// Get the skip writing tag property
        /// </summary>
        /// <param name="props">property collection</param>
        /// <returns>true if need to skip</returns>
        public static bool GetSkipWritingTag(PropertyBag props)
        {
            return props.GetBoolProperty(SkipWritingTagProperty);
        }

        /// <summary>
        /// Get the dont write tag property
        /// </summary>
        /// <param name="props">property collection</param>
        /// <returns>true if we shouldnt write out the tag</returns>
        public static bool GetDoNotExpectTag(PropertyBag props)
        {
            return props.GetBoolProperty(DoNotExpectTagProperty);
        }

        /// <summary>
        /// Write a node 
        /// </summary>
        /// <param name="node">node to write</param>
        public void WriteNode(Node node)
        {
            switch (node.XamlNodeType)
            {
                case XamlNodeType.NamespaceDeclaration:
                    WriteNamespace(node as NamespaceNode);
                    break;

                case XamlNodeType.StartObject:
                    WriteStartObject(node as StartObject);
                    break;

                case XamlNodeType.GetObject:
                    WriteGetObject(node as GetObject);
                    break;

                case XamlNodeType.EndObject:
                    WriteEndObject(node as EndObject);
                    break;

                case XamlNodeType.StartMember:
                    WriteStartMember(node as StartMember);
                    break;

                case XamlNodeType.EndMember:
                    WriteEndMember(node as EndMember);
                    break;

                case XamlNodeType.Value:
                    WriteValue(node as ValueNode);
                    break;

                case XamlNodeType.None:

                default:
                    throw new InvalidOperationException("Unknon XamlNodeType" + node.XamlNodeType);
            }
        }

        /// <summary>
        /// Write a given NodeList
        /// </summary>
        /// <param name="nodes">Node list to writ eout</param>
        /// <returns>string rep of node stream</returns>
        public string WriteNodes(NodeList nodes)
        {
            foreach (Node node in nodes)
            {
                this.WriteNode(node);
            }

            return this.Result;
        }

        /// <summary>
        /// Write a namespace node
        /// </summary>
        /// <param name="ns">namespace node</param>
        private void WriteNamespace(NamespaceNode ns)
        {
            this._pendingNamespaces.Add(ns);
        }

        /// <summary>
        /// Write StartObject node
        /// </summary>
        /// <param name="startObject">Start Object node</param>
        private void WriteStartObject(StartObject startObject)
        {
            AddToExpected(startObject);

            _innerXmlWriter.WriteStartElement(startObject.XamlType.Name, startObject.XamlType.PreferredXamlNamespace);
            this._skipEndObject.Push(false);

            //// Write all pending namespaces 
            WriteOutNamespaces();
        }

        /// <summary>
        /// Write GetObject node
        /// </summary>
        /// <param name="getObject">GetObject node</param>
        private void WriteGetObject(GetObject getObject)
        {
            AddToExpected(getObject);

            //// dont do anything //
            this._skipEndObject.Push(true);
        }

        /// <summary>
        /// Write EndObject node
        /// </summary>
        /// <param name="endObject">End Object node</param>
        private void WriteEndObject(EndObject endObject)
        {
            AddToExpected(endObject);

            bool skip = this._skipEndObject.Pop();
            if (!skip)
            {
                _innerXmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Write StartMember node
        /// </summary>
        /// <param name="startMember">Start Member node</param>
        private void WriteStartMember(StartMember startMember)
        {
            AddToExpected(startMember);

            bool skipTag = GetSkipWritingTag(startMember.TestMetadata);
            if (IsImplicit(startMember.XamlMember))
            {
                skipTag = true;
            }

            this._skipEndMember.Push(skipTag);
            if (skipTag)
            {
                return;
            }

            XamlMember member = startMember.XamlMember;

            //// output only member name for directives //
            string fullName = member.Name;
            if (!member.IsDirective)
            {
                fullName = member.DeclaringType.Name + "." + member.Name;
            }

            //// can't write content property as an attribute
            bool writeMemberAsAttribute = WriteAsAttribute(startMember.TestMetadata);
            this._memberAsAttribute.Push(writeMemberAsAttribute);

            if (writeMemberAsAttribute)
            {
                _innerXmlWriter.WriteStartAttribute(fullName, member.PreferredXamlNamespace);
            }
            else
            {
                _innerXmlWriter.WriteStartElement(fullName, member.PreferredXamlNamespace);
            }
        }

        /// <summary>
        /// Write EndMember node
        /// </summary>
        /// <param name="endMember">End Memember node</param>
        private void WriteEndMember(EndMember endMember)
        {
            AddToExpected(endMember);

            bool skipTag = this._skipEndMember.Pop();
            if (!skipTag)
            {
                bool writeMemberAsAttribute = _memberAsAttribute.Pop();
                if (writeMemberAsAttribute)
                {
                    _innerXmlWriter.WriteEndAttribute();
                }
                else
                {
                    _innerXmlWriter.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Write down a value node
        /// </summary>
        /// <param name="value">value node</param>
        private void WriteValue(ValueNode value)
        {
            AddToExpected(value);

            if (value.Value is XmlQualifiedName)
            {
                XmlQualifiedName qname = (XmlQualifiedName)value.Value;
                _innerXmlWriter.WriteQualifiedName(qname.Name, qname.Namespace);
            }
            else
            {
                _innerXmlWriter.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// Write out all pending namespaces.
        /// </summary>
        private void WriteOutNamespaces()
        {
            //// write out the namespaces //
            foreach (NamespaceNode ns in this._pendingNamespaces)
            {
                _innerXmlWriter.WriteAttributeString("xmlns", ns.NS.Prefix, null, ns.NS.Namespace);
            }

            this._pendingNamespaces.Clear();
        }

        /// <summary>
        /// Implicit directives are part of the node-stream, but not the textual representation
        /// </summary>
        /// <param name="xamlMember">xaml member information</param>
        /// <returns>true if member is implicit</returns>
        private bool IsImplicit(XamlMember xamlMember)
        {
            return xamlMember.IsDirective &&
                (xamlMember == XamlLanguage.Items ||
                 xamlMember == XamlLanguage.Initialization ||
                 xamlMember == XamlLanguage.PositionalParameters ||
                 xamlMember == XamlLanguage.UnknownContent);
        }

        /// <summary>
        /// Add to the expected node list - what to 
        /// expect when reading the xaml using XamlXmlReader
        /// </summary>
        /// <param name="node">node to add</param>
        private void AddToExpected(Node node)
        {
            if (!GetDoNotExpectTag(node.TestMetadata))
            {
                this.ExpectedNodeList.Add(node);
            }
        }
    }
}
