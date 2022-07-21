// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module with static methods to assist with Reflective operations.

using System;
using System.Windows;
using System.Reflection;
using System.Security.Permissions;	// ReflectionPermission.

namespace Annotations.Test.Reflection
{
	public class ReflectionHelper 
	{
		#region Public Methods

		/// <summary>
		/// Throws an Exception if there are not sufficient permissions to execute all the
		/// methods in this class.  Should be called on startup of any app that uses this class.
		/// </summary>
		public static void EnsurePermissions()
		{
			ReflectionPermission permission = new ReflectionPermission(PermissionState.Unrestricted);
			permission.Demand();
		}

		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static Type GetType(string classname)
		{
			return Type.GetType(classname);
		}

		/// <summary>
		/// Invoke the constructor for the given classname that takes the following parameters.
		/// Uses default assembly to load 'classname'.
		/// </summary>
		/// <param name="classname">Full name of class to create (e.g. System.Windows.Annotations.AnnotationService).</param>
		/// <param name="parameterTypes">Array of parameter Types to uniquely identify constructor signature.
		/// If no parameters pass 'Type.EmptyTypes'.</param>
		/// <param name="parameters">Array of parameter values to invoke the constructor with.
		/// If no parameters pass 'Object[0]'.</param>
		/// <returns>New instance of type 'classname'.</returns>
		/// <exception cref="ArgumentException">If parameterTypes and parameters are not the same length or
		/// if no constructor is found.</exception>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object GetInstance(string classname, Type [] parameterTypes, object [] parameters)
		{
			return GetInstance(GetType(classname), parameterTypes, parameters);
		}

		/// <summary>
		/// Invoke the constructor for the given Type that takes the following parameters.
		/// </summary>
		/// <param name="type">Type of object to create an instance of.</param>
		/// <param name="parameterTypes">Array of parameter Types to uniquely identify constructor signature.
		/// If no parameters pass 'Type.EmptyTypes'.</param>
		/// <param name="parameters">Array of parameter values to invoke the constructor with.
		/// If no parameters pass 'Object[0]'.</param>
		/// <returns>New instance of type 'classname'.</returns>
		/// <exception cref="ArgumentException">If parameterTypes and parameters are not the same length or
		/// if no constructor is found.</exception>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object GetInstance(Type type, Type[] parameterTypes, object[] parameters)
		{
			if (parameterTypes.Length != parameters.Length)
				throw new AnnotationProxyException("Number of parameter values must match number of parameter types.");

			ConstructorInfo constructor = FindConstructor(type, parameterTypes, parameters);

			if (constructor == null)
				throw new AnnotationProxyException("No public of private constructor found for given parameters.");

			try 
			{
				return constructor.Invoke(parameters);
			}
			catch (TargetInvocationException e)
			{
				// This is an exception thrown by the invoked method.  Make it look like
				// method was not invoked through reflection.
				ProcessException(e);
			}
			return null;
		}

		/// <summary>
		/// Return ConstructorInfo for a constructor with the given parameters of ANY protection level.
		/// (Type.GetConstructor(Type []) only returns public constructors.
		/// </summary>
		/// <returns>ConstructorInfo of constructor with these precise parameter types or null.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		private static ConstructorInfo FindConstructor(Type type, Type [] parameterTypes, object [] parameters)
		{
			ConstructorInfo constructor = null;
			constructor = type.GetConstructor(
												INSTANCE_PERMISSION,
												null,
												CallingConventions.Any,
												parameterTypes,
												null);
			return constructor;
		}

		/// <summary>
		/// Invoke default constructor for 'classname'.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object GetInstance(string classname)
		{
			return GetInstance(classname, Type.EmptyTypes, new Object[0]);
		}

		/// <summary>
		/// Invoke Static method with given name on the given object.
		/// </summary>
		/// <param name="context">Object to invoke method on.</param>
		/// <param name="methodName">Name of method to invoke.</param>param>
		/// <param name="parameterTypes">Types of parameters to pass to method.</param>
		/// <param name="parameters">Parameter values to pass to method. Can be null if no parameters.</param>
		/// <returns>Return value of method.</returns>
		/// <exception cref="ArgumentException">If method is not found.</exception>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object InvokeStaticMethod(Type contextType, string methodName, Type[] parameterTypes, object[] parameters)
		{
			MethodInfo methodHandle = contextType.GetMethod(methodName,
																STATIC_PERMISSION,
																null /* default binder */,
																parameterTypes,
																null /* not used by default binder */);

			if (methodHandle == null)
				throw new ArgumentException("No static method '" + methodName + "' found in class '" + contextType.ToString() + "'.");

			try
			{
				if (parameters == null)
					parameters = new object[0];

				return methodHandle.Invoke(null, parameters);
			}
			catch (TargetInvocationException e)
			{				
				ProcessException(e);
			}
			return null;
		}

		/// <summary>
		/// This is an exception thrown by the invoked method.  Make it look like
		/// method was not invoked through reflection.
		/// 
		/// Store the original stack trace in the new exceptions Data property using "ProxyStackTrace" as the key.
		/// </summary>
		/// <param name="e"></param>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		private static void ProcessException(TargetInvocationException e)
		{
			Exception inner = e.InnerException;
			inner.Data.Add("ProxyStackTrace", e.GetBaseException().StackTrace);
			throw inner;
		}

		/// <summary>
		/// Invoke an Static method that takes no parameters.
		/// </summary>
		/// <param name="context">Object to invoke method on.</param>
		/// <param name="methodName">Name of method to invoke.</param>
		/// <returns>Return value of method 'methodName'.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object InvokeMethod(Type contextType, string methodName)
		{
			return InvokeStaticMethod(contextType, methodName, Type.EmptyTypes, null);
		}

		/// <summary>
		/// Invoke Instance method with given name on the given object, try and infer the parameter types based on the
		/// objects.
		/// </summary>
		/// <param name="context">Object to invoke method on.</param>
		/// <param name="methodName">Name of method to invoke.</param>param>
		/// <param name="parameters">Parameter values to pass to method. Can be null if no parameters.</param>
		/// <returns>Return value of method.</returns>
		/// <exception cref="ArgumentException">If method is not found.</exception>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object InvokeMethod(object context, string methodName, object[] parameters)
		{
			MethodInfo method = FindMethod(context.GetType(), methodName, ArgsToTypes(parameters));
			if (method == null)
				throw new ArgumentException("Could not find method '" + methodName + "' in class '" + context.GetType().FullName + "'.");
			return method.Invoke(context, parameters);
		}

		/// <summary>
		/// Convert args to there types.  If arg is null then leave the type null (FindMethod will accept null types and
		/// try and deduce the intended method).
		/// </summary>
		/// <param name="args">Arguments to convert.</param>
		/// <returns>Types corresponding to the given arguments.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static Type[] ArgsToTypes(Object[] args)
		{
			Type[] argTypes = new Type[0];
			if (args != null)
			{
				argTypes = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					argTypes[i] = ((args[i] == null) ? null : args[i].GetType());
				}
			}
			return argTypes;
		}

		/// <summary>
		/// Find method with the given name given its arg types.  
		/// </summary>
		/// <remarks>Differs from GetMethod because some of the argTypes can be null and it
		/// will try and infer what the correct method is.</remarks>
		/// <param name="context">Class to search for method on.</param>
		/// <param name="methodName">Name of method.</param>
		/// <param name="argTypes">Fully or partially populated array of parameter types.</param>
		/// <returns>MethodInfo of method or null if method could not be found.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static MethodInfo FindMethod(Type context, string methodName, Type[] argTypes)
		{
			BindingFlags permission = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			MethodInfo method = null;
			try
			{
				method = FindMethodInHierarchy(context, methodName, argTypes);
			}
			catch (Exception)
			{
				// ignore.
			}

			/*
			 * If someone tried to call a method and pass null, then we cannot determine the full list of
			 * argument types.  Therefore, we want to iterate through all methods with the same name, and
			 * try and pick the correct method based on the argument types we do have.
			 */
			if (method == null)
			{
				MethodInfo[] methodCandidates = context.GetMethods(permission);
				foreach (MethodInfo candidate in methodCandidates)
				{
					if (candidate.Name.Equals(methodName))
					{
						ParameterInfo[] parameters = candidate.GetParameters();
						if (parameters.Length == argTypes.Length)
						{
							bool found = true;
							for (int i = 0; i < argTypes.Length; i++)
							{
								if (argTypes[i] != null)
								{
									if (!argTypes[i].Equals(parameters[i].ParameterType) && !argTypes[i].IsSubclassOf(parameters[i].ParameterType))
									{
										found = false;
										break;
									}
								}
							}

							if (found)
								return candidate;
						}
					}
				}
			}

			return method;
		}

		/// <summary>
		/// Invoke Instance method with given name on the given object.
		/// </summary>
		/// <param name="context">Object to invoke method on.</param>
		/// <param name="methodName">Name of method to invoke.</param>param>
		/// <param name="parameterTypes">Types of parameters to pass to method.</param>
		/// <param name="parameters">Parameter values to pass to method. Can be null if no parameters.</param>
		/// <returns>Return value of method.</returns>
		/// <exception cref="ArgumentException">If method is not found.</exception>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object InvokeMethod(object context, string methodName, Type[] parameterTypes, object[] parameters)
		{
			MethodInfo methodHandle = FindMethodInHierarchy(context.GetType(), methodName, parameterTypes);

			if (methodHandle == null)
				throw new ArgumentException("No instance method '" + methodName + "' found in class '" + context.GetType().ToString() + "'.");
			
			try
			{
				if (parameters == null)
					parameters = new object[0];

				return methodHandle.Invoke(context, parameters);
			}
			catch (TargetInvocationException e)
			{
				ProcessException(e);
			}
			return null;
		}

		/// <summary>
		/// Invoke an Instance method that takes no parameters.
		/// </summary>
		/// <param name="context">Object to invoke method on.</param>
		/// <param name="methodName">Name of method to invoke.</param>
		/// <returns>Return value of method 'methodName'.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object InvokeMethod(object context, string methodName)
		{
			return InvokeMethod(context, methodName, Type.EmptyTypes, null);
		}

		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object GetField(object context, string fieldName)
		{
			return FindFieldInHierarchy(context.GetType(), fieldName).GetValue(context);
		}

		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static void SetField(object context, string fieldName, object value)
		{
			FindFieldInHierarchy(context.GetType(), fieldName).SetValue(context, value);
		}

		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static object GetProperty(object context, string propertyName)
		{
			return FindPropertyInHierarchy(context.GetType(), propertyName).GetValue(context, null);
		}

		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static void SetProperty(object context, string propertyName, object value)
		{
			FindPropertyInHierarchy(context.GetType(), propertyName).SetValue(context, value, null);
		}

		/// <summary>
		/// Recursively walk the hierarchy for this type to find a member with the given name.
		/// </summary>
		/// <returns>Member with the given name.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static MemberInfo FindMemberInHierarchy(Type targetType, string memberName)
		{
			MemberInfo result = null;

			if (targetType != null)
			{

				MemberInfo[] results = targetType.GetMember(memberName, FULL_PERMISSION);
				if (results.Length > 1)
					throw new Exception("Found multiple members with the name '" + memberName + "' for Type '" + targetType.FullName + "'.");
				if (results.Length == 1)
					result = results[0];

				// Search Interface definitions.
				if (result == null)
				{
					Type[] interfaces = targetType.GetInterfaces();
					foreach (Type current in interfaces)
					{
						result = FindMemberInHierarchy(current, memberName);
						if (result != null)
							break;
					}
				}

				// Search Base Classes.
				if (result == null)
				{
					result = FindMemberInHierarchy(targetType.BaseType, memberName);
				}
			}
			return result;
		}

		/// <summary>
		/// Recursively walk the hierarch for this type to find a property with the given name
		/// </summary>		
		/// <returns>Property with given name.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static PropertyInfo FindPropertyInHierarchy(Type targetType, string name)
		{
			PropertyInfo property = (PropertyInfo)FindMemberInHierarchy(targetType, name);
			if (property == null)
				throw new Exception("Could not find property with name '" + name + "' for Type '" + targetType.FullName + "'.");
			return property;
		}

		/// <summary>
		/// Recursively walk the hierarch for this type to find a field with the given name
		/// </summary>		
		/// <returns>Field with given name.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static FieldInfo FindFieldInHierarchy(Type targetType, string name)
		{
			FieldInfo member = (FieldInfo)FindMemberInHierarchy(targetType, name);
			if (member == null)
				throw new Exception("Could not find field with name '" + name + "' for Type '" + targetType.FullName + "'.");
			return member;
		}

		/// <summary>
		/// Recursively walk the hierarch for this type to find a Method with the given name
		/// </summary>		
		/// <returns>Method with given name.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public static MethodInfo FindMethodInHierarchy(Type targetType, string name, Type [] parameterTypes)
		{
			MethodInfo result = null;

			if (targetType != null)
			{

				result = targetType.GetMethod(name, FULL_PERMISSION, null, parameterTypes, null);			

				// Search Interface definitions.
				if (result == null)
				{
					Type[] interfaces = targetType.GetInterfaces();
					foreach (Type current in interfaces)
					{
						result = FindMethodInHierarchy(current, name, parameterTypes);
						if (result != null)
							break;
					}
				}

				// Search Base Classes.
				if (result == null)
				{
					result = FindMethodInHierarchy(targetType.BaseType, name, parameterTypes);
				}
			}
			return result;
		}

		#endregion Public Methods

		#region Private Methods

		private static MemberInfo GetMember(MemberTypes memberType, object context, string name)
		{
			MemberInfo[] members = context.GetType().GetMember(name, FULL_PERMISSION);
			for (int i = 0; i < members.Length; i++)
			{
				if (members[i].MemberType == memberType)
					return members[i];
			}

			throw new ArgumentException("No public member '" + name + "' was found in class '" + context.GetType().ToString() + "'.");
		}

		#endregion Private Methods

		public static BindingFlags FULL_PERMISSION = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		public static BindingFlags INSTANCE_PERMISSION = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		public static BindingFlags STATIC_PERMISSION = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
	}
}


