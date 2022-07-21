// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Creates n ProxyDefinitions for a given directory.
//				 Handles determining proxy inheritence and namespace 
//				 conversions.

using System;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{
	/// <summary>
	/// Creates a set of ProxyDefinitions, then interates through all these definitions and
	/// sets up the baseclasses, interfaces, and namespaces.  Uses ProxyWriter to convert these
	/// definitions to c# and write them to disk.
	/// </summary>
	public class ProxyFactory
	{
		//------------------------------------------------------
		//
		//  Constructors
		//
		//------------------------------------------------------

		#region Constructors

		/// <summary>
		/// Initialze a ProxyFactory that will locate classes below the given directory path, and
		/// in the given assembly.
		/// </summary>
		/// <param name="assemblyToLoadFrom">Assembly that contains the definitions of all types to
		/// create proxies for (e.g. PresentationFramework).</param>
		/// <param name="sourcePaths">Base directory and specific files to search for classes to generate proxies for.</param>
		public ProxyFactory(Assembly assemblyToLoadFrom, string [] sourcePaths)
		{
			Initialize(sourcePaths);
			assembly = assemblyToLoadFrom;
		}

		#endregion Constructors

		//------------------------------------------------------
		//
		//  Public Methods
		//
		//------------------------------------------------------

		#region Public Methods

		/// <summary>
		/// Create proxies for each class defined in the given list of allowedClasses.
		/// 
		/// Recurse through each directory below 'sources' parsing each .cs file.  Each class declaration
		/// whos type exists in the given list will have a proxy generated for it.
		/// 
		/// Directory and file structure is preserved.  In other words, proxies generated for multiple classes 
		/// defined in the same file will will exist in the same file.
		/// </summary>
		/// <param name="allowedClasses">List of fully qualified type names of classes that
		/// proxies should be generated for.</param>
		/// <param name="importOverloads">Blob of text defining import overloads and alias' to be appended to
		/// each proxy.</param>
		/// <returns>List of Types that proxies were generated for.</returns>
		public IList<Type> CreateProxies(IList<string> allowedClasses, string importOverloads)
		{
			foreach(FileSystemInfo src in sources)
				CreateProxyDefinitions(src, allowedClasses);
			EnsureNamespaces();
			EnsureBaseClasses();
			return WriteProxies(importOverloads);
		}

		#endregion Public Methods

		//------------------------------------------------------
		//
		//  Public Properties
		//
		//------------------------------------------------------

		#region Public Properties

		#endregion Public Properties

		//------------------------------------------------------
		//
		//  Private Methods
		//
		//------------------------------------------------------

		#region Private Methods

		private void Initialize(string [] paths)
		{
			FileSystemInfo[] srcs = new FileSystemInfo[paths.Length];
			for (int i = 0; i < paths.Length; i++) 
			{
				if (Directory.Exists(paths[i]))
					srcs[i] = new DirectoryInfo(paths[i]);
				else if (File.Exists(paths[i]))
					srcs[i] = new FileInfo(paths[i]);
				else
					throw new FileNotFoundException("ProxyFactory initialized with invalid directory/file path '" + paths[i] + "'.");
			}
			sources = srcs;
		}

		/// <summary>
		/// Create Proxies for all sources in current directory and all subdirectories.
		/// </summary>
		private void CreateProxyDefinitions(FileSystemInfo path, IList<string> allowedClasses)
		{
			FileInfo[] files;
			DirectoryInfo[] subDirectories;
		
			// Create ProxyDefinitions for all sources files in current directory.			
			if (path is DirectoryInfo)
			{
				DirectoryInfo dir = (DirectoryInfo)path;
				files = dir.GetFiles("*.cs");
				subDirectories = dir.GetDirectories();
			}
			else
			{
				files = new FileInfo[] { (FileInfo)path };
				subDirectories = new DirectoryInfo[0];
			}

			for (int i=0; i < files.Length; i++) 
			{
				ProxyDefinition def;
				if (assembly == null)
					def = new ProxyDefinition(files[i], allowedClasses);
				else
					def = new ProxyDefinition(assembly, files[i], allowedClasses);

				if (def.NeedsProxy)
				{
					proxyDefinitions.Add(def);

					// Record what types we are creating proxies for.
					IEnumerator<Type> classTypes = def.ClassTypes.GetEnumerator();
					while (classTypes.MoveNext())
					{
						if (proxyDelegateTypes.Contains(classTypes.Current))
							throw new AnnotationProxyException("Detected multiple proxies for the same type.");

						proxyDelegateTypes.Add(classTypes.Current);
					}

					// Record what namespaces we are creating proxies for.
					if (!namespacesWithProxies.Contains(def.Namespace))
						namespacesWithProxies.Add(def.Namespace);
				}
			}

			// Recursively process subdirectories.
			for (int i=0; i < subDirectories.Length; i++)
			{
				CreateProxyDefinitions(subDirectories[i], allowedClasses);
			}
		}

		/// <summary>
		/// We want to create generate proxies in a symmetric directory structure to its delegates
		/// relative to the current directory.
		/// 
		/// e.g.
		/// source = somepath\Annotations\
		///								 \Components
		/// 
		/// output = ourpath\Proxies\Annotations\
		///										\Components
		///										...
		///
		/// </summary>
		/// <param name="currentDir"></param>
		/// <returns></returns>
		private DirectoryInfo DetermineOutputDirectory(FileInfo file)
		{
			string absolutedir = file.Directory.FullName;
			if (!absolutedir.EndsWith("\\\\"))
				absolutedir += "\\";

			DirectoryInfo dir = null;
			foreach (FileSystemInfo cur in sources)
			{
				DirectoryInfo delegateDir = cur as DirectoryInfo;
				if (delegateDir == null)
					delegateDir = ((FileInfo)cur).Directory;

				if (absolutedir.StartsWith(delegateDir.FullName))
				{
					dir = delegateDir;
					break;
				}
			}

			string relativepath = "Proxies/" + absolutedir.Replace(dir.FullName, "");
			DirectoryInfo outputdir = new DirectoryInfo(relativepath);
			if (!outputdir.Exists)
				outputdir.Create();
			return outputdir;
		}

		/// <summary>
		/// Proxies live in a different namespace than their Delegate (e.g. "Proxies.XXX"), therefore
		/// any XXX namespaces that are being 'used' must be converted to their symmetric Proxy namespace.
		/// </summary>
		private void EnsureNamespaces() 
		{
			IEnumerator<ProxyDefinition> defs = proxyDefinitions.GetEnumerator();
			while (defs.MoveNext())
			{
				IList<string> imports = defs.Current.Imports;
				for (int i=0; i < imports.Count; i++) 
				{
					if (namespacesWithProxies.Contains(imports[i]))
					{
						imports[i] = ProxyConstants.PROXY_NAMESPACE_PREFIX + "." + imports[i];
					}
				}
			}
		}

		/// <summary>
		/// Most proxies can inherit directly from AReflectiveProxy.  However we need to support this case:
		///			ClassA inst = new SubClassA();
		/// This means that if a delegate's BaseType is also a delegate it must extend its parent's proxy.
		/// 
		/// e.g. 
		///		public ClassA : AReflectiveProxy {...}
		/// then
		///		public SubClassA : ClassA { ... }
		/// </summary>
		private void EnsureBaseClasses() 
		{
			IEnumerator<ProxyDefinition> defs = proxyDefinitions.GetEnumerator();
			while (defs.MoveNext())
			{
				IList<Type> classTypes = defs.Current.ClassTypes;
				for (int i=0; i< classTypes.Count; i++) {
					Type baseType = classTypes[i].BaseType;
					if (proxyDelegateTypes.Contains(baseType)) // If there is a proxy for the baseType.
					{
						// Set base class to original baseclass name, otherwise it will
						// default to AReflectiveProxy.
						defs.Current.SetBaseClass(classTypes[i], baseType);
					}

					// Ok interfaces are those that we created proxies for.
					IList<Type> okInterfaces = new List<Type>();
					Type[] interfaces = defs.Current.Interfaces(classTypes[i]);
					for (int n = 0; n < interfaces.Length; n++)
					{
						if (proxyDelegateTypes.Contains(interfaces[n]))
							okInterfaces.Add(interfaces[n]);
					}

					Type[] result = new Type[okInterfaces.Count];
					okInterfaces.CopyTo(result, 0);
					defs.Current.SetInterfaces(classTypes[i], result);

//					if (baseType != null)
//					{
//						Type[] baseClassInterfaces = baseType.GetInterfaces();
//
//						// Ok interfaces are those that we implement directly.
//						IList<Type> okInterfaces = new List<Type>();
//						Type[] interfaces = defs.Current.Interfaces(classTypes[i]);
//						for (int n = 0; n < interfaces.Length; n++)
//						{
//							bool ok = true;
//							foreach (Type inter in baseClassInterfaces)
//							{
//								if (Type.Equals(inter, interfaces[i]))
//								{
//									ok = false;
//								}
//							}
//							if (ok)
//								okInterfaces.Add(interfaces[n]);
//						}
//
//						Type[] result = new Type[okInterfaces.Count];
//						okInterfaces.CopyTo(result, 0);
//						defs.Current.SetInterfaces(classTypes[i], result);
//					}
				}
			}
		}

		/// <summary>
		/// Iterate through each ProxyDefinition and use ProxyWriter to write the actual proxy class.
		/// </summary>
		private IList<Type> WriteProxies(string importOverloads)
		{
			IList<Type> proxyNames = new List<Type>();

			ProxyWriter proxyWriter = new ProxyWriter();
			proxyWriter.ImportOverloads = importOverloads;
			IEnumerator<ProxyDefinition> defs = proxyDefinitions.GetEnumerator();
			while (defs.MoveNext())
			{
				DirectoryInfo outputdir = DetermineOutputDirectory(defs.Current.Source);
				//Console.WriteLine("Writing proxy: " +  outputdir.FullName + "\\" + defs.Current.Source.Name);
				proxyWriter.WriteProxy(defs.Current, outputdir);

				foreach(Type classType in defs.Current.ClassTypes)
					proxyNames.Add(classType);
			}

			return proxyNames;
		}

		#endregion Private Methods

		//------------------------------------------------------
		//
		//  Protected Members
		//
		//------------------------------------------------------

		#region protected Members

		protected FileSystemInfo[] sources;
		protected Assembly assembly;

		protected IList<ProxyDefinition> proxyDefinitions = new List<ProxyDefinition>();

		/// <summary>
		/// List of Namespaces that contain proxies.  
		/// Used to convert imports from delegate namespaces to proxy namespaces.
		/// </summary>
		protected IList<string> namespacesWithProxies = new List<string>();

		/// <summary>
		/// List of all Type's for which a ProxyDefinition exists.
		/// Used for determining Proxy inheritance.
		/// </summary>
		protected IList<Type> proxyDelegateTypes = new List<Type>();

		#endregion protected Members
	}
}
