// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a BamlRecord that has a fixed size.
 *      May be extended to perform specific fuzzing
 *      operations for different BamlRecord types.
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
    public class FixedSizeBamlRecord : BamlRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="data"></param>
        public FixedSizeBamlRecord(Int16 type, Int32 size, byte[] data)
            : base(type, size, data)
        {
        }

        /// <summary>
        /// The size (in bytes) of this BamlRecord's raw data.
        /// </summary>
        public override int RawDataSize
        {
            get
            {
                return data.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override byte[] ToByteArray()
        {
            // type = 2 bytes, data = ? bytes
            byte[] bytes = new byte[2 + RawDataSize];

            MemCopy(type, bytes, 0);
            if (data.Length > 0)
            {
                MemCopy(data, bytes, 2);
            }

            return bytes;
        }
    }
}
