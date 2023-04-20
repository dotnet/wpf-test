// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace Microsoft.Test.DataServices
{
    // This is a wrapper to help us manipulate context of type ManualResentEventSlim
    // when we pass it to register for CrossThread access.  We might use this wrapper
    // to pass more context in the future.

// ManualResetEventSlim is new in .NET 4.0 so should not be included in 3.5 test builds.
    public class ManualResetEventSlimWrapper
    {
        public ManualResetEventSlimWrapper()
        {
        }
        public ManualResetEventSlim SlimObject
        {
            get
            {
                return _slimObject;
            }
            set
            {
                _slimObject = value;
            }
        }
        private ManualResetEventSlim _slimObject;
    }
}
