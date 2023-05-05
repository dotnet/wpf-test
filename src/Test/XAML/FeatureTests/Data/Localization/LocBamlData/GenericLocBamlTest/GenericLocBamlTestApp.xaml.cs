// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace Microsoft.Test.Xaml.Localization
{
    public partial class GenericLocBamlTestApp : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                GenericLocBamlTest genericLocBamlTest = new GenericLocBamlTest();
                genericLocBamlTest.ShowDialog();
                Console.WriteLine("The window was created and shown successfully.");
                Application.Current.Shutdown(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine("The constructor threw the following exception.\n" + exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
