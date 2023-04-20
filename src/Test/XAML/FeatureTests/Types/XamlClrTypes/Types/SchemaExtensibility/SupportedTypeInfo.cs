// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.SchemaExtensibility
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xaml.Schema;
    using Microsoft.Test.Xaml.Types.AttachedProperties;
    using Microsoft.Test.Xaml.Types.ContentProperties;
    using Microsoft.Test.Xaml.Types.InstanceReference;
    
    static class SupportedTypeInfo
    {
        static readonly Dictionary<Type, TypeInfo> s_supportedTypes = new Dictionary<Type, TypeInfo>
        {
            { typeof(ClassType1), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new ClassType1(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              {"Category", typeof(ClassType1).GetProperty("Category")}
                          }
                      }
            },
            { typeof(ClassType2), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new ClassType2(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              { "Category", typeof(ClassType2).GetProperty("Category")}
                          }
                      }
            },
            { typeof(GenericType1<DateTime, double>), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new GenericType1<DateTime, double>(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              { "Info1", typeof(GenericType1<DateTime, double>).GetProperty("Info1")},
                              { "Info2", typeof(GenericType1<DateTime, double>).GetProperty("Info2")}
                          }
                      }
            },
            { typeof(Dictionary<GenericType1<DateTime, double>, ClassType2>), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new Dictionary<GenericType1<DateTime, double>, ClassType2>(),
                          CollectionKind = XamlCollectionKind.Dictionary,
                          AddMethod = typeof(Dictionary<GenericType1<DateTime, double>, ClassType2>).GetMethod("Add"),
                          EnumeratorMethod = typeof(Dictionary<GenericType1<DateTime, double>, ClassType2>).GetMethod("GetEnumerator")
                      }
            },
            { typeof(List<GenericType1<DateTime, double>>), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new List<GenericType1<DateTime, double>>(),
                          CollectionKind = XamlCollectionKind.Collection,
                          AddMethod = typeof(List<GenericType1<DateTime, double>>).GetMethod("Add"),
                          EnumeratorMethod = typeof(List<GenericType1<DateTime, double>>).GetMethod("GetEnumerator")
                      }
            },
            { typeof(List<ImplicitNamingBar>), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new List<ImplicitNamingBar>(),
                          CollectionKind = XamlCollectionKind.Collection,
                          AddMethod = typeof(List<ImplicitNamingBar>).GetMethod("Add"),
                          EnumeratorMethod = typeof(List<ImplicitNamingBar>).GetMethod("GetEnumerator")
                      }
            },
            { typeof(ClassType2[]), 
                  new TypeInfo
                      {
                          CollectionKind = XamlCollectionKind.Array,
                      }
            },
            { typeof(StringContentPropertyWClass), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new StringContentPropertyWClass(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              {"ContentProperty", typeof(StringContentPropertyWClass).GetProperty("ContentProperty")},
                              {"ClassProperty", typeof(StringContentPropertyWClass).GetProperty("ClassProperty")}
                          },
                          ContentProperty = typeof(StringContentPropertyWClass).GetProperty("ContentProperty")
                      }
            },
            { typeof(string), new TypeInfo { CollectionKind = XamlCollectionKind.None } },
            { typeof(DateTime), new TypeInfo {  } },
            { typeof(double), new TypeInfo {  } },
            { typeof(bool), new TypeInfo {  } },
            { typeof(int), new TypeInfo {  } },
            { typeof(object), new TypeInfo {  } },
            { typeof(ItemType), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new ItemType(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              {"ItemName", typeof(ItemType).GetProperty("ItemName")},
                              {"Price", typeof(ItemType).GetProperty("Price")}
                          },
                      }
            },
            { typeof(CollectionContainerType14), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new CollectionContainerType14(),
                          CollectionKind = XamlCollectionKind.Collection,
                          AddMethod = typeof(CollectionContainerType14).GetMethod("Add"),
                          EnumeratorMethod = typeof(CollectionContainerType14).GetMethod("GetEnumerator")
                      }
            },
            { typeof(CollectionContainerType15), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new CollectionContainerType15(),
                          CollectionKind = XamlCollectionKind.Dictionary,
                          AddMethod = typeof(CollectionContainerType15).GetMethod("Add"),
                          EnumeratorMethod = typeof(CollectionContainerType15).GetMethod("GetEnumerator")
                      }
            },
            { typeof(AttachedPropertySource), 
                  new TypeInfo
                      {
                          AttachableMembers = { "BoolProp" }
                      }
            },
            { typeof(ImplicitNamingBar), 
                  new TypeInfo
                      {
                          CreationDelegate = () => new ImplicitNamingBar(),
                          CollectionKind = XamlCollectionKind.None,
                          MemberMap = 
                          {
                              {"IntProperty", typeof(ImplicitNamingBar).GetProperty("IntProperty")},
                              {"StringProperty", typeof(ImplicitNamingBar).GetProperty("StringProperty")},
                              {"MyImplicitName", typeof(ImplicitNamingBar).GetProperty("MyImplicitName")}
                          }
                      }
            },
        };

        public static object CreateInstance(Type t)
        {
            return GetTypeInfo(t).CreationDelegate();
        }

        public static MethodInfo LookupAddMethod(Type t)
        {
            return GetTypeInfo(t).AddMethod;
        }

        public static MethodInfo LookupEnumeratorMethod(Type t)
        {
            return GetTypeInfo(t).EnumeratorMethod;
        }

        public static IEnumerable<MemberInfo> GetMembers(Type t)
        {
            return GetTypeInfo(t).MemberMap.Values;
        }

        public static IEnumerable<string> GetAttachableMembers(Type t)
        {
            return GetTypeInfo(t).AttachableMembers;
        }

        public static MemberInfo GetMember(Type t, string name)
        {
            TypeInfo info = GetTypeInfo(t);
            return info.MemberMap[name];
        }

        public static PropertyInfo GetContentProperty(Type t)
        {
            return GetTypeInfo(t).ContentProperty;
        }

        public static XamlCollectionKind GetCollectionKind(Type t)
        {
            return GetTypeInfo(t).CollectionKind;
        }

        public static IList<Type> GetContentTypes(Type t)
        {
            return GetTypeInfo(t).ContentTypes;
        }

        public static IList<Type> GetContentWrappers(Type t)
        {
            return GetTypeInfo(t).ContentWrappers;
        }

        static TypeInfo GetTypeInfo(Type t)
        {
            TypeInfo info;
            if (!s_supportedTypes.TryGetValue(t, out info))
            {
                throw new NotSupportedException(t.ToString() + " is not supported.");
            }
            return info;
        }

        class TypeInfo
        {
            public Func<object> CreationDelegate { get; set; }
            public MethodInfo AddMethod { get; set; }
            public MethodInfo EnumeratorMethod { get; set; }

            private Dictionary<string, MemberInfo> _memberMap = new Dictionary<string, MemberInfo>();
            public Dictionary<string, MemberInfo> MemberMap { get { return _memberMap; } }

            private HashSet<string> _attachableMembers = new HashSet<string>();
            public ICollection<string> AttachableMembers { get { return _attachableMembers; } }

            public XamlCollectionKind CollectionKind { get; set; }

            public PropertyInfo ContentProperty { get; set; }

            List<Type> _contentTypes = new List<Type>();
            public IList<Type> ContentTypes { get { return _contentTypes; } }

            List<Type> _contentWrappers = new List<Type>();
            public IList<Type> ContentWrappers { get { return _contentWrappers; } }
        }
    }
}
