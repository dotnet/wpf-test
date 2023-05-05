// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeGenInspect.Proxies
{
    public class FieldDefinition: CodeBehindElement
    {
        static ProxyHelper s_fieldDefinitionType;
        static PropertyInfo s_nameProperty;
        static PropertyInfo s_typeNameProperty;

        object _instance;

        static FieldDefinition()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_fieldDefinitionType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.FieldDefinition");
            s_nameProperty = s_fieldDefinitionType.GetProperty("Name");
            s_typeNameProperty = s_fieldDefinitionType.GetProperty("TypeName");
        }

        public FieldDefinition(object instance)
            :base(instance)
        {
            _instance = instance;
        }

        public String Name
        {
            get
            {
                return (string)s_nameProperty.GetValue(_instance, null);
            }
        }

        public String TypeName
        {
            get
            {
                return (string)s_typeNameProperty.GetValue(_instance, null);
            }
        }

    }
}
