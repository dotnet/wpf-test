// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace BamlDasm
{

    public enum BamlFieldType: byte
    {
        None =0,
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
        LastFieldType
    }

    public struct BamlDasmField
    {
        private static Type[] s_clrTypes;

        private BamlFieldType _fieldType;
        private string _name;
        private object _val;

        public BamlDasmField(BamlFieldType ft, string n)
        {
            _fieldType = ft;
            _name = n;
            _val = null;
        }

        public BamlDasmField(BamlFieldType ft)
        {
            _fieldType = ft;
            _name = null;
            _val = null;
        }

        static BamlDasmField()
        {
            s_clrTypes = new Type[(int)BamlFieldType.LastFieldType];

            s_clrTypes[(int)BamlFieldType.BamlVersion]      = typeof(MockFormatVersion);
            s_clrTypes[(int)BamlFieldType.LoadAsync]        = typeof(bool);
            s_clrTypes[(int)BamlFieldType.MaxAsyncRecords]  = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.TypeId]           = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.AttributeId]      = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.Value]            = typeof(string);
            s_clrTypes[(int)BamlFieldType.ExtensionTypeId]  = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.SerializerTypeId] = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.LineNumber]       = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.LinePosition]     = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.ConverterTypeId]  = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.NamespacePrefix]  = typeof(string);
            s_clrTypes[(int)BamlFieldType.NameId]           = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.XmlNamespace]     = typeof(string);
            s_clrTypes[(int)BamlFieldType.ClrNamespace]     = typeof(string);
            s_clrTypes[(int)BamlFieldType.AssemblyId]       = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.AssemblyFullName] = typeof(string);
            s_clrTypes[(int)BamlFieldType.TypeFullName]     = typeof(string);
            s_clrTypes[(int)BamlFieldType.AttributeUsage]   = typeof(byte);
            s_clrTypes[(int)BamlFieldType.StringId]         = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.ValuePosition]    = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.ValueId]          = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.StaticResourceId] = typeof(Int16);
            s_clrTypes[(int)BamlFieldType.ContentSize]      = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.Shared]           = typeof(bool);
            s_clrTypes[(int)BamlFieldType.SharedSet]        = typeof(bool);
            s_clrTypes[(int)BamlFieldType.ConnectionId]     = typeof(Int32);
            s_clrTypes[(int)BamlFieldType.RuntimeName]      = typeof(string);
            s_clrTypes[(int)BamlFieldType.FlagsByte]        = typeof(byte);
            s_clrTypes[(int)BamlFieldType.DebugBamlStream]  = typeof(bool);

            for(int i=1; i<(int)BamlFieldType.LastFieldType; i++)
            {
                if(s_clrTypes[i] == null)
                    throw new Exception("A CLR definition for BamlFieldType " + ((BamlFieldType)i).ToString() +" is missing.  ");
            }
        }

        public string Name
        {
            get { return (null != _name) ? _name : _fieldType.ToString(); }
        }

        public Type ClrType
        {
            get { return s_clrTypes[(int)_fieldType]; }
        }

        public BamlFieldType BamlFieldType
        {
            get { return _fieldType; }
        }

        public object Value
        {
            set { _val = value; }
            get { return _val; }
        }
    }
}

