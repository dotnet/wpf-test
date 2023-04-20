// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xaml;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Globalization;
using Microsoft.Test.Xaml.Types.ClrClasses;
using Microsoft.Test.Xaml.Types.SDX;

namespace Microsoft.Test.Xaml.NonParserMethodTests
{
    /// <summary>
    /// ObjectWriter BasicTests
    /// </summary>
    public static class ObjectWriterBasicTests
    {
        /// <summary>
        /// Namespacesprefixvalidation test.
        /// </summary>
        public static void NamespacePrefixValidationTest()
        {
            string xaml = @"
                        <NamespacePrefixValidation xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.SDX;assembly=XamlClrTypes"" xmlns:s=""clr-namespace:System;assembly=mscorlib"">
                            <NamespacePrefixValidation.BarContainer>
                                <BarContainer xmlns:st=""clr-namespace:System.Threading;assembly=System"" xmlns:scg=""clr-namespace:System.Collection.Generic;assembly=System"" >
                                    <BarContainer.Bar>1</BarContainer.Bar>
                                </BarContainer>
                            </NamespacePrefixValidation.BarContainer>
                            <NamespacePrefixValidation.FooContainer>
                                <FooContainer xmlns:sx=""clr-namespace:System.Xaml;assembly=System.Xaml"" xmlns:s=""clr-namespace:System.Collections.Generic;assembly=System"">
                                    <FooContainer.Foo>4</FooContainer.Foo>
                                </FooContainer>
                            </NamespacePrefixValidation.FooContainer>
                            <NamespacePrefixValidation.FooContainerTemplate>
                                <FooContainer xmlns:sx=""clr-namespace:System.Xaml;assembly=System.Xaml"" xmlns:st=""clr-namespace:System.Threading;assembly=System"" xmlns:scg=""clr-namespace:System.Collection.Generic;assembly=System"">
                                    <FooContainer.Foo>6</FooContainer.Foo>
                                </FooContainer>
                            </NamespacePrefixValidation.FooContainerTemplate>
                        </NamespacePrefixValidation>
                        ";
            NamespacePrefixValidation npv = XamlServices.Parse(xaml) as NamespacePrefixValidation;

            FooContainer container1 = npv.FooContainerTemplate();
            FooContainer container2 = npv.FooContainerTemplate();
        }

        /// <summary>
        /// Generics the dictionary key attribute.
        /// </summary>
        public static void GenericDictionaryKeyAttribute()
        {
            string xaml = @"
                <Dictionary x:TypeArguments=""s:Int32, s:String"" xmlns=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <s:String x:Key=""1"">One</s:String>
                </Dictionary>
                ";
            Dictionary<int, string> dictionary = (Dictionary<int, string>)XamlServices.Parse(xaml);
        }

        /// <summary>
        /// Generics the dictionary key element.
        /// </summary>
        public static void GenericDictionaryKeyElement()
        {
            string xaml = @"
                <Dictionary x:TypeArguments=""s:Int32, s:String"" xmlns=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <s:String><x:Key>1</x:Key>One</s:String>
                </Dictionary>
                ";
            Dictionary<int, string> dictionary = (Dictionary<int, string>)XamlServices.Parse(xaml);
        }

        public static void ArgumentsOutOfOrder()
        {
            string xaml =
            "<StringBuilder xmlns='clr-namespace:System.Text;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>"
                + "<StringBuilder.Length>50</StringBuilder.Length>"
                + "<x:Arguments>"
                    + "<x:String>foo</x:String>"
                + "</x:Arguments>"
            + "</StringBuilder>";
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xaml), new XamlObjectWriterException(), "LateConstructionDirective", WpfBinaries.SystemXaml);
         }

        public static void ArrayOfListOfT()
        {
            string xaml =
            @"<ArrayContainer xmlns='clr-namespace:Microsoft.Test.Xaml.Types.ClrClasses;assembly=XamlClrTypes'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                xmlns:s='clr-namespace:System.Collections.Generic;assembly=mscorlib'>
                    <ArrayContainer.ListArray>
                        <s:List x:TypeArguments='x:Int32'>
                            <x:Int32>12</x:Int32>
                            <x:Int32>13</x:Int32>
                        </s:List>
                    </ArrayContainer.ListArray>
                </ArrayContainer>";

            ArrayContainer arrayList = XamlServices.Parse(xaml) as ArrayContainer;
        }
    }
}
