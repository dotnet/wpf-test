// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using Microsoft.Test.CrossProcess;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// Xaml Browser Host
    /// </summary>
    public class XamlBrowserHost : IDisposable
    {
        #region Private Data

        ApplicationMonitor monitor = null;
        IntPtr hWndHost = IntPtr.Zero;
        UiaDistributedTestcaseHost remoteHost = null;
        string xbapFileName;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new XamlBrowserHost using default host xbap
        /// </summary>
        public XamlBrowserHost() : this("XamlBrowserHost.xbap")
        {
        }

        /// <summary>
        /// Creates a new XamlBrowserHost using specified host xbap
        /// </summary>
        /// <remarks>
        /// The host xbap must create a HostController object
        /// </remarks>
        public XamlBrowserHost(string xbapFileName)
        {
            if (string.IsNullOrEmpty(xbapFileName))
                throw new ArgumentNullException("xbapFileName");

            //

            //if under piper ensure that we launch the xbap from the work dir, otherwise, just use the current dir
            //
            if (DriverState.DriverParameters["workdir"] != null)
                xbapFileName = Path.Combine(Environment.ExpandEnvironmentVariables(DriverState.DriverParameters["workdir"]), xbapFileName);
            
            this.xbapFileName = Path.GetFullPath(xbapFileName);

            //make sure that the xbap exists
            if (!File.Exists(this.xbapFileName))
                throw new FileNotFoundException("The specified xbap could not be found", this.xbapFileName);
        }

        #endregion


        #region Public Members

        /// <summary>
        /// Starts the Host application and returns the hwnd to the IE application window
        /// </summary>
        /// <returns>the hwnd to the IE application window</returns>
        /// <exception cref="TimeoutException">Thrown if the host times-out waiting for the default xaml page to navigate</exception>
        public void StartHost()
        {
            StartHost(null);
        }

        /// <summary>
        /// Start Host
        /// </summary>
        /// <param name="startupUri"></param>
        public void StartHost(string startupUri)
        {
            if (monitor != null)
                throw new InvalidOperationException("You must first Close the Host before calling StartHost again.");

            DictionaryStore.StartServer();
            //use a custom startup page
            if (!string.IsNullOrEmpty(startupUri))
            {
                DictionaryStore.Current[UiaDistributedTestcaseHost.StartupUriId] = startupUri;
            }

            monitor = new ApplicationMonitor();
            GetHwndUIHandler handler = new GetHwndUIHandler();
            
            // register for iexplore and have your app set the title
            monitor.RegisterUIHandler(handler, "iexplore", "RegExp:(Ready)", UIHandlerNotification.TitleChanged);

            //Clear the Click Once cache so the app is always re-activated
            ApplicationDeploymentHelper.CleanClickOnceCache();

            //run the app
            monitor.StartProcess(xbapFileName);

            //wait for the UIHandler to return abort or timeout in 90 seconds
            if (!monitor.WaitForUIHandlerAbort(90000) || handler.topLevelhWnd == IntPtr.Zero)
                throw new TimeoutException("A timeout occured while waiting for the XamlBrowserHost to navigate to the startUpPage");

            //

            monitor.StopMonitoring();

            //Get the remoteHost object
            //

            
            //if we timedout then let the caller know that the app is not hosting this object
            //if (remoteHost == null)
            //    throw new InvalidOperationException("The launched application did not create a host object in the Harness remote site");

            //set the host hwnd
            hWndHost = handler.topLevelhWnd;
        }

        /// <summary>
        /// Gets the window handle of the host application (Internet Explorer)
        /// </summary>
        public IntPtr HostWindowHandle
        {
            get { return hWndHost; }
        }

        /// <summary>
        /// Navigate the host application to a xaml file
        /// </summary>
        /// <param name="xamlFileName">name of the xamlfile</param>
        /// <param name="looseXaml">true if the file is not compiled in the app otherwise false</param>
        /// <remarks>
        /// if the file is a loose xaml file then it must be in the applications site of origion
        /// </remarks>
        public void Navigate(string xamlFileName, bool looseXaml)
        {
            if (string.IsNullOrEmpty(xamlFileName))
                throw new ArgumentNullException("xamlFileName");

            EnsureState();

            if (looseXaml)
            {
                //Make the relative xaml file path relative to the site of origin
                if (!Path.IsPathRooted(xamlFileName))
                    xamlFileName = Path.Combine(Path.GetDirectoryName(xbapFileName), xamlFileName);
                if (!File.Exists(xamlFileName))
                    throw new FileNotFoundException("The specified loose xaml file could not be found", xamlFileName);
            }

            //tell the remote app to navigate
            remoteHost.Navigate(new Uri(xamlFileName, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Gets the host application base directory
        /// </summary>
        /// <returns></returns>
        public string GetHostApplicationBaseDirectory()
        {
            EnsureState();
            return remoteHost.GetApplicationBaseDirectory();
        }

        /// <summary>
        /// Run Testcase
        /// </summary>
        /// <param name="testcase"></param>
        /// <param name="targetControlName"></param>
        public void RunTestcase(UiaDistributedTestcase testcase, string targetControlName)
        {
            EnsureState();

            //get the top window element
            AutomationElement topWindowElement = AutomationElement.FromHandle(hWndHost);

            //get the UIElement
            AutomationElement clientElement = topWindowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, targetControlName));

#if TESTBUILD_CLR40

            //Since we could not find the element we were looking for, there is a chance it is virtualized
            //This only applies to .NET 4.0 and above assuming UIACore 7 is also present.
            if ((clientElement == null) && (AutomationElement.IsItemContainerPatternAvailableProperty != null))
            {
                //here we will find all elements that are item containers and therefore might contain virtualized items
                AutomationElementCollection elementCollection = topWindowElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsItemContainerPatternAvailableProperty, true));
                for (int i = 0; i < elementCollection.Count; i++)
                {
                    
                    ItemContainerPattern itemContainerPattern = elementCollection[i].GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;
                    // look for the item we are searching for in each container
                    clientElement = itemContainerPattern.FindItemByProperty(null, AutomationElement.AutomationIdProperty, targetControlName);
                    if (clientElement != null)
                    {
                        break;
                    }
                }
            }

#endif
            
            //perform the test
            testcase.RunTest(remoteHost, clientElement, targetControlName);
        }

        /// <summary>
        /// Capture the window region of the specified size
        /// </summary>
        public Microsoft.Test.RenderingVerification.ImageAdapter CaptureWindow(double targetWidth, double targetHeight)
        {
            //Reposition Mouse to the origin
            Input.Input.MoveTo(new System.Windows.Point(0, 0));

            //Get window, move it to the origin
            AutomationElement windowElement = AutomationElement.FromHandle(hWndHost);
            WindowPattern window = windowElement.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            window.SetWindowVisualState(WindowVisualState.Normal);
            TransformPattern transform = windowElement.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;
            transform.Move(0, 0);

            //we need to size the window (IE) to get the client area the correct size
            //Get the size of the target window
            AutomationElement clientElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "RootBrowserWindow"));
            Rect clientRect = (Rect)clientElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            //Determine what the new size of the window should be to get the client area the correct size
            System.Windows.Rect ierect = (System.Windows.Rect)windowElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            double w = (double)(ierect.Width + targetWidth - clientRect.Width);
            double h = (double)(ierect.Height + targetHeight - clientRect.Height);
            transform.Resize(w, h);

            // Wait for document to load and resize to occurs (assuming render would have occured)
            Thread.Sleep(3000);

            //Get bounds of content region, convert to Rectangle form, and capture to image adapter
            Rect ContentRect = (Rect)clientElement.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            System.Drawing.Rectangle captureRect = new Rectangle((int)ContentRect.X, (int)ContentRect.Y, (int)ContentRect.Width, (int)ContentRect.Height);
            return new Microsoft.Test.RenderingVerification.ImageAdapter(captureRect);
        }

 
        /// <summary>
        /// Closes the XamlBrowserHost application
        /// </summary>
        public void Close()
        {
            if (monitor != null)
            {
                //remove the UiaDistributedTestcaseHost from the remote site because we are about to close the host process
                //

                remoteHost = null;

                //Close the host process
                monitor.Close();
                monitor = null;
                hWndHost = IntPtr.Zero;

                //give some time for the app to shutdown
                Thread.Sleep(2000);
            }
        }

        #endregion


        #region Private Members

        private void EnsureState()
        {
            if (monitor == null || hWndHost == IntPtr.Zero)
                throw new InvalidOperationException("You must call StartHost before using this API");
        }

        private class GetHwndUIHandler : UIHandler
        {
            public IntPtr topLevelhWnd = IntPtr.Zero;

            public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
            {
                this.topLevelhWnd = topLevelhWnd;
                return UIHandlerAction.Abort;
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

    }
}
