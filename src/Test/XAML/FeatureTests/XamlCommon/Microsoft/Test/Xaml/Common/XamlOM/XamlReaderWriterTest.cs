// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.Xaml.Driver;

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    /// <summary>
    /// XamlReaderWriter test.
    /// Two test types for each scenario
    /// * Write using XamlXmlWriter, Read using XamlXmlReader
    /// * Write using TestXmlWriter, Reader using XamlXmlReader
    /// </summary>
    public abstract class XamlReaderWriterTest
    {
        /// <summary>
        /// Gets the Input NodeLists
        /// </summary>
        public abstract IEnumerable<NodeList> NodeLists { get; }

        /// <summary>
        ///  Gets the Collection of TestCaseInfo objects, describing the tests
        /// </summary>
        public abstract ICollection<TestCaseInfo> TestCases { get; }

        /// <summary>
        /// Get the TestCaseInfo colleciton
        /// </summary>
        /// <param name="testIdPrefix">test case id prefix</param>
        /// <returns>collection of test case infos</returns>
        protected virtual ICollection<TestCaseInfo> GetTestCaseInfo(string testIdPrefix)
        {
            var drivers = new List<TestDrivers> 
            { 
                TestDrivers.XamlXmlWriterXamlXmlReaderDriver, 
                TestDrivers.NodeWriterXamlXmlReaderDriver 
            };

            return (from driver in drivers
                    from nodeList in NodeLists
                    select new TestCaseInfo
                    {
                        Target = nodeList,
                        TestDriver = driver,
                        TestID = driver.ToString() + "+" + nodeList.Name + testIdPrefix
                    }).ToList();
        }
    }
}
