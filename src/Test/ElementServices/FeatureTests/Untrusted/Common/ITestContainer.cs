// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// This interface abstracts the need for Surface and a Message Pump for any
    /// test case that implements IHostedTest.
    /// </summary>
    public interface ITestContainer
    {
        /// <summary>
        /// Relationship between the ITestContainer and the IHostedTest
        /// </summary>
        IHostedTest CurrentHostedTest
        {
            get;
            set;
        }

        /// <summary>
        /// Present the specified object on a new Surface, you might suggest
        /// some attributes for the surface. Only the TestContainer must know
        /// the details about the surface.
        /// </summary>
        Surface DisplayObject(object visual, int x, int y, int w, int h);

        /// <summary>
        /// Present the specified object on a new Modal Surface, you might suggest
        /// some attributes for the surface. Only the TestContainer must know
        /// the details about the surface.
        /// </summary>
        void DisplayObjectModal(object visual, int x, int y, int w, int h);

        /// <summary>
        /// Navigate to object on currently displayed surface.
        /// </summary>
        /// <param name="visual">A visual object</param>
        /// <returns>true if the navigation succeeds, false otherwise.</returns>
        bool NavigateToObject(object visual);


        /// <summary>
        /// Go back to object most recently navigated from.
        /// </summary>
        /// <remarks>
        /// Similar to browser Back button.
        /// </remarks>
        void GoBack();

        /// <summary>
        /// Go forward to object most recently navigated back from.
        /// </summary>
        /// <remarks>
        /// Similar to browser Forward button.
        /// </remarks>
        void GoForward();

        /// <summary>
        /// Close the last modal window
        /// </summary>
        void CloseLastModal();

        /// <summary>
        /// Request that the dispatcher needs to be run. If there is a dispatcher already running
        /// this method should be a NOP.
        /// </summary>
        /// <returns>true if a Dispatcher was running. false if it was a NOP (No Operation)</returns>
        bool RequestStartDispatcher();

        /// <summary>
        /// Request to End the test case, for example stopping the dispatcher or
        /// closing Internet Explorer. Async using Avalon queues
        /// </summary>
        void EndTest();


        /// <summary>
        /// Request to End the test case, for example stopping the dispatcher or
        /// closing Internet Explorer. Sync version.
        /// </summary>
        void EndTestNow();

        /// <summary>
        /// Dictates whether or not unhandled dispatcher exceptions should be logged 
        /// automatically as failures.
        /// </summary>
        bool ShouldLogUnhandledException
        {
            get;
            set;
        }

        /// <summary>
        /// Raised when an unhandled exception occurs.
        /// </summary>
        event EventHandler ExceptionThrown;

        /// <summary>
        /// Returns all the surfaces created on this ITestContainer.
        /// </summary>
        Surface[] CurrentSurface
        {
            get;
        }

        /// <summary>
        /// Returns all the modal surfaces created on this ITestContainer. The last surface created is the position 0 on the array
        /// </summary>
        Surface[] CurrentModalSurfaces
        {
            get;
        }
    }
}

