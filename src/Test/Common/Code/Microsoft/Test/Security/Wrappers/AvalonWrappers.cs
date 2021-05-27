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
    internal class AvalonwrappersSecurityWarehouse
    {

        static System.Collections.Hashtable wrappers = new System.Collections.Hashtable();

        static AvalonwrappersSecurityWarehouse()
        {

            wrappers["System.Windows.FrameworkElement"] = typeof(Microsoft.Test.Security.Wrappers.FrameworkElementSW);
            wrappers["System.Windows.Window"] = typeof(Microsoft.Test.Security.Wrappers.WindowSW);
            wrappers["System.Windows.Interop.WindowInteropHelper"] = typeof(Microsoft.Test.Security.Wrappers.WindowInteropHelperSW);
            wrappers["System.Windows.Navigation.NavigationWindow"] = typeof(Microsoft.Test.Security.Wrappers.NavigationWindowSW);
            wrappers["System.Windows.Application"] = typeof(Microsoft.Test.Security.Wrappers.ApplicationSW);
            wrappers["System.Windows.Interop.HwndSource"] = typeof(Microsoft.Test.Security.Wrappers.HwndSourceSW);
            wrappers["System.Windows.Input.StagingAreaInputItem"] = typeof(Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW);
            wrappers["System.Windows.Input.ProcessInputEventArgs"] = typeof(Microsoft.Test.Security.Wrappers.ProcessInputEventArgsSW);
            wrappers["System.Windows.Clipboard"] = typeof(Microsoft.Test.Security.Wrappers.ClipboardSW);
            wrappers["System.Windows.DataObject"] = typeof(Microsoft.Test.Security.Wrappers.DataObjectSW);
            wrappers["System.Windows.Threading.Dispatcher"] = typeof(Microsoft.Test.Security.Wrappers.DispatcherSW);
            wrappers["System.Windows.Threading.DispatcherHooks"] = typeof(Microsoft.Test.Security.Wrappers.DispatcherHooksSW);
        }

        /// <summary/>
        public static object Wrap(object o)
        {
            if (o == null)
                return o;
            System.Type typ = o.GetType();
            System.Type wrappedTyp = null;
            if (typ.IsArray)
            {
                System.Type itemType = typ.Assembly.GetType(typ.FullName.TrimEnd('[', ',', ']'));
                wrappedTyp = wrappers[itemType.FullName] as System.Type;
                if (wrappedTyp == null)
                    return o;
                System.Array arr = System.Array.CreateInstance(wrappedTyp, ((System.Array)o).Length);
                for (int i = 0; i < arr.Length; i++)
                    arr.SetValue(Wrap(((System.Array)o).GetValue(i)), i);
                return arr;
            }

            while (typ != null && wrappedTyp == null)
            {
                wrappedTyp = (System.Type)wrappers[typ.FullName];
                if (wrappedTyp == null)
                    typ = typ.BaseType;
            }
            if (wrappedTyp == null)
                return o;

            System.Reflection.MethodInfo mInfo = wrappedTyp.GetMethod("Wrap", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new System.Type[] { typ }, null);
            return mInfo.Invoke(null, new object[] { o });
        }

    }


    /// <summary>Security Wrapper for Type: System.Windows.FrameworkElement</summary>
    [System.CLSCompliant(false)]
    public class FrameworkElementSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.FrameworkElementSW Wrap(System.Windows.FrameworkElement o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.FrameworkElementSW(o);
        }
        //TEMP: changed to public for Ribbon wrapper usage.  change back 
        //      to internal when Ribbon is in the framework.
        public object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.FrameworkElement InnerObject
        {
            get { return innerObject as System.Windows.FrameworkElement; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal FrameworkElementSW(System.Windows.FrameworkElement o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal FrameworkElementSW(System.DayOfWeek day) { }

        public virtual System.Double Height
        {
            get
            {
                new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                System.Double innerHeight = InnerObject.Height;
                // Workaround erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
                return innerHeight;
            }
            set
            {
                new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                InnerObject.Height = value;
                // Workaround for erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
            }
        }
        /// <summary/>
        public virtual System.Double Width
        {
            get
            {
                new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                System.Double innerWidth = InnerObject.Width;
                // Workaround erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
                return innerWidth;
            }
            set
            {
                new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                InnerObject.Width = value;
                // Workaround erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
            }
        }

    }

    /// <summary>Security Wrapper for Type: System.Windows.Window</summary>
    [System.CLSCompliant(false)]
    public class WindowSW : Microsoft.Test.Security.Wrappers.FrameworkElementSW
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.WindowSW Wrap(System.Windows.Window o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.WindowSW(o);
        }
        /// <summary>Gets the wrapped instance</summary>
        public new System.Windows.Window InnerObject
        {
            get { return innerObject as System.Windows.Window; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        public WindowSW(System.Windows.Window o)
            : base(o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        public WindowSW(System.DayOfWeek day) : base(System.DayOfWeek.Friday) { }

        /// <summary/>
        public WindowSW()
            : base(System.DayOfWeek.Friday)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Window();
        }

        /// <summary/>
        public System.Boolean Topmost
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Topmost; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.Topmost = value; }
        }
        /// <summary/>
        public System.Double Top
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); 
                  System.Double innerTop = InnerObject.Top;
                  // Workaround for erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                  var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
                  return innerTop;
                }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); 
                  InnerObject.Top = value;
                  // Workaround for erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                  var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;         
                }
        }
        /// <summary/>
        public System.Double Left
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); 
                  System.Double innerLeft = InnerObject.Left;
                  // Workaround for erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                  var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;
                  return innerLeft;
                }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); 
                  InnerObject.Left = value;
                  // Workaround for erroneous SecurityException thrown on XP AMD64 scenarios for GAC elevation
                  var ignored = Microsoft.Test.Diagnostics.SystemInformation.Current.OSProductType;         
                }
        }
        /// <summary/>
        public System.Boolean Activate()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.Activate();
        }

        /// <summary/>
        public void Show()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.Show();
        }

        /// <summary/>
        public void Close()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.Close();
        }

        /// <summary/>
        public System.Windows.WindowStyle WindowStyle
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.WindowStyle; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.WindowStyle = value; }
        }
        /// <summary/>
        public virtual System.Object Content
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Content; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.Content = value; }
        }
        /// <summary/>
        public System.Windows.Media.Brush Background
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Background; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.Background = value; }
        }
        /// <summary/>
        public System.Windows.SizeToContent SizeToContent
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.SizeToContent; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.SizeToContent = value; }
        }

    }

    /// <summary>Security Wrapper for Type: System.Windows.Interop.WindowInteropHelper</summary>
    [System.CLSCompliant(false)]
    public class WindowInteropHelperSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.WindowInteropHelperSW Wrap(System.Windows.Interop.WindowInteropHelper o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.WindowInteropHelperSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Interop.WindowInteropHelper InnerObject
        {
            get { return innerObject as System.Windows.Interop.WindowInteropHelper; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal WindowInteropHelperSW(System.Windows.Interop.WindowInteropHelper o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal WindowInteropHelperSW(System.DayOfWeek day) { }

        /// <summary/>
        public WindowInteropHelperSW(System.Windows.Window window)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Interop.WindowInteropHelper(window);
        }

        /// <summary/>
        public System.IntPtr Handle
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Handle; }
        }

    }

    /// <summary>Security Wrapper for Type: System.Windows.Navigation.NavigationWindow</summary>
    [System.CLSCompliant(false)]
    public class NavigationWindowSW : Microsoft.Test.Security.Wrappers.WindowSW
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.NavigationWindowSW Wrap(System.Windows.Navigation.NavigationWindow o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.NavigationWindowSW(o);
        }
        /// <summary>Gets the wrapped instance</summary>
        public new System.Windows.Navigation.NavigationWindow InnerObject
        {
            get { return innerObject as System.Windows.Navigation.NavigationWindow; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal NavigationWindowSW(System.Windows.Navigation.NavigationWindow o)
            : base(o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal NavigationWindowSW(System.DayOfWeek day) : base(System.DayOfWeek.Friday) { }

        /// <summary/>
        public NavigationWindowSW()
            : base(System.DayOfWeek.Friday)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Navigation.NavigationWindow();
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.Application</summary>
    [System.CLSCompliant(false)]
    public class ApplicationSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.ApplicationSW Wrap(System.Windows.Application o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.ApplicationSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Application InnerObject
        {
            get { return innerObject as System.Windows.Application; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal ApplicationSW(System.Windows.Application o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal ApplicationSW(System.DayOfWeek day) { }

        /// <summary/>
        public static Microsoft.Test.Security.Wrappers.ApplicationSW Current
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return (Microsoft.Test.Security.Wrappers.ApplicationSW)AvalonwrappersSecurityWarehouse.Wrap(System.Windows.Application.Current); }
        }
        /// <summary/>
        public void Shutdown()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.Shutdown();
        }

        /// <summary/>
        public void Shutdown(System.Int32 exitCode)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.Shutdown(exitCode);
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.Interop.HwndSource</summary>
    [System.CLSCompliant(false)]
    public class HwndSourceSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.HwndSourceSW Wrap(System.Windows.Interop.HwndSource o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.HwndSourceSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Interop.HwndSource InnerObject
        {
            get { return innerObject as System.Windows.Interop.HwndSource; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal HwndSourceSW(System.Windows.Interop.HwndSource o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal HwndSourceSW(System.DayOfWeek day) { }

        /// <summary/>
        public HwndSourceSW(System.Int32 classStyle, System.Int32 style, System.Int32 exStyle, System.Int32 x, System.Int32 y, System.String name, System.IntPtr parent)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Interop.HwndSource(classStyle, style, exStyle, x, y, name, parent);
        }

        /// <summary/>
        public HwndSourceSW(System.Int32 classStyle, System.Int32 style, System.Int32 exStyle, System.Int32 x, System.Int32 y, System.Int32 width, System.Int32 height, System.String name, System.IntPtr parent, System.Boolean adjustSizingForNonClientArea)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Interop.HwndSource(classStyle, style, exStyle, x, y, width, height, name, parent, adjustSizingForNonClientArea);
        }

        /// <summary/>
        public HwndSourceSW(System.Int32 classStyle, System.Int32 style, System.Int32 exStyle, System.Int32 x, System.Int32 y, System.Int32 width, System.Int32 height, System.String name, System.IntPtr parent)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Interop.HwndSource(classStyle, style, exStyle, x, y, width, height, name, parent);
        }

        /// <summary/>
        public HwndSourceSW(System.Windows.Interop.HwndSourceParameters parameters)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            this.innerObject = new System.Windows.Interop.HwndSource(parameters);
        }

        /// <summary/>
        public virtual void Dispose()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.Dispose();
        }

        /// <summary/>
        public virtual System.Windows.Media.Visual RootVisual
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.RootVisual; }
            set { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.RootVisual = value; }
        }
        /// <summary/>
        public virtual System.IntPtr Handle
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Handle; }
        }

    }

    /// <summary>Security Wrapper for Type: System.Windows.Input.StagingAreaInputItem</summary>
    [System.CLSCompliant(false)]
    public class StagingAreaInputItemSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW Wrap(System.Windows.Input.StagingAreaInputItem o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Input.StagingAreaInputItem InnerObject
        {
            get { return innerObject as System.Windows.Input.StagingAreaInputItem; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal StagingAreaInputItemSW(System.Windows.Input.StagingAreaInputItem o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal StagingAreaInputItemSW(System.DayOfWeek day) { }

        /// <summary/>
        public System.Windows.Input.InputEventArgs Input
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return InnerObject.Input; }
        }

    }

    /// <summary>Security Wrapper for Type: System.Windows.Input.ProcessInputEventArgs</summary>
    [System.CLSCompliant(false)]
    public class ProcessInputEventArgsSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.ProcessInputEventArgsSW Wrap(System.Windows.Input.ProcessInputEventArgs o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.ProcessInputEventArgsSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Input.ProcessInputEventArgs InnerObject
        {
            get { return innerObject as System.Windows.Input.ProcessInputEventArgs; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal ProcessInputEventArgsSW(System.Windows.Input.ProcessInputEventArgs o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal ProcessInputEventArgsSW(System.DayOfWeek day) { }

        /// <summary/>
        public Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW PeekInput()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return (Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW)AvalonwrappersSecurityWarehouse.Wrap(InnerObject.PeekInput());
        }

        /// <summary/>
        public Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW PushInput(System.Windows.Input.InputEventArgs input, System.Windows.Input.StagingAreaInputItem promote)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return (Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW)AvalonwrappersSecurityWarehouse.Wrap(InnerObject.PushInput(input, promote));
        }

        /// <summary/>
        public Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW PushInput(System.Windows.Input.StagingAreaInputItem input)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return (Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW)AvalonwrappersSecurityWarehouse.Wrap(InnerObject.PushInput(input));
        }

        /// <summary/>
        public Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW PopInput()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return (Microsoft.Test.Security.Wrappers.StagingAreaInputItemSW)AvalonwrappersSecurityWarehouse.Wrap(InnerObject.PopInput());
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.Clipboard</summary>
    [System.CLSCompliant(false)]
    public static class ClipboardSW : System.Object
    {

        /// <summary/>
        public static System.Object GetData(System.String format)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return System.Windows.Clipboard.GetData(format);
        }

        /// <summary/>
        public static System.Windows.IDataObject GetDataObject()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return System.Windows.Clipboard.GetDataObject();
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.DataObject</summary>
    [System.CLSCompliant(false)]
    public class DataObjectSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.DataObjectSW Wrap(System.Windows.DataObject o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.DataObjectSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.DataObject InnerObject
        {
            get { return innerObject as System.Windows.DataObject; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal DataObjectSW(System.Windows.DataObject o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal DataObjectSW(System.DayOfWeek day) { }

        /// <summary/>
        public virtual System.Object GetData(System.String format, System.Boolean autoConvert)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetData(format, autoConvert);
        }

        /// <summary/>
        public virtual System.Object GetData(System.String format)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetData(format);
        }

        /// <summary/>
        public virtual System.Object GetData(System.Type format)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return InnerObject.GetData(format);
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.Threading.Dispatcher</summary>
    [System.CLSCompliant(false)]
    public class DispatcherSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.DispatcherSW Wrap(System.Windows.Threading.Dispatcher o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.DispatcherSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Threading.Dispatcher InnerObject
        {
            get { return innerObject as System.Windows.Threading.Dispatcher; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal DispatcherSW(System.Windows.Threading.Dispatcher o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal DispatcherSW(System.DayOfWeek day) { }

        /// <summary/>
        public static Microsoft.Test.Security.Wrappers.DispatcherSW CurrentDispatcher
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return (Microsoft.Test.Security.Wrappers.DispatcherSW)AvalonwrappersSecurityWarehouse.Wrap(System.Windows.Threading.Dispatcher.CurrentDispatcher); }
        }
        /// <summary/>
        public void BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority priority)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.BeginInvokeShutdown(priority);
        }

        /// <summary/>
        public void InvokeShutdown()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            InnerObject.InvokeShutdown();
        }

        /// <summary/>
        public Microsoft.Test.Security.Wrappers.DispatcherHooksSW Hooks
        {
            get { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); return (Microsoft.Test.Security.Wrappers.DispatcherHooksSW)AvalonwrappersSecurityWarehouse.Wrap(InnerObject.Hooks); }
        }
        /// <summary/>
        public static void Run()
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Windows.Threading.Dispatcher.Run();
        }

        /// <summary/>
        public static void PushFrame(System.Windows.Threading.DispatcherFrame frame)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }


    }

    /// <summary>Security Wrapper for Type: System.Windows.Threading.DispatcherHooks</summary>
    [System.CLSCompliant(false)]
    public class DispatcherHooksSW : System.Object
    {

        /// <summary>Wraps an Exsisting instance of an object</summary>
        /// <param name="o">Instance of the object to wrap</param>
        public static Microsoft.Test.Security.Wrappers.DispatcherHooksSW Wrap(System.Windows.Threading.DispatcherHooks o)
        {
            if (o == null)
                return null;
            return new Microsoft.Test.Security.Wrappers.DispatcherHooksSW(o);
        }
        internal object innerObject;

        /// <summary>Gets the wrapped instance</summary>
        public System.Windows.Threading.DispatcherHooks InnerObject
        {
            get { return innerObject as System.Windows.Threading.DispatcherHooks; }
        }

        /// <summary>Creates a new wrapper from an exsisting instance of an object</summary>
        internal DispatcherHooksSW(System.Windows.Threading.DispatcherHooks o)
        {
            this.innerObject = o;
        }

        /// <summary/>
        internal DispatcherHooksSW(System.DayOfWeek day) { }

        /// <summary/>
        public event System.EventHandler DispatcherInactive
        {
            add { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.DispatcherInactive += value; }
            remove { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.DispatcherInactive -= value; }
        }

        /// <summary/>
        public event System.Windows.Threading.DispatcherHookEventHandler OperationPosted
        {
            add { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationPosted += value; }
            remove { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationPosted -= value; }
        }

        /// <summary/>
        public event System.Windows.Threading.DispatcherHookEventHandler OperationCompleted
        {
            add { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationCompleted += value; }
            remove { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationCompleted -= value; }
        }

        /// <summary/>
        public event System.Windows.Threading.DispatcherHookEventHandler OperationPriorityChanged
        {
            add { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationPriorityChanged += value; }
            remove { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationPriorityChanged -= value; }
        }

        /// <summary/>
        public event System.Windows.Threading.DispatcherHookEventHandler OperationAborted
        {
            add { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationAborted += value; }
            remove { new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); InnerObject.OperationAborted -= value; }
        }


    }

}
