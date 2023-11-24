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
		private Type m_type = null;
		private bool m_ignoreDerived = true;
		private bool m_ignorePrivate = true;
		private bool m_ignoreConstructorMembers = true;
		private bool m_ignoreEventMembers = true;
		private bool m_ignorePropertyMembers = true;
		private bool m_ignoreConstructorMethods = true;
		private bool m_ignoreEventMethods = true;
		private bool m_ignorePropertyMethods = true;
		private ConstructorCallback m_constructorCallback = null;
		private EventCallback m_eventCallback = null;
		private FieldCallback m_fieldCallback = null;
		private InterfaceCallback m_interfaceCallback = null;
		private MemberCallback m_memberCallback = null;
		private MethodCallback m_methodCallback = null;
		private ParameterCallback m_parameterCallback = null;
		private NestedTypeCallback m_nestedTypeCallback = null;
		private PropertyCallback m_propertyCallback = null;
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
		public ConstructorCallback Constructors { get { return m_constructorCallback; } set { m_constructorCallback = value; } }
		/// <summary>
		/// collection of EventCallback's currently registered
		/// </summary>
		public EventCallback Events { get { return m_eventCallback; } set { m_eventCallback = value; } }
		/// <summary>
		/// collection of FieldCallback's currently registered
		/// </summary>
		public FieldCallback Fields { get { return m_fieldCallback; } set { m_fieldCallback = value; } }
		/// <summary>
		/// collection of InterfaceCallback's currently registered
		/// </summary>
		public InterfaceCallback Interfaces { get { return m_interfaceCallback; } set { m_interfaceCallback = value; } }
		/// <summary>
		/// collection of MemberCallback's currently registered
		/// </summary>
		public MemberCallback Members { get { return m_memberCallback; } set { m_memberCallback = value; } }
		/// <summary>
		/// collection of MethodCallback's currently registered
		/// </summary>
		public MethodCallback Methods { get { return m_methodCallback; } set { m_methodCallback = value; } }
		/// <summary>
		/// collection of ParameterCallback's currently registered
		/// </summary>
		public ParameterCallback Parameters { get { return m_parameterCallback; } set { m_parameterCallback = value; } }
		/// <summary>
		/// collection of NestedTypeCallback's currently registered
		/// </summary>
		public NestedTypeCallback NestedTypes { get { return m_nestedTypeCallback; } set { m_nestedTypeCallback = value; } }
		/// <summary>
		/// collection of PropertyCallback's currently registered
		/// </summary>
		public PropertyCallback Properties { get { return m_propertyCallback; } set { m_propertyCallback = value; } }
		#endregion
		
		#region control flags
		/// <summary>
		/// ignore items that are derived 
		/// </summary>
		public bool IgnoreDerived { get { return m_ignoreDerived; } set { m_ignoreDerived = value; } }
		/// <summary>
		/// ignore private items
		/// </summary>
		public bool IgnorePrivate { get { return m_ignorePrivate; } set { m_ignorePrivate = value; } }
		/// <summary>
		/// during member walk, ignore constructor items.
		/// </summary>
		public bool IgnoreConstructorMembers { get { return m_ignoreConstructorMembers; } set { m_ignoreConstructorMembers = value; } }
		/// <summary>
		/// during member walk, ignore event items.
		/// </summary>
		public bool IgnoreEventMembers { get { return m_ignoreEventMembers; } set { m_ignoreEventMembers = value; } }
		/// <summary>
		/// during member walk, ignore property items.
		/// </summary>
		public bool IgnorePropertyMembers { get { return m_ignorePropertyMembers; } set { m_ignorePropertyMembers = value; } }
		/// <summary>
		/// during method walk, ignore constructor items.
		/// </summary>
		public bool IgnoreConstructorMethods { get { return m_ignoreConstructorMethods; } set { m_ignoreConstructorMethods = value; } }
		/// <summary>
		/// during method walk, ignore event items.
		/// </summary>
		public bool IgnoreEventMethods { get { return m_ignoreEventMethods; } set { m_ignoreEventMethods = value; } }
		/// <summary>
		/// during method walk, ignore property items.
		/// </summary>
		public bool IgnorePropertyMethods { get { return m_ignorePropertyMethods; } set { m_ignorePropertyMethods = value; } }
		#endregion
		
		/// <summary>
		/// type that is being walked
		/// </summary>
		public Type Target { get { return m_type; } }

		/// <summary>
		/// constructor.  does not start walking process
		/// </summary>
		/// <param name="type">Type to be walked</param>
		public TypeWalker(Type type)
		{
			if (type == null) throw new ArgumentNullException("type == null");
			m_type = type;
		}

		/// <summary>
		/// start the walking process.
		/// </summary>
		public void Walk()
		{
			foreach(ConstructorInfo info in SafeMethods.GetConstructors(m_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }

				if(Constructors != null) { Constructors(info); } 
				foreach(ParameterInfo paramInfo in info.GetParameters()) { if(Parameters != null) { Parameters(paramInfo); } }
			}

			foreach(EventInfo info in SafeMethods.GetEvents(m_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Events != null) { Events(info); }
			}

			foreach(FieldInfo info in SafeMethods.GetFields(m_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }
				if(Fields != null) { Fields(info); } 
			}

			foreach(Type info in SafeMethods.GetInterfaces(m_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Interfaces != null) { Interfaces(info); } 
			}

			foreach(MemberInfo info in SafeMethods.GetMembers(m_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnoreConstructorMembers && info.MemberType == MemberTypes.Constructor) { continue; }
				if(IgnoreEventMembers && (info.Name.StartsWith("add_") || info.Name.StartsWith("remove_"))) { continue; }
				if(IgnorePropertyMembers && (info.Name.StartsWith("get_") || info.Name.StartsWith("set_"))) { continue; }
				
				if(Members != null) { Members(info); }
			}

			foreach(MethodInfo info in SafeMethods.GetMethods(m_type)) 
			{ 
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(IgnorePrivate && info.IsPrivate) { continue; }
				if(IgnoreConstructorMethods && info.MemberType == MemberTypes.Constructor) { continue; }
				if(IgnoreEventMethods && (info.Name.StartsWith("add_") || info.Name.StartsWith("remove_"))) { continue; }
				if(IgnorePropertyMethods && (info.Name.StartsWith("get_") || info.Name.StartsWith("set_"))) { continue; }

				if(Methods != null) { Methods(info); } 
				foreach(ParameterInfo paramInfo in info.GetParameters()) { if(Parameters != null) { Parameters(paramInfo); } }
			}

			foreach(Type info in SafeMethods.GetNestedTypes(m_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(NestedTypes != null) { NestedTypes(info); } 
			}

			foreach(PropertyInfo info in SafeMethods.GetProperties(m_type)) 
			{
				if(IgnoreDerived && info.DeclaringType != Target) { continue; }
				if(Properties != null) { Properties(info); } 
			}
		}
	}
}
