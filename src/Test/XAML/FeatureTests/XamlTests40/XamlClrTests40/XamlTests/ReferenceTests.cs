// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System.Collections.Generic;
    using System.Xaml;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Types.CreateFrom;

    public class ReferenceTests
    {
        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void ReferenceEqualityCheckTest()
        {
            SameEqualsHashCode foo1 = new SameEqualsHashCode
                                          {
                                              B = "blah", A = 27
                                          };
            SameEqualsHashCode foo2 = new SameEqualsHashCode
                                          {
                                              B = "blah2", A = 15
                                          };
            List<SameEqualsHashCode> foos = new List<SameEqualsHashCode>
                                                {
                                                    foo1,
                                                    foo2,
                                                    foo1,
                                                    foo2
                                                };

            List<SameEqualsHashCode> foosAfter = (List<SameEqualsHashCode>) XamlServices.Parse(XamlServices.Save(foos));

            if (foosAfter[0] != foosAfter[2] ||
                foosAfter[1] != foosAfter[3])
            {
                throw new DataTestException("Expected references to be equal.");
            }
        }
    }

    public class SameEqualsHashCode
    {
        public int A { get; set; }
        public string B { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Foo)
            {
                return true;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return 3;
        }
    }
}
