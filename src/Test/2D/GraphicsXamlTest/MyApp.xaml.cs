// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.Data;
using System.Xml;
using System.Configuration;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class MyApp : Application
    {
        /// <summary/>
        public static void Launch()
        {
            char[] tokens ={ ' ' };
            XamlTestHelper.args = DriverState.DriverParameters["Args"].Split(tokens);
            MyApp.Main();
        }

        void AppStartup(object sender, StartupEventArgs e)
        {
            if (XamlTestHelper.args == null)
            {
                //Save the command line for future use
                XamlTestHelper.args = e.Args;
            }

            //Getting the test variables from command line
            SetTestArguments(XamlTestHelper.args);

            

            //Getting current Assembly
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type[] types = currentAssembly.GetTypes();

            //Loop through all types
            //search for the right type
            //Create an instance of the type
            foreach (Type t in types)
            {
                if (t.Name == _testName)
                {
                    XamlTestHelper.LogStatus("Calling the xaml test - " + _testName);
                    _testWindow = Activator.CreateInstance(t) as System.Windows.Window;
                    break;
                }
            }

            //If the test class is found and created
            //Set the width and Height
            //Set the title of the avalon window
            //Display the window
            if (_testWindow != null)
            {
                XamlTestHelper.LogStatus("Create test Avalon window");
                _testWindow.Width = _winWidth;
                _testWindow.Height = _winHeight;
                _testWindow.WindowStyle = WindowStyle.None;
                _testWindow.ResizeMode = ResizeMode.NoResize;
                _testWindow.Title = _testName + " test";

                //Set up the window information in the Helper class
                XamlTestHelper.window = _testWindow;
                XamlTestHelper.currentDispatcher = _testWindow.Dispatcher;

                _testWindow.Show();
            }
            else
            {
                throw new System.ApplicationException("Incorrect test name");
            }
        }

        //------------------------------------------------------
        //  private methohs
        private void SetTestArguments(string[] args)
        {
            if (XamlTestHelper.GetArgument("Width", args) != null)
            {
                this._winWidth = int.Parse(XamlTestHelper.GetArgument("Width", args));
            }

            if (XamlTestHelper.GetArgument("Height", args) != null)
            {
                this._winHeight = int.Parse(XamlTestHelper.GetArgument("Height", args));
            }

            if (XamlTestHelper.GetArgument("Test", args) != null)
            {
                this._testName = XamlTestHelper.GetArgument("Test", args);
            }
        }

        // -------------------------------------------------------------------
        //  Local variables
        Window _testWindow = null;
        string _testName = null;
        double _winWidth = 400;
        double _winHeight = 400;
    }

}