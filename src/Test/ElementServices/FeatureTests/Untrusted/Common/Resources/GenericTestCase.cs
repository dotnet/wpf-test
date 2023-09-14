// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon core property system test automation 
// (C) Microsoft Corporation 2003
// File 
// Description 
//		


using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Windows;
using System.Collections;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// This is abstract implementation of a Property System test case.
    /// This implementation comes with lots of goodies as it inherits from the helper.
    /// </summary>
	/// <remarks>
	/// This calss impelemnts the generic test case ,
	/// It is derived from the Helper
	/// <para>
	///		<see cref="TestHelper">Helper</see>
	/// </para>	
	/// </remarks>
    public abstract class GenericTestCase : TestHelper
    {
		/// <summary>
		/// Abstact function that must be implemented by every concrete test case.
		/// </summary>
		public abstract void Test();
		/// <summary>
		/// 
		/// </summary>
		public static Dispatcher MainDispatcher = null;

        /// <summary>
        /// Calls <see cref="Test">Test</see> after initializing a Dispatcher.
        /// This is always the entry point of the test suit 
        /// </summary>
        /// <remarks>
        ///  note that it uses DispatcherAccess and all creation and deletion of the UIelements must happen bewteen the rented access 
        /// </remarks>
        public void RunTest()
        {
            MainDispatcher = Dispatcher.CurrentDispatcher;
            this.Init();
	    
            Test();
	    
            return;
        }
    }
	
}
//end of file
