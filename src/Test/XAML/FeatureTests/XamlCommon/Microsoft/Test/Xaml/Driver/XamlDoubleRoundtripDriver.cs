// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;

namespace Microsoft.Test.Xaml.Driver
{
    /// <summary>
    /// XamlDoubleRoundtripDriver:
    /// start with xaml, deserialize, serialize, deserialize then serialize
    /// optionally inspect the intermediate xaml 
    /// compare the intermediate objects 
    /// </summary>
    internal class XamlDoubleRoundtripDriver : XamlTestDriverBase
    {
        /// <summary>
        /// Execute the test
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="testCaseInfo">test case information</param>
        protected override void ExecuteTest(string source, TestCaseInfo testCaseInfo)
        {
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(testCaseInfo.Target as string));
            stream.Position = 0;

            // xaml -> object
            object object1 = Deserialize(stream);

            // inspect the object (InspectMethod)
            XamlFirstTestCaseInfo xamlTest = testCaseInfo as XamlFirstTestCaseInfo;
            if (xamlTest != null && xamlTest.InspectMethod != null)
            {
                xamlTest.InspectMethod(object1);
            }

            // object -> xaml 
            MemoryStream stream1 = Serialize(object1);

            // xaml -> object
            object object2 = Deserialize(stream1);

            // compare the two 
            if (testCaseInfo.CompareAttachedProperties)
            {
                XamlObjectComparer.CompareObjectsAndAttachedProperties(object1, object2);
            }
            else
            {
                XamlObjectComparer.CompareObjects(object1, object2);
            }

            // object -> xaml (no validation on this)
            MemoryStream stream2 = Serialize(object2);
        }
    }
}
