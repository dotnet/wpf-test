// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Reflection;
using System.Collections.Generic;

namespace DRT
{
    [TestedSecurityLevelAttribute(SecurityLevel.PartialTrust)]
    public class BitStreamTest : DrtInkTestcase
    {
        public override void Run()
        {
            List<BitState> bitStates = new List<BitState>();
            for (int x = 1; x < 256; x++)
            {
                bitStates.Add(new BitState((byte)x));
            }
            for (int x = 255; x >= 1; x--)
            {
                bitStates.Add(new BitState((byte)x));
            }

            List<BitStateUInt32> bitStateInts = new List<BitStateUInt32>();
            for (uint x = 1; x < 2147483648; x *= 2)
            {
                bitStateInts.Add(new BitStateUInt32(x));
            }

            List<byte> bytes = new List<byte>();
            BitStreamWriter writer = new BitStreamWriter(bytes);
            foreach (BitState bitState in bitStates)
            {
                writer.Write(bitState.Byte, bitState.BitsUsed);
            }

            foreach (BitStateUInt32 bitStateUInt32 in bitStateInts)
            {
                writer.Write(bitStateUInt32.UInt32, bitStateUInt32.BitsUsed);
            }


            BitStreamReader reader = new BitStreamReader(bytes.ToArray());

            foreach (BitState bitState in bitStates)
            {
                byte b = reader.ReadByte(bitState.BitsUsed);
                if (b != bitState.Byte)
                {
                    throw new InvalidOperationException("Unexpected BitStreamReader.Read return value");
                }
            }

            foreach (BitStateUInt32 bitStateUInt32 in bitStateInts)
            {
                uint n = reader.ReadUInt32(bitStateUInt32.BitsUsed);
                if (n != bitStateUInt32.UInt32)
                {
                    throw new InvalidOperationException("Unexpected BitStreamReader.Read return value");
                }
            }
            
            // done
            Success = true;
        }
    }

    public class BitState
    {
        public BitState(byte b)
        {
            this.Byte = b;
            //figure out how many bits are set
            byte mask = 128;
            for (int x = 8; x >= 1; x--)
            {
                if ((mask & b) != 0)
                {
                    BitsUsed = x;
                    break;
                }
                mask >>= 1;
            }
            if (BitsUsed == 0)
            {
                Random r = new Random();
                BitsUsed = r.Next(1, 8);
            }
        }
        public byte Byte;
        public int BitsUsed;
    }

    public class BitStateUInt32
    {
        public BitStateUInt32(uint n)
        {
            this.UInt32 = n;
            //figure out how many bits are set
            uint mask = 2147483648; //32nd bit set
            for (int x = 32; x >= 1; x--)
            {
                if ((mask & n) != 0)
                {
                    BitsUsed = x;
                    break;
                }
                mask >>= 1;
            }
            if (BitsUsed == 0)
            {
                Random r = new Random();
                BitsUsed = r.Next(1, 8);
            }
        }
        public uint UInt32;
        public int BitsUsed;
    }

    /// <summary>
    /// A wrapper over the internal BitStreamReader that is called using reflection
    /// </summary>
    public class BitStreamReader
    {
        private object _bitStreamReader;
        private Type _bitStreamType;

        public BitStreamReader(byte[] buffer)
        {
            Assembly core = typeof(UIElement).Assembly;  // PresentationCore
            //Assembly core = Assembly.Load("PresentationCore");
            _bitStreamType = core.GetType("MS.Internal.Ink.BitStreamReader");
            ConstructorInfo ci = _bitStreamType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(byte[]) }, null);
            _bitStreamReader = ci.Invoke(new object[] { buffer });
        }

        public BitStreamReader(byte[] buffer, uint bufferLengthInBits)
        {
            Assembly core = typeof(UIElement).Assembly;  // PresentationCore
            //Assembly core = Assembly.Load("PresentationCore");
            _bitStreamType = core.GetType("MS.Internal.Ink.BitStreamReader");
            ConstructorInfo ci = _bitStreamType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(byte[]), typeof(uint) }, null);
            _bitStreamReader = ci.Invoke(new object[] { buffer, bufferLengthInBits });
        }

        public byte ReadByte(int countOfBits)
        {
            MethodInfo mi = _bitStreamType.GetMethod("ReadByte", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
            return (byte)mi.Invoke(_bitStreamReader, new object[] { countOfBits });
        }

        public uint ReadUInt32(int countOfBits)
        {
            MethodInfo mi = _bitStreamType.GetMethod("ReadUInt32", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
            return (uint)mi.Invoke(_bitStreamReader, new object[] { countOfBits });
        }

        public bool EndOfStream
        {
            get
            {
                PropertyInfo pi = _bitStreamType.GetProperty("EndOfStream", BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(bool), new Type[] { }, null);
                return (bool)pi.GetValue(_bitStreamReader, null);
            }
        }
    }

    /// <summary>
    /// A wrapper over the internal BitStreamWriter that is called using reflection
    /// </summary>
    public class BitStreamWriter
    {
        private object _bitStreamWriter;
        private Type _bitStreamType;

        public BitStreamWriter(List<byte> buffer)
        {
            Assembly core = typeof(UIElement).Assembly;  // PresentationCore
            //Assembly core = Assembly.Load("PresentationCore");
            _bitStreamType = core.GetType("MS.Internal.Ink.BitStreamWriter");
            ConstructorInfo ci = _bitStreamType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof(List<byte>)}, null);
            _bitStreamWriter = ci.Invoke(new object[] { buffer });
        }

        public void Write(byte bits, int countOfBits)
        {
            MethodInfo mi = _bitStreamType.GetMethod("Write", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(byte), typeof(int) }, null);
            mi.Invoke(_bitStreamWriter, new object[] { bits, countOfBits });
        }

        public void Write(uint bits, int countOfBits)
        {
            MethodInfo mi = _bitStreamType.GetMethod("Write", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(uint), typeof(int) }, null);
            mi.Invoke(_bitStreamWriter, new object[] { bits, countOfBits });
        }
    }
}
