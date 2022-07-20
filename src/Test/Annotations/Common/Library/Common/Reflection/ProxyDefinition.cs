// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Object containing all information needed to automatically 
//				 generate an AReflectiveProxy.

using System;
using System.Windows;
using System.Reflection;
using System.Security.Permissions;	// ReflectionPermission.
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{
	/// <summary>
	/// Description of all proxies for a single file.
	/// </summary>
	public class ProxyDefinition
	{
		//------------------------------------------------------
		//
		//  Constructors
		//
		//------------------------------------------------------

		#region Constructors

		/// <summary>
		/// Create a ProxyDefinition for this file, all public classes within this file will be 
		/// included in the definition.
		/// </summary>
		/// <param name="file">C# source file to parse.</param>
		public ProxyDefinition(FileInfo file)
		{
			Source = file;
			CreateDefinition();
		}

		/// <summary>
		/// Create a ProxyDefinition for this file.  Only include classes in the given list in
		/// the definition.
		/// </summary>
		/// <param name="file">C# source file to parse.</param>
		/// <param name="allowedClasses">List of fully qualified class names to include in definition.</param>
		public ProxyDefinition(FileInfo file, IList<string> allowedClasses)
		{
			Source = file;
			this.allowedClasses = allowedClasses;
			CreateDefinition();
		}

		/// <summary>
		/// Create a ProxyDefinition for this file and use the given assembly for loading.  
		/// Only include classes in the given list in the definition.
		///
		/// Use when the classes you are trying to build proxies for are not contained within the
		/// calling assembly but exist in an external dll.
		/// </summary>
		/// <param name="assemblyToLoadFrom">Assembly to load Types from.</param>
		/// <param name="file">C# source file to parse.</param>
		/// <param name="allowedClasses">List of fully qualified class names to include in definition.</param>
		public ProxyDefinition(Assembly assemblyToLoadFrom, FileInfo file, IList<string> allowedClasses)
		{
			Source = file;
			assembly = assemblyToLoadFrom;
			this.allowedClasses = allowedClasses;
			CreateDefinition();
		}


		#endregion Constructors

		//------------------------------------------------------
		//
		//  Public Methods
		//
		//------------------------------------------------------

		#region Public Methods

		public Type [] Interfaces(Type type)
		{
			return interfaces[type];
		}

		public void SetInterfaces(Type type, Type[] interfaces)
		{
			this.interfaces[type] = interfaces;
		}

		public string BaseClass(Type type)
		{
			return baseClass[type];
		}

		public void SetBaseClass(Type type, Type newBaseClass)
		{
			baseClass[type] = newBaseClass.Name;
		}

		public bool NonDefaultBaseClass(Type type)
		{
			return !baseClass[type].Equals(DEFAULT_BASECLASS);
		}

		public ConstructorInfo[] Constructors(Type type)
		{
			return classConstructors[type];
		}

		public MethodInfo[] PublicMethods(Type type)
		{
			return publicMethods[type];
		}

		public MethodInfo[] NonPublicMethods(Type type)
		{
			return nonPublicMethods[type];
		}

		public PropertyInfo[] Properties(Type type)
		{
			return allProperties[type];
		}

		public FieldInfo[] PublicFields(Type type)
		{
			return publicFields[type];
		}

		public EventInfo[] Events(Type type)
		{
			return events[type];
		}

		#endregion Public Methods

		//------------------------------------------------------
		//
		//  Public Properties
		//
		//------------------------------------------------------

		#region Public Properties

		/// <summary>
		/// ProxyDefinition describes an actual proxy if the file it parsed contained at
		/// least 1 interesting class.
		/// </summary>
		public bool NeedsProxy
		{
			get
			{
				return internalClasses.Count > 0;
			}
		}

		public FileInfo Source
		{
            get
            {
                return source;
            }
            set 
            { 
                source = value; 
            }
		}

		public IList<Type> ClassTypes
		{
			get
			{
				return internalClasses;
			}
		}

		public IList<string> Imports
		{
			get 
            { 
                return imports; 
            }
		}

		public string Namespace
		{
			get
			{
				return classNamespace;
			}
		}

		public IList<string> Delegates
		{
			get
			{
				return delegateDeclarations;
			}
		}

		#endregion Public Properties

		//------------------------------------------------------
		//
		//  Private Methods
		//
		//------------------------------------------------------

		#region Private Methods

		private void CreateDefinition()
		{
			FileStream input = Source.OpenRead();
			StreamReader reader = new StreamReader(input);

			// Match lines that start with '//', '*', '/*', or '*/'.
			Regex commentExpression = new Regex("(^\\s*//.*)|(^\\s*\\*.*)|(^\\s*/\\*.*)|(\\s*\\*/.*)");
			Regex importExpression = new Regex("using\\s(.+);");
			Regex namespaceExpression = new Regex("^\\s*namespace\\s(.+){*");
			Regex classDeclarationExpression = new Regex(".*?(class|enum|interface)\\s(\\S+?)(:|\\s|$).*");
			Regex delegateDefinitionExpression = new Regex(".*?delegate.*\\s(\\S*)\\(.*");

			Match match = null;

			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				if ((match = commentExpression.Match(line)).Success)
				{
					// Ignore comments.
				}
				else if ((match = importExpression.Match(line)).Success)
				{
					// Ignore imports. - AddImport(match.Groups[1].Value);
				}
				else if ((match = namespaceExpression.Match(line)).Success)
				{
					SetNamespace(match.Groups[1].Value);
				}
				else if ((match = classDeclarationExpression.Match(line)).Success)
				{
					AddClass(match.Groups[2].Value);
				}
				else if ((match = delegateDefinitionExpression.Match(line)).Success)
				{
					string classname = this.Namespace + "." + match.Groups[1].Value;
					Type delegateType = LoadType(classname);
					if (IsAllowed(delegateType))
					{
						string delegateDef = line;
						// Make all delegates public by default.
						if (delegateDef.Contains("internal"))
							delegateDef = delegateDef.Replace("internal", "public");
						delegateDeclarations.Add(delegateDef);
					}
				}
			}

			input.Close();
		}

		/// <summary>
		/// If an assembly was specified, try to load from it, if that fails try to load from
		/// calling assembly.
		/// </summary>
		/// <param name="classname">Fully qualified classname of Type to load.</param>
		/// <returns>Type corresponding to given classname</returns>
		/// <exception cref="AnnotationProxyException">If no Type can be found.</exception>
		private Type LoadType(string classname)
		{
			Type type = null;
			if (assembly != null)
				type = assembly.GetType(classname);

			if (type == null)
				type = Assembly.GetCallingAssembly().GetType(classname);

			if (type == null)
				//throw new AnnotationProxyException("Failed to load type '" + classNamespace + "." + classname + "'.");
				Console.WriteLine("WARNING: failed to load type '" + classname + "'.");

			return type;
		}

		private void AddImport(string importedNamespace)
		{
			if (importedNamespace != null && !Imports.Contains(importedNamespace))
				Imports.Add(importedNamespace);
		}

		private void SetNamespace(string definedNamespace)
		{
			if (classNamespace != null)
				throw new AnnotationProxyException("File contains duplicate namespaces.");
			classNamespace = definedNamespace;
		}

		private void AddClass(string classname)
		{
			if (classNamespace == null)
				throw new AnnotationProxyException("Invalid file format, class '" + classNamespace + "' declared outside of namespace.");

			Type type = LoadType(classNamespace + "." + classname);
			// Check for internal classes:
			if (type == null)
			{
				foreach (Type baseClass in internalClasses)
				{
					type = LoadType(classNamespace + "." + baseClass.Name + "+" + classname);
					if (type != null)
						break;
				}
			}
			// Verify that type is allowed:
			if (TypeRequiresProxy(type))
			{
				internalClasses.Add(type);
				baseClass.Add(type, "AReflectiveProxy");
				ParsePublicAPI(type);
			}
		}


		/// <summary>
		/// Determine if a proxy should be generated for the given type.
		/// </summary>
		/// <returns>Returns true only if type is in list of 'allowedClasses'.</returns>
		private bool TypeRequiresProxy(Type type)
		{
			bool result = false;
			if (type != null)
			{	
				if (allowedClasses != null)
					result = allowedClasses.Contains(type.FullName);				
			}
			return result;
		}

		private bool IsAllowed(Type type)
		{
			bool result = false;
			if (type != null)
			{
				// If it is a generic list then check the inner types:
				if (type.IsGenericType)
				{
                    if (type.IsPublic)
                    {
                        result = true;
                        Type[] genericArgs = type.GetGenericArguments();
                        foreach (Type innerType in genericArgs)
                            result &= IsAllowed(innerType);
                    }
				}
				else
				{
					// Ignore templated types.
					if (!string.IsNullOrEmpty(type.FullName) && !string.IsNullOrEmpty(type.Namespace))
					{
						if (allowedClasses != null)
							result = allowedClasses.Contains(type.FullName) || type.IsPublic;
						else
							result = type.IsPublic;
					}
				}
			}
			return result;
		}

		private void ParsePublicAPI(Type type)
		{
			BindingFlags basicFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
			BindingFlags PUBLIC_MEMBERS = basicFlags | BindingFlags.Public;
			BindingFlags NONPUBLIC_MEMBERS = basicFlags | BindingFlags.NonPublic;
			BindingFlags ALL_MEMBERS = basicFlags | BindingFlags.Public | BindingFlags.NonPublic;

			// Constructors: This will return default constructor if no other constructors exist.			
			ConstructorInfo[] constructors = type.GetConstructors(ALL_MEMBERS); 
			
			// Properties:
			PropertyInfo[] properties = type.GetProperties(ALL_MEMBERS);
			
			// Fields:
			FieldInfo[] fields = type.GetFields(ALL_MEMBERS);
			
			// Event:
			EventInfo[] publicevents = type.GetEvents(ALL_MEMBERS);
			
			// Methods: Will return property set/get and event add/remove methods.
			MethodInfo[] methods = type.GetMethods(PUBLIC_MEMBERS);				
			MethodInfo[] nonpublic = type.GetMethods(NONPUBLIC_MEMBERS);			
			
			// Interface:
			Type[] typeInterfaces = type.GetInterfaces();

			//
			// Now filter out all the members that reference or expose non-public and non-proxied types.
			//
			if (allowedClasses != null)
			{
				constructors = FilterConstructors(constructors);
				properties = FilterProperties(properties);
				methods = FilterMethods(methods);
				nonpublic = FilterMethods(nonpublic);
				fields = FilterFields(fields);
				publicevents = FilterEvents(publicevents);
			}

			//
			// Finalize list of members to provide in proxy.
			//
			classConstructors.Add(type, constructors);
			publicMethods.Add(type, methods);
			nonPublicMethods.Add(type, nonpublic);
			allProperties.Add(type, properties);
			publicFields.Add(type, fields);
			events.Add(type, publicevents);			
			interfaces.Add(type, typeInterfaces);
		}

		private ConstructorInfo[] FilterConstructors(ConstructorInfo[] constructors)
		{
			IList<ConstructorInfo> filtered = new List<ConstructorInfo>();
			foreach (ConstructorInfo ctor in constructors)
			{
				bool isOk = true;
				foreach (ParameterInfo param in ctor.GetParameters())
				{
					isOk &= IsAllowed(param.ParameterType);
					EnsureNamespaces(param.ParameterType);
				}
				if (isOk)
					filtered.Add(ctor);
			}

			ConstructorInfo[] filteredArray = new ConstructorInfo[filtered.Count];
			filtered.CopyTo(filteredArray, 0);
			return filteredArray;
		}

		private EventInfo[] FilterEvents(EventInfo[] events)
		{
			IList<EventInfo> filtered = new List<EventInfo>();
			foreach (EventInfo e in events)
			{
				Type handlerType = e.EventHandlerType;
				if (IsAllowed(handlerType))
				{
					filtered.Add(e);
					EnsureNamespaces(handlerType);
				}
			}

			EventInfo[] filteredArray = new EventInfo[filtered.Count];
			filtered.CopyTo(filteredArray, 0);
			return filteredArray;
		}

		private FieldInfo[] FilterFields(FieldInfo[] fields)
		{
			IList<FieldInfo> filtered = new List<FieldInfo>();
			foreach (FieldInfo field in fields)
			{
				// Allowed and not an event.
				if (IsAllowed(field.FieldType) && field.DeclaringType.GetEvent(field.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) == null)
				{
					filtered.Add(field);
					EnsureNamespaces(field.FieldType);
				}
			}

			FieldInfo[] filteredArray = new FieldInfo[filtered.Count];
			filtered.CopyTo(filteredArray, 0);
			return filteredArray;
		}

		private PropertyInfo[] FilterProperties(PropertyInfo[] properties)
		{
			IList<PropertyInfo> filtered = new List<PropertyInfo>();
			foreach (PropertyInfo prop in properties)
			{
				MethodInfo[] accessors = prop.GetAccessors(true);
				if (accessors.Length > 0 && IsAllowed(prop.PropertyType))
				{
					filtered.Add(prop);
					EnsureNamespaces(prop.PropertyType);
				}
			}

			PropertyInfo[] filteredArray = new PropertyInfo[filtered.Count];
			filtered.CopyTo(filteredArray, 0);
			return filteredArray;
		}

		private MethodInfo[] FilterMethods(MethodInfo[] methods)
		{
			IList<MethodInfo> filtered = new List<MethodInfo>();
			foreach (MethodInfo meth in methods)
			{
				if (meth.IsPublic || VerifyMethodTypes(meth))
				{
					filtered.Add(meth);
					EnsureNamespaces(meth.ReturnType);
					ParameterInfo[] parameters = meth.GetParameters();
					foreach (ParameterInfo param in parameters)
						EnsureNamespaces(param.ParameterType);
				}
			}

			MethodInfo[] filteredArray = new MethodInfo[filtered.Count];
			filtered.CopyTo(filteredArray, 0);
			return filteredArray;
		}

		/// <summary>
		/// Ensures that the Namespace for the given type will be imported by this proxy.
		/// </summary>
		private void EnsureNamespaces(Type type)
		{
			AddImport(type.Namespace);
		}

		/// <summary>
		/// Verify that the return type and parameter types for this method
		/// are Public or "Allowed".
		/// </summary>
		/// <returns>True if all types associated with this method are "allowed".</returns>
		private bool VerifyMethodTypes(MethodInfo meth)
		{            
		    if (!IsAllowed(meth.ReturnType))
		        return false;

		    ParameterInfo[] parameters = meth.GetParameters();
		    foreach (ParameterInfo param in parameters)
		    {
		        if (!IsAllowed(param.ParameterType))
 		            return false;
		    }

		    return true;
		}

		/// <summary>
		/// Return true if property is private.
		/// </summary>
		private bool PropertyIsPrivate(PropertyInfo property)
		{
			return property.GetAccessors(true).Length > 0 && property.GetAccessors(true)[0].IsPrivate;
		}

		#endregion Private Methods

		//------------------------------------------------------
		//
		//  Protected Members
		//
		//------------------------------------------------------

		#region protected Members

		protected static string DEFAULT_BASECLASS = "AReflectiveProxy";

		protected IList<string> allowedClasses = null;

		private FileInfo source;
		protected Assembly assembly;
        
        private IList<string> imports = new List<string>();
		protected string classNamespace = null;
		protected IList<string> delegateDeclarations = new List<string>();
		protected IList<Type> internalClasses = new List<Type>();
		protected Dictionary<Type, ConstructorInfo[]> classConstructors = new Dictionary<Type, ConstructorInfo[]>();
        
        private Dictionary<Type, MethodInfo[]> publicMethods = new Dictionary<Type, MethodInfo[]>();
		protected Dictionary<Type, PropertyInfo[]> allProperties = new Dictionary<Type, PropertyInfo[]>();
        
        private Dictionary<Type, FieldInfo[]> publicFields = new Dictionary<Type, FieldInfo[]>();
        
        private Dictionary<Type, string> baseClass = new Dictionary<Type, string>();
        
        private Dictionary<Type, EventInfo[]> events = new Dictionary<Type, EventInfo[]>();
        
        private Dictionary<Type, MethodInfo[]> nonPublicMethods = new Dictionary<Type, MethodInfo[]>();
        
        private Dictionary<Type, Type[]> interfaces = new Dictionary<Type, Type[]>();

		#endregion protected Members
	}
}


