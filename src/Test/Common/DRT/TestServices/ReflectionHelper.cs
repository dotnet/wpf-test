// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Simplifies reflection for use with 'white box' testing, and Asserts for

//  unrestricted reflection permission on reflection calls.  As such it can

//  not be used to validated reflection demands.

// </summary>



using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;

namespace DRT
{
    /// <summary>
    /// Simplifies reflection for use with 'white box' testing, and Asserts for
    /// unrestricted reflection permission on reflection calls.  As such it can
    /// not be used to validated reflection demands.    /// </summary>
    public static class ReflectionHelper
    {
        #region Public Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Invoke a non public constructor using the type and arguments given.
        /// </summary>
        /// <param name="type">Type to construct.</param>
        /// <param name="args">The arguments matching the constructor's
        /// signature.</param>
        /// <returns>An instance of the type.</returns>
        public static object CreateInstance(Type type, object[] args)
        {
            object result = type.InvokeMember(
                ".ctor",
                BindingFlags.CreateInstance
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic,
                null,
                null,
                args);
            return result;
        }

        /// <summary>
        /// Will get a non public property for the specified object.
        /// </summary>
        /// <param name="instance">The object.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value.</returns>
        public static object GetProperty(object instance, string name)
        {
            Type type = instance.GetType();
            object result = type.InvokeMember(
                name,
                BindingFlags.GetProperty
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic,
                null,
                instance,
                new object[0]);
            return result;
        }

        /// <summary>
        /// Will get a non public property for the type.
        /// </summary>
        /// <remarks>
        /// Note that static type instances are per AppDomain, thus static state
        /// may vary from one domain to the other.
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property value.</returns>
        public static object GetProperty(Type type, string name)
        {
            object result = type.InvokeMember(
                name,
                BindingFlags.GetProperty
                    | BindingFlags.Static
                    | BindingFlags.NonPublic,
                null,
                null,
                new object[0]);
            return result;
        }

        /// <summary>
        /// Will get a non public type.
        /// </summary>
        /// <param name="fullName">The full name of the type.</param>
        /// <param name="assembly">The assembly where the type can be found.
        /// </param>
        /// <returns>The non public type.</returns>
        public static Type GetType(string fullName, Assembly assembly)
        {
            Type result = null;
            foreach (Type t in assembly.GetTypes())
            {
                if (t.FullName.Equals(fullName))
                {
                    result = t;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Create a generic List where T is the proved type.
        /// </summary>
        /// <param name="element">The T value for the List.</param>
        /// <param name="values">The value array used to fill the list.</param>
        /// <param name="converter">If not null, this Converter will be used to
        /// convert the value to type T before adding it to the list.</param>
        /// <returns>A generic list of the specified type.</returns>
        public static IList MakeAndFillGenericList(
            Type element,
            object[] values,
            Converter<object, object> converter)
        {
            IList list;

            Type genericList = GetType(
                "System.Collections.Generic.List`1", typeof(string).Assembly);

            Type madeList = genericList.MakeGenericType(element);

            // create an isstance
            list = (IList)madeList.InvokeMember(
                ".ctor",
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.CreateInstance,
                null,
                null,
                new object[1] { values.Length });

            // fill list
            foreach (object v in values)
            {
                // use the converter if present
                list.Add(converter == null ? v : converter(v));
            }

            return list;
        }

        /// <summary>
        /// Invokes the specified non public method with matching arguments on
        /// the object provided.
        /// </summary>
        /// <param name="instance">The object.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="args">The arguments for the method.</param>
        /// <returns>The return value of the method.</returns>
        public static object InvokeMethod(object instance, string name, object[] args)
        {
            object result;
            Type type = instance.GetType();
            result = type.InvokeMember(
                name,
                BindingFlags.InvokeMethod
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic,
                null,
                instance,
                args);
            return result;
        }

        /// <summary>
        /// Invokes the specified non public static method with matching
        /// arguments on the type provided.
        /// </summary>
        /// <remarks>
        /// Note that static type instances are per AppDomain, thus static state
        /// may vary from one domain to the other.
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="args">The arguments for the method.</param>
        /// <returns>The return value of the method.</returns>
        public static object InvokeMethod(Type type, string name, object[] args)
        {
            object result = type.InvokeMember(
                name,
                BindingFlags.InvokeMethod
                    | BindingFlags.Static
                    | BindingFlags.NonPublic,
                null,
                null,
                args);
            return result;
        }

        /// <summary>
        /// Will set a non public static field on the specified type.
        /// </summary>
        /// <remarks>
        /// Note that static type instances are per AppDomain, thus static state
        /// may vary from one domain to the other.
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The new value for the field.</param>
        public static void SetField(Type type, string name, object value)
        {
            type.InvokeMember(
                name,
                BindingFlags.SetField
                    | BindingFlags.Static
                    | BindingFlags.NonPublic,
                null,
                null,
                new object[] { value });
        }
        #endregion Public Methods
    }

    /// <summary>
    /// Provides convient type and instance properties for proxies that use
    /// reflection.
    /// </summary>
    public abstract class ReflectionProxy : MarshalByRefObject
    {
        #region Protected Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Sets the reflected instance of this proxy.
        /// </summary>
        /// <param name="instance"></param>
        protected void BindInstance(object instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// Sets the reflected type for this proxy.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type.</param>
        /// <param name="assembly">The assembly which contains the type.</param>
        protected static void BindProxy(string fullTypeName, Assembly assembly)
        {
            BindProxy(ReflectionHelper.GetType(fullTypeName, assembly));
        }

        /// <summary>
        /// Sets the reflected type for this proxy.
        /// </summary>
        /// <param name="type">The reflected type.</param>
        protected static void BindProxy(Type type)
        {
            _reflectedType = type;
        }
        #endregion Protected Methods

        #region Protected Properties
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Will return the current reflected instance.
        /// 
        /// Will throw if there is none.
        /// </summary>
        protected object Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(
                        "Must call BindInstance before using this instance.");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Will return the current reflected type.
        /// </summary>
        protected static Type ReflectedType
        {
            get
            {
                if (_reflectedType == null)
                {
                    throw new InvalidOperationException(
                        "Must call BindProxy before using this class.");
                }
                return _reflectedType;
            }
        }
        #endregion Protected Properties

        #region Private Fields
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        private object _instance;
        private static Type _reflectedType;
        #endregion Private Fields
    }
}
