// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a ConnectionId BamlRecord
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConnectionIdBamlRecord : FixedSizeBamlRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public ConnectionIdBamlRecord(byte[] data)
            : base((Int16)BamlRecordType.ConnectionId, 4, data)
        {
        }

        /// <summary>
        /// A convenient way to access the ConnectionId from the raw Data
        /// </summary>
        public Int32                ConnectionId
        {
            get { return ToInt32(data, 0); }
            set { MemCopy(value, data, 0); }
        }
    }
}
