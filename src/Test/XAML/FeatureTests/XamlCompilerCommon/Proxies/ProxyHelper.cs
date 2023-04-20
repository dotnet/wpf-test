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
    class ProxyHelper
    {
        Type _type;

        public ProxyHelper(Assembly asm, String fullName)
        {
            _type = asm.GetType(fullName, true);
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(_type);
        }

        public object CreateInstance(object[] args)
        {
            return Activator.CreateInstance(_type, args);
        }

        public PropertyInfo GetProperty(String name)
        {
            PropertyInfo pi = _type.GetProperty(name);
            if (pi == null)
            {
                throw new InvalidOperationException(String.Format("Could not get Property {0} from Type {1}.", name, _type.FullName));
            }
            return pi;
        }

        public MethodInfo GetMethod(String name)
        {
            MethodInfo mi = _type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
            if (mi == null)
            {
                throw new InvalidOperationException(String.Format("Could not get Method {0} from Type {1}.", name, _type.FullName));
            }
            return mi;
        }


    }
}
