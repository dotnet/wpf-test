// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace RegressionIssue111
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// Verify using StaticExtension within MergedDictionaries works fine
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Test.Logging.TestLog log = TestLog.Current;
            if (button1.Background == Brushes.Red)
            {
                log.Result = TestResult.Pass;
                Application.Current.Shutdown(0);
            }
            else
            {
                log.Result = TestResult.Fail;
                Application.Current.Shutdown(-1);
            }
        }
    }

    public static class SharedResourcesManager
    {
        public static ResourceDictionary Resources
        {
            get
            {
                if (s_resources == null)
                {
                    Uri resourceLocater = new Uri("SharedDictionary.xaml", System.UriKind.Relative);
                    s_resources = (ResourceDictionary)Application.LoadComponent(resourceLocater);
                }

                return s_resources;
            }
        }

        private static ResourceDictionary s_resources;
    }

}
