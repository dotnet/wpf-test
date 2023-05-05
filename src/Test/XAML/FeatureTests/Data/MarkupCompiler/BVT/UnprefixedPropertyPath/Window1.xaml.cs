// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UnprefixedPropertyPath
{
    /// <summary>
    /// Verify a property path without prefixes is resolved
    /// </summary>
    public partial class Window1 : Page
    {
        public Window1()
        {
            InitializeComponent();
        }

        protected void PageInitialized(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
