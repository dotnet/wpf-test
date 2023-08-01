// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:   
     This class is intended to be a TestContainer that only uses Core features.
     There is a Framework that inherits from this and extends the functionality.
     This TestContainer doesn't provide any compilation hosting scenarios.
*    
 
  
*    Revision:         $Revision: 1 $
 
*    Filename:         $Source: //depot/devdiv/private/Orcas/pu/WPF/WPFMQMQ/ElementServices/FeatureTests/Untrusted/Common/ExeStubContainer.cs $
******************************************************************************/
using System;
using System.Windows;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Collections.Generic;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Integration;
using Microsoft.Test.Integration.Windows;
using Microsoft.Test.Loaders;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// </summary>
    public class StubTestContainer : ITestContainer
    {
        /// <summary>
        /// </summary>
        public static void CreateTestContainer(ContentItem contentItem)
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), (string)contentItem.Content);

            Controller controller = (Controller)CommonStorage.Current.Get("Controller");

            ITestContainer container = new StubTestContainer(hostType, controller);

            CommonStorage.Current.Store("TestContainer", container);
        }

        /// <summary>
        /// </summary>
        private StubTestContainer(HostType hostType, Controller controller)
        {
            if (hostType == HostType.Browser && (Application.Current == null || Application.Current.MainWindow == null))
            {
                throw new Microsoft.Test.TestSetupException("Application.Current.MainWindow cannot be null when HostType is '" + HostType.Browser.ToString() + "'.");
            }

            _hostType = hostType;
            _controller = controller;

            _shouldLogUnhandledException = false;

            _currentDispatcher = Dispatcher.CurrentDispatcher;
            _currentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(CommonDispatcherExceptionHandler);
        }
        /// <summary>
        /// </summary>
        public Surface DisplayObject(object visual, int x, int y, int w, int h)
        {
            SurfaceFramework surface = null;

            if (Avalon.Test.CoreUI.Common.HostType.Browser != HostType)
            {
                TestLog.Current.LogStatus("Creating '" + HostType.ToString() + "' surface...");
                surface = new SurfaceFramework(
                    HostType.ToString(),
                    x,
                    y,
                    w,
                    h);

                surface.DisplayObject(visual);
            }
            else
            {

                if (_surfaces.Count == 0)
                {
                    TestLog.Current.LogStatus("Creating 'Browser' surface...");
                    
                    // On this case this is a Browser hosted so we can get the main window
                    surface = new SurfaceFramework(Application.Current.MainWindow);

                    surface.DisplayObject(visual);
                    surface.SetPosition(x, y);
                    surface.SetSize(w, h);
                }
                else
                {
                    TestLog.Current.LogStatus("Creating 'Window' surface...");

                    surface = new SurfaceFramework(
                        Avalon.Test.CoreUI.Common.HostType.Window.ToString(),
                        x,
                        y,
                        w,
                        h);

                    surface.DisplayObject(visual);

                }
            }

            if (surface != null)
            {
                _surfaces.Add(surface);
            }

            return (Surface)surface;
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
            SurfaceFramework surface = new SurfaceFramework("Window", x, y, w, h, false);

            lock (ModalStack)
            {
                ModalStack.Push(surface);
            }

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
            return true;
        }

        /// <summary>
        /// Stops the dispatcher posting an item to background
        /// </summary>
        public void EndTest()
        {
            _controller.EndTest();

            //// We have to do this due 








        }


        /// <summary>
        /// Stops the dispatcher Sync
        /// </summary>
        public void EndTestNow()
        {
            _controller.EndTest();

            //// We have to do this due 








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
        public void CloseLastModal()
        {
            Surface modalWindow = null;

            lock (ModalStack)
            {
                if (ModalStack.Count > 0)
                {
                    modalWindow = ModalStack.Pop();
                }
            }

            if (modalWindow != null)
            {
                modalWindow.Close();
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

        bool _shouldLogUnhandledException = true;
        private Dispatcher _currentDispatcher = null;
        private IHostedTest _currentHostedTest = null;
        private HostType _hostType = HostType.Browser;
        private Controller _controller = null;
    }
}

