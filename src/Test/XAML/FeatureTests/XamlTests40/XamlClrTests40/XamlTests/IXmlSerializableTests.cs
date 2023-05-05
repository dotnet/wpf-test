// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types.IXmlSerializableTypes;

    public class IXmlSerializableTests
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
                                                                          "xx", "clr-namespace:Microsoft.Test.Xaml.Types.IXmlSerializableTypes;assembly=XamlClrTypes"
                                                                          },
                                                                      {
                                                                          "p", "http://schemas.microsoft.com/netfx/2008/xaml/schema"
                                                                          }
                                                                  };

        [TestCase]
        public void SimpleIXmlSerializableTest()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            TypeContaingingIXmlSerializableProperty obj = new TypeContaingingIXmlSerializableProperty();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:TypeContaingingIXmlSerializableProperty/xx:TypeContaingingIXmlSerializableProperty.IxmlProperty/x:XData"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void SimpleIXmlSerializableTestNested()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            TypeContaingingNestedIXmlSerializableProperty obj = new TypeContaingingNestedIXmlSerializableProperty();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };
            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:TypeContaingingNestedIXmlSerializableProperty/xx:TypeContaingingNestedIXmlSerializableProperty.IxmlProperty/x:XData"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void SimpleIXmlSerializableNotMarkedContentTestNested()
        {
            // verify IXmlSerializable RO props are written as XData even
            // when not marked with DesignerSerializationVisibility.Content
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            var obj = new TypeContaingingNestedIXmlSerializablePropertyNotMarkedContent();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Auto
            };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:TypeContaingingNestedIXmlSerializablePropertyNotMarkedContent/xx:TypeContaingingNestedIXmlSerializablePropertyNotMarkedContent.IxmlProperty/x:XData"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void EmptyXml()
        {
            string data = @"";

            TypeContaingingIXmlSerializableProperty obj = new TypeContaingingIXmlSerializableProperty();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            try
            {
                using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
                {
                    XamlServices.Save(writer, obj);
                }
            }
            catch (XmlException ex)
            {
                Tracer.LogTrace("expected exception recieved - " + ex.Message);
            }
        }

        [TestCase]
        public void MultipleIxmlProperties()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            MultipleIxmlProperties obj = new MultipleIxmlProperties();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:MultipleIxmlProperties/xx:MultipleIxmlProperties.IxmlProperty/x:XData",
                                                        @"/xx:MultipleIxmlProperties/xx:MultipleIxmlProperties.IxmlProperty2/x:XData"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void PreserveCommentsAndCdata()
        {
            string data = @"<![CDATA[CDATA should be preserved]]>
                    <?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some><x:XData xmlns:x=""http:\\schemas.microsoft.com/winfx/2006/xaml"">Something</x:XData>";

            TypeContaingingIXmlSerializableProperty obj = new TypeContaingingIXmlSerializableProperty();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            if (!xaml.Contains(@"<![CDATA[CDATA should be preserved]]>") ||
                !xaml.Contains(@"<!-- some comments -->") ||
                !xaml.Contains(@"<x:XData xmlns:x=""http:\\schemas.microsoft.com/winfx/2006/xaml"">Something</x:XData>"))
            {
                throw new TestCaseFailedException("serialzied xaml does not contain the Ixml data - serialized xaml is " + xaml);
            }
        }

        [TestCase]
        public void IxmlDictionary()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            TypeContaingingIXmlDictionary obj = new TypeContaingingIXmlDictionary();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:TypeContaingingIXmlDictionary/xx:TypeContaingingIXmlDictionary.IxmlProperty/x:XData"
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void IxmlCollection()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            TypeContaingingIXmlCollection obj = new TypeContaingingIXmlCollection();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            XamlTestDriver.RoundTripCompareExamineXaml(obj,
                                                new string[]
                                                    {
                                                        @"/xx:TypeContaingingIXmlCollection/xx:TypeContaingingIXmlCollection.IxmlProperty/x:XData"
                                                    },
                                                namespaces);
        }

        // 29585 //
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void XmlReaderClosedOnInitializeXdataProperty()
        {
            string data = @"<?Processing Inst?>
                    <!-- some comments -->
                    <some xmlns=""boo""><xml isparsed=""true""/></some>";

            TypeContaingingIXmlSerializableProperty obj = new TypeContaingingIXmlSerializableProperty();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                                                      {
                                                          ConformanceLevel = ConformanceLevel.Auto
                                                      };
            using (XmlReader reader = XmlReader.Create(new StringReader(data), xmlReaderSettings))
            {
                obj.IxmlProperty.ReadXml(reader);
            }

            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                                                      {
                                                          Indent = true, OmitXmlDeclaration = true
                                                      };

            using (var writer = XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture), xmlWriterSettings))
            {
                XamlServices.Save(writer, obj);
            }
            string xaml = stringBuilder.ToString();

            string fileName = XamlTestDriver.WriteToFile(xaml, "xdata.xaml");
            var roundTripped = XamlServices.Load(fileName);

            if (!File.Exists(fileName))
            {
                throw new TestCaseFailedException("Saved file does not exist");
            }
            File.Delete(fileName);

            if (File.Exists(fileName))
            {
                throw new TestCaseFailedException("Unable to delete file - somebody has a lock on to it");
            }
        }
    }
}
