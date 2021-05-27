// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


/*******************************************************************
 *  This file contains machine generated code.  They are wrappers
 *  of CLR types that can be used without permission demands for
 *  testing within the SEE.
 *  
 *  If there are breaks in this file due to a new CLR drop, it
 *  needs to be regenerated.  To regenerate this file follow the
 *  instructions in HowToGenerate.txt in the same folder as this
 *  file.
 *  
 ******************************************************************/


namespace Microsoft.Test.Security.Wrappers
{
    /// <summary/>
    internal partial class WrapperclassesSecurityWarehouse
    {
        static void Initialize40()
        {
            wrappers["System.Diagnostics.Trace"] = typeof(Microsoft.Test.Security.Wrappers.TraceSW);
            wrappers["System.ComponentModel.PropertyDescriptor"] = typeof(Microsoft.Test.Security.Wrappers.PropertyDescriptorSW);
            wrappers["System.ComponentModel.MemberDescriptor"] = typeof(Microsoft.Test.Security.Wrappers.MemberDescriptorSW);
            
            #if TESTBUILD_CLR40
            wrappers["System.Xaml.XamlObjectWriterSettings"] = typeof(Microsoft.Test.Security.Wrappers.XamlObjectWriterSettingsSW);
            wrappers["System.Xaml.XamlWriterSettings"] = typeof(Microsoft.Test.Security.Wrappers.XamlWriterSettingsSW);
            #endif
        }
    }

    /// <summary>Security Wrapper for Type: System.Diagnostics.Trace</summary>
    [System.CLSCompliant(false)]
    public class TraceSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.TraceSW Wrap(System.Diagnostics.Trace o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.TraceSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Diagnostics.Trace InnerObject
        {
            get { return innerObject as System.Diagnostics.Trace; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal TraceSW(System.Diagnostics.Trace o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal TraceSW(System.DayOfWeek day) { }

        /// <summary/>
        public static System.Diagnostics.TraceListenerCollection Listeners
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.Listeners; }
        }
        /// <summary/>
        public static System.Boolean AutoFlush
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.AutoFlush; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); System.Diagnostics.Trace.AutoFlush = value; }
        }
        /// <summary/>
        public static System.Boolean UseGlobalLock
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.UseGlobalLock; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); System.Diagnostics.Trace.UseGlobalLock = value; }
        }
        /// <summary/>
        public static System.Diagnostics.CorrelationManager CorrelationManager
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.CorrelationManager; }
        }
        /// <summary/>
        public static System.Int32 IndentLevel
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.IndentLevel; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); System.Diagnostics.Trace.IndentLevel = value; }
        }
        /// <summary/>
        public static System.Int32 IndentSize
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return System.Diagnostics.Trace.IndentSize; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); System.Diagnostics.Trace.IndentSize = value; }
        }
        /// <summary/>
        public static void Flush()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Flush();
        }

        /// <summary/>
        public static void Close()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Close();
        }

        /// <summary/>
        public static void Assert(System.Boolean condition)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Assert(condition);
        }

        /// <summary/>
        public static void Assert(System.Boolean condition, System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Assert(condition, message);
        }

        /// <summary/>
        public static void Assert(System.Boolean condition, System.String message, System.String detailMessage)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Assert(condition, message, detailMessage);
        }

        /// <summary/>
        public static void Fail(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Fail(message);
        }

        /// <summary/>
        public static void Fail(System.String message, System.String detailMessage)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Fail(message, detailMessage);
        }

        /// <summary/>
        public static void Refresh()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Refresh();
        }

        /// <summary/>
        public static void TraceInformation(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceInformation(message);
        }

        /// <summary/>
        public static void TraceInformation(System.String format, System.Object[] args)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceInformation(format, args);
        }

        /// <summary/>
        public static void TraceWarning(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceWarning(message);
        }

        /// <summary/>
        public static void TraceWarning(System.String format, System.Object[] args)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceWarning(format, args);
        }

        /// <summary/>
        public static void TraceError(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceError(message);
        }

        /// <summary/>
        public static void TraceError(System.String format, System.Object[] args)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.TraceError(format, args);
        }

        /// <summary/>
        public static void Write(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Write(message);
        }

        /// <summary/>
        public static void Write(System.Object value)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Write(value);
        }

        /// <summary/>
        public static void Write(System.String message, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Write(message, category);
        }

        /// <summary/>
        public static void Write(System.Object value, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Write(value, category);
        }

        /// <summary/>
        public static void WriteLine(System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLine(message);
        }

        /// <summary/>
        public static void WriteLine(System.Object value)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLine(value);
        }

        /// <summary/>
        public static void WriteLine(System.String message, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLine(message, category);
        }

        /// <summary/>
        public static void WriteLine(System.Object value, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLine(value, category);
        }

        /// <summary/>
        public static void WriteIf(System.Boolean condition, System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteIf(condition, message);
        }

        /// <summary/>
        public static void WriteIf(System.Boolean condition, System.Object value)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteIf(condition, value);
        }

        /// <summary/>
        public static void WriteIf(System.Boolean condition, System.String message, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteIf(condition, message, category);
        }

        /// <summary/>
        public static void WriteIf(System.Boolean condition, System.Object value, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteIf(condition, value, category);
        }

        /// <summary/>
        public static void WriteLineIf(System.Boolean condition, System.String message)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLineIf(condition, message);
        }

        /// <summary/>
        public static void WriteLineIf(System.Boolean condition, System.Object value)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLineIf(condition, value);
        }

        /// <summary/>
        public static void WriteLineIf(System.Boolean condition, System.String message, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLineIf(condition, message, category);
        }

        /// <summary/>
        public static void WriteLineIf(System.Boolean condition, System.Object value, System.String category)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.WriteLineIf(condition, value, category);
        }

        /// <summary/>
        public static void Indent()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Indent();
        }

        /// <summary/>
        public static void Unindent()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Diagnostics.Trace.Unindent();
        }
    }

    /// <summary>Security Wrapper for Type: System.ComponentModel.PropertyDescriptor</summary>
    [System.CLSCompliant(false)]
    public class PropertyDescriptorSW : Microsoft.Test.Security.Wrappers.MemberDescriptorSW
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.PropertyDescriptorSW Wrap(System.ComponentModel.PropertyDescriptor o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.PropertyDescriptorSW(o);
        }
        /// <summary>Gets the wrapped instance</summary>
        public new System.ComponentModel.PropertyDescriptor InnerObject
        {
            get { return innerObject as System.ComponentModel.PropertyDescriptor; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal PropertyDescriptorSW(System.ComponentModel.PropertyDescriptor o)
            : base(o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal PropertyDescriptorSW(System.DayOfWeek day) : base(System.DayOfWeek.Friday) { }

        /// <summary/>
        protected PropertyDescriptorSW(System.String name, System.Attribute[] attrs)
            : base(System.DayOfWeek.Friday)
        {
        }

        /// <summary/>
        protected PropertyDescriptorSW(System.ComponentModel.MemberDescriptor descr)
            : base(System.DayOfWeek.Friday)
        {
        }

        /// <summary/>
        protected PropertyDescriptorSW(System.ComponentModel.MemberDescriptor descr, System.Attribute[] attrs)
            : base(System.DayOfWeek.Friday)
        {
        }

        /// <summary/>
        public virtual Microsoft.Test.Security.Wrappers.TypeSW ComponentType
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return (Microsoft.Test.Security.Wrappers.TypeSW)WrapperclassesSecurityWarehouse.Wrap(InnerObject.ComponentType); }
        }
        /// <summary/>
        public virtual System.ComponentModel.TypeConverter Converter
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Converter; }
        }
        /// <summary/>
        public virtual System.Boolean IsLocalizable
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.IsLocalizable; }
        }
        /// <summary/>
        public virtual System.Boolean IsReadOnly
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.IsReadOnly; }
        }
        /// <summary/>
        public System.ComponentModel.DesignerSerializationVisibility SerializationVisibility
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.SerializationVisibility; }
        }
        /// <summary/>
        public virtual Microsoft.Test.Security.Wrappers.TypeSW PropertyType
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return (Microsoft.Test.Security.Wrappers.TypeSW)WrapperclassesSecurityWarehouse.Wrap(InnerObject.PropertyType); }
        }
        /// <summary/>
        public virtual System.Boolean SupportsChangeEvents
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.SupportsChangeEvents; }
        }
        /// <summary/>
        public override System.Int32 GetHashCode()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetHashCode();
        }

        /// <summary/>
        public virtual void AddValueChanged(System.Object component, System.EventHandler handler)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.AddValueChanged(component, handler);
        }

        /// <summary/>
        public virtual System.Boolean CanResetValue(System.Object component)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.CanResetValue(component);
        }

        /// <summary/>
        public override System.Boolean Equals(System.Object obj)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.Equals(obj);
        }

        /// <summary/>
        public System.ComponentModel.PropertyDescriptorCollection GetChildProperties()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetChildProperties();
        }

        /// <summary/>
        public System.ComponentModel.PropertyDescriptorCollection GetChildProperties(System.Attribute[] filter)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetChildProperties(filter);
        }

        /// <summary/>
        public System.ComponentModel.PropertyDescriptorCollection GetChildProperties(System.Object instance)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetChildProperties(instance);
        }

        /// <summary/>
        public virtual System.ComponentModel.PropertyDescriptorCollection GetChildProperties(System.Object instance, System.Attribute[] filter)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetChildProperties(instance, filter);
        }

        /// <summary/>
        public virtual System.Object GetEditor(System.Type editorBaseType)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetEditor(editorBaseType);
        }

        /// <summary/>
        public virtual System.Object GetValue(System.Object component)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetValue(component);
        }

        /// <summary/>
        public virtual void RemoveValueChanged(System.Object component, System.EventHandler handler)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.RemoveValueChanged(component, handler);
        }

        /// <summary/>
        public virtual void ResetValue(System.Object component)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.ResetValue(component);
        }

        /// <summary/>
        public virtual void SetValue(System.Object component, System.Object value)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.SetValue(component, value);
        }

        /// <summary/>
        public virtual System.Boolean ShouldSerializeValue(System.Object component)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.ShouldSerializeValue(component);
        }


    }

    /// <summary>Security Wrapper for Type: System.ComponentModel.MemberDescriptor</summary>
    [System.CLSCompliant(false)]
    public class MemberDescriptorSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.MemberDescriptorSW Wrap(System.ComponentModel.MemberDescriptor o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.MemberDescriptorSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.ComponentModel.MemberDescriptor InnerObject
        {
            get { return innerObject as System.ComponentModel.MemberDescriptor; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal MemberDescriptorSW(System.ComponentModel.MemberDescriptor o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal MemberDescriptorSW(System.DayOfWeek day) { }

        /// <summary/>
        protected MemberDescriptorSW(System.String name, System.Attribute[] attributes)
        {
        }

        /// <summary/>
        protected MemberDescriptorSW(System.String name)
        {
        }

        /// <summary/>
        protected MemberDescriptorSW(System.ComponentModel.MemberDescriptor oldMemberDescriptor, System.Attribute[] newAttributes)
        {
        }

        /// <summary/>
        public virtual System.ComponentModel.AttributeCollection Attributes
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Attributes; }
        }
        /// <summary/>
        public virtual System.String Category
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Category; }
        }
        /// <summary/>
        public virtual System.String Description
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Description; }
        }
        /// <summary/>
        public virtual System.Boolean IsBrowsable
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.IsBrowsable; }
        }
        /// <summary/>
        public virtual System.String Name
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Name; }
        }
        /// <summary/>
        public virtual System.Boolean DesignTimeOnly
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.DesignTimeOnly; }
        }
        /// <summary/>
        public virtual System.String DisplayName
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.DisplayName; }
        }
        /// <summary/>
        public override System.Boolean Equals(System.Object obj)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.Equals(obj);
        }

        /// <summary/>
        public override System.Int32 GetHashCode()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetHashCode();
        }


    }

    #if TESTBUILD_CLR40
    /// <summary>Security Wrapper for Type: System.Xaml.XamlObjectWriterSettings</summary>
    [System.CLSCompliant(false)]
    public class XamlObjectWriterSettingsSW : Microsoft.Test.Security.Wrappers.XamlWriterSettingsSW
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.XamlObjectWriterSettingsSW Wrap(System.Xaml.XamlObjectWriterSettings o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.XamlObjectWriterSettingsSW(o);
        }
        /// <summary>Gets the wrapped instance</summary>
        public new System.Xaml.XamlObjectWriterSettings InnerObject
        {
            get { return innerObject as System.Xaml.XamlObjectWriterSettings; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal XamlObjectWriterSettingsSW(System.Xaml.XamlObjectWriterSettings o)
            : base(o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal XamlObjectWriterSettingsSW(System.DayOfWeek day) : base(System.DayOfWeek.Friday) { }

        /// <summary/>
        public XamlObjectWriterSettingsSW()
            : base(System.DayOfWeek.Friday)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Xaml.XamlObjectWriterSettings();
        }

        /// <summary/>
        public System.EventHandler<System.Xaml.XamlObjectEventArgs> AfterBeginInitHandler
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.AfterBeginInitHandler; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.AfterBeginInitHandler = value; }
        }
        /// <summary/>
        public System.EventHandler<System.Xaml.XamlObjectEventArgs> BeforePropertiesHandler
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.BeforePropertiesHandler; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.BeforePropertiesHandler = value; }
        }
        /// <summary/>
        public System.EventHandler<System.Xaml.XamlObjectEventArgs> AfterPropertiesHandler
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.AfterPropertiesHandler; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.AfterPropertiesHandler = value; }
        }
        /// <summary/>
        public System.EventHandler<System.Xaml.XamlObjectEventArgs> AfterEndInitHandler
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.AfterEndInitHandler; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.AfterEndInitHandler = value; }
        }
        /// <summary/>
        public System.EventHandler<System.Windows.Markup.XamlSetValueEventArgs> XamlSetValueHandler
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.XamlSetValueHandler; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.XamlSetValueHandler = value; }
        }
        /// <summary/>
        public System.Object RootObjectInstance
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.RootObjectInstance; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.RootObjectInstance = value; }
        }
        /// <summary/>
        public System.Boolean IgnoreCanConvert
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.IgnoreCanConvert; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.IgnoreCanConvert = value; }
        }
        /// <summary/>
        public System.Windows.Markup.INameScope ExternalNameScope
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.ExternalNameScope; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.ExternalNameScope = value; }
        }
        /// <summary/>
        public System.Boolean SkipDuplicatePropertyCheck
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.SkipDuplicatePropertyCheck; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.SkipDuplicatePropertyCheck = value; }
        }
        /// <summary/>
        public System.Boolean RegisterNamesOnExternalNamescope
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.RegisterNamesOnExternalNamescope; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.RegisterNamesOnExternalNamescope = value; }
        }
        /// <summary/>
        public System.Xaml.Permissions.XamlAccessLevel AccessLevel
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.AccessLevel; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.AccessLevel = value; }
        }
    }

    /// <summary>Security Wrapper for Type: System.Xaml.XamlWriterSettings</summary>
    [System.CLSCompliant(false)]
    public class XamlWriterSettingsSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.XamlWriterSettingsSW Wrap(System.Xaml.XamlWriterSettings o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.XamlWriterSettingsSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Xaml.XamlWriterSettings InnerObject
        {
            get { return innerObject as System.Xaml.XamlWriterSettings; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal XamlWriterSettingsSW(System.Xaml.XamlWriterSettings o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal XamlWriterSettingsSW(System.DayOfWeek day) { }

        /// <summary/>
        public XamlWriterSettingsSW()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Xaml.XamlWriterSettings();
        }


    }

    /// <summary>Security Wrapper for Type: System.Xaml.XamlServices</summary>
    [System.CLSCompliant(false)]
    public static class XamlServicesSW : System.Object
    {
        /// <summary/>
        public static void Transform(System.Xaml.XamlReader reader, System.Xaml.XamlWriter writer, System.Xaml.Permissions.XamlAccessLevel accessLevel, bool requestReflectionPermission)
        {
            if (requestReflectionPermission)
            {
                System.Security.PermissionSet permissions = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.None);
                permissions.AddPermission(new System.Xaml.Permissions.XamlLoadPermission(accessLevel));
                permissions.AddPermission(new System.Security.Permissions.ReflectionPermission(System.Security.Permissions.ReflectionPermissionFlag.RestrictedMemberAccess));
                permissions.Assert();
            }
            else
            {
                new System.Xaml.Permissions.XamlLoadPermission(accessLevel).Assert();
            }            
            
            System.Xaml.XamlServices.Transform(reader, writer);
        }
    }
#endif
}
