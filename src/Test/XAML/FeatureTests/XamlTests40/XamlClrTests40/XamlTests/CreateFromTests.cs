// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CDF.Test.TestCases.Xaml.XamlTests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;
    using CDF.Test.TestCases.Xaml.Types.CreateFrom;
    using CDF.Test.TestCases.Xaml.Driver.XamlReaderWriter;
    using CDF.Test.TestCases.Xaml.Common.XamlOM;
    using CDF.Test.Common;
    using Microsoft.Infrastructure.Test;
    using CDF.Test.Common.TestObjects.Utilities;
    using CDF.Test.TestCases.Xaml.Driver;
    using Microsoft.Test.CDFInfrastructure;

    public class CreateFromTests
    {
        public static string localNamespace = "clr-namespace:CDF.Test.TestCases.Xaml.Types.CreateFrom;assembly=CDF.Test.TestCases.Xaml";

        [TestCase]
        public void SingleCreateFrom()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            //string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //               <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //               xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //                  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //               </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                    {
                            Helpers.GetxCreateFromMember(filePath, false),
                    }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        [TestCase]
        public void MergePropertiesParentOverridesChild()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            //string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //              <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //               xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
            //               IntProperty=""100"" StringProperty=""child"" >
            //                  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //              </xc:Bar>";

            bar.IntProperty = 100;
            bar.StringProperty = "child";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          Helpers.CreateMember("IntProperty", 100, true),
                          Helpers.CreateMember("StringProperty", "child", true),
                          Helpers.GetxCreateFromMember(filePath, false),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        // 
        [TestCase]
        public void MergePropertiesAttributes()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            // string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //                  <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //                   xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //                   <IntProperty>100</IntProperty>
            //                   <StringProperty>child</StringProperty>
            //                   <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //                 </xc:Bar>";

            bar.IntProperty = 100;
            bar.StringProperty = "child";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          Helpers.CreateMember("IntProperty", 100, false),
                          Helpers.CreateMember("StringProperty", "child", false),
                          Helpers.GetxCreateFromMember(filePath, false),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        [TestCase]
        public void MergePropertiesParentChildUnion()
        {
            Bar bar = new Bar() { StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            //  string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //                 <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //                   xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
            //                   IntProperty=""5"" >
            //                 <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //                </xc:Bar>";

            bar.IntProperty = 5;

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          Helpers.CreateMember("IntProperty", 5, true),
                          Helpers.GetxCreateFromMember(filePath, false),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        [TestCase]
        public void MergePropertiesParentSetsNone()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //    xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //   <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            // </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          Helpers.GetxCreateFromMember(filePath, false),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        [TestCase]
        public void MergePropertiesChildSetsNone()
        {
            Bar bar = new Bar() { };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //  xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
            //  IntProperty=""100"" StringProperty=""Hello World"">
            //  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //</xc:Bar>";

            bar.IntProperty = 100;
            bar.StringProperty = "Hello World";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          Helpers.CreateMember("IntProperty", 100, true),
                          Helpers.CreateMember("StringProperty", "Hello World", true),
                          Helpers.GetxCreateFromMember(filePath, false),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
        }

        // 
        [TestCase]
        public void NestedCreateFrom()
        {
            Nest2 nest2 = new Nest2() { StringProperty = "Nested2" };
            Nest1 nest1 = new Nest1() { StringProperty = "Nested1", Nest2 = nest2 };
            NestedRoot nestRoot = new NestedRoot() { StringProperty = "NestRoot", Nest1 = nest1 };

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Nest2 StringProperty=""Nested2"" xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" />";
            string nest2Path = Helpers.WriteToFile(nest2, "nest2.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //  <xc:Nest1 StringProperty=""Nested1"" 
            //          xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //           xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //     <xc:Nest1.Nest2 x2:CreateFrom='nest2.xaml'/>
            //  </xc:Nest1>";
            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Nest1"),
                    ExpectedNamespaces = 
                    {
                        GraphNodeXaml.Xaml2008Ns
                    },
                    Children =
                      {
                          Helpers.CreateMember("StringProperty", "Nested1", true),
                          new GraphNodeMember{
                              MemberName = "Nest2",
                              Children = 
                              {
                                  new GraphNodeRecord
                                    {
                                        RecordName = GraphNodeXaml.BuildXName(localNamespace, "Nest2"),
                                        Children = {
                                            Helpers.GetxCreateFromMember(nest2Path, true),    
                                        }
                                    },
                              }
                          },
                      }
                }
            };
            string nest1xaml = Helpers.Serialize(xamlDoc);
            string nest1Path = Helpers.WriteToFile(nest1xaml, "nest1.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:NestedRoot StringProperty=""NestRoot"" xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //    xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //   <xc:NestedRoot.Nest1>
            //       <xc:Nest1>
            //          <x2:CreateFrom>nest1.xaml</x2:CreateFrom>
            //       </xc:Nest1>
            //   </xc:NestedRoot.Nest1>
            // </xc:NestedRoot>";

            XamlDocument xamlDocRoot = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "NestedRoot"),
                    ExpectedNamespaces = 
                    {
                        GraphNodeXaml.Xaml2008Ns
                    },
                    Children =
                      {
                          Helpers.CreateMember("StringProperty", "NestRoot", true),
                          new GraphNodeMember{
                              MemberName =  "Nest1",
                              Children = 
                              {
                                  new GraphNodeRecord
                                    {
                                        RecordName = GraphNodeXaml.BuildXName(localNamespace,"Nest1"),
                                        Children = {
                                            Helpers.GetxCreateFromMember(nest1Path, false),    
                                        }
                                   },
                                                     
                              }
                          },
                      }
                }
            };

            string xaml1 = Helpers.XamlFirstCompareObjects(xamlDoc, nest1);
            // 
            string xaml = Helpers.XamlFirstCompareObjects(xamlDocRoot, nestRoot);

        }

        // 
        [TestCase]
        public void NestedCreateFromElement()
        {
            Nest2 nest2 = new Nest2() { StringProperty = "Nested2" };
            Nest1 nest1 = new Nest1() { StringProperty = "Nested1", Nest2 = nest2 };
            NestedRoot nestRoot = new NestedRoot() { StringProperty = "NestRoot", Nest1 = nest1 };

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Nest2 StringProperty=""Nested2"" xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" />";
            string nest2Path = Helpers.WriteToFile(nest2, "nest2.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Nest1 StringProperty=""Nested1""
            //     xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //    <xc:Nest1.Nest2>
            //       <x2:CreateFrom>nest2.xaml</x2:CreateFrom>
            //    </xc:Nest1.Nest2>
            //  </xc:Nest1>";
            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Nest1"),
                    ExpectedNamespaces = 
                    {
                        GraphNodeXaml.Xaml2008Ns
                    },
                    Children =
                      {
                          Helpers.CreateMember("StringProperty", "Nested1", true),
                          new GraphNodeMember{
                              MemberName = "Nest2",
                              Children = 
                              {  
                                  new GraphNodeRecord
                                    {
                                        RecordName = GraphNodeXaml.BuildXName(localNamespace, "Nest2"),
                                        Children = {
                                            Helpers.GetxCreateFromMember(nest2Path, false),    
                                        }
                                    },
                              }
                          },
                      }
                }
            };

            string nest1xaml = Helpers.Serialize(xamlDoc);
            string nest1Path = Helpers.WriteToFile(nest1xaml, "nest1.xaml");

            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //  <xc:NestedRoot StringProperty=""NestRoot"" 
            //      xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests""
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //    <xc:NestedRoot.Nest1>
            //       <xc:Nest1>
            //          <x2:CreateFrom>nest1.xaml</x2:CreateFrom>
            //       </xc:Nest1>
            //    </xc:NestedRoot.Nest1>
            //   </xc:NestedRoot>";
            XamlDocument xamlDocRoot = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "NestedRoot"),
                    ExpectedNamespaces = 
                    {
                        GraphNodeXaml.Xaml2008Ns
                    },
                    Children =
                      {
                          Helpers.CreateMember("StringProperty", "NestRoot", true),
                          new GraphNodeMember{
                              MemberName =  "Nest1",
                              Children = 
                              {
                                  new GraphNodeRecord
                                    {
                                        RecordName = GraphNodeXaml.BuildXName(localNamespace,"Nest1"),
                                        Children = {
                                            Helpers.GetxCreateFromMember(nest1Path, false),    
                                        }
                                   },
                                                     
                              }
                          },
                      }
                }
            };

            string xaml1 = Helpers.XamlFirstCompareObjects(xamlDoc, nest1);
            string xaml = Helpers.XamlFirstCompareObjects(xamlDocRoot, nestRoot);

        }

        // 
        [TestCase]
        public void MultipleCreateFrom()
        {
            Child1 child1 = new Child1() { IntProperty = 5, StringProperty = "woo hoo" };
            Child2 child2 = new Child2() { IntProperty = 15, StringProperty = "I am child 2" };
            MultipleRoot root = new MultipleRoot() { Child1 = child1, Child2 = child2 };

            string child1Path = Helpers.WriteToFile(child1, "child1.xaml");
            string child2Path = Helpers.WriteToFile(child2, "child2.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:MultipleRoot xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //      <xc:MultipleRoot.Child1>
            //        <x2:CreateFrom>child1.xaml</x2:CreateFrom>
            //      </xc:MultipleRoot.Child1>
            //      <xc:MultipleRoot.Child2>
            //        <x2:CreateFrom>child2.xaml</x2:CreateFrom>
            //      </xc:MultipleRoot.Child2>
            //    </xc:MultipleRoot>";
            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "MultipleRoot"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                          new GraphNodeMember
                          {
                              MemberName = "Child1",
                              Children = 
                              {
                                  new GraphNodeRecord
                                    {
                                           RecordName = GraphNodeXaml.BuildXName(localNamespace, "Child1"),
                                           Children = 
                                           {
                                               Helpers.GetxCreateFromMember(child1Path, false),
                                           }
                                    }
                              }
                          }
                         ,
                         new GraphNodeMember
                          {
                              MemberName = "Child2",
                              Children = 
                              {
                                  new GraphNodeRecord
                                  {
                                           RecordName = GraphNodeXaml.BuildXName(localNamespace, "Child2"),
                                           Children = 
                                           {
                                               Helpers.GetxCreateFromMember(child2Path, false),
                                           }
                                    }
                              }
                          },
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, root);

        }

        [TestCase]
        public void CreateFromDifferentLocations()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string barLocalPath = Helpers.WriteToFile(bar, "barLocal.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //     <x2:CreateFrom>barLocal.xaml</x2:CreateFrom>
            //    </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                            Helpers.GetxCreateFromMember(barLocalPath, false),
                      }
                }
            };
            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);


            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //    xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //  <x2:CreateFrom>..\barRelative.xaml</x2:CreateFrom>
            // </xc:Bar>";

            string barRelative = Helpers.WriteToFile(bar, @"\..\barRelative.xaml");
            xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                            Helpers.GetxCreateFromMember(barRelative, false),
                      }
                }
            };
            xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);

            string absPath = Helpers.WriteToFile(bar, @"barAbsolute.xaml");
            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //   <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //     xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //      <x2:CreateFrom>" + path + @"</x2:CreateFrom>
            //   </xc:Bar>";
            xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                      {
                            Helpers.GetxCreateFromMember(absPath, false),
                      }
                }
            };
            xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);


            // 

            // 
        }

        [TestCase]
        public void URINotPresent()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string path = DirectoryAssistance.GetArtifactDirectory("barLocal.xaml");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //   <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests""
            //      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //      <x2:CreateFrom>barLocal.xaml</x2:CreateFrom>
            //   </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
             {
                 Root = new GraphNodeRecord
                 {
                     ExpectedNamespaces = 
                     {
                         GraphNodeXaml.Xaml2008Ns,
                     },
                     RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                     Children =
                      {
                            Helpers.GetxCreateFromMember(path, false),
                      }
                 }
             };

            try
            {
                string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
            }
            catch (WebException)
            {
                Tracer.LogTrace("Got the expected exception");
            }
        }

        [TestCase]
        public void InvalidXamlAtUri()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string InvalidXaml = @"invalid xaml";
            string path = Helpers.WriteToFile(InvalidXaml, "invalid.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //     <x2:CreateFrom>invalid.xaml</x2:CreateFrom>
            //  </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    Children =
                      {
                            Helpers.GetxCreateFromMember(path, false),
                      }
                }
            };

            try
            {
                string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
            }
            catch (XmlException)
            {
                Tracer.LogTrace("Got the expected exception");
                return;
            }
            throw new TestCaseFailedException("Should not reach here");
        }

        // 
        [TestCase]
        public void CircularInstantiation()
        {
            string child1str = @"<CircleChild1 xmlns=""clr-namespace:CDF.Test.TestCases.Xaml.Types.CreateFrom;assembly=CDF.Test.TestCases.Xaml"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
              <CircleChild1.Child2>
                <CircleChild2 x2:CreateFrom=""{0}"" />
              </CircleChild1.Child2>
            </CircleChild1>";

            string child2str = @"<CircleChild2 xmlns=""clr-namespace:CDF.Test.TestCases.Xaml.Types.CreateFrom;assembly=CDF.Test.TestCases.Xaml"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
              <CircleChild2.Child1>
                <CircleChild1 x2:CreateFrom=""{0}"" />
              </CircleChild2.Child1>
            </CircleChild2>";

            string fileName1 = Helpers.WriteToFile(child1str, "Child1.xaml");
            string fileName2 = Helpers.WriteToFile(child2str, "Child2.xaml");
            child1str = string.Format(child1str, fileName2);
            child2str = string.Format(child2str, fileName1);

            Helpers.WriteToAbsoluteFile(child1str, fileName1);
            Helpers.WriteToAbsoluteFile(child2str, fileName2);

            XamlServices.LoadFromUri(new Uri(fileName1));
        }

        [TestCase]
        public void CreateFromNameScope()
        {
            MyFoo foo = new MyFoo() { IntProperty = 5, StringProperty = "hello" };
            MyFoo foo2 = new MyFoo() { IntProperty = 15, StringProperty = "hello world" };

            Node Node1 = new Node() { Foo1 = foo, Foo2 = foo };
            Node Node2 = new Node() { Foo1 = foo2, Foo2 = foo2 };

            NameRoot root = new NameRoot() { Node1 = Node1, Node2 = Node2 };

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:Node xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //        <xc:Node.Foo1>
            //           <xc:MyFoo x:Name=""FOO"" IntProperty=""5"" StringProperty=""hello"" />
            //        </xc:Node.Foo1>
            //        <xc:Node.Foo2>
            //           <x2:Reference>FOO</x2:Reference>
            //        </xc:Node.Foo2>
            //    </xc:Node>";
            XamlDocument xamlDoc1 = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                    Children = {
                          new GraphNodeMember
                          {
                              MemberName = "Foo1",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "MyFoo"),
                                      Children = {
                                          Helpers.CreateMember("IntProperty", 5, true),
                                          Helpers.CreateMember("StringProperty", "hello", true),
                                          Helpers.GetxNameMember("FOO", true),
                                      }
                                  }
                              }
                          },
                          new GraphNodeMember
                          {
                              MemberName = "Foo2",
                              Children = {
                                     Helpers.GetxRefRecord("FOO", false),
                              }
                          }
                       }
                }
            };
            string Node1xaml = Helpers.Serialize(xamlDoc1);
            string node1Path = Helpers.WriteToFile(Node1xaml, "node1.xaml");


            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //   <xc:Node xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //     <xc:Node.Foo1>
            //        <xc:MyFoo x:Name=""FOO"" IntProperty=""15"" StringProperty=""hello world"" />
            //     </xc:Node.Foo1>
            //     <xc:Node.Foo2>
            //        <x2:Reference>FOO</x2:Reference>
            //     </xc:Node.Foo2>
            //   </xc:Node>";
            XamlDocument xamlDoc2 = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                    Children = {
                          new GraphNodeMember
                          {
                              MemberName = "Foo1",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "MyFoo"),
                                      Children = {
                                          Helpers.CreateMember("IntProperty", 15, true),
                                          Helpers.CreateMember("StringProperty", "hello world", true),
                                          Helpers.GetxNameMember("FOO", true),
                                      }
                                  }
                              }
                          },
                           new GraphNodeMember
                          {
                              MemberName = "Foo2",
                              Children = {
                                     Helpers.GetxRefRecord("FOO", false),
                              }
                          }
                       }
                }
            };
            string Node2xaml = Helpers.Serialize(xamlDoc2);
            string node2Path = Helpers.WriteToFile(Node2xaml, "node2.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:NameRoot xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"">
            //   <xc:NameRoot.Node1>
            //     <xc:Node x2:CreateFrom='node1.xaml'/>
            //   </xc:NameRoot.Node1>
            //   <xc:NameRoot.Node2>
            //         <xc:Node x2:CreateFrom='node2.xaml'/>
            //   </xc:NameRoot.Node2>
            // </xc:NameRoot>";

            XamlDocument xamlDocRoot = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "NameRoot"),
                    Children = {
                          new GraphNodeMember
                          {
                              MemberName = "Node1",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                                      Children = {
                                          Helpers.GetxCreateFromMember(node1Path, true),
                                      }
                                  }
                              }
                          },
                          new GraphNodeMember
                          {
                              MemberName = "Node2",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                                      Children = {
                                          Helpers.GetxCreateFromMember(node2Path, true),
                                      }
                                  }
                              }
                          },
                       }
                }
            };


            Helpers.Deserialize(Node1xaml, null);
            Helpers.Deserialize(Node2xaml, null);

            Helpers.XamlFirstCompareObjects(xamlDocRoot, root);
        }

        [TestCase]
        public void CreateFromReferenceOutsideFileScope()
        {
            MyFoo foo = new MyFoo() { IntProperty = 5, StringProperty = "hello" };
            Node Node1 = new Node() { Foo1 = foo, Foo2 = foo };
            NameRoot root = new NameRoot() { Foo = foo, Node1 = Node1 };

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:Node xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //     xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //      <xc:Node.Foo1>
            //        <xc:MyFoo x:Name=""FOO1"" IntProperty=""5"" StringProperty=""hello"" />
            //      </xc:Node.Foo1>
            //      <xc:Node.Foo2>
            //        <x2:Reference>FOO3</x2:Reference>
            //      </xc:Node.Foo2>
            //    </xc:Node>";
            XamlDocument xamlDoc1 = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                    Children = {
                          new GraphNodeMember
                          {
                              MemberName = "Foo1",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "MyFoo"),
                                      Children = {
                                          Helpers.CreateMember("IntProperty", 5, true),
                                          Helpers.CreateMember("StringProperty", "hello", true),
                                          Helpers.GetxNameMember("FOO1", true),
                                      }
                                  }
                              }
                          },
                          new GraphNodeMember
                          {
                              MemberName = "Foo2",
                              Children = {
                                     Helpers.GetxRefRecord("FOO3", false),
                              }
                          }
                       }
                }
            };
            string Node1xaml = Helpers.Serialize(xamlDoc1);
            string node1Path = Helpers.WriteToFile(Node1xaml, "node1.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:NameRoot xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
            //        xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"">
            //      <xc:NameRoot.Node1>
            //        <xc:Node x2:CreateFrom='node1.xaml'/>
            //      </xc:NameRoot.Node1>
            //      <xc:NameRoot.Foo>
            //            <xc:MyFoo x:Name=""FOO3"" IntProperty=""5"" StringProperty=""hello"" />
            //      </xc:NameRoot.Foo>
            //    </xc:NameRoot>";
            XamlDocument xamlDocRoot = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "NameRoot"),
                    Children = {
                          new GraphNodeMember
                          {
                              MemberName = "Node1",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "Node"),
                                      Children = {
                                          Helpers.GetxCreateFromMember(node1Path, true),
                                      }
                                  }
                              }
                          },
                          new GraphNodeMember
                          {
                              MemberName = "Foo",
                              Children = {
                                  new GraphNodeRecord
                                  {
                                      RecordName = GraphNodeXaml.BuildXName(localNamespace, "MyFoo"),
                                      Children = {
                                          Helpers.CreateMember("IntProperty", 5, true),
                                          Helpers.CreateMember("StringProperty", "hello", true),
                                          Helpers.GetxNameMember("FOO3", true),
                                      }
                                  }
                              }
                          },
                       }
                }
            };
            try
            {
                Helpers.XamlFirstCompareObjects(xamlDocRoot, null);
            }
            catch (InvalidOperationException)
            {
                Tracer.LogTrace("Caught the expected exception");
            }
        }

        [TestCase]
        public void CreateFromDerivedType()
        {
            Base baseObject = new Base() { IntProperty = 5, StringProperty = "hello " };
            Derived derivedObject = new Derived() { IntProperty = 10, StringProperty = "hello world", IntProperty2 = 15, StringProperty2 = "howdy" };

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //   <xc:Derived IntProperty=""10"" IntProperty2=""15"" 
            //     StringProperty=""hello world"" StringProperty2=""howdy"" 
            //     xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests""  />";
            string derivedPath = Helpers.WriteToFile(derivedObject, "derived.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:Base x2:CreateFrom='derived.xaml'
            //  xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //  xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""/>";
            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Base"),
                    Children =
                      {
                            Helpers.GetxCreateFromMember(derivedPath, true),
                      }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, derivedObject);
        }

        // 23526
        [TestCase]
        public void CreateFromNonDerivedType()
        {
            Base baseObject = new Base() { IntProperty = 5, StringProperty = "hello " };
            NotDerived derivedObject = new NotDerived() { IntProperty2 = 10, StringProperty2 = "hello world" };

            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            // <xc:NotDerived IntProperty2=""10"" StringProperty2=""hello world"" 
            // xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests""  />";
            string nonDerpath = Helpers.WriteToFile(derivedObject, "NotDerived.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //  <xc:Base x2:CreateFrom='NotDerived.xaml' 
            // xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            // xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""/>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Base"),
                    Children =
                      {
                            Helpers.GetxCreateFromMember(nonDerpath, true),
                      }
                }
            };

            try
            {
                string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, baseObject);
            }
            catch (XamlParseException)
            {
                Tracer.LogTrace("Found expected exception");
            }

        }


        public class CustomUriResolver : XamlUriResolver
        {
            public override Uri ResolveUri(Uri baseUri, Uri relativeUri)
            {
                Uri uri = base.ResolveUri(baseUri, relativeUri);
                if (uri.ToString().Contains("forbid"))
                    throw new InvalidOperationException("Forbidden uri");
                return base.ResolveUri(baseUri, relativeUri);
            }
        }

        [TestCase (Owner="Microsoft", Category=TestCategory.IDW)]
        public void CustomUriResolverForbidUri()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "forbidden.xaml");

            //string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //               <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //               xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //                  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //               </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                    {
                            Helpers.GetxCreateFromMember(filePath, false),
                    }
                }
            };

            string xaml = Helpers.Serialize(xamlDoc);

            XamlObjectDeserializerSettings settings = new XamlObjectDeserializerSettings();
            settings.UriResolver = new CustomUriResolver();
            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(settings);

            bool passed = false;
            try
            {
                object roundtripped = deserializer.Parse(xaml);
            }
            catch (InvalidOperationException ex)
            {
                Tracer.LogTrace("Caught expected exception.." + ex.ToString());
                passed = true;
            }

            if (!passed)
                throw new TestCaseFailedException("Expected test to fail but it passed");
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void CustomUriResolverLoadFromUri()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "valid.xaml");

            //string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //               <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //               xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //                  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //               </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                    {
                            Helpers.GetxCreateFromMember(filePath, false),
                    }
                }
            };

            string xaml = Helpers.Serialize(xamlDoc);
            string filePath2 = Helpers.WriteToFile(xaml, "root.xaml");

            XamlObjectDeserializerSettings settings = new XamlObjectDeserializerSettings();
            settings.UriResolver = new CustomUriResolver();
            XamlObjectDeserializer deserializer = new XamlObjectDeserializer(settings);

            object roundtripped = deserializer.LoadFromUri(new Uri(filePath2));

            XamlObjectComparer.CompareObjects(bar, roundtripped);
        }


        [TestCase (Owner="Microsoft", Category=TestCategory.IDW)]
        public void DeleteAfterDeserialize()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string filePath = Helpers.WriteToFile(bar, "bar.xaml");

            //string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //               <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //               xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            //               xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //                  <x2:CreateFrom>bar.xaml</x2:CreateFrom>
            //               </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    Children =
                    {
                            Helpers.GetxCreateFromMember(filePath, false),
                    }
                }
            };

            string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);

            File.Delete(filePath);
            if (File.Exists(filePath))
                throw new TestCaseFailedException("File was not deleted successfully");
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void DeleteInvalidAfterDeserialize()
        {
            Bar bar = new Bar() { IntProperty = 5, StringProperty = "Hello world" };

            string InvalidXaml = @"invalid xaml";
            string path = Helpers.WriteToFile(InvalidXaml, "invalid.xaml");

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xc:Bar xmlns:xc=""clr-namespace:XamlTests.CreateFrom;assembly=XamlTests"" 
            //      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //      xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
            //     <x2:CreateFrom>invalid.xaml</x2:CreateFrom>
            //  </xc:Bar>";

            XamlDocument xamlDoc = new XamlDocument
            {
                Root = new GraphNodeRecord
                {
                    ExpectedNamespaces = 
                    {
                         GraphNodeXaml.Xaml2008Ns,
                    },
                    RecordName = GraphNodeXaml.BuildXName(localNamespace, "Bar"),
                    Children =
                      {
                            Helpers.GetxCreateFromMember(path, false),
                      }
                }
            };

            try
            {
                string xaml = Helpers.XamlFirstCompareObjects(xamlDoc, bar);
            }
            catch (XmlException)
            {
                Tracer.LogTrace("Got the expected exception");
             
                File.Delete(path);
                if (File.Exists(path))
                    throw new TestCaseFailedException("File was not deleted successfully");
                return;
            }

            throw new TestCaseFailedException("Should not reach here");
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void Nested3Levels()
        {
            MyNode node1 = new MyNode() { Name = "Node1" };
            MyNode node2 = new MyNode() { Name = "Node2" };
            MyNode node3 = new MyNode() { Name = "Node3" };

            node1.NextNode = node2;
            node2.NextNode = node3;
            node3.NextNode = null;

            string node3File = Helpers.WriteToFile(node3, "node3.xaml");
            string xaml2 = string.Format(@"<MyNode Name=""Node2"" xmlns=""clr-namespace:CDF.Test.TestCases.Xaml.Types.CreateFrom;assembly=CDF.Test.TestCases.Xaml""
                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                                <MyNode.NextNode>
                                    <MyNode x2:CreateFrom=""{0}"" />
                                </MyNode.NextNode>
                                </MyNode>", node3File);
            string node2File = Helpers.WriteToFile(node2, "node2.xaml");

            string xaml1 = string.Format(@"<MyNode Name=""Node1"" xmlns=""clr-namespace:CDF.Test.TestCases.Xaml.Types.CreateFrom;assembly=CDF.Test.TestCases.Xaml""
                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                              <MyNode.NextNode>
                                <MyNode x2:CreateFrom=""{0}"" /> 
                              </MyNode.NextNode>
                            </MyNode>", node2File);


            XamlServices.Parse(xaml2);
            XamlServices.Parse(xaml1);

            Helpers.XamlFirstCompareObjects(xaml1, node1);

        }
    }

}
