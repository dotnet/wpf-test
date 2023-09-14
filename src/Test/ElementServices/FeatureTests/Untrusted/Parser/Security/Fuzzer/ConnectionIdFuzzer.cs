// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      A fuzzer that changes ConnectionIds in
 *      a baml file.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionIdFuzzer : BamlFuzzer
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectionIdFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionIdFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
            XmlAttribute adi = xmlElement.Attributes["AllowDuplicateIds"];

            _allowDuplicateIds = (adi == null) ? false : Convert.ToBoolean(adi.Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Grab all the ConnectionId records and swap their values.  This will
        /// ensure that the ConnectionIds are valid, but they are referring to
        /// the wrong objects.
        /// </summary>
        public override void FuzzBamlRecords(BamlRecord[] records)
        {
            CoreLogger.LogStatus("Fuzzing ConnectionId records.");
            CoreLogger.LogStatus("AllowDuplicateIds = " + _allowDuplicateIds);

            if (records == null)
            {
                CoreLogger.LogStatus("No records available to fuzz -- Exiting.\r\n");
                return;
            }

            SortedList<int,int> ids = new SortedList<int,int>();

            // The terms "index" and "key" are used interchangeably in the following code.
            // This is because the index to the ConnectionIdBamlRecord is used as the key
            //  for the SortedList.
            // i.e. ids.Keys[0] is the index to the first ConnectionIdBamlRecord in "records"

            for (int index = 0; index < records.Length; index++)
            {
                if (records[index] is ConnectionIdBamlRecord)
                {
                    int id = ((ConnectionIdBamlRecord)records[index]).ConnectionId;
                    ids.Add(index, id);
                }
            }

            if (ids.Count == 0)
            {
                CoreLogger.LogStatus("No ConnectionId records found.");
            }
            else if (ids.Count == 1)
            {
                CoreLogger.LogStatus("Only 1 ConnectionId record found.");
                CoreLogger.LogStatus("Setting ConnectionId to random number...");

                // Get the index (aka: key) of the record to alter
                int index = ids.Keys[0];
                ConnectionIdBamlRecord r = (ConnectionIdBamlRecord)records[index];

                // Set the ConnectionId to some random number
                while (r.ConnectionId == ids[index])
                {
                    r.ConnectionId = random.Next(Int32.MinValue, Int32.MaxValue);
                }
                CoreLogger.LogStatus("new ConnectionId = " + r.ConnectionId);
            }
            else
            {
                CoreLogger.LogStatus(ids.Count + " ConnectionIds found");
                CoreLogger.LogStatus("Swapping valid Ids...");

                // Get the indices (aka: keys) of the records to alter
                int[] indices = new int[ids.Count];
                ids.Keys.CopyTo(indices, 0);

                // Get a random ConnectionId and set it on a random record
                foreach (int index in indices)
                {
                    // Get the record to alter
                    ConnectionIdBamlRecord r = (ConnectionIdBamlRecord)records[index];

                    int indexOfRandomRecord;
                    Int32 oldId = r.ConnectionId;

                    // Set its ConnectionId to some random valid ConnectionId
                    do
                    {
                        int randomKey = random.Next(0, ids.Count);
                        indexOfRandomRecord = ids.Keys[randomKey];
                        r.ConnectionId = ids[indexOfRandomRecord];

                    } while (r.ConnectionId == oldId && ids.Count > 1);

                    CoreLogger.LogStatus("Record[" + index + "].ConnectionId = " + r.ConnectionId);

                    if (!_allowDuplicateIds)
                    {
                        // Remove the ConnectionId used so that none are used twice
                        ids.Remove(indexOfRandomRecord);
                    }
                }

                CoreLogger.LogStatus("Done.");
            }
            CoreLogger.LogStatus("");
        }

        /// <summary>
        /// Specifies whether or not all ConnectionIds should be unique.
        /// </summary>
        public bool AllowDuplicateIds
        {
            get { return _allowDuplicateIds; }
            set { _allowDuplicateIds = value; }
        }

        private bool _allowDuplicateIds;
    }
}

