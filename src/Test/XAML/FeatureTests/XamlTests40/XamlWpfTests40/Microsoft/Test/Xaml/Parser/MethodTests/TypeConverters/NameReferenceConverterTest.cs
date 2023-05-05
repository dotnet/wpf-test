// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.ClrClasses;

namespace Microsoft.Test.Xaml.Dev10.Parser.MethodTests.TypeConverters
{
    /******************************************************************************
    * CLASS:          NameReferenceConverterTest
    ******************************************************************************/

    /// <summary>
    /// Verifies basic behavior of various TypeConverters.
    /// </summary>
    public class NameReferenceConverterTest
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
                    <x:String x:Name='id1'>Hello</x:String>
                </CustomType1.Property2>
              </CustomType1>";
          
            CustomType1 customType1 = ((CustomType1)XamlServices.Parse(xamlString));

            string expectedValue = customType1.Property2.ToString();
            string actualValue = customType1.Property1.ToString();

            // The test passes if the content of custom Property1 is converted to the content of Property2.
            if (actualValue == expectedValue)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Expected: " + expectedValue + " / Actual: " + actualValue);
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
