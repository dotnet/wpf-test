// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;

namespace CoreInteropLateBound
{
	/// <summary>
	/// Core office interop functionality
	/// </summary>
	public class CoreLateBound
	{		
		/// <summary>
		/// Creates an instance of an object from a prog id. Designed to work the same as CreateObject in VB
		/// </summary>		
		protected object CreateObject(string progId)
		{			
			try
			{
				return Activator.CreateInstance(Type.GetTypeFromProgID(progId));
			}
			catch {}
			throw new Exception("A failure occured when trying to create instance of '" + progId + "'");
		}		

		protected object GetProperty(object targetObject, string propertyName)
		{
			try
			{
				return targetObject.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, targetObject, null );
			}
			catch {}
			throw new Exception("A failure occured when setting property '" + propertyName + "' on " + targetObject.GetType().Name + ". ");
		}


		protected object SetProperty(object targetObject, string propertyName, object[] parameters)
		{
			try
			{
				return targetObject.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, targetObject, parameters);	
			}
			catch {}
			throw new Exception("A failure occured when setting property '" + propertyName + "'. With parameters: " + ObjectParamsToString(parameters));
		}

		protected object SetProperty(object targetObject, string propertyName, bool boolValue)
		{		
			return SetProperty(targetObject, propertyName, new object[] {boolValue});			
		}

		protected object InvokeMethod(object targetObject, string methodName, object[] parameters)
		{
			try
			{
				return targetObject.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, targetObject, parameters);
			}
			catch{}
			throw new Exception("A failure occured when invoking method '" + methodName + "'. With parameters: " + ObjectParamsToString(parameters));
		}

		private string ObjectParamsToString(object[] parameters)
		{
			string szParams = "{ ";
			if (parameters != null)
				for (int i = 0; i < parameters.Length; i++)
					szParams += (parameters[i] + ( (i < parameters.Length -1) ? ", " : ""));
			szParams += " }";

			return szParams;
		}		
	}
}
