// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xaml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types.MethodArguments;
    using Microsoft.Test.Xaml.Utilities;

    public class xMethodxArgumentsTests
    {
        public static Dictionary<string, string> namespaces = new Dictionary<string, string>()
                                                                  {
                                                                      {
                                                                          "x", "http://schemas.microsoft.com/winfx/2006/xaml"
                                                                          },
                                                                      {
                                                                          "x2", "http://schemas.microsoft.com/netfx/2008/xaml"
                                                                          },
                                                                      {
                                                                          "xx", "clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes"
                                                                          },
                                                                      {
                                                                          "p", "http://schemas.microsoft.com/winfx/2006/xaml"
                                                                          }
                                                                  };

        [TestCase]
        public void ConstructorWithDifferentTypesOfArgs()
        {
            ConstructorArgTypes1 obj = new ConstructorArgTypes1(10, "Hello World");

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:ConstructorArgTypes1/x2:Arguments[p:Int32='10']",
                                                        @"/xx:ConstructorArgTypes1/x2:Arguments[p:String='Hello World']"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void ConstructorWithComplexType()
        {
            ConstructorArgTypes2 obj = new ConstructorArgTypes2(new ComplexType()
                                                                    {
                                                                        x = 10,
                                                                        y = "Hello World"
                                                                    });

            string xaml = XamlTestDriver.RoundTripCompare(obj);

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:ConstructorArgTypes2/x2:Arguments/xx:ComplexType[@x='10']",
                                                        @"/xx:ConstructorArgTypes2/x2:Arguments/xx:ComplexType[@y='Hello World']"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void ConstructorWithCollections()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(10, 100);
            dict.Add(20, 200);
            dict.Add(30, 300);

            int[] array = new int[]
                              {
                                  1, 2, 3, 4, 5
                              };

            List<int> list = new List<int>()
                                 {
                                     100, 200, 300, 400
                                 };

            IEnumerable enumerable = new List<int>()
                                         {
                                             10, 20, 30, 40
                                         };

            ConstructorArgTypes3 obj = new ConstructorArgTypes3(dict, list, array, enumerable);

            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void ConstructorWithGenericArgument()
        {
            ConstructorArgTypes4 obj = new ConstructorArgTypes4(new List<int>()
                                                                    {
                                                                        1, 5, 6, 9
                                                                    });
            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void ConstructorWithNullableArgument()
        {
            // 
            ConstructorArgTypes5 obj = new ConstructorArgTypes5(null);
            string xaml = XamlTestDriver.RoundTripCompare(obj);

            ConstructorArgTypes5 obj1 = new ConstructorArgTypes5(10);
            string xaml1 = XamlTestDriver.RoundTripCompare(obj1);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NoArgumentMethod()
        {
            NoArgumentMethodClass obj = NoArgumentMethodClass.Create();
            obj.x = 10;
            obj.y = "Hello World";
            string xaml = XamlTestDriver.RoundTripCompare(obj);

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:NoArgumentMethodClass[@x2:FactoryMethod='MyMethod']"
                                                    },
                                                namespaces);
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        
        public void DifferentTypeArgumentMethod()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(10, 100);
            dict.Add(20, 200);
            dict.Add(30, 300);

            int[] array = new int[]
                              {
                                  1, 2, 3, 4, 5
                              };

            List<int> list = new List<int>()
                                 {
                                     100, 200, 300, 400
                                 };

            IEnumerable enumerable = new List<int>()
                                         {
                                             10, 20, 30, 40
                                         };

            DifferentArgumentMethodClass obj = new DifferentArgumentMethodClass()
                                                   {
                                                       array = array,
                                                       list = list,
                                                       nullableInt = 10,
                                                       enumerable = enumerable,
                                                       dictionary = dict,
                                                       x = 100
                                                   };

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:DifferentArgumentMethodClass[@x2:FactoryMethod='MyMethod']",
                                                        @"/xx:DifferentArgumentMethodClass/x2:Arguments"
                                                    },
                                                namespaces);

            // 
            DifferentArgumentMethodClass obj1 = new DifferentArgumentMethodClass()
                                                    {
                                                        array = array,
                                                        list = list,
                                                        nullableInt = null,
                                                        enumerable = enumerable,
                                                        dictionary = dict,
                                                        x = 100
                                                    };

            XamlTestDriver.RoundTripCompareExamineXaml(obj1,
                                                new String[]
                                                    {
                                                        @"/xx:DifferentArgumentMethodClass[@x2:FactoryMethod='MyMethod']",
                                                        @"/xx:DifferentArgumentMethodClass/x2:Arguments"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void ConstructorHasOverloadedMethods()
        {
            OverloadedConstructors obj = new OverloadedConstructors(10, "Hello World");

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:OverloadedConstructors/x2:Arguments[p:Int32='10']",
                                                        @"/xx:OverloadedConstructors/x2:Arguments[p:String='Hello World']"
                                                    },
                                                namespaces);

            OverloadedConstructors obj1 = new OverloadedConstructors(10);

            XamlTestDriver.RoundTripCompareExamineXaml(obj1,
                                                new String[]
                                                    {
                                                        @"/xx:OverloadedConstructors/x2:Arguments[p:Int32='10']"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void ConstructorHasOverloadedDerivedMethods()
        {
            OverloadedConstructorsDerived obj = new OverloadedConstructorsDerived(10, new Base()
                                                                                          {
                                                                                              x = 20
                                                                                          });

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:OverloadedConstructorsDerived/x2:Arguments[p:Int32='10']",
                                                        @"/xx:OverloadedConstructorsDerived/x2:Arguments/xx:Base[@x=20]"
                                                    },
                                                namespaces);

            OverloadedConstructorsDerived obj1 = new OverloadedConstructorsDerived(10, new Derived()
                                                                                           {
                                                                                               y = "Hello World",
                                                                                               x = 32
                                                                                           });

            XamlTestDriver.RoundTripCompareExamineXaml(obj1,
                                                new String[]
                                                    {
                                                        @"/xx:OverloadedConstructorsDerived/x2:Arguments[p:Int32='10']",
                                                        @"/xx:OverloadedConstructorsDerived/x2:Arguments/xx:Derived"
                                                    },
                                                namespaces);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodWithOverloads()
        {
            OverloadedArgumentMethodClass obj = OverloadedArgumentMethodClass.Create();
            obj.x = 10;
            obj.y = "Hello World";

            string xaml = XamlTestDriver.RoundTripCompare(obj);

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:OverloadedArgumentMethodClass/x2:Arguments[p:Int32='10']",
                                                        @"/xx:OverloadedArgumentMethodClass/x2:Arguments[p:String='Hello World']"
                                                    },
                                                namespaces);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodOnDifferentType()
        {
            MethodOnDifferentType obj = Microsoft.Test.Xaml.Types.MethodArguments.MethodOnDifferentType.Create();
            obj.x = 10;
            obj.y = "Hello World";
                                  
            string xaml = XamlTestDriver.RoundTripCompare(obj);

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:MethodOnDifferentType/x2:Arguments[p:Int32='10']",
                                                        @"/xx:MethodOnDifferentType/x2:Arguments[p:String='Hello World']",
                                                        @"/xx:MethodOnDifferentType[@x2:FactoryMethod]"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void CreateGenericType()
        {
            GenericTypeConstructor<int> obj = new GenericTypeConstructor<int>()
                                                  {
                                                      x = 10,
                                                      y = "Hello"
                                                  };

            XamlTestDriver.RoundTripCompare(obj);
        }

        [SecurityLevel(SecurityLevel.FullTrust)]
        [TestCase]
        public void CreateGenericTypexMethod()
        {
            GenericTypexMethod<int> obj = new GenericTypexMethod<int>()
                                              {
                                                  x = 10,
                                                  y = "Hello world"
                                              };

            XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void GenericArgumentMethod()
        {
            GenericArgTypeMethod obj = new GenericArgTypeMethod()
                                           {
                                               x = new List<int>()
                                                       {
                                                           1, 2, 3
                                                       },
                                               y = "Hello"
                                           };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void ConstructorWithParameterList()
        {
            ConstructorWithParameterListClass obj = new ConstructorWithParameterListClass(new int[]
                                                                                              {
                                                                                                  1, 2, 3
                                                                                              });

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <xx:ConstructorWithParameterListClass xmlns:p=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                                xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" 
                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                xmlns:x2=""http://schemas.microsoft.com/winfx/2006/xaml""
                                xmlns:xx=""clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes"">
                              <x2:Arguments>
                                <x:Array Type=""p:Int32"">
                                    <p:Int32>1</p:Int32>
                                    <p:Int32>2</p:Int32>
                                    <p:Int32>3</p:Int32>
                                </x:Array>
                              </x2:Arguments>
                            </xx:ConstructorWithParameterListClass>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodWithParameterList()
        {
            MethodWithParameterListClass obj = new MethodWithParameterListClass()
                                                   {
                                                       x = new List<int>()
                                                               {
                                                                   1, 2, 3
                                                               },
                                                       y = "Hello"
                                                   };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodWithOverloadsParameterList()
        {
            MethodWithParameterListOverloadClass obj = new MethodWithParameterListOverloadClass()
                                                           {
                                                               x = new List<int>()
                                                                       {
                                                                           1, 2, 3
                                                                       },
                                                               y = "Hello"
                                                           };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        // 
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodTakesNullValue()
        {
            GenericArgTypeMethod obj = new GenericArgTypeMethod()
                                           {
                                               x = null,
                                               y = "hello"
                                           };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        // 
        [TestCase]
        public void ConstructorTakesNullValue()
        {
            ConstructorArgTypes1 obj = new ConstructorArgTypes1(10, null);

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new String[]
                                                    {
                                                        @"/xx:ConstructorArgTypes1/x2:Arguments[p:Int32='10']",
                                                        @"/xx:ConstructorArgTypes1/x2:Arguments/x:Null"
                                                    },
                                                namespaces);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void ConstructorTakesDerivedValue()
        {
            Derived1 derived = new Derived1()
                                   {
                                       BaseString = "Base",
                                       DerivedString = "Derived"
                                   };
            ConstructorArgTypesDerived obj = new ConstructorArgTypesDerived(derived);

            string xaml = XamlTestDriver.RoundTripCompare(obj);
        }

        // 23619
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MismatchedParameters()
        {
            MismatchedParametersClass obj = MismatchedParametersClass.Create();
            obj.x = new List<int>()
                    {
                        1, 2, 3
                    };
            obj.y = "hello world";

            string expectedMessage = Exceptions.GetMessage("ObjectReaderInstanceDescriptorIncompatibleArgumentTypes", WpfBinaries.SystemXaml);
            XamlTestDriver.RoundTripCompare(obj, expectedMessage);
        }

        // 
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NonStaticMethodTest()
        {
            NonStaticMethod obj = NonStaticMethod.Create();
            obj.x = new List<int>()
                  {
                      1, 2, 3
                  };
            obj.y = "hello world";

            string message = Exceptions.GetMessage("TypeConverterFailed2", WpfBinaries.SystemXaml);
            message = string.Format(message, "Microsoft.Test.Xaml.Types.MethodArguments.NonStaticMethod", "System.ComponentModel.Design.Serialization.InstanceDescriptor");

            XamlTestDriver.RoundTripCompare(obj, message);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodPointsToProperty()
        {
           MethodPointsProperty obj = MethodPointsProperty.Create();
            obj.x = new List<int>()
                   {
                       1, 2, 3
                   };
            obj.y = "hello world";
            string message = Exceptions.GetMessage("TypeConverterFailed2", WpfBinaries.SystemXaml);
            message = string.Format(message, "Microsoft.Test.Xaml.Types.MethodArguments.MethodPointsProperty", "System.ComponentModel.Design.Serialization.InstanceDescriptor");

            XamlTestDriver.RoundTripCompare(obj, message);
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void MethodReturnsVoid()
        {
            VoidMethod obj = new VoidMethod()
                                 {
                                     x = new List<int>()
                                             {
                                                 1, 2, 3
                                             }, y = "hello world"
                                 };
            //The method 'Microsoft.Test.Xaml.Types.MethodArguments.VoidMethod.MyMethod' does not return a value.
            //XamlMethodHasNoReturnType=The method '{0}.{1}' does not return a value.

            string expectedMessage = "The error message for this has yet to be determined.  Setting the string so the test fails as currently expected.";

            XamlTestDriver.RoundTripCompare(obj, expectedMessage);
        }

        [SecurityLevel(SecurityLevel.FullTrust)]
        [TestCase]
        public void ArgumentHasxMethod()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(10, 100);
            dict.Add(20, 200);
            dict.Add(30, 300);

            int[] array = new int[]
                              {
                                  1, 2, 3, 4, 5
                              };

            List<int> list = new List<int>()
                                 {
                                     100, 200, 300, 400
                                 };

            IEnumerable enumerable = new List<int>()
                                         {
                                             10, 20, 30, 40
                                         };

            DifferentArgumentMethodClass obj1 = new DifferentArgumentMethodClass()
                                                    {
                                                        array = array,
                                                        list = list,
                                                        nullableInt = 10,
                                                        enumerable = enumerable,
                                                        dictionary = dict,
                                                        x = 100
                                                    };

            Nested obj = new Nested(obj1);

            XamlTestDriver.RoundTripCompare(obj);
        }

        // 
        [SecurityLevel(SecurityLevel.FullTrust)]
        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void DefaultCtorArgument()
        {
            object obj = new ConstructorArgTypesDefault();

            string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <ConstructorArgTypesDefault xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes"" 
                            xmlns:p=""http://schemas.microsoft.com/winfx/2006/xaml""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                            xmlns:x2=""http://schemas.microsoft.com/winfx/2006/xaml"" x2:FactoryMethod="""">
                            <x2:Arguments/>
                          </ConstructorArgTypesDefault>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, obj);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void xMethodGivesWrongType()
        {
            string xaml = @"<s:DateTime
                              x2:FactoryMethod='s:Guid.NewGuid'
                              xmlns:s='clr-namespace:System;assembly=mscorlib'
                              xmlns:x2='http://schemas.microsoft.com/winfx/2006/xaml' />";

            bool passed = false;
            try
            {
                var x = XamlServices.Parse(xaml);
            }
            catch (XamlObjectWriterException)
            {
                Tracer.LogTrace("Caught expected exception");
                passed = true;
            }

            if (!passed)
            {
                throw new TestCaseFailedException("expected Format exception not thrown");
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void NoDefaultCtorTest()
        {
            NoDefaultCtor obj = new NoDefaultCtor("foobar");
            string message = Exceptions.GetMessage("ObjectReaderNoDefaultConstructor", WpfBinaries.SystemXaml);
            XamlTestDriver.RoundTripCompare(obj, message);
        }

        /// <summary>
        /// All construction directives must come before the first property element
        /// </summary>
        [TestCase]
        public void OutOfOrderCtorDirectives()
        {
            var xamls = new List<string>()
            {
                @"<CtorDirectiveClass xmlns='clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >
                    <CtorDirectiveClass.Data>blah</CtorDirectiveClass.Data>
                    <x:FactoryMethod>Factory</x:FactoryMethod>
                </CtorDirectiveClass>",

                @"<CtorDirectiveClass xmlns='clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >
                    <CtorDirectiveClass.Data>blah</CtorDirectiveClass.Data>
                    <x:Arguments><x:String>Factory</x:String></x:Arguments>
                </CtorDirectiveClass>",

                @"<CtorDirectiveClass xmlns='clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >
                    <x:FactoryMethod>Factory</x:FactoryMethod>
                    <CtorDirectiveClass.Data>blah</CtorDirectiveClass.Data>
                    <x:Arguments><x:String>Factory</x:String></x:Arguments>
                </CtorDirectiveClass>",

                @"<CtorDirectiveClass xmlns='clr-namespace:Microsoft.Test.Xaml.Types.MethodArguments;assembly=XamlClrTypes' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >
                    <x:Arguments><x:String>Factory</x:String></x:Arguments>
                    <CtorDirectiveClass.Data>blah</CtorDirectiveClass.Data>
                    <x:FactoryMethod>Factory</x:FactoryMethod>
                </CtorDirectiveClass>"
            };

            string expectedMessage = Exceptions.GetMessage("LateConstructionDirective", WpfBinaries.SystemXaml);
            foreach (string xaml in xamls)
            {
                ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), expectedMessage, () => XamlServices.Parse(xaml));
            }
        }
    }
}
