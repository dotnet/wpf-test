// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a unit of data in a Baml file.
 *      Provides basic type conversion services.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Globalization;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BamlRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        protected BamlRecord(Int16 type, Int32 size, byte[] data)
        {
            this.type = type;
            this.size = size;
            this.data = data;
        }

        /// <summary>
        /// Convert a BamlRecord to a string
        /// </summary>
        /// <returns>The string representation of this BamlRecord</returns>
        public override string ToString()
        {
            return  "Record:\r\n" +
                    "\tType: " + (BamlRecordType)type + "\r\n" + 
                    "\tSize: " + size + " (0x" + size.ToString("X", CultureInfo.CurrentCulture) + ")\r\n" +
                    "\tData: " + BytesToString(data);
        }

        private string BytesToString(byte[] bytes)
        {
            string s = string.Empty;
            foreach (byte b in bytes)
            {
                s += (char)b;
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected Int32 ToInt32(byte[] bytes, int offset)
        {
            // Convert from Big-endian (intel) format

            Int32 n = bytes[3 + offset];
            for (int i = 2 + offset; i >= offset; i--)
            {
                n = n << 8;
                n |= bytes[i];
            }
            return n;
        }

        /// <summary>
        /// Convert a BamlRecord to an array of bytes.  If BamlRecord fuzzing is enabled,
        /// the array might not be the exact binary represenation of this BamlRecord.
        /// </summary>
        /// <param name="record">The BamlRecord to convert</param>
        /// <returns>A new byte array representing the binary format of this BamlRecord</returns>
        public static explicit operator byte[](BamlRecord record)
        {
            return record.ToByteArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract byte[] ToByteArray();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        protected void MemCopy(Int16 n, byte[] buffer, int offset)
        {
            if (buffer == null || buffer.Length < offset + 2)
            {
                throw new ApplicationException("Not enough room in byte array to store Int16 at this position");
            }

            // Save in Big-endian (intel) format
            buffer[offset] = (byte) (n & 0xff);
            buffer[offset+1] = (byte) ((n >> 8) & 0xff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        protected void MemCopy(Int32 n, byte[] buffer, int offset)
        {
            if (buffer == null || buffer.Length < offset + 4)
            {
                throw new ApplicationException("Not enough room in byte array to store Int32 at this position");
            }

            // Save in Big-endian (intel) format
            buffer[offset] = (byte) (n & 0xff);
            buffer[offset+1] = (byte) ((n >> 8) & 0xff);
            buffer[offset+2] = (byte) ((n >> 16) & 0xff);
            buffer[offset+3] = (byte) ((n >> 24) & 0xff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        protected void MemCopy(byte[] bytes, byte[] buffer, int offset)
        {
            if (buffer == null || buffer.Length < offset + bytes.Length)
            {
                throw new ApplicationException("Not enough room in byte array to store byte[] at this position");
            }

            for (int n = 0; n < bytes.Length; n++)
            {
                buffer[offset+n] = bytes[n];
            }
        }

        /// <summary>
        /// A 16 bit integer representing the type of the BamlRecord.
        /// It is not always accurate because it can be fuzzed.
        /// </summary>
        public Int16 Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// The size (in bytes) of the Data in this BamlRecord.
        /// It is not always accurate because it can be fuzzed.
        /// </summary>
        public Int32 Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// The size (in bytes) of this BamlRecord's raw data.
        /// </summary>
        public abstract int RawDataSize
        {
            get;
        }

        /// <summary>
        /// The raw data contained by this BamlRecord.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Int16 type;
        /// <summary>
        /// 
        /// </summary>
        protected Int32 size;
        /// <summary>
        /// 
        /// </summary>
        protected byte[] data;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BamlRecordType : byte
    {
        /// <summary>
        /// </summary>
        AssemblyInfo = 0x1c,
        /// <summary>
        /// </summary>
        AttributeInfo = 0x1f,
        /// <summary>
        /// </summary>
        ClrEvent = 0x13,
        /// <summary>
        /// </summary>
        Comment = 0x17,
        /// <summary>
        /// </summary>
        ConnectionId = 0x2d,
        /// <summary>
        /// </summary>
        ConstructorParametersEnd = 0x2b,
        /// <summary>
        /// </summary>
        ConstructorParametersStart = 0x2a,
        /// <summary>
        /// </summary>
        ConstructorParameterType = 0x2c,
        /// <summary>
        /// </summary>
        DefAttribute = 0x19,
        /// <summary>
        /// </summary>
        DefAttributeKeyString = 0x25,
        /// <summary>
        /// </summary>
        DefAttributeKeyType = 0x26,
        /// <summary>
        /// </summary>
        DeferableContentStart = 0x24,
        /// <summary>
        /// </summary>
        DefTag = 0x18,
        /// <summary>
        /// </summary>
        DocumentEnd = 2,
        /// <summary>
        /// </summary>
        DocumentStart = 1,
        /// <summary>
        /// </summary>
        ElementEnd = 4,
        /// <summary>
        /// </summary>
        ElementStart = 3,
        /// <summary>
        /// </summary>
        EndAttributes = 0x1a,
        /// <summary>
        /// </summary>
        KeyElementEnd = 40,
        /// <summary>
        /// </summary>
        KeyElementStart = 0x27,
        /// <summary>
        /// </summary>
        LastRecordType = 0x2e,
        /// <summary>
        /// </summary>
        LiteralContent = 15,
        /// <summary>
        /// </summary>
        PIMapping = 0x1b,
        /// <summary>
        /// </summary>
        ProcessingInstruction = 0x16,
        /// <summary>
        /// </summary>
        Property = 5,
        /// <summary>
        /// </summary>
        PropertyArrayEnd = 10,
        /// <summary>
        /// </summary>
        PropertyArrayStart = 9,
        /// <summary>
        /// </summary>
        PropertyComplexEnd = 8,
        /// <summary>
        /// </summary>
        PropertyComplexStart = 7,
        /// <summary>
        /// </summary>
        PropertyCustom = 6,
        /// <summary>
        /// </summary>
        PropertyIDictionaryEnd = 14,
        /// <summary>
        /// </summary>
        PropertyIDictionaryStart = 13,
        /// <summary>
        /// </summary>
        PropertyIListEnd = 12,
        /// <summary>
        /// </summary>
        PropertyIListStart = 11,
        /// <summary>
        /// </summary>
        PropertyStringReference = 0x21,
        /// <summary>
        /// </summary>
        PropertyTypeReference = 0x22,
        /// <summary>
        /// </summary>
        PropertyWithConverter = 0x23,
        /// <summary>
        /// </summary>
        RootElement = 0x29,
        /// <summary>
        /// </summary>
        RoutedEvent = 0x12,
        /// <summary>
        /// </summary>
        StringInfo = 0x20,
        /// <summary>
        /// </summary>
        Text = 0x10,
        /// <summary>
        /// </summary>
        TextWithConverter = 0x11,
        /// <summary>
        /// </summary>
        TypeInfo = 0x1d,
        /// <summary>
        /// </summary>
        TypeSerializerInfo = 30,
        /// <summary>
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// </summary>
        XmlAttribute = 0x15,
        /// <summary>
        /// </summary>
        XmlnsProperty = 20
    }
}
