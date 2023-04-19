// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Test.Logging;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Class to interact with IE webbrowser object 
    /// </summary>
    public class WebBrowserInteropProxy
    {
        private object _ieObject = null;        // pointer to the IE browser
        private Type _typeIEObject = Type.GetTypeFromProgID("InternetExplorer.Application");

        // COM binding flags
        private static BindingFlags s_COMCallSetProperty = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
        private static BindingFlags s_COMCallGetProperty = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
        private static BindingFlags s_COMCallMethod = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;

        /// <summary>
        /// Default Constructor
        /// Create a new Browser window
        /// </summary>
        public WebBrowserInteropProxy()
        {
            try
            {
                _typeIEObject = Type.GetTypeFromProgID("InternetExplorer.Application");
                _ieObject = Activator.CreateInstance(_typeIEObject);
            }
            catch (Exception e)
            {
                NavigationHelper.Output("Exception caught in WebBrowserInteropProxy() " + e.Message);
            }

            if (_ieObject == null)
            {
                throw new ApplicationException("Was unable to create IE object");
            }

            SetProperty("Visible", true);
            WaitWhileBusy();
        }

        /// <summary>
        /// Gets web browser proxy given a Window handle
        /// Used in attaching to an existing IE window
        /// </summary>
        public WebBrowserInteropProxy(IntPtr hWnd)
        {
            object shellWindows = null;
            Type typeShellWindows = Type.GetTypeFromProgID("Shell.Application");
            try
            {
                shellWindows = Activator.CreateInstance(typeShellWindows);
            }
            catch (Exception e)
            {
                NavigationHelper.Output("Exception caught in Activator.CreateInstance(typeShellWindows) " + e.Message);
                NavigationHelper.Output(e.InnerException.Message);
                NavigationHelper.Output(e.GetType().ToString());
            }

            if (shellWindows == null)
            {
                NavigationHelper.Fail("Could not create Shell Windows object.");
            }

            int countShellWindows = 0;
            object objWindows = null;
                            
            try
            {
                objWindows = typeShellWindows.InvokeMember("Windows", s_COMCallMethod, null, shellWindows, null);
                countShellWindows = (int)typeShellWindows.InvokeMember("Count", s_COMCallGetProperty, null, objWindows, null);
            }
            catch (Exception e)
            {
                NavigationHelper.Output("Exception caught in typeShellWindows.InvokeMember " + e.Message);
                NavigationHelper.Output(e.InnerException.Message);
                NavigationHelper.Output(e.GetType().ToString());

                // this issue we could not get resolved and shows up on XP
                // it can be fixed if you set the reg key 
                // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\Windows Presentation Foundation\Hosting\rununrestricted to 1
                // this doesn't show up on Vista
                if (e is System.Reflection.TargetInvocationException)
                {
                    // return for XP to enable the test case that uses only WaitWhileBusy() method
                    return;
                }
            }

            if (objWindows == null)
            {
                NavigationHelper.Fail("Could not create Windows object.");
            }

            object objItem;
            Type typeItem;

            // Go through the collection and find the one that matches with given hWnd
            for (int i = 0; i < countShellWindows; i++)
            {
                objItem = typeShellWindows.InvokeMember("Item", s_COMCallMethod, null, objWindows, new object[] { i });
                if (objItem != null)
                {
                    typeItem = objItem.GetType();
                    IntPtr hwndPtr = GetHwnd(objItem, typeItem);
                    if (hwndPtr == hWnd)
                    {
                        _ieObject = objItem;
                        _typeIEObject = typeItem;
                        break;
                    }
                }
            }

            if (_ieObject == null)
            {
                NavigationHelper.Fail("Could not retrieve the browser window handle");
            }
        }

        /// <summary>
        /// Return the browser window handle
        /// </summary>
        /// <returns></returns>
        public IntPtr GetHwnd()
        {
            return GetHwnd(_ieObject, _typeIEObject);
        }

        /// <summary>
        /// Wait while IE window is busy
        /// </summary>
        public void WaitWhileBusy()
        {
            if (_ieObject != null)
            {
                while ((bool)GetProperty("Busy"))
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else
            {
                // For XP WaitWhile is just wait :)
                System.Threading.Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// returns a .NET object representing IE COM object
        /// </summary>
        public object InnerObject
        {
            get
            {
                return _ieObject;
            }
        }

        /// <summary>
        /// Navigates the entire IE window to a Path or Url. Frame navigation not supported by this method
        /// </summary>
        /// <param name="pathOrUri"></param>
        public void NavigateAndWait(string pathOrUri)
        {
            InvokeMethodAllByVal("Navigate2", new object[] { pathOrUri, null, null, null, null });
            WaitWhileBusy();
        }

        /// <summary>
        /// Gets a web browser property given a property name
        /// </summary>
        public object GetProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName", "Name of requested property to get on Web Browser is null or empty");
            }

            return _typeIEObject.InvokeMember(propertyName, s_COMCallGetProperty, null, _ieObject, null);
        }

        /// <summary>
        /// Sets a web browser property
        /// </summary>
        public void SetProperty(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName", "Name of requested property to set on Web Browser is null or empty");
            }
            _typeIEObject.InvokeMember(propertyName, s_COMCallSetProperty, null, _ieObject, new object[] { value });
        }

        /// <summary>
        /// Invokes a WB method with value params
        /// </summary>
        public object InvokeMethodAllByVal(string name, object[] args)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name of method to call on Web Browser is null or empty");
            }

            return _typeIEObject.InvokeMember(name, s_COMCallMethod, null, _ieObject, args);
        }

        /// <summary>
        /// Retrieve the window handle for the given object and object type
        /// </summary>
        /// <param name="objItem"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        private static IntPtr GetHwnd(Object objItem, Type objType)
        {
            // if we're 64-bit, or at least the OS is 64-bit (xbap on Vista)
            if (SystemInformation.Current.IsWow64Process || (IntPtr.Size == 8))
            {
                // Some of the time, even on 64-bit, an object apparently returns a 32-bit result.
                // If we get an InvalidCastException, just fall through and try 32-bit.
                try
                {
                    Int64 currentHwnd64 = (Int64)objType.InvokeMember("HWND", s_COMCallGetProperty, null, objItem, null);
                    return new IntPtr(currentHwnd64);
                }
                catch (InvalidCastException) {}
            }

            // we're 32-bit or the object is 32-bit
            int currentHwnd = (int)objType.InvokeMember("HWND", s_COMCallGetProperty, null, objItem, null);
            return new IntPtr(currentHwnd);
        }
    }
}
