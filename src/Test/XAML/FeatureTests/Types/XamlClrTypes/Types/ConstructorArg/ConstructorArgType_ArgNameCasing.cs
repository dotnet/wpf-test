// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using System.Windows.Markup;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;

    // ReadOnly properties 'Name' and 'Email' map to the constructor arguments in different casing
    // Property Age is not map anywhere
    public class ConstructorArgType_ArgNameCasing1
    {
        private readonly string _name;
        private readonly string _email;
        private int _age;

        public ConstructorArgType_ArgNameCasing1(string nAme, string emaiL)
        {
            this._name = nAme;
            this._email = emaiL;
        }

        [ConstructorArgument("nAme")]
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        [ConstructorArgument("emaiL")]
        public string Email
        {
            get
            {
                return this._email;
            }
        }

        public int Age
        {
            get
            {
                return this._age;
            }
            set
            {
                this._age = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ArgNameCasing1 instance1 = new ConstructorArgType_ArgNameCasing1("John", "john.1@XamlTests.com");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // constuctor arg name matching base constructor arg name
    public class ConstructorArgType_ArgNameCasing2 : ConstructorArgType_ArgNameCasing1
    {
        public ConstructorArgType_ArgNameCasing2(string nAme, string emaiL)
            : base(nAme, emaiL)
        {
        }

        #region Test Implementation

        public new static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ArgNameCasing2 instance1 = new ConstructorArgType_ArgNameCasing2("John", "john.1@XamlTests.com");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // constructor arg name case-different from base constructor arg name
    public class ConstructorArgType_ArgNameCasing3 : ConstructorArgType_ArgNameCasing1
    {
        public ConstructorArgType_ArgNameCasing3(string naMe, string emaiL)
            : base(naMe, emaiL)
        {
        }

        #region Test Implementation

        public new static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ArgNameCasing3 instance1 = new ConstructorArgType_ArgNameCasing3("Microsoft", "Microsoft.1@XamlTests.com");
            // update message //
            testCases.Add(new TestCaseInfo
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

    // base and derive have the same property name differ in case
    public class ConstructorArgType_ArgNameCasing4 : ConstructorArgType_ArgNameCasing1
    {
        private readonly string _name;
        private readonly string _email;

        public ConstructorArgType_ArgNameCasing4(string naMe, string emaiL)
            : base(naMe, emaiL)
        {
            this._name = naMe;
            this._email = emaiL;
        }

        [ConstructorArgument("naMe")]
        public string naME
        {
            get
            {
                return this._name;
            }
        }
        [ConstructorArgument("emaiL")]
        public string emaIL
        {
            get
            {
                return this._email;
            }
        }

        #region Test Implementation

        public new static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ConstructorArgType_ArgNameCasing4 instance1 = new ConstructorArgType_ArgNameCasing4("John", "john.1@XamlTests.com");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1 + "bug14804",
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoMatchingConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    // different properties match to the same constructor arg
    // negative test 9104
    public class ConstructorArgType_ArgNameCasing5
    {
        private readonly string _name;

        public ConstructorArgType_ArgNameCasing5(string nAme)
        {
            this._name = nAme;
        }

        [ConstructorArgument("nAme")]
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public string NAME
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

            ConstructorArgType_ArgNameCasing5 instance1 = new ConstructorArgType_ArgNameCasing5("John");
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // This case should not be serialized. 
    public class ConstructorArgType_ArgNameCasing6
    {
        private readonly string _name;

        public ConstructorArgType_ArgNameCasing6()
        {
            this._name = "John.AA";
        }

        public ConstructorArgType_ArgNameCasing6(string nAme, string name)
        {
            this._name = name + "." + nAme;
            throw new DataTestException("This Constructor should not be called.");
        }

        public string Name
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

            ConstructorArgType_ArgNameCasing6 instance1 = new ConstructorArgType_ArgNameCasing6();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1, TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }
}
