// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
        //using System.Runtime.Serialization.Description;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    public class ClassType_Inheritance1
    {
        private string _name;

        public ClassType_Inheritance1()
        {
            this._name = "base";
        }

        public virtual string Name
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
    }

    // 

    public class ClassType_Inheritance2 : ClassType_Inheritance1
    {
        private string _name;

        public ClassType_Inheritance2()
        {
            this._name = "derived";
        }

        public new string Name
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
            ClassType_Inheritance2 instance1 = new ClassType_Inheritance2();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // "override" the virtual property (primitive)
    public class ClassType_Inheritance3 : ClassType_Inheritance1
    {
        private string _name;

        public ClassType_Inheritance3()
        {
            this._name = "overide_new";
        }

        public override string Name
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
            ClassType_Inheritance3 instance1 = new ClassType_Inheritance3();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    public class ClassType_Inheritance4
    {
        private string[] _names;

        public ClassType_Inheritance4()
        {
        }

        public virtual string[] Names
        {
            get
            {
                return this._names;
            }
            set
            {
                this._names = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            //
            ClassType_Inheritance4 instance1 = new ClassType_Inheritance4();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // "override" the virtual property (array)
    public class ClassType_Inheritance5 : ClassType_Inheritance4
    {
        private string[] _names;

        public ClassType_Inheritance5()
        {
        }

        public override string[] Names
        {
            get
            {
                return this._names;
            }
            set
            {
                this._names = value;
            }
        }

        #region Test Implementation

        public new static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            string[] names1 = new string[]
                                  {
                                      "a", "b"
                                  };
            string[] names2 = new string[]
                                  {
                                      "c", "d", "e"
                                  };
            ClassType_Inheritance5 instance1 = new ClassType_Inheritance5();
            instance1.Names = names1;
            ((ClassType_Inheritance4) instance1).Names = names2;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // "new" to hide the inherited member (array)
    public class ClassType_Inheritance6 : ClassType_Inheritance4
    {
        private string[] _names;

        public ClassType_Inheritance6()
        {
        }

        public new string[] Names
        {
            get
            {
                return this._names;
            }
            set
            {
                this._names = value;
            }
        }

        #region Test Implementation

        public new static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // 
            string[] names1 = new string[]
                                  {
                                      "a", "b"
                                  };
            string[] names2 = new string[]
                                  {
                                      "c", "d", "e"
                                  };
            ClassType_Inheritance6 instance1 = new ClassType_Inheritance6();
            instance1.Names = names1;
            ((ClassType_Inheritance4) instance1).Names = names2;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    /**************deep inheritance****************/

    public class ClassType_Inheritance7 : ClassType_Inheritance1
    {
        private int _intValue = -1;

        public int IntValue
        {
            get
            {
                return this._intValue;
            }
            set
            {
                this._intValue = value;
            }
        }
    }

    public class ClassType_Inheritance8 : ClassType_Inheritance7
    {
        private long _longValue = long.MaxValue;

        public long LongValue
        {
            get
            {
                return this._longValue;
            }
            set
            {
                this._longValue = value;
            }
        }
    }

    public class ClassType_Inheritance9 : ClassType_Inheritance8
    {
        private double _doubleValue = double.MinValue;

        public double DoubleValue
        {
            get
            {
                return this._doubleValue;
            }
            set
            {
                this._doubleValue = value;
            }
        }
    }

    public class ClassType_Inheritance10 : ClassType_Inheritance9
    {
        private short _shortValue = 23;

        public short ShortValue
        {
            get
            {
                return this._shortValue;
            }
            set
            {
                this._shortValue = value;
            }
        }
    }

    public class ClassType_Inheritance11 : ClassType_Inheritance10
    {
        private byte _byteValue = (byte) 1;

        public byte ByteValue
        {
            get
            {
                return this._byteValue;
            }
            set
            {
                this._byteValue = value;
            }
        }
    }

    public class ClassType_Inheritance12 : ClassType_Inheritance11
    {
        private sbyte _sbyteValue = sbyte.MinValue;

        public sbyte SbyteValue
        {
            get
            {
                return this._sbyteValue;
            }
            set
            {
                this._sbyteValue = value;
            }
        }
    }

    public class ClassType_Inheritance13 : ClassType_Inheritance12
    {
        private ushort _ushortValue = ushort.MinValue;

        public ushort UshortValue
        {
            get
            {
                return this._ushortValue;
            }
            set
            {
                this._ushortValue = value;
            }
        }
    }

    public class ClassType_Inheritance14 : ClassType_Inheritance13
    {
        private bool _boolValue = false;

        public bool BoolValue
        {
            get
            {
                return this._boolValue;
            }
            set
            {
                this._boolValue = value;
            }
        }
    }

    public class ClassType_Inheritance15 : ClassType_Inheritance14
    {
        private float _floatValue = 2.3F;

        public float FloatValue
        {
            get
            {
                return this._floatValue;
            }
            set
            {
                this._floatValue = value;
            }
        }
    }

    public class ClassType_Inheritance16 : ClassType_Inheritance15
    {
        private char _charValue = 'a';

        public char CharValue
        {
            get
            {
                return this._charValue;
            }
            set
            {
                this._charValue = value;
            }
        }
    }

    public class ClassType_Inheritance17 : ClassType_Inheritance16
    {
        private decimal _decimalValue = decimal.MaxValue;

        public decimal DecimalValue
        {
            get
            {
                return this._decimalValue;
            }
            set
            {
                this._decimalValue = value;
            }
        }
    }

    // 10111: boxed primitive values fail XamlSerializer serialization
    public class ClassType_Inheritance18 : ClassType_Inheritance17
    {
        private object _objectValue = 12;

        public object ObjectValue
        {
            get
            {
                return this._objectValue;
            }
            set
            {
                this._objectValue = value;
            }
        }
    }

    public class ClassType_Inheritance19 : ClassType_Inheritance18
    {
        private byte[] _byteArrayValue = new byte[]
                                            {
                                                (byte) '1', (byte) 'c'
                                            };

        public byte[] ByteArrayValue
        {
            get
            {
                return this._byteArrayValue;
            }
            set
            {
                this._byteArrayValue = value;
            }
        }
    }

    public class ClassType_Inheritance20 : ClassType_Inheritance19
    {
        private Guid _guidValue = new Guid();

        public Guid GuidValue
        {
            get
            {
                return this._guidValue;
            }
            set
            {
                this._guidValue = value;
            }
        }
    }

    public class ClassType_Inheritance21 : ClassType_Inheritance20
    {
        private DateTime _dateTimeValue = new DateTime(2007, 7, 16, 5, 27, 30);

        public DateTime DateTimeValue
        {
            get
            {
                return this._dateTimeValue;
            }
            set
            {
                this._dateTimeValue = value;
            }
        }
    }

    public class ClassType_Inheritance22 : ClassType_Inheritance21
    {
        private TimeSpan _timeSpanValue = TimeSpan.FromDays(123);

        public TimeSpan TimeSpanValue
        {
            get
            {
                return this._timeSpanValue;
            }
            set
            {
                this._timeSpanValue = value;
            }
        }
    }

    // 
    public class ClassType_Inheritance23 : ClassType_Inheritance22
    {
        private Uri _uriValue = new Uri("http://test.com");

        public Uri UriValue
        {
            get
            {
                return this._uriValue;
            }
            set
            {
                this._uriValue = value;
            }
        }
    }
}
