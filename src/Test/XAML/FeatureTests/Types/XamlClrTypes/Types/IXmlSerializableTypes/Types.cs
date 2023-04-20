// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Test.Xaml.Types.IXmlSerializableTypes
{
    internal static class XmlHelper
    {
        public static string ReadXml(XmlReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto,
                                                          Indent = true,
                                                          OmitXmlDeclaration = true,
                                                      };
            XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings);
            while (!reader.EOF)
            {
                writer.WriteNode(reader, true);
                writer.Flush();
            }

            return stringBuilder.ToString();
        }

        public static void WriteXml(XmlWriter writer, string data)
        {
            XmlReader reader = XmlReader.Create(new StringReader(data), new XmlReaderSettings()
                                                                            {
                                                                                ConformanceLevel = ConformanceLevel.Auto
                                                                            });
            while (!reader.EOF)
            {
                writer.WriteNode(reader, true);
            }
        }
    }

    public class TypeContaingingIXmlSerializableProperty
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new SimpleIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class SimpleIXmlSerializable : IXmlSerializable
    {
        private readonly TypeContaingingIXmlSerializableProperty _data;

        public SimpleIXmlSerializable(TypeContaingingIXmlSerializableProperty data)
        {
            this._data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this._data.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this._data.data);
        }

        #endregion
    }

    public class TypeContaingingNestedIXmlSerializableProperty
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new NestedIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class TypeContaingingNestedIXmlSerializablePropertyNotMarkedContent
    {
        public string data;
        private IXmlSerializable _loader;

        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new NestedIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class NestedIXmlSerializable : IXmlSerializable
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new Nested2IXmlSerializable(this);
                }
                return this._loader;
            }
        }

        public NestedIXmlSerializable(TypeContaingingNestedIXmlSerializableProperty data)
        {
            this.data = data.data;
        }

        public NestedIXmlSerializable(TypeContaingingNestedIXmlSerializablePropertyNotMarkedContent data)
        {
            this.data = data.data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this.data);
        }

        #endregion
    }

    public class Nested2IXmlSerializable : IXmlSerializable
    {
        private readonly NestedIXmlSerializable _data;

        public Nested2IXmlSerializable(NestedIXmlSerializable data)
        {
            this._data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this._data.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this._data.data);
        }

        #endregion
    }

    public class MultipleIxmlProperties
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new MultipleIXmlSerializable(this);
                }
                return this._loader;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty2
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new MultipleIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class MultipleIXmlSerializable : IXmlSerializable
    {
        private readonly MultipleIxmlProperties _data;

        public MultipleIXmlSerializable(MultipleIxmlProperties data)
        {
            this._data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this._data.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this._data.data);
        }

        #endregion
    }

    public class TypeContaingingIXmlDictionary
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new DictionaryIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class DictionaryIXmlSerializable : IXmlSerializable, IDictionary
    {
        private readonly TypeContaingingIXmlDictionary _data;
        private readonly Dictionary<object, object> _dictionary = new Dictionary<object, object>();

        public DictionaryIXmlSerializable(TypeContaingingIXmlDictionary data)
        {
            this._data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this._data.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this._data.data);
        }

        #endregion

        #region IDictionary Members

        public void Add(object key, object value)
        {
            _dictionary.Add(key, value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(object key)
        {
            return false;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IDictionary) _dictionary).IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary) _dictionary).IsReadOnly;
            }
        }

        public ICollection Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        public void Remove(object key)
        {
            _dictionary.Remove(key);
        }

        public ICollection Values
        {
            get
            {
                return _dictionary.Values;
            }
        }

        public object this[object key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection) _dictionary).CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                return ((ICollection) _dictionary).Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection) _dictionary).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection) _dictionary).SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dictionary).GetEnumerator();
        }

        #endregion
    }

    public class TypeContaingingIXmlCollection
    {
        public string data;
        private IXmlSerializable _loader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable IxmlProperty
        {
            get
            {
                if (this._loader == null)
                {
                    this._loader = new CollectionIXmlSerializable(this);
                }
                return this._loader;
            }
        }
    }

    public class CollectionIXmlSerializable : IXmlSerializable, ICollection
    {
        private readonly TypeContaingingIXmlCollection _data;
        private readonly Dictionary<object, object> _dictionary = new Dictionary<object, object>();

        public CollectionIXmlSerializable(TypeContaingingIXmlCollection data)
        {
            this._data = data;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this._data.data = XmlHelper.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlHelper.WriteXml(writer, this._data.data);
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection) _dictionary).CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                return ((ICollection) _dictionary).Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection) _dictionary).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection) _dictionary).SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dictionary).GetEnumerator();
        }

        #endregion
    }
}
