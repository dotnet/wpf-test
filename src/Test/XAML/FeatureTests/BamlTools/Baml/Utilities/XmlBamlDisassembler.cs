// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;

using System.Windows.Markup;
using System.Xml;
using System.Globalization;

namespace Microsoft.Test.Baml.Utilities
{
    public class XmlBamlDisassembler
    {
        const int LineLength = 16;    // hexbyte length
        static Type s_formatVersionType = typeof(MockFormatVersion);

        BamlBinaryReader _reader;
        bool _showAddresses = true;
        bool _showHex = true;
        Dictionary<int, string> _attributeIdMap;
        TextWriter _writer = null;

        XmlTextWriter _xmlTextWriter;

        Stack<string> _recordStack;

        public struct TextLineSections
        {
            public string addrStr;
            public string hexStr;
            public string meaningStr;

            public TextLineSections(string pos, string hex, string meaning)
            {
                addrStr = pos;
                hexStr = hex;
                meaningStr = meaning;
            }

            public TextLineSections(long pos, byte[] hex, string meaning)
            {
                addrStr = string.Format("{0}", pos.ToString("X4"));
                hexStr = FormatHexBytes(hex);
                meaningStr = meaning;
            }
        };

        public XmlBamlDisassembler(BamlBinaryReader binaryReader, TextWriter textWriter, BamlDisassemblerSettings settings)
        {
            _reader = binaryReader;
            _writer = textWriter;
            _showAddresses = settings.ShowAddresses;
            _showHex = settings.ShowHex;
            _attributeIdMap = new Dictionary<int, string>();  // Keeps track of all defined attributes (Localization, etc..)

            _xmlTextWriter = new XmlTextWriter(_writer);
            _xmlTextWriter.Formatting = Formatting.Indented;
            _recordStack = new Stack<string>();  // Keeps track of what end record we expect, will print errors in xml in the event of a mismatch
        }

        public Type FormatVersionType
        {
            get { return s_formatVersionType; }
        }

        public bool ShowAddresses
        {
            get { return _showAddresses; }
            set { _showAddresses = value; }
        }

        public bool ShowHex
        {
            get { return _showHex; }
            set { _showHex = value; }
        }


        public long Position
        {
            get { return _reader.BaseStream.Position; }
            set { _reader.BaseStream.Position = value; }
        }

        public long Length
        {
            get { return _reader.BaseStream.Length; }
        }

        public void Run()
        {
            _recordStack.Clear();
            _xmlTextWriter.WriteStartElement("BAML");
            this.DumpVersionHeader();

            while (!this.Done())
            {
                this.DumpRecord();
            }
            if (_recordStack.Count != 0)
            {
                _xmlTextWriter.WriteComment("ERROR Records still present on stack");
                foreach (string s in _recordStack)
                {
                    WriteEndRecord(s); //This guarantees we produce valid XML
                }
            }
            _xmlTextWriter.WriteEndElement(); //Close "BAML" record
        }

        public bool Done()
        {
            return (Position >= Length - 1);
        }

        public void DumpVersionHeader()
        {
            _xmlTextWriter.WriteStartElement("VersionHeader");
            ReadFormatVersion();
            _xmlTextWriter.WriteEndElement();
        }

        public void DumpRecord()
        {
            BamlRecord record;
            BamlRecordType recordType;
            int variableSize = 0;
            string text;

            recordType = ReadRecordType(out text);
            WriteStartRecord(recordType.ToString());

            long startPosition = Position;

            record = StaticBamlRecords.GetRecord(recordType);
            if (null == record)
            {
                throw new InvalidOperationException(string.Format("{0} BAML record type is undefined", recordType.ToString()));
            }

            if (record.IsVariableSize)
            {
                variableSize = Read7bitEncodedInt();
                _xmlTextWriter.WriteAttributeString("Size", variableSize.ToString());
            }

            for (int i = 0; i < record.Fields.Length; i++)
            {
                string fieldStringValue;
                string expandedInfo;
                object val = ReadField(record.Fields[i], out fieldStringValue);
                record.Fields[i].Value = val;
                ImproveMeaning(record.Fields[i], out expandedInfo);
                if (!String.IsNullOrEmpty(expandedInfo))
                {
                    fieldStringValue = expandedInfo;
                }
                _xmlTextWriter.WriteAttributeString(record.Fields[i].Name, fieldStringValue);
            }

            // this deals with the rare case of undefined padding
            // at the end of a record.  Only allowed for PropertyCustom
            if (record.IsVariableSize)
            {
                int processedSize = (int)(Position - startPosition);
                int extraPadding = variableSize - processedSize;
                if (extraPadding < 0)
                {
                    System.Diagnostics.Debugger.Break();
                }
                if (extraPadding > 0)
                {
                    long pos = Position;
                    byte[] bytes = _reader.ReadBytes(extraPadding);
                }
            }

            ProcessRecord(record);
            WriteEndRecord(recordType.ToString());
        }

        private void WriteStartRecord(string recordTypeName)
        {
            if (recordTypeName.EndsWith("Start", true, CultureInfo.InvariantCulture))
            {
                string shortName = recordTypeName.Remove(recordTypeName.Length - 5); // Remove "Start"
                _recordStack.Push(shortName);
                _xmlTextWriter.WriteStartElement(shortName);
            }
            else if (recordTypeName.EndsWith("End", true, CultureInfo.InvariantCulture))
            {
                //skip
            }
            else
            {
                _xmlTextWriter.WriteStartElement(recordTypeName);
            }

        }

        private void WriteEndRecord(string recordTypeName)
        {
            if (recordTypeName.EndsWith("End", true, CultureInfo.InvariantCulture))
            {
                string shortName = recordTypeName.Remove(recordTypeName.Length - 3); // remove "End"
                string expected = _recordStack.Pop();
                if (!String.Equals(shortName, expected))
                {
                    if (expected == "DeferableContent")
                    {
                        _xmlTextWriter.WriteEndElement();
                        _xmlTextWriter.WriteComment("DeferableContent (Inserted)");
                        WriteEndRecord(recordTypeName);
                        return;
                    }
                    else
                    {
                        HandleRecordStackMismatch(expected, shortName);
                    }
                }
                _xmlTextWriter.WriteEndElement();
                _xmlTextWriter.WriteComment(recordTypeName);
            }
            else if (recordTypeName.EndsWith("Start", true, CultureInfo.InvariantCulture))
            {
                //skip
            }
            else
            {
                _xmlTextWriter.WriteEndElement(); //We generally only hit this with single-line records i.e. Text
            }

        }

        private void HandleRecordStackMismatch(string expected, string actual)
        {
            _xmlTextWriter.WriteComment(String.Format("ERROR expected: {0}, actual: {1}", expected, actual));
        }

        private BamlRecordType ReadRecordType(out string recordName)
        {
            long pos = Position;
            int recordId = _reader.ReadByte();
            Position = pos;

            BamlRecordType val = (BamlRecordType)recordId;

            byte[] bytes = _reader.ReadBytes(1);
            recordName = val.ToString();
            return val;
        }

        private object ReadField(BamlField field, out string fieldStringValue)
        {
            object val = null;
            Type t = field.ClrType;
            string strValue = "";

            TextLineSections[] textList;

            if (t == typeof(short))
            {
                val = (Int16)ReadIntType(2, out strValue);
            }
            else if (t == typeof(int))
            {
                val = (Int32)ReadIntType(4, out strValue);
            }
            else if (t == typeof(string))
            {
                val = (string)ReadString(out strValue);
            }
            else if (t == typeof(bool))
            {
                val = (bool)ReadBool(out strValue);
            }
            else if (t == typeof(byte))
            {
                val = (byte)ReadIntType(1, out strValue);
            }
            else if (t == FormatVersionType)
            {
                ReadFormatVersion();
            }
            else if (t == typeof(Int16[]))
            {
                val = (Int16[])ReadIntArrayType(field.Name, out textList);
            }
            else
            {
                throw new InvalidOperationException("Field Type not known");
            }
            fieldStringValue = strValue;
            return val;
        }

        private void ReadFormatVersion()
        {
            string UnicodeLength;
            string Unicode;
            string Version1;
            string Version2;
            string Version3;

            int strByteLength = (int)ReadIntType(4, /*"Unicode String Length(bytes)",*/out UnicodeLength);
            ReadByteLengthPrefixedDWordPaddedUnicodeString(strByteLength, out Unicode);
            ReadVersion("Reader Version", out Version1);
            ReadVersion("Update Version", out Version2);
            ReadVersion("Writer Version", out Version3);

            _xmlTextWriter.WriteAttributeString("Unicode_String_Length_bytes", UnicodeLength);
            _xmlTextWriter.WriteAttributeString("Unicode_Name", Unicode);
            _xmlTextWriter.WriteAttributeString("Reader_Version", Version1);
            _xmlTextWriter.WriteAttributeString("Update_Version", Version2);
            _xmlTextWriter.WriteAttributeString("Writer_Version", Version3);

            return;
        }

        private void ReadByteLengthPrefixedDWordPaddedUnicodeString(int strByteLength, out string strValue)
        {
            string val = "";

            long pos = Position;
            {
                BinaryReader tempReader = new BinaryReader(_reader.BaseStream, System.Text.Encoding.Unicode);
                char[] chars = tempReader.ReadChars(strByteLength / 2);
                val = new string(chars);
            }
            Position = pos;

            byte[] bytes = _reader.ReadBytes(strByteLength);
            strValue = val;

        }

        private void ReadVersion(string label, out string versionValue)
        {
            short val1;
            short val2;

            long pos = Position;
            val1 = _reader.ReadInt16();
            val2 = _reader.ReadInt16();
            Position = pos;

            byte[] bytes = _reader.ReadBytes(4);

            versionValue = string.Format("{0}.{1}", val1.ToString(), val2.ToString());
        }

        private long ReadIntType(int size, out string strValue)
        {
            long val = -1;


            string meaning = "";

            long pos = Position;
            switch (size)
            {
                case 1:
                    val = (sbyte)_reader.ReadSByte();
                    break;

                case 2:
                    val = (short)_reader.ReadInt16();
                    break;

                case 4:
                    val = (int)_reader.ReadInt32();
                    break;

                case 8:
                    val = (long)_reader.ReadInt64();
                    break;

                default:
                    throw new InvalidOperationException("Bad integral datatype size");
            }
            Position = pos;

            byte[] bytes = _reader.ReadBytes(size);

            sbyte val8 = -1;
            short val16 = -1;
            int val32 = -1;
            long val64 = -1;
            string formatString = (0 <= val && val < 10) ? "{0}" : "{0}({1})";
            switch (size)
            {
                case 1:
                    val8 = (sbyte)val;
                    meaning = string.Format(formatString, val8.ToString("x"), val8.ToString("d"));
                    break;

                case 2:
                    val16 = (short)val;
                    meaning = string.Format(formatString, val16.ToString("x"), val16.ToString("d"));
                    break;

                case 4:
                    val32 = (int)val;
                    meaning = string.Format(formatString, val32.ToString("x"), val32.ToString("d"));
                    break;

                case 8:
                    val64 = (long)val;
                    meaning = string.Format(formatString, val64.ToString("x"), val64.ToString("d"));
                    break;
            }

            strValue = meaning;
            return val;
        }

        private bool ReadBool(out string strValue)
        {
            long pos = Position;
            bool val = _reader.ReadBoolean();
            Position = pos;

            byte[] bytes = _reader.ReadBytes(1);
            strValue = val.ToString();

            return val;
        }

        private int Read7bitEncodedInt()
        {
            long pos = Position;
            int val = _reader.Read7BitEncodedInt();
            int sizeOfEncoding = (int)(Position - pos);
            Position = pos;

            byte[] bytes = _reader.ReadBytes(sizeOfEncoding);
            return val;
        }

        private string ReadString(out string strValue)
        {
            long pos = Position;
            string val = _reader.ReadString();
            int sizeInBytesOfString = (int)(Position - pos);
            Position = pos;

            // Can't use val.Length to compute number of bytes because multiple
            // bytes might make one Unicode character.
            // For Example:   the bytes 03 E2 97 8f make one Unicode char (The
            // Dot used in password boxes).
            byte[] bytes = _reader.ReadBytes(sizeInBytesOfString);
            strValue = val;
            return val;
        }

        private short[] ReadIntArrayType(string label, out TextLineSections[] textList)
        {
            long pos = Position;

            short arrayLength = (short)_reader.ReadInt16();
            short[] array = new short[arrayLength];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (short)_reader.ReadInt16();
            }
            int sizeOfArray = (int)(Position - pos);
            Position = pos;

            textList = new TextLineSections[arrayLength + 1];
            string meaning = string.Format("{0}={1}", label, array.Length);
            byte[] subBytes = _reader.ReadBytes(2);
            textList[0] = new TextLineSections(Position - 2, subBytes, meaning);

            for (int i = 0; i < array.Length; i++)
            {
                meaning = string.Format("Assembly ID[{0}]={1}", i, array[i]);
                subBytes = _reader.ReadBytes(2);
                textList[i + 1] = new TextLineSections(Position - 2, subBytes, meaning);
            }
            return array;
        }






        private static string FormatHexBytes(byte[] bytes)
        {
            if (null == bytes || bytes.Length > 16 || bytes.Length < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            string outStr = "";

            if (bytes.Length == 16)
            {
                outStr += string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                    bytes[0].ToString("x2"), bytes[1].ToString("x2"),
                    bytes[2].ToString("x2"), bytes[3].ToString("x2"),
                    bytes[4].ToString("x2"), bytes[5].ToString("x2"),
                    bytes[6].ToString("x2"), bytes[7].ToString("x2"));
                outStr += string.Format("  {0} {1} {2} {3} {4} {5} {6} {7}",
                    bytes[8].ToString("x2"), bytes[9].ToString("x2"),
                    bytes[10].ToString("x2"), bytes[11].ToString("x2"),
                    bytes[12].ToString("x2"), bytes[13].ToString("x2"),
                    bytes[14].ToString("x2"), bytes[15].ToString("x2"));
            }
            else
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    outStr += string.Format("{0} ", bytes[i].ToString("x2"));
                    if (7 == i % 8)
                        outStr += " ";
                }
            }

            return outStr;
        }


        private void ProcessRecord(BamlRecord record)
        {
            switch (record.Id)
            {
                case BamlRecordType.AttributeInfo:
                    ProcessAttributeInfo(record);
                    break;
            }
        }

        private void ProcessAttributeInfo(BamlRecord record)
        {
            System.Diagnostics.Debug.Assert(record.Fields[0].BamlFieldType == BamlFieldType.AttributeId);
            Int16 attrId = (Int16)record.Fields[0].Value;

            System.Diagnostics.Debug.Assert(record.Fields[3].BamlFieldType == BamlFieldType.Value);
            string strValue = (string)record.Fields[3].Value;

            _attributeIdMap[attrId] = strValue; //store attribute name in dictionary
        }

        private void ImproveMeaning(BamlField field, out string expandedInfo)
        {
            string strValue = null;
            switch (field.BamlFieldType)
            {
                case BamlFieldType.AttributeId:
                    Int16 attrId = (Int16)field.Value;
                    if (attrId < 0)
                    {
                        KnownProperty knownProp = KnownProperties.Properties[-attrId];
                        strValue = knownProp.ToString();
                    }
                    else if (_attributeIdMap.ContainsKey(attrId))
                    {
                        strValue = "(" + attrId + "):" + _attributeIdMap[attrId];
                    }
                    break;
                case BamlFieldType.TypeId:
                case BamlFieldType.ConverterTypeId:
                    Int16 typeId = (Int16)field.Value;
                    if (typeId < 0)
                    {
                        KnownElement knownElem = KnownElements.Elements[-typeId];
                        strValue = knownElem.ToString();
                    }
                    break;
                case BamlFieldType.AttributeUsage:
                    byte usage = (byte)field.Value;
                    switch (usage)
                    {
                        case 0: strValue = "Default"; break;
                        case 1: strValue = "XmlLang"; break;
                        case 2: strValue = "XmlSpace"; break;
                        case 3: strValue = "RuntimeName"; break;
                    }
                    break;
            }
            if (null != strValue)
            {
                expandedInfo = strValue;
            }
            else
            {
                expandedInfo = "";
            }
        }
    }
}
