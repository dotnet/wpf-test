// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;

    public class ValueSerializerTests
    {
        #region BasicValueSerializerTest

        private readonly List<Type> _basicValueSerializerTestTypes = new List<Type>
                                                                        {
                                                                            typeof (PrimitiveClass),
                                                                            typeof (DerivedClassA),
                                                                            typeof (PrimitiveClassB),
                                                                            typeof (ClassWithValSerializerB),
                                                                            typeof (ClassReceivingException),
                                                                            typeof (ContentTypeClass),
                                                                            typeof (ManagerWithValueSerializer),
                                                                            typeof (ClassWithNoSerializer),
                                                                            typeof (IntListWithValueSerializer),
                                                                        };

        [TestCaseGenerator]
        public void BasicValueSerializerTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._basicValueSerializerTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicValueSerializerTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._basicValueSerializerTestTypes, instanceID));
        }

        #endregion

        #region ValueSerializerOnPropertyTest

        private readonly List<Type> _valueSerializerOnPropertyTestTypes = new List<Type>
                                                                             {
                                                                                 typeof (NoTCVSOnPropContainer),
                                                                                 typeof (TCVSOnBothContainer),
                                                                                 typeof (TCOnPropVSOnPropContainer),
                                                                                 typeof (TCOnTypeVSOnPropContainer),
                                                                                 typeof (TCOnPropVSOnTypeContainer),
                                                                                 typeof (VSOnStringPropertyContainer)
                                                                             };

        [TestCaseGenerator]
        public void ValueSerializerOnPropertyTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._valueSerializerOnPropertyTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void ValueSerializerOnPropertyTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._valueSerializerOnPropertyTestTypes, instanceID));
        }

        #endregion

        //[TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        //public void ValueSerializerWithXamlTemplateTest()
        //{
        //    Point p = new Point
        //    {
        //        X = 3,
        //        Y = 42
        //    };

        //    IEnumerable<XamlNode> nodes = new XamlXmlReader(new StringReader(XamlServices.Save(p))).ReadToEnd();

        //    PointContainer pc = new PointContainer
        //    {
        //        Point = new PointWithXamlTemplate(nodes)
        //        {
        //            X = 42,
        //            Y = 3,
        //        },

        //    };

        //    string xaml = XamlServices.Save(pc);
        //    XamlServices.Parse(xaml);
        //}
    }
}
