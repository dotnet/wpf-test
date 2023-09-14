// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    ///<summary>
    ///</summary>"
    ///<remarks>
    ///     <filename>HwndSourceDispose.cs</filename>
    ///</remarks>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    [TestDefaults]
    public class HwndSourceDisposeSimple : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndSourceDisposeSimple() :base(TestCaseType.None)
        {

        }

        /// <summary>
        /// Creating a CheckButton (all the tree on code) and adding a property trigger for IsChecked
        /// </summary>
        public override void Run(){}


        /// <summary>
        /// Disposing the HwndSource using a Win32 DestroyWindow async.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Create a HwndSource and Hook a button</li>
        ///     <li>Dispatcher.Run</li>
        ///     <li>on a posted object call Win32 DestroyWindow</li>
        ///     <li>Exit the dispatcher and validating Source.IsDisposed</li>
        ///  </ol>
	    ///     <filename>HwndSourceDispose.cs</filename>
        /// </remarks>
        [Test(0, @"Source\Destroy\Win32API", TestCaseSecurityLevel.FullTrust, "CallingWin32Destroy", Area = "AppModel")]
        public void CallingWin32Destroy()
        {
            CoreLogger.BeginVariation();
            Button b = new Button();
            b.Content="Button";
        
            Source = SourceHelper.CreateHwndSource( 500, 500,0,0);
            Source.RootVisual = b;



            MainDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DestroyW), null);

            Dispatcher.Run();


            if(!Source.IsDisposed)
                throw new Microsoft.Test.TestValidationException("The window is not disposed");

            if (!_isPass)
                throw new Microsoft.Test.TestValidationException("DestroyWindow is not called");
            CoreLogger.EndVariation();
        }


        object DestroyW(object o)
        {
            NativeMethods.DestroyWindow(new HandleRef(null,Source.Handle));
            _isPass = true;

            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }
        
        bool _isPass = false;
    }
}










