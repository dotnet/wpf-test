// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsPrefix("http:\\myfoo", "mtxxn")]
[assembly: XmlnsDefinition("http:\\myfoo", "Microsoft.Test.Xaml.XamlReaderWriterTests.namespace5")]

namespace Microsoft.Test.Xaml.XamlReaderWriterTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xaml;
    using System.Xaml.Schema;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Utilities;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace4;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace5;
    using Microsoft.Test.Xaml.XamlTests;


    public delegate string RunTest(XamlXmlWriter writer);

    public class XamlTextWriterTests
    {
        private const string namespace1 = "clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40";
        private const string namespace2 = "clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40";
        private const string namespace3 = "clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40";

        public string RunXTWTest(RunTest runTest)
        {
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Indent = true,
                                                          OmitXmlDeclaration = true
                                                      };
            string expectedXaml = null;
            using (XamlXmlWriter writer = new XamlXmlWriter(XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture),
                                                                             xmlWriterSettings), xamlSchemaContext))
            {
                expectedXaml = runTest(writer);
            }
            if (expectedXaml == null)
            {
                return stringBuilder.ToString();
            }

            if (!stringBuilder.ToString().Equals(expectedXaml))
            {
                Tracer.LogTrace("Expected:\r\n " + expectedXaml);
                Tracer.LogTrace("Actual:\r\n " + stringBuilder.ToString());
                throw new TestCaseFailedException("Xaml does not match");
            }
            return String.Empty;
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeWriteObject()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "mtx"));
                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);
                               writer.WriteEndObject();

                               string expectedXaml = @"<mtx:Point1 xmlns:mtx=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeStartMember()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));

                               XamlMember point2Type = pointType.GetMember("Point2");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.Point2 xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">20</Point1.Point2>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeSOandSM()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));

                               XamlMember point2Type = pointType.GetMember("Point2");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <ns1:Point1.Point2 xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">20</ns1:Point1.Point2>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void SameNsOnSOandSM()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlMember point2Type = pointType.GetMember("X");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <ns1:Point1.X xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">20</ns1:Point1.X>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void SameNsOnSOandSMDifferentPrefix()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns2"));

                               XamlMember point2Type = pointType.GetMember("X");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <ns2:Point1.X xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">20</ns2:Point1.X>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void DifferentNamespaceWithSamePrefix()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns1"));

                               XamlMember point2Type = pointType.GetMember("X");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.X xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">20</Point1.X>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void WriteDefaultNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, ""));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               XamlMember point2Type = pointType.GetMember("X");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 X=""20"" xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void OverrideWriteDefaultNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, ""));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, ""));

                               XamlMember point2Type = pointType.GetMember("Point2");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.Point2 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">20</Point1.Point2>
</Point1>";
                               return expectedXaml;
                           });
        }

        private void NearestNamespaceMethod(bool writeNSNodes, XamlXmlWriter writer)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            if (writeNSNodes)
            {
                writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));
            }
            {
                XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                writer.WriteStartObject(pointType);
                {
                    if (writeNSNodes)
                    {
                        writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));
                    }
                    {
                        XamlMember xProperty = pointType.GetMember("X");
                        writer.WriteStartMember(xProperty);
                        writer.WriteValue("10");
                        writer.WriteEndMember();
                    }

                    if (writeNSNodes)
                    {
                        writer.WriteNamespace(new NamespaceDeclaration(namespace3, "ns3"));
                    }
                    {
                        XamlMember point3Property = pointType.GetMember("Point3");
                        writer.WriteStartMember(point3Property);

                        XamlType point3Type = xamlSchemaContext.GetXamlType(typeof(Point3));
                        writer.WriteStartObject(point3Type);
                        writer.WriteStartMember(point3Type.GetMember("X"));
                        writer.WriteValue("20");
                        writer.WriteEndMember();
                        writer.WriteEndObject();

                        writer.WriteEndMember();
                    }
                }
                writer.WriteEndObject();
            }
        }

        // 10.	NS1 { SO, NS2 { SM(ns1) , V, EM }, NS3 { SM (ns3), V, EM }, EO }  (nearest namespace chosen)
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NearstNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               NearestNamespaceMethod(true, writer);

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <ns1:Point1.X xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">10</ns1:Point1.X>
  <ns1:Point1.Point3 xmlns:ns3=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"">
    <ns3:Point3 X=""20"" />
  </ns1:Point1.Point3>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeEONeg()
        {
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                                               () => RunXTWTest(delegate(XamlXmlWriter writer)
                                                                    {
                                                                        XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                        XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                        writer.WriteStartObject(pointType);

                                                                        XamlMember point2Type = pointType.GetMember("X");
                                                                        writer.WriteStartMember(point2Type);

                                                                        writer.WriteValue("20");

                                                                        writer.WriteEndMember();

                                                                        writer.WriteNamespace(new NamespaceDeclaration(namespace1, "foo"));

                                                                        writer.WriteEndObject();

                                                                        string expectedXaml = @"";
                                                                        return expectedXaml;
                                                                    }));
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeEMNeg()
        {
            ExceptionHelpers.CheckForException(typeof(XamlXmlWriterException),
                                               () =>
                                               RunXTWTest(delegate(XamlXmlWriter writer)
                                                              {
                                                                  XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                  XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                  writer.WriteStartObject(pointType);

                                                                  XamlMember point2Type = pointType.GetMember("X");
                                                                  writer.WriteStartMember(point2Type);

                                                                  writer.WriteValue("20");

                                                                  writer.WriteNamespace(new NamespaceDeclaration(namespace1, "foo"));

                                                                  writer.WriteEndMember();
                                                                  writer.WriteEndObject();

                                                                  string expectedXaml = @"";
                                                                  return expectedXaml;
                                                              }));
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NamespaceBeforeVNeg()
        {
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                                               () =>
                                               RunXTWTest(delegate(XamlXmlWriter writer)
                                                              {
                                                                  XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                  XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                  writer.WriteStartObject(pointType);

                                                                  XamlMember point2Type = pointType.GetMember("X");
                                                                  writer.WriteStartMember(point2Type);

                                                                  writer.WriteNamespace(new NamespaceDeclaration(namespace1, "foo"));
                                                                  writer.WriteValue("20");

                                                                  writer.WriteEndMember();
                                                                  writer.WriteEndObject();

                                                                  string expectedXaml = @"";
                                                                  return expectedXaml;
                                                              }));
        }

        //14.	NS1 NS2 { SO, {SM (ns1), V, EM}, { SM (ns2), V, EM},  EO}
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NearstNamespace2()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));
                               {
                                   XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                   writer.WriteStartObject(pointType);
                                   {
                                       // ns1
                                       {
                                           XamlMember xProperty = pointType.GetMember("X");
                                           writer.WriteStartMember(xProperty);
                                           writer.WriteValue("10");
                                           writer.WriteEndMember();
                                       }

                                       //ns2
                                       {
                                           XamlMember point2Property = pointType.GetMember("Point2");
                                           writer.WriteStartMember(point2Property);

                                           XamlType point2Type = xamlSchemaContext.GetXamlType(typeof(Point2));
                                           writer.WriteStartObject(point2Type);
                                           writer.WriteStartMember(point2Type.GetMember("X"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                           writer.WriteEndObject();

                                           writer.WriteEndMember();
                                       }
                                   }
                                   writer.WriteEndObject();
                               }
                               string expectedXaml = @"<ns1:Point1 X=""10"" xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">
  <ns1:Point1.Point2>
    <ns2:Point2 X=""20"" />
  </ns1:Point1.Point2>
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        //15.	SO {NS1, SM (ns1), V, EM} {SM (ns1), V, EM} EO (should add xmlns for second member because the first one is not in scope)
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void PrefixNotinScope()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               {
                                   XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                   writer.WriteStartObject(pointType);
                                   {
                                       writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                                       // ns1
                                       {
                                           XamlMember xProperty = pointType.GetMember("X");
                                           writer.WriteStartMember(xProperty);
                                           writer.WriteValue("10");
                                           writer.WriteEndMember();
                                       }

                                       // ns1
                                       {
                                           XamlMember xProperty = pointType.GetMember("Y");
                                           writer.WriteStartMember(xProperty);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                       }
                                   }
                                   writer.WriteEndObject();
                               }
                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <ns1:Point1.X xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">10</ns1:Point1.X>
  <Point1.Y>20</Point1.Y>
</Point1>";
                               return expectedXaml;
                           });
        }

        //16.	NS1, NS2, NS3, NS4 SO { SM (ns2), V, EM } EO  multiple namespaces
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void MultipleNamespaces()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace3, "ns3"));
                               {
                                   XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                   writer.WriteStartObject(pointType);
                                   {
                                       // ns1
                                       {
                                           XamlMember xProperty = pointType.GetMember("X");
                                           writer.WriteStartMember(xProperty);
                                           writer.WriteValue("10");
                                           writer.WriteEndMember();
                                       }
                                   }
                                   writer.WriteEndObject();
                               }
                               string expectedXaml = @"<ns1:Point1 X=""10"" xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" xmlns:ns2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" xmlns:ns3=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW, TestType = TestType.Automated)]
        public void WriteNamespaceBeforeWriteObject_ISOFM_True()
        {
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                                               () => RunXTWTest(delegate(XamlXmlWriter writer)
                                                                    {
                                                                        XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                        XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                        writer.WriteStartObject(pointType);
                                                                        {
                                                                            writer.WriteStartMember(pointType.GetMember("StringList"));
                                                                            {
                                                                                XamlType myListType = xamlSchemaContext.GetXamlType(typeof(MyList));
                                                                                writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));
                                                                                writer.WriteGetObject();
                                                                                writer.WriteEndObject();
                                                                            }
                                                                            writer.WriteEndMember();
                                                                        }
                                                                        writer.WriteEndObject();

                                                                        string expectedXaml = @"";
                                                                        return expectedXaml;
                                                                    }));
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW, TestType = TestType.Automated)]
        public void WriteNamespaceBeforeWriteMember_Implicit_True()
        {
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                                               () => RunXTWTest(delegate(XamlXmlWriter writer)
                                                                    {
                                                                        XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                        XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                        writer.WriteStartObject(pointType);
                                                                        {
                                                                            writer.WriteStartMember(pointType.GetMember("StringList"));
                                                                            {
                                                                                XamlType myList = xamlSchemaContext.GetXamlType(typeof(MyList));
                                                                                writer.WriteStartObject(myList);
                                                                                {
                                                                                    writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns2"));

                                                                                    writer.WriteStartMember(XamlLanguage.Items);
                                                                                    writer.WriteValue("20");
                                                                                    writer.WriteEndMember();
                                                                                }
                                                                                writer.WriteEndObject();
                                                                            }
                                                                            writer.WriteEndMember();
                                                                        }
                                                                        writer.WriteEndObject();

                                                                        return "";
                                                                    }));
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void WriteObjectNull()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlTypeName typeName = new XamlTypeName("http://schemas.microsoft.com/winfx/2006/xaml", "Null");
                               XamlType xamlType = writer.SchemaContext.GetXamlType(typeName);

                               writer.WriteStartObject(xamlType);
                               writer.WriteEndObject();

                               string expectedXaml = @"<Null xmlns=""http://schemas.microsoft.com/winfx/2006/xaml"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void TextWriterSettingsNeg()
        {
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Indent = true,
                                                          OmitXmlDeclaration = true
                                                      };

            using (XamlXmlWriter writer = new XamlXmlWriter(XmlWriter.Create(new StringWriter(stringBuilder, CultureInfo.InvariantCulture),
                                                                             xmlWriterSettings), xamlSchemaContext))
            {
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_atRootNeg()
        {
            ExceptionHelpers.CheckForException(typeof(XamlXmlWriterException),
                                               () =>
                                               RunXTWTest(delegate(XamlXmlWriter writer)
                                                              {
                                                                  XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                  XamlType myListType = xamlSchemaContext.GetXamlType(typeof(MyList));
                                                                  writer.WriteGetObject();
                                                                  writer.WriteEndObject();

                                                                  string expectedXaml = @"";
                                                                  return expectedXaml;
                                                              }));
        }

        public enum Implicit
        {
            None,
            Items,
            Init
        } ;

        public void CombinationTest(XamlXmlWriter writer, bool isOFM, Implicit implicitPropety, bool readOnlyCollection)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
            writer.WriteStartObject(pointType);
            {
                string listPropertyName = readOnlyCollection ? "StringList" : "RWStringList";
                writer.WriteStartMember(pointType.GetMember(listPropertyName));
                {
                    XamlType myList = xamlSchemaContext.GetXamlType(typeof(MyList));
                    if (isOFM)
                    {
                        writer.WriteGetObject();
                    }
                    else
                    {
                        writer.WriteStartObject(myList);
                    }
                    {
                        XamlMember property;

                        switch (implicitPropety)
                        {
                            case Implicit.None:
                                property = myList.GetMember("StringProperty");
                                break;
                            case Implicit.Init:
                                property = XamlLanguage.Initialization;
                                break;
                            case Implicit.Items:
                                property = XamlLanguage.Items;
                                break;
                            default:
                                throw new TestCaseFailedException("Unknown implicit proeprty");
                        }

                        writer.WriteStartMember(property);
                        writer.WriteValue("20");
                        writer.WriteEndMember();
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndMember();
            }
            writer.WriteEndObject();
        }

        public void CombinationTestMethod(string isOFM, string implicitProperty, string readOnlyCollection)
        {
            string xaml = RunXTWTest(delegate(XamlXmlWriter writer)
                                         {
                                             CombinationTest(writer, bool.Parse(isOFM), (Implicit)Enum.Parse(typeof(Implicit), implicitProperty), bool.Parse(readOnlyCollection));

                                             return null;
                                         });

            Tracer.LogTrace(String.Format(CultureInfo.InvariantCulture, "ISOFM = {0} ImplicitPropety={1} ReadOnlyCollection={2}", isOFM, implicitProperty, readOnlyCollection));
            Tracer.LogTrace("ActualXaml: " + xaml);

            if (bool.Parse(isOFM) == true)
            {
                if (xaml.Contains("<n:MyList>"))
                {
                    throw new TestCaseFailedException("IOFM=true but object tag was writtin");
                }
            }
            if (!implicitProperty.Equals(Implicit.None.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                if (xaml.Contains("Items"))
                {
                    throw new TestCaseFailedException("Implicit proepty written out to xaml");
                }

                if (xaml.Contains("Initialization"))
                {
                    throw new TestCaseFailedException("Implicit property written out to xaml");
                }
            }
        }

        [TestCaseGenerator()]
        public void CombinationTestGenerator(AddTestCaseEventHandler addCase)
        {
            bool[] isOFMs = {
                                true, false
                            };
            bool[] readOnlyCollections = {
                                             true, false
                                         };
            Implicit[] implicits = {
                                       Implicit.None, Implicit.Items, Implicit.Init
                                   };
            foreach (bool isOFM in isOFMs)
            {
                foreach (bool readOnly in readOnlyCollections)
                {
                    foreach (Implicit imp in implicits)
                    {
                        addCase(new TestCaseAttribute()
                                    {
                                        Author = "Microsoft",
                                        DisplayName = "ISOFMImplitRW_" + isOFM + imp + readOnly
                                    },
                                typeof(XamlTextWriterTests).GetMethod("CombinationTestMethod"),
                                isOFM.ToString(), imp.ToString(), readOnly.ToString());
                    }
                }
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_Implicit_True_InMiddle()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point2"));
                                   {
                                       XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteStartObject(point2);
                                       {
                                           writer.WriteStartMember(point2.GetMember("X"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           writer.WriteStartMember(point2.GetMember("Y"));
                                           writer.WriteValue("40");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       CombinationTest(writer, true, Implicit.Items, true);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point2>
    <mtxxn:Point2 X=""20"" Y=""40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
  </PointContainer.Point2>
  <PointContainer.Point1>
    <Point1>
      <Point1.StringList>20</Point1.StringList>
    </Point1>
  </PointContainer.Point1>
</PointContainer>";

                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_Implicit_False_InMiddle()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point2"));
                                   {
                                       XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteStartObject(point2);
                                       {
                                           writer.WriteStartMember(point2.GetMember("X"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           writer.WriteStartMember(point2.GetMember("Y"));
                                           writer.WriteValue("40");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       CombinationTest(writer, true, Implicit.None, true);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point2>
    <mtxxn:Point2 X=""20"" Y=""40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
  </PointContainer.Point2>
  <PointContainer.Point1>
    <Point1>
      <Point1.StringList>
        <MyList.StringProperty>20</MyList.StringProperty>
      </Point1.StringList>
    </Point1>
  </PointContainer.Point1>
</PointContainer>";

                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_False_Implicit_True_InMiddle()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "n"));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point2"));
                                   {
                                       XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteNamespace(new NamespaceDeclaration(namespace2, "n1"));
                                       writer.WriteStartObject(point2);
                                       {
                                           writer.WriteStartMember(point2.GetMember("X"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           writer.WriteStartMember(point2.GetMember("Y"));
                                           writer.WriteValue("40");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       CombinationTest(writer, false, Implicit.Items, true);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<n:PointContainer xmlns:n=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <n:PointContainer.Point2>
    <n1:Point2 X=""20"" Y=""40"" xmlns:n1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
  </n:PointContainer.Point2>
  <n:PointContainer.Point1>
    <n:Point1>
      <n:Point1.StringList>
        <n:MyList>20</n:MyList>
      </n:Point1.StringList>
    </n:Point1>
  </n:PointContainer.Point1>
</n:PointContainer>";

                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_False_Implicit_False_InMiddle()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "n"));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point2"));
                                   {
                                       XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteNamespace(new NamespaceDeclaration(namespace2, "n1"));
                                       writer.WriteStartObject(point2);
                                       {
                                           writer.WriteStartMember(point2.GetMember("X"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           writer.WriteStartMember(point2.GetMember("Y"));
                                           writer.WriteValue("40");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       CombinationTest(writer, false, Implicit.None, true);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<n:PointContainer xmlns:n=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <n:PointContainer.Point2>
    <n1:Point2 X=""20"" Y=""40"" xmlns:n1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
  </n:PointContainer.Point2>
  <n:PointContainer.Point1>
    <n:Point1>
      <n:Point1.StringList>
        <n:MyList StringProperty=""20"" />
      </n:Point1.StringList>
    </n:Point1>
  </n:PointContainer.Point1>
</n:PointContainer>";

                               return expectedXaml;
                           });
        }

        //SO
        //   SM (ISOFM = true)
        //    SM (implicit = true)
        //    V
        //    EM
        //    SM (Implicit = false)
        //    V
        //    EM
        //   EM
        //EO
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ImplicitAndNonImplicitUnderISOFMtrue()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);
                               {
                                   // Implicit //
                                   writer.WriteStartMember(pointType.GetMember("StringList"));
                                   {
                                       XamlType myList = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           writer.WriteStartMember(XamlLanguage.Items);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   // Non implicit //
                                   writer.WriteStartMember(pointType.GetMember("X"));
                                   writer.WriteValue("40");
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.StringList>20</Point1.StringList>
  <Point1.X>40</Point1.X>
</Point1>";
                               return expectedXaml;
                           });
        }

        // Does not throw now - Will throw once validation is added //
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void TwoImplicitMembersUnderISOFMtrueNeg()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);
                               {
                                   // Implicit //
                                   writer.WriteStartMember(pointType.GetMember("StringList"));
                                   {
                                       XamlType myList = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           writer.WriteStartMember(XamlLanguage.Items);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           writer.WriteStartMember(XamlLanguage.Initialization);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.StringList>2020</Point1.StringList>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void NonImplicitMemberUnderImplicit()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);
                               {
                                   // Implicit //
                                   writer.WriteStartMember(pointType.GetMember("StringList"));
                                   {
                                       XamlType myList = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           XamlMember property = XamlLanguage.Items;

                                           writer.WriteStartMember(property);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();

                                           // Non implicit //
                                           writer.WriteStartMember(myList.GetMember("StringProperty"));
                                           writer.WriteValue("40");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               // This doesnt look right? //
                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.StringList>20<MyList.StringProperty>40</MyList.StringProperty></Point1.StringList>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ContentSyntax()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               XamlMember point2Type = pointType.GetMember("MyContent");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 MyContent=""20"" xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void WriteNullValue()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               XamlMember point2Type = pointType.GetMember("MyContent");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue(null);

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Null xmlns=""http://schemas.microsoft.com/winfx/2006/xaml"" />
</ns1:Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_AttachedProperty()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "n"));
                               writer.WriteStartObject(pointType);
                               {
                                   writer.WriteStartMember(pointType.GetMember("StringList"));
                                   {
                                       XamlType myListType = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           XamlType apsType = xamlSchemaContext.GetXamlType(typeof(AttachedPropertySource));
                                           XamlMember attachableProperty = apsType.GetAttachableMember("BoolProp");

                                           writer.WriteStartMember(attachableProperty);
                                           writer.WriteValue("True");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<n:Point1 xmlns:n=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <n:Point1.StringList>
    <AttachedPropertySource.BoolProp xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.AttachedProperties;assembly=XamlClrTypes"">True</AttachedPropertySource.BoolProp>
  </n:Point1.StringList>
</n:Point1>";
                               return expectedXaml;
                           });
        }

        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_ContentProperty()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "n"));
                               writer.WriteStartObject(pointType);
                               {
                                   writer.WriteStartMember(pointType.GetMember("Point2"));
                                   {
                                       XamlType myListType = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           writer.WriteStartMember(myListType.GetMember("MyContent"));
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<n:Point1 xmlns:n=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <n:Point1.Point2>20</n:Point1.Point2>
</n:Point1>";
                               return expectedXaml;
                           });
        }

        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ISOFM_True_DirectiveProperty()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "n"));
                               writer.WriteStartObject(pointType);
                               {
                                   writer.WriteStartMember(pointType.GetMember("Point2"));
                                   {
                                       XamlType myListType = xamlSchemaContext.GetXamlType(typeof(MyList));
                                       writer.WriteGetObject();
                                       {
                                           XamlMember property = xamlSchemaContext.GetXamlDirective("http://schemas.microsoft.com/winfx/2006/xaml", "Name");
                                           writer.WriteStartMember(property);
                                           writer.WriteValue("MyName");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<n:Point1 xmlns:n=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <n:Point1.Point2>
    <Name xmlns=""http://schemas.microsoft.com/winfx/2006/xaml"">MyName</Name>
  </n:Point1.Point2>
</n:Point1>";
                               return expectedXaml;
                           });
        }

        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void DictionaryWithKey()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType dictionary = xamlSchemaContext.GetXamlType(typeof(Dictionary<int, int>));
                               writer.WriteStartObject(dictionary);
                               {
                                   writer.WriteStartMember(XamlLanguage.Items);
                                   {
                                       XamlType intType = xamlSchemaContext.GetXamlType(typeof(int));
                                       writer.WriteStartObject(intType);
                                       {
                                           XamlMember property = xamlSchemaContext.GetXamlDirective("http://schemas.microsoft.com/winfx/2006/xaml", "Key");
                                           writer.WriteStartMember(property);
                                           writer.WriteValue("20");
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               // x:Key needs to have x defined in the same tag ...//
                               string expectedXaml = @"<Dictionary xmlns=""clr-namespace:System.Collections.Generic;assembly=mscorlib"">
  <x:TypeArguments xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">x:Int32, x:Int32</x:TypeArguments>
  <x:Int32 x:Key=""20"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />
</Dictionary>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void WriteDoubleValueNeg()
        {
            ExceptionHelpers.CheckForException(typeof(ArgumentException),
                                               () =>
                                               RunXTWTest(delegate(XamlXmlWriter writer)
                                                              {
                                                                  XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                                                                  writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns1"));

                                                                  XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                                                                  writer.WriteStartObject(pointType);

                                                                  XamlMember point2Type = pointType.GetMember("MyContent");
                                                                  writer.WriteStartMember(point2Type);

                                                                  writer.WriteValue((double)20.5);

                                                                  writer.WriteEndMember();

                                                                  writer.WriteEndObject();

                                                                  string expectedXaml = @"<ns1:Point1 xmlns:ns1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40""><x:Null /></ns1:Point1>";
                                                                  return expectedXaml;
                                                              }));
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void DefaultNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               XamlMember point2Type = pointType.GetMember("X");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 X=""20"" xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void OverrideDefaultNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, ""));

                               XamlMember point2Type = pointType.GetMember("Point2");
                               writer.WriteStartMember(point2Type);

                               writer.WriteValue("20");

                               writer.WriteEndMember();

                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.Point2 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">20</Point1.Point2>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ConflictingNamespacesInScope()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               XamlType pointType = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(pointType);
                               {
                                   XamlMember point2Property = pointType.GetMember("Point2");
                                   writer.WriteStartMember(point2Property);
                                   {
                                       XamlType point2Type = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteStartObject(point2Type);
                                       writer.WriteStartMember(point2Type.GetMember("X"));
                                       writer.WriteValue("20");
                                       writer.WriteEndMember();
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();

                                   XamlMember point3Property = pointType.GetMember("Point3");
                                   writer.WriteStartMember(point3Property);
                                   {
                                       XamlType point3Type = xamlSchemaContext.GetXamlType(typeof(Point3));
                                       writer.WriteStartObject(point3Type);
                                       writer.WriteStartMember(point3Type.GetMember("X"));
                                       writer.WriteValue("20");
                                       writer.WriteEndMember();
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <Point1.Point2>
    <mtxxn:Point2 X=""20"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
  </Point1.Point2>
  <Point1.Point3>
    <mtxxn1:Point3 X=""20"" xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />
  </Point1.Point3>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void TwoBranchesOutOfScope()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       WritePoint1(writer, true, false);
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point3"));
                                   {
                                       WritePoint3(writer);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point1>
    <Point1 X=""20"" Y=""40"">
      <Point1.Point2>
        <mtxxn:Point2 X=""20"" Y=""40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
      </Point1.Point2>
    </Point1>
  </PointContainer.Point1>
  <PointContainer.Point3>
    <mtxxn1:Point3 X=""20"" Y=""40"" xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />
  </PointContainer.Point3>
</PointContainer>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void TwoInScopeOneOutOfScope()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       WritePoint1(writer, true, true);
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point4"));
                                   {
                                       WritePoint4(writer);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point1>
    <Point1 X=""20"" Y=""40"">
      <Point1.Point2>
        <mtxxn:Point2 X=""20"" Y=""40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
      </Point1.Point2>
      <Point1.Point3>
        <mtxxn1:Point3 X=""20"" Y=""40"" xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />
      </Point1.Point3>
    </Point1>
  </PointContainer.Point1>
  <PointContainer.Point4>
    <mtxxn2:Point4 X=""20"" Y=""40"" xmlns:mtxxn2=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace4;assembly=XamlClrTests40"" />
  </PointContainer.Point4>
</PointContainer>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void TwoInScopeOneOutOfScopeWhichMatchesOneInScope()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       WritePoint1(writer, true, true);
                                   }
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(pointContainerType.GetMember("Point3"));
                                   {
                                       WritePoint3(writer);
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point1>
    <Point1 X=""20"" Y=""40"">
      <Point1.Point2>
        <mtxxn:Point2 X=""20"" Y=""40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
      </Point1.Point2>
      <Point1.Point3>
        <mtxxn1:Point3 X=""20"" Y=""40"" xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />
      </Point1.Point3>
    </Point1>
  </PointContainer.Point1>
  <PointContainer.Point3>
    <mtxxn1:Point3 X=""20"" Y=""40"" xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"" />
  </PointContainer.Point3>
</PointContainer>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void XmlnsPrefixAttributeTest()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));

                               XamlType point5 = xamlSchemaContext.GetXamlType(typeof(Point5));
                               writer.WriteNamespace(new NamespaceDeclaration("", ""));
                               writer.WriteStartObject(point5);
                               {
                                   writer.WriteStartMember(point5.GetMember("X"));
                                   writer.WriteValue("20");
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(point5.GetMember("Y"));
                                   writer.WriteValue("40");
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               // mtxxn should be chosen over the default namespace since this namespace has an xmlns prefix attribute defined //
                               string expectedXaml = @"<mtxxn:Point5 X=""20"" Y=""40"" xmlns="""" xmlns:mtxxn=""http:\myfoo"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void OverrideUsingXmlnsPrefixAttributeTest()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));

                               // take up both mtxxn and the empty namespace //
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "mtxxn"));
                               writer.WriteNamespace(new NamespaceDeclaration("", ""));

                               XamlType point5 = xamlSchemaContext.GetXamlType(typeof(Point5));
                               writer.WriteStartObject(point5);
                               {
                                   writer.WriteStartMember(point5.GetMember("X"));
                                   writer.WriteValue("20");
                                   writer.WriteEndMember();

                                   writer.WriteStartMember(point5.GetMember("Y"));
                                   writer.WriteValue("40");
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<mtxxn1:Point5 X=""20"" Y=""40"" xmlns="""" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" xmlns:mtxxn1=""http:\myfoo"" />";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ImplicitRedefineOfShadowedNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));

                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, "ns"));
                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteNamespace(new NamespaceDeclaration(namespace2, "ns"));
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       XamlType point1 = xamlSchemaContext.GetXamlType(typeof(Point1));
                                       writer.WriteStartObject(point1);
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               // namespace1 should get redefined since Point1 needs this shadowed namespace //
                               string expectedXaml = @"<ns:PointContainer xmlns:ns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"" xmlns:ns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">
    <Point1 />
  </PointContainer.Point1>
</ns:PointContainer>";
                               return expectedXaml;
                           });
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void OverrideAutoGeneratedNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));

                               writer.WriteStartObject(pointContainerType);
                               {
                                   // override default namespace with namespace2 //
                                   writer.WriteNamespace(new NamespaceDeclaration(namespace2, ""));
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       XamlType point1 = xamlSchemaContext.GetXamlType(typeof(Point1));
                                       writer.WriteStartObject(point1);
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <mtxxn:PointContainer.Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
    <mtxxn:Point1 />
  </mtxxn:PointContainer.Point1>
</PointContainer>";
                               return expectedXaml;
                           });
        }

        // Auto generate ns - then shadow using wNS - 
        // write something in attribute form from first namespace 
        // ( needs to redefine  - but cannot do in attribute form) - should write in element form
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ShadowForceElementForm()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;
                               XamlType pointContainerType = xamlSchemaContext.GetXamlType(typeof(PointContainer));

                               writer.WriteStartObject(pointContainerType);
                               {
                                   writer.WriteStartMember(pointContainerType.GetMember("Point1"));
                                   {
                                       XamlType point1 = xamlSchemaContext.GetXamlType(typeof(Point1));
                                       writer.WriteStartObject(point1);
                                       {
                                           // write Name //
                                           XamlMember xNameProperty = xamlSchemaContext.GetXamlDirective("http://schemas.microsoft.com/winfx/2006/xaml", "Name");
                                           writer.WriteNamespace(new NamespaceDeclaration(Namespaces.Namespace2006, "x"));
                                           writer.WriteStartMember(xNameProperty);
                                           writer.WriteValue("Name1");
                                           writer.WriteEndMember();

                                           // shadow the x: namespace //
                                           writer.WriteNamespace(new NamespaceDeclaration(namespace2, "x"));
                                           // point 2 //
                                           writer.WriteStartMember(point1.GetMember("Point2"));
                                           {
                                               XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                               writer.WriteStartObject(point2);
                                               {
                                                   // write Name //
                                                   // x is shadowed - name has to be written in element form //
                                                   writer.WriteStartMember(xNameProperty);
                                                   writer.WriteValue("Name2");
                                                   writer.WriteEndMember();
                                               }
                                               writer.WriteEndObject();
                                           }
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<PointContainer xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <PointContainer.Point1>
    <Point1>
      <x:Name xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">Name1</x:Name>
      <Point1.Point2 xmlns:x=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"">
        <x:Point2>
          <x1:Name xmlns:x1=""http://schemas.microsoft.com/winfx/2006/xaml"">Name2</x1:Name>
        </x:Point2>
      </Point1.Point2>
    </Point1>
  </PointContainer.Point1>
</PointContainer>";
                               return expectedXaml;
                           });
        }

        //Shadow same prefix twice - try using something from second namespace - should redefine.//
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = TestType.Automated)]
        public void ShadowPrefixTwiceRedefineSecondNamespace()
        {
            RunXTWTest(delegate(XamlXmlWriter writer)
                           {
                               XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

                               // default = ns1 //
                               writer.WriteNamespace(new NamespaceDeclaration(namespace1, ""));
                               XamlType point1 = xamlSchemaContext.GetXamlType(typeof(Point1));
                               writer.WriteStartObject(point1);
                               {
                                   // default = ns2 //
                                   writer.WriteNamespace(new NamespaceDeclaration(namespace2, ""));
                                   // point 2 property //
                                   writer.WriteStartMember(point1.GetMember("Point2"));
                                   {
                                       XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
                                       writer.WriteStartObject(point2);
                                       {
                                           writer.WriteStartMember(point2.GetMember("Point3"));
                                           {
                                               XamlType point3 = xamlSchemaContext.GetXamlType(typeof(Point3));
                                               // default = ns 3 //
                                               writer.WriteNamespace(new NamespaceDeclaration(namespace3, ""));
                                               writer.WriteStartObject(point3);
                                               {
                                                   writer.WriteStartMember(point3.GetMember("Point2"));
                                                   {
                                                       // ns2 should get redefined here //
                                                       writer.WriteStartObject(point2);
                                                       {
                                                       }
                                                       writer.WriteEndObject();
                                                   }
                                               }
                                           }
                                           writer.WriteEndMember();
                                       }
                                       writer.WriteEndObject();
                                   }
                                   writer.WriteEndMember();
                               }
                               writer.WriteEndObject();

                               string expectedXaml = @"<Point1 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
  <mtxxn:Point1.Point2 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" xmlns:mtxxn=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40"">
    <Point2>
      <Point2.Point3>
        <Point3 xmlns=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;assembly=XamlClrTests40"">
          <Point3.Point2>
            <mtxxn1:Point2 xmlns:mtxxn1=""clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;assembly=XamlClrTests40"" />
          </Point3.Point2>
        </Point3>
      </Point2.Point3>
    </Point2>
  </mtxxn:Point1.Point2>
</Point1>";
                               return expectedXaml;
                           });
        }

        [TestCase]
        public void DependsOnAttribute()
        {
            Depend obj = new Depend()
            {
                Property1 = new MyString("One"),
                Property2 = new MyString("Two"),
                Property3 = new MyString("Three")
            };

            string xaml = XamlServices.Save(obj);
            int index3 = xaml.IndexOf("Three");
            int index2 = xaml.IndexOf("Two");
            int index1 = xaml.IndexOf("One");

            if ((index3 > index2) || (index2 > index1))
            {
                throw new DataTestException("The order of properties not based on dependson attribute");
            }
        }

        // [DISABLED]
        // [TestCase]
        public void DependsOnAttributeCircular1()
        {
            DependCircular1 obj = new DependCircular1()
            {
                Property1 = new MyString("Hello"),
                Property2 = new MyString("World")
            };

            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                 () => XamlServices.Save(obj));
        }

        // [DISABLED]
        // [TestCase]
        public void DependsOnAttributeCircular2()
        {
            DependCircular2 obj = new DependCircular2()
            {
                Property1 = new MyString("Hello"),
            };
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException),
                () => XamlServices.Save(obj));
        }

        [TestCase]
        public void XamlXmlWriterFlush()
        {
            var obj = new MyString("Hello");

            XamlObjectReader reader = new XamlObjectReader(obj);
            StringWriter stringWriter = new StringWriter();
            XamlXmlWriter writer = new XamlXmlWriter(stringWriter, reader.SchemaContext);

            //Run XAML Node Pump
            while (reader.Read())
            {
                writer.WriteNode(reader);
            }

            // Should not have to call flush or close //
            // writer.Flush();
            // writer.Close();  

            if (String.IsNullOrEmpty(stringWriter.ToString()))
            {
                throw new TestCaseFailedException("XamlXmlWriter did not flush the output on last write");
            }
        }

        [TestCase]
        public void DisposeShouldCloseOpenRecords()
        {
            var obj = new MyString("Hello");

            XamlObjectReader reader = new XamlObjectReader(obj);
            StringWriter stringWriter = new StringWriter();
            XamlXmlWriter writer = new XamlXmlWriter(stringWriter, reader.SchemaContext);

            for (int i = 0; i < 5; i++)
            {
                reader.Read();
                writer.WriteNode(reader);
            }
            ((IDisposable)writer).Dispose();

            if (!stringWriter.ToString().EndsWith("/>"))
            {
                Tracer.LogTrace("Serialized Xaml is " + stringWriter.ToString());
                throw new TestCaseFailedException("XamlXmlWriter when disposed did not close open records");
            }
        }

        [TestCase]
        public void WriteAfterDisposeThrows()
        {
            StringWriter stringWriter = new StringWriter();
            XamlXmlWriter writer = new XamlXmlWriter(stringWriter, new XamlSchemaContext());
            writer.Close();
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteEndMember());
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteEndObject());
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteGetObject());
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteNamespace(null));
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteStartMember(null));
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteStartObject(null));
            ExceptionHelpers.CheckForException(typeof(ObjectDisposedException), () => writer.WriteValue(null));
        }

        [TestCase]
        public void PreserveSingleSpace()
        {
            var obj = "Hello World";
            XamlTestDriver.RoundTripCompare(obj);
        }

        [TestCase]
        public void PreserveDoubleSpace()
        {
            var obj = "Hello  World";
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (!xaml.Contains(@"xml:space=""preserve"""))
            {
                throw new TestCaseFailedException("xml:space=preserve not found");
            }
        }

        [TestCase]
        public void PreserveNewline()
        {
            var obj = "Hello \n World";

            string xaml = XamlTestDriver.RoundTripCompare(obj);

            if (!xaml.Contains(@"xml:space=""preserve"""))
            {
                throw new TestCaseFailedException("xml:space=preserve not found");
            }
        }

        [TestCase]
        public void PreserveTabs()
        {
            // Please make sure that the string has tabs. VS editor updates tabs to spaces
            var obj = @"Hello		World";
            string xaml = XamlTestDriver.RoundTripCompare(obj);
            if (!xaml.Contains(@"xml:space=""preserve"""))
            {
                throw new TestCaseFailedException("xml:space=preserve not found");
            }
        }

        [TestCase]
        public void PositionalParameters()
        {
            string xaml = @"<AnswerHolder Answer='{AddExtension 20,22}' xmlns='clr-namespace:Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;assembly=XamlClrTests40' />";
            string xaml2 = null;
            using (XamlReader reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml))))
            {
                StringWriter stringWriter = new StringWriter();
                XamlWriter writer = new XamlXmlWriter(stringWriter, reader.SchemaContext);

                // The transform used to throw. The validation is to make sure that
                // it does not throw any more
                XamlServices.Transform(reader, writer);
                xaml2 = stringWriter.ToString();
                Tracer.LogTrace("Xaml is " + xaml2);
            }

            var obj = XamlServices.Parse(xaml) as AnswerHolder;
            if (obj.Answer != 42)
            {
                throw new TestCaseFailedException("Answer is not 42. MarkupExtension.ProvideValue not called");
            }

            obj = XamlServices.Parse(xaml2) as AnswerHolder;
            if (obj.Answer != 42)
            {
                throw new TestCaseFailedException("Answer is not 42. MarkupExtension.ProvideValue not called on roundtripped Xaml");
            }
        }

        /// <summary>
        /// Use XamlServices to create XAML with nested xml:space elements
        /// </summary>
        [TestCase]
        public void WriteNestedXmlSpaceXamlServices()
        {
            var list = new AnimalList
            {
                new Animal { Name = "Billy   Giraffe", Number = 3 },
                new Animal { Name = "Tommy   Tiger", Number = 6 }
            };

            var strings = new List<string>
            {
                "\tstring  with\tspaces"
            };

            AttachedPropertySource.SetIListProp(list, strings);

            var testCaseInfo = new TestCaseInfo()
            {
                TestID = "",
                Target = list,
                ExpectedResult = true,
                CompareAttachedProperties = true,
            };

            new ObjectDoubleRoundtripDriver().Execute("ObjectFirstTest", testCaseInfo);
            
        }

        /// <summary>
        /// Use the XamlXmlWriter to create XAML with nested xml:space elements
        /// </summary>
        [TestCase]
        public void WriteNestedXmlSpaceXamlXmlWriter()
        {
            var context = new XamlSchemaContext();

            var arrayList = context.GetXamlType(typeof(ArrayList));
            var xaml = new StringWriter();
            var xxw = new XamlXmlWriter(xaml, context);

            xxw.WriteStartObject(arrayList);
            xxw.WriteStartMember(XamlLanguage.Items);
            xxw.WriteValue("\tstring with\tspaces");
            xxw.WriteStartObject(XamlLanguage.String);
            xxw.WriteStartMember(XamlLanguage.Initialization);
            xxw.WriteValue("\tstring with\tspaces");
            xxw.WriteEndMember();
            xxw.WriteEndObject();
            xxw.WriteEndMember();
            xxw.WriteEndObject();

            var expected = new ArrayList
            {
                "\tstring with\tspaces",
                "\tstring with\tspaces"
            };

            var actual = XamlServices.Parse(xaml.ToString());

            XamlObjectComparer.CompareObjects(expected, actual);
        }

        private void WritePoint1(XamlXmlWriter writer, bool writePoint2, bool writePoint3)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType point1 = xamlSchemaContext.GetXamlType(typeof(Point1));
            writer.WriteStartObject(point1);
            {
                writer.WriteStartMember(point1.GetMember("X"));
                writer.WriteValue("20");
                writer.WriteEndMember();

                writer.WriteStartMember(point1.GetMember("Y"));
                writer.WriteValue("40");
                writer.WriteEndMember();

                if (writePoint2)
                {
                    writer.WriteStartMember(point1.GetMember("Point2"));
                    WritePoint2(writer);
                    writer.WriteEndMember();
                }
                if (writePoint3)
                {
                    writer.WriteStartMember(point1.GetMember("Point3"));
                    WritePoint3(writer);
                    writer.WriteEndMember();
                }
            }
            writer.WriteEndObject();
        }

        private void WritePoint2(XamlXmlWriter writer)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType point2 = xamlSchemaContext.GetXamlType(typeof(Point2));
            writer.WriteStartObject(point2);
            {
                writer.WriteStartMember(point2.GetMember("X"));
                writer.WriteValue("20");
                writer.WriteEndMember();

                writer.WriteStartMember(point2.GetMember("Y"));
                writer.WriteValue("40");
                writer.WriteEndMember();
            }
            writer.WriteEndObject();
        }

        private void WritePoint3(XamlXmlWriter writer)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType point3 = xamlSchemaContext.GetXamlType(typeof(Point3));
            writer.WriteStartObject(point3);
            {
                writer.WriteStartMember(point3.GetMember("X"));
                writer.WriteValue("20");
                writer.WriteEndMember();

                writer.WriteStartMember(point3.GetMember("Y"));
                writer.WriteValue("40");
                writer.WriteEndMember();
            }
            writer.WriteEndObject();
        }

        private void WritePoint4(XamlXmlWriter writer)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType point4 = xamlSchemaContext.GetXamlType(typeof(Point4));
            writer.WriteStartObject(point4);
            {
                writer.WriteStartMember(point4.GetMember("X"));
                writer.WriteValue("20");
                writer.WriteEndMember();

                writer.WriteStartMember(point4.GetMember("Y"));
                writer.WriteValue("40");
                writer.WriteEndMember();
            }
            writer.WriteEndObject();
        }

        private void WritePoint5(XamlXmlWriter writer)
        {
            XamlSchemaContext xamlSchemaContext = writer.SchemaContext;

            XamlType point5 = xamlSchemaContext.GetXamlType(typeof(Point5));
            writer.WriteStartObject(point5);
            {
                writer.WriteStartMember(point5.GetMember("X"));
                writer.WriteValue("20");
                writer.WriteEndMember();

                writer.WriteStartMember(point5.GetMember("Y"));
                writer.WriteValue("40");
                writer.WriteEndMember();
            }
            writer.WriteEndObject();
        }
    }
}

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace4;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace5;

    public class PointContainer
    {
        public Point1 Point1 { get; set; }
        public Point2 Point2 { get; set; }
        public Point3 Point3 { get; set; }
        public Point4 Point4 { get; set; }
        public Point5 Point5 { get; set; }
    }

    [ContentProperty("MyContent")]
    public class Point1
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int MyContent { get; set; }

        public Point2 Point2 { get; set; }
        public Point3 Point3 { get; set; }

        private MyList _stringList = new MyList();
        public MyList StringList
        {
            get
            {
                return this._stringList;
            }
        }

        public MyList RWStringList
        {
            get
            {
                return this._stringList;
            }
            set
            {
                this._stringList = value;
            }
        }
    }

    [ContentProperty("MyContent")]
    public class MyList : List<string>
    {
        public string StringProperty { get; set; }

        public int MyContent { get; set; }
    }
    public class Depend
    {
        [DependsOn("Property2")]
        public MyString Property1 { get; set; }
        [DependsOn("Property3")]
        public MyString Property2 { get; set; }
        public MyString Property3 { get; set; }
    }

    public class DependCircular1
    {
        [DependsOn("Property2")]
        public MyString Property1 { get; set; }
        [DependsOn("Property1")]
        public MyString Property2 { get; set; }
    }

    public class DependCircular2
    {
        [DependsOn("Property1")]
        public MyString Property1 { get; set; }
    }

    public class MyString
    {
        public string String { get; set; }
        public MyString()
        {

        }
        public MyString(string val)
        {
            this.String = val;
        }
    }

    public class AddExtension : MarkupExtension
    {
        int _a,_b;
        public AddExtension(int a, int b)
        {
            this._a = a;
            this._b = b;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this._a + this._b;
        }
    }

    public class AnswerHolder
    {
        public int Answer { get; set; }
    }
}

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2
{
    using System.Windows.Markup;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace1;
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3;

    [ContentProperty("MyContent")]
    public class Point2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point3 Point3 { get; set; }

        public int MyContent { get; set; }

        private readonly MyList _stringList = new MyList();
        public MyList StringList
        {
            get
            {
                return this._stringList;
            }
        }
    }
}

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.namespace3
{
    using Microsoft.Test.Xaml.XamlReaderWriterTests.namespace2;

    public class Point3
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2 Point2 { get; set; }
    }
}

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.namespace4
{
    public class Point4
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}

// This namespace has a prefix defined above //

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.namespace5
{
    public class Point5
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
