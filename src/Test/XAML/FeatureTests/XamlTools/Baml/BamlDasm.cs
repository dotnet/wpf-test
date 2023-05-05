// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;

using System.Windows.Markup;

namespace BamlDasm
{
    public class BamlDasm
    {

        private static void Usage()
        {
            Console.WriteLine("Usage: BamlDasm.exe [-a] <Filename>");
            Console.WriteLine("    -a   Print Address on each line.");
            return;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Usage();
                return;
            }

            bool showAddresses = false;
            int i;
            for(i=0; i<args.Length-1; i++)
            {
                if(args[i] == "-a")
                    showAddresses = true;
            }

            string filename = args[args.Length-1];
            Console.WriteLine("Baml File {0}", filename);

            FileStream filestream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BamlBinaryReader reader = new BamlBinaryReader(filestream);

            BamlDisassembler bdasm = new BamlDisassembler(reader);
            bdasm.ShowAddresses = showAddresses;

            bdasm.DumpVersionHeader();
            Console.WriteLine();

            while(!bdasm.Done())
            {
                bdasm.ReadRecord();
                Console.WriteLine();
            }
            return;
        }
    }

    public class MockFormatVersion
    {
        // the real format version is Window Internal.
        // But we don't need the real type, we just need a unique type to standin for it.
    }

    public class BamlDisassembler
    {
        const int LineLength=16;    // hexbyte length
        static Type s_formatVersionType = typeof(MockFormatVersion);

        BamlBinaryReader _reader;
        bool _showAddresses=true;
        Dictionary<int, string> _attributeIdMap;

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

        public BamlDisassembler(BamlBinaryReader reader)
        {
            _reader = reader;
            _attributeIdMap = new Dictionary<int, string>();
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

        public long Position
        {
            get { return _reader.BaseStream.Position; }
            set { _reader.BaseStream.Position = value; }
        }
        
        public long Length
        {
            get { return _reader.BaseStream.Length; }
        }

        public bool Done()
        {
            return (Position >= Length-1);
        }

        public void DumpVersionHeader()
        {
            TextLineSections[] textSections;

            ReadFormatVersion(out textSections);
            OutputTextLines(textSections);
        }

        public BamlDasmRecord ReadRecord()
        {
            BamlDasmRecord record;
            BamlRecordType recordType;
            int variableSize = 0;
            TextLineSections text;
            TextLineSections[] textList;
            long startPosition = Position;

            recordType = ReadRecordType(out text);
            record = StaticBamlRecords.GetRecord(recordType);
            if (record == null)
                return null;

            if (record.IsVariableSize)
            {
                variableSize = Read7bitEncodedInt("size", out text);
//                OutputTextLine(text);
            }

            for (int i = 0; i < record.Fields.Length; i++)
            {
                object val = ReadField(record.Fields[i], out textList);
                record.Fields[i].Value = val;
                if (textList.Length == 1)
                {
                    ImproveMeaning(record.Fields[i], textList);
                }
  //              OutputTextLines(textList);
            }

            // this deals with the rare case of undefined padding
            // at the end of a record.  Only allowed for PropertyCustom
            if (record.IsVariableSize)
            {
                int processedSize = (int)(Position - startPosition - 1);
                int extraPadding = variableSize - processedSize;
                if (extraPadding < 0)
                {
              //      System.Diagnostics.Debugger.Break();
                }
                if (extraPadding > 0)
                {
                    long pos = Position;
                    byte[] bytes = _reader.ReadBytes(extraPadding);

                    string meaning = "?? custom";
                    if (recordType == BamlRecordType.PropertyCustom)
                    {
                        meaning = "Custom Data (variable size)";
                    }
                    textList = CutIntoLines(pos, bytes, meaning, "");
    //                OutputTextLines(textList);
                }
            }

            ProcessRecord(record);

            return record;
        }

        private BamlRecordType ReadRecordType(out TextLineSections text)
        {
            long pos = Position;
            int recordId = _reader.ReadByte();
            Position = pos;

            BamlRecordType val = (BamlRecordType)recordId;
            string meaning = string.Format("{0} [BamlRecord]", val.ToString());
            
            byte[] bytes = _reader.ReadBytes(1);
            text = new TextLineSections(pos, bytes, meaning);
            return val;
        }

        private object ReadField(BamlDasmField field, out TextLineSections[] textList)
        {
            TextLineSections text= new TextLineSections("Err", "Err", "Err");
            object val=null;
            Type t = field.ClrType;

            textList=null;
            if(t == typeof(short))
            {
                val = (Int16) ReadIntType(2, field.Name, out text);
            }
            else if(t == typeof(int))
            {
                val = (Int32) ReadIntType(4, field.Name, out text);
            }
            else if(t == typeof(string))
            {
                val = (string) ReadString(field.Name, out textList);
            }
            else if(t == typeof(bool))
            {
                val = (bool) ReadBool(field.Name, out text);
            }
            else if(t == typeof(byte))
            {
                val = (byte) ReadIntType(1, field.Name, out text);
            }
            else if(t == FormatVersionType)
            {
                ReadFormatVersion(out textList);
            }
            else
            {
                throw new InvalidOperationException("Field Type not known");
            }
            if(null == textList)
            {
                textList = new TextLineSections[1];
                textList[0] = text;
            }

            return val;
        }
        internal void ReadFormatVersion()
        {
            TextLineSections foo;
            TextLineSections[] bar;
            int strByteLength = (int)ReadIntType(4, "Unicode String Length(bytes)", out foo);
            ReadByteLengthPrefixedDWordPaddedUnicodeString(strByteLength, out bar);
            ReadVersion("Reader Version", out foo);
            ReadVersion("Update Version", out foo);
            ReadVersion("Writer Version", out foo);
        }
        internal void ReadFormatVersion(out TextLineSections[] text)
        {
            TextLineSections UnicodeLength;
            TextLineSections[] Unicode;
            TextLineSections Version1;
            TextLineSections Version2;
            TextLineSections Version3;

            int strByteLength = (int)ReadIntType(4, "Unicode String Length(bytes)", out UnicodeLength);
            ReadByteLengthPrefixedDWordPaddedUnicodeString(strByteLength, out Unicode);
            ReadVersion("Reader Version", out Version1);
            ReadVersion("Update Version", out Version2);
            ReadVersion("Writer Version", out Version3);

            text = new TextLineSections[4+Unicode.Length];
            text[0] = UnicodeLength;
            for(int i=0; i<Unicode.Length; i++)
                text[1+i] = Unicode[i];
            text[1+Unicode.Length] = Version1;
            text[2+Unicode.Length] = Version2;
            text[3+Unicode.Length] = Version3;
            return;
        }

        private void ReadByteLengthPrefixedDWordPaddedUnicodeString(int strByteLength, out TextLineSections[] textList)
        {
            string val="";

            long pos = Position;
            {
                BinaryReader tempReader = new BinaryReader(_reader.BaseStream, System.Text.Encoding.Unicode);
                char[] chars = tempReader.ReadChars(strByteLength/2);
                val = new string(chars);
            }
            Position = pos;

            byte[] bytes = _reader.ReadBytes(strByteLength);
            string meaning = string.Format("\"{0}\"", val);

            textList = CutIntoLines(pos, bytes, meaning, "");
        }

        private void ReadVersion(string label, out TextLineSections text)
        {
            short val1;
            short val2;

            long pos = Position;
            val1 = _reader.ReadInt16();
            val2 = _reader.ReadInt16();
            Position = pos;

            byte[] bytes = _reader.ReadBytes(4);
            string meaning = string.Format("{0}={1}.{2}", label, val1.ToString(), val2.ToString());

            text = new TextLineSections(pos, bytes, meaning);
        }

        private long ReadIntType(int size, string label, out TextLineSections text)
        {
            long  val=-1;


            string meaning="";

            long pos = Position;
            switch(size)
            {
                case 1:
                    val= (sbyte)_reader.ReadSByte();
                    break;

                case 2:
                    val= (short)_reader.ReadInt16();
                    break;

                case 4:
                    val = (int)_reader.ReadInt32();
                    break;

                case 8:
                    val= (long)_reader.ReadInt64();
                    break;

                default:
                    throw new InvalidOperationException("Bad integral datatype size");
            }
            Position = pos;

            byte[] bytes = _reader.ReadBytes(size);

            sbyte val8=-1;
            short val16=-1;
            int   val32=-1;
            long  val64=-1;
            string formatString = (0 <= val && val < 10) ? "{0}={1}" : "{0}={1}({2})";
            switch(size)
            {
                case 1:
                    val8 = (sbyte)val;
                    meaning = string.Format(formatString, label, val8.ToString("x"), val8.ToString("d"));
                    break;

                case 2:
                    val16 = (short)val;
                    meaning = string.Format(formatString, label, val16.ToString("x"), val16.ToString("d"));
                    break;

                case 4:
                    val32 = (int)val;
                    meaning = string.Format(formatString, label, val32.ToString("x"), val32.ToString("d"));
                    break;

                case 8:
                    val64 = (long)val;
                    meaning = string.Format(formatString, label, val64.ToString("x"), val64.ToString("d"));
                    break;
            }

            text = new TextLineSections(pos, bytes, meaning);
            return val;
        }

        private bool ReadBool(string label, out TextLineSections text)
        {
            long pos = Position;
            bool val = _reader.ReadBoolean();
            Position = pos;

            byte[] bytes = _reader.ReadBytes(1);
            string meaning = string.Format("{0}={1}", label, val.ToString());

            text = new TextLineSections(pos, bytes, meaning);
            return val;
        }

        private int Read7bitEncodedInt(string label, out TextLineSections text)
        {
            long pos = Position;
            int val = _reader.Read7BitEncodedInt();
            int sizeOfEncoding = (int)(Position - pos);
            Position = pos;

            byte[] bytes = _reader.ReadBytes(sizeOfEncoding);
            string meaning = string.Format("{0}={1}", label, val.ToString());
            text = new TextLineSections(pos, bytes, meaning);
            return val;
        }

        private string ReadString(string label, out TextLineSections[] textList)
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
            if(sizeInBytesOfString <= LineLength)
            {
                string meaning = string.Format("{0}='{1}'", label, val);
                textList = new TextLineSections[1];
                textList[0] = new TextLineSections(pos, bytes, meaning);
            }
            else
            {
                textList = CutIntoLines(pos, bytes, val, label);
            }
            return val;
        }


        // OUTPUT methods.


        private void OutputTextLine(TextLineSections text)
        {
            string outputLine = "";
            if(ShowAddresses)
                outputLine = string.Format("{0}: ", text.addrStr);
            outputLine += string.Format(" {0}", text.hexStr);
            
            int column=0;
            if(outputLine.Length < 40)
                column = 40;
            else
                column = 60;
            outputLine += this.FormatPadToColumn(column, outputLine.Length);
            Console.WriteLine(outputLine + text.meaningStr);
        }

        private void OutputTextLines(TextLineSections[] textList)
        {
            foreach(TextLineSections text in textList)
                OutputTextLine(text);
        }

        // Output a long block of bytes possibly over several lines.
        // If "label" is non-null then there is a header line of output.
        // If "val" is non-null then the byte block has a parallel "string" version.
        private TextLineSections[] CutIntoLines(long position, byte[] bytes, string val, string label)
        {
            TextLineSections[] textList;
            bool hasLabel = (null == label) ? false : (0 == label.Length) ? false : true;
            bool hasString = (null == val) ? false : (0 == val.Length) ? false : true;
            int lineCount = (bytes.Length + LineLength-1)  / LineLength;

            if(hasLabel)
                lineCount += 1;
            textList = new TextLineSections[lineCount];

            // Output the first "header" line.
            // If there is a Lable then the header line is the string length
            //   and label text.
            // If there is a Lable but no string, then the bytes is not a string
            //   is the just a blob of bytes.
            int headerOffset = 0;
            int currentLine = 0;
            if(hasLabel)
            {
                int sizeOfHeader=0;
                byte[] headerBytes=null;
                if(!hasString)
                {
                    sizeOfHeader = (bytes.Length > LineLength) ? LineLength : bytes.Length;
                    headerBytes = SnipBytes(bytes, 0, LineLength);
                    textList[currentLine++] = new TextLineSections(position, headerBytes, label);
                    headerOffset = sizeOfHeader;
                }
                else
                {
                    sizeOfHeader = ComputeLengthOf7bitEncodedLengthPrefix(bytes.Length);
                    headerBytes = SnipBytes(bytes, 0, sizeOfHeader);
                    int stringLength = val.Length;
                    string meaning = string.Format("{0}= string of length {1}({2})", label, stringLength.ToString("x2"), stringLength);
                    textList[currentLine++] = new TextLineSections(position, headerBytes, meaning);
                    headerOffset = sizeOfHeader;
                }
                
            }

            // Output the middle "full length lines"
            int byteOffset=0;
            int valOffset=0;
            for(valOffset=0; (byteOffset=valOffset+headerOffset)<bytes.Length-LineLength; valOffset+=LineLength)
            {
                byte[] subBytes = SnipBytes(bytes, byteOffset, LineLength);
                string subText = "";
                if(hasString)
                {   // The unicode string may run short early.
                    int remainingVal = val.Length - valOffset;
                    if(remainingVal > 0)
                    {
                        int subValLen = (remainingVal < LineLength) ? remainingVal : LineLength;
                        char[] subChars = val.ToCharArray(valOffset, subValLen);
                        subText = new string(subChars);
                    }
                }
                textList[currentLine++] = new TextLineSections(position+byteOffset, subBytes, subText);
            }

            // output the last line.
            int remaining = bytes.Length - byteOffset;
            if(remaining > 0)
            {
                byte[] subBytes = SnipBytes(bytes, byteOffset, remaining);
                string subText = "";
                if(hasString)
                {
                    int remainingVal = val.Length - valOffset;
                    if(remainingVal > 0)
                    {
                        char[] subChars = val.ToCharArray(valOffset, remainingVal);
                        subText = new string(subChars);
                    }
                }
                textList[currentLine++] = new TextLineSections(position+byteOffset, subBytes, subText);
            }
            return textList;
        }

        private byte[] SnipBytes(byte[] src, int start, int count)
        {
            byte[] dest = new byte[count];
            for(int i=0; i<count; i++)
                dest[i] = src[start+i];
            return dest;
        }

        // this function is passed the length of a buffer, including the
        // variable size length prefix.   Given that total length, how 
        // many bytes long is the length prefix.
        private int ComputeLengthOf7bitEncodedLengthPrefix(int length)
        {
            if(length <= 0x80)  // 0 to 7F have 1 byte headers 7F+1=0x80.
                return 1;
            if(length <= 0x4001) // 80 to 3FFF have 2 byte headers 3FFF+2=0x4001
                return 2;
            if(length < 0x200002) // 1FFFFF + 3 = 0x200002
                return 3;
            return 4;       // this could go bigger and bigger, but reasonable numbers...
        }

        private static string FormatHexBytes(byte[] bytes)
        {
            if(null == bytes || bytes.Length > 16 || bytes.Length <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            string outStr = "";

            if(bytes.Length == 16)
            {
                outStr += string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                    bytes[ 0].ToString("x2"), bytes[ 1].ToString("x2"),
                    bytes[ 2].ToString("x2"), bytes[ 3].ToString("x2"),
                    bytes[ 4].ToString("x2"), bytes[ 5].ToString("x2"),
                    bytes[ 6].ToString("x2"), bytes[ 7].ToString("x2") );
                outStr += string.Format("  {0} {1} {2} {3} {4} {5} {6} {7}",
                    bytes[ 8].ToString("x2"), bytes[ 9].ToString("x2"),
                    bytes[10].ToString("x2"), bytes[11].ToString("x2"),
                    bytes[12].ToString("x2"), bytes[13].ToString("x2"),
                    bytes[14].ToString("x2"), bytes[15].ToString("x2") );
            }
            else
            {
                for(int i=0; i<bytes.Length; i++)
                {
                    outStr += string.Format("{0} ", bytes[i].ToString("x2"));
                    if(7 == i%8)
                        outStr += " ";
                }
            }

            return outStr;
        }

        private string FormatPadToColumn(int column, int strLength)
        {
            int diff = column - strLength;
            string padding = "";
            while(diff > 0)
            {
                if(diff >= 8)
                {
                    padding += "        ";
                    diff -= 8;
                }
                else if(diff >= 4)
                {
                    padding += "    ";
                    diff -= 4;
                }
                else if(diff >= 2)
                {
                    padding += "  ";
                    diff -= 2;
                }
                else if(diff >= 1)
                {
                    padding += " ";
                    diff -= 1;
                }
            }
            return padding;
        }

        private void ProcessRecord(BamlDasmRecord record)
        {
            switch(record.Id)
            {
                case BamlRecordType.AttributeInfo:
                    ProcessAttributeInfo(record);
                    break;
            }
        }

        private void ProcessAttributeInfo(BamlDasmRecord record)
        {
            System.Diagnostics.Debug.Assert(record.Fields[0].BamlFieldType == BamlFieldType.AttributeId);
            Int16 attrId = (Int16) record.Fields[0].Value;

            System.Diagnostics.Debug.Assert(record.Fields[3].BamlFieldType == BamlFieldType.Value);
            string strValue = (string) record.Fields[3].Value;

            _attributeIdMap[attrId] = strValue;
        }

        private void ImproveMeaning(BamlDasmField field, TextLineSections[] textList)
        {
            string oldMeaning = textList[0].meaningStr;
            string strValue = null;
            switch(field.BamlFieldType)
            {
            case BamlFieldType.AttributeId:
                Int16 attrId = (Int16)field.Value;
                if(attrId < 0)
                {
                }
                else if(_attributeIdMap.ContainsKey(attrId))
                {
                    strValue = _attributeIdMap[attrId];
                }
                break;
            case BamlFieldType.TypeId:
            case BamlFieldType.ConverterTypeId:
                Int16 typeId = (Int16)field.Value;
                if(typeId < 0)
                {
                }
                break;
            case BamlFieldType.AttributeUsage:
                byte usage = (byte)field.Value;
                switch(usage)
                {
                    case 0: strValue = "Default"; break;
                    case 1: strValue = "XmlLang"; break;
                    case 2: strValue = "XmlSpace"; break;
                    case 3: strValue = "RuntimeName"; break;
                }
                break;
            }
            if(null != strValue)
            {
                textList[0].meaningStr = string.Format("{0}  <{1}>", oldMeaning, strValue);
            }
        }
    }
}
