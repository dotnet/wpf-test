// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse GetPosition fails for invalid input element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseGetPositionInvalidInputElementApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify Mouse GetPosition fails for invalid input element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify Mouse GetPosition fails for invalid input element in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify Mouse GetPosition fails for invalid input element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseGetPositionInvalidInputElementApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify Mouse GetPosition fails for invalid input element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify Mouse GetPosition fails for invalid input element in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseGetPositionInvalidInputElementApp(),"Run");
            
        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {


            CoreLogger.LogStatus("Constructing window....");
            
            {
                

                // Construct test element, add event handling
                _rootElement = new InstrPanel();

                // Construct invalid input element.
                _dio = new TestDependencyInputObject();

                // Put the test element on the screen
                DisplayMe(_rootElement, 10, 10, 100, 100);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Invalid input element.
        /// </summary>
        private IInputElement _dio;

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Setting focus to the invalid input element....");
            try
            {
                Point pt = Mouse.GetPosition(_dio);
            }
            catch (InvalidOperationException e)
            {
                _exceptionLog.Add(e);
            }
            
            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need no focus events to fire. (It IS an invalid input element.)
            // We also need an invalid operation exception to occur.
            
            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            CoreLogger.LogStatus("Exceptions found: "+_exceptionLog.Count);
            if (_exceptionLog.Count == 1)
            {
                CoreLogger.LogStatus("Logged exception:\n"+(Exception)_exceptionLog[0]);                
            }
            
            bool actual = (_exceptionLog.Count==1) && (_eventLog.Count==0) && (_exceptionLog[0].GetType() == typeof(InvalidOperationException));
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        /// <summary>
        /// Store record of our fired exceptions.
        /// </summary>
        private ArrayList _exceptionLog = new ArrayList();

    }
}
