// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Deployment;
using System.Windows.Automation;
//using UIA = System.Windows.Automation;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.CrossProcess;


namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Different methods of browser Navigation
    /// </summary>
	public enum BrowserNavigationMethod
    {
		/// <summary>
		/// Navigate using the browser's Back button
		/// </summary>
		BackButton,
		/// <summary>
		/// Navigate using the browser's Next button
		/// </summary>
		NextButton,
		/// <summary>
		/// Refresh Content to cause reload
		/// </summary>
		RefreshButton,
		/// <summary>
		/// Navigate using the navigation bar
		/// </summary>
		IENavBar
    }

    /// <summary>
    /// Loader Step that can be used to navigate between different Avalon application types
    /// if Method is "Launch", it opens an IE window and navigates using it.
    /// if Method is "Navigate", it looks for an existing IE window and navigates using it.
    /// </summary>
    
    public class BrowserNavigationStep : LoaderStep 
    {
        #region private data

        private FileHost _fileHost;
		private bool _usingSeparateFileHost = false;
        private ApplicationMonitor _appMonitor;
        private UIHandler[] _uiHandlers = new UIHandler[0];

		private int _registeredHandlers = 0;

		//Valid at least for XPSP2
		private static String s_buttonBackID = "Back";
		private static String s_buttonNextID = "Next";
		private static String s_buttonRefreshID = "Refresh";
		/*
		private static String buttonStopID = "Item 1016";		
		private static String buttonHomeID = "Item 1018";
		private static String buttonSearchID = "Item 1019";
		private static String buttonFavoritesID = "Item 1020";
		private static String buttonHistoryID = "Item 1021";
		private static String buttonMailID = "Item 369";
		private static String buttonPrintID = "Item 260"*/
		

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for Navigation step.
        /// Puts value into property bag to prevent AppVerifier from closing windows in navigation steps.
        /// </summary>
		public BrowserNavigationStep()
        {
            DictionaryStore.Current["NavigationStepRunning"] = "true";
        }
        #endregion
        
        #region Public members

        /// <summary>
        /// Gets or sets the File that should be navigated to
        /// </summary>
        public string FileName = null;

        /// <summary>
        /// Gets or sets method to use to open file.  Navigate by default,
        /// should only be changed for the first step in a chain of NavigateSteps
        /// </summary>
        public ActivationMethod Method = ActivationMethod.Navigate;

        /// <summary>
        /// Gets or sets method to use for navigation, be it clicking links in one app to another
        /// or typing into the IE nav. bar.
        /// </summary>
        public BrowserNavigationMethod NavigationType = BrowserNavigationMethod.BackButton;

        /// <summary>
        /// Gets or sets the Scheme to use when activating the application
        /// </summary>
        public ActivationScheme Scheme = ActivationScheme.Local;
        
        /// <summary>
        /// Gets or sets an array of SupportFiles that should be remotely hosted
        /// </summary>
        public SupportFile[] SupportFiles = new SupportFile[0];
        
        /// <summary>
        /// gets or sets an array of UIHandlers that will be registered with Activation Host
        /// </summary>
        public UIHandler[] UIHandlers 
        {
            get { return _uiHandlers; }
            set 
            {
                foreach (UIHandler handler in value)
                    handler.Step = this;
                _uiHandlers = value;
            }
        }

        /// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor 
        {
            get { return _appMonitor; }
        }

        /// <summary>
        /// Gets or sets a boolean value representing if this is the last navigation step to run.
        /// The last navigation step in an indefinite list of these steps needs to close the final 
        /// window, thus must know this.
        /// </summary>
        public bool IsFinalStep = false;

        #endregion

        #region Step Implementation

        /// <summary>
        /// Performs the Navigation step
        /// </summary>
        /// <returns>returns true if the rest of the steps should be executed, otherwise, false</returns>
        protected override bool BeginStep() 
        {
			bool beginSucceeded = false;

            GlobalLog.LogDebug("Starting navigation step to : " + FileName);
            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            UploadSupportFiles();

            RegisterUIHandlers();

            string param = FileName;
			

			//Detects the file scheme?
            if (((Scheme != ActivationScheme.Local)&&(SupportFiles.Length > 0)) || (_usingSeparateFileHost && (!param.StartsWith("about:", true, System.Globalization.CultureInfo.InvariantCulture))))
            {
                FileHostUriScheme hostScheme = (FileHostUriScheme)Enum.Parse(typeof(FileHostUriScheme), Scheme.ToString());
                param = _fileHost.GetUri(FileName, hostScheme).ToString();
            }

            // if this is the last step in the navigation sequence, or a standalone app,
            // get rid of prop bag value so that appverifier will close the window.
            if (IsFinalStep)
            {
                DictionaryStore.Current["NavigationStepRunning"] = null;
            }

            //if local... need to fully qualify the path
            if ((Scheme == ActivationScheme.Local) && (!param.StartsWith("about:", true, System.Globalization.CultureInfo.InvariantCulture)))
                param = Path.GetFullPath(param);

            // Launch IE if we need to
            if (Method == ActivationMethod.Launch)
            {
                //appMonitor.StartProcess(param);
                // Process.Start("IEXPLORE", "-nohome");
                Process.Start("IEXPLORE", "about:Nothing");
                // Let IE load up...
                Thread.Sleep(3000);
                // Start monitoring for registered UIHandlers
                _appMonitor.StartMonitoring(false);
            }

            // Start monitoring for registered UIHandlers
            _appMonitor.StartMonitoring(false);

			switch (NavigationType)
            {
				case (BrowserNavigationMethod.BackButton):
					beginSucceeded = NavigateButtonIE(IntPtr.Zero, s_buttonBackID);
                    break;
				case (BrowserNavigationMethod.NextButton):
					beginSucceeded = NavigateButtonIE(IntPtr.Zero, s_buttonNextID);
                    break;
				case (BrowserNavigationMethod.RefreshButton):
					System.Threading.Thread.Sleep(10000);
					beginSucceeded = NavigateButtonIE(IntPtr.Zero, s_buttonRefreshID);
					System.Threading.Thread.Sleep(10000);
					break;
				case (BrowserNavigationMethod.IENavBar):
					beginSucceeded = NavigateUsingIEBar(param, IntPtr.Zero);
					break;
            }
			return beginSucceeded;
        }

        /// <summary>
        /// Waits for the ApplicationMonitor to Abort and Closes any remaining
        /// processes
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep() {
            //Wait for the application to be done			

			//We need at least one handler to abort
			if (_registeredHandlers > 0)
			{
				GlobalLog.LogEvidence("BrowserNavigationStep EndStep - Waiting for All Dialogs (Reged: " + _registeredHandlers + ")");
				_appMonitor.WaitForUIHandlerAbort();
			}
			else
			{
				GlobalLog.LogEvidence("BrowserNavigationStep EndStep - No custom reged handlers");
				System.Threading.Thread.Sleep(3000);
			}

            if (IsFinalStep)
            {
                //killAllIEWindows();
                _appMonitor.Close();

				// close the fileHost if one was created within ActivationStep. 
				// Don't close if the filehost is in the context of a FileHostStep
				if ((_fileHost != null) && (SupportFiles.Length > 0))
					_fileHost.Close();
            }
            else
            {
				_appMonitor.StopMonitoring();
            }

			GlobalLog.LogEvidence("BrowserNavigationStep EndStep - Done");

            return true;
        }
        #endregion

		#region Private Methods

        private void killAllIEWindows()
        {
            GlobalLog.LogDebug("Killing IE windows (navigation ending with standalone app)");
            Process[] procList = Process.GetProcesses();
            foreach (Process p in procList)
            {
                if (p.ProcessName.ToLowerInvariant().Equals("iexplore"))
                {
                    p.CloseMainWindow();
                }
            }
        }

        private void UploadSupportFiles()
        {
            if (Scheme != ActivationScheme.Local)
            {
                if (SupportFiles.Length > 0)
                {
                    _fileHost = new FileHost();
                    foreach (SupportFile suppFile in SupportFiles)
                    {
                        if (suppFile.IncludeDependencies)
                            _fileHost.UploadFileWithDependencies(suppFile.Name);
                        else
                            _fileHost.UploadFile(suppFile.Name);
                    }
                }
                else
                {
                    LoaderStep parent = this.ParentStep;
                    _usingSeparateFileHost = true;

                    while (parent != null)
                    {
                        if (parent.GetType() == typeof(Microsoft.Test.Loaders.Steps.FileHostStep))
                        {
                            this._fileHost = ((FileHostStep)parent).fileHost;
                            break;
                        }
                        // Failed to find it in the immediate parent: try til we hit null or the right one
                        parent = parent.ParentStep;
                    }
                }
            }
        }

        private void RegisterUIHandlers()
        {
            foreach (UIHandler handler in UIHandlers)
            {
                //standAloneApp = standAloneApp || (handler.GetType() == typeof(Microsoft.Test.Deployment.StandAloneApplicationVerifier));

				if (handler.NamedRegistration != null)
				{
					_appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
					_registeredHandlers++;
				}
				else
				{
					_appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
					_registeredHandlers++;
				}

				
            }
        }

		private AutomationElement GetIEWindow(IntPtr hWnd)
		{
			AutomationElement IEWindow = null;
			String IEWindowClass = String.Empty;			

			try
			{
				if (hWnd != IntPtr.Zero)
					IEWindow = AutomationElement.FromHandle(hWnd);
			}
			catch (Exception)
			{
				IEWindow = null;
			}
			finally
			{
				if (IEWindow != null)
					IEWindowClass = IEWindow.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as String;

				if ((IEWindow == null) || (IEWindowClass == null) || (!IEWindowClass.ToLowerInvariant().Equals("ieframe")))
				{
					//We are dealing with a wrong window
					PropertyCondition isIEWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");
					IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isIEWindow);
				}
			}

			if (IEWindow != null)
			{
				IEWindowClass = IEWindow.GetCurrentPropertyValue(AutomationElement.ClassNameProperty) as String;

				if (!((IEWindowClass != null) && (IEWindowClass.ToLowerInvariant().Equals("ieframe"))))
				{
					IEWindow = null;
				}
			}

			return IEWindow;
		}

		private bool NavigateButtonIE(IntPtr hWnd, string buttonID)
        {
			AutomationElement IEWindow = GetIEWindow(hWnd);
			bool invoked = false;

			if (IEWindow != null)
			{
				PropertyCondition buttonSearch = new PropertyCondition(AutomationElement.NameProperty, buttonID);
				AutomationElement button = IEWindow.FindFirst(TreeScope.Descendants, buttonSearch); ;

				object patternObject;
				button.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
				InvokePattern invokeButton = patternObject as InvokePattern;
				invokeButton.Invoke();
				invoked = true;
			}

			return invoked;
		}		

		private bool NavigateUsingIEBar(string param, IntPtr hWnd)
		{			
			AutomationElement IEWindow = GetIEWindow(hWnd);

			if (IEWindow == null)
			{
				GlobalLog.LogDebug("Couldn't find an IE Window to navigate");
				return false;
			}
            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();

			Thread.Sleep(4000);
			AutomationElement address = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);
			Thread.Sleep(1000);

			if (address != null)
			{
				address.SetFocus();

				//Sleep a second to make sure that the address bar has focus
				Thread.Sleep(2500);

				MTI.Input.SendUnicodeString(param.ToLowerInvariant(), 1, 10);
				MTI.Input.SendKeyboardInput(Key.Enter, true);

			}
			else
			{
				GlobalLog.LogDebug("Couldn't find the IE address bar to navigate");
				return false;
			}
			return true;
		}
		    
        #endregion
    }
}
