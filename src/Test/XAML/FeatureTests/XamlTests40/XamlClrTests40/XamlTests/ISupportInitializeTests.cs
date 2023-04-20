// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Infrastructure.Test;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Xaml.Driver;
using Microsoft.Test.Xaml.Types.ISupportInitializeTypes;

namespace Microsoft.Test.Xaml.XamlTests
{
    public class ISupportInitializeTests
    {
        [TestCase]
        public void SimpleISupportInitialize()
        {
            SimpleIsupportInitialize obj = new SimpleIsupportInitialize()
                                               {
                                                   IntProperty = 10
                                               };

            string xaml = XamlTestDriver.Serialize(obj);
            SimpleIsupportInitialize roundTripped = (SimpleIsupportInitialize) XamlTestDriver.Deserialize(xaml, null);

            if (!roundTripped.BeginInitCalled || !roundTripped.EndInitCalled)
            {
                throw new TestCaseFailedException("BeginInit or EndInit was not invoked");
            }
        }

        [TestCase]
        public void NestedISupportInitialize()
        {
            IsupportInitializeParent obj = new IsupportInitializeParent()
                                               {
                                                   ISIProperty = new SimpleIsupportInitialize()
                                                                     {
                                                                         IntProperty = 100,
                                                                     },
                                               };

            string xaml = XamlTestDriver.Serialize(obj);
            IsupportInitializeParent roundTripped = (IsupportInitializeParent) XamlTestDriver.Deserialize(xaml, null);

            if (!roundTripped.BeginInitCalled || !roundTripped.EndInitCalled)
            {
                throw new TestCaseFailedException("BeginInit or EndInit was not invoked");
            }

            if (!roundTripped.ISIProperty.BeginInitCalled || !roundTripped.ISIProperty.EndInitCalled)
            {
                throw new TestCaseFailedException("BeginInit or EndInit was not invoked");
            }
        }
    }
}
