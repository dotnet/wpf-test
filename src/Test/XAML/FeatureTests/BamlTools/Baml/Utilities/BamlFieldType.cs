// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Baml.Utilities
{

    public enum BamlFieldType : byte
    {
        None = 0,
        BamlVersion,
        LoadAsync,
        MaxAsyncRecords,
        TypeId,
        AttributeId,
        Value,
        ExtensionTypeId,
        SerializerTypeId,
        LineNumber,
        LinePosition,
        ConverterTypeId,
        NamespacePrefix,
        NameId,
        XmlNamespace,
        ClrNamespace,
        AssemblyId,
        AssemblyFullName,
        TypeFullName,
        AttributeUsage,
        StringId,
        ValuePosition,
        ValueId,
        StaticResourceId,
        ContentSize,
        Shared,
        SharedSet,
        ConnectionId,
        RuntimeName,
        FlagsByte,
        DebugBamlStream,
        AssemblyIdList,
        LastFieldType
    }
}
