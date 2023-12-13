// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using ReflectTools;

namespace WFCTestLib.Util
{
	/// <summary>
	/// Uses reflection to walk through a suppilied type.  Has a several callbacks back
	/// to recieve notification of different aspects of the type.  Also has several boolean flags
	/// to control how the type is walked.  The default is a shallow walk.
	/// 
	/// NOTE: item refers to anything that can be walked; methods, events, etc.
	/// NOTE: control flags and callbacks can be changed at any time, but
	///       the walking process is not restarted.
	/// </summary>
	public class TypeWalker
	{
		#region members
		private Type _type = null;
		private bool _ignoreDerived = true;
		private bool _ignorePrivate = true;
		private bool _ignoreConstructorMembers = true;
		private bool _ignoreEventMembers = true;
		private bool _ignorePropertyMembers = true;
		private bool _ignoreConstructorMethods = true;
		private bool _ignoreEventMethods = true;
		private bool _ignorePropertyMethods = true;
		private ConstructorCallback _constructorCallback = null;
		private EventCallback _eventCallback = null;
		private FieldCallback _fieldCallback = null;
		private InterfaceCallback _interfaceCallback = null;
		private MemberCallback _memberCallback = null;
		private MethodCallback _methodCallback = null;
		private ParameterCallback _parameterCallback = null;
		private NestedTypeCallback _nestedTypeCallback = null;
		private PropertyCallback _propertyCallback = null;
		#endregion

		#region callback types
		/// <summary>
		/// constructor callback type
		/// </summary>
		public delegate void ConstructorCallback(ConstructorInfo info);
		/// <summary>
		/// event callback type
		/// </summary>
		public delegate void EventCallback(EventInfo info);
		/// <summary>
		/// field callback type
		/// </summary>
		public delegate void FieldCallback(FieldInfo info);
		/// <summary>
		/// implemented interface callback type
		/// </summary>
		public delegate void InterfaceCallback(Type info);
		/// <summary>
		/// member callback type
		/// </summary>
		public delegate void MemberCallback(MemberInfo info);
		/// <summary>
		/// method callback type
		/// </summary>
		public delegate void MethodCallback(MethodInfo info);
		/// <summary>
		/// parameter callback type; called during method and constructor walking
		/// </summary>
		public delegate void ParameterCallback(ParameterInfo info);
		/// <summary>
		/// nested type callback type
		/// </summary>
		public delegate void NestedTypeCallback(Type info);
		/// <summary>
		/// property callback type
		/// </summary>
		public delegate void PropertyCallback(PropertyInfo info);
		#endregion

		#region callback collections
		/// <summary>
		/// collection of ConstructorCallback's currently registered
		/// </summary>
		public ConstructorCallback Constructors { get { return _constructorCallback; } set { _constructorCallback = value; } }
		/// <summary>
		/// collection of EventCallback's currently registered
		/// </summary>
		public EventCallback Events { get { return _eventCallback; } set { _eventCallback = value; } }
		/// <summary>
		/// collection of FieldCallback's currently registered
		/// </summary>
		public FieldCallback Fields { get { return _fieldCallback; } set { _fieldCallback = value; } }
		/// <summary>
		/// collection of InterfaceCallback's currently registered
		/// </summary>
		public InterfaceCallback Interfaces { get { return _interfaceCallback; } set { _interfaceCallback = value; } }
		/// <summary>
		/// collection of MemberCallback's currently registered
		/// </summary>
		public MemberCallback Members { get { return _memberCallback; } set { _memberCallback = value; } }
		/// <summary>
		/// collection of MethodCallback's currently registered
		/// </summary>
		public MethodCallback Methods { get { return _methodCallback; } set { _methodCallback = value; } }
		/// <summary>
		/// collection of ParameterCallback's currently registered
		/// </summary>
		public ParameterCallback Parameters { get { return _parameterCallback; } set { _parameterCallback = value; } }
		/// <summary>
		/// collection of NestedTypeCallback's currently registered
		/// </summary>
		public NestedTypeCallback NestedTypes { get { return _nestedTypeCallback; } set { _nestedTypeCallback = value; } }
		/// <summary>
		/// collection of PropertyCallback's currently registered
		/// </summary>
		public PropertyCallback Properties { get { return _propertyCallback; } set { _propertyCallback = value; } }
		#endregion
		
		#region control flags
		/// <summary>
		/// ignore items that are derived 
		/// </summary>
		public bool IgnoreDerived { get { return _ignoreDerived; } set { _ignoreDerived = value; } }
		/// <summary>
		/// ignore private items
		/// </summary>
		public bool IgnorePrivate { get { return _ignorePrivate; } set { _ignorePrivate = value; } }
		/// <summary>
		/// during member walk, ignore constructor items.
		/// </summary>
		public bool IgnoreConstructorMembers { get { return _ignoreConstructorMembers; } set { _ignoreConstructorMembers = value; } }
		/// <summary>
		/// during member walk, ignore event items.
		/// </summary>
		public bool IgnoreEventMembers { get { return _ignoreEventMembers; } set { _ignoreEventMembers = value; } }
		/// <summary>
		/// during member walk, ignore property items.
		/// </summary>
		public bool IgnorePropertyMembers { get { return _ignorePropertyMembers; } set { _ignorePropertyMembers = value; } }
		/// <summary>
		/// during method walk, ignore constructor items.
		/// </summary>
		public bool IgnoreConstructorMethods { get { return _ignoreConstructorMethods; } set { _ignoreConstructorMethods = value; } }
		/// <summary>
		/// during method walk, ignore event items.
		/// </summary>
		public bool IgnoreEventMethods { get { return _ignoreEventMethods; } set { _ignoreEventMethods = value; } }
		/// <summary>
		/// during method walk, ignore property items.
		/// </summary>
		public bool IgnorePropertyMethods { get { return _ignorePropertyMethods; } set { _ignorePropertyMethods = value; } }
		#endregion
		
		/// <summary>
		/// type that is being walked
		/// </summary>
		public Type Target { get { return _type; } }

		/// <summary>
		/// constructor.  does not start walking process
		/// </summary>
		/// <param name="type">Type to be walked</param>
		public TypeWalker(Type type)
		{
			if (type == null) throw new ArgumentNullException("type == null");
			_type = type;
		}

		/// <summary>
		/// start the walking process.
		/// </summary>
		public void Walk()
		{
			foreach(ConstructorInfo info in SafeMethods.GetConstructors(_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }

				if(Constructors != null) { Constructors(info); } 
				foreach(ParameterInfo paramInfo in info.GetParameters()) { if(Parameters != null) { Parameters(paramInfo); } }
			}

			foreach(EventInfo info in SafeMethods.GetEvents(_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Events != null) { Events(info); }
			}

			foreach(FieldInfo info in SafeMethods.GetFields(_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }
				if(Fields != null) { Fields(info); } 
			}

			foreach(Type info in SafeMethods.GetInterfaces(_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Interfaces != null) { Interfaces(info); } 
			}

			foreach(MemberInfo info in SafeMethods.GetMembers(_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnoreConstructorMembers && info.MemberType == MemberTypes.Constructor) { continue; }
				if(IgnoreEventMembers && (info.Name.StartsWith("add_") || info.Name.StartsWith("remove_"))) { continue; }
				if(IgnorePropertyMembers && (info.Name.StartsWith("get_") || info.Name.StartsWith("set_"))) { continue; }
				
				if(Members != null) { Members(info); }
			}

			foreach(MethodInfo info in SafeMethods.GetMethods(_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }
				if(IgnoreConstructorMethods && info.MemberType == MemberTypes.Constructor) { continue; }
				if(IgnoreEventMethods && (info.Name.StartsWith("add_") || info.Name.StartsWith("remove_"))) { continue; }
				if(IgnorePropertyMethods && (info.Name.StartsWith("get_") || info.Name.StartsWith("set_"))) { continue; }

				if(Methods != null) { Methods(info); } 
				foreach(ParameterInfo paramInfo in info.GetParameters()) { if(Parameters != null) { Parameters(paramInfo); } }
			}

			foreach(Type info in SafeMethods.GetNestedTypes(_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(NestedTypes != null) { NestedTypes(info); } 
			}

			foreach(PropertyInfo info in SafeMethods.GetProperties(_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Properties != null) { Properties(info); } 
			}
		}
	}
}
