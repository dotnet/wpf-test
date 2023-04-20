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

    public partial class MyAppName : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            FieldInfo fi = typeof(MyClassName).GetField("NameField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (fi.IsPrivate)
            {
                Console.WriteLine("Field has correct access level defined by x:FieldModifier");
                Application.Current.Shutdown(0);
            }
            else
            {
                Console.WriteLine("Field has incorrect access level");
                Application.Current.Shutdown(1);
            }
        }
    }

    public partial class MyClassName : Page
    {
        void StubFunction()
        {
        }
    }

