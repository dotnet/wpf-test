// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.ContentProperties
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Markup;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    
    [ContentProperty("ContentProperty")]
    public class StringContentProperty
    {
        public string ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            StringContentProperty target = new StringContentProperty { ContentProperty = "blah" };
           
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "StringContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:StringContentProperty[.='" + target.ContentProperty + "']"
                    }
                },
            };

            return testCases;
        }
        #endregion
    }

    public class ShadowedContentProperty : StringContentProperty
    {
        public new string ContentProperty { get; set; }

        #region Test Implementation
        public new static List<TestCaseInfo> GetTestCases()
        {
            ShadowedContentProperty target = new ShadowedContentProperty { ContentProperty = "blah" };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "ShadowedContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:ShadowedContentProperty[.='" + target.ContentProperty + "']"
                        
                    }
                }
            };

            return testCases;
        }
        #endregion
    }

    [ContentProperty("ContentProperty")]
    public class StringContentPropertyWClass
    {
        public string ContentProperty { get; set; }
        public ClassType2 ClassProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            StringContentPropertyWClass target = new StringContentPropertyWClass
            {
                ContentProperty = "blah",
                ClassProperty = new ClassType2
                {
                    Category = new ClassType1
                    {
                        Category = "category"
                    }
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "StringContentPropertyWClass" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:StringContentPropertyWClass[.='" + target.ContentProperty + "']"
                        
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class BooleanContentProperty
    {
        public bool ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            BooleanContentProperty target = new BooleanContentProperty { ContentProperty = true };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "BooleanContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:BooleanContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class Int8ContentProperty
    {
        public sbyte ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            Int8ContentProperty target = new Int8ContentProperty { ContentProperty = sbyte.MinValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "Int8ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:Int8ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class Int16ContentProperty
    {
        public short ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            Int16ContentProperty target = new Int16ContentProperty { ContentProperty = short.MaxValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "Int16ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:Int16ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class Int32ContentProperty
    {
        public int ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            Int32ContentProperty target = new Int32ContentProperty { ContentProperty = int.MaxValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "Int32ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:Int32ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class Int64ContentProperty
    {
        public long ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            Int64ContentProperty target = new Int64ContentProperty { ContentProperty = long.MinValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "Int64ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:Int64ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class UInt8ContentProperty
    {
        public byte ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            UInt8ContentProperty target = new UInt8ContentProperty { ContentProperty = byte.MaxValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "UInt8ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:UInt8ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class UInt16ContentProperty
    {
        public ushort ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            UInt16ContentProperty target = new UInt16ContentProperty { ContentProperty = ushort.MinValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "UInt16ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:UInt16ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class UInt32ContentProperty
    {
        public uint ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            UInt32ContentProperty target = new UInt32ContentProperty { ContentProperty = uint.MaxValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "UInt32ContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:UInt32ContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class UInt64ConentProperty
    {
        public ulong ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            UInt64ConentProperty target = new UInt64ConentProperty { ContentProperty = ulong.MinValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "UInt64ConentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:UInt64ConentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class SingleContentProperty
    {
        public float ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            SingleContentProperty target = new SingleContentProperty { ContentProperty = float.NegativeInfinity };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "SingleContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:SingleContentProperty[@ContentProperty='" + target.ContentProperty.ToString(CultureInfo.InvariantCulture) + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class DoubleContentProperty
    {
        public double ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            DoubleContentProperty target = new DoubleContentProperty { ContentProperty = double.NaN };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "DoubleContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:DoubleContentProperty[@ContentProperty='" + target.ContentProperty.ToString(CultureInfo.InvariantCulture) + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class DecimalContentProperty
    {
        public decimal ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            DecimalContentProperty target = new DecimalContentProperty { ContentProperty = decimal.MaxValue };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "DecimalContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:DecimalContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class DateTimeConentProperty
    {
        public DateTime ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            DateTimeConentProperty target = new DateTimeConentProperty { ContentProperty = new DateTime(1999, 10, 3, 4, 5, 16, DateTimeKind.Unspecified) };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "DateTimeConentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:DateTimeConentProperty[@ContentProperty='" + "1999-10-03T04:05:16" + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class DateTimeOffsetContentProperty
    {
        public DateTimeOffset ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            DateTimeOffset value = new DateTime(1999, 9, 3, 4, 5, 16, DateTimeKind.Unspecified);
            DateTimeOffsetContentProperty target = new DateTimeOffsetContentProperty { ContentProperty = value };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "DateTimeOffsetContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:DateTimeOffsetContentProperty[@ContentProperty='" + value.ToString("O", CultureInfo.InvariantCulture) + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class CharContentProperty
    {
        public char ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            CharContentProperty target = new CharContentProperty { ContentProperty = 'a' };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "CharContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:CharContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class EnumContentProperty
    {
        public TestEnum ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            EnumContentProperty target = new EnumContentProperty { ContentProperty = TestEnum.Value2 };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "EnumContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:EnumContentProperty[@ContentProperty='" + target.ContentProperty + "']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class StructContentProperty
    {
        public ClassType1 ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            StructContentProperty target = new StructContentProperty { ContentProperty = new ClassType1 { Category = "category" } };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "StructContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:StructContentProperty/mtxt:ClassType1[@Category='category']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class EmptyStructContentProperty
    {
        public ClassType4 ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            EmptyStructContentProperty target = new EmptyStructContentProperty { ContentProperty = new ClassType4() };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "EmptyStructContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:EmptyStructContentProperty/mtxt:ClassType4"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class ClassContentProperty
    {
        public ClassType2 ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            ClassContentProperty target = new ClassContentProperty
            {
                ContentProperty = new ClassType2
                {
                    Category = new ClassType1
                    {
                        Category = "category"
                    }
                }
            };

            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "ClassContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:ClassContentProperty/mtxt:ClassType2/mtxt:ClassType2.Category/mtxt:ClassType1[@Category='category']"
                        
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class EmptyClassContentProperty
    {
        public ClassType3 ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            EmptyClassContentProperty target = new EmptyClassContentProperty { ContentProperty = new ClassType3() };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "EmptyClassContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:EmptyClassContentProperty/mtxt:ClassType3"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class TypeConverterContentProperty
    {
        public AnimalList ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            TypeConverterContentProperty target = new TypeConverterContentProperty
            {
                ContentProperty = new AnimalList
                {
                    new Animal{ Name = "Cassowary", Number = int.MaxValue},
                    new Animal{ Name = "Emu", Number = int.MinValue},
                    new Animal{ Name = "Ostrich", Number = 0}
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                // [DISABLED] : Known Bug / Test Case Hang ?
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "TypeConverterContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:TypeConverterContentProperty[.='" + "Cassowary:2147483647#Emu:-2147483648#Ostrich:0" + "']"
                    },
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class GenericContentProperty<T>
    {
        public T ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            GenericContentProperty<string> target = new GenericContentProperty<string> { ContentProperty = "some value" };
            GenericContentProperty<ClassType2> target2 = new GenericContentProperty<ClassType2>
            {
                ContentProperty = new ClassType2
                {
                    Category = new ClassType1
                    {
                        Category = "category"
                    }
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "GenericContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:GenericContentProperty[.='" + target.ContentProperty + "']"
                    }
                },
                new TestCaseInfo
                {
                    Target = target2,
                    TestID = "GenericContentProperty" + 1,
                    XPathExpresions = 
                    {
                        "/mtxtc:GenericContentProperty/mtxt:ClassType2/mtxt:ClassType2.Category/mtxt:ClassType1[@Category='category']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class PrimitiveArraryContentProperty
    {
        public string[] ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            PrimitiveArraryContentProperty target = new PrimitiveArraryContentProperty();
            target.ContentProperty = new string[3];
            target.ContentProperty[0] = "string1";
            target.ContentProperty[1] = null;
            target.ContentProperty[2] = "string2";

            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "PrimitiveArraryContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:PrimitiveArraryContentProperty/x:Array[xasl:String='string1']",
                        "/mtxtc:PrimitiveArraryContentProperty/x:Array/x:Null",
                        "/mtxtc:PrimitiveArraryContentProperty/x:Array[xasl:String='string2']"
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class ClassArrayContentProperty
    {
        public ClassType2[] ContentProperty { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            ClassArrayContentProperty target = new ClassArrayContentProperty();

            target.ContentProperty = new ClassType2[4];
            target.ContentProperty[0] = new ClassType2
            {
                Category = new ClassType1
                {
                    Category = "blah"
                }
            };
            target.ContentProperty[1] = null;
            target.ContentProperty[2] = new ClassType2();
            target.ContentProperty[3] = new ClassType2
            {
                Category = new ClassType1
                {
                    Category = "category"
                }
            };

            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "ClassArrayContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:ClassArrayContentProperty/x:Array/mtxt:ClassType2/mtxt:ClassType2.Category/mtxt:ClassType1[@Category='blah']",
                        "/mtxtc:ClassArrayContentProperty/x:Array/x:Null",
                        "/mtxtc:ClassArrayContentProperty/x:Array/mtxt:ClassType2/mtxt:ClassType2.Category/mtxt:ClassType1[@Category='{x:Null}']"
                    },
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class ArrayListContentProperty
    {
        ArrayList _contentProperty = new ArrayList();
        public ArrayList ContentProperty { get { return _contentProperty; } set { _contentProperty = value; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            ArrayListContentProperty target = new ArrayListContentProperty
            {
                ContentProperty =
                {
                    "blah",
                    null,
                    3.14,
                    new ClassType3()
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "ArrayListContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:ArrayListContentProperty/sc:ArrayList[xasl:String='blah']",
                        "/mtxtc:ArrayListContentProperty/sc:ArrayList/x:Null",
                        "/mtxtc:ArrayListContentProperty/sc:ArrayList[xasl:Double='3.14']",
                        "/mtxtc:ArrayListContentProperty/sc:ArrayList/mtxt:ClassType3",
                    },

                    XPathNamespacePrefixMap =
                    {
                        {"sc", "clr-namespace:System.Collections;assembly=mscorlib"}
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class HashtableContentProperty
    {
        Hashtable _contentProperty = new Hashtable();
        public Hashtable ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            HashtableContentProperty target = new HashtableContentProperty
            {
                ContentProperty =
                {
                    { "key1", "value1"},
                    {23, null}
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "HashtableContentProperty" + 0,
                    /*XPathExpresions = 
                    {
                        "/mtxtc:HashtableContentProperty/xasl:String[@x:Key='key1']",
                        "/mtxtc:HashtableContentProperty[xasl:String='value1']",
                        "/mtxtc:HashtableContentProperty/x:Null/x:Key[xasl:Int32='23']",
                    },*/
                    XPathNamespacePrefixMap = 
                    {
                        { "s" , "clr-namespace:System;assembly=mscorlib"}
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class IListContentProperty
    {
        public ArrayList contentProperty = new ArrayList();
        public IList ContentProperty { get { return contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            IListContentProperty target = new IListContentProperty
            {
                ContentProperty =
                {
                    3.14,
                    new ClassType_Inheritance17(),
                    null
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "IListContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:IListContentProperty[xasl:Double='3.14']",
                        "/mtxtc:IListContentProperty/mtxt:ClassType_Inheritance17",
                        "/mtxtc:IListContentProperty/x:Null",
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class IListTContentProperty
    {
        List<string> _contentProperty = new List<string>();
        public IList<string> ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            IListTContentProperty target = new IListTContentProperty
            {
                ContentProperty =
                {
                    "string1",
                    null,
                    "string2"
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "IListTContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:IListTContentProperty[xasl:String='string1']",
                        "/mtxtc:IListTContentProperty/x:Null",
                        "/mtxtc:IListTContentProperty[xasl:String='string2']",
                    }
                }
            };

            return testCases;
        }
        #endregion
    }

    [ContentProperty("ContentProperty")]
    public class ListTContentProperty
    {
        List<string> _contentProperty = new List<string>();
        public List<string> ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            ListTContentProperty target = new ListTContentProperty
            {
                ContentProperty =
                {
                    "string1",
                    null,
                    "string2"
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "ListTContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:ListTContentProperty[xasl:String='string1']",
                        "/mtxtc:ListTContentProperty/x:Null",
                        "/mtxtc:ListTContentProperty[xasl:String='string2']",
                    }
                }
            };

            return testCases;
        }
        #endregion
    }


    [ContentProperty("ContentProperty")]
    public class IDictionaryContentProperty
    {
        IDictionary _contentProperty = new Dictionary<object, object>();
        public IDictionary ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            IDictionaryContentProperty target = new IDictionaryContentProperty
            {
                ContentProperty =
                {
                    { "key1", "value1"},
                    {23, null}
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "IDictionaryContentProperty" + 0,
                    /*XPathExpresions = 
                    {
                        "/mtxtc:IDictionaryContentProperty/xasl:String[@x:Key='key1']",
                        "/mtxtc:IDictionaryContentProperty[xasl:String='value1']",
                        "/mtxtc:IDictionaryContentProperty/x:Null/x:Key[xasl:Int32='23']",
                    },*/
                    XPathNamespacePrefixMap = 
                    {
                        { "s" , "clr-namespace:System;assembly=mscorlib"}
                    }
                }
            };

            return testCases;
        }
        #endregion
    }



    [ContentProperty("ContentProperty")]
    public class IDictionaryKVContentProperty
    {
        IDictionary<ClassType1, string> _contentProperty = new Dictionary<ClassType1, string>();

        public IDictionary<ClassType1, string> ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            IDictionaryKVContentProperty target = new IDictionaryKVContentProperty
            {
                ContentProperty =
                {
                    { new ClassType1 { Category = "category1"}, "string1"},
                    { new ClassType1 { Category = "category2"}, null}
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "IDictionaryKVContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:IDictionaryKVContentProperty[xasl:String='string1']",
                        "/mtxtc:IDictionaryKVContentProperty/xasl:String/x:Key/mtxt:ClassType1[@Category='category1']",
                        "/mtxtc:IDictionaryKVContentProperty/x:Null/x:Key/mtxt:ClassType1[@Category='category2']",
                    }
                }
            };

            return testCases;
        }
        #endregion
    }

    [ContentProperty("ContentProperty")]
    public class DictionaryKVContentProperty
    {
        Dictionary<ClassType1, string> _contentProperty = new Dictionary<ClassType1, string>();

        public Dictionary<ClassType1, string> ContentProperty { get { return _contentProperty; } }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            DictionaryKVContentProperty target = new DictionaryKVContentProperty
            {
                ContentProperty =
                {
                    { new ClassType1 { Category = "category1"}, "string1"},
                    { new ClassType1 { Category = "category2"}, null}
                }
            };
            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    TestID = "DictionaryKVContentProperty" + 0,
                    XPathExpresions = 
                    {
                        "/mtxtc:DictionaryKVContentProperty[xasl:String='string1']",
                        "/mtxtc:DictionaryKVContentProperty/xasl:String/x:Key/mtxt:ClassType1[@Category='category1']",
                        "/mtxtc:DictionaryKVContentProperty/x:Null/x:Key/mtxt:ClassType1[@Category='category2']",
                    }
                }
            };

            return testCases;
        }
        #endregion
    }

    static class XamlFirstContentPropertyHelper
    {
        public static IEnumerable<TestCaseInfo> GetTestCases(List<Type> types)
        {
            List<TestCaseInfo> unwrappedTestCases = XamlTestDriver.GetInstances(types);

            var wrappedTestCases =
                from unwrappedTestCase in unwrappedTestCases
                select (TestCaseInfo)new XamlFirstTestCaseInfo
                {
                    Target = GetXamlStringFromObject(unwrappedTestCase.Target),
                    TestID = GetIdFromWrappedId(unwrappedTestCase.TestID),
                    InspectMethod = (target) => XamlObjectComparer.CompareObjects(target, unwrappedTestCase.Target)
                };


            return wrappedTestCases.ToList();
        }

        private static string GetIdFromWrappedId(string id)
        {
            return id;
        }

        private static string GetXamlStringFromObject(object obj)
        {
            NodeList nodes = NodeListTransforms.ObjectToNodeList(obj);
            nodes.Insert(0, new NamespaceNode("x", Namespaces.NamespaceV2));
            nodes.Insert(0, new NamespaceNode("mtxt", "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes"));
            return NodeListTransforms.NodeListToXml(nodes);
        }
    }

    public class WrappedPrimitiveContentProperties
    {
        #region Test Implementation
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            IEnumerable<TestCaseInfo> testCases = XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(StringContentProperty),
                    typeof(StringContentPropertyWClass),
                    typeof(BooleanContentProperty),
                    typeof(Int8ContentProperty),
                    typeof(Int16ContentProperty),
                    typeof(Int32ContentProperty),
                    typeof(Int64ContentProperty),
                    typeof(UInt8ContentProperty),
                    typeof(UInt16ContentProperty),
                    typeof(UInt32ContentProperty),
                    typeof(UInt64ConentProperty),
                    typeof(SingleContentProperty),
                    typeof(DoubleContentProperty),
                    typeof(DecimalContentProperty),
                    typeof(DateTimeConentProperty),
                    typeof(DateTimeOffsetContentProperty),
                    typeof(CharContentProperty),
                    typeof(EnumContentProperty)
                }
                );


            return testCases;
        }
        #endregion
    }

    public class WrappedClassContentProperties
    {
        #region Test Implementation
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            return XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(ClassContentProperty),
                    typeof(EmptyClassContentProperty),
                    typeof(StructContentProperty),
                    typeof(EmptyStructContentProperty)
                }
                );
        }
        #endregion
    }

    public class WrappedTypeConverterContentProperties
    {
        #region Test Implementation
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            IEnumerable<TestCaseInfo> testCases = XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(TypeConverterContentProperty),
                }
                );

            return testCases;
        }
        #endregion
    }

    public class WrappedGenericContentProperties
    {
        #region Test Implementation
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            IEnumerable<TestCaseInfo> testCases = XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(GenericContentProperty<string>),
                }
                );
            return testCases;
        }
        #endregion
    }

    public class WrappedCollectionContentProperties
    {
        #region Test Implementation
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            IEnumerable<TestCaseInfo> nonDictionaryTestCases = XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(PrimitiveArraryContentProperty),
                    typeof(ArrayListContentProperty),
                    typeof(IListContentProperty),
                    typeof(IListTContentProperty),
                    typeof(ListTContentProperty),
                }
                );

            testCases.AddRange(nonDictionaryTestCases);

            IEnumerable<TestCaseInfo> contentTests = XamlFirstContentPropertyHelper.GetTestCases(
                new List<Type>
                {
                    typeof(ClassArrayContentProperty),
                });
            foreach (TestCaseInfo test in contentTests)
            {
                string xaml = test.Target as string;
                test.Target = xaml.Replace("p:ClassType2", "mtxt:ClassType2");
            }
            testCases.AddRange(contentTests);

            testCases.Add(GetTestCaseInfoFromXamlDoc(s_IDictionaryKVDoc, typeof(IDictionaryKVContentProperty)));
            testCases.Add(GetTestCaseInfoFromXamlDoc(s_hashtableDoc, typeof(HashtableContentProperty)));
            testCases.Add(GetTestCaseInfoFromXamlDoc(s_dictionaryKVDoc, typeof(DictionaryKVContentProperty)));
            testCases.Add(GetTestCaseInfoFromXamlDoc(s_IDictionaryDoc, typeof(IDictionaryContentProperty)));

            return testCases;
        }

        static TestCaseInfo GetTestCaseInfoFromXamlDoc(NodeList doc, Type unwrappedType)
        {
            TestCaseInfo unwrappedInfo = XamlTestDriver.GetInstances(new List<Type> { unwrappedType })[0];
            XamlFirstTestCaseInfo wrappedInfo = new XamlFirstTestCaseInfo
            {
                Target = doc.NodeListToXml(),
                TestID = unwrappedInfo.TestID,
                InspectMethod = (target) => XamlObjectComparer.CompareObjects(target, unwrappedInfo.Target)
            };

            return wrappedInfo;
        }
        const string localAssemblyNamespace = "clr-namespace:Microsoft.Test.Xaml.Types.ContentProperties;assembly=XamlClrTypes";
        const string classTypeNamespace = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";

        #region IDictionaryKVDoc document
        static NodeList s_IDictionaryKVDoc = new NodeList
        {
            new StartObject(typeof(IDictionaryKVContentProperty)),
                new StartMember(typeof(IDictionaryKVContentProperty), "ContentProperty"),
                    new NamespaceNode("x2", Namespaces.NamespaceV2),
                    new StartObject(XamlLanguage.String),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(typeof(ClassType1)),
                                new StartMember(typeof(ClassType1), "Category"),
                                    new ValueNode("category1"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),
                        new StartMember(XamlLanguage.Initialization),
                            new ValueNode("string1"),
                        new EndMember(),
                    new EndObject(),
                    new StartObject(XamlLanguage.Null),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(typeof(ClassType1)),
                                new StartMember(typeof(ClassType1), "Category"),
                                    new ValueNode("category2"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),                      
                    new EndObject(),
                new EndMember(),
            new EndObject(),
        };

        #endregion

        #region DictionaryKVDoc document
        static NodeList s_dictionaryKVDoc = new NodeList
        {
            new StartObject(typeof(DictionaryKVContentProperty)),
                new StartMember(typeof(DictionaryKVContentProperty), "ContentProperty"),
                    new NamespaceNode("x2", Namespaces.NamespaceV2),
                    new StartObject(XamlLanguage.String),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(typeof(ClassType1)),
                                new StartMember(typeof(ClassType1), "Category"),
                                    new ValueNode("category1"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),
                        new StartMember(XamlLanguage.Initialization),
                            new ValueNode("string1"),
                        new EndMember(),
                    new EndObject(),
                    new StartObject(XamlLanguage.Null),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(typeof(ClassType1)),
                                new StartMember(typeof(ClassType1), "Category"),
                                    new ValueNode("category2"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),                      
                    new EndObject(),
                new EndMember(),
            new EndObject(),
        };

        #endregion

        #region Hashtable document

        static NodeList s_hashtableDoc = new NodeList
        {
            new NamespaceNode("x2", Namespaces.NamespaceV2),
            new StartObject(typeof(HashtableContentProperty)),
                new StartMember(typeof(HashtableContentProperty), "ContentProperty"),
                    new NamespaceNode("x2", Namespaces.NamespaceV2),
                    new StartObject(XamlLanguage.String),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(XamlLanguage.String),
                                new StartMember(XamlLanguage.Initialization),
                                    new ValueNode("key1"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),
                        new StartMember(XamlLanguage.Initialization),
                            new ValueNode("value1"),
                        new EndMember(),
                    new EndObject(),
                    new StartObject(XamlLanguage.Null),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(XamlLanguage.Int32),
                                new StartMember(XamlLanguage.Initialization),
                                    new ValueNode(23),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),                      
                    new EndObject(),
                new EndMember(),
            new EndObject(),
        };

        #endregion

        #region IDictionary document
        static NodeList s_IDictionaryDoc = new NodeList
        {
            new NamespaceNode("x2", Namespaces.NamespaceV2),
            new StartObject(typeof(IDictionaryContentProperty)),
                new StartMember(typeof(IDictionaryContentProperty), "ContentProperty"),
                    new NamespaceNode("x2", Namespaces.NamespaceV2),
                    new StartObject(XamlLanguage.String),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(XamlLanguage.String),
                                new StartMember(XamlLanguage.Initialization),
                                    new ValueNode("key1"),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),
                        new StartMember(XamlLanguage.Initialization),
                            new ValueNode("value1"),
                        new EndMember(),
                    new EndObject(),
                    new StartObject(XamlLanguage.Null),
                        new StartMember(XamlLanguage.Key),
                            new StartObject(XamlLanguage.Int32),
                                new StartMember(XamlLanguage.Initialization),
                                    new ValueNode(23),
                                new EndMember(),
                            new EndObject(),
                        new EndMember(),                      
                    new EndObject(),
                new EndMember(),
            new EndObject(),
        };
        #endregion


        #endregion
    }

    public class SomeTypeWithoutAContentProperty
    {
        public string Stuff { get; set; }
    }

    public class UselessConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProperty("Element")]
    public class UselessConverterContainer
    {
        [TypeConverter(typeof(UselessConverter))]
        public Element Element { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            UselessConverterContainer instance1 = new UselessConverterContainer()
            {
                Element = new Element(),
            };

            // Element contains TypeConverter that does not convert to string
            // Regression test 
            testCases.Add(new TestCaseInfo
            {
                Target = instance1,
                TestID = instanceIDPrefix + 1,
                XPathExpresions = 
                {
                    "/z:UselessConverterContainer/z:Element"   
                },
                XPathNamespacePrefixMap = 
                {
                   {"z", "clr-namespace:Microsoft.Test.Xaml.Types.ContentProperties;assembly=XamlClrTypes" }, 
                }
            });

            return testCases;
        }

        #endregion
    }

    public class Element { }

}
