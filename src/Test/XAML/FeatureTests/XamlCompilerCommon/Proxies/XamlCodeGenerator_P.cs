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
    class XamlCodeGenerator
    {
        static ProxyHelper s_xamlCodeGeneratorType;
        static MethodInfo s_generateMethod;

        object _instance;
        String _language;

        static XamlCodeGenerator()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_xamlCodeGeneratorType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.XamlCodeGenerator");
            s_generateMethod = s_xamlCodeGeneratorType.GetMethod("Generate");
        }

        public XamlCodeGenerator(string language)
        {
            _language = language;
            object[] args = new object[] { language };
            _instance = s_xamlCodeGeneratorType.CreateInstance(args);
        }

        public List<KeyValuePair<String, String>> Generate(XamlClassCodeInfo codeInfo,
                                                            String loadComponentUri,
                                                            String relativePathToXamlFile)
        {
            Object[] args = new Object[] { codeInfo.Instance, loadComponentUri, relativePathToXamlFile };
            Object result = s_generateMethod.Invoke(_instance, args);
            return (List<KeyValuePair<String, String>>)result;
        }
    }
}
