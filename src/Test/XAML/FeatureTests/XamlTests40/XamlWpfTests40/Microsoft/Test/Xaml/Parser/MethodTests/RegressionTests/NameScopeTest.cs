// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xaml;
using Microsoft.Test.Xaml.Common.XamlOM;
using Microsoft.Test.Xaml.Types.InstanceReference;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Add coverage for System.Windows.NameScope
    /// </summary>
    public class NameScopeTest
    {
        /// <summary>
        /// Add coverage for a few methods in System.Windows.NameScope
        /// </summary>
        public static void NameScopeCoverage()
        {
            NodeList xamlDoc = new NodeList
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("bar", false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        NodeListFactory.CreateReference("bar2", false),
                    new EndMember(),
                new EndObject()                    
            };

            NameScope scope = new NameScope();

            Assert.AreEqual(scope.Count, 0);
            Assert.AreEqual(scope.Values, null);
            Assert.AreEqual(scope.Keys, null);

            var bar = new SimpleRefBar();
            var bar2 = new SimpleRefBar();
            scope["bar"] = bar;
            scope["bar2"] = bar2;

            var xows = new XamlObjectWriterSettings { ExternalNameScope = scope };
            var xxr = new XamlXmlReader(new StringReader(xamlDoc.NodeListToXml()));
            var xow = new XamlObjectWriter(xxr.SchemaContext, xows);

            XamlServices.Transform(xxr, xow);

            Assert.AreEqual(scope.Count, 2);
            Assert.AreEqual(scope.Values.Count, 2);
            Assert.AreEqual(scope.Keys.Count, 2);
            
            var pairs = new KeyValuePair<string, object>[scope.Count];
            scope.CopyTo(pairs, 0);
            Assert.AreEqual(pairs.Length, 2);
        }
    }
}
