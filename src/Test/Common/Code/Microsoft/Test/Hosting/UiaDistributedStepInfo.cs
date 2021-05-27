// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Hosting
{
    [Serializable]
    internal sealed class UiaDistributedStepInfo
    {
        private UiaDistributedStep step;
        private UiaDistributedStepTarget target;
        private bool alwaysRun;

        internal UiaDistributedStepInfo(UiaDistributedStep step, UiaDistributedStepTarget target, bool alwaysRun)
        {
            this.step = step;
            this.target = target;
            this.alwaysRun = alwaysRun;
        }

        public UiaDistributedStep Step
        {
            get { return step; }
        }

        public UiaDistributedStepTarget Target
        {
            get { return target; }
        }

        public bool AlwaysRun
        {
            get { return alwaysRun; }
        }
    }
}
