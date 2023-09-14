// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:   
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Threading;
using System.Collections.Generic;
using System.Reflection;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Threading;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Common
{

    /// <summary>
    /// </summary>
    public class GenericCompileHostedCaseParams
    {
        /// <summary>
        /// </summary>
        public string clrNameSpace;

        /// <summary>
        /// </summary>
        public string assembly;

        /// <summary>
        /// </summary>
        public string testCaseClassName;

        /// <summary>
        /// </summary>
        public string testCaseMethodName;

        /// <summary>
        /// </summary>
        public HostType hostType;

        // public Resource[] Resources;

        // public Content[] Contents;

        /// <summary>
        /// </summary>
        public string[] XamlPages;
    }


    /// <summary>
    /// This class provides services for executing test cases that implements the IHostedTest
    /// interface.
    /// </summary>
    public class GenericTestContainerTest
    {
        /// <summary>
        /// This is a generic entry point for a core test case.  This is planed for being used
        /// on the .tests file.
        /// An example fo usage will be this, the /ClassName will be Avalon.Test.CoreUI.Common.GenericTestContainerTest
        /// and the entry point RunCase().
        ///
        ///     <TestCase Area="buu" Priority="0" 
        ///	        TestContainerClass="All" 
        ///	        HostedTestEntryMethod="Run" 
        ///	        HostedTestClassName="Avalon.Test.CoreUI.CoreInput.ContentElementCaptureApp"	
        ///	        HostedTestAssemblyName="CoreTestsUntrusted.dll" 
        ///	        HostType="All" />
        /// </summary>
        public void RunCase()
        {
            ContentPropertyBag parameters = DriverState.DriverParameters;

            // Getting all the values that where set on the XML

            string testContainerClass = parameters["TestContainer"];
            string hostedTestEntryMethod = parameters["HostedTestEntryMethod"];
            string hostedTestClassName = parameters["HostedTestClassName"];
            string hostType = parameters["HostType"];
            string hostedTestAssemblyName = parameters["HostedTestAssemblyName"];

            if (testContainerClass == null || testContainerClass == "")
            {
                throw new InvalidOperationException("TestContainer parameter not initialized.");
            }
            if (hostedTestEntryMethod == null || hostedTestEntryMethod == "")
            {
                throw new InvalidOperationException("HostedTestEntryMethod parameter not initialized.");
            }
            if (hostedTestClassName == null || hostedTestClassName == "")
            {
                throw new InvalidOperationException("HostedTestClassName parameter not initialized.");
            }
            if (hostType == null || hostType == "")
            {
                throw new InvalidOperationException("HostType parameter not initialized.");
            }
            if (hostedTestAssemblyName == null || hostedTestAssemblyName == "")
            {
                throw new InvalidOperationException("HostedTestAssemblyName parameter not initialized.");
            }


            HostType hType = (HostType)Enum.Parse(typeof(HostType), hostType);


            // Today we have two types for TestContainer: TestApplicationStub and ExeStub

            if (testContainerClass.IndexOf("TestApplicationStub") != -1)
            {


                // Because the namepace and the class are on the same string, we need to 
                // parse it to get the the namespace and class separate.

                string[] namespaceArray = hostedTestClassName.Split(".".ToCharArray());

                if (namespaceArray.Length < 2)
                {
                    throw new InvalidOperationException("The hostedTestClassName doesn't namespace");
                }
                string className = namespaceArray[namespaceArray.Length - 1];

                string namespaceString = "";

                for (int i = 0; i < namespaceArray.Length - 1; i++)
                {
                    namespaceString += namespaceArray[i];

                    if (i < namespaceArray.Length - 2)
                    {
                        namespaceString += ".";
                    }
                }

                GenericCompileHostedCase.RunCase(namespaceString, hostedTestAssemblyName, className, hostedTestEntryMethod, hType);
            }
            else if (testContainerClass.IndexOf("WinformsTestContainer") != -1)
            {
                WinformsTestContainer test = new WinformsTestContainer();
                test.HostType = hType;
                test.Run(hostedTestClassName, hostedTestEntryMethod, hostedTestAssemblyName);

            }
            else
            {
                ExeStubContainerFramework test = new ExeStubContainerFramework(hType);
                test.Run(hostedTestClassName, hostedTestEntryMethod, hostedTestAssemblyName);
            }
        }

    }

    /// <summary>
    /// </summary>
    public class GenericCompileHostedCase
    {
        /// <summary>
        /// </summary>
        static public void RunCase(string clrNameSpace,
            string testCaseClassName,
            string testCaseMethodName,
            HostType hostType, Object[] resources, string[] contents)
        {
            RunCase(clrNameSpace, "CoreTestsUntrusted", testCaseClassName, testCaseMethodName, hostType, resources, contents);
        }


        /// <summary>
        /// </summary>
        static public void RunCase(string clrNameSpace,
            string testCaseClassName,
            string testCaseMethodName,
            HostType hostType)
        {
            RunCase(clrNameSpace, "CoreTestsUntrusted", testCaseClassName, testCaseMethodName, hostType);
        }


        /// <summary>
        /// </summary>
        static public void RunCase(string clrNameSpace,
            string assembly,
            string testCaseClassName,
            string testCaseMethodName,
            HostType hostType)
        {
            RunCase(clrNameSpace, assembly, testCaseClassName, testCaseMethodName, hostType, null, null);
        }

        /// <summary>
        /// Abstracts GenericCompileHostedCase instanciation and calling Run on the class
        /// </summary>
        static public void RunCase(string clrNameSpace,
            string assembly,
            string testCaseClassName,
            string testCaseMethodName,
            HostType hostType, Object[] resources, string[] contents)
        {
            RunCase(clrNameSpace, assembly, testCaseClassName, testCaseMethodName, hostType, resources, contents, null);
        }

        /// <summary>
        /// Abstracts GenericCompileHostedCase instanciation and calling Run on the class
        /// </summary>
        static public void RunCase(IHostedTest testCase, string testCaseMethodName,
            HostType hostType, Object[] resources, string[] contents, string[] xamlPages)
        {
            Type type = testCase.GetType();

            string ns = type.Namespace;
            string name = type.Name;
            string assemblyName = type.Assembly.GetName().Name;

            if (!Path.HasExtension(assemblyName))
            {
                assemblyName += ".dll";
            }

            RunCase(ns,
                assemblyName,
                name,
                testCaseMethodName,
                hostType,
                resources,
                contents,
                xamlPages);
        }

        /// <summary>
        /// Abstracts GenericCompileHostedCase instanciation and calling Run on the class
        /// </summary>
        static public void RunCase(string clrNameSpace,
            string assembly,
            string testCaseClassName,
            string testCaseMethodName,
            HostType hostType, Object [] resources, string[] contents, string[] xamlPages)
        {
            //GenericCompileHostedCaseParams parameters = new GenericCompileHostedCaseParams();

            //parameters.clrNameSpace = clrNameSpace;

            //// In case that the Assembly has extension, we need to remove the extension.
            //// The reason is that mapping PI Assembly property doesn't use extension.

            //if (Path.HasExtension(assembly))
            //{
            //    parameters.assembly = Path.GetFileNameWithoutExtension(assembly);
            //}
            //else
            //{
            //    parameters.assembly = assembly;
            //}

            //parameters.testCaseClassName = testCaseClassName;
            //parameters.testCaseMethodName = testCaseMethodName;
            //parameters.hostType = hostType;
            //parameters.Resources = resources;
            //if (contents != null)
            //{
            //    parameters.Contents = new Content[contents.Length];
            //    for (int i = 0; i < contents.Length; i++)
            //    {
            //        parameters.Contents[i] = new Content(contents[i]);
            //    }
            //}
            //parameters.XamlPages = xamlPages;

            //GenericCompileHostedCase testcase = new GenericCompileHostedCase();
            //testcase.Parameters = parameters;
            //testcase.Run();

        }

        /// <summary>
        /// </summary>
        public GenericCompileHostedCaseParams Parameters
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        /// <summary>
        /// Here we Compile the testApplicationStyb using MSBUILD and also
        /// we launch the test case using ActivationStep component.
        /// </summary>
        public void Run()
        {
            //if (_params == null)
            //{
            //    throw new InvalidOperationException("The Parameters is null.");
            //}

            //TestApplicationStubCompileTestParams param = new TestApplicationStubCompileTestParams(
            //    _params.clrNameSpace,
            //    _params.assembly,
            //    _params.testCaseClassName,
            //    _params.testCaseMethodName,
            //    _params.hostType
            //    );

            //if (_params.Resources != null)
            //{
            //    param.Resources.AddRange(_params.Resources);
            //}

            //if (_params.Contents != null)
            //{
            //    param.Contents.AddRange(_params.Contents);
            //}

            //if (_params.XamlPages != null)
            //{
            //    param.XamlPages.AddRange(_params.XamlPages);
            //}

            //// Compiling the Application

            //string compiledExtension;
            //if (_params.hostType == HostType.Browser)
            //{
            //    compiledExtension = ".xbap";
            //}
            //else
            //{
            //    compiledExtension = ".application";
            //}

            //TestApplicationStubCompileTest.Compile(param);

            //if ((!File.Exists("bin\\release\\" + param.AssemblyName + compiledExtension)))
            //{
            //    throw new InvalidOperationException("Compilation Failed. The file bin\\release\\" + param.AssemblyName + compiledExtension + " doesn't exist");
            //}

            //// Using ActivationStep component to launch the Application

            //List<SupportFile> files = new List<SupportFile>();

            //if (_params.Contents != null)
            //{
            //    for (int i = 0; i < _params.Contents.Length; i++)
            //    {
            //        SupportFile file = new SupportFile();
            //        file.Name = _params.Contents[i].FileName;
            //        files.Add(file);
            //    }
            //}

            //if (_params.Resources != null)
            //{
            //    for (int i = 0; i < _params.Resources.Length; i++)
            //    {
            //        SupportFile file = new SupportFile();
            //        file.Name = _params.Resources[i].FileName;
            //        files.Add(file);
            //    }
            //}

            //ActivationStep activationStep = new ActivationStep();
            //activationStep.FileName = "bin\\release\\" + param.AssemblyName + compiledExtension;
            //activationStep.SupportFiles = files.ToArray();
            //activationStep.SignAllApps = false;
            //activationStep.DoStep();

            //CleanUpCompilation();
        }

        /// <summary>
        /// For all processes running on the system, show the process name 
        /// and the name of the file that was used to create the process
        /// </summary>
        private void LogProcessesInfo()
        {
            Process[] processes = Process.GetProcesses();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < processes.Length; i++)
            {
                Process process = processes[i];
                sb.Append(process.ProcessName);
                sb.Append(" ");
            }
            GlobalLog.LogDebug(sb.ToString());
        }

        private void CleanUpCompilation()
        {
            //CoreLogger.LogStatus("Cleaning up compilation directory, if there is any....");

            //// Initialization for the multi-attempt logic
            //bool cleanupDone = false;
            //int maxAttempts = 5;
            //int timeBetweenAttempts = 1000; // sleep time, in milliseconds

            //int attempt = 0;
            //while (!cleanupDone && attempt < maxAttempts)
            //{
            //    attempt++;
            //    try
            //    {
            //        string currentFolder = Directory.GetCurrentDirectory();

            //        // Delete bin directory.
            //        string binPath = Path.Combine(currentFolder, "bin");
            //        if (Directory.Exists(binPath))
            //        {
            //            CoreLogger.LogStatus("Found bin folder " + binPath + ". Deleting...");
            //            Directory.Delete(binPath, true);
            //            CoreLogger.LogStatus("Bin folder deleted.");
            //        }

            //        // Delete obj directory.
            //        string objPath = Path.Combine(currentFolder, "obj");
            //        if (Directory.Exists(objPath))
            //        {
            //            CoreLogger.LogStatus("Found obj folder " + objPath + ". Deleting...");
            //            Directory.Delete(objPath, true);
            //            CoreLogger.LogStatus("Obj folder deleted.");
            //        }

            //        // Cleanup done!
            //        cleanupDone = true;
            //    }
            //    catch (Exception e)
            //    {
            //        // We catch only IOException or UnauthorizedAccessException
            //        // since those are thrown if some other process is using the 
            //        // files and directories we are trying to delete.
            //        if (e is System.IO.IOException || e is UnauthorizedAccessException)
            //        {
            //            CoreLogger.LogStatus("Cleanup attempt #" + attempt + " failed.");
            //            if ((1 == attempt) || (maxAttempts == attempt))
            //            {
            //                CoreLogger.LogStatus("Here are the active processes on the system.");
            //                LogProcessesInfo();
            //            }
            //            if (maxAttempts == attempt)
            //            {
            //                CoreLogger.LogStatus("Maximum no. of cleanup attempts reached. Bailing out....");
            //                throw;
            //            }
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }

            //    System.Threading.Thread.Sleep(timeBetweenAttempts);
            //}
        }

        GenericCompileHostedCaseParams _params = null;
    }

    /// <summary>
    /// </summary>
    public class TestApplicationStub : Application, ITestContainer
    {
        /// <summary>
        /// </summary>
        public TestApplicationStub()
        {
            this.Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(CommonDispatcherExceptionHandler);
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
        /// </summary>
        public string TestEntryPointMethodName
        {
            get
            {
                return _testEntryPointMethodName;
            }
            set
            {
                _testEntryPointMethodName = value;
            }
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

                if (_currentHostedTest != null)
                {
                    _currentHostedTest.TestContainer = this;
                }
            }
        }



        /// <summary>
        /// </summary>
        public Surface DisplayObject(object visual, int x, int y, int w, int h)
        {

            SurfaceFramework surface = null;

            if (Avalon.Test.CoreUI.Common.HostType.Browser != HostType)
            {
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
                    // On this case this is a Browser hosted so we can get the main window
                    surface = new SurfaceFramework(this.MainWindow);

                    surface.DisplayObject(visual);
                    surface.SetPosition(x, y);
                    surface.SetSize(w, h);
                }
                else
                {
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
        /// </summary>
        public void DisplayObjectModal(object visual, int x, int y, int w, int h)
        {

            SurfaceFramework surface = new SurfaceFramework("Window", x, y, w, h, false);

            lock (_modalStack)
            {
                _modalStack.Push(surface);
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

            // We have at least one current surface. 
            if (_surfaces.Count > 0)
            {
                NavigateToObjectCore(visual);
            }

            return true;
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
        /// Core overridable functionality for navigating to an object in an application.
        /// </summary>
        /// <param name="visual"></param>
        protected virtual void NavigateToObjectCore(object visual)
        {
            _surfaces[0].DisplayObject(visual);
        }

        /// <summary>
        /// Close the last modal window
        /// </summary>
        public void CloseLastModal()
        {
            Surface modalWindow = null;

            lock (_modalStack)
            {
                if (_modalStack.Count > 0)
                {
                    modalWindow = _modalStack.Pop();
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
                lock (_modalStack)
                {
                    return _modalStack.ToArray();
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <returns>true if a Dispatcher was running. false if it was a NOP (No Operation)</returns>
        public bool RequestStartDispatcher()
        {
            return false;
        }

        /// <summary>
        /// </summary>
        public void EndTest()
        {

            DispatcherHelper.EnqueueBackgroundCallback(this.Dispatcher,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    // We have to do this due 
                    if (Avalon.Test.CoreUI.Common.HostType.Browser != HostType)
                    {
                        DispatcherHelper.ShutDown();
                    }

                    // Stop Application Monitor to close IE
                    ApplicationMonitor.NotifyStopMonitoring();
                    return null;
                }, null);


        }



        /// <summary>
        /// </summary>
        public void EndTestNow()
        {
            // Stop Application Monitor to close IE
            ApplicationMonitor.NotifyStopMonitoring();
        }

        /// <summary>
        /// </summary>
        public Surface[] CurrentSurface
        {
            get
            {
                return _surfaces.ToArray();
            }
        }

        /// <summary>
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            CoreLogger.LogStatus("In TestApplicationStub.OnStartup()...");
            CoreLogger.LogStatus("Current directory: " + Environment.CurrentDirectory);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            DispatcherHelper.EnqueueBackgroundCallback(this.Dispatcher,
                (DispatcherOperationCallback)delegate(object arg)
                {

                    Type testCaseType = CurrentHostedTest.GetType();

                    testCaseType.InvokeMember(TestEntryPointMethodName,
                        BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        CurrentHostedTest,
                        null,
                        System.Globalization.CultureInfo.InvariantCulture);


                    return null;
                }, null);
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

        bool _shouldLogUnhandledException = true;
        private List<Surface> _surfaces = new List<Surface>();
        private IHostedTest _currentHostedTest = null;
        private HostType _hostType = HostType.Browser;
        private string _testEntryPointMethodName = "";
        private System.Collections.Generic.Stack<Surface> _modalStack = new System.Collections.Generic.Stack<Surface>();

    }

    /// <summary>
    /// Like TestApplicationStub, but with Navigation functionality.
    /// </summary>
    public class TestNavigationApplicationStub : TestApplicationStub
    {
    }
}
