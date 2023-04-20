// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace XamlTestsDev10.Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    ///  Regression tests for TF 613715
    /// </summary>
    public class XamlSubReaderRegressionTests
    {
        /// <summary>
        /// Read the parent until the end and then try
        /// reading the subreader
        /// </summary>
        public void XamlReaderAtEndTest()
        {
            string xaml = XamlServices.Save(new List<string>() { "Hello world" });

            XamlXmlReader reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)));
            
            // Read one node //
            reader.Read();
            XamlReader subReader = reader.ReadSubtree();
            while (reader.Read())
            { 
            }

            // Read over None Node //
            subReader.Read();

            if (subReader.Read() != false)
            {
                GlobalLog.LogEvidence("SubReader.Read returned true when false was expected");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("SubReader.Read returned false as expected");
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }
}
