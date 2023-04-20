// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Samples
{
    /// <summary>
	/// <area>XmlWorker</area>

	/// <priority>2</priority>
    /// <description>
    /// Provides coverage for Xml bugs.
    /// - Verifies that XmlNamespaceMappingCollection will not throw an exception when the enumerator 
    /// is invoked on a collection with non-default mappings (1699577)
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(2, "Xml", TestCaseSecurityLevel.PartialTrust,"XmlBugsCoverage")]
    public class XmlBugsCoverage : WindowTest
    {
        public XmlBugsCoverage()
        {
            RunSteps += new TestStep(MappingCollectionException);
        }

        private TestResult MappingCollectionException()
        {
            Status("MappingCollectionException");

            XmlNamespaceMappingCollection collection = new XmlNamespaceMappingCollection();
            XmlNamespaceMapping mapping = new XmlNamespaceMapping("dpi", new Uri("clr-namespace:UntitleProject1"));
            collection.Add(mapping);
            int count = collection.Count; // this used to throw

            return TestResult.Pass;
        }
    }
}
