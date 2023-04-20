// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xaml;

namespace CodeGenInspect.Proxies
{
    class XamlCompilerReflectionHelper
    {
        static ProxyHelper s_xamlCompilerReflectionHelperType;
        static MethodInfo s_createCompilerDomRootMethod;

        object _instance;

        static XamlCompilerReflectionHelper()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_xamlCompilerReflectionHelperType = new ProxyHelper(asm, "XamlCompilerCore.SampleTaskCode.XamlCompilerReflectionHelper");
            s_createCompilerDomRootMethod = s_xamlCompilerReflectionHelperType.GetMethod("CreateCompilerDomRoot");
        }

        public XamlCompilerReflectionHelper()
        {
            _instance = s_xamlCompilerReflectionHelperType.CreateInstance();
        }

        public CompilerDomRoot CreateCompilerDomRoot(XamlReader xamlReader)
        {
            Object[] args = new Object[] { xamlReader };
            Object result = s_createCompilerDomRootMethod.Invoke(_instance, args);
            return new CompilerDomRoot(result);
        }

    }
}
