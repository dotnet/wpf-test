// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Markup;
    using System.Reflection;
    using Microsoft.Test.Serialization;

    public partial class MyAppName : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }

    
