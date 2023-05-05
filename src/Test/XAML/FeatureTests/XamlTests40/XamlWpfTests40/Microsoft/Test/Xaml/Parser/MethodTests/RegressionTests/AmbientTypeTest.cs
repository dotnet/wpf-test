// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    public class AmbientTypeTest
    {
        /// <summary>
        /// Lookup AmbientType in ME 
        /// </summary>
        public static void RegressionIssue139()
        {
            string xaml = @"<AmbientType xmlns='clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests;assembly=XamlWpfTests40' Prop1='hello' Prop2='{AmbientTypeME}'/>";
            AmbientType obj = (AmbientType)XamlServices.Parse(xaml);

            if (obj.Prop1 != (string)obj.Prop2)
            {
                GlobalLog.LogEvidence("AmbientType lookup failed");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("AmbientType lookup passed");
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }

    /// <summary>
    /// An ambient type
    /// </summary>
    [Ambient]
    public class AmbientType
    {
        /// <summary>
        /// Gets or sets Prop1
        /// </summary>
        public string Prop1 { get; set; }
        
        /// <summary>
        /// Gets or sets Prop2
        /// </summary>
        public object Prop2 { get; set; }
    }

    /// <summary>
    /// ME that gets an Ambient Type
    /// </summary>
    public class AmbientTypeME : MarkupExtension
    {
        /// <summary>
        /// Get AmbientType and return value of Prop1
        /// </summary>
        /// <param name="serviceProvider">Service provider to use</param>
        /// <returns>result of ambient type lookup</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IXamlSchemaContextProvider schemaProvider = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
            XamlSchemaContext xsc = schemaProvider.SchemaContext;
            IAmbientProvider ambient = serviceProvider.GetService(typeof(IAmbientProvider)) as IAmbientProvider;
            XamlType ambientType = xsc.GetXamlType(typeof(AmbientType));
            AmbientType obj = ambient.GetFirstAmbientValue(ambientType) as AmbientType;
            return obj.Prop1;
        }
    }
}
