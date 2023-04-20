// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xaml;
    using System.Xml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Types.MarkupExtensions;
    using Microsoft.Test.Xaml.Utilities;
    
    public class AttachedPropertyTests
    {
        #region BasicPrimitiveTypesWAttachedProps

        private readonly List<Type> _basicPrimitiveTypesWAttachedPropsTypes = new List<Type>
                                                                                 {
                                                                                     typeof (PrimitiveOnTopWAttachedProps)
                                                                                 };

        [TestCaseGenerator()]
        public void BasicPrimitiveTypesWAttachedPropsTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._basicPrimitiveTypesWAttachedPropsTypes, MethodInfo.GetCurrentMethod());
        }

        public void BasicPrimitiveTypesWAttachedPropsTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._basicPrimitiveTypesWAttachedPropsTypes, instanceID));
        }

        #endregion

        #region BasicPrimitiveTypesWAttachedProps2

        private readonly List<Type> _basicPrimitiveTypesWAttachedPropsTypes2 = new List<Type>
                                                                                  {
                                                                                      typeof (BasicPrimitiveTypesWAttachedProps),
                                                                                  };

        [TestCaseGenerator()]
        public void BasicPrimitiveTypesWAttachedPropsTest2(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._basicPrimitiveTypesWAttachedPropsTypes2, MethodInfo.GetCurrentMethod());
        }

        public void BasicPrimitiveTypesWAttachedPropsTest2Method(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._basicPrimitiveTypesWAttachedPropsTypes2, instanceID));
        }

        #endregion

        #region Classes/structs w/attached properties

        private readonly List<Type> _classStructsWAttachedPropsTypes = new List<Type>
                                                                          {
                                                                              typeof (StructWAttachedProps),
                                                                              typeof (EmptyStructWAttachedProps),
                                                                              typeof (ClassWAttachedProps),
                                                                              typeof (EmptyClassWAttachedProps)
                                                                          };

        [TestCaseGenerator()]
        public void ClassStructsWAttachedPropsTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._classStructsWAttachedPropsTypes, MethodInfo.GetCurrentMethod());
        }

        public void ClassStructsWAttachedPropsTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._classStructsWAttachedPropsTypes, instanceID));
        }

        #endregion

        #region Collections w/attached properties

        private readonly List<Type> _collectionsWAttachedPropsTypes = new List<Type>
                                                                         {
                                                                             typeof (CollectionsWAttachedProps),
                                                                             typeof (AttachedCollectionProperty)
                                                                         };

        [TestCaseGenerator()]
        public void CollectionsWAttachedPropsTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionsWAttachedPropsTypes, MethodInfo.GetCurrentMethod());
        }

        public void CollectionsWAttachedPropsTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionsWAttachedPropsTypes, instanceID));
        }

        #endregion

        #region Collections w/attached properties 2

        private readonly List<Type> _collectionsWAttachedPropsTypes2 = new List<Type>
                                                                          {
                                                                              typeof (BareCollectionsWAttachedProps)
                                                                          };

        [TestCaseGenerator()]
        public void CollectionsWAttachedPropsTest2(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._collectionsWAttachedPropsTypes2, MethodInfo.GetCurrentMethod());
        }

        public void CollectionsWAttachedPropsTest2Method(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._collectionsWAttachedPropsTypes2, instanceID));
        }

        #endregion

        #region TypeConverter w/attached properties

        private readonly List<Type> _typeConverterWAttachedPropsTypes = new List<Type>
                                                                           {
                                                                               typeof (TypeConverterWAttachedProps)
                                                                           };

        // [DISABLED]
        // [TestCaseGenerator()]
        public void TypeConverterWAttachedPropsTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._typeConverterWAttachedPropsTypes, MethodInfo.GetCurrentMethod());
        }

        public void TypeConverterWAttachedPropsTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._typeConverterWAttachedPropsTypes, instanceID));
        }

        #endregion

        #region Class w/own store

        private readonly List<Type> _classWOwnStoreTypes = new List<Type>
                                                              {
                                                                  typeof (ClassWithOwnStore)
                                                              };

        [TestCaseGenerator()]
        public void ClassWOwnStoreTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._classWOwnStoreTypes, MethodInfo.GetCurrentMethod());
        }

        public void ClassWOwnStoreTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._classWOwnStoreTypes, instanceID));
        }

        #endregion

        #region Multiple sources tests

        private readonly List<Type> _multipleSourcesTypes = new List<Type>
                                                               {
                                                                   typeof (TargetFromMultipleSources)
                                                               };

        [TestCaseGenerator()]
        public void MultipleSourcesTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._multipleSourcesTypes, MethodInfo.GetCurrentMethod());
        }

        public void MultipleSourcesTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._multipleSourcesTypes, instanceID));
        }

        #endregion

        #region Attached to self tests

        private readonly List<Type> _attachedToSelfTypes = new List<Type>
                                                              {
                                                                  typeof (AttachedPropertySource2),
                                                                  typeof (AttachedPropertySourceDerived)
                                                              };

        [TestCaseGenerator()]
        public void AttachedToSelfTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._attachedToSelfTypes, MethodInfo.GetCurrentMethod());
        }

        public void AttachedToSelfTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._attachedToSelfTypes, instanceID));
        }

        #endregion

        #region MarkupExtensionTypesWAttachedPropsTest

        private readonly List<Type> _markupExtensionTypesWAttachedPropsTypes = new List<Type>
                                                                                  {
                                                                                      typeof (BareMarkupExtensions),
                                                                                      typeof (WrappedMarkupExtensions)
                                                                                  };

        // [DISABLED]
        // [TestCaseGenerator()]
        public void MarkupExtensionTypesWAttachedPropsTest(AddTestCaseEventHandler addCase)
        {
            XamlTestDriver.GenerateTestCases(addCase, this._markupExtensionTypesWAttachedPropsTypes, MethodInfo.GetCurrentMethod());
        }

        public void MarkupExtensionTypesWAttachedPropsTestMethod(string instanceID)
        {
            XamlTestDriver.RunTest(MethodInfo.GetCurrentMethod(), XamlTestDriver.GetInstance(this._markupExtensionTypesWAttachedPropsTypes, instanceID));
        }

        #endregion

        // [DISABLED]
        // [TestCase(
        //     Owner = "Microsoft",
        //     Category = TestCategory.Functional,
        //     TestType = TestType.Automated
        //     )]
        public void AttachToSelfSamePropNameNegative()
        {
            AttachedPropertySourceWithSameName item = new AttachedPropertySourceWithSameName();
            item.Bar = "bingo";
            AttachedPropertySourceWithSameName.SetBar(item, "WRONG");

            string expectedMessage = Exceptions.GetMessage("Xml_DupAttributeName", typeof(XmlWriter).Assembly, "System.Xml");
            ExceptionHelpers.CheckForException(typeof(XmlException), expectedMessage, () => XamlServices.Save(item));
        }

        [TestCase(Owner = "Microsoft")]
        public void MeOnAttachedProperty()
        {
            ClassWAttachedProps source = new ClassWAttachedProps();
            SimpleMEClass attachedProperty = new SimpleMEClass()
                                                 {
                                                     Address = "Hello",
                                                     AptNo = 20
                                                 };
            AttachedPropertySource.SetMEOnAttachedProp(source, attachedProperty);

            string xaml = XamlTestDriver.RoundTripCompare(source);
            if (!xaml.Contains(@"AttachedPropertySource.MEOnAttachedProp=""{mtxtm:SimpleME Address=Hello, AptNo=20}"""))
            {
                throw new TestCaseFailedException("Squigly syntax of ME not found: Xaml is " + xaml);
            }

            string xaml2 = xaml.Replace(@"{mtxtm:SimpleME Address=Hello, AptNo=20}", @"{mtxtm:SimpleME Hello, AptNo=20}");
            XamlTestDriver.XamlFirstCompareObjects(xaml2, source);

            ClassWAttachedProps roundtripped = (ClassWAttachedProps)XamlTestDriver.Roundtrip(source);
            XamlObjectComparer.CompareObjects(AttachedPropertySource.GetMEOnAttachedProp(roundtripped), attachedProperty);
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated
            )]
        public void AttachToSelfSamePropName()
        {
            AttachedPropertySourceWithSameName item = new AttachedPropertySourceWithSameName();
            item.Bar = "bingo";

            AttachedPropertySourceWithSameName itemAfter = (AttachedPropertySourceWithSameName)XamlTestDriver.Roundtrip(item);

            if (item.Bar != itemAfter.Bar)
            {
                throw new DataTestException("Expected:  " + item.Bar + " Actual: " + itemAfter.Bar);
            }

            if (!String.IsNullOrEmpty(AttachedPropertySourceWithSameName.GetBar(itemAfter)))
            {
                throw new DataTestException("Expected empty string.  Actual: " + AttachedPropertySourceWithSameName.GetBar(itemAfter));
            }
        }

        private NodeList GetDocWAttachedProp(Type attachedPropertySource, string attachedPropertyName)
        {
            XName apSource = Namespaces.GetXNameFromType(attachedPropertySource);
            return new NodeList()
            {
                new NamespaceNode("ap", apSource.Namespace.NamespaceName),
                new StartObject(typeof(ClassType2)),
                    new StartMember(typeof(ClassType2), "Category"),
                        new StartObject(typeof(ClassType1)),
                            new StartMember(typeof(ClassType1), "Category"),
                                new ValueNode("Category"),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                    new StartMember(attachedPropertySource, "Bar"),
                        new ValueNode("BarBarBar"),          
                    new EndMember(),
                new EndObject(),
            };
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated
            )]
        public void InvalidSourceImplementationTest()
        {
            ClassType2 x = new ClassType2
                               {
                                   Category = new ClassType1
                                                  {
                                                      Category = "blah"
                                                  }
                               };
            InvalidImplementationSource.SetBar(InvalidImplementationSource.BarName, x, "attached prop");
            string expectedMessage =
                Exceptions.GetMessage("ObjectReaderAttachedPropertyNotFound", WpfBinaries.SystemXaml);

            ExceptionHelpers.CheckForException(typeof(XamlObjectReaderException), expectedMessage, () => XamlServices.Save(x));
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated
            )]
        public void AttachedPropHiddenGetTest()
        {
            ClassType2 before = new ClassType2
                                    {
                                        Category = new ClassType1
                                                       {
                                                           Category = "blah"
                                                       }
                                    };
            AttachedPropertySourceHiddenGetter.SetBar(before, "BarBarBar");
            ClassType2 after = (ClassType2)XamlTestDriver.Roundtrip(before);

            if (!String.IsNullOrEmpty(AttachedPropertySourceHiddenGetter.GetBar(after)))
            {
                throw new DataTestException("Expected empty or null string.  Actual:  " + AttachedPropertySourceHiddenGetter.GetBar(after));
            }

            NodeList doc = GetDocWAttachedProp((typeof(AttachedPropertySourceHiddenGetter)), "Bar");

            string xaml = doc.NodeListToXaml();

            ClassType2 xamlFirst = (ClassType2)XamlServices.Parse(xaml);

            if (AttachedPropertySourceHiddenGetter.GetBar(xamlFirst) != "BarBarBar")
            {
                throw new DataTestException("Expected: BarBarBar Actual: " + AttachedPropertySourceHiddenGetter.GetBar(xamlFirst));
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.Functional,
            TestType = TestType.Automated
            )]
        public void AttachedPropHiddenSetTest()
        {
            ClassType2 before = new ClassType2
                                    {
                                        Category = new ClassType1
                                                       {
                                                           Category = "blah"
                                                       }
                                    };
            AttachedPropertySourceHiddenSetter.SetBar(before, "BarBarBar");
            ClassType2 after = (ClassType2)XamlTestDriver.Roundtrip(before);

            if (AttachedPropertySourceHiddenSetter.GetBar(after) != "BarBarBar")
            {
                throw new DataTestException("Expected BarBarBar.  Actual:  " + AttachedPropertySourceHiddenSetter.GetBar(after));
            }

            NodeList doc = GetDocWAttachedProp(typeof(AttachedPropertySourceHiddenSetter), "Bar");

            string xaml = doc.NodeListToXaml();

            ClassType2 xamlFirst = (ClassType2)XamlServices.Parse(xaml);

            if (AttachedPropertySourceHiddenSetter.GetBar(xamlFirst) != "BarBarBar")
            {
                throw new DataTestException("Expected: BarBarBar Actual: " + AttachedPropertySourceHiddenSetter.GetBar(xamlFirst));
            }
        }

        [TestCase(
            Owner = "Microsoft",
            Category = TestCategory.IDW,
            TestType = TestType.Automated
            )]
        public void AttachedPropertyApiTest()
        {
            ExceptionHelpers.CheckForException(typeof(ArgumentNullException), () => AttachablePropertyServices.SetProperty(new object(), (AttachableMemberIdentifier)null, new object()));
            AttachablePropertyServices.SetProperty(null, (AttachableMemberIdentifier)null, new object());
        }

        // [DISABLED]
        // [TestCase]
        public void AttachToEmptyString()
        {
            string target = string.Empty;
            foreach (KeyValuePair<AttachableMemberIdentifier, object> property in AttachedPropertySource.Properties)
            {
                AttachablePropertyServices.SetProperty(target, property.Key, property.Value);
            }

            try
            {
                var roundtrippedObject = XamlServices.Parse(XamlServices.Save(target));
                XamlObjectComparer.CompareObjectsAndAttachedProperties(target, roundtrippedObject);
            }
            finally
            {
                ClearAttachedPropertiesOnEmptyString();
            }
        }

        [TestCase]
        public void GetAttachableMembers()
        {
            XamlSchemaContext xsc = new XamlSchemaContext();

            XamlType xt = xsc.GetXamlType(typeof(HasAttachableProperty));

            ICollection<XamlMember> members = xt.GetAllAttachableMembers();
            if (members.Count == 0)
            {
                throw new TestCaseFailedException("XamlType.GetAttachableMembers() did not provide get only property");
            }
        }

        /// <summary>
        /// If a type implements Get/Set overloads for an attachable property, where one is public and the other
        /// is internal, the public accessors should be used
        /// </summary>
        [TestCase]
        public void PreferPublicAccessors()
        {
            var target = new ClassType2
            {
                Category = new ClassType1
                {
                    Category = "blah"
                }
            };

            AttachedPropertySourceInternalAccessors.SetBar(target, "42");
            TestCaseInfo testCaseInfo = new TestCaseInfo()
            {
                TestID = "",
                Target = target,
                CompareAttachedProperties = true
            };

            new ObjectDoubleRoundtripDriver().Execute("ObjectFirstTest", testCaseInfo);
        }

        /// <summary>
        /// Attachable properties aren't allowed on types that convert to string, verify exception is thrown
        /// </summary>
        [TestCase]
        public void VerifyNoAttachedPropsOnTypeConvertedObject()
        {
            var zoo = new Zoo
            {
                Monkey = new Monkey("Bubbles"),
                Tiger = new Tiger("Billy")
            };

            AttachedPropertySource.SetBoolProp(zoo.Monkey, true);

            string expectedMessage = Exceptions.GetMessage("AttachedPropertyOnTypeConvertedOrStringProperty", WpfBinaries.SystemXaml);
            ExceptionHelpers.CheckForException(typeof(InvalidOperationException), expectedMessage, () => XamlServices.Save(zoo));
        }

        /// <summary>
        /// provide coverage of AttachablePropertyServices.RemoveProperty
        /// </summary>
        [TestCase]
        public void RemoveProperty()
        {
            var zoo = new Zoo
            {
                Monkey = new Monkey("Bubbles"),
                Tiger = new Tiger("Billy")
            };

            AttachedPropertySource.SetBoolProp(zoo, true);

            Assert.AreEqual(1, AttachablePropertyServices.GetAttachedPropertyCount(zoo));

            AttachablePropertyServices.RemoveProperty(zoo, new AttachableMemberIdentifier(typeof(AttachedPropertySource), "BoolProp"));

            Assert.AreEqual(0, AttachablePropertyServices.GetAttachedPropertyCount(zoo));
        }

        void ClearAttachedPropertiesOnEmptyString()
        {
            if (AttachablePropertyServices.GetAttachedPropertyCount(string.Empty) == 0)
            {
                return;
            }

            var props = new KeyValuePair<AttachableMemberIdentifier, object>[AttachablePropertyServices.GetAttachedPropertyCount(string.Empty)];

            AttachablePropertyServices.CopyPropertiesTo(string.Empty, props, 0);
            foreach (KeyValuePair<AttachableMemberIdentifier, object> prop in props)
            {
                AttachablePropertyServices.RemoveProperty(string.Empty, prop.Key);
            }

        }
    }
}
