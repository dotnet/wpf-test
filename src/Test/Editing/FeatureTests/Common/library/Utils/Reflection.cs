// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides Reflection utility methods for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Reflection.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Reflection;
    using Test.Uis.Loggers;

    #endregion Namespaces.

    /// <summary>
    /// Provides Reflection utility methods for test cases.
    /// </summary>
    public static class ReflectionUtils
    {
        #region Public methods.

        /// <summary>Adds an event handler to a named event on an object.</summary>
        /// <param name='target'>Object on which to add event handler.</param>
        /// <param name='eventName'>Name of event to add to.</param>
        /// <param name='handler'>Delegate for event.</param>
        public static void AddInstanceEventHandler(object target, string eventName, Delegate handler)
        {
            AddRemoveInstanceEventHandler(target, eventName, handler, true);
        }

        /// <summary>
        /// Creates an instance of the specified type.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <returns>A reference to the newly created object.</returns>
        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Supply type name and it will create the object for you.
        /// </summary>
        /// <param name="typeName">name of the type it will try to create</param>
        /// <param name="args">args to the ctor of the object</param>
        /// <returns>A new instance of the specified type.</returns>
        public static object CreateInstanceOfType(string typeName, object[] args)
        {
            Type type;  // Type to be created.

            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (typeName.Length == 0)
            {
                throw new ArgumentException("typeName should not be empty.",
                    "typeName");
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted).
                Assert();

            type = ReflectionUtils.FindType(typeName);
            if (type == null)
            {
                Logger.Current.Log("Cannot get type typename [{0}]", typeName);
                throw new Exception("FindType failed to find type " + typeName);
            }

            // we let it throw exception when error occurs.
            return Activator.CreateInstance(type, args, null);
        }

        /// <summary>
        /// Retrieves an array of strings describing property values on the
        /// specified object.
        /// </summary>
        /// <param name='o'>Object to describe.</param>
        /// <returns>Lines describing the specified object.</returns>
        /// <remarks>
        /// The format of the returned strings is one line per property,
        /// [name]=[value].
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   string[] props = ReflectionUtils.DescribeProperties(MyControl);
        ///   Logger.Current.Log(String.Join("\n", props));
        /// }</code></example>
        public static string[] DescribeProperties(object o)
        {
            if (o == null)
            {
                return new string[0];
            }
            else
            {
                Type type = o.GetType();
                PropertyInfo[] props = type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic);
                string[] result = new string[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    result[i] = props[i].Name + "=" +
                        props[i].GetValue(o, null);
                }
                return result;
            }
        }

        /// <summary>
        /// Retrieves a type given its name.
        /// </summary>
        /// <param name='typeName'>
        /// Simple name, name with namespace, or type name with
        /// partial or fully qualified assembly to look for.
        /// </param>
        /// <returns>The specified Type.</returns>
        /// <remarks>
        /// If an assembly is specified, then this method is equivalent
        /// to calling Type.GetType(). Otherwise, all loaded assemblies
        /// will be used to look for the type (unlike Type.GetType(),
        /// which would only look in the calling assembly and in mscorlib).
        /// </remarks>
        public static Type FindType(string typeName)
        {
            Type returnValue=null;
            string preferedType = "System.Windows";
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            // A comma indicates a type with an assembly reference
            if (typeName.IndexOf(',') != -1)
                return Type.GetType(typeName, true);

            // A period indicates a namespace is present.
            bool hasNamespace = typeName.IndexOf('.') != -1;

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                if (hasNamespace)
                {
                    Type t = assembly.GetType(typeName);
                    if (t != null)
                    {
                        return t;
                    }
                }
                else
                {
                    // For type names that are not fully qualified
                    // and don't even have namespaces, explicitly
                    // opt out of WinForms (avoids collision with Avalon
                    // the more common case).
                    // Also exclude System.Activities because of LoaderExceptions caused on load attempt
                    if (assembly.FullName.Contains("Forms") ||
                        assembly.FullName.Contains("System.Activities"))
                    {
                        continue;
                    }
                    Type [] testTypes = SafeGetTypes(assembly);
                    foreach(Type type in testTypes)
                    {
                        if (type != null && type.Name == typeName)
                        {
                            if (type.FullName.Contains(preferedType))
                            {
                                return type;
                            }
                            else if(returnValue == null)
                            {
                                returnValue = type;
                            }
                        }
                    }
                }
            }
            if (returnValue != null)
            {
                return returnValue;
            }
            throw new InvalidOperationException(
                "Unable to find type [" + typeName + "] in loaded assemblies");
        }

        /// <summary>
        /// Gets the name of a type from the (possibly) full-qualified name.
        /// </summary>
        /// <param name="fullTypeName">Full name of type.</param>
        /// <returns>The bare name.</returns>
        /// <example>The following code shows how to use this method.<code>...
        /// private void LogTypeName() {
        ///   string typeName = "My.Namespace.Type.HelloThere, myassembly ver1.2.";
        ///   System.Diagnostics.Debug.Assert(TestFinder.GetNameFromFullTypeName(typeName) == "HelloThere");
        /// }
        /// </code></example>
        public static string GetNameFromFullTypeName(string fullTypeName)
        {
            if (fullTypeName == null)
            {
                throw new ArgumentNullException("fullTypeName");
            }
            if (fullTypeName.Length == 0)
            {
                throw new ArgumentException("Full type name cannot be empty", "fullTypeName");
            }
            // Remove the assembly reference.
            int comma = fullTypeName.IndexOf(',');
            if (comma != -1)
            {
                fullTypeName = fullTypeName.Substring(0, comma);
            }
            // Remove the prefixed namespaces.
            int lastPeriod = fullTypeName.LastIndexOf('.');
            if (lastPeriod == -1)
                return fullTypeName;
            else
                return fullTypeName.Substring(lastPeriod+1);
        }

        /// <summary>
        /// Retrieves the value of a named field, disregarding the
        /// visibility.
        /// </summary>
        /// <param name="target">Object from which to get value.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the specified field.</returns>
        public static object GetField(object target, string fieldName)
        {
            Type type;
            Type queryType;
            FieldInfo fieldInfo;

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (fieldName.Length == 0)
            {
                throw new ArgumentException("Field name cannot be blank", fieldName);
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.FlattenHierarchy;

            type = target.GetType();
            queryType = type;
            fieldInfo = queryType.GetField(fieldName, bindingAttr);
            while (fieldInfo == null && queryType != typeof(object))
            {
                queryType = queryType.BaseType;
                fieldInfo = queryType.GetField(fieldName, bindingAttr);
            }
            if (fieldInfo == null)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve field information for field [" +
                    fieldName + "] with attributes [" + bindingAttr +
                    "] from type [" + type + "]");
            }
            return fieldInfo.GetValue(target);
        }

        /// <summary>
        /// Retrieves the value of a named property as implementing the 
        /// specified interface.
        /// </summary>
        /// <param name="target">Object from which to get value.</param>
        /// <param name="interfaceName">Name of the interface on which property is declared.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the specified property.</returns>
        public static object GetInterfaceProperty(object target,
            string interfaceName, string propertyName)
        {
            BindingFlags bindingFlags;  // Flags to match property to get.
            Type type;                  // Type to get property from.

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (interfaceName == null)
            {
                throw new ArgumentNullException("interfaceName");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            type = target.GetType();
            type = type.GetInterface(interfaceName);
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy |
                BindingFlags.Instance | BindingFlags.GetProperty;
            return type.InvokeMember(propertyName, bindingFlags, null, target, null);
        }

        /// <summary>
        /// Retrieves the value of a named property.
        /// </summary>
        /// <param name="target">Object from which to get value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the specified property.</returns>
        public static object GetProperty(object target, string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            if (propertyName.Length == 0)
            {
                throw new ArgumentException("Property name cannot be blank", propertyName);
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.FlattenHierarchy;
            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName, bindingAttr);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve property information for property [" +
                    propertyName + "] with attributes [" + bindingAttr +
                    "] from type [" + type + "]");
            }
            return propertyInfo.GetValue(target, null);
        }

        /// <summary>
        /// Retrieves the value of a static named field.
        /// </summary>
        /// <param name="type">Type from which to get value.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the specified field.</returns>
        public static object GetStaticField(Type type, string fieldName)
        {
            FieldInfo fieldInfo;

            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.FlattenHierarchy;

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (fieldName.Length == 0)
            {
                throw new ArgumentException("Field name cannot be blank", fieldName);
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            fieldInfo = type.GetField(fieldName, bindingAttr);
            if (fieldInfo == null)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve field information for property [" +
                    fieldName + "] with attributes [" + bindingAttr +
                    "] from type [" + type + "]");
            }
            return fieldInfo.GetValue(null);
        }

        /// <summary>
        /// Retrieves the value of a static named property.
        /// </summary>
        /// <param name="type">Type from which to get value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the specified property.</returns>
        public static object GetStaticProperty(Type type, string propertyName)
        {
            PropertyInfo propertyInfo;

            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.FlattenHierarchy;

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            if (propertyName.Length == 0)
            {
                throw new ArgumentException("Property name cannot be blank", propertyName);
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            propertyInfo = type.GetProperty(propertyName, bindingAttr);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve property information for property [" +
                    propertyName + "] with attributes [" + bindingAttr +
                    "] from type [" + type + "]");
            }
            return propertyInfo.GetValue(null, null);
        }

        /// <summary>
        /// Retrieves the type of the named property on a type.
        /// </summary>
        /// <param name="type">Type that has the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The Type of the specified property.
        /// </returns>
        public static Type GetPropertyType(Type type, string propertyName)
        {
            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.FlattenHierarchy;

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            PropertyInfo propertyInfo = type.GetProperty(propertyName, bindingAttr);
            return propertyInfo.PropertyType;
        }

        /// <summary>
        /// Returns an object suitable for comparison with the specified
        /// target type from the given object.
        /// </summary>
        /// <param name='o'>Object to return value for.</param>
        /// <param name='targetType'>Target type.</param>
        /// <remarks>
        /// This method is similar to the Change.ChangeType API call.
        /// However, if o is a string with value '*null', a null value
        /// will be returned. If targetType is System.String and o is
        /// not null, o.ToString() will be returned rather than the
        /// simple ChangeType (*null if o is null). If the target is an
        /// enumeration and o is a string, then the Enum.Parse method
        /// is invoked. Also, all conversions are done with the invariant
        /// culture.
        /// </remarks>
        public static object GetValueForComparison(object o, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            const string NullString = "*null";
            bool isTargetString = targetType == typeof(System.String);
            bool isTargetEnum = targetType.IsSubclassOf(typeof(System.Enum));

            // Handle the case of a null object.
            if (o == null)
            {
                return (isTargetString)? NullString : null;
            }
            else
            {
                if (isTargetString)
                {
                    return o.ToString();
                }
                if ((o is System.String) && isTargetEnum)
                {
                    return Enum.Parse(targetType, (string)o, true);
                }
                return Convert.ChangeType(o, targetType,
                    System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        /// <summary>Removes an event handler from a named event on an object.</summary>
        /// <param name='target'>Object from which to remove event handler.</param>
        /// <param name='eventName'>Name of event to remove from.</param>
        /// <param name='handler'>Delegate for event.</param>
        public static void RemoveInstanceEventHandler(object target, string eventName, Delegate handler)
        {
            AddRemoveInstanceEventHandler(target, eventName, handler, false);
        }

        /// <summary>
        /// Retrieves types, guaranteeing that no exceptions will be thown.
        /// </summary>
        /// <param name="assembly">Assembly to get types from.</param>
        /// <returns>
        /// The types in the specifies assembly, or a zero-length array if an
        /// exception was thrown and no types could be accessed. If only some
        /// types are loaded, these are returned, and the returned array
        /// will have null elements.
        /// </returns>
        public static Type[] SafeGetTypes(Assembly assembly)
        {
            Type[] testTypes = Type.EmptyTypes;
            try
            {
                Type[] types = assembly.GetTypes();
                testTypes = new Type[types.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    testTypes[i] = types[i];
                }
            }
            catch(ReflectionTypeLoadException re)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("ReflectionTypeLoadException thrown while getting types for ");
                sb.Append(assembly.FullName);
                sb.Append(System.Environment.NewLine);
                sb.Append(re.ToString());
                sb.Append("Loader exceptions: ");
                sb.Append(re.LoaderExceptions.Length);
                sb.Append(System.Environment.NewLine);
                foreach(Exception exception in re.LoaderExceptions)
                {
                    sb.Append(exception.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                Logger.Current.Log("Partial ReflectionUtils.SafeGetTypes execution completed.");
                Logger.Current.Log(sb.ToString());
                return re.Types;
            }
            catch(Exception exception)
            {
                Logger.Current.Log(
                    "Exception thrown while getting types for {0}{1}{2}",
                    assembly.FullName, Environment.NewLine,
                    exception.ToString());
            }
            return testTypes;
        }

        /// <summary>
        /// Invokes a method defined on a specific interface on the
        /// given object.
        /// </summary>
        /// <param name="instance">Object to invoke method on.</param>
        /// <param name="interfaceName">Name of interface defining the method.</param>
        /// <param name="methodName">Name of method to invoke.</param>
        /// <param name="methodArguments">Arguments for the method.</param>
        /// <returns>The return value of the invoked member.</returns>
        public static object InvokeInterfaceMethod(object instance, string interfaceName,
            string methodName, object[] methodArguments)
        {
            BindingFlags bindingFlags;  // Flags to match method to invoke.
            Type type;                  // Type to invoke method on.

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (interfaceName == null)
            {
                throw new ArgumentNullException("interfaceName");
            }
            if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }
            if (methodArguments == null)
            {
                throw new ArgumentNullException("methodArguments");
            }

            type = instance.GetType();
            type = type.GetInterface(interfaceName);
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy |
                BindingFlags.Instance | BindingFlags.InvokeMethod;
            return type.InvokeMember(methodName, bindingFlags, null, instance,
                methodArguments);
        }

        /// <summary>
        /// Invokes a method on an object.
        /// </summary>
        /// <param name="instance">Object to invoke method on.</param>
        /// <param name="methodName">Name of method to invoke.</param>
        /// <param name="methodArguments">Arguments for the method.</param>
        /// <returns>The return value of the invoked member.</returns>
        public static object InvokeInstanceMethod(object instance, string methodName,
            object[] methodArguments)
        {
            BindingFlags bindingFlags;  // Flags to match method to invoke.
            Type type;                  // Type to invoke method on.

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }
            if (methodArguments == null)
            {
                throw new ArgumentNullException("methodArguments");
            }

            type = instance.GetType();
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy |
                BindingFlags.Instance | BindingFlags.InvokeMethod;
            return type.InvokeMember(methodName, bindingFlags, null, instance,
                methodArguments);
        }

        /// <summary>
        /// Invokes a method on an type.
        /// </summary>
        /// <param name="type">Type to invoke method on.</param>
        /// <param name="methodName">Name of method to invoke.</param>
        /// <param name="methodArguments">Arguments for the method.</param>
        /// <returns>The return value of the invoked member.</returns>
        public static object InvokeStaticMethod(Type type, string methodName,
            object[] methodArguments)
        {
            BindingFlags bindingFlags;  // Flags to match method to invoke.

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }
            if (methodArguments == null)
            {
                throw new ArgumentNullException("methodArguments");
            }

            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.ExactBinding | BindingFlags.FlattenHierarchy |
                BindingFlags.Static | BindingFlags.InvokeMethod;
            return type.InvokeMember(methodName, bindingFlags, null, null,
                methodArguments);
        }

        /// <summary>
        /// call className.methodName static method
        /// </summary>
        /// <param name="className">name of the class where the method exists.</param>
        /// <param name="methodName">name of the method to invoke</param>
        /// <param name="parameters">parameters to the method</param>
        /// <param name="invokeType">Invoke type. See Test.Uis.Utils.InvokeType for details</param>
        /// <returns>return value of hte invoked method</returns>
        public static object InvokePropertyOrMethod(string className,
            string methodName, object[] parameters, InvokeType invokeType)
        {
            System.Security.Permissions.ReflectionPermission perm =
                new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted);
            perm.Assert();

            Type type;
            bool isGetProperty;
            object[] newargs;
            object instance;
            BindingFlags defaultFlags;
            object resultObject;

            if (ActionManager.IsInvokeTypeStatic(invokeType))
            {
                if (String.IsNullOrEmpty(className))
                {
                    throw new ArgumentException(
                        "className cannot be null or empty for static invocations",
                        "className");
                }
                type = ReflectionUtils.FindType(className);
                if (type == null)
                {
                    Logger.Current.Log("Cannot get type typename [{0}]", className);
                    throw new InvalidOperationException("FindType fails");
                }
            }
            else
            {
                if (parameters == null || parameters.Length == 0)
                {
                    throw new ArgumentException(
                        "parameters cannot be null or empty for instance invocations",
                        "parameters");
                }
                if (parameters[0] == null)
                {
                    throw new ArgumentException(
                        "parameters cannot have a null first element for instance invocations",
                        "parameters");
                }
                type = parameters[0].GetType();
            }
            System.Diagnostics.Debug.Assert(type != null);

            defaultFlags = BindingFlags.ExactBinding | BindingFlags.Public;
            newargs = null;
            instance = null;

            switch(invokeType)
            {
                case InvokeType.StaticMethod:
                    defaultFlags |= BindingFlags.InvokeMethod;
                    defaultFlags |= BindingFlags.Static;
                    break;
                case InvokeType.InstanceMethod:
                    defaultFlags |= BindingFlags.InvokeMethod;
                    defaultFlags |= BindingFlags.Instance;
                    instance = ExtractFirstParameterAsInstance(parameters, ref newargs);
                    parameters = newargs;
                    break;
                case InvokeType.GetStaticProperty:
                    defaultFlags |= BindingFlags.GetProperty;
                    defaultFlags |= BindingFlags.Static;
                    break;
                case InvokeType.SetStaticProperty:
                    defaultFlags |= BindingFlags.SetProperty;
                    defaultFlags |= BindingFlags.Static;
                    break;
                case InvokeType.GetInstanceProperty:
                    defaultFlags |= BindingFlags.GetProperty;
                    defaultFlags |= BindingFlags.Instance;
                    instance = ExtractFirstParameterAsInstance(parameters, ref newargs);
                    parameters = newargs;
                    break;
                case InvokeType.SetInstanceProperty:
                    defaultFlags |= BindingFlags.SetProperty;
                    defaultFlags |= BindingFlags.Instance;
                    instance = ExtractFirstParameterAsInstance(parameters, ref newargs);
                    parameters = newargs;
                    break;
                case InvokeType.GetStaticField:
                    defaultFlags |= BindingFlags.GetField;
                    defaultFlags |= BindingFlags.Static;
                    break;
                case InvokeType.SetStaticField:
                    defaultFlags |= BindingFlags.SetField;
                    defaultFlags |= BindingFlags.Static;
                    break;
                case InvokeType.GetInstanceField:
                    defaultFlags |= BindingFlags.GetField;
                    defaultFlags |= BindingFlags.Instance;
                    instance = ExtractFirstParameterAsInstance(parameters, ref newargs);
                    parameters = newargs;
                    break;
                case InvokeType.SetInstanceField:
                    defaultFlags |= BindingFlags.SetField;
                    defaultFlags |= BindingFlags.Instance;
                    instance = ExtractFirstParameterAsInstance(parameters, ref newargs);
                    parameters = newargs;
                    break;
            }

            try
            {
                resultObject = type.InvokeMember(
                    methodName, defaultFlags, null, instance, parameters);
            }
            catch (MissingMethodException e)
            {
                isGetProperty = invokeType == InvokeType.GetInstanceProperty ||
                    invokeType == InvokeType.GetStaticProperty;
                Logger.Current.Log(
                    "MissingMethodException message [{0}], " +
                    "class name [{1}] member name [{2}] binding [{3}]",
                    e.Message, className, methodName, defaultFlags);
                Logger.Current.Log(DescribeParameters(parameters));
                if (parameters.Length > 0 && isGetProperty)
                {
                    Logger.Current.Log(
                        "Note that you are using argument when reading a property. " +
                        "Unless you are using an indexed property, this will fail binding.");
                    Logger.Current.Log(
                        "Double-check whether you want to read a property (remove arg) " +
                        "or set it (change invocation type).");
                }
                throw;
            }
            catch (AmbiguousMatchException e)
            {
                Logger.Current.Log(
                    "AmbiguousMatchException message [{0}], " +
                    "class name [{1}] member name [{2}]",
                    e.Message, className, methodName);
                Logger.Current.Log(DescribeParameters(parameters));
                throw;
            }
            return resultObject;
        }

        /// <summary>
        /// Sets the value of the named property.
        /// </summary>
        /// <param name='target'>Object on which to set value.</param>
        /// <param name='propertyName'>Name of property to set.</param>
        /// <param name='value'>Value of property.</param>
        public static void SetProperty(object target, string propertyName, object value)
        {
            const BindingFlags invokeAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.SetProperty;

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            object[] args = new object[] { value };
            Type type = target.GetType();
            type.InvokeMember(propertyName, invokeAttr, null, target, args);
        }

        #endregion Public methods.


        #region Private methods.

        /// <summary>Adds or removes an event handler to a named event on an object.</summary>
        /// <param name='target'>Object on which to add/remove event handler.</param>
        /// <param name='eventName'>Name of event to add/remove to.</param>
        /// <param name='handler'>Delegate for event.</param>
        /// <param name='add'>true to add the handler, false to remove it.</param>
        public static void AddRemoveInstanceEventHandler(object target, string eventName, 
            Delegate handler, bool add)
        {
            const BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance;

            EventInfo eventInfo;
            Type type;

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (eventName == null)
            {
                throw new ArgumentNullException("eventName");
            }
            if (eventName.Length == 0)
            {
                throw new ArgumentException("Event name should not be blank.", "eventName");
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            type = target.GetType();
            eventInfo = type.GetEvent(eventName, bindingAttr);
            if (eventInfo == null)
            {
                throw new InvalidOperationException(
                    "Unable to retrieve event information for event [" +
                    eventName + "] with attributes [" + bindingAttr +
                    "] from type [" + type + "]");
            }
            if (add)
            {
                eventInfo.AddEventHandler(target, handler);
            }
            else
            {
                eventInfo.RemoveEventHandler(target, handler);
            }
        }

        /// <summary>
        /// Describes the specified array of parameters.
        /// </summary>
        /// <param name="parameters">Parameters to describe.</param>
        /// <returns>A string describing the specified parameters.</returns>
        private static string DescribeParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return "No parameter supplied.";
            }
            string result = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                result += "Parameter " + i + ": ";
                if (parameters[i] == null)
                {
                    result += "null\r\n";
                }
                else
                {
                    result += "[" + parameters[i] + "]" +
                        " (" + parameters[i].GetType().Name + ")\r\n";
                }
            }
            return result;
        }

        /// <summary>
        /// Extract first parameter in args and return it in instance parameter,
        /// the rest of objects in args are copied to newargs.
        /// </summary>
        /// <param name="args">array of objects to be extracted</param>
        /// <param name="newargs">aray of objects in args excluding the first one</param>
        /// <returns>args[0] as object</returns>
        private static object ExtractFirstParameterAsInstance(object[] args, ref object[] newargs)
        {
            if (args.Length > 0)
            {
                object instance = args[0];
                newargs = null;
                if (args.Length > 1)
                {
                    // if we call instance method the first object in args is the instance
                    // we need to extract that.
                    newargs = new object[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        newargs[i - 1] = args[i];
                    }
                }
                return instance;
            }
            else
            {
                throw new ArgumentException("args doesn't have any element", "args");
            }
        }

        #endregion Private methods.
    }
}
