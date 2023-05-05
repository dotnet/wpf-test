// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xaml.Permissions;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Utilities;

    public class XamlServicesApi
    {
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void XamlServiceLoad()
        {
            object obj = BasicPrimitiveTypes.GetTestCases()[0].Target;
            string xamlString = XamlServices.Save(obj);
            string fileName = XamlTestDriver.WriteToFile(obj, "BasicTypes.xaml");

            FileStream fs = new FileStream(fileName, FileMode.Open);
            object roundTripped = XamlServices.Load(fs);
            XamlObjectComparer.CompareObjects(obj, roundTripped);
            fs.Close();

            fs = new FileStream(fileName, FileMode.Open);
            roundTripped = XamlServices.Load(fs);
            XamlObjectComparer.CompareObjects(obj, roundTripped);
            fs.Close();

            roundTripped = XamlServices.Load(new StringReader(xamlString));
            XamlObjectComparer.CompareObjects(obj, roundTripped);

            fs = new FileStream(fileName, FileMode.Open);
            roundTripped = XamlServices.Load((System.Xaml.XamlReader) new XamlXmlReader(XmlReader.Create(fs)));
            XamlObjectComparer.CompareObjects(obj, roundTripped);
            fs.Close();
        }

        [SecurityLevel(SecurityLevel.FullTrust)]
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void XamlServiceLoadFromFile()
        {
            BasicPrimitiveTypes obj = (BasicPrimitiveTypes) BasicPrimitiveTypes.GetTestCases()[0].Target;
            string fileName = XamlTestDriver.WriteToFile(obj, "BasicPrimitiveTypes.xaml");

            object roundTripped = XamlServices.Load(fileName);

            XamlObjectComparer.CompareObjects(obj, roundTripped);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void XamlServiceLoadParse()
        {
            // XamlServices.Parse overloads
            object obj = BasicPrimitiveTypes.GetTestCases()[0].Target;
            string xamlString = XamlServices.Save(obj);

            object roundTripped = XamlServices.Parse(xamlString);

            XamlObjectComparer.CompareObjects(obj, roundTripped);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void XamlServiceLoadSave()
        {
            // XamlServices.Save overloads
            object obj = BasicPrimitiveTypes.GetTestCases()[0].Target;

            string xamlString = XamlServices.Save(obj);
            XamlTestDriver.XamlFirstCompareObjects(xamlString, obj);

            string fileName = DirectoryAssistance.GetArtifactDirectory(Guid.NewGuid() + "foo.xaml");
            FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
            XamlServices.Save(fileStream, obj);
            fileStream.Close();
            XamlTestDriver.XamlFirstCompareObjects(File.ReadAllText(fileName), obj);

            StringBuilder stringBuilder = new StringBuilder();
            XamlServices.Save(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), obj);
            object roundtripped = XamlServices.Load(new StringReader(stringBuilder.ToString()));
            XamlObjectComparer.CompareObjects(obj, roundtripped);

            fileName = DirectoryAssistance.GetArtifactDirectory(Guid.NewGuid() + "foo.xaml");
            using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true }))
            {
                XamlServices.Save(writer, obj);
            }
            XamlTestDriver.XamlFirstCompareObjects(File.ReadAllText(fileName), obj);
        }

        [TestCase]
        public void VerifyCorrectElementPrefix()
        {
            string xaml = @"<List x:TypeArguments='s:Object' Capacity='4' xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:s='clr-namespace:System;assembly=mscorlib'  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                              <ItemType ItemName='blah' Price='3.14' xmlns='clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes' />
                              <List x:TypeArguments='s:Object' Capacity='4'>
                                <x:String>blah</x:String>
                              </List>
                            </List>";

            XamlServices.Parse(xaml);
        }

        // [TestCase]
        // public void VerifyXamlLoadPermission()
        // {
            // XamlLoadPermission unrestrictedPerm = new XamlLoadPermission(PermissionState.Unrestricted);

            // if (!unrestrictedPerm.Includes(XamlAccessLevel.PrivateAccessTo(typeof(XamlServicesApi))))
            // {
                // throw new Exception("Expected permission.");
            // }
        // }
    }
}
