// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:   
     This class is intended to be a TestContainer that only uses Core features.
     There is a Framework that inherits from this and extends the functionality.
     This TestContainer doesn't provide any compilation hosting scenarios.
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Collections.Generic;
using Avalon.Test.CoreUI.Threading;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// This class is intended to be a TestContainer that only uses Core features.
    /// There is a Framework that inherits from this and extends the functionality.
    /// This TestContainer doesn't provide any compilation hosting scenarios.
    /// </summary>
    public class WinformsTestContainer : ITestContainer
    {
        /// <summary>
        /// </summary>
        public WinformsTestContainer()
        {

        }

        /// <summary>
        /// </summary>
        public HostType HostType
        {
            get
            {
                return _hostType;
            }
            set
            {
                _hostType = value;
            }
        }        

        /// <summary>
        /// Displays the specifed object on a SurfaceCore
        /// </summary>
        public virtual Surface DisplayObject(object visual, int x, int y, int w, int h)
        {
            SurfaceFramework surface = new SurfaceFramework(HostType.ToString(), x, y, w, h);

            surface.DisplayObject(visual);

            _surfaces.Add(surface);

            return surface;
        }

        /// <summary>
        /// Go back to object most recently navigated from.
        /// </summary>
        /// <remarks>
        /// Similar to browser Back button.
        /// </remarks>
        public void GoBack()
        {
            if (_surfaces.Count > 0)
            {
                GoBackCore();
            }
        }

        /// <summary>
        /// Go forward to object most recently navigated back from.
        /// </summary>
        /// <remarks>
        /// Similar to browser Forward button.
        /// </remarks>
        public void GoForward()
        {
            if (_surfaces.Count > 0)
            {
                GoForwardCore();
            }
        }

        /// <summary>
        /// Implement core GoBack functionality for this stub.
        /// </summary>
        protected virtual void GoBackCore()
        {
            _surfaces[0].GoBack();
        }

        /// <summary>
        /// Implement core GoForward functionality for this stub.
        /// </summary>
        protected virtual void GoForwardCore()
        {
            _surfaces[0].GoForward();
        }

        /// <summary>
        /// Displays the specifed object on a modal SurfaceCore
        /// </summary>
        public virtual void DisplayObjectModal(object visual, int x, int y, int w, int h)
        {
            SurfaceFramework surface = new SurfaceFramework(HostType.ToString(), x, y, w, h);

            ModalStack.Push(surface);

            surface.DisplayObject(visual);
            surface.ShowModal();

        }

        /// <summary>
        /// Navigate to object on an application.
        /// </summary>
        /// <param name="visual">A visual object</param>
        /// <returns>true if the navigation succeeds, false otherwise.</returns>
        public bool NavigateToObject(object visual)
        {
            if (_surfaces.Count == 0)
            {
                throw new ArgumentException("Object must be displayed in an existing surface if you want to navigate.");
            }

            if (_surfaces.Count > 0)
            {
                _surfaces[0].DisplayObject(visual);
            }

            return true;
        }

        /// <summary>
        /// We try to start an AvalonDispatcher
        /// </summary>
        public bool RequestStartDispatcher()
        {
            _currentDispatcher = Dispatcher.CurrentDispatcher;
            _currentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(CommonDispatcherExceptionHandler);

            Microsoft.Test.WindowsForms.Application.Run();

            return true;
        }


        /// <summary>
        /// Entry point to execute a IHostedTest with a specified method name
        /// </summary>
        public bool Run(string typeName, string methodName, string assemblyName)
        {
            Assembly asm = null;

            Type type = null;

            if (assemblyName == CoreTests.CoreTestsDefaultAssemblies)
            {
                type = Utility.FindType(typeName, true);
            }
            else
            {
                // Try to load item in list as an assembly.
                if (File.Exists(assemblyName))
                {
                    GlobalLog.LogStatus("Checking " + assemblyName + "...");
                    asm = Assembly.LoadFrom(assemblyName);
                }
                else
                {
                    GlobalLog.LogStatus(assemblyName + " not found.");

                }

                type = asm.GetType(typeName);
            }

            if (type == null)
            {
                throw new InvalidOperationException("The type cannot be found.");
            }



            object o = Activator.CreateInstance(type);

            if (!(o is IHostedTest))
            {
                throw new Microsoft.Test.TestSetupException("The type '" + typeName + "' doesn't implement IHostedTest.");
            }

            Run((IHostedTest)o, methodName);

            return true;
        }


        /// <summary>
        /// Entry point to execute a IHostedTest with a specified method name
        /// </summary>
        public void Run(IHostedTest testCase, string methodName)
        {
            // Setting up relationshipt Container - Host
            testCase.TestContainer = this;
            CurrentHostedTest = testCase;


            // Executing the method that was specified

            Type testCaseType = testCase.GetType();
            testCaseType.InvokeMember(methodName,
                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null,
                testCase,
                null,
                System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Stops the dispatcher posting an item to background
        /// </summary>
        public void EndTest()
        {
            DispatcherHelper.EnqueueBackgroundCallback(
                (DispatcherOperationCallback)delegate 
                {
                    Microsoft.Test.WindowsForms.Application.Exit();
                    return null;
                }, null);
        }


        /// <summary>
        /// Stops the dispatcher Sync
        /// </summary>
        public void EndTestNow()
        {
            Microsoft.Test.WindowsForms.Application.Exit();
        }

        /// <summary>
        /// Returns library of current surfaces (modal and modeless).
        /// </summary>
        public Surface[] CurrentSurface
        {
            get
            {
                return _surfaces.ToArray();
            }
        }
        private List<Surface> _surfaces = new List<Surface>();

        /// <summary>
        /// Add surface to library of current surfaces.
        /// </summary>
        /// <param name="s">Surface to add.</param>
        public virtual void AddSurface(Surface s)
        {
            _surfaces.Add(s);
        }

        /// <summary>
        /// </summary>
        public IHostedTest CurrentHostedTest
        {
            get
            {
                return _currentHostedTest;
            }
            set
            {
                _currentHostedTest = value;
            }

        }

        /// <summary>
        /// Close the last modal window
        /// </summary>
        public virtual void CloseLastModal() 
        { 
            if (ModalStack.Count > 0)
            {
                Surface surface = ModalStack.Pop();
                surface.Close();
            }
        }

        /// <summary>
        /// Returns all the modal surfaces created on this ITestContainer. The last surface created is the position 0 on the array
        /// </summary>
        public Surface[] CurrentModalSurfaces
        {
            get
            {
                lock (ModalStack)
                {
                    return ModalStack.ToArray();
                }
            }
        }

        /// <summary>
        /// Dictates whether or not unhandled dispatcher exceptions should be logged 
        /// automatically as failures.
        /// </summary>
        public bool ShouldLogUnhandledException
        {
            get
            {
                return _shouldLogUnhandledException;
            }
            set
            {
                _shouldLogUnhandledException = value;
            }
        }

        /// <summary>
        /// Raised when an unhandled exception occurs.
        /// </summary>
        public event EventHandler ExceptionThrown;

        private void CommonDispatcherExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (_shouldLogUnhandledException)
            {
                CoreLogger.LogTestResult(false, "Unhandled exception occurred in dispatcher.\r\n" + e.Exception.ToString());
                e.Handled = true;
                this.EndTestNow();
            }
            else
            {
                OnExceptionThrown(e.Exception);
            }
        }

        private void OnExceptionThrown(Exception e)
        {
            if (ExceptionThrown != null)
            {
                ExceptionThrown(e, EventArgs.Empty);
            }
        }

        /// <summary>
        /// </summary>
        protected System.Collections.Generic.Stack<Surface> ModalStack = new System.Collections.Generic.Stack<Surface>();

        HostType _hostType;
        bool _shouldLogUnhandledException = true;
        private Dispatcher _currentDispatcher = null;
        private IHostedTest _currentHostedTest = null;
    }
}


