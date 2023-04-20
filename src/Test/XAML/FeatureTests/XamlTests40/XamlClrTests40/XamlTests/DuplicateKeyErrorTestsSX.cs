// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml;
using Microsoft.Infrastructure.Test;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Globalization;
using Microsoft.Test.Xaml.Driver;

namespace Microsoft.Test.Xaml.XamlTests
{
    /******************************************************************************
    * Class:          DuplicateKeyErrorTestsSX
    ******************************************************************************/

    /// <summary>
    /// Duplicate x:Key tests (Regression tests)
    /// </summary>
    public class DuplicateKeyErrorTestsSX
    {
        /******************************************************************************
        * Function:          TEST 1: VerifyDuplicatePropertyElements
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys, each set as a property element.
        /// </summary>
        [TestCase]
        public void VerifyDuplicatePropertyElements()
        {
            string xamlString = @"
                <Dictionaries
                      x:TypeArguments='x:Int32, x:String'
                      xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                      xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Dictionaries.IDictionary>
                        <x:String>
                          <x:Key>
                            <x:Int32>1</x:Int32>
                          </x:Key>
                          <x:Key>
                            <x:Int32>2</x:Int32>
                          </x:Key>
                          2
                        </x:String>
                      </Dictionaries.IDictionary>
                </Dictionaries>";

            ExecuteTest(xamlString, XamlLanguage.String);
        }

        /******************************************************************************
        * Function:          TEST 2: VerifyDuplicateKeyMixed
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys, one set as a property, another as a property element.
        /// </summary>
        [TestCase]
        public void VerifyDuplicateKeyMixed()
        {
            string xamlString = @"
                <Dictionaries
                      x:TypeArguments='x:Int32, x:String'
                      xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                      xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Dictionaries.IDictionary>
                        <x:String x:Key='1'>
                          <x:Key>
                            <x:Int32>2</x:Int32>
                          </x:Key>
                          2
                        </x:String>
                      </Dictionaries.IDictionary>
                </Dictionaries>";

            ExecuteTest(xamlString, XamlLanguage.String);
        }

        /******************************************************************************
        * Function:          TEST 3: VerifyDuplicateKeyNested
        ******************************************************************************/

        /// <summary>
        ///  Verifies duplicate keys when nested.
        /// </summary>
        [TestCase]
        public void VerifyDuplicateKeyNested()
        {
            string xamlString = @"
                <Dictionaries
                      x:TypeArguments='x:Int32, x:String'
                      xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes'
                      xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Dictionaries.IDictionary>
                        <x:String>
                          <x:Key><x:Int32><x:Key>21</x:Key><x:Key>22</x:Key></x:Int32></x:Key>
                        </x:String>
                      </Dictionaries.IDictionary>
                </Dictionaries>";

            ExecuteTest(xamlString, XamlLanguage.Int32);
        }

        /******************************************************************************
        * Function:          ExecuteTest
        ******************************************************************************/

        /// <summary>
        ///  Call XamlServices.Transform and verify the exception.
        /// </summary>
        /// <param name="xamlString">The markup being tested.</param>
        /// <param name="parentType">The ParentType set on XamlDuplicateMemberException.</param>
        private void ExecuteTest(string xamlString, XamlType parentType)
        {
            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            XamlXmlReader reader = new XamlXmlReader(xmlReader, schemaContext);
            XamlObjectWriter writer = new XamlObjectWriter(reader.SchemaContext);

            string xamlNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";
            XamlTypeName xamlTypeName = new XamlTypeName(xamlNS, "Dictionaries");
            XamlSchemaContext xSC = new XamlSchemaContext();
            XamlType xamlType = xSC.GetXamlType(xamlTypeName);
            XamlMember xamlMember = XamlLanguage.Key;
            XamlDuplicateMemberException duplicateException = new XamlDuplicateMemberException(xamlMember, xamlType);
            duplicateException.ParentType = parentType;

            ExceptionHelper.ExpectException<XamlDuplicateMemberException>(() => XamlServices.Transform(reader, writer), duplicateException, "DuplicateMemberSet", WpfBinaries.SystemXaml);
        }
    }
}

