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
    class XamlHarvester
    {
        static ProxyHelper s_xamlHarvesterType;
        static MethodInfo s_harvestMethod;

        object _instance;

        static XamlHarvester()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_xamlHarvesterType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.XamlHarvester");
            s_harvestMethod = s_xamlHarvesterType.GetMethod("Harvest");
        }

        public XamlHarvester()
        {
            _instance = s_xamlHarvesterType.CreateInstance();
        }

        public XamlClassCodeInfo Harvest(CompilerDomRoot domRoot, String xamlFileName)
        {
            Object[] args = new Object[] { domRoot.Instance, xamlFileName };
            Object result = s_harvestMethod.Invoke(_instance, args);
            return new XamlClassCodeInfo(result);
        }

    }
}
