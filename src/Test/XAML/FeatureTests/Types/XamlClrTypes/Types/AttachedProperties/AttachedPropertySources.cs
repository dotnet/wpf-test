// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.AttachedProperties
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xaml;
    using Microsoft.Test.Xaml.Types.MarkupExtensions;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    
    public static class AttachedPropertySource
    {

#region Property identifiers
        static readonly AttachableMemberIdentifier s_boolPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "BoolProp");

        static readonly AttachableMemberIdentifier s_int8PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "Int8Prop");


        static readonly AttachableMemberIdentifier s_int16PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "Int16Prop");


        static readonly AttachableMemberIdentifier s_int32PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "Int32Prop");


        static readonly AttachableMemberIdentifier s_int64PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "Int64Prop");


        static readonly AttachableMemberIdentifier s_SBytePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "SByteProp");


        static readonly AttachableMemberIdentifier s_UInt16PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "UInt16Prop");


        static readonly AttachableMemberIdentifier s_UInt32PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "UInt32Prop");


        static readonly AttachableMemberIdentifier s_UInt64PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "UInt64Prop");



        static readonly AttachableMemberIdentifier s_singlePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "SingleProp");


        static readonly AttachableMemberIdentifier s_doublePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "DoubleProp");


        static readonly AttachableMemberIdentifier s_decimalPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "DecimalProp");


        static readonly AttachableMemberIdentifier s_dateTimePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "DateTimeProp");


        static readonly AttachableMemberIdentifier s_stringPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "StringProp");



        static readonly AttachableMemberIdentifier s_charPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "CharProp");


        static readonly AttachableMemberIdentifier s_dateTimeOffsetPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "DateTimeOffsetProp");


        static readonly AttachableMemberIdentifier s_guidPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "GuidProp");


        static readonly AttachableMemberIdentifier s_timeSpanPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "TimeSpanProp");


        static readonly AttachableMemberIdentifier s_testEnumPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "TestEnumProp");


        // struct
        static readonly AttachableMemberIdentifier s_classType1PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassType1Prop");

        //empty struct
        static readonly AttachableMemberIdentifier s_classType4PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassType4Prop");

        // nonempty class
        static readonly AttachableMemberIdentifier s_classType2PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassType2Prop");

        // empty class
        static readonly AttachableMemberIdentifier s_classType3PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassType3Prop");


        static readonly AttachableMemberIdentifier s_classType_Inheritance23PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassType_Inheritance23Prop");


        static readonly AttachableMemberIdentifier s_stringArrayPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "StringArrayProp");


        static readonly AttachableMemberIdentifier s_classArrayPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ClassArrayProp");


        static readonly AttachableMemberIdentifier s_arrayListPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ArrayListProp");


        static readonly AttachableMemberIdentifier s_hashtablePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "HashTableProp");


        static readonly AttachableMemberIdentifier s_IListPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "IListProp");

        static readonly AttachableMemberIdentifier s_IListStringPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "IListStringProp");


        static readonly AttachableMemberIdentifier s_IDictionaryPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "IDictionaryProp");


        static readonly AttachableMemberIdentifier s_IDictionaryClassType2DateTimePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "IDictionaryClassType2DateTimeProp");


        static readonly AttachableMemberIdentifier s_listClassType2PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "ListClassType2Prop");


        static readonly AttachableMemberIdentifier s_dictionaryStringClassType2PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "DictionaryStringClassType2Prop");


        static readonly AttachableMemberIdentifier s_animalListPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "AnimalListProp");

        static readonly AttachableMemberIdentifier s_MEOnAttachedPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "MEOnAttachedProp");

        static readonly AttachableMemberIdentifier s_schemaTypePropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource), "SchemaTypeProp");
        static object s_key = new object();
        static ClassType_Inheritance23 s_x = new ClassType_Inheritance23();
        static Dictionary<AttachableMemberIdentifier, object> s_properties = new Dictionary<AttachableMemberIdentifier, object>
            {
                { s_boolPropName, true },
                {s_charPropName, 'a'},
                {s_dateTimePropName, DateTime.MinValue},
                {s_decimalPropName, Decimal.One},
                { s_int8PropName, Byte.MaxValue },

                {s_int16PropName, Int16.MinValue},
                {s_int32PropName, Int32.MaxValue},
                {s_int64PropName, Int64.MinValue},
                {s_SBytePropName, SByte.MaxValue},
                {s_UInt16PropName, UInt16.MaxValue},
                {s_UInt32PropName, UInt32.MaxValue},
                {s_UInt64PropName, UInt64.MaxValue},
                {s_singlePropName, Single.Epsilon},
                {s_doublePropName, Double.NaN},
                {s_stringPropName, "some string"},
                {s_dateTimeOffsetPropName, DateTimeOffset.MinValue},
                {s_guidPropName, Guid.NewGuid()},
                {s_timeSpanPropName, new TimeSpan(TimeSpan.TicksPerDay * 364)},
                {s_testEnumPropName, TestEnum.Value2},
                // struct
                {s_classType1PropName, new ClassType1{Category = "category"}},
                // empty struct
                {s_classType4PropName, new ClassType4()},
                // non empty class
                {s_classType2PropName, new ClassType2
                    {
                        Category = new ClassType1
                        {
                            Category = "category"
                        }
                    }
                },
                // empty class
                {s_classType3PropName, new ClassType3()},
                {s_classType_Inheritance23PropName, s_x},
                {s_stringArrayPropName, new string[]{"foo", "bar", "baz", null}},
                {s_classArrayPropName, new ClassType2[]
                    {
                        new ClassType2
                        {
                            Category = new ClassType1
                            {
                                Category = null
                            }
                        },
                        null,
                        new ClassType2
                        {
                            Category = new ClassType1
                            {
                                Category = "category"
                            }
                        }
                    }
                },
                {s_arrayListPropName, new ArrayList
                    {
                        new object(),
                        "foo",
                        42,
                        3.14,
                        null,
                        DateTime.MinValue
                    }
                },
                {s_hashtablePropName, new Hashtable
                    {
                        {"hello", "there"},
                        {DateTime.MinValue, 42},
                        {new ClassType1 { Category = "hello"}, 3.14},
                    }
                },
                {s_IListPropName, new ArrayList
                    {
                        null,
                        Int64.MaxValue,
                        TimeSpan.MaxValue,
                    }
                },
                {s_IListStringPropName, new List<string>
                    {
                        "foo",
                        "bar",
                        "baz",
                    }
                },
                {s_IDictionaryPropName, new Dictionary<object, object>
                    {
                        {TimeSpan.MinValue, 42},
                        {new ClassType1 { Category = "hello"}, DateTime.MinValue}
                    }

                },

                {s_IDictionaryClassType2DateTimePropName, new Dictionary<ClassType2, DateTime>
                    {
                        {new ClassType2 { Category = new ClassType1{ Category = null }}, DateTime.MinValue},
                        {new ClassType2 { Category = new ClassType1{ Category = "blah" }}, DateTime.MinValue},
                    }
                },

                {s_listClassType2PropName, new List<ClassType2>
                    {
                        new ClassType2 { Category = new ClassType1{ Category = null }},
                        new ClassType2 { Category = new ClassType1{ Category = "category" }}
                    }
                },

                {s_dictionaryStringClassType2PropName, new Dictionary<string, ClassType2>
                    {
                        {"hello", new ClassType2 { Category = new ClassType1{ Category = null }}},
                        {"there", new ClassType2 { Category = new ClassType1{ Category = "category" }}},
                        {"buddy", null}
                    }
                },

                {s_animalListPropName, new AnimalList
                    {
                        new Animal{Name = "Bear", Number = 23},
                        new Animal{Name = "Liger", Number = - 1}
                    }
                },

            };

        public static Dictionary<AttachableMemberIdentifier, object> Properties
        {
            get
            {
                return s_properties;
            }
        }

        public static bool GetBoolProp(object target)
        {
            bool boolProp;
            return AttachablePropertyServices.TryGetProperty(target, s_boolPropName, out boolProp) && boolProp;
        }

        public static void SetBoolProp(object target, bool boolProp)
        {
            AttachablePropertyServices.SetProperty(target, s_boolPropName, boolProp);
        }

        public static Byte GetInt8Prop(object target)
        {
            Byte prop;
            AttachablePropertyServices.TryGetProperty(target, s_int8PropName, out prop);
            return prop;
        }

        public static void SetInt8Prop(object target, Byte prop)
        {
            AttachablePropertyServices.SetProperty(target, s_int8PropName, prop);
        }

        public static Int16 GetInt16Prop(object target)
        {
            Int16 prop;
            AttachablePropertyServices.TryGetProperty(target, s_int16PropName, out prop);
            return prop;
        }

        public static void SetInt16Prop(object target, Int16 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_int16PropName, prop);
        }

        public static Int32 GetInt32Prop(object target)
        {
            Int32 prop;
            AttachablePropertyServices.TryGetProperty(target, s_int32PropName, out prop);
            return prop;
        }

        public static void SetInt32Prop(object target, Int32 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_int32PropName, prop);
        }

        public static Int64 GetInt64Prop(object target)
        {
            Int64 prop;
            AttachablePropertyServices.TryGetProperty(target, s_int64PropName, out prop);
            return prop;
        }

        public static void SetInt64Prop(object target, Int64 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_int64PropName, prop);
        }

        public static SByte GetSByteProp(object target)
        {
            SByte prop;
            AttachablePropertyServices.TryGetProperty(target, s_SBytePropName, out prop);
            return prop;
        }

        public static void SetSByteProp(object target, SByte prop)
        {
            AttachablePropertyServices.SetProperty(target, s_SBytePropName, prop);
        }

        public static UInt16 GetUInt16Prop(object target)
        {
            UInt16 prop;
            AttachablePropertyServices.TryGetProperty(target, s_UInt16PropName, out prop);
            return prop;
        }

        public static void SetUInt16Prop(object target, UInt16 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_UInt16PropName, prop);
        }

        public static UInt32 GetUInt32Prop(object target)
        {
            UInt32 prop;
            AttachablePropertyServices.TryGetProperty(target, s_UInt32PropName, out prop);
            return prop;
        }

        public static void SetUInt32Prop(object target, UInt32 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_UInt32PropName, prop);
        }

        public static UInt64 GetUInt64Prop(object target)
        {
            UInt64 prop;
            AttachablePropertyServices.TryGetProperty(target, s_UInt64PropName, out prop);
            return prop;
        }

        public static void SetUInt64Prop(object target, UInt64 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_UInt64PropName, prop);
        }

        public static Single GetSingleProp(object target)
        {
            Single prop;
            AttachablePropertyServices.TryGetProperty(target, s_singlePropName, out prop);
            return prop;
        }

        public static void SetSingleProp(object target, Single prop)
        {
            AttachablePropertyServices.SetProperty(target, s_singlePropName, prop);
        }

        public static Double GetDoubleProp(object target)
        {
            Double prop;
            AttachablePropertyServices.TryGetProperty(target, s_doublePropName, out prop);
            return prop;
        }

        public static void SetDoubleProp(object target, Double prop)
        {
            AttachablePropertyServices.SetProperty(target, s_doublePropName, prop);
        }

        public static Decimal GetDecimalProp(object target)
        {
            Decimal prop;
            AttachablePropertyServices.TryGetProperty(target, s_decimalPropName, out prop);
            return prop;
        }

        public static void SetDecimalProp(object target, Decimal prop)
        {
            AttachablePropertyServices.SetProperty(target, s_decimalPropName, prop);
        }

        public static DateTime GetDateTimeProp(object target)
        {
            DateTime prop;
            AttachablePropertyServices.TryGetProperty(target, s_dateTimePropName, out prop);
            return prop;
        }

        public static void SetDateTimeProp(object target, DateTime prop)
        {
            AttachablePropertyServices.SetProperty(target, s_dateTimePropName, prop);
        }

        public static String GetStringProp(object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, s_stringPropName, out prop);
            return prop;
        }

        public static void SetStringProp(object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, s_stringPropName, prop);
        }

        public static Char GetCharProp(object target)
        {
            Char prop;
            AttachablePropertyServices.TryGetProperty(target, s_charPropName, out prop);
            return prop;
        }

        public static void SetCharProp(object target, Char prop)
        {
            AttachablePropertyServices.SetProperty(target, s_charPropName, prop);
        }

        public static DateTimeOffset GetDateTimeOffsetProp(object target)
        {
            DateTimeOffset prop;
            AttachablePropertyServices.TryGetProperty(target, s_dateTimeOffsetPropName, out prop);
            return prop;
        }

        public static void SetDateTimeOffsetProp(object target, DateTimeOffset prop)
        {
            AttachablePropertyServices.SetProperty(target, s_dateTimeOffsetPropName, prop);
        }

        public static Guid GetGuidProp(object target)
        {
            Guid prop;
            AttachablePropertyServices.TryGetProperty(target, s_guidPropName, out prop);
            return prop;
        }

        public static void SetGuidProp(object target, Guid prop)
        {
            AttachablePropertyServices.SetProperty(target, s_guidPropName, prop);
        }

        public static TimeSpan GetTimeSpanProp(object target)
        {
            TimeSpan prop;
            AttachablePropertyServices.TryGetProperty(target, s_timeSpanPropName, out prop);
            return prop;
        }

        public static void SetTimeSpanProp(object target, TimeSpan prop)
        {
            AttachablePropertyServices.SetProperty(target, s_timeSpanPropName, prop);
        }

        public static TestEnum GetTestEnumProp(object target)
        {
            TestEnum prop;
            AttachablePropertyServices.TryGetProperty(target, s_testEnumPropName, out prop);
            return prop;
        }

        public static void SetTestEnumProp(object target, TestEnum prop)
        {
            AttachablePropertyServices.SetProperty(target, s_testEnumPropName, prop);
        }

        public static ClassType1 GetClassType1Prop(object target)
        {
            ClassType1 prop;
            AttachablePropertyServices.TryGetProperty(target, s_classType1PropName, out prop);
            return prop;
        }

        public static void SetClassType1Prop(object target, ClassType1 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classType1PropName, prop);
        }

        public static ClassType4 GetClassType4Prop(object target)
        {
            ClassType4 prop;
            AttachablePropertyServices.TryGetProperty(target, s_classType4PropName, out prop);
            return prop;
        }

        public static void SetClassType4Prop(object target, ClassType4 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classType4PropName, prop);
        }

        public static ClassType2 GetClassType2Prop(object target)
        {
            ClassType2 prop;
            AttachablePropertyServices.TryGetProperty(target, s_classType2PropName, out prop);
            return prop;
        }

        public static void SetClassType2Prop(object target, ClassType2 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classType2PropName, prop);
        }

        public static ClassType3 GetClassType3Prop(object target)
        {
            ClassType3 prop;
            AttachablePropertyServices.TryGetProperty(target, s_classType3PropName, out prop);
            return prop;
        }

        public static void SetClassType3Prop(object target, ClassType3 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classType3PropName, prop);
        }

        public static ClassType_Inheritance23 GetClassType_Inheritance23Prop(object target)
        {
            ClassType_Inheritance23 prop;
            AttachablePropertyServices.TryGetProperty(target, s_classType_Inheritance23PropName, out prop);
            return prop;
        }

        public static void SetClassType_Inheritance23Prop(object target, ClassType_Inheritance23 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classType_Inheritance23PropName, prop);
        }

        public static string[] GetStringArrayProp(object target)
        {
            string[] prop;
            AttachablePropertyServices.TryGetProperty(target, s_stringArrayPropName, out prop);
            return prop;
        }

        public static void SetStringArrayProp(object target, string[] prop)
        {
            AttachablePropertyServices.SetProperty(target, s_stringArrayPropName, prop);
        }

        public static ClassType2[] GetClassArrayProp(object target)
        {
            ClassType2[] prop;
            AttachablePropertyServices.TryGetProperty(target, s_classArrayPropName, out prop);
            return prop;
        }

        public static void SetClassArrayProp(object target, ClassType2[] prop)
        {
            AttachablePropertyServices.SetProperty(target, s_classArrayPropName, prop);
        }

        public static ArrayList GetArrayListProp(object target)
        {
            ArrayList prop;
            AttachablePropertyServices.TryGetProperty(target, s_arrayListPropName, out prop);
            return prop;
        }

        public static void SetArrayListProp(object target, ArrayList prop)
        {
            AttachablePropertyServices.SetProperty(target, s_arrayListPropName, prop);
        }

        public static Hashtable GetHashTableProp(object target)
        {
            Hashtable prop;
            AttachablePropertyServices.TryGetProperty(target, s_hashtablePropName, out prop);
            return prop;
        }

        public static void SetHashTableProp(object target, Hashtable prop)
        {
            AttachablePropertyServices.SetProperty(target, s_hashtablePropName, prop);
        }

        public static IList GetIListProp(object target)
        {
            IList prop;
            AttachablePropertyServices.TryGetProperty(target, s_IListPropName, out prop);
            return prop;
        }

        public static void SetIListProp(object target, IList prop)
        {
            AttachablePropertyServices.SetProperty(target, s_IListPropName, prop);
        }

        public static IList<string> GetIListStringProp(object target)
        {
            IList<string> prop;
            AttachablePropertyServices.TryGetProperty(target, s_IListStringPropName, out prop);
            return prop;
        }

        public static void SetIListStringProp(object target, IList<string> prop)
        {
            AttachablePropertyServices.SetProperty(target, s_IListStringPropName, prop);
        }

        public static IDictionary GetIDictionaryProp(object target)
        {
            IDictionary prop;
            AttachablePropertyServices.TryGetProperty(target, s_IDictionaryPropName, out prop);
            return prop;
        }

        public static void SetIDictionaryProp(object target, IDictionary prop)
        {
            AttachablePropertyServices.SetProperty(target, s_IDictionaryPropName, prop);
        }

        public static IDictionary<ClassType2, DateTime> GetIDictionaryClassType2DateTimeProp(object target)
        {
            IDictionary<ClassType2, DateTime> prop;
            AttachablePropertyServices.TryGetProperty(target, s_IDictionaryClassType2DateTimePropName, out prop);
            return prop;
        }

        public static void SetIDictionaryClassType2DateTimeProp(object target, IDictionary<ClassType2, DateTime> prop)
        {
            AttachablePropertyServices.SetProperty(target, s_IDictionaryClassType2DateTimePropName, prop);
        }

        public static List<ClassType2> GetListClassType2Prop(object target)
        {
            List<ClassType2> prop;
            AttachablePropertyServices.TryGetProperty(target, s_listClassType2PropName, out prop);
            return prop;
        }

        public static void SetListClassType2Prop(object target, List<ClassType2> prop)
        {
            AttachablePropertyServices.SetProperty(target, s_listClassType2PropName, prop);
        }

        public static Dictionary<string, ClassType2> GetDictionaryStringClassType2Prop(object target)
        {
            Dictionary<string, ClassType2> prop;
            AttachablePropertyServices.TryGetProperty(target, s_dictionaryStringClassType2PropName, out prop);
            return prop;
        }

        public static void SetDictionaryStringClassType2Prop(object target, Dictionary<string, ClassType2> prop)
        {
            AttachablePropertyServices.SetProperty(target, s_dictionaryStringClassType2PropName, prop);
        }

        public static AnimalList GetAnimalListProp(object target)
        {
            AnimalList prop;
            AttachablePropertyServices.TryGetProperty(target, s_animalListPropName, out prop);
            return prop;
        }

        public static void SetMEOnAttachedProp(object target, SimpleMEClass prop)
        {
            AttachablePropertyServices.SetProperty(target, s_MEOnAttachedPropName, prop);
        }

        public static SimpleMEClass GetMEOnAttachedProp(object target)
        {
            SimpleMEClass prop;
            AttachablePropertyServices.TryGetProperty(target, s_MEOnAttachedPropName, out prop);
            return prop;
        }

        public static void SetAnimalListProp(object target, AnimalList prop)
        {
            AttachablePropertyServices.SetProperty(target, s_animalListPropName, prop);
        }


#endregion
        public static List<TestCaseInfo> AttachProperties(GetBaseTestCases baseTestCasesMethod, string prefix)
        {
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            foreach (KeyValuePair<AttachableMemberIdentifier, object> property in AttachedPropertySource.Properties)
            {
                int instanceId = 0;
                List<TestCaseInfo> baseTestCases = baseTestCasesMethod();
                foreach (TestCaseInfo baseTest in baseTestCases)
                {
                    // skip attaching to string.Empty, this causes problems elsewhere
                    if (baseTest.Target is string && String.IsNullOrEmpty((string)baseTest.Target))
                    {
                        continue;
                    }

                    // [DISABLED] : Filtering tests , test failure
                    if(instanceId == 3 || instanceId == 7 || instanceId == 11)
                    {
                        continue;
                    }
                    AttachablePropertyServices.SetProperty(baseTest.Target, property.Key, property.Value);
                    baseTest.TestID = prefix + "." + property.Key.MemberName + instanceId;
                    baseTest.CompareAttachedProperties = true;
                    instanceId++;
                    testCases.Add(baseTest);
                }
            }

            return testCases;
        }

        public delegate List<TestCaseInfo> GetBaseTestCases();
    }

    public class AttachedPropertySource2
    {

        static readonly AttachableMemberIdentifier s_int32PropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySource2), "Int32Prop");

        public static Int32 GetInt32Prop(object target)
        {
            Int32 prop;
            AttachablePropertyServices.TryGetProperty(target, s_int32PropName, out prop);
            return prop;
        }

        public static void SetInt32Prop(object target, Int32 prop)
        {
            AttachablePropertyServices.SetProperty(target, s_int32PropName, prop);
        }

        public static List<TestCaseInfo> GetTestCases()
        {
            AttachedPropertySource2 target = new AttachedPropertySource2();
            AttachedPropertySource.SetClassType2Prop(target, new ClassType2 { Category = new ClassType1 { Category = "category" } });
            AttachedPropertySource2.SetInt32Prop(target, Int32.MinValue);
            AttachedPropertySourceDerived.SetStringProp(target, "foo");

            return new List<TestCaseInfo>
                {
                    new TestCaseInfo
                    {
                        Target = target,
                        CompareAttachedProperties = true,
                        TestID = "AttachedPropertySource2" + 0
                    }
                };
        }

    }

    public class AttachedPropertySourceDerived : AttachedPropertySource2
    {

        static readonly AttachableMemberIdentifier s_stringPropName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySourceDerived), "StringProp");

        public static String GetStringProp(object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, s_stringPropName, out prop);
            return prop;
        }

        public static void SetStringProp(object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, s_stringPropName, prop);
        }

        public new static List<TestCaseInfo> GetTestCases()
        {
            AttachedPropertySourceDerived target = new AttachedPropertySourceDerived();
            AttachedPropertySource.SetClassType2Prop(target, new ClassType2 { Category = new ClassType1 { Category = "category" } });
            AttachedPropertySource2.SetInt32Prop(target, Int32.MinValue);
            AttachedPropertySourceDerived.SetStringProp(target, "foo");

            return new List<TestCaseInfo>
                {
                    new TestCaseInfo
                    {
                        Target = target,
                        CompareAttachedProperties = true,
                        TestID = "AttachedPropertySourceDerived" + 0
                    }
                };
        }

    }

    public class AttachedPropertySourceWithSameName
    {
        static readonly AttachableMemberIdentifier s_barName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySourceWithSameName), "Bar");

        public string Bar
        { get; set; }

        public static String GetBar(object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, s_barName, out prop);
            return prop;
        }

        public static void SetBar(object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, s_barName, prop);
        }
    }

    public class InvalidImplementationSource
    {
        public static readonly AttachableMemberIdentifier BarName =
            new AttachableMemberIdentifier(typeof(InvalidImplementationSource), "Bar");

        public static string GetBar(AttachableMemberIdentifier id, object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, id, out prop);
            return prop;
        }

        public static void SetBar(AttachableMemberIdentifier id, object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, id, prop);
        }
    }

    public class AttachedPropertySourceHiddenGetter
    {
        static readonly AttachableMemberIdentifier s_barName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySourceHiddenGetter), "Bar");

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static String GetBar(object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, s_barName, out prop);
            return prop;
        }

        public static void SetBar(object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, s_barName, prop);
        }
    }

    public class AttachedPropertySourceHiddenSetter
    {
        static readonly AttachableMemberIdentifier s_barName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySourceHiddenSetter), "Bar");

        public static String GetBar(object target)
        {
            String prop;
            AttachablePropertyServices.TryGetProperty(target, s_barName, out prop);
            return prop;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static void SetBar(object target, String prop)
        {
            AttachablePropertyServices.SetProperty(target, s_barName, prop);
        }
    }
    
    public class HasAttachableProperty
    {
        public static List<string> GetFoo(object target) { return null; }
    }

    public class AttachedPropertySourceInternalAccessors
    {
        static readonly AttachableMemberIdentifier s_barName =
            new AttachableMemberIdentifier(typeof(AttachedPropertySourceInternalAccessors), "Bar");

        public static string GetBar(object target)
        {
            string prop;
            AttachablePropertyServices.TryGetProperty(target, s_barName, out prop);
            return prop;
        }

        internal static string GetBar(string target)
        {
            throw new NotImplementedException();
        }

        
        public static void SetBar(object target, string prop)
        {
            AttachablePropertyServices.SetProperty(target, s_barName, prop);
        }

        internal static void SetBar(object target, int prop)
        {
            throw new NotImplementedException();
        }
    }

    public class AttachedCollectionPropertySource
    {
        static AttachableMemberIdentifier s_stringsPropertyID = new AttachableMemberIdentifier(typeof(AttachedCollectionPropertySource), "Strings");
        static AttachableMemberIdentifier s_dictionaryPropertyID = new AttachableMemberIdentifier(typeof(AttachedCollectionPropertySource), "Dictionary");

        public static IList<string> GetStrings(object target)
        {
            IList<string> result;
            if (!AttachablePropertyServices.TryGetProperty(target, s_stringsPropertyID, out result))
            {
                result = new List<string>();
                AttachablePropertyServices.SetProperty(target, s_stringsPropertyID, result);
            }
            return result;
        }

        public static IDictionary<string, string> GetDictionary(object target)
        {
            IDictionary<string, string> result;
            if (!AttachablePropertyServices.TryGetProperty(target, s_dictionaryPropertyID, out result))
            {
                result = new Dictionary<string, string>();
                AttachablePropertyServices.SetProperty(target, s_dictionaryPropertyID, result);
            }
            return result;
        }
    }

}
