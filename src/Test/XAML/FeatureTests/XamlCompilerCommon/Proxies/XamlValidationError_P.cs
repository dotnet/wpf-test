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
    public class XamlValidationError
    {
        static ProxyHelper s_xamlValidationErrorType;
        static PropertyInfo s_messageProperty;
        static PropertyInfo s_errorTagProperty;
        static PropertyInfo s_lineNumberProperty;
        static PropertyInfo s_lineOffsetProperty;

        object _instance;

        static XamlValidationError()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_xamlValidationErrorType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.XamlValidationError");
            s_messageProperty = s_xamlValidationErrorType.GetProperty("Message");
            s_errorTagProperty = s_xamlValidationErrorType.GetProperty("ErrorTag");
            s_lineNumberProperty = s_xamlValidationErrorType.GetProperty("LineNumber");
            s_lineOffsetProperty = s_xamlValidationErrorType.GetProperty("LineOffset");
        }

        public XamlValidationError(object instance)
        {
            _instance = instance;
        }

        public String Message
        {
            get
            {
                return (string)s_messageProperty.GetValue(_instance, null);
            }
        }

        public String ErrorTag
        {
            get
            {
                return (string)s_errorTagProperty.GetValue(_instance, null);
            }
        }

        public int LineNumber
        {
            get
            {
                return (int)s_lineNumberProperty.GetValue(_instance, null);
            }
        }

        public int LineOffset
        {
            get
            {
                return (int)s_lineOffsetProperty.GetValue(_instance, null);
            }
        }
    }
}
