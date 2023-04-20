// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace CodeGenInspect.Proxies
{
    public class CodeBehindElement
    {
        static ProxyHelper s_codeBehindElementType;
        static PropertyInfo s_lineNumberProperty;
        static PropertyInfo s_linePositionProperty;
        static PropertyInfo s_connectionIdProperty;

        object _instance;

        static CodeBehindElement()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_codeBehindElementType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.CodeBehindElement");
            s_lineNumberProperty = s_codeBehindElementType.GetProperty("LineNumber");
            s_linePositionProperty = s_codeBehindElementType.GetProperty("LinePosition");
            s_connectionIdProperty = s_codeBehindElementType.GetProperty("ConnectionId");
        }

        public CodeBehindElement() { }

        public CodeBehindElement(object instance)
        {
            _instance = instance;
        }

        public int LineNumber
        {
            get
            {
                return (int)s_lineNumberProperty.GetValue(_instance, null);
            }
        }

        public int LinePosition
        {
            get
            {
                return (int)s_linePositionProperty.GetValue(_instance, null);
            }
        }

        [DefaultValue(0)]
        public int ConnectionId
        {
            get
            {
                return (int)s_connectionIdProperty.GetValue(_instance, null);
            }
        }
    }
}

