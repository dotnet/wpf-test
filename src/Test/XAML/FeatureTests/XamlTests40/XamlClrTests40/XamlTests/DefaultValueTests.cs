// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Infrastructure.Test;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Xaml.Driver;
using Microsoft.Test.Xaml.Types.DefaultValues;

namespace Microsoft.Test.Xaml.XamlTests
{
    public class DefaultValueTests
    {
        public static Dictionary<string, string> namespaces = new Dictionary<string, string>()
                                                                  {
                                                                      {
                                                                          "x", "http://schemas.microsoft.com/winfx/2006/xaml"
                                                                          },
                                                                      {
                                                                          "x2", "http://schemas.microsoft.com/netfx/2008/xaml"
                                                                          },
                                                                      {
                                                                          "xx", "clr-namespace:Microsoft.Test.Xaml.Types.DefaultValues;assembly=XamlClrTypes"
                                                                          },
                                                                      {
                                                                          "p", "http://schemas.microsoft.com/netfx/2008/xaml/schema"
                                                                          }
                                                                  };

        private static void FailOnContains(string subject, string substr)
        {
            if (subject.ToLower(CultureInfo.InvariantCulture).Contains(substr.ToLower(CultureInfo.InvariantCulture)))
            {
                throw new TestCaseFailedException(subject + " contains " + substr + " which is not expected");
            }
        }

        [TestCase]
        public void MemberHasDefaultValue()
        {
            DefaultValueType obj = new DefaultValueType()
                                       {
                                           BooleanProperty = false,
                                           ByteProperty = (byte) 20,
                                           CharProperty = 'a',
                                           DoubleProperty = 100.5,
                                           IntProperty = 10,
                                           LongProperty = (long) 102.4,
                                           ShortProperty = 10,
                                           StringProperty = "Hello",
                                           ClassWithTypeConverter = new ClassWithTypeConverter()
                                                                        {
                                                                            foo = "MyDefaultValue"
                                                                        },
                                           MyObjectProperty = null,
                                       };

            string xaml = XamlTestDriver.Serialize(obj);

            FailOnContains(xaml, "BooleanProperty");
            FailOnContains(xaml, "ByteProperty");
            FailOnContains(xaml, "CharProperty");
            FailOnContains(xaml, "DoubleProperty");
            FailOnContains(xaml, "IntProperty");
            FailOnContains(xaml, "LongProperty");
            FailOnContains(xaml, "ShortProperty");
            FailOnContains(xaml, "StringProperty");
            FailOnContains(xaml, "ClassWithTypeConverter");
            FailOnContains(xaml, "MyObjectProperty");
        }

        [TestCase]
        public void MemberHasNonDefaultValue()
        {
            DefaultValueType obj = new DefaultValueType()
                                       {
                                           BooleanProperty = true,
                                           ByteProperty = (byte) 21,
                                           CharProperty = 'c',
                                           DoubleProperty = 101.5,
                                           IntProperty = 11,
                                           LongProperty = (long) 103.4,
                                           ShortProperty = 4,
                                           StringProperty = "Hello2",
                                           ClassWithTypeConverter = new ClassWithTypeConverter()
                                                                        {
                                                                            foo = "MyNonDefaultValue"
                                                                        },
                                           MyObjectProperty = new MyObject
                                                                  {
                                                                      foo = 5
                                                                  },
                                       };

            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void NonDefaultBoundary()
        {
            DefaultValueType obj = new DefaultValueType()
                                       {
                                           BooleanProperty = false,
                                           ByteProperty = byte.MaxValue,
                                           CharProperty = 'c',
                                           DoubleProperty = double.MaxValue,
                                           IntProperty = int.MaxValue,
                                           LongProperty = long.MaxValue,
                                           ShortProperty = short.MaxValue,
                                           StringProperty = "",
                                       };
            string xaml = XamlTestDriver.RoundTripCompare(obj);

            obj = new DefaultValueType()
                      {
                          BooleanProperty = false,
                          ByteProperty = byte.MinValue,
                          CharProperty = 'c',
                          DoubleProperty = double.MinValue,
                          IntProperty = int.MinValue,
                          LongProperty = long.MinValue,
                          ShortProperty = short.MinValue,
                          StringProperty = null,
                      };
            xaml = XamlTestDriver.RoundTripCompare(obj);

            obj = new DefaultValueType()
                      {
                          BooleanProperty = false,
                          ByteProperty = byte.MinValue,
                          CharProperty = 'c',
                          DoubleProperty = double.NaN,
                          IntProperty = int.MinValue,
                          LongProperty = long.MinValue,
                          ShortProperty = short.MinValue,
                          StringProperty = null,
                      };
            xaml = XamlTestDriver.RoundTripCompare(obj);

            obj = new DefaultValueType()
                      {
                          BooleanProperty = false,
                          ByteProperty = byte.MinValue,
                          CharProperty = 'c',
                          DoubleProperty = double.NegativeInfinity,
                          IntProperty = int.MinValue,
                          LongProperty = long.MinValue,
                          ShortProperty = short.MinValue,
                          StringProperty = null,
                      };
            xaml = XamlTestDriver.RoundTripCompare(obj);

            obj = new DefaultValueType()
                      {
                          BooleanProperty = false,
                          ByteProperty = byte.MinValue,
                          CharProperty = 'c',
                          DoubleProperty = double.PositiveInfinity,
                          IntProperty = int.MinValue,
                          LongProperty = long.MinValue,
                          ShortProperty = short.MinValue,
                          StringProperty = null,
                      };
            xaml = XamlTestDriver.RoundTripCompare(obj);

            obj = new DefaultValueType()
                      {
                          BooleanProperty = false,
                          ByteProperty = byte.MinValue,
                          CharProperty = 'c',
                          DoubleProperty = double.Epsilon,
                          IntProperty = int.MinValue,
                          LongProperty = long.MinValue,
                          ShortProperty = short.MinValue,
                          StringProperty = null,
                      };
            xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void BoundaryDefaultValueTests()
        {
            BoundaryDefaultValueType obj = new BoundaryDefaultValueType()
                                               {
                                                   BooleanPropertyTrue = true,
                                                   BytePropertyMax = byte.MaxValue,
                                                   BytePropertyMin = byte.MinValue,
                                                   DoublePropertyEps = double.Epsilon,
                                                   DoublePropertyMax = double.MaxValue,
                                                   DoublePropertyMin = double.MinValue,
                                                   DoublePropertyNan = double.NaN,
                                                   DoublePropertyNin = double.NegativeInfinity,
                                                   DoublePropertyPin = double.PositiveInfinity,
                                                   IntPropertyMax = int.MaxValue,
                                                   IntPropertyMin = int.MinValue,
                                                   LongPropertyMax = long.MaxValue,
                                                   LongPropertyMin = long.MinValue,
                                                   ShortPropertyMax = short.MaxValue,
                                                   ShortPropertyMin = short.MinValue,
                                                   StringPropertyEmpty = "",
                                                   StringPropertyNull = null,
                                               };

            string xaml = XamlTestDriver.Serialize(obj);
            FailOnContains(xaml, "Boolean");
            FailOnContains(xaml, "Byte");
            FailOnContains(xaml, "Double");
            FailOnContains(xaml, "Int");
            FailOnContains(xaml, "Short");
            FailOnContains(xaml, "String");
        }

        [TestCase]
        public void DefaultValueOnList()
        {
            var obj = new ClassWithDefaultList();

            string xaml = XamlTestDriver.Serialize(obj);
            if (xaml.Contains("Data"))
            {
                throw new TestCaseFailedException("Default value was set but property was serialized");
            }
        }

        [TestCase]
        public void DerivedDefaultValue()
        {
            var defaultValue = new DerivedDefaultValue
            {
                PropWithDefaultValue = "some value"
            };

            string xaml = XamlTestDriver.Serialize(defaultValue);
            FailOnContains(xaml, "PropWithDefaultValue");

            var nonDefaultValue = new DerivedDefaultValue
            {
                PropWithDefaultValue = "some other value"
            };

            XamlTestDriver.RoundTripCompare(nonDefaultValue);
        }

        [TestCase]
        public void DerivedSerializationVisibility()
        {
            var value = new DerivedDesignerSerializationVisibility
            {
                PropWithDesignerSerializationVisibility = "some value"
            };

            string xaml = XamlTestDriver.Serialize(value);
            FailOnContains(xaml, "PropWithDesignerSerializationVisibility");
        }
    }
}
