// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Win32;  // MSG

namespace Avalon.Test.CoreUI.Hosting.Security
{
#region MyIkis
    /// <summary>
    /// User implementation of IKeyboardInputSink. 
    /// This implementation does nothing it is only for registering with IKeyboardInputSink
    /// implemented by parent HwndSource.
    /// </summary>
    internal class MyIkis : IKeyboardInputSink
    {
        bool IKeyboardInputSink.HasFocusWithin()
        {
            return false;
        }

        bool IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            return false;   
        }

        IKeyboardInputSite IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            return null;
        }

        bool IKeyboardInputSink.TabInto(TraversalRequest request)
        {
            return false;
        }

        bool IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            return false;
        }

        bool IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
        {
            return false;
        }

        IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite 
        {
            get { return null; }
            set {}
        }

    }
#endregion

    /// <summary>
    /// Verify it is not possible for an object implementing IKeyboardInputSink 
    /// to modify or invent messages by showing it is not possible to register an IKeyboardInputSink
    /// with an HwndSource in partial trust.
    /// </summary>
    /// <description>
    /// Unit test for IKeyboardInputSink.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class IKeyboardInputSinkRegistration: TestCase
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.None
        /// </summary>
        public IKeyboardInputSinkRegistration()
            : base(TestCaseType.None)
        { }     

        /// <summary>
        /// Launch the test.
        /// </summary>        
        [Test(0, @"Hosting\Security", TestCaseSecurityLevel.PartialTrust, "IKeyboardInputSinkRegistration", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            bool caughtException = false;

            using (CoreLogger.AutoStatus("Building window"))
            {
                MyIkis sink = new MyIkis();

                SurfaceFramework surface = new SurfaceFramework("HwndSource", 200, 200, 300, 300);

                CoreLogger.LogStatus("Getting HwndSource");
                HwndSource hs = (HwndSource)surface.GetPresentationSource();

                try
                {
                    CoreLogger.LogStatus("Attempting to register user defined IKeyboardInputSink with HwndSource,");
                    CoreLogger.LogStatus("this should throw a security exception");
                    ((IKeyboardInputSink)hs).RegisterKeyboardInputSink(sink);
                }
                catch (Exception e)
                {
                    caughtException = true;

                    if (e is SecurityException)
                    {
                        CoreLogger.LogTestResult(true, "Caught security exception as expected: " + e);
                    }
                    else
                    {
                        CoreLogger.LogTestResult(false, "Caught the wrong kind of exception. Registering user defined IKeyboardInputSink in partial trust should have thrown a security exception. " + e);
                    }
                }

                if (!caughtException)
                {
                    CoreLogger.LogTestResult(false, "Didn't catch any exceptions. Registering user defined IKeyboardInputSink in partial trust should have thrown a security exception.");
                }
            }
            CoreLogger.EndVariation();
        }      
    }
}

