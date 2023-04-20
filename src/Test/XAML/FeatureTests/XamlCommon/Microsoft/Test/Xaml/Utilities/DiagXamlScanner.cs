// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// DiagXaml Scanner
    /// </summary>
    public class DiagXamlScanner
    {
        /// <summary>
        /// diagXaml String
        /// </summary>
        private readonly string _diagXamlString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagXamlScanner"/> class.
        /// </summary>
        /// <param name="diagXaml">The diag xaml.</param>
        public DiagXamlScanner(string diagXaml)
        {
            _diagXamlString = diagXaml;
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>List of string args</returns>
        public List<string[]> Read()
        {
            List<string[]> instructionList = new List<string[]>();
            string[] lineSplitArray = _diagXamlString.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lineSplitArray)
            {
                instructionList.Add(ParseInstruction(line));
            }

            return instructionList;
        }

        /// <summary>
        /// Parses the instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <returns>parsed string</returns>
        private string[] ParseInstruction(string instruction)
        {
            string[] tabSplit = instruction.Split(new string[1] { "\t" }, StringSplitOptions.None);
            string[] output = new string[4];
            output[0] = tabSplit[0];
            switch (tabSplit[0])
            {
                case "NS":
                    output[1] = tabSplit[1].Substring(0, tabSplit[1].IndexOf('='));
                    output[2] = tabSplit[1].Substring(tabSplit[1].IndexOf('=') + 1);
                    break;
                case "SO":
                case "SM":
                    output[1] = tabSplit[1].Substring(0, tabSplit[1].IndexOf(':'));
                    output[2] = tabSplit[1].Substring(tabSplit[1].IndexOf(':') + 1);
                    if (tabSplit.Length > 2)
                    {
                        output[3] = tabSplit[2];
                    }

                    break;
                case "V":
                    output[1] = tabSplit[1];
                    break;
            }

            return output;
        }
    }
}
