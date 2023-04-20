// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          BamlDasmHelper
    ******************************************************************************/

    /// <summary>
    /// Class for generating BamlDasm content from a .baml file.
    /// </summary>
    public class BamlDasmHelper
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          DisassembleBamlFile
        ******************************************************************************/

        /// <summary>
        /// Call the BamlDasm utility to create a BamlDasm string based on the specified Baml file.
        /// </summary>
        /// <param name="bamlFileName">The name of the .baml file.</param>
        /// <returns>The generated BamlDasm string</returns>
        public static string DisassembleBamlFile(string bamlFileName)
        {
            string bamlDasmString = string.Empty;

            //// TO-DO: Call the BamlDasm utility here using a TextWriter.

            return bamlDasmString;
        }

        #endregion
    }
}
