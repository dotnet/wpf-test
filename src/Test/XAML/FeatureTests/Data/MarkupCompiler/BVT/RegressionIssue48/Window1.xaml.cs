// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace RegressionIssue48
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool _failed = false;

        public Window1()
        {
            InitializeComponent();
        }

        void OnClick(object sender, EventArgs e)
        {
            CreateNewWindowThread();
        }

        Thread CreateNewWindowThread()
        {
            Thread thread = new Thread(this.NewWindow);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return thread;
        }

        // display a window on a new thread
        void NewWindow()
        {
            Window window = null;
            try
            {
                window = new Window();
                Panel panel = new StackPanel();
                Button button = new Button();
                button.Template = (ControlTemplate)Resources["SelectAllButtonTemplate"];

                panel.Children.Add(button);
                window.Content = panel;
                window.Show();
            }
            catch(Exception e)
            {
                GlobalLog.LogDebug(e.ToString());
                _failed = true;
            }
            finally
            {
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        void OnLoad(object sender, EventArgs e)
        {
            Thread thread1 = CreateNewWindowThread();
            Thread thread2 = CreateNewWindowThread();

            thread1.Join();
            thread2.Join();

            Application.Current.Shutdown(_failed ? -1 : 0);
        }
    }
}
