// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types.Integration;
    using Microsoft.Test.Xaml.Utilities;

    public class IntegrationTests
    {
        /// <summary>
        ///  The property in a markup extension has a type converter
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void TypeConverterInME()
        {
            var obj = new Container() { Content = new CustomMEClass() { Content = new TimeSpanContainer() } };
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (!xaml.Contains("$"))
            {
                throw new TestCaseFailedException("TypeConverter in ME was not invoked");
            }

            if (!xaml.Contains("{CustomME"))
            {
                throw new TestCaseFailedException("Did not serialize in squigly format");
            }
        }

        /// <summary>
        /// Markup Extension containing a Markup Extension property
        /// </summary>
        [TestCase]
        public void MEInME()
        {
            var obj = new Container() { Content = new CustomMEClass() { Content = new CustomMEClass() { Content = "Hello" } } };
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        ///  Markup extension containg x:args - no default constructor 
        /// </summary>
        [TestCase]
        public void MEWithXArgs()
        {
            var obj = new Container() { Content = new CustomMEXargsClass() { Content = "Hello" } };
            string xaml = XamlServices.Save(obj);
            Tracer.LogTrace("Xaml " + xaml);
            Container container = (Container)XamlServices.Parse(xaml);
            string value = (string)(container.Content as CustomMEClass).Content;
            if (!string.Equals("Hello", value))
            {
                throw new DataTestException("ME With XArgs did not roundtrip. Expected Hello - Actual :" + value);
            }
        }

        /// <summary>
        ///  ME created using FactoryMethod 
        /// </summary>
        [TestCase]
        public void MEWithFactoryMethod()
        {
            string xaml = @"<Container xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes'
                                       xmlns:x2='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <Container.Content>
                                <CustomME x2:FactoryMethod='CustomME.Create'>
                                </CustomME>
                              </Container.Content>
                            </Container>";

            var obj = XamlServices.Parse(xaml);
            if (((obj as Container).Content as CustomMEClass).Content == null)
                throw new TestCaseException("Factory Method was not called");
        }

        /// <summary>
        /// Two objects reference within a ME 
        /// </summary>
        [TestCase]
        public void ReferenceWithinME()
        {
            object someObj = new object();
            var obj = new Container() { Content = new CustomMEClass() { Content = someObj, SecondContent = someObj } };
            var roundtripped = XamlTestDriver.RoundtripAndCompareObjects(obj) as Container;
            CustomMEClass instance = roundtripped.Content as CustomMEClass;
            if (instance.Content.GetHashCode() != instance.SecondContent.GetHashCode())
            {
                throw new TestCaseFailedException("Referenced objects did not roundtrip");
            }
        }

        /// <summary>
        /// backward reference outside the ME 
        /// </summary>
        [TestCase]
        public void BackwardReferenceOutsideME()
        {
            var obj = new Container();
            obj.Content = new CustomMEClass() { Content = obj, SecondContent = obj };

            string xaml = XamlServices.Save(obj);
            string message = Exceptions.GetMessage("ProvideValueCycle", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), message, () => XamlServices.Parse(xaml));
        }

        /// <summary>
        /// Forward reference outside the ME 
        /// </summary>
        [TestCase]
        public void ForwardReferenceOutsideME()
        {
            var content = new object();
            var container = new Container() { Content = new CustomMEClass() { Content = content, SecondContent = content } };
            var obj1 = new List<object> { container, content };
            XamlTestDriver.RoundTripCompare(obj1);
            // Note: Object tag has to be second on the list - that's what makes this a forward reference //
            string xaml = @"<List x:TypeArguments='s:Object' Capacity='4' 
                                                xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                                xmlns:mtxti='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                                xmlns:s='clr-namespace:System;assembly=mscorlib' 
                                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>    
                                            <mtxti:Container 
                                                Content='{mtxti:CustomME Content={x:Reference __ReferenceID0}, SecondContent={x:Reference __ReferenceID0}}' 
                                                SecondContent='{x:Null}' />
                                            <s:Object x:Name='__ReferenceID0' />                                                         
                                        </List>";
            var obj2 = XamlServices.Parse(xaml);
            XamlObjectComparer.CompareObjects(obj2, obj1);
        }



        /// <summary>
        /// Markup extension with generic type property 
        /// </summary>
        [TestCase]
        public void MEWithGeneric()
        {
            var obj = new Container() { Content = new CustomMEClass() { Content = new List<string>() { "Hello", "World" } } };
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// Markup extension has a type converter as well - TC.ProvideValue should be called
        /// </summary>
        [TestCase]
        public void MEHasTC()
        {
            var obj = new Container() { Content = new CustomMEWithTCClass() { Content = "Hello", SecondContent = "World" } };
            XamlTestDriver.RoundTripCompare(obj);
            string xaml = XamlServices.Save(obj);
            if (!xaml.Contains("*"))
            {
                throw new TestCaseException("TypeConverter was not called");
            }
        }

        /// <summary>
        /// Markup extension containg x:args - no default constructor 
        /// Arguments should be type converted 
        /// CustomMEClass is the argument that has a type converter 
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void MEWithXArgsTC()
        {
            var obj = new Container()
            {
                Content = new CustomMEXargsClass()
                {
                    Content = new CustomMEClass()
                    {
                        Content = "Hello",
                        SecondContent = "World"
                    }
                }
            };
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// Should be able to set events with {} form
        /// </summary>
        [TestCase]
        public void SetEventInMESquiggly()
        {
            string xamlSquiggly = @"<WrapperWithEventHanlder xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes'  Data='{EventHolderExtension SomeEvent=SomeEventHandler}' />";

            var squiggly = (WrapperWithEventHanlder)XamlServices.Parse(xamlSquiggly);

            ((EventHolder)squiggly.Data).RaiseSomeEvent();

            if (!squiggly.Fired)
            {
                throw new TestCaseException("Event not fired.");
            }
        }

        /// <summary>
        /// Should be able to set events with element form
        /// </summary>
        [TestCase]
        public void SetEventInMEElement()
        {
            string xamlElement = @"<WrapperWithEventHanlder xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' >
                                    <WrapperWithEventHanlder.Data>
                                        <EventHolderExtension SomeEvent='SomeEventHandler' />
                                    </WrapperWithEventHanlder.Data>
                                   </WrapperWithEventHanlder>";

            var element = (WrapperWithEventHanlder)XamlServices.Parse(xamlElement);

            ((EventHolder)element.Data).RaiseSomeEvent();

            if (!element.Fired)
            {
                throw new TestCaseException("Event not fired.");
            }
        }

        /// <summary>
        /// Property contains both x:Arguments and TypeConverter
        /// </summary>
        [TestCase]
        public void PropertyWithxArgsAndTC()
        {
            var obj = new TCAndXArgsContainer(new TimeSpan(1000));
            XamlTestDriver.RoundTripCompare(obj);
            string xaml = XamlServices.Save(obj);
            if (xaml.Contains("$"))
            {
                throw new TestCaseException("Argument was type converted when it was not expected to");
            }
        }

        /// <summary>
        /// Property has both x:Args and ME 
        /// </summary>
        [TestCase]
        public void PropertyWithxArgsAndME()
        {
            var obj = new MEAndXArgsContainer(new CustomMEClass()
                                                 {
                                                     Content = "Hello",
                                                     SecondContent = "World"
                                                 });
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// nested x:argument
        /// </summary>
        [TestCase]
        public void NestedArgs()
        {
            var obj = new ArgumentContainer(new ArgumentContainer("Hello"));
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// Argument created using Factory Method 
        /// </summary>
        [TestCase]
        public void CreateArgUsingFactoryMethod()
        {
            string xaml = @"<ArgumentContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <x:Arguments>
                                <ArgumentContainer x:FactoryMethod='ArgumentContainer.Create' />
                              </x:Arguments>
                            </ArgumentContainer>";
            var obj = XamlServices.Parse(xaml);
            if ((int)((obj as ArgumentContainer).Content as ArgumentContainer).Content != 20)
            {
                throw new TestCaseException("Factory Method was not invoked");
            }
        }

        /// <summary>
        /// Arguemnt references an object in the graph
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void ArgReferencesAnObject()
        {
            var obj = new Container();
            obj.Content = new ArgumentContainer(obj);

            string xaml = XamlServices.Save(obj);
            string message = Exceptions.GetMessage("ForwardRefDirectives", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), message, () => XamlServices.Parse(xaml));
        }

        /// <summary>
        /// Argument is a generic type
        /// </summary>
        [TestCase]
        public void ArgIsGeneric()
        {
            var obj = new ArgumentContainer(new List<string>() { "Hello", "World" });
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// Factory method creating a type converted object 
        /// </summary>
        [TestCase]
        public void FactoryMethodTC()
        {
            string xaml = @"<CustomMEXargsClass 
                            x:FactoryMethod='Factory.CreateTypeConvertedObject'
                            xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' />";
            var obj = XamlServices.Parse(xaml) as CustomMEXargsClass;
            if ((string)obj.Content != "Hello")
            {
                throw new TestCaseFailedException("Factory method not invoked");
            }

            // CustomMEWithXArgs, markup extension that is written for CustomMEXargsClass doesn't have a default public ctor
            // since it is the root element it must be written in element form so positional parameters aren't allowed
            string message = Exceptions.GetMessage("ExpandPositionalParametersinTypeWithNoDefaultConstructor", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(XamlXmlWriterException), message, () => XamlServices.Save(obj));
        }

        /// <summary>
        /// Create a generic type using FactoryMethod 
        /// factory method is in wrong namespace - (NullRef)
        /// </summary>
        [TestCase]
        public void CreateGenericUsingFactoryMethodNegative()
        {
            string xaml = @"<List x:TypeArguments='x:String' 
                                xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                               x:FactoryMethod='Factory.CreateStringList'/>";

            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), () => XamlServices.Parse(xaml));
        }

        /// <summary>
        /// Create a generic type using FactoryMethod 
        /// </summary>
        [TestCase]
        public void CreateGenericUsingFactoryMethod()
        {
            string xaml = @"<List x:TypeArguments='x:String' 
                                xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                xmlns:f='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                               x:FactoryMethod='f:Factory.CreateStringList'/>";

            var obj = XamlServices.Parse(xaml);
            if (obj == null)
            {
                throw new TestCaseFailedException("Create Generic using factory method returned null");
            }
        }

        /// <summary>
        /// If the Markup extension is named, the object returned by provide value is not named
        /// This is a bug in v3 - so v4 has to follow for compat reasons.
        /// </summary>
        [TestCase]
        public void NamingME()
        {
            string xaml = @"<Container 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >
                                <Container.Content>
                                    <CustomME x:Name='foobar' Content='Hello'/>
                                </Container.Content>
                            </Container>";
            var obj = XamlServices.Parse(xaml);
            if (((obj as Container).Content as CustomMEClass).Name != null)
            {
                throw new TestCaseFailedException("The object returned from ProvideValue should not be named (the ME is named). This is to maintain v3 comptibility");
            }
        }

        /// <summary>
        /// ME value as x:Key in dictionary
        /// </summary>
        [TestCase]
        public void MEValueAsKey()
        {
            string xaml = @"<Dictionary x:TypeArguments='s:Object, s:Object' 
                                        xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                        xmlns:mtxti='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                        xmlns:s='clr-namespace:System;assembly=mscorlib' 
                                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <mtxti:CustomME SecondContent='{x:Null}' Content='Hello' x:Key='{mtxti:CustomME Content=World}' />
                             </Dictionary>";

            var obj = XamlServices.Parse(xaml);
            if ((obj as Dictionary<object, object>).ElementAt(0).Key.GetType() != typeof(CustomMEClass))
            {
                throw new TestCaseFailedException("ProvideValue was not called on markup extension before adding it to the dictionary as key");
            }
        }

        /// <summary>
        /// Reference an object that has a TypeConverter 
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void ReferenceTC()
        {
            var obj = new CustomType() { Content = "hello" };
            var obj1 = new Container()
            {
                Content = obj,
                SecondContent = obj,
            };
            var roundTripped = XamlTestDriver.RoundtripAndCompareObjects(obj1) as Container;
            if (roundTripped.Content.GetHashCode() != roundTripped.SecondContent.GetHashCode())
            {
                throw new TestCaseFailedException("Refrenced instance was duplicated");
            }
        }

        /// <summary>
        /// Reference an object that is a markup extension
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void ReferenceME()
        {
            var obj = new CustomMEClass() { Content = "hello", SecondContent = "World" };
            var container = new Container()
            {
                Content = obj,
                SecondContent = obj,
            };
            var roundTripped = XamlTestDriver.RoundtripAndCompareObjects(container) as Container;
            if (roundTripped.Content.GetHashCode() != roundTripped.SecondContent.GetHashCode())
            {
                throw new TestCaseFailedException("Refrenced instance was duplicated");
            }
        }

        /// <summary>
        /// Reference an Argument
        /// </summary>
        [TestCase]
        public void ReferenceAnArgument()
        {
            string xaml = @"<Container 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <Container.Content>
                                    <ArgumentContainer>
                                      <x:Arguments>
                                        <x:String x:Name='arg1'>Hello</x:String>
                                      </x:Arguments>
                                    </ArgumentContainer>
                                </Container.Content>
                                <Container.SecondContent>
                                    <x:Reference>arg1</x:Reference>
                                </Container.SecondContent>
                            </Container>";
            var obj = XamlServices.Parse(xaml);
            if (!((obj as Container).SecondContent as string).Equals("Hello"))
            {
                throw new TestCaseFailedException("Reference to an argument failed");
            }
        }

        /// <summary>
        /// Reference an Argument created using factory method
        /// </summary>
        [TestCase]
        public void ReferenceArgumentFactoryMethod()
        {
            string xaml = @"<Container 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <Container.Content>
                                    <ArgumentContainer>
                                      <x:Arguments>
                                        <x:String x:Name='arg1' x:FactoryMethod='Factory.CreateString'/>
                                      </x:Arguments>
                                    </ArgumentContainer>
                                </Container.Content>
                                <Container.SecondContent>
                                    <x:Reference>arg1</x:Reference>
                                </Container.SecondContent>
                            </Container>";
            var obj = XamlServices.Parse(xaml);
            if (!((obj as Container).SecondContent as string).Equals("Hello World"))
            {
                throw new TestCaseFailedException("Reference to an argument failed");
            }
        }

        /// <summary>
        /// Forward reference to a reference 
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void ForwardReferenceAReference()
        {
            string xaml = @"<List x:TypeArguments='s:Object' Capacity='4' 
                                    xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                    xmlns:s='clr-namespace:System;assembly=mscorlib'
                                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <s:Object x:Name='id0' />
                              <x:Reference x:Name='id1'>id0</x:Reference>
                              <x:Reference>id1</x:Reference>
                            </List>";
            var obj = XamlServices.Parse(xaml);
            List<object> list = obj as List<object>;
            if (list[0].GetHashCode() != list[1].GetHashCode() ||
                list[0].GetHashCode() != list[2].GetHashCode())
            {
                throw new TestCaseFailedException("backward Reference to reference failed");
            }
        }

        /// <summary>
        /// backward reference to a reference 
        /// </summary>
        
        // [DISABLED]
        // [TestCase]
        public void BackwardReferenceAReference()
        {
            string xaml = @"<List x:TypeArguments='s:Object' Capacity='4' 
                                    xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                    xmlns:s='clr-namespace:System;assembly=mscorlib'
                                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <x:Reference>id1</x:Reference>
                              <x:Reference x:Name='id1'>id0</x:Reference>                              
                              <s:Object x:Name='id0' />
                            </List>";
            var obj = XamlServices.Parse(xaml);
            List<object> list = obj as List<object>;
            if (list[0].GetHashCode() != list[1].GetHashCode() ||
                list[0].GetHashCode() != list[2].GetHashCode())
            {
                throw new TestCaseFailedException("backward Reference to reference failed");
            }
        }

        /// <summary>
        /// Reference a generic type 
        /// </summary>
        [TestCase]
        public void ReferenceAGeneric()
        {
            var list = new List<string> { "Hello", "world" };
            var obj = new Container()
            {
                Content = list,
                SecondContent = list,
            };
            XamlTestDriver.RoundTripCompare(obj);
        }

        /// <summary>
        /// generic type arguments
        /// </summary>
        [TestCase]
        public void GenericTypeArgs()
        {
            var obj = new List<List<int>>();
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (!xaml.Contains("List(x:Int32)"))
            {
                throw new TestCaseFailedException("generic type argument expected to go into params form");
            }
        }

        /// <summary>
        /// Markupextension is generic - 
        /// skip 'extension' from the name
        /// Regression test
        /// </summary>
        [TestCase]
        public void GenericMarkupExtensionSkipExtension()
        {
            string xaml = @"<StringContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes'
            xmlns:x2='http://schemas.microsoft.com/winfx/2006/xaml'>
                <StringContainer.Content>
                    <GenericMarkup x2:TypeArguments='x2:Int32, x2:Int32'>
                        <GenericMarkup.PropX> 
                            <x2:Int32>5</x2:Int32>
                        </GenericMarkup.PropX>
                    </GenericMarkup>
                </StringContainer.Content>
            </StringContainer>";

            var Container = XamlServices.Parse(xaml);
            if (!Container.ToString().Equals("[System.Collections.Generic.List`1[System.Int32],System.Collections.Generic.List`1[System.Int32]]"))
            {
                Tracer.LogTrace("Content did not match :");
                Tracer.LogTrace(Container.ToString());
                throw new TestCaseFailedException("Content did not match");
            }
        }

        /// <summary>
        /// Markupextension is generic - 
        /// with 'extension' in the name
        /// Regression test
        /// </summary>
        [TestCase]
        public void GenericMarkupExtension()
        {
            string xaml = @"<StringContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes'
            xmlns:x2='http://schemas.microsoft.com/winfx/2006/xaml'>
                <StringContainer.Content>
                    <GenericMarkupExtension x2:TypeArguments='x2:Int32, x2:Int32'>
                        <GenericMarkupExtension.PropX> 
                            <x2:Int32>5</x2:Int32>
                        </GenericMarkupExtension.PropX>
                    </GenericMarkupExtension>
                </StringContainer.Content>
            </StringContainer>";

            var Container = XamlServices.Parse(xaml);
            if (!Container.ToString().Equals("[System.Collections.Generic.List`1[System.Int32],System.Collections.Generic.List`1[System.Int32]]"))
            {
                Tracer.LogTrace("Content did not match :");
                Tracer.LogTrace(Container.ToString());
                throw new TestCaseFailedException("Content did not match");
            }
        }

        /// <summary>
        /// type argument in element form not supported
        /// </summary>
        [TestCase]
        public void TypeArgElement()
        {
            string xaml = @"<List
                            xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <x:TypeArguments>
                                    <x:String/>
                               </x:TypeArguments>
                           </List>";
            try
            {
                var obj = XamlServices.Parse(xaml);
                throw new TestCaseFailedException("Expected loading xaml to throw but it did not");
            }
            catch (XamlObjectWriterException)
            {
                Tracer.LogTrace("Caught expected exception");
            }
        }

        /// <summary>
        ///  ME value as x:Key in dictionary
        /// </summary>
        [TestCase]
        public void MeAsKey()
        {
            string xaml = @"<Dictionary x:TypeArguments='s:Object,s:Object' 
                                        xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                        xmlns:s='clr-namespace:System;assembly=mscorlib' 
                                        xmlns:mtxti='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <mtxti:CustomME Content='Hello' x:Key='world' />
                            </Dictionary>";

            var obj = XamlServices.Parse(xaml) as Dictionary<Object, Object>;
            var entry = obj["world"] as CustomMEClass;
            if (entry == null || (string)entry.Content != "Hello")
            {
                throw new TestCaseFailedException("ME as key failed to roundtrip");
            }
        }

        /// <summary>
        /// Template containing type converted object 
        /// </summary>
        [TestCase]
        public void TemplateWithTC()
        {
            string xaml = @"<TemplateContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <TemplateContainer.Template>
                                   <CustomType>$Hello$</CustomType>
                                </TemplateContainer.Template>
                            </TemplateContainer>";

            var container = XamlServices.Parse(xaml) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as CustomType;
            if (templateInstance.Content != "Hello")
            {
                throw new TestCaseFailedException("Type converter was not called on the template instance");
            }
        }

        /// <summary>
        /// Template containing markup extension 
        /// </summary>
        [TestCase]
        public void TemplateWithME()
        {
            string xaml5 = @"<TemplateContainer Template='{CustomME Content=Hello}'
                                            xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'/>";

            var container = XamlServices.Parse(xaml5) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as CustomMEClass;
            if ((string)templateInstance.Content != "Hello")
            {
                throw new TestCaseFailedException("MarkupExtension.ProvideValue was not called on the template instance");
            }
        }

        /// <summary>
        /// Template containing markup extension 
        /// </summary>
        [TestCase]
        public void TemplateWithMESkipProvideValue()
        {
            string xaml5 = @"<TemplateContainer Template='{CustomME Content=Hello}'
                                            xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'/>";

            var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml5)));
            var writer = new XamlObjectWriter(reader.SchemaContext);
            XamlServices.Transform(reader, writer);
            var container = (TemplateContainer)writer.Result;
            var templateInstance = container.Template
                .LoadTemplate(new XamlObjectWriterSettings { SkipProvideValueOnRoot = true }) as CustomME;
            if (templateInstance == null)
            {
                throw new TestCaseFailedException("MarkupExtension.ProvideValue was called on the template instance");
            }
        }

        /// <summary>
        /// Template within a template
        /// </summary>
        [TestCase]
        public void NestedTemplate()
        {
            string xaml = @"<TemplateContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <TemplateContainer.Template>
                                    <TemplateContainer>
                                        <TemplateContainer.Template>
                                            <x:String>Hello</x:String>
                                        </TemplateContainer.Template>
                                    </TemplateContainer>
                                </TemplateContainer.Template>
                            </TemplateContainer>";

            var container = XamlServices.Parse(xaml) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as TemplateContainer;
            var nestedTemplateInstance = templateInstance.Template.LoadTemplate(null) as string;
            if (nestedTemplateInstance != "Hello")
            {
                throw new TestCaseFailedException("nested template instance is incorrect");
            }
        }

        /// <summary>
        /// Template containing x:Arguments 
        /// </summary>
        [TestCase]
        public void TemplateWithxArgs()
        {
            string xaml = @"<TemplateContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <TemplateContainer.Template>
                                    <ArgumentContainer>
                                      <x:Arguments>
                                        <x:String>Hello</x:String>
                                      </x:Arguments>
                                    </ArgumentContainer>
                                </TemplateContainer.Template>
                            </TemplateContainer>";

            var container = XamlServices.Parse(xaml) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as ArgumentContainer;
            if ((templateInstance.Content as string) != "Hello")
            {
                throw new TestCaseFailedException("x:argument in template instance is incorrect");
            }
        }

        /// <summary>
        /// Template containing x:FactoryMethod
        /// </summary>
        [TestCase]
        public void TemplateWithFactoryMethod()
        {
            string xaml = @"<TemplateContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                        <TemplateContainer.Template>
                                           <ArgumentContainer x:FactoryMethod='ArgumentContainer.Create' />
                                        </TemplateContainer.Template>
                            </TemplateContainer>";

            var container = XamlServices.Parse(xaml) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as ArgumentContainer;
            if (((int)templateInstance.Content) != 20)
            {
                throw new TestCaseFailedException("x:argument in template instance is incorrect");
            }
        }

        /// <summary>
        /// Template containing generic type 
        /// </summary>
        [TestCase]
        public void TemplateWithGeneric()
        {
            string xaml = @"<TemplateContainer 
                                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Integration;assembly=XamlClrTypes' 
                                xmlns:m='clr-namespace:System.Collections.Generic;assembly=mscorlib' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                        <TemplateContainer.Template>
                                           <m:List x:TypeArguments='x:String'/>
                                        </TemplateContainer.Template>
                            </TemplateContainer>";

            var container = XamlServices.Parse(xaml) as TemplateContainer;
            var templateInstance = container.Template.LoadTemplate(null) as List<string>;
            if (templateInstance == null)
            {
                throw new TestCaseFailedException("generic type in template instance failed");
            }
        }
    }
}
