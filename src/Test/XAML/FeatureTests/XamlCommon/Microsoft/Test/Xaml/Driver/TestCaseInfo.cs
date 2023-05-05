// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Delegate describing the test method
    /// </summary>
    /// <param name="source">some string identifier</param>
    /// <param name="info">test case information</param>
    public delegate void RunTest(string source, TestCaseInfo info);

    /// <summary>
    /// Test case information 
    /// </summary>
    public class TestCaseInfo
    {
        /// <summary>
        /// set of xpath expressions to verify
        /// </summary>
        private readonly HashSet<string> _xpathExpressions = new HashSet<string>();

        /// <summary>
        /// Dictionary of prefix, namespace mappings
        /// </summary>
        private readonly Dictionary<string, string> _xpathNamespacePrefixMap = new Dictionary<string, string>
                                                                                  {
                                                                                      {
                                                                                          "mtxtc", "clr-namespace:Microsoft.Test.Xaml.Types.ContentProperties;assembly=XamlClrTypes"
                                                                                          },
                                                                                      {
                                                                                          "mtxt", "http://XamlTestTypes"
                                                                                          },
                                                                                      {
                                                                                          "x", Namespaces.Namespace2006
                                                                                          },
                                                                                      {
                                                                                          "x2", Namespaces.NamespaceV2
                                                                                          },
                                                                                      {
                                                                                          "xasl", Namespaces.NamespaceBuiltinTypes
                                                                                          }
                                                                                  };

        /// <summary>
        /// Initializes a new instance of the TestCaseInfo class
        /// </summary>
        public TestCaseInfo()
        {
            this.ExpectedResult = true;
            this.ExpectedMessage = null;
        }

        /// <summary>
        /// Gets or sets the BugNumber property
        /// Bug number associated with this test
        /// </summary>
        public int BugNumber { get; set; }

        /// <summary>
        /// Gets or sets the Target property
        /// Target object to work on
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test is expected to pass
        /// </summary>
        public bool ExpectedResult { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedMessage property
        /// If the test is expected to fail, what is
        /// the expected exception message
        /// </summary>
        public string ExpectedMessage { get; set; }

        /// <summary>
        /// Gets or sets the TestID property
        /// Test identifier
        /// </summary>
        public string TestID { get; set; }

        /// <summary>
        /// Gets or sets the TestDriver property
        /// Test driver to use
        /// </summary>
        public TestDrivers TestDriver { get; set; }

        /// <summary>
        /// Gets the XPathExpressions property
        /// </summary>
        public ICollection<string> XPathExpresions
        {
            get
            {
                return _xpathExpressions;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether test is expected to pass AttachedProeprties 
        /// are compared
        /// </summary>
        public bool CompareAttachedProperties { get; set; }

        /// <summary>
        /// Gets the prefix/namespace mapping 
        /// </summary>
        public IDictionary<string, string> XPathNamespacePrefixMap
        {
            get
            {
                return _xpathNamespacePrefixMap;
            }
        }
    }
}
