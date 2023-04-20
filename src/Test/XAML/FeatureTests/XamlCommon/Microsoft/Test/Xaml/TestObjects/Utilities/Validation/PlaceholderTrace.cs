// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

////////////////////////////////////////////////////////////
// This file contains core tracing/validation types. No Oslo product
// dependencies or non-SilverLight compatible code should be
// introduced.
namespace Microsoft.Test.Xaml.Common.TestObjects.Utilities.Validation
{
    public class PlaceholderTrace : WorkflowTraceStep, IPlaceholderTraceProvider
    {
        protected IPlaceholderTraceProvider Provider { get; set; }

        public PlaceholderTrace()
        {
        }

        public PlaceholderTrace(IPlaceholderTraceProvider provider)
        {
            this.Provider = provider;
        }

        public virtual TraceGroup GetPlaceholderTrace()
        {
            return this.Provider.GetPlaceholderTrace();
        }
    }

    public interface IPlaceholderTraceProvider
    {
        TraceGroup GetPlaceholderTrace();
    }
}
