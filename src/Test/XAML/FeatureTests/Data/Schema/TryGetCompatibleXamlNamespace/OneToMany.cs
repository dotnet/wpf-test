// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;
using Microsoft.Test.Logging;

[assembly: XmlnsDefinition("http://a", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsDefinition("http://b", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsDefinition("http://c", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsCompatibleWith("http://a", "http://b")]
[assembly: XmlnsCompatibleWith("http://a", "http://c")]

namespace Microsoft.Test.Xaml.Schema
{
    /// <summary>
    /// One xmlns subsumed by many xmlns
    /// </summary>
    [SchemaTest]
    class OneToMany : TryGetCompatibleXamlNamespaceTest
    {
        public override void Run()
        {
            try
            {
                string outValue = null;
                SchemaContext.TryGetCompatibleXamlNamespace("http://a", out outValue);
            }
            // 
            catch (Exception)
            {
                GlobalLog.LogDebug("Exception thrown as expected");
                return;
            }

            GlobalLog.LogEvidence("Exception not thrown");
            throw new SchemaTestFailedException();
        }
    }
}
