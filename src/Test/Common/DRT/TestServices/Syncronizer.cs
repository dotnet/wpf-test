// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------------
//
//
// Description: Syncronizer for Use with Test Services
// 
//
//---------------------------------------------------------------------------

using System;
using System.Threading;

namespace DRT
{
    /// <summary>
    /// Provides an easy means to syncronize calls across threads or AppDomains.
    /// </summary>
    public class Syncronizer : MarshalByRefObject, IDisposable
    {
        #region Public Methods
        /// <summary>
        /// Blocks until the caller is signaled to continue.
        /// </summary>
        public void Wait()
        {
        _sync.WaitOne();
        }
        /// <summary>
        /// Will unblock callers who called Wait.
        /// </summary>
        public void AllowCalls()
        {
        _sync.Set();
        }
        /// <summary>
        /// Will block callers who call Wait.
        /// </summary>
        public void BlockCalls()
        {
        _sync.Reset();
        }
        #endregion

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            // ensures all blocked calls are released
            _sync.Set();
        }
        #endregion

        #region Private Fields
        private ManualResetEvent _sync = new ManualResetEvent(false);
        #endregion

}
}
