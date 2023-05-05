// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;

    public class ClassType_Nested1
    {
        // nested empty type
        public struct NestedType1
        {
            #region Test Implementation

            public static List<TestCaseInfo> GetTestCases()
            {
                string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
                List<TestCaseInfo> testCases = new List<TestCaseInfo>();

                NestedType1 instance1 = new NestedType1();
                testCases.Add(new TestCaseInfo
                                  {
                                      Target = instance1,
                                      ExpectedResult = false,
                                      ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                      TestID = instanceIDPrefix + 1
                                  });

                List<NestedType1> instance2 = new List<NestedType1>()
                                                  {
                                                      new NestedType1()
                                                  };

                testCases.Add(new TestCaseInfo
                                  {
                                      Target = instance2,
                                      ExpectedResult = false,
                                      ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                      TestID = instanceIDPrefix + 2
                                  });

                ClassType_Nested1.NestedType4.NestedNestedType2 instance3 = new ClassType_Nested1.NestedType4.NestedNestedType2();
                testCases.Add(new TestCaseInfo()
                {
                    Target = instance3,
                    ExpectedResult = false,
                    ExpectedMessage = Exceptions.GetMessage("ObjectReader_TypeNotVisible", WpfBinaries.SystemXaml),
                    TestID = instanceIDPrefix + 3
                });


                return testCases;
            }

            #endregion
        }

        // with matching constructor arg
        public class NestedType2
        {
            private readonly NestedType1 _field1;

            public NestedType2()
            {
            }

            public NestedType2(NestedType1 field1)
            {
                this._field1 = field1;
            }

            public NestedType1 Field1
            {
                get
                {
                    return this._field1;
                }
            }

            #region Test Implementation

            public static List<TestCaseInfo> GetTestCases()
            {
                string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
                List<TestCaseInfo> testCases = new List<TestCaseInfo>();

                NestedType2 instance1 = new NestedType2(new NestedType1());
                testCases.Add(new TestCaseInfo
                                  {
                                      Target = instance1,
                                      ExpectedResult = false,
                                      ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                      TestID = instanceIDPrefix + 1
                                  });

                return testCases;
            }

            #endregion
        }

        // without matching constructor arg
        public class NestedType3
        {
            private readonly NestedType1 _field1;
            private readonly NestedType1 _field2;
            private readonly int _field3;

            public NestedType3()
            {
            }

            public NestedType3(NestedType1 field)
            {
                this._field1 = field;
                throw new DataTestException("This constructor should not be called since the parameter name does not mach to the Property name.");
            }

            public NestedType3(NestedType1 field2, int field3)
            {
                this._field2 = field2;
                this._field3 = field3;
            }

            public NestedType1 Field1
            {
                get
                {
                    return this._field1;
                }
            }

            public NestedType1 Field2
            {
                get
                {
                    return this._field2;
                }
            }

            public int Field3
            {
                get
                {
                    return this._field3;
                }
            }

            #region Test Implementation

            public static List<TestCaseInfo> GetTestCases()
            {
                string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
                List<TestCaseInfo> testCases = new List<TestCaseInfo>();

                NestedType3 instance1 = new NestedType3(new NestedType1(), 100);
                testCases.Add(new TestCaseInfo
                                  {
                                      Target = instance1,
                                      ExpectedResult = false,
                                      ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                      TestID = instanceIDPrefix + 1
                                  });

                return testCases;
            }

            #endregion
        }

        public class NestedType4
        {
            // nested-nested with inheritance
            public class NestedNestedType1 : NestedType2
            {
                public NestedNestedType1()
                {
                }

                public NestedNestedType1(NestedType1 field1)
                    : base(field1)
                {
                }

                #region Test Implementation

                public new static List<TestCaseInfo> GetTestCases()
                {
                    string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
                    List<TestCaseInfo> testCases = new List<TestCaseInfo>();

                    NestedNestedType1 instance1 = new NestedNestedType1(new NestedType1());
                    testCases.Add(new TestCaseInfo()
                                      {
                                          Target = instance1,
                                          ExpectedResult = false,
                                          ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                          TestID = instanceIDPrefix + 1
                                      });

                    return testCases;
                }

                #endregion
            }

            // non-public nested type
            // should not be serialized
            internal class NestedNestedType2
            {
                private int _field;

                public int Field
                {
                    get
                    {
                        return this._field;
                    }
                    set
                    {
                        this._field = value;
                    }
                }
            }
        }
    }

    public class ClassType_Nested2
    {
        private NestedType _nested;

        public ClassType_Nested2()
        {
        }

        public NestedType Nested
        {
            get
            {
                return this._nested;
            }
            set
            {
                this._nested = value;
            }
        }

        public class NestedType
        {
            private string[] _infos;

            public string[] Infos
            {
                get
                {
                    return this._infos;
                }
                set
                {
                    this._infos = value;
                }
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType_Nested2 instance1 = new ClassType_Nested2();
            instance1._nested = new NestedType();
            instance1._nested.Infos = new string[]
                                         {
                                             "info1", "info2"
                                         };
            testCases.Add(new TestCaseInfo()
                              {
                                  Target = instance1,
                                  ExpectedResult = false,
                                  ExpectedMessage = Exceptions.GetMessage("ObjectReaderTypeIsNested", WpfBinaries.SystemXaml),
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }
}
