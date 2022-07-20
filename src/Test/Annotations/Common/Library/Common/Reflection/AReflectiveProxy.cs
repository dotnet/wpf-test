// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;  // ReflectionPermission.
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{
    /// <summary>
    /// Base class for all Proxy implementations.  
    /// Provides methods for routing instance and static method calls to a delegate class.
    /// </summary>
    public abstract partial class AReflectiveProxy : DependencyObject
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// For internal use only.
        /// 
        /// Constructor for when we want to create a proxy for an existing object.
        /// </summary>
        protected AReflectiveProxy(object delegateInstance)
        {
            delegateObject = delegateInstance;
            delegateTypeValue = delegateObject.GetType();
            delegateAssembly = delegateObject.GetType().Assembly;
        }

        /// <summary>
        /// For internal use only.
        /// 
        /// Constructor for when we want to create a proxy for an existing object and provide an assembly
        /// reference other then the assembly of this object.
        /// </summary>
        protected AReflectiveProxy(object delegateInstance, Assembly assembly)
        {
            delegateObject = delegateInstance;
            delegateTypeValue = delegateObject.GetType();
            delegateAssembly = assembly;
        }

        /// <summary>
        /// Create a proxy on a new instance of the given class.  Instantiate this class
        /// using a constructor with the given parameters.
        /// </summary>
        /// <param name="classname">Classname to create proxy for.</param>
        /// <param name="parameterTypes">Types of parameters of the constructor to instantiate classname with. Null if no parameters.</param>
        /// <param name="parameterValues">Parameters to pass to classname's constructor, null if no parameters.</param>
        public AReflectiveProxy(Type[] parameterTypes, object[] parameterValues)
        {
            if (parameterTypes == null)
                parameterTypes = Type.EmptyTypes;
            if (parameterValues == null)
                parameterValues = new object[0];

            Initialize(DelegateClassName(), parameterTypes, parameterValues);
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Invoke a non-public static method of this delegate.
        /// </summary>
        /// <param name="methodName">Name of non-public method to invoke.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of methodName.</returns>
        public static object InvokeStaticDelegateMethod(string methodName, Type type, object[] parameters)
        {
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return RouteStatic(method, parameters, type.Assembly);
        }

        /// <summary>
        /// Invoke a non-public instance method of this delegate.
        /// </summary>
        /// <param name="methodName">Name of non-public method to invoke.</param>
        /// <param name="parameters">Parameters to pass to method.</param>
        /// <returns>Return value of methodName.</returns>
        public object InvokeDelegateMethod(string methodName, object[] parameters)
        {
            MethodInfo method = delegateTypeValue.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return RouteInstance(method, parameters);
        }

        public object GetField(string fieldName)
        {
            object returnValue = ReflectionHelper.GetField(delegateObject, fieldName);
            return ProxyTypeConverter.Convert(delegateAssembly, returnValue, ConversionType.Wrap);
        }

        public void SetField(string fieldName, object value)
        {
            object convertedValue = ProxyTypeConverter.Convert(delegateAssembly, value, ConversionType.Unwrap);
            ReflectionHelper.SetField(delegateObject, fieldName, convertedValue);
        }

        /// <summary>
        /// Route the call to this Instance method to this proxy's delegate.
        /// </summary>
        /// <param name="method">MethodBase of calling method (e.g. MethodBase.GetCurrentMethod()).</param>
        /// <param name="parameters">Parameters for method.</param>
        /// <returns>Return value of method or null if no return value.</returns>
        public object RouteInstance(MethodBase method, object[] parameters)
        {
            // By default, route method calls to Proxy's delegate (e.g. Proxy -> Delegate).
            object caller = delegateObject;
            ConversionType parameterConversion = ConversionType.Unwrap;
            ConversionType returnConversion = ConversionType.Wrap;

            // Convert parameter values and types.
            Type[] convertedParameterTypes = ProxyTypeConverter.ConvertParameterTypes(delegateAssembly, method, parameterConversion);
            object[] convertedParamters = ArrayModule.Convert(delegateAssembly, parameters, parameterConversion);

            // Invoke method.
            object returnValue = ReflectionHelper.InvokeMethod(
                                      caller,
                                      method.Name,
                                      convertedParameterTypes,
                                      convertedParamters);

            // Convert return value (opposite conversion relative to parameters).
            if (returnValue != null)
                returnValue = ProxyTypeConverter.Convert(delegateAssembly, returnValue, returnConversion);

            // If the method has ref parameters or out parameters we need to convert the parameters array back
            // to the world it came from and put these values back in the original input array.
            if (parameters != null && parameters.Length > 0)
            {
                object[] outparameters = ArrayModule.Convert(delegateAssembly, convertedParamters, returnConversion);
                Array.Copy(outparameters, 0, parameters, 0, parameters.Length);
            }

            return returnValue;
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <returns>Fully qualified type name of delegate class.</returns>
        abstract public string DelegateClassName();

        /// <summary>
        /// By definition proxies live in a different assembly than their delegates.  Therefore
        /// we need to know the name of the assembly in order to reflectively load the delegate Type.
        /// </summary>
        /// <returns>
        /// Name of assembly that delegate Type is defined in.
        /// (e.g. "PresentationFramework, Version=6.0.*.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")   
        /// </returns>
        /// <remarks>
        /// This design will not work for Side-by-Side versioning!
        /// </remarks>
        abstract protected string DelegateAssemblyName();

        /// <summary>
        /// Route the call to this Static method to this proxy's delegate.
        /// </summary>
        /// <param name="method">MethodBase of calling method (e.g. MethodBase.GetCurrentMethod()).</param>
        /// <param name="parameters">Parameters for method.</param>
        /// <returns>Return value of method or null if no return value.</returns>
        /// <exception cref="AnnotationProxyException">If 'method' is not static.</exception>
        protected static object RouteStatic(MethodBase method, object[] parameters, Assembly assembly)
        {
            if (!method.IsStatic)
                throw new AnnotationProxyException("'RouteStatic' was called on a Instance method '" + method.Name + "', use 'RouteInstance'.");

            object[] unwrappedParameters = ArrayModule.Convert(assembly, parameters, ConversionType.Unwrap);
            Type[] parameterTypes = ProxyTypeConverter.ConvertParameterTypes(assembly, method, ConversionType.Unwrap);
            Type staticDelegateType = ProxyTypeConverter.ProxyToDelegateType(assembly, method.DeclaringType);

            object returnValue = ReflectionHelper.InvokeStaticMethod(staticDelegateType,
                                          method.Name,
                                          parameterTypes,
                                          unwrappedParameters);
            if (returnValue != null)
                returnValue = ProxyTypeConverter.Convert(assembly, returnValue, ConversionType.Wrap);

            return returnValue;
        }

        /// <summary>
        /// Route the call to this Static method to this proxy's delegate.
        /// </summary>
        /// <param name="method">MethodBase of calling method (e.g. MethodBase.GetCurrentMethod()).</param>
        /// <param name="parameters">Parameters for method.</param>
        /// <returns>Return value of method or null if no return value.</returns>
        /// <exception cref="AnnotationProxyException">If 'method' is not static.</exception>
        protected static object RouteStatic(MethodBase method, object[] parameters, string assemblyName)
        {
            return RouteStatic(method, parameters, LoadAssembly(assemblyName));
        }


        /// <summary>
        /// Route a call to and event method (e.g. "add_X" or "remove_X").
        /// 
        /// Converts to given delegate into a compatible object that can then be reflectively passed to
        /// the delegateObject.
        /// </summary>
        /// <param name="method">Invoking method e.g. MethodBase.GetCurrentMethod()), used to determine whether
        /// this is an "add_" or "remove_".</param>
        /// <param name="handler">'value' passed into the event method.</param>
        protected void RouteEventMethod(MethodBase method, MulticastDelegate handler)
        {
            Match match = new Regex("(add_|remove_)(.*)").Match(method.Name);
            EventInfo eventInfo = delegateTypeValue.GetEvent(match.Groups[2].Value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MulticastDelegate convertedHandler = ConvertEventHandler(handler, eventInfo.EventHandlerType);

            MethodInfo toInvoke;
            switch (match.Groups[1].Value)
            {
                case "add_":
                    toInvoke = eventInfo.GetAddMethod(true);
                    break;
                case "remove_":
                    toInvoke = eventInfo.GetRemoveMethod(true);
                    break;
                default:
                    toInvoke = null;
                    break;
            }
            toInvoke.Invoke(delegateObject, new object[] { convertedHandler });
        }

        #endregion Protected Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Protected Properties

        public object Delegate
        {
            get
            {
                return delegateObject;
            }
        }

        public Type DelegateType
        {
            get
            {
                return delegateTypeValue;
            }
        }

        public Assembly Assembly
        {
            get
            {
                return delegateAssembly;
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Common constructor operations.  
        /// Load the assembly which the delegate claims to be defined within.  Then load the Type specified
        /// by 'classname', then instantiate this type using the constructor that matches the given parameter
        /// types.  Finally try and reflectivly initialize all public fields.
        /// </summary>
        private void Initialize(string classname, Type[] parameterTypes, object[] parameterValues)
        {
            InitializeDelegateAssemblyName();

            Type[] convertedParametersTypes = ProxyTypeConverter.ProxyToDelegateType(delegateAssembly, parameterTypes);
            object[] convertedParameters = ArrayModule.UnwrapArray(delegateAssembly, parameterValues);

            Type proxyType = this.GetType();
            delegateTypeValue = ProxyTypeConverter.ProxyToDelegateType(delegateAssembly, proxyType);
            if (proxyType.Equals(delegateTypeValue))
            {
                delegateTypeValue = delegateAssembly.GetType(classname);
                if (delegateTypeValue == null)
                    throw new AnnotationProxyException("Could not find delegate for proxy '" + proxyType.Name + "' in assembly '" + delegateAssembly.FullName + "'.");
            }
            delegateObject = ReflectionHelper.GetInstance(delegateTypeValue, convertedParametersTypes, convertedParameters);
        }

        private void InitializeDelegateAssemblyName()
        {
            // Default to "EntryAssembly".
            delegateAssembly = Assembly.GetEntryAssembly();

            string assemblyName = DelegateAssemblyName();
            if (!string.IsNullOrEmpty(assemblyName))
            {
                delegateAssembly = LoadAssembly(assemblyName);
            }

            if (delegateAssembly == null)
                throw new AnnotationProxyException("Could not load assembly '" + assemblyName + "'.");
        }

        /// <summary>
        /// Load an assembly with the given name.  Name can be fully qualified or partial name.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static Assembly LoadAssembly(string assemblyName)
        {
            Assembly asm = null;

            if (!assemblyName.Equals("PresentationFramework"))
                throw new NotSupportedException("DelegateAssemblyName() use is disabled until design can be modified to support SxS versions of dlls.");
            asm = typeof(FrameworkElement).Assembly;

            return asm;
        }

        /// <summary>
        /// Assign each public field in the Proxy class to its corresponding field in the
        /// delegate class.
        /// </summary>
        protected static void InitializeStaticFields(string assemblyName, Type myType)
        {
            Assembly assembly = LoadAssembly(assemblyName);

            //Type myType = MethodBase.GetCurrentMethod().DeclaringType;
            Type delegateType = ProxyTypeConverter.ProxyToDelegateType(assembly, myType);

            FieldInfo[] proxyStaticFields = myType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo proxyField in proxyStaticFields)
            {
                FieldInfo delegateField = delegateType.GetField(proxyField.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (delegateField != null)
                {
                    object fieldValue = delegateField.GetValue(null);
                    fieldValue = ProxyTypeConverter.WrapObject(assembly, fieldValue);
                    proxyField.SetValue(null, fieldValue);
                }
            }
        }

        /// <summary>
        /// Convert the given EventHandler to handler of the given Type.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="newType">Type of a subclass of EventHandler.</param>
        /// <returns></returns>
        private MulticastDelegate ConvertEventHandler(MulticastDelegate handler, Type newType)
        {
            if (!newType.IsSubclassOf(typeof(MulticastDelegate)))
                throw new AnnotationProxyException("'ConvertEventHandler' was given a non-MulticastDelegate type '" + newType.ToString() + "'.");

            ConstructorInfo[] constructors = newType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return (MulticastDelegate)constructors[0].Invoke(new object[] { handler.Target, handler.Method.MethodHandle.GetFunctionPointer() });
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Protected Fields
        //
        //------------------------------------------------------

        #region Protected Fields

        /// <summary>
        /// Type of object we are a proxy for.
        /// </summary>
        protected Type delegateTypeValue;

        /// <summary>
        /// Object which we are a proxy for.
        /// </summary>
        protected object delegateObject;

        /// <summary>
        /// Assembly that delegateType is defined in.
        /// </summary>
        protected Assembly delegateAssembly;

        #endregion Protected Fields
    }
}


