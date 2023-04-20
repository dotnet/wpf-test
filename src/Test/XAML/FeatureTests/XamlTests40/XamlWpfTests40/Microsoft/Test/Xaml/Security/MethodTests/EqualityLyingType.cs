// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XamlTestsDev10.Microsoft.Test.Xaml.Security.MethodTests
{
    /// <summary>
    /// Type that lies about its equality - Equals and GetHashCode lie
    /// </summary>
    public class EqualityLyingType : Type
    {
        /// <summary>
        /// Gets the Name property
        /// </summary>
        public override string Name
        {
            get { return "EqualityLyingType"; }
        }

        /// <summary>
        /// Gets the Assembly
        /// </summary>
        public override Assembly Assembly
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the Namespace
        /// </summary>
        public override string Namespace
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the UnderlyingSystemType
        /// </summary>
        public override Type UnderlyingSystemType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the AssemblyQualified Name
        /// </summary>
        public override string AssemblyQualifiedName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the Module
        /// </summary>
        public override Module Module
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the base type
        /// </summary>
        public override Type BaseType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the FullName
        /// </summary>
        public override string FullName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the GUID
        /// </summary>
        public override Guid GUID
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Equals method
        /// </summary>
        /// <param name="o">type parameter</param>
        /// <returns>true if equal</returns>
        public override bool Equals(Type o)
        {
            return o == typeof(Element);
        }

        /// <summary>
        /// GetHashCode method
        /// </summary>
        /// <returns>the hashcode</returns>
        public override int GetHashCode()
        {
            return typeof(Element).GetHashCode();
        }

        /// <summary>
        /// Get the constructors
        /// </summary>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>Array of ConstructorInfo</returns>
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the Type of Element
        /// </summary>
        /// <returns>the elements type</returns>
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the requested event
        /// </summary>
        /// <param name="name">name of event</param>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>Event information</returns>
        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get requested Events
        /// </summary>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>Array of Events</returns>
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get requested Field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>Field information</returns>
        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get requested Fields
        /// </summary>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>Array of fields</returns>
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an interface
        /// </summary>
        /// <param name="name">name of the interface</param>
        /// <param name="ignoreCase">ignore casing</param>
        /// <returns>Type of the interface</returns>
        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all the interfaces
        /// </summary>
        /// <returns>Array of interface types</returns>
        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get members
        /// </summary>
        /// <param name="bindingAttr">Binding flags to use</param>
        /// <returns>List of member information</returns>
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets methods
        /// </summary>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <returns>array of methods</returns>
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a nested type
        /// </summary>
        /// <param name="name">name of the nested type</param>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <returns>nested type</returns>
        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get custom attributes
        /// </summary>
        /// <param name="attributeType">type of attribute</param>
        /// <param name="inherit">inherited or not</param>
        /// <returns>array of custom attributes</returns>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get custom attributes
        /// </summary>
        /// <param name="inherit">inherited or not</param>
        /// <returns>array of custom attributes</returns>
        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Is defined ?
        /// </summary>
        /// <param name="attributeType">type of attribute</param>
        /// <param name="inherit">inherit or not</param>
        /// <returns>true if defined</returns>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get nested types
        /// </summary>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <returns>array of nested types</returns>
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a property
        /// </summary>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <returns>property information</returns>
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Invoke a member
        /// </summary>
        /// <param name="name">name of member to invoke</param>
        /// <param name="invokeAttr">binding flags to use</param>
        /// <param name="binder">binder to use</param>
        /// <param name="target">target object to invoke on</param>
        /// <param name="args">arguments for the member</param>
        /// <param name="modifiers">parameter modifiers to use</param>
        /// <param name="culture">culture to use</param>
        /// <param name="namedParameters">named parameters to use</param>
        /// <returns>return value from invocation</returns>
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the TypeAttributes
        /// </summary>
        /// <returns>type attributes</returns>
        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the ctor info
        /// </summary>
        /// <param name="bindingAttr">binding attr</param>
        /// <param name="binder">the binder to use</param>
        /// <param name="callConvention">callconvention to use</param>
        /// <param name="types">the types to use</param>
        /// <param name="modifiers">the modifiers</param>
        /// <returns>ConstructorInfo instance</returns>
        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Method
        /// </summary>
        /// <param name="name">name of method</param>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <param name="binder">binder to use</param>
        /// <param name="callConvention">call convention to use</param>
        /// <param name="types">types to use</param>
        /// <param name="modifiers">modifiers to use</param>
        /// <returns>Method information</returns>
        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a property
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="bindingAttr">binding flags to use</param>
        /// <param name="binder">binder to use</param>
        /// <param name="returnType">return type</param>
        /// <param name="types">types to use</param>
        /// <param name="modifiers">modifiers to use</param>
        /// <returns>prpoerty information</returns>
        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Does it have an element type impl
        /// </summary>
        /// <returns>true if element type has impl</returns>
        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// is array ?
        /// </summary>
        /// <returns>true if array</returns>
        protected override bool IsArrayImpl()
        {
            return false;
        }

        /// <summary>
        /// is by ref ?
        /// </summary>
        /// <returns>true if by ref</returns>
        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// is com object ?
        /// </summary>
        /// <returns>true if com object</returns>
        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// is pointer ?
        /// </summary>
        /// <returns>true if pointer</returns>
        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// is primitive type ?
        /// </summary>
        /// <returns>true if primitive type</returns>
        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Simple public type
    /// </summary>
    public class Element
    {
    }
}
