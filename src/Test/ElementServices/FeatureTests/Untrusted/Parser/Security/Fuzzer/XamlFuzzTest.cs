// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a xaml fuzz test. Contains overrides for generating, 
 *      fuzzing, and loading xaml files.
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.IO.Packaging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// </summary>
    public class XamlFuzzTest : ParserFuzzTest
    {
        /// <summary> 
        /// Represents a xaml fuzz test. Contains overrides for generating, 
        /// fuzzing, and loading xaml files.
        /// </summary>
        public XamlFuzzTest() : base()
        {
        }

        /// <summary>
        /// </summary>
        public XamlFuzzTest(XmlElement xmlElement) : base(xmlElement)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFilePath"></param>
        protected override void DoFuzz(string sourceFilePath, string destinationFilePath)
        {
            File.Copy(sourceFilePath, destinationFilePath, true);

            // Fuzz the records.
            foreach (XamlFuzzer fuzzer in this.Fuzzers)
            {
                fuzzer.FuzzXaml(destinationFilePath, destinationFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fuzzedFile"></param>
        protected override void TestFuzzedFile(string fuzzedFile)
        {
            FileStream fileStream = File.OpenRead(fuzzedFile);

            try
            {
                CoreLogger.LogStatus("Loading fuzzed xaml...");
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                System.Windows.Markup.XamlReader.Load(fileStream, pc);
            }
            finally
            {
                fileStream.Close();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override bool IsExceptionOkay(Exception ex)
        {
            return (ex is XamlParseException) &&
                   base.IsExceptionOkay(ex);
        }
    }
}

