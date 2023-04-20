// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xaml;

    /// <summary>
    /// Base Node class 
    /// </summary>
    public class Node
    {
        /// <summary>
        /// test metadata information
        /// </summary>
        private PropertyBag _testMetadata = null;

        /// <summary>
        /// Gets the test metadata collection
        /// </summary>
        public PropertyBag TestMetadata
        {
            get
            {
                if (this._testMetadata == null)
                {
                    this._testMetadata = new PropertyBag();
                }

                return this._testMetadata;
            }
        }

        /// <summary>
        /// Gets or sets the Xaml node type
        /// </summary>
        public XamlNodeType XamlNodeType { get; set; }
    }

    /// <summary>
    /// Namespace node
    /// Wraps a NamespaceDeclaration
    /// </summary>
    public class NamespaceNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the NamespaceNode class
        /// </summary>
        /// <param name="prefix">xmlns prefix to use</param>
        /// <param name="ns">the namespace value</param>
        public NamespaceNode(string prefix, string ns)
        {
            this.NS = new NamespaceDeclaration(ns, prefix);
            this.XamlNodeType = XamlNodeType.NamespaceDeclaration;
        }

        /// <summary>
        /// Gets or sets the namespace declaration
        /// </summary>
        public NamespaceDeclaration NS { get; set; }

        /// <summary>
        /// String rep
        /// </summary>
        /// <returns>string rep</returns>
        public override string ToString()
        {
            string prefix = string.Empty;
            if (NS.Prefix != null && NS.Prefix.Length > 0)
            {
                prefix = ":" + NS.Prefix;
            }

            var value = String.Format(CultureInfo.InvariantCulture, @"NS xmlns{0}={1}", prefix, NS.Namespace);
            return value;
        }
    }

    /// <summary>
    /// StartObject node
    /// Wraps a XamlType 
    /// </summary>
    public class StartObject : Node
    {
        /// <summary>
        /// Initializes a new instance of the StartObject class.
        /// </summary>
        /// <param name="xamlType">xaml type information</param>
        public StartObject(XamlType xamlType)
        {
            this.XamlType = xamlType;
            this.XamlNodeType = XamlNodeType.StartObject;
        }

        /// <summary>
        /// Initializes a new instance of the StartObject class.
        /// </summary>
        /// <param name="type">clr type information</param>
        /// <param name="xsc">xaml schema context to use</param>
        public StartObject(Type type, XamlSchemaContext xsc)
        {
            this.XamlType = xsc.GetXamlType(type);
            this.XamlNodeType = XamlNodeType.StartObject;
        }

        /// <summary>
        /// Initializes a new instance of the StartObject class.
        /// </summary>
        /// <param name="type">clr type information</param>
        public StartObject(Type type)
        {
            this.XamlType = new XamlType(type, new XamlSchemaContext());
            this.XamlNodeType = XamlNodeType.StartObject;
        }

        /// <summary>
        /// Gets or sets the xaml type
        /// </summary>
        public XamlType XamlType { get; set; }

        /// <summary>
        /// String rep
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            //// This is to initialize the namespaces - 
            //// otherwise XamlMember.ToString() returns clr namespaces
            string ns = this.XamlType.PreferredXamlNamespace;
            var value = String.Format(
                CultureInfo.InvariantCulture,
                "SO {0} [{1}]",
                this.XamlType.Name,
                ns);

            return value;
        }
    }

    /// <summary>
    /// GetObject node
    /// </summary>
    public class GetObject : Node
    {
        /// <summary>
        /// Initializes a new instance of the GetObject class.
        /// </summary>
        public GetObject()
        {
            this.XamlNodeType = XamlNodeType.GetObject;
        }

        /// <summary>
        /// String rep
        /// </summary>
        /// <returns>string repersentation</returns>
        public override string ToString()
        {
            var value = String.Format(CultureInfo.InvariantCulture, "GO");
            return value;
        }
    }

    /// <summary>
    /// EndObject node
    /// </summary>
    public class EndObject : Node
    {
        /// <summary>
        /// Initializes a new instance of the EndObject class.
        /// </summary>
        public EndObject()
        {
            this.XamlNodeType = XamlNodeType.EndObject;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var value = String.Format(CultureInfo.InvariantCulture, "EO");
            return value;
        }
    }

    /// <summary>
    /// StartMember node
    /// Wraps a XamlMember
    /// </summary>
    public class StartMember : Node
    {
        /// <summary>
        /// Initializes a new instance of the StartMember class
        /// </summary>
        /// <param name="xamlMember">xaml member information</param>
        public StartMember(XamlMember xamlMember)
        {
            this.XamlMember = xamlMember;
            this.XamlNodeType = XamlNodeType.StartMember;
        }

        /// <summary>
        /// Initializes a new instance of the StartMember class.
        /// </summary>
        /// <param name="type">clr type information</param>
        /// <param name="memberName">clr member name</param>
        public StartMember(Type type, string memberName)
        {
            XamlType xamlType = new XamlSchemaContext().GetXamlType(type);
            this.XamlMember = xamlType.GetMember(memberName);

            if (this.XamlMember == null)
            {
                this.XamlMember = xamlType.GetAttachableMember(memberName);

                if (this.XamlMember == null)
                {
                    throw new DataTestException("Unable to create XamlMember type = " + type.Name + " Member = " + memberName);
                }
            }

            this.XamlNodeType = XamlNodeType.StartMember;
        }

        /// <summary>
        /// Initializes a new instance of the StartMember class.
        /// </summary>
        /// <param name="type">clr type information</param>
        /// <param name="memberName">clr member name</param>
        /// <param name="xsc">xaml shcmea context</param>
        public StartMember(Type type, string memberName, XamlSchemaContext xsc)
        {
            this.XamlMember = xsc.GetXamlType(type).GetMember(memberName);
            this.XamlNodeType = XamlNodeType.StartMember;
        }

        /// <summary>
        /// Gets or sets the Xaml member information
        /// </summary>
        public XamlMember XamlMember { get; set; }

        /// <summary>
        /// String rep
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            //// This is to initialize the namespaces - 
            //// otherwise XamlMember.ToString() returns clr namespaces
            string ns = this.XamlMember.PreferredXamlNamespace;

            var value = String.Format(CultureInfo.InvariantCulture, @"SM {0} [{1}]", this.XamlMember.Name, ns);
            return value;
        }
    }

    /// <summary>
    /// EndMember node
    /// </summary>
    public class EndMember : Node
    {
        /// <summary>
        /// Initializes a new instance of the EndMember class.
        /// </summary>
        public EndMember()
        {
            this.XamlNodeType = XamlNodeType.EndMember;
        }

        /// <summary>
        /// string value for logging
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var value = String.Format(CultureInfo.InvariantCulture, "EM");
            return value;
        }
    }

    /// <summary>
    /// Value node
    /// Wraps the value object
    /// </summary>
    public class ValueNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the ValueNode class.
        /// </summary>
        /// <param name="value">value contained by the class</param>
        public ValueNode(object value)
        {
            this.Value = value;
            this.XamlNodeType = XamlNodeType.Value;
        }

        /// <summary>
        /// Gets or sets the value property
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var value = String.Format(CultureInfo.InvariantCulture, "V {0}", this.Value.ToString());
            return value;
        }
    }
}
