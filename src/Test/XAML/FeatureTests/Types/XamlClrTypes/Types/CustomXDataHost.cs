// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

 // Purpose: This is a Custom XdataHost, and the Verifiers to run the test case. 
namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// CustomXDataHost class
    /// </summary>
    [ContentProperty("XmlSerializer")]
    public class CustomXDataHost
    {
        /// <summary>
        /// My XmlSerializer
        /// </summary>
        private readonly MySerializer _xmlSerializer;

        /// <summary>
        /// Xml Document
        /// </summary>
        private XmlDocument _doc = new XmlDocument();

        /// <summary>
        /// Is inline XmlLoaded
        /// </summary>
        private bool _inlineXmlLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXDataHost"/> class.
        /// </summary>
        public CustomXDataHost()
        {
            _xmlSerializer = new MySerializer(this);
        }

        /// <summary>
        /// Gets the XML serializer.
        /// </summary>
        /// <value>The XML serializer.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable XmlSerializer
        {
            get
            {
                return _xmlSerializer;
            }
        }

        /// <summary>
        /// Gets the document.
        /// Inner document to hold xml content.
        /// </summary>
        /// <value>The document.</value>
        public XmlDocument Document
        {
            get
            {
                return _doc;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [inline XML loaded].
        /// </summary>
        /// <value><c>true</c> if [inline XML loaded]; otherwise, <c>false</c>.</value>
        public bool InlineXmlLoaded
        {
            get
            {
                return _inlineXmlLoaded;
            }

            set
            {
                _inlineXmlLoaded = value;
            }
        }

        /// <summary>
        /// Should serialize it only if loaded. 
        /// </summary>
        /// <returns>bool value.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeXmlSerializer()
        {
            return _inlineXmlLoaded;
        }
    }

    /// <summary>
    /// Another Custom XData Host using MySerializer2 as the IXmlSerializer property type.
    /// </summary>
    [ContentProperty("XmlSerializer")]
    public class CustomXDataHost2 : CustomXDataHost
    {
        /// <summary>
        /// My XmlSerializer
        /// </summary>
        private readonly MySerializer2 _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomXDataHost2"/> class.
        /// </summary>
        public CustomXDataHost2()
        {
            _xmlSerializer = new MySerializer2(this);
        }

        /// <summary>
        /// Gets the XML serializer.
        /// </summary>
        /// <value>The XML serializer.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new IXmlSerializable XmlSerializer
        {
            get
            {
                return _xmlSerializer;
            }
        }
    }

    /// <summary>
    /// IXmlSerializable that inplement the methods in current type.
    /// </summary>
    public class MySerializer : IXmlSerializable
    {
        /// <summary>
        /// Custom XDataHost
        /// </summary>
        private readonly CustomXDataHost _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySerializer"/> class.
        /// </summary>
        /// <param name="host">The host value.</param>
        public MySerializer(CustomXDataHost host)
        {
            this._host = host;
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            _host.Document.Save(writer);
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            _host.Document.Load(reader);
            _host.InlineXmlLoaded = true;
        }
    }

    /// <summary>
    /// IXmlSerializable that inplement the methods in IXmlSerializable interface.
    /// </summary>
    public class MySerializer2 : IXmlSerializable
    {
        /// <summary>
        /// Custom XDataHost2
        /// </summary>
        private readonly CustomXDataHost2 _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySerializer2"/> class.
        /// </summary>
        /// <param name="host">The host value.</param>
        public MySerializer2(CustomXDataHost2 host)
        {
            this._host = host;
        }

        /// <summary>
        /// IXmlSerializable .GetSchema
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// IXmlSerializable. WriteXml
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            _host.Document.Save(writer);
        }

        /// <summary>
        /// IXmlSerializable. ReadXml
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            _host.Document.Load(reader);
            _host.InlineXmlLoaded = true;
        }
    }
}
