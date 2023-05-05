// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 *FxCop Violation Message Suppressions
 */
using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Scope = "member", Target = "Microsoft.Test.Xaml.Schema.ExtensionMethods.#Serialize(System.Xaml.Schema.XamlTypeName)")]
[module: SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Scope = "member", Target = "Microsoft.Test.Xaml.Schema.GetAllXamlNamespacesTest.#Run()")]
[module: SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Scope = "member", Target = "Microsoft.Test.Xaml.Schema.GetPreferredPrefixTest.#Run()")]
[module: SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Scope = "member", Target = "Microsoft.Test.Xaml.Schema.GetXamlNamespacesTest.#Run()")]
[module: SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods", Scope = "member", Target = "Microsoft.Test.Xaml.Schema.SchemaTestBase.#.ctor()")]

[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#AddPrivateAttachableEvent1Handler(System.Object,System.EventHandler)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#AddPrivateAttachableEvent2Handler(System.Object,System.EventHandler)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#GetPrivateAttachableIntProperty(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#GetPrivateAttachableStringProperty(System.Object)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#get_PrivateIntProperty()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#set_PrivateIntProperty(System.Int32)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#get_PrivateStaticIntProperty()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#set_PrivateStaticIntProperty(System.Int32)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#get_PrivateStaticStringProperty()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#set_PrivateStaticStringProperty(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#get_PrivateStringProperty()")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#set_PrivateStringProperty(System.String)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#RemovePrivateAttachableEvent1Handler(System.Object,System.EventHandler)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#RemovePrivateAttachableEvent2Handler(System.Object,System.EventHandler)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#SetPrivateAttachableIntProperty(System.Object,System.Int32)")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#SetPrivateAttachableStringProperty(System.Object,System.String)")]

[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Namescope", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeImplementingINamescope")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithUidProperty")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithUidProperty.#UidProperty")]

[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeImplementingIEnumerableAndICollectionOfString")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeImplementingIEnumerableAndIDictionaryOfIntString")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeImplementingIEnumerableAndWithOneParameterAddMethod")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeImplementingIEnumerableAndWithTwoParameterAddMethod")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithGetEnumeratorAndImplementingICollectionOfString")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithGetEnumeratorAndImplementingIDictionaryOfIntString")]
[module: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithGetEnumeratorAndImplementingIList")]

[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.ContentWrapperWithIntContent.#IntProperty")]
[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithOneContentProperty.#IntProperty")]
[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#GetPublicAttachableIntProperty(System.Object)")]
[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#PublicIntProperty")]
[module: SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Scope = "member", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithPropertiesAndEvents.#PublicStaticIntProperty")]

[module: SuppressMessage("Microsoft.Design", "CA1039:ListsAreStronglyTyped", Scope = "type", Target = "Microsoft.Test.Xaml.CustomTypes.Schema.TypeWithGetEnumeratorAndImplementingIList")]
