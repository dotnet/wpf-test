// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.AddIn.Pipeline;
using System.Windows;

namespace Microsoft.Test.AddIn
{
    [AddInBase]
    public abstract class HostSequenceFocusView : HostViewBase
    {
        public abstract FocusItem[] GetFocusSequence();

        public abstract void ClearSequence();
    }

}
