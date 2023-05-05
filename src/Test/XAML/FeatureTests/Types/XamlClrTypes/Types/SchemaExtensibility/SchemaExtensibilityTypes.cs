// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Types.ContentProperties;
    using Microsoft.Test.Xaml.Types.InstanceReference;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Driver;
    
    public class SchemaExtensibilityHolder
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            var instanceWithAttachedProperties = new ClassType2
            {
                Category = new ClassType1 { Category = "blah" }
            };

            AttachedPropertySource.SetBoolProp(instanceWithAttachedProperties, true);

            var referencedInstance = new GenericType1<DateTime, double>
            {
                Info1 = DateTime.Now,
                Info2 = 3.14
            };

            var referencedInstances = new List<GenericType1<DateTime, double>>
            {
                referencedInstance,
                referencedInstance
            };

            var referencedRuntimeNameProperty = new ImplicitNamingBar
            {
                IntProperty = 42,
                MyImplicitName = "MyName",
                StringProperty = "foo"
            };

            var referencedRuntimeNamePropertyCollection = new List<ImplicitNamingBar>
            {
                referencedRuntimeNameProperty,
                referencedRuntimeNameProperty
            };

            return new List<TestCaseInfo>
            {
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new ClassType2
                    {
                        Category = new ClassType1
                        {
                            Category = "blah"
                        }
                    },
                    TestID = instanceIDPrefix + 1,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new ClassType2
                    {
                        Category = new ClassType1
                        {
                            Category = "blah"
                        }
                    },
                    TestID = instanceIDPrefix + 2,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new GenericType1<DateTime, double>
                    {
                        Info1 = DateTime.Now,
                        Info2 = 3.14
                    },
                    TestID = instanceIDPrefix + 3,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new GenericType1<DateTime, double>
                    {
                        Info1 = DateTime.Now,
                        Info2 = 3.14
                    },
                    TestID = instanceIDPrefix + 4,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new ClassType2[]
                    {
                        new ClassType2 { Category = new ClassType1 { Category = "blah" } }
                    },
                    TestID = instanceIDPrefix + 5,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new ClassType2[]
                    {
                        new ClassType2 { Category = new ClassType1 { Category = "blah" } }
                    },
                    TestID = instanceIDPrefix + 6,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new List<GenericType1<DateTime, double>>
                    {
                        new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 }
                    },
                    TestID = instanceIDPrefix + 7,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new List<GenericType1<DateTime, double>>
                    {
                        new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 }
                    },
                    TestID = instanceIDPrefix + 8,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new Dictionary<GenericType1<DateTime, double>, ClassType2>
                    {
                        {
                            new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 },
                            new ClassType2 { Category = new ClassType1 { Category = "foo" } }
                        }
                    },
                    TestID = instanceIDPrefix + 9,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new Dictionary<GenericType1<DateTime, double>, ClassType2>
                    {
                         {
                            new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 },
                            new ClassType2 { Category = new ClassType1 { Category = "foo" } }
                         }
                    },
                    TestID = instanceIDPrefix + 10,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new StringContentPropertyWClass
                    {
                        ContentProperty = "blah",
                        ClassProperty = new ClassType2
                        {
                            Category = new ClassType1
                            {
                                Category = "category"
                            }
                        }
                    },
                    TestID = instanceIDPrefix + 11,
                    Context = ()=>new CustomSchemaContext(true),
                    XPathExpresions = 
                    {
                        "/mtxtc:StringContentPropertyWClass[@ContentProperty='" + "blah" + "']"
                    }
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new StringContentPropertyWClass
                    {
                        ContentProperty = "blah",
                        ClassProperty = new ClassType2
                        {
                            Category = new ClassType1
                            {
                                Category = "category"
                            }
                        }
                    },
                    TestID = instanceIDPrefix + 12,
                    Context = ()=>new CustomSchemaContext(false),
                    XPathExpresions = 
                    {
                        "/mtxtc:StringContentPropertyWClass[.='" + "blah" + "']"
                    }
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new CollectionContainerType14
                    {
                        new ItemType { ItemName = "Name", Price = 3.14 }
                    },
                    TestID = instanceIDPrefix + 13,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new CollectionContainerType14
                    {
                        new ItemType { ItemName = "Name", Price = 3.14 }
                    },
                    TestID = instanceIDPrefix + 14,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new CollectionContainerType15
                    {
                        { "key", new ItemType { ItemName = "Name", Price = 3.14 }}
                    },
                    TestID = instanceIDPrefix + 15,
                    Context = ()=>new CustomSchemaContext(true)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = new CollectionContainerType15
                    {
                        { "key", new ItemType { ItemName = "Name", Price = 3.14 }}
                    },
                    TestID = instanceIDPrefix + 16,
                    Context = ()=>new CustomSchemaContext(false)
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = instanceWithAttachedProperties,
                    TestID = instanceIDPrefix + 17,
                    Context = ()=>new CustomSchemaContext(true),
                    CompareAttachedProperties = true
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = instanceWithAttachedProperties,
                    TestID = instanceIDPrefix + 18,
                    Context = ()=>new CustomSchemaContext(false),
                    CompareAttachedProperties = true
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = referencedInstances,
                    TestID = instanceIDPrefix + 19,
                    Context = ()=>new CustomSchemaContext(true),
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = referencedInstances,
                    TestID = instanceIDPrefix + 20,
                    Context = ()=>new CustomSchemaContext(false),
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = referencedRuntimeNamePropertyCollection,
                    TestID = instanceIDPrefix + 21,
                    Context = ()=>new CustomSchemaContext(true),
                },
                new SchemaExtensibilityTestCaseInfo
                {
                    Target = referencedRuntimeNamePropertyCollection,
                    TestID = instanceIDPrefix + 22,
                    Context = ()=>new CustomSchemaContext(false),
                },

            };
        }
    }

    public class SchemaExtensibilityNegativeTests
    {
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);

            var simpleClass = new ClassType2
            {
                Category = new ClassType1
                {
                    Category = "blah"
                }
            };

            var array = new ClassType2[]
            {
                new ClassType2 { Category = new ClassType1 { Category = "blah" } }
            };

            var collection = new List<GenericType1<DateTime, double>>
            {
                new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 }
            };

            var dictionary = new Dictionary<GenericType1<DateTime, double>, ClassType2>
            {
                 {
                    new GenericType1<DateTime, double> { Info1 = DateTime.Now, Info2 = 3.14 },
                    new ClassType2 { Category = new ClassType1 { Category = "foo" } }
                 }
            };

            ErrorType[] allowedErrorTypes = 
            {
                ErrorType.SchemaContextReturnsNull, 
                ErrorType.SchemaContextThrows,
                ErrorType.XamlTypeReturnsNull,
                ErrorType.XamlTypeThrows,
                ErrorType.XamlTypeInvokerReturnsNull,
                ErrorType.XamlTypeInvokerThrows,
                ErrorType.XamlMemberReturnsNull,
                ErrorType.XamlMemberThrows,
                ErrorType.XamlMemberInvokerReturnsNull,
                ErrorType.XamlMemberInvokerThrows
            };

            return from target in new object[] { simpleClass, array, collection, dictionary }
                   from customXamlTypeMember in new bool[] { true, false }
                   from errorType in allowedErrorTypes
                   where FilterTests(customXamlTypeMember, errorType)
                   let testId = GetTestId(instanceIDPrefix, target, customXamlTypeMember, errorType)
                   select new SchemaExtensibilityTestCaseInfo
                   {
                       Target = target,
                       TestID = testId,
                       Context = ()=> new CustomSchemaContext(customXamlTypeMember, errorType),
                       ExpectedResult = false,
                       ExpectedMessage = GetExpectedMessage(errorType, testId),
                       BugNumber = GetBugNumber(errorType)
                   };            
        }

        static int GetBugNumber(ErrorType errorType)
        {
            switch(errorType)
            {
                case ErrorType.XamlTypeInvokerReturnsNull:
                    return 0;
                default:
                    return 0;
            }
        }

        static string GetTestId(string baseId, object target, bool customXamlTypeMember, ErrorType errorType)
        {
            return baseId + target.GetType().Name + (customXamlTypeMember ? "Custom" : "Base") + errorType.ToString();
        }

        static bool FilterTests(bool customXamlTypeMember, ErrorType errorType)
        {
            if (!customXamlTypeMember &&
                (errorType == ErrorType.XamlTypeThrows ||
                 errorType == ErrorType.XamlTypeReturnsNull ||
                 errorType == ErrorType.XamlMemberThrows ||
                 errorType == ErrorType.XamlMemberReturnsNull ||
                 errorType == ErrorType.XamlMemberInvokerThrows ||
                 errorType == ErrorType.XamlMemberInvokerReturnsNull))
            {
                return false;
            }

            return true;
        }

        static string GetExpectedMessage(ErrorType errorType, string testId)
        {
            switch(errorType)
            {
                case ErrorType.XamlMemberInvokerReturnsNull:
                case ErrorType.XamlTypeReturnsNull:
                    return "Two Xaml Objects are different.";
                case ErrorType.SchemaContextReturnsNull:
                    return Exceptions.GetMessage("ObjectReaderTypeNotAllowed", WpfBinaries.SystemXaml);
                case ErrorType.SchemaContextThrows:
                case ErrorType.XamlMemberThrows:
                case ErrorType.XamlTypeThrows:
                    return "This is an expected exception thrown in a negative test case.";
                case ErrorType.XamlTypeInvokerThrows:
                    if(testId.Contains("ClassType2"))
                    {
                        return Exceptions.GetMessage("ConstructorInvocation", WpfBinaries.SystemXaml);
                    }
                    else
                    {
                        return Exceptions.GetMessage("GetItemsException", WpfBinaries.SystemXaml);
                    }
                case ErrorType.XamlMemberInvokerThrows:
                case ErrorType.XamlMemberReturnsNull:
                    return Exceptions.GetMessage("GetValue", WpfBinaries.SystemXaml);
                default:
                    return null;
            }
        }
    }
}
