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
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    public class Zoo
    {
        private Tiger _tiger;
        private Monkey _monkey;

        public Tiger Tiger
        {
            get
            {
                return _tiger;
            }
            set
            {
                _tiger = value;
            }
        }

        [TypeConverter(typeof (MonkeyConverter))]
        public Monkey Monkey
        {
            get
            {
                return _monkey;
            }
            set
            {
                _monkey = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // 
            Zoo instance1 = new Zoo();
            instance1.Tiger = new Tiger(string.Empty);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            Zoo instance2 = new Zoo();
            instance2.Monkey = new Monkey("TinyMonkey");
            instance2.Tiger = new Tiger("HahaTiger");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2, TestID = instanceIDPrefix + 2
                              });

            Zoo instance3 = new Zoo();
            instance3.Monkey = new Monkey("");
            instance3.Tiger = new Tiger("");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3, TestID = instanceIDPrefix + 3
                              });

            // WPF compatibility test: if type converter returns null, change it to String.Empty
            // Also see GetValueFromStringTypeConverter(...) in prodcut source code XamlWriteHelper
            // However, this fails XamlTreeComparer. Need to figure out a way to handle this case
            Zoo instance4 = new Zoo();
            instance4.Monkey = new Monkey(string.Empty);
            instance4.Tiger = new Tiger(string.Empty);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance4, TestID = instanceIDPrefix + 4
                              });

            return testCases;
        }

        #endregion
    }

    [TypeConverter(typeof (TigerConverter))]
    public struct Tiger
    {
        private readonly string _name;

        public Tiger(string name)
        {
            this._name = name;
        }

        public string NickName
        {
            get
            {
                return this._name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Tiger instance1 = new Tiger("");
            // [DISABLED] : Known Bug / Test Case Hang ?
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            
            Tiger instance2 = new Tiger(string.Empty);
            // [DISABLED] : Known Bug / Test Case Hang ?
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2, TestID = instanceIDPrefix + 2
                              });

            Tiger instance3 = new Tiger("Cutie");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3, TestID = instanceIDPrefix + 3
                              });

            return testCases;
        }

        #endregion
    }

    public class TigerConverter : TypeConverter
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
            if (value is string)
            {
                return new Tiger(value as string);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Tiger)
            {
                return ((Tiger) value).NickName;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }

    public class Monkey
    {
        private readonly string _name;

        public Monkey(string name)
        {
            this._name = name;
        }

        public string NickName
        {
            get
            {
                return this._name;
            }
        }
    }

    public class MonkeyConverter : TypeConverter
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
            if (value is string)
            {
                return new Monkey(value as string);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Monkey)
            {
                return ((Monkey) value).NickName;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }

    [TypeConverter(typeof (EmptyTypeConverter<EmptyClassWithTypeConverter>))]
    public class EmptyClassWithTypeConverter
    {
    }

    public class EmptyClass
    {
    }

    public class EmptyClassContainer
    {
        public EmptyClassWithTypeConverter ConverterOnType { get; set; }

        [TypeConverter(typeof (EmptyTypeConverter<EmptyClass>))]
        public EmptyClass ConverterOnProperty { get; set; }
    }

    public class EmptyTypeConverter<T> : TypeConverter
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
            if (value is string)
            {
                return default(T);
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is T)
            {
                return "This is an empty class";
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }

    [TypeConverter(typeof(TypeWithDirectiveAttributesTypeConverter))]
    [RuntimeNameProperty("RuntimeName")]
    [UidProperty("Uid")]
    [XmlLangProperty("XmlLang")]
    public class TypeWithDirectiveAttributes
    {
        public string Data { get; set; }

        public string RuntimeName { get; set; }

        public string Uid { get; set; }

        public string XmlLang { get; set; }
    }

    public class TypeWithDirectiveAttributesTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new TypeWithDirectiveAttributes { Data = (string)value };
            }
            else
            {
                throw new ArgumentException("In ConvertFrom.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TypeWithDirectiveAttributes)
            {
                return ((TypeWithDirectiveAttributes)value).Data;
            }
            else
            {
                throw new ArgumentException("In ConvertTo.");
            }
        }
    }
}
