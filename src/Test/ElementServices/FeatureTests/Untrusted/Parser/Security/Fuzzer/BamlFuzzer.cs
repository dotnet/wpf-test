// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Base class for all fuzzing operations
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.Reflection;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BamlFuzzer : FuzzerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected BamlFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected BamlFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
        }

        /// <summary>
        /// </summary>
        public abstract void FuzzBamlRecords(BamlRecord[] array);
    }
}

