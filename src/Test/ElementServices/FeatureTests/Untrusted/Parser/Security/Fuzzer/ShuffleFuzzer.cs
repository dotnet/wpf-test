// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      A fuzzer that shuffles objects in an array.
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
    public class ShuffleFuzzer : BamlFuzzer
    {
        /// <summary>
        /// 
        /// </summary>
        public ShuffleFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ShuffleFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
            XmlAttribute ns = xmlElement.Attributes["NumShuffles"];

            if (ns == null)
            {
                throw new XmlException("Cannot find 'NumShuffles' in XmlElement '" + xmlElement.Name + "'");
            }

            _numShuffles = Convert.ToInt32(ns.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Fuzz an array by swaping random items.
        /// </summary>
        public override void FuzzBamlRecords(BamlRecord[] records)
        {
            CoreLogger.LogStatus("Shuffling records.");
            CoreLogger.LogStatus("NumShuffles = " + _numShuffles);

            if (records == null)
            {
                CoreLogger.LogStatus("No records available to fuzz -- Exiting.\r\n");
                return;
            }

            for (int n = 0; n < _numShuffles; n++)
            {
                int position1 = random.Next(0, records.Length-1);
                int position2 = random.Next(0, records.Length-1);

                while (position1 == position2)
                {
                    position2 = random.Next(0, records.Length-1);
                }

                CoreLogger.LogStatus("Swapping these two:");
                CoreLogger.LogStatus("Record " + position1);
                CoreLogger.LogStatus(records.GetValue(position1).ToString());
                CoreLogger.LogStatus("Record " + position2);
                CoreLogger.LogStatus(records.GetValue(position2).ToString());

                // Swap record positions
                object object1 = records.GetValue(position1);
                object object2 = records.GetValue(position2);
                records.SetValue(object1, position2);
                records.SetValue(object2, position1);
            }
            CoreLogger.LogStatus("");
        }

        /// <summary>
        /// The number of item swaps that should be performed during the
        /// Fuzz operation.
        /// </summary>
        public int NumShuffles
        {
            get { return _numShuffles; }
            set { _numShuffles = value; }
        }

        private int _numShuffles;
    }
}

