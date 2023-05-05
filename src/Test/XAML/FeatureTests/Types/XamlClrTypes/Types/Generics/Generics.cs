// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;
    
    public class GenericType1<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
    {
        private T1 _info1;
        private T2 _info2;
        private T3 _info3;
        private T4 _info4;
        private T5 _info5;
        private T6 _info6;
        private T7 _info7;
        private T8 _info8;
        private T9 _info9;
        private T10 _info10;
        private T11 _info11;
        private T12 _info12;
        private T13 _info13;
        private T14 _info14;
        private T15 _info15;
        private T16 _info16;
        private T17 _info17;

        public T1 Info1
        {
            get
            {
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }

        public T2 Info2
        {
            get
            {
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }

        public T3 Info3
        {
            get
            {
                return _info3;
            }
            set
            {
                _info3 = value;
            }
        }

        public T4 Info4
        {
            get
            {
                return _info4;
            }
            set
            {
                _info4 = value;
            }
        }

        public T5 Info5
        {
            get
            {
                return _info5;
            }
            set
            {
                _info5 = value;
            }
        }

        public T6 Info6
        {
            get
            {
                return _info6;
            }
            set
            {
                _info6 = value;
            }
        }

        public T7 Info7
        {
            get
            {
                return _info7;
            }
            set
            {
                _info7 = value;
            }
        }

        public T8 Info8
        {
            get
            {
                return _info8;
            }
            set
            {
                _info8 = value;
            }
        }

        public T9 Info9
        {
            get
            {
                return _info9;
            }
            set
            {
                _info9 = value;
            }
        }

        public T10 Info10
        {
            get
            {
                return _info10;
            }
            set
            {
                _info10 = value;
            }
        }

        public T11 Info11
        {
            get
            {
                return _info11;
            }
            set
            {
                _info11 = value;
            }
        }

        public T12 Info12
        {
            get
            {
                return _info12;
            }
            set
            {
                _info12 = value;
            }
        }

        public T13 Info13
        {
            get
            {
                return _info13;
            }
            set
            {
                _info13 = value;
            }
        }

        public T14 Info14
        {
            get
            {
                return _info14;
            }
            set
            {
                _info14 = value;
            }
        }

        public T15 Info15
        {
            get
            {
                return _info15;
            }
            set
            {
                _info15 = value;
            }
        }

        public T16 Info16
        {
            get
            {
                return _info16;
            }
            set
            {
                _info16 = value;
            }
        }

        public T17 Info17
        {
            get
            {
                return _info17;
            }
            set
            {
                _info17 = value;
            }
        }

        public override string ToString()
        {
            return "Microsoft.Test.Xaml.Types.GenericType";
        }
    }

    // We recently made a compat change to conform to 3.0 behavior where 
    // "get-only" syntax was written down even for get/set collection properties.  
    // This relies on the getter following coding guidelines which instruct that 
    // getters of collection properties should return a non null value even if 
    // the property has a setter. 
    public class Factory
    {
        public static T Create<T>(T value)
        {
            if (typeof(ICollection).IsAssignableFrom(typeof(T)))
            {
                T obj = (T)Activator.CreateInstance(typeof(T));
                if (obj == null)
                {
                    throw new TestCaseFailedException("Unable to create instance of " + typeof(T));
                }
                return obj;
            }
            else
            {
                return value;
            }
        }
    }
    
    public class GenericType1<T1, T2> 
    {
        private T1 _info1;
        private T2 _info2;

        public T1 Info1
        {
            get
            {
                if (_info1 == null)
                {
                    _info1 = (T1)Factory.Create<T1>(_info1);
                }
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }

        public T2 Info2
        {
            get
            {
                if (_info2 == null )
                {
                    _info2 = (T2)Factory.Create<T2>(_info2);
                }
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }

        public override string ToString()
        {
            return "ClassGenericType1T1T2";
        }
    }

    public struct GenericType1<T1, T2, T3>
    {
        private T1 _info1;
        private T2 _info2;
        private T3 _info3;

        public T1 Info1
        {
            get
            {
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }

        public T2 Info2
        {
            get
            {
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }

        public T3 Info3
        {
            get
            {
                return _info3;
            }
            set
            {
                _info3 = value;
            }
        }
    }

    public class GenericContainerType1
    {
        private GenericType1<bool, sbyte, short, int, long, uint, ushort, byte, ulong, float, double, decimal, string, DateTime, char, TimeSpan, Guid> _infos;

        public GenericType1<bool, sbyte, short, int, long, uint, ushort, byte, ulong, float, double, decimal, string, DateTime, char, TimeSpan, Guid> Infos
        {
            get
            {
                return _infos;
            }
            set
            {
                _infos = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericContainerType1 instance1 = new GenericContainerType1();
            //all with default value
            instance1.Infos = new GenericType1<bool, sbyte, short, int, long, uint, ushort, byte, ulong, float, double, decimal, string, DateTime, char, TimeSpan, Guid>();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            GenericContainerType1 instance2 = new GenericContainerType1();
            instance2.Infos = new GenericType1<bool, sbyte, short, int, long, uint, ushort, byte, ulong, float, double, decimal, string, DateTime, char, TimeSpan, Guid>();
            instance2.Infos.Info1 = true;
            instance2.Infos.Info2 = sbyte.MinValue;
            instance2.Infos.Info3 = short.MaxValue;
            instance2.Infos.Info4 = int.MinValue;
            instance2.Infos.Info5 = long.MinValue;
            instance2.Infos.Info6 = uint.MinValue;
            instance2.Infos.Info7 = ushort.MaxValue;
            instance2.Infos.Info8 = byte.MinValue;
            instance2.Infos.Info9 = ulong.MinValue;
            instance2.Infos.Info10 = float.Epsilon;
            instance2.Infos.Info11 = double.NaN;
            instance2.Infos.Info12 = decimal.Zero;
            instance2.Infos.Info13 = string.Empty;
            instance2.Infos.Info14 = DateTime.MinValue;
            instance2.Infos.Info15 = 'a';
            instance2.Infos.Info16 = TimeSpan.MaxValue;
            instance2.Infos.Info17 = Guid.Empty;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2, TestID = instanceIDPrefix + 2
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2.Infos, TestID = instanceIDPrefix + 3
                              }); // generic on top
            return testCases;
        }

        #endregion
    }

    public class GenericType2<T1, T2, T3>
    {
        private T1 _info1;
        private T2 _info2;
        private T3 _info3;

        public T1 Info1
        {
            get
            {
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }

        public T2 Info2
        {
            get
            {
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }

        public T3 Info3
        {
            get
            {
                return _info3;
            }
            set
            {
                _info3 = value;
            }
        }
    }

    public class GenericContainerType2<T1, T2, T3>
    {
        private GenericType2<T1[], T2[][], T3[][][]> _infos;

        public GenericType2<T1[], T2[][], T3[][][]> Infos
        {
            get
            {
                return _infos;
            }
            set
            {
                _infos = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericContainerType2<sbyte, TimeSpan, string> instance1 = new GenericContainerType2<sbyte, TimeSpan, string>();
            sbyte[] sbytes = new sbyte[]
                                 {
                                     sbyte.MinValue, sbyte.MinValue, 0
                                 };
            TimeSpan[][] timeSpans = new TimeSpan[][]
                                         {
                                             new TimeSpan[]
                                                 {
                                                 }, new TimeSpan[]
                                                        {
                                                            TimeSpan.MaxValue, TimeSpan.MinValue
                                                        }
                                         };
            string[][][] strings = new string[][][]
                                       {
                                           new string[][]
                                               {
                                               }, new string[][]
                                                      {
                                                          new string[]
                                                              {
                                                              }
                                                      }, new string[][]
                                                             {
                                                                 new string[]
                                                                     {
                                                                         "a", "b"
                                                                     }
                                                             }
                                       };
            instance1.Infos = new GenericType2<sbyte[], TimeSpan[][], string[][][]>();
            instance1.Infos.Info1 = sbytes;
            instance1.Infos.Info2 = timeSpans;
            instance1.Infos.Info3 = strings;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1,
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Infos, TestID = instanceIDPrefix + 2,
                              }); // generic on top

            GenericContainerType2<bool, short, int> instance2 = new GenericContainerType2<bool, short, int>();
            bool[] bools = new bool[]
                               {
                                   true, false
                               };
            short[][] shorts = new short[][]
                                   {
                                       new short[]
                                           {
                                               short.MaxValue, short.MinValue
                                           }, new short[]
                                                  {
                                                  }
                                   };
            int[][][] ints = new int[][][]
                                 {
                                     new int[][]
                                         {
                                         }, new int[][]
                                                {
                                                    new int[]
                                                        {
                                                        }, new int[]
                                                               {
                                                                   1, 2
                                                               }, new int[]
                                                                      {
                                                                      }
                                                }, new int[][]
                                                       {
                                                           new int[]
                                                               {
                                                                   int.MinValue, int.MaxValue
                                                               }
                                                       }
                                 };
            instance2.Infos = new GenericType2<bool[], short[][], int[][][]>();
            instance2.Infos.Info1 = bools;
            instance2.Infos.Info2 = shorts;
            instance2.Infos.Info3 = ints;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2, TestID = instanceIDPrefix + 3,
                              });

            GenericContainerType2<long, uint, ushort> instance3 = new GenericContainerType2<long, uint, ushort>();
            long[] longs = new long[]
                               {
                                   long.MinValue, long.MaxValue
                               };
            uint[][] uints = new uint[][]
                                 {
                                     new uint[]
                                         {
                                             uint.MaxValue, uint.MinValue
                                         }, new uint[]
                                                {
                                                }
                                 };
            ushort[][][] ushorts = new ushort[][][]
                                       {
                                           new ushort[][]
                                               {
                                               }, new ushort[][]
                                                      {
                                                          new ushort[]
                                                              {
                                                              }, new ushort[]
                                                                     {
                                                                         1, 2
                                                                     }, new ushort[]
                                                                            {
                                                                            }
                                                      }, new ushort[][]
                                                             {
                                                                 new ushort[]
                                                                     {
                                                                         ushort.MinValue, ushort.MaxValue
                                                                     }
                                                             }
                                       };
            instance3.Infos = new GenericType2<long[], uint[][], ushort[][][]>();
            instance3.Infos.Info1 = longs;
            instance3.Infos.Info2 = uints;
            instance3.Infos.Info3 = ushorts;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance3, TestID = instanceIDPrefix + 4,
                              });

            GenericContainerType2<byte, ulong, float> instance4 = new GenericContainerType2<byte, ulong, float>();
            byte[] bytes = new byte[]
                               {
                                   byte.MinValue, byte.MaxValue
                               };
            ulong[][] ulongs = new ulong[][]
                                   {
                                       new ulong[]
                                           {
                                               ulong.MaxValue, ulong.MinValue
                                           }, new ulong[]
                                                  {
                                                  }
                                   };
            float[][][] floats = new float[][][]
                                     {
                                         new float[][]
                                             {
                                             }, new float[][]
                                                    {
                                                        new float[]
                                                            {
                                                            }, new float[]
                                                                   {
                                                                       1, 2
                                                                   }, new float[]
                                                                          {
                                                                          }
                                                    }, new float[][]
                                                           {
                                                               new float[]
                                                                   {
                                                                       float.MinValue, float.MaxValue
                                                                   }
                                                           }
                                     };
            instance4.Infos = new GenericType2<byte[], ulong[][], float[][][]>();
            instance4.Infos.Info1 = bytes;
            instance4.Infos.Info2 = ulongs;
            instance4.Infos.Info3 = floats;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance4, TestID = instanceIDPrefix + 5,
                              });

            GenericContainerType2<double, decimal, char> instance5 = new GenericContainerType2<double, decimal, char>();
            double[] doubles = new double[]
                                   {
                                       double.MinValue, double.MaxValue
                                   };
            decimal[][] decimals = new decimal[][]
                                       {
                                           new decimal[]
                                               {
                                                   decimal.MaxValue, decimal.MinValue
                                               }, new decimal[]
                                                      {
                                                      }
                                       };
            char[][][] chars = new char[][][]
                                   {
                                       new char[][]
                                           {
                                           }, new char[][]
                                                  {
                                                      new char[]
                                                          {
                                                          }, new char[]
                                                                 {
                                                                     '1', '2'
                                                                 }, new char[]
                                                                        {
                                                                        }
                                                  }, new char[][]
                                                         {
                                                             new char[]
                                                                 {
                                                                     'a', '1'
                                                                 }
                                                         }
                                   };
            instance5.Infos = new GenericType2<double[], decimal[][], char[][][]>();
            instance5.Infos.Info1 = doubles;
            instance5.Infos.Info2 = decimals;
            instance5.Infos.Info3 = chars;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance5, TestID = instanceIDPrefix + 6,
                              });

            GenericContainerType2<Guid, DateTime, char> instance6 = new GenericContainerType2<Guid, DateTime, char>();
            Guid[] Guids = new Guid[]
                               {
                                   Guid.Empty, new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4")
                               };
            DateTime[][] DateTimes = new DateTime[][]
                                         {
                                             new DateTime[]
                                                 {
                                                     CultureInfo.InvariantCulture.DateTimeFormat.Calendar.MaxSupportedDateTime, 
                                                     CultureInfo.InvariantCulture.DateTimeFormat.Calendar.MinSupportedDateTime
                                                 }, new DateTime[]
                                                        {
                                                        }
                                         };
            instance6.Infos = new GenericType2<Guid[], DateTime[][], char[][][]>();
            instance6.Infos.Info1 = Guids;
            instance6.Infos.Info2 = DateTimes;
            instance6.Infos.Info3 = null;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance6, TestID = instanceIDPrefix + 7,
                              });

            return testCases;
        }

        #endregion
    }

    public struct GenericContainerType3
    {
        private GenericType1<GenericType1<int, double, Guid>, GenericType2<bool, long, ulong>> _infos;

        public GenericType1<GenericType1<int, double, Guid>, GenericType2<bool, long, ulong>> Infos
        {
            get
            {
                return _infos;
            }
            set
            {
                _infos = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericContainerType3 instance1 = new GenericContainerType3();
            instance1.Infos = new GenericType1<GenericType1<int, double, Guid>, GenericType2<bool, long, ulong>>();
            instance1.Infos.Info1 = new GenericType1<int, double, Guid>();
            GenericType1<int, double, Guid> info1 = new GenericType1<int, double, Guid>();
            info1.Info1 = 100;
            info1.Info2 = 100.001;
            info1.Info3 = Guid.Empty;
            instance1.Infos.Info1 = info1;
            instance1.Infos.Info2 = new GenericType2<bool, long, ulong>();
            instance1.Infos.Info2.Info1 = true;
            instance1.Infos.Info2.Info2 = (long) -0.000001;
            instance1.Infos.Info2.Info3 = (ulong) 0.000001;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Infos, TestID = instanceIDPrefix + 2
                              }); // generic on top
            return testCases;
        }

        #endregion
    }

    public class GenericContainerType4
    {
        private GenericType1<ArrayList, Hashtable> _info;

        public GenericType1<ArrayList, Hashtable> Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            //empty
            GenericContainerType4 instance1 = new GenericContainerType4();
            instance1.Info = new GenericType1<ArrayList, Hashtable>();
            instance1.Info.Info1 = new ArrayList();
            instance1.Info.Info2 = new Hashtable(StringComparer.OrdinalIgnoreCase);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Info, TestID = instanceIDPrefix + 2
                              }); // generic on top

            return testCases;
        }

        #endregion
    }

    public class GenericContainerType5
    {
        private GenericType1<ArrayList, Hashtable> _info;

        public GenericType1<ArrayList, Hashtable> Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }


        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericContainerType5 instance1 = new GenericContainerType5();
            ArrayList arrayList = new ArrayList();
            Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            instance1.Info = new GenericType1<ArrayList, Hashtable>();
            instance1.Info.Info1 = arrayList;
            instance1.Info.Info2 = hashtable;
            arrayList.Add(sbyte.MinValue);
            arrayList.Add(true);
            arrayList.Add(short.MinValue);
            arrayList.Add(int.MaxValue);
            arrayList.Add(long.MaxValue);
            arrayList.Add(uint.MinValue);
            arrayList.Add(ushort.MaxValue);
            arrayList.Add(byte.MinValue);
            arrayList.Add(ulong.MinValue);
            arrayList.Add(float.Epsilon);
            arrayList.Add(double.NaN);
            arrayList.Add(decimal.Zero);
            arrayList.Add("ArrayList");
            arrayList.Add(DateTime.MinValue);
            arrayList.Add('a');
            arrayList.Add(TimeSpan.MinValue);
            arrayList.Add(Guid.Empty);
            hashtable.Add(true, true);
            hashtable.Add(sbyte.MinValue, sbyte.MaxValue);
            hashtable.Add(short.MinValue, short.MaxValue);
            //hashtable.Add(int.MinValue, int.MaxValue);
            // [DISABLED]
            // testCases.Add(new TestCaseInfo
            //                   {
            //                       Target = instance1, 
            //                       TestID = instanceIDPrefix + 1 + "[bug14787]",
            //                   });
            // testCases.Add(new TestCaseInfo
            //                   {
            //                       Target = instance1.Info, 
            //                       TestID = instanceIDPrefix + 2 + "[bug14787]",
            //                   }); // generic on top

            return testCases;
        }

        #endregion
    }

    public class GenericContainerType6
    {
        private GenericType1<GenericType1<int, double>, GenericType1<int, double>> _info;

        public GenericType1<GenericType1<int, double>, GenericType1<int, double>> Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericContainerType6 instance1 = new GenericContainerType6();
            instance1.Info = new GenericType1<GenericType1<int, double>, GenericType1<int, double>>();
            GenericType1<int, double> item = new GenericType1<int, double>();
            item.Info1 = 100;
            item.Info2 = 100.001;
            instance1.Info.Info1 = item;
            instance1._info.Info2 = item;
            // 
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Info, TestID = instanceIDPrefix + 2
                              }); // generic on top

            return testCases;
        }

        #endregion
    }

    public interface IExtensible<T> where T : IExtensible<T>
    {
        T Info { get; set; }
    }

    public class GenericType3 : IExtensible<GenericType3>
    {
        private GenericType3 _info;
        public GenericType3 Info
        {
            get
            {
                return this._info;
            }
            set
            {
                this._info = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            GenericType3 instance1 = new GenericType3();
            instance1.Info = new GenericType3();
            instance1.Info.Info = new GenericType3();
            instance1.Info.Info.Info = new GenericType3();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Info, TestID = instanceIDPrefix + 2
                              }); // generic on top

            return testCases;
        }

        #endregion
    }

    public class GenericType4<T1, T2> : GenericType1<T1, T2>
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType4<double, Guid>
                                                {
                                                    Info1 = double.Epsilon,
                                                    Info2 = Guid.NewGuid()
                                                },
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType4<int, string>
                                                {
                                                    Info1 = int.MaxValue,
                                                    Info2 = "blah"
                                                },
                                   TestID = instanceIDPrefix + 2
                               }
                       };
        }

        #endregion
    }

    public class GenericType5 : GenericType1<int, string>
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType5(),
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType5
                                                {
                                                    Info1 = int.MaxValue,
                                                    Info2 = string.Empty
                                                },
                                   TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType5
                                                {
                                                    Info1 = int.MinValue,
                                                    Info2 = "some random string"
                                                },
                                   TestID = instanceIDPrefix + 3
                               }
                       };
        }

        #endregion
    }

    public class GenericType6<T> : GenericType1<int, string>
    {
        public T DerivedInfo { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType6<string>
                                                {
                                                    Info1 = 3,
                                                    Info2 = "test",
                                                    DerivedInfo = "some string"
                                                },
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType6<int>
                                                {
                                                    Info1 = 3,
                                                    Info2 = "test",
                                                    DerivedInfo = 4
                                                },
                                   TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType6<int>(),
                                   TestID = instanceIDPrefix + 3
                               }
                       };
        }

        #endregion
    }

    public class GenericType_Outer1<T>
    {
        public class GenericType_Inner<S>
        {
            public S InnerInfo { get; set; }
        }

        public GenericType_Inner<T> OuterInfo { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType_Inner<string>
                                                {
                                                    InnerInfo = "blah"
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType_Outer1<int>
                                                {
                                                    OuterInfo = new GenericType_Outer1<int>.GenericType_Inner<int>
                                                                    {
                                                                        InnerInfo = int.MaxValue
                                                                    }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                               }
                       };
        }

        #endregion
    }

    public class GenericType_Outer2
    {
        public class GenericType_Inner<T>
        {
            public T InnerInfo { get; set; }
        }

        public GenericType_Inner<int> OuterInfo { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType_Inner<string>
                                                {
                                                    InnerInfo = "some string"
                                                },
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType_Outer2
                                                {
                                                    OuterInfo = new GenericType_Inner<int>
                                                                    {
                                                                        InnerInfo = 4
                                                                    }
                                                },
                                   TestID = instanceIDPrefix + 2,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                               }
                       };
        }

        #endregion
    }

    public class GenericContainerType7
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType1<Guid, double>[0], TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType1<Guid, double>[]
                                                {
                                                    new GenericType1<Guid, double>
                                                        {
                                                            Info1 = Guid.NewGuid(), Info2 = double.NaN
                                                        },
                                                    new GenericType1<Guid, double>()
                                                },
                                   TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = new List<GenericType1<DateTime, int>>(), TestID = instanceIDPrefix + 3
                               },
                           new TestCaseInfo
                               {
                                   Target = new Dictionary<string, GenericType1<short, string>>(),
                                   TestID = instanceIDPrefix + 4,
                               },
                           new TestCaseInfo
                               {
                                   Target = new Dictionary<string, GenericType1<short, string>>
                                                {
                                                    {
                                                        "Key1", new GenericType1<short, string>
                                                                    {
                                                                        Info1 = short.MinValue, Info2 = string.Empty
                                                                    }
                                                        },
                                                    {
                                                        "Key2", new GenericType1<short, string>
                                                                    {
                                                                        Info1 = short.MaxValue, Info2 = "some string"
                                                                    }
                                                        }
                                                },
                                   TestID = instanceIDPrefix + 5,
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType1<string, int>[0][], TestID = instanceIDPrefix + 6,
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType1<string, int>[3][]
                                                {
                                                    new GenericType1<string, int>[]
                                                        {
                                                            new GenericType1<string, int>(),
                                                            new GenericType1<string, int>
                                                                {
                                                                    Info1 = "some string", Info2 = 3
                                                                }
                                                        },
                                                    new GenericType1<string, int>[]
                                                        {
                                                            null
                                                        },
                                                    null
                                                },
                                   TestID = instanceIDPrefix + 7,
                               },
                           new TestCaseInfo
                               {
                                   Target = new List<GenericType1<DateTime, int>>
                                                {
                                                    new GenericType1<DateTime, int>
                                                        {
                                                            Info1 = DateTime.Now, Info2 = int.MinValue
                                                        },
                                                    new GenericType1<DateTime, int>
                                                        {
                                                            Info1 = DateTime.MinValue, Info2 = int.MaxValue
                                                        }
                                                },
                                   TestID = instanceIDPrefix + 8
                               },
                       };
        }

        #endregion
    }

    public class GenericType7<T1, T2>
    {
        private readonly T1 _info1;
        public T1 Info1
        {
            get
            {
                return _info1;
            }
        }

        private readonly T2 _info2;
        public T2 Info2
        {
            get
            {
                return _info2;
            }
        }

        public GenericType7(T1 Info1, T2 Info2)
        {
            _info1 = Info1;
            _info2 = Info2;
        }

        public GenericType7()
        {
            _info1 = default(T1);
            _info2 = default(T2);
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType7<double, short>(), TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType7<Guid, DateTime>(Guid.NewGuid(), DateTime.Now),
                                   TestID = instanceIDPrefix + 2
                               },
                       };
        }

        #endregion
    }

    /// <summary>
    /// This has two derived generic types.  1 overrides Data the other doesn't.
    /// Verify roundtripping succeeds.
    /// </summary>
    public class GenericType8
    {
        public virtual ClassType1 Data { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new GenericType8<double>() { Data = new ClassType1 { Category = "stuff" }},
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new GenericType8<double, double> { Data = new ClassType1 { Category = "stuff" }},
                                   TestID = instanceIDPrefix + 2
                               },
                       };
        }

        #endregion
    }

    public class GenericType8<T> : GenericType8
    {
        public override ClassType1 Data { get; set; }
    }

    public class GenericType8<T1, T2> : GenericType8
    {
    }

    public class GenericType9<T>
    {
       public T Value { get; set; }
    }
}
