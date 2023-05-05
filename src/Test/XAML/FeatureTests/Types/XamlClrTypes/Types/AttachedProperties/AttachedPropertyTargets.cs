// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.AttachedProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Markup;
    using System.Xaml;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Driver;
    
    public class BasicPrimitiveTypesWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(BasicPrimitiveTypes.GetTestCases, "BasicPrimitiveTypesWAttachedProps");
            foreach (TestCaseInfo test in tests)
            {
                test.BugNumber = 0;
            }

            return tests;
        }
    }

    public class PrimitiveOnTopWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(PrimitivesOnTopWrapper.GetTestCases, "PrimitiveOnTopWAttachedProps");
            return tests;
        }
    }

    public class StructWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(ClassType1.GetTestCases, "StructWAttachedProps");
            return tests;
        }
    }

    public class EmptyStructWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(ClassType4.GetTestCases, "EmptyStructWAttachedProps");
            return tests;
        }
    }

    public class ClassWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(ClassType2.GetTestCases, "ClassWAttachedProps");
            return tests;
        }
    }

    public class EmptyClassWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(ClassType3.GetTestCases, "EmptyClassWAttachedProps");
            return tests;
        }
    }

    public class CollectionsWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(CollectionContainerType1.GetTestCases, "CollectionsWAttachedProps");
            return tests;
        }
    }

    public class BareCollectionsWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(GetBareCollections, "BareCollectionsWAttachedProps");
            return tests;
        }

        private static List<TestCaseInfo> GetBareCollections()
        {
            List<TestCaseInfo> wrappedInfos = CollectionContainerType1.GetTestCases();
            List<TestCaseInfo> bareInfos = new List<TestCaseInfo>();

            foreach (TestCaseInfo info in wrappedInfos)
            {
                bareInfos.Add(new TestCaseInfo
                                  {
                                      Target = ((CollectionContainerType1)info.Target).ArrayList
                                  });
                bareInfos.Add(new TestCaseInfo
                                  {
                                      Target = ((CollectionContainerType1)info.Target).Dictionary
                                  });
                bareInfos.Add(new TestCaseInfo
                                  {
                                      Target = ((CollectionContainerType1)info.Target).HashTable
                                  });
                // [DISABLE] : Known bug / Test hang ?
                bareInfos.Add(new TestCaseInfo { Target = ((CollectionContainerType1)info.Target).Items});
            }

            return bareInfos;
        }
    }

    public class TypeConverterWAttachedProps
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(Tiger.GetTestCases, "TypeConverterWAttachedProps");
            return tests;
        }
    }

    public class ClassWithOwnStore : ClassType_Inheritance23, IAttachedPropertyStore
    {
        private readonly Dictionary<AttachableMemberIdentifier, object> _attachedProperties = new Dictionary<AttachableMemberIdentifier, object>();

        int IAttachedPropertyStore.PropertyCount
        {
            get
            {
                return _attachedProperties.Count;
            }
        }

        public static List<TestCaseInfo> GetTestCases()
        {
            List<TestCaseInfo> tests = AttachedPropertySource.AttachProperties(ClassWithOwnStore.GetBaseTestCases, "ClassWithOwnStore");
            return tests;
        }

        public static List<TestCaseInfo> GetBaseTestCases()
        {
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = new ClassWithOwnStore()
                               }
                       };
        }

        #region IAttachedPropertyStore Members

        void IAttachedPropertyStore.CopyPropertiesTo(KeyValuePair<AttachableMemberIdentifier, object>[] array, int index)
        {
            ((ICollection<KeyValuePair<AttachableMemberIdentifier, object>>)_attachedProperties).CopyTo(array, index);
        }

        public bool RemoveProperty(AttachableMemberIdentifier memberName)
        {
            lock (_attachedProperties)
            {
                return _attachedProperties.Remove(memberName);
            }
        }

        public void SetProperty(AttachableMemberIdentifier memberName, object value)
        {
            lock (_attachedProperties)
            {
                _attachedProperties[memberName] = value;
            }
        }

        public bool TryGetProperty(AttachableMemberIdentifier memberName, out object value)
        {
            lock (_attachedProperties)
            {
                return _attachedProperties.TryGetValue(memberName, out value);
            }
        }

        #endregion
    }

    public class TargetFromMultipleSources : ClassType_Inheritance23
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            TargetFromMultipleSources target = new TargetFromMultipleSources();
            AttachedPropertySource.SetClassType2Prop(target, new ClassType2
                                                                 {
                                                                     Category = new ClassType1
                                                                                    {
                                                                                        Category = "category"
                                                                                    }
                                                                 });
            AttachedPropertySource2.SetInt32Prop(target, Int32.MinValue);
            AttachedPropertySourceDerived.SetStringProp(target, "foo");

            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   Target = target,
                                   CompareAttachedProperties = true,
                                   TestID = "TargetFromMultipleSources" + 0
                               }
                       };
        }
    }

    public class BareMarkupExtensions
    {
        public static List<TestCaseInfo> GetTestCases()
        {
            return AttachedPropertySource.AttachProperties(() => new List<TestCaseInfo>
                                                                     {
                                                                        // [DISABLE] ??
                                                                         new TestCaseInfo
                                                                             {
                                                                                 Target = new AttachedPropMETarget("blah", 42)
                                                                             }
                                                                     }, "BareMarkupExtensions");
        }
    }

    public class WrappedMarkupExtensions
    {
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            return from infos in BareMarkupExtensions.GetTestCases()
                   select new TestCaseInfo
                              {
                                  Target = new AttachedPropMETargetWrapper
                                               {
                                                   Target = (AttachedPropMETarget)infos.Target
                                               },
                                  CompareAttachedProperties = true,
                                  TestID = infos.TestID.Replace("BareMarkupExtensions", "WrappedMarkupExtensions"),
                                  BugNumber = GetBugNumber(infos.TestID)
                              };
        }

        private static int GetBugNumber(string testId)
        {
            if (testId.Contains("ClassType") ||
                testId.Contains("Array") ||
                testId.Contains("HashTable") ||
                testId.Contains("IList") ||
                testId.Contains("Dictionary"))
            {
                return 0;
            }

            return 0;
        }
    }

    public class AttachedCollectionProperty
    {
        public static IEnumerable<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            object o = new object();
            AttachedCollectionPropertySource.GetStrings(o).Add("Hello");
            AttachedCollectionPropertySource.GetStrings(o).Add("Goodbye");
            AttachedCollectionPropertySource.GetDictionary(o).Add("Hello", "Goodbye");

            return new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    TestID = instanceIDPrefix + 1,
                    Target = o,
                    CompareAttachedProperties = true,
                }
            };
        }
    }        

    public class AttachedPropMETargetWrapper
    {
        public AttachedPropMETarget Target { get; set; }
    }

    [TypeConverter(typeof(AttachedPropMETargetConverter))]
    public class AttachedPropMETarget
    {
        public AttachedPropMETarget(string name, int age)
        {
            this._name = name;
            this._age = age;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
        public int Age
        {
            get
            {
                return _age;
            }
        }

        private readonly string _name;
        private readonly int _age;
    }

    public class AttachedPropMETargetExtension : MarkupExtension
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var attachedProps =
                new KeyValuePair<AttachableMemberIdentifier, object>[AttachablePropertyServices.GetAttachedPropertyCount(this)];

            AttachablePropertyServices.CopyPropertiesTo(this, attachedProps, 0);

            var retVal = new AttachedPropMETarget(this.Name, this.Age);
            foreach (KeyValuePair<AttachableMemberIdentifier, object> prop in attachedProps)
            {
                AttachablePropertyServices.SetProperty(retVal, prop.Key, prop.Value);
            }

            return retVal;
        }
    }

    public class AttachedPropMETargetConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("desitinationType is not a MarkupExtension", "destinationType");
            }

            AttachedPropMETarget simpleMEClass = value as AttachedPropMETarget;

            if (null == simpleMEClass)
            {
                throw new ArgumentNullException("SimpleMEClass");
            }

            var retVal = new AttachedPropMETargetExtension
                             {
                                 Age = simpleMEClass.Age,
                                 Name = simpleMEClass.Name
                             };

            var attachedProps =
                new KeyValuePair<AttachableMemberIdentifier, object>[AttachablePropertyServices.GetAttachedPropertyCount(simpleMEClass)];

            AttachablePropertyServices.CopyPropertiesTo(simpleMEClass, attachedProps, 0);

            foreach (KeyValuePair<AttachableMemberIdentifier, object> prop in attachedProps)
            {
                AttachablePropertyServices.SetProperty(retVal, prop.Key, prop.Value);
            }

            return retVal;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
}
