// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    public class Address
    {
        private readonly string _street;

        public Address(string st)
        {
            this._street = st;
        }

        public string Street
        {
            get
            {
                return _street;
            }
        }
    }

    public class Manager
    {
        private Address _address;

        [TypeConverter(typeof (AddressTypeConverter))]
        public Address Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            //Address not initialized
            Manager instance1 = new Manager();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            Manager instance2 = new Manager();
            instance2.Address = new Address("One Microsoft Way");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2, TestID = instanceIDPrefix + 2
                              });

            Manager instance3 = new Manager();
            instance3.Address = new Address("    ");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3, TestID = instanceIDPrefix + 3
                              });

            Manager instance4 = new Manager();
            instance4.Address = new Address(null);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance4, TestID = instanceIDPrefix + 4
                              });

            return testCases;
        }

        #endregion
    }

    public class AddressTypeExtension : MarkupExtension
    {
        private string _street;

        public string Street
        {
            get
            {
                return _street;
            }
            set
            {
                _street = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Address(this._street);
        }
    }

    public class AddressTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (MarkupExtension);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is AddressTypeExtension)
            {
                return new Address((value as AddressTypeExtension).Street);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Address)
            {
                AddressTypeExtension address = new AddressTypeExtension();
                address.Street = (value as Address).Street;
                return address;
            }
            else
            {
                throw new ArgumentException("In ConvertTo");
            }
        }
    }
}
