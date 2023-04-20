// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Markup;

namespace XamlExperiment
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application app = (Application)Application.LoadComponent(new Uri("ApplicationStyledNavigationWindow.xaml", UriKind.RelativeOrAbsolute));
            ((IComponentConnector)app).InitializeComponent();
            app.Run();
        }
    }
}
