// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using System.Globalization;
    using Microsoft.Test.Xaml.Driver;

    //Boolean, Int8, Int16, Int32, Int64, Uint8, UInt16, UInt32, UInt64, 
    // Single, Double, Decimal, DateTime, Char, String
    public class BasicPrimitiveTypes
    {
        bool _boolPrimitive;
        sbyte _sbytePrimitive;
        short _shortPrimitive;
        int _intPrimitive;
        long _longPrimitive;
        uint _uintPrimitive;
        ushort _ushortPrimitive;
        byte _bytePrimitve;
        ulong _ulongPrimitve;
        float _floatPrimitve;
        double _doublePrimitve;
        decimal _decimalPrimitve;
        string _stringPrimitive;
        DateTime _dateTimeValue;
        char _charValue;
        TimeSpan _timeSpanValue;
        Guid _guidValue;

        public short ShortPrimitive
        {
            get { return _shortPrimitive; }
            set { _shortPrimitive = value; }
        }

        public int IntPrimitive
        {
            get { return _intPrimitive; }
            set { _intPrimitive = value; }
        }

        public long LongPrimitive
        {
            get { return _longPrimitive; }
            set { _longPrimitive = value; }
        }

        public uint UintPrimitive
        {
            get { return _uintPrimitive; }
            set { _uintPrimitive = value; }
        }

        public ushort UshortPrimitive
        {
            get { return _ushortPrimitive; }
            set { _ushortPrimitive = value; }
        }

        public byte BytePrimitve
        {
            get { return _bytePrimitve; }
            set { _bytePrimitve = value; }
        }

        public ulong UlongPrimitve
        {
            get { return _ulongPrimitve; }
            set { _ulongPrimitve = value; }
        }

        public float FloatPrimitve
        {
            get { return _floatPrimitve; }
            set { _floatPrimitve = value; }
        }

        public double DoublePrimitve
        {
            get { return _doublePrimitve; }
            set { _doublePrimitve = value; }
        }

        public decimal DecimalPrimitve
        {
            get { return _decimalPrimitve; }
            set { _decimalPrimitve = value; }
        }

        public bool BoolPrimitive
        {
            get { return this._boolPrimitive; }
            set { this._boolPrimitive = value; }
        }

        public sbyte SbytePrimitive
        {
            get { return this._sbytePrimitive; }
            set { this._sbytePrimitive = value; }
        }

        public string StringPrimitive
        {
            get { return _stringPrimitive; }
            set { _stringPrimitive = value; }
        }

        public char CharValue
        {
            get { return _charValue; }
            set { _charValue = value; }
        }

        public DateTime DateTimeValue
        {
            get { return _dateTimeValue; }
            set { _dateTimeValue = value; }
        }


        public Guid GuidValue
        {
            get { return _guidValue; }
            set { _guidValue = value; }
        }

        public TimeSpan TimeSpanValue
        {
            get { return _timeSpanValue; }
            set { _timeSpanValue = value; }
        }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            BasicPrimitiveTypes instance1 = new BasicPrimitiveTypes();
            instance1._boolPrimitive = false;
            instance1._bytePrimitve = byte.MinValue;
            instance1._decimalPrimitve = decimal.MinValue;
            instance1._doublePrimitve = double.MinValue;
            instance1._floatPrimitve = float.MinValue;
            instance1._intPrimitive = int.MinValue;
            instance1._longPrimitive = long.MinValue;
            instance1._sbytePrimitive = sbyte.MinValue;
            instance1._shortPrimitive = short.MinValue;
            instance1._uintPrimitive = uint.MinValue;
            instance1._ulongPrimitve = ulong.MinValue;
            instance1._ushortPrimitive = ushort.MinValue;
            instance1._stringPrimitive = "  ";
            instance1._charValue = 'a';
            instance1._dateTimeValue = DateTime.MinValue; // 
            instance1._timeSpanValue = TimeSpan.MinValue;
            instance1._guidValue = Guid.Empty;
            testCases.Add(new TestCaseInfo { Target = instance1, TestID = instanceIDPrefix + 1 });

            BasicPrimitiveTypes instance2 = new BasicPrimitiveTypes();
            instance2._boolPrimitive = true;
            instance2._bytePrimitve = byte.MaxValue;
            instance2._decimalPrimitve = decimal.MaxValue;
            instance2._doublePrimitve = double.MaxValue;
            instance2._floatPrimitve = float.MaxValue;
            instance2._intPrimitive = int.MaxValue;
            instance2._longPrimitive = long.MaxValue;
            instance2._sbytePrimitive = sbyte.MaxValue;
            instance2._shortPrimitive = short.MaxValue;
            instance2._uintPrimitive = uint.MaxValue;
            instance2._ulongPrimitve = ulong.MaxValue;
            instance2._ushortPrimitive = ushort.MaxValue;
            instance2._stringPrimitive = "";
            instance2._charValue = '\0';
            instance2._dateTimeValue = CultureInfo.InvariantCulture.DateTimeFormat.Calendar.MaxSupportedDateTime;
            instance2.TimeSpanValue = TimeSpan.MaxValue;
            testCases.Add(new TestCaseInfo { Target = instance2, TestID = instanceIDPrefix + 2 });

            return testCases;
        }
        #endregion
    }

    public class PrimitivesOnTopWrapper
    {

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            bool boolPrimitive = true;
            sbyte sbytePrimitive = sbyte.MinValue;
            short shortPrimitive = short.MinValue;
            int intPrimitive = int.MaxValue;
            long longPrimitive = long.MaxValue;
            uint uintPrimitive = uint.MinValue;
            ushort ushortPrimitive = ushort.MinValue;
            byte bytePrimitve = byte.MaxValue;
            ulong ulongPrimitve = ulong.MinValue;
            float floatPrimitve = float.MinValue;
            double doublePrimitve = double.NaN;
            decimal decimalPrimitve = decimal.Zero;
            string stringPrimitive = new string("OnTop".ToCharArray());
            DateTime dateTimeValue = DateTime.MinValue;
            char charValue = 'a';
            TimeSpan timeSpanValue = TimeSpan.MinValue;
            Guid guidValue = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");

            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            testCases.Add(new TestCaseInfo { Target = boolPrimitive, TestID = instanceIDPrefix + 1 });
            testCases.Add(new TestCaseInfo { Target = sbytePrimitive, TestID = instanceIDPrefix + 2 }); //
            testCases.Add(new TestCaseInfo { Target = shortPrimitive, TestID = instanceIDPrefix + 3 }); //
            testCases.Add(new TestCaseInfo { Target = intPrimitive, TestID = instanceIDPrefix + 4 });
            testCases.Add(new TestCaseInfo { Target = longPrimitive, TestID = instanceIDPrefix + 5 });
            testCases.Add(new TestCaseInfo { Target = uintPrimitive, TestID = instanceIDPrefix + 6 }); //
            testCases.Add(new TestCaseInfo { Target = ushortPrimitive, TestID = instanceIDPrefix + 7 });
            testCases.Add(new TestCaseInfo { Target = bytePrimitve, TestID = instanceIDPrefix + 8 });
            testCases.Add(new TestCaseInfo { Target = ulongPrimitve, TestID = instanceIDPrefix + 9 });
            testCases.Add(new TestCaseInfo { Target = floatPrimitve, TestID = instanceIDPrefix + 10 });
            testCases.Add(new TestCaseInfo { Target = doublePrimitve, TestID = instanceIDPrefix + 11 });
            testCases.Add(new TestCaseInfo { Target = decimalPrimitve, TestID = instanceIDPrefix + 12 });
            testCases.Add(new TestCaseInfo { Target = stringPrimitive, TestID = instanceIDPrefix + 13 });
            testCases.Add(new TestCaseInfo { Target = dateTimeValue, TestID = instanceIDPrefix + 14 });
            testCases.Add(new TestCaseInfo { Target = charValue, TestID = instanceIDPrefix + 15 + "bug11682" }); //
            testCases.Add(new TestCaseInfo { Target = timeSpanValue, TestID = instanceIDPrefix + 16 });
            testCases.Add(new TestCaseInfo { Target = guidValue, TestID = instanceIDPrefix + 17 });
            testCases.Add(new TestCaseInfo { Target = string.Empty, TestID = instanceIDPrefix + 18 });
            testCases.Add(new TestCaseInfo { Target = "{0}", TestID = instanceIDPrefix + 19 });
            testCases.Add(new TestCaseInfo { Target = "{", TestID = instanceIDPrefix + 20 });   //
            testCases.Add(new TestCaseInfo { Target = "{}", TestID = instanceIDPrefix + 21 });  //
            return testCases;
        }
        #endregion
    }

    public class SpecialPrimitiveValues
    {
        decimal[] _decimalValues;
        double[] _doubleValues;
        float[] _floatValues;

        public double[] DoubleValues
        {
            get { return this._doubleValues; }
            set { this._doubleValues = value; }
        }

        public decimal[] DecimalValues
        {
            get { return this._decimalValues; }
            set { this._decimalValues = value; }
        }

        public float[] FloatValues
        {
            get { return this._floatValues; }
            set { this._floatValues = value; }
        }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            SpecialPrimitiveValues instance1 = new SpecialPrimitiveValues();
            instance1._decimalValues = new decimal[] { decimal.MinusOne, decimal.One, decimal.Zero };
            instance1._doubleValues = new double[] { double.NaN, double.NegativeInfinity, double.PositiveInfinity, double.Epsilon };
            instance1._floatValues = new float[] { float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon };
            testCases.Add(new TestCaseInfo { Target = instance1, TestID = instanceIDPrefix + 1 });

            return testCases;
        }
        #endregion
    }

    public class DateTimeKindType
    {
        DateTime[] _dateTimes;

        public DateTime[] DateTimes
        {
            get { return _dateTimes; }
            set { _dateTimes = value; }
        }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            DateTimeKindType instance1 = new DateTimeKindType();
            instance1._dateTimes = new DateTime[] { 
                                    new DateTime(11111, DateTimeKind.Local),
                                    new DateTime(11111, DateTimeKind.Unspecified),
                                    new DateTime(11111, DateTimeKind.Utc)};
            testCases.Add(new TestCaseInfo { Target = instance1, TestID = instanceIDPrefix + 1 });

            return testCases;
        }
        #endregion
    }

    public class ClassWithEmptyList
    {
        IList<string> _data = new List<string>();
        public IList<string> Bar
        {
            get { return _data; }
            //set { this.data = value; }
        }
    }

}

namespace Microsoft.Test.Xaml.Types.PointDisambiguationNamespace
{
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
}
