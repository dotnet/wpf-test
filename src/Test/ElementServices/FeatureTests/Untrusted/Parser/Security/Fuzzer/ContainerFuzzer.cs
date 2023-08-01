// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Base class for all container fuzzing operations
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class ContainerFuzzer : FuzzerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ContainerFuzzer(Random random)
            : base(random)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ContainerFuzzer(XmlElement xmlElement, Random random)
            : base(random)
        {
        }

        /// <summary>
        /// </summary>
        public virtual void FuzzContainer(string sourceFilePath, string destinationFilePath)
        {
            this.FuzzRandom(sourceFilePath);

            File.Copy(_fuzzedContainerPath, destinationFilePath, true);
        }

        private void FuzzRandom(string fileName)
        {
            Stream inStream = File.OpenRead(fileName);
            int inc = 4;
            int read = 0;
            Stream outStream = new FileStream(_fuzzedContainerPath, FileMode.Create, FileAccess.Write);
            byte[] buffer = new byte[inc];
            while (0 < (read = inStream.Read(buffer, 0, inc)))
            {
                if (random.Next(40) < 1)
                {
                    random.NextBytes(buffer);
                }
                outStream.Write(buffer, 0, read);
            }

            inStream.Close();
            outStream.Close();
        }

        private readonly string _fuzzedContainerPath = "_containerFuzzerTemp.xps";
    }
}

