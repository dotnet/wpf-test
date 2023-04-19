// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public interface IProvideJournalingState
    {
        bool IsBackEnabled();
        bool IsBackEnabled(ContentControl c);
        bool IsForwardEnabled();
        bool IsForwardEnabled(ContentControl c);
        String[] GetBackMenuItems();
        String[] GetBackMenuItems(ContentControl c);
        String[] GetForwardMenuItems();
        String[] GetForwardMenuItems(ContentControl c);
        String WindowTitle
        {
            get;
        }
    }
}
