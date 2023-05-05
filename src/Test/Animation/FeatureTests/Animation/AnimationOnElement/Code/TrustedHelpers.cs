// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  This file provides a single point of containment for all
//  functions that require special security permissions.
//  This separation is provided so that we improve our security
//  story.
//
// $Id:$ $Change:$
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Remoting;
using System.Reflection;
using System.IO;
using System.Xml;

using System.IO.Packaging;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

using System.Windows.Interop;
//using Microsoft.Test.Internal;

using System.Windows.Automation;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// Single Point of call for all issues that require security
    /// </summary>
    public class TrustedHelper
    {
        /// <summary>
        /// Delegate for logging
        /// </summary>
        public delegate void LogDelegate(string message);

        /// <summary>
        /// Delegate for logging
        /// </summary>
        public static LogDelegate LogFunction
        {
            set { s_logFunction = value; }
        }

        //Delegate used for logging
        private static LogDelegate s_logFunction;

        public static void Log( string message )
        {
            if ( s_logFunction != null )
            {
                s_logFunction( message );
            }
        }

        /// <summary>
        /// Gets command line arguments, or equivalents.
        /// ASSERTS: EnvironmentPermission
        /// </summary>
        public static string[] GetCommandLineArgs()
        {
            // This asserts Environment permissions
            new EnvironmentPermission( PermissionState.Unrestricted ).Assert();
            // Echo requested stuff
            return System.Environment.GetCommandLineArgs();
        }


        #region File/IO utilities

        /// <summary>
        /// Writes to a file.
        /// ASSERTS: FileIOPermission
        /// </summary>
        public static void WriteToFile( string msg, string filePath )
        {
            // This asserts file IO permissions
            new FileIOPermission( PermissionState.Unrestricted ).Assert();
            // Appends to log file
            System.IO.FileStream   stream = new System.IO.FileStream( filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write );
            System.IO.StreamWriter sw = new System.IO.StreamWriter( stream );
            sw.WriteLine( msg );
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Returns Text contents of a file
        /// ASSERTS: FileIOPermission
        /// </summary>
        public static string GetTextFile( string filePath )
        {
            // This asserts file IO permissions

            new FileIOPermission( PermissionState.Unrestricted ).Assert();

            StreamReader    sr = File.OpenText(filePath);
            String          fileText;

            fileText = sr.ReadToEnd();
            sr.Close();
            return fileText;
        }


        /// <summary>
        /// Gets the small name for the assembly of that type.
        /// ASSERTS: FileIOPermission
        /// </summary>
        public static string GetAssemblyName( Type t )
        {
            // This asserts file IO permissions
            new FileIOPermission( PermissionState.Unrestricted ).Assert();
            return t.Assembly.GetName().Name;
        }

        /// <summary>
        /// Gets version for the assembly of that type.
        /// ASSERTS: FileIOPermission
        /// </summary>
        public static string GetAssemblyVersion( Type t )
        {
            // This asserts file IO permissions
            new FileIOPermission( PermissionState.Unrestricted ).Assert();
            return t.Assembly.GetName().Version.ToString();
        }

        /// <summary>
        /// Reads a file from the current application.
        /// ASSERTS: FileIOPermission
        /// </summary>
        public static XmlTextWriter GetXmlTextWriter( string fileName )
        {
            // This asserts file IO permissions
            new FileIOPermission( PermissionState.Unrestricted ).Assert();

            return new XmlTextWriter(fileName,System.Text.Encoding.UTF8);
        }

        #endregion File/IO utilities

        /// <summary>
        /// Sets a property value on an object indirectly.
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static void SetValueForProperty( object target, Type targetType, string property, object propertyValue )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();
            // Sets a property value on an object indirectly
            targetType.GetProperty(property).SetValue(target, propertyValue, null);
        }

        /// <summary>
        /// Sets a property value on an object indirectly.
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static object GetValueForProperty( object target, Type targetType, string property )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();
            // Sets a property value on an object indirectly
            return targetType.GetProperty(property).GetValue(target, null);
        }

        /// <summary>
        /// Gets the current TimeManager from the given context
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static InternalTimeManagerAOE TimeManagerFromContext(Dispatcher dispatcher)
        {
        
            InternalTimeManagerAOE theTimeManager = new InternalTimeManagerAOE(dispatcher);

            // cast and return
            return theTimeManager as InternalTimeManagerAOE;
        }

        /// <summary>
        /// Gets the current TimeManager from the given context
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static object ExecuteStaticMethod( object target, string methodName, object[] args )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();
            Log("[permission granted]");

            // get the type
            Type theType = target.GetType();
            if ( theType == null ) return null;
            Log(String.Format("[type obtained --> {0}]",theType.FullName));

            // get the method
            MethodInfo theMethod = theType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            if ( theMethod == null ) return null;
            Log(String.Format("[method obtained --> {0}]", theMethod.Name ));

            // execute
            return theMethod.Invoke( null, args );
        }

        /// <summary>
        /// Gets the current TimeManager from the given context
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static object ExecuteInstanceMethod( object target, string methodName, object[] args )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();

            // get the type
            Type theType = target.GetType();
            if ( theType == null ) return null;
            Log(String.Format("[type obtained --> {0}]",theType.FullName));

            // get the method
            MethodInfo theMethod = theType.GetMethod( methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase  );
            if ( theMethod == null ) return null;
            Log(String.Format("[method obtained --> {0}]", theMethod.Name ));

            // execute
            return theMethod.Invoke( target, args );
        }

        /// <summary>
        /// Gets the current TimeManager from the given context
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static object GetInstanceProperty( object target, string propertyName )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();

            // get the type
            Type theType = target.GetType();
            if ( theType == null ) return null;
            Log(String.Format("[type obtained --> {0}]",theType.FullName));
            // get the method
            PropertyInfo theProperty = theType.GetProperty( propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase );
            if ( theProperty == null ) return null;
            Log(String.Format("[property obtained --> {0}]",theProperty.Name));

            // execute
            return theProperty.GetValue( target, null );
        }
    
        /// <summary>
        /// Gets the specified attribute for the given type
        /// ASSERTS: ReflectionPermission
        /// </summary>
        public static Attribute GetAttributeForType( Type objectType, Type attributeType )
        {
            // This asserts Reflection permissions
            new ReflectionPermission( PermissionState.Unrestricted).Assert();
            return Attribute.GetCustomAttribute(objectType, attributeType) as Attribute;
        }

        /// <summary>
        /// Sets the current thread's culture
        /// ASSERTS: ControlThread
        /// </summary>
        public static void SetThreadCulture( System.Globalization.CultureInfo culture )
        {
            new SecurityPermission(SecurityPermissionFlag.ControlThread).Assert();
            Thread.CurrentThread.CurrentCulture = culture ;
        }


        #region Mouse and Keyboard Input

        public static void SendMouseInput(double x, double y, int data, MTI.SendMouseInputFlags flags)
        {
            MTI.Input.SendMouseInput( x, y, data, flags );
        }
        public static void SendKeyboardInput(System.Windows.Input.Key key, bool press)
        {
            MTI.Input.SendKeyboardInput( key, press );
        }

        public static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint)
        {
            return Win32.MoveWindow(hWnd, X, Y, nWidth , nHeight , bRepaint);
        }

        public static void Delay(int ms)
        {
            new SecurityPermission(SecurityPermissionFlag.Execution).Assert();
            System.Threading.Thread.Sleep( ms );
        }

        public static IntPtr FindChildWindowFromPoint(IntPtr parentHwnd, int x, int y)
        {
            IntPtr childHwnd = IntPtr.Zero;
            if(parentHwnd != IntPtr.Zero)
            {
                Win32.POINT win32Point = new Win32.POINT(x, y);
                childHwnd = Win32.ChildWindowFromPoint(parentHwnd, win32Point);
            }
            else
            {
                Log("Fail: parentHwnd was null");
            }
            return childHwnd;
        }

        // This finds the top window of avalon content within the frame.
        // This content exists in the window class "Shell DocObject View"
        // On ie 7/Longhorn, this is found in "IEFrame", which in turn is found in the top level hwnd, "BrowserFrameClass"
        // On XP/Server2003, this is found directly in the top level hwnd, which is "IEFrame".
        public static IntPtr FindContentInIEWindow(IntPtr parentHwnd)
        {
            Log("[Trusted Helper] Parent hwnd = " + parentHwnd);
            IntPtr childHwnd = IntPtr.Zero;
            if (parentHwnd != IntPtr.Zero)
            {
                childHwnd = Win32.FindWindowEx(parentHwnd, IntPtr.Zero, "Shell DocObject View", null);
                if (childHwnd == IntPtr.Zero)
                {
                    // inner content not found here, look one more level down
                    IntPtr ieHwnd = Win32.FindWindowEx(parentHwnd, IntPtr.Zero, "IEFrame", null);
                    childHwnd = Win32.FindWindowEx(ieHwnd, IntPtr.Zero, "Shell DocObject View", null);
                    Log("[Trusted Helper] No child ieFrame found - using ie7? Looking down one more level");
                }
            }
            else
            {
                Log("[Trusted Helper] Fail: parentHwnd was null");
            }
            return childHwnd;
        }
        #endregion

        #region UIAutomation-AutomationElement
        public static AutomationElement FindAutomationElement(IntPtr hwnd, PropertyCondition condition)
        {
            //Create the root automation element.
            AutomationElement root = AutomationElement.FromHandle(hwnd);
            //Return the element matching the condition
            return root.FindFirst(TreeScope.Element | TreeScope.Descendants, condition);
        }
        #endregion UIAutomation-AutomationElement
    }

    #region InternalTimeManagerAOE
    public class InternalTimeManagerAOE
    {
        public InternalTimeManagerAOE(Dispatcher dispatcher)
        {
            new ReflectionPermission( PermissionState.Unrestricted).Assert();

            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.Media.Animation.Timeline));
            Type type = assembly.GetType("System.Windows.Media.MediaContext");
            PropertyInfo property = type.GetProperty("CurrentMediaContext", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);
            object mediaContext = property.GetValue(null, null);
            property = type.GetProperty("TimeManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            _timeManager = property.GetValue(mediaContext, null);
            _timeManagerType = assembly.GetType("System.Windows.Media.Animation.TimeManager");

        }


        #region TimeManager Internal Methods
        public void Restart()
        {
            _timeManagerType.InvokeMember("Restart",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Start()
        {
            _timeManagerType.InvokeMember("Start",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Stop()
        {
            _timeManagerType.InvokeMember("Stop",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Pause()
        {
            _timeManagerType.InvokeMember("Pause",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Resume()
        {
            _timeManagerType.InvokeMember("Resume",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Seek(int offset, TimeSeekOrigin origin)
        {
            _timeManagerType.InvokeMember("Seek",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, new object[] {offset, origin});
        }

        #endregion

        public Nullable<TimeSpan> CurrentTime
        {
            get
            {
               return (Nullable<TimeSpan>)_timeManagerType.InvokeMember("CurrentTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.GetProperty,
                            null, _timeManager, null);
            }
        }

        public TimeSpan CurrentGlobalTime
        {
            get
            {
               return (TimeSpan)_timeManagerType.InvokeMember("CurrentGlobalTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.GetProperty,
                            null, _timeManager, null);
            }
        }

        // This property allows the test
        // to change the TimeManager's clock.  Calling either get or set
        // replaces the TimeManager's current clock
        // with a special clock instance that allows us to set and get
        // the current time values at will.
        //
        public TimeSpan ClockCurrentTime
        {
            get
            {
                if (_clock != null)
                {
                    return (TimeSpan)_clockType.InvokeMember("CurrentTime",
                                BindingFlags.Public | BindingFlags.NonPublic |
                                BindingFlags.Instance | BindingFlags.GetProperty,
                                null, _clock, null);
                }
                else
                {
                    throw new InvalidOperationException("You must call SetClock before calling this property");
                }

            }
            set
            {
                if (_clock != null)
                {          
                
                
                    _clockType.InvokeMember("CurrentTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.SetProperty,
                            null, _clock, new object[] { value });
                }
                else
                {
                   throw new InvalidOperationException("You must call SetClock before calling this property");
                }
            }
        }
        
        public void RestoreClock()
        {
            if (_oldTMClock != null)
            {
                _clock = null;

                //
                // Restore the old clock
                //
                 _timeManagerType.InvokeMember("Clock",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.SetProperty,
                            null, _timeManager, new Object[] { _oldTMClock });
            }
        }

        /// <summary>
        /// Creates a class that implements System.Windows.Media.IClock.  That interface
        /// is internal to PresentationCore so this class has to be dynamically generated 
        /// at runtime.  An instance of this class (CustomClock) is then passed into the
        /// TimeManager, which uses it as its clock.  This allows us to muck with the TimeManager's
        /// time by setting CurrentTime
        /// </summary>
        public void SetClock()
        {
            if (_clock == null)
            {
                //
                // Save off the old clock
                //
                _oldTMClock = _timeManagerType.InvokeMember("Clock",
                                    BindingFlags.DeclaredOnly |
                                    BindingFlags.Public | BindingFlags.NonPublic |
                                    BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, _timeManager, null);

                //
                // look up System.Windows.Media.TimeManager.TestTimingClock
                //
                _clockType = (Type)_timeManagerType.GetNestedType("TestTimingClock", BindingFlags.NonPublic);


                //
                // create an instance of TestTimingClock and save it
                //
                _clock = Activator.CreateInstance(_clockType);

                //
                // Pass that instance into the TimeManager
                //

                _timeManagerType.InvokeMember("Clock",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.SetProperty,
                            null, _timeManager, new Object[] { _clock });
            }
        }
        
      
     
        private object _timeManager;
        private Type _timeManagerType;

        // this will actually point to a dynamically-generated derived type
        // that derives from System.Windows.Media.TimingClock
        // this clock replaces the TimeManager's default clock
        private object _clock = null;
        
        // this stores the TimeManager's old clock so that it can 
        // be restored
        private object _oldTMClock = null; 
        private Type _clockType;
    }
    #endregion InternalTimeManagerAOE
}
