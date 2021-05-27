// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Markup;
using System.ComponentModel;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Integration.Windows
{
    /// <summary>
    /// Represents a type of application or executable in which variations
    /// will run.  Together with HostType, it is the context of the variations.
    /// </summary>
    public enum ApplicationType
    {
        /// <summary>   
        /// </summary>
        Xbap,

        /// <summary>
        /// </summary>
        WpfApplication,

        /// <summary>
        /// </summary>
        WinFormsApplication,

        /// <summary>
        /// </summary>
        ClrExe
    }

    /// <summary>
    /// Executes child variations in specified container.
    /// </summary>
    /// <remarks>
    /// ContainerVariationItem depends on ControllerProxy and Controller to run 
    /// variations on different kinds of Dispatcher threads.  
    /// 
    /// Since a Dispatcher thread may exist within another process, e.g. in a .Xbap, 
    /// the driver thread is always kept separate from the dispatcher thread.  Also, since 
    /// some actions start nested message pumps, e.g. opening a dialog, the variation thread 
    /// is always kept separate from the dispatcher thread.
    /// 
    /// Thread 1 (Driver thread): 
    ///     ContainerVariationItem - creates ControllerProxy, sends variations to ControllerProxy
    ///     ControllerProxy - initiates execution of Controller, forwards variations to Controller
    /// 
    /// Thread 2 (Dispatcher thread):
    ///     Controller - either is created within a running Dispatcher, e.g. in a .Xbap, 
    ///                  or runs a Dispatcher
    /// 
    /// Thread 3 (Variation loop thread):
    ///     Controller - runs loop that accepts variations from ControllerProxy.
    ///                - invokes variations on Dispatcher thread
    /// 
    /// </remarks>
    [ContentProperty("Children")]
    public class ContainerVariationItem : VariationItemGroup
    {    
        /// <summary>
        /// Executes child variations in specified container.
        /// </summary>
        public override void Execute()
        {
            GlobalLog.LogStatus("In ContainerVariationItem.Execute()...");
            GlobalLog.LogStatus("ContainerVariationItem.ApplicationType: " + this.ApplicationType);

            ApplicationType appType = (ApplicationType)Enum.Parse(typeof(ApplicationType), this.ApplicationType);

            //
            // Runs all child variations within single app, i.e. Controller.
            // When ControllerProxy is created, it automatically starts the Dispatcher
            // thread, whether that's a simple call to Dispatcher.Run() or starting an app.
            //
            using (ControllerProxy controller = new ControllerProxy(appType))
            {
                foreach (VariationItem variation in Children)
                {
                    controller.Perform(variation);
                }

                controller.WaitUntilDone();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationType
        {
            get { return _applicationType; }
            set { _applicationType = value; }
        }

        private string _applicationType = null;
    }
}
