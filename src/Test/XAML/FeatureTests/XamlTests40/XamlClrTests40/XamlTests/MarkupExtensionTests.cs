// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Types.MarkupExtensions;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Utilities;
    
    public class MarkupExtensionTests
    {
        public static Dictionary<string, string> Namespaces = new Dictionary<string, string>()
        {
            {"x", Microsoft.Test.Xaml.Common.Namespaces.Namespace2006},
            {"x2", Microsoft.Test.Xaml.Common.Namespaces.NamespaceV2},
            {"xx", "clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"},
            {"p", Microsoft.Test.Xaml.Common.Namespaces.NamespaceBuiltinTypes}
        };

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SimpleMarkupExtensionConverter()
        {
            SimpleMEClassContainer obj = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = "city1",
                    AptNo = 210
                }
            };

            XamlTestDriver.RoundTripCompareExamineXaml(
                obj,
                new string[]{
                @"/xx:SimpleMEClassContainer[@SimpleMEClass=""{SimpleME Address=city1, AptNo=210}""]"
                },
                Namespaces);
        }

        [TestCase]
        public void SimpleMarkupExtensionConverterWithCurlyBraceValue()
        {
            SimpleMEClassContainer obj = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = "[{city1}]",
                    AptNo = 210
                }
            };

            XamlTestDriver.RoundTripCompareExamineXaml(
                obj,
                new string[]{
                @"/xx:SimpleMEClassContainer/@SimpleMEClass[contains(.,'[{city1}]')]"
                },
                Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SimpleMarkupExtensionSingleProperty()
        {
            SimpleMEClassSPContainer obj = new SimpleMEClassSPContainer()
            {
                SimpleMESPClass = new SimpleMESPClass()
                    {
                        FloatProperty = (float)201.100
                    }
            };

            XamlTestDriver.RoundTripCompareExamineXaml(
                            obj,
                            new string[]{
                @"/xx:SimpleMEClassSPContainer[@SimpleMESPClass=""{SimpleSPME FloatProperty=201.1}""]"
                },
                Namespaces);
        }


        // 


















        //    // string xamlS = Helpers.Serialize(obj);

        //    XamlTestDriver.RoundTripCompareExamineXaml(
        //                    obj,
        //                    new string[]{
        //        @"/xx:MultiplePropetiesMEContainer[@MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME DifferentNamespaceTypeProperty={x:Null}, SimpleMEClass={x:Null}, XMethodProperty={x:Null}, BooleanProperty=False, ByteProperty=20, CharProperty=c, DoubleProperty=291, IntProperty=10, LongProperty=30, ShortProperty=32, StringProperty=hello}""]"
        //        },
        //                    namespaces);

        //}

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void GenericProperty()
        {
            GenericMEContainer obj = new GenericMEContainer()
            {
                GenericMEClass = new GenericMEClass<string>() { GenericProperty = "hello" },
            };

            XamlTestDriver.RoundTripCompareExamineXaml(
                            obj,
                            new string[]{
                                @"/xx:GenericMEContainer[@GenericMEClass=""{GenericME StringProperty=hello}""]"
                            },
                            Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SimpleMENonAttributable()
        {
            SimpleNonAttribMEClassContainer obj = new SimpleNonAttribMEClassContainer()
            {
                SimpleNonAttribMEClass = new SimpleNonAttribMEClass() { Strings = new List<string>() { } }
            };
            obj.SimpleNonAttribMEClass.Strings.Add("hello");
            obj.SimpleNonAttribMEClass.Strings.Add("world");

            XamlTestDriver.RoundTripCompareExamineXaml(
                           obj,
                           new string[]{
                                @"/xx:SimpleNonAttribMEClassContainer/xx:SimpleNonAttribMEClassContainer.SimpleNonAttribMEClass/xx:SimpleNonAttribME/xx:SimpleNonAttribME.Strings"
                            },
                           Namespaces);
        }


        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void CollectionME()
        {
            CollectionMEContainer obj = new CollectionMEContainer() { CollectionMEClass = new CollectionMEClass() };
            obj.CollectionMEClass.Add("Hello");
            obj.CollectionMEClass.Add("1");
            obj.CollectionMEClass.Add("2");
            obj.CollectionMEClass.Add("3");
            obj.CollectionMEClass.Add("4");

            XamlTestDriver.RoundTripCompareExamineXaml(
                           obj,
                           new string[]{
                                @"/xx:CollectionMEContainer[@CollectionMEClass=""{CollectionME Strings=Hello-1-2-3-4-}""]"
                            },
                           Namespaces);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MEOnPrimitiveType()
        {
            StringMEContainer obj = new StringMEContainer() { StringProperty = "Hello" };

            XamlTestDriver.RoundTripCompareExamineXaml(
                          obj,
                          new string[]{
                              @"/xx:StringMEContainer[@StringProperty=""Hello""]"
                            },
                          Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void PropertyWithoutSetter()
        {
            SimpleMEWoSetterContainer obj = new SimpleMEWoSetterContainer() { SimpleMEWoSetterClass = new SimpleMEWoSetterClass("foo") };
            XamlTestDriver.RoundTripCompareExamineXaml(
                          obj,
                          new string[]{
                                @"/xx:SimpleMEWoSetterContainer[@SimpleMEWoSetterClass=""{SimpleMEWoSetter Address=foo}""]"
                            },
                          Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void CollectionDictionaryWithoutSetter()
        {
            List<string> strings = new List<string>() { "1", "2", "3" };
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("1", "Hello");
            dics.Add("2", "World");

            SimpleMECodWoSetterContainer obj = new SimpleMECodWoSetterContainer()
            {
                SimpleMECodWoSetterClass = new SimpleMECodWoSetterClass(strings, dics),
            };

            string xamlS = XamlTestDriver.RoundTripCompare(obj);
        }


        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SimpleNestedMe()
        {
            NestedMeContainer obj = new NestedMeContainer()
            {
                NestedMEClass = new NestedMEClass()
                {
                    Address = "Redmond WA",
                    SimpleMEClass = new SimpleMEClass()
                    {
                        AptNo = 98005,
                        Address = "Bellevue, wa"
                    }
                }
            };

            string xamlS = XamlTestDriver.RoundTripCompare(obj);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MEWithTypeConverter()
        {
            MeWithTCContainer obj = new MeWithTCContainer()
            {
                MeWithTCClass = new MeWithTCClass()
                {
                    Address = "Redmond Wa",
                }
            };

            string xamlS = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void BothMEAndStringTypeConverter()
        {
            ComplexMEClassContainer obj = new ComplexMEClassContainer()
            {
                ComplexMEClass = new ComplexMEClass()
                {
                    Address = "tinseltown",
                }
            };

            XamlTestDriver.RoundTripCompareExamineXaml(
                          obj,
                          new string[]{
                                @"/xx:ComplexMEClassContainer[@ComplexMEClass=""{ComplexME Address=tinseltown}""]"
                            },
                          Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MEOnPropertyAndType()
        {
            TypePropertyClassContainer obj = new TypePropertyClassContainer()
            {
                TypePropertyClass = new TypePropertyClass()
                {
                    Address = "RedmondWa",
                }
            };
            XamlTestDriver.RoundTripCompareExamineXaml(
                          obj,
                          new string[]{
                                @"/xx:TypePropertyClassContainer[@TypePropertyClass=""{PropertyME PropertyAddress=RedmondWa}""]"
                            },
                          Namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SingleArgumentConstructor()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    StringProperty = "hello"
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME2 hello}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MultipleArgumentConstructor()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    StringProperty = "hello",
                    BooleanProperty = false,
                    CharProperty = 'c',
                    LongProperty = (long)30,
                    ShortProperty = 20,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME hello,False,c,30,20}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }

        // 

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void ParamArgumentConstructor()
        {
            ParamsMEClassContainer obj = new ParamsMEClassContainer()
            {
                ParamsMEClass = new ParamsMEClass()
                {
                    Address = "hello",
                    AptNo = 20,
                }
            };

            // ME with param args does not create squigly format - so just test round trip //
            XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void CtorArgHasME()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    }
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address=city1, AptNo=210}}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";


            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MemberArgHasME()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    }
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME SimpleMEClass={mtxtm:SimpleME Address=city1, AptNo=210}}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        }


        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MEHasXMethodProperty()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    XMethodProperty = MethodPointsProperty.MyMethod(10, "hello"),
                }
            };

            // When using x2:Method - markup extension should expand to element syntax //
            XamlTestDriver.RoundTripCompareExamineXaml(
                        obj,
                        new string[]{
                                @"/xx:MultiplePropetiesMEContainer/xx:MultiplePropetiesMEContainer.MultiplePropertiesMEClass/xx:MultiplePropertyME/xx:MultiplePropertyME.XMethodProperty/xx:MethodPointsProperty[@x2:FactoryMethod]"
                            },
                        Namespaces);

        }


        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void NullConstructorArgument()
        {
            SimpleMEClassContainer obj = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = null,
                }
            };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                        <mtxtm:SimpleMEClassContainer SimpleMEClass=""{mtxtm:SimpleME Address={x:Null}, AptNo=0}""
                        xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes""
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />";


            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void NullMemberinME()
        {
            SimpleMEClassContainer obj = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = null,
                }
            };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                        <mtxtm:SimpleMEClassContainer SimpleMEClass=""{mtxtm:SimpleME Address={x:Null}}""
                        xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes""
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />";


            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void CtorArgsAndProperties()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address=city1, AptNo=210}, BooleanProperty=true, DoubleProperty=30.5}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void EmptyCtorArgsAndProperties()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                    StringProperty = "",
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address='', AptNo=210}, BooleanProperty=true, DoubleProperty=30.5, StringProperty=''}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void MemberWithTypeQualifiedName()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            // note the fully qualified memeber name mtxtm:SimpleME.Address=city1 //
            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME mtxtm:SimpleME.Address=city1, AptNo=210}, BooleanProperty=true, DoubleProperty=30.5}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SingleQuotedCtorValues()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    StringProperty = "hello",
                    BooleanProperty = false,
                    CharProperty = 'c',
                    LongProperty = (long)30,
                    ShortProperty = 20,
                }
            };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME 'hello','False','c','30','20'}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);

        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SingleQuotedMemberValues()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address='city1', AptNo='210'}, BooleanProperty='true', DoubleProperty='30.5'}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }


        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void SingleQuotedEscape()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "'city1",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address='\'city1', AptNo='210'}, BooleanProperty='true', DoubleProperty='30.5'}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void DoubleQuotedCtorValues()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    StringProperty = "hello",
                    BooleanProperty = false,
                    CharProperty = 'c',
                    LongProperty = (long)30,
                    ShortProperty = 20,
                }
            };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME &quot;hello&quot;,&quot;False&quot;,&quot;c&quot;,&quot;30&quot;,&quot;20&quot;}""
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);

        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void DoubleQuotedMemberValues()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = "city1",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address=&quot;city1&quot;, AptNo=&quot;210&quot;}, BooleanProperty=&quot;true&quot;, DoubleProperty=&quot;30.5&quot;}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }


        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void DoubleQuotedEscape()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = @"""city1\", // "city1\
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address=&quot;\&quot;city1\\&quot;, AptNo=&quot;210&quot;}, BooleanProperty=&quot;true&quot;, DoubleProperty=&quot;30.5&quot;}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        public void UnquotedEscape()
        {
            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
            {
                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
                {
                    SimpleMEClass = new SimpleMEClass()
                    {
                        Address = @"city1[',={}\",
                        AptNo = 210,
                    },
                    BooleanProperty = true,
                    DoubleProperty = 30.5,
                }
            };

            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME {mtxtm:SimpleME Address=city1[\'\,\=\{\}\\, AptNo=210}, BooleanProperty=true, DoubleProperty=30.5}"" 
                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);

        }

        public class DifferentNamespaceType
        {
            public int IntegerProperty { get; set; }
        }

        //        scenario works - rewrite this 
        //        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT)]
        //        public void SwitchNamespaces()
        //        {
        //            MultiplePropetiesMEContainer obj = new MultiplePropetiesMEContainer()
        //            {
        //                MultiplePropertiesMEClass = new MultiplePropertiesMEClass()
        //                {
        //                    DifferentNamespaceTypeProperty = new DifferentNamespaceType()
        //                    {
        //                        IntegerProperty = 30,
        //                    }
        //                }
        //            };

        //            string xaml = XamlTestDriver.Serialize(obj);

        //            string xamlS = @"<?xml version=""1.0"" encoding=""utf-16""?>
        //                                <mtxtm:MultiplePropetiesMEContainer MultiplePropertiesMEClass=""{mtxtm:MultiplePropertyME hello,False,c,30,20}"" 
        //                                xmlns:mtxtm=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" />";

        //            XamlTestDriver.XamlFirstCompareObjects(xamlS, obj);
        //        }


        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        
        public void MarkupStringAsElement()
        {
            SimpleMEClassContainer obj = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = "city1",
                    AptNo = 210
                }
            };

            string xaml = @"<SimpleMEClassContainer 
                            xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"">
                                <SimpleMEClassContainer.SimpleMEClass>{SimpleME Address=city1, AptNo=210}</SimpleMEClassContainer.SimpleMEClass>
                                </SimpleMEClassContainer>";

            var loaded = XamlServices.Parse(xaml);

        }


        //        [TestCase(
        //             Owner = "Microsoft",
        //             Category = TestCategory.Functional,
        //             TestType = TestType.Automated
        //             )]
        //        public void IXNameResolverExtensionTest()
        //        {
        //            string xaml = @"<Wrapper 
        //                            xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"" 
        //                            Value=""{Context}""/>";

        //            Wrapper val = (Wrapper)XamlServices.Parse(xaml);
        //            if (val.Value == null || !(val.Value is IXNameResolver))
        //            {
        //                throw new DataTestException("Value is null or not an IXNameResolver.");
        //            }
        //        }

        //        [TestCase(
        //            Owner = "Microsoft",
        //            Category = TestCategory.Functional,
        //            TestType = TestType.Automated
        //            )]
        //        public void XamlReaderRegressionTest()
        //        {
        //            string foo = @"<Example xmlns=""clr-namespace:Test;assembly=Test""
        //                            xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
        //                            Person=""{Person Name=Dan}"" />";

        //            XamlTextReader reader = new XamlTextReader(new StringReader(foo));

        //            IEnumerable<XamlNode> nodes = reader.ReadToEnd();
        //            string nodesString = "";
        //            foreach (XamlNode node in nodes)
        //            {
        //                Tracer.LogTrace(node.NodeType.ToString());
        //                nodesString += node.NodeType.ToString();
        //            }

        //            string expected = "RecordMemberRecordMemberAtomEndMemberEndRecordEndMemberEndRecord";

        //            if (!nodesString.Equals(expected))
        //            {
        //                throw new TestCaseFailedException("Expected nodes not obtained");
        //            }

        //        }

        // [DISABLED]
        // [TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.IDW,
        //     TestType = TestType.Automated
        //     )]
        public void InvalidPrefixInMarkupExtensionTest()
        {
            // 
            string xaml = @"<List x:TypeArguments='s:SimpleMEClassContainer' Capacity='4' xmlns='clr-namespace:System.Collections;assembly=mscorlib' xmlns:s='clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <s:SimpleMEClassContainer SimpleMEClass='{s2:SimpleME Address=city1, AptNo=210}' />
                            </List>";

            string error = Exceptions.GetMessage("PrefixNotFound", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlParseException), error, () => XamlServices.Parse(xaml));
        }

        [TestCase]
        public void TruncateExtension()
        {
            // Test for TF 
            ClassType obj = new ClassType();
            string xaml = XamlServices.Save(obj);

            // Xaml should contain Null and not NullExtension 
            if (xaml.Contains("Extension"))
            {
                string trace = "ERROR: 'Extension' found in serialized xaml :" + xaml;
                Tracer.LogTrace(trace);
                throw new TestCaseFailedException(trace);
            }

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
            string xaml2 = xaml.Replace("Null", "NullExtension");
            Tracer.LogTrace("New Xaml is " + xaml2);
            XamlTestDriver.XamlFirstCompareObjects(xaml2, obj);
        }

        [TestCase]
        public void SupportMarkupExtensionsWithDuplicateArityTest()
        {
            string xaml = @"<MultiplePropetiesMEContainer2 MultiplePropertiesMEClass='{MultiplePropertyMEWMultiCtors blah}' xmlns='clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes' />";
            ExceptionHelpers.CheckForException(typeof(XamlSchemaException), () => XamlServices.Parse(xaml));

            XamlSchemaContext ctx = new XamlSchemaContext(new XamlSchemaContextSettings { SupportMarkupExtensionsWithDuplicateArity = true });
            XamlReader reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), ctx);
            XamlWriter writer = new XamlObjectWriter(ctx);
            XamlServices.Transform(reader, writer);
        }

        [TestCase]
        public void PreferExtensionOverNoneTest()
        {
            // when encountering {SomeType} on read, try to instantiate SomeTypeExtension
            // before SomeType
            PreferExtensionWrapper wrapper = new PreferExtensionWrapper
            {
                Data = new PreferExtensionType(3)
            };

            XamlTestDriver.RoundTripCompare(wrapper);
        }

        [TestCase]
        public void MEDiffNS()
        {
            var obj = new SimpleMEClassContainerDiffNS()
            {
                SimpleMEClass = new Microsoft.Test.Xaml.Types.MarkupExtensions2.SimpleMEClassDiffNS()
                {
                    Address = "Hello",
                    AptNo = 23,
                }
            };

            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (!xaml.Contains(@"xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.MarkupExtensions;assembly=XamlClrTypes"""))
            {
                throw new TestCaseFailedException("Default namespace is not the namespace of the markup extension");
            }
        }
    }
}
