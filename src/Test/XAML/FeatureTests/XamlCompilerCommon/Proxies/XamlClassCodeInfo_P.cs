// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace CodeGenInspect.Proxies
{
    public class XamlClassCodeInfo
    {
        static Assembly s_xamlCompilerCoreAssembly;
        static ProxyHelper s_xcciType;
        static PropertyInfo s_classFullNameProperty;
        static PropertyInfo s_xamlFileNameProperty;
        static PropertyInfo s_classNamespaceProperty;
        static PropertyInfo s_classShortProperty;
        static PropertyInfo s_baseTypeNameProperty;
        static PropertyInfo s_fieldDefinitionsProperty;
        static PropertyInfo s_eventAssignmentsProperty;
        static MethodInfo s_toStringMethod;

        object _instance;

        static XamlClassCodeInfo()
        {
            s_xamlCompilerCoreAssembly = Assembly.Load("XamlCompilerCore");

            s_xcciType = new ProxyHelper(s_xamlCompilerCoreAssembly, "Microsoft.Xaml.Tools.XamlCompiler.XamlClassCodeInfo");
            s_classFullNameProperty = s_xcciType.GetProperty("ClassFullName");
            s_xamlFileNameProperty = s_xcciType.GetProperty("XamlFileName");
            s_classNamespaceProperty = s_xcciType.GetProperty("ClassNamespace");
            s_classShortProperty = s_xcciType.GetProperty("ClassShortName");
            s_baseTypeNameProperty = s_xcciType.GetProperty("BaseTypeName");
            s_fieldDefinitionsProperty = s_xcciType.GetProperty("FieldDefinitions");
            s_eventAssignmentsProperty = s_xcciType.GetProperty("EventAssignments");
            s_toStringMethod = s_xcciType.GetMethod("ToString");

        }

        // -------------------- static properties -------------

        public static Assembly XamlCompilerCoreAssembly
        {
            get { return s_xamlCompilerCoreAssembly; }
        }

        // ------------------ instance properties ----------------

        public XamlClassCodeInfo(object instance)
        {
            _instance = instance;
        }

        public object Instance
        {
            get { return _instance; }
        }

        public string ClassFullName
        {
            get
            {
                return (string)s_classFullNameProperty.GetValue(_instance, null);
            }
        }

        public string XamlFileName
        {
            get
            {
                return (string)s_xamlFileNameProperty.GetValue(_instance, null);
            }
        }

        public string ClassNamespace
        {
            get
            {
                return (string)s_classNamespaceProperty.GetValue(_instance, null);
            }
        }

        public string ClassShortName
        {
            get
            {
                return (string)s_classShortProperty.GetValue(_instance, null);
            }
        }


        public String BaseTypeName
        {
            get
            {
                return (string)s_baseTypeNameProperty.GetValue(_instance, null);
            }
        }

        public List<FieldDefinition> FieldDefinitions
        {
            get
            {
                IEnumerable objectList = (IEnumerable)s_fieldDefinitionsProperty.GetValue(_instance, null);
                List<FieldDefinition> fieldList = new List<FieldDefinition>();
                foreach (Object obj in objectList)
                {
                    FieldDefinition field = new FieldDefinition(obj);
                    fieldList.Add(field);
                }
                return fieldList;
            }
        }

        public List<EventAssignment> EventAssignments
        {
            get
            {
                IEnumerable objectList = (IEnumerable)s_eventAssignmentsProperty.GetValue(_instance, null);
                List<EventAssignment> EventAssignmentList = new List<EventAssignment>();
                foreach (Object obj in objectList)
                {
                    EventAssignment eventAssignment = new EventAssignment(obj);
                    EventAssignmentList.Add(eventAssignment);
                }
                return EventAssignmentList;

            }
        }

        public override string ToString()
        {
            return (string)s_toStringMethod.Invoke(_instance, null);
        }

    }
}
