// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Globalization;
    using System.Globalization;
    using Microsoft.Test.Xaml.Driver;
    
    public class ConstructorArgType_ROProperty1
    {
        private string _name;

        public ConstructorArgType_ROProperty1()
        {
        }

        public ConstructorArgType_ROProperty1(string name)
        {
            this._name = name;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a RW Property.");
        }

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

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty1
                                                {
                                                    Name = "MyName"
                                                },
                                   TestID = instanceIDPrefix + 1
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty2
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty2()
        {
        }

        public ConstructorArgType_ROProperty2(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2("MyName"),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = "Two Xaml Objects are different."
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty2_Class
    {
        private readonly ClassType _name;

        public ConstructorArgType_ROProperty2_Class()
        {
        }

        public ConstructorArgType_ROProperty2_Class(ClassType name)
        {
            this._name = name;
        }

        public ClassType Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            ClassType instance = new ClassType();
            ClassType2[] array = new ClassType2[3];
            array[0] = new ClassType2
                           {
                               Category = new ClassType1
                                              {
                                                  Category = "blah"
                                              }
                           };
            array[1] = new ClassType2
                           {
                               Category = new ClassType1
                                              {
                                                  Category = "hello world"
                                              }
                           };
            array[2] = null;
            instance.Array = array;

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Class(new ClassType()),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = "Two Xaml Objects are different."
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Class(instance),
                                   TestID = instanceIDPrefix + 2,
                                   ExpectedResult = false,
                                   ExpectedMessage = "Two Xaml Objects are different."
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Class(null),
                                   TestID = instanceIDPrefix + 3
                               },
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty2_Struct
    {
        private readonly ClassType1 _name;

        public ConstructorArgType_ROProperty2_Struct()
        {
        }

        public ConstructorArgType_ROProperty2_Struct(ClassType1 name)
        {
            this._name = name;
        }

        public ClassType1 Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Struct(new ClassType1
                                                                                          {
                                                                                              Category = "category"
                                                                                          }),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = "Two Xaml Objects are different."
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Struct(new ClassType1()),
                                   TestID = instanceIDPrefix + 2
                               },
                       };
        }

        #endregion
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    public class ConstructorArgType_ROProperty2_Enum
    {
        private readonly TestEnum _name;

        public ConstructorArgType_ROProperty2_Enum()
        {
        }

        public ConstructorArgType_ROProperty2_Enum(TestEnum name)
        {
            this._name = name;
        }

        public TestEnum Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Enum(TestEnum.Value1),
                                   TestID = instanceIDPrefix + 1
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Enum(TestEnum.Value2),
                                   TestID = instanceIDPrefix + 2
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Enum(default(TestEnum)),
                                   TestID = instanceIDPrefix + 3
                               },
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty2_Collection
    {
        private readonly List<string> _name;

        public ConstructorArgType_ROProperty2_Collection()
        {
        }

        public ConstructorArgType_ROProperty2_Collection(List<string> name)
        {
            this._name = name;
        }

        public List<string> Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Collection(new List<string>
                                                                                              {
                                                                                                  "string1",
                                                                                                  "string2",
                                                                                              }
                                       ),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedMessage = Exceptions.GetMessage("GetObjectNull", WpfBinaries.SystemXaml),
                                   ExpectedResult = false,
                               },
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty2_Collection(null),
                                   TestID = instanceIDPrefix + 2,
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty3
    {
        private string _name = "DefaultName";

        public ConstructorArgType_ROProperty3()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            ConstructorArgType_ROProperty3 instance1 = new ConstructorArgType_ROProperty3();
            instance1._name = "Some other name";
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = instance1,
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = "Two Xaml Objects are different.",
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty4
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty4(string someOtherName)
        {
            this._name = someOtherName;
        }

        [ConstructorArgument("someOtherName")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty4("MyName"),
                                   TestID = instanceIDPrefix + 1
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty5
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty5(string someOtherName)
        {
            this._name = someOtherName;
        }

        [ConstructorArgument("someMissingName")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty5("MyName"),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty6
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty6(string someOtherName)
        {
            this._name = someOtherName;
        }

        [ConstructorArgument("someMissingName")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty6("MyName"),
                                   TestID = instanceIDPrefix + 1,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                                   ExpectedResult = false
                               },
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty7
    {
        private string _name;

        public ConstructorArgType_ROProperty7()
        {
        }

        public ConstructorArgType_ROProperty7(string someOtherName)
        {
            this._name = someOtherName;
        }

        [ConstructorArgument("someOtherName")]
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

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ConstructorArgType_ROProperty7
                                                {
                                                    Name = "MyName"
                                                },
                                   TestID = instanceIDPrefix + 1
                               }
                       };
        }

        #endregion
    }

    public class ConstructorArgType_ROProperty_R1
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R1()
        {
        }

        public ConstructorArgType_ROProperty_R1(string name)
        {
            this._name = name;
            //throw new DataTestException("This constructor should not be called since the parameter name mapped to a RW Property.");
        }

        public string Name
        {
            get
            {
                return _name;
            }
            //set { name = value; }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R1 instance1 = new ConstructorArgType_ROProperty_R1();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with same name
    /// </summary>
    public class ConstructorArgType_ROProperty_R2
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R2()
        {
        }

        public ConstructorArgType_ROProperty_R2(string name)
        {
            this._name = name;
            //throw new DataTestException("This constructor should not be called since the parameter name mapped to a RW Property.");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
            //set { name = value; }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R2 instance1 = new ConstructorArgType_ROProperty_R2();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg with different name 
    /// </summary>
    public class ConstructorArgType_ROProperty_R3
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R3()
        {
        }

        public ConstructorArgType_ROProperty_R3(string differentName)
        {
            this._name = differentName;
        }

        [ConstructorArgument("differentName")]
        public string Name
        {
            get
            {
                return _name;
            }
            //set { name = value; }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R3 instance1 = new ConstructorArgType_ROProperty_R3();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with different name plus having ctor with same RO property name but different type
    /// </summary>
    public class ConstructorArgType_ROProperty_R4
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R4(int value)
        {
        }

        public ConstructorArgType_ROProperty_R4(StringBuilder name)
        {
            this._name = name.ToString();
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different RO Property.");
        }

        public ConstructorArgType_ROProperty_R4(string differentName)
        {
            this._name = differentName;
        }

        [ConstructorArgument("differentName")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R4 instance1 = new ConstructorArgType_ROProperty_R4(123);
            // [DISABLED]
            // testCases.Add(new TestCaseInfo
            //                   {
            //                       Target = instance1, TestID = instanceIDPrefix + 1
            //                   });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with non existing name
    /// </summary>
    public class ConstructorArgType_ROProperty_R5
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R5(string name)
        {
            this._name = name;
        }

        [ConstructorArgument("nonExistingName")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R5 instance1 = new ConstructorArgType_ROProperty_R5("MyName");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with extra 'params'
    /// </summary>
    public class ConstructorArgType_ROProperty_R6
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R6(int value)
        {
        }

        public ConstructorArgType_ROProperty_R6(string name, params object[] args)
        {
            this._name = name;
        }

        public ConstructorArgType_ROProperty_R6(string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R6 instance1 = new ConstructorArgType_ROProperty_R6(123);
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with extra 'params'
    /// </summary>
    public class ConstructorArgType_ROProperty_R62
    {
        private readonly string _name;
        private readonly object[] _args = new object[]
                                             {
                                                 "Hello"
                                             };

        public ConstructorArgType_ROProperty_R62(string name, params object[] args)
        {
            this._name = name;
        }

        public ConstructorArgType_ROProperty_R62(string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public object[] Args
        {
            get
            {
                return this._args;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R62 instance1 = new ConstructorArgType_ROProperty_R62("abc", "cdf");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with extra argument
    /// </summary>
    public class ConstructorArgType_ROProperty_R7
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R7(string name, string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor with lesser equal param names");
        }

        public ConstructorArgType_ROProperty_R7(string name)
        {
            this._name = name;
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R7 instance1 = new ConstructorArgType_ROProperty_R7("abc");

            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg name with extra argument no other matching ctor with same parms
    /// </summary>
    public class ConstructorArgType_ROProperty_R8
    {
        private readonly string _name;

        public ConstructorArgType_ROProperty_R8(int value)
        {
        }

        public ConstructorArgType_ROProperty_R8(string name, string differentName)
        {
            this._name = differentName;
        }

        public ConstructorArgType_ROProperty_R8(string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor with lesser equal param names");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R8 instance1 = new ConstructorArgType_ROProperty_R8(1234);

            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg names - two argument ctor with two RO properties 
    /// </summary>
    public class ConstructorArgType_ROProperty_R9
    {
        private readonly string _name;
        private readonly string _differentName;

        public ConstructorArgType_ROProperty_R9(string name, string differentName)
        {
            this._name = name;
            this._differentName = differentName;
        }

        public ConstructorArgType_ROProperty_R9(string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor with lesser equal param names");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string DifferentName
        {
            get
            {
                return _differentName;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R9 instance1 = new ConstructorArgType_ROProperty_R9("abc", "cdf");

            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    /// <summary>
    /// RO Property with CtorArgumentAttribute matching ctor arg names - more than two argument ctor with more RO properties 
    /// </summary>
    public class ConstructorArgType_ROProperty_R10
    {
        private readonly string _name = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _differentName = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name3 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name4 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name5 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name6 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name7 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name8 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name9 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name10 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _123__Local = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _0x001_Local = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string __Local = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string ___Local = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
        private readonly string _name15 = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

        public ConstructorArgType_ROProperty_R10(bool init)
        {
        }

        public ConstructorArgType_ROProperty_R10(string name, string differentName, string name3, string name4, string name5, string name6, string name7, string name8, string name9, string _name10, string _123__, string _0x001, string __, string ___, string name15)
        {
            this._name = name;
            this._differentName = differentName;
        }

        public ConstructorArgType_ROProperty_R10(string differentName)
        {
            this._name = differentName;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a different ctor with more equal param names");
        }

        [ConstructorArgument("name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }
        public string Name3
        {
            get
            {
                return _name3;
            }
        }
        public string Name4
        {
            get
            {
                return _name4;
            }
        }
        public string Name5
        {
            get
            {
                return _name5;
            }
        }
        public string Name6
        {
            get
            {
                return _name6;
            }
        }
        public string Name7
        {
            get
            {
                return _name7;
            }
        }

        public string Name8
        {
            get
            {
                return _name8;
            }
        }

        public string Name9
        {
            get
            {
                return _name9;
            }
        }

        public string _Name10
        {
            get
            {
                return _name10;
            }
        }

        public string _123__
        {
            get
            {
                return _123__Local;
            }
        }

        public string _0x001
        {
            get
            {
                return _0x001_Local;
            }
        }

        public string __
        {
            get
            {
                return __Local;
            }
        }
        public string ___
        {
            get
            {
                return ___Local;
            }
        }
        public string Name15
        {
            get
            {
                return _name15;
            }
        }

        public string DifferentName
        {
            get
            {
                return _differentName;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ROProperty_R10 instance1 = new ConstructorArgType_ROProperty_R10(true);

            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    #region RW Property Types

    public class ConstructorArgWithRWPropertyTest_R1
    {
        private string _name;

        public ConstructorArgWithRWPropertyTest_R1()
        {
        }

        public ConstructorArgWithRWPropertyTest_R1(string name)
        {
            this._name = name;
            throw new DataTestException("This constructor should not be called since the parameter name mapped to a RW Property.");
        }

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

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgWithRWPropertyTest_R1 instance1 = new ConstructorArgWithRWPropertyTest_R1();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    #endregion
}
