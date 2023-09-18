// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Used for serializing & deserializing a hashtable
 ********************************************************************/
using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Test.Pict.MatrixOutput;

namespace Microsoft.Test.Effects
{
    public class HashtableSerializer : IXmlSerializable
    {
        #region Private Data

        private Hashtable _hashtable;

        #endregion

        /// <summary>
        /// Initialize using an empty hashtable.
        /// </summary>
        public HashtableSerializer()
        {
            this._hashtable = new Hashtable();
        }

        /// <summary>
        /// Initialize using the provided hashtable.
        /// </summary>
        /// <param name="hashtable">The input hashtable.</param>
        public HashtableSerializer(Hashtable hashtable)
        {
            this._hashtable = hashtable;
        }

        /// <summary>
        /// Given a hashtable and a filename, this function will serialize the hashtable and stores it in a file.
        /// </summary>
        /// <param name="hashtable">The input hashtable.</param>
        /// <param name="filename">The output file name.</param>
        public static void Serialize(Hashtable hashtable, string filename)
        {
            TextWriter writer = new StreamWriter(filename);
            HashtableSerializer hs = new HashtableSerializer(hashtable);
            XmlSerializer xs = new XmlSerializer(typeof(HashtableSerializer));
            xs.Serialize(writer, hs);
            writer.Close();
        }

        /// <summary>
        /// Given the name of a file which contains a serialized hashtable, this function will deserialize the file into a hashtable object.
        /// </summary>
        /// <param name="filename">The input file name.</param>
        /// <returns>The deserialized hashtable object.</returns>
        public static Hashtable Deserialize(string filename)
        {
            TextReader reader = new StreamReader(filename);
            XmlSerializer xs = new XmlSerializer(typeof(HashtableSerializer));
            HashtableSerializer hs = (HashtableSerializer)xs.Deserialize(reader);
            return hs._hashtable;
        }

        /// <summary>
        /// Implemented as a prerequisite for implementing IXmlSerializable interface.
        /// Always returns null.
        /// </summary>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Implemented as a prerequisite for implementing IXmlSerializable interface.
        /// </summary>
        /// <param name="reader">An xml reader.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.Read();
            reader.ReadStartElement("dictionary");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                string key = reader.ReadElementString("key");
                string value = reader.ReadElementString("value");
                reader.ReadEndElement();
                reader.MoveToContent();
                this._hashtable.Add(key, value);
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Implemented as a prerequisite for implementing IXmlSerializable interface.
        /// </summary>
        /// <param name="writer">An xml writer.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("dictionary");
            foreach (object key in this._hashtable.Keys)
            {
                object value = this._hashtable[key];
                writer.WriteStartElement("item");
                writer.WriteElementString("key", key.ToString());
                writer.WriteElementString("value", value.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}

