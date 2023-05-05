// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace RegressionIssue61_InsertResource
{
    /// <summary>
    /// Verify that resources inserted in the lookup path after BAML is loaded
    /// aren't resolved
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // panel is a Grid which is a child of Window which already defines resources 
            // with names string1, string2 etc. By adding resources with the same names to
            // panel.Resources we are trying to intercept the resource lookup.
            panel.Resources["string1"] = "ERROR";
            panel.Resources["string2"] = "ERROR";
            panel.Resources["string3"] = "ERROR";
            panel.Resources["string4"] = "ERROR";
            panel.Resources["string5"] = "ERROR";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool passed = true;

            for (int i = 1; i <= 5; i++)
            {
                string output = Serialize(this.FindName("button" + i.ToString()));
                if(!output.Contains("OK"))
                {
                    Console.WriteLine("Inserted resource got resolved for button" + i.ToString());
                    passed = false;
                }
            }

            if(passed)
            {
                Console.WriteLine("Inserted resources in lookup path didn't get resolved");
                Application.Current.Shutdown(0);
            }
            else
            {
                Application.Current.Shutdown(1);
            }
        }

        private string Serialize(object obj)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            XmlWriter writer = XmlWriter.Create(sb, settings);

            XamlWriter.Save(obj, writer);
            return sb.ToString();
        }
    }
}
