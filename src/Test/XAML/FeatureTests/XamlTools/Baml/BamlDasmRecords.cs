// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace BamlDasm
{
    // Some of the records are "Not Implemented" this means there really is
    // such record.  Some of the records are "Untested" because I was not able
    // actually find an example usage of that record type in generated BAML.
    // These record formats are implemented in this tool but are commented out
    // to draw attention to the fact that the record type may actually be obsolete.
    // If you hit a usage of one of these records, all you should need to do is
    // uncomment the record and recompile.
    public enum BamlRecordType : byte
    {
        Unknown = 0,
        DocumentStart,              // 1
        DocumentEnd,                // 2
        ElementStart,               // 3
        ElementEnd,                 // 4
        Property,                   // 5
        PropertyCustom,             // 6
        PropertyComplexStart,       // 7
        PropertyComplexEnd,         // 8
        PropertyArrayStart,         // 9
        PropertyArrayEnd,           // 10
        PropertyIListStart,         // 11
        PropertyIListEnd,           // 12
        PropertyIDictionaryStart,   // 13
        PropertyIDictionaryEnd,     // 14
        LiteralContent,             // 15
        Text,                       // 16
        TextWithConverter,          // 17
        RoutedEvent,                // 18       Untested because never seen this record in actual BAML
        ClrEvent,                   // 19       NOT IMPLEMENTED in Avalon
        XmlnsProperty,              // 20
        XmlAttribute,               // 21       NOT IMPLEMENTED in Avalon
        ProcessingInstruction,      // 22       NOT IMPLEMENTED in Avalon
        Comment,                    // 23       NOT IMPLEMENTED in Avalon
        DefTag,                     // 24       NOT IMPLEMENTED in Avalon
        DefAttribute,               // 25
        EndAttributes,              // 26       NOT IMPLEMENTED in Avalon
        PIMapping,                  // 27
        AssemblyInfo,               // 28
        TypeInfo,                   // 29
        TypeSerializerInfo,         // 30       Untested because never seen this record in actual BAML
        AttributeInfo,              // 31
        StringInfo,                 // 32
        PropertyStringReference,    // 33       Untested because never seen this record in actual BAML
        PropertyTypeReference,      // 34
        PropertyWithExtension,      // 35
        PropertyWithConverter,      // 36
        DeferableContentStart,      // 37
        DefAttributeKeyString,      // 38
        DefAttributeKeyType,        // 39
        KeyElementStart,            // 40
        KeyElementEnd,              // 41
        ConstructorParametersStart, // 42
        ConstructorParametersEnd,   // 43
        ConstructorParameterType,   // 44
        ConnectionId,               // 45
        ContentProperty,            // 46
        NamedElementStart,          // 47
        StaticResourceStart,        // 48
        StaticResourceEnd,          // 49
        StaticResourceId,           // 50
        TextWithId,                 // 51
        PresentationOptionsAttribute,// 52
        LineNumberAndPosition,      // 53
        LinePosition,               // 54
        OptimizedStaticResource,     // 55,
        PropertyWithStaticResourceId,// 56,
        LastRecordType
    }


    public class BamlDasmRecord
    {

        public BamlDasmRecord(BamlRecordType recordId, bool isVariableSize, BamlDasmField[] fields)
        {
            if(null == fields)
            {
                throw new ArgumentNullException("fields");
            }
            if(recordId <= BamlRecordType.Unknown || recordId >= BamlRecordType.LastRecordType)
            {
                throw new ArgumentOutOfRangeException("BamlRecordType recordId");
            }
            _recordId = recordId;
            _isVariableSize = isVariableSize;
            _fields = fields;
        }

        public BamlDasmRecord Clone()
        {
            return new BamlDasmRecord( _recordId, _isVariableSize, (BamlDasmField[])_fields.Clone());
        }

        public BamlRecordType Id
        {
            get { return _recordId; }
        }

        public bool IsVariableSize
        {
            get { return _isVariableSize; }
        }

        public BamlDasmField[] Fields
        {
            get { return _fields; }
        }

        private BamlRecordType _recordId;
        private bool _isVariableSize;
        private BamlDasmField[] _fields;
    }

    class StaticBamlRecords
    {
        private static BamlDasmRecord[] s_records;

        public static BamlDasmRecord GetRecord(BamlRecordType recordId)
        {
            BamlDasmRecord rec = s_records[(int)recordId];
            if (rec == null)
                return null;
            else
                return rec.Clone();
        }

        static StaticBamlRecords()
        {
            BamlRecordType recordId;
            BamlDasmField[] fields;

            s_records = new BamlDasmRecord[(int)BamlRecordType.LastRecordType];

            // DocumentStart = 1
            recordId = BamlRecordType.DocumentStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.LoadAsync),
                new BamlDasmField(BamlFieldType.MaxAsyncRecords),
                new BamlDasmField(BamlFieldType.DebugBamlStream),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // DocumentEnd = 2
            recordId = BamlRecordType.DocumentEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ElementStart = 3
            recordId = BamlRecordType.ElementStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.FlagsByte),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ElementEnd = 4
            recordId = BamlRecordType.ElementEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // Property = 5
            recordId = BamlRecordType.Property;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // PropertyCustom = 6
            recordId = BamlRecordType.PropertyCustom;
            fields = new BamlDasmField[]             // same interface as Property 
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.SerializerTypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // PropertyComplexStart = 7
            recordId = BamlRecordType.PropertyComplexStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyComplexEnd = 8
            recordId = BamlRecordType.PropertyComplexEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyArrayStart = 9
            recordId = BamlRecordType.PropertyArrayStart;
            fields = new BamlDasmField[]           // based on ComplexPropertyStart
            {
                new BamlDasmField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyArrayEnd = 10
            recordId = BamlRecordType.PropertyArrayEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyIListStart = 11
            recordId = BamlRecordType.PropertyIListStart;
            fields = new BamlDasmField[]           // based on ComplexPropertyStart
            {
                new BamlDasmField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyIListEnd = 12
            recordId = BamlRecordType.PropertyIListEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyIDictionaryStart = 13
            recordId = BamlRecordType.PropertyIDictionaryStart;
            fields = new BamlDasmField[]           // based on PropertyComplexStart
            {
                new BamlDasmField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyIDictionaryEnd = 14
            recordId = BamlRecordType.PropertyIDictionaryEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // LiteralContent = 15
            recordId = BamlRecordType.LiteralContent;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.Value),
                new BamlDasmField(BamlFieldType.LineNumber),
                new BamlDasmField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // Text =16
            recordId = BamlRecordType.Text;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // TextWithConverter = 17
            recordId = BamlRecordType.TextWithConverter;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.Value),
                new BamlDasmField(BamlFieldType.ConverterTypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // RoutedEvent disassembly is untested because this code
            // has never seen this record in actual BAML

            // RoutedEvent = 18
            recordId = BamlRecordType.RoutedEvent;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // ClrEvent = 19                        NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.ClrEvent;
            fields = new BamlDasmField[] {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // XmlnsProperty = 20
            recordId = BamlRecordType.XmlnsProperty;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.NamespacePrefix),
                new BamlDasmField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // XmlAttribute = 21                     NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.XmlAttribute;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ProcessingInstruction = 22            NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.ProcessingInstruction;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // Comment = 23                          NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.Comment;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // DefTag = 24                           NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.DefTag;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // DefAttribute = 25
            recordId = BamlRecordType.DefAttribute;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.Value),
                new BamlDasmField(BamlFieldType.NameId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // EndAttributes = 26                    NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.EndAttributes;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PIMapping = 27
            recordId = BamlRecordType.PIMapping;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.XmlNamespace),
                new BamlDasmField(BamlFieldType.ClrNamespace),
                new BamlDasmField(BamlFieldType.AssemblyId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // AssemblyInfo = 28
            recordId = BamlRecordType.AssemblyInfo;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AssemblyId),
                new BamlDasmField(BamlFieldType.AssemblyFullName),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // TypeInfo = 29
            recordId = BamlRecordType.TypeInfo;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.AssemblyId),
                new BamlDasmField(BamlFieldType.TypeFullName),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // TypeSerializerInfo disassembly is untested because this code
            // has never seen this record in actual BAML

            // TypeSerializerInfo = 30
            recordId = BamlRecordType.TypeSerializerInfo;
            fields = new BamlDasmField[]  //                       based on TypeInfo
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.AssemblyId),
                new BamlDasmField(BamlFieldType.TypeFullName),
                new BamlDasmField(BamlFieldType.SerializerTypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // AttributeInfo = 31
            recordId = BamlRecordType.AttributeInfo;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId, "NewAttributeId"),
                new BamlDasmField(BamlFieldType.TypeId, "OwnerTypeId"),
                new BamlDasmField(BamlFieldType.AttributeUsage),
                new BamlDasmField(BamlFieldType.Value, "NewAttributeValue"),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // StringInfo = 32
            recordId = BamlRecordType.StringInfo;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.StringId),
                new BamlDasmField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // PropertyStringReference disassembly is untested because this code
            // has never seen this record in actual BAML

            // PropertyStringReference = 33
            recordId = BamlRecordType.PropertyStringReference;
            fields = new BamlDasmField[]           // based on PropertyComplexStart
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.StringId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyTypeReference = 34
            recordId = BamlRecordType.PropertyTypeReference;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.TypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // PropertyWithExtension = 35
            recordId = BamlRecordType.PropertyWithExtension;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.ExtensionTypeId),
                new BamlDasmField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields);

            // PropertyWithConverter = 36
            recordId = BamlRecordType.PropertyWithConverter;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.Value),
                new BamlDasmField(BamlFieldType.ConverterTypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // DeferableContentStart = 37
            recordId = BamlRecordType.DeferableContentStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.ContentSize),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // DefAttributeKeyString = 38
            recordId = BamlRecordType.DefAttributeKeyString;
            fields = new BamlDasmField[]
            {
                // the "value" is not serialized on this record.
                new BamlDasmField(BamlFieldType.ValueId),
                new BamlDasmField(BamlFieldType.ValuePosition),
                new BamlDasmField(BamlFieldType.Shared),
                new BamlDasmField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // DefAttributeKeyType = 39
            recordId = BamlRecordType.DefAttributeKeyType;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.ValuePosition),
                new BamlDasmField(BamlFieldType.FlagsByte),
                new BamlDasmField(BamlFieldType.Shared),
                new BamlDasmField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // KeyElementStart = 40
            recordId = BamlRecordType.KeyElementStart;
            fields = new BamlDasmField[]        // same as DefAttributeKeyType
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.ValuePosition),
                new BamlDasmField(BamlFieldType.FlagsByte),
                new BamlDasmField(BamlFieldType.Shared),
                new BamlDasmField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // KeyElementEnd = 41
            recordId = BamlRecordType.KeyElementEnd;
            fields = new BamlDasmField[]         // same as ElementEnd
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ConstructorParametersStart=  42
            recordId = BamlRecordType.ConstructorParametersStart;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ConstructorParametersEnd = 43
            recordId = BamlRecordType.ConstructorParametersEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ConstructorParameterType = 44
            recordId = BamlRecordType.ConstructorParameterType;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ConnectionId = 45
            recordId = BamlRecordType.ConnectionId;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.ConnectionId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // ContentProperty = 46
            recordId = BamlRecordType.ContentProperty;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // NamedElementStart = 47
            recordId = BamlRecordType.NamedElementStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.RuntimeName),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // StaticResourceStart = 48
            recordId = BamlRecordType.StaticResourceStart;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.TypeId),
                new BamlDasmField(BamlFieldType.FlagsByte),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // StaticResourceEnd = 49
            recordId = BamlRecordType.StaticResourceEnd;
            fields = new BamlDasmField[]
            {
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // StaticResourceId = 50
            recordId = BamlRecordType.StaticResourceId;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.StaticResourceId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // TextWithId = 51
            recordId = BamlRecordType.TextWithId;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // PresentationOptionsAttribute = 52
            recordId = BamlRecordType.PresentationOptionsAttribute;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.Value),
                new BamlDasmField(BamlFieldType.NameId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, true, fields );

            // LineNumber = 53
            recordId = BamlRecordType.LineNumberAndPosition;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.LineNumber),
                new BamlDasmField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );

            // LinePosition = 54
            recordId = BamlRecordType.LinePosition;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields );
            
            // PropertyWithStaticResourceId = 55
            recordId = BamlRecordType.PropertyWithStaticResourceId;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.AttributeId),
                new BamlDasmField(BamlFieldType.StaticResourceId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields);
            
            // OptimizedStaticResource = 56
            recordId = BamlRecordType.OptimizedStaticResource;
            fields = new BamlDasmField[]
            {
                new BamlDasmField(BamlFieldType.FlagsByte),
                new BamlDasmField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlDasmRecord(recordId, false, fields);
        }
    }
}

