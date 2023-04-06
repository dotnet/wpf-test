// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment
{
	/// <summary>
	/// Appverifier for simple HTML markup 
	/// </summary>
	public class HTMLVerifier : AppVerifier
	{

        /// <summary>
        /// Constructor : Adds iexplore to list of processes to monitor
        /// </summary>
		public HTMLVerifier()
		{
            GlobalLog.LogDebug("Entering constructor for  " + this.ToString());

            // check for these processes
			this.ProcessesToCheck.Add("iexplore");
            this.AppShouldBeInStore = false;
		}
        /// <summary>
        /// Calls base HandleWindow()
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            // Sleep several seconds to allow HTML to render...
            Thread.Sleep(7000);
			base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            return UIHandlerAction.Abort;
        }

	}
}
