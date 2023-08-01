// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using System.Collections;

using Microsoft.Test.Logging;
using System.Windows.Markup;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using System.Windows.Media;
using System.Windows.Input;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// MDEIHostedTestBase Model class
    /// </summary>
    public abstract class MDEIHostedTestBase : IHostedTest
    {
        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _iTestContainer;
            }
            set
            {
                _iTestContainer = value;
            }
        }

        /// <summary>
        /// </summary>
        protected CoreModelState ModelState;

        /// <summary>
        /// </summary>
        protected abstract void HostedTestEntryPointCore();

        /// <summary>
        /// Internal fully-specified generated class name to initialize.
        /// </summary>
        /// <remarks>
        /// DANGER: This is subject to change.
        /// </remarks>
        private const string GeneratedMyClassRootTypeName = "MyClassRoot";

        /// <summary>
        /// Internal method name to invoke on generated class.
        /// </summary>
        /// <remarks>
        /// DANGER: This is subject to change.
        /// </remarks>
        private const string GeneratedMyClassInitializeComponentName = "InitializeComponent";

        /// <summary>
        /// </summary>
        public void HostedTestEntryPoint()
        {
            MouseHelper.MoveOnVirtualScreenMonitor();

            TestContainer.ExceptionThrown += new EventHandler(ExceptionBeenThrown);

            // Loading the test case state or config

            ModelState = CoreModelState.Load();


            // We need create the root.  Depending if the test case is compiled or not
            // we Parser or create the object

            if (ModelState.CompiledVersion)
            {
                Log("Creating the object via reflection....");
                Assembly assembly = null;
                Application app = Application.Current;
                string assemblyPath = Path.GetDirectoryName(Assembly.GetAssembly(app.GetType()).Location);

                CoreLogger.LogStatus(assemblyPath + "\\CoreTestGeneric.exe");
                assembly = Assembly.LoadFrom(assemblyPath + "\\CoreTestGeneric.exe");
                //}

                Type type = assembly.GetType(GeneratedMyClassRootTypeName);
                if (type == null)
                {
                    throw new Microsoft.Test.TestValidationException("Type '" + GeneratedMyClassRootTypeName + "' not found. Could not retrieve model state from built assembly.");
                }

                XamlRootObject = Activator.CreateInstance(type);

                type.InvokeMember(GeneratedMyClassInitializeComponentName, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                    null, XamlRootObject, null);

            }
            else
            {
                Log("Creating the object via parsing markup file....");
                FileStream fs = new FileStream(XamlFile, FileMode.Open);
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                XamlRootObject = System.Windows.Markup.XamlReader.Load(fs, pc);
                fs.Close();
            }

            TestContainer.DisplayObject(XamlRootObject, 0, 0, 500, 500);


            HostedTestEntryPointCore();


            TestContainer.RequestStartDispatcher();


        }


        /// <summary>
        /// </summary>
        protected object XamlRootObject = null;

        /// <summary>
        /// </summary>
        protected Visual RealRootVisual
        {
            get
            {
                if (XamlRootObject == null || !(XamlRootObject is Visual))
                {
                    return null;
                }

                Visual visual = (Visual)XamlRootObject;

                while (VisualTreeHelper.GetParent(visual) != null)
                {
                    visual = (Visual)VisualTreeHelper.GetParent(visual);
                }

                return visual;
            }
        }

        /// <summary>
        /// </summary>
        protected void ExceptionBeenThrown(object o, EventArgs args)
        {
            LogTest(false, "An exception has been caught");
        }

        /// <summary>
        /// </summary>
        protected void LogTest(bool v, string reason)
        {
            if (!v)
            {
                _isTestPassed = false;
            }

            CoreLogger.LogTestResult(v, reason);
        }

        /// <summary>
        /// </summary>
        protected void Log(string str)
        {
            CoreLogger.LogStatus(str);
        }

        /// <summary>
        /// </summary>
        protected string XamlFile = "__test1.xaml";

        /// <summary>
        /// </summary>
        protected bool IsTestPassed
        {
            get
            {
                return _isTestPassed;

            }
            set
            {

                _isTestPassed = value;
            }
        }

        private bool _isTestPassed = true;
        private ITestContainer _iTestContainer = null;

    }
}



