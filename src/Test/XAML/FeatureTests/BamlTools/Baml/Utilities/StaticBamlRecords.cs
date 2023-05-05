// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Baml.Utilities
{
    class StaticBamlRecords
    {
        private static BamlRecord[] s_records;

        public static BamlRecord GetRecord(BamlRecordType recordId)
        {
            BamlRecord rec = s_records[(int)recordId];
            return rec.Clone();
        }

        static StaticBamlRecords()
        {
            BamlRecordType recordId;
            BamlField[] fields;

            s_records = new BamlRecord[(int)BamlRecordType.LastRecordType];

            // DocumentStart = 1
            recordId = BamlRecordType.DocumentStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.LoadAsync),
                new BamlField(BamlFieldType.MaxAsyncRecords),
                new BamlField(BamlFieldType.DebugBamlStream),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // DocumentEnd = 2
            recordId = BamlRecordType.DocumentEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ElementStart = 3
            recordId = BamlRecordType.ElementStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.FlagsByte),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ElementEnd = 4
            recordId = BamlRecordType.ElementEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // Property = 5
            recordId = BamlRecordType.Property;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // PropertyCustom = 6
            recordId = BamlRecordType.PropertyCustom;
            fields = new BamlField[]             // same interface as Property 
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.SerializerTypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // PropertyComplexStart = 7
            recordId = BamlRecordType.PropertyComplexStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyComplexEnd = 8
            recordId = BamlRecordType.PropertyComplexEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyArrayStart = 9
            recordId = BamlRecordType.PropertyArrayStart;
            fields = new BamlField[]           // based on ComplexPropertyStart
            {
                new BamlField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyArrayEnd = 10
            recordId = BamlRecordType.PropertyArrayEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyIListStart = 11
            recordId = BamlRecordType.PropertyIListStart;
            fields = new BamlField[]           // based on ComplexPropertyStart
            {
                new BamlField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyIListEnd = 12
            recordId = BamlRecordType.PropertyIListEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyIDictionaryStart = 13
            recordId = BamlRecordType.PropertyIDictionaryStart;
            fields = new BamlField[]           // based on PropertyComplexStart
            {
                new BamlField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyIDictionaryEnd = 14
            recordId = BamlRecordType.PropertyIDictionaryEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // LiteralContent = 15
            recordId = BamlRecordType.LiteralContent;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.LineNumber),
                new BamlField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // Text =16
            recordId = BamlRecordType.Text;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // TextWithConverter = 17
            recordId = BamlRecordType.TextWithConverter;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.ConverterTypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // RoutedEvent disassembly is untested because this code
            // has never seen this record in actual BAML

            // RoutedEvent = 18
            recordId = BamlRecordType.RoutedEvent;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // ClrEvent = 19                        NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.ClrEvent;
            fields = new BamlField[] {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // XmlnsProperty = 20
            recordId = BamlRecordType.XmlnsProperty;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.NamespacePrefix),
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.AssemblyIdList),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // XmlAttribute = 21                     NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.XmlAttribute;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ProcessingInstruction = 22            NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.ProcessingInstruction;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // Comment = 23                          NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.Comment;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // DefTag = 24                           NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.DefTag;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // DefAttribute = 25
            recordId = BamlRecordType.DefAttribute;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.NameId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // EndAttributes = 26                    NOT IMPLEMENTED in Avalon
            recordId = BamlRecordType.EndAttributes;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PIMapping = 27
            recordId = BamlRecordType.PIMapping;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.XmlNamespace),
                new BamlField(BamlFieldType.ClrNamespace),
                new BamlField(BamlFieldType.AssemblyId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // AssemblyInfo = 28
            recordId = BamlRecordType.AssemblyInfo;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AssemblyId),
                new BamlField(BamlFieldType.AssemblyFullName),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // TypeInfo = 29
            recordId = BamlRecordType.TypeInfo;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.AssemblyId),
                new BamlField(BamlFieldType.TypeFullName),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // TypeSerializerInfo disassembly is untested because this code
            // has never seen this record in actual BAML

            // TypeSerializerInfo = 30
            recordId = BamlRecordType.TypeSerializerInfo;
            fields = new BamlField[]  //                       based on TypeInfo
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.AssemblyId),
                new BamlField(BamlFieldType.TypeFullName),
                new BamlField(BamlFieldType.SerializerTypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // AttributeInfo = 31
            recordId = BamlRecordType.AttributeInfo;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId, "NewAttributeId"),
                new BamlField(BamlFieldType.TypeId, "OwnerTypeId"),
                new BamlField(BamlFieldType.AttributeUsage),
                new BamlField(BamlFieldType.Value, "NewAttributeValue"),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // StringInfo = 32
            recordId = BamlRecordType.StringInfo;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.StringId),
                new BamlField(BamlFieldType.Value),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // PropertyStringReference disassembly is untested because this code
            // has never seen this record in actual BAML

            // PropertyStringReference = 33
            recordId = BamlRecordType.PropertyStringReference;
            fields = new BamlField[]           // based on PropertyComplexStart
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.StringId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyTypeReference = 34
            recordId = BamlRecordType.PropertyTypeReference;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.TypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyWithExtension = 35
            recordId = BamlRecordType.PropertyWithExtension;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.ExtensionTypeId),
                new BamlField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyWithConverter = 36
            recordId = BamlRecordType.PropertyWithConverter;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.ConverterTypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // DeferableContentStart = 37
            recordId = BamlRecordType.DeferableContentStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.ContentSize),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // DefAttributeKeyString = 38
            recordId = BamlRecordType.DefAttributeKeyString;
            fields = new BamlField[]
            {
                // the "value" is not serialized on this record.
                new BamlField(BamlFieldType.ValueId),
                new BamlField(BamlFieldType.ValuePosition),
                new BamlField(BamlFieldType.Shared),
                new BamlField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // DefAttributeKeyType = 39
            recordId = BamlRecordType.DefAttributeKeyType;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.FlagsByte),
                new BamlField(BamlFieldType.ValuePosition),
                new BamlField(BamlFieldType.Shared),
                new BamlField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // KeyElementStart = 40
            recordId = BamlRecordType.KeyElementStart;
            fields = new BamlField[]        // same as DefAttributeKeyType
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.ValuePosition),
                new BamlField(BamlFieldType.FlagsByte),
                new BamlField(BamlFieldType.Shared),
                new BamlField(BamlFieldType.SharedSet),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // KeyElementEnd = 41
            recordId = BamlRecordType.KeyElementEnd;
            fields = new BamlField[]         // same as ElementEnd
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ConstructorParametersStart=  42
            recordId = BamlRecordType.ConstructorParametersStart;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ConstructorParametersEnd = 43
            recordId = BamlRecordType.ConstructorParametersEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ConstructorParameterType = 44
            recordId = BamlRecordType.ConstructorParameterType;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ConnectionId = 45
            recordId = BamlRecordType.ConnectionId;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.ConnectionId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // ContentProperty = 46
            recordId = BamlRecordType.ContentProperty;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // NamedElementStart = 47
            recordId = BamlRecordType.NamedElementStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.RuntimeName),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // StaticResourceStart = 48
            recordId = BamlRecordType.StaticResourceStart;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.TypeId),
                new BamlField(BamlFieldType.FlagsByte),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // StaticResourceEnd = 49
            recordId = BamlRecordType.StaticResourceEnd;
            fields = new BamlField[]
            {
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // StaticResourceId = 50
            recordId = BamlRecordType.StaticResourceId;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.StaticResourceId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // TextWithId = 51
            recordId = BamlRecordType.TextWithId;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // PresentationOptionsAttribute = 52
            recordId = BamlRecordType.PresentationOptionsAttribute;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.Value),
                new BamlField(BamlFieldType.NameId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, true, fields);

            // LineNumber = 53
            recordId = BamlRecordType.LineNumberAndPosition;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.LineNumber),
                new BamlField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // LinePosition = 54
            recordId = BamlRecordType.LinePosition;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.LinePosition),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // PropertyWithStaticResourceId = 55
            recordId = BamlRecordType.PropertyWithStaticResourceId;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.AttributeId),
                new BamlField(BamlFieldType.StaticResourceId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);

            // OptimizedStaticResource = 56
            recordId = BamlRecordType.OptimizedStaticResource;
            fields = new BamlField[]
            {
                new BamlField(BamlFieldType.FlagsByte),
                new BamlField(BamlFieldType.ValueId),
            };
            s_records[(int)recordId] = new BamlRecord(recordId, false, fields);
        }
    }
}
