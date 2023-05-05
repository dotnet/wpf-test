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
    public class EventAssignment: CodeBehindElement
    {
        static ProxyHelper s_eventAssignmenType;

        static PropertyInfo s_eventNameProperty;
        static PropertyInfo s_eventTypeNameProperty;
        static PropertyInfo s_targetTypeNamerProperty;
        static PropertyInfo s_handlerNameProperty;

        object _instance;

        static EventAssignment()
        {
            Assembly asm = XamlClassCodeInfo.XamlCompilerCoreAssembly;
            s_eventAssignmenType = new ProxyHelper(asm, "Microsoft.Xaml.Tools.XamlCompiler.EventAssignment");
            s_eventNameProperty = s_eventAssignmenType.GetProperty("EventName");
            s_eventTypeNameProperty = s_eventAssignmenType.GetProperty("EventTypeName");
            s_targetTypeNamerProperty = s_eventAssignmenType.GetProperty("TargetTypeName");
            s_handlerNameProperty = s_eventAssignmenType.GetProperty("HandlerName");
        }

        public EventAssignment(object instance)
            : base(instance)
        {
            _instance = instance;
        }

        public String EventName
        {
            get
            {
                return (string)s_eventNameProperty.GetValue(_instance, null);
            }
        }

        public String EventTypeName
        {
            get
            {
                return (string)s_eventTypeNameProperty.GetValue(_instance, null);
            }
        }

        public String TargetTypeName
        {
            get
            {
                return (string)s_targetTypeNamerProperty.GetValue(_instance, null);
            }
        }

        public String HandlerName
        {
            get
            {
                return (string)s_handlerNameProperty.GetValue(_instance, null);
            }
        }

    }
}
