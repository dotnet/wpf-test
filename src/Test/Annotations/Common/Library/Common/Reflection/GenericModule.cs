// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;	// ReflectionPermission.
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{
	/// <summary>
	/// Methods for performing conversions on collections in the System.Collections.Generic namespace.
	/// 
	/// Format of Generic objects:
	/// (GENERIC_TYPE)'(NUM_PARAMETERS)(+possible-attribute)[[TYPENAME, ASSEMBLY_NAME],...]
	/// you can convert between generic types by swapping the typname and assembly names and then
	/// using Type.GetType(string typename).
	/// </summary>
	public class GenericModule
	{
		#region Public Methods

		public static object Convert(Assembly assembly, object generic, ConversionType conversion)
		{
			object unwrappedGeneric = null;
			if (generic.GetType().GetInterface("IDictionary") != null)
				unwrappedGeneric = ConvertDictionary(assembly, generic, conversion);
			else if (generic.GetType().GetInterface("IList`1") != null)
				unwrappedGeneric = ConvertList(assembly, generic, conversion);
			else
				throw new AnnotationProxyException("Unknown generic type '" + generic.GetType().FullName + "'.");

			return unwrappedGeneric;
		}

		public static object UnwrapGeneric(Assembly assembly, object generic)
		{
			return Convert(assembly, generic, ConversionType.Unwrap);
		}

		public static object WrapGeneric(Assembly assembly, object generic)
		{
			return Convert(assembly, generic, ConversionType.Wrap);
		}

		/// <summary>
		/// Convert generic collection of proxy type to delegate type.
		/// (e.g. List<ProxyA> -> List<A>).
		/// </summary>
		public static Type ProxyToDelegate(Assembly assembly, Type generic)
		{
			return ConvertType(assembly, generic, ConversionType.Unwrap);
		}

		/// <summary>
		/// Convert generic collection of delegate type to proxy type.
		/// (e.g. List<A> -> List<ProxyA>).
		/// </summary>
		public static Type DelegateToProxy(Assembly assembly, Type generic)
		{
			return ConvertType(assembly, generic, ConversionType.Wrap);
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Try and get type first from calling assembly, then from the given assembly, and finally
		/// from the given type's assembly.
		/// </summary>
		private static Type GetType(Assembly assembly, Type generic, string typename)
		{
			Type type = Type.GetType(typename);
			if (type == null && assembly != null)
			{
				type = assembly.GetType(typename);
			}
			if (type == null)
			{
				type = generic.Assembly.GetType(typename);
			}
			return type;
		}

		/// <summary>
		/// Convert generic collection type between Proxy and Delegate.  
		/// If conversion is Wrap: delegate -> proxy.
		/// If conversion is Unwrap: proxy -> delegate
		/// 
		/// Generic description looks something like:
		/// "System.Collections.Generic.Dictionary`2[[Proxies.ReflectionTest.Internal.ClassA, ReflectionTest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[Proxies.ReflectionTest.Internal.SubNamespace.ClassC, ReflectionTest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]"
		/// </summary>
		private static Type ConvertType(Assembly assembly, Type generic, ConversionType conversion)
		{			
			Match match = new Regex("(.*?\\d)(.*?)(\\[)(\\[.*?\\])(\\].*)").Match(generic.FullName);
			if (!match.Success)
				throw new AnnotationProxyException("Could not convert generic proxy type: '" + generic.FullName + "'.");

			string genericName = match.Groups[1].Value;

			// If collection is read only, use a normal list instead 
			// PD7:		
			if (genericName.Equals("System.Collections.ObjectModel.ReadOnlyCollection`1"))
				genericName = "System.Collections.Generic.List`1";	
			// PD6:
			if (!match.Groups[2].Value.Contains("ReadOnly"))
				genericName += match.Groups[2].Value;

			string parameterDescription = match.Groups[4].Value;
			parameterDescription = ConvertParameterDescriptions(assembly, generic, conversion);

			string typename = genericName + "[" + parameterDescription + "]";
			return GetType(assembly, generic, typename);
		}

		/// <summary>
		/// Unwrap the contents of an implementor of System.Collections.Generic.IList.
		/// </summary>
		/// <param name="assembly">Assembly to load delegates from.</param>
		/// <param name="collection">IList to unwrap.</param>
		/// <returns>Unwrapped System.Collections.Generic.IList</returns>
		private static object ConvertList(Assembly assembly, object collection, ConversionType conversion)
		{
			Type[] parameterType = collection.GetType().GetGenericArguments();
			object[] contents = GetCollectionContents(collection, parameterType[0]);
			contents = ArrayModule.Convert(assembly, contents, conversion);

			Type convertedType;
			if (conversion == ConversionType.Wrap)
				convertedType = ProxyTypeConverter.DelegateToProxyType(parameterType[0]);
			else
				convertedType = ProxyTypeConverter.ProxyToDelegateType(assembly, parameterType[0]);

			Type convertedCollectionType = ConvertType(assembly, collection.GetType(), conversion);
			object convertedCollection = ReflectionHelper.GetInstance(convertedCollectionType, Type.EmptyTypes, new object[0]);

			for (int i = 0; i < contents.Length; i++)
			{
				// NOTE: some contents could be null therefore we need to use a fixed type when adding.
				ReflectionHelper.InvokeMethod(convertedCollection,
												"Add",
												new Type[] { convertedType },
												new object[] { contents[i] });
			}

			return convertedCollection;
		}

		/// <summary>
		/// Unwrap all the elements within a System.Collection.Generic.IDictionary.
		/// </summary>
		/// <returns>System.Collection.Generic.IDictionary composed of all non-proxy objects.</returns>
		private static object ConvertDictionary(Assembly assembly, object dictionary, ConversionType conversion)
		{
			Type[] types = dictionary.GetType().GetGenericArguments();

			object keyCollection = ReflectionHelper.GetProperty(dictionary, "Keys");
			object valuesCollection = ReflectionHelper.GetProperty(dictionary, "Values");

			object[] keys = GetCollectionContents(keyCollection, types[0]);
			object[] values = GetCollectionContents(valuesCollection, types[1]);

			keys = ArrayModule.Convert(assembly, keys, conversion);
			values = ArrayModule.Convert(assembly, values, conversion);

			Type convertedDictionaryType = ConvertType(assembly, dictionary.GetType(), conversion);
			object convertedDictionary = ReflectionHelper.GetInstance(convertedDictionaryType, Type.EmptyTypes, new object[0]);

			for (int i = 0; i < keys.Length; i++)
			{
				ReflectionHelper.InvokeMethod(convertedDictionary,
												"Add",
												new Type[] { keys[i].GetType(), values[i].GetType() },
												new object[] { keys[i], values[i] });
			}

			return convertedDictionary;
		}

		/// <summary>
		/// Parameter description looks like this:
		/// [TYPENAME, ReflectionTest, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[...],...
		/// this method converts TYPENAME from proxy to delegate and returns the full description.
		/// 
		/// if ConversionType.Wrap then assembly parameter is ignored.
		/// </summary>
		private static string ConvertParameterDescriptions(Assembly assembly, Type generic, ConversionType conversion)
		{
			string converted = "";
			Type[] parameterTypes = generic.GetGenericArguments();
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				Type type = parameterTypes[i];
				Type convertedType;
				if (conversion == ConversionType.Wrap)
					convertedType = ProxyTypeConverter.DelegateToProxyType(type);
				else
					convertedType = ProxyTypeConverter.ProxyToDelegateType(assembly, type);
				converted += "[" + convertedType.FullName + ", " + convertedType.Assembly.FullName + "]";
				
				if (i < parameterTypes.Length-1)
					converted += ",";
			}
			return converted;
		}

		/// <summary>
		/// Given a Generic collection, return the contents as an array.
		/// </summary>
		/// <param name="collection">Single dimension generic collection (e.g. List, KeyCollection etc)</param>
		/// <param name="collectionType">Type of generic collection (e.g. List<int> type == int.</param>
		/// <returns>Contents of generic collection in an array form.</returns>
		private static object[] GetCollectionContents(object collection, Type collectionType)
		{
			int collectionSize = (int)ReflectionHelper.GetProperty(collection, "Count");
			Type arrayType = Assembly.GetAssembly(collectionType).GetType(collectionType.FullName + "[]");
			object[] contents = (object[])ReflectionHelper.GetInstance(arrayType,
																		new Type[] { typeof(int) },
																		new object[] { collectionSize });
			ReflectionHelper.InvokeMethod(collection,
											"CopyTo",
											new Type[] { contents.GetType(), typeof(int) },
											new object[] { contents, 0 });
			return contents;
		}

		#endregion Private Methods
	}
}


