// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xaml;
    using Microsoft.Infrastructure.Test;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using XT = Microsoft.Test.Xaml.Types.PointDisambiguationNamespace;

    public class PrimitiveTests
    {
        #region BasicPrimitiveTest

        private List<Type> _basicPrimitiveTestTypes;

        private List<Type> BasicPrimitiveTestTypes
        {
            get
            {
                if (this._basicPrimitiveTestTypes == null)
                {
                    this._basicPrimitiveTestTypes = new List<Type>();
                    this._basicPrimitiveTestTypes.Add(typeof (BasicPrimitiveTypes));
                    this._basicPrimitiveTestTypes.Add(typeof (SpecialPrimitiveValues));
                }
                return this._basicPrimitiveTestTypes;
            }
        }

        [TestCaseGenerator]
        public void BasicPrimitiveTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.BasicPrimitiveTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicPrimitiveTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.BasicPrimitiveTestTypes, instanceID));
        }

        #endregion

        #region DateTimeKindTest

        private List<Type> _dateTimeKindTestTypes;

        private List<Type> DateTimeKindTestTypes
        {
            get
            {
                if (this._dateTimeKindTestTypes == null)
                {
                    this._dateTimeKindTestTypes = new List<Type>();
                    this._dateTimeKindTestTypes.Add(typeof (DateTimeKindType));
                }
                return this._dateTimeKindTestTypes;
            }
        }

        [TestCaseGenerator]
        public void DateTimeKindTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.DateTimeKindTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void DateTimeKindTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.DateTimeKindTestTypes, instanceID));
        }

        #endregion

        #region PrimitiveOnTopTest

        private List<Type> _primitiveOnTopTestTypes;

        private List<Type> PrimitiveOnTopTestTypes
        {
            get
            {
                if (this._primitiveOnTopTestTypes == null)
                {
                    this._primitiveOnTopTestTypes = new List<Type>();
                    this._primitiveOnTopTestTypes.Add(typeof (PrimitivesOnTopWrapper));
                }
                return _primitiveOnTopTestTypes;
            }
        }

        [TestCaseGenerator]
        public void PrimitiveOnTopTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this.PrimitiveOnTopTestTypes, MethodInfo.GetCurrentMethod());
        }

        public void PrimitiveOnTopTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this.PrimitiveOnTopTestTypes, instanceID));
        }

        #endregion

        //[TestCase(Owner = "Microsoft",
        //          Category = TestCategory.Functional, 
        //          TestType = TestType.NotComplete,
        //          Title = @"Time zone test")]
        //public void TimeZoneTest()
        //{
        //    throw new NotImplementedException("Test case method TimeZoneTest is not implemented.");
        //}

        [TestCase(Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated,
            Title = @"Negative test on primitive value through XAML file")]
        public void XamlNegatvePrimitiveValueTest()
        {
            var data = (BasicPrimitiveTypes) BasicPrimitiveTypes.GetTestCases()[0].Target;
            string xaml = XamlServices.Save(data);
            xaml = xaml.Replace(short.MinValue.ToString(), long.MinValue.ToString());

            // System.ComponentModel.BaseNumberConverter throws System.Exception, check that the inner 
            // exception is an OverflowException
            try
            {
                XamlServices.Parse(xaml);
            }
            catch (XamlObjectWriterException e)
            {
                if (e.InnerException == null || e.InnerException.InnerException == null)
                {
                    throw;
                }
                if (e.InnerException.InnerException.GetType() != typeof (OverflowException))
                {
                    throw;
                }
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void EscapeBraces()
        {
            BasicPrimitiveTypes basic = new BasicPrimitiveTypes()
                                            {
                                                StringPrimitive = "{Looking for apartment:)}"
                                            };

            string xaml = XamlTestDriver.RoundTripCompare(basic);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void UnknownProperty()
        {
            XT.Point x = new XT.Point(10, 20);
            string xx = XamlServices.Save(x);

            string xaml = @"<Point X=""10"" Y=""20"" U=""50"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.XamlTemplate;assembly=XamlClrTypes"" />";

            bool pass = false;
            try
            {
                object foo = XamlServices.Parse(xaml);
            }
            catch (XamlObjectWriterException)
            {
                pass = true;
            }
            if (!pass)
            {
                throw new TestCaseFailedException("expected invalid opeartion exception not thrown");
            }
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void ClassWithEmptyList()
        {
            ClassWithEmptyList obj = new ClassWithEmptyList()
                                         {
                                         };

            string x = XamlServices.Save(obj);

            string xaml = @"<ClassWithEmptyList xmlns=""clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes""
                            xmlns:p=""http://schemas.microsoft.com/netfx/2008/xaml/schema"" 
                            xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                              <ClassWithEmptyList.Bar>
                              </ClassWithEmptyList.Bar>
                            </ClassWithEmptyList>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
        }
    }
}
