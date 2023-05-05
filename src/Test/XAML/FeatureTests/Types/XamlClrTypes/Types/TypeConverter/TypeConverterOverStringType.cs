// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    public struct Frog
    {
        private string _name;

        [TypeConverter(typeof (FrogConverter))]
        public string NickName
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // 
            Frog instance1 = new Frog();
            instance1.NickName = "cutie";
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1 + "bug15016"
                              });

            return testCases;
        }

        #endregion
    }

    public class FrogConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new DataTestException("In ConverTo: this type converter should not be called.");
        }
    }
}
