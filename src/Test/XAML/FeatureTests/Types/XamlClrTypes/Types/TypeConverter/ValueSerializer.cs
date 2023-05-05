// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;

    public class PrimitiveWrapper
    {
        public PrimitiveClass Data { get; set; }
    }

    [ValueSerializer(typeof (PrimitiveValueSerializer))]
    [TypeConverter(typeof (PrimitiveTypeConverter))]
    public class PrimitiveClass
    {
        private int _data;
        public int Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>()
                                               {
                                                   new TestCaseInfo
                                                       {
                                                           Target = new PrimitiveClass(),
                                                           TestID = instanceIDPrefix + 1,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:PrimitiveClass[.='int32#0']"
                                                               }
                                                       },
                                                   new TestCaseInfo
                                                       {
                                                           Target = new PrimitiveClass()
                                                                        {
                                                                            Data = int.MaxValue
                                                                        },
                                                           TestID = instanceIDPrefix + 2,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:PrimitiveClass[.='int32#" + int.MaxValue + "']"
                                                               }
                                                       },
                                                   new TestCaseInfo
                                                       {
                                                           Target = new PrimitiveClass()
                                                                        {
                                                                            Data = int.MinValue
                                                                        },
                                                           TestID = instanceIDPrefix + 3,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:PrimitiveClass[.='int32#" + int.MinValue + "']"
                                                               }
                                                       },
                                                   new TestCaseInfo
                                                       {
                                                           Target = new PrimitiveWrapper
                                                                        {
                                                                            Data = new PrimitiveClass
                                                                                       {
                                                                                           Data = int.MinValue
                                                                                       }
                                                                        },
                                                           TestID = instanceIDPrefix + 4,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:PrimitiveWrapper[@Data='int32#" + int.MinValue + "']"
                                                               }
                                                       }
                                               };

            return testCases;
        }

        #endregion
    }

    public class PrimitiveValueSerializer : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new DataTestException("Shouldn't ever call ConvertFromString in a ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            PrimitiveClass primitive = value as PrimitiveClass;
            return "int32#" + primitive.Data.ToString(CultureInfo.InvariantCulture);
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is PrimitiveClass)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class PrimitiveTypeConverter : TypeConverter
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
                string[] parts = ((string) value).Split('#');
                return new PrimitiveClass
                           {
                               Data = int.Parse(parts[1], CultureInfo.InvariantCulture)
                           };
            }
            else
            {
                throw new ArgumentException("In ConvertFrom: can not convert.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is PrimitiveClass)
            {
                PrimitiveClass myClass = value as PrimitiveClass;
                return "int32#" + myClass.Data.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,"In ConvertTo: can not convert type '{0}' to string.", value.GetType().AssemblyQualifiedName));
            }
        }
    }

    [ValueSerializer(typeof (PrimitiveClassBValueSerializer))]
    [TypeConverter(typeof (PrimitiveClassBTypeConverter))]
    public class PrimitiveClassB
    {
        private bool _boolPrimitive;
        private sbyte _sbytePrimitive;
        private short _shortPrimitive;
        private int _intPrimitive;
        private long _longPrimitive;
        private uint _uintPrimitive;
        private ushort _ushortPrimitive;
        private byte _bytePrimitve;
        private ulong _ulongPrimitve;
        private float _floatPrimitve;
        private double _doublePrimitve;
        private decimal _decimalPrimitve;
        private string _stringPrimitive;
        private DateTime _dateTimeValue;
        private char _charValue;
        private TimeSpan _timeSpanValue;
        private Guid _guidValue;

        public short ShortPrimitive
        {
            get
            {
                return _shortPrimitive;
            }
            set
            {
                _shortPrimitive = value;
            }
        }

        public int IntPrimitive
        {
            get
            {
                return _intPrimitive;
            }
            set
            {
                _intPrimitive = value;
            }
        }

        public long LongPrimitive
        {
            get
            {
                return _longPrimitive;
            }
            set
            {
                _longPrimitive = value;
            }
        }

        public uint UintPrimitive
        {
            get
            {
                return _uintPrimitive;
            }
            set
            {
                _uintPrimitive = value;
            }
        }

        public ushort UshortPrimitive
        {
            get
            {
                return _ushortPrimitive;
            }
            set
            {
                _ushortPrimitive = value;
            }
        }

        public byte BytePrimitve
        {
            get
            {
                return _bytePrimitve;
            }
            set
            {
                _bytePrimitve = value;
            }
        }

        public ulong UlongPrimitve
        {
            get
            {
                return _ulongPrimitve;
            }
            set
            {
                _ulongPrimitve = value;
            }
        }

        public float FloatPrimitve
        {
            get
            {
                return _floatPrimitve;
            }
            set
            {
                _floatPrimitve = value;
            }
        }

        public double DoublePrimitve
        {
            get
            {
                return _doublePrimitve;
            }
            set
            {
                _doublePrimitve = value;
            }
        }

        public decimal DecimalPrimitve
        {
            get
            {
                return _decimalPrimitve;
            }
            set
            {
                _decimalPrimitve = value;
            }
        }

        public bool BoolPrimitive
        {
            get
            {
                return this._boolPrimitive;
            }
            set
            {
                this._boolPrimitive = value;
            }
        }

        public sbyte SbytePrimitive
        {
            get
            {
                return this._sbytePrimitive;
            }
            set
            {
                this._sbytePrimitive = value;
            }
        }

        public string StringPrimitive
        {
            get
            {
                return _stringPrimitive;
            }
            set
            {
                _stringPrimitive = value;
            }
        }

        public char CharValue
        {
            get
            {
                return _charValue;
            }
            set
            {
                _charValue = value;
            }
        }

        public DateTime DateTimeValue
        {
            get
            {
                return _dateTimeValue;
            }
            set
            {
                _dateTimeValue = value;
            }
        }

        public Guid GuidValue
        {
            get
            {
                return _guidValue;
            }
            set
            {
                _guidValue = value;
            }
        }

        public TimeSpan TimeSpanValue
        {
            get
            {
                return _timeSpanValue;
            }
            set
            {
                _timeSpanValue = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            PrimitiveClassB classB = new PrimitiveClassB();
            classB.BoolPrimitive = true;
            classB.BytePrimitve = 1;
            classB.CharValue = 'a';
            classB.DateTimeValue = new DateTime(2008, 6, 6);
            classB.DecimalPrimitve = 1000;
            classB.DoublePrimitve = 13256.655;
            classB.FloatPrimitve = 1322;
            classB.GuidValue = new Guid();
            classB.IntPrimitive = 32;
            classB.LongPrimitive = 4758802;
            classB.SbytePrimitive = 0;
            classB.ShortPrimitive = 4;
            classB.StringPrimitive = "primitives";
            classB.TimeSpanValue = new TimeSpan(12, 34, 55);
            classB.UintPrimitive = 34;
            classB.UlongPrimitve = 5422;
            classB.UshortPrimitive = 6;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = classB,
                                  TestID = instanceIDPrefix + 1,
                                  XPathExpresions =
                                      {
                                          "/mtxt:PrimitiveClassB[.='" + "4#32#4758802#34#6#1#5422#1322#13256.655#1000#True#0#primitives#a#06/06/2008 00:00:00#00000000-0000-0000-0000-000000000000#12:34:55#" + "']"
                                      }
                              });

            return testCases;
        }

        #endregion
    }

    public class PrimitiveClassBValueSerializer : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            string[] properties = (value as string).Split(new char[]
                                                              {
                                                                  '#'
                                                              }, StringSplitOptions.RemoveEmptyEntries);
            PrimitiveClassB classB = new PrimitiveClassB();
            classB.ShortPrimitive = short.Parse(properties[0], CultureInfo.InvariantCulture);
            classB.IntPrimitive = int.Parse(properties[1], CultureInfo.InvariantCulture);
            classB.LongPrimitive = long.Parse(properties[2], CultureInfo.InvariantCulture);
            classB.UintPrimitive = uint.Parse(properties[3], CultureInfo.InvariantCulture);
            classB.UshortPrimitive = ushort.Parse(properties[4], CultureInfo.InvariantCulture);
            classB.BytePrimitve = byte.Parse(properties[5], CultureInfo.InvariantCulture);
            classB.UlongPrimitve = ulong.Parse(properties[6], CultureInfo.InvariantCulture);
            classB.FloatPrimitve = float.Parse(properties[7], CultureInfo.InvariantCulture);
            classB.DoublePrimitve = double.Parse(properties[8], CultureInfo.InvariantCulture);
            classB.DecimalPrimitve = decimal.Parse(properties[9], CultureInfo.InvariantCulture);
            classB.BoolPrimitive = bool.Parse(properties[10]);
            classB.SbytePrimitive = sbyte.Parse(properties[11], CultureInfo.InvariantCulture);
            classB.StringPrimitive = properties[12];
            classB.CharValue = char.Parse(properties[13]);
            classB.DateTimeValue = DateTime.Parse(properties[14], CultureInfo.InvariantCulture);
            classB.GuidValue = new Guid(properties[15]);
            classB.TimeSpanValue = TimeSpan.Parse(properties[16]);

            return classB;
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            PrimitiveClassB classB = value as PrimitiveClassB;
            StringBuilder builder = new StringBuilder();
            builder.Append(classB.ShortPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.IntPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.LongPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.UintPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.UshortPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.BytePrimitve.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.UlongPrimitve.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.FloatPrimitve.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.DoublePrimitve.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.DecimalPrimitve.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.BoolPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.SbytePrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.StringPrimitive.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.CharValue.ToString(CultureInfo.InvariantCulture));
            builder.Append("#");
            builder.Append(classB.DateTimeValue.ToString(CultureInfo.InvariantCulture.DateTimeFormat));
            builder.Append("#");
            builder.Append(classB.GuidValue.ToString());
            builder.Append("#");
            builder.Append(classB.TimeSpanValue.ToString());
            builder.Append("#");

            return builder.ToString();
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is PrimitiveClassB)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class PrimitiveClassBTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string[] properties = (value as string).Split(new char[]
                                                              {
                                                                  '#'
                                                              }, StringSplitOptions.RemoveEmptyEntries);
            PrimitiveClassB classB = new PrimitiveClassB();
            classB.ShortPrimitive = short.Parse(properties[0], CultureInfo.InvariantCulture);
            classB.IntPrimitive = int.Parse(properties[1], CultureInfo.InvariantCulture);
            classB.LongPrimitive = long.Parse(properties[2], CultureInfo.InvariantCulture);
            classB.UintPrimitive = uint.Parse(properties[3], CultureInfo.InvariantCulture);
            classB.UshortPrimitive = ushort.Parse(properties[4], CultureInfo.InvariantCulture);
            classB.BytePrimitve = byte.Parse(properties[5], CultureInfo.InvariantCulture);
            classB.UlongPrimitve = ulong.Parse(properties[6], CultureInfo.InvariantCulture);
            classB.FloatPrimitve = float.Parse(properties[7], CultureInfo.InvariantCulture);
            classB.DoublePrimitve = double.Parse(properties[8], CultureInfo.InvariantCulture);
            classB.DecimalPrimitve = decimal.Parse(properties[9], CultureInfo.InvariantCulture);
            classB.BoolPrimitive = bool.Parse(properties[10]);
            classB.SbytePrimitive = sbyte.Parse(properties[11], CultureInfo.InvariantCulture);
            classB.StringPrimitive = properties[12];
            classB.CharValue = char.Parse(properties[13]);
            classB.DateTimeValue = DateTime.Parse(properties[14], CultureInfo.InvariantCulture);
            classB.GuidValue = new Guid(properties[15]);
            classB.TimeSpanValue = TimeSpan.Parse(properties[16]);

            return classB;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("This ConvertTo should never be called.");
        }
    }

    [ValueSerializer(typeof (BaseClassValueSerializer))]
    [TypeConverter(typeof (BaseClassTypeConverter))]
    public abstract class BaseClass
    {
        private List<int> _marks;
        public List<int> Marks
        {
            get
            {
                return _marks;
            }
            set
            {
                _marks = value;
            }
        }
    }

    public class DerivedClassA : BaseClass
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            DerivedClassA classA = new DerivedClassA();
            List<int> marks = new List<int>
                                  {
                                      23, 45, 67, 899
                                  };
            classA.Marks = marks;

            List<TestCaseInfo> testCases = new List<TestCaseInfo>()
                                               {
                                                   new TestCaseInfo
                                                       {
                                                           Target = classA,
                                                           TestID = instanceIDPrefix + 1,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:DerivedClassA[.='" + "23#45#67#899#" + "']"
                                                               }
                                                       },
                                                   new TestCaseInfo
                                                       {
                                                           Target = new DerivedClassB()
                                                                        {
                                                                            Marks = new List<int>
                                                                                        {
                                                                                            1,
                                                                                            2,
                                                                                            3
                                                                                        }
                                                                        },
                                                           TestID = instanceIDPrefix + 2,
                                                           XPathExpresions =
                                                               {
                                                                   "/mtxt:DerivedClassB/mtxt:DerivedClassB.Marks"
                                                               }
                                                       },
                                               };

            return testCases;
        }

        #endregion
    }

    public class DerivedClassB : BaseClass
    {
        private Dictionary<int, string> _record;
        public Dictionary<int, string> Record
        {
            get
            {
                return _record;
            }
            set
            {
                _record = value;
            }
        }
    }

    public class BaseClassValueSerializer : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            string[] marks = (value as string).Split(new char[]
                                                         {
                                                             '#'
                                                         }, StringSplitOptions.RemoveEmptyEntries);
            List<int> actualMarks = new List<int>();
            for (int i = 0; i < marks.Length; i++)
            {
                actualMarks.Add(Int32.Parse(marks[i]));
            }
            DerivedClassA classA = new DerivedClassA();
            classA.Marks = actualMarks;
            return classA;
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            DerivedClassA classA = value as DerivedClassA;
            StringBuilder builder = new StringBuilder();
            foreach (int mark in classA.Marks)
            {
                builder.Append(mark.ToString());
                builder.Append("#");
            }
            return builder.ToString();
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is DerivedClassA)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class BaseClassTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string[] marks = (value as string).Split(new char[]
                                                         {
                                                             '#'
                                                         }, StringSplitOptions.RemoveEmptyEntries);
            List<int> actualMarks = new List<int>();
            for (int i = 0; i < marks.Length; i++)
            {
                actualMarks.Add(Int32.Parse(marks[i]));
            }
            DerivedClassA classA = new DerivedClassA();
            classA.Marks = actualMarks;
            return classA;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            DerivedClassA classA = value as DerivedClassA;
            StringBuilder builder = new StringBuilder();
            foreach (int mark in classA.Marks)
            {
                builder.Append(mark.ToString());
                builder.Append("#");
            }
            return builder.ToString();
        }
    }

    [ValueSerializer(typeof (IntListValueSerializer))]
    [TypeConverter(typeof (IntListTypeConverter))]
    public class IntListWithValueSerializer : List<int>
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new IntListWithValueSerializer
                                                {
                                                    1,
                                                    2,
                                                    0,
                                                    int.MaxValue,
                                                    int.MinValue
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:IntListWithValueSerializer[.='" + "1#2#0#2147483647#-2147483648" + "']"
                                       }
                               }
                       };
        }
    }

    public class IntListValueSerializer : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFromString shouldn't be called on a ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            StringBuilder sb = new StringBuilder();
            IntListWithValueSerializer list = (IntListWithValueSerializer) value;
            foreach (int i in list)
            {
                sb.Append(i);
                sb.Append("#");
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return value is IntListWithValueSerializer;
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class IntListTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var ints = new IntListWithValueSerializer();
            string[] strings = ((string) value).Split('#');
            foreach (string s in strings)
            {
                ints.Add(int.Parse(s));
            }

            return ints;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("This ConvertTo should never be called.");
        }
    }

    [ValueSerializer("")]
    public class ClassWithNoSerializer
    {
        private List<byte> _attendance;
        public List<byte> Attendance
        {
            get
            {
                return _attendance;
            }
            set
            {
                _attendance = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            string message = Exceptions.GetMessage("UnexpectedConstructorArg", WpfBinaries.SystemXaml);
            message = string.Format(message, "System.Windows.Markup.ValueSerializerAttribute",
                "Microsoft.Test.Xaml.Types.ClassWithNoSerializer",
                "1",
                "System.Type");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = new ClassWithNoSerializer(), TestID = instanceIDPrefix + 1,
                                  ExpectedResult = false,
                                  ExpectedMessage = message,
                              });

            return testCases;
        }

        #endregion
    }

    [ValueSerializer(typeof (ValueSerializerClassA))]
    [TypeConverter(typeof (TypeConverterClassA))]
    public class ClassWithValSerializerA
    {
        private bool _classAProperty;

        public bool ClassAProperty
        {
            get
            {
                return _classAProperty;
            }
            set
            {
                _classAProperty = value;
            }
        }
    }

    [ValueSerializer(typeof (ValueSerializerClassB))]
    [TypeConverter(typeof (TypeConverterClassB))]
    public class ClassWithValSerializerB
    {
        private ClassWithValSerializerA _subProperty;

        public ClassWithValSerializerA SubProperty
        {
            get
            {
                return _subProperty;
            }
            set
            {
                _subProperty = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassWithValSerializerB ValSerClassB = new ClassWithValSerializerB();
            ClassWithValSerializerA ValSerClassA = new ClassWithValSerializerA();
            ValSerClassA.ClassAProperty = true;
            ValSerClassB.SubProperty = ValSerClassA;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = ValSerClassB,
                                  TestID = instanceIDPrefix + 1,
                                  XPathExpresions =
                                      {
                                          "/mtxt:ClassWithValSerializerB[.='True*']"
                                      }
                              });

            return testCases;
        }

        #endregion
    }

    public class ValueSerializerClassA : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFrom shouldn't be called for a ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("This ValueSerializer should never be called.");
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is ClassWithValSerializerA)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class TypeConverterClassA : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("Should never call ConvertTo on TypeConverter");
        }
    }

    public class ValueSerializerClassB : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFrom shouldn't be called for a ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            ClassWithValSerializerB myClass = value as ClassWithValSerializerB;
            return myClass.SubProperty.ClassAProperty.ToString() + "*";
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is ClassWithValSerializerB)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class TypeConverterClassB : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = (string) value;
            ClassWithValSerializerB myClass = new ClassWithValSerializerB();
            myClass.SubProperty = new ClassWithValSerializerA();
            str = str.Substring(0, str.Length - 1);
            myClass.SubProperty.ClassAProperty = bool.Parse(str);

            return myClass;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("Should never call ConvertTo on TypeConverter");
        }
    }

    [ValueSerializer(typeof (ValueSerializerForString))]
    public class ClassWithStringProperty
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }

    public class ValueSerializerForString : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            //Should this method get called?
            ClassWithStringProperty myClass = new ClassWithStringProperty();
            myClass.Name = value;
            return myClass;
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            ClassWithStringProperty myClass = value as ClassWithStringProperty;
            return myClass.Name;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is ClassWithStringProperty)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class GenericValueSerializer<T> : ValueSerializer
    {
        private T _dataType;
        public T Data
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            ClassWithGenericSerializerA myClass = new ClassWithGenericSerializerA();
            myClass.Data = int.Parse(value);
            return myClass;
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            ClassWithGenericSerializerA myClass = value as ClassWithGenericSerializerA;
            return myClass.Data.ToString();
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value.GetType().Equals(typeof (ClassWithGenericSerializerA)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    [ValueSerializer(typeof (GenericValueSerializer<int>))]
    public class ClassWithGenericSerializerA
    {
        private int _data;
        public int Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    [ValueSerializer(typeof (GenericValueSerializer<float>))]
    public class ClassWithGenericSerializerB
    {
        private float _data;
        public float Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    [ValueSerializer(typeof (ValueSerializerThrowsException))]
    [TypeConverter(typeof (TypeConverterClassA))]
    public class ClassReceivingException
    {
        private float _data;
        public float Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            string message = Exceptions.GetMessage("TypeConverterFailed2", WpfBinaries.SystemXaml);
            message = string.Format(message, "Microsoft.Test.Xaml.Types.ClassReceivingException", "System.String");

            testCases.Add(new TestCaseInfo
                              {
                                  Target = new ClassReceivingException(),
                                  TestID = instanceIDPrefix + 1,
                                  ExpectedResult = false,
                                  ExpectedMessage = message,
                              });

            return testCases;
        }

        #endregion
    }

    public class ValueSerializerThrowsException : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new Exception("Exception thrown from ConvertFromString");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            throw new Exception("Exception thrown from ConvertToString");
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    [ValueSerializer(typeof (ValueSerializerReturnsNull))]
    public class ClassReceivingNull
    {
        private float _data;
        public float Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }

    public class ValueSerializerReturnsNull : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            return null;
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return null;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }

    public class CustomValueSerializerContext : IValueSerializerContext
    {
        #region IValueSerializerContext Members

        public ValueSerializer GetValueSerializerFor(PropertyDescriptor descriptor)
        {
            return new PrimitiveValueSerializer();
        }

        public ValueSerializer GetValueSerializerFor(Type type)
        {
            return new PrimitiveValueSerializer();
        }

        #endregion

        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object Instance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IValueSerializerContext Members

        ValueSerializer IValueSerializerContext.GetValueSerializerFor(PropertyDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        ValueSerializer IValueSerializerContext.GetValueSerializerFor(Type type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ITypeDescriptorContext Members

        IContainer ITypeDescriptorContext.Container
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ITypeDescriptorContext.Instance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void ITypeDescriptorContext.OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //public class PointContainer
    //{
    //    public PointWithXamlTemplate Point
    //    {
    //        get;
    //        set;
    //    }
    //}

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point()
        {
        }
    }

    //[XamlTemplate(typeof(PointXamlTemplateTypeConverterGood), typeof(Point))]
    //[ValueSerializer(typeof(AlwaysThrowValueSerializer))]
    //[TypeConverter(typeof(AlwaysThrowTypeConverter))]
    //public class PointWithXamlTemplate : Point
    //{
    //    protected IEnumerable<XamlNode> nodes;
    //    protected IXamlLoaderFactory loader;

    //    public PointWithXamlTemplate()
    //    {
    //    }

    //    public PointWithXamlTemplate(IXamlLoaderFactory loader,
    //        System.Runtime.Xaml.XamlReader reader)
    //    {
    //        this.loader = loader;
    //        this.nodes = reader == null ? null : reader.ReadToEnd().ToList();
    //    }

    //    public PointWithXamlTemplate(IEnumerable<XamlNode> nodes)
    //    {
    //        this.nodes = nodes;
    //    }

    //    public IEnumerable<XamlNode> Nodes
    //    {
    //        get
    //        {
    //            return nodes;
    //        }
    //    }

    //    public virtual PointWithXamlTemplate Evaluate()
    //    {
    //        return Evaluate(null);
    //    }

    //    public virtual PointWithXamlTemplate Evaluate(object context)
    //    {
    //        return (PointWithXamlTemplate)loader.CreateLoader(context).Load(new System.Runtime.Xaml.XamlReader(nodes)).Instance;
    //    }

    //    #region Test Implementation

    //    public static List<TestCaseInfo> GetTestCases()
    //    {
    //        string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
    //        List<TestCaseInfo> testCases = new List<TestCaseInfo>();

    //        PointWithXamlTemplate ClassWithTemplate = new PointWithXamlTemplate();
    //        ClassWithTemplate.X = 1;
    //        ClassWithTemplate.Y = 2;
    //        testCases.Add(new TestCaseInfo { Target = ClassWithTemplate, TestID = instanceIDPrefix + 1 });

    //        return testCases;
    //    }
    //    #endregion
    //}

    public class AlwaysThrowValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("This ValueSerializer should never be called.");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("This ValueSerializer should never be called.");
        }
    }

    public class AlwaysThrowTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }
    }

    public class ReadOnlyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }
    }

    //public class PointXamlTemplateTypeConverterGood : TypeConverter
    //{
    //    public override bool CanConvertFrom(ITypeDescriptorContext context,
    //        Type sourceType)
    //    {
    //        return sourceType == typeof(System.Runtime.Xaml.XamlReader);
    //    }

    //    public override bool CanConvertTo(ITypeDescriptorContext context,
    //        Type destinationType)
    //    {
    //        return destinationType == typeof(System.Runtime.Xaml.XamlReader);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context,
    //        System.Globalization.CultureInfo culture,
    //        object value)
    //    {
    //        Type type = context.GetService(typeof(Type)) as Type;

    //        var loaderFactory = (IXamlLoaderFactory)context.GetService(typeof(IXamlLoaderFactory));

    //        return new PointWithXamlTemplate(loaderFactory, (System.Runtime.Xaml.XamlReader)value);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context,
    //        System.Globalization.CultureInfo culture,
    //        object value,
    //        Type destinationType)
    //    {
    //        PropertyInfo getNodes = value.GetType().GetProperty("Nodes");
    //        return new System.Runtime.Xaml.XamlReader(((PointWithXamlTemplate)value).Nodes);
    //    }
    //}
    [ContentProperty("Data")]
    [ValueSerializer(typeof (ContentTypeValueSerializer))]
    [TypeConverter(typeof (ContentTypeTypeConverter))]
    public class ContentTypeClass
    {
        public int Data { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ContentTypeClass ContentClass = new ContentTypeClass();
            ContentClass.Data = 14329;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = ContentClass,
                                  TestID = instanceIDPrefix + 1,
                                  XPathExpresions =
                                      {
                                          "/mtxt:ContentTypeClass[.='14329*']"
                                      }
                              });

            return testCases;
        }

        #endregion
    }

    public class ContentTypeValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            ContentTypeClass myClass = value as ContentTypeClass;
            if (myClass == null)
            {
                throw new ArgumentException("Type of value must be MyPoint", "value");
            }
            else
            {
                return true;
            }
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFromString shouldn't be called on a ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value == null)
            {
                return null;
            }

            ContentTypeClass myClass = (ContentTypeClass) value;

            return myClass.Data.ToString(CultureInfo.InvariantCulture) + "*";
        }
    }

    public class ContentTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            string str = (string) value;
            ContentTypeClass myClass = new ContentTypeClass();
            myClass.Data = int.Parse(str.Substring(0, str.Length - 1), CultureInfo.InvariantCulture);
            return myClass;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("Should never call ConvertTo on TypeConverter");
        }
    }

    [ValueSerializer(typeof (ValueSerializerForManager))]
    [TypeConverter(typeof (TypeConverterForManager))]
    public class ManagerWithValueSerializer
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

            ManagerWithValueSerializer manager = new ManagerWithValueSerializer();
            Address address = new Address("Bel-Red Road");
            manager.Address = address;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = manager,
                                  TestID = instanceIDPrefix + 1,
                                  XPathExpresions =
                                      {
                                          "/mtxt:ManagerWithValueSerializer[.='Bel-Red Road']"
                                      }
                              });

            return testCases;
        }

        #endregion
    }

    public class ValueSerializerForManager : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            ManagerWithValueSerializer myClass = value as ManagerWithValueSerializer;
            if (myClass == null)
            {
                throw new ArgumentException("Type of value must be ManagerWithValueSerializer", "value");
            }
            else
            {
                return true;
            }
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFromString shouldn't be called for ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value == null)
            {
                return null;
            }

            ManagerWithValueSerializer myClass = (ManagerWithValueSerializer) value;

            return myClass.Address.Street;
        }
    }

    public class TypeConverterForManager : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            ManagerWithValueSerializer myClass = new ManagerWithValueSerializer();
            myClass.Address = new Address((string) value);
            return myClass;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("Should never call ConvertTo on TypeConverter");
        }
    }

    public class AddressTypeExtensionForValueSerializer : MarkupExtension
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

    public class AddressTypeConverterForValueSerializer : TypeConverter
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

    public interface IDataHolder
    {
        List<int> Data { get; set; }
    }

    public class ValueSerializerOnPropTypeConverter<T> : TypeConverter where T : IDataHolder, new()
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            T retVal = new T();
            if (((string) value) == "null")
            {
                retVal.Data = null;
                return retVal;
            }

            retVal.Data = new List<int>();
            string[] strings = ((string) value).Split('#', '$');
            foreach (string s in strings)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    retVal.Data.Add(int.Parse(s, CultureInfo.InvariantCulture));
                }
            }

            return retVal;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            StringBuilder sb = new StringBuilder();
            IDataHolder list = (IDataHolder) value;
            if (list.Data == null)
            {
                return "nullTC";
            }
            foreach (int i in list.Data)
            {
                sb.Append(i);
                sb.Append("$");
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                return "empty";
            }

            return sb.ToString();
        }
    }

    public class ValueSerializerOnProp : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new InvalidOperationException("ConvertFromString shouldn't be called for ValueSerializer");
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            StringBuilder sb = new StringBuilder();
            IDataHolder list = (IDataHolder) value;
            if (list.Data == null)
            {
                return "null";
            }
            foreach (int i in list.Data)
            {
                sb.Append(i);
                sb.Append("#");
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }

    [TypeConverter(typeof (ValueSerializerOnPropTypeConverter<TCOnTypeVSOnPropPropType>))]
    public class TCOnTypeVSOnPropPropType : IDataHolder
    {
        public List<int> Data { get; set; }
    }

    public class TCOnPropVSOnPropPropType : IDataHolder
    {
        public List<int> Data { get; set; }
    }

    [TypeConverter(typeof (AlwaysThrowTypeConverter))]
    [ValueSerializer(typeof (AlwaysThrowValueSerializer))]
    public class TCVSOnBothPropType : IDataHolder
    {
        public List<int> Data { get; set; }
    }

    [ValueSerializer(typeof (ValueSerializerOnProp))]
    public class TCOnPropVSOnTypePropType : IDataHolder
    {
        public List<int> Data { get; set; }
    }

    public class TCOnTypeVSOnPropContainer
    {
        [ValueSerializer(typeof (ValueSerializerOnProp))]
        public TCOnTypeVSOnPropPropType Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new TCOnTypeVSOnPropContainer
                                                {
                                                    Prop = new TCOnTypeVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnTypeVSOnPropContainer[@Prop='']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnTypeVSOnPropContainer
                                                {
                                                    Prop = new TCOnTypeVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                                              {
                                                                                  1, 2, 3, int.MaxValue, 0, int.MinValue
                                                                              }
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnTypeVSOnPropContainer[@Prop='1#2#3#2147483647#0#-2147483648']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnTypeVSOnPropContainer
                                                {
                                                    Prop = new TCOnTypeVSOnPropPropType
                                                               {
                                                                   Data = null
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 3,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnTypeVSOnPropContainer[@Prop='null']"
                                       },
                               },
                       };
        }
    }

    public class TCOnPropVSOnPropContainer
    {
        [ValueSerializer(typeof (ValueSerializerOnProp))]
        [TypeConverter(typeof (ValueSerializerOnPropTypeConverter<TCOnPropVSOnPropPropType>))]
        public TCOnPropVSOnPropPropType Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnPropContainer[@Prop='']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                                              {
                                                                                  1, 2, 3, int.MaxValue, 0, int.MinValue
                                                                              }
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnPropContainer[@Prop='1#2#3#2147483647#0#-2147483648']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = null
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 3,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnPropContainer[@Prop='null']"
                                       },
                               },
                       };
        }
    }

    public class TCVSOnBothContainer
    {
        [ValueSerializer(typeof (ValueSerializerOnProp))]
        [TypeConverter(typeof (ValueSerializerOnPropTypeConverter<TCVSOnBothPropType>))]
        public TCVSOnBothPropType Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new TCVSOnBothContainer
                                                {
                                                    Prop = new TCVSOnBothPropType
                                                               {
                                                                   Data = new List<int>()
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCVSOnBothContainer[@Prop='']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCVSOnBothContainer
                                                {
                                                    Prop = new TCVSOnBothPropType
                                                               {
                                                                   Data = new List<int>()
                                                                              {
                                                                                  1, 2, 3, int.MaxValue, 0, int.MinValue
                                                                              }
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCVSOnBothContainer[@Prop='1#2#3#2147483647#0#-2147483648']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCVSOnBothContainer
                                                {
                                                    Prop = new TCVSOnBothPropType
                                                               {
                                                                   Data = null
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 3,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCVSOnBothContainer[@Prop='null']"
                                       },
                               },
                       };
        }
    }

    public class NoTCVSOnPropContainer
    {
        [ValueSerializer(typeof (ValueSerializerOnProp))]
        public TCOnPropVSOnPropPropType Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new NoTCVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:NoTCVSOnPropContainer/mtxt:NoTCVSOnPropContainer.Prop"
                                       }
                               },
                           new TestCaseInfo
                               {
                                   Target = new NoTCVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = new List<int>()
                                                                              {
                                                                                  1, 2, 3, int.MaxValue, 0, int.MinValue
                                                                              }
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   XPathExpresions =
                                       {
                                           "/mtxt:NoTCVSOnPropContainer/mtxt:NoTCVSOnPropContainer.Prop"
                                       }
                               },
                           new TestCaseInfo
                               {
                                   Target = new NoTCVSOnPropContainer
                                                {
                                                    Prop = new TCOnPropVSOnPropPropType
                                                               {
                                                                   Data = null
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 3,
                                   XPathExpresions =
                                       {
                                           "/mtxt:NoTCVSOnPropContainer/mtxt:NoTCVSOnPropContainer.Prop"
                                       }
                               },
                       };
        }
    }

    public class TCOnPropVSOnTypeContainer
    {
        [TypeConverter(typeof (ValueSerializerOnPropTypeConverter<TCOnPropVSOnTypePropType>))]
        public TCOnPropVSOnTypePropType Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnTypeContainer
                                                {
                                                    Prop = new TCOnPropVSOnTypePropType
                                                               {
                                                                   Data = new List<int>()
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnTypeContainer[@Prop='']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnTypeContainer
                                                {
                                                    Prop = new TCOnPropVSOnTypePropType
                                                               {
                                                                   Data = new List<int>()
                                                                              {
                                                                                  1, 2, 3, int.MaxValue, 0, int.MinValue
                                                                              }
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnTypeContainer[@Prop='1#2#3#2147483647#0#-2147483648']"
                                       },
                               },
                           new TestCaseInfo
                               {
                                   Target = new TCOnPropVSOnTypeContainer
                                                {
                                                    Prop = new TCOnPropVSOnTypePropType
                                                               {
                                                                   Data = null
                                                               }
                                                },
                                   TestID = instanceIDPrefix + 3,
                                   XPathExpresions =
                                       {
                                           "/mtxt:TCOnPropVSOnTypeContainer[@Prop='null']"
                                       },
                               },
                       };
        }
    }

    public class VSOnStringPropertyContainer
    {
        [ValueSerializer(typeof (AlwaysThrowValueSerializer))]
        [TypeConverter(typeof (ReadOnlyTypeConverter))]
        public string Prop { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new VSOnStringPropertyContainer
                                                {
                                                    Prop = null
                                                },
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new VSOnStringPropertyContainer
                                                {
                                                    Prop = string.Empty
                                                },
                                   TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = new VSOnStringPropertyContainer
                                                {
                                                    Prop = "some random string"
                                                },
                                   TestID = instanceIDPrefix + 3
                               },
                       };
        }
    }
}
