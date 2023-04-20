// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.IO;
using BamlDasm;
using System.Xaml.Schema;

namespace XamlTools.Baml
{
    public class BamlReader : XamlReader
    {
        BamlDisassembler _bdasm;
        //XamlContext xamlContext;
        XamlNodePipe _xamlNodeQueue;
        XamlWriter _queueWriter;
        XamlReader _queueReader;
        public BamlReader(string filename)
        {
            _xamlNodeQueue = new XamlNodePipe();
            _queueReader = _xamlNodeQueue.Reader;
            _queueWriter = _xamlNodeQueue.Writer;

            //xamlContext = new XamlContext();
            FileStream filestream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BamlBinaryReader reader = new BamlBinaryReader(filestream);

            _bdasm = new BamlDisassembler(reader);
            _bdasm.ReadFormatVersion();
        }

        public override bool IsEof
        {
            get { return (_record == null); }
        }

        public override XamlNamespace Namespace
        {
            get { return null; }
        }

        struct BamlTypeInfo
        {
            public BamlTypeInfo(int assembly, string typeName)
            {
                Assembly = assembly;
                TypeName = typeName;
            }
            public int Assembly;
            public string TypeName;
        }
        struct BamlPropertyInfo
        {
            public BamlPropertyInfo(string propertyName)
            {
                PropertyName = propertyName;
            }
            public string PropertyName;
        }
        struct BamlAssemblyInfo
        {
            public BamlAssemblyInfo(string assemblyName)
            {
                AssemblyName = assemblyName;
            }
            public string AssemblyName;
        }
        Dictionary<Int16, BamlTypeInfo> _bamlTypeInfos = new Dictionary<Int16, BamlTypeInfo>();
        Dictionary<Int16, BamlPropertyInfo> _bamlPropertyInfos = new Dictionary<Int16, BamlPropertyInfo>();
        Dictionary<Int16, BamlAssemblyInfo> _bamlAssemblyInfos = new Dictionary<Int16, BamlAssemblyInfo>();
        bool _isPropertyStart = false;

        bool _useQueue = false;
        BamlDasmRecord _record;
        public override bool Read()
        {
            _isPropertyStart = false;
            if (_useQueue)
                _useQueue = _queueReader.Read();
            if (!_xamlNodeQueue.IsEmpty)
            {
                _useQueue = true;
            }
            else
            {
                _useQueue = false;
                _record = _bdasm.ReadRecord();
                if (_record == null || _record.Id == BamlRecordType.DocumentEnd)
                    return false;
                else
                    return true;
            }
            return _useQueue;
        }

        public override XamlNodeType NodeType
        {

            get
            {
                if (_useQueue)
                    return _queueReader.NodeType;
                
                bool firstTime = true;
                bool readAnother = false;

                while (readAnother || firstTime)
                {
                    if (readAnother)
                    {
                        _record = _bdasm.ReadRecord();
                        readAnother = false;
                    }

                    firstTime = false;
                    switch (_record.Id)
                    {
                        case BamlRecordType.DocumentStart:
                            readAnother = true;
                            break;
                        case BamlRecordType.DocumentEnd:
                            break;

                        case BamlRecordType.ContentProperty:
                            readAnother = true;
                            //the first field list the attributeID of the content property.
                            break;

                        case BamlRecordType.PropertyWithConverter:
                            _isPropertyStart = true;
                            _queueWriter.WriteText("*value*");
                            _queueWriter.WriteEndProperty();
                            return XamlNodeType.StartProperty;

                        case BamlRecordType.Property:
                        case BamlRecordType.PropertyCustom:
                            //
                            break;

                        case BamlRecordType.PropertyArrayStart:
                        case BamlRecordType.PropertyComplexStart:
                        case BamlRecordType.PropertyIDictionaryStart:
                        case BamlRecordType.PropertyIListStart:
                            _isPropertyStart = true;
                            return XamlNodeType.StartProperty;
                        case BamlRecordType.PropertyArrayEnd:
                        case BamlRecordType.PropertyComplexEnd:
                        case BamlRecordType.PropertyIDictionaryEnd:
                        case BamlRecordType.PropertyIListEnd:
                            return XamlNodeType.EndProperty;

                        case BamlRecordType.ElementStart:
                            return XamlNodeType.StartObject;
                        case BamlRecordType.ElementEnd:
                            return XamlNodeType.EndObject;

                        case BamlRecordType.AssemblyInfo:
                            _bamlAssemblyInfos.Add((Int16)(_record.Fields[0].Value), new BamlAssemblyInfo((string)(_record.Fields[1].Value)));
                            readAnother = true;
                            break;
                        case BamlRecordType.TypeInfo:
                            _bamlTypeInfos.Add((Int16)(_record.Fields[0].Value), new BamlTypeInfo((Int16)(_record.Fields[1].Value), (string)(_record.Fields[2].Value)));
                            readAnother = true;
                            break;
                        case BamlRecordType.AttributeInfo:
                            _bamlPropertyInfos.Add((Int16)(_record.Fields[0].Value), new BamlPropertyInfo((string)(_record.Fields[3].Value)));
                            readAnother = true;
                            break;
                        case BamlRecordType.PIMapping:
                            readAnother = true;
                            break;
                        case BamlRecordType.XmlnsProperty:
                            return XamlNodeType.Namespace;
                        case BamlRecordType.ConnectionId:
                            //
                            readAnother = true;
                            break;
                        default:
                            Console.WriteLine(_record.Id);
                            return XamlNodeType.None;
                    }
                }
                return XamlNodeType.None;
            }
        }

        public override string Prefix
        {
            get {
                if (_useQueue)
                    return _queueReader.Prefix;
                
                return string.Empty; }
        }


        public override string Text
        {
            get {
                if (_useQueue)
                    return _queueReader.Text;

                return null; }
        }

        public override XamlType Type
        {
            get
            {
                if (_useQueue)
                    return _queueReader.Type;

                if (_record.Id == BamlRecordType.ElementStart)
                {
                    XaslType newType = new XaslType();
                    Int16 typeId = (Int16)(_record.Fields[0].Value);
                    newType.Name = _bamlTypeInfos.ContainsKey(typeId) ? _bamlTypeInfos[typeId].TypeName : "**KnownType?**";
                    return newType;
                }
                else
                    return null;
            }
        }
        public override XamlProperty Property
        {
            get
            {
                if (_useQueue)
                    return _queueReader.Property;

                if (_isPropertyStart)
                {
                    XaslProperty newProp = new XaslProperty();
                    //Int16 typeId = (Int16)(record.Fields[0].Value);
                    newProp.Name = "**propName**";
                    return newProp;
                }
                else
                    return null;
            }
        }
    }
}
