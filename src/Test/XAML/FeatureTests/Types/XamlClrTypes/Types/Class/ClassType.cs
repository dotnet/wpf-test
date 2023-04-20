// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Driver;
//[assembly: System.Windows.Markup.XmlnsDefinition("http://rootnamespace", "")]

public class TypeInRootNamespace
{
    public string Data { get; set; }

    #region Test Implementation

    public static System.Collections.Generic.List<TestCaseInfo> GetTestCases()
    {
        string instanceIDPrefix = 
            XamlTestDriver.GetInstanceIDPrefix(System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType);
        var testCases = new System.Collections.Generic.List<TestCaseInfo>();

        testCases.Add(new TestCaseInfo
        {
            Target = new TypeInRootNamespace { Data = "some string" },
            TestID = instanceIDPrefix + 1,
            XPathExpresions =
            {
                "/r:TypeInRootNamespace"
            },
            XPathNamespacePrefixMap = 
            {
                { "r" , "clr-namespace:;assembly=XamlClrTypes"}
            }
        });

        return testCases;
    }

    #endregion
}


namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Types;

    public class ClassType
    {
        private ClassType2[] _array = new ClassType2[3];

        public object Array
        {
            get
            {
                return this._array;
            }
            set
            {
                if (value is ClassType2[])
                {
                    _array = value as ClassType2[];
                }
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // 
            ClassType instance1 = new ClassType();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    public struct ClassType1
    {
        private string _category;

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

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            // property field initinalized to null, 
            ClassType1 instance1 = new ClassType1();
            instance1.Category = null;
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    public class ClassType2
    {
        private ClassType1 _category;

        public ClassType1 Category
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

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType2 instance1 = new ClassType2();
            instance1.Category = new ClassType1();
            instance1._category.Category = "";
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    //empty class
    public class ClassType3
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType3 instance1 = new ClassType3();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    //empty struct
    public struct ClassType4
    {
        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType4 instance1 = new ClassType4();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    // mix
    public class ClassType5
    {
        private ClassType1 _field1;
        private ClassType2 _field2;
        private ClassType3 _field3;
        private ClassType4 _field4;

        public ClassType5()
        {
            this._field1 = new ClassType1();
            this._field1.Category = "Next Gen";

            this._field2 = new ClassType2();
            this._field2.Category = new ClassType1();
            //this.field2.Category.Category = "multiple/r/nlines";

            this._field3 = new ClassType3();
            this._field4 = new ClassType4();
        }

        public ClassType1 Field1
        {
            get
            {
                return _field1;
            }
            set
            {
                _field1 = value;
            }
        }

        public ClassType2 Field2
        {
            get
            {
                return _field2;
            }
            set
            {
                _field2 = value;
            }
        }

        public ClassType3 Field3
        {
            get
            {
                return _field3;
            }
            set
            {
                _field3 = value;
            }
        }

        public ClassType4 Field4
        {
            get
            {
                return _field4;
            }
            set
            {
                _field4 = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType5 instance1 = new ClassType5();
            instance1._field1 = new ClassType1();
            instance1._field1.Category = "<Category>";
            instance1._field2 = new ClassType2();
            instance1._field2.Category = new ClassType1();
            instance1._field3 = new ClassType3();
            instance1._field4 = new ClassType4();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }

        #endregion
    }

    //static members in a class (won't be serialized)
    public class ClassType7
    {
        public static string Data { get; set; }
        public static ItemType Item { get; set; }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            ClassType7.Data = "some data";
            ClassType7.Item = new ItemType
                                  {
                                      ItemName = "My item",
                                      Price = 3.14
                                  };
            ClassType7 instance1 = new ClassType7();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            return testCases;
        }
    }

    // holder for special framework types
    public class ClassType8
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>()
            {
                new TestCaseInfo
                {
                    Target = new PropertyDefinition
                    {
                        Attributes = 
                        {
                            new TestAttribute()
                        },
                        Modifier = "public",
                        Name = "Name"
                    },
                    TestID = instanceIDPrefix + 1
                }
            };
        }
    }

    #region Composed classes

    public class Composition0
    {
        public string Data { get; set; }
        public ItemType Item { get; set; }

        public Composition0()
        {
            Data = "Some data";
            Item = new ItemType
                       {
                           ItemName = "My item",
                           Price = 3.14
                       };
        }
    }

    public class Composition1
    {
        public Composition0 Inner { get; set; }

        public Composition1()
        {
            Inner = new Composition0();
        }
    }

    public class Composition2
    {
        public Composition1 Inner { get; set; }

        public Composition2()
        {
            Inner = new Composition1();
        }
    }

    public class Composition3
    {
        public Composition2 Inner { get; set; }

        public Composition3()
        {
            Inner = new Composition2();
        }
    }

    public class Composition4
    {
        public Composition3 Inner { get; set; }

        public Composition4()
        {
            Inner = new Composition3();
        }
    }

    public class Composition5
    {
        public Composition4 Inner { get; set; }

        public Composition5()
        {
            Inner = new Composition4();
        }
    }

    public class Composition6
    {
        public Composition5 Inner { get; set; }

        public Composition6()
        {
            Inner = new Composition5();
        }
    }

    public class Composition7
    {
        public Composition6 Inner { get; set; }

        public Composition7()
        {
            Inner = new Composition6();
        }
    }

    public class Composition8
    {
        public Composition7 Inner { get; set; }

        public Composition8()
        {
            Inner = new Composition7();
        }
    }

    public class Composition9
    {
        public Composition8 Inner { get; set; }

        public Composition9()
        {
            Inner = new Composition8();
        }
    }

    public class Composition10
    {
        public Composition9 Inner { get; set; }

        public Composition10()
        {
            Inner = new Composition9();
        }

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            List<Type> types = new List<Type>
                                   {
                                       typeof (Composition0),
                                       typeof (Composition1),
                                       typeof (Composition2),
                                       typeof (Composition3),
                                       typeof (Composition4),
                                       typeof (Composition5),
                                       typeof (Composition6),
                                       typeof (Composition7),
                                       typeof (Composition8),
                                       typeof (Composition9),
                                       typeof (Composition10),
                                   };

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type);
                testCases.Add(new TestCaseInfo
                                  {
                                      Target = instance,
                                      TestID = instanceIDPrefix + type.Name
                                  });
            }

            return testCases;
        }
    }

    #endregion

    // Type that does not implement ICollection - but schematype is a collection 
    // because GetEnumerator and Add are implemented 
    public class Bar
    {
        private List<int> MyProperty { get; set; }

        public Bar()
        {
            MyProperty = new List<int>();
        }

        public IEnumerator GetEnumerator()
        {
            return MyProperty.GetEnumerator();
        }

        public void Add(int obj)
        {
            MyProperty.Add(obj);
        }
    }

    // Type does not implement IDictionary but implementing the
    // Add(T,V) , GetEnumerator() and TryGetValue makes it a 
    // dictionary 
    public class MyDictionary2<T, V>
    {
        private readonly Dictionary<T, V> _dict = new Dictionary<T, V>();

        //public void Add(T a, V b)
        //{
        //    dict.Add(a, b);
        //}

        public void Add(object a, object b)
        {
            _dict.Add((T)a, (V)b);
        }

        public IEnumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool TryGetValue(T key, out V value)
        {
            return _dict.TryGetValue(key, out value);
        }
    }

    public class ShouldSerializeSample
    {
        public String StringProperty { get; set; }

        public String StringProperty1 { get; set; }

        public bool ShouldSerializeStringProperty()
        {
            return true;
        }

        public bool ShouldSerializeStringProperty1()
        {
            return false;
        }
    }

    public class ShouldSerializeArray
    {
        public string[] StringArrayProperty { get; set; }

        public bool ShouldSerializeStringArrayProperty()
        {
            return false;
        }
    }

    public class ShouldSerializeDictionary
    {
        public Dictionary<string,string> DictionaryProperty { get; set; }

        public bool ShouldSerializeDictionaryProperty()
        {
            return false;
        }
    }

    public class ShouldSerializeList
    {
        public List<string> StringListProperty { get; set; }

        public bool ShouldSerializeStringListProperty()
        {
            return false;
        }
    }

    public class ObjectContainer
    {
        public object Content { get; set; }
    }

    public class TestAttribute : Attribute
    {
    }

    public class WriteOnlyPropertyClass
    {
        private string _data;
        public string Data { set { _data = value; } }

        public string GetData()
        {
            return _data;
        }
    }
}
