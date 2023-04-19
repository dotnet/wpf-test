// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public interface INavigationProvider : IProvideJournalingState
    {
        void GoBack();
        void GoForward();
        void GoBack(int DropdownIndex);
        void GoForward(int DropdownIndex);
        void FindAndClickLabelHyperlink(String labelId);
    }
}
