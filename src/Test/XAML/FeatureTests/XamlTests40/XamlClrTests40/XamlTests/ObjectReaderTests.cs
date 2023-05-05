// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xaml;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Driver;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Types.ObjectReader;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace1;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace2;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace3;
using OR = Microsoft.Test.Xaml.Types.ObjectReader;

namespace Microsoft.Test.Xaml.XamlTests
{
    public class ObjectReaderTests
    {
        private const string TypesNamespaces = "http://XamlTestTypes";
        private const string ORTypesNamespaces = "clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader;assembly=XamlClrTypes";
        private const string Namespace1 = "clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader.Namespace1;assembly=XamlClrTypes";
        private const string Namespace2 = "clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader.Namespace2;assembly=XamlClrTypes";
        private const string Namespace3 = "clr-namespace:Microsoft.Test.Xaml.Types.ObjectReader.Namespace3;assembly=XamlClrTypes";
        private const string GenericNamespace = "clr-namespace:System.Collections.Generic;assembly=mscorlib";

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstSimpleGraph()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new Ns1Type(),
                                     child2 = new Ns2Type()
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace2, "mtxton1")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void NamespacesFirstGenericTypeArgsInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new Ns1GenericType<Ns2Type>(),
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace2, "mtxton1")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstMEInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new SimpleMEClass()
                                                  {
                                                      Data = "foo",
                                                  },
                                     child2 = new Ns1Type(),
                                 };

            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton1")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace2, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void NamespacesFirstMEPropertyInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new SimpleMEClass()
                                                  {
                                                      Data = new Ns3Type(),
                                                  },
                                 };

            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace3, "mtxton1")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace2, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void NamespacesFirstRecursiveTypeArgsInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new Ns1GenericType<List<Ns2Type>>(),
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace2, "mtxton1")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(GenericNamespace, "scg")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstInstanceDescriptorMethodInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new SomeType()
                                                  {
                                                      Data = "hello"
                                                  },
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstInstanceDescriptorMethodAndArgsInDifferentNamespace()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new SomeType()
                                                  {
                                                      Data = new Ns2Type()
                                                  },
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstNsInsideTypeConvertedString()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new Ns1Type()
                                                  {
                                                      XName = XName.Get("Ns3Type", Namespace3)
                                                  },
                                 };
            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton1")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace3, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void NamespacesFirstNsValueSerializedString()
        {
            OR.Container root = new OR.Container()
                                 {
                                     child1 = new Ns1Type()
                                                  {
                                                      XNameVS = XName.Get("Ns3Type", Namespace3)
                                                  },
                                 };

            XamlObjectReader objectReader = new XamlObjectReader(root);
            string xaml = XamlServices.Save(root);
            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(ORTypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace1, "mtxton1")
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespace3, "mtxton")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                actual.Add(ObjectReaderState.FromObjecReader(objectReader));
            }
            CompareValidateNamespaceNodeStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void NamespaceNodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new BasicPrimitiveTypes());

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(TypesNamespaces, ""),
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Namespace = new NamespaceDeclaration(Namespaces.Namespace2006, "x")
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.NamespaceDeclaration)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void SONodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new BasicPrimitiveTypes());

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               TypeName = "BasicPrimitiveTypes", Object = typeof(BasicPrimitiveTypes).FullName,
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               TypeName = "NullExtension", 
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.StartObject)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void SMNodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new Point());

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               MemberName = "X"
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               MemberName = "Y"
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.StartMember)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ValueNodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new Point()
                                                                     {
                                                                         X = 5,
                                                                         Y = 10
                                                                     });

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               Value = "5"
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                               Value = "10"
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.Value)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void EONodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new Point()
                                                                     {
                                                                         X = 5,
                                                                         Y = 10
                                                                     });

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.EndObject)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void EMNodeValidatePropeties()
        {
            XamlObjectReader objectReader = new XamlObjectReader(new Point()
                                                                     {
                                                                         X = 5,
                                                                         Y = 10
                                                                     });

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                           },
                                                       new ObjectReaderState
                                                           {
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.EndMember)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RO_Collection()
        {
            ROCollectionContainer root = new ROCollectionContainer();
            root.ROCollection.Add("hello");

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               IsObjectFromMember = true
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.GetObject)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RO_Dictionary()
        {
            ROCollectionContainer root = new ROCollectionContainer();
            root.RODictionary.Add("hello", "world");

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               IsObjectFromMember = true
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.GetObject)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        //RO Arrays aren't written by design, this is a negative test
        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RO_Array()
        {
            ROCollectionContainer root = new ROCollectionContainer();
            root.ROArray[0] = "hello";
            root.ROArray[1] = "world";

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               TypeName = "ArrayExtension", IsObjectFromMember = true
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.StartObject &&
                    objectReader.Type.Name == "ArrayExtension")
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }

            try
            {
                CompareValidateObjectReaderStates(expected, actual);
                throw new TestCaseFailedException("Expected exception not thrown.");
            }
            catch (TestCaseFailedException exception)
            {
                if (!exception.Message.Contains("Traces do not match"))
                {
                    throw;
                }
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RW_Collection()
        {
            RWCollectionContainer root = new RWCollectionContainer()
                                             {
                                                 RWCollection = new List<string>()
                                                                    {
                                                                        "hello"
                                                                    },
                                             };

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               TypeName = "List", 
                                                               IsObjectFromMember = false, 
                                                               Object = "System.Collections.Generic.List`1[System.String]"
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.StartObject &&
                    objectReader.Type.Name == "List")
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RW_Dictionary()
        {
            RWCollectionContainer root = new RWCollectionContainer()
                                             {
                                                 RWDictionary = new Dictionary<string, string>(),
                                             };
            root.RWDictionary.Add("hello", "world");

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               IsObjectFromMember = true,
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.GetObject)
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.BVT, TestType = Microsoft.Test.CDFInfrastructure.TestType.Automated)]
        public void ISOFM_RW_Array()
        {
            RWCollectionContainer root = new RWCollectionContainer()
                                             {
                                                 RWArray = new string[]
                                                               {
                                                                   "Hello", "World"
                                                               },
                                             };

            XamlObjectReader objectReader = new XamlObjectReader(root);

            List<ObjectReaderState> expected = new List<ObjectReaderState>
                                                   {
                                                       new ObjectReaderState
                                                           {
                                                               TypeName = "ArrayExtension", IsObjectFromMember = false, Object = "System.String[]",
                                                           },
                                                   };
            List<ObjectReaderState> actual = new List<ObjectReaderState>();

            while (objectReader.Read())
            {
                if (objectReader.NodeType == XamlNodeType.StartObject &&
                    objectReader.Type.Name == "ArrayExtension")
                {
                    actual.Add(ObjectReaderState.FromObjecReader(objectReader));
                }
            }
            CompareValidateObjectReaderStates(expected, actual);
        }

        /// <summary>
        /// Regression test :XOR fails to wrap dictionary that has x:Name property
        /// </summary>
        [TestCase]
        public void DerivedDictionaryAsContent()
        {
            DerivedDictionary dictionary = new DerivedDictionary() { { "Hello", "World" } };
            dictionary.Name = "myName";
            DerivedDictionaryContainer root = new DerivedDictionaryContainer()
            {
                ContentProperty = new DictionaryAsContent()
                {
                    TheContent = dictionary,
                },
                NormalProperty = new DictionaryAsProperty()
                {
                    TheProperty = dictionary,
                },
            };

            XamlTestDriver.RoundTripCompare(root);
        }

        /// <summary>
        /// Regression test : XOR fails to wrap dictionary that has x:Name property
        /// </summary>
        [TestCase]
        public void DerivedDictionary()
        {
            DerivedDictionary dictionary = new DerivedDictionary() { { "Hello", "World" } };
            dictionary.Name = "myName";
            DoubleDerivedDictionaryContainer root = new DoubleDerivedDictionaryContainer()
            {
                NormalProperty1 = new DictionaryAsProperty()
                {
                    TheProperty = dictionary,
                },
                NormalProperty2 = new DictionaryAsProperty()
                {
                    TheProperty = dictionary,
                },
            };

            XamlTestDriver.RoundTripCompare(root);
        }

        private struct ObjectReaderState : IEquatable<ObjectReaderState>
        {
            public XamlNodeType NodeType { get; set; }
            public string TypeName { get; set; }
            public string MemberName { get; set; }
            public string Value { get; set; }
            public string Object { get; set; }
            public bool IsObjectFromMember { get; set; }
            public NamespaceDeclaration Namespace { get; set; }

            #region IEquatable<ObjectReaderState> Members

            public static ObjectReaderState FromObjecReader(XamlObjectReader objectReader)
            {
                ObjectReaderState state = new ObjectReaderState();
                state.NodeType = objectReader.NodeType;

                state.TypeName = objectReader.Type == null ? null : objectReader.Type.Name;
                state.MemberName = objectReader.Member == null ? null : objectReader.Member.Name;
                state.Value = objectReader.Value == null ? null : objectReader.Value.ToString();

                state.Object = objectReader.Instance == null ? null : objectReader.Instance.ToString();

                state.IsObjectFromMember = objectReader.NodeType == XamlNodeType.GetObject;
                state.Namespace = objectReader.Namespace;

                return state;
            }

            public bool Equals(ObjectReaderState that)
            {
                return (String.Equals(this.TypeName, that.TypeName) &&
                        String.Equals(this.MemberName, that.MemberName) &&
                        String.Equals(this.Value, that.Value) &&
                        String.Equals(this.Object, that.Object) &&
                        (this.IsObjectFromMember == that.IsObjectFromMember) &&
                        IsSameNs(this.Namespace, that.Namespace));
            }

            public static bool IsSameNs(NamespaceDeclaration ns1, NamespaceDeclaration ns2)
            {
                if (ns1 == null && ns2 == null)
                {
                    return true;
                }
                if (ns1 == null || ns2 == null)
                {
                    return false;
                }

                return (ns1.Namespace.Equals(ns2.Namespace) &&
                        ns1.Prefix.Equals(ns2.Prefix));
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} TypeName={1} MemberName={2} Value={3} Object={4} IsOFM={5} NS={6}",
                                     this.NodeType, this.TypeName, this.MemberName, this.Value, this.Object, this.IsObjectFromMember,
                                     this.Namespace == null ? "{Null}" : (this.Namespace.Prefix + ":" + this.Namespace.Namespace));
            }

            #endregion
        }

        // Ordered compare of expected and actual ObjectReader states //
        private static void CompareValidateObjectReaderStates(IList<ObjectReaderState> expected,
                                                              IList<ObjectReaderState> actual)
        {
            if (expected.Count != actual.Count)
            {
                Fail("Traces do not match", expected, actual);
            }

            for (int i = 0; i < expected.Count; i++)
            {
                if (!expected[i].Equals(actual[i]))
                {
                    Fail("Traces do not match", expected, actual);
                }
            }
        }

        // Unordered compare and makes sure all namespace 
        // nodes are always at the begining 
        private static void CompareValidateNamespaceNodeStates(IList<ObjectReaderState> expected,
                                                               IList<ObjectReaderState> actual)
        {
            // unordered compare - make sure all expected states are present
            foreach (ObjectReaderState expctedState in expected)
            {
                if (expctedState.Namespace == null)
                {
                    continue;
                }

                bool match = false;
                foreach (ObjectReaderState actualState in actual)
                {
                    if (actualState.Namespace != null &&
                        expctedState.Equals(actualState))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    Fail("Expected Namespace trace did not match", expected, actual);
                }
            }

            // make sure all namespace nodes are at the begining //
            bool namespacesDone = false;
            foreach (ObjectReaderState state in actual)
            {
                if (namespacesDone && state.Namespace != null)
                {
                    Fail("Namespace node found in the middle of the node stream", expected, actual);
                }
                if (state.Namespace == null)
                {
                    namespacesDone = true;
                }
            }
        }

        private static void TraceObjectReaderStates(string message, IList<ObjectReaderState> states)
        {
            Tracer.LogTrace(message);
            foreach (ObjectReaderState state in states)
            {
                Tracer.LogTrace(state.ToString());
            }
        }

        private static void Fail(string message, IList<ObjectReaderState> expected, IList<ObjectReaderState> actual)
        {
            Tracer.LogTrace("Traces do not match");
            TraceObjectReaderStates("Expected:", expected);
            TraceObjectReaderStates("Actual:", actual);
            throw new TestCaseFailedException(message);
        }
    }
}
