// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System;
    using System.IO;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// ObjectDoubleRoundtripDriver : 
    /// Takes an object, serialize, deserialize, serailize then deserialize
    /// compare the intermediate objects twice and also optionally inspect the
    /// intermediate XAML
    /// </summary>
    public class ObjectDoubleRoundtripDriver : XamlTestDriverBase
    {
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="source">test identifier</param>
        /// <param name="testCaseInfo">test case information</param>
        protected override void ExecuteTest(string source, TestCaseInfo testCaseInfo)
        {
            Func<XamlSchemaContext> context = null;
            if (testCaseInfo is SchemaExtensibilityTestCaseInfo)
            {
                context = ((SchemaExtensibilityTestCaseInfo)testCaseInfo).Context;
            }

            //// object -> xaml
            MemoryStream stream1 = context == null ? Serialize(testCaseInfo.Target) : Serialize(testCaseInfo.Target, context());

            //// run xpath inspectors
            string xaml = new StreamReader(stream1).ReadToEnd();
            stream1.Position = 0;
            XPathInspector.Inspect(xaml, testCaseInfo.XPathExpresions, testCaseInfo.XPathNamespacePrefixMap);

            //// xaml -> object
            object object1 = context == null ? Deserialize(stream1) : Deserialize(stream1, context());

            //// compare the two objects
            if (testCaseInfo.CompareAttachedProperties)
            {
                XamlObjectComparer.CompareObjectsAndAttachedProperties(testCaseInfo.Target, object1);
            }
            else
            {
                XamlObjectComparer.CompareObjects(testCaseInfo.Target, object1);
            }

            //// object -> xaml 
            MemoryStream stream2 = context == null ? Serialize(object1) : Serialize(object1, context());

            //// run xpath inspectors
            string xaml2 = new StreamReader(stream2).ReadToEnd();
            stream2.Position = 0;
            XPathInspector.Inspect(xaml2, testCaseInfo.XPathExpresions, testCaseInfo.XPathNamespacePrefixMap);

            //// xaml -> object 
            object object2 = context == null ? Deserialize(stream2) : Deserialize(stream2, context());

            //// compare the two objects
            if (testCaseInfo.CompareAttachedProperties)
            {
                XamlObjectComparer.CompareObjectsAndAttachedProperties(object1, object2);
            }
            else
            {
                XamlObjectComparer.CompareObjects(object1, object2);
            }
        }
    }
}
