// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DRT.ThirdPartyThemes
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    
    
    /// <summary>
    ///     Third Party Themes Page
    /// </summary>
    partial class ThirdPartyThemesPage
    {
        void OnLoaded(object sender, EventArgs e)
        {
            if (Button1.Index != 2)
            {
//                throw new Exception("Button1.Index != 2");
            }
        }
    }

}
