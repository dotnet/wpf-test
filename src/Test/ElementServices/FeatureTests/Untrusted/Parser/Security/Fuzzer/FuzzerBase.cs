// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Base class for all fuzzing operations.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.Reflection;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Utilities;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// Base class for all fuzzing operations.
    /// </summary>
    public abstract class FuzzerBase
    {
        /// <summary>
        /// Creates a FuzzerBase sub-type according to the given xml test description.
        /// </summary>
        /// <param name="xmlElement">The xml describing the type to create.</param>
        /// <param name="random">The global random number generator.</param>
        /// <returns>A new FuzzerBase object.</returns>
        public static FuzzerBase Create(XmlElement xmlElement, Random random)
        {
            string typeName = "Avalon.Test.CoreUI.Parser.Security." + xmlElement.Name;
            InternalObject internalObject = InternalObject.CreateInstance(typeof(FuzzerBase).Assembly.FullName, typeName, new object[] { xmlElement, random });
            FuzzerBase fuzzer = (FuzzerBase)internalObject.Target;

            return fuzzer;
        }

        /// <summary>
        /// Creates a default FuzzerBase object.
        /// </summary>
        /// <param name="random"></param>
        protected FuzzerBase(Random random)
        {
            this.random = random;
        }

        /// <summary>
        /// The global random number generator.
        /// </summary>
        protected Random random = null;

        /// <summary>
        /// Returns a randomly-generated string.
        /// </summary>
        /// <returns></returns>
        public string GetFuzz()
        {
            byte[] bytes = new byte[100];
            (random).NextBytes(bytes);
            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Returns a randomly-generated string.
        /// </summary>
        /// <returns></returns>
        public static string GetFuzz(Random random)
        {
            byte[] bytes = new byte[100];
            random.NextBytes(bytes);
            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            return encoding.GetString(bytes);
        }
    }
}

