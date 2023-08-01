// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Corrupts random bytes in a baml stream.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.Globalization;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class RandomByteFuzzer : BamlFuzzer
    {
        /// <summary>
        /// 
        /// </summary>
        public RandomByteFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public RandomByteFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
            XmlAttribute f = xmlElement.Attributes["Frequency"];
            XmlAttribute v = xmlElement.Attributes["Variance"];

            if (f == null)
            {
                throw new XmlException("Cannot find 'Frequency' in XmlElement '" + xmlElement.Name + "'");
            }
            if (v == null)
            {
                throw new XmlException("Cannot find 'Variance' in XmlElement '" + xmlElement.Name + "'");
            }

            frequency = Convert.ToInt32(f.Value, CultureInfo.InvariantCulture);
            variance = Convert.ToInt32(v.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Corrupt random bytes in the baml stream.
        /// </summary>
        public override void FuzzBamlRecords(BamlRecord[] records)
        {
            CoreLogger.LogStatus("Corrupting bytes.");
            CoreLogger.LogStatus("Frequency = " + frequency);
            CoreLogger.LogStatus("Variance = " + variance);

            if (records == null)
            {
                CoreLogger.LogStatus("No records available to fuzz -- Exiting.\r\n");
                return;
            }

            int cumulativeBytes = 0;
            int indexOfNextCorruption = AmountToSkip;

            for (int index = 0; index < records.Length; index++)
            {
                int recordSize = 2 + records[index].RawDataSize;

                // Does the next corrupt byte fall within this record?
                while (cumulativeBytes <= indexOfNextCorruption &&
                        indexOfNextCorruption < cumulativeBytes + recordSize)
                {
                    CoreLogger.LogStatus("Corrupting record " + index);
                    CoreLogger.LogStatus("Global corruption index = " + indexOfNextCorruption);

                    int localIndex = indexOfNextCorruption - cumulativeBytes;
                    if (localIndex < 2)
                    {
                        // Random corruption falls on the type field
                        CorruptTypeField(records[index], localIndex == 0);
                    }
                    else
                    {
                        CoreLogger.LogStatus("Data corruption at index = " + (localIndex - 2));

                        // Random corruption falls on the data field
                        if (records[index] is VariableSizeBamlRecord)
                        {
                            CorruptDataField((VariableSizeBamlRecord)records[index], localIndex - 2);
                        }
                        else
                        {
                            CorruptDataField(records[index], localIndex - 2);
                        }
                    }

                    // Set the next spot in the data stream to corrupt
                    indexOfNextCorruption += AmountToSkip;
                }

                cumulativeBytes += recordSize;
            }
            CoreLogger.LogStatus("");
        }

        private void CorruptTypeField(BamlRecord record, bool corruptLowByte)
        {
            CoreLogger.LogStatus("Type corruption.");

            // Promote to 32 bit int to avoid sign extension issues
            Int32 type = (Int32)record.Type;
            type &= 0x0000ffff;

            CoreLogger.LogStatus("Old Type = " + type);

            byte b = RandomByte;
            if (corruptLowByte)
            {
                type &= 0xff00;
                type |= b;
            }
            else // Corrupt High Byte
            {
                type &= 0x00ff;
                type |= (b << 8);
            }
            CoreLogger.LogStatus("New Type = " + type);
            record.Type = (Int16)type;
        }

        private void CorruptDataField(VariableSizeBamlRecord record, int offset)
        {
            // Promote to 64 bit int to avoid sign extension issues
            Int64 size = (Int64)record.Size;
            size &= 0x00000000ffffffff;
            byte b = RandomByte;

            switch (offset)
            {
            case 0:
                    size &= 0xffffff00;
                    size += b;
                    break;

            case 1:
                    size &= 0xffff00ff;
                    size += (uint)(b << 8);
                    break;

            case 2:
                    size &= 0xff00ffff;
                    size += (uint)(b << 16);
                    break;

            case 3:
                    size &= 0x00ffffff;
                    size += (uint)(b << 24);
                    break;

            default:
                    record.Data[offset-4] = b;
                    break;
            }

            if (size != record.Size)
            {
                CoreLogger.LogStatus("Size corruption.");
                CoreLogger.LogStatus("Old Size = " + record.Size);
                CoreLogger.LogStatus("New Size = " + size);
                record.Size = (Int32)size;
            }
        }

        private void CorruptDataField(BamlRecord record, int offset)
        {
            record.Data[offset] = RandomByte;
        }

        /// <summary>
        /// 
        /// </summary>
        protected int AmountToSkip
        {
            get
            {
                int n = random.Next(frequency - variance, frequency + variance);

                return (n < 1) ? 1 : n;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected byte RandomByte
        {
            get
            {
                return (byte)random.Next(0, 255);
            }
        }

        /// <summary>
        /// The frequency at which corruption happens.
        /// </summary>
        public int Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        /// <summary>
        /// The amount by which Frequency can vary (+/-).
        /// </summary>
        public int Variance
        {
            get { return variance; }
            set { variance = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int frequency;
        /// <summary>
        /// 
        /// </summary>
        protected int variance;
    }
}

