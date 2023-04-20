// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Markup;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;

    public class Person
    {
        private readonly string _name;

        public Person(string name)
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
    }

    public class Blogger : Person
    {
        private readonly string _homepage;

        public Blogger(string homepage, string name)
            : base(name)
        {
            this._homepage = homepage;
        }

        public string Homepage
        {
            get
            {
                return _homepage;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // 
            Blogger instance1 = new Blogger("http://www.jj.com", "jj");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    public class BookCategory
    {
        private string _category;

        public BookCategory(string category)
        {
            this._category = category;
        }

        public string Category
        {
            get
            {
                return this._category;
            }
            set
            {
                this._category = value;
            }
        }
    }

    public class ScientificBook : BookCategory
    {
        private readonly string _title;

        public ScientificBook(string title)
            : base("Sientific")
        {
            this._title = title;
        }

        public string Title
        {
            get
            {
                return this._title;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ScientificBook instance1 = new ScientificBook("Adventure");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    public class ImmutableAddress
    {
        private readonly string _city;
        private readonly string _street;

        public ImmutableAddress(string street, string city)
        {
            this._street = street;
            this._city = city;
        }

        public string City
        {
            get
            {
                return this._city;
            }
        }

        public string Street
        {
            get
            {
                return this._street;
            }
        }
    }

    public class DerivedImmutableAddressWithoutArgs : ImmutableAddress
    {
        public DerivedImmutableAddressWithoutArgs(string city)
            : base("aaa", city)
        {
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            DerivedImmutableAddressWithoutArgs instance1 = new DerivedImmutableAddressWithoutArgs("Seattle");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    public class DerivedImmutableAddress : ImmutableAddress
    {
        public DerivedImmutableAddress(string street, string city)
            : base(street, city)
        {
        }
    }

    public class DerivedImmutableAddressWithoutArgs2 : DerivedImmutableAddress
    {
        public DerivedImmutableAddressWithoutArgs2(string city)
            : base("aaa", city)
        {
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            DerivedImmutableAddressWithoutArgs2 instance1 = new DerivedImmutableAddressWithoutArgs2("Seattle");
            testCases.Add(
                new TestCaseInfo
                    {
                        Target = instance1,
                        TestID = instanceIDPrefix + 1,
                        ExpectedResult = false,
                        ExpectedMessage = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml),
                    });

            return testCases;
        }

        #endregion
    }

    public class BaseConstructorArg
    {
        public BaseConstructorArg(string Prop)
        {
            _propWithCtorArg = Prop;
        }

        private string _propWithCtorArg;

        [ConstructorArgument("Prop")]
        public virtual string PropWithCtorArg { get { return _propWithCtorArg; } }
    }

    public class DerivedConstructorArg : BaseConstructorArg
    {
        public DerivedConstructorArg(string Prop) : base(Prop) { }

        public override string PropWithCtorArg { get { return base.PropWithCtorArg; } }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            var instance1 = new DerivedConstructorArg("some value");
            testCases.Add(
                new TestCaseInfo
                {
                    Target = instance1,
                    TestID = instanceIDPrefix + 1,
                });

            return testCases;
        }

        #endregion
    }
}
