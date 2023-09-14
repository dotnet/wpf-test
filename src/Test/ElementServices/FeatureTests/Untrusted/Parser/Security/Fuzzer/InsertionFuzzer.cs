// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      A fuzzer that changes the size of a
 *      BamlRecord by inserting random data.
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
    public class InsertionFuzzer : RandomByteFuzzer
    {
        /// <summary>
        /// 
        /// </summary>
        public InsertionFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InsertionFuzzer(XmlElement xmlElement, Random random)
            : base(xmlElement, random)
        {
            XmlAttribute mbi = xmlElement.Attributes["MaxBytesToInsert"];

            _maxBytesToInsert = (mbi == null) ? 0 : Convert.ToInt32(mbi.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Fuzz by inserting random bytes into a VariableSizeBamlRecord.
        /// The size of the record will be changed to reflect the insertions made.
        /// </summary>
        public override void FuzzBamlRecords(BamlRecord[] records)
        {
            CoreLogger.LogStatus("Inserting bytes.");
            CoreLogger.LogStatus("Frequency = " + frequency);
            CoreLogger.LogStatus("Variance = " + variance);
            CoreLogger.LogStatus("MaxBytesToInsert = " + _maxBytesToInsert);

            if (records == null)
            {
                CoreLogger.LogStatus("No records available to fuzz -- Exiting.\r\n");
                return;
            }

            int indexOfNextCorruption = AmountToSkip;

            if (indexOfNextCorruption < 0 || records.Length <= indexOfNextCorruption)
            {
                // The first random index was outside the valid range.
                // Just fuzz any record in the array.
                FuzzOne(records);
            }
            else
            {
                FuzzMany(records, indexOfNextCorruption);
            }
            CoreLogger.LogStatus("");
        }

        private void FuzzOne(BamlRecord[] records)
        {
            while (true)
            {
                int index = random.Next(0, records.Length-1);
                if (records[index] is VariableSizeBamlRecord)
                {
                    CoreLogger.LogStatus("Inserting bytes into record #" + index);
                    InsertRandomBytes((VariableSizeBamlRecord)records[index]);
                    break;
                }
            }
        }

        private void FuzzMany(BamlRecord[] records, int indexOfNextCorruption)
        {
            for (int index = 0; index < records.Length; index++)
            {
                if (indexOfNextCorruption <= index &&
                     records[index] is VariableSizeBamlRecord)
                {
                    CoreLogger.LogStatus("Inserting bytes into record #" + index);
                    InsertRandomBytes((VariableSizeBamlRecord)records[index]);

                    indexOfNextCorruption = index + AmountToSkip;
                }
            }
        }

        private void InsertRandomBytes(VariableSizeBamlRecord record)
        {
            byte[] oldBytes = record.Data;
            int insertionBytesLength = random.Next(1, (_maxBytesToInsert <= 0) ? oldBytes.Length : _maxBytesToInsert);
            byte[] insertionBytes = new byte[insertionBytesLength];
            byte[] newBytes = new byte[oldBytes.Length + insertionBytes.Length];
            random.NextBytes(newBytes);

            CoreLogger.LogStatus("Inserting " + insertionBytes.Length + " bytes randomly...");

            int oldIndex = 0;
            int insertionIndex = 0;
            double ratio = (double)insertionBytes.Length / (double)newBytes.Length;

            for (int i = 0; i < newBytes.Length; i++)
            {
                if (oldIndex == oldBytes.Length)
                {
                    // We don't have anymore valid bytes to copy
                    newBytes[i] = insertionBytes[insertionIndex++];
                }
                else if (insertionIndex == insertionBytes.Length)
                {
                    // We don't have anymore random bytes to insert
                    newBytes[i] = oldBytes[oldIndex++];
                }
                else
                {
                    // Randomly decide whether to insert or copy a byte
                    newBytes[i] = (random.NextDouble() < ratio) ? insertionBytes[insertionIndex++] : oldBytes[oldIndex++];
                }
            }
            record.Data = newBytes;

            // Add 4 to the size because the Data does not include
            //  the 4 bytes required to store the size field
            record.Size = newBytes.Length + 4;
        }

        /// <summary>
        /// The maximum number of bytes you can insert per BamlRecord.
        /// If unset (or equal to 0), the maximum will be the size of the BamlRecord being fuzzed.
        /// </summary>
        public int MaxBytesToInsert
        {
            get { return _maxBytesToInsert; }
            set { _maxBytesToInsert = value; }
        }

        private int _maxBytesToInsert;
    }
}

