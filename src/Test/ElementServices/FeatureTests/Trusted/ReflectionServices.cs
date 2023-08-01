// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Provides convenient access to non-public classes and members.
 * 
 * Contributors: 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using System.Security;
using System.Diagnostics;

namespace Avalon.Test.CoreUI.Trusted.Utilities
{
    /// <summary>
    /// Provides convenient access to non-public classes and members.
    /// </summary>
    public class InternalObject
    {
        /// <summary>
        /// Hold a hard reference to the internal object and type.
        /// </summary>
        private object _target = null;
        private Type _originalType = null;
        
        /// <summary>
        /// Returns the object wrapped by this InternalObject.
        /// </summary>
        public object Target
        {
            get
            {
                return _target;
            }
        }

        /// <summary>
        /// Returns the real, wrapped type.
        /// </summary>
        /// <returns></returns>
        public Type GetRealType()
        {
            return _originalType;
        }

        /// <summary>
        /// Instantiates an internal type wrapped by a new InternalObject instance.
        /// </summary>
        /// <param name="assemblyName">The assembly that has the type.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="args">The args to pass to the type's constructor.</param>
        public static InternalObject CreateInstance(string assemblyName, string typeName, object[] args)
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            Type type = GetType(assemblyName, typeName);

            object obj = Activator.CreateInstance(
                type,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                args,
                null
            );

            CodeAccessPermission.RevertAssert();

            return new InternalObject(obj);
        }
        /// <summary>
        /// Creates a delegate of the specified type on the given method.
        /// </summary>
        /// <param name="assemblyName">The assembly that has the type.</param>
        /// <param name="typeName">The name of the delegate type.</param>
        /// <param name="methodName">The method on which to create the delegate.</param>
        /// <param name="instance">Instance of the type that has the method.</param>
        /// <returns></returns>
        public static Delegate CreateDelegate(string assemblyName, string typeName, string methodName, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            Type delegateType = InternalObject.GetType(assemblyName, typeName);
            
            MethodInfo info = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            
            Delegate callback = Delegate.CreateDelegate(delegateType, instance, info);

            CodeAccessPermission.RevertAssert();

            return callback;
        }
        /// <summary>
        /// Returns a Type from the given assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly that has the type.</param>
        /// <param name="typeName">The name of the type.</param>
        public static Type GetType(string assemblyName, string typeName)
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            Assembly assembly = Assembly.Load(assemblyName);

            Type type = assembly.GetType(typeName, true, false);

            CodeAccessPermission.RevertAssert();

            return type;
        }
        /// <summary>
        /// Instantiates an internal generic type wrapped by a new InternalObject instance.
        /// </summary>
        /// <param name="assemblyName">The assembly that has the type.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <param name="args">The args to pass to the type's constructor.</param>
        /// <param name="typeArguments">The Type args to bind to the generic type.</param>
        public static InternalObject CreateGenericInstance(string assemblyName, string typeName, object[] args, Type[] typeArguments)
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            Type type = GetType(assemblyName, typeName);

            type = type.MakeGenericType(typeArguments);

            // Create instance of generic type.
            object obj = Activator.CreateInstance(
                type,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                args,
                null
            );

            CodeAccessPermission.RevertAssert();

            return new InternalObject(obj);
        }
        /// <summary>
        /// Wraps an existing object.
        /// </summary>
        public InternalObject(object obj) 
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            // Validation Code
            if (null==obj) throw new ArgumentNullException("object");
            
            // Create a hard reference
            _target = obj;
            _originalType = obj.GetType();

            System.Security.CodeAccessPermission.RevertAssert();
        }
        /// <summary>
        /// Creates a new InternalObject based on a specified type within a specified assembly. The InternalObject doesn't hold an instance of the type.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <remarks>This constructor is useful for getting internal classes which have static members, which cannot be called by getting an instance of the type.</remarks>
        public InternalObject(string assemblyName, string typeName) 
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            Assembly assembly = Assembly.Load(assemblyName);
            _originalType = assembly.GetType(typeName, true, false);
            
            _target = null;

            System.Security.CodeAccessPermission.RevertAssert();
        }
        /// <summary>
        /// Invokes a member with specific BindingFlags and arguments.
        /// </summary>
        public object AccessMember(string name, BindingFlags bindingFlags, object[] parameters)
        {
            ReflectionPermission permission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
            permission.Assert();

            object target = this.Target;
            
            // Call the Method
            object returnval =_originalType.InvokeMember(
                name,
                bindingFlags,
                null,
                target,
                parameters);

            System.Security.CodeAccessPermission.RevertAssert();

            return returnval;
        }
        /// <summary>
        /// Invokes an internal method.
        /// </summary>
        public object InvokeMethod(string name)
        {
            return InvokeMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, new object[] { });
        }
        /// <summary>
        /// Invokes an internal method with specific parameters.
        /// </summary>
        public object InvokeMethod(string name, params object[] parameters)
        {
            return InvokeMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, parameters);
        }
        /// <summary>
        /// Invokes a method with specific parameters and BindingFlags.
        /// </summary>
        public object InvokeMethod(string name, BindingFlags bindingFlags, object[] parameters)
        {
            return AccessMember(name, BindingFlags.InvokeMethod | bindingFlags, parameters);
        }
        /// <summary>
        /// Gets an internal indexer.
        /// </summary>
        public object GetIndexer(object key)
        {
            return GetIndexer(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        /// <summary>
        /// Gets an internal indexer using specific BindingFlags.
        /// </summary>
        public object GetIndexer(object key, BindingFlags bindingFlags)
        {
            return AccessMember("Item", BindingFlags.GetProperty | bindingFlags, new object[] { key });
        }
        /// <summary>
        /// Sets an internal indexer.
        /// </summary>
        public void SetIndexer(object key, object val)
        {
            SetIndexer(key, val, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        /// <summary>
        /// Sets an internal indexer using specific BindingFlags.
        /// </summary>
        public void SetIndexer(object key, object val, BindingFlags bindingFlags)
        {
            AccessMember("Item", BindingFlags.SetProperty | bindingFlags, new object[] { key, val });
        }
        /// <summary>
        /// Reads an internal field.
        /// </summary>
        public object GetField(string name)
        {
            return GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        /// <summary>
        /// Reads a field using specific BindingFlags.
        /// </summary>
        public object GetField(string name, BindingFlags bindingFlags)
        {
            return AccessMember(name, BindingFlags.GetField | bindingFlags, null);
        }
        /// <summary>
        /// Sets an internal field.
        /// </summary>
        public void SetField(string name, object val)
        {
            SetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, val);
        }
        /// <summary>
        /// Sets a field using specific BindingFlags.
        /// </summary>
        public void SetField(string name, BindingFlags bindingFlags, object val)
        {
            AccessMember(name, BindingFlags.SetField | bindingFlags, new object[] { val });
        }
        /// <summary>
        /// Reads an internal property.
        /// </summary>
        public object GetProperty(string name)
        {
            return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        /// <summary>
        /// Reads a property using specific BindingFlags.
        /// </summary>
        public object GetProperty(string name, BindingFlags bindingFlags)
        {
            return AccessMember(name, BindingFlags.GetProperty | bindingFlags, null);
        }
        /// <summary>
        /// Sets an internal property.
        /// </summary>
        public void SetProperty(string name, object val)
        {
            this.SetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, val);
        }
        /// <summary>
        /// Sets a property using specific BindingFlags.
        /// </summary>
        public void SetProperty(string name, BindingFlags bindingFlags, object val)
        {
            AccessMember(name, BindingFlags.SetProperty | bindingFlags, new object[]{val});
        }
    }
}
