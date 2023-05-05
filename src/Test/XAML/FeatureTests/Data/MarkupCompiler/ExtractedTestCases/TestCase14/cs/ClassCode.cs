// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Markup;
    using Microsoft.Test.Serialization;

    public partial class Application__ : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
        }
    }


    public partial class MyAppName : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {            
            FileStream fs = new FileStream(".\\obj\\release\\ClassMarkup.baml", FileMode.Open);
            BamlReaderWrapper reader = new BamlReaderWrapper(fs);
            reader.Read();
            reader.Read();
            string name = reader.Name;

            if (name == "MyClassName")
            {
                Console.WriteLine("BAML has correct name as defined by x:Subclass");
                Application.Current.Shutdown(0);
            }
            else
            {
                Console.WriteLine("BAML record has incorrect value: " + name);
                Application.Current.Shutdown(1);
            }

            MyClassName instance = new MyClassName();
            instance.Title="Foo";
        }
    }

