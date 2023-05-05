// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace RegressionIssue61_OverwriteResource
{
    /// <summary>
    /// Verify that resources overwritten after BAML is loaded
    /// aren't resolved
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Resources["string1"] = "ERROR";
            Resources["string2"] = "ERROR";
            Resources["string3"] = "ERROR";
            Resources["string4"] = "ERROR";
            Resources["string5"] = "ERROR";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool passed = true;

            for (int i = 1; i <= 5; i++)
            {
                string output = Serialize(this.FindName("button" + i.ToString()));
                if(!output.Contains("OK"))
                {
                    Console.WriteLine("Overwritten resource got resolved for button" + i.ToString());
                    passed = false;
                }
            }

            if(passed)
            {
                Console.WriteLine("Overwritten resources didn't get resolved");
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
