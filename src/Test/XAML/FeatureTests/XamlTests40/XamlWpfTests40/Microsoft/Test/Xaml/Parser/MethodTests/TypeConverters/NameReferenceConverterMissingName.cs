// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.ClrClasses;

namespace Microsoft.Test.Xaml.Dev10.Parser.MethodTests.TypeConverters
{
    /******************************************************************************
    * CLASS:          NameReferenceConverterMissingName
    ******************************************************************************/

    /// <summary>
    /// Verifies basic behavior of various TypeConverters.
    /// </summary>
    public class NameReferenceConverterMissingName
    {
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/

        /// <summary>
        /// Verifies basic behavior of the NameReferenceConverter.
        /// </summary>
        public void RunTest()
        {
            string xamlString = @"
             <CustomType1 
                xmlns='clr-namespace:Microsoft.Test.Xaml.Types.ClrClasses;assembly=XamlClrTypes'
                xmlns:s='clr-namespace:System;assembly=mscorlib'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <CustomType1.Property1>
                    id1
                </CustomType1.Property1>
                <CustomType1.Property2>
                    <x:String>GoodBye</x:String>
                </CustomType1.Property2>
              </CustomType1>";

            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xamlString), new XamlObjectWriterException(), "UnresolvedForwardReferences", WpfBinaries.SystemXaml);            
        }
    }
}
