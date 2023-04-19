// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// An instance of this test class is passed to an html page so it can call back on us.
    /// Must be public and ComVisible.
    /// </summary>

    [ComVisibleAttribute(true)]
    public class HtmlInteropTestClass
    {
        public void SetPageTitle(string title)
        {
            PageTitle = title;
        }

        public string PageTitle;
    }
}
