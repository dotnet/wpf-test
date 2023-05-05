// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xaml;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /// <summary>
    /// Class for the UnknownType/Property tests
    /// </summary>
    public static class UnknownElementTests
    {
        /// <summary>
        /// Test loads xaml file and checks each StratObject and StartProperty for
        /// expected Unknow_Types/Unknown_Properties.  When found, it verifies that
        /// the IsUnknown property is true
        /// </summary>
        public static void RunUnknownElementTest()
        {
            string xamlFile = DriverState.DriverParameters["TestParams"];
            var reader = new System.Xaml.XamlXmlReader(XmlReader.Create(xamlFile));
            InfosetProcessor ip = new InfosetProcessor(reader.SchemaContext);
            ip.WriteObjectDelegate = CheckWriteObject;
            ip.WriteMemberDelegate = CheckWriteMember;
            GlobalLog.LogStatus("Loading: " + xamlFile);
            XamlServices.Transform(reader, ip);
        }

        /// <summary>
        /// Checks the write object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteObject(XamlType xamlType, XamlSchemaContext context)
        {
            if (xamlType.Name == "Unknown_Type")
            {
                if (xamlType.IsUnknown)
                {
                    GlobalLog.LogStatus("Unknown_Type was marked Unknown");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Unknown_Type was not marked Unknown");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Checks the write member.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteMember(XamlMember property, XamlSchemaContext context)
        {
            if (property.Name == "Unknown_Property")
            {
                if (property.IsUnknown)
                {
                    GlobalLog.LogStatus("Unknown_Property was marked Unknown");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Unknown_Property was not marked Unknown");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }
    }
}
