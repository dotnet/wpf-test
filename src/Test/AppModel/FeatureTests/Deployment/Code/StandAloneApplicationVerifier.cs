// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Appverifier for StandAlone Applications
    /// </summary>
    public class StandAloneApplicationVerifier : AppVerifier
    {

        ActivationScheme _scheme = ActivationScheme.Local;

        /// <summary>
        /// Constructor: calls base ctor
        /// </summary>
        public StandAloneApplicationVerifier()
        {
            GlobalLog.LogDebug("Entering the constructor for " + this.ToString());
        }


        /// <summary>
        /// Used for verifying process name, Title if otherwise
        /// not specified, and shortcut name.
        /// </summary>
        /// <value></value>
        public override string AppName
        {
            set
            {
                base.AppName = value;
                this.ProcessesToCheck.Add(value);
            }
            get { return base.AppName; }
        }

        /// <summary>
        /// Scheme by which deployment is being tested. Adds dfsvc to processes to 
        /// monitor for certain cases (When method is launch, and scheme is UNC or Local)
        /// </summary>
        /// <value></value>
        public ActivationScheme Scheme
        {
            set { _scheme = value; }
            get { return _scheme; }
        }
        /// <summary>
        /// Calls base HandleWindow(), then cleans up any extra instances of IE that may be left over
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            LoaderStep myStep = this.Step;
            if (_scheme != ActivationScheme.HttpIntranet)
            {
                if ((myStep.GetType() == typeof(Microsoft.Test.Loaders.Steps.ActivationStep)) && ((ActivationStep)myStep).Method == ActivationMethod.Launch)                    
                {
                    this.ProcessesToCheck.Add("dfsvc");
                }
            }
            base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            // Once the window has been handled, we're done: Abort out
            return UIHandlerAction.Abort;
        }
    }
}
